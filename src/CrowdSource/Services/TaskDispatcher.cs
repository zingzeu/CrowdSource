using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrowdSource.Models.CoreModels;
using Microsoft.Extensions.Logging;
using System.Threading;
using CrowdSource.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;

namespace CrowdSource.Services
{
    /// <summary>
    /// Used to control concurrency in dispatching tasks.
    /// Should be a singleton
    /// </summary>
    public class TaskDispatcher : SingletonWithDbAndConfig, ITaskDispatcher
    {
        // ToDo -> Doing -> ToReview -> Reviewing -> Done
        //                      ^           |         |
        //                      |           |         |
        //                      ----------------------- 

        private Queue<QueueMember> _queueToDo = new Queue<QueueMember>();
        private HashSet<QueueMember> _setDoing = new HashSet<QueueMember>();
        private Queue<QueueMember> _queueToReview = new Queue<QueueMember>();
        private HashSet<QueueMember> _setReviewing = new HashSet<QueueMember>();
        private readonly TimeSpan DoingTimeOut = new TimeSpan(0, 10, 0); // 10 min
        private readonly TimeSpan ReviewTimeout = new TimeSpan(0, 3, 0); // 3 min

        // Lock for thread safety
        // Because ASP.NET MVC will handle requests in multiple threads
        readonly object _locker = new object();
        private readonly ILogger<TaskDispatcher> _logger;
        public TaskDispatcher(ILoggerFactory loggerFactory, IServiceScopeFactory scopeFactory)
            :base(scopeFactory)
        {
            _logger = loggerFactory.CreateLogger<TaskDispatcher>();

            // fire and forget
            // TODO: better?
            LoadToDoFromDB().Wait();

            ScheduleTimers();
        }

        public Group GetNextToDo()
        {
            lock (_locker)
            {
                if (_queueToDo.Count > 0)
                {
                    var todo = _queueToDo.Dequeue();
                    _setDoing.Add(new QueueMember(todo.group));
                    return todo.group;
                } else
                {
                    return null;
                }
            }
        }

        public Group GetNextReview()
        {
            lock (_locker)
            {
                if (_queueToReview.Count > 0)
                {
                    var toreview = _queueToReview.Dequeue();
                    _setReviewing.Add(new QueueMember(toreview.group));
                    return toreview.group;
                }
                else
                {
                    return null;
                }
            }
        }

        private void CleanUpDoing()
        {
            lock (_locker)
            {
                var requeued = new List<QueueMember>();
                foreach (var member in _setDoing)
                {
                    if ((DateTime.Now - member.added) > DoingTimeOut)
                    {
                        _logger.LogInformation($"Doing GroupID {member.group.GroupId}, Timeout: {DateTime.Now - member.added}");
                        // _setDoing.Remove(member);
                        requeued.Add(member);
                        _logger.LogInformation($"Requeuing {member.group.GroupId}");
                        _queueToDo.Enqueue(new QueueMember(member.group));
                    }
                }

                foreach (var i in requeued)
                {
                    _setDoing.Remove(i);
                }
            }
        }

        public void RequeueToDo(Group group)
        {
            lock (_locker)
            {
                var member = _setDoing.SingleOrDefault(t => t.group.GroupId == group.GroupId);
                if (member != null)
                {
                    _setDoing.Remove(member);
                    FlagEnum? flagtype = null;
                    try
                    {
                        RunWithDbContext(_context => {
                            flagtype = _context.Groups.SingleOrDefault(g => g.GroupId == group.GroupId).FlagType;
                        });
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                        _logger.LogError(e.StackTrace);
                    }
                    if (flagtype==null)
                    {
                        _logger.LogInformation($"Manually Requeuing {member.group.GroupId}");
                        _queueToDo.Enqueue(new QueueMember(member.group));
                    }

                }

            }
        }

        private void CleanUpReviewing()
        {
            lock (_locker)
            {
                var requeued = new List<QueueMember>();
                foreach (var member in _setReviewing)
                {
                    if ((DateTime.Now - member.added) > ReviewTimeout)
                    {
                        _logger.LogInformation($"Doing GroupID {member.group.GroupId}, ReviewTimeout: {DateTime.Now - member.added}");
                        //_setReviewing.Remove(member);
                        requeued.Add(member);
                        _logger.LogInformation($"Requeuing {member.group.GroupId}");
                        _queueToReview.Enqueue(new QueueMember(member.group));
                    }
                }

                foreach (var i in requeued)
                {
                    _setReviewing.Remove(i);
                }
            }
        }

        public async Task LoadToDoFromDB()
        {
            _logger.LogInformation("Loading From DB...");
            // Load config from DB
            int minimumReview = 1;
            bool randomize = false;
            bool bucFirst = true;
            RunWithConfigContext(_config => {
                minimumReview = _config.GetMinimumReview();
                randomize = (_config.Get("Randomize") == "true") && !bucFirst;
            });
            
            _logger.LogInformation($"Config: minimumReview={minimumReview} randomize={randomize} bucFirst={bucFirst}");
            List<Group> todo = null;
            List<Group> toreview = null;
            
            await RunWithDbContextAsync(async _context => {
                var query = _context
                    .Groups
                    .FromSql(
                        @"
                        SELECT 
                            distinct ""GroupId"",
                            ""CollectionId"",
                            ""FlagType"",
                            ""GroupMetadata"",
                            ""score""
                        FROM
                        (
                            SELECT 
                                ""Groups"".* ,
                                score_group(""buc"".""GroupId"" IS NOT NULL, 
                                ""eng"".""GroupId"" is not null,
                                ""chi"".""GroupId"" is not null) as ""score""
                            FROM ""Groups""
                            LEFT JOIN count_suggestions('TextBUC') AS ""buc""
                                ON ""buc"".""GroupId"" = ""Groups"".""GroupId""
                            LEFT JOIN count_suggestions('TextEnglish') AS ""eng""
                                ON ""eng"".""GroupId"" = ""Groups"".""GroupId""
                            LEFT JOIN count_suggestions('TextChinese') AS ""chi""
                                ON ""chi"".""GroupId"" = ""Groups"".""GroupId""
                            where ""Groups"".""FlagType"" is null
                            ORDER BY ""score"" DESC
                        ) as foo
                        where foo.""score"" > 0
                        ORDER BY ""score"" DESC"
                    );

                todo = bucFirst ? 
                    await query.ToListAsync()
                    : await query.OrderBy(g => g.GroupId).ToListAsync();

                toreview = await _context
                    .Groups
                    .FromSql("SELECT * FROM \"Groups\" AS \"gg\"" +
                    " WHERE" +
                    "(SELECT COUNT(DISTINCT \"FieldTypes\".\"Name\") FROM" +
                    "  \"GVSuggestions\"" +
                    "   INNER JOIN \"GroupVersions\" ON \"GVSuggestions\".\"GroupVersionForeignKey\" = \"GroupVersions\".\"GroupVersionId\"" +
                    "   INNER JOIN \"FieldTypes\" ON \"GVSuggestions\".\"FieldTypeForeignKey\" = \"FieldTypes\".\"FieldTypeId\"" +
                    " WHERE \"GroupVersions\".\"GroupId\" = \"gg\".\"GroupId\"" +
                    " AND \"FieldTypes\".\"Name\" IN('TextBUC', 'TextEnglish', 'TextChinese')" +
                    " AND \"GroupVersions\".\"NextVersionGroupVersionId\" IS NULL" +
                    ") >= 3 " + //罗 英 中 都有内容
                    "AND  " +
                    "(" +
                    " SELECT COUNT(DISTINCT \"Reviews\".\"Id\") FROM \"Reviews\"" +
                    "   INNER JOIN \"GroupVersions\" ON \"Reviews\".\"GroupVersionId\" = \"GroupVersions\".\"GroupVersionId\"" +
                    "   WHERE \"GroupVersions\".\"GroupId\" = \"gg\".\"GroupId\"" +
                    "   AND \"GroupVersions\".\"NextVersionGroupVersionId\" IS NULL" +
                    ") < {0}" +  // Review 少于 minimumReview 次
                    " AND \"gg\".\"FlagType\" IS NULL",
                    minimumReview
                    )
                    .OrderBy(g => g.GroupId).ToListAsync();
            });

            if (randomize)
            {
                // shuffle
                _logger.LogInformation("Randomizing");
                Shuffle.DoShuffle(todo);
                Shuffle.DoShuffle(toreview);
            }

            lock (_locker)
            {
                _logger.LogInformation("Populating memory collections...");
                _queueToDo.Clear();
                _queueToReview.Clear();
                _setDoing.Clear();
                _setReviewing.Clear();
                for (int i = 0; i < todo.Count; ++i)
                {
                    _queueToDo.Enqueue(new QueueMember(todo[i]));
                }

                for (int i = 0; i < toreview.Count; ++i)
                {
                    _queueToReview.Enqueue(new QueueMember(toreview[i]));
                }
            }
            _logger.LogInformation($"ToDo Queue length: {_queueToDo.Count}");
            _logger.LogInformation($"Review Queue length: {_queueToReview.Count}");
        }

        public void Done(Group group)
        {
            lock (_locker)
            {
                var member = _setDoing.SingleOrDefault(m => m.group.GroupId == group.GroupId);
                if (member != null)
                {
                    _setDoing.Remove(member);
                    _queueToReview.Enqueue(new QueueMember(member.group));
                }
            }
        }

        public void DoneReview(Group group)
        {
            lock (_locker)
            {
                var member = _setReviewing.SingleOrDefault(m => m.group.GroupId == group.GroupId);
                if (member != null)
                {
                    _setReviewing.Remove(member);
                }
            }
        }

        private Timer aTimer;
        private void ScheduleTimers()
        {
            aTimer = new Timer(a => {
                //_logger.LogInformation("Timer");
                CleanUpDoing();
                CleanUpReviewing();
            },null,0,10000);
        }

        public IEnumerable<QueueMember> ListToReview()
        {
            return _queueToReview.OrderBy(t => t.added);
        }

        public IEnumerable<QueueMember> ListToDo()
        {
            return _queueToDo.OrderBy(t => t.added);
        }

        public IEnumerable<QueueMember> ListDoing()
        {
            return _setDoing.ToList().OrderBy(t => t.added);
        }

        public IEnumerable<QueueMember> ListReviewing()
        {
            return _setReviewing.ToList().OrderBy(t => t.added);
        }

        public int CountToDo()
        {
            return _queueToDo.Count;
        }

        public int CountToReview()
        {
            return _queueToReview.Count;
        }

        public void RequeueToReview(Group group)
        {
            lock (_locker)
            {
                var member = _setReviewing.SingleOrDefault(t => t.group.GroupId == group.GroupId);
                if (member != null)
                {
                    _setReviewing.Remove(member);
                    FlagEnum? flagtype = null;
                    try
                    {
                        RunWithDbContext(_context => {
                            flagtype = _context.Groups.SingleOrDefault(g => g.GroupId == group.GroupId).FlagType;
                        });
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                        _logger.LogError(e.StackTrace);
                    }
                    if (flagtype == null)
                    {
                        _logger.LogInformation($"Manually Requeuing {member.group.GroupId}");
                        _queueToReview.Enqueue(new QueueMember(member.group));
                    }
                }
            }
        }

        public async Task ReloadAsync()
        {
            _logger.LogInformation("Triggering reloading from DB....");
            await LoadToDoFromDB();
        }

    }


    public interface ITaskDispatcher
    {
        Group GetNextReview();
        /// <summary>
        /// 取得下一个需要识别的词条。
        /// </summary>
        /// <returns></returns>
        Group GetNextToDo();
        /// <summary>
        /// 标示一个词条识别完毕。
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        void Done(Group group);
        void DoneReview(Group group);
        /// <summary>
        /// 手动重新加入到 ToDo 队列
        /// </summary>
        /// <param name="group"></param>
        void RequeueToDo(Group group);
        /// <summary>
        /// 手动重新加入到 ToReview 队列
        /// </summary>
        /// <param name="group"></param>
        void RequeueToReview(Group group);

        Task ReloadAsync();

        IEnumerable<QueueMember> ListToDo();
        IEnumerable<QueueMember> ListToReview();
        IEnumerable<QueueMember> ListDoing();
        IEnumerable<QueueMember> ListReviewing();
        int CountToDo();
        int CountToReview();
    }

    public class QueueMember
    {
        public Group group { get; set; }
        public DateTime added { get; set; }
        public QueueMember(Group group)
        {
            this.group = group;
            this.added = DateTime.Now;
        }

    }
}

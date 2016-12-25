using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrowdSource.Models.CoreModels;
namespace CrowdSource.Services
{
    /// <summary>
    /// Used to control concurrency in dispatching tasks.
    /// Should be a singleton
    /// </summary>
    public class TaskDispatcher
    {
        // ToDo -> Doing -> ToReview -> Reviewing -> Done
        //                      ^                     |
        //                      |                     |
        //                      ----------------------- 

        private Queue<QueueMember> _queueToDo;
        private HashSet<QueueMember> _setDoing;
        private Queue<QueueMember> _queueToReview;
        private HashSet<QueueMember> _setReviewing;
        private readonly TimeSpan DoingTimeOut = new TimeSpan(0,15,0); // 15min
        private readonly TimeSpan ReviewTimeout = new TimeSpan(0, 5, 0); //5min

        // Lock for thread safety
        // Because ASP.NET MVC will handle requests in multiple threads
        readonly object _locker = new object();

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

        private void CleanUpDoing()
        {
            lock (_locker)
            {
                foreach (var member in _setDoing)
                {
                    if ((member.added - DateTime.Now) > DoingTimeOut)
                    {
                        _setDoing.Remove(member);
                        _queueToDo.Enqueue(new QueueMember(member.group));
                    }
                }
            }
        }

        private void CleanUpReviewing()
        {
            lock (_locker)
            {
                foreach (var member in _setReviewing)
                {
                    if ((member.added - DateTime.Now) > ReviewTimeout)
                    {
                        _setReviewing.Remove(member);
                        _queueToReview.Enqueue(new QueueMember(member.group));
                    }
                }
            }
        }

        private void LoadToDoFromDB()
        {

        }

        public void Done(Group group)
        {
            lock (_locker)
            {
                var member = _setDoing.Where(m => m.group.GroupId == group.GroupId).Single();
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
                var member = _setReviewing.Where(m => m.group.GroupId == group.GroupId).Single();
                if (member != null)
                {
                    _setReviewing.Remove(member);
                 
                }
            }
        }

    }

    class QueueMember
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

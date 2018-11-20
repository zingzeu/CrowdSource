using CrowdSource.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;

namespace CrowdSource.Services
{
    public class Analytics : SingletonWithDbAndConfig, IAnalytics
    {
        private readonly ITaskDispatcher _taskDispacher;
        private readonly Timer aTimer;

        public int ToDoTotal { get; private set; }

        public int Done { get; private set; }
        
        public int DoneBUC { get; private set; }
        public int DoneEnglish { get; private set; }
        public int DoneChinese { get; private set; }

        public int ReviewTotal { get; private set; }

        public int Reviewed { get; private set; }

        public IEnumerable<Tuple<string, int>> TopContributors  {get; private set; }

        public Analytics(ITaskDispatcher taskDispatcher, IServiceScopeFactory scopeFactory)
            :base(scopeFactory)
        {
            _taskDispacher = taskDispatcher;
            TopContributors = new List<Tuple<string,int>>();

            // fire and forget
            // TODO: better?
            UpdateStatistics().Wait();

            aTimer = new Timer(async a => {
               await UpdateStatistics();
            }, null, 0, 5 * 60 * 1000); // 10 min
        }

        public async Task UpdateStatistics()
        {
            await RunWithDbContextAsync(async _context => {
                int total = _context.Groups.Where(g => g.Collection.CollectionId == 1).Count();
                int todo = _taskDispacher.CountToDo();
                int toreview = _taskDispacher.CountToReview();
                int done = await _context
                        .Groups
                        .FromSql("SELECT * FROM \"Groups\" AS \"gg\" \n" +
                        "WHERE\n" +
                        "(SELECT COUNT(DISTINCT \"FieldTypes\".\"Name\") FROM\n" +
                        "  \"GVSuggestions\"\n" +
                        "   INNER JOIN \"GroupVersions\" ON \"GVSuggestions\".\"GroupVersionForeignKey\" = \"GroupVersions\".\"GroupVersionId\"\n" +
                        "   INNER JOIN \"FieldTypes\" ON \"GVSuggestions\".\"FieldTypeForeignKey\" = \"FieldTypes\".\"FieldTypeId\"\n" +
                        " WHERE \"GroupVersions\".\"GroupId\" = \"gg\".\"GroupId\"\n" +
                        " AND \"FieldTypes\".\"Name\" IN('TextBUC', 'TextEnglish', 'TextChinese')\n" +
                        " AND \"GroupVersions\".\"NextVersionGroupVersionId\" IS NULL\n" +
                        ") >= 3\n" //罗 英 中 全
                        )
                        .CountAsync();
                int doneBUC = await _context
                        .Groups
                        .FromSql("SELECT * FROM \"Groups\" AS \"gg\" \n" +
                        "WHERE\n" +
                        "(SELECT COUNT(DISTINCT \"FieldTypes\".\"Name\") FROM\n" +
                        "  \"GVSuggestions\"\n" +
                        "   INNER JOIN \"GroupVersions\" ON \"GVSuggestions\".\"GroupVersionForeignKey\" = \"GroupVersions\".\"GroupVersionId\"\n" +
                        "   INNER JOIN \"FieldTypes\" ON \"GVSuggestions\".\"FieldTypeForeignKey\" = \"FieldTypes\".\"FieldTypeId\"\n" +
                        " WHERE \"GroupVersions\".\"GroupId\" = \"gg\".\"GroupId\"\n" +
                        " AND \"FieldTypes\".\"Name\" IN('TextBUC')\n" +
                        " AND \"GroupVersions\".\"NextVersionGroupVersionId\" IS NULL\n" +
                        ") >= 1\n" //罗
                        )
                        .CountAsync();
                int doneChinese = await _context
                        .Groups
                        .FromSql("SELECT * FROM \"Groups\" AS \"gg\" \n" +
                        "WHERE\n" +
                        "(SELECT COUNT(DISTINCT \"FieldTypes\".\"Name\") FROM\n" +
                        "  \"GVSuggestions\"\n" +
                        "   INNER JOIN \"GroupVersions\" ON \"GVSuggestions\".\"GroupVersionForeignKey\" = \"GroupVersions\".\"GroupVersionId\"\n" +
                        "   INNER JOIN \"FieldTypes\" ON \"GVSuggestions\".\"FieldTypeForeignKey\" = \"FieldTypes\".\"FieldTypeId\"\n" +
                        " WHERE \"GroupVersions\".\"GroupId\" = \"gg\".\"GroupId\"\n" +
                        " AND \"FieldTypes\".\"Name\" IN('TextChinese')\n" +
                        " AND \"GroupVersions\".\"NextVersionGroupVersionId\" IS NULL\n" +
                        ") >= 1\n" //中
                        )
                        .CountAsync();
                int doneEnglish = await _context
                        .Groups
                        .FromSql("SELECT * FROM \"Groups\" AS \"gg\" \n" +
                        "WHERE\n" +
                        "(SELECT COUNT(DISTINCT \"FieldTypes\".\"Name\") FROM\n" +
                        "  \"GVSuggestions\"\n" +
                        "   INNER JOIN \"GroupVersions\" ON \"GVSuggestions\".\"GroupVersionForeignKey\" = \"GroupVersions\".\"GroupVersionId\"\n" +
                        "   INNER JOIN \"FieldTypes\" ON \"GVSuggestions\".\"FieldTypeForeignKey\" = \"FieldTypes\".\"FieldTypeId\"\n" +
                        " WHERE \"GroupVersions\".\"GroupId\" = \"gg\".\"GroupId\"\n" +
                        " AND \"FieldTypes\".\"Name\" IN('TextEnglish')\n" +
                        " AND \"GroupVersions\".\"NextVersionGroupVersionId\" IS NULL\n" +
                        ") >= 1\n" //英
                        )
                        .CountAsync();
                int reviewed = done - toreview;

                done = done > 0 ? done : 0;
                reviewed = reviewed > 0 ? reviewed : 0;
                ToDoTotal = total;
                Done = done;
                DoneBUC = doneBUC;
                DoneChinese = doneChinese;
                DoneEnglish = doneEnglish;
                ReviewTotal = done;
                Reviewed = reviewed;



                // Top Contributors
                ((List<Tuple<string,int>>)TopContributors).Clear();

                var reader = await RDFacadeExtensions.ExecuteSqlCommandAsync(_context.Database,
                "SELECT  \"AspNetUsers\".\"Email\", \"AspNetUsers\".\"NickName\",\"cc\".\"Count\" FROM\n" +
                "(SELECT COUNT(DISTINCT \"SuggestionId\") AS \"Count\", \"AuthorId\" \n" +
                "FROM \"Suggestions\"\n" +
                "GROUP BY \"AuthorId\"\n" +
                ") AS \"cc\" \n" +
                "LEFT OUTER JOIN \"AspNetUsers\" ON \"cc\".\"AuthorId\" = \"AspNetUsers\".\"Id\" \n" +
                "ORDER BY \"Count\" DESC \n" +
                "LIMIT 10 \n" 
                );

                while (reader.DbDataReader.Read()) {
                    var email = reader.DbDataReader[0].ToString();
                    var nickname = reader.DbDataReader[1].ToString();
                    var contrib =  reader.DbDataReader[2].ToString(); 
                    int contribInt = 0;
                    Int32.TryParse(contrib, out contribInt);
                    if (email=="" && nickname=="") {
                        ((List<Tuple<string,int>>)TopContributors).Add(new Tuple<string, int>("匿名者 (众人)",contribInt));
                    } else {
                        email = CensorEmail(email);
                        ((List<Tuple<string,int>>)TopContributors).Add(new Tuple<string, int>($"{nickname} ({email})",contribInt));
                    }

                }

                reader.Dispose();
            });
        }

        private string CensorEmail(string email) {
            var atPos = email.IndexOf('@');
            if (atPos != -1) {
                var username =  email.Substring(0,atPos);
                string masked = Regex.Replace(username, @"(?<=[\w]{3})[\w-\._\+%]*(?=[\w]{2})", m => new string('*', m.Length));
                return masked + "@" + "***.com";
            } else {
                return email;
            }
        }
    }

    public interface IAnalytics
    {
        int ToDoTotal { get; }
        int Done { get; }
        int DoneBUC { get; }
        int DoneEnglish { get; }
        int DoneChinese { get; }
        int ReviewTotal { get; }
        int Reviewed { get;  }

        IEnumerable<Tuple<string, int> > TopContributors {get; }
    }

}

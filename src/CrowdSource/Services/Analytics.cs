using CrowdSource.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CrowdSource.Services
{
    public class Analytics : IAnalytics
    {
        private readonly ITaskDispatcher _taskDispacher;
        private readonly ApplicationDbContext _context;
        private readonly Timer aTimer;

        public int ToDoTotal { get; private set; }

        public int Done { get; private set; }
        
        public int DoneBUC { get; private set; }
        public int DoneEnglish { get; private set; }
        public int DoneChinese { get; private set; }

        public int ReviewTotal { get; private set; }

        public int Reviewed { get; private set; }

        public Analytics(ITaskDispatcher taskDispatcher, ApplicationDbContext context)
        {
            _taskDispacher = taskDispatcher;
            _context = context;

            UpdateStatistics().Wait();

            aTimer = new Timer(async a => {
               await UpdateStatistics();
            },null,0,1*60*1000); //1 min
        }

        private async Task UpdateStatistics()
        {
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
    }
}

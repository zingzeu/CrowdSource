using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CrowdSource.Services;
using CrowdSource.Data;
using Microsoft.EntityFrameworkCore;

namespace CrowdSource.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITaskDispatcher _taskDispacher;
        private readonly ApplicationDbContext _context;
        public HomeController(ITaskDispatcher taskDispatcher, ApplicationDbContext context)
        {
            _taskDispacher = taskDispatcher;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            int total = _context.Groups.Where(g => g.Collection.CollectionId == 1).Count(); 
            int todo = _taskDispacher.CountToDo();
            int toreview = _taskDispacher.CountToReview();
            int done = _context
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
                    .Count();
            int reviewed = done - toreview;
            int percentDone = (int)done * 100 / total;
            int percentReviewd = (int)reviewed * 100 / done;
            done = done > 0 ? done : 0;
            reviewed = reviewed > 0 ? reviewed : 0;
            ViewData["ToDoTotal"] = total;
            ViewData["Done"] = done;
            ViewData["ReviewTotal"] = done;
            ViewData["Reviewed"] = reviewed;
            ViewData["DonePercent"] = percentDone;
            ViewData["ReviewPercent"] = percentReviewd;
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}

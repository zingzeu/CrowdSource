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
        private readonly IAnalytics _analytics;
        public HomeController(IAnalytics analytics)
        {
            _analytics = analytics;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["ToDoTotal"] = _analytics.ToDoTotal;
            ViewData["Done"] = _analytics.Done;
            ViewData["ReviewTotal"] = _analytics.ReviewTotal;
            ViewData["Reviewed"] = _analytics.Reviewed;
            int percentDone = _analytics.ToDoTotal>0 ? (int)_analytics.Done * 100 / _analytics.ToDoTotal : 0;
            int percentReviewd = _analytics.ReviewTotal>0 ? (int)_analytics.Reviewed * 100 / _analytics.ReviewTotal : 0;
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

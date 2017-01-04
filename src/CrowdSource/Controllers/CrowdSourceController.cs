using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CrowdSource.Models.CoreViewModels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CrowdSource.Controllers
{
    public class CrowdSourceController : Controller
    {
        private readonly ILogger<CrowdSourceController> _logger;
        public CrowdSourceController(ILoggerFactory loggerFactory)
        {
             _logger = loggerFactory.CreateLogger<CrowdSourceController>();
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult EditGroup()
        {
            return View(new GroupViewModel()
            {
                GroupId = 2,
                TextBUC = "BUC2",
                TextChinese = "Chi",
                TextEnglish = "Eng"
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitGroup(GroupViewModel data)
        {
            _logger.LogInformation(JsonConvert.SerializeObject(data));
            return RedirectToAction("EditGroup");
        }
    }
}
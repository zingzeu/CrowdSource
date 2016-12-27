using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CrowdSource.Controllers
{
    public class TestController : Controller
    {
        private ILogger<TestController> _logger;
        public TestController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TestController>();
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }

        public IActionResult SubmitToDo(int gid)
        {
            _logger.LogInformation($"{gid} submitted");
            return View("List");
        }

    }

}

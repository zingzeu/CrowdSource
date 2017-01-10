using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CrowdSource.Controllers
{
    public class QueueController : Controller
    {
        private ILogger<QueueController> _logger;
        public QueueController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<QueueController>();
        }


        [Route("Admin/Queue/List")]
        public IActionResult List()
        {
            //TODO: Authentication: Admin only
            return View("List");
        }

        public IActionResult SubmitToDo(int gid)
        {
            _logger.LogInformation($"{gid} submitted");
            return View("List");
        }

    }

}

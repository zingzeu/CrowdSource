using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using CrowdSource.Services;

namespace CrowdSource.Controllers
{
    [Authorize(Roles="Administrator")]
    public class QueueController : Controller
    {
        private ILogger<QueueController> _logger;
        private readonly ITaskDispatcher _taskDispatcher;

        public QueueController(ILoggerFactory loggerFactory, ITaskDispatcher taskDispatcher)
        {
            _logger = loggerFactory.CreateLogger<QueueController>();
            _taskDispatcher = taskDispatcher;
        }

        [Route("Admin/Queue/List")]
        public IActionResult List()
        {
            return View("List");
        }

        [Route("Admin/Queue/Reload")]
        public IActionResult Reload()
        {
            _taskDispatcher.Reload();
            return RedirectToAction("List");
        }

    }

}

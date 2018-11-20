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
        public async Task<IActionResult> List([FromQuery]int? page)
        {
            Pager @pager;
            var todos = _taskDispatcher.ListToDo();
            @pager = new Pager(todos.Count(), page ?? 1, 20);
            ViewData["pager"] = @pager;
            var shownTodos = todos.Skip(@pager.PageSkip).Take(@pager.PageSize).ToList();
            ViewData["ToDo"] = shownTodos;
            ViewData["Doing"] = _taskDispatcher.ListDoing();
            return View("List");
        }


        [Route("Admin/Queue/ListReview")]
        public async Task<IActionResult> ListReview([FromQuery]int? page)
        {
            Pager @pager;
            var todos = _taskDispatcher.ListToReview();
            @pager = new Pager(todos.Count(), page ?? 1, 20);
            ViewData["pager"] = @pager;
            var shownTodos = todos.Skip(@pager.PageSkip).Take(@pager.PageSize).ToList();
            ViewData["ToReview"] = shownTodos;
            ViewData["Reviewing"] = _taskDispatcher.ListReviewing();
            return View("ListReview");
        }

        [Route("Admin/Queue/Reload")]
        public async Task<IActionResult> Reload()
        {
            await _taskDispatcher.ReloadAsync();
            return RedirectToAction("List");
        }

    }

}

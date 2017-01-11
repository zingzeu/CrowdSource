using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CrowdSource.Models.CoreViewModels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CrowdSource.Services;
using Microsoft.EntityFrameworkCore;
using CrowdSource.Data;
using CrowdSource.Models.CoreModels;

namespace CrowdSource.Controllers
{
    public class CrowdSourceController : Controller
    {
        private readonly ILogger<CrowdSourceController> _logger;
        private readonly IDataLogic _logic;
        private readonly ApplicationDbContext _context;
        private readonly ITaskDispatcher _taskDispatcher;
        private readonly ITextSanitizer _textSanitizer;

        public CrowdSourceController(ILoggerFactory loggerFactory, IDataLogic logic, ApplicationDbContext context, ITaskDispatcher taskDispatcher, ITextSanitizer textSanitizer)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger<CrowdSourceController>();
            _logic = logic;
            _taskDispatcher = taskDispatcher;
            _textSanitizer = textSanitizer;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("CrowdSource/EditGroup/{id?}")]
        public async Task<IActionResult> EditGroup(int? id)
        {
            int gid;
            if (id != null)
            {
                gid = id ?? 0;
                ViewData["Admin"] = true;
            } else
            {
                gid = _taskDispatcher.GetNextToDo().GroupId;
                ViewData["Admin"] = false;
            }

            if (!(await _logic.GroupExists(gid))) {
                return View("Error");
            }
            var fields = _logic.GetLastestVersionFields(gid);
            var types = _logic.GetAllFieldTypesByGroup(gid);
            
            return View(new GroupViewModel()
            {
                GroupId = gid,
                TextBUC = fields[types.Single(t => t.Name == "TextBUC")],
                TextChinese = fields[types.Single(t => t.Name == "TextChinese")],
                TextEnglish = fields[types.Single(t => t.Name == "TextEnglish")],
                IsOral = (fields[types.Single(t => t.Name == "IsOral")] == "True"),
                IsLiterary = (fields[types.Single(t => t.Name == "IsLiterary")] == "True"),
                IsPivotRow = (fields[types.Single(t => t.Name == "IsPivotRow")] == "True"),
                BoPoMoFo = fields[types.Single(t => t.Name == "BoPoMoFo")],
                Radical = fields[types.Single(t => t.Name == "Radical")],
                FlagType = _context.Groups.Single(g => g.GroupId == gid).FlagType,
                //Flagged = (_context.Groups.Single(g => g.GroupId == id).FlagType != null)
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitGroup(GroupViewModel data, [FromForm]bool? Admin)
        {

            _logger.LogInformation(JsonConvert.SerializeObject(data));
            var id = data.GroupId;
            var fields = new Dictionary<FieldType, string>();
            var types = _logic.GetAllFieldTypesByGroup(id);

            fields[types.Single(t => t.Name == "TextBUC")] = _textSanitizer.BanJiao(data.TextBUC);
            fields[types.Single(t => t.Name == "TextChinese")] = _textSanitizer.BanJiao(data.TextChinese);
            fields[types.Single(t => t.Name == "TextEnglish")] = _textSanitizer.BanJiao(data.TextEnglish);
            fields[types.Single(t => t.Name == "IsOral")] = data.IsOral.ToString();
            fields[types.Single(t => t.Name == "IsLiterary")] = data.IsLiterary.ToString();
            fields[types.Single(t => t.Name == "IsPivotRow")] = data.IsPivotRow.ToString();
            fields[types.Single(t => t.Name == "BoPoMoFo")] = _textSanitizer.BanJiao(data.BoPoMoFo);
            fields[types.Single(t => t.Name == "Radical")] = _textSanitizer.BanJiao(data.Radical);


            _logic.GroupNewSuggestion(id, fields);
            if (Admin ?? false) {
                return RedirectToAction("EditGroup", new { id = id });
            } else
            {
                if (fields[types.Single(t => t.Name == "TextBUC")] != null && 
                    fields[types.Single(t => t.Name == "TextChinese")] != null && 
                    fields[types.Single(t => t.Name == "TextEnglish")] != null)
                {
                    _taskDispatcher.Done(new Group { GroupId = data.GroupId });
                } else
                {
                    _taskDispatcher.RequeueToDo(new Group { GroupId = data.GroupId });
                }
            }


            return RedirectToAction("EditGroup");
        }

        [HttpGet] 
        public IActionResult ReportError(int groupId)
        {
            _logger.LogCritical($"Error Reported. GroupId = {groupId}");

            var group = _context.Groups.SingleOrDefault(g => g.GroupId == groupId);

            group.FlagType = FlagEnum.SegmentationError;
            _context.SaveChanges();

            return RedirectToAction("EditGroup");
        }
    }
}
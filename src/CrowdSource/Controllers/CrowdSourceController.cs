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

        public CrowdSourceController(ILoggerFactory loggerFactory, IDataLogic logic, ApplicationDbContext context)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger<CrowdSourceController>();
            _logic = logic;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult EditGroup()
        {
            var id = 2;
            var fields = _logic.GetLastestVersionFields(id);
            var types = _logic.GetAllFieldTypesByGroup(id);

            return View(new GroupViewModel()
            {
                GroupId = id,
                TextBUC = fields[types.Single(t => t.Name == "TextBUC")],
                TextChinese = fields[types.Single(t => t.Name == "TextChinese")],
                TextEnglish = fields[types.Single(t => t.Name == "TextEnglish")],
                IsOral = (fields[types.Single(t => t.Name == "IsOral")] == "True"),
                IsLiterary = (fields[types.Single(t => t.Name == "IsLiterary")] == "True"),
                IsPivotRow = (fields[types.Single(t => t.Name == "IsPivotRow")] == "True"),
                BoPoMoFo = fields[types.Single(t => t.Name == "BoPoMoFo")],
                Radical = fields[types.Single(t => t.Name == "Radical")]
                //FlagType = _context.Groups.Single(g => g.GroupId == id).FlagType,
                //Flagged = (_context.Groups.Single(g => g.GroupId == id).FlagType != null)
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitGroup(GroupViewModel data)
        {

            _logger.LogInformation(JsonConvert.SerializeObject(data));
            var id = data.GroupId;
            var fields = new Dictionary<FieldType, string>();
            var types = _logic.GetAllFieldTypesByGroup(id);

            fields[types.Single(t => t.Name == "TextBUC")] = data.TextBUC;
            fields[types.Single(t => t.Name == "TextChinese")] = data.TextChinese;
            fields[types.Single(t => t.Name == "TextEnglish")] = data.TextEnglish;
            fields[types.Single(t => t.Name == "IsOral")] = data.IsOral.ToString();
            fields[types.Single(t => t.Name == "IsLiterary")] = data.IsLiterary.ToString();
            fields[types.Single(t => t.Name == "IsPivotRow")] = data.IsPivotRow.ToString();
            fields[types.Single(t => t.Name == "BoPoMoFo")] = data.BoPoMoFo;
            fields[types.Single(t => t.Name == "Radical")] = data.Radical;


            _logic.GroupNewSuggestion(id, fields);
            return RedirectToAction("EditGroup");
        }

        [HttpGet] 
        public IActionResult ReportError(int GroupId)
        {
            //TODO: Add Validation
            _logger.LogCritical($"Error Reported. GroupId = {GroupId}");
            return RedirectToAction("EditGroup");
        }

    }
}
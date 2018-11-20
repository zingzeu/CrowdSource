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
using Microsoft.AspNetCore.Identity;
using CrowdSource.Models;
using Microsoft.AspNetCore.Http;

namespace CrowdSource.Controllers
{
    public class CrowdSourceController : Controller
    {
        private readonly ILogger<CrowdSourceController> _logger;
        private readonly IDataLogic _logic;
        private readonly ApplicationDbContext _context;
        private readonly ITaskDispatcher _taskDispatcher;
        private readonly ITextSanitizer _textSanitizer;
        private readonly IDbConfig _config;
        private readonly UserManager<ApplicationUser> _userMan;
        private readonly IHttpContextAccessor _httpContext;

        public CrowdSourceController(
            ILoggerFactory loggerFactory,
            IDataLogic logic,
            ApplicationDbContext context,
            ITaskDispatcher taskDispatcher,
            ITextSanitizer textSanitizer,
            IDbConfig config,
            UserManager<ApplicationUser> userMan,
            IHttpContextAccessor httpContext
            )
        {
            _context = context;
            _logger = loggerFactory.CreateLogger<CrowdSourceController>();
            _logic = logic;
            _taskDispatcher = taskDispatcher;
            _textSanitizer = textSanitizer;
            _config = config;
            _httpContext = httpContext;
            _userMan = userMan;
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
                var t = _taskDispatcher.GetNextToDo()?.GroupId;
                if (t == null)
                {
                    // no more tasks
                    return View("NoTasks");
                }
                gid = (int)t;
                ViewData["Admin"] = false;
            }

            if (!(await _logic.GroupExists(gid))) {
                return View("Error");
            }
            var fields = _logic.GetLastestVersionFields(gid);
            var types = _logic.GetAllFieldTypesByGroup(gid);
            
            //legacy view
            if (id != null) return View("LegacyEditGroup", await FieldsToGroupViewModel(gid,fields,types));
            return View(await FieldsToGroupViewModel(gid,fields,types));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitGroup(GroupViewModel data, [FromForm]bool? Admin, [FromForm]bool? Review)
        {
            var currentUser = await _userMan.GetUserAsync(_httpContext.HttpContext.User);

            _logger.LogInformation($"User {currentUser?.Email ?? "NotLoggedIn"}");
            _logger.LogInformation(JsonConvert.SerializeObject(data));

            var types = _logic.GetAllFieldTypesByGroup(data.GroupId);
            var fields = GroupViewModelToFields(data, types);
            _logic.GroupNewSuggestion(data.GroupId, fields, currentUser);

            if (Admin ?? false) {
                return RedirectToAction("EditGroup", new { id = data.GroupId });
            } else if (Review ?? false)
            {
                return RedirectToAction("ReviewGroup");
            } else
            {
                if (fields[types.Single(t => t.Name == "TextBUC")] != null && 
                    fields[types.Single(t => t.Name == "TextChinese")] != null && 
                    fields[types.Single(t => t.Name == "TextEnglish")] != null)
                {
                    _logger.LogDebug($"Done : Group {data.GroupId}");
                    _taskDispatcher.Done(new Group { GroupId = data.GroupId });
                } else
                {
                    _logger.LogDebug($"Still has empty fields, requeuing: Group {data.GroupId}");
                    _taskDispatcher.RequeueToDo(new Group { GroupId = data.GroupId });
                }
            }

            return RedirectToAction("EditGroup");
        }

        [Route("CrowdSource/ReviewGroup")]
        public async Task<IActionResult> ReviewGroup()
        {
            var t = _taskDispatcher.GetNextReview()?.GroupId;
            if (t == null)
            {
                // no more tasks
                return View("NoTasks");
            }
            var gid = (int)t;
            ViewData["Admin"] = false;
            ViewData["Review"] = true;

            if (!(await _logic.GroupExists(gid)))
            {
                return View("Error");
            }
            var fields = _logic.GetLastestVersionFields(gid);
            var types = _logic.GetAllFieldTypesByGroup(gid);

            return View("LegacyEditGroup",await FieldsToGroupViewModel(gid, fields, types));
        }

        [HttpGet]
        public async Task<IActionResult> ReviewGroupSubmit(int groupId)
        {
            var currentUser = await _userMan.GetUserAsync(_httpContext.HttpContext.User);

            _logger.LogInformation($"Review Submitted. GroupId = {groupId}");
     
            _logic.ReviewGroup(groupId,currentUser);

            // find review numbers
            var group = _context.Groups
                .Include(g => g.Versions)
                    .ThenInclude(v => v.UserReviews)
                .Where(g => g.GroupId == groupId).SingleOrDefault();
            var reviewedCount = group?.Versions.FirstOrDefault()?.UserReviews.Count() ?? 0;

            // load config for minimal review
            int minimumReview = 2; //Ĭ��ֵ2
            string minimumReviewFromConfig = _config.Get("ReviewThreshold");

            if (minimumReviewFromConfig != null)
            {
                int.TryParse(minimumReviewFromConfig, out minimumReview);
            }

            _logger.LogDebug($"Group {groupId} has {reviewedCount} reviews for last version.");
            if (reviewedCount >= minimumReview)
            {
                _logger.LogDebug($"Done Review: Group {groupId}");
                _taskDispatcher.DoneReview(new Group { GroupId = groupId });
            } else
            {
                _logger.LogDebug($"Requeued to Review: Group {groupId}");
                _taskDispatcher.RequeueToReview(new Group { GroupId = groupId });
            }
            

            return RedirectToAction("ReviewGroup");
        }

        [HttpPost]
        [ValidateAntiForgeryTokenAttribute] 
        public IActionResult ReportError([FromForm]int GroupId)
        {
            _logger.LogCritical($"Error Reported. GroupId = {GroupId}");

            var group = _context.Groups.SingleOrDefault(g => g.GroupId == GroupId);

            group.FlagType = FlagEnum.SegmentationError;
            _context.SaveChanges();

            return RedirectToAction("EditGroup");
        }

        private Dictionary<FieldType,string> GroupViewModelToFields(GroupViewModel data, IEnumerable<FieldType> types)
        {
            var id = data.GroupId;
            var fields = new Dictionary<FieldType, string>();

            fields[types.Single(t => t.Name == "TextBUC")] = _textSanitizer.BanJiao(data.TextBUC?.Trim());
            fields[types.Single(t => t.Name == "TextChinese")] = _textSanitizer.BanJiao(data.TextChinese?.Trim());
            fields[types.Single(t => t.Name == "TextEnglish")] = _textSanitizer.BanJiao(data.TextEnglish?.Trim());
            fields[types.Single(t => t.Name == "IsOral")] = data.IsOral.ToString();
            fields[types.Single(t => t.Name == "IsLiterary")] = data.IsLiterary.ToString();
            fields[types.Single(t => t.Name == "IsPivotRow")] = data.IsPivotRow.ToString();
            fields[types.Single(t => t.Name == "BoPoMoFo")] = _textSanitizer.BanJiao(data.BoPoMoFo?.Trim());
            fields[types.Single(t => t.Name == "Radical")] = _textSanitizer.BanJiao(data.Radical?.Trim());

            return fields;
        }

        private async Task<GroupViewModel> FieldsToGroupViewModel(int gid, Dictionary<FieldType, string> fields, IEnumerable<FieldType>types)
        {
            return new GroupViewModel()
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
                FlagType = _context.Groups.Single(g => g.GroupId == gid).FlagType
            };
        }


        // use this to redirect the user back to EditGroup (Skip Group)
        // avoid using location.reload on client side (causes cache to reload)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RedirectToEditGroup() {
            return RedirectToAction("EditGroup");
        }
    }
}
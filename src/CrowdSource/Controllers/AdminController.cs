using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CrowdSource.Services;
using CrowdSource.Data;
using CrowdSource.Models.CoreModels;
using CrowdSource.Models.CoreViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CrowdSource.Controllers
{
    [Authorize(Roles="Administrator")]
    public class AdminController : Controller
    {
        private readonly IDataLogic _logic;
        private readonly ApplicationDbContext _context;
        private readonly IDbConfig _config;

        public AdminController(ILoggerFactory loggerFactory, IDataLogic logic, ApplicationDbContext context, IDbConfig config)
        {
            _logic = logic;
            _context = context;
            _config = config;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("Admin/Collection/{id}/Group")]
        public async Task<IActionResult> ListGroupsInOneCollection(int id, [FromQuery]string SearchFileName, [FromQuery]bool? ShowFlagged, [FromQuery]int? page)
        {

            ViewData["CollectionId"] = id;
            ViewData["SearchFileName"] = SearchFileName;
            ViewData["ShowFlagged"] = ShowFlagged ?? false;
            var groups = new List<Group>();
            IQueryable<Group> @query;
            Pager @pager; 
            if (ShowFlagged ?? false)
            {
                @query = _context.Groups.Where(g => g.Collection.CollectionId == id && g.FlagType != null);
            }
            else if (SearchFileName == null || SearchFileName == "")
            {
                @query = _context.Groups.Where(g => g.Collection.CollectionId == id);
            }
            else
            {
                @query = _context
                    .Groups
                    .FromSql("SELECT * FROM \"Groups\" WHERE \"GroupMetadata\"->>'ImgFileName' LIKE {0}", "%" + SearchFileName + "%")
                    .Where(g => g.Collection.CollectionId == id);       
            }
            var count = await @query.CountAsync();
            @pager = new Pager(count, page ?? 1, 20);
            ViewData["pager"] = @pager;
            groups = await query.OrderBy(g => g.GroupId).Skip(@pager.PageSkip).Take(@pager.PageSize).ToListAsync();
            return View("ListGroup", groups);
        }

        [Route("Admin/Group/{id}")]
        public IActionResult GroupDetails(int id)
        {
            //TODO: Authentication: Admin Only
            var versions = _logic.GetAllVersions(id);
            var versionsWithFields = new List<Dictionary<FieldType, string>>();
            foreach (var version in versions)
            {
                versionsWithFields.Add(_logic.GetVersionFields(version));
            }

            var vm = new GroupDetailsViewModel()
            {
                Group = _context.Groups.Single(g => g.GroupId == id),
                Versions = versionsWithFields,
                RawVersions = versions.ToList(),
                FieldTypes = _logic.GetAllFieldTypesByGroup(id)
            };

            return View("GroupDetails", vm);
        }

        // GET: Groups/Delete/5
        [Route("Admin/Group/Delete/{id}")]
        public async Task<IActionResult> GroupDelete(int id)
        {
            var @group = await _context.Groups
                .Include(g=> g.Collection)
                .SingleOrDefaultAsync(m => m.GroupId == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View("GroupDeleteConfirm",@group);
        }

        // POST: Groups/Delete/5
        [HttpPost]
        [Route("Admin/Group/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GroupDeleteConfirmed(int id)
        {
            var @group = await _context.Groups
                .Include(g=>g.Collection)
                .SingleOrDefaultAsync(m => m.GroupId == id);
            var collectionId = @group.Collection.CollectionId;
            _context.Groups.Remove(@group);
            await _context.SaveChangesAsync();
            return RedirectToAction("ListGroupsInOneCollection",new { id = collectionId });
        }


        [Route("Admin/Group/UnsetError/{id}")]
        public async Task<IActionResult> UnsetGroupError(int id)
        {
            //TODO: auth admin
            var @group = await _context.Groups
                .SingleOrDefaultAsync(g => g.GroupId == id);
            @group.FlagType = null;
            await _context.SaveChangesAsync();
            return RedirectToAction("EditGroup", "CrowdSource", new { id = id });
        }


        [Route("Admin/Options")]
        public async Task<IActionResult> Options()
        {
            ViewData["ReviewEnabled"] = _config.Get("ReviewEnabled");
            ViewData["ReviewThreshold"] = _config.Get("ReviewThreshold");
            return View();
        }

        [HttpPost]
        [Route("Admin/Options")]
        public async Task<IActionResult> OptionsSubmit([FromForm] string ReviewEnabled, [FromForm] int ReviewThreshold)
        {
            _config.Set("ReviewEnabled", ReviewEnabled);
            _config.Set("ReviewThreshold", ReviewThreshold.ToString());
            return RedirectToAction("Options");
        }

    }
}


public class Pager
{
    public int PageIndex { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }

    public Pager(int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        PageSize = pageSize;
    }

    public bool HasPreviousPage
    {
        get
        {
            return (PageIndex > 1);
        }
    }

    public bool HasNextPage
    {
        get
        {
            return (PageIndex < TotalPages);
        }
    }
    public int PageSkip
    {
        get
        {
            return (PageIndex - 1) * PageSize;
        }
    }

}
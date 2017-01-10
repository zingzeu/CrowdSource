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

namespace CrowdSource.Controllers
{
    public class AdminController : Controller
    {
        private readonly IDataLogic _logic;
        private readonly ApplicationDbContext _context;

        public AdminController(ILoggerFactory loggerFactory, IDataLogic logic, ApplicationDbContext context)
        {
            _logic = logic;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("Admin/Collection/{id}/Group")]
        public async Task<IActionResult> ListGroupsInOneCollection(int id, [FromQuery]string SearchFileName, [FromQuery]bool? ShowFlagged)
        {
            //TODO: Authentication: Admin Only
            //TODO: ADD PAGER
            //_logic.GetAllGroupViewModels();
            var groups = new List<Group>();
            if (ShowFlagged ?? false)
            {
                groups = await _context.Groups.Where(g => g.Collection.CollectionId == id && g.FlagType != null).ToListAsync();
            }
            else if (SearchFileName == null || SearchFileName == "")
            {
                groups = await _context.Groups.Where(g => g.Collection.CollectionId == id).ToListAsync();
            }
            else
            {
                groups = await _context
                    .Groups
                    .FromSql("SELECT * FROM \"Groups\" WHERE \"GroupMetadata\"->>'ImgFileName' LIKE {0}", "%" + SearchFileName + "%")
                    .Where(g => g.Collection.CollectionId == id)
                    .ToListAsync();
            }

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


    }
}

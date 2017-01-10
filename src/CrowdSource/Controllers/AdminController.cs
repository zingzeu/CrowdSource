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
        public IActionResult ListGroupsInOneCollection(int id)
        {
            //TODO: Authentication: Admin Only
            //_logic.GetAllGroupViewModels();
            List<Group> groups = _context.Groups.Where(g => g.Collection.CollectionId == id).ToList();

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


    }
}

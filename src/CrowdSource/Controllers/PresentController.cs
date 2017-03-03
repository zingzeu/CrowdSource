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
    public class PresentController : Controller
    {
        private readonly IDataLogic _logic;
        private readonly ApplicationDbContext _context;
        private readonly IDbConfig _config;

        public PresentController(ILoggerFactory loggerFactory, IDataLogic logic, ApplicationDbContext context, IDbConfig config)
        {
            _logic = logic;
            _context = context;
            _config = config;
        }


        [Route("Present/Collection/{id}")]
        public async Task<IActionResult> PresentGroupsInOneCollection([FromQuery]int? page,int id = 1)
        {

            ViewData["CollectionId"] = id;

            var types =  _logic.GetAllFieldTypes(id);

            var groups = new List<Group>();
            IQueryable<Group> @query;
            Pager @pager; 

            var minimumReview = _config.GetMinimumReview();

            @query = _context.Groups
                    .FromSql("SELECT * FROM \"Groups\" AS \"gg\"" +
                    "WHERE" +
                    "(SELECT COUNT(DISTINCT \"FieldTypes\".\"Name\") FROM" +
                    "  \"GVSuggestions\"" +
                    "   INNER JOIN \"GroupVersions\" ON \"GVSuggestions\".\"GroupVersionForeignKey\" = \"GroupVersions\".\"GroupVersionId\"" +
                    "   INNER JOIN \"FieldTypes\" ON \"GVSuggestions\".\"FieldTypeForeignKey\" = \"FieldTypes\".\"FieldTypeId\"" +
                    " WHERE \"GroupVersions\".\"GroupId\" = \"gg\".\"GroupId\"" +
                    " AND \"FieldTypes\".\"Name\" IN('TextBUC', 'TextEnglish', 'TextChinese')" +
                    " AND \"GroupVersions\".\"NextVersionGroupVersionId\" IS NULL" +
                    ") >= 3" + //罗 英 中 都有内容
                    "AND" +
                    "(" +
                    " SELECT COUNT(DISTINCT \"Reviews\".\"Id\") FROM \"Reviews\"" +
                    "   INNER JOIN \"GroupVersions\" ON \"Reviews\".\"GroupVersionId\" = \"GroupVersions\".\"GroupVersionId\"" +
                    "   WHERE \"GroupVersions\".\"GroupId\" = \"gg\".\"GroupId\"" +
                    "   AND \"GroupVersions\".\"NextVersionGroupVersionId\" IS NULL" +
                    ") >= {0}" +  // Review 不少于 minimumReview 次
                    " AND \"gg\".\"FlagType\" IS NULL",
                    minimumReview
                    )
                    .Where(g => g.Collection.CollectionId == id && g.FlagType == null);

            var count = await @query.CountAsync();
            @pager = new Pager(count, page ?? 1, 20);
            ViewData["pager"] = @pager;
            groups = await query.OrderBy(g => g.GroupId).Skip(@pager.PageSkip).Take(@pager.PageSize).ToListAsync();
            List<Dictionary<string,string>> groupsWithFields = new List<Dictionary<string,string>>(); 
            for (int i = 0; i< groups.Count; ++i) {
                var version = _logic.GetLastestVersion(groups[i].GroupId);
                var versionFields = _logic.GetVersionFields(version);
                var groupWithFields = new Dictionary<string,string> () {
                    {"TextBUC", versionFields[types.Single(t => t.Name == "TextBUC")]},
                    {"TextChinese", versionFields[types.Single(t => t.Name == "TextChinese")]},
                    {"TextEnglish", versionFields[types.Single(t => t.Name == "TextEnglish")]},
                };
                groupsWithFields.Add(groupWithFields);
            }
            ViewData["GroupsWithFields"] = groupsWithFields;
            return View("PresentGroup", groups);
        }

    }
}


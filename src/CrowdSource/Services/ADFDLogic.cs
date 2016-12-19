using CrowdSource.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrowdSource.Models.CoreModels;

namespace CrowdSource.Services
{
    public class ADFDLogic : IDataLogic
    {
        private ApplicationDbContext context;
        public ADFDLogic(ApplicationDbContext _context)
        {
            this.context = _context;
        }

        public IEnumerable<Dictionary<FieldType, string>> GetAllVersions(Group group)
        {
            
            throw new NotImplementedException();
        }
        /// <summary>
        /// 获取一个修订版本的所有字段。
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public Dictionary<FieldType, string> GetVersionFields(GroupVersion version)
        {
            // Get All GVRefersSuggestions for a version
            List<GroupVersionRefersSuggestion> references = context.GVSuggestions
                .Where(i => i.GroupVersion.GroupVersionId == version.GroupVersionId)
                .ToList();

            Dictionary<FieldType, string> fields = new Dictionary<FieldType, string>();
            foreach (var item in references)
            {
                var suggestion = context.Entry(item)
                     .Reference(i => i.Suggestion).Query().Single();
                var type = context.Entry(suggestion).Reference(i => i.Field).Query().Single().FieldType;
                fields.Add(type, suggestion.Content);
            }
            return fields;

        }
        public Dictionary<FieldType, string> GetLastestVersion(Group group)
        {
            context.Entry(group);
            var version = context.GroupVersions
                .Single(i => i.Group.GroupId == group.GroupId && i.NextVersion == null);
            
            throw new NotImplementedException();
        }

        public IEnumerable<Field> GetOriginalFields(int groupId)
        {
            var g = context.Groups.Single(b => b.GroupId == groupId);

            var list = context.Entry(g)
                .Collection( b => b.Fields)
                .Query()
                .ToList();
            return list;
        }

        public void GroupNewSuggestion(Group group, Dictionary<FieldType, string> fields)
        {
            throw new NotImplementedException();
        }

        public void ReviewGroup(Group group)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Field> GetOriginalFields(Group group)
        {
            throw new NotImplementedException();
        }
    }
}


using CrowdSource.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrowdSource.Models.CoreModels;
using CrowdSource.Models;

namespace CrowdSource.Services
{
    /// <summary>
    /// Database logic for ADFD.
    /// </summary>
    public class ADFDLogic : IDataLogic
    {
        private ApplicationDbContext context;
        public ADFDLogic(ApplicationDbContext _context)
        {
            this.context = _context;
        }

        /// <summary>
        /// 获取一个词条的全部版本.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public IEnumerable<Dictionary<FieldType, string>> GetAllVersions(Group group)
        {
            var versions = context.GroupVersions
                .Where(i => i.Group.GroupId == group.GroupId)
                .ToList();

            List<Dictionary<FieldType, string>> result = new List<Dictionary<FieldType, string>>();

            foreach (var item in versions)
            {
                result.Add(GetVersionFields(item));
            }
            return result;
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
            return GetVersionFields(version);
        }

        public IEnumerable<Field> GetOriginalFields(int groupId)
        {
            var g = context.Groups.Single(b => b.GroupId == groupId);

            var list = context.Entry(g)
                .Collection(b => b.Fields)
                .Query()
                .ToList();
            return list;
        }

        /// <summary>
        /// 添加一条新Suggestion (用户提交)
        /// </summary>
        /// <param name="group"></param>
        /// <param name="fields"></param>
        public void GroupNewSuggestion(Group group, Dictionary<FieldType, string> fields)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var latestVersion = context.GroupVersions
                        .Single(i => i.Group.GroupId == group.GroupId && i.NextVersion == null);

                    // for each field type
                    // check if the existing suggestion is the same
                    // add a new Suggestion if changed
                    // add GroupVersionRefersSuggestion for each field type
                    var newVersion = new GroupVersion();

                    foreach (KeyValuePair<FieldType, string> entry in fields)
                    {
                        var fieldType = entry.Key;
                        var newValue = entry.Value;
                        var newReference = new GroupVersionRefersSuggestion();
                        newReference.GroupVersion = newVersion;
                        var oldSuggestion = latestVersion?.FieldSuggestions.Single(i => i.Suggestion.Field.FieldType.FieldTypeId == fieldType.FieldTypeId).Suggestion;
                        if (oldSuggestion.Content != newValue)
                        {
                            var newSuggestion = new Suggestion();
                            newSuggestion.Content = newValue;
                            newSuggestion.Field = null;//;
                            newReference.Suggestion = newSuggestion;
                        }
                        else
                        {
                            newReference.Suggestion = oldSuggestion;
                        }

                        newVersion.FieldSuggestions.Add(newReference);
                    }

                    if (latestVersion!=null)
                        latestVersion.NextVersion = newVersion;
                    context.GroupVersions.Add(newVersion);
                    context.SaveChanges();
                    transaction.Commit();
                    
                } 
                catch (Exception)
                {
                    Console.WriteLine("Error");
                    transaction.Rollback();
                }
            }
     
        }

        /// <summary>
        /// User Endorse a version
        /// </summary>
        /// <param name="group"></param>
        public void ReviewGroup(GroupVersion groupVesion, ApplicationUser user)
        {
            var review = new ApplicationUserEndorsesGroupVersion();
            review.User = user;
            review.GroupVersion = groupVesion;
            context.Reviews.Add(review);
            context.SaveChanges();
        }

    }
}


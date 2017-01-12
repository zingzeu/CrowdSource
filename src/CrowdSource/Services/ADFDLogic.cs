using CrowdSource.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrowdSource.Models.CoreModels;
using CrowdSource.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

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

        public IEnumerable<FieldType> GetAllFieldTypes(int CollectionId)
        {
            //TODO resolve reference
            return context.FieldTypes
                .Include(f=>f.Collection)
                .Where(t => t.Collection.CollectionId == CollectionId)
                .ToList();
        }

        public IEnumerable<FieldType> GetAllFieldTypesByGroup(int groupId)
        {
            return GetAllFieldTypes(
                context
                .Groups
                .Include(g=>g.Collection)
                .Single(g => g.GroupId == groupId)
                .Collection
                .CollectionId);
        }

        /// <summary>
        /// 获取一个词条的全部版本。
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public IEnumerable<GroupVersion> GetAllVersions(int groupId)
        {
            var versions = new List<GroupVersion>();
            try
            {
                versions = context.GroupVersions
                           .Include(v => v.Group)
                               .ThenInclude(g => g.Collection)
                            .Include(v => v.NextVersion)
                            .Include(v => v.UserReviews)
                           .Where(v => v.Group.GroupId == groupId)
                           .OrderBy(v => v.GroupVersionId)
                           .ToList();
            } catch(Exception e)
            {
                Console.WriteLine("ERROR in GetALLVERSIONs");
            }
                
            return versions;
        }

        /// <summary>
        /// 获取一个修订版本的所有字段。
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public Dictionary<FieldType, string> GetVersionFields(GroupVersion version)
        {
            // Get All FieldTypes
            var fieldTypes = GetAllFieldTypes(version.Group.Collection.CollectionId);

            Dictionary<FieldType, string> fields = new Dictionary<FieldType, string>();

            foreach (var type in fieldTypes)
            {
                fields[type] = null;
            }

            if (version == null)
            {
                return fields;
            }

            // Get All GVRefersSuggestions for a version
            List<GroupVersionRefersSuggestion> references = new List<GroupVersionRefersSuggestion>();
            
            if (context.GVSuggestions.Count()>0){
                references = context.GVSuggestions.Include(i=> i.GroupVersion)
                .Where(i => i.GroupVersion.GroupVersionId == version.GroupVersionId)
                .ToList();
            }


            foreach (var item in references)
            {
                var suggestion = context.Entry(item)
                     .Reference(i => i.Suggestion).Query().Single();
                fields[item.FieldType] = suggestion.Content;
            }
            
            return fields;

        }

        public GroupVersion GetLastestVersion(int groupId)
        {
            var version = context.GroupVersions
                .Include(v => v.Group)
                    .ThenInclude(g=> g.Collection)
                .Include(v => v.NextVersion)
                .SingleOrDefault(i => i.Group.GroupId == groupId && i.NextVersion == null);
            return version;
        }

        public Dictionary<FieldType, string> GetLastestVersionFields(int groupId)
        {
            var version = GetLastestVersion(groupId);
            if (version != null)
            {
                return GetVersionFields(version);
            }
            else
            {
                var types = GetAllFieldTypesByGroup(groupId);
                var fields = new Dictionary<FieldType, string>();
                foreach (var t in types)
                {
                    fields.Add(t, null);
                }
                return fields;
            }
        }


        /// <summary>
        /// 添加一条新Suggestion (用户提交)
        /// </summary>
        /// <param name="group"></param>
        /// <param name="fields"></param>
        public void GroupNewSuggestion(int groupId, Dictionary<FieldType, string> newFields)
        {
            var fieldTypes = GetAllFieldTypesByGroup(groupId);
            var oldFields = GetLastestVersionFields(groupId);
            var latestVersion = GetLastestVersion(groupId);


            // for each field type
            // check if the existing suggestion is the same
            // add a new Suggestion if changed
            // add GroupVersionRefersSuggestion for each field type
            var newVersion = new GroupVersion()
            {
                Group = context.Groups.Single(g => g.GroupId == groupId),
                NextVersion = null
                // CREATED TIME.
            };
            foreach (var type in fieldTypes)
            {
                var newReference = new GroupVersionRefersSuggestion()
                {
                    FieldType = type,
                    GroupVersion = newVersion
                };
                if (newFields[type] != null)
                {
                    if ((latestVersion == null) || (oldFields[type] == null) || (oldFields[type] != null && oldFields[type] != newFields[type]))
                    {
                        var newSuggestion = new Suggestion()
                        {
                            Content = newFields[type]
                            // Author = CURRENT USER,
                            // CREATED = time 
                        };
                        newReference.Suggestion = newSuggestion;
                        context.Suggestions.Add(newSuggestion);
                    } else
                    {
                        var oldSuggestion = context.Entry(latestVersion)
                                        .Collection(v => v.FieldSuggestions)
                                        .Query()
                                        .Include(f => f.Suggestion)
                                        .Single(f => f.FieldType == type).Suggestion;

                        newReference.Suggestion = oldSuggestion;
                    }
                    context.GVSuggestions.Add(newReference);
                }
                else
                {
                    //Don't add suggestion
                    // do nothing
                }
                
            }
 
            if (latestVersion!=null)
                latestVersion.NextVersion = newVersion;
            context.GroupVersions.Add(newVersion);

            context.SaveChanges();
        }

        /// <summary>
        /// User Endorse a version
        /// </summary>
        /// <param name="group"></param>
        public void ReviewGroup(GroupVersion groupVesion, ApplicationUser user)
        {
            var review = new ApplicationUserEndorsesGroupVersion()
            {
                User = user,
                GroupVersion = groupVesion
            };
            context.Reviews.Add(review);
            context.SaveChanges();
        }

        public async Task<bool> GroupExists(int groupId)
        {
            return await context.Groups.AnyAsync(g => g.GroupId == groupId);
        }

        public Dictionary<string,string> GetGroupMetadata(int groupId)
        {
            var group = context.Groups.Single(g => g.GroupId == groupId);
            return JsonConvert.DeserializeObject<Dictionary<string,string>>(group.GroupMetadata);
        }
    }
}


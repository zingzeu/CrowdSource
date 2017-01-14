using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrowdSource.Models.CoreModels;
using CrowdSource.Models;

namespace CrowdSource.Services
{
    public interface IDataLogic
    {
        Dictionary<FieldType, string> GetLastestVersionFields(int groupId);

        IEnumerable<GroupVersion> GetAllVersions(int groupId);
        GroupVersion GetLastestVersion(int groupId);

        Dictionary<FieldType, string> GetVersionFields(GroupVersion version);

        IEnumerable<FieldType> GetAllFieldTypes(int CollectionId);

        IEnumerable<FieldType> GetAllFieldTypesByGroup(int groupId);

        void GroupNewSuggestion(int groupId, Dictionary<FieldType, string> newFields, ApplicationUser user = null);

        void ReviewGroup(int groupId, ApplicationUser user = null);

        Task<bool> GroupExists(int groupId);

        Dictionary<string,string> GetGroupMetadata(int groupId);

    }
}

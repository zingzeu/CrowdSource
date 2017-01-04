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
        IEnumerable<Field> GetOriginalFields(int groupId);

        Dictionary<FieldType, string> GetLastestVersion(Group group);

        IEnumerable<Dictionary<FieldType, string>> GetAllVersions(Group group);

        void GroupNewSuggestion(Group group, Dictionary<FieldType, string> fields);

        void ReviewGroup(GroupVersion groupVesion, ApplicationUser user);

        Dictionary<string,string> GetGroupMetadata(int groupId);

    }
}

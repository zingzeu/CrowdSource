using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrowdSource.Models.CoreModels;

namespace CrowdSource.Services
{
    public interface IDataLogic
    {
        IEnumerable<Field> GetOriginalFields(Group group);

        Dictionary<FieldType, string> GetLastestVersion(Group group);

        IEnumerable<Dictionary<FieldType, string>> GetAllVersions(Group group);

        void GroupNewSuggestion(Group group, Dictionary<FieldType, string> fields);

        void ReviewGroup(Group group);


    }
}

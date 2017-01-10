using CrowdSource.Models.CoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSource.Models.CoreViewModels
{
    public class GroupDetailsViewModel
    {
        public IEnumerable<Dictionary<FieldType, string>> Versions { get; set; }
        public Group Group { get; set; }
        public IEnumerable<FieldType> FieldTypes { get; set; }
    }
}

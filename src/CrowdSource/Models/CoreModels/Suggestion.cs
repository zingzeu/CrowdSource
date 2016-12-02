using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSource.Models.CoreModels
{
    public class Suggestion
    {
        public int SuggestionId { get; set; }

        public int FieldId { get; set; }

        public string Content { get; set; }

        public ApplicationUser Author { get; set; }

        public DateTime Created { get; set; }
    }
}

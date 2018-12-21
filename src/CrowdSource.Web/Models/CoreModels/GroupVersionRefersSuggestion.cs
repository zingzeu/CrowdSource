using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSource.Models.CoreModels
{
    /// <summary>
    /// Associated entity between GroupVersion and Suggestion
    /// </summary>
    public class GroupVersionRefersSuggestion
    {
        public int GroupVersionForeignKey { get; set; }
        public int FieldTypeForeignKey { get; set; }

        //FK,PK set up in FluentAPI
        [ForeignKey("GroupVersionForeignKey")]
        public GroupVersion GroupVersion { get; set; }
        [ForeignKey("FieldTypeForeignKey")]
        public FieldType FieldType { get; set; }


        //[ForeignKey("SuggestionForeignKey")]
        public Suggestion Suggestion { get; set; }
    }
}

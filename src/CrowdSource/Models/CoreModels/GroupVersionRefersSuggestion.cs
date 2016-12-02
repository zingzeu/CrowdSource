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
        [Key]
        public int Id { get; set; }

        //[ForeignKey("GroupVersionForeignKey")]
        public GroupVersion GroupVersion { get; set; }

        //[ForeignKey("SuggestionForeignKey")]
        public Suggestion Suggestion { get; set; }
    }
}

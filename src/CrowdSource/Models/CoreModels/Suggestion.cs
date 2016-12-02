using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrowdSource.Models.CoreModels
{
    public class Suggestion
    {
        [Key]
        public int SuggestionId { get; set; }

        [ForeignKey("FieldForeignKey")]
        public Field Field { get; set; }

        [Required]
        public string Content { get; set; }

        //[ForeignKey("ApplicationUserForeignKey")]
        public ApplicationUser Author { get; set; }

        public DateTime Created { get; set; }

        public List<GroupVersionRefersSuggestion> GroupVersionsReferredTo { get; set; }
    }
}

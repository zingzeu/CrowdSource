using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrowdSource.Models.CoreModels
{
    public class GroupVersion
    {
        [Key]
        public int GroupVersionId { get; set; }

        [Required]
        //[ForeignKey("GroupForeignKey")]
        public Group Group { get; set; }

        //[ForeignKey("NextVersionForeignKey")]
        public GroupVersion NextVersion { get; set; }

        public DataType Created { get; set; }

        public List<GroupVersionRefersSuggestion> FieldSuggestions { get; set; }

        public List<ApplicationUserEndorsesGroupVersion> UserReviews { get; set; }
    }
}

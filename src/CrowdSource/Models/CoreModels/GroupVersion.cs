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

        [ForeignKey("GroupForeignKey")]
        public Group Group { get; set; }

        public GroupVersion NextVersion { get; set; }
    }
}

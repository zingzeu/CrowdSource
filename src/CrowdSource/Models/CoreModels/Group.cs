using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrowdSource.Models.CoreModels
{
    /// <summary>
    /// Group
    /// 显示组。一起显示的单位。如字典里的一行/一个词条。
    /// </summary>
    public class Group
    {
        [Key]
        public int GroupId { get; set; }
        /// <summary>
        ///  属于哪个 Collection
        /// </summary>
        //[ForeignKey("CollectionForeignKey")]
        [Required]
        public Collection Collection { get; set; }

        [Required]
        public string GroupMetadata { get; set; }

        public IEnumerable<GroupVersion> Versions;

        public FlagEnum? FlagType { get; set; }
    }
}

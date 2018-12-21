using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CrowdSource.Models.CoreModels
{
    /// <summary>
    /// Collection
    /// 众包的项目。如一本字典是一个Collection。
    /// </summary>
    public class Collection
    {
        [Key]
        public int CollectionId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        public List<Group> Groups { get; set; }

        public IEnumerable<FieldType> FieldTypes { get; set; }
    }
}

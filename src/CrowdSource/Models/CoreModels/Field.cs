using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrowdSource.Models.CoreModels
{
    /// <summary>
    /// Field
    /// </summary>
    public class Field
    {
        [Key]
        public int FieldId { get; set; }

        //[ForeignKey("GroupForeignKey")]
        [Required]
        public Group Group { get; set; }

        [Required]
        //[ForeignKey("FieldTypeForeignKey")]
        public FieldType FieldType { get; set; }
        /// <summary>
        /// 用于显示和还原这个字段的信息。如字典中的页码，对应的图片文件名等。
        /// </summary>
        [Required]
        public string FieldMetadata { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSource.Models.CoreModels
{
    /// <summary>
    /// Field
    /// </summary>
    public class Field
    {
        public int FieldId { get; set; }

        // FOREIGH KEY
        public int GroupId { get; set; }
        public Group Group { get; set; }

        // FOREIGH KEY
        public int FieldTypeId { get; set; }
        public FieldType FieldType { get; set; }
        /// <summary>
        /// 用于显示和还原这个字段的信息。如字典中的页码，对应的图片文件名等。
        /// </summary>
        public string FieldMetadata { get; set; }

    }
}

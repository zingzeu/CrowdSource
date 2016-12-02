using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSource.Models.CoreModels
{
    /// <summary>
    /// Group
    /// 显示组。一起显示的单位。如字典里的一行/一个词条。
    /// </summary>
    public class Group
    {
        public int GroupId { get; set; }
        /// <summary>
        ///  属于哪个Collection
        /// </summary>
        public Collection Collection { get; set; }

        public Dictionary<FieldType, Field> Fields;
    }
}

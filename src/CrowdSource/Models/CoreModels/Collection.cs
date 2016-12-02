using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSource.Models.CoreModels
{
    /// <summary>
    /// Collection
    /// 众包的项目。如一本字典是一个Collection。
    /// </summary>
    public class Collection
    {
        public int CollectionId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        public List<Group> Groups;
    }
}

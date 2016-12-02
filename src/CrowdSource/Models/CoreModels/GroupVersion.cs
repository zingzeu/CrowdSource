using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSource.Models.CoreModels
{
    public class GroupVersion
    {
        public int GroupVersionId { get; set; }

        public int GroupId { get; set;  }
        public Group Group { get; set; }

        public GroupVersion NextVersion { get; set; }


        

    }
}

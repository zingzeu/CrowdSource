using System;
using System.Collections.Generic;
using Zezo.Core.Configuration;

namespace Zezo.Core.GrainInterfaces
{
    public class ProjectGrainData {
        public ProjectNode Config { get; set; }
        public List<Guid> Entities { get; } = new List<Guid>();
    }
}

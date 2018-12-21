using System;
using System.Collections.Generic;
using Zezo.Core.Configuration.Steps;

namespace Zezo.Core.GrainInterfaces
{
    public class StepGrainState
    {
        public StepStatus Status { get; set; } = StepStatus.Uninitialized;
        public string Type { get; set; }
        public Guid ParentNode { get; set; }
        public Guid Entity {get; set; }
        public List<Guid> ChildNodes { get; set; } = new List<Guid>();
        public int ChildCount { get { return ChildNodes.Count; } }
        public Dictionary<string, object> Data = new Dictionary<string, object>();
        public StepNode Config { get; set; }
    }
}

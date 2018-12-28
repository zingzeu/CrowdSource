using System;
using System.Collections.Generic;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.GrainInterfaces.Observers;

namespace Zezo.Core.GrainInterfaces
{
    public class StepGrainData
    {
        public StepStatus Status { get; set; } = StepStatus.Uninitialized;
        public string Id { get; set; }
        public Guid? ParentNode { get; set; }
        public Guid Entity {get; set; }
        public List<Guid> ChildNodes { get; } = new List<Guid>();
        public int ChildCount => ChildNodes.Count;
        public Dictionary<string, object> Data = new Dictionary<string, object>();
        public StepNode Config { get; set; }
        
        /// <summary>
        /// Client-side observers. (For test only).
        /// </summary>
        public HashSet<IStepGrainObserver> Observers { get; } = new HashSet<IStepGrainObserver>();
    }
}

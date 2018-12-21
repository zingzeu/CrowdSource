using System;
using System.Collections.Generic;
using System.Text;

namespace CrowdSource.Grains.State
{
    public class StateGraphNodeState
    {
        public string Type { get; set; }
        public Guid ParentNode { get; set; }
        public List<Guid> ChildNodes { get; set; } = new List<Guid>();
        public int ChildCount { get { return ChildNodes.Count; } }
        public Dictionary<string, object> Annotations = new Dictionary<string, object>();
    }
}

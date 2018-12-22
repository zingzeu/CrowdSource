using System;
using System.Collections.Generic;

namespace Zezo.Core.GrainInterfaces
{
    public class EntityGrainData {
        public enum EntityStatus {
            Active,
            Archived
        }
        public EntityStatus Status { get; set; }

        public Guid ProjectKey { get; set; }

        public Guid PipelineRoot { get; set; }

        public IDictionary<string, Guid> Steps { get; } = new Dictionary<string, Guid>();
    }
}

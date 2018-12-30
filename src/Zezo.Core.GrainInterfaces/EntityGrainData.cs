using System;
using System.Collections.Generic;

namespace Zezo.Core.GrainInterfaces
{
    public class EntityGrainData {

        public enum EntityStatus {
            Uninitialized = 0,
            Initialized = 1,
            Active = 2,
            Archived
        }

        public EntityStatus Status { get; set; } = EntityStatus.Uninitialized;

        public Guid ProjectKey { get; set; }

        public Guid PipelineRoot { get; set; }

        public IDictionary<string, Guid> Steps { get; } = new Dictionary<string, Guid>();
    }
}

using System.Collections.Generic;

namespace Zezo.Core.GrainInterfaces.Datastores
{
    public class SimpleStoreGrainData
    {
        public bool Initialized { get; set; } = false;
        public IReadOnlyList<FieldDef> FieldDefs { get; set; }
        public IDictionary<string, object> Values { get; set; } = new Dictionary<string, object>();
    }
}
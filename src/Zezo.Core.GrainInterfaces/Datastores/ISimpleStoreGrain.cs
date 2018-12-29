using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Zezo.Core.Datastores.SimpleStore;

namespace Zezo.Core.GrainInterfaces.Datastores
{
    public interface ISimpleStoreGrain : IGrainWithGuidCompoundKey
    {
        Task Init(IReadOnlyList<FieldDef> fieldDefs, IReadOnlyDictionary<string, object> initialValues = null);
        Task<ISimpleStoreProxy> GetProxy();
        Task ApplyChanges(IReadOnlyList<ChangeRecord> changeRecords);
        /// <summary>
        /// Clears all values.
        /// </summary>
        /// <returns></returns>
        Task Clear();
    }
}
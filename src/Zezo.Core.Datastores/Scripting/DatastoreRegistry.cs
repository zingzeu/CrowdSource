using System.Collections;
using System.Collections.Generic;

namespace Zezo.Core.Grains.Datastores.Scripting
{
    /// <summary>
    /// This is the API to Datastores exposed to scripts.
    /// Example usage (inside script):
    /// ```
    ///     // access a data store (proxy object)
    ///     var simpleStore = DataStore["default"];
    ///     // get value
    ///     var fileNo = DataStore["default"]["fileNo"];
    ///     // modify value (write to temporary ledger)
    ///     DataStore["default"]["fileNo"] = "2002";
    ///     
    /// ```
    /// </summary>
    public sealed class DatastoreRegistry
    {
        public dynamic this[string key] => _dataStoreProxies[key];

        private IDictionary<string, object> _dataStoreProxies;

        public sealed class Builder
        {
            private IDictionary<string, object> _dataStoreProxies = new Dictionary<string, object>(); 
            
            public Builder AddDatastoreProxy(string name, object proxy)
            {
                _dataStoreProxies[name] = proxy;
                return this;
            }

            public DatastoreRegistry Build()
            {
                var registry = new DatastoreRegistry();
                registry._dataStoreProxies = this._dataStoreProxies;
                return registry;
            }
        }
    }
}
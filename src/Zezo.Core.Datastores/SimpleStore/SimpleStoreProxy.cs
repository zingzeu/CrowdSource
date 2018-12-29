using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Zezo.Core.Datastores.SimpleStore
{

    public interface IHasChangeRecords
    {
        IReadOnlyList<ChangeRecord> _ChangeRecords { get;  }
    }

    public sealed class SimpleStoreProxy : ISimpleStoreProxy, IHasChangeRecords
    {
        private readonly IList<ChangeRecord> changeRecords = new List<ChangeRecord>();
        private readonly IReadOnlyList<FieldDef> fields;
        private readonly IReadOnlyDictionary<string, object> fieldInitialValues;

        IReadOnlyList<ChangeRecord> IHasChangeRecords._ChangeRecords
            => new ReadOnlyCollection<ChangeRecord>(changeRecords);
        
        public dynamic this[string key]
        {
            get
            {
                // check key
                if (fields.All(x => x.Name != key))
                {
                    throw new Exception($"Unknown field '{key}'");
                }
                
                // if there is a ChangeRecord, return the new value
                if (changeRecords.Any(x => x.Name == key))
                {
                    return changeRecords.Single(x => x.Name == key).NewValue;
                }
                
                // otherwise return the old value
                var initialValue = fieldInitialValues[key];
                var fieldDef = fields.Single(x => x.Name == key);
                return initialValue;
            }
            set
            {
                // check key
                if (fields.All(x => x.Name != key))
                {
                    throw new Exception($"Unknown field '{key}'");
                }
                // check type
                var fieldDef = fields.Single(x => x.Name == key);
                if (value.GetType() != fieldDef.Type)
                {
                    throw new Exception($"The value for field '{key}' is not of the correct type. " +
                                        $"Expected {fieldDef.Type}, found {value.GetType()}.");
                }
                dynamic initialValue = fieldInitialValues[key];

                if (changeRecords.Any(x => x.Name == key))
                {
                    var changeRecord = changeRecords.Single(x => x.Name == key);
                    if (value == initialValue)
                    {
                        changeRecords.Remove(changeRecord);
                    }
                    else
                    {
                        changeRecord.NewValue = value;
                    }
                }
                else
                {
                    if (value != initialValue)
                    {
                        changeRecords.Add(new ChangeRecord()
                        {
                            Name = key,
                            NewValue = value
                        });
                    }
                }
                
            }
        }

        public SimpleStoreProxy(IList<FieldDef> fields, IDictionary<string, object> fieldInitialValues)
        {
            this.fields = new List<FieldDef>(fields);
            this.fieldInitialValues = new ReadOnlyDictionary<string, object>(fieldInitialValues);
        }
        
        
    }

}
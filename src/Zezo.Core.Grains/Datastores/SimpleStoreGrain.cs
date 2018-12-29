using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orleans;
using Orleans.Providers;
using Zezo.Core.Datastores.SimpleStore;
using Zezo.Core.GrainInterfaces.Datastores;
using Zezo.Core.Grains.Datastores.Scripting;

namespace Zezo.Core.Grains.Datastores
{
    [StorageProvider(ProviderName="DevStore")]
    public class SimpleStoreGrain : Grain<SimpleStoreGrainData>, ISimpleStoreGrain
    {
        public Task Init(IReadOnlyList<FieldDef> fieldDefs, IReadOnlyDictionary<string, object> initialValues)
        {
            if (State.Initialized)
            {
                return Task.FromException(new Exception("SimpleStoreGrain: has already been initialized."));
            }

            var storedFieldDefs = new List<FieldDef>();
            foreach (var fieldDef in fieldDefs)
            {
                if (storedFieldDefs.Count(x => x.Name == fieldDef.Name) == 0)
                {
                    storedFieldDefs.Add(fieldDef);
                }
                else
                {
                    return Task.FromException(
                        new Exception($"SimpleStoreGrain: field '{fieldDef.Name}' already exists."));
                }
            }

            State.FieldDefs = storedFieldDefs;

            if (initialValues != null)
            {
                foreach (var pair in initialValues)
                {
                    if (fieldDefs.Any(x => x.Name == pair.Key))
                    {
                        var fieldDef = fieldDefs.Single(x => x.Name == pair.Key);
                        dynamic value = pair.Value;
                        if (fieldDef.Type != value.GetType())
                        {
                            return Task.FromException(new Exception($"Field '{fieldDef.Name}' expected type" +
                                                                    $" {fieldDef.Type}, found {value.GetType()}"));
                        }

                        if (!fieldDef.Nullable && value == null)
                        {
                            return Task.FromException(new Exception($"Field '{fieldDef.Name}' is not nullable"));
                        }
                        State.Values[pair.Key] = pair.Value;
                    }
                    else
                    {
                        return Task.FromException(
                            new Exception($"SimpleStoreGrain: field '{pair.Key}' does not exist."));
                    }
                }
            }
            
            // check all non-null fields has value
            foreach (var fieldDef in State.FieldDefs.Where(x => !x.Nullable))
            {
                if (!State.Values.ContainsKey(fieldDef.Name) || (State.Values[fieldDef.Name] == null))
                {
                    return Task.FromException(new Exception($"Field '{fieldDef.Name}' is not nullable"));
                }
            }
                
            State.Initialized = true;

            return WriteStateAsync();
        }

        public async Task<ISimpleStoreProxy> GetProxy()
        {
            await ReadStateAsync();
            // TODO: deal with null values
            return new SimpleStoreProxy(new List<FieldDef>(State.FieldDefs),
                State.Values);
        }

        public Task ApplyChanges(IReadOnlyList<ChangeRecord> changeRecords)
        {
            foreach (var changeRecord in changeRecords)
            {
                State.Values[changeRecord.Name] = changeRecord.NewValue;
            }
            return WriteStateAsync();
        }

        public Task Clear()
        {
            throw new System.NotImplementedException();
        }
    }
}
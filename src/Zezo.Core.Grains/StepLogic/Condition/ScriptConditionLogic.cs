using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Zezo.Core.Configuration.Steps.Condition;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Zezo.Core.Grains.Datastores.Scripting;

namespace Zezo.Core.Grains.StepLogic.Condition
{
    public class ScriptConditionLogic : IConditionLogic
    {
        private readonly IContainer _container;
        private readonly string _code;

        public ScriptConditionLogic(ScriptConditionNode config, IContainer container)
        {
            _container = container;
            _code = config.InlineSource;
        }

        public class Globals
        {
            public DatastoreRegistry Datastores;
        }
        
        public async Task<bool> Evaluate()
        {
            try
            {
                // setup references needed for dynamics
                var refs = new List<MetadataReference>{
                    MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(System.Runtime.CompilerServices.DynamicAttribute).GetTypeInfo().Assembly.Location)};
                
                var script = CSharpScript.Create<bool>(_code, options: ScriptOptions.Default.AddReferences(refs), globalsType: typeof(Globals));
                script.Compile();
                
                bool result = (await script.RunAsync(new Globals()
                {
                    Datastores = await _container.GetDatastoreRegistry()
                })).ReturnValue;
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
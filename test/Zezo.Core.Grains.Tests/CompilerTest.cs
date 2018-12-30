using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Xunit;
using Zezo.Core.Grains.StepLogic.Condition;

namespace Zezo.Core.Grains.Tests
{
    public class CompilerTest
    {
        [Theory]
        [InlineData(@"
            var a = 1;
            var b = 2;
            return a + b == 3;
        ")]
        [InlineData(@"
            var a  = 1;
            var b = 3;
            var c = a+b;
            dynamic x = 1;
            return x == 1;
        ")]
        public async Task Can_Compile_and_Run(string _code)
        {
            var refs = new List<MetadataReference>{
                MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.CompilerServices.DynamicAttribute).GetTypeInfo().Assembly.Location)};
                
            var script = CSharpScript.Create<bool>(_code, options: ScriptOptions.Default.AddReferences(refs), globalsType: typeof(ScriptConditionLogic.Globals));
            script.Compile();
                
            bool result = (await script.RunAsync(new ScriptConditionLogic.Globals()
            {
                Datastores = null
            })).ReturnValue;
            Assert.True(result);
        }
    }
}
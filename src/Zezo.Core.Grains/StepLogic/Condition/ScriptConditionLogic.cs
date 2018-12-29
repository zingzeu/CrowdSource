using System;
using System.Threading.Tasks;
using Zezo.Core.Configuration.Steps.Condition;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace Zezo.Core.Grains.StepLogic.Condition
{
    public class ScriptConditionLogic : IConditionLogic
    {
        private readonly string _code;


        public ScriptConditionLogic(ScriptConditionNode config)
        {
            _code = config.InlineSource;
        }

        public async Task<bool> Evaluate()
        {
            try
            {
                bool result = await CSharpScript.EvaluateAsync<bool>(_code);
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
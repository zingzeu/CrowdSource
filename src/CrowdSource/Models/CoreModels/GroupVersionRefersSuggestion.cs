using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSource.Models.CoreModels
{
    /// <summary>
    /// Associated entity between GroupVersion and Suggestion
    /// </summary>
    public class GroupVersionRefersSuggestion
    {
        public int GroupVersionId;
        public GroupVersion GroupVersion;

        public int SuggestionId;
        public Suggestion Suggestion;
    }
}

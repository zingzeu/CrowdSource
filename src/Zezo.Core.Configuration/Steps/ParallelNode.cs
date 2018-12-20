using System.Collections.Generic;
using System.Xml;
using Zezo.Core.Configuration.Extensions;

namespace Zezo.Core.Configuration.Steps {
    
    public sealed class ParallelNode : StepNode {
        public new static string TagName { get { return "Parallel"; } }
        private readonly List<StepNode> _children = new List<StepNode>();
        public IReadOnlyList<StepNode> Children { get { return _children; } }
        public ParallelNode(XmlElement xmlElem, IParser parser) : base(xmlElem, parser)
        {
            this._children.AddRange(xmlElem.GetCollectionAttribute<StepNode>("Children", parser));
        }
    }

}
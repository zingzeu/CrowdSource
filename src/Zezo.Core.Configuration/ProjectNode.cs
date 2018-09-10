using System;
using System.Collections.Generic;
using System.Xml;
using Zezo.Core.Configuration.Extensions;
using Zezo.Core.Configuration.Steps;


namespace Zezo.Core.Configuration {

    public sealed class ProjectNode : ConfigurationNode {

        public ProjectNode(XmlElement xmlElem, IParser parser)
        {
            this.Id = xmlElem.GetAttribute("Id");
            this.Name = xmlElem.GetStringAttribute("Name").Trim();
            var pipelineNodes = xmlElem.GetComplexAttribute("Pipeline");
            if (pipelineNodes.Count > 1) {
                throw new Exception("Project.Pipeline can only have one value");   
            } else if (pipelineNodes.Count == 1) {
                this.Pipeline = parser.ParseXmlElement(pipelineNodes[0]) as StepNode;
                if (this.Pipeline == null) {
                    throw new Exception($"{pipelineNodes[0].LocalName} is not a valid Step");
                }
            }
        }
        public new static string TagName { get { return "Project"; } }
        public string Id { get; private set; }
        public string Name { get; private set; }

        // Pipeline Root
        public StepNode Pipeline {get; private set;}

        private readonly List<QueueNode> _queues = new List<QueueNode>();
        public IReadOnlyList<QueueNode> Queues {get {return this._queues;}}
    }
}
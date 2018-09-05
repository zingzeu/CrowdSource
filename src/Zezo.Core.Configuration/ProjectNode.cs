using System.Collections.Generic;
using System.Xml;
using Zezo.Core.Configuration.Steps;


namespace Zezo.Core.Configuration {

    public sealed class ProjectNode : ConfigurationNode {

        public ProjectNode(XmlElement xmlElem, IParser parser)
        {
            this.Id = xmlElem.GetAttribute("Id");
            this.Name = parser.GetStringAttribute(xmlElem, "Name", this.GetTagName()+"."+"Name");
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
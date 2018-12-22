using System;
using System.Collections.Generic;
using System.Xml;
using Zezo.Core.Configuration.Extensions;
using Zezo.Core.Configuration.Middleware;

namespace Zezo.Core.Configuration.Steps {
    
    public sealed class InteractiveNode : StepNode {
        public new static string TagName { get { return "Interactive"; } }

        public override string StepType { get => "Interactive"; }

        // id of the queue to publish to
        public string Queue { get; private set; }
        public TimeSpan TimeLimit { get; private set; }
        public IReadOnlyList<MiddlewareNode> BeforePublish { get; private set; }
        public IReadOnlyList<MiddlewareNode> BeforeSubmit { get; private set; }
        public IReadOnlyList<MiddlewareNode> AfterSubmit { get; private set; }

        public InteractiveNode(XmlElement xmlElem, IParser parser) : base(xmlElem, parser) {
            Queue = xmlElem.GetStringAttribute("Queue");
            TimeLimit = ParseTime(xmlElem.GetStringAttribute("TimeLimit"));
            BeforePublish = xmlElem.GetCollectionAttribute<MiddlewareNode>("BeforePublish", parser);
            BeforeSubmit = xmlElem.GetCollectionAttribute<MiddlewareNode>("BeforeSubmit", parser);
            AfterSubmit = xmlElem.GetCollectionAttribute<MiddlewareNode>("AfterSubmit", parser);
        }

        private TimeSpan ParseTime(String t) {
            if (t.EndsWith("min")) {
                var minutesStr = t.Substring(0, t.Length - 3);
                int minutes;
                if (Int32.TryParse(minutesStr, out minutes)) {
                    return new TimeSpan(0, minutes, 0);
                }
            }
            throw new Exception($"Invalid time \"{t}\"");
        }
    }

}
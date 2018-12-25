using System;
using System.Collections.Generic;
using System.Xml;
using Zezo.Core.Configuration.Extensions;

namespace Zezo.Core.Configuration.Steps {
    
    public sealed class DummyStepNode : StepNode {
        public new static string TagName => "DummyStep";
        public override string StepType { get => "DummyStep"; }
        
        /// <summary>
        /// Time to wait before eagerly starting working (in ms).
        /// </summary>
        public TimeSpan BeforeStart { get; private set; }
        
        /// <summary>
        /// Time to work (in ms).
        /// </summary>

        public TimeSpan Working { get; private set; }

        public DummyStepNode(XmlElement xmlElem, IParser parser) : base(xmlElem, parser)
        {
            BeforeStart = ParseTime(xmlElem.GetStringAttribute("BeforeStart"));
            Working = ParseTime(xmlElem.GetStringAttribute("Working"));
        }
    }

}
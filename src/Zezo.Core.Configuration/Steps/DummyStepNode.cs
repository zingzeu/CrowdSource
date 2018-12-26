using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Zezo.Core.Configuration.Extensions;

namespace Zezo.Core.Configuration.Steps {
    
    /// <summary>
    /// For testing only.
    /// </summary>
    public sealed class DummyStepNode : StepNode {
        public new static string TagName => "DummyStep";
        public override string StepType { get => "DummyStep"; }
        
        /// <summary>
        /// Time to wait before eagerly starting working (in ms).
        /// </summary>
        public TimeSpan BeforeStart { get; }
        
        /// <summary>
        /// e.g. "1000ms,2000ms,1000ms"
        /// Work 1000ms, pause 2000ms, Work 1000ms
        /// This overrides the "Working attribute".
        /// </summary>
        public IList<TimeSpan> WorkingSequence { get; }

        public TimeSpan Working { get; }
        public DummyStepNode(XmlElement xmlElem, IParser parser) : base(xmlElem, parser)
        {
            BeforeStart = ParseTime(xmlElem.GetStringAttribute("BeforeStart"));
            
            Working = ParseTime(xmlElem.GetStringAttribute("Working"));

            var workingSeq = xmlElem.GetStringAttribute("WorkingSequence");
            WorkingSequence = workingSeq == String.Empty ? null :
                workingSeq.Split(new char[]{','})
                .Select(s => s.Trim())
                .Select(ParseTime)
                .ToList() ?? null;
        }
    }

}
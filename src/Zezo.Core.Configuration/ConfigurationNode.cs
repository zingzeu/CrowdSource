using System;
using System.Xml;
using System.Collections.Generic;

namespace Zezo.Core.Configuration
{
    public abstract class ConfigurationNode {
        public static string TagName => throw null;

        public string GetTagName() {
            return this.GetType().GetProperty("TagName").GetValue(null) as string;
        }
        
        protected TimeSpan ParseTime(String t) {
            if (t.EndsWith("min")) 
            {
                var minutesStr = t.Substring(0, t.Length - 3);
                if (int.TryParse(minutesStr, out var minutes)) {
                    return new TimeSpan(0, minutes, 0);
                }
            }
            else if (t.EndsWith("ms")) 
            {
                var mmsStr = t.Substring(0, t.Length - 2);
                if (int.TryParse(mmsStr, out var mms)) {
                    return TimeSpan.FromMilliseconds(mms);
                }
            }
            throw new Exception($"Invalid time \"{t}\"");
        }
    }
}
using System;

namespace Zezo.Core
{
    public class FieldDef
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public bool Nullable { get; set; }
    }
}
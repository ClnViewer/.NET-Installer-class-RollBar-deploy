using System;

namespace AssemblyExt.Properties
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyRollBarId : Attribute
    {
        public string Value { get; private set; }

        public AssemblyRollBarId() : this("") { }
        public AssemblyRollBarId(string value) { Value = value; }
    }
}

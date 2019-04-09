using System;

namespace Graphify.EPiServer.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class GraphPropertyAttribute : Attribute
    {
        public bool Hide { get; set; }
        public string PropertyName { get; set; }
    }
}

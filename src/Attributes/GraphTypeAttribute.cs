using System;
using System.Collections.Generic;
using System.Text;

namespace Graphify.EPiServer.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GraphTypeAttribute : Attribute
    {
        public bool Hide { get; set; }
        public string GraphName { get; set; }
    }
}

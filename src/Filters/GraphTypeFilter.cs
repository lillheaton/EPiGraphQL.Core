using EPiServer.Core;
using Graphify.EPiServer.Core.Attributes;
using System;

namespace Graphify.EPiServer.Core.Filters
{
    public class GraphTypeFilter
    {
        public static bool ShouldFilter(Type type)
        {
            return type.HasAttributeWithCondition<GraphTypeAttribute>(attr => attr.Hide == true);
        }

        public static bool ShouldFilter(IContent content)
        {
            return ShouldFilter(content.GetType());
        }
    }
}

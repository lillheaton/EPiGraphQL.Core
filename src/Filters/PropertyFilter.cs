using Graphify.EPiServer.Core.Attributes;
using System.Reflection;

namespace Graphify.EPiServer.Core.Filters
{
    public class PropertyFilter
    {
        public static bool ShouldFilter(PropertyInfo propertyInfo)
        {
            return propertyInfo.HasAttributeWithCondition<GraphPropertyAttribute>(attr => attr.Hide == true);
        }
    }
}

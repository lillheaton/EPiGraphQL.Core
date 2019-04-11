using EPiServer.Core;
using Graphify.EPiServer.Core.Filters;
using System.Collections.Generic;
using System.Linq;

namespace Graphify.EPiServer
{
    public static class IContentEnumerableEx
    {
        public static IEnumerable<IContent> FilterHiddenGraphTypes(this IEnumerable<IContent> enumerable)
        {
            return enumerable.Where(x => !GraphTypeFilter.ShouldFilter(x));
        }
    }
}

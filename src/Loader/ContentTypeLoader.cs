using EPiServer.DataAbstraction;
using Graphify.EPiServer.Core.Filters;
using System.Collections.Generic;
using System.Linq;

namespace Graphify.EPiServer.Core.Loader
{
    public class ContentTypeLoader
    {
        public static IEnumerable<ContentType> GetAvailableEpiContentTypes(IContentTypeRepository contentTypeRepository)
        {
            return contentTypeRepository
                .List()
                .Where(x => 
                    x.ModelType != null && !GraphTypeFilter.ShouldFilter(x.ModelType)
                );
        }        
    }
}

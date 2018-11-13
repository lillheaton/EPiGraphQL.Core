using Eols.EPiGraphQL.Core.Attributes;
using EPiServer.DataAbstraction;
using System.Collections.Generic;
using System.Linq;

namespace Eols.EPiGraphQL.Core.Loader
{
    public class ContentTypeLoader
    {
        public static IEnumerable<ContentType> GetAvailableEpiContentTypes(IContentTypeRepository contentTypeRepository)
        {
            return contentTypeRepository
                .List()
                .Where(x => 
                    x.ModelType != null && 
                    x.ModelType.HasAttribute<GraphHideAttribute>() == false
                );
        }        
    }
}

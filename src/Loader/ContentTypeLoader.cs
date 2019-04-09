using Graphify.EPiServer.Core.Attributes;
using EPiServer.DataAbstraction;
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
                    x.ModelType != null && 
                    x.ModelType.HasAttributeWithConditionOrTrue<GraphTypeAttribute>(attr => attr.Hide == false)
                );
        }        
    }
}

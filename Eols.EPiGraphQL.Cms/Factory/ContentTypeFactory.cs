using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using GraphQL.Types;
using System.Collections.Generic;
using System.Linq;

namespace Eols.EPiGraphQL.Cms.Factory
{
    public class ContentTypeFactory
    {
        public static IEnumerable<ContentType> GetAvailableContentTypes(IContentTypeRepository contentTypeRepository)
        {
            return contentTypeRepository.List().Where(x => x.ModelType != null);
        }

        public static IInterfaceGraphType GetContentGraphInterface()
        {
            var interfaces = ServiceLocator.Current
                .GetAllInstances<IInterfaceGraphType>()
                .ToArray();

            return interfaces
                .FirstOrDefault(x => 
                    x.GetType()
                    .BaseType
                    .GetGenericArguments()[0] == typeof(IContent)
                );
        }
    }
}

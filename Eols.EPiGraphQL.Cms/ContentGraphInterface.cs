using Eols.EPiGraphQL.Cms.Factory;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using GraphQL.Types;
using System.Linq;

namespace Eols.EPiGraphQL.Cms
{
    [ServiceConfiguration(typeof(IInterfaceGraphType), Lifecycle = ServiceInstanceScope.Singleton)]    
    public class ContentGraphInterface : InterfaceGraphType<IContent>
    {
        public ContentGraphInterface(
            IContentLoader contentLoader, 
            IContentTypeRepository contentTypeRepository)
        {
            var availableTypes = ContentTypeFactory
                .GetAvailableContentTypes(contentTypeRepository)
                .Select(x => x.ModelType)
                .ToArray();

            Name = "ContentInterface";
            Field(x => x.Name);
            Field(x => x.ContentTypeID);
            Field<IntGraphType>("Id", resolve: x => x.Source.ContentLink.ID);
            Field<IntGraphType>("ParentId", resolve: x => x.Source.ParentLink.ID);
            Field<StringGraphType>("Type", resolve: x => x.Source.GetOriginalType().Name);
            Field<ListGraphType<ContentGraphInterface>>(
                "Children",
                resolve: x =>
                {
                    return contentLoader
                            .GetChildren<PageData>(x.Source.ContentLink)
                            .Where(content =>
                                availableTypes
                                .Contains(content.GetOriginalType())
                            );
                });
        }
    }
}

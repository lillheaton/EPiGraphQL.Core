using Graphify.EPiServer.Core.Loader;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using GraphQL.Types;
using System;
using Graphify.EPiServer.Core.Filters;

namespace Graphify.EPiServer.Core.Types
{
    [ServiceConfiguration(typeof(ICustomGraphType), Lifecycle = ServiceInstanceScope.Singleton)]
    class PageReferenceGraphType : ObjectGraphType<PageReference>, ICustomGraphType
    {
        public Type TargetType => typeof(PageReference);

        public PageReferenceGraphType(IContentLoader contentLoader, IServiceLocator serviceLocator)
        {
            Name = "PageReference";

            // Get IContent graph interface type
            var contentInterface = GraphTypeLoader.GetGraphInterface<IContent>(serviceLocator);

            Field("Id", x => x.ID);
            Field(contentInterface.GetType(),
                "Content",
                resolve: x => {
                    var content = contentLoader.Get<IContent>(x.Source);
                    return GraphTypeFilter.ShouldFilter(content) ? null : content;
                });
        }
    }
}

using Eols.EPiGraphQL.Core;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using GraphQL.Types;
using System;

namespace Eols.EPiGraphQL.Cms.Types
{
    [ServiceConfiguration(typeof(ICustomGraphType), Lifecycle = ServiceInstanceScope.Singleton)]
    class PageReferenceGraphType : ObjectGraphType<PageReference>, ICustomGraphType
    {
        public Type TargetType => typeof(PageReference);

        public PageReferenceGraphType(IContentLoader contentLoader)
        {
            Name = "PageReference";

            Field("Id", x => x.ID);
            Field<ContentGraphInterface>("Content",
                resolve: x => contentLoader.Get<IContent>(x.Source));

        }
    }
}

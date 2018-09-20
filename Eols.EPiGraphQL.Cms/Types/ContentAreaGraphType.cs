using Eols.EPiGraphQL.Core;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using GraphQL.Types;
using System;
using System.Linq;

namespace Eols.EPiGraphQL.Cms.Types
{
    [ServiceConfiguration(typeof(ICustomGraphType), Lifecycle = ServiceInstanceScope.Singleton)]
    public class ContentAreaGraphType : ObjectGraphType<ContentArea>, ICustomGraphType
    {
        public Type TargetType => typeof(ContentArea);

        public ContentAreaGraphType()
        {
            Name = "ContentArea";
            
            Field(x => x.Count);
            Field<ListGraphType<ContentGraphInterface>>(
                "FilteredItems",
                resolve: context =>
                    context
                    .Source
                    .FilteredItems
                    .Select(item
                        => item.GetContent()
                    )
                );
        }
    }
}

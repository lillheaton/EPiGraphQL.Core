using Eols.EPiGraphQL.Core.Loader;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using GraphQL.Types;
using System;
using System.Linq;

namespace Eols.EPiGraphQL.Core.Types
{
    [ServiceConfiguration(typeof(ICustomGraphType), Lifecycle = ServiceInstanceScope.Singleton)]
    public class ContentAreaGraphType : ObjectGraphType<ContentArea>, ICustomGraphType
    {
        public Type TargetType => typeof(ContentArea);

        public ContentAreaGraphType(IServiceLocator serviceLocator)
        {
            Name = "ContentArea";

            // Create ListGraphType<> with IContent graph interface type
            var contentInterface = GraphTypeLoader.GetGraphInterface<IContent>(serviceLocator);
            var itemsType = typeof(ListGraphType<>).MakeGenericType(contentInterface.GetType());

            Field(x => x.Count);
            Field(itemsType,
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

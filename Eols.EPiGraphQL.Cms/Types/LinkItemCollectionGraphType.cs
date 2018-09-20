using Eols.EPiGraphQL.Core;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;
using GraphQL.Types;
using System;

namespace Eols.EPiGraphQL.Cms.Types
{
    [ServiceConfiguration(typeof(ICustomGraphType), Lifecycle = ServiceInstanceScope.Singleton)]
    public class LinkItemCollectionGraphType : ObjectGraphType<LinkItemCollection>, ICustomGraphType
    {
        public Type TargetType => typeof(LinkItemCollection);

        public LinkItemCollectionGraphType()
        {
            Name = "LinkItemCollection";
            Description = "Property that is used to store multiple html links";

            Field(x => x.Count);
            Field<ListGraphType<LinkItemGraphType>>("List", resolve: x => x.Source);
        }
    }
}

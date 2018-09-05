using Eols.EPiGraphQL.Core;
using EPiServer.ServiceLocation;
using GraphQL.Types;

namespace Eols.EPiGraphQL.Cms
{
    [ServiceConfiguration(typeof(IEPiServerGraph), Lifecycle = ServiceInstanceScope.Singleton)]
    public class ContentGraph : ObjectGraphType, IEPiServerGraph
    {
        public ContentGraph()
        {
            Name = "Content";
            Field<StringGraphType>("Id");
            Field<StringGraphType>("Name");
        }
    }
}
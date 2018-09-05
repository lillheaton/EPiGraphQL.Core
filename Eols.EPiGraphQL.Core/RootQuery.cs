using GraphQL.Types;

namespace Eols.EPiGraphQL.Core
{
    public class RootQuery : ObjectGraphType, IRootQuery
    {
        public RootQuery(IGraphQLEPiServerDependencyResolver resolver)
        {
            Name = "Query";

            var graphs = resolver.GetAllInstances<IEPiServerGraph>();

            foreach(var graph in graphs)
            {
                Field(graph.GetType(), graph.Name, graph.Description);
            }
        }
    }
}
using EPiServer.ServiceLocation;
using GraphQL.Types;

namespace EPiGraphQL.Core
{
    [ServiceConfiguration(typeof(IRootQuery), Lifecycle = ServiceInstanceScope.Singleton)]
    public class RootQuery : ObjectGraphType, IRootQuery
    {
        public RootQuery(IServiceLocator serviceLocator)
        {
            Name = "Query";

            var graphs = serviceLocator.GetAllInstances<IEPiServerGraph>();

            foreach(var graph in graphs)
            {
                Field(
                    graph.GetType(), 
                    graph.Name, 
                    graph.Description, 
                    resolve: context =>  
                    {
                        return new { }; // For some reason need to respond empty object?
                    });
            }
        }
    }
}
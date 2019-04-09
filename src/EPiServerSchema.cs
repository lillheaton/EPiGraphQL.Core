using Graphify.EPiServer.Core.Loader;
using EPiServer.ServiceLocation;
using GraphQL.Types;
using GraphQL.Utilities;
using System.Linq;

namespace Graphify.EPiServer.Core
{
    [ServiceConfiguration(typeof(ISchema), Lifecycle = ServiceInstanceScope.Singleton)]
    public class EPiServerSchema : Schema
    {
        public EPiServerSchema(GraphQL.IDependencyResolver resolver) : base(resolver)
        {            
            var serviceLocator = resolver.Resolve<IServiceLocator>();

            // Step 1: Register all types so we can for example use (x => x.contentarea)
            RegisterCustomGraphTypes(serviceLocator);

            // step 2: Register all graphs, interfaces, unions etc
            RegisterInterfacesAndUnions(serviceLocator);
            
            Query = resolver.Resolve<IRootQuery>();
        }

        private void RegisterCustomGraphTypes(IServiceLocator serviceLocator)
        {
            var customGraphTypes = GraphTypeLoader.GetCustomGraphTypes(serviceLocator).ToArray();
            foreach (var customGraphType in customGraphTypes)
            {
                GraphTypeTypeRegistry.Register(customGraphType.TargetType, customGraphType.GetType());
            }
        }

        private void RegisterInterfacesAndUnions(IServiceLocator serviceLocator)
        {
            var interfaces = GraphTypeLoader.GetGraphInterfaces(serviceLocator).ToArray();
            var unions = GraphTypeLoader.GetGraphUnions(serviceLocator).ToArray();

            RegisterTypes(interfaces);
            RegisterTypes(unions);
        }
    }
}

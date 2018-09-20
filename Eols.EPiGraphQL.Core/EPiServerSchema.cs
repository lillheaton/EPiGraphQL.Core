using EPiServer.Core;
using EPiServer.ServiceLocation;
using GraphQL;
using GraphQL.Types;
using GraphQL.Utilities;
using System.Linq;

namespace Eols.EPiGraphQL.Core
{
    [ServiceConfiguration(typeof(ISchema), Lifecycle = ServiceInstanceScope.Singleton)]
    public class EPiServerSchema : Schema
    {
        public EPiServerSchema(GraphQL.IDependencyResolver resolver) : base(resolver)
        {
            var serviceLocator = resolver.Resolve<IServiceLocator>();
            
            // Step 1: Register all types so we can (x => x.contentarea)
            var customGraphTypes = serviceLocator.GetAllInstances<ICustomGraphType>().ToArray();
            foreach (var customGraphType in customGraphTypes)
            {
                GraphTypeTypeRegistry.Register(customGraphType.TargetType, customGraphType.GetType());
            }
            
            // step 2: Register all graphs, interfaces, unions etc
            var interfaces = serviceLocator.GetAllInstances<IInterfaceGraphType>().ToArray();
            var unions = serviceLocator.GetAllInstances<IEPiServerGraphUnion>().ToArray();
            
            RegisterTypes(interfaces);
            RegisterTypes(unions);
            
            Query = resolver.Resolve<IRootQuery>();
        }
    }
}
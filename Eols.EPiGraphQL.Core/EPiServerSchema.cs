using EPiServer.ServiceLocation;
using GraphQL.Types;
using System.Linq;

namespace Eols.EPiGraphQL.Core
{
    [ServiceConfiguration(typeof(ISchema), Lifecycle = ServiceInstanceScope.Singleton)]
    public class EPiServerSchema : Schema
    {
        public EPiServerSchema(GraphQL.IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<IRootQuery>();

            var serviceLocator = resolver.Resolve<IServiceLocator>();
            var interfaces = serviceLocator.GetAllInstances<IInterfaceGraphType>().ToArray();
            var unions = serviceLocator.GetAllInstances<IEPiServerGraphUnion>().ToArray();

            RegisterTypes(interfaces);
            RegisterTypes(unions);
        }
    }
}
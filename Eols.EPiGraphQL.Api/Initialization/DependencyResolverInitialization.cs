using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using GraphQL;
using GraphQL.Http;

namespace Eols.EPiGraphQL.Api.Initialization
{
    [InitializableModule]
    public class DependencyResolverInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.ConfigurationComplete += (o, e) =>
            {
                var services = context.Services;
                
                services.AddSingleton<IDocumentExecuter>(new DocumentExecuter());
                services.AddSingleton<IDocumentWriter>(new DocumentWriter(true));
                services.AddSingleton<GraphQL.Relay.Http.RequestExecutor>();
                
                services.AddSingleton<GraphQL.IDependencyResolver>(x => 
                    new FuncDependencyResolver(type =>  x.GetInstance(type))
                );
            };
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
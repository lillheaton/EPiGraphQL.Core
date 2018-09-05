using Eols.EPiGraphQL.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;

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

                services.AddSingleton<IGraphQLEPiServerDependencyResolver>(x => new GraphQLEPiServerFuncDependencyResolver(x));
                services.AddSingleton<GraphQL.IDependencyResolver>(x => new GraphQLEPiServerFuncDependencyResolver(x));
                

                services.AddSingleton<IDocumentExecuter>(new DocumentExecuter());
                services.AddSingleton<IDocumentWriter>(new DocumentWriter(true));

                services.AddSingleton<IRootQuery, RootQuery>();
                services.AddSingleton<ISchema, EPiServerSchema>();
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
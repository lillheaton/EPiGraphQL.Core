using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using System.Web.Http;

namespace Eols.EPiGraphQL.Api.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(FrameworkInitialization))]
    public class ApiInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            GlobalConfiguration.Configure(config =>
            {
                // TODO: Check if already initialized

                config.MapHttpAttributeRoutes();
            });
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
using EPiServer.Core;
using EPiServer.ServiceLocation;
using GraphQL.Types;

namespace Eols.EPiGraphQL.Cms
{
    [ServiceConfiguration(typeof(IInterfaceGraphType), Lifecycle = ServiceInstanceScope.Singleton)]
    public class ContentDataGraphInterface : InterfaceGraphType<IContentData>
    {
        public ContentDataGraphInterface()
        {
            Name = "ContentDataInterface";
        }
    }
}

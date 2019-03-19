using EPiGraphQL.Core.Loader;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using GraphQL.Types;
using System;

namespace EPiGraphQL.Core.Types
{
    [ServiceConfiguration(typeof(ICustomGraphType), Lifecycle = ServiceInstanceScope.Singleton)]
    public class ContentReferenceGraphType : ObjectGraphType<ContentReference>, ICustomGraphType
    {
        public Type TargetType => typeof(ContentReference);

        public ContentReferenceGraphType(IContentLoader contentLoader, IServiceLocator serviceLocator)
        {
            Name = "ContentReference";

            // Get IContent graph interface type
            var contentInterface = GraphTypeLoader.GetGraphInterface<IContent>(serviceLocator);            

            Field("Id", x => x.ID);
            Field(contentInterface.GetType(),
                "Content",
                resolve: x => {
                    return contentLoader.Get<IContent>(x.Source);                    
                });
        }
    }
}

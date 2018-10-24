using Eols.EPiGraphQL.Cms.Factory;
using Eols.EPiGraphQL.Core;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using GraphQL.Types;
using System;
using System.Linq;

namespace Eols.EPiGraphQL.Cms.Types
{
    [ServiceConfiguration(typeof(ICustomGraphType), Lifecycle = ServiceInstanceScope.Singleton)]
    public class ContentReferenceGraphType : ObjectGraphType<ContentReference>, ICustomGraphType
    {
        public Type TargetType => typeof(ContentReference);

        public ContentReferenceGraphType(IContentLoader contentLoader, IContentTypeRepository contentTypeRepository)
        {
            Name = "ContentReference";
            
            Field("Id", x => x.ID);
            Field<ContentGraphInterface>("Content",
                resolve: x => {
                    return contentLoader.Get<IContent>(x.Source);                    
                });
        }
    }
}

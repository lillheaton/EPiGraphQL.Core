﻿using Eols.EPiGraphQL.Core;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using GraphQL.Types;
using System;

namespace Eols.EPiGraphQL.Cms.Types
{
    [ServiceConfiguration(typeof(ICustomGraphType), Lifecycle = ServiceInstanceScope.Singleton)]
    public class ContentReferenceGraphType : ObjectGraphType<ContentReference>, ICustomGraphType
    {
        public Type TargetType => typeof(ContentReference);

        public ContentReferenceGraphType(IContentLoader contentLoader)
        {
            Name = "ContentReference";

            Field("Id", x => x.ID);
            Field<ContentGraphInterface>("Content",
                resolve: x => contentLoader.Get<IContent>(x.Source));

        }
    }
}

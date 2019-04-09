using EPiServer;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using GraphQL.Language.AST;
using GraphQL.Types;
using System;

namespace Graphify.EPiServer.Core.Types
{
    [ServiceConfiguration(typeof(ICustomGraphType), Lifecycle = ServiceInstanceScope.Singleton)]
    public class UrlGraphType : ScalarGraphType, ICustomGraphType
    {
        private readonly IUrlResolver _urlResolver;
        public Type TargetType => typeof(Url);

        public UrlGraphType(IUrlResolver urlResolver)
        {
            _urlResolver = urlResolver;

            Name = "Url";
        }

        public override object Serialize(object value)
        {
            return ParseValue(value);
        }

        public override object ParseValue(object value)
        {
            return _urlResolver.GetUrl(((Url)value).ToString());
        }

        public override object ParseLiteral(IValue value)
        {
            return _urlResolver.GetUrl(((Url)value.Value).ToString());
        }
    }
}

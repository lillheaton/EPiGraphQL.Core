using EPiServer.Core;
using EPiServer.ServiceLocation;
using GraphQL.Language.AST;
using GraphQL.Types;
using System;

namespace EPiGraphQL.Core.Types
{
    //https://github.com/graphql-dotnet/graphql-dotnet/issues/787

    [ServiceConfiguration(typeof(ICustomGraphType), Lifecycle = ServiceInstanceScope.Singleton)]
    public class XhtmlStringGraphType : ScalarGraphType, ICustomGraphType
    {
        public Type TargetType => typeof(XhtmlString);

        public XhtmlStringGraphType()
        {
            Name = "XhtmlString";
        }

        public override object ParseLiteral(IValue value)
        {
            return (value.Value as XhtmlString).ToHtmlString();
        }

        public override object ParseValue(object value)
        {
            return ((XhtmlString)value).ToHtmlString();
        }

        public override object Serialize(object value)
        {
            return ParseValue(value);
        }
    }
}

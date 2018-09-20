using EPiServer.SpecializedProperties;
using EPiServer.Web.Routing;
using GraphQL.Types;
using Newtonsoft.Json;

namespace Eols.EPiGraphQL.Cms.Types
{
    public class LinkItemGraphType : ObjectGraphType<LinkItem>
    {
        public LinkItemGraphType(IUrlResolver urlResolver)
        {
            Name = "LinkItem";

            Field<StringGraphType>("Attributes", resolve: x => JsonConvert.SerializeObject(x.Source.Attributes));
            Field<StringGraphType>("Href", resolve: x => urlResolver.GetUrl(x.Source.Href));
            Field(x => x.Target);
            Field(x => x.Text);
            Field(x => x.Title);
        }
    }
}

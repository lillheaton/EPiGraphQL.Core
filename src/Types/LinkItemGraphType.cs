using EPiServer;
using EPiServer.Core;
using EPiServer.SpecializedProperties;
using EPiServer.Web;
using EPiServer.Web.Routing;
using GraphQL.Types;
using Newtonsoft.Json;

namespace Graphify.EPiServer.Core.Types
{
    public class LinkItemGraphType : ObjectGraphType<LinkItem>
    {
        public LinkItemGraphType(IUrlResolver urlResolver, IContentLoader contentLoader, IPermanentLinkMapper permanentLinkMapper)
        {
            Name = "LinkItem";
            
            Field<StringGraphType>("Attributes", resolve: x => JsonConvert.SerializeObject(x.Source.Attributes));
            Field<StringGraphType>("Href", 
                arguments: new QueryArguments(
                    new QueryArgument<BooleanGraphType>
                    {
                        DefaultValue = false,
                        Name = Constants.Arguments.ARGUMENT_ABSOLUTE_URL
                    },
                    new QueryArgument<BooleanGraphType>()
                    {
                        Name = Constants.Arguments.ARGUMENT_ALLOWFALLBACK_LANG,
                        Description = "Allow Fallback Language",
                        DefaultValue = true
                    }
                ),
                resolve: x => 
                {
                    var locale = x.GetLocaleFromArgument();
                    var absoluteUrl = x.GetArgument<bool>(Constants.Arguments.ARGUMENT_ABSOLUTE_URL);
                    var permanentLinkMap = permanentLinkMapper.Find(new UrlBuilder(x.Source.Href));

                    if(permanentLinkMap == null)
                    {
                        return urlResolver.GetUrl(x.Source.Href);
                    }

                    var localizable = contentLoader
                        .Get<IContent>(
                            permanentLinkMap.ContentReference,
                            x.CreateLoaderOptionsFromAgruments()
                        ) as ILocale;

                    if(localizable != null)
                    {
                        return permanentLinkMap.ContentReference.GetUrl(localizable.Language.Name, absoluteUrl);
                    }

                    return urlResolver.GetUrl(x.Source.Href);
                });
            Field(x => x.Target);
            Field(x => x.Text);
            Field(x => x.Title);
        }
    }
}

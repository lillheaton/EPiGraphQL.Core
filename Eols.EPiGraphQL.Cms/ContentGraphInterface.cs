using Eols.EPiGraphQL.Cms.Factory;
using Eols.EPiGraphQL.Core;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using GraphQL.Types;
using System.Linq;

namespace Eols.EPiGraphQL.Cms
{
    [ServiceConfiguration(typeof(IInterfaceGraphType), Lifecycle = ServiceInstanceScope.Singleton)]    
    public class ContentGraphInterface : InterfaceGraphType<IContent>
    {
        public ContentGraphInterface(
            IContentLoader contentLoader,            
            IContentTypeRepository contentTypeRepository)
        {
            var availableTypes = ContentTypeFactory
                .GetAvailableContentTypes(contentTypeRepository)
                .Select(x => x.ModelType)
                .ToArray();            

            Name = "ContentInterface";
            Field(x => x.Name);
            Field(x => x.ContentTypeID);
            Field<IntGraphType>("Id", resolve: x => x.Source.ContentLink.ID);
            Field<IntGraphType>("ParentId", resolve: x => x.Source.ParentLink.ID);
            Field<StringGraphType>("Type", resolve: x => x.Source.GetOriginalType().Name);
            Field<StringGraphType>(
                "Path",
                arguments: new QueryArguments(
                    new QueryArgument<BooleanGraphType>
                    {
                        DefaultValue = false,
                        Name = "absoluteUrl"
                    },
                    new QueryArgument<StringGraphType>
                    {
                        DefaultValue = Constants.Value.DefaultLocale,
                        Name = "locale"
                    }
                ),
                resolve: x =>
                {
                    var locale = x.GetLocaleFromArgumentOrContext();

                    return x.Source.ContentLink.GetUrl(
                        locale.Name, x.GetArgument<bool>("absoluteUrl")
                    );
                });

            Field<ListGraphType<ContentGraphInterface>>(
                "Children",
                arguments: new QueryArguments(
                    new QueryArgument<BooleanGraphType>
                    {
                        DefaultValue = true,
                        Name = "allowFallbackLanguage"
                    }
                ),
                resolve: x =>
                {
                    var locale = x.GetLocaleFromArgumentOrContext();
                    var allowFallback = x.GetArgument<bool>("allowFallbackLanguage");

                    var loaderOptions = new LoaderOptions
                    { 
                        allowFallback
                            ? LanguageLoaderOption.Fallback(locale)
                            : LanguageLoaderOption.Specific(locale)
                    };                    

                    return contentLoader
                            .GetChildren<PageData>(x.Source.ContentLink, loaderOptions)
                            .Where(content =>
                                availableTypes
                                .Contains(content.GetOriginalType())
                            );
                });
        }        
    }
}

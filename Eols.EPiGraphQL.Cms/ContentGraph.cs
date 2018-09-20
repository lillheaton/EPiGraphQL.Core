using Eols.EPiGraphQL.Core;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using GraphQL;
using GraphQL.Types;
using System.Globalization;

namespace Eols.EPiGraphQL.Cms
{
    [ServiceConfiguration(typeof(IEPiServerGraph), Lifecycle = ServiceInstanceScope.Singleton)]
    public class ContentGraph : ObjectGraphType, IEPiServerGraph
    {
        private readonly IContentLoader _contentLoader;

        public ContentGraph(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;

            Name = "Content";

            Field<ContentGraphInterface>(
                "search",
                "Search after content by ContentReferenceID, default => \"current site\" start page",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType>()
                    {
                        Name = "id",
                        DefaultValue = SiteDefinition.Current.StartPage.ID
                    },
                    new QueryArgument<StringGraphType>()
                    {
                        Name = "locale",
                        DefaultValue = "en"
                    }
                ),
                resolve: context =>
                {
                    int id = context.GetArgument<int>("id");
                    string locale = context.GetArgument<string>("locale");

                    if (_contentLoader
                        .TryGet<IContent>(
                            new ContentReference(id),
                            new CultureInfo(locale),
                            out IContent result))
                    {
                        return result;
                    }
                    else
                    {
                        context.Errors.Add(new ExecutionError($"Could not find content with id {id}"));
                        return null;
                    }
                }
            );
        }
    }
}

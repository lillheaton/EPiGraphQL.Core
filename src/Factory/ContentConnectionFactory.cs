using EPiServer;
using EPiServer.Core;
using GraphQL.Builders;
using GraphQL.Types.Relay.DataObjects;
using System.Collections.Generic;
using System.Linq;

namespace Graphify.EPiServer.Core.Factory
{
    public class ContentConnectionFactory
    {
        private readonly IContentLoader _contentLoader;

        public ContentConnectionFactory(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

        private IEnumerable<IContent> GetChildren(
            ContentReference contentReference, 
            LoaderOptions loaderOptions, 
            int startIndex, 
            int maxRows,
            out bool hasNextPage)
        {
            hasNextPage = false;

            var items = _contentLoader
                .GetChildren<IContent>(
                    contentReference,
                    loaderOptions,
                    startIndex,
                    maxRows
                );

            if(items.Count() < maxRows)
            {
                return items;
            }

            hasNextPage = _contentLoader
                .GetChildren<IContent>(
                    contentReference,
                    loaderOptions,
                    startIndex,
                    1
                )
                .Any();

            return items;
        }

        private PageInfo CreatePageInfo(int first, int after, bool hasNextPage)
        {
            int endCursor = first + after;
            int startCursor = after - first;

            return new PageInfo
            {
                StartCursor = (startCursor < 0) ? "0" : startCursor.ToString(),
                EndCursor = endCursor.ToString(),
                HasNextPage = hasNextPage,
                HasPreviousPage = after > 0
            };
        }

        public Connection<IContent> CreateIContentConnection(
            ResolveConnectionContext<IContent> context, 
            LoaderOptions loaderOptions, 
            int defaultPageSize)
        {
            int first = context.PageSize ?? defaultPageSize;
            int.TryParse(context.After, out int after);

            var items = GetChildren(
                context.Source.ContentLink, 
                loaderOptions, 
                after, 
                first, 
                out bool hasNextPage
            );
            
            var pageInfo = CreatePageInfo(first, after, hasNextPage);
            int endCursor = int.Parse(pageInfo.EndCursor);

            var edges = new List<Edge<IContent>>();
            for (int i = 0; i < items.Count(); i++)
            {
                edges.Add(new Edge<IContent> { Node = items.ElementAt(i), Cursor = ((endCursor - first) + i + 1).ToString() });
            }

            return new Connection<IContent>
            {
                TotalCount = items.Count(),
                Edges = edges,
                PageInfo = pageInfo
            };
        }
    }
}

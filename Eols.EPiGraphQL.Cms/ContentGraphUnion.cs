using Eols.EPiGraphQL.Cms.Factory;
using Eols.EPiGraphQL.Core;
using EPiServer;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using GraphQL.Types;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Eols.EPiGraphQL.Cms
{
    //https://github.com/vietnam-devs/crmcore/blob/master/src/modules/crm/CRMCore.Module.GraphQL/Types/TableType.cs
    //https://medium.com/the-graphqlhub/graphql-tour-interfaces-and-unions-7dd5be35de0d

    [ServiceConfiguration(typeof(IEPiServerGraphUnion), Lifecycle = ServiceInstanceScope.Singleton)]
    public class ContentGraphUnion : UnionGraphType, IEPiServerGraphUnion
    {
        public ContentGraphUnion(
            IContentTypeRepository contentTypeRepository,
            IContentLoader contentLoader)
        {
            Name = "ContentUnion";

            var contentGraphInterface = ContentTypeFactory.GetContentGraphInterface();

            var graphs = ContentTypeFactory
                .GetAvailableContentTypes(contentTypeRepository)
                .Select(contentType =>
                    CreateGraphFromType(contentType, contentGraphInterface)
                );

            // Set all graph types to union
            foreach (var graph in graphs)
            {                
                AddPossibleType(graph);
            }
        }

        private static ObjectGraphType CreateGraphFromType(ContentType contentType, IInterfaceGraphType interfaceGraph)
        {
            var graph = new ObjectGraphType();
            graph.Name = contentType.Name;

            // Add all interface fields to graph
            foreach (var field in interfaceGraph.Fields)
            {
                graph.AddField(field);
            }

            // Loop through epi content type properties with display attribute
            var propertiesTuple = contentType
                .ModelType
                .GetPropertiesWithAttribute<DisplayAttribute>();

            // Create fields out of them
            foreach (var tuple in propertiesTuple)
            {
                var propType = tuple.PropertyInfo.PropertyType;
                var displayAttribute = tuple.attribute;

                if (propType == typeof(string))
                {
                    graph.Field<StringGraphType>(propType.Name, displayAttribute.Description);
                }
                if (propType == typeof(int))
                {
                    graph.Field<IntGraphType>(propType.Name, displayAttribute.Description);
                }
            }

            // Method to check if is type
            graph.IsTypeOf = obj =>
            {
                return obj.GetOriginalType() == contentType.ModelType;
            };

            // Add resolved interface to graph type
            graph.AddResolvedInterface(interfaceGraph);
            
            return graph;
        }
    }
}
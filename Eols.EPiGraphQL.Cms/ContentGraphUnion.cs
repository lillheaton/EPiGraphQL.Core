using Eols.EPiGraphQL.Core;
using Eols.EPiGraphQL.Core.Loader;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using GraphQL.Types;
using GraphQL.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Eols.EPiGraphQL.Cms
{
    [ServiceConfiguration(typeof(IEPiServerGraphUnion), Lifecycle = ServiceInstanceScope.Singleton)]
    public class ContentGraphUnion : UnionGraphType, IEPiServerGraphUnion
    {
        public const string NONE_RESOLVED_GRAPH_NAME = "NoneResolvedType";        

        private readonly IInterfaceGraphType _contentInterface;
        private readonly IInterfaceGraphType _localizableInterface;

        public ContentGraphUnion(IContentTypeRepository contentTypeRepository, IServiceLocator serviceLocator)
        {
            Name = "ContentUnion";
            
            _contentInterface = GraphTypeLoader.GetGraphInterface<IContent>(serviceLocator);
            _localizableInterface = GraphTypeLoader.GetGraphInterface<ILocalizable>(serviceLocator);

            var availableTypes = ContentTypeLoader.GetAvailableEpiContentTypes(contentTypeRepository);            

            var blockTypes = availableTypes.Where(IsBlockType);
            var otherTypes = availableTypes.Where(x => IsBlockType(x) == false);
            
            // Create graphs of type Block
            var blockGraphs = CreateGraphs(blockTypes);

            // Add types so we can utilize them on other types (PageData)
            foreach (var graph in blockGraphs)
            {
                AddPossibleType(graph);
            }

            var otherGraphs = 
                CreateGraphs(otherTypes)
                .Concat(
                    new[] { CreateGraphFromType(new ContentType { Name = NONE_RESOLVED_GRAPH_NAME }, _contentInterface) }
                );
            
            foreach (var graph in otherGraphs)
            {                
                AddPossibleType(graph);
            }
        }

        private static bool IsBlockType(ContentType contentType) 
            => typeof(BlockData).IsAssignableFrom(contentType.ModelType);

        /// <summary>
        /// NOTE! Converts block types first so they can be used as local blocks
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ObjectGraphType> CreateGraphs(IEnumerable<ContentType> contentTypes)
        {
            return contentTypes.Select(contentType =>
            {
                if (contentType.ModelType != null && typeof(PageData).IsAssignableFrom(contentType.ModelType))
                {
                    return CreateGraphFromType(contentType, _contentInterface, _localizableInterface);
                }
                return CreateGraphFromType(contentType, _contentInterface);
            });
        }

        private void SetFields(ref ObjectGraphType objectGraph, (PropertyInfo propertyInfo, DisplayAttribute attribute) tuple)
        {
            var propType = tuple.propertyInfo.PropertyType;
            var displayAttribute = tuple.attribute;

            // Check to see if the type is already registred in the GraphTypeRegistry
            var resolvedType = GraphTypeTypeRegistry.Get(propType);
            if (resolvedType != null)
            {
                objectGraph.Field(resolvedType, tuple.propertyInfo.Name, tuple.attribute.Description);
            }

            // Check if it's a Block (IContentData) type
            if (typeof(IContentData).IsAssignableFrom(propType))
            {
                // NOTE! Assumes that all blocks that could be local blocks are already processed and resolved and inserted into the "PossibleTypes"
                var resolvedBlockGraphType = base.PossibleTypes
                    .FirstOrDefault(x => 
                        x.HasMetadata("type") && 
                        ((System.Type)x.Metadata["type"]).Equals(propType)
                    );

                objectGraph.AddField(
                    new FieldType
                    {
                        Name = tuple.propertyInfo.Name,
                        Description = displayAttribute.Description,
                        ResolvedType = resolvedBlockGraphType
                    });                
            }
        }

        private ObjectGraphType CreateGraphFromType(ContentType contentType, params IInterfaceGraphType[] interfaces)
        {
            var graph = new ObjectGraphType();
            graph.Name = contentType.Name;

            if(contentType.ModelType != null)
            {
                graph.Metadata["type"] = contentType.ModelType;
            }

            // Add all interface fields to graph
            foreach (var field in interfaces.SelectMany(x => x.Fields))
            {
                graph.AddField(field);
            }

            // Loop through epi content type properties with display attribute
            var propertiesTuple = contentType
                .ModelType
                ?.GetPropertiesWithAttribute<DisplayAttribute>() 
                ?? new (PropertyInfo PropertyInfo, DisplayAttribute attribute)[] { } ;
            
            // Create fields out of them
            foreach (var tuple in propertiesTuple)
            {
                SetFields(ref graph, tuple);              
            }

            // Method to check if is type
            graph.IsTypeOf = target =>
            {
                bool isTypeOf = target.GetOriginalType() == contentType.ModelType;

                if(!isTypeOf && contentType.ModelType == null)
                {
                    bool hasAnyType = base.PossibleTypes
                        .Any(x =>
                            x.HasMetadata("type") &&
                            x.GetMetadata<Type>("type").Equals(target.GetOriginalType())
                        );

                    if (hasAnyType == false)
                        return true;
                }

                return isTypeOf;
            };

            foreach (var @interface in interfaces)
            {
                // Add resolved interface to graph type
                graph.AddResolvedInterface(@interface);
            }
            
            return graph;
        }        
    }
}
using EPiServer.DataAbstraction;
using Graphify.EPiServer.Core.Attributes;
using GraphQL.Types;
using GraphQL.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Graphify.EPiServer.Core.Factory
{
    public class ObjectGraphTypeFactory
    {
        public delegate void FieldCreationFallback(
            ref ObjectGraphType objectGraph,
            (PropertyInfo propertyInfo, string description) fieldInfo
        );
        
        private string getGraphName(ContentType contentType)
        {
            var attribute = contentType.ModelType?.GetCustomAttributes<GraphTypeAttribute>().FirstOrDefault();

            return attribute == null
                ? contentType.Name
                : string.IsNullOrWhiteSpace(attribute.GraphName)
                    ? contentType.Name
                    : attribute.GraphName;            
        }

        public static string getPropertyName(PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetAttribute<GraphPropertyAttribute>();

            return attribute == null
                ? propertyInfo.Name
                : string.IsNullOrWhiteSpace(attribute.PropertyName)
                    ? propertyInfo.Name
                    : attribute.PropertyName;
        }

        private void AddField(
            ref ObjectGraphType objectGraph,
            (PropertyInfo propertyInfo, string description) tuple,
            FieldCreationFallback fallbackFieldCreator = null)
        {
            var propType = tuple.propertyInfo.PropertyType;

            // Check to see if the type is already registred in the GraphTypeRegistry
            var resolvedType = GraphTypeTypeRegistry.Get(propType);
            string propertyName = getPropertyName(tuple.propertyInfo);

            if (!string.IsNullOrWhiteSpace(propertyName) &&
                resolvedType != null &&
                objectGraph.Fields.Count(x => x.Name == propertyName) < 1)
            {
                objectGraph.Field(resolvedType, propertyName, tuple.description);
            }

            // We did not find the type in GraphTypeTypeRegistery - initiate fallback
            fallbackFieldCreator?.Invoke(ref objectGraph, tuple);
        }

        public void AddPropertiesToGraph(
            ref ObjectGraphType graph,
            ContentType contentType,
            FieldCreationFallback fallbackFieldCreator = null)
        {
            // Loop through epi content type properties with display attribute
            var propertiesTuple = contentType
              .PropertyDefinitions
              .Where(propDefinition => propDefinition.ExistsOnModel && contentType.ModelType.GetProperty(propDefinition.Name) != null)
              .Select(propDefinition => contentType.ModelType.GetProperty(propDefinition.Name))
              .FilterHiddenProperties()
              .Select(propertyInfo =>
              {
                  var attr = propertyInfo.GetCustomAttribute<DisplayAttribute>();
                  return (propertyInfo, attr?.Description);
              });

            // Create fields out of them
            foreach (var tuple in propertiesTuple)
            {
                AddField(ref graph, tuple, fallbackFieldCreator);
            }
        }

        public ObjectGraphType CreateGraphFromType(
            ContentType contentType,
            IEnumerable<IInterfaceGraphType> interfaces,
            Func<object, bool> IsTypeOf)
        {
            var graph = new ObjectGraphType
            {
                Name = getGraphName(contentType),
                Description = contentType.Description,
                ResolvedInterfaces = interfaces,
                IsTypeOf = IsTypeOf
            };

            if (contentType.ModelType != null)
            {
                graph.Metadata["type"] = contentType.ModelType;
            }

            // Add all interface fields to graph
            foreach (var field in interfaces.SelectMany(x => x.Fields))
            {
                graph.AddField(field);
            }

            return graph;
        }
    }
}

using Eols.EPiGraphQL.Core.Attributes;
using EPiServer.DataAbstraction;
using GraphQL.Types;
using GraphQL.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Eols.EPiGraphQL.Core.Factory
{
  public class ObjectGraphTypeFactory
    {
        public delegate void FieldCreationFallback(
            ref ObjectGraphType objectGraph, 
            (PropertyInfo propertyInfo, string description) fieldInfo
        );        
        
        private void AddField(
            ref ObjectGraphType objectGraph, 
            (PropertyInfo propertyInfo, string description) tuple,
            FieldCreationFallback fallbackFieldCreator = null)
        {
            var propType = tuple.propertyInfo.PropertyType;            

            // Check to see if the type is already registred in the GraphTypeRegistry
            var resolvedType = GraphTypeTypeRegistry.Get(propType);
            if (resolvedType != null)
            {
                objectGraph.Field(resolvedType, tuple.propertyInfo.Name, tuple.description);
            }

            // We did not find the type in GraphTypeTypeRegistery - initiate fallback
            fallbackFieldCreator?.Invoke(ref objectGraph, tuple);
        }

        public ObjectGraphType CreateGraphFromType(
            ContentType contentType, 
            IEnumerable<IInterfaceGraphType> interfaces,
            Func<object, bool> IsTypeOf,
            FieldCreationFallback fallbackFieldCreator = null)
        {
            var graph = new ObjectGraphType
            {
                Name = contentType.Name,
                Description = contentType.Description,
                ResolvedInterfaces = interfaces,                
                IsTypeOf = IsTypeOf
            };

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
              .PropertyDefinitions
              .Where(propDefinition => propDefinition.ExistsOnModel)
              .Select(propDefinition => contentType.ModelType.GetProperty(propDefinition.Name))
              .Where(x => x.HasAttribute<GraphHideAttribute>() == false)
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
            
            return graph;
        }        
    }
}

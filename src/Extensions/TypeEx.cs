using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EPiGraphQL
{
    public static class TypeEx
    {
        public static IEnumerable<(PropertyInfo PropertyInfo, TAttribute attribute)> GetPropertiesWithAttribute<TAttribute>(
            this Type type, 
            Func<PropertyInfo, bool> filter = null)
            where TAttribute : Attribute
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => filter?.Invoke(x) ?? true)
                .Where(x => x.HasAttribute<TAttribute>())
                .Select(x => (x, x.GetCustomAttribute<TAttribute>(true)));            
        }

        public static bool HasAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        {
            return type.GetCustomAttributes<TAttribute>(true).Any();
        }

        public static bool HasAttributeWithConditionOrTrue<TAttribute>(this Type type, Func<TAttribute, bool> filter) where TAttribute : Attribute
        {
            var attribute = type.GetCustomAttributes<TAttribute>().FirstOrDefault();
            if (attribute == null)
                return true;

            return filter(attribute);
        }
    }
}

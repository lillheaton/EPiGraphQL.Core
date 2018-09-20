using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Eols.EPiGraphQL
{
    public static class TypeEx
    {
        public static IEnumerable<(PropertyInfo PropertyInfo, TAttribute attribute)> GetPropertiesWithAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetCustomAttributes<TAttribute>(true).Any())
                .Select(x => (x, x.GetCustomAttribute<TAttribute>(true)));            
        }
    }
}
using System;
using System.Linq;
using System.Reflection;

namespace Eols.EPiGraphQL
{
    public static class PropertyInfoEx
    {
        public static bool HasAttribute<TAttribute>(this PropertyInfo propertyInfo) where TAttribute : Attribute
        {
            return propertyInfo.GetCustomAttributes<TAttribute>(true).Any();
        }
    }
}

using Graphify.EPiServer.Core.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Graphify.EPiServer
{
    public static class PropertyInfoEx
    {
        public static TAttribute GetAttribute<TAttribute>(this PropertyInfo propertyInfo) where TAttribute : Attribute
        {
            return propertyInfo.GetCustomAttributes<TAttribute>(true).FirstOrDefault();
        }

        public static bool HasAttribute<TAttribute>(this PropertyInfo propertyInfo) where TAttribute : Attribute
        {
            return propertyInfo.GetCustomAttributes<TAttribute>(true).Any();
        }

        public static bool HasAttributeWithConditionOrTrue<TAttribute>(this PropertyInfo propertyInfo, Func<TAttribute, bool> filter) where TAttribute : Attribute
        {
            var attribute = propertyInfo.GetAttribute<TAttribute>();
            if (attribute == null)
                return true;

            return filter(attribute);
        }

        public static bool HasAttributeWithCondition<TAttribute>(this PropertyInfo propertyInfo, Func<TAttribute, bool> filter, bool noAttributeReturn = false) where TAttribute : Attribute
        {
            var attribute = propertyInfo.GetAttribute<TAttribute>();
            if (attribute == null)
                return noAttributeReturn;

            return filter(attribute);
        }

        public static IEnumerable<PropertyInfo> FilterHiddenProperties(this IEnumerable<PropertyInfo> enumerable)
        {
            return enumerable.Where(x => !PropertyFilter.ShouldFilter(x));
        }
    }
}

using Eols.EPiGraphQL.Core;
using GraphQL.Types;
using System.Globalization;

namespace Eols.EPiGraphQL
{
    public static class ResolveFieldContextEx
    {
        public static CultureInfo GetLocaleFromArgumentOrContext<TSource>(this ResolveFieldContext<TSource> context)
        {
            // Get current context locale argument
            string localeArgument = context.GetArgument<string>("locale") ?? Constants.Value.DefaultLocale;

            // Get parent locale argument
            string parentLocale = context.Variables.ValueFor("locale") as string ?? Constants.Value.DefaultLocale;
            
            return new CultureInfo(
                localeArgument == Constants.Value.DefaultLocale 
                ? parentLocale 
                : localeArgument
            );
        }        
    }
}
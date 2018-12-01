using Eols.EPiGraphQL.Core;
using EPiServer.Core;
using GraphQL.Types;
using System;
using System.Globalization;

namespace Eols.EPiGraphQL
{
    public static class ResolveFieldContextEx
    {
        public const string ARGUMENT_LOCALE = "locale";
        public const string ARGUMENT_ALLOWFALLBACK_LANG = "allowFallbackLanguage";

        public static CultureInfo GetLocaleFromArgument<TSource>(this ResolveFieldContext<TSource> context, bool fallbackToParentContext = true)
        {
            if (!fallbackToParentContext && !context.HasArgument(ARGUMENT_LOCALE))
            {
                throw new ArgumentException($"ResolveFieldContext does not contain any argument \"{ARGUMENT_LOCALE}\"");
            }

            // Get current context locale argument
            string localeArgument = context.GetArgument<string>(ARGUMENT_LOCALE) ?? Constants.Value.DefaultLocale;

            if (!fallbackToParentContext)
            {
                return new CultureInfo(localeArgument);
            }

            // Get parent locale argument
            string parentLocale = context.Variables.ValueFor(ARGUMENT_LOCALE) as string ?? Constants.Value.DefaultLocale;
            
            return new CultureInfo(
                localeArgument == Constants.Value.DefaultLocale 
                ? parentLocale 
                : localeArgument
            );
        }

        public static LoaderOptions CreateLoaderOptionsFromAgruments<TSource>(this ResolveFieldContext<TSource> context)
        {
            if (!context.HasArgument(ARGUMENT_ALLOWFALLBACK_LANG))
            {
                throw new ArgumentException($"ResolveFieldContext does not contain any argument \"{ARGUMENT_ALLOWFALLBACK_LANG}\"");
            }

            var allowFallbackLang = context.GetArgument<bool>(ARGUMENT_ALLOWFALLBACK_LANG);
            var locale = context.GetLocaleFromArgument();

            return new LoaderOptions
            {
                allowFallbackLang
                    ? LanguageLoaderOption.Fallback(locale)
                    : LanguageLoaderOption.Specific(locale)
            };
        }
    }
}
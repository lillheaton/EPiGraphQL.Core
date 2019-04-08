using EPiGraphQL.Core;
using EPiGraphQL.Core.Factory;
using EPiServer.Core;
using GraphQL.Types;
using System;
using System.Globalization;

namespace EPiGraphQL
{
    public static class ResolveFieldContextEx
    {
        /// <summary>
        /// Tries to get current locale and traverse back to parent context to find
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="context"></param>
        /// <param name="fallbackToParentContext"></param>
        /// <returns></returns>
        public static CultureInfo GetLocaleFromArgument<TSource>(this ResolveFieldContext<TSource> context, bool fallbackToParentContext = true)
        {
            if (!fallbackToParentContext && !context.HasArgument(Constants.Arguments.ARGUMENT_LOCALE))
            {
                throw new ArgumentException($"ResolveFieldContext does not contain any argument \"{Constants.Arguments.ARGUMENT_LOCALE}\"");
            }

            // Get current context locale argument
            string localeArgument = context.GetArgument<string>(Constants.Arguments.ARGUMENT_LOCALE) ?? Constants.Value.DefaultLocale;

            if (!fallbackToParentContext)
            {
                return new CultureInfo(localeArgument);
            }

            // Get parent locale argument
            string parentLocale = context.Variables.ValueFor(Constants.Arguments.ARGUMENT_LOCALE) as string ?? Constants.Value.DefaultLocale;
            
            return new CultureInfo(
                localeArgument == Constants.Value.DefaultLocale 
                ? parentLocale 
                : localeArgument
            );
        }

        public static LoaderOptions CreateLoaderOptionsFromAgruments<TSource>(this ResolveFieldContext<TSource> context)
        {
            if (!context.HasArgument(Constants.Arguments.ARGUMENT_ALLOWFALLBACK_LANG))
            {
                throw new ArgumentException($"ResolveFieldContext does not contain any argument \"{Constants.Arguments.ARGUMENT_ALLOWFALLBACK_LANG}\"");
            }

            var allowFallbackLang = context.GetArgument<bool>(Constants.Arguments.ARGUMENT_ALLOWFALLBACK_LANG);
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

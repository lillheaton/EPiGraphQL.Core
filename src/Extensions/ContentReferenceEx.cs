using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System;
using System.Globalization;
using System.Linq;

namespace Graphify.EPiServer
{
    public static class ContentReferenceEx
    {
        private static Injected<IContentLoader> _contentLoader = default(Injected<IContentLoader>);
        private static Injected<IUrlResolver> _urlResolver = default(Injected<IUrlResolver>);
        private static Injected<ISiteDefinitionResolver> _siteDefinitionResolver = default(Injected<ISiteDefinitionResolver>);
        private static Injected<ISiteDefinitionRepository> _siteDefinitionRepository = default(Injected<ISiteDefinitionRepository>);

        public static string GetUrl(
            this ContentReference reference,
            string locale,
            bool absolutUrl)
        {
            var result = _urlResolver.Service.GetUrl(
                reference,
                locale,
                new UrlResolverArguments
                {
                    ContextMode = ContextMode.Default,
                    ForceCanonical = absolutUrl
                });

            if (result == null)
            {
                return string.Empty;
            }

            if (Uri.TryCreate(result, UriKind.RelativeOrAbsolute, out Uri uri))
            {
                if (absolutUrl && uri.IsAbsoluteUri)
                {
                    return uri.AbsoluteUri;
                }
                else if (!absolutUrl)
                {
                    return uri.IsAbsoluteUri ? uri.PathAndQuery : uri.ToString();
                }
            }

            var siteDefinition = _siteDefinitionResolver.Service.GetByContent(reference, true, true);
            
            if (siteDefinition.SiteUrl == null)
            {
                siteDefinition = _siteDefinitionRepository.Service
                    .List()
                    .FirstOrDefault(x => x.SiteUrl != null);
            }

            var hosts = siteDefinition.GetHosts(new CultureInfo(locale), true);

            var host = hosts.FirstOrDefault(h => h.Type == HostDefinitionType.Primary)
                    ?? hosts.FirstOrDefault(h => h.Type == HostDefinitionType.Undefined);

            var basetUri = siteDefinition.SiteUrl;

            if (host != null && host.Name.Equals("*") == false)
            {
                // Try to create a new base URI from the host with the site's URI scheme. Name should be a valid
                // authority, i.e. have a port number if it differs from the URI scheme's default port number.
                Uri.TryCreate(siteDefinition.SiteUrl.Scheme + "://" + host.Name, UriKind.Absolute, out basetUri);
            }
            var absoluteUri = new Uri(basetUri, uri);
            return absoluteUri.AbsoluteUri;
        }
        public static string GetInternalUrl(this ContentReference reference, string locale) => GetUrl(reference, locale, false);
        public static string GetExternalUrl(this ContentReference reference, string locale) => GetUrl(reference, locale, true);
    }
}

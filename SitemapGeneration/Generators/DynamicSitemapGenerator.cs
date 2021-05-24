namespace Blazor.Contentful_.Blog.Starter.SitemapGeneration.Generators
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Api;
    using Blazor.Contentful_.Blog.Starter.Data;
    using Blazor.Contentful_.Blog.Starter.SitemapGeneration.Api;
    using Microsoft.Extensions.Options;

    public class DynamicSitemapGenerator
        : SitemapGenerator
    {
        private readonly ContentfulApi _contentfulApi;
        private readonly SiteConfig _siteConfig;
        private string Cached_XML = string.Empty;

        public int Order => 10;

        public DynamicSitemapGenerator(
            ContentfulApi contentfulApi,
            IOptions<SiteConfig> siteConfigOptions
        )
        {
            _contentfulApi = contentfulApi;
            _siteConfig = siteConfigOptions.Value;
        }

        public Task<bool> BustCache()
        {
            Cached_XML = string.Empty;

            return Task.FromResult(true);
        }

        public async Task<string> Generate()
        {
            if (Cached_XML.IsNullOrWhitespace())
            {
                Cached_XML = await CreateSitemapXml();
            }
            return Cached_XML;
        }

        private async Task<string> CreateSitemapXml()
        {
            var head = string.Join(
                Environment.NewLine,
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
                "<urlset xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd\" xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">"
            );
            var footer = "</urlset>";
            var body = new StringBuilder();
            var urlTemplate = "<url><loc>{{LOC}}</loc><changefreq>daily</changefreq><priority>0.7</priority><lastmod>{{LASTMOD}}</lastmod></url>";

            // Add Home Page
            body.Append(
                urlTemplate.Replace(
                    "{{LOC}}",
                    $"{_siteConfig.GetSiteUrl(_siteConfig.PageMeta.Home.Url)}"
                ).Replace(
                    "{{LASTMOD}}",
                    DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssK")
                )
            );
            // Add Blog Index Page
            body.Append(
                urlTemplate.Replace(
                    "{{LOC}}",
                    $"{_siteConfig.GetSiteUrl(_siteConfig.PageMeta.BlogIndex.Url)}"
                ).Replace(
                    "{{LASTMOD}}",
                    DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssK")
                )
            );

            // Add Blog Posts
            var blogPostSlugs = await _contentfulApi.GetAllPostSlugs();

            foreach (var blogPostSlug in blogPostSlugs)
            {
                body.Append(
                    urlTemplate.Replace(
                        "{{LOC}}",
                        $"{_siteConfig.GetSiteUrl(_siteConfig.PageMeta.BlogIndex.Url)}/{blogPostSlug}"
                    ).Replace(
                        "{{LASTMOD}}",
                        DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssK")
                    )
                );
            }

            // Add Blog Pages
            var totalPosts = blogPostSlugs.Count();
            var totalPages = (int)Math.Ceiling(
                decimal.Divide(totalPosts, _siteConfig.Pagination.PageSize)
            );

            for (var page = 2; page <= totalPages; page++)
            {
                body.Append(
                    urlTemplate.Replace(
                        "{{LOC}}",
                        $"{_siteConfig.GetSiteUrl(_siteConfig.PageMeta.BlogIndex.Url)}/page/{page}"
                    ).Replace(
                        "{{LASTMOD}}",
                        DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssK")
                    )
                );
            }

            return string.Join(
                Environment.NewLine,
                head,
                body,
                footer
            );
        }
    }
}

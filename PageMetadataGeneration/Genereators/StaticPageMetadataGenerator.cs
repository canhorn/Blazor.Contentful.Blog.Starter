namespace Blazor.Contentful_.Blog.Starter.PageMetadataGeneration.Genereators
{
    using System;
    using System.Threading.Tasks;
    using Blazor.Contentful_.Blog.Starter.CacheBusting.Api;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Api;
    using Blazor.Contentful_.Blog.Starter.Data;
    using Blazor.Contentful_.Blog.Starter.Localization;
    using Blazor.Contentful_.Blog.Starter.PageMetadataGeneration.Api;
    using EventHorizon.Blazor.DocumentMetadata.Api;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class StaticPageMetadataGenerator
        : PageMetadataGenerator,
        CacheBuster
    {
        private readonly ILogger<StaticPageMetadataGenerator> _logger;
        private readonly ContentfulApi _api;
        private readonly IDocumentMetadataCollection _registrator;
        private readonly IStringLocalizer<LocalizationResource> _localizer;
        private readonly SiteConfig _siteConfig;

        /// <summary>
        /// The order is high so it is last in the cache busting chain.
        /// </summary>
        public int Order => 99999;

        public StaticPageMetadataGenerator(
            ILogger<StaticPageMetadataGenerator> logger,
            ContentfulApi api,
            IOptions<SiteConfig> siteConfigOptions,
            IDocumentMetadataCollection registrator,
            IStringLocalizer<LocalizationResource> localizer
        )
        {
            _logger = logger;
            _api = api;
            _registrator = registrator;
            _localizer = localizer;
            _siteConfig = siteConfigOptions.Value;
        }

        public async Task<bool> BustCache()
        {
            await Generate(
                _registrator,
                _localizer
            );

            return true;
        }

        public async Task Generate(
            IDocumentMetadataCollection registrator,
            IStringLocalizer<LocalizationResource> localizer
        )
        {
            // Delay 1/2 second, the API has an edge case that when content is saved
            //  and the WebHook starts processing the request it can still get stale data from the API.
            await Task.Delay(500);
            _logger.LogInformation("Generating Document Metadata...");
            registrator.AddDefault()
                .Title(_siteConfig.Site.Title)
                .Link(
                    "alterntive",
                    $"https://{_siteConfig.Site.Domain}/feed.xml",
                    localizer["RSS Feed for {0}", _siteConfig.Site.Domain]!
                        ?? $"RSS Feed For {_siteConfig.Site.Domain}",
                    "application/rss+xml"
                )
                .Meta("twitter:card", "summary_large_image")
                .Meta("twitter:site", $"@{_siteConfig.PageMeta.OpenGraph.TwitterUser}")
                .Meta("twitter:creator", $"@{_siteConfig.PageMeta.OpenGraph.TwitterUser}")
            ;

            void BuildPage(
                string slug,
                string siteConfigTitle,
                string pageTitle,
                string description,
                string url,
                string cononical = ""
            )
            {
                var title = $"{pageTitle} | {siteConfigTitle}";
                var openGraphImage = OpenGraph.GenerateImageUrl(title);

                var page = registrator.AddPage(slug)
                    .Title(title)
                    .Meta("title", title)
                    .OpenGraph("og:title", title)
                    .OpenGraph("twitter:title", title)

                    .Meta("description", description)
                    .OpenGraph("og:description", description)
                    .OpenGraph("twitter:description", description)

                    .OpenGraph("og:url", url)
                    .OpenGraph("twitter:url", url)

                    .OpenGraph("og:image", openGraphImage)
                    .OpenGraph("twitter:image", openGraphImage)
                ;
                if (cononical.IsNotNullOrWhitespace())
                {
                    page.Link("canonical", cononical);
                }
            }


            // Add Not Found Page
            BuildPage(
                "/not-found",
                _siteConfig.Site.Title,
                localizer["Not Found"]!,
                localizer["This page was not found"]!,
                _siteConfig.GetSiteUrl("/not-found")
            );

            // Add Index Page
            var indexResult = await _api.GetPageContentBySlug(
                _siteConfig.PageMeta.Home.Slug
            );
            if (indexResult is not null)
            {
                var slug = _siteConfig.PageMeta.Home.Slug;
                var indexPageContent = indexResult;
                var pageTitle = indexPageContent.Title.IsNotNullOrWhitespace()
                    ? indexPageContent.Title
                    : localizer["Home"]!;
                var pageDescription = indexPageContent.Description.IsNotNullOrWhitespace()
                    ? indexPageContent.Description
                    : localizer["Welcome to the Blazor Contentful blog Starter"]!;

                BuildPage(
                    slug,
                    _siteConfig.Site.Title,
                    pageTitle,
                    pageDescription,
                    _siteConfig.GetSiteUrl(slug)
                );
            }


            // Add Blog Posts
            var posts = await _api.GetAllCachedBlogPosts();
            var totalPosts = 0;
            foreach (var post in posts)
            {
                var slug = $"{_siteConfig.PageMeta.BlogIndex.Slug}/{post.Slug}";
                BuildPage(
                    slug,
                    _siteConfig.Site.Title,
                    post.Title,
                    post.Excerpt,
                    _siteConfig.GetSiteUrl(slug),
                    post.ExternalUrl.IsNotNullOrWhitespace()
                        ? post.ExternalUrl
                        : "false"
                );

                totalPosts += 1;
            }


            // Add Blog Page
            var blogResult = await _api.GetPageContentBySlug(
                _siteConfig.PageMeta.BlogIndex.Slug
            );
            if (blogResult is not null)
            {
                var slug = _siteConfig.PageMeta.BlogIndex.Slug;
                var blogPageContent = blogResult;
                var pageTitle = blogPageContent.Title.IsNotNullOrWhitespace()
                    ? blogPageContent.Title
                    : localizer["Blog"]!
                        ?? "Blog";
                var pageDescription = blogPageContent.Description.IsNotNullOrWhitespace()
                    ? blogPageContent.Description
                    : localizer["Articles | Blazor Contentful blog starter"]!
                        ?? "Articles | Blazor Contentful blog starter";

                BuildPage(
                    slug,
                    _siteConfig.Site.Title,
                    pageTitle,
                    pageDescription,
                    _siteConfig.GetSiteUrl(slug)
                );

                // Add Blog Post Page(s)
                var totalPages = (int)Math.Ceiling(
                    decimal.Divide(totalPosts, _siteConfig.Pagination.PageSize)
                );

                for (var page = 2; page <= totalPages; page++)
                {
                    slug = $"{_siteConfig.PageMeta.BlogIndex.Slug}/page/{page}";
                    BuildPage(
                        slug,
                        _siteConfig.Site.Title,
                        localizer["{0} Page {1}", pageTitle, page]!
                            ?? $"{pageTitle} Page {page}",
                        pageDescription,
                        $"{_siteConfig.GetSiteUrl(slug)}/page/{page}"
                    );
                }
            }
        }
    }
}

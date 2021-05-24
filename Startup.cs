namespace Blazor.Contentful_.Blog.Starter
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Blazor.Contentful_.Blog.Starter.CacheBusting.Api;
    using Blazor.Contentful_.Blog.Starter.CacheBusting.Manual;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Api;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Renderer;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Sdk;
    using Blazor.Contentful_.Blog.Starter.Data;
    using Blazor.Contentful_.Blog.Starter.FeedGeneration.Api;
    using Blazor.Contentful_.Blog.Starter.FeedGeneration.Generators;
    using Blazor.Contentful_.Blog.Starter.Localization;
    using Blazor.Contentful_.Blog.Starter.SitemapGeneration.Api;
    using Blazor.Contentful_.Blog.Starter.SitemapGeneration.Generators;
    using Contentful.AspNetCore;
    using EventHorizon.Blazor.DocumentMetadata;
    using EventHorizon.Blazor.DocumentMetadata.Api;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Options;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();


            services.AddDocumentMetadata(async (serviceProvider, registrator) =>
            {
                System.Console.WriteLine("hi");


                var localizer = serviceProvider.GetRequiredService<IStringLocalizer<LocalizationResource>>();
                var api = serviceProvider.GetRequiredService<ContentfulApi>();
                var siteConfig = serviceProvider.GetRequiredService<IOptions<SiteConfig>>().Value;

                registrator.AddDefault()
                    .Title(siteConfig.Site.Title)
                    .Link(
                        "alterntive",
                        $"https://{siteConfig.Site.Domain}/feed.xml",
                        localizer["RSS Feed for {0}", siteConfig.Site.Domain]
                            ?? $"RSS Feed For {siteConfig.Site.Domain}",
                        "application/rss+xml"
                    )
                    .Meta("twitter:card", "summary_large_image")
                    .Meta("twitter:site", $"@{siteConfig.PageMeta.OpenGraph.TwitterUser}")
                    .Meta("twitter:creator", $"@{siteConfig.PageMeta.OpenGraph.TwitterUser}")
                ;

                void BuildPage(
                    IDocumentMetadataCollection registar,
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
                    registrator,
                    "/not-found",
                    siteConfig.Site.Title,
                    localizer["Not Found"]!,
                    localizer["This page was not found"]!,
                    siteConfig.GetSiteUrl("/not-found")
                );

                // Add Index Page
                var indexResult = await api.GetPageContentBySlug(
                    siteConfig.PageMeta.Home.Slug
                );
                if (indexResult is not null)
                {
                    var slug = siteConfig.PageMeta.Home.Slug;
                    var indexPageContent = indexResult;
                    var pageTitle = indexPageContent.Title.IsNotNullOrWhitespace()
                        ? indexPageContent.Title
                        : localizer["Home"]!;
                    var pageDescription = indexPageContent.Description.IsNotNullOrWhitespace()
                        ? indexPageContent.Description
                        : localizer["Welcome to the Blazor Contentful blog Starter"]!;

                    BuildPage(
                        registrator,
                        slug,
                        siteConfig.Site.Title,
                        pageTitle,
                        pageDescription,
                        siteConfig.GetSiteUrl(slug)
                    );
                }



                // Add Blog Posts
                var posts = await api.GetAllCachedBlogPosts();

                var totalPosts = 0;
                foreach (var post in posts)
                {
                    System.Console.WriteLine($"blog/{post.Slug}");
                    System.Console.WriteLine(post.Title);
                    var slug = $"{siteConfig.PageMeta.BlogIndex.Slug}/{post.Slug}";
                    BuildPage(
                        registrator,
                        slug,
                        siteConfig.Site.Title,
                        post.Title,
                        post.Excerpt,
                        siteConfig.GetSiteUrl(slug),
                        post.ExternalUrl.IsNotNullOrWhitespace()
                            ? post.ExternalUrl
                            : "false"
                    );

                    totalPosts += 1;
                }


                // Add Blog Page
                var blogResult = await api.GetPageContentBySlug(
                    siteConfig.PageMeta.BlogIndex.Slug
                );
                if (blogResult is not null)
                {
                    var slug = siteConfig.PageMeta.BlogIndex.Slug;
                    var blogPageContent = blogResult;
                    var pageTitle = blogPageContent.Title.IsNotNullOrWhitespace()
                        ? blogPageContent.Title
                        : localizer["Blog"]
                            ?? "Blog";
                    var pageDescription = blogPageContent.Description.IsNotNullOrWhitespace()
                        ? blogPageContent.Description
                        : localizer["Articles | Blazor Contentful blog starter"]
                            ?? "Articles | Blazor Contentful blog starter";

                    BuildPage(
                        registrator,
                        slug,
                        siteConfig.Site.Title,
                        pageTitle,
                        pageDescription,
                        siteConfig.GetSiteUrl(slug)
                    );
                    // Add Blog Page(s)
                    var totalPages = (int)Math.Ceiling(
                        decimal.Divide(totalPosts, siteConfig.Pagination.PageSize)
                    );

                    for (var page = 2; page <= totalPages; page++)
                    {
                        slug = $"{siteConfig.PageMeta.BlogIndex.Slug}/page/{page}";
                        BuildPage(
                            registrator,
                            slug,
                            siteConfig.Site.Title,
                            localizer["{0} Page {1}", pageTitle, page]
                                ?? $"{pageTitle} Page {page}",
                            pageDescription,
                            ($"{siteConfig.GetSiteUrl(slug)}/page/{page}")
                        );
                    }
                }
            });

            // Setup Sitemap Services
            services.AddSingleton<SitemapGenerator, DynamicSitemapGenerator>();

            // Setup RSS Feed Services
            services.AddSingleton<RssFeedGenerator, ContentfulRssFeedGenerator>();

            // I18n Services
            // This registers the supported Locales for localization.
            // As more platform locales are supported, Localization Resource files can be added in 
            // ~/Resources/Localization/LocalizationResource-**.resx. 
            services
                .AddLocalization(options => options.ResourcesPath = "Resources")
                .Configure<RequestLocalizationOptions>(
                    opts =>
                    {
                        var supportedCultures = new List<CultureInfo>
                        {
                            // Set Supported Locales
                            new CultureInfo("en-US"),
                        };

                        opts.DefaultRequestCulture = new RequestCulture("en-US");
                        // Formatting numbers, dates, etc.
                        opts.SupportedCultures = supportedCultures;
                        // UI strings that we have localized.
                        opts.SupportedUICultures = supportedCultures;
                    }
                );

            // Setup Site and Contentful Services
            services.Configure<SiteConfig>(Configuration);
            services.AddContentful(Configuration);
            services.AddSingleton<ContentfulApi, SdkContentfulApi>();
            services.AddSingleton<ContentfulHtmlRenderer>();

            // Setup Cache Busting
            services.AddSingleton<CacheBuster, ManualCacheBuster>()
                // These are registered manually, as more services are added that have a cache,
                // they should follow the patterns here.
                .AddTransient<BustCache>(
                    services => services.GetRequiredService<ContentfulApi>()
                ).AddTransient<BustCache>(
                    services => services.GetRequiredService<SitemapGenerator>()
                );
        }

        private int IStringLocalizer<T>()
        {
            throw new System.NotImplementedException();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDocumentMetadata();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();

                // Here we register <domain>/sitemap.xml to return the generated Sitemap
                endpoints.MapGet(
                    "/sitemap.xml",
                    async context =>
                    {
                        await context.Response.WriteAsync(
                            await context.RequestServices
                                .GetRequiredService<SitemapGenerator>()
                                .Generate()
                        );
                    }
                );

                // Here we register <domain>/feed.xml to return the generated RSS Feed
                endpoints.MapGet(
                    "/feed.xml",
                    async context =>
                    {
                        await context.Response.WriteAsync(
                            await context.RequestServices
                                .GetRequiredService<RssFeedGenerator>()
                                .Generate()
                        );
                    }
                );

                // This is our cache busting endpoint that when called will trigger the CacheBuster service.
                // This can be registered in Contentful, so any changes in Contentful will trigger clear the cache.
                endpoints.MapPost(
                    "/webhook/cache-buster",
                    async context =>
                    {
                        await context.RequestServices
                            .GetRequiredService<CacheBuster>()
                            .BustCache();
                        await context.Response.WriteAsync("Ok");
                    }
                );

                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}

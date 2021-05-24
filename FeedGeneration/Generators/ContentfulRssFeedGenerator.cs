namespace Blazor.Contentful_.Blog.Starter.FeedGeneration.Generators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Api;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Model.Blog;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Model.Page;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Renderer;
    using Blazor.Contentful_.Blog.Starter.Data;
    using Blazor.Contentful_.Blog.Starter.FeedGeneration.Api;
    using Microsoft.Extensions.Options;

    public class ContentfulRssFeedGenerator
        : RssFeedGenerator
    {
        private readonly ContentfulApi _contentfulApi;
        private readonly ContentfulHtmlRenderer _contentfulHtmlRenderer;
        private readonly SiteConfig _siteConfig;
        private string Cached_XML = string.Empty;

        public ContentfulRssFeedGenerator(
            ContentfulApi contentfulApi,
            IOptions<SiteConfig> siteConfigOptions,
            ContentfulHtmlRenderer contentfulHtmlRenderer
        )
        {
            _contentfulApi = contentfulApi;
            _contentfulHtmlRenderer = contentfulHtmlRenderer;
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
                Cached_XML = await BuildXML();
            }

            return Cached_XML;
        }

        public async Task<string> BuildXML()
        {
            var siteTitle = _siteConfig.Site.Title;
            var siteDomain = _siteConfig.Site.Domain;
            var siteEmail = _siteConfig.Site.Email;
            var siteOwner = _siteConfig.Site.Owner;
            var feedDescription = _siteConfig.Site.FeedDescription;
            var posts = await _contentfulApi.GetAllBlogPosts();

            return FEED_TEMPLATE.Replace(
                "{{RSS_ITEMS}}",
                await BuildRssItems(
                    posts
                )
            ).Replace(
                "{{SITE_TITLE}}",
                siteTitle
            ).Replace(
                "{{SITE_DOMAIN}}",
                siteDomain
            ).Replace(
                "{{SITE_EMAIL}}",
                siteEmail
            ).Replace(
                "{{SITE_OWNER}}",
                siteOwner
            ).Replace(
                "{{FEED_DESCRIPTION}}",
                feedDescription
            );
        }

        private async Task<string> BuildRssItems(
            IEnumerable<BlogPost> posts
        )
        {
            var postStringList = new List<string>();
            foreach (var post in posts)
            {
                postStringList.Add(
                    POST_TEMPLATE.Replace(
                        "{{POST_TITLE}}",
                        post.Title
                    ).Replace(
                        "{{POST_EXCERPT}}",
                        post.Excerpt
                    ).Replace(
                        "{{POST_SLUG}}",
                        post.Slug
                    ).Replace(
                        "{{POST_DATE}}",
                        post.Date
                    ).Replace(
                        "{{AUTHOR_EMAIL}}",
                        "{{SITE_EMAIL}}"
                    ).Replace(
                        "{{AUTHOR_NAME}}",
                        post.Author?.Name ?? "{{SITE_OWNER}}"
                    ).Replace(
                        "{{POST_TAGS}}",
                        BuildPostTags(post)
                    ).Replace(
                        "{{POST_CONTENT}}",
                        await BuildPostContent(post)
                    )
                );
            }

            return string.Join(
                "",
                postStringList
            );
        }
        private static string BuildPostTags(
            BlogPost post
        )
        {
            return string.Join(
                "",
                post.Tags.Select(
                    tag => POST_TAG.Replace(
                        "{{TAG}}",
                        tag
                    )
                )
            );

        }

        private async Task<string> BuildPostContent(
            BlogPost post
        )
        {
            return POST_CONTENT_TEMPLATE.Replace(
                "{{POST_HTML_CONTENT}}",
                await ContentToString(
                    post.Body
                )
            );
        }

        private async Task<string> ContentToString(
            PageBody? body
        )
        {
            if (body is null)
            {
                return string.Empty;
            }
            return await _contentfulHtmlRenderer.Render(
                body
            );
        }

        private static readonly string FEED_TEMPLATE = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<rss version = ""2.0""
     xmlns:atom=""http://www.w3.org/2005/Atom""
     xmlns:content=""http://purl.org/rss/1.0/modules/content/"">
    <channel>
      <title>{{SITE_TITLE}}</title>
      <atom:link href=""https://{{SITE_DOMAIN}}/feed.xml"" rel=""self"" type=""application/rss+xml"" />
      <link>https://{{SITE_DOMAIN}}</link>
      <description>{{FEED_DESCRIPTION}}</description>
      {{RSS_ITEMS}}
    </channel>
</rss>";

        private static readonly string POST_TEMPLATE = @"
<item>
    <title>{{POST_TITLE}}</title>
    <description>{{POST_EXCERPT}}</description>
    <author>{{AUTHOR_EMAIL}} ({{AUTHOR_NAME}})</author>
    <link>https://{{SITE_DOMAIN}}/blog/{{POST_SLUG}}</link>
    <guid>https://{{SITE_DOMAIN}}/blog/{{POST_SLUG}}</guid>
    <pubDate>{{POST_DATE}}</pubDate>
    {{POST_TAGS}}
    {{POST_CONTENT}}
</item>
";

        private static readonly string POST_CONTENT_TEMPLATE = @"
<content:encoded><![CDATA[
{{POST_HTML_CONTENT}}
]]></content:encoded>
";

        private static readonly string POST_TAG = "<category>{{TAG}}</category>";
    }
}

namespace Blazor.Contentful_.Blog.Starter.ContentfulSdk.Api
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Blazor.Contentful_.Blog.Starter.CacheBusting.Api;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Model;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Model.Blog;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Model.Page;

    public interface ContentfulApi
        : BustCache
    {
        Task<PageContent?> GetPageContentBySlug(
            string slug,
            ContentfulOptions options = default
        );

        Task<(int Total, IEnumerable<BlogPost> Items)> GetPaginatedBlogPosts(
            int page,
            ContentfulOptions options = default
        );

        Task<IEnumerable<string>> GetAllPostSlugs(
            ContentfulOptions options = default
        );

        Task<IEnumerable<BlogPost>> GetAllBlogPosts(
            ContentfulOptions options = default
        );

        Task<IEnumerable<BlogPost>> GetAllCachedBlogPosts(
            ContentfulOptions options = default
        );

        Task<BlogPost?> GetPostBySlug(
            string slug,
            ContentfulOptions options = default
        );

        Task<(int Total, IEnumerable<BlogPost> Items)> GetPaginatedPostSummaries(
            int page,
            ContentfulOptions options = default
        );

        Task<IEnumerable<BlogPost>> GetRecentPostList();

        Task GetTotalPostsNumber();
    }
}

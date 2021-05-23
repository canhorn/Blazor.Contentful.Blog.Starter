namespace Blazor.Contentful.Blog.Starter.Contentful.Api
{
    using Blazor.Contentful.Blog.Starter.Contentful.Model;
    using Blazor.Contentful.Blog.Starter.Contentful.Model.Blog;
    using Blazor.Contentful.Blog.Starter.Contentful.Model.Page;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ContentfulApi
    {
        void BustCache();

        Task<PageContent?> GetPageContentBySlug(
            string slug,
            ContentfulOptions options = default
        );

        Task<(int Total, IEnumerable<BlogPost> Items)> GetPaginatedSlugs(
            int page,
            ContentfulOptions options = default
        );

        Task<IEnumerable<string>> GetAllPostSlugs(
            ContentfulOptions options = default
        );
        
        Task GetPaginatedBlogPosts(
            string slug,
            ContentfulOptions options = default
        );
        
        Task GetAllBlogPosts(
            string slug,
            ContentfulOptions options = default
        );
        
        Task<BlogPost?> GetPostBySlug(
            string slug,
            ContentfulOptions options = default
        );
        
        Task<(int Total, IEnumerable<BlogPost> Items)>  GetPaginatedPostSummaries(
            int page,
            ContentfulOptions options = default
        );
        
        Task<IEnumerable<BlogPost>> GetRecentPostList();
        
        Task GetTotalPostsNumber();

        Task CallContentful(
            string query,
            ContentfulOptions options = default
        );
    }
}

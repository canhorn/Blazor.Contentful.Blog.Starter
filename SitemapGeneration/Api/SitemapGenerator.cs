namespace Blazor.Contentful.Blog.Starter.SitemapGeneration.Api
{
    using System.Threading.Tasks;
    using Blazor.Contentful.Blog.Starter.CacheBusting.Api;

    public interface SitemapGenerator
        : BustCache
    {
        Task<string> Generate();
    }
}

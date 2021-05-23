namespace Blazor.Contentful_.Blog.Starter.SitemapGeneration.Api
{
    using System.Threading.Tasks;
    using Blazor.Contentful_.Blog.Starter.CacheBusting.Api;

    public interface SitemapGenerator
        : BustCache
    {
        Task<string> Generate();
    }
}

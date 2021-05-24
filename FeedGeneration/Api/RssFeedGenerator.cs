namespace Blazor.Contentful_.Blog.Starter.FeedGeneration.Api
{
    using System.Threading.Tasks;
    using Blazor.Contentful_.Blog.Starter.CacheBusting.Api;

    public interface RssFeedGenerator
        : CacheBuster
    {
        Task<string> Generate();
    }
}

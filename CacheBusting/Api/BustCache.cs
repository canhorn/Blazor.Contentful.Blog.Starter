namespace Blazor.Contentful_.Blog.Starter.CacheBusting.Api
{
    using System.Threading.Tasks;

    public interface BustCache
    {
        int Order { get; }
        Task<bool> BustCache();
    }
}

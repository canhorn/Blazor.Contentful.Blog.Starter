namespace Blazor.Contentful_.Blog.Starter.CacheBusting.Api
{
    using System.Threading.Tasks;

    public interface CacheBuster
    {
        Task<bool> BustCache();
    }
}

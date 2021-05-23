namespace Blazor.Contentful.Blog.Starter.SitemapGeneration.Api
{
    using System.Threading.Tasks;

    public interface SitemapGenerator
    {
        Task<string> Generate();
    }
}

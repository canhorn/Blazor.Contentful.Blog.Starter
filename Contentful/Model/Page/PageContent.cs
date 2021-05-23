namespace Blazor.Contentful.Blog.Starter.Contentful.Model.Page
{
    using global::Contentful.Core.Models;

    public class PageContent
    {
        public SystemProperties Sys { get; set; } = new SystemProperties();
        public PageHeroBanner? HeroBanner { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public PageBody? Body { get; set; }
    }
}

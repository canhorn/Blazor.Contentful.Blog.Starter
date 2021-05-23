namespace Blazor.Contentful_.Blog.Starter.ContentfulSdk.Model.Page
{
    using Contentful.Core.Models;

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

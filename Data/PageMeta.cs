namespace Blazor.Contentful_.Blog.Starter.Data
{
    public class PageMeta
    {
        public OpenGraph OpenGraph { get; set; } = new OpenGraph();
        public UrlSlug Home { get; set; } = new UrlSlug();
        public UrlSlug BlogIndex { get; set; } = new UrlSlug();
        public UrlSlug BlogIndexPage { get; set; } = new UrlSlug();
        public UrlSlug Post { get; set; } = new UrlSlug();
        public UrlSlug BuildRss { get; set; } = new UrlSlug();
        public UrlSlug NotFound { get; set; } = new UrlSlug();
    }
}
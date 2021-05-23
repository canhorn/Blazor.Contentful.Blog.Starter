namespace Blazor.Contentful_.Blog.Starter.ContentfulSdk.Model.Page
{
    using Contentful.Core.Models;

    public class PageHeroBanner
    {
        public string Headline { get; set; } = string.Empty;
        public string SubHeading { get; set; } = string.Empty;
        public string InternalLink { get; set; } = string.Empty;
        public string ExternalLink { get; set; } = string.Empty;
        public string CtaText { get; set; } = string.Empty;
        public Asset? Image { get; set; }
    }
}
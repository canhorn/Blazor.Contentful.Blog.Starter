namespace Blazor.Contentful_.Blog.Starter.Data
{
    using System.Collections.Generic;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Model;

    public class SiteConfig
    {
        public ContentfulConfig ContentfulOptions { get; set; } = new ContentfulConfig();
        public string SITE_URL { get; set; } = string.Empty;
        public SiteDetails Site { get; set; } = new SiteDetails();
        public PageMeta PageMeta { get; set; } = new PageMeta();
        public Pagination Pagination { get; set; } = new Pagination();
        public List<MenuLink> MenuLinks { get; set; } = new List<MenuLink>();

        public string GetSiteUrl(
            string urlPath
        ) => $"{SITE_URL}{urlPath}";
    }
}

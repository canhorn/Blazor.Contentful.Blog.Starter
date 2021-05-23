namespace Blazor.Contentful.Blog.Starter.Data
{
    using System.Collections.Generic;

    public class SiteConfig
    {
        public SiteDetails Site { get; set; } = new SiteDetails();
        public PageMeta PageMeta { get; set; } = new PageMeta();
        public Pagination Pagination { get; set; } = new Pagination();
        public List<MenuLink> MenuLinks { get; set; } = new List<MenuLink>();
    }
}

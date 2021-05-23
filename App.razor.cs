namespace Blazor.Contentful_.Blog.Starter
{
    using Blazor.Contentful_.Blog.Starter.Data;
    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.Options;

    public class AppBase
        : ComponentBase
    {
        [Inject]
        IOptions<SiteConfig> SiteConfigOptions { get; set; } = null!;

        public SiteConfig SiteConfig { get; set; } = new SiteConfig();

        protected override void OnInitialized()
        {
            SiteConfig = SiteConfigOptions.Value;
        }
    }
}

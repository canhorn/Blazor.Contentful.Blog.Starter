namespace Blazor.Contentful_.Blog.Starter.Pages
{
    using System.Threading.Tasks;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Model.Page;
    using Blazor.Contentful_.Blog.Starter.Data;
    using Blazor.Contentful_.Blog.Starter.Shared.Components;
    using Microsoft.AspNetCore.Components;

    public class IndexBase
        : StandardComponentBase
    {
        [CascadingParameter]
        public SiteConfig Config { get; set; } = null!;

        [Inject]
        public ContentfulSdk.Api.ContentfulApi ContentfulApi { get; set; } = null!;

        public PageContent PageContent { get; set; } = new PageContent();


        protected override async Task OnInitializedAsync()
        {
            var result = await ContentfulApi.GetPageContentBySlug(
                Config.PageMeta.Home.Slug
            );
            if (result is not null)
            {
                PageContent = result;
            }
        }

    }
}

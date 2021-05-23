namespace Blazor.Contentful.Blog.Starter.Pages
{
    using System.Threading.Tasks;
    using Blazor.Contentful.Blog.Starter.Contentful.Model.Page;
    using Blazor.Contentful.Blog.Starter.Data;
    using Blazor.Contentful.Blog.Starter.Shared.Components;
    using Microsoft.AspNetCore.Components;

    public class IndexBase
        : StandardComponentBase
    {
        [CascadingParameter]
        public SiteConfig Config { get; set; } = null!;

        [Inject]
        public Contentful.Api.ContentfulApi ContentfulApi { get; set; } = null!;

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

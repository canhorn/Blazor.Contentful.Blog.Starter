namespace Blazor.Contentful.Blog.Starter.Pages.Blog
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Blazor.Contentful.Blog.Starter.Contentful.Model.Blog;
    using Blazor.Contentful.Blog.Starter.Contentful.Model.Page;
    using Blazor.Contentful.Blog.Starter.Data;
    using Blazor.Contentful.Blog.Starter.Shared.Components;
    using Microsoft.AspNetCore.Components;

    public class PostPageBase
        : StandardComponentBase
    {
        [CascadingParameter]
        public SiteConfig Config { get; set; } = null!;

        [Parameter]
        public string Slug { get; set; } = string.Empty;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public Contentful.Api.ContentfulApi ContentfulApi { get; set; } = null!;

        public BlogPost? Post { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var postResult = await ContentfulApi.GetPostBySlug(
               Slug
           );
            if (postResult is null)
            {
                NavigationManager.NavigateTo("/not-found");
                return;
            }

            Post = postResult;
        }

    }
}

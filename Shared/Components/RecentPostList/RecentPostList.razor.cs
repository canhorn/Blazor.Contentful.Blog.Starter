namespace Blazor.Contentful_.Blog.Starter.Shared.Components.RecentPostList
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Api;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Model.Blog;
    using Blazor.Contentful_.Blog.Starter.Data;
    using Microsoft.AspNetCore.Components;

    public class RecentPostListBase
        : StandardComponentBase
    {
        [CascadingParameter]
        public SiteConfig Config { get; set; } = null!;

        [Inject]
        public ContentfulApi ContentfulApi { get; set; } = null!;

        public IEnumerable<BlogPost> Posts { get; set; } = new List<BlogPost>();

        protected override async Task OnInitializedAsync()
        {
            Posts = await ContentfulApi.GetRecentPostList();
        }

    }
}

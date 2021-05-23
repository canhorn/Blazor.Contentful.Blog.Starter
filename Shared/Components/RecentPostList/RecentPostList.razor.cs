namespace Blazor.Contentful.Blog.Starter.Shared.Components.RecentPostList
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Blazor.Contentful.Blog.Starter.Contentful.Api;
    using Blazor.Contentful.Blog.Starter.Contentful.Model.Blog;
    using Blazor.Contentful.Blog.Starter.Data;
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

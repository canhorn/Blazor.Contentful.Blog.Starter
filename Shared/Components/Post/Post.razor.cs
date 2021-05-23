namespace Blazor.Contentful.Blog.Starter.Shared.Components.Post
{
    using System.Reflection.Metadata;
    using Blazor.Contentful.Blog.Starter.Contentful.Model.Blog;
    using Microsoft.AspNetCore.Components;

    public class PostBase
        : StandardComponentBase
    {
        [Parameter]
        public BlogPost Post { get; set; } = null!;
    }
}

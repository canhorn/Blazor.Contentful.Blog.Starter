namespace Blazor.Contentful_.Blog.Starter.Shared.Components.Post
{
    using System.Reflection.Metadata;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Model.Blog;
    using Microsoft.AspNetCore.Components;

    public class PostBase
        : StandardComponentBase
    {
        [Parameter]
        public BlogPost Post { get; set; } = null!;
    }
}

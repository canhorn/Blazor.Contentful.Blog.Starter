namespace Blazor.Contentful.Blog.Starter.Shared.Components.RichTextPageContent
{
    using System.Threading.Tasks;
    using Blazor.Contentful.Blog.Starter.Contentful.Model.Page;
    using global::Contentful.Core.Models;
    using Microsoft.AspNetCore.Components;

    public partial class RichTextPageContent
        : ComponentBase
    {
        [Parameter]
        public PageBody? Body { get; set; }

        public string Content { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            if (Body is not null)
            {
                Content = await new HtmlRenderer().ToHtml(Body);
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Body is not null)
            {
                Content = await new HtmlRenderer().ToHtml(Body);
            }
        }
    }
}

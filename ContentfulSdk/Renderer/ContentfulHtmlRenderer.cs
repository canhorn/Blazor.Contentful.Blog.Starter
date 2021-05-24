namespace Blazor.Contentful_.Blog.Starter.ContentfulSdk.Renderer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Model.Page;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Renderer.Renderers;
    using Contentful.Core.Models;

    public class ContentfulHtmlRenderer
    {
        private readonly HtmlRenderer _renderer;

        public ContentfulHtmlRenderer()
        {
            _renderer = new HtmlRenderer();
            _renderer.AddRenderer(
                new ContentfulBlocksEmbeddedEntryRender()
            );
        }

        public async Task<string> Render(
            PageBody body
        ) => await _renderer.ToHtml(
            body
        );
    }
}

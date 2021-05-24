namespace Blazor.Contentful_.Blog.Starter.ContentfulSdk.Renderer.Renderers
{
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Contentful.Core.Models;

    public class ContentfulBlocksEmbeddedEntryRender
        : IContentRenderer
    {
        public int Order
        {
            get;
            set;
        } = 100;

        public bool SupportsContent(IContent content)
        {
            return content is EntryStructure;
        }

        public string Render(IContent content)
        {
            var entryStructure = content as EntryStructure;
            var stringBuilder = new StringBuilder();
            if (entryStructure?.Data.Target is not CustomNode target)
            {
                return string.Empty;
            }

            var targetTyped = target.JObject.ToObject<CustomNodeTyped>();
            var contentType = targetTyped.Sys.ContentType.SystemProperties.Id;

            switch (contentType)
            {
                case "videoEmbed":
                    var title = targetTyped.Title;
                    var embedUrl = targetTyped.EmbedUrl;
                    stringBuilder.Append("<div class=\"video-embed\"><iframe class=\"video-embed__iframe\" ")
                        .Append(" src=\"").Append(embedUrl).Append('"')
                        .Append(" height=\"100%\"")
                        .Append(" width=\"100%\"")
                        .Append(" frameBoarder=\"0\"")
                        .Append(" scrolling=\"no\"")
                        .Append(" allowFullScreen=\"true\"")
                        .Append(" title=\"").Append(title).Append('"')
                        .Append("></iframe></div>");
                    break;
                case "codeBlock":
                    var language = targetTyped.Language;
                    var code = targetTyped.Code;
                    stringBuilder.Append("<pre class=\"code-block\"><code class=\"code-block__inner ")
                        .Append("language-").Append(language)
                        .Append("\">")
                        .Append(WebUtility.HtmlEncode(code))
                        .Append("</code></pre>");
                    break;
                default:
                    break;
            }

            return stringBuilder.ToString();
        }

        public Task<string> RenderAsync(IContent content)
        {
            return Task.FromResult(Render(content));
        }

        public class CustomNodeTyped
        {
            public SystemProperties Sys { get; set; } = new SystemProperties();
            public string Title { get; set; } = string.Empty;
            public string EmbedUrl { get; set; } = string.Empty;
            public string Language { get; set; } = string.Empty;
            public string Code { get; set; } = string.Empty;
        }
    }
}

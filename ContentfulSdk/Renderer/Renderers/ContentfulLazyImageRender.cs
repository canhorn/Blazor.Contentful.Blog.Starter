namespace Blazor.Contentful_.Blog.Starter.ContentfulSdk.Renderer.Renderers
{
    using System.Text;
    using System.Threading.Tasks;
    using Contentful.Core.Models;

    public class ContentfulLazyImageRender
        : IContentRenderer
    {
        public int Order
        {
            get;
            set;
        } = 50;

        public bool SupportsContent(IContent content)
        {
            if (content is not AssetStructure assetStructure)
            {
                return false;
            }

            var target = assetStructure.Data.Target;
            var nodeType = assetStructure.NodeType;
            return nodeType != "asset-hyperlink"
                && target.File?.ContentType != null
                && target.File.ContentType.ToLower().Contains("image");
        }

        public string Render(IContent content)
        {
            var assetStructure = content as AssetStructure;
            var target = assetStructure?.Data.Target;
            var nodeType = assetStructure?.NodeType;
            var stringBuilder = new StringBuilder();
            if (nodeType != "asset-hyperlink" 
                && target?.File?.ContentType != null 
                && target.File.ContentType.ToLower().Contains("image")
            )
            {
                stringBuilder.Append("<img class=\"lazy\" src=\"/images/placeholder-image.png\" data-src=\"" + target.File.Url + "\" alt=\"" + target.Title + "\" ");

                stringBuilder.Append(" data-srcset=\"");
                stringBuilder.Append(ContentfulImageSourceSetGeneration.CreateSourceSet(target.File.Url, "75"));
                stringBuilder.Append('"');

                stringBuilder.Append(" />");
            }

            return stringBuilder.ToString();
        }

        public Task<string> RenderAsync(IContent content)
        {
            return Task.FromResult(Render(content));
        }
    }
}

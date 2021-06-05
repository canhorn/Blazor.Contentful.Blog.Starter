namespace Blazor.Contentful_.Blog.Starter.Shared.Components.Image
{
    using System.Collections.Generic;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Renderer;
    using Microsoft.AspNetCore.Components;

    public partial class Image : ComponentBase
    {
        [Parameter]
        public string @class { get; set; } = string.Empty;
        [Parameter]
        public string src { get; set; } = string.Empty;
        [Parameter]
        public string decoding { get; set; } = "async";
        [Parameter]
        public string loading { get; set; } = "lazy";
        [Parameter]
        public string srcset { get; set; } = string.Empty;

        [Parameter]
        public int ImageQuality { get; set; } = 75;
        [Parameter]
        public bool LazyLoad { get; set; } = true;

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> Attributes { get; set; } = null!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (srcset.IsNullOrWhitespace()
                && ContentfulImageSourceSetGeneration.IsContentfulImage(
                    src
                )
            )
            {
                srcset = ContentfulImageSourceSetGeneration.CreateSourceSet(
                    src,
                    ImageQuality.ToString()
                );
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (srcset.IsNullOrWhitespace()
                && ContentfulImageSourceSetGeneration.IsContentfulImage(
                    src
                )
            )
            {
                srcset = ContentfulImageSourceSetGeneration.CreateSourceSet(
                    src,
                    ImageQuality.ToString()
                );
            }
        }
    }
}

namespace Blazor.Contentful_.Blog.Starter.ContentfulSdk.Renderer
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ContentfulImageSourceSetGeneration
    {
        public static bool IsContentfulImage(
            string src
        ) => src.Contains(
            "images.ctfassets.net"
        );

        public static string CreateSourceSet(
            string imageSrc,
            string imageQuality
        )
        {
            var sourceSets = string.Empty;
            var template = "{image}?w={width}&q={quality} {setWidth}w";
            var sourceSetValues = new List<(string image, string width, string quality, string setWidth)>
            {
                (imageSrc, "640", imageQuality, "640"),
                (imageSrc, "750", imageQuality, "750"),
                (imageSrc, "828", imageQuality, "828"),
                (imageSrc, "1080", imageQuality, "1080"),
                (imageSrc, "1200", imageQuality, "1200"),
                (imageSrc, "1920", imageQuality, "1920"),
                (imageSrc, "2048", imageQuality, "2048"),
                (imageSrc, "3840", imageQuality, "3840"),
            };

            return string.Join(
                ",",
                sourceSetValues.Select(
                    a => template.Replace(
                        "{image}", a.image
                    ).Replace(
                        "{width}", a.width
                    ).Replace(
                        "{quality}", a.quality
                    ).Replace(
                        "{setWidth}", a.setWidth
                    )
                )
            );
        }
    }
}

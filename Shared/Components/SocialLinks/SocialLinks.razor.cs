namespace Blazor.Contentful_.Blog.Starter.Shared.Components.SocialLinks
{
    using System;
    using System.Collections.Generic;
    using Blazor.Contentful_.Blog.Starter.Data;
    using Blazor.Contentful_.Blog.Starter.Shared.Components.SocialLinks.svg;
    using Microsoft.AspNetCore.Components;

    public class SocialLinksBase
        : StandardComponentBase
    {
        [CascadingParameter]
        public SiteConfig Config { get; set; } = null!;

        [Parameter]
        public string FillColor { get; set; } = string.Empty;

        public List<SocialLinkDetails> SocialLinkList = new();

        protected override void OnInitialized()
        {
            SocialLinkList = new List<SocialLinkDetails>
            {
                new SocialLinkDetails
                {
                    Name = Localizer["Twitter"]!,
                    Url = $"https://twitter.com/{Config.PageMeta.OpenGraph.TwitterUser}",
                    AriaLabel = Localizer["Follow me on Twitter"]!,
                    SVG = typeof(Twitter),
                },
                new SocialLinkDetails
                {
                    Name = Localizer["RSS Feed"]!,
                    Url = "feed.xml",
                    AriaLabel = Localizer["View the RSS feed of {0}", Config.Site.Domain]!,
                    SVG = typeof(Feed),
                }
            };
        }

        public struct SocialLinkDetails
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public string AriaLabel { get; set; }
            public Type SVG { get; set; }
        }
    }
}

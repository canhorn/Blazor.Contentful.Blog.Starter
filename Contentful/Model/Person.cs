namespace Blazor.Contentful.Blog.Starter.Contentful.Model
{
    using global::Contentful.Core.Models;

    public class Person
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TwitterUsername { get; set; } = string.Empty;
        public string GitHubUsername { get; set; } = string.Empty;
        public string TwitchUsername { get; set; } = string.Empty;
        public string WebsiteUrl { get; set; } = string.Empty;

        public Asset? Image { get; set; }
    }
}
﻿namespace Blazor.Contentful.Blog.Starter.Contentful.Model.Blog
{
    using System.Collections.Generic;
    using Blazor.Contentful.Blog.Starter.Contentful.Model.Page;
    using global::Contentful.Core.Models;

    public class BlogPost
    {
        public SystemProperties Sys { get; set; } = new SystemProperties();
        public string Date { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
        public string ExternalUrl { get; set; } = string.Empty;
        public Person? Author { get; set; } 

        public PageBody? Body { get; set; }
    }
}

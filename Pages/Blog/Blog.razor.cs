﻿namespace Blazor.Contentful.Blog.Starter.Pages.Blog
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Blazor.Contentful.Blog.Starter.Contentful.Model.Blog;
    using Blazor.Contentful.Blog.Starter.Contentful.Model.Page;
    using Blazor.Contentful.Blog.Starter.Data;
    using Blazor.Contentful.Blog.Starter.Shared.Components;
    using Microsoft.AspNetCore.Components;

    public class BlogBase
        : StandardComponentBase
    {
        [CascadingParameter]
        public SiteConfig Config { get; set; } = null!;

        [Inject]
        public Contentful.Api.ContentfulApi ContentfulApi { get; set; } = null!;

        public PageContent PageContent { get; set; } = new PageContent();

        public int TotalPages { get; set; }
        public IEnumerable<BlogPost> PostSummaries { get; set; } = new List<BlogPost>();
        public int CurrentPage { get; set; } = 1;


        protected override async Task OnInitializedAsync()
        {
            var result = await ContentfulApi.GetPageContentBySlug(
                Config.PageMeta.BlogIndex.Slug
            );
            if (result is not null)
            {
                PageContent = result;
            }
            var (Total, Items) = await ContentfulApi.GetPaginatedPostSummaries(
                1
            );
            TotalPages = (int)Math.Ceiling(
                decimal.Divide(Total, Config.Pagination.PageSize)
            );
            PostSummaries = Items;
        }

    }
}
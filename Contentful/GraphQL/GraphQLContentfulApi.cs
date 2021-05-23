namespace Blazor.Contentful.Blog.Starter.Contentful.GraphQL
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Blazor.Contentful.Blog.Starter.Contentful.Api;
    using Blazor.Contentful.Blog.Starter.Contentful.Model;
    using Blazor.Contentful.Blog.Starter.Contentful.Model.Blog;
    using Blazor.Contentful.Blog.Starter.Contentful.Model.Page;
    using Blazor.Contentful.Blog.Starter.Data;
    using global::Contentful.Core;
    using global::Contentful.Core.Search;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class GraphQLContentfulApi
        : ContentfulApi
    {
        private readonly ILogger<GraphQLContentfulApi> _logger;
        private readonly IContentfulClient _client;
        private readonly SiteConfig _siteConfig;

        public GraphQLContentfulApi(
            ILogger<GraphQLContentfulApi> logger,
            IContentfulClient client,
            IOptions<SiteConfig> siteConfigOptions
        )
        {
            _logger = logger;
            _client = client;
            _siteConfig = siteConfigOptions.Value;
        }

        public void BustCache()
        {

        }

        public Task CallContentful(string query, ContentfulOptions options = default)
        {
            throw new NotImplementedException();
        }

        public Task GetAllBlogPosts(string slug, ContentfulOptions options = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> GetAllPostSlugs(
            ContentfulOptions options = default
        )
        {
            try
            {
                var slugList = new List<string>();
                var page = 1;
                var shouldQueryMoreSlugs = true;

                while (shouldQueryMoreSlugs)
                {
                    var response = await this.GetPaginatedSlugs(
                        page
                    );
                    slugList.AddRange(
                        response.Items.Select(
                            a => a.Slug
                        )
                    );

                    shouldQueryMoreSlugs = slugList.Count < response.Total;
                }


                return slugList;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to get Slug List."
                );
                return new List<string>();
            }
        }

        private static readonly ConcurrentDictionary<string, PageContent> GetPageContentBySlugCache = new();

        public async Task<PageContent?> GetPageContentBySlug(
            string slug,
            ContentfulOptions options = default
        )
        {
            try
            {
                var queryBuilder = QueryBuilder<PageContent>.New
                    .ContentTypeIs("pageContent")
                    .FieldEquals(f => f.Slug, slug)
                    .Limit(1)
                    .Include(5);

                // Check Cache
                var queryCacheKey = queryBuilder.Build();
                if (GetPageContentBySlugCache.TryGetValue(
                    queryCacheKey,
                    out var cached
                ))
                {
                    Console.WriteLine("Cache Hit");
                    return cached;
                }

                // Not found in Cache, look up new based on query
                var result = await _client.GetEntries(
                    queryBuilder
                );

                var fetched = result.Items.FirstOrDefault();
                if (fetched is not null)
                {
                    // Was a Valid Fetch, cache it for later.
                    GetPageContentBySlugCache.AddOrUpdate(
                        queryCacheKey,
                        fetched,
                        (_, _) => fetched
                    );
                }

                return fetched;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to get Content for {Slug}",
                    slug
                );
                return null;
            }
        }

        public Task GetPaginatedBlogPosts(string slug, ContentfulOptions options = default)
        {
            throw new NotImplementedException();
        }

        private static readonly ConcurrentDictionary<string, (int Total, IEnumerable<BlogPost> Items)> GetPaginatedPostSummariesCache = new();

        public async Task<(int Total, IEnumerable<BlogPost> Items)> GetPaginatedPostSummaries(
            int page,
            ContentfulOptions options = default
        )
        {
            try
            {
                var skipMultiplier = page == 1 ? 0 : page - 1;
                var skip = skipMultiplier > 0
                    ? _siteConfig.Pagination.PageSize * skipMultiplier
                    : 0;

                var queryBuilder = QueryBuilder<BlogPost>.New
                    .ContentTypeIs(
                        "blogPost"
                    ).Limit(
                        _siteConfig.Pagination.PageSize
                    ).Skip(
                        skip
                    ).OrderBy(
                        SortOrderBuilder<BlogPost>.New(
                            a => a.Date,
                            SortOrder.Reversed
                        ).Build()
                    );

                // Check Cache
                var queryCacheKey = queryBuilder.Build();
                if (GetPaginatedPostSummariesCache.TryGetValue(
                    queryCacheKey,
                    out var cached
                ))
                {
                    Console.WriteLine("Cache Hit");
                    return cached;
                }

                // Not found in Cache, look up new based on query
                var result = await _client.GetEntries(
                    queryBuilder
                );

                var fetched = (result.Total, result.Items);

                // Cache it for later.
                GetPaginatedPostSummariesCache.AddOrUpdate(
                    queryCacheKey,
                    fetched,
                    (_, _) => fetched
                );

                return fetched;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to get Content for 'blogPost'"
                );
                return (0, new List<BlogPost>());
            }
        }

        private static readonly ConcurrentDictionary<string, (int Total, IEnumerable<BlogPost> Items)> GetPaginatedSlugsCache = new();

        public async Task<(int Total, IEnumerable<BlogPost> Items)> GetPaginatedSlugs(
            int page,
            ContentfulOptions options = default
        )
        {
            try
            {
                var queryLimit = 100;
                var skipMultiplier = page == 1 ? 0 : page - 1;
                var skip = skipMultiplier > 0
                    ? queryLimit * skipMultiplier
                    : 0;

                var queryBuilder = QueryBuilder<BlogPost>.New
                    .ContentTypeIs(
                        "blogPost"
                    ).Limit(
                        queryLimit
                    ).Skip(
                        skip
                    ).OrderBy(
                        SortOrderBuilder<BlogPost>.New(
                            a => a.Date,
                            SortOrder.Reversed
                        ).Build()
                    );

                // Check Cache
                var queryCacheKey = queryBuilder.Build();
                if (GetPaginatedSlugsCache.TryGetValue(
                    queryCacheKey,
                    out var cached
                ))
                {
                    return cached;
                }

                // Not found in Cache, look up new based on query
                var result = await _client.GetEntries(
                    queryBuilder
                );

                var fetched = (result.Total, result.Items);

                // Cache it for later.
                GetPaginatedSlugsCache.AddOrUpdate(
                    queryCacheKey,
                    fetched,
                    (_, _) => fetched
                );

                return fetched;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to get Content for 'blogPost'"
                );
                return (0, new List<BlogPost>());
            }
        }

        private static ConcurrentDictionary<string, BlogPost> GetPostBySlugCache = new();

        public async Task<BlogPost?> GetPostBySlug(
            string slug,
            ContentfulOptions options = default
        )
        {
            try
            {
                var queryBuilder = QueryBuilder<BlogPost>.New
                    .ContentTypeIs(
                        "blogPost"
                    ).FieldEquals(
                        a => a.Slug,
                        slug
                    ).Limit(
                        1
                    );

                // Check Cache
                var queryCacheKey = queryBuilder.Build();
                if (GetPostBySlugCache.TryGetValue(
                    queryCacheKey,
                    out var cached
                ))
                {
                    Console.WriteLine("Cache Hit");
                    return cached;
                }

                // Not found in Cache, look up new based on query
                var result = await _client.GetEntries(
                    queryBuilder
                );

                var fetched = result.Items.FirstOrDefault();
                if (fetched is not null)
                {
                    // Was a Valid Fetch, cache it for later.
                    GetPostBySlugCache.AddOrUpdate(
                        queryCacheKey,
                        fetched,
                        (_, _) => fetched
                    );
                }

                return fetched;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to get Content for 'blogPost'"
                );
                return null;
            }
        }

        public async Task<IEnumerable<BlogPost>> GetRecentPostList()
        {
            try
            {
                var queryBuilder = QueryBuilder<BlogPost>.New
                    .ContentTypeIs(
                        "blogPost"
                    ).Limit(
                        _siteConfig.Pagination.RecentPostsSize
                    ).OrderBy(
                        SortOrderBuilder<BlogPost>.New(
                            a => a.Date, SortOrder.Reversed
                        ).Build()
                    );

                var result = await _client.GetEntries(
                    queryBuilder
                );

                return result.Items;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to get Content for 'blogPost'"
                );
                return new List<BlogPost>();
            }
        }

        public Task GetTotalPostsNumber()
        {
            throw new NotImplementedException();
        }
    }
}

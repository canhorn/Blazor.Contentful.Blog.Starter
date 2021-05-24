namespace Blazor.Contentful_.Blog.Starter.ContentfulSdk.Sdk
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Api;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Model;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Model.Blog;
    using Blazor.Contentful_.Blog.Starter.ContentfulSdk.Model.Page;
    using Blazor.Contentful_.Blog.Starter.Data;
    using Contentful.Core;
    using Contentful.Core.Search;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class SdkContentfulApi
        : ContentfulApi
    {
        private static readonly ConcurrentDictionary<string, PageContent> GetPageContentBySlugCache = new();
        private static readonly ConcurrentDictionary<string, (int Total, IEnumerable<BlogPost> Items)> GetPaginatedPostSummariesCache = new();
        private static readonly ConcurrentDictionary<string, (int Total, IEnumerable<BlogPost> Items)> GetPaginatedSlugsCache = new();
        private static readonly ConcurrentDictionary<string, BlogPost> GetPostBySlugCache = new();
        public ConcurrentBag<BlogPost> GetAllCachedBlogPostsCache = new();

        private readonly ILogger<SdkContentfulApi> _logger;
        private readonly IContentfulClient _client;
        private readonly SiteConfig _siteConfig;

        public int Order => 1;

        public SdkContentfulApi(
            ILogger<SdkContentfulApi> logger,
            IContentfulClient client,
            IOptions<SiteConfig> siteConfigOptions
        )
        {
            _logger = logger;
            _client = client;
            _siteConfig = siteConfigOptions.Value;
        }

        public Task<bool> BustCache()
        {
            GetPageContentBySlugCache.Clear();
            GetPaginatedPostSummariesCache.Clear();
            GetPaginatedSlugsCache.Clear();
            GetPostBySlugCache.Clear();
            GetAllCachedBlogPostsCache.Clear();

            return Task.FromResult(true);
        }

        public async Task<IEnumerable<BlogPost>> GetAllBlogPosts(
            ContentfulOptions options = default
        )
        {
            try
            {
                var postList = new List<BlogPost>();
                var page = 1;
                var shouldQueryMorePosts = true;

                while (shouldQueryMorePosts)
                {
                    var (Total, Items) = await GetPaginatedBlogPosts(
                        page,
                        options
                    );
                    postList.AddRange(
                        Items
                    );

                    shouldQueryMorePosts = postList.Count < Total;
                }

                return postList;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to get Slug List."
                );
                return new List<BlogPost>();
            }
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
                    var (Total, Items) = await GetPaginatedBlogPosts(
                        page,
                        options
                    );
                    slugList.AddRange(
                        Items.Select(
                            a => a.Slug
                        )
                    );

                    shouldQueryMoreSlugs = slugList.Count < Total;
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

        public async Task<(int Total, IEnumerable<BlogPost> Items)> GetPaginatedBlogPosts(
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

        public async Task<IEnumerable<BlogPost>> GetAllCachedBlogPosts(
            ContentfulOptions options = default
        )
        {
            var posts = await GetAllBlogPosts(options);
            if (!GetAllCachedBlogPostsCache.IsEmpty)
            {
                return GetAllCachedBlogPostsCache;
            }
            GetAllCachedBlogPostsCache = new ConcurrentBag<BlogPost>(posts);
            return GetAllCachedBlogPostsCache;
        }
    }
}

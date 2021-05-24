namespace Blazor.Contentful_.Blog.Starter.PageMetadataGeneration.Api
{
    using System.Threading.Tasks;
    using Blazor.Contentful_.Blog.Starter.CacheBusting.Api;
    using Blazor.Contentful_.Blog.Starter.Localization;
    using EventHorizon.Blazor.DocumentMetadata.Api;
    using Microsoft.Extensions.Localization;

    public interface PageMetadataGenerator
        : BustCache
    {
        Task Generate(
           IDocumentMetadataCollection registrator,
           IStringLocalizer<LocalizationResource> localizer
       );
    }
}

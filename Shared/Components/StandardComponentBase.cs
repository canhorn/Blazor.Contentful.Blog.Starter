namespace Blazor.Contentful_.Blog.Starter.Shared.Components
{
    using Blazor.Contentful_.Blog.Starter.Localization;
    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.Localization;

    public class StandardComponentBase
        : ComponentBase
    {
        [Inject]
        public IStringLocalizer<LocalizationResource> Localizer { get; set; } = null!;
    }
}

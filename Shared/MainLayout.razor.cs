namespace Blazor.Contentful_.Blog.Starter.Shared
{
    using System.Reflection;
    using Microsoft.AspNetCore.Components;

    public class MainLayoutBase
        : LayoutComponentBase
    {
        [CascadingParameter(Name = "IsPreview")]
        public bool IsPreview { get; set; }

        public string Pathname { get; set; } = "/";

        protected override void OnParametersSet()
        {
            Pathname = (Body?.Target as RouteView)
                ?.RouteData
                .PageType
                ?.GetCustomAttribute<RouteAttribute>()
                ?.Template ?? "/";
        }
    }
}

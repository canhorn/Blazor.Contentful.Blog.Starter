namespace Blazor.Contentful_.Blog.Starter.Data
{
    using System.Web;

    public class OpenGraph
    {
        public string TwitterUser { get; set; } = string.Empty;

        public static string GenerateImageUrl(
            string title
        )
        {
            // Here we are using a Vercel application to generate a dynamic Image for our Open Graph tags.
            return string.Join(
                string.Empty,
                "https://blazor-og-image.vercel.app/",
                title,
                ".png?theme=light&md=0fontSize=100px",
                "&images=https%3A%2F%2Fblazor-og-image.vercel.app%2Fblazor-logo.svg",
                "&images=data%3Aimage%2Fsvg%2Bxml%3Bbase64%2CPHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIyOSIgaGVpZ2h0PSIzMiI%2BCiAgPHBhdGggZmlsbD0iI0ZGRDg1RiIgZD0iTTkuNyAyMi4zQzggMjAuNyA3IDE4LjUgNyAxNnMxLTQuNyAyLjYtNi4zYzEuNC0xLjQgMS40LTMuNiAwLTVzLTMuNi0xLjQtNSAwQzEuOCA3LjYgMCAxMS42IDAgMTZzMS44IDguNCA0LjcgMTEuM2MxLjQgMS40IDMuNiAxLjQgNSAwIDEuMy0xLjQgMS4zLTMuNiAwLTV6Ij48L3BhdGg%2BCiAgPHBhdGggZmlsbD0iIzNCQjRFNyIgZD0iTTkuNyA5LjdDMTEuMyA4IDEzLjUgNyAxNiA3czQuNyAxIDYuMyAyLjZjMS40IDEuNCAzLjYgMS40IDUgMHMxLjQtMy42IDAtNUMyNC40IDEuOCAyMC40IDAgMTYgMFM3LjYgMS44IDQuNyA0LjdjLTEuNCAxLjQtMS40IDMuNiAwIDUgMS40IDEuMyAzLjYgMS4zIDUgMHoiPjwvcGF0aD4KICA8cGF0aCBmaWxsPSIjRUQ1QzY4IiBkPSJNMjIuMyAyMi4zQzIwLjcgMjQgMTguNSAyNSAxNiAyNXMtNC43LTEtNi4zLTIuNmMtMS40LTEuNC0zLjYtMS40LTUgMHMtMS40IDMuNiAwIDVDNy42IDMwLjIgMTEuNiAzMiAxNiAzMnM4LjQtMS44IDExLjMtNC43YzEuNC0xLjQgMS40LTMuNiAwLTUtMS40LTEuMy0zLjYtMS4zLTUgMHoiPjwvcGF0aD4KICA8Y2lyY2xlIGN4PSI3LjIiIGN5PSI3LjIiIHI9IjMuNSIgZmlsbD0iIzMwOEJDNSI%2BPC9jaXJjbGU%2BCiAgPGNpcmNsZSBjeD0iNy4yIiBjeT0iMjQuOCIgcj0iMy41IiBmaWxsPSIjRDU0NjVGIj48L2NpcmNsZT4KPC9zdmc%2B"
            );
        }
    }
}
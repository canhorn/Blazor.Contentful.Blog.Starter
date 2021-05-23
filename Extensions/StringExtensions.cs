#pragma warning disable CA1050 // Declare types in namespaces
public static class StringExtensions
#pragma warning restore CA1050 // Declare types in namespaces
{
    public static bool IsNullOrWhitespace(
        this string str
    )
    {
        return string.IsNullOrWhiteSpace(
            str
        );
    }

    public static bool IsNotNullOrWhitespace(
        this string str
    )
    {
        return !string.IsNullOrWhiteSpace(
            str
        );
    }

}

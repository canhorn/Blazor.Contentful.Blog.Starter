using System;

#pragma warning disable CA1050 // Declare types in namespaces
public static class DateTimeExtensions
#pragma warning restore CA1050 // Declare types in namespaces
{
    public static string FormatDateForDateTime(
        this string dateTimeString
    )
    {
        var dateTime = DateTime.Parse(dateTimeString);

        return $"{dateTime.Year}-{AddLeadingZero(dateTime.Month)}-{dateTime.Day}";
    }
    public static string FormatDateForDisplay(
        this string dateTimeString
    )
    {
        var dateTime = DateTime.Parse(dateTimeString);

        return $"{dateTime.Day} {GetMonthStringFromInt(dateTime.Month)} {dateTime.Year}";
    }

    private static string AddLeadingZero(
        int num
    )
    {
        var numStr = num.ToString();
        while (numStr.Length < 2) numStr = "0" + numStr;
        return numStr;
    }

    private static readonly string[] Months = new string[]
    {
        "Jan",
        "Feb",
        "Mar",
        "Apr",
        "May",
        "Jun",
        "Jul",
        "Aug",
        "Sep",
        "Oct",
        "Nov",
        "Dec",
    };

    private static string GetMonthStringFromInt(
        int month
    )
    {
        return Months[month - 1];
    }
}

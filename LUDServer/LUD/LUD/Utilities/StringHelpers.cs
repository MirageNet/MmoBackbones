using System.Net;

namespace LUD.Utilities
{
    public static class StringHelpers
    {
        public static string SplitPortFromAddress(string endPoint)
        {
            string[] parsedList = endPoint.Split(':');

            endPoint = parsedList.Length switch
            {
                > 2 => IPAddress.Parse(endPoint).ToString(),
                2 => parsedList[0],
                _ => throw new ArgumentException("No port found", nameof(endPoint))
            };

            return endPoint;
        }
    }
}

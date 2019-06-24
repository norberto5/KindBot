namespace KindBot.Tools
{
    public static class ExtensionMethods
    {
        public static string ConvertToTeamspeakString(this string str)
        {
            return str
                .Replace(@"\", @"\\")
                .Replace("/", @"\/")
                .Replace(" ", @"\s")
                .Replace("|", @"\p");
        }

        public static string ConvertTeamspeakToNormal(this string teamspeakString)
        {
            return teamspeakString
                .Replace(@"\\", @"\")
                .Replace(@"\/", "/")
                .Replace(@"\s", " ")
                .Replace(@"\p", "|");
        }

        public static bool ToBool(this int value) => value > 0;
    }
}
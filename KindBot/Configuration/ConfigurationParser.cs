using KindBot.Tools;

namespace KindBot.Configuration
{
    public static class ConfigurationParser
    {
        public static bool Parse(string stringToParse, string configFile, string parameterName, out bool result) => ParseBool(stringToParse, configFile, parameterName, out result);
        public static bool ParseBool(string stringToParse, string configFile, string parameterName, out bool result)
        {
            if(!bool.TryParse(stringToParse, out result))
            {
                ConsoleEx.Error($"[Configuration]: There was a problem with loading '{parameterName}' in {configFile}\n" +
                "Use 'true' or 'false' in this configuration");
                return false;
            }
            return true;
        }

        public static bool Parse(string stringToParse, string configFile, string parameterName, out int result) => ParseInteger(stringToParse, configFile, parameterName, out result);
        public static bool ParseInteger(string stringToParse, string configFile, string parameterName, out int result)
        {
            if(!int.TryParse(stringToParse, out result))
            {
                ConsoleEx.Error($"[Configuration]: There was a problem with loading '{parameterName}' in {configFile}");
                return false;
            }
            return true;
        }

    }
}
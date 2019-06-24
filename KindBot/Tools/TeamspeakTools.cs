using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KindBot.Tools
{
    public static class TeamspeakTools
    {
        public static Dictionary<string, string> GetParameters(string str)
        {
            var dict = new Dictionary<string, string>();

            foreach(string s in str.Trim().Split(' '))
            {
                int index = s.IndexOf('=');
                if(index == -1) continue;
                string key = s.Substring(0, index).Trim();
                string value = s.Substring(index + 1).Trim();
                dict.Add(key, value);
            }

            return dict;
        }

        public static T GetParameter<T>(string str, string param)
        {
            try
            {
                str = str.Trim();
                param = $"{param}=";
                int index = str.IndexOf(param);
                if(index == -1) return default;
                if(index != 0)
                {
                    param = $" {param}";
                    index = str.IndexOf(param);
                    if(index == -1) return default;
                }
                index += param.Length;

                int length = str.IndexOf(' ', index) - index;
                if(length <= 0) length = str.IndexOf("\n", index) - index;
                if(length <= 0) length = str.Length - index;
                string parameter = str.Substring(index, length);

                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                if(typeof(T) == typeof(bool))
                {
                    int intParameter = (int)Convert.ChangeType(parameter, typeof(int));
                    return (T)converter.ConvertFromString((intParameter == 1).ToString());
                }
                else if(typeof(T) == typeof(string))
                {
                    return (T)converter.ConvertFromString(parameter.ConvertTeamspeakToNormal());
                }
                return converter != null ? (T)converter.ConvertFromString(parameter) : default;
            }
            catch(NotSupportedException)
            {
                return default;
            }
        }

        public static bool TryGetParameter<T>(string str, string param, out T output)
        {
            try
            {
                output = GetParameter<T>(str, param);
                return true;
            }
            catch
            {
                output = default;
                return false;
            }

        }

        public static IEnumerable<int> GetListOfGroups(string str)
        {
            if(str == null)
            {
                yield break;
            }

            foreach(string s in str.Split(','))
            {
                if(int.TryParse(s, out int gr))
                {
                    yield return gr;
                }
            }
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            date = date.AddSeconds(unixTimeStamp).ToLocalTime();
            return date;
        }
    }
}
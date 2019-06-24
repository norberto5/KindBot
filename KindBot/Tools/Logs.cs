using System;
using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;

namespace KindBot.Tools
{
    public enum LogType
    {
        Normal,
        Debug,
        Warning,
        Error
    };

    [MoonSharpUserData]
    public static class Logs
    {
#if LINUX
        private const string dir = @"logs/";
#else
        private const string dir = @"logs\";
#endif
        private static readonly string logPath = dir + "log.txt";
        private static readonly string logDebugPath = dir + "debug.txt";
        private static readonly string logErrorPath = dir + "error.txt";

        private static readonly TimeSpan logSaveInterval = TimeSpan.FromSeconds(10);
        private static List<string> logList = new List<string>();
        private static List<string> logDebugList = new List<string>();
        private static List<string> logErrorList = new List<string>();
        private static DateTime lastTimeLogWasSaved = DateTime.Now;


        public static void Error(string text) => WriteLog(text, LogType.Error);

        public static void Warning(string text) => WriteLog(text, LogType.Warning);

        public static void Write(string text) => WriteLog(text, LogType.Normal);

        public static void Debug(string text) => WriteLog(text, LogType.Debug);

        [MoonSharpHidden]
        public static void WriteLog(string text, LogType type = LogType.Normal)
        {
            string prefix = "";
            switch(type)
            {
                case LogType.Warning:
                    prefix = "[WARNING]: ";
                    logErrorList.Add(prefix + text);
                    break;
                case LogType.Error:
                    prefix = "[ERROR]: ";
                    logErrorList.Add(prefix + text);
                    break;
                case LogType.Debug:
                    prefix = "[DEBUG]: ";
                    logDebugList.Add(prefix + text);
                    break;
            }
            logList.Add(prefix + text);


            if(lastTimeLogWasSaved + logSaveInterval < DateTime.Now)
            {
                SaveAllLogs();
            }

        }

        [MoonSharpHidden]
        public static void SaveAllLogs()
        {
            SaveLog(LogType.Normal);
            SaveLog(LogType.Debug);
            SaveLog(LogType.Error);
            lastTimeLogWasSaved = DateTime.Now;
        }

        [MoonSharpHidden]
        public static void SaveLog(LogType type = LogType.Normal)
        {
            List<string> list;
            string path;
            switch (type)
            {
                case LogType.Normal:
                    list = logList;
                    path = logPath;
                    break;
                case LogType.Debug:
                    list = logDebugList;
                    path = logDebugPath;
                    break;
                case LogType.Warning:
                case LogType.Error:
                    list = logErrorList;
                    path = logErrorPath;
                    break;
                default:
                    return;
            }
            try
            {
                //create directory if it doesn't exist
                string pathdir = Path.GetDirectoryName(path);
                if(pathdir.Length > 0) Directory.CreateDirectory(pathdir);

                if(list.Count == 0)
                    return;

                using(StreamWriter f = File.AppendText(path))
                {
                    foreach(string s in list)
                        f.WriteLine(s);
                    f.Flush();
                    f.Close();
                    f.Dispose();
                }
                list.Clear();
            }
            catch(Exception)
            {
                ConsoleEx.Error("There was a problem with saving logs to the file");
            }

        }
    }
}
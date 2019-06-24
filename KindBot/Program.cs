using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using KindBot.Communication;
using KindBot.Tools;

namespace KindBot
{
    internal class Program
    {
        private static Bot bot;

        private static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColors.DefaultColor;
            Console.Title = $"{Assembly.GetExecutingAssembly().GetName().Name} ver. {Assembly.GetExecutingAssembly().GetName().Version.ToString()}";
            //Console.BufferHeight = 25;
            Console.SetCursorPosition(0, 0);
            Console.Clear();
            ConsoleEx.WriteLine($"{Assembly.GetExecutingAssembly().GetName().Name} ver. {Assembly.GetExecutingAssembly().GetName().Version.ToString()} (build-time {File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).ToString("g", new CultureInfo("en-GB"))} )");

#if !LINUX
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);
#endif

            try
            {
                bot = new Bot();
                bot.Start();
            }
            catch(Exception ex)
            {
                ConsoleEx.Error($"I've caught an unhandled exception: {ex.Message} source: {ex.Source}");
                ConsoleEx.Debug(ex.StackTrace);
            }

        }

        private static void OnExit()
        {
            ConsoleEx.WriteLine("Kill the process signal received. Program is closing...");
            bot.Stop();
            Logs.SaveAllLogs(); // saving logs to files
            TelnetConnector.Instance.Stop();
        }

#if !LINUX
        private static bool ConsoleEventCallback(int eventType)
        {
            if(eventType == 0 || eventType == 2 || eventType == 5 || eventType == 6)
            {
                OnExit();
            }
            return true;
        }
        private static ConsoleEventDelegate handler;
        private delegate bool ConsoleEventDelegate(int eventType);
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
#endif
    }
}
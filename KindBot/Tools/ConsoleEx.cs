using System;
using System.Runtime.CompilerServices;
using MoonSharp.Interpreter;

namespace KindBot.Tools
{
    public static class ConsoleColors
    {
        public const ConsoleColor DefaultColor = ConsoleColor.Green;
        public const ConsoleColor DebugColor = ConsoleColor.Magenta;
        public const ConsoleColor WarningColor = ConsoleColor.Yellow;
        public const ConsoleColor ErrorColor = ConsoleColor.Red;
        public const ConsoleColor StatusTextColor = ConsoleColor.Yellow;
        public const ConsoleColor StatusBackgroundColor = ConsoleColor.DarkGreen;
    }

    [MoonSharpUserData]
    public static class ConsoleEx
    {
        /// <summary>
        /// Writes a text to the console without a new line character. Uses different font color.
        /// </summary>
        /// <param name="text">A text to write to the console.</param>
        /// <param name="color">A font color.</param>
        public static void Write(string text, ConsoleColor color = ConsoleColors.DefaultColor, bool log = true)
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(AddTimestamp(text));
            Console.ForegroundColor = old;
            if(log) Logs.WriteLog(AddTimestamp(text), LogType.Normal);
        }

        /// <summary>
        /// Writes a text to the console with a new line character. Uses different font color.
        /// </summary>
        /// <param name="text">A text to write to the console.</param>
        /// <param name="color">A font color.</param>
        public static void WriteLine(string text, ConsoleColor color = ConsoleColors.DefaultColor, bool log = true) => Write(text + "\n", color, log);

        /*
        public static void WriteStatusBar(string text, ConsoleColor color = StatusTextColor, ConsoleColor background = StatusBackgroundColor)
        {
            ConsoleColor old = Console.ForegroundColor;
            ConsoleColor oldbg = Console.BackgroundColor;
            Console.ForegroundColor = color;
            Console.BackgroundColor = background;
            string temp = text;
            temp += new string(' ', Console.WindowWidth - text.Length);
            Console.Write(temp);
            Console.ForegroundColor = old;
            Console.BackgroundColor = oldbg;
        }
        */

        /// <summary>
        /// Writes a text to the console with a new line character. Uses debug font color.
        /// </summary>
        /// <param name="text">A text to write to the console.</param>
        public static void Debug(string text)
        {
            WriteLine(text, ConsoleColors.DebugColor, false);
            Logs.WriteLog(AddTimestamp(text), LogType.Debug);
        }

        /// <summary>
        /// Writes a text to the console with a new line character. Uses warning font color and beeps.
        /// </summary>
        /// <param name="text">A text to write to the console.</param>
        public static void Warning(string text)
        {
            WriteLine(text, ConsoleColors.WarningColor, false);
            Console.Beep();
            Logs.WriteLog(AddTimestamp(text), LogType.Warning);
        }

        /// <summary>
        /// Writes a text to the console with a new line character. Uses error font color and beeps. Also prints out more details about an error (only in debug build).
        /// </summary>
        /// <param name="text">A text to write to the console.</param>
        public static void Error(string text, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "", [CallerLineNumber] int callerLine = 0)
        {
            WriteLine(text, ConsoleColors.ErrorColor, false);
#if DEBUG
            string stack = $"Error caller: {callerName} from {callerFile} (line: {callerLine} )";
            WriteLine(stack, ConsoleColors.ErrorColor, false);
            Logs.Error(stack);
            //MailSystem.MailError($"Message: {text}\nError caller: {callerName} from {callerFile} (line: {callerLine} )");
#endif
            Console.Beep();
            Logs.WriteLog(AddTimestamp(text), LogType.Error);
        }

        private static string AddTimestamp(string text)
        {
            DateTime date = DateTime.Now;
            int hour = date.Hour;
            int minute = date.Minute;
            int second = date.Second;
            string temp = "[" + hour.ToString("00") + ":" + minute.ToString("00") + ":" + second.ToString("00") + "]: " + text;
            return temp;
        }
    }
}
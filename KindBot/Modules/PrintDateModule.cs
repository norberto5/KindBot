using System;
using KindBot.Modules.Common;
using KindBot.Tools;

namespace KindBot.Modules
{
    public class PrintDateModule : Module
    {
        private int dayOfYear = 0;

        public PrintDateModule() : base()
        {
            ModuleName = "PrintDateModule";
        }

        public override void Run()
        {
            DateTime date = DateTime.Now;
            if(dayOfYear != date.DayOfYear)
            {
                dayOfYear = date.DayOfYear;
                ConsoleEx.WriteLine($"Actual date: {date.ToString("dd.MM.yyyy")}");
            }
        }
    }
}
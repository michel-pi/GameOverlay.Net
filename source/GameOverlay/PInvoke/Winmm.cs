using System;

namespace GameOverlay.PInvoke
{
    internal static class Winmm
    {
        public delegate MultimediaResult TimeBeginPeriodDelegate(uint period);
        public delegate MultimediaResult TimeEndPeriodDelegate(uint period);

        public static readonly TimeBeginPeriodDelegate TimeBeginPeriod;
        public static readonly TimeEndPeriodDelegate TimeEndPeriod;

        static Winmm()
        {
            var library = DynamicImport.ImportLibrary("winmm.dll");

            TimeBeginPeriod = DynamicImport.Import<TimeBeginPeriodDelegate>(library, "timeBeginPeriod");
            TimeEndPeriod = DynamicImport.Import<TimeEndPeriodDelegate>(library, "timeEndPeriod");
        }
    }
}

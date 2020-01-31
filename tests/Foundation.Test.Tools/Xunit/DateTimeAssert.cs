using System;

namespace Xunit
{
    public enum DateTimeAssertTolerance
    {
        None,
        Time,
        Minutes,
        Seconds,
        Milliseconds
    }

    public partial class Assert
    {
        public static void Equal(DateTime expected, DateTime actual, DateTimeAssertTolerance tolerance)
        {
            var toleranceSpan = Tolerance(tolerance);

            Equal(Truncate(expected, toleranceSpan), Truncate(actual, toleranceSpan));
        }

        public static void NotEqual(DateTime expected, DateTime actual, DateTimeAssertTolerance tolerance)
        {
            var toleranceSpan = Tolerance(tolerance);

            NotEqual(Truncate(expected, toleranceSpan), Truncate(actual, toleranceSpan));
        }

        private static DateTime Truncate(DateTime dateTime, TimeSpan timeSpan)
        {
            return timeSpan == TimeSpan.Zero ? dateTime : dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
        }

        private static TimeSpan Tolerance(DateTimeAssertTolerance tolerance)
        {
            switch (tolerance)
            {
                case DateTimeAssertTolerance.Time:
                    return TimeSpan.FromDays(1);
                case DateTimeAssertTolerance.Minutes:
                    return TimeSpan.FromHours(1);
                case DateTimeAssertTolerance.Seconds:
                    return TimeSpan.FromMinutes(1);
                case DateTimeAssertTolerance.Milliseconds:
                    return TimeSpan.FromSeconds(1);
                case DateTimeAssertTolerance.None:
                default:
                    return TimeSpan.Zero;
            }
        }
    }
}

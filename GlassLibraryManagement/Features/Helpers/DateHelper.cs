namespace GlassLibraryManagement.Features.Helpers
{
    public static class DateHelper
    {
        public static readonly TimeSpan DefaultNearDueThreshold = TimeSpan.FromHours(24);

        /// <summary>Default loan length (days) used for simple borrow/return flows.</summary>
        public const int DefaultBorrowDays = 1;

        public static DateTime UtcNow() => DateTime.UtcNow;

        public static DateTime CalculateDueDate(DateTime borrowedAtUtc, int borrowDays = DefaultBorrowDays)
            => borrowedAtUtc.Date.AddDays(borrowDays);

        public static bool IsOverdue(DateTime dueAtUtc, DateTime? returnedAtUtc = null)
            => (returnedAtUtc ?? UtcNow()) > dueAtUtc;

        public static bool IsNearDue(DateTime dueAtUtc, DateTime nowUtc, TimeSpan threshold)
            => dueAtUtc > nowUtc && (dueAtUtc - nowUtc) <= threshold;

        public static string FormatTimeRemaining(DateTime dueAtUtc, DateTime nowUtc)
        {
            if (nowUtc >= dueAtUtc)
            {
                var overdue = nowUtc - dueAtUtc;
                if (overdue.TotalDays >= 1)
                {
                    return $"Overdue by {(int)overdue.TotalDays} day(s)";
                }

                if (overdue.TotalHours >= 1)
                {
                    return $"Overdue by {(int)overdue.TotalHours} hour(s)";
                }

                var minutes = Math.Max(1, (int)overdue.TotalMinutes);
                return $"Overdue by {minutes} minute(s)";
            }

            var remaining = dueAtUtc - nowUtc;
            if (remaining.TotalDays >= 1)
            {
                return $"{(int)remaining.TotalDays} day(s), {remaining.Hours} hour(s) left";
            }

            if (remaining.TotalHours >= 1)
            {
                return $"{(int)remaining.TotalHours} hour(s), {remaining.Minutes} minute(s) left";
            }

            var minsLeft = Math.Max(1, (int)remaining.TotalMinutes);
            return $"{minsLeft} minute(s) left";
        }
    }
}
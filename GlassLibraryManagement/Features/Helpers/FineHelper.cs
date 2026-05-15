namespace GlassLibraryManagement.Features.Helpers
{
    public static class FineHelper
    {
        public static decimal CalculateFine(DateTime dueAtUtc, DateTime? returnedAtUtc = null, decimal dailyRate = 10m)
        {
            var effectiveReturn = returnedAtUtc ?? DateTime.UtcNow;
            if (effectiveReturn <= dueAtUtc)
            {
                return 0m;
            }

            var overdueDays = (effectiveReturn.Date - dueAtUtc.Date).Days;
            return overdueDays <= 0 ? 0m : overdueDays * dailyRate;
        }
    }
}
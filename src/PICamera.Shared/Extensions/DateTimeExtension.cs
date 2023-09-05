namespace PICamera.Shared.Extensions
{
    public static class DateTimeExtension
    {
        public static string AsTimeAgo(this DateTimeOffset dateTime)
        {
            TimeSpan timeSpan = DateTimeOffset.Now.Subtract(dateTime);

            return timeSpan.TotalSeconds switch
            {
                <= 60 => $"{timeSpan.Seconds} seconds ago",

                _ => timeSpan.TotalMinutes switch
                {
                    <= 1 => "a minute ago",
                    < 60 => $"{timeSpan.Minutes} minutes ago",
                    _ => timeSpan.TotalHours switch
                    {
                        <= 1 => "an hour ago",
                        < 24 => $"{timeSpan.Hours} hours ago",
                        _ => timeSpan.TotalDays switch
                        {
                            <= 1 => "yesterday",
                            <= 30 => $"{timeSpan.Days} days ago",

                            <= 60 => "a month ago",
                            < 365 => $"{timeSpan.Days / 30} months ago",

                            <= 365 * 2 => "a year ago",
                            _ => $"{timeSpan.Days / 365} years ago"
                        }
                    }
                }
            };
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}

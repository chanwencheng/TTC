namespace TTCRestAPI
{
    public static class LocalTime
    {
        public static string Culture { get; set; } = "zh-TW";

        public static long Timestamp => (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;

        public static long TimeSeconds => (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;

        public static string CurrentDateTime => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        public static string CurrentDate => DateTime.Now.ToString("yyyy-MM-dd");

        public static string CurrentTime => DateTime.Now.ToString("HH:mm:ss");
    }
}

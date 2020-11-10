namespace lab2
{
    public static class Utils
    {
        public static bool IsNullOrEmpty(byte[] source)
        {
            return source is null || source.Length == 0;
        }
        public static bool IsNullOrEmpty(string source)
        {
            return source is null || source.Length == 0;
        }
    }
}

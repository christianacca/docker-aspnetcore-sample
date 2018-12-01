namespace ImageGallery.Client.Helpers
{
    public static class StringExtensions
    {
        public static string IfNullOrWhiteSpace(this string source, string defaultValue)
        {
            return string.IsNullOrWhiteSpace(source) ? defaultValue : source;
        }
    }
}
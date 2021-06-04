namespace PokeAPI.Domain.Helpers
{
    public class AppSettings
    {
        public static int CacheExpirationInMinutes { get; set; } = 60;
        public static string BaseURL { get; set; }
        public static int ResultLimit { get; set; }
    }
}

namespace PokeAPI.Extensions
{
    public static class StringExtension
    {
        public static string SanitizedName(this string name)
        {
            if (name == null) return "";

            return  name
              .Replace(" ", "-")
              .Replace("'", "")
              .Replace(".", "");
        }
    }
}

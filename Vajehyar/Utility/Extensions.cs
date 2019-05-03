using System.Globalization;

namespace Vajehyar.Utility
{
    public static class Extensions
    {
        public static int Round(this int num)
        {
            return num % 1000 >= 500 ? num + 1000 - num % 1000 : num - num % 1000;
        }

        public static string Format(this int num)
        {
            return num.ToString("N0", CultureInfo.GetCultureInfo("fa-IR"));
        }
    }
}

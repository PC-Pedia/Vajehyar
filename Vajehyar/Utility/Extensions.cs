using System;
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

        public static bool IsBeta(this Version version)
        {
            return version<new Version(1,0,0);
        }

        /// <summary>
        /// Return Sem Version: x.y.z
        /// </summary>
        /// <param name="ver"></param>
        /// <returns></returns>
        public static string ToSemVersion(this Version ver)
        {
            int major = ver.Major;
            int minor = ver.Minor;
            int patch = ver.Build;
            return $"{major}.{minor}.{patch}";
        }
    }
}

using System;
using System.Text.RegularExpressions;

namespace Vajehyar
{
    public class Database
    {
        private static string[] _lines;

        public static string[] GetData()
        {
            string dict1 = Properties.Resources.Motaradef_Motazad;
            string dict2 = Properties.Resources.Teyfi;
            string data = dict1 + Environment.NewLine + dict2;

            _lines = data.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            return _lines;

        }

        public static int GetCount(string str)
        {
            string linesWithoutDigit = Regex.Replace(str, @"\d", "");
            int count = linesWithoutDigit.Split('،').Length;
            return count;
        }
    }
}

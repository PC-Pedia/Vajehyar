using System;
using System.Text.RegularExpressions;

namespace Vajehyar
{
    public sealed class Database
    {
        public static Database Instance { get; } = new Database();

        public string[] Lines { get; }

        private Database()
        {
            string dict1 = Properties.Resources.Motaradef_Motazad;
            string dict2 = Properties.Resources.Teyfi;
            string data = dict1 + Environment.NewLine + dict2;

            Lines = data.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
        }

        public int GetCount()
        {
            string linesWithoutDigit = Regex.Replace(String.Concat(Lines), @"\d", "");
            int count = linesWithoutDigit.Split('،').Length;
            return count;
        }
    }
}

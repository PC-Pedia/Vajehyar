using System;

namespace Vajehyar
{
    public class Database
    {
        public static string[] GetData()
        {
            string dict1 = Properties.Resources.Motaradef_Motazad;
            string dict2=  Properties.Resources.Teyfi;
            string data = dict1 + Environment.NewLine + dict2;

            return data.Split(new[] { Environment.NewLine }, 
                StringSplitOptions.RemoveEmptyEntries);
        }
    }
}

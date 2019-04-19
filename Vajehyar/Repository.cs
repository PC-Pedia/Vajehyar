using System;

namespace Vajehyar
{
    public class Repository
    {
        public static string[] GetData()
        {
            string content = Properties.Resources.Motaradef_Motazad + Environment.NewLine + Properties.Resources.Teyfi;
            string[] lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            return lines;
        }
    }
}

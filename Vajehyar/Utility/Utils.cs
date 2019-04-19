using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vajehyar.Utility
{
    public static class Utils
    {
        public static int RoundNumber(int num)
        {
            return num % 1000 >= 500 ? num + 1000 - num % 1000 : num - num % 1000;
        }

        public static int Count(this ICollectionView view)
        {
            var index = 0;
            foreach (var unused in view)
            {
                index++;
            }
            return index;
        }
    }
}

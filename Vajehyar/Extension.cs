using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vajehyar
{
    static class Extension
    {
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

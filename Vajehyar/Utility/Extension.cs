using System.ComponentModel;

namespace Vajehyar.Utility
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

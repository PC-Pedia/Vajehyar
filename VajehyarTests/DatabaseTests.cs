using System;
using Vajehyar;
using Xunit;

namespace VajehyarTests
{
    public class DatabaseTests
    {
        [Fact]
        public void Test1()
        {
            String str1 = "یک، دو، سه";

            var count = Database.GetCount(str1);
            Assert.Equal(3,count);
        }

        [Fact]
        public void Test2()
        {
            String str1 = "یک،         دو، سه";

            var count = Database.GetCount(str1);
            Assert.Equal(3, count);
        }

        [Fact]
        public void Test3()
        {
            String str1 = "یک، دو،" + Environment.NewLine + "سه";

            var count = Database.GetCount(str1);
            Assert.Equal(3, count);
        }
    }
}

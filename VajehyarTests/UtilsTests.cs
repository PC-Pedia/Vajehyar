using System;
using Vajehyar.Utility;
using Xunit;

namespace VajehyarTests
{
    public class UtilsTests
    {
        [Fact]
        public void Test1()
        {
            int sample1 = 1;
            string expected = "1";
            string actual = sample1.Format();

            Assert.Equal(expected,actual);
        }

        [Fact]
        public void Test2()
        {
            int sample1 = 10;
            string expected = "10";
            string actual = sample1.Format();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test3()
        {
            int sample1 = 100;
            string expected = "100";
            string actual = sample1.Format();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test4()
        {
            int sample1 = 1000;
            string expected = "1,000";
            string actual = sample1.Format();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test5()
        {
            int sample1 = 12000;
            string expected = "12,000";
            string actual = sample1.Format();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test6()
        {
            int sample1 = 125000;
            string expected = "125,000";
            string actual = sample1.Format();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test7()
        {
            int sample1 = 125965;
            string expected = "125,965";
            string actual = sample1.Format();

            Assert.Equal(expected, actual);
        }
    }
}

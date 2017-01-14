using System;
using Xunit;
using CrowdSource.Services;
using System.Collections.Generic;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public void Test1()
        {
            Assert.True(true);
        }

        [Fact]
        public void TestBanJiao()
        {

            Assert.Equal(new TextSanitizer().BanJiao("abc（），；"), "abc(),;");
            // Unicode extended
            Assert.Equal(new TextSanitizer().BanJiao("abc（）𪴯𢲫"), "abc()𪴯𢲫");
            // BUC
            Assert.Equal(new TextSanitizer().BanJiao("abc（）ṳ̆ngṳ̄ngé̤ṳngé̤ṳkṳ̀ngê̤ṳngṳ̆k"), "abc()ṳ̆ngṳ̄ngé̤ṳngé̤ṳkṳ̀ngê̤ṳngṳ̆k");
            // 中文句号不会变成句点
            Assert.Equal(new TextSanitizer().BanJiao("。"), "。");

        }

        [Fact]
        public void TestShuffle()
        {
            var aList = new List<int>();
            for (int i = 1; i <= 100; ++i)
            {
                aList.Add(i);
            }
            var oldLength = aList.Count;
            Shuffle.DoShuffle(aList);

            Assert.Equal(aList.Count, oldLength);

            bool inverseOrder = false;
            for (int i =0; i<aList.Count; ++i)
            {
                if (aList[i]!=i)
                {
                    inverseOrder = true;
                    break;
                }
            }
            Assert.True(inverseOrder);

            // 确保没有丢东西
            var intSet = new HashSet<int>();
            foreach (var item in aList)
            {
                intSet.Add(item);
            }
            Assert.Equal(intSet.Count, oldLength);
        }

    }
}
using System;
using Xunit;
using CrowdSource.Services;

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
    }
}
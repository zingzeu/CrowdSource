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
            Assert.Equal(new TextSanitizer().BanJiao("abc（）??"), "abc()??");
            // BUC
            Assert.Equal(new TextSanitizer().BanJiao("abc（）??ng??ngé??ngé??k??ngê??ng??k"), "abc()??ng??ngé??ngé??k??ngê??ng??k");
            // 中文句号不会变成句点
            Assert.Equal(new TextSanitizer().BanJiao("。"), "。");

        }
    }
}
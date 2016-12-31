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

            Assert.Equal(new TextSanitizer().BanJiao("abc��������"), "abc(),;");
            // Unicode extended
            Assert.Equal(new TextSanitizer().BanJiao("abc����??"), "abc()??");
            // BUC
            Assert.Equal(new TextSanitizer().BanJiao("abc����??ng??ng��??ng��??k??ng��??ng??k"), "abc()??ng??ng��??ng��??k??ng��??ng??k");
            // ���ľ�Ų����ɾ��
            Assert.Equal(new TextSanitizer().BanJiao("��"), "��");

        }
    }
}
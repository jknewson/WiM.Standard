using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiM.Extensions;
namespace WiM.Test
{
    [TestClass]
    public class StringExtensionsTest
    {
        [TestMethod]
        public void TrimToLowerTest()
        {
            string TestString = " Teststring ";
            var result = TestString.TrimToLower();
            Assert.AreEqual("teststring", result);
        }
        [TestMethod]
        public void TrimToUpperTest()
        {
            string TestString = " Teststring ";
            var result = TestString.TrimToUpper();
            Assert.AreEqual("TESTSTRING", result);
        }
        [TestMethod]
        public void IgnoreCaseEqualsTest()
        {
            string TestString1 = " Teststring ";
            string TestString2 = " testString";
            var result = TestString1.IgnoreCaseEquals(TestString2);
            Assert.IsTrue(result);
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiM.Security;

namespace WiMServices.Test
{
    [TestClass]
    public class SecurityTest
    {
        [TestMethod]
        public void PasswordHashTest()
        {
            string password = "Dog1";
            string salt = Cryptography.CreateSalt();
            Assert.IsNotNull(salt);

            string hashstring = Cryptography.GenerateSHA256Hash(password, salt);
            Assert.IsNotNull(hashstring);

            Boolean verified = Cryptography.VerifyPassword(password, salt, hashstring);
            Assert.IsTrue(verified);
        }
    }
}

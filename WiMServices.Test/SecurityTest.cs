using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiM.Authentication;

namespace WiMServices.Test
{
    [TestClass]
    public class SecurityTest
    {
        [TestMethod]
        public void PasswordHashTest()
        {
            string password = "Dog1";
            string salt = Security.CreateSalt();
            Assert.IsNotNull(salt);

            string hashstring = Security.GenerateSHA256Hash(password, salt);
            Assert.IsNotNull(hashstring);

            Boolean verified = Security.VerifyPassword(password, salt,hashstring);
            Assert.IsTrue(verified);
        }
    }
}

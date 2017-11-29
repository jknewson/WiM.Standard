using System;
using Xunit;

using WiM.Security;

namespace Security.XUnitTest
{
    public class CryptographyXUnitTest
    {
        [Fact]
        public void PasswordHashTest()
        {
            try
            {
                string password = "Dog1";
                string salt = Cryptography.CreateSalt();
                Assert.NotNull(salt);

                string hashstring = Cryptography.GenerateSHA256Hash(password, salt);
                Assert.NotNull(hashstring);

                Boolean verified = Cryptography.VerifyPassword(password, salt, hashstring);
                Assert.True(verified);

            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }
    }
}

using System;
using Xunit;
using WiM.Security;

namespace Security.XUnitTest
{
    public class EasySecureStringXUnitTest
    {
        [Fact]
        public void SecureStringTest()
        {
            try
            {
                string password = "Dog1";
                var secureString = new EasySecureString(password);
                var decriptedString = secureString.decryptString();
                Assert.Equal(password, decriptedString);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }
    }
}

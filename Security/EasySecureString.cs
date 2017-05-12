#region Comments
//05.04.2017- JKN- Modified for cross platform. 
//02.17.12 - JB - Created
#endregion
#region Copywright
/* Authors:
 *      Jonathan Baier (jbaier@usgs.gov)
 *      Jeremy Newson (jknewson@usgs.gov)
 * Copyright:
 *      2017 USGS - WiM
 */
#endregion

using System;
using System.Security;
using System.Runtime.InteropServices;

namespace WiM.Security
{
    public class EasySecureString : IDisposable
    {
        private IntPtr _stringPointer;
        private SecureString _secureString = new SecureString();

        public EasySecureString(String value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                _secureString.AppendChar(value[i]);
            }
            _secureString.MakeReadOnly();
        }

        public String decryptString()
        {
            _stringPointer = SecureStringMarshal.SecureStringToCoTaskMemUnicode(_secureString);
            return Marshal.PtrToStringUni(_stringPointer);
        }


        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // Take this off the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Free other state (managed objects).
                _secureString.Dispose();
            }
            Marshal.ZeroFreeCoTaskMemUnicode(_stringPointer);
        }

        // Use C# destructor syntax for finalization code.
        ~EasySecureString()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
    }
}
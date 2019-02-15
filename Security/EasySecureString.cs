//------------------------------------------------------------------------------
//----- EasySecureString -------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   Secure string with disposable support
//
//discussion:
//     

#region Comments
// 05.04.2017 - updated to .net standard
#endregion

using System;
using System.Security;
using System.Runtime.InteropServices;

namespace WIM.Security
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
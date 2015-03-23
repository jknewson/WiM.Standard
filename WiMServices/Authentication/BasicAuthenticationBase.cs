#region Comments
// 12.11.14 - jkn - Converted to base class 
// 08.23.12 - JB - Fixed Case sensitivity bug
// 07.02.12 - JKN - Implemented Authorization Roles
// 05.24.12 - JB - Made member table check case insensitive
// 02.17.12 - JB - Added Base64 decode method for Basic Auth
// 01.24.12 - JB - Created
#endregion
#region Copywright
/* Authors:
 *      Jonathan Baier (jbaier@usgs.gov)
 * Copyright:
 *      2012 USGS - WiM
 */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Configuration;

using OpenRasta.Authentication;
using OpenRasta.Authentication.Basic;

namespace WiM.Authentication
{
    public abstract class BasicAuthenticationBase : IBasicAuthenticator
    {
        #region Properties

        public abstract string Realm { get;}
        #endregion

        #region Methods
        public abstract AuthenticationResult Authenticate(BasicAuthRequestHeader header);

        //Add public method for decoding base 64 later
        public static BasicAuthRequestHeader ExtractBasicHeader(string value)
        {
            try
            {
                var basicBase64Credentials = value.Split(' ')[1];

                var basicCredentials = Encoding.UTF8.GetString( Convert.FromBase64String( basicBase64Credentials ) ).Split(':');

                if (basicCredentials.Length != 2)
                    return null;

                return new BasicAuthRequestHeader(basicCredentials[0], basicCredentials[1]);
            }
            catch
            {
                return null;
            }

        }//end ExtractBasicHeader
        #endregion

    }//end Class BasicAuthenticationBase

}//end namespace
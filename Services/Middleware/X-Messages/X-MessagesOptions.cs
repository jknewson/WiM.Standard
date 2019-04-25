//------------------------------------------------------------------------------
//----- Messaging --------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2019 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   add messages to http response header
//
//discussion:
// 

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace WIM.Services.Messaging
{
    public class x_MessagesOptions
    {
        /// <summary>
        /// Gets or sets the message header key.
        /// default is 'X-USGSWIM-Messages'
        /// </summary>
        public string messageHeaderString { get; set; } = X_MessagesDefault.msgheader;
        /// <summary>
        /// Gets or sets the httpContext.Items Key where messages can be retreived
        /// default is 'wim_msgs'
        /// </summary>
        public string ItemsKey { get; set; } = X_MessagesDefault.ItemsKey;
        /// <summary>
        /// Gets or sets the machine host (NetBios name of local machine) will be populated in header, using specified headername,
        /// Default, no hostname header will be set.
        /// </summary>
        public string HostKey { get; set; } = string.Empty;

    }
}

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

namespace WIM.Services.Messaging
{
    public static class X_MessagesDefault
    {
        public const string msgheader = "X-USGSWIM-Messages";
        public const string ItemsKey = "wim_msgs";
    }
}

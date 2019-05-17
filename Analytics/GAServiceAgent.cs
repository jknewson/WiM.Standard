//------------------------------------------------------------------------------
//----- ServiceAgent -------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   The service agent is responsible for initiating the service call, 
//              capturing the data that's returned and forwarding the data back to 
//              the requestor.
//
//discussion:   delegated hunting and gathering responsibilities.   
//
//              Parameters protocol
//              https://developers.google.com/analytics/devguides/collection/protocol/v1/parameters
//
//              v=1               // version (1)
//              &tid=UA-XXXXX-Y   // Tracking ID/Property ID
//              &cid=555          // Anonymous Client ID

//              &t=event         // Event hit type
//              &ec=video        // Event Category. Required.
//              &ea=play         // Event Action. Required.
//              &el=holiday      // Event label.
//              &ev=300          // Event value.

using System;
using System.Collections.Generic;
using System.Linq;
using WIM.Services.Analytics;

namespace WIM.Utilities.ServiceAgent
{

    public class GoogleAnalyticsAgent : ServiceAgentBase, IAnalyticsAgent
    {
        #region Properties
        public string ClientID { get; private set; }
        #endregion
        #region Constructor
        public GoogleAnalyticsAgent(string clientID) : base("https://www.google-analytics.com")
        {
            this.ClientID = clientID;
        }
        #endregion
        #region Methods
        public void Submit(Dictionary<parameterType, string> parameters)
        {
            var requestInfo = new RequestInfo( getResourcrUrl(parameters)); ;
            this.ExecuteAsync(requestInfo);
        }
        #endregion
        #region HelperMethods
       
        private String getResourcrUrl(Dictionary<parameterType, string> parameters)
        {
            try
            {
                List<string> uri = new List<string>();
                uri.Add("/collect?v=1");
                uri.Add("tid="+this.ClientID);
                uri.Add("cid=" + 555);
                uri.Add("t=event");
                foreach (KeyValuePair<parameterType, string> entry in parameters)
                {
                    uri.Add(getGAParameter(entry));
                }//next

                String resulturl = string.Join("&", uri);
                return resulturl;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        protected virtual string getGAParameter(KeyValuePair<parameterType, string> entry)
        {
            if (entry.Key == parameterType.path)
            {
                //split path
                String[] uripath = entry.Value.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                var resource = String.Join("=", getGAParameterCode(parameterType.resource), uripath[0]);                
                var item = (uripath.Count() > 0)? String.Join("=", getGAParameterCode(parameterType.item), String.Join("/",uripath.Skip(1))): resource;

                return String.Join("&", resource, item);
            }
            else
                return String.Join("=", getGAParameterCode(entry.Key), entry.Value);
        }
        protected virtual string getGAParameterCode(parameterType ptype)
        {
            switch (ptype)
            {
                case parameterType.serviceHost:
                    return "cd1";//https://developers.google.com/analytics/devguides/collection/protocol/v1/parameters#cd_
                case parameterType.operation:
                    //Event Category. Required.
                    return "ec";
                case parameterType.resource:
                    // Event Action. Required.
                    return "ea";
                case parameterType.item:
                    // Event Label
                    return "el";
                case parameterType.queryparams:
                    // custom
                    return "_qps";
                case parameterType.referrer_ip_address:
                    //IP override
                    return "uip";
                default:
                    return "other1";
            }
        }

        #endregion

    }
 
    
}

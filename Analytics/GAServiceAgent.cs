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
//              &uid=555          // Anonymous User ID

//              &t=event         // Event hit type
//              &ec=video        // Event Category. Required.
//              &ea=play         // Event Action. Required.
//              &el=holiday      // Event label.
//              &ev=300          // Event value.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
       
        protected String getResourcrUrl(Dictionary<parameterType, string> parameters)
        {
            try
            {
                List<string> uri = new List<string>();
                uri.Add("/collect?v=1");
                uri.Add("tid="+this.ClientID);
                uri.Add("cid=" + getClientID((parameters.ContainsKey(parameterType.referrer_ip_address) ? parameters[parameterType.referrer_ip_address] : "0.0.0.0")));
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
        protected string getClientID(string plainText)
        {
            Int32 Timestamp = (Int32)(DateTime.Today.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;            
            try
            {              
                return $"{plainText.Replace(".","")}.{Timestamp}";
            }
            catch (Exception)
            {
                Random random = new Random();
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var user= new string(Enumerable.Repeat(chars, 10)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
                return $"{user}.{Timestamp}";
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
                case parameterType.operation:                   //request method
                    //custom
                    return "cd1";
                case parameterType.datasource:                  //machinename
                    return "ds";
                case parameterType.basepath:                    //basepath
                    //Event Category. Required.
                    return "ec";
                case parameterType.resource:                    //request resource    
                    // Event Action. Required.
                    return "ea";
                case parameterType.item:                        //request resource
                    // Event Label
                    return "el";
                case parameterType.queryparams:                 //query parameters
                    // custom
                    return "cd2";
                case parameterType.referrer_ip_address:         // referrer IP
                    //IP override
                    return "uip";
                case parameterType.referrer:
                    //Specifies which referral source brought traffic to a website. This value is also used to compute the traffic source. 
                    return "dr";
                default:
                    return "other1";
            }
        }

        #endregion

    }
 
    
}

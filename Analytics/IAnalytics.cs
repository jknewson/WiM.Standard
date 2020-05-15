using System;
using System.Collections.Generic;
using System.Text;

namespace WIM.Services.Analytics
{
    public interface IAnalyticsAgent
    {
        string ClientID { get; }
        void Submit(Dictionary<parameterType, string> parameters);
    }    
}

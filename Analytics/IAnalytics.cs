using System;
using System.Collections.Generic;
using System.Text;

namespace WiM.Services.Analytics
{
    public interface IAnalyticsAgent
    {
        string ClientID { get; }
        void Submit(Dictionary<parameterType, string> parameters);
    }    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRasta.Security;

namespace WiM.Security
{
    public class WiMCredentials:Credentials
    {
        public string salt { get; set; }
    }
}

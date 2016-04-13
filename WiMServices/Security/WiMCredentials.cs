using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiM.Security
{
    public class WiMCredentials:OpenRasta.Security.Credentials
    {
        public string salt { get; set; }
    }
}

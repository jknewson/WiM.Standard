using System;
using System.Collections.Generic;
using System.Text;

namespace WIM.Security
{
    public class User
    {
        public string username { get; set; }
        public string password { get; set; }
        public string[] roles { get; set; }
        public string salt { get; set; }
    }
}

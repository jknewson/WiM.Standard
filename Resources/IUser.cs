using System;
using System.Collections.Generic;
using System.Text;

namespace WIM.Resources
{
    public interface IUser
    {
        int ID { get; set; }
        string Username { get; set; }
        string Role { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace WiM.Security.Authentication.Basic
{
    public interface IBasicUser
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        string Role { get; set; }
        Int32 RoleID { get; set; }
        Int32 ID { get; set; }
        string Username { get; set; }
        string Salt { get; set; }
        string password { get; set; }
    }
    public interface IBasicUserAgent
    {
        IBasicUser GetUserByUsername(string username);
    }
}

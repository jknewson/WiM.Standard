using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIM.Resources;

namespace WIM.Security.Authentication
{
    public interface IAuthenticationAgent
    {
        IUser GetUserByID(int id);
        IUser GetUserByUsername(string username);
        IUser AuthenticateUser(string username, string password);

    }
}

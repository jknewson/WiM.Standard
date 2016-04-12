//------------------------------------------------------------------------------
//----- STNRequiresRoleInterceptor ---------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2012 WiM - USGS

//    authors:  Jeremy K. Newson USGS Wisconsin Internet Mapping
//              
//  
//   purpose:   Intercepts role authorization using dependancy injection
//
//discussion:   
//
//     

#region Comments
// 7.03.12 - jkn - Created, adapeted from openRasta.Security.RequiresRoleInterceptor

#endregion
using System;
using System.Collections.Generic;

using OpenRasta.OperationModel;
using OpenRasta.OperationModel.Interceptors;
using OpenRasta.Web;


namespace WiM.Authentication
{
    public class RequiresRoleInterceptor  : OperationInterceptor
    {
        
        #region Constructors
        
        public RequiresRoleInterceptor(ICommunicationContext context)
        {
            _context = context;
        }

        #endregion

        #region Properties & Fields
        ICommunicationContext _context;

        public List<string> Roles { get; set; }

        #endregion

        #region Method

        public override bool BeforeExecute(IOperation operation)
        {
            Boolean isAuthorized = false;
            if (_context.User == null || _context.User.Identity == null || !_context.User.Identity.IsAuthenticated) 
            {
                DenyAuthorization(_context);
                return false;
            }


            foreach (string role in Roles)
            {
                //one role is all that is needed
                if (role == null || _context.User.IsInRole(role))
                {
                    isAuthorized = true;
                    break;
                }
                
            }//next

           
            if (!isAuthorized)
                DenyAuthorization(_context);

            
            return isAuthorized;
        }
        protected virtual void DenyAuthorization(ICommunicationContext context)
        {
            _context.OperationResult = new OperationResult.Unauthorized();
        }

        #endregion

    }//end class RequiresRolesInterceptor
}//end namespace
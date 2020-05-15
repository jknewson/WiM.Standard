//------------------------------------------------------------------------------
//----- ControllerBase ---------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   contains base methods for aspnetcore controllers
//
//discussion:   
//  http://www.codeproject.com/Articles/704865/Salted-Password-Hashing-Doing-it-Right
//     

#region Comments
// 05.04.2017 - updated to .net standard
// 7.03.12 - jkn created

#endregion
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIM.Resources;

namespace WIM.Services.Controllers
{

    public abstract class ControllerBase: Microsoft.AspNetCore.Mvc.Controller
    {    
        protected virtual List<string> parse(string items)
        {
            if (items == null) items = string.Empty;
            char[] delimiterChars = { ';', ',' };
            return items.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(i=>i.Trim().ToLower()).ToList();
        }
        protected virtual Boolean isValid(object item)
        {
            try
            {
                //Must clear ModelState before validation to ensure any manual updates are applied during validation.
                ModelState.Clear();
                var isvalid = this.TryValidateModel(item);
                return isvalid;
            }
            catch (Exception ex)
            {
                var error = ex;
                return false;
            }
        }
        protected virtual async Task<IActionResult> HandleExceptionAsync(Exception ex)
        {
            return await Task.Run(() => { return HandleException(ex); });
        }
        protected virtual IActionResult HandleException(Exception ex)
        {
            if (ex is WIM.Exceptions.Services.BadRequestException)
            {
                sm(ex.Message, MessageType.warning);
                return new BadRequestObjectResult(new Error(errorEnum.e_badRequest, ex.Message));
            }
            else if (ex is WIM.Exceptions.Services.NotFoundRequestException)
            {
                sm(ex.Message, MessageType.warning);
                return new NotFoundObjectResult(new Error(errorEnum.e_notFound, ex.Message));
            }
            else if (ex is WIM.Exceptions.Services.UnAuthorizedRequestException)
            {
                sm(ex.Message, MessageType.warning);
                return new UnauthorizedObjectResult(new Error(errorEnum.e_unauthorize, ex.Message));
            }
            else
            {
                sm(ex.Message, MessageType.error);
                return StatusCode(500, new Error(errorEnum.e_internalError, "An error occured while processing your request. See messages for more information."));
            }
        }
        protected struct Error
        {
            public int Code { get; private set; }
            public string Message { get; private set; }
            public string Content { get; private set; }

            public Error(errorEnum c, string msg) {
                this.Code = (int)c;
                this.Message = msg;
                this.Content = getContent(c);
            }
            public Error(errorEnum c)
            {
                this.Code = (int)c;
                this.Message = getDefaultmsg(c);
                this.Content = getContent(c);
            }

            private static string getContent(errorEnum code) {
                switch (code)
                {
                    case errorEnum.e_badRequest: return "Bad Request Received";
                    case errorEnum.e_notFound: return "Not Found";
                    case errorEnum.e_notAllowed: return "Method Not Allowed.";
                    case errorEnum.e_internalError: return "Internal Server Error Occured";
                    case errorEnum.e_unauthorize: return "Unauthorized";
                    default: return "Error not specified";                        
                }

            }
            private static string getDefaultmsg(errorEnum code)
            {
                switch (code)
                {
                    case errorEnum.e_badRequest: return "Object is invalid, please check you have populated all required properties and try again.";
                    case errorEnum.e_notFound: return "Object was not found.";
                    case errorEnum.e_notAllowed: return "Method not allowed.";
                    case errorEnum.e_internalError: return "Internal server error occured";
                    case errorEnum.e_unauthorize: return "Unauthorized to perform this action.";
                    default: return "Error not specified";

                }

            }


        }
        protected enum errorEnum
        {
            e_badRequest=400,
            e_unauthorize =401,
            e_notFound=404,
            e_notAllowed=405,
            e_internalError=500,
            e_error=0
        }
        /// <summary>
        /// Sends a message to the X-usgswim-message header
        /// </summary>
        protected void sm(string msg, MessageType type = MessageType.info)
        {
            sm(new Message() { msg = msg, type = type });
        }
        /// <summary>
        /// Sends a message to the X-usgswim-message header
        /// </summary>
        protected void sm(Message msg)
        {
            if (!this.HttpContext.Items.ContainsKey(WIM.Services.Messaging.X_MessagesDefault.ItemsKey))
                this.HttpContext.Items[WIM.Services.Messaging.X_MessagesDefault.ItemsKey] = new List<Message>();
            ((List<Message>)this.HttpContext.Items[WIM.Services.Messaging.X_MessagesDefault.ItemsKey]).Add(msg);
        }
    }
}

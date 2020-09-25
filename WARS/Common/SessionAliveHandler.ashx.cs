/*
File Name   :   SessionAliveHandler.cs
Purpose     :   Resets session and keeps session alive until active session application is closed from browser

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     06-Feb-2019     Harish(Infosys Limited)   Initial Creation
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WARS.Common
{
    /// <summary>
    /// Summary description for SessionAliveHandler
    /// </summary>
    public class SessionAliveHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            //handler's cache should be disabled, otherwise the browser will cache the handler and won't make a new request to the server
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            context.Response.Cache.SetNoStore();
            context.Response.Cache.SetNoServerCaching();
        }

        //need to implement this as this is an interface method of IHttpHandler
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
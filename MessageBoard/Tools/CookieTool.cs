using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace MessageBoard.Tools
{
    public static class CookieTool
    {
        public static (bool isValid, string userName) CheckUserNameExist(HttpRequestBase httpRequest)
        {
            bool isValid = false;
            string userName = string.Empty;
            string cookieName = FormsAuthentication.FormsCookieName;
            HttpCookie authCookie = httpRequest.Cookies[cookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                userName = ticket.Name;
                isValid = true;
            }

            return (isValid, userName);
        }
    }
}
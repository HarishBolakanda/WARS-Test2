using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace WARS
{
    public class Global : System.Web.HttpApplication
    {
        //This is the character symbol used to pass null value as parameter value from screen to database procedure
        public static string DBNullParamValue = "-";
        public static string DBDelimiter = " " + Convert.ToChar(0); //Blank space is added to the delimiter so that two delimiters won't be considered as one by oracle if there is no data present between them

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {


        }


        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {


        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WARS
{
    public partial class ExceptionPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //WUIN-930: hide home button if an authentication error
            if (Session["UserRole"] == null)
            {
                this.Master.FindControl("lnkBtnHome").Visible = false;
            }

            string sessionTimedOut = Request.QueryString["SessionTimedOut"];

            if (sessionTimedOut != null)
            {
                Exp_reason.Text = "Your session has timed out. Please reload page to continue.";
            }
            else
            {
                if (Session["Exception_Reason"] == null || Session["Exception_Reason"].ToString().Trim().Equals(""))
                    Exp_reason.Text = "There is an error in the application. Please login again.";
                else
                    Exp_reason.Text = Session["Exception_Reason"].ToString();
            }

            //this.Master.FindControl("tblWelcomeInfo").Visible = false;

            // If the user is redirected to this page, then the UserName session is set to null
            // as the user should not be allowed to access any other pages once he is redirected to 
            // the exception page, unless he logged in again.
            Session["UserRole"] = null;
            Session["Exception_Reason"] = null;            

        }

        

    }
}
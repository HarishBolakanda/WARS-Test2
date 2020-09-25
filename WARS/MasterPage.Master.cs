using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace WARS
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        //Global variables
        public string warningMsgOnUnSavedData = "You have made changes that are not saved. Return to Save or Exit without saving.";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //string[] sUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\');
                //lblLoginUser.Text = sUser[sUser.Length - 1];
                
                if (Session["DatabaseName"] != null && Convert.ToString(Session["DatabaseName"]) != string.Empty)
                {
                    lblDBname.Text = " - " + Convert.ToString(Session["DatabaseName"]);
                }
                else
                {
                    Utilities util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    lblDBname.Text = " - " + dbName.Split('.')[0].ToString();
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                }

                hdnSessionTimeOutVal.Value = Session.Timeout.ToString();

            }
           
           
        }
        
        protected void lnkBtnHome_Click(object sender, EventArgs e)
        {
            if (hdnOpenMenuScreen.Value == "Y")
            {
                Response.Redirect(@"~/Common/MenuScreen.aspx", false);
            }
        }

        protected void imgbtnWARSGlobal_Click(object sender, ImageClickEventArgs e)
        {
            Session.Abandon();
            Response.Redirect(@"~/Common/WARSAffiliates.aspx", false);
        }

        protected void btnClientSideError_Click(object sender, EventArgs e)
        {
            //Session["Exception_Reason"] = "Browser side error";
            Session["Exception_Reason"] = hdnBrowserError.Value.ToString();
            Response.Redirect(@"~/Common/ExceptionPage.aspx", true);
        }
        
       
    }
}
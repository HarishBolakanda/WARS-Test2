using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WARS.UserControls
{
    public partial class MessageBoxControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void SetMessage(string sMessage, MessageType messageType, PositionType posType, string sVal = "")
        {
            lblMessage.Text = sMessage;
            lblMessage.Visible = true;
            mpeMessageBoxPopup.Show();
        }
    }
}
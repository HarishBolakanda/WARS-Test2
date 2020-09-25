/*
File Name   :   AutoParticipantAudit.cs
Purpose     :   To display Participant Group Audit data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     10-Jun-2019     Rakesh(Infosys Limited)   Initial Creation
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Net;
using System.IO;
using System.Xml;
using WARS.BusinessLayer;
using System.Threading;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.Web.SessionState;
using System.Text;
using System.Drawing;

namespace WARS.Audit
{
    public partial class AutoParticipantAudit : System.Web.UI.Page
    {
        #region Global Declarations
        AutoParticipantAuditBL autoParticipantAuditBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        #endregion Global Declarations
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Auto Participant Audit";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Auto Participant Audit";
                }

                PnlAutoParticipantAudit.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadData();
                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }

                    util = null;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }
        }
        #region Methods

        private void LoadData()
        {

            if (Request.QueryString != null && Request.QueryString.Count > 0)
            {
                string autoPartId = Request.QueryString[0];
                autoParticipantAuditBL = new AutoParticipantAuditBL();
                DataSet auditData = autoParticipantAuditBL.GetAutoParticipantAuditData(autoPartId, out errorId);
                autoParticipantAuditBL = null;

                if (auditData.Tables.Count != 0 && errorId != 2)
                {
                    BindGrid(auditData.Tables[0]);
                }
                else
                {
                    ExceptionHandler("Error in fetching Participation Group audit data", string.Empty);
                }

            }
        }


        private void BindGrid(DataTable gridData)
        {
            if (gridData.Rows.Count > 0)
            {
                gvAutoParticipantAudit.DataSource = gridData;
                gvAutoParticipantAudit.DataBind();
                CompareAuditRows();
            }
            else
            {
                dtEmpty = new DataTable();
                gvAutoParticipantAudit.DataSource = dtEmpty;
                gvAutoParticipantAudit.DataBind();
            }
        }

        private void CompareAuditRows()
        {
            for (int i = 0; i < gvAutoParticipantAudit.Rows.Count - 1; i++)
            {
                GridViewRow currentRow = gvAutoParticipantAudit.Rows[i];
                GridViewRow nextRow = gvAutoParticipantAudit.Rows[i + 1];


                //Compare Marketing Owner
                if ((currentRow.FindControl("lblMarketingOwner") as Label).Text != (nextRow.FindControl("lblMarketingOwner") as Label).Text)
                {
                    (currentRow.FindControl("lblMarketingOwner") as Label).ForeColor = Color.Red;
                }

                //Compare Artist
                if ((currentRow.FindControl("lblArtist") as Label).Text != (nextRow.FindControl("lblArtist") as Label).Text)
                {
                    (currentRow.FindControl("lblArtist") as Label).ForeColor = Color.Red;
                }

                //Compare WEA Sales Label
                if ((currentRow.FindControl("lblWeaSalesLabel") as Label).Text != (nextRow.FindControl("lblWeaSalesLabel") as Label).Text)
                {
                    (currentRow.FindControl("lblWeaSalesLabel") as Label).ForeColor = Color.Red;
                }

                //Compare Project Title
                if ((currentRow.FindControl("lblProjectTitle") as Label).Text != (nextRow.FindControl("lblProjectTitle") as Label).Text)
                {
                    (currentRow.FindControl("lblProjectTitle") as Label).ForeColor = Color.Red;
                }

            }
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

        }

        #endregion Methods

    }
}
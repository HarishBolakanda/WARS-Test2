/*
File Name   :   AutoParticipantMaintAudit.cs
Purpose     :   To display Auto Participant Details Audit data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     12-Jun-2019      Rakesh                     Initial Creation
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
    public partial class AutoParticipantMaintAudit : System.Web.UI.Page
    {
        #region Global Declarations
        AutoParticipantMaintAuditBL autoPartDetailsAuditBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        #endregion Global Declarations


        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Auto Participant Audit Details";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Auto Participant Audit Details";
                }

                PnlAutoPartDetailsAudit.Style.Add("height", hdnGridPnlHeight.Value);

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
        #endregion Events
        #region Methods

        private void LoadData()
        {
            if ((Request.QueryString != null && Request.QueryString.Count > 0))
            {
                string autoPartId = Request.QueryString[0];
                hdnAutoPartId.Value = autoPartId;
                autoPartDetailsAuditBL = new AutoParticipantMaintAuditBL();
                DataSet auditData = autoPartDetailsAuditBL.GetAutoPartMaintAuditData(autoPartId, out errorId);
                autoPartDetailsAuditBL = null;

                if (auditData.Tables.Count != 0 && errorId != 2)
                {
                    BindGrid(auditData.Tables[0]);
                }
                else
                {
                    dtEmpty = new DataTable();
                    gvAutoPartDetailsAudit.DataSource = dtEmpty;
                    gvAutoPartDetailsAudit.DataBind();
                }
            }
        }


        private string TimeFormat(string timeVal)
        {
            if (!timeVal.Contains(':'))
            {
                string totalTime = String.Format("{0:D7}", Convert.ToInt32(timeVal));//append leading zeros if the length is < 7
                string seconds = totalTime.Substring((totalTime.Length - 2), 2);
                string mins = totalTime.Substring((totalTime.Length - 4), 2);
                string hrs = totalTime.Substring((totalTime.Length - 7), 3);
                return hrs + ":" + mins + ":" + seconds;
            }
            else
            {
                return timeVal;
            }

        }

        protected void gvAutoPartDetailsAudit_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                Label lblShareTime;
                Label lblShareTotalTime;
                Label lblIsActive;
                Label lblIncludeEscalation;
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    lblShareTime = (e.Row.FindControl("lblShareTime") as Label);
                    lblShareTotalTime = (e.Row.FindControl("lblShareTotalTime") as Label);
                    lblIsActive = (e.Row.FindControl("lblIsActive") as Label);
                    lblIncludeEscalation = (e.Row.FindControl("lblIncludeEscalation") as Label);
                    //format the time and total time fields as hhh:mm:ss
                    if (lblShareTime.Text != string.Empty && lblShareTime.Text != "_______")
                    {
                        lblShareTime.Text = TimeFormat(lblShareTime.Text);
                    }

                    if (lblShareTotalTime.Text != string.Empty && lblShareTotalTime.Text != "_______")
                    {
                        lblShareTotalTime.Text = TimeFormat(lblShareTotalTime.Text);
                    }


                    if (lblIsActive.Text == "Y")
                    {
                        lblIsActive.Text = "Active";
                    }
                    else
                    {
                        lblIsActive.Text = "Inactive";
                    }

                    if (lblIncludeEscalation.Text == "Y")
                    {
                        lblIncludeEscalation.Text = "Yes";
                    }
                    else
                    {
                        lblIncludeEscalation.Text = "No";
                    }

                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding data to grid", ex.Message);
            }
        }


        private void BindGrid(DataTable gridData)
        {
            if (gridData.Rows.Count > 0)
            {
                gvAutoPartDetailsAudit.DataSource = gridData;
                gvAutoPartDetailsAudit.DataBind();
                CompareAuditRows();
            }
            else
            {
                dtEmpty = new DataTable();
                gvAutoPartDetailsAudit.DataSource = dtEmpty;
                gvAutoPartDetailsAudit.DataBind();
            }
        }


        private void CompareAuditRows()
        {
            //data to compare participants
            //unique fiedls of a participant - ROYALTOR_ID, OPTION_PERIOD_CODE, SELLER_GROUP_CODE, ESC CODE, INC IN ESCALATION, PARTICIPATION_TYPE            
            string autoPartDetailIdCurrent = string.Empty;
            string royaltorIdCurrent = string.Empty;
            string optionPeriodCodeCurrent = string.Empty;
            string sellerGroupCodeCurrent = string.Empty;
            string tuneIdCurrent = string.Empty;
            string participationTypeCurrent = string.Empty;
            string escCodeCurrentCurrent = string.Empty;
            string includeEscalationCurrent = string.Empty;

            string autoPartDetailIdNext = string.Empty;
            string royaltorIdNext = string.Empty;
            string optionPeriodCodeNext = string.Empty;
            string sellerGroupCodeNext = string.Empty;
            string tuneIdNext = string.Empty;
            string participationTypeNext = string.Empty;
            string escCodeCurrentNext = string.Empty;
            string includeEscalationNext = string.Empty;

            for (int i = 0; i < gvAutoPartDetailsAudit.Rows.Count-1; i++)
            {
                GridViewRow currentRow = gvAutoPartDetailsAudit.Rows[i];

                //Rows from main table are displayed on top. Need to ignore them for coloring the changes
                if ((currentRow.FindControl("hdnDisplayOrder") as HiddenField).Value == "1")
                {
                    continue;
                }

                autoPartDetailIdCurrent = (currentRow.FindControl("hdnAutoPartDetailId") as HiddenField).Value;
                royaltorIdCurrent = (currentRow.FindControl("hdnRoyaltorId") as HiddenField).Value;
                optionPeriodCodeCurrent = (currentRow.FindControl("hdnOptionPeriodCode") as HiddenField).Value;
                sellerGroupCodeCurrent = (currentRow.FindControl("hdnSellerGroupCode") as HiddenField).Value;
                escCodeCurrentCurrent = (currentRow.FindControl("hdnEscCode") as HiddenField).Value;
                includeEscalationCurrent = (currentRow.FindControl("hdnIncludeEscalation") as HiddenField).Value;
                participationTypeCurrent = (currentRow.FindControl("hdnParticipationType") as HiddenField).Value;

                for (int j = i + 1; j < gvAutoPartDetailsAudit.Rows.Count; j++)
                {
                    GridViewRow nextRow = gvAutoPartDetailsAudit.Rows[j];
                    autoPartDetailIdNext = (nextRow.FindControl("hdnAutoPartDetailId") as HiddenField).Value;
                    royaltorIdNext = (nextRow.FindControl("hdnRoyaltorId") as HiddenField).Value;
                    optionPeriodCodeNext = (nextRow.FindControl("hdnOptionPeriodCode") as HiddenField).Value;
                    sellerGroupCodeNext = (nextRow.FindControl("hdnSellerGroupCode") as HiddenField).Value;
                    participationTypeNext = (nextRow.FindControl("hdnParticipationType") as HiddenField).Value;
                    escCodeCurrentNext = (currentRow.FindControl("hdnEscCode") as HiddenField).Value;
                    includeEscalationNext = (currentRow.FindControl("hdnIncludeEscalation") as HiddenField).Value;

                    //need to compare with other unique fields of the participation as well to highlight the differences
                    //exclude PARTICIPATION_TYPE in comparision
                    if (autoPartDetailIdCurrent == autoPartDetailIdNext ||
                         (royaltorIdCurrent == royaltorIdNext && optionPeriodCodeCurrent == optionPeriodCodeNext && sellerGroupCodeCurrent == sellerGroupCodeNext && escCodeCurrentCurrent == escCodeCurrentNext && includeEscalationCurrent == includeEscalationNext)
                    )
                    {
                        //Compare Royaltor
                        if ((currentRow.FindControl("lblRoyaltor") as Label).Text != (nextRow.FindControl("lblRoyaltor") as Label).Text)
                        {
                            (currentRow.FindControl("lblRoyaltor") as Label).ForeColor = Color.Red;
                        }

                        //Compare Option Period
                        if ((currentRow.FindControl("lblOptionPeriod") as Label).Text != (nextRow.FindControl("lblOptionPeriod") as Label).Text)
                        {
                            (currentRow.FindControl("lblOptionPeriod") as Label).ForeColor = Color.Red;
                        }

                        //Compare IsActive
                        if ((currentRow.FindControl("lblIsActive") as Label).Text != (nextRow.FindControl("lblIsActive") as Label).Text)
                        {
                            (currentRow.FindControl("lblIsActive") as Label).ForeColor = Color.Red;
                        }

                        //Compare Territory
                        if ((currentRow.FindControl("lblTerritory") as Label).Text != (nextRow.FindControl("lblTerritory") as Label).Text)
                        {
                            (currentRow.FindControl("lblTerritory") as Label).ForeColor = Color.Red;
                        }

                        //Compare EscaltionCode
                        if ((currentRow.FindControl("lblEscCode") as Label).Text != (nextRow.FindControl("lblEscCode") as Label).Text)
                        {
                            (currentRow.FindControl("lblEscCode") as Label).ForeColor = Color.Red;
                        }

                        //Compare Escaltion Units
                        if ((currentRow.FindControl("lblIncludeEscalation") as Label).Text != (nextRow.FindControl("lblIncludeEscalation") as Label).Text)
                        {
                            (currentRow.FindControl("lblIncludeEscalation") as Label).ForeColor = Color.Red;
                        }

                        //Compare Tracks
                        if ((currentRow.FindControl("lblShareTracks") as Label).Text != (nextRow.FindControl("lblShareTracks") as Label).Text)
                        {
                            (currentRow.FindControl("lblShareTracks") as Label).ForeColor = Color.Red;
                        }

                        //Compare Total Tracks
                        if ((currentRow.FindControl("lblShareTotalTracks") as Label).Text != (nextRow.FindControl("lblShareTotalTracks") as Label).Text)
                        {
                            (currentRow.FindControl("lblShareTotalTracks") as Label).ForeColor = Color.Red;
                        }

                        //Compare TrackTitle
                        if ((currentRow.FindControl("lblTrackTitle") as Label).Text != (nextRow.FindControl("lblTrackTitle") as Label).Text)
                        {
                            (currentRow.FindControl("lblTrackTitle") as Label).ForeColor = Color.Red;
                        }

                        //Compare Time
                        if ((currentRow.FindControl("lblShareTime") as Label).Text != (nextRow.FindControl("lblShareTime") as Label).Text)
                        {
                            (currentRow.FindControl("lblShareTime") as Label).ForeColor = Color.Red;
                        }

                        //Compare TotalTime
                        if ((currentRow.FindControl("lblShareTotalTime") as Label).Text != (nextRow.FindControl("lblShareTotalTime") as Label).Text)
                        {
                            (currentRow.FindControl("lblShareTotalTime") as Label).ForeColor = Color.Red;
                        }


                        break;

                    }

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


        #region Handling Session Timeout
        //Handling session timeout
        [System.Web.Services.WebMethod(EnableSession = true)]
        public static int ResetSession()
        {
            int timeout = HttpContext.Current.Session.Timeout * 1000 * 60;
            return timeout;
        }

        #endregion Handling Session Timeout

    }
}
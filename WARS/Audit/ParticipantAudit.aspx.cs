/*
File Name   :   ParticipantAudit.cs
Purpose     :   To display Participant Audit data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     12-Mar-2018      Bala                     Initial Creation
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
    public partial class ParticipantAudit : System.Web.UI.Page
    {
        #region Global Declarations
        ParticipantAuditBL participantAuditBL;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Participant Audit Details";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Participant Audit Details";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlParticipantDetailsAudit.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    tdData.Style.Add("display", "none");
                    trParAudit.Style.Add("display", "none");

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

        protected void btnParticipantDetails_Click(object sender, EventArgs e)
        {
            try
            {
                string pageName = string.Empty;
                if (Request.QueryString != null && Request.QueryString.Count > 0)
                {
                    pageName = Request.QueryString[1];
                }

                if (txtCatalogueSearch.Text.Trim() != string.Empty && pageName == "ParticipantMaint")
                {
                    Response.Redirect("../Participants/ParticipantMaintenance.aspx?catNo=" + txtCatalogueSearch.Text + "", false);
                }
                else if (txtCatalogueSearch.Text.Trim() != string.Empty && pageName == "ParticipantSum")
                {
                    Response.Redirect("../Participants/ParticipantSummary.aspx?catNo=" + txtCatalogueSearch.Text + "", false);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting to screen.", ex.Message);
            }
        }

        #endregion Events

        #region Methods

        private void LoadData()
        {
           
            if ((Request.QueryString != null && Request.QueryString.Count > 0) || (txtCatalogueSearch.Text != string.Empty))
            {
                string catNo = Request.QueryString[0];
                txtCatalogueSearch.Text = catNo;
                CatalogueSelected();
            }
        }
        
        protected void btnHdnCatalogue_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate("valSearch");
                if (!Page.IsValid)
                {
                    return;
                }
                else if (!string.IsNullOrWhiteSpace(txtCatalogueSearch.Text))
                {
                    CatalogueSelected();
                }
                else
                {
                    tdData.Style.Add("display", "none");
                    trParAudit.Style.Add("display", "none");

                    dtEmpty = new DataTable();
                    gvPartAudit.DataSource = dtEmpty;
                    gvPartAudit.DataBind();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in search.", ex.Message);
            }
        }
        
        private void CatalogueSelected()
        {
            participantAuditBL = new ParticipantAuditBL();
            
            tdData.Style.Remove("display");
            trParAudit.Style.Remove("display");

            DataSet auditData = participantAuditBL.GetParticipantData(txtCatalogueSearch.Text.ToUpper(), (txtFromDate.Text == string.Empty || txtFromDate.Text.Contains("_")) ? string.Empty : txtFromDate.Text,
                                                                        (txtToDate.Text == string.Empty || txtToDate.Text.Contains("_")) ? string.Empty : txtToDate.Text, out errorId);
            participantAuditBL = null;

            if (auditData.Tables.Count != 0 && errorId != 2)
            {
                BindGrid(auditData.Tables[0]);
            }
            else
            {
                dtEmpty = new DataTable();
                gvPartAudit.DataSource = dtEmpty;
                gvPartAudit.DataBind();
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
        
        protected void gvPartAudit_RowDataBound(object sender, GridViewRowEventArgs e)
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
        
        protected void valFromDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                DateTime temp;
                if (txtFromDate.Text.Trim() != "__/__/____" && txtFromDate.Text.Trim() != "")
                {
                    if (DateTime.TryParse(txtFromDate.Text, out temp))
                    {

                    }
                    else
                    {
                        args.IsValid = false;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in validating date.", ex.Message);
            }
        }

        protected void valToDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                DateTime temp;
                if (txtToDate.Text.Trim() != "__/__/____" && txtToDate.Text.Trim() != "")
                {
                    if (DateTime.TryParse(txtToDate.Text, out temp))
                    {
                        if (txtFromDate.Text.Trim() != "__/__/____" && txtFromDate.Text.Trim() != "")
                        {
                            if (DateTime.TryParse(txtFromDate.Text, out temp))
                            {
                                if (Convert.ToDateTime(txtFromDate.Text) > Convert.ToDateTime(txtToDate.Text))
                                {
                                    valToDate.ToolTip = "From date should be earlier than To date";
                                    args.IsValid = false;
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        args.IsValid = false;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in validating date.", ex.Message);
            }
        }
        
        private void BindGrid(DataTable gridData)
        {
            if (gridData.Rows.Count > 0)
            {
                gvPartAudit.DataSource = gridData;
                gvPartAudit.DataBind();
                CompareAuditRows();
            }
            else
            {
                dtEmpty = new DataTable();
                gvPartAudit.DataSource = dtEmpty;
                gvPartAudit.DataBind();
            }
        }
        
        protected void fuzzySearchCatalogue_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchCatalogue();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor search.", ex.Message);
            }
        }

        private void CompareAuditRows()
        {
            //data to compare participants
            //unique fiels of a participant - ROYALTOR_ID, OPTION_PERIOD_CODE, SELLER_GROUP_CODE, TUNE_ID, PARTICIPATION_TYPE            
            string participationIdCurrent = string.Empty;
            string royaltorIdCurrent = string.Empty;
            string optionPeriodCodeCurrent = string.Empty;
            string sellerGroupCodeCurrent = string.Empty;
            string tuneIdCurrent = string.Empty;
            string participationTypeCurrent = string.Empty;

            string participationIdNext = string.Empty;
            string royaltorIdNext = string.Empty;
            string optionPeriodCodeNext = string.Empty;
            string sellerGroupCodeNext = string.Empty;
            string tuneIdNext = string.Empty;
            string participationTypeNext = string.Empty;

            for (int i = 0; i < gvPartAudit.Rows.Count - 1; i++)
            {
                GridViewRow currentRow = gvPartAudit.Rows[i];

                //Rows from main table are displayed on top. Need to ignore them for coloring the changes
                if ((currentRow.FindControl("hdnDisplayOrder") as HiddenField).Value == "1")
                {
                    continue;
                }

                participationIdCurrent = (currentRow.FindControl("hdnParticipationId") as HiddenField).Value;
                royaltorIdCurrent = (currentRow.FindControl("hdnRoyaltorId") as HiddenField).Value;
                optionPeriodCodeCurrent = (currentRow.FindControl("hdnOptionPeriodCode") as HiddenField).Value;
                sellerGroupCodeCurrent = (currentRow.FindControl("hdnSellerGroupCode") as HiddenField).Value;
                tuneIdCurrent = (currentRow.FindControl("hdnTuneId") as HiddenField).Value;
                participationTypeCurrent = (currentRow.FindControl("hdnParticipationType") as HiddenField).Value;

                for (int j = i + 1; j < gvPartAudit.Rows.Count; j++)
                {
                    GridViewRow nextRow = gvPartAudit.Rows[j];
                    participationIdNext = (nextRow.FindControl("hdnParticipationId") as HiddenField).Value;
                    royaltorIdNext = (nextRow.FindControl("hdnRoyaltorId") as HiddenField).Value;
                    optionPeriodCodeNext = (nextRow.FindControl("hdnOptionPeriodCode") as HiddenField).Value;
                    sellerGroupCodeNext = (nextRow.FindControl("hdnSellerGroupCode") as HiddenField).Value;
                    tuneIdNext = (nextRow.FindControl("hdnTuneId") as HiddenField).Value;
                    participationTypeNext = (nextRow.FindControl("hdnParticipationType") as HiddenField).Value;

                    //WUIN-806 - as a participant have different participation id's(one end dated and other not), 
                    //need to compare with other unique fields of the participation as well to highlight the differences
                    //exclude PARTICIPATION_TYPE in comparision
                    if (participationIdCurrent == participationIdNext ||
                         (royaltorIdCurrent == royaltorIdNext && optionPeriodCodeCurrent == optionPeriodCodeNext && sellerGroupCodeCurrent == sellerGroupCodeNext && tuneIdCurrent == tuneIdNext)
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

                        //Compare Status
                        if ((currentRow.FindControl("lblStatus") as Label).Text != (nextRow.FindControl("lblStatus") as Label).Text)
                        {
                            (currentRow.FindControl("lblStatus") as Label).ForeColor = Color.Red;
                        }

                        break;

                    }

                }

            }
        }
                
        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtCatalogueSearch.Text = string.Empty;
                    return;
                }
                txtCatalogueSearch.Text = lbFuzzySearch.SelectedValue.ToString();
                CatalogueSelected();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor.", ex.Message);
            }
        }

        private void FuzzySearchCatalogue()
        {
            if (txtCatalogueSearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in catalogue search field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllCatalogueList(txtCatalogueSearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }
        
        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtCatalogueSearch.Text = string.Empty;
                tdData.Style.Add("display", "none");
                trParAudit.Style.Add("display", "none");
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting catalogue from searh list.", ex.Message);
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
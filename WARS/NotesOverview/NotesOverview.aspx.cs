/*
File Name   :   NotesOverview.cs
Purpose     :   To view the notes in the application

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     22-Jan-2019       Ravi                 WUIN-927- initial creation
2.0     20-Jan-2020       Harshika             WUIN-927- Selecting reporting schedule for Workflow button.
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
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;

namespace WARS.NotesOverview
{
    public partial class NotesOverview : System.Web.UI.Page
    {
        #region Global Declarations
        Utilities util;
        Int32 errorId;
        NotesOverviewBL notesOverviewBL;
        #endregion Global Declarations

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Notes Overview";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Notes Overview";
                }

                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        txtRoyaltor.Focus();
                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }
                }
                else
                {
                    txtNotes.Height = new Unit(hdnGridPnlHeight.Value);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }
        }

        /// <summary>
        /// Closes a fuzzy Search Royaltor Pop-up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtRoyaltor.Text = string.Empty;
                txtNotes.Text = string.Empty;
                mpeFuzzySearch.Hide();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing search list", ex.Message);
            }
        }

        /// <summary>
        /// When we select a Royaltor from Fuzzy Search List
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                txtRoyaltor.Text = lbFuzzySearch.SelectedValue.ToString();
                txtNotes.Text = string.Empty;

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting item from search list", ex.Message);
            }
        }

        /// <summary>
        /// When we click on Work FLow button, this method executes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnWorkFlow_Click(object sender, EventArgs e)
        {
            try
            {
                txtNotes.Text = string.Empty;

                if (txtRoyaltor.Text != " ")
                {
                    string royaltorId = txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1);
                    notesOverviewBL = new NotesOverviewBL();
                    DataSet workFlowDropdown = notesOverviewBL.GetWorkflowDropdown(royaltorId, out errorId);
                    notesOverviewBL = null;

                    if (workFlowDropdown.Tables.Count != 0 && errorId != 2)
                    {
                        if (workFlowDropdown.Tables[1].Rows.Count > 1)
                        {
                            ddlReportingSchedule.DataTextField = "Statement_period_list_item";
                            ddlReportingSchedule.DataValueField = "Statement_period_id";
                            ddlReportingSchedule.DataSource = workFlowDropdown.Tables[1];
                            ddlReportingSchedule.DataBind();
                            ddlReportingSchedule.Items.Insert(0, new ListItem("-"));

                            mpeReportingSchedule.Show();
                        }
                        else
                        {
                            if (workFlowDropdown.Tables[0].Rows.Count > 0)
                            {
                                txtNotes.Text = Convert.ToString(workFlowDropdown.Tables[0].Rows[0][0]);
                                txtNotes.Text = Regex.Replace(txtNotes.Text, @"&lt;.+?&gt;|&nbsp;|&apos;|&amp;nbsp;", "").Trim();
                            }
                            else
                            {
                                msgView.SetMessage("There are no WorkFlow Notes available for this Royaltor!", MessageType.Warning, PositionType.Auto);
                                return;
                            }

                        }
                    }
                    else
                    {
                        ExceptionHandler("Error in getting data for WorkFlow Notes", string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in getting data for WorkFlow Notes", ex.Message);
            }
        }

        /// <summary>
        /// When we click on Contract button, this method executes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContract_Click(object sender, EventArgs e)
        {
            try
            {
                txtNotes.Text = string.Empty;

                if (txtRoyaltor.Text != "")
                {
                    string royaltorId = txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1);
                    notesOverviewBL = new NotesOverviewBL();

                    DataSet contractNotesData = notesOverviewBL.GetNotesData(royaltorId, "CONTRACT", string.Empty, out errorId);
                    notesOverviewBL = null;

                    if (errorId != 2)
                    {
                        if (contractNotesData.Tables[0].Rows.Count > 0)
                        {
                            txtNotes.Text = Convert.ToString(contractNotesData.Tables[0].Rows[0][0]);
                            txtNotes.Text = Regex.Replace(txtNotes.Text, @"&lt;.+?&gt;|&nbsp;|&apos;|&amp;nbsp;", "").Trim();
                        }
                        else
                        {
                            msgView.SetMessage("There are no Contract Notes available for this Royaltor!", MessageType.Warning, PositionType.Auto);
                            return;
                        }
                    }
                    else
                    {
                        ExceptionHandler("Error in getting data for Contract Notes", string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in getting data for Contract Notes", ex.Message);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtRoyaltor.Text = string.Empty;
            txtNotes.Text = string.Empty;
        }

        /// <summary>
        /// When we press Proceed button on the Pop-up menu,this method executes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnWorkflowProceed_click(object sender, EventArgs e)
        {
            try
            {
                if (ddlReportingSchedule.SelectedValue != "")
                {
                    string royaltorId = txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1);
                    notesOverviewBL = new NotesOverviewBL();
                    DataSet workflowNotesData = notesOverviewBL.GetNotesData(royaltorId, "WORKFLOW", ddlReportingSchedule.SelectedValue, out errorId);
                    notesOverviewBL = null;
                    if (errorId != 2)
                    {
                        if (workflowNotesData.Tables[0].Rows.Count > 0)
                        {
                            txtNotes.Text = Convert.ToString(workflowNotesData.Tables[0].Rows[0][0]);
                            txtNotes.Text = Regex.Replace(txtNotes.Text, @"&lt;.+?&gt;|&nbsp;|&apos;|&amp;nbsp;", "").Trim();
                        }
                        else
                        {
                            msgView.SetMessage("There are no Workflow Notes available for this Royaltor!", MessageType.Warning, PositionType.Auto);
                            return;
                        }
                    }
                    else
                    {
                        ExceptionHandler("Error in getting data for Workflow Notes", string.Empty);
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in getting data for Workflow Notes", ex.Message);
            }
        }

        #endregion Events

        #region Private Methods
        /// <summary>
        /// Exception Method
        /// </summary>
        /// <param name="errorMsg">errorMsg</param>
        /// <param name="expMsg">expMsg</param>
        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            util = new Utilities();
            util.GenericExceptionHandler(errorMsg + "<br />" + expMsg);
            util = null;
        }

        #endregion Private Methods

        #region Fuzzy Search

        protected void btnFuzzyRoyaltorListPopup_Click(object sender, EventArgs e)
        {
            try
            {
                txtNotes.Text = string.Empty;
                FuzzySearchRoyaltor(txtRoyaltor.Text);
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor fuzzy search popup", ex.Message);
            }
        }
        private void FuzzySearchRoyaltor(string searchText)
        {
            lblFuzzySearchPopUp.Text = "Royaltor - Complete Search List";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllRoyaltorList(searchText.ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }
        #endregion Fuzzy Search
                
    }
}
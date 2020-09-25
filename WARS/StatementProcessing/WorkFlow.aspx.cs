/*
File Name   :   WorkFlow.cs
Purpose     :   to filter a list of royaltors / owner groups based on the status

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     28-Dec-2015     Harish(Infosys Limited)   Initial Creation
        08-Nov-2017     Harish                    WUIN-154 changes
                                                  change to the parameters passed to generate summary and invoice statements
        06-Dec-2017     Harish                    WUIN-382 changes
        02-Jan-2018     Harish                    WUIN-290 changes
        18-May-2018     Harish                    WUIN-648 changes
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
using AjaxControlToolkit;
using Microsoft.SharePoint.Client;
using System.Security;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;


namespace WARS
{
    public partial class WorkFlow : System.Web.UI.Page
    {
        #region Global Declarations
        string loggedUserID;
        WorkFlowBL workFlowBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        string rowIndexOfOwnerRow;
        string curRoyUnderOwnStatus;
        string prevRoyUnderOwnStatus;
        string ownRowStatusSet;
        string curRoyUnderOwnFlag;
        string prevRoyUnderOwnFlag;
        string curRoyUnderOwnFlag2;
        string prevRoyUnderOwnFlag2;
        string curRoyUnderOwnFlag3;//used to handle owner level recalculate summary
        string prevRoyUnderOwnFlag3;//used to handle owner level recalculate summary
        string ownRowFlagSet;
        string ownRowRecalFlagSet;//used to handle owner level recalculate summary
        int gridDefaultPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["WorkflowGridDefaultPageSize"].ToString());
        //SharePoint related - start
        string sharePointPDFFolderAffiliate = ConfigurationManager.AppSettings["sharePointPDFFolderAffiliate"].ToString();
        string sharePointDocumentLibrary = ConfigurationManager.AppSettings["SharePointDocumentLibrary"].ToString();
        string sharePointAccount = ConfigurationManager.AppSettings["SharePointAccount"].ToString();
        string sharePointAccountPwd = ConfigurationManager.AppSettings["SharePointAccountPwd"].ToString();
        string sharePointSite = ConfigurationManager.AppSettings["SharePointSite"].ToString();
        string stmtSaveLocation = string.Empty;
        ClientContext SharePointContext = null;
        Web SharePointClientWeb = null;
        Folder SharePointUploadFolder = null;
        List SharePointList = null;
        //SharePoint related - end
        #endregion Global Declarations

        #region PAGE EVENTS

        protected void Page_Load(object sender, EventArgs e)
        {            
            try
            {
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Workflow";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Workflow";
                }

                //reset summary statement file name to be opened
                Session["WorkflowSummReport"] = null;
                hdnIsCommentChanged.Value = "N";//reset to default
                hdnIsConfirmPopup.Value = "N";//reset to default

                if (IsPostBack)
                {
                    //set gridview panel height                    
                    PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                    if (hdnCommentUpload.Value == "Y")
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(Page), "SetHeightAfterUpload", "SetHeightAfterUpload();", true);
                    }

                    UserAuthorization();
                }

                txtOwnSearch.Focus();//tabbing sequence starts here

                if (!IsPostBack)
                {
                    //commented as per WOS-186
                    //set width of next run details control
                    //HtmlTableCell tdNextRun = (HtmlTableCell)stmtNextRun.FindControl("nextRunTd");
                    //tdNextRun.Style.Add("width", "25%");
                    //=========== End ===================

                    Session["UpdatedOwnerCode"] = null;
                    Session["WorkflowCommentRoyId"] = null;
                    Session["WorkflowCommentStmtPeriodId"] = null;

                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        hdnUserRole.Value = Session["UserRole"].ToString();
                        //stmtNextRun.LoadData();//commented as per WOS-186
                        PopulateDropDowns();
                        LoadGridData();
                        HideTeamOrMngrSignOff();
                        util = null;

                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }
                    UserAuthorization();

                }

            }
            catch (ThreadAbortException ex1)
            {

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }


        }

        protected void ddlResponsibility_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                PopulateDropDownsForResp(ddlResponsibility.SelectedValue, ddlMngrResponsibility.SelectedValue);

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting team responsibility dropdown.", ex.Message);
            }
        }

        protected void ddlMngrResponsibility_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                PopulateDropDownsForResp(ddlResponsibility.SelectedValue, ddlMngrResponsibility.SelectedValue);

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting manager responsibility dropdown.", ex.Message);
            }
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            try
            {                
                Session["UpdatedOwnerCode"] = null;
                hdnUpdatedStmtPeriod.Value = string.Empty;
                hdnPageIndex.Value = "1";
                hdnGridPageSize.Value = "";
                LoadSearchData();
                //holding if search mode state of the page. Used in retaining the state of grid after refresh                
                hdnPageMode.Value = PageMode.Search.ToString();
                UserAuthorization();
                //WUIN-1057
                ClearRerunRecalBulkControls();

            }
            catch (ThreadAbortException ex1)
            {
                // do nothing
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in Go button click.", ex.Message);
            }

        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ddlCompany.SelectedIndex = 0;
                ddlReportingSch.SelectedIndex = 0;
                txtOwnSearch.Text = string.Empty;
                txtRoyaltor.Text = string.Empty;
                ddlResponsibility.SelectedIndex = 0;
                ddlMngrResponsibility.SelectedIndex = 0;
                ddlPriority.SelectedIndex = 0;
                txtProducer.Text = string.Empty;
                ddlStatus.SelectedIndex = 0;
                txtEarnings.Text = string.Empty;
                ddlEarningsCompare.SelectedIndex = 0;
                txtClosingBalance.Text = string.Empty;
                ddlClosingBalCompare.SelectedIndex = 0;
                hdnPageMode.Value = string.Empty;
                Session["UpdatedOwnerCode"] = null;
                hdnUpdatedStmtPeriod.Value = string.Empty;
                hdnPageIndex.Value = "1";
                rptPager.Visible = false;                
                //WUIN-1057
                ClearRerunRecalBulkControls();
                hdnAllowGridPageChange.Value = "Y";//set to default

                LoadGridData();
                PopulateDropDownsForResp(ddlResponsibility.SelectedValue, ddlMngrResponsibility.SelectedValue);
            }
            catch (ThreadAbortException ex1)
            {
                // do nothing
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in clearing and reloading the data.", ex.Message);
            }


        }

        protected void btnEditFrontSheet_Click(object sender, EventArgs e)
        {
            try
            {
                if (UpdateRecreateStat())
                    actScreen.PopupScreen(hdnGridPnlHeight.Value);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in opening 'Statement Activity List' popup.", ex.Message);
            }

        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            Session.Abandon();
        }

        protected void btnYesRecalFrntSht_Click(object sender, EventArgs e)
        {
            RecalculateFrontSheet();
        }

        protected void fuzzySearchOwner_Click(object sender, ImageClickEventArgs e)
        {

            try
            {
                FuzzySearchOwner();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in owner fuzzy search", ex.Message);
            }

        }

        protected void fuzzySearchRoyaltor_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchRoyaltor();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor fuzzy search", ex.Message);
            }

        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "Royaltor")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtRoyaltor.Text = string.Empty;
                        return;
                    }

                    txtRoyaltor.Text = lbFuzzySearch.SelectedValue.ToString();
                    txtOwnSearch.Text = string.Empty;
                }
                else if (hdnFuzzySearchField.Value == "Owner")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtOwnSearch.Text = string.Empty;
                        return;
                    }

                    txtOwnSearch.Text = lbFuzzySearch.SelectedValue.ToString();
                    txtRoyaltor.Text = string.Empty;
                }

                //WUIN-736
                //trigger search
                btnGo_Click(new object(), new EventArgs());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fuzzy search selection", ex.Message);
            }
        }

        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "Royaltor")
                {
                    txtRoyaltor.Text = string.Empty;
                }
                else if (hdnFuzzySearchField.Value == "Owner")
                {
                    txtOwnSearch.Text = string.Empty;
                }

                mpeFuzzySearch.Hide();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing complete search list pop up", ex.Message);
            }
        }

        protected void btnSaveComment_Click(object sender, EventArgs e)
        {
            try
            {
                Button btnSaveDelete = (Button)sender;
                string saveOrDelete = string.Empty;

                if (btnSaveDelete.Text == "Save")
                {
                    saveOrDelete = "S";
                }
                else if (btnSaveDelete.Text == "Delete")
                {
                    saveOrDelete = "D";
                }

                string comment = string.Empty;

                //get data from rich text control value
                comment = txtHidCommentData.Value.ToString();

                string owner = string.Empty;
                string royaltor = string.Empty;
                if (txtOwnSearch.Text != string.Empty)
                {
                    try
                    {
                        owner = txtOwnSearch.Text.Substring(0, txtOwnSearch.Text.IndexOf("-") - 1);
                    }
                    catch (Exception ex)
                    {
                        msgView.SetMessage("Please select the owner from search list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                }

                if (txtRoyaltor.Text != string.Empty)
                {
                    try
                    {
                        royaltor = txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1);
                    }
                    catch (Exception ex)
                    {
                        msgView.SetMessage("Please select the royaltor from search list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                }

                workFlowBL = new WorkFlowBL();
                //JIRA-1048 Changes to handle single quote --Start
                DataSet activitySearchData = workFlowBL.SaveComment(hdnCommentRoyaltorId.Value, hdnCommentStmtPeriodId.Value, comment, saveOrDelete, ddlCompany.SelectedValue, ddlReportingSch.SelectedValue.ToString(),
                                                                    owner, royaltor, ddlResponsibility.SelectedValue.ToString(), ddlMngrResponsibility.SelectedValue.ToString(), ddlPriority.SelectedValue.ToString(), txtProducer.Text.Replace("'", "").Trim(),
                                                                    ddlStatus.SelectedValue.ToString(), txtEarnings.Text.Replace("'", "").Trim(), ddlEarningsCompare.SelectedValue, txtClosingBalance.Text.Replace("'", "").Trim(), ddlClosingBalCompare.SelectedValue,
                                                                    Convert.ToString(Session["UserCode"]), out errorId);
                //JIRA-1048 Changes to handle single quote -- End
                workFlowBL = null;

                //WUIN-1057
                ClearRerunRecalBulkControls();

                mpeCommentPopup.Show();
                pnlCommentPopup.Style.Add("height", (Convert.ToDouble(hdnGridPnlHeight.Value)).ToString());
                iFrameComment.Style.Add("height", (Convert.ToDouble(hdnGridPnlHeight.Value) * 0.5).ToString());

                rptPager.Visible = false;

                Session["UpdatedOwnerCode"] = hdnCommentOwnerCode.Value; ;
                hdnUpdatedStmtPeriod.Value = hdnCommentStmtPeriodId.Value;

                if (activitySearchData.Tables.Count != 0 && errorId != 2)
                {
                    Session["dtWorkflowData"] = activitySearchData.Tables[0];
                    if (activitySearchData.Tables[0].Rows.Count == 0)
                    {
                        gvRoyActivity.DataSource = activitySearchData.Tables[0];
                        gvRoyActivity.EmptyDataText = "No data found for the selected filter criteria";
                        gvRoyActivity.DataBind();
                    }
                    else
                    {
                        if (activitySearchData.Tables[0].Rows.Count > (hdnGridPageSize.Value == "" ? gridDefaultPageSize : Convert.ToInt32(hdnGridPageSize.Value)))
                        {
                            SetPageStartEndRowNum(activitySearchData.Tables[0].Rows.Count);
                            PopulateGridPage(hdnPageIndex.Value == "" ? 1 : Convert.ToInt32(hdnPageIndex.Value));
                        }
                        else
                        {
                            gvRoyActivity.DataSource = activitySearchData.Tables[0];
                            gvRoyActivity.DataBind();
                        }
                    }

                }
                else if (activitySearchData.Tables.Count == 0 && errorId != 2)
                {
                    dtEmpty = new DataTable();
                    gvRoyActivity.DataSource = dtEmpty;
                    gvRoyActivity.EmptyDataText = "No data found for the selected filter criteria";
                    gvRoyActivity.DataBind();

                }
                else
                {
                    ExceptionHandler("Error in saving comment", string.Empty);
                }


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving comment", ex.Message);
            }
        }

        protected void btnCommentUploadFileHidden_Click(object sender, EventArgs e)
        {
            try
            {
                string uploadFolderPath = @AppDomain.CurrentDomain.BaseDirectory + @"\WorkflowCommentAttachments\" + Session["WARSAffiliate"].ToString() + @"\";

                if (uploadCommentAttachment.HasFile)
                {
                    string fileExtension = Path.GetExtension(uploadCommentAttachment.PostedFile.FileName);
                    string fileName = hdnCommentRoyaltorId.Value + "_" + hdnCommentStmtPeriodId.Value + "_" + Path.GetFileNameWithoutExtension(uploadCommentAttachment.FileName);

                    //Save File to the Directory (Folder).
                    uploadCommentAttachment.SaveAs(uploadFolderPath + fileName + fileExtension);
                }


                mpeCommentPopup.Show();
                pnlCommentPopup.Style.Add("height", (Convert.ToDouble(hdnGridPnlHeight.Value)).ToString());
                iFrameComment.Style.Add("height", (Convert.ToDouble(hdnGridPnlHeight.Value) * 0.5).ToString());
                PopulateCommentAttachmentFiles();
                hdnCommentUpload.Value = "N";

                RefreshGridData();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in uploading comment attachment", ex.Message);
            }
        }

        protected void btnCommentDownloadFile_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = hdnCommentRoyaltorId.Value + "_" + hdnCommentStmtPeriodId.Value + "_" + hdnCommentSelectedFileName.Value;
                string filePath = @AppDomain.CurrentDomain.BaseDirectory + @"\WorkflowCommentAttachments\" + Session["WARSAffiliate"].ToString() + @"\" + fileName;

                FileInfo file = new FileInfo(filePath);
                if (file.Exists)
                {
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + hdnCommentSelectedFileName.Value);
                    Response.AddHeader("Content-Length", file.Length.ToString());
                    Response.ContentType = "text/plain";
                    Response.Flush();
                    Response.TransmitFile(file.FullName);
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in downloading comment attachment", ex.Message);
            }
        }

        protected void btnYesCommentDeleteFile_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = hdnCommentRoyaltorId.Value + "_" + hdnCommentStmtPeriodId.Value + "_" + hdnCommentSelectedFileName.Value;
                string filePath = @AppDomain.CurrentDomain.BaseDirectory + @"\WorkflowCommentAttachments\" + Session["WARSAffiliate"].ToString() + @"\" + fileName;

                FileInfo file = new FileInfo(filePath);
                if (file.Exists)
                {
                    file.Delete();
                }

                mpeCommentPopup.Show();
                pnlCommentPopup.Style.Add("height", (Convert.ToDouble(hdnGridPnlHeight.Value)).ToString());
                iFrameComment.Style.Add("height", (Convert.ToDouble(hdnGridPnlHeight.Value) * 0.5).ToString());
                PopulateCommentAttachmentFiles();

                RefreshGridData();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in deleting comment attachment", ex.Message);
            }

        }

        protected void btnNoStatusBulkUpdateConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                rblistStatusHeader.ClearSelection();
                mpeStatusBulkUpdateConfirm.Hide();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing status bulk update confirm popup", ex.Message);
            }

        }

        /// <summary>
        /// WUIN-1057 - changes
        /// implemented to close the explicit message pop up on enter key for validation message on search
        /// </summary>        
        protected void btnCloseValSearchMsgPopup_Click(object sender, EventArgs e)
        {
            txtValSearchMsgPopup.Visible = false;
            mpeValSearchMsgPopup.Hide();
        }

        #endregion PAGE EVENTS

        #region GRIDVIEW EVENTS

        protected void gvRoyActivity_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                RadioButtonList rblistStatus;
                CheckBox cbReCreateStats;
                CheckBox cbRecalFrntSht;
                Label lblEarnings;
                Label lblClosingBal;
                ImageButton imgBtnCommentWithLine;
                ImageButton imgBtnCommentWithOutLine;

                string ownerCode;
                string ownerName;
                string royaltorID;
                string stmtPeriodId;
                string status;
                string reCreateStats;
                string dtlFileFlag;
                string updatedOwnerCode;
                string stmtHeld;
                string hdnRerunStmt;
                string hdnRecalStmt;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    rblistStatus = (RadioButtonList)e.Row.FindControl("rblistStatus");
                    cbReCreateStats = (CheckBox)e.Row.FindControl("cbReCreateStats");
                    cbRecalFrntSht = (CheckBox)e.Row.FindControl("cbRecalFrntSht");
                    lblEarnings = (Label)e.Row.FindControl("lblEarnings");
                    lblClosingBal = (Label)e.Row.FindControl("lblClosingBal");
                    imgBtnCommentWithLine = (ImageButton)e.Row.FindControl("imgBtnCommentWithLine");
                    imgBtnCommentWithOutLine = (ImageButton)e.Row.FindControl("imgBtnCommentWithOutLine");
                    ownerCode = (e.Row.FindControl("lblOwner") as Label).Text;
                    ownerName = (e.Row.FindControl("lblOwnerName") as Label).Text;
                    royaltorID = (e.Row.FindControl("lblRoyaltor") as Label).Text;
                    stmtPeriodId = (e.Row.FindControl("lblStmtPeriodId") as Label).Text;
                    status = (e.Row.FindControl("lblStatus") as Label).Text;
                    reCreateStats = (e.Row.FindControl("lblReCreateStats") as Label).Text;
                    dtlFileFlag = (e.Row.FindControl("lblDtlFileFlag") as Label).Text;
                    updatedOwnerCode = Convert.ToString(Session["UpdatedOwnerCode"]);
                    stmtHeld = (e.Row.FindControl("hdnStmtHeld") as HiddenField).Value;
                    hdnRerunStmt = (e.Row.FindControl("hdnRerunStmt") as HiddenField).Value;
                    hdnRecalStmt = (e.Row.FindControl("hdnRecalStmt") as HiddenField).Value;

                    //hide text of radiobutton list items
                    rblistStatus.Items[0].Text = "";
                    rblistStatus.Items[1].Text = "";
                    rblistStatus.Items[2].Text = "";
                    rblistStatus.Items[3].Text = "";
                    rblistStatus.Items[4].Text = "";

                    #region hide Team/Manager sign off status
                    //WUIN-920 change
                    //Team sign off and Manager sign off status will be available upon the Registry setting
                    if (hdnTeamSignOffVisible.Value == "N")
                    {
                        rblistStatus.Items.RemoveAt(1);

                        if (hdnMngrSignOffVisible.Value == "N")
                        {
                            rblistStatus.Items.RemoveAt(1);
                            rblistStatus.RepeatColumns = 3;
                        }
                        else
                        {
                            rblistStatus.RepeatColumns = 4;
                        }

                    }

                    if (hdnTeamSignOffVisible.Value != "N" && hdnMngrSignOffVisible.Value == "N")
                    {
                        rblistStatus.Items.RemoveAt(2);
                        rblistStatus.RepeatColumns = 4;
                    }
                    #endregion hide Team/Manager sign off status

                    #region populate status and recreate controls
                    //Populate Status radiobuttonlist                    
                    if (status != string.Empty)
                    {
                        //WOS-209 - display status selected when Archive requested i.e when status_code = 3 and flag = A
                        if (status == "3" && (reCreateStats == "A" || reCreateStats == "B"))//Added flag as part of WUIN-438
                        {
                            if (rblistStatus.Items.FindByValue("4") != null)
                            {
                                rblistStatus.Items.FindByValue("4").Selected = true;
                            }
                        }
                        else
                        {
                            if (rblistStatus.Items.FindByValue(status) != null)
                            {
                                rblistStatus.Items.FindByValue(status).Selected = true;
                            }
                        }
                        //End - WOS-209
                    }

                    //Populate Recreate statement checkbox                    
                    if (reCreateStats == "Y")
                        cbReCreateStats.Checked = true;
                    else
                        cbReCreateStats.Checked = false;
                    #endregion populate status and recreate controls

                    if (ownerCode != "" && ownerName != "")//owner level row
                    {
                        //WUIN-605 - comments are displayed only for royaltor level rows
                        imgBtnCommentWithLine.Visible = false;
                        imgBtnCommentWithOutLine.Visible = false;

                        //WOS-254 - When a specific Royaltor is selected on the Statement Workflow filter, display without the Owner line                          
                        if (txtRoyaltor.Text != string.Empty)
                        {
                            e.Row.Visible = false;
                        }
                        else
                        {
                            //WOS-254 - When a specific Owner is selected on the Statement Workflow filter, display the royaltors by default (Owner expanded view)
                            //if ((ownerCode == updatedOwnerCode) && (stmtPeriodId == hdnUpdatedStmtPeriod.Value))
                            if (((ownerCode == updatedOwnerCode) && (stmtPeriodId == hdnUpdatedStmtPeriod.Value)) || txtOwnSearch.Text != string.Empty)
                                HideUnHideToggleButtons(e.Row.Cells[0], false, true);//hide expand button
                            else
                                HideUnHideToggleButtons(e.Row.Cells[0], true, false);//hide collapse button

                            e.Row.Cells[1].Font.Bold = true;
                            e.Row.Cells[2].Font.Bold = true;
                            e.Row.Cells[3].Font.Bold = true;

                            #region to handle owner level row based on child row state
                            //hold current owner row index
                            rowIndexOfOwnerRow = e.Row.RowIndex.ToString();

                            //when all royaltors under a owner group are of same status/recreate statement/recalculate summary checked, reflect the same on owner line.
                            //When the royaltors are of different status/ recreate statement/recalculate summary checked, leave the owner level line as is
                            curRoyUnderOwnStatus = string.Empty;
                            prevRoyUnderOwnStatus = string.Empty;
                            curRoyUnderOwnFlag2 = string.Empty;
                            prevRoyUnderOwnFlag2 = string.Empty;
                            ownRowStatusSet = string.Empty;
                            curRoyUnderOwnFlag = string.Empty;
                            prevRoyUnderOwnFlag = string.Empty;
                            curRoyUnderOwnFlag3 = string.Empty;
                            prevRoyUnderOwnFlag3 = string.Empty;
                            ownRowFlagSet = string.Empty;
                            ownRowRecalFlagSet = string.Empty;
                            #endregion to handle owner level row based on child row state
                        }
                    }
                    //WOS-254 - When a specific Royaltor is selected on the Statement Workflow filter, display without the Owner line  
                    //else if (ownerCode != "" && ownerCode != "N/A" && ownerName == "")//royaltor level row under a owner
                    else if (ownerCode != "" && ownerCode != "N/A" && ownerName == "" && txtRoyaltor.Text == string.Empty)//royaltor level row under a owner
                    {
                        //keep the owner group rows visible when an update is made    
                        //WOS-254 - When a specific Owner is selected on the Statement Workflow filter, display the royaltors by default (Owner expanded view)                        
                        if (((ownerCode == updatedOwnerCode) && (stmtPeriodId == hdnUpdatedStmtPeriod.Value)) || txtOwnSearch.Text != string.Empty)
                            e.Row.Visible = true;
                        else
                            e.Row.Visible = false;

                        #region to handle owner level row based on child row state
                        GridViewRow row = gvRoyActivity.Rows[Convert.ToInt32(rowIndexOfOwnerRow)];
                        RadioButtonList rblistStatusOwn = (RadioButtonList)row.FindControl("rblistStatus");//owner row status button
                        CheckBox cbReCreateStatsOwn = (CheckBox)row.FindControl("cbReCreateStats");//owner row checkbox
                        CheckBox cbRecalFrntShtOwn = (CheckBox)row.FindControl("cbRecalFrntSht");//owner row checkbox                        
                        HiddenField hdnOwnerLevelStatusOwn = (HiddenField)row.FindControl("hdnOwnerLevelStatus");//WUIN-290 change: to populate previous value on popup cancel
                        HiddenField hdnOwnerLevelReCreateStatus = (HiddenField)row.FindControl("hdnOwnerLevelReCreateStatus");//WUIN-290 change: used in final sign off warning

                        //when all royaltors under a owner group are of same status, reflect the same on owner line.
                        //When the royaltors are of different status, leave the owner level line as is
                        if (ownRowStatusSet != "false")
                        {
                            prevRoyUnderOwnStatus = curRoyUnderOwnStatus;
                            curRoyUnderOwnStatus = status;
                            prevRoyUnderOwnFlag2 = curRoyUnderOwnFlag2;
                            curRoyUnderOwnFlag2 = reCreateStats;
                            if (((prevRoyUnderOwnStatus == string.Empty || curRoyUnderOwnStatus == prevRoyUnderOwnStatus) &&
                                (prevRoyUnderOwnFlag2 == string.Empty || curRoyUnderOwnFlag2 == prevRoyUnderOwnFlag2)) ||

                                ((prevRoyUnderOwnStatus == "3" && (prevRoyUnderOwnFlag2 == "A" || prevRoyUnderOwnFlag2 == "B")) &&
                                 (curRoyUnderOwnStatus == "3" && (curRoyUnderOwnFlag2 == "A" || curRoyUnderOwnFlag2 == "B"))) ||

                                ((prevRoyUnderOwnStatus == "3" && (prevRoyUnderOwnFlag2 == "A" || prevRoyUnderOwnFlag2 == "B")) && (curRoyUnderOwnStatus == "4")) ||
                                ((curRoyUnderOwnStatus == "3" && (curRoyUnderOwnFlag2 == "A" || curRoyUnderOwnFlag2 == "B")) && (prevRoyUnderOwnStatus == "4"))
                                )
                            {
                                //WOS-209 changes                                                               
                                if ((reCreateStats == "A" || reCreateStats == "B") && status == "3")//WUIN-438 Added flag as part of this JIRA
                                {
                                    if (rblistStatusOwn.Items.FindByValue("4") != null)
                                    {
                                        rblistStatusOwn.Items.FindByValue("4").Selected = true;
                                    }

                                    hdnOwnerLevelReCreateStatus.Value = reCreateStats;
                                }
                                else
                                {
                                    if (rblistStatusOwn.Items.FindByValue(status) != null)
                                    {
                                        rblistStatusOwn.Items.FindByValue(status).Selected = true;
                                    }
                                }
                                //End-WOS-209 changes
                            }
                            else
                            {
                                rblistStatusOwn.ClearSelection();
                                hdnOwnerLevelReCreateStatus.Value = string.Empty;//This needs to be cleared when owner level status is not selected
                                ownRowStatusSet = "false";
                            }
                        }

                        //WUIN-290 change : Holding previous status value. used in warning popup cancel
                        hdnOwnerLevelStatusOwn.Value = rblistStatusOwn.SelectedValue.ToString();

                        //when all royaltors under a owner group are of same recreate statement checked, reflect the same on owner line.
                        //When the royaltors are of different recreate statement checked, leave the owner level line as is
                        if (ownRowFlagSet != "false")
                        {
                            prevRoyUnderOwnFlag = curRoyUnderOwnFlag;
                            //WUIN-1057 changes - set the value based on grid level option selection
                            curRoyUnderOwnFlag = (hdnRerunStmt == "" ? reCreateStats : hdnRerunStmt);
                            if ((prevRoyUnderOwnFlag == string.Empty && curRoyUnderOwnFlag == "Y") || curRoyUnderOwnFlag == prevRoyUnderOwnFlag)
                            {
                                cbReCreateStatsOwn.Checked = true;
                            }
                            else
                            {
                                cbReCreateStatsOwn.Checked = false;
                                ownRowFlagSet = "false";
                            }
                        }

                        //WUIN-920 - Owner level ‘Recreate stmt’ should be disabled if any of the royaltor level statements are disabled 
                        //WUIN-1057 - on bulk option check - as only enabled child rows are checked, un-check owner level checkbox when
                        //            it is disabled
                        if (!(status == "1" && reCreateStats == "N" && dtlFileFlag == "N"))
                        {
                            cbReCreateStatsOwn.Enabled = false;
                            cbReCreateStatsOwn.Checked = false;
                        }
                                                
                        //Owner level ‘Recalculate Front Sheet’ should be disabled if any of the child level is disabled
                        if (reCreateStats != "N" || dtlFileFlag != "N" || status != "1" || stmtHeld == "Y")
                        {
                            cbRecalFrntShtOwn.Enabled = false;
                        }

                        //WUIN-1057
                        //when all statements under a owner group are of same recalculate summary checked, reflect the same on owner line.
                        //When the royaltors are of different recalculate summary checked, leave the owner level line as is
                        if (ownRowRecalFlagSet != "false")
                        {
                            prevRoyUnderOwnFlag3 = curRoyUnderOwnFlag3;
                            //WUIN-1057 changes - set the value based on grid level option selection
                            curRoyUnderOwnFlag3 = hdnRecalStmt;
                            if ((prevRoyUnderOwnFlag3 == string.Empty && curRoyUnderOwnFlag3 == "Y") ||
                                ((curRoyUnderOwnFlag3 == "Y") && (curRoyUnderOwnFlag3 == prevRoyUnderOwnFlag3)))
                            {
                                cbRecalFrntShtOwn.Checked = true;
                            }
                            else
                            {
                                cbRecalFrntShtOwn.Checked = false;
                                ownRowRecalFlagSet = "false";
                            }
                        }
                        


                        #endregion to handle owner level row based on child row state

                    }


                    //for a royaltor level row : either it be for a royaltor under owner group or an individual royaltor without owner
                    if ((ownerCode != "" && ownerName == ""))
                    {
                        //WOS-254 - When a specific Royaltor is selected on the Statement Workflow filter, display without the Owner line  
                        if (txtRoyaltor.Text != string.Empty)
                        {
                            e.Row.BackColor = System.Drawing.Color.White;
                        }

                        //WOS-235 - Add check for DTL_FILE_FLAG where ever royaltor_stmt_flag check is done
                        //Validation: Status button will be disabled for a royaltor if the ROYALTOR_ACTIVITY.royaltor_stmt_flag != 'N' and DTL_FILE_FLAG != 'N'
                        //              and !='A' (for all users)  
                        //WOS-209 - Do not disable Royaltor status details when ROYALTOR_STMT_FLAG has been set to 'A'                          
                        if ((reCreateStats == "N" && dtlFileFlag == "N") || (reCreateStats == "A" || reCreateStats == "B"))//WUIN-438 Added flag B
                        {
                            rblistStatus.Enabled = true;
                        }
                        else
                        {
                            rblistStatus.Enabled = false;
                        }
                        //End - WOS-209 changes


                        //Validation: Recreate Statement Checkbox is enabled for a royaltor only if status is 'under review'  and 
                        //              where the ROYALTY_ACTIVITY.royaltor_stmt_flag = 'N' and and DTL_FILE_FLAG = 'N'. (for all users)
                        if (status == "1" && reCreateStats == "N" && dtlFileFlag == "N")
                        {
                            cbReCreateStats.Enabled = true;
                        }
                        else
                        {
                            cbReCreateStats.Enabled = false;
                        }


                        //WOS-138 - When Royaltor Statement has been set to Archived status - disable edit 
                        //          - do not allow update to status or allow statement recreate or front sheet print
                        if (status == "4" || (status == "3" && reCreateStats == "B"))
                        {
                            rblistStatus.Enabled = false;
                            cbReCreateStats.Enabled = false;
                            //WOS-295 - changing visible to enable 
                            cbRecalFrntSht.Enabled = false;
                        }

                        //WOS-162 - Create Front Sheet option should be disabled when the Royaltor Statement Flag is not set to N
                        //WOS-235 - OR when DTL_FILE_FLAG != N
                        //WOS-295 - Disable the option to Recalculate Front Sheet if status is not under review (STATUS_CODE != 1)
                        //WOS-419 - recalculate front sheet should be available at owner level also
                        //WUIN-920 - recalculate front sheet should be disabled if a statement is held(ROYALTOR_ACTIVITY.STMT_HELD)
                        if (reCreateStats != "N" || dtlFileFlag != "N" || status != "1" || stmtHeld == "Y")
                        {
                            cbRecalFrntSht.Enabled = false;
                        }

                        //WUIN-605 - display comments image 
                        //WUIN-920 - display comments image with lines if there are comments or if has is an attachment and no comments for a statement
                        if (((e.Row.FindControl("hdnComments") as HiddenField).Value != string.Empty) || IsAttachmentExistForStmt(royaltorID, stmtPeriodId))
                        {
                            (e.Row.FindControl("imgBtnCommentWithOutLine") as ImageButton).Visible = false;
                        }
                        else
                        {
                            (e.Row.FindControl("imgBtnCommentWithLine") as ImageButton).Visible = false;
                        }

                    }


                    //User Authorization : Enable only 'Under Review' and 'Team Sign off' for Editor user and enable all for Super user                    
                    if (hdnUserRole.Value.ToLower() == UserRole.EditorUser.ToString().ToLower())
                    {
                        rblistStatus.Items.FindByValue("1").Enabled = true;

                        if (rblistStatus.Items.FindByValue("2") != null)
                        {
                            rblistStatus.Items.FindByValue("2").Enabled = true;
                        }

                        if (rblistStatus.Items.FindByValue("8") != null)
                        {
                            rblistStatus.Items.FindByValue("8").Enabled = false;
                        }

                        if (rblistStatus.Items.FindByValue("3") != null)
                        {
                            rblistStatus.Items.FindByValue("3").Enabled = false;
                        }

                        if (rblistStatus.Items.FindByValue("4") != null)
                        {
                            rblistStatus.Items.FindByValue("4").Enabled = false;
                        }
                    }

                    //JIRA-983 Changes by Ravi on 21/2/2019 -- Start
                    //User Authorization : Enable only 'Under Review' , 'Team Sign off' and 'Manager Sign off' for Supervisor user and enable all for Super user                    
                    if (hdnUserRole.Value.ToLower() == UserRole.Supervisor.ToString().ToLower())
                    {
                        rblistStatus.Items.FindByValue("1").Enabled = true;

                        if (rblistStatus.Items.FindByValue("2") != null)
                        {
                            rblistStatus.Items.FindByValue("2").Enabled = true;
                        }

                        if (rblistStatus.Items.FindByValue("3") != null)
                        {
                            rblistStatus.Items.FindByValue("3").Enabled = false;
                        }

                        if (rblistStatus.Items.FindByValue("4") != null)
                        {
                            rblistStatus.Items.FindByValue("4").Enabled = false;
                        }

                        if (rblistStatus.Items.FindByValue("8") != null)
                        {
                            rblistStatus.Items.FindByValue("8").Enabled = true;
                        }
                    }
                    //JIRA-983 Changes by Ravi on 21/2/2019 -- End

                    //WUIN-876 - If ROYALTOR_ACTIVITY.STMT_HELD = 'Y' do not allow status to be updated except to Archive
                    if (stmtHeld == "Y")
                    {
                        rblistStatus.Items.FindByValue("1").Enabled = false;

                        if (rblistStatus.Items.FindByValue("2") != null)
                        {
                            rblistStatus.Items.FindByValue("2").Enabled = false;
                        }

                        if (rblistStatus.Items.FindByValue("8") != null)
                        {
                            rblistStatus.Items.FindByValue("8").Enabled = false;
                        }

                        if (rblistStatus.Items.FindByValue("3") != null)
                        {
                            rblistStatus.Items.FindByValue("3").Enabled = false;
                        }
                    }

                    // Display 0 as 0.00 in Earnings and Balance
                    if (lblEarnings.Text == "0")
                        lblEarnings.Text = "0.00";

                    if (lblClosingBal.Text == "0")
                        lblClosingBal.Text = "0.00";

                    //WUIN-1057 - populated from bulk option check
                    if (cbReCreateStats.Enabled && hdnRerunStmt == "Y")
                    {
                        cbReCreateStats.Checked = true;
                    }

                    if (cbRecalFrntSht.Enabled && hdnRecalStmt == "Y")
                    {
                        cbRecalFrntSht.Checked = true;
                    }
                    

                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding grid data.", ex.Message);
            }

        }

        protected void gvRoyActivity_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                string ownerCode;
                string ownerName;
                string stmtPeriodId;
                string responsibility;
                int currRowIndex = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvRoyActivity.Rows[currRowIndex];
                string hdrOwnerCode = (row.FindControl("lblOwner") as Label).Text;
                string hdrOwnerName = (row.FindControl("lblOwnerName") as Label).Text;
                string hdrStmtPeriodId = (row.FindControl("lblStmtPeriodId") as Label).Text;
                string hdrresponsibility = (row.FindControl("lblResponsibility") as Label).Text;

                try
                {
                    if (e.CommandName == "Expand")
                    {
                        for (int i = currRowIndex + 1; i < gvRoyActivity.Rows.Count; i++)
                        {
                            ownerCode = (gvRoyActivity.Rows[i].FindControl("lblOwner") as Label).Text;
                            ownerName = (gvRoyActivity.Rows[i].FindControl("lblOwnerName") as Label).Text;
                            stmtPeriodId = (gvRoyActivity.Rows[i].FindControl("lblStmtPeriodId") as Label).Text;
                            responsibility = (gvRoyActivity.Rows[i].FindControl("lblResponsibility") as Label).Text;

                            if ((hdrOwnerCode == ownerCode) && (hdrStmtPeriodId == stmtPeriodId) && (hdrresponsibility == responsibility))
                            {
                                gvRoyActivity.Rows[i].Visible = true;
                            }
                            else
                            {
                                //we have reached the end of the current group
                                //make expand image invisible and collapse image visible
                                HideUnHideToggleButtons(row.Cells[0], false, true);
                                break;
                            }

                            //if we are dealing with the last row,
                            //hide/unhide collapse/expand logic needs to be
                            //handled here
                            if (i + 1 == gvRoyActivity.Rows.Count)
                                HideUnHideToggleButtons(row.Cells[0], false, true);
                        }

                    }

                    if (e.CommandName == "Collapse")
                    {
                        for (int i = currRowIndex + 1; i < gvRoyActivity.Rows.Count; i++)
                        {
                            ownerCode = (gvRoyActivity.Rows[i].FindControl("lblOwner") as Label).Text;
                            ownerName = (gvRoyActivity.Rows[i].FindControl("lblOwnerName") as Label).Text;
                            stmtPeriodId = (gvRoyActivity.Rows[i].FindControl("lblStmtPeriodId") as Label).Text;
                            responsibility = (gvRoyActivity.Rows[i].FindControl("lblResponsibility") as Label).Text;

                            if ((hdrOwnerCode == ownerCode) && (hdrStmtPeriodId == stmtPeriodId) && (hdrresponsibility == responsibility))
                            {
                                gvRoyActivity.Rows[i].Visible = false;
                            }
                            else
                            {
                                //we have reached the end of the current group
                                //make expand image invisible and collapse image visible
                                HideUnHideToggleButtons(row.Cells[0], true, false);
                                break;
                            }

                            //if we are dealing with the last row,
                            //hide/unhide collapse/expand logic needs to be
                            //handled here
                            if (i + 1 == gvRoyActivity.Rows.Count)
                                HideUnHideToggleButtons(row.Cells[0], true, false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler("Error in expanding/collapsing owner group in grid.", ex.Message);
                }

                try
                {
                    if (e.CommandName == "Comment")
                    {
                        Session["WorkflowCommentRoyId"] = (row.FindControl("lblRoyaltor") as Label).Text;
                        Session["WorkflowCommentStmtPeriodId"] = (row.FindControl("hdnStmtPeriodId") as HiddenField).Value;
                        hdnCommentRoyaltorId.Value = (row.FindControl("lblRoyaltor") as Label).Text;
                        hdnCommentStmtPeriodId.Value = (row.FindControl("hdnStmtPeriodId") as HiddenField).Value;
                        hdnCommentOwnerCode.Value = hdrOwnerCode;

                        mpeCommentPopup.Show();
                        pnlCommentPopup.Style.Add("height", (Convert.ToDouble(hdnGridPnlHeight.Value)).ToString());
                        iFrameComment.Style.Add("height", (Convert.ToDouble(hdnGridPnlHeight.Value) * 0.5).ToString());

                        //Load attachments
                        PopulateCommentAttachmentFiles();

                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler("Error in loading comment", ex.Message);
                }


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in workflow screen", ex.Message);
            }


        }

        /* // WOS-253 - no need to popup statement PDF 
        protected void btnCreateFrontSheet_Click(object sender, ImageClickEventArgs e)
        {
            try
            {                
                ImageButton btnCreateFrontSheet = (ImageButton)sender;
                GridViewRow gvr = ((ImageButton)sender).NamingContainer as GridViewRow;
                string royaltorID = (gvr.FindControl("lblRoyaltor") as Label).Text;
                string stmtPeriodId = (gvr.FindControl("lblStmtPeriodId") as Label).Text;
                string createFrontSheet;
                string masterGroupedRoyaltor;
                string stmtPeriodSortCode;
                
                //WOS-98 - Front sheet PDF to be generated only if costs are refreshed 
                //WOS-182 - summary stmt to be generated only if selected royaltor_id = ROYALTOR_GROUPING.SUMMARY_MASTER_ROYALTOR
                if (UpdateSummaryStmtDetails(royaltorID, stmtPeriodId, out createFrontSheet, out masterGroupedRoyaltor, out stmtPeriodSortCode))
                {
                    if (createFrontSheet == "Y")
                    {
                        if (masterGroupedRoyaltor == "Y")
                        {
                            Session["WorkflowSummReport"] = royaltorID + "+" + stmtPeriodId + "+" + stmtPeriodSortCode;
                            mpeSummStmt.Show();
                        }
                        else
                        {
                            msgView.SetMessage("Details have been updated but Summary is grouped with " + masterGroupedRoyaltor,
                                    MessageType.Warning, PositionType.Auto);
                        }
                    }
                    else
                    {
                        msgView.SetMessage("More than one statement for Royaltor, cannot recalculate the Front Sheet.",
                                MessageType.Warning, PositionType.Auto);
                    }

                }
                else
                {
                    ExceptionHandler("Error in updating summary statement details.", string.Empty);
                }
                 
            }
            catch (ThreadAbortException ex)
            {

            }
            catch (Exception ex)
            {                
                ExceptionHandler("Error in opening summary statement report.", ex.Message);
            }
            
        }
        */

        protected void rblistStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnIgnoreStatusClick.Value == "Y")
                {
                    return;
                }

                RadioButtonList rbList = (RadioButtonList)sender;
                GridViewRow gvr = ((RadioButtonList)sender).NamingContainer as GridViewRow;
                string ownerCode = (gvr.FindControl("lblOwner") as Label).Text;
                string ownerName = (gvr.FindControl("lblOwnerName") as Label).Text;
                string royaltorID = (gvr.FindControl("lblRoyaltor") as Label).Text;
                string stmtPeriodId = (gvr.FindControl("lblStmtPeriodId") as Label).Text;
                string previousStatus = (gvr.FindControl("lblStatus") as Label).Text;
                string changedStatus = rbList.SelectedValue.ToString();
                string reCreateStats = (gvr.FindControl("lblReCreateStats") as Label).Text;

                //Validations: 
                //1. WUIN-920 - status can be moved to any status and not just to next status.                
                //              All validations should be handled accordingly on the availability of the status hidden                
                //2. Editor User can't change the status of a statement from Manager Sign off/Final Sign off/Archive back to Team Sign off/Under Review
                //          royaltor level - check for the selected statement
                //          owner level - check for all statements under the owner and if any stmt is in Manager Sign off/Final Sign off/Archive,
                //                        Confirmation message and if selected to proceed, update all other than Manager Sign off/Final Sign off/Archive to the selected status
                //3. WOS-209 - Status can be changed from Archive requested to final sign off
                //             - when archive requested, set status_code = 3 and flag = A                
                //4. Status cannot be changed from Archive/Final Sign Off to Team Sign Off/Manager sign off/Under Review
                //          royaltor level - check for the selected statement
                //          owner level - check for all statements under the owner and if any stmt is in Archive/Final sign off,
                //                        Confirmation message and if selected to proceed, update all other than archive/final sign off to the selected status
                //5. Owner level status update - check for all statements under the owner if any stmt is disabled to be updated to the selected status then
                //                               Confirmation message and if selected to proceed, update all other than disabled to selected status
                //WUIN-438 Added flag B
                //WUIN-741 - Added Manager Sign off status. As Status code of Manager sign off is 8, check on changedstatus > previous status fails. 
                //           considering changed status as 3

                hdnStatusUpdateConfirmOwnerCode.Value = ownerCode;
                hdnStatusUpdateConfirmStmtPrdId.Value = stmtPeriodId;
                hdnStatusUpdateConfirmStatusCode.Value = changedStatus;
                string validationMsg = string.Empty;
                bool validationPass = true;

                if (ownerName == string.Empty)//Royaltor level
                {
                    //JIRA-983 Changes by Ravi on 20/02/2019 -- Start
                    if (((hdnUserRole.Value == UserRole.EditorUser.ToString())
                        || (hdnUserRole.Value == UserRole.Supervisor.ToString())) &&
                         ((hdnMngrSignOffVisible.Value == "Y" && previousStatus == "8") || previousStatus == "3" || previousStatus == "4")
                       )
                    //JIRA-983 Changes by Ravi on 20/02/2019 -- End
                    {
                        validationPass = false;

                        if (hdnTeamSignOffVisible.Value == "Y" && hdnMngrSignOffVisible.Value == "Y")
                        {
                            if (hdnUserRole.Value == UserRole.Supervisor.ToString())
                            {
                                validationMsg = "Supervisor can't change the status of a statement from Final Sign off/Archive back to Manager Sign off/Team Sign off/Under Review";
                            }
                            else
                            {
                                validationMsg = "Editor User can't change the status of a statement from Manager Sign off/Final Sign off/Archive back to Team Sign off/Under Review";
                            }
                        }
                        else if (hdnTeamSignOffVisible.Value == "N" && hdnMngrSignOffVisible.Value == "N")
                        {
                            if (hdnUserRole.Value == UserRole.Supervisor.ToString())
                            {
                                validationMsg = "Supervisor can't change the status of a statement from Final Sign off/Archive back to Under Review";
                            }
                            else
                            {
                                validationMsg = "Editor User can't change the status of a statement from Final Sign off/Archive back to Under Review";
                            }
                        }
                        else if (hdnTeamSignOffVisible.Value == "Y" && hdnMngrSignOffVisible.Value == "N")
                        {
                            if (hdnUserRole.Value == UserRole.Supervisor.ToString())
                            {
                                validationMsg = "Supervisor can't change the status of a statement from Final Sign off/Archive back to Manager Sign off/Team Sign off/Under Review";
                            }
                            else
                            {
                                validationMsg = "Editor User can't change the status of a statement from Final Sign off/Archive back to Team Sign off/Under Review";
                            }
                        }
                        else if (hdnTeamSignOffVisible.Value == "N" && hdnMngrSignOffVisible.Value == "Y")
                        {
                            if (hdnUserRole.Value == UserRole.Supervisor.ToString())
                            {
                                validationMsg = "Supervisor can't change the status of a statement from Final Sign off/Archive back to Under Review";
                            }
                            else
                            {
                                validationMsg = "Editor User can't change the status of a statement from Manager Sign off/Final Sign off/Archive back to Under Review";
                            }
                        }

                        msgView.SetMessage(validationMsg, MessageType.Warning, PositionType.Auto);
                        //populate the status button with original value
                        if (reCreateStats == "A" || reCreateStats == "B")
                            rbList.SelectedValue = "4";
                        else
                            rbList.SelectedValue = previousStatus;
                    }
                    //else if (previousStatus == "3" && (changedStatus == "8" || changedStatus == "2" || changedStatus == "1")) /*Modified by Harish 19-10-2018 as this is not working on owner level update*/
                    else if ((previousStatus == "3" || previousStatus == "4") && (changedStatus == "8" || changedStatus == "2" || changedStatus == "1"))
                    {
                        validationPass = false;

                        //if (reCreateStats == "A" || reCreateStats == "B") /*Modified by Harish 19-10-2018 as this is not working on owner level update*/
                        if (reCreateStats == "A" || reCreateStats == "B" || previousStatus == "4")
                        {
                            if (hdnTeamSignOffVisible.Value == "Y" && hdnMngrSignOffVisible.Value == "Y")
                            {
                                validationMsg = "Status can't be changed from Archive to Manager Sign off/Team Sign off/Under Review";
                            }
                            else if (hdnTeamSignOffVisible.Value == "N" && hdnMngrSignOffVisible.Value == "N")
                            {
                                validationMsg = "Status can't be changed from Archive to Under Review";
                            }
                            else if (hdnTeamSignOffVisible.Value == "Y" && hdnMngrSignOffVisible.Value == "N")
                            {
                                validationMsg = "Status can't be changed from Archive to Team Sign off/Under Review";
                            }
                            else if (hdnTeamSignOffVisible.Value == "N" && hdnMngrSignOffVisible.Value == "Y")
                            {
                                validationMsg = "Status can't be changed from Archive to Manager Sign off/Under Review";
                            }

                            msgView.SetMessage(validationMsg, MessageType.Warning, PositionType.Auto);

                            //populate the status button with original value
                            rbList.SelectedValue = "4";
                        }
                        else
                        {
                            if (hdnTeamSignOffVisible.Value == "Y" && hdnMngrSignOffVisible.Value == "Y")
                            {
                                validationMsg = "Status can't be changed from Final Sign Off to Manager Sign off/Team Sign off/Under Review";
                            }
                            else if (hdnTeamSignOffVisible.Value == "N" && hdnMngrSignOffVisible.Value == "N")
                            {
                                validationMsg = "Status can't be changed from Final Sign Off to Under Review";
                            }
                            else if (hdnTeamSignOffVisible.Value == "Y" && hdnMngrSignOffVisible.Value == "N")
                            {
                                validationMsg = "Status can't be changed from Final Sign Off to Team Sign off/Under Review";
                            }
                            else if (hdnTeamSignOffVisible.Value == "N" && hdnMngrSignOffVisible.Value == "Y")
                            {
                                validationMsg = "Status can't be changed from Final Sign Off to Manager Sign off/Under Review";
                            }

                            msgView.SetMessage(validationMsg, MessageType.Warning, PositionType.Auto);

                            rbList.SelectedValue = previousStatus;
                        }
                    }

                }
                else //Owner level
                {
                    bool stmtDisabled = false;
                    bool hasHeldStmt = false;
                    string warningMsg = string.Empty;
                    string ownerCodeRoy;
                    string currentStatusRoy;
                    string stmtPeriodIdRoy;
                    string stmtHeld;

                    //check if any statement is disabled to be updated to the selected status                    
                    for (int i = gvr.RowIndex + 1; i < gvRoyActivity.Rows.Count; i++)
                    {
                        GridViewRow gvRow = gvRoyActivity.Rows[i];
                        RadioButtonList rbListRoy = gvRow.FindControl("rblistStatus") as RadioButtonList;
                        ownerCodeRoy = (gvRow.FindControl("lblOwner") as Label).Text;
                        stmtPeriodIdRoy = (gvRow.FindControl("lblStmtPeriodId") as Label).Text;
                        stmtHeld = (gvRow.FindControl("hdnStmtHeld") as HiddenField).Value;

                        if (ownerCode == ownerCodeRoy && stmtPeriodIdRoy == stmtPeriodId)
                        {
                            if (!rbListRoy.Enabled)
                            {
                                stmtDisabled = true;
                            }

                            if (stmtHeld == "Y")
                            {
                                hasHeldStmt = true;
                            }

                        }

                    }


                    for (int i = gvr.RowIndex + 1; i < gvRoyActivity.Rows.Count; i++)
                    {
                        GridViewRow gvRow = gvRoyActivity.Rows[i];
                        RadioButtonList rbListRoy = gvRow.FindControl("rblistStatus") as RadioButtonList;
                        ownerCodeRoy = (gvRow.FindControl("lblOwner") as Label).Text;
                        currentStatusRoy = rbListRoy.SelectedValue.ToString();
                        stmtPeriodIdRoy = (gvRow.FindControl("lblStmtPeriodId") as Label).Text;
                        stmtHeld = (gvRow.FindControl("hdnStmtHeld") as HiddenField).Value;

                        //WUIN-648 change
                        //bug fix - need to check for the selected reporting schedule(statement period) only.                         
                        if (ownerCode == ownerCodeRoy && stmtPeriodIdRoy == stmtPeriodId)
                        {

                            //JIRA-983 Changes by Ravi on 20/02/2019 -- Start
                            if (((hdnUserRole.Value == UserRole.EditorUser.ToString()) ||
                                (hdnUserRole.Value == UserRole.Supervisor.ToString())) &&
                                 ((hdnMngrSignOffVisible.Value == "Y" && currentStatusRoy == "8") || currentStatusRoy == "3" || currentStatusRoy == "4")
                                    && (changedStatus == "2" || changedStatus == "1"))
                            {
                                //JIRA-983 Changes by Ravi on 20/02/2019 -- End
                                validationPass = false;

                                //When a statement is in manager sign off/final sign off/Archive and tried to change status to under review/team sign off by an Editor user
                                //  AND/OR if any statement is disabled to be updated to the selected status then
                                //  Confirmation message and if selected to proceed, update all other than manager sign off/final sign off/archive and enabled to the selected status
                                if (hdnTeamSignOffVisible.Value == "Y" && hdnMngrSignOffVisible.Value == "Y")
                                {
                                    warningMsg = "There are statements in manager sign off/final sign off/archive";
                                }
                                else if (hdnTeamSignOffVisible.Value == "N" && hdnMngrSignOffVisible.Value == "N")
                                {
                                    warningMsg = "There are statements in final sign off/archive";
                                }
                                else if (hdnTeamSignOffVisible.Value == "Y" && hdnMngrSignOffVisible.Value == "N")
                                {
                                    warningMsg = "There are statements in final sign off/archive";
                                }
                                else if (hdnTeamSignOffVisible.Value == "N" && hdnMngrSignOffVisible.Value == "Y")
                                {
                                    warningMsg = "There are statements in manager sign off/final sign off/archive";
                                }


                                if (stmtDisabled || hasHeldStmt)
                                {
                                    warningMsg = warningMsg + " and/or statements which are held/disabled";
                                }

                                warningMsg = warningMsg + " which can’t be moved to the selected status by an Editor User. Do you want to continue for the rest?";

                                lblMsgStatusUpdateConfirm.Text = warningMsg;
                                mpeStatusUpdateConfirm.Show();
                                rbList.ClearSelection();

                                return;

                            }
                            else if ((currentStatusRoy == "3" || currentStatusRoy == "4") && (changedStatus == "8" || changedStatus == "2" || changedStatus == "1"))
                            {
                                validationPass = false;

                                //When a statement is in final sign off/Archive and tried to change status to under review/team sign off/manager sign off 
                                //  AND/OR if any statement is disabled to be updated to the selected status then
                                //  Confirmation message and if selected to proceed, update all other than final sign off/archive to the selected status
                                //No need to check the status as per the availability of manager sign off/team sign off here. This check will cover these scenarios as well
                                warningMsg = "There are statements in final sign off/archive";

                                if (stmtDisabled || hasHeldStmt)
                                {
                                    warningMsg = warningMsg + " and/or statements which are held/disabled";
                                }

                                warningMsg = warningMsg + " which can’t be moved to the selected status. Do you want to continue for the rest?";

                                lblMsgStatusUpdateConfirm.Text = warningMsg;
                                mpeStatusUpdateConfirm.Show();
                                rbList.ClearSelection();

                                return;

                            }
                            else if (stmtHeld == "Y" && (changedStatus == "2" || changedStatus == "3" || changedStatus == "8"))
                            {
                                //held statements can only be updated to Under review/Archive
                                warningMsg = "There are statements which are held";

                                if (stmtDisabled)
                                {
                                    warningMsg = "There are statements which are held/disabled";
                                }

                                warningMsg = warningMsg + " which can’t be moved to the selected status. Do you want to continue for the rest?";

                                lblMsgStatusUpdateConfirm.Text = warningMsg;
                                mpeStatusUpdateConfirm.Show();
                                rbList.ClearSelection();

                                return;
                            }
                            else if (!rbListRoy.Enabled)
                            {
                                warningMsg = "There are statements which are disabled which can’t be moved to the selected status. Do you want to continue for the rest?";

                                lblMsgStatusUpdateConfirm.Text = warningMsg;
                                mpeStatusUpdateConfirm.Show();
                                rbList.ClearSelection();

                                return;
                            }


                        }
                        else
                        {
                            break;
                        }
                    }

                }

                if (validationPass)
                {
                    Session["UpdatedOwnerCode"] = ownerCode;
                    hdnUpdatedStmtPeriod.Value = stmtPeriodId;
                    UpdateStatus((ownerName == "" ? "" : ownerCode), royaltorID, stmtPeriodId, changedStatus, false);
                }

            }
            catch (ThreadAbortException ex)
            {

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in updating the status.", ex.Message);
            }

        }

        protected void rblistStatusHeader_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //validate and proceed to update if all validations pass 
                //else prompt confirmation message to proceed or not
                if (ValidateGridLevelStatusUpdate())
                {
                    UpdateStatus(string.Empty, string.Empty, string.Empty, rblistStatusHeader.SelectedValue.ToString(), true);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in updating the status.", ex.Message);
            }
        }

        protected void btnYesStatusUpdateConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateStatus(hdnStatusUpdateConfirmOwnerCode.Value, string.Empty, hdnStatusUpdateConfirmStmtPrdId.Value, hdnStatusUpdateConfirmStatusCode.Value, true);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in updating the status.", ex.Message);
            }
        }

        protected void btnYesStatusBulkUpdateConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateStatus(string.Empty, string.Empty, string.Empty, rblistStatusHeader.SelectedValue.ToString(), true);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in updating the status.", ex.Message);
            }
        }

        /// <summary>
        /// WUIN-741 - this event has been added to have progress image on status update
        /// no need to handle anything here because this is fired after rblistStatus_SelectedIndexChanged event
        /// </summary>
        protected void btnRbStatusClick_Click(object sender, EventArgs e)
        {
            //no need to handle anything here
        }

        /// <summary>
        /// WUIN-920 - this event has been added to retain grid scroll position on recalculate front sheet checkbox without autopostback true
        /// no need to handle anything here because this is fired after rblistStatus_SelectedIndexChanged event
        /// </summary>
        protected void btnCbRecalFrntShtClick_Click(object sender, EventArgs e)
        {
            //no need to handle anything here
        }

        protected void btnRecalFrntSht_Click(object sender, EventArgs e)
        {
            try
            {
                hdnReCalFrntShtErrRow.Value = string.Empty;
                bool valRecalFrntSht = false;
                CheckBox cbRecalFrntShtVal;

                //WUIN-1057 changes - when header level recalculate summary option is selected, loop through all pages(datatable) and not grid
                //                    else loop through grid(which loops only the current page)
                if (!cbRecalSummaryHeader.Checked)
                {
                    foreach (GridViewRow row in gvRoyActivity.Rows)
                    {
                        cbRecalFrntShtVal = (CheckBox)row.FindControl("cbRecalFrntSht");
                        if (cbRecalFrntShtVal.Checked == true)
                        {
                            valRecalFrntSht = true;
                            break;
                        }
                    }

                    if (valRecalFrntSht == true)
                    {
                        RecalculateFrontSheet();
                    }
                    else
                    {
                        msgView.SetMessage("Please select a royaltor to recalculate the front sheet.",
                                        MessageType.Warning, PositionType.Auto);
                    }
                }
                else
                {
                    DataTable dtWorkflowData = (DataTable)Session["dtWorkflowData"];
                    List<string> royaltorStmts = new List<string>();
                    string royaltorId;
                    string stmtPeriodId;
                    string recalStmtFlag;

                    foreach (DataRow drStmt in dtWorkflowData.Rows)
                    {
                        royaltorId = Convert.ToString(drStmt["royaltor"]);
                        stmtPeriodId = Convert.ToString(drStmt["stmt_period_id"]);
                        recalStmtFlag = Convert.ToString(drStmt["recalculate_stmt"]);

                        if (recalStmtFlag == "Y")
                        {
                            royaltorStmts.Add(royaltorId + "," + stmtPeriodId);
                        }
                    }

                    if (royaltorStmts.Count > 0)
                    {
                        workFlowBL = new WorkFlowBL();                        
                        workFlowBL.RecalSummaryBulk(royaltorStmts.ToArray(), Convert.ToString(Session["UserCode"]), out errorId);
                        workFlowBL = null;

                        if (errorId == 2)
                        {
                            ExceptionHandler("Error in updating the Recalculate Stmt Summary flag.", string.Empty);                            
                        }
                        else if (errorId == 0)
                        {
                            //reload the grid
                            btnGo_Click(new object(), new EventArgs());

                            msgView.SetMessage("Recalculation of the Stmt Summaries have been requested. They will be generated in the next scheduled run (Every 5 mins).",
                                MessageType.Warning, PositionType.Auto);
                        }
                        
                    }
                                        

                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in recalculating front sheet.", ex.Message);
            }

        }

        //WOS-419
        protected void cbRecalFrntSht_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox cbRecalFrntShtHdr = (CheckBox)sender;
                GridViewRow gvr = ((CheckBox)sender).NamingContainer as GridViewRow;
                int currRowIndex = gvr.RowIndex;
                string hdrOwnerCode = (gvr.FindControl("lblOwner") as Label).Text;
                string hdrOwnerName = (gvr.FindControl("lblOwnerName") as Label).Text;
                string hdrStmtPeriodId = (gvr.FindControl("lblStmtPeriodId") as Label).Text;
                string hdrResponsibility = (gvr.FindControl("lblResponsibility") as Label).Text;
                string ownerCode;
                string ownerName;
                string stmtPeriodId;
                string responsibility;
                CheckBox cbRecalFrntSht;

                //check if it is group header row...
                //if group is expanded then check/uncheck the child rows
                //if the group is collapsed then expand and check/uncheck the child rows
                //WUIN-920 - if royaltor level is disabled, it should not be checked if owner level is checked
                if (hdrOwnerCode != "" && hdrOwnerName != "")//group header row
                {
                    //string collapseExpandId = (gvr.Cells[0].Controls[1] as ImageButton).ID;
                    ImageButton imgExpand = (gvr.Cells[0].Controls[1] as ImageButton);
                    ImageButton imgCollapse = (gvr.Cells[0].Controls[3] as ImageButton);
                    if (imgExpand != null && imgExpand.Visible)
                    {
                        for (int i = currRowIndex + 1; i < gvRoyActivity.Rows.Count; i++)
                        {
                            ownerCode = (gvRoyActivity.Rows[i].FindControl("lblOwner") as Label).Text;
                            ownerName = (gvRoyActivity.Rows[i].FindControl("lblOwnerName") as Label).Text;
                            stmtPeriodId = (gvRoyActivity.Rows[i].FindControl("lblStmtPeriodId") as Label).Text;
                            responsibility = (gvRoyActivity.Rows[i].FindControl("lblResponsibility") as Label).Text;
                            cbRecalFrntSht = gvRoyActivity.Rows[i].FindControl("cbRecalFrntSht") as CheckBox;

                            if ((hdrOwnerCode == ownerCode) && (hdrStmtPeriodId == stmtPeriodId) && (hdrResponsibility == responsibility))
                            {
                                gvRoyActivity.Rows[i].Visible = true;

                                if (cbRecalFrntShtHdr.Checked && cbRecalFrntSht.Enabled)
                                {
                                    cbRecalFrntSht.Checked = true;
                                }
                                else
                                {
                                    cbRecalFrntSht.Checked = false;
                                }
                            }
                            else
                            {
                                //we have reached the end of the current group
                                //make expand image invisible and collapse image visible
                                HideUnHideToggleButtons(gvr.Cells[0], false, true);
                                break;
                            }

                            //if we are dealing with the last row,
                            //hide/unhide collapse/expand logic needs to be
                            //handled here
                            if (i + 1 == gvRoyActivity.Rows.Count)
                                HideUnHideToggleButtons(gvr.Cells[0], false, true);


                        }
                    }
                    else if (imgCollapse != null && imgCollapse.Visible)
                    {
                        for (int i = currRowIndex + 1; i < gvRoyActivity.Rows.Count; i++)
                        {
                            ownerCode = (gvRoyActivity.Rows[i].FindControl("lblOwner") as Label).Text;
                            ownerName = (gvRoyActivity.Rows[i].FindControl("lblOwnerName") as Label).Text;
                            stmtPeriodId = (gvRoyActivity.Rows[i].FindControl("lblStmtPeriodId") as Label).Text;
                            responsibility = (gvRoyActivity.Rows[i].FindControl("lblResponsibility") as Label).Text;
                            cbRecalFrntSht = gvRoyActivity.Rows[i].FindControl("cbRecalFrntSht") as CheckBox;

                            if ((hdrOwnerCode == ownerCode) && (hdrStmtPeriodId == stmtPeriodId) && (hdrResponsibility == responsibility))
                            {
                                if (cbRecalFrntShtHdr.Checked && cbRecalFrntSht.Enabled)
                                {
                                    cbRecalFrntSht.Checked = true;
                                }
                                else
                                {
                                    cbRecalFrntSht.Checked = false;
                                }
                            }

                        }
                    }
                }
                else if (hdrOwnerCode != "" && hdrOwnerCode != "N/A" && hdrOwnerName == "" && txtRoyaltor.Text == string.Empty)//royaltor level row under a owner
                {
                    for (int i = 0; i < gvRoyActivity.Rows.Count; i++)
                    {
                        ownerCode = (gvRoyActivity.Rows[i].FindControl("lblOwner") as Label).Text;
                        ownerName = (gvRoyActivity.Rows[i].FindControl("lblOwnerName") as Label).Text;
                        stmtPeriodId = (gvRoyActivity.Rows[i].FindControl("lblStmtPeriodId") as Label).Text;
                        responsibility = (gvRoyActivity.Rows[i].FindControl("lblResponsibility") as Label).Text;
                        cbRecalFrntSht = gvRoyActivity.Rows[i].FindControl("cbRecalFrntSht") as CheckBox;

                        if ((hdrOwnerCode == ownerCode) && (hdrStmtPeriodId == stmtPeriodId) && (hdrResponsibility == responsibility))
                        {
                            if (cbRecalFrntShtHdr.Checked == false)
                            {
                                cbRecalFrntSht.Checked = false;
                                break;
                            }

                        }

                    }
                }
                
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting recalculate front sheet rows.", ex.Message);
            }
        }

        protected void cbRerunStmtHeader_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable dtWorkflowData = (DataTable)Session["dtWorkflowData"];
                string status;
                string reCreateStats;
                string dtlFileFlag;

                foreach (DataRow drStmt in dtWorkflowData.Rows)
                {
                    status = Convert.ToString(drStmt["status_code"]);
                    reCreateStats = Convert.ToString(drStmt["royaltor_stmt_flag"]);
                    dtlFileFlag = Convert.ToString(drStmt["dtl_file_flag"]);

                    //when header checkgox is checked:
                    //Recreate Statement Checkbox is enabled for a royaltor only if status is 'under review'  and 
                    //where the ROYALTY_ACTIVITY.royaltor_stmt_flag = 'N' and and DTL_FILE_FLAG = 'N'. (for all users)
                    //set the rerun_stmt flag to Y for the statements where Recreate Statement Checkbox is enabled
                    if (cbRerunStmtHeader.Checked && (status == "1" && reCreateStats == "N" && dtlFileFlag == "N"))
                    {
                        drStmt["rerun_stmt"] = "Y";
                    }
                    else
                    {
                        drStmt["rerun_stmt"] = string.Empty;
                    }
                }

                Session["dtWorkflowData"] = dtWorkflowData;

                //re-bind grid
                if (dtWorkflowData.Rows.Count > (hdnGridPageSize.Value == "" ? gridDefaultPageSize : Convert.ToInt32(hdnGridPageSize.Value)))
                {
                    SetPageStartEndRowNum(dtWorkflowData.Rows.Count);
                    PopulateGridPage(hdnPageIndex.Value == "" ? 1 : Convert.ToInt32(hdnPageIndex.Value));
                }
                else
                {
                    gvRoyActivity.DataSource = dtWorkflowData;
                    gvRoyActivity.DataBind();
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting grid level Re-run Statement", ex.Message);
            }
        }

        protected void cbRecalSummaryHeader_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable dtWorkflowData = (DataTable)Session["dtWorkflowData"];
                string status;
                string reCreateStats;
                string dtlFileFlag;
                string stmtHeld;

                foreach (DataRow drStmt in dtWorkflowData.Rows)
                {
                    status = Convert.ToString(drStmt["status_code"]);
                    reCreateStats = Convert.ToString(drStmt["royaltor_stmt_flag"]);
                    dtlFileFlag = Convert.ToString(drStmt["dtl_file_flag"]);
                    stmtHeld = Convert.ToString(drStmt["statement_held"]);

                    //when header checkgox is checked:                    
                    //from gvRoyActivity_RowDataBound -  Rules when Recalculate summary Checkbox is disabled are:
                    //      if (reCreateStats != "N" || dtlFileFlag != "N" || status != "1" || stmtHeld == "Y")
                    //      if (status == "3" || status == "4")
                    //      if (status == "4" || (status == "3" && reCreateStats == "B"))
                    //Recalculate summary Checkbox is enabled for a royaltor when above rules are toggled.  
                    //set the recalculate_stmt flag to Y for the statements where Recalculate summary Checkbox is enabled                    
                    if (cbRecalSummaryHeader.Checked && (!(reCreateStats != "N" || dtlFileFlag != "N" || status != "1" || stmtHeld == "Y")))
                    {
                        drStmt["recalculate_stmt"] = "Y";
                    }
                    else
                    {
                        drStmt["recalculate_stmt"] = string.Empty;
                    }
                }

                Session["dtWorkflowData"] = dtWorkflowData;

                //re-bind grid
                if (dtWorkflowData.Rows.Count > (hdnGridPageSize.Value == "" ? gridDefaultPageSize : Convert.ToInt32(hdnGridPageSize.Value)))
                {
                    SetPageStartEndRowNum(dtWorkflowData.Rows.Count);
                    PopulateGridPage(hdnPageIndex.Value == "" ? 1 : Convert.ToInt32(hdnPageIndex.Value));
                }
                else
                {
                    gvRoyActivity.DataSource = dtWorkflowData;
                    gvRoyActivity.DataBind();
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting grid level Recalculate Stmt Summary", ex.Message);
            }
        }

        #endregion GRIDVIEW EVENTS

        #region METHODS

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);
        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only Read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnCommentDownloadFile.Enabled = false;
                btnCommentUploadFile.Enabled = false;
                btnEditFrontSheet.Enabled = false;
                btnRecalFrntSht.Enabled = false;
                rblistStatusHeader.Enabled = false;
                btnSaveComment.Enabled = false;
                btnDeleteComment.Enabled = false;
                btnCommentUploadFile.Enabled = false;
                foreach (GridViewRow rows in gvRoyActivity.Rows)
                {
                    (rows.FindControl("rblistStatus") as RadioButtonList).Enabled = false;
                    (rows.FindControl("cbReCreateStats") as CheckBox).Enabled = false;
                    (rows.FindControl("cbRecalFrntSht") as CheckBox).Enabled = false;
                }
                foreach (GridViewRow rows in gvCommentDownloadFile.Rows)
                {
                    (rows.FindControl("gridCommentDownloadFile") as ImageButton).Enabled = false;
                    (rows.FindControl("btnCommentDeleteFile") as ImageButton).Enabled = false;
                }

            }

        }

        private void PopulateDropDowns()
        {
            workFlowBL = new WorkFlowBL();
            DataSet dropdownListData = workFlowBL.GetDropDownData(out errorId);
            workFlowBL = null;

            if (dropdownListData.Tables.Count != 0 && errorId != 2)
            {
                ddlCompany.DataTextField = "com_name";
                ddlCompany.DataValueField = "com_number";
                ddlCompany.DataSource = dropdownListData.Tables[0];
                ddlCompany.DataBind();
                ddlCompany.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-"));

                ddlReportingSch.DataTextField = "stmt_period";
                ddlReportingSch.DataValueField = "statement_period_id";
                ddlReportingSch.DataSource = dropdownListData.Tables[1];
                ddlReportingSch.DataBind();
                ddlReportingSch.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-"));

                //for owner search
                Session["FuzzyWorkflowOwnerList"] = dropdownListData.Tables[2];

                //for royaltor search
                Session["FuzzyWorkflowRoyaltorList"] = dropdownListData.Tables[3];

                ddlStatus.DataTextField = "status";
                ddlStatus.DataValueField = "status_code";
                ddlStatus.DataSource = dropdownListData.Tables[4];
                ddlStatus.DataBind();
                ddlStatus.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-"));

                ddlResponsibility.DataTextField = "responsibility";
                ddlResponsibility.DataValueField = "responsibility_code";
                ddlResponsibility.DataSource = dropdownListData.Tables[5];
                ddlResponsibility.DataBind();
                ddlResponsibility.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-"));                

                ddlPriority.DataTextField = "priority";
                ddlPriority.DataValueField = "priority_code";
                ddlPriority.DataSource = dropdownListData.Tables[6];
                ddlPriority.DataBind();
                ddlPriority.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-"));

                ddlMngrResponsibility.DataTextField = "responsibility";
                ddlMngrResponsibility.DataValueField = "responsibility_code";
                ddlMngrResponsibility.DataSource = dropdownListData.Tables[8];
                ddlMngrResponsibility.DataBind();
                ddlMngrResponsibility.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-"));

                if (dropdownListData.Tables[7].Rows.Count > 0)
                {
                    hdnTeamSignOffVisible.Value = dropdownListData.Tables[7].Rows[0][2].ToString();
                    hdnMngrSignOffVisible.Value = dropdownListData.Tables[7].Rows[1][2].ToString();
                }

            }
            else
            {
                ExceptionHandler("Error in loading the dropdown list values.", string.Empty);
            }

        }

        private void HideTeamOrMngrSignOff()
        {

            //hide text of grid header radiobutton list items
            rblistStatusHeader.Items[0].Text = "";
            rblistStatusHeader.Items[1].Text = "";
            rblistStatusHeader.Items[2].Text = "";
            rblistStatusHeader.Items[3].Text = "";
            rblistStatusHeader.Items[4].Text = "";

            //User Authorization : Enable only 'Under Review' and 'Team Sign off' for Editor user and enable all for Super user   
            if (hdnUserRole.Value.ToLower() == UserRole.EditorUser.ToString().ToLower())
            {
                rblistStatusHeader.Items.FindByValue("8").Enabled = false;
                rblistStatusHeader.Items.FindByValue("3").Enabled = false;
                rblistStatusHeader.Items.FindByValue("4").Enabled = false;
            }

            //JIRA-983 Changes by Ravi on 21/2/2019 -- Start
            //User Authorization : Enable only 'Under Review' ,'Team Sign off' and 'Manager Sign off' for SuperVisor user and enable all for Super user   
            if (hdnUserRole.Value.ToLower() == UserRole.Supervisor.ToString().ToLower())
            {
                rblistStatusHeader.Items.FindByValue("4").Enabled = false;
                rblistStatusHeader.Items.FindByValue("3").Enabled = false;
                rblistStatusHeader.Items.FindByValue("8").Enabled = true;
            }
            //JIRA-983 Changes by Ravi on 21/2/2019 -- End

            //WUIN-920 change
            //Team sign off and Manager sign off status will be available upon the Registry setting
            if (hdnTeamSignOffVisible.Value == "N")
            {
                tdTeamSignOffHdr.Visible = false;
                rblistStatusHeader.Items.RemoveAt(1);

                if (hdnMngrSignOffVisible.Value == "N")
                {
                    tdMngrSignOffHdr.Visible = false;
                    tdStatusHdr.Width = "12%";
                    rblistStatusHeader.Items.RemoveAt(1);
                    rblistStatusHeader.RepeatColumns = 3;
                    gvRoyActivity.Columns[9].ItemStyle.Width = new Unit(12, UnitType.Percentage);
                }
                else
                {
                    tdStatusHdr.Width = "16%";
                    rblistStatusHeader.RepeatColumns = 4;
                    gvRoyActivity.Columns[9].ItemStyle.Width = new Unit(16, UnitType.Percentage);
                }

            }

            if (hdnTeamSignOffVisible.Value != "N" && hdnMngrSignOffVisible.Value == "N")
            {
                tdMngrSignOffHdr.Visible = false;
                tdStatusHdr.Width = "16%";
                rblistStatusHeader.Items.RemoveAt(2);
                rblistStatusHeader.RepeatColumns = 4;
                gvRoyActivity.Columns[9].ItemStyle.Width = new Unit(16, UnitType.Percentage);
            }


        }

        private void PopulateDropDownsForResp(string teamRespCode, string mngrRespCode)
        {
            workFlowBL = new WorkFlowBL();
            DataSet dropdownListData = workFlowBL.GetFiltersForResp(teamRespCode, mngrRespCode, out errorId);
            workFlowBL = null;
            if (dropdownListData.Tables.Count != 0 && errorId != 2)
            {
                //for owner search
                Session["FuzzyWorkflowOwnerList"] = dropdownListData.Tables[0];

                //for royaltor search
                Session["FuzzyWorkflowRoyaltorList"] = dropdownListData.Tables[1];
            }
            else
            {
                ExceptionHandler("Error in loading the dropdown list values.", string.Empty);
            }
        }

        private void LoadGridData()
        {
            //WOS-259 - The Statement workflow should always be initially displayed with no data, as it does currently for SuperUser
            dtEmpty = new DataTable();
            gvRoyActivity.EmptyDataText = "No data is displayed initially.";
            gvRoyActivity.DataSource = dtEmpty;
            gvRoyActivity.DataBind();

        }

        private void LoadSearchData()
        {
            string owner = string.Empty;
            string royaltor = string.Empty;

            if (txtOwnSearch.Text != string.Empty)
            {
                try
                {
                    owner = txtOwnSearch.Text.Substring(0, txtOwnSearch.Text.IndexOf("-") - 1);
                }
                catch (Exception ex)
                {
                    msgView.SetMessage("Please select the owner from search list", MessageType.Warning, PositionType.Auto);
                    return;
                }
            }

            if (txtRoyaltor.Text != string.Empty)
            {
                try
                {
                    royaltor = txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1);
                }
                catch (Exception ex)
                {
                    msgView.SetMessage("Please select the royaltor from search list", MessageType.Warning, PositionType.Auto);
                    return;
                }
            }

            workFlowBL = new WorkFlowBL();
            //JIRA-1048 Changes to handle single quote --Start
            DataSet activitySearchData = workFlowBL.GetWorkflowSearchData(ddlCompany.SelectedValue, ddlReportingSch.SelectedValue.ToString(), owner,
                royaltor, ddlResponsibility.SelectedValue.ToString(), ddlMngrResponsibility.SelectedValue.ToString(), ddlPriority.SelectedValue.ToString(), txtProducer.Text.Replace("'", "").Trim(),
                ddlStatus.SelectedValue.ToString(), txtEarnings.Text.Replace("'", "").Trim(), ddlEarningsCompare.SelectedValue, txtClosingBalance.Text.Replace("'", "").Trim(), ddlClosingBalCompare.SelectedValue,
                out errorId);
            //JIRA-1048 Changes to handle single quote--End
            workFlowBL = null;

            rptPager.Visible = false;
            if (activitySearchData.Tables.Count != 0 && errorId != 2)
            {
                Session["dtWorkflowData"] = activitySearchData.Tables[0];
                if (activitySearchData.Tables[0].Rows.Count == 0)
                {
                    gvRoyActivity.DataSource = activitySearchData.Tables[0];
                    gvRoyActivity.EmptyDataText = "No data found for the selected filter criteria.";
                    gvRoyActivity.DataBind();
                }
                else
                {
                    if (activitySearchData.Tables[0].Rows.Count > (hdnGridPageSize.Value == "" ? gridDefaultPageSize : Convert.ToInt32(hdnGridPageSize.Value)))
                    {
                        SetPageStartEndRowNum(activitySearchData.Tables[0].Rows.Count);
                        PopulateGridPage(hdnPageIndex.Value == "" ? 1 : Convert.ToInt32(hdnPageIndex.Value));
                    }
                    else
                    {
                        gvRoyActivity.DataSource = activitySearchData.Tables[0];
                        gvRoyActivity.DataBind();
                    }
                }

            }
            else if (activitySearchData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvRoyActivity.DataSource = dtEmpty;
                gvRoyActivity.EmptyDataText = "No data found for the selected filter criteria.";
                gvRoyActivity.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading search data.", string.Empty);
            }
            UserAuthorization();


        }

        public void RefreshGridData()
        {
            try
            {
                if (hdnPageMode.Value == PageMode.Search.ToString())
                {
                    LoadSearchData();
                }
                else
                {
                    LoadGridData();
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in refreshing the grid data after updating next statemetn run.", ex.Message);
            }


        }

        private void HideUnHideToggleButtons(TableCell cell, bool hideCollapseButton, bool hideExpandButton)
        {
            ImageButton imgExpand = (ImageButton)cell.FindControl("imgExpand");//+
            imgExpand.Visible = !hideExpandButton;
            ImageButton imgCollapse = (ImageButton)cell.FindControl("imgCollapse");//-
            imgCollapse.Visible = !hideCollapseButton;
        }

        private void UpdateStatus(string ownerCode, string royaltorId, string stmtPeriodId, string status, bool isBuklUpdate)
        {
            List<string> stmtRowsToUpdate = new List<string>();
            string owner = string.Empty;
            string royaltor = string.Empty;
            if (txtOwnSearch.Text != string.Empty)
            {
                try
                {
                    owner = txtOwnSearch.Text.Substring(0, txtOwnSearch.Text.IndexOf("-") - 1);
                }
                catch (Exception ex)
                {
                    msgView.SetMessage("Please select the owner from search list", MessageType.Warning, PositionType.Auto);
                    return;
                }
            }

            if (txtRoyaltor.Text != string.Empty)
            {
                try
                {
                    royaltor = txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1);
                }
                catch (Exception ex)
                {
                    msgView.SetMessage("Please select the royaltor from search list", MessageType.Warning, PositionType.Auto);
                    return;
                }
            }

            if (royaltorId != string.Empty && stmtPeriodId != string.Empty)
            {
                //Royaltor level update
                stmtRowsToUpdate.Add(royaltorId + "," + stmtPeriodId);

            }
            else if (ownerCode != string.Empty)
            {
                //Owner level update
                //WUIN-306 change
                //on owner level update, update status of royaltors which are in the screen but do not update all royaltors of that owner            

                string ownerRowRoyaltorId;
                string ownerRowStmtPeriodId;
                string ownerRowOwnerCode;
                string ownerRowOwnerName;
                string royRowStatusCode;
                string stmtHeld;
                RadioButtonList rbListRoy;

                foreach (GridViewRow row in gvRoyActivity.Rows)
                {
                    ownerRowRoyaltorId = (row.FindControl("lblRoyaltor") as Label).Text;
                    ownerRowStmtPeriodId = (row.FindControl("lblStmtPeriodId") as Label).Text;
                    ownerRowOwnerCode = (row.FindControl("lblOwner") as Label).Text;
                    ownerRowOwnerName = (row.FindControl("lblOwnerName") as Label).Text;
                    stmtHeld = (row.FindControl("hdnStmtHeld") as HiddenField).Value;

                    //for a royaltor level row of selected owner
                    if ((ownerRowOwnerCode != "" && ownerRowOwnerName == "") && (ownerCode == ownerRowOwnerCode && ownerRowStmtPeriodId == stmtPeriodId))
                    {
                        rbListRoy = row.FindControl("rblistStatus") as RadioButtonList;
                        royRowStatusCode = rbListRoy.SelectedValue.ToString();

                        //include only the statements which pass the validations
                        //validations:
                        //When a statement is in manager sign off/final sign off/Archive and tried to change status to under review/team sign off by Editor User - do not include this
                        //When a statement is in final sign off/Archive and tried to change status to under review/team sign off/manager sign off - do not include this
                        //When a statement is held and tried to change status other than under review/archive  - do not include this
                        //When a statement is disabled - do not include this                        
                        //No need to check the status as per the availability of manager sign off/team sign off here. This check will cover these scenarios as well

                        if ((hdnUserRole.Value.ToLower() == UserRole.EditorUser.ToString().ToLower()) && (royRowStatusCode == "8" || royRowStatusCode == "3" || royRowStatusCode == "4")
                                && (status == "2" || status == "1"))
                        {
                            continue;
                        }
                        else if ((royRowStatusCode == "3" || royRowStatusCode == "4") && (status == "8" || status == "2" || status == "1"))
                        {
                            continue;
                        }
                        else if (stmtHeld == "Y" && (status == "2" || status == "3" || status == "8"))
                        {
                            continue;
                        }
                        else if (!rbListRoy.Enabled)
                        {
                            continue;
                        }

                        stmtRowsToUpdate.Add((ownerRowRoyaltorId != "" ? ownerRowRoyaltorId : "N/A") + "," + (ownerRowStmtPeriodId != "" ? ownerRowStmtPeriodId : "N/A"));
                    }

                }
            }
            else if (isBuklUpdate)
            {
                //Grid level bulk update
                string royaltorIdToUpdate = string.Empty;
                string stmtPeriodIdToUpdate = string.Empty;
                string stmtStatus = string.Empty;
                string stmtHeld;
                string reCreateStats;
                string dtlFileFlag;
                DataTable dtWorkflowData = (DataTable)Session["dtWorkflowData"];

                foreach (DataRow drStmt in dtWorkflowData.Rows)
                {
                    royaltorIdToUpdate = Convert.ToString(drStmt["royaltor"]);

                    if (royaltorIdToUpdate != string.Empty)
                    {
                        stmtPeriodIdToUpdate = Convert.ToString(drStmt["stmt_period_id"]);
                        stmtStatus = Convert.ToString(drStmt["status_code"]);
                        stmtHeld = Convert.ToString(drStmt["statement_held"]);
                        reCreateStats = Convert.ToString(drStmt["royaltor_stmt_flag"]);
                        dtlFileFlag = Convert.ToString(drStmt["dtl_file_flag"]);

                        //include only the statements which pass the validations
                        //validations:
                        //When a statement is in manager sign off/final sign off/Archive and tried to change status to under review/team sign off by Editor User - do not include this
                        //When a statement is in final sign off/Archive and tried to change status to under review/team sign off/manager sign off - do not include this
                        //When a statement is held and tried to change status other than under review/archive  - do not include this
                        //When a statement is disabled - do not include this  
                        //No need to check the status as per the availability of manager sign off/team sign off here. This check will cover these scenarios as well

                        if ((hdnUserRole.Value.ToLower() == UserRole.EditorUser.ToString().ToLower()) && (stmtStatus == "8" || stmtStatus == "3" || stmtStatus == "4")
                                && (status == "2" || status == "1"))
                        {
                            continue;
                        }
                        else if ((stmtStatus == "3" || stmtStatus == "4") && (status == "8" || status == "2" || status == "1"))
                        {
                            continue;
                        }
                        else if (stmtHeld == "Y" && (status == "2" || status == "3" || status == "8"))
                        {
                            continue;
                        }
                        else if ((!((reCreateStats == "N" && dtlFileFlag == "N") || (reCreateStats == "A" || reCreateStats == "B"))) ||
                                  (stmtStatus == "4" || (stmtStatus == "3" && reCreateStats == "B")))//need to use same checks as in gvRoyActivity_RowDataBound event
                        {
                            continue;
                        }

                        stmtRowsToUpdate.Add(royaltorIdToUpdate + "," + stmtPeriodIdToUpdate);
                    }
                }

                rblistStatusHeader.ClearSelection();

            }

            workFlowBL = new WorkFlowBL();
            //JIRA-1048 Changes to handle single quote --Start
            DataSet activitySearchData = workFlowBL.UpdateWorkflowStatus(hdnPageMode.Value, status, stmtRowsToUpdate.ToArray(), ddlCompany.SelectedValue, ddlReportingSch.SelectedValue.ToString(), owner,
                    royaltor, ddlResponsibility.SelectedValue.ToString(), ddlMngrResponsibility.SelectedValue.ToString(), ddlPriority.SelectedValue.ToString(), txtProducer.Text.Replace("'", "").Trim(),
                    ddlStatus.SelectedValue.ToString(), txtEarnings.Text.Replace("'", "").Trim(), ddlEarningsCompare.SelectedValue, txtClosingBalance.Text.Replace("'", "").Trim(), ddlClosingBalCompare.SelectedValue,
                    Convert.ToString(Session["UserCode"]), out errorId);
            //JIRA-1048 Changes to handle single quote --End
            workFlowBL = null;

            rptPager.Visible = false;
            //WUIN-1057
            ClearRerunRecalBulkControls();

            if (activitySearchData.Tables.Count != 0 && errorId != 2)
            {
                Session["dtWorkflowData"] = activitySearchData.Tables[0];
                if (activitySearchData.Tables[0].Rows.Count == 0)
                {
                    gvRoyActivity.DataSource = activitySearchData.Tables[0];
                    gvRoyActivity.EmptyDataText = "No data found for the selected filter criteria.";
                    gvRoyActivity.DataBind();
                }
                else
                {
                    if (activitySearchData.Tables[0].Rows.Count > (hdnGridPageSize.Value == "" ? gridDefaultPageSize : Convert.ToInt32(hdnGridPageSize.Value)))
                    {
                        SetPageStartEndRowNum(activitySearchData.Tables[0].Rows.Count);
                        PopulateGridPage(hdnPageIndex.Value == "" ? 1 : Convert.ToInt32(hdnPageIndex.Value));
                    }
                    else
                    {
                        gvRoyActivity.DataSource = activitySearchData.Tables[0];
                        gvRoyActivity.DataBind();
                    }
                }

            }
            else if (activitySearchData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvRoyActivity.DataSource = dtEmpty;
                gvRoyActivity.EmptyDataText = "No data found for the selected filter criteria.";
                gvRoyActivity.DataBind();

            }
            else
            {
                ExceptionHandler("Error in updating the status.", string.Empty);
            }
        }

        private bool UpdateRecreateStat()
        {
            hdnIsRecreateStmtUpdated.Value = "";
            List<string> rowsToUpdate = new List<string>();
            string royaltorId;
            string stmtPeriodId;
            string ownerCode;
            string ownerName;
            string originalFlagVal;
            string originalCheckedState;
            string currentCheckedState;
            CheckBox cbReCreateStats;
            bool bBreak = false;

            //WUIN-1057 changes - when header level rerun option is selected, loop through all pages(datatable) and not grid
            //                    else loop through grid(which loops only the current page)
            if (!cbRerunStmtHeader.Checked)
            {
                foreach (GridViewRow row in gvRoyActivity.Rows)
                {
                    cbReCreateStats = (CheckBox)row.FindControl("cbReCreateStats");
                    royaltorId = (row.FindControl("lblRoyaltor") as Label).Text;
                    stmtPeriodId = (row.FindControl("lblStmtPeriodId") as Label).Text;
                    ownerCode = (row.FindControl("lblOwner") as Label).Text;
                    ownerName = (row.FindControl("lblOwnerName") as Label).Text;
                    originalFlagVal = (row.FindControl("lblReCreateStats") as Label).Text;
                    originalCheckedState = originalFlagVal == "Y" ? "Y" : "N";
                    currentCheckedState = cbReCreateStats.Checked == true ? "Y" : "N";

                    if (cbReCreateStats.Enabled == true && originalCheckedState != currentCheckedState)
                    {
                        //if a owner level row is checked, validate if its child rows are not checked changed
                        #region validation
                        if (ownerCode != "" && ownerName != "")
                        {
                            string originalFlagValChild;
                            string originalCheckedStateChild;
                            string currentCheckedStateChild;
                            CheckBox cbReCreateStatsChild;

                            var childRows = from GridViewRow gvRow in gvRoyActivity.Rows
                                            where ((Label)gvRow.FindControl("lblOwner")).Text == ownerCode &&
                                            ((Label)gvRow.FindControl("lblOwnerName")).Text == ""
                                            select gvRow;

                            foreach (var childRow in childRows)
                            {
                                cbReCreateStatsChild = (CheckBox)childRow.FindControl("cbReCreateStats");
                                originalFlagValChild = (childRow.FindControl("lblReCreateStats") as Label).Text;
                                originalCheckedStateChild = originalFlagValChild == "Y" ? "Y" : "N";
                                currentCheckedStateChild = cbReCreateStatsChild.Checked == true ? "Y" : "N";

                                if (cbReCreateStatsChild.Enabled == true && originalCheckedStateChild != currentCheckedStateChild)
                                {
                                    //both owner level and a child row of it are checked to update the recreate stmt
                                    msgView.SetMessage("Please select either owner level or a royaltor level recreate stmt checkbox for the owner - " + ownerCode,
                                                        MessageType.Warning, PositionType.Auto);
                                    bBreak = true;
                                    break;

                                }
                            }

                            if (bBreak)
                            {
                                return false;
                            }

                        }
                        #endregion validation

                        rowsToUpdate.Add((royaltorId != "" ? royaltorId : "N/A") + "," + (stmtPeriodId != "" ? stmtPeriodId : "N/A") + "," +
                                            (ownerName != "" ? ownerCode : "N/A") + "," + currentCheckedState);
                    }

                }
            }
            else
            {
                DataTable dtWorkflowData = (DataTable)Session["dtWorkflowData"];
                string rerunStmtFlag = string.Empty;

                foreach (DataRow drStmt in dtWorkflowData.Rows)
                {
                    rerunStmtFlag = Convert.ToString(drStmt["rerun_stmt"]);
                    royaltorId = Convert.ToString(drStmt["royaltor"]);
                    stmtPeriodId = Convert.ToString(drStmt["stmt_period_id"]);
                    ownerCode = "N/A";//ROYALTOR_ACTIVITY.LEVEL_FLAG is set to 'R' when updating bulk level                    
                    currentCheckedState = rerunStmtFlag;

                    if (rerunStmtFlag == "Y")
                    {
                        rowsToUpdate.Add((royaltorId != "" ? royaltorId : "N/A") + "," + (stmtPeriodId != "" ? stmtPeriodId : "N/A") + "," +
                                                ownerCode + "," + currentCheckedState);
                    }
                }
            }

            if (rowsToUpdate.Count > 0 && !bBreak)
            {
                workFlowBL = new WorkFlowBL();
                loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                workFlowBL.UpdateRecreateStat(rowsToUpdate.ToArray(), loggedUserID, out errorId);
                workFlowBL = null;               
               
                if (errorId == 2)
                {
                    ExceptionHandler("Error in updating the rerun statement flag.", string.Empty);
                    return false;
                }
                hdnIsRecreateStmtUpdated.Value = "Y";

            }

            //WUIN-1057
            ClearRerunRecalBulkControls();

            return true;

        }

        private bool UpdateSummaryStmtDetails(string royaltorId, string stmtPeriodId, out string createFrontSheet, out string isMasterGroupedRoyaltor, out string summaryMasterRotaltor,
                                                out string stmtPeriodSortCode, out string createInvoice)
        {
            //WOS-98 - Update Summary Statement details before generating Statement Front Sheet
            workFlowBL = new WorkFlowBL();
            workFlowBL.UpdateSummaryStmtDetails(royaltorId, stmtPeriodId, out createFrontSheet, out isMasterGroupedRoyaltor, out summaryMasterRotaltor, out stmtPeriodSortCode, out createInvoice, out errorId);
            workFlowBL = null;
            if (errorId != 2)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private void RecalculateFrontSheet()
        {
            string royaltorId;
            string stmtPeriodId;
            CheckBox cbRecalFrntSht;
            int loopRowIndex = 0;
            string ownerCode;
            string ownerName;
            string isPopUpShown = "N";

            if (LoginBO() == true)
            {
                try
                {
                    stmtSaveLocation = GetStmtDirectory();

                    //Connect to SharePoint 
                    if (stmtSaveLocation == "SharePoint")
                    {
                        if (CreateSharePointContext() == true && GetSharePointPDFFolder() == true)
                        {
                            //just connect to SharePoint. do nothing
                        }
                        else
                        {
                            throw new Exception("SharePoint connection error!");
                        }
                    }

                    foreach (GridViewRow row in gvRoyActivity.Rows)
                    {
                        cbRecalFrntSht = (CheckBox)row.FindControl("cbRecalFrntSht");
                        ownerCode = (row.FindControl("lblOwner") as Label).Text;
                        ownerName = (row.FindControl("lblOwnerName") as Label).Text;
                        string createFrontSheet = string.Empty;
                        string createInvoice = string.Empty;
                        string masterGroupedRoyaltor = string.Empty;
                        string summaryMasterRotaltor = string.Empty;
                        string stmtPeriodSortCode = string.Empty;
                        string startOfPeriod;
                        string endOfPeriod;
                        string notifyMessage = string.Empty;
                        loopRowIndex = row.RowIndex;

                        //Harish 09-1-2017 - changed as yes/no popup is being repeated when first row is selected 
                        //if ((ownerCode != "" && ownerName == "") && cbRecalFrntSht.Checked == true && ((row.RowIndex == 0) || 
                        //    row.RowIndex > (hdnReCalFrntShtErrRow.Value == "" ? 0 : Convert.ToInt32(hdnReCalFrntShtErrRow.Value))))
                        if ((ownerCode != "" && ownerName == "") && cbRecalFrntSht.Checked == true &&
                            ((hdnReCalFrntShtErrRow.Value == "") ||
                             ((hdnReCalFrntShtErrRow.Value != "") && row.RowIndex > Convert.ToInt32(hdnReCalFrntShtErrRow.Value))
                            )
                           )
                        {
                            royaltorId = (row.FindControl("lblRoyaltor") as Label).Text;
                            stmtPeriodId = (row.FindControl("lblStmtPeriodId") as Label).Text;

                            //WOS-98 - Front sheet PDF to be generated only if this is the only Statement Period Id on Royaltor Activity for this Royaltor 
                            //          (that is not archived (status = 4)).  
                            //WOS-182 - summary stmt to be generated only if selected royaltor_id = ROYALTOR_GROUPING.SUMMARY_MASTER_ROYALTOR
                            //WOS-279 - Recreate Front Sheets option should also generate Invoice if required
                            //           generate invoice only if front sheet gets generated
                            if (UpdateSummaryStmtDetails(royaltorId, stmtPeriodId, out createFrontSheet, out masterGroupedRoyaltor, out summaryMasterRotaltor, out stmtPeriodSortCode, out createInvoice))
                            {
                                if (createFrontSheet == "Y")
                                {
                                    if (masterGroupedRoyaltor == "Y")
                                    {
                                        startOfPeriod = stmtPeriodSortCode.Split('-')[0];
                                        endOfPeriod = stmtPeriodSortCode.Split('-')[1];
                                        GenerateSummaryStmt(royaltorId, stmtPeriodId, summaryMasterRotaltor, startOfPeriod, endOfPeriod, createInvoice);
                                        hdnReCalFrntShtErrRow.Value = string.Empty;
                                    }
                                    else
                                    {
                                        hdnReCalFrntShtErrRow.Value = row.RowIndex.ToString();
                                        isPopUpShown = "Y";
                                        mpeRecalFrntSht.Show();
                                        notifyMessage = "Could not generate front sheet for royaltor - " + royaltorId + " and statement period - " +
                                                            stmtPeriodId + Environment.NewLine;
                                        notifyMessage += "Details have been updated but Summary is grouped with " + masterGroupedRoyaltor + Environment.NewLine;
                                        notifyMessage += "Do you want to process the rest?";
                                        txtRecalFrntShtMsg.Text = notifyMessage;
                                        break;
                                    }

                                }
                                else
                                {
                                    hdnReCalFrntShtErrRow.Value = row.RowIndex.ToString();
                                    isPopUpShown = "Y";
                                    mpeRecalFrntSht.Show();
                                    notifyMessage = "Could not generate front sheet for royaltor - " + royaltorId + " and statement period - " +
                                                        stmtPeriodId + Environment.NewLine;
                                    notifyMessage += "More than one statement for Royaltor, cannot recalculate the Front Sheet" + Environment.NewLine;
                                    notifyMessage += "Do you want to process the rest?";
                                    txtRecalFrntShtMsg.Text = notifyMessage;
                                    break;
                                }

                            }
                            else
                            {
                                ExceptionHandler("Error in updating front sheet details of royaltor - " + royaltorId, string.Empty);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler("Error in recalculating front sheet.", ex.Message);
                }
                finally
                {
                    LogOffBO();
                }

                //if (hdnReCalFrntShtErrRow.Value == string.Empty)
                if (loopRowIndex == gvRoyActivity.Rows.Count - 1 && isPopUpShown == "N")
                {
                    foreach (GridViewRow row in gvRoyActivity.Rows)
                    {
                        cbRecalFrntSht = (CheckBox)row.FindControl("cbRecalFrntSht");
                        cbRecalFrntSht.Checked = false;
                    }
                    LoadSearchData();
                    msgView.SetMessage("Recalculation of front sheets completed successfully.",
                                    MessageType.Warning, PositionType.Auto);
                }

            }
            else
            {
                ExceptionHandler("Could not login to Business Objects.", string.Empty);
            }
                        
        }
                
        private void FuzzySearchRoyaltor()
        {

            if (txtRoyaltor.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in royaltor filter field", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "Royaltor";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzyWorkflowRoyaltorList(txtRoyaltor.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void FuzzySearchOwner()
        {

            if (txtOwnSearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in owner filter field", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "Owner";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzyWorkflowOwnerList(txtOwnSearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void PopulateCommentAttachmentFiles()
        {
            string fileFolderPath = @AppDomain.CurrentDomain.BaseDirectory + @"\WorkflowCommentAttachments\" + Session["WARSAffiliate"].ToString() + @"\";
            string fileNameContains = hdnCommentRoyaltorId.Value + "_" + hdnCommentStmtPeriodId.Value + "_";

            //string[] fileArray = Directory.GetFiles(fileFolderPath, "*" + fileNameContains + "*", SearchOption.TopDirectoryOnly); // to get behaviour of Contains
            string[] fileArray = Directory.GetFiles(fileFolderPath, fileNameContains + "*", SearchOption.TopDirectoryOnly); // to get behaviour of file name starts with

            DataTable dtFiles = new DataTable();
            dtFiles.Columns.Add("FileName", typeof(string));

            foreach (string fileArrayName in fileArray)
            {
                string fileName = Path.GetFileName(fileArrayName);
                fileName = fileName.Replace(fileNameContains, string.Empty);
                dtFiles.Rows.Add(fileName);
            }

            if (dtFiles.Rows.Count == 0)
            {
                dtEmpty = new DataTable();
                gvCommentDownloadFile.EmptyDataText = "No attachments for the statement";
                gvCommentDownloadFile.DataSource = dtEmpty;
                gvCommentDownloadFile.DataBind();
            }
            else
            {
                gvCommentDownloadFile.DataSource = dtFiles;
                gvCommentDownloadFile.DataBind();

                plnGridCommentDownloadFile.Style.Add("height", ((Convert.ToDouble(hdnGridPnlHeight.Value) * 0.2) - 5).ToString());
            }
            UserAuthorization();

        }

        private bool IsAttachmentExistForStmt(string royaltorId, string stmtPeriodId)
        {
            bool attachmentExist = false;
            string fileFolderPath = @AppDomain.CurrentDomain.BaseDirectory + @"\WorkflowCommentAttachments\" + Session["WARSAffiliate"].ToString() + @"\";
            string fileNameStartWith = royaltorId + "_" + stmtPeriodId + "_";
            string[] fileArray = Directory.GetFiles(fileFolderPath, fileNameStartWith + "*", SearchOption.TopDirectoryOnly); // to get behaviour of file name starts with

            if (fileArray.Length > 0)
            {
                attachmentExist = true;
            }

            return attachmentExist;
        }

        /// <summary>
        /// WUIN- 920 - Bulk update of the status
        /// Validations:
        /// 1.Status can be updated from any status to any status        
        /// 2.When a statement is in final sign off/Archive and tried to change status to under review/team sign off/manager sign off 
        ///     Confirmation message and if selected to proceed, update all other than archive/final sign off to the selected status
        /// 3.Editor User can't change the status of a statement from Manager Sign off/Final Sign off/Archive back to Team Sign off/Under Review    
        ///     Confirmation message and if selected to proceed, update all other than Manager Sign off/Final Sign off/Archive to the selected status
        /// 4.Check for all statements under the owner if any stmt is disabled to be updated to the selected status then    
        ///     Confirmation message and if selected to proceed, update all other than disabled to selected status
        /// </summary>        
        private bool ValidateGridLevelStatusUpdate()
        {
            DataTable dtWorkflowData = new DataTable();
            bool isValid = true;
            string warningMsg = string.Empty;
            string royaltorId = string.Empty;
            string stmtStatus = string.Empty;
            string royaltorStmtFlag = string.Empty;
            bool stmtDisabled = false;
            bool hasHeldStmt = false;
            string stmtHeld;
            string reCreateStats;
            string dtlFileFlag;

            string changedStatus = rblistStatusHeader.SelectedValue.ToString();

            if (Session["dtWorkflowData"] != null)
            {
                dtWorkflowData = (DataTable)Session["dtWorkflowData"];
            }

            if (dtWorkflowData.Rows.Count == 0)
            {
                msgView.SetMessage("No statements present to update status", MessageType.Warning, PositionType.Auto);
                return false;
            }

            //check if any statement is disabled to be updated to the selected status 
            foreach (DataRow drStmt in dtWorkflowData.Rows)
            {
                royaltorId = Convert.ToString(drStmt["royaltor"]);

                //check only for the royaltor level rows not the owner rows
                if (royaltorId == string.Empty)
                {
                    continue;
                }

                stmtHeld = Convert.ToString(drStmt["statement_held"]);
                stmtStatus = Convert.ToString(drStmt["status_code"]);
                reCreateStats = Convert.ToString(drStmt["royaltor_stmt_flag"]);
                dtlFileFlag = Convert.ToString(drStmt["dtl_file_flag"]);

                if (stmtHeld == "Y")
                {
                    hasHeldStmt = true;
                }

                //need to use same checks as in gvRoyActivity_RowDataBound event
                if (!((reCreateStats == "N" && dtlFileFlag == "N") || (reCreateStats == "A" || reCreateStats == "B")) ||
                     (stmtStatus == "4" || (stmtStatus == "3" && reCreateStats == "B")))
                {
                    stmtDisabled = true;
                }

            }


            foreach (DataRow drStmt in dtWorkflowData.Rows)
            {
                royaltorId = Convert.ToString(drStmt["royaltor"]);

                //validate only for the royaltor level rows not the owner rows
                if (royaltorId == string.Empty)
                {
                    continue;
                }

                stmtHeld = Convert.ToString(drStmt["statement_held"]);
                stmtHeld = Convert.ToString(drStmt["statement_held"]);
                stmtStatus = Convert.ToString(drStmt["status_code"]);
                reCreateStats = Convert.ToString(drStmt["royaltor_stmt_flag"]);
                dtlFileFlag = Convert.ToString(drStmt["dtl_file_flag"]);


                //JIRA-983 Changes by Ravi on 20/02/2019 -- Start
                if (((hdnUserRole.Value == UserRole.EditorUser.ToString()) ||
                    (hdnUserRole.Value == UserRole.Supervisor.ToString())) &&
                    ((hdnMngrSignOffVisible.Value == "Y" && stmtStatus == "8") || stmtStatus == "3" || stmtStatus == "4")
                    && (changedStatus == "2" || changedStatus == "1"))
                {
                    //JIRA-983 Changes by Ravi on 20/02/2019 -- End

                    //When a statement is in manager sign off/final sign off/Archive and tried to change status to under review/team sign off by an Editor user
                    //  AND/OR if any statement is disabled to be updated to the selected status then
                    //  Confirmation message and if selected to proceed, update all other than manager sign off/final sign off/archive to the selected status

                    if (hdnTeamSignOffVisible.Value == "Y" && hdnMngrSignOffVisible.Value == "Y")
                    {
                        warningMsg = "There are statements in manager sign off/final sign off/archive";
                    }
                    else if (hdnTeamSignOffVisible.Value == "N" && hdnMngrSignOffVisible.Value == "N")
                    {
                        warningMsg = "There are statements in final sign off/archive";
                    }
                    else if (hdnTeamSignOffVisible.Value == "Y" && hdnMngrSignOffVisible.Value == "N")
                    {
                        warningMsg = "There are statements in final sign off/archive";
                    }
                    else if (hdnTeamSignOffVisible.Value == "N" && hdnMngrSignOffVisible.Value == "Y")
                    {
                        warningMsg = "There are statements in manager sign off/final sign off/archive";
                    }

                    if (stmtDisabled || hasHeldStmt)
                    {
                        warningMsg = warningMsg + " and/or statements which are held/disabled";
                    }

                    warningMsg = warningMsg + " which can’t be moved to the selected status by an Editor User. Do you want to continue for the rest?";

                    lblMsgStatusBulkUpdateConfirm.Text = warningMsg;

                    mpeStatusBulkUpdateConfirm.Show();
                    isValid = false;
                    break;
                }
                else if ((stmtStatus == "3" || stmtStatus == "4") && (changedStatus == "8" || changedStatus == "2" || changedStatus == "1"))
                {
                    //When a statement is in final sign off/Archive and tried to change status to under review/team sign off/manager sign off 
                    //  AND/OR if any statement is disabled to be updated to the selected status then
                    //  Confirmation message and if selected to proceed, update all other than final sign off/archive to the selected status
                    //No need to check the status as per the availability of manager sign off/team sign off here. This check will cover these scenarios as well
                    warningMsg = "There are statements in final sign off/archive";

                    if (stmtDisabled || hasHeldStmt)
                    {
                        warningMsg = warningMsg + " and/or statements which are held/disabled";
                    }

                    warningMsg = warningMsg + " which can’t be moved to the selected status. Do you want to continue for the rest?";

                    lblMsgStatusBulkUpdateConfirm.Text = warningMsg;
                    mpeStatusBulkUpdateConfirm.Show();

                    isValid = false;
                    break;
                }
                else if (stmtHeld == "Y" && (changedStatus == "2" || changedStatus == "3" || changedStatus == "8"))
                {
                    //held statements can only be updated to Under review/Archive
                    warningMsg = "There are statements which are held";

                    if (stmtDisabled)
                    {
                        warningMsg = "There are statements which are held/disabled";
                    }

                    warningMsg = warningMsg + " which can’t be moved to the selected status. Do you want to continue for the rest?";

                    lblMsgStatusBulkUpdateConfirm.Text = warningMsg;
                    mpeStatusBulkUpdateConfirm.Show();

                    isValid = false;
                    break;
                }
                else if ((!((reCreateStats == "N" && dtlFileFlag == "N") || (reCreateStats == "A" || reCreateStats == "B"))) ||
                          (stmtStatus == "4" || (stmtStatus == "3" && reCreateStats == "B")))//need to use same checks as in gvRoyActivity_RowDataBound event
                {
                    warningMsg = "There are statements which are disabled which can’t be moved to the selected status. Do you want to continue for the rest?";

                    lblMsgStatusBulkUpdateConfirm.Text = warningMsg;
                    mpeStatusBulkUpdateConfirm.Show();

                    isValid = false;
                    break;
                }

            }

            return isValid;
        }

        //WUIN-1057 - changes
        //clear bulk selection of Rerun statement/Recalculate Stmt Summary checkbox's
        private void ClearRerunRecalBulkControls()
        {            
            cbRerunStmtHeader.Checked = false;
            cbRecalSummaryHeader.Checked = false;
        }

        #endregion METHODS

        #region GRID PAGING
        /// <summary>
        /// object to hold page number, start and end row numbers
        /// </summary>
        struct PageStartEnd
        {
            int pageNum;
            int startRowNum;
            int endRowNum;

            public int PageNum
            {
                get { return pageNum; }
                set
                {
                    pageNum = value;
                }
            }

            public int StartRowNum
            {
                get { return startRowNum; }
                set
                {
                    startRowNum = value;
                }
            }

            public int EndRowNum
            {
                get { return endRowNum; }
                set
                {
                    endRowNum = value;
                }
            }

        };

        private void SetPageStartEndRowNum(int recordCount)
        {
            DataTable dtWorkflowData = (DataTable)Session["dtWorkflowData"];

            //set the page size based on maximum royaltors under a owner
            DataTable dt0 = GroupBy("owner_code", "rownum", dtWorkflowData);
            if (dt0.Rows.Count != 0)
            {
                //WUIN-874 - change
                DataTable dt01 = new DataTable();
                if (dt0.Select("owner_code<>'' AND owner_code<>'N/A'").Count() > 0)
                {
                    if (dt0.Select("owner_code<>'' AND owner_code<>'N/A'").CopyToDataTable().Select("count=MAX(count)").Count() > 0)
                    {
                        dt01 = dt0.Select("owner_code<>'' AND owner_code<>'N/A'").CopyToDataTable().Select("count=MAX(count)").CopyToDataTable();
                    }
                }

                if (dt01.Rows.Count != 0)
                {
                    if (Convert.ToInt32(dt01.Rows[0]["count"]) > gridDefaultPageSize)
                    {
                        hdnGridPageSize.Value = (Convert.ToInt32(dt01.Rows[0]["count"]) + 10).ToString();
                    }
                    else
                    {
                        hdnGridPageSize.Value = gridDefaultPageSize.ToString();
                    }
                }
                else
                {
                    hdnGridPageSize.Value = gridDefaultPageSize.ToString();
                }
            }


            double dblPageCount = (double)((decimal)recordCount / decimal.Parse(hdnGridPageSize.Value.ToString()));
            int pageCount = (int)Math.Ceiling(dblPageCount);
            List<PageStartEnd> pages = new List<PageStartEnd>();
            int currentPageStartRowNum = 1;
            int currentPageEndRowNum;
            int roysWithoutOwnersRowNum = 0;
            string query;
            PageStartEnd page;

            if (pageCount > 0)
            {

                //get the row number from where royaltors without owners start (owcer_code='N/A')
                //modified by harish 03-06-16
                //DataTable dt1 = dtWorkflowData.Select("owner_code='N/A'").CopyToDataTable().Select("rownum=MIN(rownum)").CopyToDataTable();
                DataTable dt1 = new DataTable();
                if (dtWorkflowData.Select("owner_code='N/A'").Count() != 0)
                    dt1 = dtWorkflowData.Select("owner_code='N/A'").CopyToDataTable().Select("rownum=MIN(rownum)").CopyToDataTable();

                if (dt1.Rows.Count != 0)
                {
                    roysWithoutOwnersRowNum = Convert.ToInt32(dt1.Rows[0]["rownum"]);
                }
                else
                {
                    roysWithoutOwnersRowNum = dtWorkflowData.Rows.Count + 1; //if there is no single royaltor then assing this as rowcount + 1
                }

                //set the page number, start and end row numbers for each page
                for (int i = 1; i <= pageCount; i++)
                {
                    currentPageEndRowNum = i * (hdnGridPageSize.Value == "" ? gridDefaultPageSize : Convert.ToInt32(hdnGridPageSize.Value));

                    if ((roysWithoutOwnersRowNum >= currentPageStartRowNum && roysWithoutOwnersRowNum <= currentPageEndRowNum)
                        || (currentPageStartRowNum > roysWithoutOwnersRowNum))
                    {
                        query = "(rownum>=" + currentPageStartRowNum + " AND rownum<=" + currentPageEndRowNum + ")";

                    }
                    else
                    {
                        query = "(rownum>=" + currentPageStartRowNum + " AND rownum<=" + currentPageEndRowNum + ") AND (owner_code<>'') AND (owner_name<>'') ";

                    }

                    DataTable dt2 = new DataTable();
                    if (dtWorkflowData.Select(query).Count() > 0)
                    {
                        dt2 = dtWorkflowData.Select(query).CopyToDataTable().Select("rownum=MAX(rownum)").CopyToDataTable();
                    }

                    if (dt2.Rows.Count != 0)
                    {
                        //WOS-281 - changes
                        /* code before changes
                        if(pageCount > 1)
                         * */
                        if (pageCount > 1 && i < pageCount)
                            currentPageEndRowNum = Convert.ToInt32(dt2.Rows[0]["rownum"]) - 1;
                        else
                            currentPageEndRowNum = Convert.ToInt32(dt2.Rows[0]["rownum"]);

                    }

                    page = new PageStartEnd();
                    page.PageNum = i;
                    page.StartRowNum = currentPageStartRowNum;
                    page.EndRowNum = currentPageEndRowNum;
                    pages.Add(page);

                    //currentPageStartRowNum = currentPageEndRowNum;
                    currentPageStartRowNum = currentPageEndRowNum + 1;
                }

            }

            Session["PageStartEnd"] = pages;

        }

        private DataTable GroupBy(string groupByColumn, string aggregateColumn, DataTable sourceTable)
        {

            DataView dv = new DataView(sourceTable);

            //getting distinct values for group column
            DataTable dtGroup = dv.ToTable(true, new string[] { groupByColumn });

            //adding column for the row count
            dtGroup.Columns.Add("Count", typeof(int));

            //looping thru distinct values for the group, counting
            foreach (DataRow dr in dtGroup.Rows)
            {
                dr["Count"] = sourceTable.Compute("Count(" + aggregateColumn + ")", groupByColumn + " = '" + dr[groupByColumn] + "'");
            }

            //returning grouped/counted result
            return dtGroup;
        }

        private void PopulateGridPage(int pageIndex)
        {

            int startingRowNum = 0;
            int endingRowNum = 0;

            List<PageStartEnd> pages = (List<PageStartEnd>)Session["PageStartEnd"];
            var page = pages.Where(p => p.PageNum == pageIndex);
            foreach (var p in page)
            {
                startingRowNum = p.StartRowNum;
                endingRowNum = p.EndRowNum;
            }

            DataTable dtWorkflowData = (DataTable)Session["dtWorkflowData"];
            //WOS-281 changes
            /*code before changes
             * if (startingRowNum != endingRowNum)
             * */
            {
                //WUIN-896
                DataTable dt3 = new DataTable();
                if (dtWorkflowData.Select("rownum>=" + startingRowNum + "AND rownum<=" + endingRowNum).Count() > 0)
                {
                    dt3 = dtWorkflowData.Select("rownum>=" + startingRowNum + "AND rownum<=" + endingRowNum).CopyToDataTable();
                }

                if (dt3.Rows.Count != 0)
                {
                    gvRoyActivity.DataSource = dt3;
                }
                else
                {
                    gvRoyActivity.EmptyDataText = "No data found for the selected filter criteria.";
                }
                gvRoyActivity.DataBind();
            }

            PopulatePager(dtWorkflowData.Rows.Count, pageIndex, (hdnGridPageSize.Value == "" ? gridDefaultPageSize : Convert.ToInt32(hdnGridPageSize.Value)));
        }

        private void PopulatePager(int recordCount, int currentPage, int pageSize)
        {
            rptPager.Visible = true;
            double dblPageCount = (double)((decimal)recordCount / decimal.Parse(pageSize.ToString()));
            int pageCount = (int)Math.Ceiling(dblPageCount);
            List<System.Web.UI.WebControls.ListItem> pages = new List<System.Web.UI.WebControls.ListItem>();
            if (pageCount > 0)
            {
                //To display only 10 pages on screen at a time
                //Divide total pages into different groups with 10 pages on each group
                //for example 
                //page group 0 ->page no 1 to 10 
                //page group 1 ->page no 11 to 20 ... 
                //calculate page group of currentpage
                int pageGroup = (int)Math.Floor((decimal)(currentPage - 1) / 10);

                //Add First page 
                pages.Add(new System.Web.UI.WebControls.ListItem("First", "1", currentPage > 1));

                //if page group > 0 means current page is >10. Add ... to screen 
                //And its page index is last page of previous page group
                if (pageGroup > 0)
                {
                    pages.Add(new System.Web.UI.WebControls.ListItem("...", (pageGroup * 10).ToString(), true));
                }

                //Add pages based on page group
                //If selected page is 5, its pagegroup is 0 so add 1 to 10 page no
                //If selected page is 18, its pagegroup is 1 so add 11 to 20 page no
                for (int i = 1; i <= 10; i++)
                {
                    int pageIndex = (pageGroup * 10) + i;
                    if (pageIndex <= pageCount)
                    {
                        pages.Add(new System.Web.UI.WebControls.ListItem(pageIndex.ToString(), pageIndex.ToString(), pageIndex != currentPage));
                    }
                }

                //If total page count is more than 10 and we are not on last page group then add ...
                //And its index is first page of next page group
                if (pageCount > 10 && ((pageCount - (pageGroup * 10)) > 10))
                {
                    pages.Add(new System.Web.UI.WebControls.ListItem("...", ((pageGroup * 10) + 11).ToString(), currentPage < pageCount));
                }

                //Finally ad Last page
                pages.Add(new System.Web.UI.WebControls.ListItem("Last", pageCount.ToString(), currentPage < pageCount));
            }
            rptPager.DataSource = pages;
            rptPager.DataBind();
        }

        protected void lnkPage_Click(object sender, EventArgs e)
        {
            /* this is handled on client side
             
            //WOS-253- validation - Warning message if change page or filter when checkboxes are selected but not processed
            CheckBox cbRecalFrntSht;
            foreach (GridViewRow row in gvRoyActivity.Rows)
            {
                cbRecalFrntSht = (CheckBox)row.FindControl("cbRecalFrntSht");
                if (cbRecalFrntSht.Checked == true)
                {
                    msgView.SetMessage("Selected recalculate front sheets have not been processed. Please unselect to proceed.",
                                    MessageType.Warning, PositionType.Auto);
                    return;
                }
            }
             * */

            //WOS-211
            //Validation - warning if recreate stmt checkbox is ticked and not updated before changing page  
            //WUIN-1057 changes - This validation to be done only when header level re-run statement option is not checked
            bool changePage = true;
            string originalFlagVal;
            string originalCheckedState;
            string currentCheckedState;
            CheckBox cbReCreateStats;

            if (!cbRerunStmtHeader.Checked)
            {
                foreach (GridViewRow row in gvRoyActivity.Rows)
                {
                    cbReCreateStats = (CheckBox)row.FindControl("cbReCreateStats");
                    originalFlagVal = (row.FindControl("lblReCreateStats") as Label).Text;
                    originalCheckedState = originalFlagVal == "Y" ? "Y" : "N";
                    currentCheckedState = cbReCreateStats.Checked == true ? "Y" : "N";

                    if (cbReCreateStats.Enabled == true && originalCheckedState != currentCheckedState)
                    {
                        msgView.SetMessage("Please update the Statement Activity List for the current page.",
                                                        MessageType.Warning, PositionType.Auto);
                        changePage = false;
                        break;
                    }
                }
            }

            if (changePage)
            {
                int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
                hdnPageIndex.Value = pageIndex.ToString();
                PopulateGridPage(pageIndex);
                hdnAllowGridPageChange.Value = "Y";//reset to default
            }
            UserAuthorization();

        }

        #endregion GRID PAGING

        #region Recalculate Front Sheet

        string baseURL = ConfigurationManager.AppSettings["BOServerBaseURL"];
        int webReqTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["WebReqTimeout"].ToString());
        string rwsLogonToken;
        //static string defaultShareIdOfSummStmt = "0";//WOS-108 //WUIN-154
        DataTable dtIntPartyIds;

        private bool LoginBO()
        {
            string userName = Utilities.GetBOAccountUserId();
            string password = Utilities.GetBOAccountUserPassoword();
            string auth = "secEnterprise";
            string LogonURI = baseURL + "logon/long";

            try
            {
                //Making GET Request to  /logon/long to receive XML template.        
                HttpWebRequest myWebRequest = (HttpWebRequest)WebRequest.Create(LogonURI);
                myWebRequest.ContentType = "application/xml";
                myWebRequest.Method = "GET";

                //Returns the response to the request made
                WebResponse myWebResponse = myWebRequest.GetResponse();
                //Creating an instance of StreamReader to read the data stream from the resource
                StreamReader sr = new StreamReader(myWebResponse.GetResponseStream());
                //Reads all the characters from the current position to the end of the stream and store it as string
                string output = sr.ReadToEnd();
                //Initialize a new instance of the XmlDocument class
                XmlDocument doc = new XmlDocument();
                //Loads the document from the specified URI
                doc.LoadXml(output);

                //Returns an XmlNodeList containing a list of all descendant elements 
                //that match the specified name i.e. attr
                XmlNodeList nodelist = doc.GetElementsByTagName("attr");
                //  Add the logon parameters to the attribute nodes of the document
                foreach (XmlNode node in nodelist)
                {
                    if (node.Attributes["name"].Value == "userName")
                        node.InnerText = userName;

                    if (node.Attributes["name"].Value == "password")
                        node.InnerText = password;

                    if (node.Attributes["name"].Value == "auth")
                        node.InnerText = auth;
                }

                //Making POST request to /logon/long to receive a logon token            
                WebRequest myWebRequest1 = WebRequest.Create(LogonURI);
                myWebRequest1.ContentType = "application/xml";
                myWebRequest1.Method = "POST";

                byte[] reqBodyBytes = System.Text.Encoding.Default.GetBytes(doc.OuterXml);
                Stream reqStream = myWebRequest1.GetRequestStream();
                reqStream.Write(reqBodyBytes, 0, reqBodyBytes.Length);
                reqStream.Close();

                using (var myWebResponse1 = myWebRequest1.GetResponse())
                {
                    //Finding the value of the X-SAP-LogonToken
                    rwsLogonToken = myWebResponse1.Headers["X-SAP-LogonToken"].ToString();
                }


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        private void LogOffBO()
        {
            string LogOffURI = baseURL + "logoff";

            try
            {
                //Making POST request to /logoff to log off the BI Platform
                WebRequest myWebRequest2 = WebRequest.Create(LogOffURI);
                myWebRequest2.ContentType = "application/xml";
                myWebRequest2.Headers.Add("X-SAP-LogonToken", rwsLogonToken);
                myWebRequest2.Method = "POST";

                //Checking for the response
                WebResponse myWebResponse2 = myWebRequest2.GetResponse();
                StreamReader sr1 = new StreamReader(myWebResponse2.GetResponseStream());
                string output1 = sr1.ReadToEnd();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in logging off from Business Objects.", ex.Message);
            }
        }

        private void GenerateSummaryStmt(string royaltorID, string stmtPeriodID, string summaryMasterRotaltor, string startOfPeriod, string endOfPeriod, string createInvoice)
        {
            try
            {
                string summaryStmtCUID = GetSummaryStmtCUID();

                if (summaryStmtCUID != string.Empty && summaryStmtCUID != "0")
                {
                    string reportID = GetReportIDfromCUID(summaryStmtCUID);
                    string summaryStmtBORptTabId = string.Empty;
                    string InvStmtBORptTabId = string.Empty;
                    GetSummInvRptTabId(reportID, out summaryStmtBORptTabId, out InvStmtBORptTabId);//WOS-172

                    //get interested party ids
                    workFlowBL = new WorkFlowBL();
                    DataSet dsIntPartyIds = workFlowBL.GetIntPartyIds(royaltorID, out errorId);
                    workFlowBL = null;

                    if (dsIntPartyIds.Tables.Count != 0 && errorId != 2)
                    {
                        dtIntPartyIds = dsIntPartyIds.Tables[0];

                        GenerateSummStmt(reportID, summaryStmtBORptTabId, royaltorID, stmtPeriodID, summaryMasterRotaltor, startOfPeriod, endOfPeriod);

                        if (createInvoice == "Y")
                        {
                            GenerateInvtmtPrimaryPayee(reportID, InvStmtBORptTabId, royaltorID, stmtPeriodID, summaryMasterRotaltor, startOfPeriod, endOfPeriod);
                        }

                        GenerateInvStmtNonPrimaryPayee(reportID, InvStmtBORptTabId, royaltorID, stmtPeriodID, summaryMasterRotaltor, startOfPeriod, endOfPeriod);

                    }
                    else
                    {
                        ExceptionHandler("Error in recalculating front sheet (fetching Interested party Ids of the royaltor - " + royaltorID + ")", string.Empty);
                    }


                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        /// <summary>
        /// Generate summary statement for primary and non primary payees
        /// </summary>       
        private void GenerateSummStmt(string reportID, string rptTabId, string royaltorID, string stmtPeriodID, string summaryMasterRoyaltor, string startOfPeriod, string endOfPeriod)
        {
            try
            {
                string intPartyId = string.Empty;
                foreach (DataRow dr in dtIntPartyIds.Rows)
                {
                    intPartyId = dr["int_party_id"].ToString();

                    PassParameters(reportID, royaltorID, stmtPeriodID, intPartyId);
                    RefreshReport(reportID);
                    SaveReport((dr["primary_payee"].ToString() == "Y" ? StmtType.SummaryStmtPrimaryPayee.ToString() : StmtType.SummaryStmtNonPrimaryPayee.ToString()),
                                reportID, rptTabId, royaltorID, stmtPeriodID, summaryMasterRoyaltor, intPartyId, startOfPeriod, endOfPeriod);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Generate invoice statement for primary payee
        /// WUIN-1125 - generate invoice when INTERESTED_PARTY.GENERATE_INVOICE = 'Y'
        /// WUIN-389 - generate invoice when ROYALTOR_PAYEE.PAYEE_PERCENTAGE > 0
        /// </summary>       
        private void GenerateInvtmtPrimaryPayee(string reportID, string rptTabId, string royaltorID, string stmtPeriodID, string summaryMasterRoyaltor, string startOfPeriod, string endOfPeriod)
        {
            try
            {
                string intPartyId = string.Empty;

                DataRow[] dtIntPartyIdPrimaryPayee = dtIntPartyIds.Select("primary_payee = 'Y' AND generate_invoice = 'Y' AND payee_percentage > 0");

                foreach (DataRow dr in dtIntPartyIdPrimaryPayee)
                {
                    intPartyId = dr["int_party_id"].ToString();

                    PassParameters(reportID, royaltorID, stmtPeriodID, intPartyId);
                    RefreshReport(reportID);
                    SaveReport(StmtType.InvoiceStmtPrimaryPayee.ToString(), reportID, rptTabId, royaltorID, stmtPeriodID, summaryMasterRoyaltor, intPartyId, startOfPeriod, endOfPeriod);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Generate invoice statement for non primary payee
        /// WUIN-1125 - generate invoice when INTERESTED_PARTY.GENERATE_INVOICE = 'Y'
        /// WUIN-389 - generate invoice when ROYALTOR_PAYEE.PAYEE_PERCENTAGE > 0
        /// </summary>       
        private void GenerateInvStmtNonPrimaryPayee(string reportID, string rptTabId, string royaltorID, string stmtPeriodID, string summaryMasterRoyaltor, string startOfPeriod, string endOfPeriod)
        {
            try
            {
                string intPartyId = string.Empty;

                DataRow[] dtIntPartyIdPrimaryPayee = dtIntPartyIds.Select("(primary_payee <> 'Y' OR primary_payee IS NULL) AND generate_invoice = 'Y' AND payee_percentage > 0");

                foreach (DataRow dr in dtIntPartyIdPrimaryPayee)
                {
                    intPartyId = dr["int_party_id"].ToString();

                    PassParameters(reportID, royaltorID, stmtPeriodID, intPartyId);
                    RefreshReport(reportID);
                    SaveReport(StmtType.InvoiceStmtNonPrimaryPayee.ToString(), reportID, rptTabId, royaltorID, stmtPeriodID, summaryMasterRoyaltor, intPartyId, startOfPeriod, endOfPeriod);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        private string GetStmtDirectory()
        {
            string stmtDirectory = string.Empty;
            try
            {
                workFlowBL = new WorkFlowBL();
                workFlowBL.GetStmtDirectory(out stmtDirectory, out errorId);
                workFlowBL = null;
                if (errorId == 2)
                    ExceptionHandler("Error in fetching statement directory.", string.Empty);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fetching statement directory.", ex.Message);
            }

            return stmtDirectory;
        }

        private string GetSummaryStmtCUID()
        {
            string summaryStmtCUID = string.Empty;
            try
            {
                CommonBL commonBL = new CommonBL();
                commonBL.GetSummaryStmtCUID(out summaryStmtCUID, out errorId);
                commonBL = null;
                if (errorId == 2)
                    ExceptionHandler("Error in fetching summary statement report CUID.", string.Empty);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fetching summary statement report CUID.", ex.Message);
            }

            return summaryStmtCUID;
        }

        private string GetReportIDfromCUID(string cuid)
        {
            string reportID = string.Empty;
            string InfoStoreURI = baseURL + "infostore/cuid_" + cuid;
            HttpWebRequest myWebRequestParam = (HttpWebRequest)WebRequest.Create(InfoStoreURI);
            myWebRequestParam.ContentType = "application/xml";
            myWebRequestParam.Headers.Add("X-SAP-LogonToken", rwsLogonToken);
            myWebRequestParam.Timeout = webReqTimeout;
            myWebRequestParam.Method = "GET";
            WebResponse myWebResponseParam = myWebRequestParam.GetResponse();
            StreamReader srParam = new StreamReader(myWebResponseParam.GetResponseStream());
            string outputParam = srParam.ReadToEnd();
            XmlDocument docParam = new XmlDocument();
            docParam.LoadXml(outputParam);

            XmlNodeList nodelist = docParam.GetElementsByTagName("attr");
            foreach (XmlNode node in nodelist)
            {
                if (node.Attributes["name"].Value == "id")
                    reportID = node.InnerText;
            }

            return reportID;
        }

        private void GetSummInvRptTabId(string reportID, out string summTabId, out string invTabId)
        {
            string tabId = string.Empty;
            summTabId = string.Empty;
            invTabId = string.Empty;
            string summaryStmtBORptTabName = ConfigurationManager.AppSettings["SummaryStmtBORptTabName"].ToString();
            string invoiceStmtBORptTabName = ConfigurationManager.AppSettings["InvoiceStmtBORptTabName"].ToString();
            try
            {
                string InfoStoreURI = baseURL + "raylight/v1/documents/" + reportID + "/reports";

                HttpWebRequest myWebRequestParam = (HttpWebRequest)WebRequest.Create(InfoStoreURI);
                myWebRequestParam.ContentType = "application/xml";
                myWebRequestParam.Headers.Add("X-SAP-LogonToken", rwsLogonToken);
                myWebRequestParam.Timeout = webReqTimeout;
                myWebRequestParam.Method = "GET";
                WebResponse myWebResponseParam = myWebRequestParam.GetResponse();
                StreamReader srParam = new StreamReader(myWebResponseParam.GetResponseStream());
                string outputParam = srParam.ReadToEnd();
                XmlDocument docParam = new XmlDocument();
                docParam.LoadXml(outputParam);

                //Returns an XmlNodeList containing a list of all descendant elements  
                string paramId = string.Empty;
                XmlNodeList paramNodelist = docParam.GetElementsByTagName("report");

                foreach (XmlNode paramNode in paramNodelist)
                {
                    foreach (XmlNode childNode1 in paramNode.ChildNodes)
                    {
                        if (childNode1.Name == "id")
                        {
                            tabId = childNode1.InnerText;
                        }
                        else if (childNode1.Name == "name" && childNode1.InnerText == summaryStmtBORptTabName && tabId != string.Empty)
                        {
                            summTabId = tabId;
                            tabId = string.Empty;
                        }
                        else if (childNode1.Name == "name" && childNode1.InnerText == invoiceStmtBORptTabName && tabId != string.Empty)
                        {
                            invTabId = tabId;
                            tabId = string.Empty;
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void PassParameters(string reportID, string royaltorID, string stmtPeriodID, string courtesyShareId)
        {
            string InfoStoreURI = baseURL + "raylight/v1/documents/" + reportID + "/parameters";

            HttpWebRequest myWebRequestParam = (HttpWebRequest)WebRequest.Create(InfoStoreURI);
            myWebRequestParam.ContentType = "application/xml";
            myWebRequestParam.Headers.Add("X-SAP-LogonToken", rwsLogonToken);
            myWebRequestParam.Timeout = webReqTimeout;
            myWebRequestParam.Method = "GET";
            WebResponse myWebResponseParam = myWebRequestParam.GetResponse();
            StreamReader srParam = new StreamReader(myWebResponseParam.GetResponseStream());
            string outputParam = srParam.ReadToEnd();
            XmlDocument docParam = new XmlDocument();
            docParam.LoadXml(outputParam);

            //Returns an XmlNodeList containing a list of all descendant elements  
            string paramId = string.Empty;
            XmlNodeList paramNodelist = docParam.GetElementsByTagName("parameter");
            //  Add the parameters with values to the attribute nodes of the document
            foreach (XmlNode paramNode in paramNodelist)
            {
                foreach (XmlNode childNode1 in paramNode.ChildNodes)
                {
                    if (childNode1.Name == "id")
                    {
                        paramId = childNode1.InnerText;
                    }
                    else if (childNode1.Name == "answer")
                    {
                        foreach (XmlNode childNode2 in childNode1.ChildNodes)
                        {
                            if (childNode2.Name == "values")
                            {
                                childNode2.RemoveAll();
                                if (paramId == "0")
                                {
                                    XmlElement elem = docParam.CreateElement("value");
                                    elem.InnerText = royaltorID;
                                    paramNode.ChildNodes[3].ChildNodes[1].AppendChild(elem);
                                }
                                else if (paramId == "1")
                                {
                                    XmlElement elem = docParam.CreateElement("value");
                                    elem.InnerText = stmtPeriodID;
                                    paramNode.ChildNodes[3].ChildNodes[1].AppendChild(elem);
                                }
                                else if (paramId == "2")
                                {
                                    XmlElement elem = docParam.CreateElement("value");
                                    elem.InnerText = courtesyShareId;
                                    paramNode.ChildNodes[3].ChildNodes[1].AppendChild(elem);
                                }

                            }

                        }
                    }

                }

            }

            //Making PUT request back 
            HttpWebRequest myWebRequest1 = (HttpWebRequest)WebRequest.Create(InfoStoreURI);
            myWebRequest1.ContentType = "application/xml";
            myWebRequest1.Headers.Add("X-SAP-LogonToken", rwsLogonToken);
            myWebRequest1.Timeout = webReqTimeout;
            myWebRequest1.Method = "PUT";
            byte[] reqBodyBytes = System.Text.Encoding.Default.GetBytes(docParam.OuterXml);
            Stream reqStream = myWebRequest1.GetRequestStream();
            reqStream.Write(reqBodyBytes, 0, reqBodyBytes.Length);
            reqStream.Close();
            using (var myWebResponse1 = myWebRequest1.GetResponse()) { }

        }

        private void RefreshReport(string reportID)
        {
            string InfoStoreURI = baseURL + "raylight/v1/documents/" + reportID + "/parameters?refresh=true";
            HttpWebRequest myWebRequestParam = (HttpWebRequest)WebRequest.Create(InfoStoreURI);
            myWebRequestParam.ContentType = "application/xml";
            myWebRequestParam.Headers.Add("X-SAP-LogonToken", rwsLogonToken);
            myWebRequestParam.Timeout = webReqTimeout;
            myWebRequestParam.Method = "PUT";
            using (var myWebResponseParam = myWebRequestParam.GetResponse()) { }

        }

        /// <summary>
        /// WOS-172  - generate only concerned tab of BO summary report
        ///            Note : Summary statement is working with report tab number 2 and invoice tab with 5
        /// WUIN-382 - The names used for the Statement files need to match what the ART Portal is expecting
        ///            Non primary payee file name change: remove CS – same format as Primary with the different int_party_id
        /// </summary>              
        private void SaveReport(string stmtType, string reportID, string rptTabId, string royaltorID, string stmtPeriodID, string summaryMasterRoyaltor, string intPartyId,
                                string startOfPeriod, string endOfPeriod)
        {
            //string InfoStoreURI = baseURL + "raylight/v1/documents/" + reportID + "/reports/" + summRptTabId + "/pages";            
            string InfoStoreURI = baseURL + "raylight/v1/documents/" + reportID;
            string PDFFolderLocation = @AppDomain.CurrentDomain.BaseDirectory + @"\PDF_Files\" + Session["WARSAffiliate"].ToString() + @"\";

            //Statement report file name
            string rptFileName = string.Empty;
            if (stmtType == StmtType.SummaryStmtPrimaryPayee.ToString())
            {
                InfoStoreURI = InfoStoreURI + "/reports/" + rptTabId + "/pages";
                rptFileName = summaryMasterRoyaltor + "_" + startOfPeriod + "_" + endOfPeriod + "_" + intPartyId + "_SUM" + ".pdf";
            }
            else if (stmtType == StmtType.SummaryStmtNonPrimaryPayee.ToString())
            {
                InfoStoreURI = InfoStoreURI + "/reports/" + rptTabId + "/pages";
                rptFileName = summaryMasterRoyaltor + "_" + startOfPeriod + "_" + endOfPeriod + "_" + intPartyId + "_SUM" + ".pdf";
            }
            else if (stmtType == StmtType.InvoiceStmtPrimaryPayee.ToString())
            {
                InfoStoreURI = InfoStoreURI + "/reports/" + rptTabId + "/pages";
                //rptFileName = summaryMasterRoyaltor + "_" + startOfPeriod + "_" + endOfPeriod + "_" + intPartyId + "_INV" + ".xlsx";
                rptFileName = summaryMasterRoyaltor + "_" + startOfPeriod + "_" + endOfPeriod + "_" + intPartyId + "_INV" + ".pdf";
            }
            else if (stmtType == StmtType.InvoiceStmtNonPrimaryPayee.ToString())
            {
                InfoStoreURI = InfoStoreURI + "/reports/" + rptTabId + "/pages";
                //rptFileName = summaryMasterRoyaltor + "_" + startOfPeriod + "_" + endOfPeriod + "_" + intPartyId + "_INV" + ".xlsx";
                rptFileName = summaryMasterRoyaltor + "_" + startOfPeriod + "_" + endOfPeriod + "_" + intPartyId + "_INV" + ".pdf";
            }

            try
            {

                try
                {
                    //Making GET request to /infostore to retrieve the contents of top level of BI Platform repository.
                    HttpWebRequest myWebRequest2 = (HttpWebRequest)WebRequest.Create(InfoStoreURI);
                    /* WUIN-1259
                    if (stmtType == StmtType.InvoiceStmtPrimaryPayee.ToString() || stmtType == StmtType.InvoiceStmtNonPrimaryPayee.ToString())
                    {
                        myWebRequest2.Accept = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; // Excel 2007 and above
                    }
                    else
                    {
                        myWebRequest2.Accept = "application/pdf";
                    }
                     * */
                    myWebRequest2.Accept = "application/pdf";
                    myWebRequest2.Headers.Add("X-SAP-LogonToken", rwsLogonToken);
                    myWebRequest2.Method = "GET";
                    myWebRequest2.Timeout = webReqTimeout;
                    using (var myWebResponse2 = myWebRequest2.GetResponse())
                    {
                        string stmtDirecLoginDomain = ConfigurationManager.AppSettings["WARSServiceDomain"];
                        string stmtDirecLoginUser = ConfigurationManager.AppSettings["WARSServiceUser"];
                        string stmtDirecLoginPwd = ConfigurationManager.AppSettings["WARSServicePwd"];

                        FileStream stream;
                        if (stmtSaveLocation == "SharePoint")
                        {
                            stream = new FileStream(PDFFolderLocation + rptFileName, FileMode.Create);
                            myWebResponse2.GetResponseStream().CopyTo(stream);
                            stream.Close();
                        }
                        else
                        {
                            using (new Impersonator(stmtDirecLoginUser, stmtDirecLoginDomain, stmtDirecLoginPwd))
                            {
                                stream = new FileStream(stmtSaveLocation + rptFileName, FileMode.Create);
                                myWebResponse2.GetResponseStream().CopyTo(stream);
                                stream.Close();

                            }
                        }

                    }

                    if (stmtSaveLocation == "SharePoint")
                    {
                        //upload file to SharePoint
                        SharePointFileUpload(PDFFolderLocation, rptFileName);

                        //Delete file from local PDF folder once it is uploaded to SharePoint
                        System.IO.File.Delete(PDFFolderLocation + rptFileName);

                    }


                }
                catch (WebException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            catch (WebException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion Recalculate Front Sheet

        #region SharePoint functionality

        private bool CreateSharePointContext()
        {
            try
            {
                //Create a clientcontext using certificate
                SharePointContext = new ClientContext(sharePointSite);
                ServicePointManager.ServerCertificateValidationCallback = delegate(object sender1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                {
                    bool validationResult = true;
                    return validationResult;
                };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(CustomXertificateValidation);
                SharePointContext.ExecutingWebRequest += new EventHandler<WebRequestEventArgs>(ContextExecutingWebRequest);

                //login SharePoint                        
                var securePwd = new SecureString();
                foreach (char c in sharePointAccountPwd)
                {
                    securePwd.AppendChar(c);
                }
                SharePointContext.Credentials = new SharePointOnlineCredentials(sharePointAccount, securePwd);

                SharePointClientWeb = SharePointContext.Web;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }


        }

        private bool GetSharePointPDFFolder()
        {
            try
            {
                sharePointPDFFolderAffiliate = sharePointPDFFolderAffiliate + Session["SharePointFolderAffiliateCode"].ToString();
                SharePointList = SharePointClientWeb.Lists.GetByTitle(sharePointDocumentLibrary);
                var folderItems = SharePointList.GetItems(CamlQuery.CreateAllFoldersQuery());
                SharePointContext.Load(folderItems, icol => icol.Include(i => i.Folder));
                SharePointContext.ExecuteQuery();
                var allFolders = folderItems.Select(i => i.Folder).ToList();
                foreach (var folder in allFolders)
                {
                    if (folder.Name == sharePointPDFFolderAffiliate)
                    {
                        SharePointUploadFolder = folder;
                        break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;

            }


        }

        private void SharePointFileUpload(string filePath, string fileName)
        {
            try
            {
                FileCreationInformation newFile = new FileCreationInformation();
                newFile.Content = System.IO.File.ReadAllBytes(filePath + fileName);
                newFile.Url = @fileName;//file url is name of the file
                newFile.Overwrite = true;//to overwrite/update an existing file

                //add file to that
                Microsoft.SharePoint.Client.File uploadFile = SharePointUploadFolder.Files.Add(newFile);
                SharePointContext.Load(SharePointList);
                SharePointContext.Load(uploadFile);
                SharePointContext.ExecuteQuery();


            }
            catch (Exception ex)
            {

            }
        }

        public void ContextExecutingWebRequest(object sender, WebRequestEventArgs e)
        {
            HttpWebRequest webReq = e.WebRequestExecutor.WebRequest;
            X509Certificate cert = X509Certificate.CreateFromCertFile(@AppDomain.CurrentDomain.BaseDirectory + @"\bin\" + "WARSService_Sharepoint_Cert.cer");
            webReq.ClientCertificates.Add(cert);
        }

        private bool CustomXertificateValidation(object sender, X509Certificate cert, X509Chain chain, System.Net.Security.SslPolicyErrors error)
        {
            return true;
        }

        #endregion SharePoint functionality

    }
}



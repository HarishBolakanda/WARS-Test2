/*
File Name   :   StatementRun.cs
Purpose     :   To trigger the initial run of the statement period and will also allow it to be 
                archived when processing is complete.

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     05-Feb-2016     Harish(Infosys Limited)   Initial Creation
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

namespace WARS
{
    public partial class StatementProcess : System.Web.UI.Page
    {
        #region Global Declarations        
        Utilities util;        
        string userRole;
        Int32 errorId;        
        DataTable dtEmpty;
        StatementRunBL statementRunBL;
        #endregion Global Declarations

        #region Sorting
        string ascending = "Asc";
        string descending = "Desc";
        string sort_Up = "~/Images/Sort_up.png";
        string sort_Down = "~/Images/Sort_down.png";
        #endregion Sorting

        #region PAGE EVENTS

        protected void Page_Load(object sender, EventArgs e)
        {            
               
            try
            {
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Statement Run";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Statement Run";
                }
               
                btnProcessStmts.Enabled = true;
                if (Session["UserRole"] != null)
                {
                    userRole = Session["UserRole"].ToString();
                }

                if (IsPostBack && tdGrid.Visible == true)
                {
                    //set gridview panel height                    
                    PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                }

                lblTab.Focus();//tabbing sequence starts here                
                if (!IsPostBack)
                {                    
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        userRole = Session["UserRole"].ToString();
                        //Validation: only Super user can access this page
                        if (userRole.ToLower() == UserRole.SuperUser.ToString().ToLower())
                        {
                            tdGrid.Visible = false;
                            btnRefresh.Visible = false;
                            btnArchive.Visible = false;
                            btnProcessStmts.Visible = false;
                            btnReprint.Visible = false; 
                            LoadInitialGridData();                            
                            
                        }
                        else
                        {                            
                            ExceptionHandler(util.OnlySuperUserExpMessage.ToString(), string.Empty);
                        }
                    }
                    else
                    {                        
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }

                    util = null;
                }

            }
            catch (ThreadAbortException ex1)
            {
                // do nothing
            }
            catch (Exception ex)
            {                
                ExceptionHandler("Error in loading Statement Process screen.", ex.Message);
            }
            
        }

        protected void txtStmtEndPeriod_TextChanged(object sender, EventArgs e)
        {
            try
            {
                tdGrid.Visible = true;

                btnRefresh.Visible = true;
                btnArchive.Visible = true;
                btnProcessStmts.Visible = true;
                btnReprint.Visible = true;

                LoadGridData();
                if (gvStmtRun.Rows.Count > 0)
                {
                    //set gridview panel height                    
                    PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading data for the selected period.", ex.Message);             
            }
            
        }

        protected void btnProcessStmts_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox cbRunStmt;
                CheckBox cbRerunStmt;
                CheckBox cbArchiveStmt;
                CheckBox cbReprintDetail;
                CheckBox cbReprintSummary;
                bool cbCheckedValidation = false;
                bool valRerunArchive = true;

                //validations
                //1.Atleast one checkbox to be selected
                //2.user to be allowed to select only either Rerun or Archive but not both at a time for a statement period
                foreach (GridViewRow row in gvStmtRun.Rows)
                {
                    cbRunStmt = (CheckBox)row.FindControl("cbRunStmt");
                    cbRerunStmt = (CheckBox)row.FindControl("cbRerunStmt");
                    cbArchiveStmt = (CheckBox)row.FindControl("cbArchiveStmt");
                    cbReprintDetail = (CheckBox)row.FindControl("cbReprintDetail");
                    cbReprintSummary = (CheckBox)row.FindControl("cbReprintSummary");

                    if (cbRunStmt.Checked == true || cbRerunStmt.Checked == true )//|| cbArchiveStmt.Checked == true )
                    {
                        cbCheckedValidation = true;
                        //if (cbArchiveStmt.Checked == true && cbRerunStmt.Checked == true)
                        if ((cbRerunStmt.Checked && (cbArchiveStmt.Checked || cbReprintDetail.Checked || cbReprintSummary.Checked)) ||
                            (cbArchiveStmt.Checked && (cbRerunStmt.Checked || cbReprintDetail.Checked || cbReprintSummary.Checked)))
                        {
                            valRerunArchive = false;
                            break;
                        }
                        
                    }

                }

                if (cbCheckedValidation == true && valRerunArchive == true)
                {
                    //pop up message for confirmation to process the statements                    
                    mpeConfirm.Show();
                    lblConfirmMsg.Text = "Do you want to process the selected statements?";
                }
                else if (cbCheckedValidation != true)
                {
                    //pop up message 
                    msgView.SetMessage("Please select a statement period to process.", MessageType.Warning, PositionType.Auto);
                }
                else if (valRerunArchive != true)
                {
                    //pop up message 
                    msgView.SetMessage("Please select only either run or rerun for a statement period.", MessageType.Warning, PositionType.Auto);
                }

            }
            catch (ThreadAbortException ex1)
            {
                // do nothing
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in processing the statements.", ex.Message);
            }

        }

        protected void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                mpeConfirm.Hide();
                List<int> stmtsToRun = new List<int>();
                List<int> stmtsToRerun = new List<int>();
                List<int> stmtsToArchive = new List<int>();
                List<int> stmtsToReprintDetail = new List<int>();
                List<int> stmtsToReprintSummary = new List<int>();
                CheckBox cbRunStmt;
                CheckBox cbRerunStmt;
                //CheckBox cbArchiveStmt;
                string stmtPeriodID;

                //loop thorugh grid and get selected list of statement ids to be run and archived
                foreach (GridViewRow row in gvStmtRun.Rows)
                {
                    cbRunStmt = (CheckBox)row.FindControl("cbRunStmt");
                    cbRerunStmt = (CheckBox)row.FindControl("cbRerunStmt");
                    //cbArchiveStmt = (CheckBox)row.FindControl("cbArchiveStmt");
                    stmtPeriodID = (row.FindControl("hdnStmtPeriodID") as Label).Text;

                    if (cbRunStmt.Checked == true)
                        stmtsToRun.Add(Convert.ToInt32(stmtPeriodID));

                    if (cbRerunStmt.Checked == true)
                        stmtsToRerun.Add(Convert.ToInt32(stmtPeriodID));

                    //if (cbArchiveStmt.Checked == true)
                    //    stmtsToArchive.Add(Convert.ToInt32(stmtPeriodID));

                }
                statementRunBL = new StatementRunBL();
                string loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                DataSet statementRunData = statementRunBL.UpdatestmtRunData(stmtsToRun.ToArray(), stmtsToRerun.ToArray(), stmtsToArchive.ToArray(), stmtsToReprintDetail.ToArray(),
                   stmtsToReprintSummary.ToArray(), txtStmtEndPeriod.Text, loggedUserID, out errorId);
                statementRunBL = null;

                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                if (statementRunData.Tables.Count != 0 && errorId != 2)
                {
                    gvStmtRun.DataSource = statementRunData.Tables[0];
                    if (statementRunData.Tables[0].Rows.Count == 0)
                    {
                        gvStmtRun.EmptyDataText = "No data found for the selected period.";
                    }
                    gvStmtRun.DataBind();
                   
                }
                else if (statementRunData.Tables.Count == 0 && errorId != 2)
                {
                    dtEmpty = new DataTable();
                    gvStmtRun.DataSource = dtEmpty;
                    gvStmtRun.EmptyDataText = "No data found for the selected period.";
                    gvStmtRun.DataBind();

                }
                else
                {
                    ExceptionHandler("Error in updating data.", string.Empty);
                }

                //WOS-408 - Message to confirm new Statement run set up
                //pop up message 
                msgView.SetMessage("Statement run has been set up and will be processed in the next run of the Royalty Engine.", MessageType.Warning, PositionType.Auto);
                
            }
            catch (ThreadAbortException ex1)
            {
                // do nothing
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in updating data.", ex.Message);
            }
            finally
            {
                mpeConfirm.Hide();
            }

        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadGridData();
        }

        protected void btnArchive_Click(object sender, EventArgs e)
        {

            try
            {
                List<int> stmtsToRun = new List<int>();
                List<int> stmtsToRerun = new List<int>();
                List<int> stmtsToArchive = new List<int>();
                List<int> stmtsToReprintDetail = new List<int>();
                List<int> stmtsToReprintSummary = new List<int>();
                CheckBox cbRerunStmt;
                CheckBox cbArchiveStmt;
                CheckBox cbReprintDetail;
                CheckBox cbReprintSummary;
                string stmtPeriodID;
                bool valRerunArchive = true;

                //loop thorugh grid and get selected list of statement ids to be run and archived
                foreach (GridViewRow row in gvStmtRun.Rows)
                {
                    cbRerunStmt = (CheckBox)row.FindControl("cbRerunStmt");
                    cbArchiveStmt = (CheckBox)row.FindControl("cbArchiveStmt");
                    cbReprintDetail = (CheckBox)row.FindControl("cbReprintDetail");
                    cbReprintSummary = (CheckBox)row.FindControl("cbReprintSummary");
                    stmtPeriodID = (row.FindControl("hdnStmtPeriodID") as Label).Text;

                    if (cbArchiveStmt.Checked && (cbRerunStmt.Checked || cbReprintDetail.Checked || cbReprintSummary.Checked))
                    {
                        valRerunArchive = false;
                        break;
                    }

                    if (cbArchiveStmt.Checked == true)
                        stmtsToArchive.Add(Convert.ToInt32(stmtPeriodID));

                }

                if (valRerunArchive != true)
                {
                    //pop up message 
                    msgView.SetMessage("Please select only archive for a statement period.", MessageType.Warning, PositionType.Auto);
                }
                else
                {
                    if (stmtsToArchive.Count > 0)
                    {
                        statementRunBL = new StatementRunBL();
                        string loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                        DataSet statementRunData = statementRunBL.UpdatestmtRunData(stmtsToRun.ToArray(), stmtsToRerun.ToArray(), stmtsToArchive.ToArray(),
                            stmtsToReprintDetail.ToArray(), stmtsToReprintSummary.ToArray(), txtStmtEndPeriod.Text, loggedUserID, out errorId);
                        statementRunBL = null;

                        //WUIN-746 clearing sort hidden files
                        hdnSortExpression.Value = string.Empty;
                        hdnSortDirection.Value = string.Empty;

                        if (statementRunData.Tables.Count != 0 && errorId != 2)
                        {
                            gvStmtRun.DataSource = statementRunData.Tables[0];
                            if (statementRunData.Tables[0].Rows.Count == 0)
                            {
                                gvStmtRun.EmptyDataText = "No data found for the selected period.";
                            }
                            gvStmtRun.DataBind();


                        }
                        else if (statementRunData.Tables.Count == 0 && errorId != 2)
                        {
                            dtEmpty = new DataTable();
                            gvStmtRun.DataSource = dtEmpty;
                            gvStmtRun.EmptyDataText = "No data found for the selected period.";
                            gvStmtRun.DataBind();

                        }
                        else
                        {
                            ExceptionHandler("Error in updating data.", string.Empty);
                        }

                        /* WOS-384 - changes - Harish - 10-11-16
                        try
                        {
                            string message;
                            CommonBL commonBl = new CommonBL();
                            commonBl.RunStatementArchive(out message, out errorId);
                            commonBl = null;
                            if (errorId == 0)
                            {
                                msgView.SetMessage(message, MessageType.Warning, PositionType.Auto);
                            }
                            else if (errorId == 2)
                            {
                                msgView.SetMessage("Error in triggering statement archive.", MessageType.Warning, PositionType.Auto);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler("Error in triggering statement archive.", ex.Message);
                        }
                         * */
                        //if (errorId == 1)
                        //{
                        //    msgView.SetMessage("One or more Royaltor statements cannot be archived, Contract is held. Archive has been requested for the rest. The process should be triggered from the Workflow when the status of Royaltors has been checked.",
                        //                        MessageType.Warning, PositionType.Auto);
                        //}
                        //else
                        //{
                        //    msgView.SetMessage("Archive has been requested. The process should be triggered from the Workflow when the status of Royaltors has been checked.",
                        //                            MessageType.Warning, PositionType.Auto);
                        //}
                        //JIRA-923 Changes done by Ravi on 14-12-2018 -- Start
                        CommonBL commonBl = new CommonBL();
                        commonBl.RunStatementArchive(out errorId);
                        commonBl = null;
                        //if (errorId == 1)
                        //{
                        //     msgView.SetMessage("There are outstanding payments to be generated for the statements requested to be archived. Statements cannot be archived until those payments are generated.",
                        //                        MessageType.Warning, PositionType.Auto);
                        //}
                        if (errorId == 3 || errorId == 4)
                        {
                            if (errorId == 3)
                            {
                                msgView.SetMessage("Archive has already been scheduled. No further action is required.", MessageType.Warning, PositionType.Auto);
                            }
                            else if (errorId == 4)
                            {
                                msgView.SetMessage("The Archive cannot be scheduled as the process is not enabled. Please contact WARS Support.", MessageType.Warning, PositionType.Auto);
                            }
                        }
                        else if (errorId == 2)
                        {
                            msgView.SetMessage("Error in triggering statement archive.", MessageType.Warning, PositionType.Auto);
                        }
                        else
                        {
                            msgView.SetMessage("The Archive is scheduled and will run as a background job until complete.", MessageType.Warning, PositionType.Auto);
                        }
                        //JIRA-923 Changes done by Ravi on 14-12-2018 -- End
                        //================== End
                    }
                    else
                    {
                        /* WOS-384 - changes - Harish - 10-11-16
                        mpeRunArchive.Show();
                        lblConfirmMessage.Text = "Archive option is not selected for any of the statement periods. Do you want to trigger statement archive?";
                         * */
                        msgView.SetMessage("Please select Archive option for the statement periods.", MessageType.Warning, PositionType.Auto);
                        //================== End
                    }
                }
            }
            catch (ThreadAbortException ex1)
            {
                // do nothing
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in updating data.", ex.Message);
            }

        }

        protected void btnReprint_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox cbRunStmt;
                CheckBox cbRerunStmt;
                CheckBox cbArchiveStmt;
                CheckBox cbReprintDetail;
                CheckBox cbReprintSummary;
                bool cbCheckedValidation = false;
                bool valReprint = true;

                //validations
                //1.Atleast one checkbox to be selected
                //2.user to be allowed to select only either Rerun or Reprint detail or Repring Summary or Archive but not more than one at a time for a statement period
                foreach (GridViewRow row in gvStmtRun.Rows)
                {
                    cbRunStmt = (CheckBox)row.FindControl("cbRunStmt");
                    cbRerunStmt = (CheckBox)row.FindControl("cbRerunStmt");
                    cbArchiveStmt = (CheckBox)row.FindControl("cbArchiveStmt");
                    cbReprintDetail = (CheckBox)row.FindControl("cbReprintDetail");
                    cbReprintSummary = (CheckBox)row.FindControl("cbReprintSummary");

                    if (cbReprintDetail.Checked || cbReprintSummary.Checked)
                    {
                        cbCheckedValidation = true;
                        if (((cbReprintDetail.Checked || cbReprintSummary.Checked) && (cbArchiveStmt.Checked || cbRerunStmt.Checked)) ||
                            (cbReprintDetail.Checked && cbReprintSummary.Checked))
                        {
                            valReprint = false;
                            break;
                        }

                    }

                }

                if (cbCheckedValidation == true && valReprint == true)
                {
                    //pop up message for confirmation to reprint the statements                    
                    mpeConfirmReprint.Show();

                }
                else if (cbCheckedValidation != true)
                {
                    //pop up message 
                    msgView.SetMessage("Please select a statement period to reprint.", MessageType.Warning, PositionType.Auto);
                }
                else if (valReprint != true)
                {
                    //pop up message 
                    msgView.SetMessage("Please select only either reprint detail or reprint summary for a statement period.", MessageType.Warning, PositionType.Auto);
                }

            }
            catch (ThreadAbortException ex1)
            {
                // do nothing
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in reprinting the statements.", ex.Message);
            }
        }

        protected void btnYesConfirmReprint_Click(object sender, EventArgs e)
        {
            try
            {
                mpeConfirmReprint.Hide();
                List<int> stmtsToReprintDetail = new List<int>();
                List<int> stmtsToReprintSummary = new List<int>();
                List<int> stmtsToRun = new List<int>();
                List<int> stmtsToRerun = new List<int>();
                List<int> stmtsToArchive = new List<int>();

                CheckBox cbReprintDetail;
                CheckBox cbReprintSummary;
                string stmtPeriodID;

                //loop thorugh grid and get selected list of statement ids to be run and archived
                foreach (GridViewRow row in gvStmtRun.Rows)
                {
                    cbReprintDetail = (CheckBox)row.FindControl("cbReprintDetail");
                    cbReprintSummary = (CheckBox)row.FindControl("cbReprintSummary");
                    stmtPeriodID = (row.FindControl("hdnStmtPeriodID") as Label).Text;

                    if (cbReprintDetail.Checked == true)
                        stmtsToReprintDetail.Add(Convert.ToInt32(stmtPeriodID));

                    if (cbReprintSummary.Checked == true)
                        stmtsToReprintSummary.Add(Convert.ToInt32(stmtPeriodID));

                }
                statementRunBL = new StatementRunBL();
                string loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                DataSet statementRunData = statementRunBL.UpdatestmtRunData(stmtsToRun.ToArray(), stmtsToRerun.ToArray(), stmtsToArchive.ToArray(), stmtsToReprintDetail.ToArray(),
                   stmtsToReprintSummary.ToArray(), txtStmtEndPeriod.Text, loggedUserID, out errorId);
                statementRunBL = null;

                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                if (statementRunData.Tables.Count != 0 && errorId != 2)
                {
                    gvStmtRun.DataSource = statementRunData.Tables[0];
                    if (statementRunData.Tables[0].Rows.Count == 0)
                    {
                        gvStmtRun.EmptyDataText = "No data found for the selected period.";
                    }
                    gvStmtRun.DataBind();

                }
                else if (statementRunData.Tables.Count == 0 && errorId != 2)
                {
                    dtEmpty = new DataTable();
                    gvStmtRun.DataSource = dtEmpty;
                    gvStmtRun.EmptyDataText = "No data found for the selected period.";
                    gvStmtRun.DataBind();

                }
                else
                {
                    ExceptionHandler("Error in updating reprint data.", string.Empty);
                }

                msgView.SetMessage("Statement reprint has been set up and will be generated in the next run of the statement generation.", MessageType.Warning, PositionType.Auto);

            }
            catch (ThreadAbortException ex1)
            {
                // do nothing
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in updating reprint data.", ex.Message);
            }
            finally
            {
                mpeConfirmReprint.Hide();
            }
        }

        #endregion PAGE EVENTS

        #region GRIDVIEW EVENTS
        protected void gvStmtRun_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    foreach (TableCell tc in e.Row.Cells)
                    {
                        if (tc.HasControls())
                        {
                            LinkButton lnkHeader = (LinkButton)tc.Controls[0];
                            lnkHeader.Style.Add("color", "black");
                            lnkHeader.Style.Add("text-decoration", "none");

                            if (lnkHeader != null && hdnSortExpression.Value == lnkHeader.CommandArgument)
                            {
                                // initialize a new image
                                Image imgSort = new Image();
                                imgSort.ImageUrl = (hdnSortDirection.Value == ascending) ? sort_Up : sort_Down;
                                // adding a space and the image to the header link
                                tc.Controls.Add(new LiteralControl(" "));
                                tc.Controls.Add(imgSort);
                            }

                        }
                    }
                }
                //JIRA-746 Changes by Ravi on 05/03/2019 -- End
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    CheckBox cbRunStmt = (CheckBox)e.Row.FindControl("cbRunStmt");
                    CheckBox cbRerunStmt = (CheckBox)e.Row.FindControl("cbRerunStmt");
                    CheckBox cbArchiveStmt = (CheckBox)e.Row.FindControl("cbArchiveStmt");
                    CheckBox cbReprintDetail = (CheckBox)e.Row.FindControl("cbReprintDetail");
                    CheckBox cbReprintSummary = (CheckBox)e.Row.FindControl("cbReprintSummary");

                    string stmtPeriodID = (e.Row.FindControl("hdnStmtPeriodID") as Label).Text;
                    string runStmt = (e.Row.FindControl("hdnRunStmt") as Label).Text;
                    string reRunStmt = (e.Row.FindControl("hdnRerunStmt") as Label).Text;
                    string archiveStmt = (e.Row.FindControl("hdnArchiveStmt") as Label).Text;
                    string reprintDetail = (e.Row.FindControl("hdnReprintDetail") as Label).Text;
                    string reprintSummary = (e.Row.FindControl("hdnReprintSummary") as Label).Text;

                    //Enable/disable Run Statement checkbox
                    if (runStmt == "Y")
                        cbRunStmt.Enabled = true;
                    else
                        cbRunStmt.Enabled = false;

                    //Enable/disable Rerun Statement checkbox
                    if (reRunStmt == "Y")
                        cbRerunStmt.Enabled = true;
                    else
                        cbRerunStmt.Enabled = false;

                    //Enable/disable Archive Statement checkbox
                    if (archiveStmt == "Y")
                        cbArchiveStmt.Enabled = true;
                    else
                        cbArchiveStmt.Enabled = false;

                    //WOS-307 - changes
                    //reprint options are enabled with same scenarios as rerun
                    //Enable/disable Reprint Detail checkbox
                    if (reprintDetail == "Y")
                        cbReprintDetail.Enabled = true;
                    else
                        cbReprintDetail.Enabled = false;

                    //Enable/disable Reprint Summary checkbox
                    if (reprintSummary == "Y")
                        cbReprintSummary.Enabled = true;
                    else
                        cbReprintSummary.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding grid data.", ex.Message);
            }

        }

        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvStmtRun_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortingDirection = string.Empty;
            Utilities util = new Utilities();
            string sortDirec = util.SortingDirection(hdnSortDirection.Value);
            if (sortDirec == ascending)
            {
                sortingDirection = descending;
            }
            else
            {
                sortingDirection = ascending;
            }
            DataTable dataTable = (DataTable)Session["StmntProcessDatat"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvStmtRun.DataSource = dataView;
                gvStmtRun.DataBind();
            }
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        #endregion GRIDVIEW EVENTS

        #region METHODS
        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            util = new Utilities();
            util.GenericExceptionHandler(errorMsg + "<br />" + expMsg);
            util = null;
        }

        private void LoadInitialGridData()
        {
            dtEmpty = new DataTable();
            gvStmtRun.DataSource = dtEmpty;
            gvStmtRun.DataBind();
        }

        private void LoadGridData()
        {
            if (txtStmtEndPeriod.Text == "__/____")
            {
                LoadInitialGridData();
                return;
            }

            statementRunBL = new StatementRunBL();
            DataSet statementRunData = statementRunBL.GetStmtRunData(txtStmtEndPeriod.Text, out errorId);
            statementRunBL = null;            
            if (statementRunData.Tables.Count != 0 && errorId != 2)
            {
                Session["StmntProcessDatat"] =  statementRunData.Tables[0];
                gvStmtRun.DataSource = statementRunData.Tables[0];
                if (statementRunData.Tables[0].Rows.Count == 0)
                {
                    gvStmtRun.EmptyDataText = "No data found for the selected period.";
                    btnProcessStmts.Enabled = false;
                }
                gvStmtRun.DataBind();                
            }
            else if (statementRunData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvStmtRun.DataSource = dtEmpty;
                gvStmtRun.EmptyDataText = "No data found for the selected period.";
                btnProcessStmts.Enabled = false;
                gvStmtRun.DataBind();                
            }
            else
            {
                ExceptionHandler("Error in fetching grid data.", string.Empty);             
            }
        }
        #endregion METHODS

       
        /* WOS-384 - changes - Harish - 10-11-16
        protected void btnContinue_Click(object sender, EventArgs e)
        {
            try
            {
                string message;
                CommonBL commonBl = new CommonBL();
                commonBl.RunStatementArchive(out message, out errorId);
                commonBl = null;
                if (errorId == 0)
                {
                    msgView.SetMessage(message, MessageType.Warning, PositionType.Auto);
                }
                else if (errorId == 2)
                {
                    msgView.SetMessage("Error in triggering statement archive.", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in triggering statement archive.", ex.Message);
            }
        }
        */
    }
}
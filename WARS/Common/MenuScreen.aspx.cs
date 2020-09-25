using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WARS.BusinessLayer;

namespace WARS
{
    public partial class MenuScreen : System.Web.UI.Page
    {
        Utilities util;
        int errorId;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //user authentication
                util = new Utilities();
                if (!util.UserAuthentication())
                {
                    ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    util = null;
                    return;
                }
                util = null;


                lblTab.Focus();//tabbing sequence starts here
                Button lnkBtnHome = this.Master.FindControl("lnkBtnHome") as Button;
                lnkBtnHome.Visible = false;

                ImageButton btnWARSGlobal = this.Master.FindControl("btnWARSGlobal") as ImageButton;
                btnWARSGlobal.Visible = true;

                HtmlTableCell tdHomeBtn = this.Master.FindControl("tdHomeBtn") as HtmlTableCell;
                tdHomeBtn.Style.Add("vertical-align", "middle");

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Menu Options";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Menu Options";
                }

                ClearSessions();
                if (!IsPostBack)
                {
                    //WUIN-1055 - Bind Shared paths to Pdf Statement and File Upload Hyperlinks                    
                    if (Session["MenuScrnPDFStatementPath"] == null && Session["MenuScrnFileUploadPath"] == null)
                    {
                        LoadSharedPaths();                        
                    }
                    else
                    {
                        if (Convert.ToString(Session["MenuScrnPDFStatementPath"]) != string.Empty)
                        {
                            hdnPDFStatementAccess.Value = "Y";
                            hlPDFStatements.NavigateUrl = Convert.ToString(Session["MenuScrnPDFStatementPath"]);
                        }
                        else
                        {
                            hdnPDFStatementAccess.Value = "N";
                        }

                        if (Convert.ToString(Session["MenuScrnFileUploadPath"]) != string.Empty)
                        {
                            hdnFileUploadAccess.Value = "Y";
                            hlFileUpload.NavigateUrl = Convert.ToString(Session["MenuScrnFileUploadPath"]);
                        }
                        else
                        {
                            hdnFileUploadAccess.Value = "N";
                        }
                    }
                    //WUIN-1221-Enable Adhoc statement option based on registry settings.
                    if (Session["MenuScrnAdhocStmtOpt"] == null)
                    {
                        LoadAdhocStmtOpt();
                    }
                    else
                    {
                        if (Convert.ToString(Session["MenuScrnAdhocStmtOpt"]) == "Y")
                        {
                            trAdhocStmt.Visible = true;
                        }
                        else
                        {
                            trAdhocStmt.Visible = false;
                        }
                    }

                    //WUIN-1096 -  ReadOnlyUser
                    hdnUserRole.Value = Session["UserRole"].ToString();
                    if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
                    {
                        hdnIsReadyonlyUser.Value = "Y";
                    }
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

        /// <summary>
        /// Harish 19-12-2017: used to clear sessions when been to Home screen
        /// </summary>
        private void ClearSessions()
        {
            //Payment approval screen - to retain search fields when opened from payment details screen
            Session["PayApprSearchFilters"] = null;
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            util = new Utilities();
            util.GenericExceptionHandler(errorMsg + "<br />" + expMsg);
            util = null;
        }

        //WUIN-1055 - Bind Shared paths to Pdf Statement and File Upload Hyperlinks
        private void LoadSharedPaths()
        {
            string pdfStatementPath = string.Empty;
            string fileUploadPath = string.Empty;
            string stmtDirecLoginDomain = ConfigurationManager.AppSettings["WARSServiceDomain"];
            string stmtDirecLoginUser = ConfigurationManager.AppSettings["WARSServiceUser"];
            string stmtDirecLoginPwd = ConfigurationManager.AppSettings["WARSServicePwd"];

            CommonBL commonBl = new CommonBL();
            commonBl.GetSharedPaths(out pdfStatementPath, out fileUploadPath, out errorId);
            commonBl = null;

            using (new Impersonator(stmtDirecLoginUser, stmtDirecLoginDomain, stmtDirecLoginPwd))
            {
                if (pdfStatementPath != string.Empty && pdfStatementPath != "null")
                {
                    // Check if user has access to PDF Statement folders
                    try
                    {
                        // This will raise an exception if the path is read only or the user do not have access to view the permissions. 
                        System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(pdfStatementPath);
                        hdnPDFStatementAccess.Value = "Y";
                        hlPDFStatements.NavigateUrl = pdfStatementPath;

                        //hold the values in session so that this need not be fetched everytime when loading Menu screen
                        Session["MenuScrnPDFStatementPath"] = pdfStatementPath;
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        hdnPDFStatementAccess.Value = "N";
                        hlPDFStatements.NavigateUrl = string.Empty;
                        Session["MenuScrnPDFStatementPath"] = string.Empty;
                    }

                }
                else
                {
                    hdnPDFStatementAccess.Value = "N";
                    hlPDFStatements.NavigateUrl = string.Empty;
                    Session["MenuScrnPDFStatementPath"] = string.Empty;
                }

                if (fileUploadPath != string.Empty && fileUploadPath != "null")
                {
                    // Check if user has access to SIF folders                
                    try
                    {
                        // This will raise an exception if the path is read only or the user do not have access to view the permissions. 
                        System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(fileUploadPath);
                        hdnFileUploadAccess.Value = "Y";
                        hlFileUpload.NavigateUrl = fileUploadPath;

                        //hold the values in session so that this need not be fetched everytime when loading Menu screen            
                        Session["MenuScrnFileUploadPath"] = fileUploadPath;

                    }
                    catch (UnauthorizedAccessException e)
                    {
                        hdnFileUploadAccess.Value = "N";
                        hlFileUpload.NavigateUrl = string.Empty;
                        Session["MenuScrnFileUploadPath"] = string.Empty;
                    }

                }
                else
                {
                    hdnFileUploadAccess.Value = "N";
                    hlFileUpload.NavigateUrl = string.Empty;
                    Session["MenuScrnFileUploadPath"] = string.Empty;
                }
            }

        }

        private void LoadAdhocStmtOpt()
        {
            string adhocStatementOption = string.Empty;
            CommonBL commonBl = new CommonBL();
            commonBl.GetAdhocStatementOption(out adhocStatementOption,out errorId);
            commonBl = null;
            Session["MenuScrnAdhocStmtOpt"] = adhocStatementOption;
            if (adhocStatementOption == "Y")
            {
                trAdhocStmt.Visible = true;
            }
            else
            {
                trAdhocStmt.Visible = false;
            }
        }

        //JIRA-938 Changes done by Ravi on 28/01/2019 -- Start
        protected void btnRunStatements_Click(object sender, EventArgs e)
        {
            try
            {
                string message;
                CommonBL commonBl = new CommonBL();
                commonBl.RunRotaltyEngine("Full", out message, out errorId);
                commonBl = null;
                if (errorId == 0)
                {
                    msgView.SetMessage(message, MessageType.Warning, PositionType.Auto);
                }
                else if (errorId == 2)
                {
                    msgView.SetMessage("Error in running royalty engine", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in running royalty engine", ex.Message);
            }
        }
        //JIRA-938 Changes done by Ravi on 28/01/2019 -- End

        protected void btnAdHocFileLoad_Click(object sender, EventArgs e)
        {
            try
            {
                string message;
                CommonBL commonBl = new CommonBL();
                commonBl.LoadAdhocCostFile(out message, out errorId);
                commonBl = null;
                if (errorId == 0)
                {
                    msgView.SetMessage(message, MessageType.Warning, PositionType.Auto);
                }
                else if (errorId == 2)
                {
                    msgView.SetMessage("Error in loading Adhoc/Cost file", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading Adhoc/Cost file", ex.Message);
            }
        }
        protected void btnAccrual_Click(object sender, EventArgs e)
        {
            util = new Utilities();
            if (util.UserAuthentication())
            {
                string userRole = Session["UserRole"].ToString();

                //Validation: only Super user can access this page
                if (userRole.ToLower() == UserRole.SuperUser.ToString().ToLower())
                {
                    //JIRA- 1172- Changes by Harshika--Start
                    string runflag = "";
                    if (hdnConfirmation.Value == "RunAccruals")
                    {
                        runflag = "RUN";
                    }
                    else if (hdnConfirmation.Value == "ReRunAccruals")
                    {
                        runflag = "RERUN";
                    }
                    //JIRA- 1172- Changes by Harshika--End
                    string message;
                    CommonBL commonBl = new CommonBL();
                    commonBl.RunAccrualProcess(runflag, out message, out errorId);
                    commonBl = null;
                    if (errorId == 0)
                    {
                        msgView.SetMessage(message, MessageType.Warning, PositionType.Auto);
                    }
                    else if (errorId == 2)
                    {
                        msgView.SetMessage("Error in Accrual process", MessageType.Warning, PositionType.Auto);
                    }
                }
                else
                {
                    msgView.SetMessage("Accruals can only be run by a SuperUser", MessageType.Warning, PositionType.Auto);
                }
            }
        }

        protected void btnGeneratePayment_Click(object sender, EventArgs e)
        {
            CommonBL commonBl = new CommonBL();
            commonBl.GeneratePayments(out errorId);
            commonBl = null;
            if (errorId == 0)
            {
                msgView.SetMessage("Job to generate payments is scheduled. ", MessageType.Warning, PositionType.Auto);
            }
            else if (errorId == 2)
            {
                msgView.SetMessage("Error in triggering payment generation job.", MessageType.Warning, PositionType.Auto);
            }
        }

        protected void btnRunAutoConsolidate_Click(object sender, EventArgs e)
        {
            try
            {
                CommonBL commonBl = new CommonBL();
                commonBl.RunAutoConsolidate(out errorId);
                commonBl = null;

                if (errorId == 0)
                {
                    msgView.SetMessage("Auto Consolidate process is complete. Check email for any consolidation failures. Consolidated Products will require Manager Sign Off before statementing.",
                                        MessageType.Warning, PositionType.Auto);
                }
                else if (errorId == 2)
                {
                    ExceptionHandler("Error in running participant auto consolidate", string.Empty);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in running participant auto consolidate", ex.Message);
            }

        }

        //JIRA-908 CHanges --Start
        protected void btnYes_Click(object sender, EventArgs e)
        {
            if (hdnConfirmation.Value == "AdhocFile")
            {
                btnAdHocFileLoad_Click(sender, e);
            }
            if (hdnConfirmation.Value == "RunStmts")
            {
                btnRunStatements_Click(sender, e);
            }
            if (hdnConfirmation.Value == "RunAccruals" || hdnConfirmation.Value == "ReRunAccruals")
            {
                btnAccrual_Click(sender, e);
            }
            if (hdnConfirmation.Value == "GenPayments")
            {
                btnGeneratePayment_Click(sender, e);
            }
            if (hdnConfirmation.Value == "RunAutoConsolid")
            {
                btnRunAutoConsolidate_Click(sender, e);
            }
        }
        //JIRA-908 CHanges --End
    }
}
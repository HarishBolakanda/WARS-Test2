/*
File Name   :   RoyaltorGroupingAudit.cs
Purpose     :   To display Royaltor Grouping Audit data

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
    public partial class RoyaltorGroupingAudit : System.Web.UI.Page
    {
        #region Global Declarations
        RoyaltorGroupingAuditBL royGrpAuditBL;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Contract Grouping";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Contract Grouping";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlParticipantDetailsAudit.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    tdData.Style.Add("display", "none");
                    
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        
                        LoadData();

                        this.Master.FindControl("lnkBtnHome").Visible = false; //WUIN-599 Hiding master page home butt
                        //WUIN-599 -- Only one user can use contract screens at the same time.
                        if (Convert.ToString(Session["UserCode"]) != Convert.ToString(Session["ContractScreenLockedUser"]))
                        {
                            hdnOtherUserScreenLocked.Value = "Y";
                        }
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
        
        protected void btnRoyaltorDetails_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtRoyaltor.Text.Trim() != string.Empty && txtRoyaltor.Text.Trim().Contains("-"))
                {
                    hdnIsMaintScreen.Value = "Y";
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "MaintScreen", "RedirectToMaintScreen(" + txtRoyaltor.Text.Split('-')[0].ToString().Trim() + ");", true);
                   
                    //Response.Redirect("../Contract/RoyContractGrouping.aspx?RoyaltorID=" + txtRoyaltor.Text.Split('-')[0].ToString().Trim() + "&isNewRoyaltor=N", false);
                }
                else
                {
                    Response.Redirect("../Contract/RoyContractGrouping.aspx?RoyaltorID" + string.Empty + "&isNewRoyaltor=N", false);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting to participant maintenance screen.", ex.Message);
            }
        }

        #endregion Events

        #region Methods

        private void LoadData()
        {                        

            if ((Request.QueryString != null && Request.QueryString.Count > 0) || (txtRoyaltor.Text != string.Empty))
            {
                string royaltorId = Request.QueryString[0];

                //populate royaltor for the royaltor id
                if (Session["FuzzySearchAllRoyaltorList"] == null)
                {
                    DataSet dsList = new FuzzySearchBL().GetFuzzySearchList("GetAllRoyaltorList", out errorId);
                    Session["FuzzySearchAllRoyaltorList"] = dsList.Tables[0];
                }

                DataRow[] filteredRow = ((DataTable)Session["FuzzySearchAllRoyaltorList"]).Select("royaltor_id = '" + royaltorId + "'");
                txtRoyaltor.Text = filteredRow[0]["royaltor"].ToString();
                LoadAuditData();
            }
        }
        
        protected void btnHdnRoyaltor_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtRoyaltor.Text))
                {
                    LoadAuditData();
                }
                else
                {
                    tdData.Style.Add("display", "none");
                    
                    dtEmpty = new DataTable();
                    gvRoyaltorGrpAudit.DataSource = dtEmpty;
                    gvRoyaltorGrpAudit.DataBind();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in search.", ex.Message);
            }
        }
        
        private void LoadAuditData()
        {
            tdData.Style.Remove("display");
                        
            royGrpAuditBL = new RoyaltorGroupingAuditBL();
            DataSet auditData = royGrpAuditBL.GetRoyaltorData(txtRoyaltor.Text.Split('-')[0].ToString().Trim(), out errorId);
            royGrpAuditBL = null;

            if (auditData.Tables.Count != 0 && errorId != 2)
            {                
                BindGrid(auditData.Tables[0]);
            }

            else
            {
                dtEmpty = new DataTable();
                gvRoyaltorGrpAudit.DataSource = dtEmpty;
                gvRoyaltorGrpAudit.DataBind();
            }
        }        
    
        private void BindGrid(DataTable gridData)
        {
            if (gridData.Rows.Count > 0)
            {
                gvRoyaltorGrpAudit.DataSource = gridData;
                gvRoyaltorGrpAudit.DataBind();
                CompareAuditRows();
            }
            else
            {
                dtEmpty = new DataTable();
                gvRoyaltorGrpAudit.DataSource = dtEmpty;
                gvRoyaltorGrpAudit.DataBind();
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
                ExceptionHandler("Error in royaltor search.", ex.Message);
            }
        }
        
        private void CompareAuditRows()
        {
            for (int i = 0; i < gvRoyaltorGrpAudit.Rows.Count - 1; i++)
            {
                GridViewRow currentRow = gvRoyaltorGrpAudit.Rows[i];
                GridViewRow nextRow = gvRoyaltorGrpAudit.Rows[i + 1];

                

                    //Compare Royaltor
                    if ((currentRow.FindControl("lblRoyaltor") as Label).Text != (nextRow.FindControl("lblRoyaltor") as Label).Text)
                    {
                        if((currentRow.FindControl("lblRoyaltor") as Label).Text=="")
                        {
                        (nextRow.FindControl("lblRoyaltor") as Label).ForeColor = Color.Red;
                        }
                        else
                        {
                        (currentRow.FindControl("lblRoyaltor") as Label).ForeColor = Color.Red;
                        }
                    }

                    //Compare Summary
                    if ((currentRow.FindControl("lblSummary") as Label).Text != (nextRow.FindControl("lblSummary") as Label).Text)
                    {
                        if ((currentRow.FindControl("lblSummary") as Label).Text == "")
                        {
                            (nextRow.FindControl("lblSummary") as Label).ForeColor = Color.Red;
                        }
                        else
                        {
                            (currentRow.FindControl("lblSummary") as Label).ForeColor = Color.Red;
                        }
                    }

                    //Compare GFS Label
                    if ((currentRow.FindControl("lblGfsLabel") as Label).Text != (nextRow.FindControl("lblGfsLabel") as Label).Text)
                    {
                        if ((currentRow.FindControl("lblGfsLabel") as Label).Text == "")
                        {
                            (nextRow.FindControl("lblGfsLabel") as Label).ForeColor = Color.Red;
                        }
                        else
                        {
                            (currentRow.FindControl("lblGfsLabel") as Label).ForeColor = Color.Red;
                        }
                    }

                    //Compare lblGfsCompany
                    if ((currentRow.FindControl("lblGfsCompany") as Label).Text != (nextRow.FindControl("lblGfsCompany") as Label).Text)
                    {
                        if ((currentRow.FindControl("lblGfsCompany") as Label).Text == "")
                        {
                            (nextRow.FindControl("lblGfsCompany") as Label).ForeColor = Color.Red;
                        }
                        else
                        {
                            (currentRow.FindControl("lblGfsCompany") as Label).ForeColor = Color.Red;
                        }
                    }

                    //Compare TXTDetail
                    if ((currentRow.FindControl("lblTxtDetail") as Label).Text != (nextRow.FindControl("lblTxtDetail") as Label).Text)
                    {
                        if ((currentRow.FindControl("lblTxtDetail") as Label).Text == "")
                        {
                            (nextRow.FindControl("lblTxtDetail") as Label).ForeColor = Color.Red;
                        }
                        else
                        {
                            (currentRow.FindControl("lblTxtDetail") as Label).ForeColor = Color.Red;
                        }
                    }

                    //Compare DspAnalytics
                    if ((currentRow.FindControl("lblDspAnalytics") as Label).Text != (nextRow.FindControl("lblDspAnalytics") as Label).Text)
                    {
                        if ((currentRow.FindControl("lblDspAnalytics") as Label).Text == "")
                        {
                            (nextRow.FindControl("lblDspAnalytics") as Label).ForeColor = Color.Red;
                        }
                        else
                        {
                            (currentRow.FindControl("lblDspAnalytics") as Label).ForeColor = Color.Red;
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
                    txtRoyaltor.Text = string.Empty;
                    return;
                }

                txtRoyaltor.Text = lbFuzzySearch.SelectedValue.ToString();                         
                LoadAuditData();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor.", ex.Message);
            }
        }
        
        private void FuzzySearchRoyaltor()
        {
            if (txtRoyaltor.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in royaltor search field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllRoyaltorList(txtRoyaltor.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }
        
        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

        }

        #endregion Methods


        #region Web Methods

        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod(EnableSession = true)]
        public static int UpdateScreenLockFlag()
        {
            int errorId;
            RoyaltorContractBL royContractBL = new RoyaltorContractBL();
            royContractBL.UpdateScreenLockFlag(HttpContext.Current.Session["ScreenLockedRoyaltorId"].ToString(), "N", HttpContext.Current.Session["UserCode"].ToString(), out errorId);
            royContractBL = null;
            return errorId;

        }

        #endregion Web Methods
    }
}
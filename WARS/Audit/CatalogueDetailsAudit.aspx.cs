/*
File Name   :   RoyaltorPayeeAudit.cs
Purpose     :   To display Royaltor Payee  Audit data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     24-Sep-2017     Pratik(Infosys Limited)   Initial Creation
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
    public partial class CatalogueDetailsAudit : System.Web.UI.Page
    {
        #region Global Declarations
        CatalogueDetailsAuditBL catalogueDetailsAuditBL;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Catalogue Details Audit";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Catalogue Details Audit";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlCatalogueDetailsAudit.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    tdData.Style.Add("display", "none");
                    trRoyAudit.Style.Add("display", "none");

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

        protected void gvCatalogueDetailsAudit_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                Label lblUserCodeHdr;
                Label lblUpdatedOnHdr;
                Label lblTitle;
                Label lblStatus;
                Label lblCompilation;
                Label lblArtistName;
                Label lblConfigName;
                Label lblComplicated;
                Label lblMurOwner;
                Label lblProject;
                Label lblLicensedOut;
                Label lblTotalTracks;
                Label lblExceptionRate;
                Label lblPlayLength;
                Label lblTotalPlay;
                Label lblUnlistedComponents;
                Label lblMarketingOwner;
                Label lblWeaSalesLabel;
                Label lblFirstSaleDate;
                Label lblLegacy;
                Label lblLabel;


                string changeType;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    lblUserCodeHdr = (e.Row.FindControl("lblUserCodeHdr") as Label);
                    lblUpdatedOnHdr = (e.Row.FindControl("lblUpdatedOnHdr") as Label);

                    lblTitle = (e.Row.FindControl("lblTitle") as Label);
                    lblStatus = (e.Row.FindControl("lblStatus") as Label);
                    lblCompilation = (e.Row.FindControl("lblCompilation") as Label);
                    lblArtistName = (e.Row.FindControl("lblArtistName") as Label);
                    lblConfigName = (e.Row.FindControl("lblConfigName") as Label);

                    lblComplicated = (e.Row.FindControl("lblComplicated") as Label);
                    lblMurOwner = (e.Row.FindControl("lblMurOwner") as Label);
                    lblProject = (e.Row.FindControl("lblProject") as Label);
                    lblLicensedOut = (e.Row.FindControl("lblLicensedOut") as Label);


                    lblTotalTracks = (e.Row.FindControl("lblTotalTracks") as Label);
                    lblExceptionRate = (e.Row.FindControl("lblExceptionRate") as Label);
                    lblPlayLength = (e.Row.FindControl("lblPlayLength") as Label);
                    lblTotalPlay = (e.Row.FindControl("lblTotalPlay") as Label);

                    lblMarketingOwner = (e.Row.FindControl("lblMarketingOwner") as Label);
                    lblWeaSalesLabel = (e.Row.FindControl("lblWeaSalesLabel") as Label);
                    lblFirstSaleDate = (e.Row.FindControl("lblFirstSaleDate") as Label);
                    lblLegacy = (e.Row.FindControl("lblLegacy") as Label);
                    lblLabel = (e.Row.FindControl("lblLabel") as Label); 

                    lblUnlistedComponents = (e.Row.FindControl("lblUnlistedComponents") as Label);
                    changeType = (e.Row.FindControl("hdnChangeType") as HiddenField).Value;

                    //For deleted records cahnge updated by, updated on to deleted by and deleted on
                    //Change the color of details to red
                    if (changeType == "D")
                    {
                        lblUserCodeHdr.Text = "Deleted by";
                        lblUpdatedOnHdr.Text = "Deleted on";
                        lblTitle.ForeColor = Color.Red;
                        lblStatus.ForeColor = Color.Red;
                        lblCompilation.ForeColor = Color.Red;
                        lblArtistName.ForeColor = Color.Red;
                        lblConfigName.ForeColor = Color.Red;

                        lblComplicated.ForeColor = Color.Red;
                        lblMurOwner.ForeColor = Color.Red;
                        lblProject.ForeColor = Color.Red;
                        lblLicensedOut.ForeColor = Color.Red;

                        lblTotalTracks.ForeColor = Color.Red;
                        lblExceptionRate.ForeColor = Color.Red;
                        lblPlayLength.ForeColor = Color.Red;
                        lblTotalPlay.ForeColor = Color.Red;
                        lblUnlistedComponents.ForeColor = Color.Red;
                        lblMarketingOwner.ForeColor = Color.Red;
                        lblWeaSalesLabel.ForeColor = Color.Red;
                        lblFirstSaleDate.ForeColor = Color.Red;
                        lblLegacy.ForeColor = Color.Red;
                        lblLabel.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding data to grid.", ex.Message);
            }
        }

        protected void btnCatalogueDetails_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("../Participants/CatalogueMaintenance.aspx?catNo=" + txtCatalogueSearch.Text + "", false);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting to catalouge maintenance screen.", ex.Message);
            }
        }

        #endregion Events

        #region Methods

        private void LoadData()
        {
            if (Request.QueryString != null && Request.QueryString.Count > 0)
            {
                string catNo = Request.QueryString[0];
                txtCatalogueSearch.Text = catNo;
                CatalogueSelected();
            }
        }

        private void CatalogueSelected()
        {
            tdData.Style.Remove("display");
            trRoyAudit.Style.Remove("display");

            catalogueDetailsAuditBL = new CatalogueDetailsAuditBL();
            DataSet auditData = catalogueDetailsAuditBL.GetSearchedData(txtCatalogueSearch.Text, out errorId);
            catalogueDetailsAuditBL = null;

            if (auditData.Tables.Count != 0 && errorId != 2)
            {
                BindGrid(auditData.Tables[0]);
            }
            else
            {
                ExceptionHandler("Error in fetching catalogue details audit data", string.Empty);
            }
        }

        private void BindGrid(DataTable gridData)
        {
            if (gridData.Rows.Count > 0)
            {
                gvCatalogueDetailsAudit.DataSource = gridData;
                gvCatalogueDetailsAudit.DataBind();
                CompareAuditRows();
            }
            else
            {
                dtEmpty = new DataTable();
                gvCatalogueDetailsAudit.DataSource = dtEmpty;
                gvCatalogueDetailsAudit.DataBind();
            }
        }

        private void CompareAuditRows()
        {
            for (int i = 0; i < gvCatalogueDetailsAudit.Rows.Count - 1; i++)
            {
                GridViewRow currentRow = gvCatalogueDetailsAudit.Rows[i];
                GridViewRow nextRow = gvCatalogueDetailsAudit.Rows[i + 1];


                //Compare Title
                if ((currentRow.FindControl("lblTitle") as Label).Text != (nextRow.FindControl("lblTitle") as Label).Text)
                {
                    (currentRow.FindControl("lblTitle") as Label).ForeColor = Color.Red;
                }

                //Compare Status
                if ((currentRow.FindControl("lblStatus") as Label).Text != (nextRow.FindControl("lblStatus") as Label).Text)
                {
                    (currentRow.FindControl("lblStatus") as Label).ForeColor = Color.Red;
                }

                //Compare Compilation
                if ((currentRow.FindControl("lblCompilation") as Label).Text != (nextRow.FindControl("lblCompilation") as Label).Text)
                {
                    (currentRow.FindControl("lblCompilation") as Label).ForeColor = Color.Red;
                }

                //Compare ArtistName
                if ((currentRow.FindControl("lblArtistName") as Label).Text != (nextRow.FindControl("lblArtistName") as Label).Text)
                {
                    (currentRow.FindControl("lblArtistName") as Label).ForeColor = Color.Red;
                }

                //Compare ConfigName
                if ((currentRow.FindControl("lblConfigName") as Label).Text != (nextRow.FindControl("lblConfigName") as Label).Text)
                {
                    (currentRow.FindControl("lblConfigName") as Label).ForeColor = Color.Red;
                }

                //Compare Complicated
                if ((currentRow.FindControl("lblComplicated") as Label).Text != (nextRow.FindControl("lblComplicated") as Label).Text)
                {
                    (currentRow.FindControl("lblComplicated") as Label).ForeColor = Color.Red;
                }

                //Compare MurOwner
                if ((currentRow.FindControl("lblMurOwner") as Label).Text != (nextRow.FindControl("lblMurOwner") as Label).Text)
                {
                    (currentRow.FindControl("lblMurOwner") as Label).ForeColor = Color.Red;
                }

                //Compare Project
                if ((currentRow.FindControl("lblProject") as Label).Text != (nextRow.FindControl("lblProject") as Label).Text)
                {
                    (currentRow.FindControl("lblProject") as Label).ForeColor = Color.Red;
                }
                //Comapre LicensedOut
                if ((currentRow.FindControl("lblLicensedOut") as Label).Text != (nextRow.FindControl("lblLicensedOut") as Label).Text)
                {
                    (currentRow.FindControl("lblLicensedOut") as Label).ForeColor = Color.Red;
                }

                //Compare TotalTracks
                if ((currentRow.FindControl("lblTotalTracks") as Label).Text != (nextRow.FindControl("lblTotalTracks") as Label).Text)
                {
                    (currentRow.FindControl("lblTotalTracks") as Label).ForeColor = Color.Red;
                }

                //Compare ExceptionRate
                if ((currentRow.FindControl("lblExceptionRate") as Label).Text != (nextRow.FindControl("lblExceptionRate") as Label).Text)
                {
                    (currentRow.FindControl("lblExceptionRate") as Label).ForeColor = Color.Red;
                }

                //Compare PlayLength
                if ((currentRow.FindControl("lblPlayLength") as Label).Text != (nextRow.FindControl("lblPlayLength") as Label).Text)
                {
                    (currentRow.FindControl("lblPlayLength") as Label).ForeColor = Color.Red;
                }

                //Compare TotalPlay
                if ((currentRow.FindControl("lblTotalPlay") as Label).Text != (nextRow.FindControl("lblTotalPlay") as Label).Text)
                {
                    (currentRow.FindControl("lblTotalPlay") as Label).ForeColor = Color.Red;
                }

              

                //Compare UnlistedComponents
                if ((currentRow.FindControl("lblUnlistedComponents") as Label).Text != (nextRow.FindControl("lblUnlistedComponents") as Label).Text)
                {
                    (currentRow.FindControl("lblUnlistedComponents") as Label).ForeColor = Color.Red;
                }

                //Compare MarketingOwner
                if ((currentRow.FindControl("lblMarketingOwner") as Label).Text != (nextRow.FindControl("lblMarketingOwner") as Label).Text)
                {
                    (currentRow.FindControl("lblMarketingOwner") as Label).ForeColor = Color.Red;
                }
                //Compare WEASalesLabel
                if ((currentRow.FindControl("lblWeaSalesLabel") as Label).Text != (nextRow.FindControl("lblWeaSalesLabel") as Label).Text)
                {
                    (currentRow.FindControl("lblWeaSalesLabel") as Label).ForeColor = Color.Red;
                }
                //Compare FirstSaleDate
                if ((currentRow.FindControl("lblFirstSaleDate") as Label).Text != (nextRow.FindControl("lblFirstSaleDate") as Label).Text)
                {
                    (currentRow.FindControl("lblFirstSaleDate") as Label).ForeColor = Color.Red;
                }

                //Compare Legacy
                if ((currentRow.FindControl("lblLegacy") as Label).Text != (nextRow.FindControl("lblLegacy") as Label).Text)
                {
                    (currentRow.FindControl("lblLegacy") as Label).ForeColor = Color.Red;
                }

                //Compare Label
                if ((currentRow.FindControl("lblLabel") as Label).Text != (nextRow.FindControl("lblLabel") as Label).Text)
                {
                    (currentRow.FindControl("lblLabel") as Label).ForeColor = Color.Red;
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
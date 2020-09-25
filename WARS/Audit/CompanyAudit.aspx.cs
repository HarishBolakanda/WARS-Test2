/*
File Name   :   CompanyAudit.cs
Purpose     :   to display Company Audit data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     30-Mar-2017     Pratik(Infosys Limited)   Initial Creation
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

namespace WARS
{

    public partial class CompanyAudit : System.Web.UI.Page
    {
        #region Global Declarations
        CompanyAuditBL companyAuditBL;
        string loggedUserID;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Company Audit";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Company Audit";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlCompanyDetails.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    txtCompanySearch.Focus();
                    tdData.Style.Add("display", "none");

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

        protected void fuzzySearchCompany_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchCompany();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in company search.", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtCompanySearch.Text = string.Empty;
                    return;
                }

                txtCompanySearch.Text = lbFuzzySearch.SelectedValue.ToString();
                int companyCode = Convert.ToInt32(txtCompanySearch.Text.Split('-')[0].ToString().Trim());
                LoadCompanyAuditData(companyCode);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting company.", ex.Message);
            }
        }

        protected void btnHdnCompanySearch_Click(object sender, EventArgs e)
        {
            try
            {
                int companyCode = Convert.ToInt32(txtCompanySearch.Text.Split('-')[0].ToString().Trim());
                LoadCompanyAuditData(companyCode);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading company details.", ex.Message);
            }
        }

        protected void btnCompanyMaint_Click(object sender, EventArgs e)
        {
            if (hdnIsValidSearch.Value == "Y")
            {
                Response.Redirect("../DataMaintenance/CompanyMaintenance.aspx?companyData=" + txtCompanySearch.Text + "");
            }
            else
            {
                Response.Redirect("../DataMaintenance/CompanyMaintenance.aspx");
            }
        }

        #endregion Events

        #region Methods

        private void LoadData()
        {

            if (Request.QueryString != null && Request.QueryString.Count > 0)
            {
                string companyValue = Request.QueryString[0];
                int companyCode = Convert.ToInt32(companyValue.Split('-')[0].ToString().Trim());
                txtCompanySearch.Text = companyValue;
                LoadCompanyAuditData(companyCode);
            }
        }

        private void LoadCompanyAuditData(int companyCode)
        {
            hdnIsValidSearch.Value = "Y";
            companyAuditBL = new CompanyAuditBL();
            DataSet companyData = companyAuditBL.GetCompanyAuditData(companyCode, out errorId);
            companyAuditBL = null;

            if (companyData.Tables.Count != 0 && errorId != 2)
            {
                BindGrid(companyData.Tables[0]);
            }
            else
            {
                ExceptionHandler("Error in fetching companydata", string.Empty);
            }
        }

        private void BindGrid(DataTable gridData)
        {
            tdData.Style.Remove("display");

            if (gridData.Rows.Count > 0)
            {
                gvCompanyDetails.DataSource = gridData;
                gvCompanyDetails.DataBind();
                CompareRows();
            }
            else
            {
                dtEmpty = new DataTable();
                gvCompanyDetails.DataSource = dtEmpty;
                gvCompanyDetails.DataBind();
            }
        }

        private void CompareRows()
        {
            for (int i = 0; i < gvCompanyDetails.Rows.Count - 1; i++)
            {
                GridViewRow currentRow = gvCompanyDetails.Rows[i];
                GridViewRow nextRow = gvCompanyDetails.Rows[i + 1];

                //Comapre Company Name
                if ((currentRow.FindControl("lblCompanyName") as Label).Text != (nextRow.FindControl("lblCompanyName") as Label).Text)
                {
                    (currentRow.FindControl("lblCompanyName") as Label).ForeColor = Color.Red;
                }

                //Comapre Company Desc
                if ((currentRow.FindControl("lblCompanyDesc") as Label).Text != (nextRow.FindControl("lblCompanyDesc") as Label).Text)
                {
                    (currentRow.FindControl("lblCompanyDesc") as Label).ForeColor = Color.Red;
                }

                //Compare Company Address
                if ((currentRow.FindControl("lblCompanyAdd1") as Label).Text != (nextRow.FindControl("lblCompanyAdd1") as Label).Text)
                {
                    (currentRow.FindControl("lblCompanyAdd1") as Label).ForeColor = Color.Red;
                }

                if ((currentRow.FindControl("lblCompanyAdd2") as Label).Text != (nextRow.FindControl("lblCompanyAdd2") as Label).Text)
                {
                    (currentRow.FindControl("lblCompanyAdd2") as Label).ForeColor = Color.Red;
                }

                if ((currentRow.FindControl("lblCompanyAdd3") as Label).Text != (nextRow.FindControl("lblCompanyAdd3") as Label).Text)
                {
                    (currentRow.FindControl("lblCompanyAdd3") as Label).ForeColor = Color.Red;
                }

                if ((currentRow.FindControl("lblCompanyAdd4") as Label).Text != (nextRow.FindControl("lblCompanyAdd4") as Label).Text)
                {
                    (currentRow.FindControl("lblCompanyAdd4") as Label).ForeColor = Color.Red;
                }

                //Compare Primary Company 
                if ((currentRow.FindControl("lblPrimary") as Label).Text != (nextRow.FindControl("lblPrimary") as Label).Text)
                {
                    (currentRow.FindControl("lblPrimary") as Label).ForeColor = Color.Red;
                }

                //Compare Display Vat
                if ((currentRow.FindControl("lblDisplayaVat") as Label).Text != (nextRow.FindControl("lblDisplayaVat") as Label).Text)
                {
                    (currentRow.FindControl("lblDisplayaVat") as Label).ForeColor = Color.Red;
                }


                //Compare Account Company
                if ((currentRow.FindControl("lblAccountCompany") as Label).Text != (nextRow.FindControl("lblAccountCompany") as Label).Text)
                {
                    (currentRow.FindControl("lblAccountCompany") as Label).ForeColor = Color.Red;
                }

                //Compare Currency
                if ((currentRow.FindControl("lblCurrency") as Label).Text != (nextRow.FindControl("lblCurrency") as Label).Text)
                {
                    (currentRow.FindControl("lblCurrency") as Label).ForeColor = Color.Red;
                }

                //Compare Domestic Currency Grouping
                if ((currentRow.FindControl("lblDomCurrencyGroup") as Label).Text != (nextRow.FindControl("lblDomCurrencyGroup") as Label).Text)
                {
                    (currentRow.FindControl("lblDomCurrencyGroup") as Label).ForeColor = Color.Red;
                }

                //Compare Recouped Statements
                if ((currentRow.FindControl("lblThresholdRecouped") as Label).Text != (nextRow.FindControl("lblThresholdRecouped") as Label).Text)
                {
                    (currentRow.FindControl("lblThresholdRecouped") as Label).ForeColor = Color.Red;
                }

                //Compare Unrecouped Statements
                if ((currentRow.FindControl("lblThresholdUnrecouped") as Label).Text != (nextRow.FindControl("lblThresholdUnrecouped") as Label).Text)
                {
                    (currentRow.FindControl("lblThresholdUnrecouped") as Label).ForeColor = Color.Red;
                }
            }
        }

        private void FuzzySearchCompany()
        {
            if (txtCompanySearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text company search field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllCompanyList(txtCompanySearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            util = new Utilities();
            util.GenericExceptionHandler(errorMsg + "<br />" + expMsg);
            util = null;
        }

        #endregion Methods



    }
}
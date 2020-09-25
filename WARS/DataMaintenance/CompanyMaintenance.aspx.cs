/*
File Name   :   CompanyMaintenance.cs
Purpose     :   used for maintaining company data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     01-Mar-2017     Pratik(Infosys Limited)   Initial Creation
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

namespace WARS
{
    public partial class CompanyMaintenance : System.Web.UI.Page
    {
        #region Global Declarations

        CompanyMaintenanceBL companyMaintenanceBL;
        string loggedUserID;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        #endregion Global Declarations

        #region EVENTS

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Company Maintenance";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Company Maintenance";
                }

                lblTab.Focus();//tabbing sequence starts here

                if (!IsPostBack)
                {
                    txtCompanySearch.Focus();
                    
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadData();
                        UserAuthorization();
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

        protected void btnHdnCompanySearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadCompanyData();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading company details.", ex.Message);
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
                    hdnCompanySearch.Value = string.Empty;
                    hdnIsValidSearch.Value = "Y";
                    return;
                }

                txtCompanySearch.Text = lbFuzzySearch.SelectedValue.ToString();
                hdnIsValidSearch.Value = "Y";
                hdnChangeNotSaved.Value = "N";
                hdnCompanySearch.Value = lbFuzzySearch.SelectedValue.ToString();
                LoadCompanyData();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting company.", ex.Message);
            }
        }

        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (hdnIsValidSearch.Value == "N")
                {
                    ClearScreenFields();
                }
                                
                hdnIsValidSearch.Value = "N";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting company from searh list.", ex.Message);
            }
        }

        protected void btnNewCompany_Click(object sender, EventArgs e)
        {
            try
            {
                ClearScreenFields();

                btnCancel.Visible = true;
                btnCompanyAudit.Visible = false;
                txtCompanySearch.Enabled = false;
                fuzzySearchCompany.Enabled = false;
                hdnIsValidSearch.Value = "N";
                
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in creating new company.", ex.Message);
            }
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {            
            try
            {                
                if (string.IsNullOrWhiteSpace(txtCompanySearch.Text))
                {
                    txtCompanySearch.Enabled = true;
                    btnCancel.Visible = false;
                    btnCompanyAudit.Visible = true;

                    string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                    string isPrimary;
                    string accountCompany;
                    string isDisplayVat;

                    if (cbPrimaryCompany.Checked)
                    {
                        isPrimary = "Y";
                    }
                    else
                    {
                        isPrimary = "N";
                    }


                    if (ddlAccountCompany.SelectedIndex > 0)
                    {
                        accountCompany = ddlAccountCompany.SelectedValue;
                    }
                    else
                    {
                        accountCompany = string.Empty;
                    }

                    if (cbDisplayVat.Checked)
                    {
                        isDisplayVat = "Y";
                    }
                    else
                    {
                        isDisplayVat = "N";
                    }


                    companyMaintenanceBL = new CompanyMaintenanceBL();
                    DataSet companyData = companyMaintenanceBL.InsertCompanyData(txtCompanyName.Text.Trim(),txtDescription.Text.Trim(), txtAddress1.Text.Trim(), txtAddress2.Text.Trim(), txtAddress3.Text.Trim(), txtAddress4.Text.Trim(),
                                          ddlCurrency.SelectedValue, txtDomesticCurrency.Text.Trim(), txtPaymentThreshold.Text.Trim(), txtRecoupedStmt.Text.Trim(), txtUnrecoupedStmt.Text.Trim(),
                                          isPrimary,isDisplayVat, accountCompany, userCode, out errorId);
                    companyMaintenanceBL = null;

                    if (errorId == 1)
                    {
                        msgView.SetMessage("Can not create new company as already a primary company exists.", MessageType.Success, PositionType.Auto);
                    }
                    else if (companyData.Tables.Count != 0 && errorId != 2)
                    {
                        txtCompanySearch.Text = companyData.Tables[0].Rows[0]["company_code"].ToString() + " - " + companyData.Tables[0].Rows[0]["company_name"].ToString();
                        hdnIsValidSearch.Value = "Y";
                        Session["FuzzySearchAllCompanyList"] = companyData.Tables[1];
                        msgView.SetMessage("New company created successfully.", MessageType.Success, PositionType.Auto);
                    }
                    else
                    {
                        msgView.SetMessage("Failed to create new company.", MessageType.Warning, PositionType.Auto);
                    }

                    hdnChangeNotSaved.Value = "N";
                }
                else if (hdnIsValidSearch.Value == "Y")
                {
                    string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                    int companyCode = Convert.ToInt32(txtCompanySearch.Text.Split('-')[0].ToString().Trim());
                    string isPrimary;
                    string accountCompany;
                    string isDisplayVat;
                    if (cbPrimaryCompany.Checked)
                    {
                        isPrimary = "Y";
                    }
                    else
                    {
                        isPrimary = "N";
                    }

                    if (ddlAccountCompany.SelectedIndex > 0)
                    {
                        accountCompany = ddlAccountCompany.SelectedValue;
                    }
                    else
                    {
                        accountCompany = string.Empty;
                    }

                    if (cbDisplayVat.Checked)
                    {
                        isDisplayVat = "Y";
                    }
                    else
                    {
                        isDisplayVat = "N";
                    }

                    companyMaintenanceBL = new CompanyMaintenanceBL();
                    DataSet companyData = companyMaintenanceBL.UpdateCompanyData(companyCode, txtCompanyName.Text.Trim(),txtDescription.Text.Trim(), txtAddress1.Text.Trim(), txtAddress2.Text.Trim(), txtAddress3.Text.Trim(), txtAddress4.Text.Trim(),
                                                                                 ddlCurrency.SelectedValue, txtDomesticCurrency.Text.Trim(), txtPaymentThreshold.Text.Trim(), txtRecoupedStmt.Text.Trim(), 
                                                                                 txtUnrecoupedStmt.Text.Trim(), isPrimary,isDisplayVat, accountCompany, userCode, out errorId);
                    companyMaintenanceBL = null;

                    if (errorId == 1)
                    {
                        msgView.SetMessage("Can not update company details as already a primary company exists.", MessageType.Success, PositionType.Auto);
                    }
                    else if (errorId == 0)
                    {
                        msgView.SetMessage("Company details updated successfully.", MessageType.Success, PositionType.Auto);
                    }
                    else
                    {
                        msgView.SetMessage("Failed to update company details.", MessageType.Warning, PositionType.Auto);
                    }

                    hdnChangeNotSaved.Value = "N";
                }
                else if (hdnIsValidSearch.Value == "N")
                {
                    msgView.SetMessage("Please select a valid company from serach list.", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving data.", ex.Message);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearScreenFields();
            btnCancel.Visible = false;
            btnCompanyAudit.Visible = true;
        }

        protected void btnCompanyAudit_Click(object sender, EventArgs e)
        {
            if (hdnIsValidSearch.Value == "Y")
            {
                Response.Redirect("../Audit/CompanyAudit.aspx?companyData=" + txtCompanySearch.Text + "");
            }
            else
            {
                Response.Redirect("../Audit/CompanyAudit.aspx");
            }
        }

        protected void btnUnSavedDataExit_Click(object sender, EventArgs e)
        {
            if (hdnButtonSelection.Value == "btnNewCompany")
            {
                btnNewCompany_Click(sender, e);
            }
            else if (hdnButtonSelection.Value == "btnHdnCompanySearch")
            {
                btnHdnCompanySearch_Click(sender, e);
            }
            else if (hdnButtonSelection.Value == "fuzzySearchCompany")
            {                
                fuzzySearchCompany_Click(sender, new ImageClickEventArgs(0, 0));
            }


            hdnIsConfirmPopup.Value = "N";
        }

        #endregion EVENTS

        #region METHODS

        private void LoadData()
        {
            companyMaintenanceBL = new CompanyMaintenanceBL();
            DataSet initialData = companyMaintenanceBL.GetInitialData(out errorId);
            companyMaintenanceBL = null;

            if (initialData.Tables.Count != 0 && errorId != 2)
            {
                
                ddlCurrency.DataSource = initialData.Tables[0];
                ddlCurrency.DataTextField = "currency_data";
                ddlCurrency.DataValueField = "currency_code";
                ddlCurrency.DataBind();
                ddlCurrency.Items.Insert(0, new ListItem("-"));

                ddlAccountCompany.DataSource = initialData.Tables[1];
                ddlAccountCompany.DataTextField = "accountcompany_data";
                ddlAccountCompany.DataValueField = "accountcompany_code";
                ddlAccountCompany.DataBind();
                ddlAccountCompany.Items.Insert(0, new ListItem("-"));

                if (Request.QueryString != null && Request.QueryString.Count > 0)
                {                    
                    txtCompanySearch.Text = Request.QueryString[0];
                    hdnIsValidSearch.Value = "Y";
                    LoadCompanyData();
                }
            }
            else
            {
                ExceptionHandler("Error in fetching filter list data", string.Empty);
            }
        }

        private void LoadCompanyData()
        {
            int companyCode =Convert.ToInt32(txtCompanySearch.Text.Split('-')[0].ToString().Trim());

            companyMaintenanceBL = new CompanyMaintenanceBL();
            DataSet companyData = companyMaintenanceBL.GetSearchedCompanyData(companyCode,out errorId);
            companyMaintenanceBL = null;

            if (companyData.Tables.Count != 0 && errorId != 2)
            {
                txtCompanyName.Text = companyData.Tables[0].Rows[0]["company_name"].ToString();
                txtDescription.Text = companyData.Tables[0].Rows[0]["company_desc"].ToString();
                txtAddress1.Text = companyData.Tables[0].Rows[0]["company_add1"].ToString();
                txtAddress2.Text = companyData.Tables[0].Rows[0]["company_add2"].ToString();
                txtAddress3.Text = companyData.Tables[0].Rows[0]["company_add3"].ToString();
                txtAddress4.Text = companyData.Tables[0].Rows[0]["company_add4"].ToString();
                txtDomesticCurrency.Text = companyData.Tables[0].Rows[0]["domestic_currency_group"].ToString();
                txtPaymentThreshold.Text = companyData.Tables[0].Rows[0]["payment_threshold"].ToString();
                txtRecoupedStmt.Text = companyData.Tables[0].Rows[0]["threshold_recouped"].ToString();
                txtUnrecoupedStmt.Text = companyData.Tables[0].Rows[0]["threshold_unrecouped"].ToString();
                hdnCompanySearch.Value = txtCompanySearch.Text;
                hdnChangeNotSaved.Value = "N";

                string currencyCode = companyData.Tables[0].Rows[0]["currency_code"].ToString().Trim();
                ddlCurrency.ClearSelection();
                ddlCurrency.Items.FindByValue(currencyCode).Selected = true;

                string accountCompany = companyData.Tables[0].Rows[0]["account_company"].ToString().Trim();
                if (!string.IsNullOrWhiteSpace(accountCompany))
                {
                    ddlAccountCompany.ClearSelection();
                    ddlAccountCompany.Items.FindByValue(accountCompany).Selected = true;
                }
                
                if (companyData.Tables[0].Rows[0]["primary"].ToString() == "Y")
                {
                    cbPrimaryCompany.Checked = true;
                }
                else
                {
                    cbPrimaryCompany.Checked = false;
                }

                if (companyData.Tables[0].Rows[0]["display_vat"].ToString() == "Y")
                {
                    cbDisplayVat.Checked = true;
                }
                else
                {
                    cbDisplayVat.Checked = false;
                }
            }
            else
            {
                ExceptionHandler("Error in fetching companydata", string.Empty);
            }
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);      
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

        private void ClearScreenFields()
        {
            txtCompanySearch.Text = string.Empty;
            txtCompanyName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtAddress1.Text = string.Empty;
            txtAddress2.Text = string.Empty;
            txtAddress3.Text = string.Empty;
            txtAddress4.Text = string.Empty;
            txtDomesticCurrency.Text = string.Empty;
            txtPaymentThreshold.Text = string.Empty;
            txtRecoupedStmt.Text = string.Empty;
            txtUnrecoupedStmt.Text = string.Empty;
            ddlCurrency.SelectedIndex = 0;
            ddlAccountCompany.SelectedIndex = 0;
            cbPrimaryCompany.Checked = false;
            cbDisplayVat.Checked = false;
            hdnChangeNotSaved.Value = "N";
            txtCompanySearch.Enabled = true;
            fuzzySearchCompany.Enabled = true;
            hdnCompanySearch.Value = string.Empty;
        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only Read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnNewCompany.Enabled = false;
                btnSaveChanges.Enabled = false;
                btnCancel.Enabled = false;

            }

        }
        
        #endregion METHODS

                                 
    }
}
/*
File Name   :   SupplierAddressOverwrite.cs
Purpose     :   Used for overwriting payee address with supplier address

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     01-Sep-2017     Pratik(Infosys Limited)   Initial Creation
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

namespace WARS.Contract
{
    public partial class SupplierAddressOverwrite : System.Web.UI.Page
    {
        #region Global Declarations

        SupplierAddressOverwriteBL supplierAddressOverwriteBL;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Supplier Details - Address Overwrite";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Supplier Details - Address Overwrite";
                }

                txtPayee.Focus();//tabbing sequence starts here
                //PnlAccountTypeDetails.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {

                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadInitialData();
                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }

                    util = null;
                }

                UserAuthorization();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }
        }

        protected void btnAddressOverwrite_Click(object sender, EventArgs e)
        {
            try
            {
                mpeSaveCancel.Show();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in address overwrite.", ex.Message);
            }
        }

        protected void btnSupplierFuzzySearch_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchSupplier();
                lbFuzzySearch.Style.Add("height", "300");

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in supplier fuzzy search", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtSupplier.Text = string.Empty;
                    return;
                }
                ClearPayeeData();
                txtSupplier.Text = lbFuzzySearch.SelectedItem.ToString();
                hdnSupplierNumberSearch.Value = "Y";
                LoadSupplierSiteNames(txtSupplier.Text.Split('-')[0].ToString().Trim());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading supplier data", ex.Message);
            }
        }

        protected void btnOverwriteSupplierSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ClearPayeeData();
                hdnSupplierNumberSearch.Value = "Y";
                hdnSupplier.Value = txtSupplier.Text.Trim();
                LoadSupplierSiteNames(txtSupplier.Text.Split('-')[0].ToString().Trim());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading supplier data", ex.Message);
            }
        }

        protected void gvIntPartySearchList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Cells[5].Visible = false;
                    LinkButton _dblClickButton = e.Row.FindControl("lnkBtnDblClk") as LinkButton;
                    string _jsDoubleClick = ClientScript.GetPostBackClientHyperlink(_dblClickButton, "");
                    e.Row.Attributes.Add("ondblclick", _jsDoubleClick);

                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in payee data", ex.Message);
            }
        }

        protected void gvIntPartySearchList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                string intPartyId = (row.FindControl("hdnIntPartyId") as HiddenField).Value;
                string intPartyName = (row.FindControl("lblName") as Label).Text;

                if (e.CommandName == "dblClk")
                {
                    ClearPayeeData();
                    ClearSupplierData();
                    hdnSupplierNumberSearch.Value = "N";
                    hdnIntPartyId.Value = intPartyId;
                    txtPayee.Text = hdnPayee.Value = intPartyName;
                    LoadPayeeDetails(intPartyId);

                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in payee data", ex.Message);
            }
        }

        protected void btnOverwritePayeeSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //ClearSupplierData();

                supplierAddressOverwriteBL = new SupplierAddressOverwriteBL();
                DataSet intPartyList = supplierAddressOverwriteBL.GetPayeeList(txtPayee.Text, out errorId);
                supplierAddressOverwriteBL = null;

                if (intPartyList.Tables.Count != 0 && errorId != 2)
                {

                    if (intPartyList.Tables[0].Rows.Count == 0)
                    {
                        gvIntPartySearchList.DataSource = intPartyList.Tables[0];
                        gvIntPartySearchList.EmptyDataText = "No data found for the selected name";
                        gvIntPartySearchList.DataBind();
                    }
                    else
                    {
                        gvIntPartySearchList.DataSource = intPartyList.Tables[0];
                        gvIntPartySearchList.DataBind();

                        //set pop up panel and gridview panel height                    
                        pnlIntPartyPopup.Style.Add("height", (Convert.ToDouble(hdnGridPnlHeight.Value) * 0.75).ToString());
                        plnGridIntPartySearch.Style.Add("height", (Convert.ToDouble(hdnGridPnlHeight.Value) * 0.75 - 100).ToString());
                    }

                }
                else if (intPartyList.Tables.Count == 0 && errorId != 2)
                {
                    dtEmpty = new DataTable();
                    gvIntPartySearchList.DataSource = dtEmpty;
                    gvIntPartySearchList.EmptyDataText = "No data found for the selected name";
                    gvIntPartySearchList.DataBind();
                }
                else
                {
                    ExceptionHandler("Error in loading payee search data", string.Empty);
                }

                mpeIntPartySearch.Show();
            }
            catch (Exception ex)
            {
                mpeIntPartySearch.Hide();
                ExceptionHandler("Error in loading payee search data", ex.Message);
            }
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            try
            {
                supplierAddressOverwriteBL = new SupplierAddressOverwriteBL();
                DataSet updatedData = supplierAddressOverwriteBL.OverwritePayeeAddress(hdnSelectedPayee.Value, ddlRoyaltor.SelectedValue, txtSuppName.Text, txtSupplier.Text.Split('-')[0].ToString().Trim(), ddlSupplierSite.SelectedValue, txtSuppAdd1.Text, txtSuppAdd2.Text, txtSuppAdd3.Text, txtSuppCity.Text, txtSuppPostcode.Text, Convert.ToString(Session["UserCode"]), out errorId);
                supplierAddressOverwriteBL = null;

                if (updatedData.Tables.Count != 0 && errorId != 2)
                {
                    BindPayeeData(updatedData.Tables[0]);
                    BindSupplierData(updatedData.Tables[1]);
                }
                else
                {
                    ExceptionHandler("Error in address overwrite.", "");
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in address overwrite.", ex.Message);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                mpeSaveCancel.Hide();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in cancelling.", ex.Message);
            }
        }

        protected void btnClosePopupSupplierSearch_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ClearSupplierData();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing pop up.", ex.Message);
            }
        }

        protected void btnClosePopupPayeeSearch_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ClearPayeeData();
                ClearSupplierData();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing pop up.", ex.Message);
            }
        }

        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ClearSupplierData();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing pop up.", ex.Message);
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                hdnSelectedPayee.Value = string.Empty;
                hdnPayee.Value = string.Empty;
                txtPayee.Text = string.Empty;
                hdnIntPartyId.Value = string.Empty;
                txtMismatchFlag.Text = string.Empty;
                txtPayeeName.Text = string.Empty;
                ddlPayee.Items.Clear();
                ddlRoyaltor.Items.Clear();
                txtPayeeAdd1.Text = string.Empty;
                txtPayeeAdd2.Text = string.Empty;
                txtPayeeAdd3.Text = string.Empty;
                txtPayeeAdd4.Text = string.Empty;
                txtPayeePostcode.Text = string.Empty;

                hdnSupplier.Value = string.Empty;
                txtSupplier.Text = string.Empty;
                ddlSupplierSite.Items.Clear();
                txtSuppName.Text = string.Empty;
                txtSuppAdd1.Text = string.Empty;
                txtSuppAdd2.Text = string.Empty;
                txtSuppAdd3.Text = string.Empty;
                txtSuppCity.Text = string.Empty;
                txtSuppPostcode.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in clearing data.", ex.Message);
            }

        }

        protected void ddlSupplierSite_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ClearPayeeData();
            if (ddlSupplierSite.SelectedValue != "-")
            {
                GetPayeeRoyaltorList(string.Empty, txtSupplier.Text.Split('-')[0].ToString().Trim(), ddlSupplierSite.SelectedValue);
                if (ddlRoyaltor.SelectedValue != "-" && hdnIntPartyId.Value != "")
                {
                    LoadSupplierDetails(hdnIntPartyId.Value, ddlRoyaltor.SelectedValue, txtSupplier.Text.Split('-')[0].ToString().Trim(), ddlSupplierSite.SelectedValue);
                }
            }

        }

        protected void ddlRoyaltor_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlRoyaltor.SelectedValue != "-")
            {
                if (hdnSupplierNumberSearch.Value == "N")
                {
                    LoadSupplierDetails(hdnIntPartyId.Value, ddlRoyaltor.SelectedValue, txtSupplier.Text.Split('-')[0].ToString().Trim(), ddlSupplierSite.SelectedValue);

                }
                else
                {
                    DataTable dtPayee = ((DataTable)Session["SupAddressPayeeRoyaltorList"]).Select("royaltor_id=" + ddlRoyaltor.SelectedValue).CopyToDataTable();
                    dtPayee = dtPayee.DefaultView.ToTable(true, "int_party_id", "int_party_name");

                    if (dtPayee.Rows.Count == 1)
                    {
                        ddlPayee.Visible = false;
                        rfPayee.Visible = false;
                        txtPayee.Visible = true;
                        valPayee.Visible = true;
                        txtPayee.Text = dtPayee.Rows[0]["int_party_name"].ToString();
                        hdnIntPartyId.Value = dtPayee.Rows[0]["int_party_id"].ToString();
                        LoadSupplierDetails(dtPayee.Rows[0]["int_party_id"].ToString(), ddlRoyaltor.SelectedValue, txtSupplier.Text.Split('-')[0].ToString().Trim(), ddlSupplierSite.SelectedValue);

                    }
                    else
                    {
                        ddlPayee.Visible = true;
                        rfPayee.Visible = true;
                        txtPayee.Visible = false;
                        valPayee.Visible = false;
                        ddlPayee.DataSource = dtPayee;
                        ddlPayee.DataTextField = "int_party_name";
                        ddlPayee.DataValueField = "int_party_id";
                        ddlPayee.DataBind();
                        ddlPayee.Items.Insert(0, new ListItem("-"));
                        ddlPayee.Focus();
                    }
                }

               

            }
        }

        protected void ddlPayee_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPayee.SelectedValue != "-")
            {
                LoadSupplierDetails(ddlPayee.SelectedValue, ddlRoyaltor.SelectedValue, txtSupplier.Text.Split('-')[0].ToString().Trim(), ddlSupplierSite.SelectedValue);
            }
        }


        #endregion Events

        #region Methods

        public void LoadInitialData()
        {
            supplierAddressOverwriteBL = new SupplierAddressOverwriteBL();
            DataSet initialData = supplierAddressOverwriteBL.GetInitialData(out errorId);
            supplierAddressOverwriteBL = null;

            if (initialData.Tables.Count != 0 && errorId != 2)
            {
                Session["SupplierList"] = initialData.Tables[0];
                DataTable dtSupplierNumberList = initialData.Tables[0].DefaultView.ToTable(true, "supplier");
                Session["SuppAddOverwriteSupplierList"] = dtSupplierNumberList;


            }
            else
            {
                util = new Utilities();
                util.GenericExceptionHandler("Error in loading supplier list.");
                util = null;
            }
        }

        /* WUIN-1156 - removing as this is not being used
        public void LoadData(string intPartyId, string royaltorId, string supplierNumber, string supplierSiteName)
        {
            try
            {
                supplierAddressOverwriteBL = new SupplierAddressOverwriteBL();
                DataSet initialData = supplierAddressOverwriteBL.GetPayeeSuppData(intPartyId, royaltorId, supplierNumber, supplierSiteName, out errorId);
                supplierAddressOverwriteBL = null;

                if (initialData.Tables.Count != 0 && errorId != 2)
                {
                    BindPayeeData(initialData.Tables[0]);
                    BindSupplierData(initialData.Tables[1]);

                }
                else
                {
                    ExceptionHandler("Error in loading data.", "");
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in getting initial data.", "");
            }

        }
         * */

        private void GetPayeeRoyaltorList(string intPartyId, string supplierNumber, string supplierSiteName)
        {
            supplierAddressOverwriteBL = new SupplierAddressOverwriteBL();
            DataSet dsPayeeRoyaltorList = supplierAddressOverwriteBL.GetPayeeRoyaltorList(intPartyId, supplierNumber, supplierSiteName, out errorId);
            supplierAddressOverwriteBL = null;
            if (dsPayeeRoyaltorList.Tables.Count != 0 && errorId != 2)
            {
                DataTable dtPayeeRoyaltorList = dsPayeeRoyaltorList.Tables[0];
                if (dtPayeeRoyaltorList.Rows.Count > 0)
                {
                    Session["SupAddressPayeeRoyaltorList"] = dtPayeeRoyaltorList;
                    DataTable dtRoyaltorList = dtPayeeRoyaltorList.DefaultView.ToTable(true, "royaltor_id", "royaltor_name");

                    ddlRoyaltor.DataSource = dtRoyaltorList;
                    ddlRoyaltor.DataTextField = "royaltor_name";
                    ddlRoyaltor.DataValueField = "royaltor_id";
                    ddlRoyaltor.DataBind();
                    if (dtRoyaltorList.Rows.Count == 1)
                    {
                        ddlRoyaltor.SelectedValue = dtRoyaltorList.Rows[0][0].ToString();
                        DataRow[] dtPayee = dtPayeeRoyaltorList.Select("royaltor_id=" + ddlRoyaltor.SelectedValue);
                        if (dtPayee.Count() == 1)
                        {
                            hdnIntPartyId.Value = dtPayee[0]["int_party_id"].ToString();
                        }
                        else
                        {
                            ddlPayee.Visible = true;
                            rfPayee.Visible = true;
                            txtPayee.Visible = false;
                            valPayee.Visible = false;
                            ddlPayee.DataSource = dtPayee.CopyToDataTable();
                            ddlPayee.DataTextField = "int_party_name";
                            ddlPayee.DataValueField = "int_party_id";
                            ddlPayee.DataBind();
                            ddlPayee.Items.Insert(0, new ListItem("-"));
                            ddlPayee.Focus();
                        }
                    }
                    else
                    {
                        ddlRoyaltor.Items.Insert(0, new ListItem("-"));
                        ddlRoyaltor.Focus();
                    }

                }

            }
            else
            {
                ExceptionHandler("Error in loading roayltor list.", "");
            }
        }

        private void LoadPayeeDetails(string intPartyId)
        {
            GetPayeeRoyaltorList(intPartyId, string.Empty, string.Empty);

            if (ddlRoyaltor.SelectedValue != "-")
            {
                supplierAddressOverwriteBL = new SupplierAddressOverwriteBL();
                DataSet payeeDetails = supplierAddressOverwriteBL.GetPayeeSuppData(intPartyId, ddlRoyaltor.SelectedValue, string.Empty, string.Empty, out errorId);
                supplierAddressOverwriteBL = null;

                if (payeeDetails.Tables.Count != 0 && errorId != 2)
                {
                    BindPayeeData(payeeDetails.Tables[0]);
                    BindSupplierData(payeeDetails.Tables[1]);
                }
                else
                {
                    ExceptionHandler("Error in loading payee data.", "");
                }
            }

        }

        public void LoadSupplierDetails(string IntPartyId, string royaltorId, string supplierNumber, string supplierSiteName)
        {
            try
            {
                supplierAddressOverwriteBL = new SupplierAddressOverwriteBL();
                DataSet initialData = supplierAddressOverwriteBL.GetPayeeSuppData(IntPartyId, royaltorId, supplierNumber, supplierSiteName, out errorId);
                supplierAddressOverwriteBL = null;

                if (initialData.Tables.Count != 0 && errorId != 2)
                {
                    BindPayeeData(initialData.Tables[0]);
                    BindSupplierData(initialData.Tables[1]);
                }
                else
                {
                    ExceptionHandler("Error in loading supplier data", "");
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in getting initial data.", "");
            }

        }

        private void LoadSupplierSiteNames(string supplierNumber)
        {

            DataTable dtSupplierList = (DataTable)Session["SupplierList"];
            DataTable dtSupplierSiteNames = (from row in dtSupplierList.AsEnumerable()
                                             where row.Field<string>("supplier_number") == supplierNumber.Trim()
                                             select row).CopyToDataTable();
            ddlSupplierSite.Items.Clear();
            foreach (DataRow row in dtSupplierSiteNames.Rows)
            {
                ddlSupplierSite.Items.Add(row["supplier_site_name"].ToString());
            }
            if (dtSupplierSiteNames.Rows.Count == 1)
            {
                if (hdnSupplierNumberSearch.Value == "Y")
                {
                    ddlSupplierSite.SelectedValue = dtSupplierSiteNames.Rows[0]["supplier_site_name"].ToString();
                    GetPayeeRoyaltorList(string.Empty, txtSupplier.Text.Split('-')[0].ToString().Trim(), ddlSupplierSite.SelectedValue);

                    if (ddlRoyaltor.SelectedValue != "-" && hdnIntPartyId.Value != "")
                    {
                        LoadSupplierDetails(hdnIntPartyId.Value, ddlRoyaltor.SelectedValue, txtSupplier.Text.Split('-')[0].ToString().Trim(), ddlSupplierSite.SelectedValue);
                    }
                   
                }
            }
            else
            {
                ddlSupplierSite.Items.Insert(0, new ListItem("-"));
            }
        }

        private void BindPayeeData(DataTable dtPayeeDetails)
        {
            if (dtPayeeDetails.Rows.Count > 0)
            {
                hdnSelectedPayee.Value = dtPayeeDetails.Rows[0]["int_party_id"].ToString();
                hdnPayee.Value = dtPayeeDetails.Rows[0]["int_party_name"].ToString();
                txtPayee.Text = dtPayeeDetails.Rows[0]["int_party_name"].ToString();
                txtMismatchFlag.Text = dtPayeeDetails.Rows[0]["mismatch_address"].ToString();
                txtPayeeName.Text = dtPayeeDetails.Rows[0]["int_party_name"].ToString();
                txtPayeeAdd1.Text = dtPayeeDetails.Rows[0]["int_party_add1"].ToString();
                txtPayeeAdd2.Text = dtPayeeDetails.Rows[0]["int_party_add2"].ToString();
                txtPayeeAdd3.Text = dtPayeeDetails.Rows[0]["int_party_add3"].ToString();
                txtPayeeAdd4.Text = dtPayeeDetails.Rows[0]["int_party_add4"].ToString();
                txtPayeePostcode.Text = dtPayeeDetails.Rows[0]["int_party_postcode"].ToString();
            }


        }

        private void BindSupplierData(DataTable dtSupplierDetails)
        {
            if (dtSupplierDetails.Rows.Count > 0)
            {
                hdnSupplier.Value = dtSupplierDetails.Rows[0]["supplier"].ToString();
                txtSupplier.Text = dtSupplierDetails.Rows[0]["supplier"].ToString();
                if (hdnSupplierNumberSearch.Value != "Y")
                {
                    ddlSupplierSite.Items.Clear();
                    ddlSupplierSite.Items.Add(dtSupplierDetails.Rows[0]["supplier_site_name"].ToString());
                }
                txtSuppName.Text = dtSupplierDetails.Rows[0]["supplier_name"].ToString();
                txtSuppAdd1.Text = dtSupplierDetails.Rows[0]["supplier_add1"].ToString();
                txtSuppAdd2.Text = dtSupplierDetails.Rows[0]["supplier_add2"].ToString();
                txtSuppAdd3.Text = dtSupplierDetails.Rows[0]["supplier_add3"].ToString();
                txtSuppCity.Text = dtSupplierDetails.Rows[0]["supplier_add4"].ToString();
                txtSuppPostcode.Text = dtSupplierDetails.Rows[0]["supplier_postcode"].ToString();
            }

            ComparePayeeSupplierDetails();
        }

        private void ComparePayeeSupplierDetails()
        {
            if (txtPayeeName.Text.Trim() != txtSuppName.Text.Trim())
            {
                txtPayeeName.CssClass = "textboxStyle_changedData";
            }
            else
            {
                txtPayeeName.CssClass = "textboxStyle";
            }

            if (txtPayeeAdd1.Text.Trim() != txtSuppAdd1.Text.Trim())
            {
                txtPayeeAdd1.CssClass = "textboxStyle_changedData";
            }
            else
            {
                txtPayeeAdd1.CssClass = "textboxStyle";
            }

            if (txtPayeeAdd2.Text.Trim() != txtSuppAdd2.Text.Trim())
            {
                txtPayeeAdd2.CssClass = "textboxStyle_changedData";
            }
            else
            {
                txtPayeeAdd2.CssClass = "textboxStyle";
            }

            if (txtPayeeAdd3.Text.Trim() != txtSuppAdd3.Text.Trim())
            {
                txtPayeeAdd3.CssClass = "textboxStyle_changedData";
            }
            else
            {
                txtPayeeAdd3.CssClass = "textboxStyle";
            }

            if (txtPayeeAdd4.Text.Trim() != txtSuppCity.Text.Trim())
            {
                txtPayeeAdd4.CssClass = "textboxStyle_changedData";
            }
            else
            {
                txtPayeeAdd4.CssClass = "textboxStyle";
            }

            if (txtPayeePostcode.Text.Trim() != txtSuppPostcode.Text.Trim())
            {
                txtPayeePostcode.CssClass = "textboxStyle_changedData";
            }
            else
            {
                txtPayeePostcode.CssClass = "textboxStyle";
            }


        }

        private void FuzzySearchSupplier()
        {
            List<string> lstSuppliers = new List<string>();
            if (txtSupplier.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in supplier field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySuppAddOverwriteSupplierList(txtSupplier.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void ClearSupplierData()
        {
            hdnSupplier.Value = string.Empty;
            txtSupplier.Text = string.Empty;
            ddlSupplierSite.Items.Clear();
            txtSuppName.Text = string.Empty;
            txtSuppAdd1.Text = string.Empty;
            txtSuppAdd2.Text = string.Empty;
            txtSuppAdd3.Text = string.Empty;
            txtSuppCity.Text = string.Empty;
            txtSuppPostcode.Text = string.Empty;
        }

        private void ClearPayeeData()
        {
            hdnSelectedPayee.Value = string.Empty;
            hdnIntPartyId.Value = string.Empty;
            hdnPayee.Value = string.Empty;
            txtPayee.Text = string.Empty;
            ddlPayee.Items.Clear();
            ddlRoyaltor.Items.Clear();
            txtMismatchFlag.Text = string.Empty;
            txtPayeeName.Text = string.Empty;
            txtPayeeAdd1.Text = string.Empty;
            txtPayeeAdd2.Text = string.Empty;
            txtPayeeAdd3.Text = string.Empty;
            txtPayeeAdd4.Text = string.Empty;
            txtPayeePostcode.Text = string.Empty;
        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only Read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnAddressOverwrite.Enabled = false;
                btnCancel.Enabled = false;
                btnContinue.Enabled = false;
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
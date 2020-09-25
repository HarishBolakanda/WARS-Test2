/*
File Name   :   RoyContractPayeeSupp.cs
Purpose     :   to maintain payment supplier of royaltor contract

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     15-Aug-2017     Harish(Infosys Limited)   Initial Creation
        05-Dec-2017     Harish                    WUIN-359 changes: for a new royaltor, next tab should be enabled
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Threading;
using System.Web.UI.HtmlControls;
using WARS.BusinessLayer;
using System.Net;

namespace WARS.Contract
{
    public partial class RoyContractPayeeSupp : System.Web.UI.Page
    {
        #region Global Declarations
        RoyContractPayeeSuppBL royContractPayeeSuppBL;
        Utilities util;
        Int32 errorId;
        string loggedUserID;
        string royaltorId = string.Empty;
        string isNewRoyaltor = string.Empty;
        #endregion Global Declarations

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                royaltorId = Request.QueryString["RoyaltorId"];
                isNewRoyaltor = Request.QueryString["isNewRoyaltor"];
                //royaltorId = "12340";
                //royaltorId = "27";
                //royaltorId = "15829";//no payee
                //isNewRoyaltor = "N";

                if (royaltorId == null || isNewRoyaltor == null)
                {
                    msgView.SetMessage("Not a valid royaltor!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Contract - Payee Supplier Details";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Contract - Payee Supplier Details";
                }

                lblTab.Focus();//tabbing sequence starts here                                
                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        //HtmlTableRow trPayeeSuppDetails = (HtmlTableRow)contractNavigationButtons.FindControl("trPayeeSuppDetails");
                        //trPayeeSuppDetails.Visible = false;
                        Button btnSupplierDetails = (Button)contractNavigationButtons.FindControl("btnSupplierDetails");
                        btnSupplierDetails.Enabled = false;
                        HiddenField hdnRoyaltorId = (HiddenField)contractNavigationButtons.FindControl("hdnRoyaltorId");
                        hdnRoyaltorId.Value = royaltorId;
                        HiddenField hdnRoyaltorIdHdr = (HiddenField)contractHdrNavigation.FindControl("hdnRoyaltorId");
                        hdnRoyaltorIdHdr.Value = royaltorId;

                        //WUIN-599 - resetting the flags
                        ((HiddenField)Master.FindControl("hdnIsContractScreen")).Value = "N";
                        ((HiddenField)Master.FindControl("hdnIsNotContractScreen")).Value = "N";
                        this.Master.FindControl("lnkBtnHome").Visible = false;
                       

                        if (isNewRoyaltor == "Y")
                        {
                            btnSave.Text = "Save & Continue";
                            btnAudit.Text = "Back";
                            //contractNavigationButtons.DisableForPayeeSupp();                            
                            contractNavigationButtons.EnableNewRoyNavButtons(ContractScreens.PayeeSupplier.ToString());
                        }

                        txtRoyaltor.Text = royaltorId;
                        LoadPayeeSupplierData("-");

                        //WUIN-450 - If a new Royaltor is set up with Lock checked it should continue to allow entry of the details (the Royaltor set up needs to be completed!)
                        if (isNewRoyaltor != "Y" && contractNavigationButtons.IsRoyaltorLocked(royaltorId))
                        {

                            btnSave.ToolTip = "Royaltor Locked";
                        }
                        //WUIN-599 -- Only one user can use contract screens at the same time.
                        // If a contract is already using by another user then making the screen readonly.
                        if (isNewRoyaltor != "Y" &&  contractNavigationButtons.IsScreenLocked(royaltorId))
                        {
                            hdnOtherUserScreenLocked.Value = "Y";
                        }

                        //WUIN-1096 - Only Read access for ReadonlyUser
                        //WUIN-599 If a contract is already using by another user then making the screen readonly.
                        //WUIN-450 -Only Read access for locked contracts
                        if ((Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower()) ||
                            (isNewRoyaltor != "Y" && contractNavigationButtons.IsRoyaltorLocked(royaltorId)) ||
                            (isNewRoyaltor != "Y" && contractNavigationButtons.IsScreenLocked(royaltorId)))
                        {
                            EnableReadonly();
                        }
                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
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
        /// To add/update/delete payee supplier link details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {

                //check if any changes to save                
                if (hdnPayeeCurrency.Value == ddlCurrency.SelectedValue && hdnSupplierNumber.Value == txtSupplier.Text && hdnSupplierSiteName.Value == txtSuppSiteName.Text)
                {
                    if (isNewRoyaltor == "N")
                    {
                        msgView.SetMessage("No changes made to save!", MessageType.Warning, PositionType.Auto);
                    }
                    else if (isNewRoyaltor == "Y")
                    {
                        //WUIN-450
                        //set screen button enabled = Y
                        contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.PayeeSupplier.ToString());

                        ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId + ");", true);
                    }

                    return;
                }

                //WUIN-1156 - Commented below code as one payee can have multiple suppliers

                //if (hdnIsSupplierLinked.Value == "Y" && hdnSupplierDelete.Value != "Y")
                //{
                //    msgView.SetMessage("The supplier selected is already linked to another payee!", MessageType.Warning, PositionType.Auto);
                //    return;
                //}

                if (hdnSupplierDelete.Value != "Y" && hdnSupplierNumber.Value != txtSupplier.Text && (txtSuppName.Text.Trim() != hdnPayeeName.Value.Trim() || txtSuppAdd1.Text.Trim() != hdnPayeeAddress1.Value.Trim() || txtPostCode.Text.Trim() != hdnPayeePostcode.Value.Trim()))
                {
                    sao1.LoadData(ddlPayee.SelectedValue, royaltorId, txtSupplier.Text, txtSuppSiteName.Text);
                    mpeAddressOverwrite.Show();
                }
                else
                {
                    SavePayeeSupplier();
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving supplier", ex.Message);
            }
        }

        protected void ddlPayee_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadPayeeSupplierData(ddlPayee.SelectedValue);

                //clear other filter fields
                txtSupplierSearch.Text = string.Empty;
                ddlSupplierSite.Items.Clear();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading supplier data", ex.Message);
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
                    txtSupplierSearch.Text = string.Empty;
                    return;
                }

                txtSupplierSearch.Text = lbFuzzySearch.SelectedValue.ToString();
                LoadSupplierSiteNames(txtSupplierSearch.Text.Split('-')[0].ToString().Trim());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading supplier data", ex.Message);
            }
        }

        protected void btnSupplierSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadSupplierSiteNames(txtSupplierSearch.Text.Split('-')[0].ToString().Trim());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading supplier data", ex.Message);
            }
        }
        protected void ddlSupplierSite_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSupplierSite.SelectedValue != "-")
            {
                OnSupplierChange(ddlSupplierSite.SelectedValue);
            }
        }

        protected void btnClosePopupAddressOverwrite_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                //Check if supplier and payee addresses are same after closing the pop up
                //If not same update mismatch flag to Y
                if ((sao1.FindControl("txtSuppAdd1") as TextBox).Text != (sao1.FindControl("txtPayeeAdd1") as TextBox).Text
                    || (sao1.FindControl("txtSuppPostcode") as TextBox).Text != (sao1.FindControl("txtPayeePostcode") as TextBox).Text)
                {
                    hdnMismatchFlag.Value = "Y";

                }
                else if ((sao1.FindControl("hdnIsAddressOverwritten") as HiddenField).Value == "Y") //If addresses are overwritten then change the text of payee in dropdown and assign new address values to hiddenfields
                {
                    ddlPayee.Items.FindByValue(ddlPayee.SelectedValue).Text = (sao1.FindControl("txtPayeeName") as TextBox).Text;
                    hdnPayeeName.Value = (sao1.FindControl("txtSuppName") as TextBox).Text;
                    hdnPayeeAddress1.Value = (sao1.FindControl("txtSuppAdd1") as TextBox).Text;
                    hdnPayeePostcode.Value = (sao1.FindControl("txtSuppPostcode") as TextBox).Text;
                }

                SavePayeeSupplier();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving supplier", ex.Message);
            }
        }

        protected void btnClearSupplier_Click(object sender, EventArgs e)
        {
            ClearSupplierFields();
            hdnSupplierDelete.Value = "Y";
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSupplierSearch.Text = string.Empty;
            ddlSupplierSite.Items.Clear();
            LoadPayeeSupplierData(ddlPayee.SelectedValue);
        }
        #endregion Events

        #region Methods

        private void LoadPayeeSupplierData(string intPartyId)
        {

            string royaltor = string.Empty;
            string supplierSiteNameRegValue = string.Empty;
            DataSet payeeSuppData = null;

            if (intPartyId == "-")
            {
                //initial load
                royContractPayeeSuppBL = new RoyContractPayeeSuppBL();
                payeeSuppData = royContractPayeeSuppBL.GetInitialData(royaltorId, out royaltor, out supplierSiteNameRegValue, out errorId);
                royContractPayeeSuppBL = null;

                if (errorId == 2 || payeeSuppData.Tables.Count == 0)
                {
                    ExceptionHandler("Error in loading supplier data", string.Empty);
                    return;
                }
                txtRoyaltor.Text = royaltor;

                ddlPayee.DataSource = payeeSuppData.Tables["PayeeList"];
                ddlPayee.DataTextField = "item_text";
                ddlPayee.DataValueField = "item_value";
                ddlPayee.DataBind();

                Session["roySuppPayeeList"] = payeeSuppData.Tables["PayeeList"];

                if (payeeSuppData.Tables["PayeeList"].Rows.Count == 0)
                {
                    ddlPayee.Items.Insert(0, new ListItem("-"));
                    ddlCurrency.Items.Insert(0, new ListItem("-"));
                    ddlCurrency.SelectedIndex = 0;

                    //Do not allow Supplier selection if no Payees exist for the Royaltor                    
                    txtSupplierSearch.ReadOnly = true;
                    btnSupplierFuzzySearch.Enabled = false;
                }
                else
                {
                    //WUIN-1223 Populatating currency only if royaltor has payees.
                    ddlCurrency.DataSource = payeeSuppData.Tables["CurrencyList"];
                    ddlCurrency.DataTextField = "currency_code";
                    ddlCurrency.DataValueField = "currency_code";
                    ddlCurrency.DataBind();

                    //select the primary payee on initial load
                    //int intPartyId = Convert.ToInt32(payeeSuppData.Tables["PayeeList"].Compute("min([item_value])", string.Empty));
                    DataRow[] dtPrimaryPayee = payeeSuppData.Tables["PayeeList"].Select("primary_payee='Y'");
                    if (dtPrimaryPayee.Count() == 1)
                    {
                        ddlPayee.SelectedValue = dtPrimaryPayee[0]["item_value"].ToString();
                        ddlCurrency.SelectedValue = hdnPayeeCurrency.Value = dtPrimaryPayee[0]["payee_currency"].ToString(); //WUIN-1233
                    }

                    DataRow[] dtPayeeAddress = payeeSuppData.Tables["PayeeList"].Select("item_value=" + ddlPayee.SelectedValue + "");
                    if (dtPayeeAddress.Count() == 1)
                    {
                        hdnPayeeName.Value = dtPayeeAddress[0]["item_text"].ToString();
                        hdnPayeeAddress1.Value = dtPayeeAddress[0]["int_party_add1"].ToString();
                        hdnPayeePostcode.Value = dtPayeeAddress[0]["int_party_postcode"].ToString();
                    }


                }
                Session["SupplierList"] = payeeSuppData.Tables["SupplierList"];
                DataTable dtSupplierNumberList = payeeSuppData.Tables["SupplierList"].DefaultView.ToTable(true, "supplier");
                Session["ContPayeeSupplierList"] = dtSupplierNumberList;
                hdnSupplierSiteNameRegValue.Value = supplierSiteNameRegValue;

            }
            else
            {
                royContractPayeeSuppBL = new RoyContractPayeeSuppBL();
                payeeSuppData = royContractPayeeSuppBL.GetPayeeSupplier(royaltorId, intPartyId, out errorId);
                royContractPayeeSuppBL = null;
                //WUIN-1233 Population currecny for selected payee 
                DataRow[] dtPayee = ((DataTable)Session["roySuppPayeeList"]).Select("item_value=" + intPartyId);
                if (dtPayee.Count() == 1)
                {
                    ddlCurrency.SelectedValue = hdnPayeeCurrency.Value = dtPayee[0]["payee_currency"].ToString();
                }
            }

            if (errorId == 2 || payeeSuppData.Tables.Count == 0)
            {
                ExceptionHandler("Error in loading supplier data", string.Empty);
                return;
            }

            LoadSupplierDetails(payeeSuppData.Tables["PayeeSupplier"]);

            //assign hidden field values           
            hdnSupplierNumber.Value = txtSupplier.Text;
            hdnSupplierSiteName.Value = txtSuppSiteName.Text;

        }

        private void LoadSupplierDetails(DataTable dtSupplier)
        {
            if (dtSupplier.Rows.Count != 0)
            {
                txtSupplier.Text = Convert.ToString(dtSupplier.Rows[0]["supplier_number"]);
                txtSuppName.Text = Convert.ToString(dtSupplier.Rows[0]["supplier_name"]);
                txtSuppAdd1.Text = Convert.ToString(dtSupplier.Rows[0]["supplier_add1"]);
                txtSuppAdd2.Text = Convert.ToString(dtSupplier.Rows[0]["supplier_add2"]);
                txtSuppAdd3.Text = Convert.ToString(dtSupplier.Rows[0]["supplier_add3"]);
                txtSuppAdd4.Text = Convert.ToString(dtSupplier.Rows[0]["supplier_add4"]);
                txtPostCode.Text = Convert.ToString(dtSupplier.Rows[0]["supplier_postcode"]);
                txtAccCompany.Text = Convert.ToString(dtSupplier.Rows[0]["account_company"]);
                txtSuppSiteName.Text = Convert.ToString(dtSupplier.Rows[0]["supplier_site_name"]);
                txtCurrency.Text = hdnPayeeCurrency.Value; //WUIN-1233
                txtPaymentTerms.Text = Convert.ToString(dtSupplier.Rows[0]["payment_terms"]);

                if (Convert.ToString(dtSupplier.Rows[0]["active"]) == "Y")
                {
                    cbActive.Checked = true;
                }
                else
                {
                    cbActive.Checked = false;
                }

                if (Convert.ToString(dtSupplier.Rows[0]["maint_expected"]) == "Y")
                {
                    cbMaintExpected.Checked = true;
                }
                else
                {
                    cbMaintExpected.Checked = false;
                }

                //Check if there is a int party id linked to the supplier
                //if (!string.IsNullOrWhiteSpace(Convert.ToString(dtSupplier.Rows[0]["int_party_id"])))
                //{
                //    hdnIsSupplierLinked.Value = "Y";
                //}
                //else
                //{
                //    hdnIsSupplierLinked.Value = "N";
                //}
            }
            else
            {
                ClearSupplierFields();
            }
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

        }

        private void ClearSupplierFields()
        {
            txtSupplier.Text = string.Empty;
            txtSuppName.Text = string.Empty;
            txtSuppAdd1.Text = string.Empty;
            txtSuppAdd2.Text = string.Empty;
            txtSuppAdd3.Text = string.Empty;
            txtSuppAdd4.Text = string.Empty;
            txtPostCode.Text = string.Empty;
            txtAccCompany.Text = string.Empty;
            txtSuppSiteName.Text = string.Empty;
            txtCurrency.Text = string.Empty;
            txtPaymentTerms.Text = string.Empty;
            cbActive.Checked = false;
            cbMaintExpected.Checked = false;

        }

        private void FuzzySearchSupplier()
        {
            if (txtSupplierSearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in supplier search field", MessageType.Warning, PositionType.Auto);
                ClearSupplierFields();
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchContPayeeSuppSupplierList(txtSupplierSearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }
        //WUIN-1060 : Bind site names for selected supplier.
        private void LoadSupplierSiteNames(string supplierNumber)
        {

            DataTable dtSupplierList = (DataTable)Session["SupplierList"];
            DataTable dtSupplierSiteNames = (from row in dtSupplierList.AsEnumerable()
                                             where row.Field<string>("supplier_number") == supplierNumber.Trim()
                                             select row).CopyToDataTable();
            ddlSupplierSite.Items.Clear();

            ddlSupplierSite.Items.Add(hdnSupplierSiteNameRegValue.Value);

            foreach (DataRow row in dtSupplierSiteNames.Rows)
            {
                //WUIN-1233 Avoiding default supplier site name getting added to dropdown for avoding duplicates
                if (row["supplier_site_name"].ToString() != hdnSupplierSiteNameRegValue.Value) 
                {
                    ddlSupplierSite.Items.Add(row["supplier_site_name"].ToString());
                }
            }

            ddlSupplierSite.SelectedValue = hdnSupplierSiteNameRegValue.Value;
            OnSupplierChange(hdnSupplierSiteNameRegValue.Value);

            //if (dtSupplierSiteNames.Rows.Count == 1)
            //{
            //    ddlSupplierSite.SelectedValue = dtSupplierSiteNames.Rows[0]["supplier_site_name"].ToString();

            //}
            //else
            //{
            //    ddlSupplierSite.Items.Insert(0, new ListItem("-"));
            //    txtSuppNameSearch.Text = string.Empty;
            //    txtSuppAdd1Search.Text = string.Empty;
            //}

        }


        public void OnSupplierChange(string supplierSiteName)
        {
            if (supplierSiteName != "-")
            {
                LoadSupplierSearchData(txtSupplierSearch.Text.Split('-')[0].ToString().Trim(), supplierSiteName);
            }

            hdnSupplierDelete.Value = "N";
        }



        private void LoadSupplierSearchData(string supplierNumber, string supplierSiteName)
        {
            if (Session["ContPayeeSupplierList"] == null)
            {
                royaltorId = txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1);
                royContractPayeeSuppBL = new RoyContractPayeeSuppBL();
                DataSet dropdownListData = royContractPayeeSuppBL.GetDropdownData(royaltorId, out errorId);
                royContractPayeeSuppBL = null;
                Session["SupplierList"] = dropdownListData.Tables["SupplierList"];
                DataTable dtSupplierNumberList = dropdownListData.Tables["SupplierList"].DefaultView.ToTable(true, "supplier");
                Session["ContPayeeSupplierList"] = dtSupplierNumberList;
            }

            DataTable dtSupplierList = (DataTable)Session["SupplierList"];

            DataRow[] drSupplier = dtSupplierList.Select("supplier_number='" + supplierNumber + "' and supplier_site_name='" + supplierSiteName + "'");
            if (drSupplier.Count() > 0)
            {
                LoadSupplierDetails(drSupplier.CopyToDataTable());
            }
            else
            {
                ClearSupplierFields();
            }

        }

        private void SavePayeeSupplier()
        {

            //check if insert/update/delete
            string insertUpdDelete = string.Empty;
            //if (hdnSupplierNumber.Value == string.Empty && txtSupplier.Text != string.Empty)
            //{
            //    //Insert
            //    insertUpdDelete = "I";
            //}
            //else 
            if (hdnSupplierDelete.Value == "Y" && hdnSupplierNumber.Value != string.Empty && hdnSupplierSiteName.Value != string.Empty)
            {
                //delete
                insertUpdDelete = "D";
            }
            else if ((txtSupplier.Text != hdnSupplierNumber.Value) || (txtSuppSiteName.Text != hdnSupplierSiteName.Value))
            {
                //update
                insertUpdDelete = "U";
            }
            else if (hdnPayeeCurrency.Value != ddlCurrency.SelectedValue)
            {
                insertUpdDelete = "C";  //WUIN-1233 - Saving currecy alone
            }


            royContractPayeeSuppBL = new RoyContractPayeeSuppBL();
            DataSet payeeSuppData = royContractPayeeSuppBL.SavePayeeSupplier(royaltorId, ddlPayee.SelectedValue, ddlCurrency.SelectedValue, hdnSupplierNumber.Value, txtSupplier.Text, hdnSupplierSiteName.Value, txtSuppSiteName.Text,
            txtAccCompany.Text, hdnMismatchFlag.Value, insertUpdDelete, Convert.ToString(Session["UserCode"]), out errorId);
            royContractPayeeSuppBL = null;

            //if (errorId == 2 || payeeSuppData.Tables["PayeeSupplier"].Rows.Count == 0)
            if (errorId == 2)
            {
                ExceptionHandler("Error in saving supplier data", string.Empty);
                return;
            }
            //WUIN-1233
            if (hdnPayeeCurrency.Value != ddlCurrency.SelectedValue)
            {
                hdnPayeeCurrency.Value = ddlCurrency.SelectedValue;

                //WUIN-1233 - If currency changed then updating payee list datatable with updated currency code
                DataRow[] dtPayee = ((DataTable)Session["roySuppPayeeList"]).Select("item_value=" + ddlPayee.SelectedValue);
                if (dtPayee.Count() == 1)
                {
                    dtPayee[0]["payee_currency"] = ddlCurrency.SelectedValue;
                }
            }

            LoadSupplierDetails(payeeSuppData.Tables["PayeeSupplier"]);
            Session["SupplierList"] = payeeSuppData.Tables["SupplierList"];
            DataTable dtSupplierNumberList = payeeSuppData.Tables["SupplierList"].DefaultView.ToTable(true, "supplier");
            Session["ContPayeeSupplierList"] = dtSupplierNumberList;


            //new royaltor - redirect to Option periods screen
            //existing royaltor - remain in same screen
            if (isNewRoyaltor == "Y")
            {
                //WUIN-450
                //set screen button enabled = Y
                contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.PayeeSupplier.ToString());

                //redirect in javascript so that issue of data not saved validation would be handled
                ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId + ");", true);
            }
            else
            {
                if (hdnMismatchFlag.Value == "Y")
                {
                    msgView.SetMessage("Payee supplier details saved. Address of payee is different to address of supplier - payments cannot be approved until difference is resolved", MessageType.Warning, PositionType.Auto);
                }
                else if (insertUpdDelete == "C") //WUIN-1233
                {
                    msgView.SetMessage("Payee currency saved", MessageType.Warning, PositionType.Auto);
                }
                else
                {
                    msgView.SetMessage("Payee supplier details saved", MessageType.Warning, PositionType.Auto);
                }
            }

            //reset the values of hidden fields            
            hdnMismatchFlag.Value = "N";
            hdnSupplierDelete.Value = "N";
            hdnSupplierNumber.Value = txtSupplier.Text;
            hdnSupplierSiteName.Value = txtSuppSiteName.Text;
            ddlSupplierSite.Items.Clear();
            txtSupplierSearch.Text = string.Empty;
        }

        private void EnableReadonly()
        {
            btnSave.Enabled = false;
            btnClearSupplier.Enabled = false;
            btnClearSearch.Enabled = false;
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
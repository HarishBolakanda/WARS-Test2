/*
File Name   :   InterestedPartyMaintenance.cs
Purpose     :   to maintain interested party data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     16-Mar-2016     Pratik(Infosys Limited)   Initial Creation
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
    public partial class InterestedPartyMaintenance : System.Web.UI.Page
    {
        #region Global Declarations

        InterestedPartyMaintenanceBL interestedPartyMaintenanceBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        int gridDefaultPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridDefaultPageSize1"].ToString());
        #endregion Global Declarations

        #region Sorting
        string ascending = "Asc";
        string descending = "Desc";
        string sort_Up = "~/Images/Sort_up.png";
        string sort_Down = "~/Images/Sort_down.png";
        #endregion Sorting

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Interested Party Maintenance";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Interested Party Maintenance";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlInterestedPartyDetails.Style.Add("height", hdnGridPnlHeight.Value);
                pnlLinkedRoyaltorsPopUp.Style.Add("height", hdnGridPnlHeight.Value);
                //pnlmpePayeeBankDetailsPopUp.Style.Add("height", hdnGridPnlHeight.Value);
                //pnlSuppliersPopUp.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    txtInterestedPartySearch.Focus();
                    btnPayeeDetails.Visible = false;
                    btnCourtesyDetails.Visible = false;
                    Session["InterestedPartyList"] = null;
                    Session["ActiveIntPartyData"] = null;
                    Session["InterestedPartyTypeList"] = null;

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

        //protected void fuzzySearchInterestedParty_Click(object sender, ImageClickEventArgs e)
        //{
        //    try
        //    {
        //        FuzzySearchInterestedParty();
        //        lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandler("Error in interested party search.", ex.Message);
        //    }
        //}

        protected void ddlIntPartyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                hdnPageNumber.Value = "1";
                //if (Session["ActiveIntPartyData"] == null || Session["InterestedPartyList"] == null)
                //{
                //    return;
                //}

                //DataTable dtActiveIntPartyData = Session["ActiveIntPartyData"] as DataTable;
                //DataTable dtInterestedPartyList = Session["InterestedPartyList"] as DataTable;
                //if (!string.IsNullOrWhiteSpace(txtInterestedPartySearch.Text) && ddlIntPartyType.SelectedIndex > 0)
                //{
                //    DataTable dtFilterData = FilterData(dtActiveIntPartyData, ddlIntPartyType.SelectedValue);
                //    BindGrid(dtFilterData);
                //}
                //else if (!string.IsNullOrWhiteSpace(txtInterestedPartySearch.Text) && ddlIntPartyType.SelectedIndex == 0)
                //{
                //    InterestedPartySelected();
                //}
                //else if (string.IsNullOrWhiteSpace(txtInterestedPartySearch.Text) && ddlIntPartyType.SelectedIndex > 0)
                //{
                //    DataTable dtFilterData = FilterData(dtInterestedPartyList, ddlIntPartyType.SelectedValue);
                //    BindGrid(dtFilterData);
                //    Session["ActiveIntPartyData"] = dtFilterData;
                //}
                //else
                //{
                //    BindGrid(dtInterestedPartyList);
                //}
                PopulateGrid();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting interested party type.", ex.Message);
            }
        }
        protected void lnkPage_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["ActiveIntPartyData"] == null)
                    return;

                DataTable dtActiveIntPartyData = Session["ActiveIntPartyData"] as DataTable;
                if (dtActiveIntPartyData.Rows.Count == 0)
                    return;

                int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
                hdnPageNumber.Value = Convert.ToString(pageIndex);
                Utilities.PopulateGridPage(pageIndex, dtActiveIntPartyData, gridDefaultPageSize, gvInterestedPartyDetails, dtEmpty, rptPager);
                hdnChangeNotSaved.Value = "N";
                UserAuthorization();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change.", ex.Message);
            }
        }
        protected void gvInterestedPartyDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                //if (Session["InterestedPartyTypeList"] == null)
                //{
                //    return;
                //}

                //DataTable dtInterestedPartyTypeList = Session["InterestedPartyTypeList"] as DataTable;
                //DropDownList ddlIntPartyTypeGrid;
                //if (e.Row.RowType == DataControlRowType.DataRow)
                //{
                //    ddlIntPartyTypeGrid = (e.Row.FindControl("ddlIntPartyTypeGrid") as DropDownList);
                //    string interestedPartyType = (e.Row.FindControl("lblInterestedPartyType") as Label).Text;

                //    ddlIntPartyTypeGrid.DataSource = dtInterestedPartyTypeList;
                //    ddlIntPartyTypeGrid.DataTextField = "dropdown_text";
                //    ddlIntPartyTypeGrid.DataValueField = "dropdown_value";
                //    ddlIntPartyTypeGrid.DataBind();
                //    ddlIntPartyTypeGrid.Items.Insert(0, new ListItem("-"));

                //    if (dtInterestedPartyTypeList.Select("dropdown_value = '" + interestedPartyType + "'").Length != 0)
                //    {
                //        ddlIntPartyTypeGrid.Items.FindByValue(interestedPartyType).Selected = true;
                //    }
                //    else
                //    {
                //        ddlIntPartyTypeGrid.SelectedIndex = 0;
                //    }
                //}
                CheckBox cbSendStatement;
                CheckBox cbGenerateInvoice;
                DropDownList ddlTaxType;
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    cbSendStatement = (e.Row.FindControl("cbSendStatement") as CheckBox);
                    cbGenerateInvoice = (e.Row.FindControl("cbGenerateInvoice") as CheckBox);
                    string isSendStatement = (e.Row.FindControl("hdnSendStatement") as HiddenField).Value;
                    string isGenerateInvoice = (e.Row.FindControl("hdnGenerateInvoice") as HiddenField).Value;
                    if (isSendStatement == "Y")
                    {
                        cbSendStatement.Checked = true;
                    }
                    if (isGenerateInvoice == "Y")
                    {
                        cbGenerateInvoice.Checked = true;
                    }

                    if ((e.Row.FindControl("lblInterestedPartyType") as Label).Text == "Payee")
                    {
                        (e.Row.FindControl("btnBankDetails") as Button).Visible = true;
                    }
                    else
                    {
                        (e.Row.FindControl("btnBankDetails") as Button).Visible = false;
                    }
                    //JIRA-1144 Changes -- Start
                    string hdnApplicableTax = (e.Row.FindControl("hdnApplicableTax") as HiddenField).Value;
                    ddlTaxType = (e.Row.FindControl("ddlTaxType") as DropDownList);

                    ddlTaxType.DataSource = (DataTable)Session["IntPartyTaxTypeList"];
                    ddlTaxType.DataTextField = "tax_type_desc";
                    ddlTaxType.DataValueField = "tax_type";
                    ddlTaxType.DataBind();
                    ddlTaxType.Items.Insert(0, new ListItem("-"));

                    if (ddlTaxType.Items.FindByValue(hdnApplicableTax) != null)
                    {
                        ddlTaxType.Items.FindByValue(hdnApplicableTax).Selected = true;
                    }
                    else
                    {
                        ddlTaxType.SelectedIndex = 0;
                    }

                    //JIRA-1144 Changes -- End
                }

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

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding data to grid.", ex.Message);
            }
        }


        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvInterestedPartyDetails_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["ActiveIntPartyData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                hdnPageNumber.Value = "1";
                Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), dataView.ToTable(), gridDefaultPageSize, gvInterestedPartyDetails, dtEmpty, rptPager);
                Session["ActiveIntPartyData"] = dataView.ToTable();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        protected void gvInterestedPartyDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                if (e.CommandName == "saverow")
                {
                    if (!string.IsNullOrEmpty(hdnGridRowSelectedPrvious.Value))
                    {
                        int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);
                        //int rowIndex = ((GridViewRow)((ImageButton)(e.CommandSource)).NamingContainer).RowIndex;
                        Int32 interestedPartyCode = Convert.ToInt32(((Label)gvInterestedPartyDetails.Rows[rowIndex].FindControl("lblInterestedPartyCode")).Text);
                        //string interestedPartyType = ((DropDownList)gvInterestedPartyDetails.Rows[rowIndex].FindControl("ddlIntPartyTypeGrid")).SelectedValue;
                        string interestedPartyName = ((TextBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("txtInterestedPartyName")).Text;
                        string interestedPartyAdd1 = ((TextBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("txtInterestedPartyAdd1")).Text;
                        string interestedPartyAdd2 = ((TextBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("txtInterestedPartyAdd2")).Text;
                        string interestedPartyAdd3 = ((TextBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("txtInterestedPartyAdd3")).Text;
                        string interestedPartyAdd4 = ((TextBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("txtInterestedPartyAdd4")).Text;
                        string interestedPartyPostcode = ((TextBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("txtInterestedPartyPostcode")).Text;
                        string interestedPartyEmail = ((TextBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("txtInterestedPartyEmail")).Text;
                        string taxNumber = ((TextBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("txtTaxNumber")).Text;
                        string taxType = ((DropDownList)gvInterestedPartyDetails.Rows[rowIndex].FindControl("ddlTaxType")).SelectedValue;
                        taxType = taxType == "-" ? string.Empty : taxType;
                        string isSendStmt;
                        if (((CheckBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("cbSendStatement")).Checked)
                        {
                            isSendStmt = "Y";
                        }
                        else
                        {
                            isSendStmt = "N";
                        }
                        string isGenerateInvoice;
                        if (((CheckBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("cbGenerateInvoice")).Checked)
                        {
                            isGenerateInvoice = "Y";
                        }
                        else
                        {
                            isGenerateInvoice = "N";
                        }

                        interestedPartyMaintenanceBL = new InterestedPartyMaintenanceBL();
                        DataSet intPartyData = interestedPartyMaintenanceBL.UpdateInterestedPartyData(interestedPartyCode, interestedPartyName, interestedPartyAdd1, interestedPartyAdd2, interestedPartyAdd3, interestedPartyAdd4, interestedPartyPostcode, interestedPartyEmail, taxNumber, taxType, isGenerateInvoice, isSendStmt, userCode, out errorId);
                        interestedPartyMaintenanceBL = null;

                        if (intPartyData.Tables.Count != 0 && errorId != 2)
                        {
                            Session["InterestedPartyList"] = intPartyData.Tables[0];
                            Session["ActiveIntPartyData"] = intPartyData.Tables[0];

                            /* Harish: 26-11-2018: Removed this as it doesn't seem right to exclude the search selection 
                             * on updating when only one row in the grid
                            if (gvInterestedPartyDetails.Rows.Count == 1)
                            {
                                ddlIntPartyType.SelectedIndex = 0;
                            }
                             * */

                            PopulateGrid();

                            hdnChangeNotSaved.Value = "N";
                            hdnGridRowSelectedPrvious.Value = null;
                            msgView.SetMessage("Interested party details updated successfully.", MessageType.Success, PositionType.Auto);
                        }
                        else
                        {
                            msgView.SetMessage("Failed to updated interested party details.", MessageType.Warning, PositionType.Auto);
                        }
                    }
                    else
                    {
                        msgView.SetMessage("No changes made to save.", MessageType.Success, PositionType.Auto);
                    }
                }
                //else if (e.CommandName == "deleterow")
                //{
                //    int rowIndex = ((GridViewRow)((ImageButton)(e.CommandSource)).NamingContainer).RowIndex;
                //    Int32 interestedPartyCode = Convert.ToInt32(((Label)gvInterestedPartyDetails.Rows[rowIndex].FindControl("lblInterestedPartyCode")).Text);
                //    interestedPartyMaintenanceBL = new InterestedPartyMaintenanceBL();
                //    DataSet linkedRoyaltorData = interestedPartyMaintenanceBL.GetLinkedRoyaltorDetails(interestedPartyCode, out errorId);
                //    //JIRA-908 Changes by Ravi in 13/02/2019 -- Start
                //    bool deletePartyCheck = true;
                //    if (linkedRoyaltorData.Tables.Count != 0 && errorId == 0)
                //    {
                //        if (linkedRoyaltorData.Tables[0].Rows.Count != 0)
                //        {
                //            msgView.SetMessage("Can't delete interested party as there is a linked Royaltor.", MessageType.Warning, PositionType.Auto);
                //            deletePartyCheck = false;
                //        }
                //    }
                //    if (deletePartyCheck)
                //    {
                //        hdninterestedPartyCode.Value = interestedPartyCode.ToString();
                //        mpeConfirmDelete.Show();
                //    }
                //    //JIRA-908 Changes by Ravi in 13/02/2019 -- End
                //}
                else if (e.CommandName == "cancelrow")
                {
                    if (Session["ActiveIntPartyData"] != null)
                    {
                        DataTable intPartyData = Session["ActiveIntPartyData"] as DataTable;

                        BindGrid(intPartyData);

                        hdnChangeNotSaved.Value = "N";
                        hdnGridRowSelectedPrvious.Value = null;
                    }
                }
                else if (e.CommandName == "linkedroyaltor")
                {
                    int rowIndex = ((GridViewRow)((Button)(e.CommandSource)).NamingContainer).RowIndex;
                    Int32 interestedPartyCode = Convert.ToInt32(((Label)gvInterestedPartyDetails.Rows[rowIndex].FindControl("lblInterestedPartyCode")).Text);

                    interestedPartyMaintenanceBL = new InterestedPartyMaintenanceBL();
                    DataSet linkedRoyaltorData = interestedPartyMaintenanceBL.GetLinkedRoyaltorDetails(interestedPartyCode, out errorId);
                    interestedPartyMaintenanceBL = null;

                    if (linkedRoyaltorData.Tables.Count != 0 && errorId == 0)
                    {
                        if (linkedRoyaltorData.Tables[0].Rows.Count == 0)
                        {
                            msgView.SetMessage("No linked Royaltors found.", MessageType.Warning, PositionType.Auto);
                        }
                        else
                        {
                            gvLinkedRoyaltors.DataSource = linkedRoyaltorData.Tables[0];
                            gvLinkedRoyaltors.DataBind();
                            mpeLinkedRoyaltors.Show();
                        }
                    }
                    else
                    {
                        ExceptionHandler("Error in displaying linked royaltor details.", "");
                    }
                }
                else if (e.CommandName == "bankdetails")
                {
                    int rowIndex = ((GridViewRow)((Button)(e.CommandSource)).NamingContainer).RowIndex;
                    Int32 interestedPartyCode = Convert.ToInt32(((Label)gvInterestedPartyDetails.Rows[rowIndex].FindControl("lblInterestedPartyCode")).Text);

                    //interestedPartyMaintenanceBL = new InterestedPartyMaintenanceBL();
                    //DataSet payeeBankData = interestedPartyMaintenanceBL.GetPayeeBankDetails(interestedPartyCode, out errorId);
                    //interestedPartyMaintenanceBL = null;

                    //if (payeeBankData.Tables.Count != 0 && errorId == 0)
                    //{
                    //    if (payeeBankData.Tables[0].Rows.Count == 0)
                    //    {
                    //        msgView.SetMessage("No Supplier Details found for Payee.", MessageType.Warning, PositionType.Auto);
                    //    }
                    //    else
                    //    {
                    //        txtVatNumber.Text = payeeBankData.Tables[0].Rows[0]["vat_number"].ToString();
                    //        txtSupplierNumber.Text = payeeBankData.Tables[0].Rows[0]["supplier_number"].ToString();
                    //        txtBankNameDom.Text = payeeBankData.Tables[0].Rows[0]["bank_name"].ToString();
                    //        txtBankAddressDom.Text = payeeBankData.Tables[0].Rows[0]["bank_address"].ToString();
                    //        txtAccountNameDom.Text = payeeBankData.Tables[0].Rows[0]["account_name"].ToString();
                    //        txtSortCodeDom.Text = payeeBankData.Tables[0].Rows[0]["sort_code"].ToString();
                    //        txtAccountNumberDom.Text = payeeBankData.Tables[0].Rows[0]["account_number"].ToString();
                    //        txtVendorSiteCodeDom.Text = payeeBankData.Tables[0].Rows[0]["vendor_site_code"].ToString();

                    //        txtAccountNameFrgn.Text = payeeBankData.Tables[0].Rows[0]["account_name"].ToString();
                    //        txtAccountNumberFrgn.Text = payeeBankData.Tables[0].Rows[0]["account_number"].ToString();
                    //        txtIBANFrgn.Text = payeeBankData.Tables[0].Rows[0]["iban"].ToString();
                    //        txtSwiftCodeFrgn.Text = payeeBankData.Tables[0].Rows[0]["swift_code"].ToString();
                    //        txtABARoutingFrgn.Text = payeeBankData.Tables[0].Rows[0]["aba_routing"].ToString();
                    //        txtBankAddressFrgn.Text = payeeBankData.Tables[0].Rows[0]["bank_address"].ToString();
                    //        txtVendorSiteCodeFrgn.Text = payeeBankData.Tables[0].Rows[0]["vendor_site_code"].ToString();

                    //        string  paymentType = payeeBankData.Tables[0].Rows[0]["payment_type"].ToString();
                    //        txtPaymentMethod.Text = payeeBankData.Tables[0].Rows[0]["payment_method_desc"].ToString();

                    //        if (paymentType == "Domestic")
                    //        {
                    //            trDomestic.Visible = true;
                    //            trForeign.Visible = false;
                    //        }
                    //        else
                    //        {
                    //            trDomestic.Visible = false;
                    //            trForeign.Visible = true;
                    //        }

                    //        mpePayeeBankDetails.Show();
                    //    }
                    //}
                    //else
                    //{
                    //    ExceptionHandler("Error in displaying linked royaltor details.", "");
                    //}

                    interestedPartyMaintenanceBL = new InterestedPartyMaintenanceBL();
                    DataSet payeeSuppliersData = interestedPartyMaintenanceBL.GetSupplierDetails(interestedPartyCode, out errorId);
                    interestedPartyMaintenanceBL = null;

                    if (payeeSuppliersData.Tables.Count != 0 && errorId == 0)
                    {
                        if (payeeSuppliersData.Tables[0].Rows.Count == 0)
                        {
                            msgView.SetMessage("No Supplier Details found for Payee.", MessageType.Warning, PositionType.Auto);
                        }
                        else
                        {
                            gvSupplierDetails.DataSource = payeeSuppliersData.Tables[0];
                            gvSupplierDetails.DataBind();
                            mpeSuppliers.Show();
                        }
                    }
                    else
                    {
                        ExceptionHandler("Error in displaying supplier details.", "");
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving/deleting interested party data.", ex.Message);
            }
        }

        protected void imgBtnInsert_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                string isSendStmt;
                if (cbSendStmtInsert.Checked)
                {
                    isSendStmt = "Y";
                }
                else
                {
                    isSendStmt = "N";
                }
                string isGenerateInvoice;
                if (cbGenerateInvoiceInsert.Checked)
                {
                    isGenerateInvoice = "Y";
                }
                else
                {
                    isGenerateInvoice = "N";
                }

                interestedPartyMaintenanceBL = new InterestedPartyMaintenanceBL();
                DataSet intPartyData = interestedPartyMaintenanceBL.InsertInterestedPartyData(ddlIntPartyTypeInsert.SelectedValue, txtInterestedPartyName.Text, txtAddress1.Text,
                    txtAddress2.Text, txtAddress3.Text, txtAddress4.Text, txtPostcode.Text, txtEmail.Text, txtTaxNumber.Text, ddlTaxType.SelectedValue == "-" ? string.Empty : ddlTaxType.SelectedValue, isGenerateInvoice, isSendStmt, userCode, out errorId);
                interestedPartyMaintenanceBL = null;
                if (intPartyData.Tables.Count != 0 && errorId != 2)
                {
                    Session["InterestedPartyList"] = intPartyData.Tables[0];
                    Session["ActiveIntPartyData"] = intPartyData.Tables[0];
                    //gvInterestedPartyDetails.PageIndex = Int32.MaxValue;
                    BindGrid(intPartyData.Tables[0]);
                    txtInterestedPartySearch.Text = string.Empty;
                    txtInterestedPartyName.Text = string.Empty;
                    txtAddress1.Text = string.Empty;
                    txtAddress2.Text = string.Empty;
                    txtAddress3.Text = string.Empty;
                    txtAddress4.Text = string.Empty;
                    txtPostcode.Text = string.Empty;
                    txtEmail.Text = string.Empty;
                    txtTaxNumber.Text = string.Empty;
                    ddlTaxType.SelectedIndex = 0;  //JIRA-1144 CHanges
                    cbGenerateInvoiceInsert.Checked = false;
                    cbSendStmtInsert.Checked = false;
                    ddlIntPartyTypeInsert.SelectedIndex = 0;
                    ddlIntPartyType.SelectedIndex = 0;
                    hdnChangeNotSaved.Value = "N";
                    hdnInsertDataNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                    msgView.SetMessage("Interested party created successfully.", MessageType.Success, PositionType.Auto);
                }
                else
                {
                    msgView.SetMessage("Failed to create interested party.", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in creating interested party.", ex.Message);
            }
        }

        protected void imgBtnCancel_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtInterestedPartyName.Text = string.Empty;
                txtAddress1.Text = string.Empty;
                txtAddress2.Text = string.Empty;
                txtAddress3.Text = string.Empty;
                txtAddress4.Text = string.Empty;
                ddlIntPartyTypeInsert.SelectedIndex = 0;
                txtPostcode.Text = string.Empty;
                txtEmail.Text = string.Empty;
                txtTaxNumber.Text = string.Empty;
                ddlTaxType.SelectedIndex = 0; //JIRA-1144 CHanges
                cbGenerateInvoiceInsert.Checked = false;
                cbSendStmtInsert.Checked = false;
                hdnInsertDataNotSaved.Value = "N";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in clearing interested party data.", ex.Message);
            }
        }

        //protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
        //        {
        //            txtInterestedPartySearch.Text = string.Empty;
        //            return;
        //        }

        //        txtInterestedPartySearch.Text = lbFuzzySearch.SelectedValue.ToString();
        //        //InterestedPartySelected();
        //        PopulateGrid();
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandler("Error in selecting Interested party.", ex.Message);
        //    }
        //}

        //protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        //{
        //    try
        //    {
        //        txtInterestedPartySearch.Text = string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandler("Error in closing search list.", ex.Message);
        //    }
        //}

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnInsertDataNotSaved.Value == "Y")
                {
                    //Validate
                    Page.Validate("valInsertInterestedParty");
                    if (!Page.IsValid)
                    {
                        mpeSaveUndo.Hide();
                        msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                        return;
                    }

                    string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                    string isSendStmt;
                    if (cbSendStmtInsert.Checked)
                    {
                        isSendStmt = "Y";
                    }
                    else
                    {
                        isSendStmt = "N";
                    }
                    string isGenerateInvoice;
                    if (cbGenerateInvoiceInsert.Checked)
                    {
                        isGenerateInvoice = "Y";
                    }
                    else
                    {
                        isGenerateInvoice = "N";
                    }

                    interestedPartyMaintenanceBL = new InterestedPartyMaintenanceBL();
                    DataSet intPartyData = interestedPartyMaintenanceBL.InsertInterestedPartyData(ddlIntPartyTypeInsert.SelectedValue, txtInterestedPartyName.Text, txtAddress1.Text,
                        txtAddress2.Text, txtAddress3.Text, txtAddress4.Text, txtPostcode.Text, txtEmail.Text, txtTaxNumber.Text, ddlTaxType.SelectedValue == "-" ? string.Empty : ddlTaxType.SelectedValue, isGenerateInvoice, isSendStmt, userCode, out errorId);
                    interestedPartyMaintenanceBL = null;
                    if (intPartyData.Tables.Count != 0 && errorId != 2)
                    {
                        Session["InterestedPartyList"] = intPartyData.Tables[0];
                        Session["ActiveIntPartyData"] = intPartyData.Tables[0];
                        //gvInterestedPartyDetails.PageIndex = Int32.MaxValue;
                        BindGrid(intPartyData.Tables[0]);
                        txtInterestedPartySearch.Text = string.Empty;
                        txtInterestedPartyName.Text = string.Empty;
                        txtAddress1.Text = string.Empty;
                        txtAddress2.Text = string.Empty;
                        txtAddress3.Text = string.Empty;
                        txtAddress4.Text = string.Empty;
                        txtPostcode.Text = string.Empty;
                        txtEmail.Text = string.Empty;
                        txtTaxNumber.Text = string.Empty;
                        ddlTaxType.SelectedIndex = 0; //JIRA-1144 CHanges
                        cbGenerateInvoiceInsert.Checked = false;
                        cbSendStmtInsert.Checked = false;
                        ddlIntPartyTypeInsert.SelectedIndex = 0;
                        ddlIntPartyType.SelectedIndex = 0;
                        hdnChangeNotSaved.Value = "N";
                        hdnInsertDataNotSaved.Value = "N";
                        hdnGridRowSelectedPrvious.Value = null;
                        msgView.SetMessage("Interested party created successfully.", MessageType.Success, PositionType.Auto);
                    }
                    else
                    {
                        msgView.SetMessage("Failed to create interested party.", MessageType.Warning, PositionType.Auto);
                    }
                }
                else if (hdnChangeNotSaved.Value == "Y")
                {
                    int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);

                    //Calculate the rowindex for validation 
                    int rowIndexValidation = (gvInterestedPartyDetails.PageIndex * gvInterestedPartyDetails.PageSize) + rowIndex;

                    //Validate
                    Page.Validate("GroupUpdate_" + rowIndexValidation + "");
                    if (!Page.IsValid)
                    {
                        mpeSaveUndo.Hide();
                        msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                        return;
                    }

                    string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                    Int32 interestedPartyCode = Convert.ToInt32(((Label)gvInterestedPartyDetails.Rows[rowIndex].FindControl("lblInterestedPartyCode")).Text);
                    //string interestedPartyType = ((DropDownList)gvInterestedPartyDetails.Rows[rowIndex].FindControl("ddlIntPartyTypeGrid")).SelectedValue;
                    string interestedPartyName = ((TextBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("txtInterestedPartyName")).Text;
                    string interestedPartyAdd1 = ((TextBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("txtInterestedPartyAdd1")).Text;
                    string interestedPartyAdd2 = ((TextBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("txtInterestedPartyAdd2")).Text;
                    string interestedPartyAdd3 = ((TextBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("txtInterestedPartyAdd3")).Text;
                    string interestedPartyAdd4 = ((TextBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("txtInterestedPartyAdd4")).Text;
                    string interestedPartyPostcode = ((TextBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("txtInterestedPartyPostcode")).Text;
                    string interestedPartyEmail = ((TextBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("txtInterestedPartyEmail")).Text;
                    string taxNumber = ((TextBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("txtTaxNumber")).Text;
                    string taxType = ((DropDownList)gvInterestedPartyDetails.Rows[rowIndex].FindControl("ddlTaxType")).SelectedValue;
                    taxType = taxType == "-" ? string.Empty : taxType;
                    string isSendStmt;
                    if (((CheckBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("cbSendStatement")).Checked)
                    {
                        isSendStmt = "Y";
                    }
                    else
                    {
                        isSendStmt = "N";
                    }

                    string isGenerateInvoice;
                    if (((CheckBox)gvInterestedPartyDetails.Rows[rowIndex].FindControl("cbGenerateInvoice")).Checked)
                    {
                        isGenerateInvoice = "Y";
                    }
                    else
                    {
                        isGenerateInvoice = "N";
                    }


                    interestedPartyMaintenanceBL = new InterestedPartyMaintenanceBL();
                    DataSet intPartyData = interestedPartyMaintenanceBL.UpdateInterestedPartyData(interestedPartyCode, interestedPartyName, interestedPartyAdd1, interestedPartyAdd2, interestedPartyAdd3, interestedPartyAdd4, interestedPartyPostcode, interestedPartyEmail, taxNumber, taxType, isGenerateInvoice, isSendStmt, userCode, out errorId);
                    interestedPartyMaintenanceBL = null;

                    if (intPartyData.Tables.Count != 0 && errorId != 2)
                    {
                        Session["InterestedPartyList"] = intPartyData.Tables[0];
                        Session["ActiveIntPartyData"] = intPartyData.Tables[0];

                        PopulateGrid();
                        //check if there is only one row in the grid before binding updated data.
                        //if count is 1 then only display that row
                        //if (gvInterestedPartyDetails.Rows.Count == 1)
                        //{
                        //    DataTable dtSearched = intPartyData.Tables[0].Clone();
                        //    DataRow[] foundRows = intPartyData.Tables[0].Select("int_party_id = '" + interestedPartyCode + "'");
                        //    if (foundRows.Length != 0)
                        //    {
                        //        dtSearched = foundRows.CopyToDataTable();
                        //        BindGrid(dtSearched);
                        //        Session["ActiveIntPartyData"] = dtSearched;
                        //    }
                        //}
                        //else if (ddlIntPartyType.SelectedIndex > 0)
                        //{
                        //    DataTable dtFilterData = FilterData(intPartyData.Tables[0], ddlIntPartyType.SelectedValue);
                        //    BindGrid(dtFilterData);
                        //    Session["ActiveIntPartyData"] = dtFilterData;
                        //}
                        //else
                        //{
                        //    BindGrid(intPartyData.Tables[0]);
                        //}
                        hdnChangeNotSaved.Value = "N";
                        hdnGridRowSelectedPrvious.Value = null;
                        msgView.SetMessage("Interested party details updated successfully.", MessageType.Success, PositionType.Auto);
                    }
                    else
                    {
                        msgView.SetMessage("Failed to updated interested party details.", MessageType.Warning, PositionType.Auto);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving interested party data.", ex.Message);
            }
        }

        protected void btnUndoChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnInsertDataNotSaved.Value == "Y")
                {
                    txtInterestedPartyName.Text = string.Empty;
                    txtAddress1.Text = string.Empty;
                    txtAddress2.Text = string.Empty;
                    txtAddress3.Text = string.Empty;
                    txtAddress4.Text = string.Empty;
                    ddlIntPartyTypeInsert.SelectedIndex = 0;
                    txtPostcode.Text = string.Empty;
                    txtEmail.Text = string.Empty;
                    txtTaxNumber.Text = string.Empty;
                    ddlTaxType.SelectedIndex = 0; //JIRA-1144 Changes
                    cbGenerateInvoiceInsert.Checked = false;
                    cbSendStmtInsert.Checked = false;
                    hdnInsertDataNotSaved.Value = "N";
                }
                else if (hdnChangeNotSaved.Value == "Y")
                {
                    //if (Session["ActiveIntPartyData"] != null)
                    //{
                    //DataTable intPartyData = Session["ActiveIntPartyData"] as DataTable;

                    //BindGrid(intPartyData);

                    PopulateGrid();
                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                    //}
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading grid data.", ex.Message);
            }
        }

        protected void btnHdnInterestedPartySearch_Click(object sender, EventArgs e)
        {
            try
            {
                hdnPageNumber.Value = "1";
                //InterestedPartySelected();
                PopulateGrid();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting interested party.", ex.Message);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;
                txtInterestedPartySearch.Text = string.Empty;
                txtInterestedPartyName.Text = string.Empty;
                txtIpNumber.Text = string.Empty;
                txtAddress1.Text = string.Empty;
                txtAddress2.Text = string.Empty;
                txtAddress3.Text = string.Empty;
                txtAddress4.Text = string.Empty;
                ddlIntPartyType.SelectedIndex = 0;
                Session["IntPartyMaintFilters"] = null;
                ddlIntPartyTypeInsert.SelectedIndex = 0;
                hdnPageNumber.Value = "1";
                if (Session["InterestedPartyList"] != null)
                {
                    DataTable dtInterestedPartyList = Session["InterestedPartyList"] as DataTable;
                    Session["ActiveIntPartyData"] = dtInterestedPartyList;
                    if (hdnIntPartyType.Value != "")
                    {
                        DataTable dtFilaterData = FilterData(dtInterestedPartyList, hdnIntPartyType.Value);
                        BindGrid(dtFilaterData);
                        ddlIntPartyType.ClearSelection();
                        ddlIntPartyType.Items.FindByValue(hdnIntPartyType.Value).Selected = true;
                        Session["ActiveIntPartyData"] = dtFilaterData;
                    }
                    else
                    {
                        BindGrid(dtInterestedPartyList);
                    }
                    hdnChangeNotSaved.Value = "N";
                    hdnInsertDataNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                }


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in reset.", ex.Message);
            }
        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {
            try
            {
                IntPartyAuditSession();

                if (gvInterestedPartyDetails.Rows.Count == 1)
                {
                    string IntPartyId = (gvInterestedPartyDetails.Rows.Count > 1) ? string.Empty : ((Label)gvInterestedPartyDetails.Rows[0].FindControl("lblInterestedPartyCode")).Text;
                    Response.Redirect("../Audit/InterestedPartyAudit.aspx?interestedPartyData=" + IntPartyId + "", false);
                }
                else
                {
                    Response.Redirect("../Audit/InterestedPartyAudit.aspx", false);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting.", ex.Message);
            }
        }

        protected void btnPayeeDetails_Click(object sender, EventArgs e)
        {
            msgView.SetMessage("screen to be developed.", MessageType.Success, PositionType.Auto);
        }

        protected void btnCourtesyDetails_Click(object sender, EventArgs e)
        {
            msgView.SetMessage("screen to be developed.", MessageType.Success, PositionType.Auto);
        }

        //JIRA-908 Changes by Ravi on 13/02/2019 -- Start
        protected void btnYes_Click(object sender, EventArgs e)
        {

            try
            {
                Int32 interestedPartyCode = Convert.ToInt32(hdninterestedPartyCode.Value);
                interestedPartyMaintenanceBL = new InterestedPartyMaintenanceBL();
                DataSet linkedRoyaltorData = interestedPartyMaintenanceBL.GetLinkedRoyaltorDetails(interestedPartyCode, out errorId);
                if (linkedRoyaltorData.Tables.Count != 0 && errorId == 0)
                {
                    if (linkedRoyaltorData.Tables[0].Rows.Count != 0)
                    {
                        msgView.SetMessage("Can't delete interested party as there is a linked Royaltor.", MessageType.Warning, PositionType.Auto);
                    }
                }
                interestedPartyMaintenanceBL = new InterestedPartyMaintenanceBL();
                DataSet intPartyData = interestedPartyMaintenanceBL.DeleteInterestedPartyData(interestedPartyCode, out errorId);
                interestedPartyMaintenanceBL = null;

                if (errorId == 1)
                {
                    msgView.SetMessage("Can't delete interested party as it's being used in contract.", MessageType.Success, PositionType.Auto);
                }
                else if (intPartyData.Tables.Count != 0 && errorId == 0)
                {
                    Session["InterestedPartyList"] = intPartyData.Tables[0];
                    Session["ActiveIntPartyData"] = intPartyData.Tables[0];

                    if (gvInterestedPartyDetails.Rows.Count == 1)
                    {
                        ddlIntPartyType.SelectedIndex = 0;
                    }

                    PopulateGrid();
                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                    msgView.SetMessage("Interested party deleted successfully.", MessageType.Success, PositionType.Auto);
                }
                else
                {
                    msgView.SetMessage("Failed to delete interested party.", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Failed to delete interested party.", ex.Message);
            }

        }
        //JIRA-908 Changes by Ravi on 13/02/2019 -- End


        #endregion Events

        #region Methods

        private void LoadData()
        {
            interestedPartyMaintenanceBL = new InterestedPartyMaintenanceBL();
            DataSet initialData = interestedPartyMaintenanceBL.GetInitialData(out errorId);
            interestedPartyMaintenanceBL = null;

            if (initialData.Tables.Count != 0 && errorId != 2)
            {
                hdnPageNumber.Value = "1";
                Session["InterestedPartyList"] = initialData.Tables[0];
                Session["ActiveIntPartyData"] = initialData.Tables[0];
                Session["InterestedPartyTypeList"] = initialData.Tables[1];
                Session["IntPartyTaxTypeList"] = initialData.Tables[2];

                ddlIntPartyType.DataSource = initialData.Tables[1];
                ddlIntPartyType.DataTextField = "dropdown_text";
                ddlIntPartyType.DataValueField = "dropdown_value";
                ddlIntPartyType.DataBind();
                ddlIntPartyType.Items.Insert(0, new ListItem("-"));

                ddlIntPartyTypeInsert.DataSource = initialData.Tables[1];
                ddlIntPartyTypeInsert.DataTextField = "dropdown_text";
                ddlIntPartyTypeInsert.DataValueField = "dropdown_value";
                ddlIntPartyTypeInsert.DataBind();
                ddlIntPartyTypeInsert.Items.Insert(0, new ListItem("-"));

                //JIRA-1144 Changes -- Start
                ddlTaxType.DataSource = initialData.Tables[2]; ;
                ddlTaxType.DataTextField = "tax_type_desc";
                ddlTaxType.DataValueField = "tax_type";
                ddlTaxType.DataBind();
                ddlTaxType.Items.Insert(0, new ListItem("-"));
                //JIRA-1144 Changes -- End

                if (Request.QueryString != null && Request.QueryString.Count > 0)
                {
                    string intPartyType = Request.QueryString[0];
                    if (intPartyType == "P")
                    {
                        DataTable dtFilterData = FilterData(initialData.Tables[0], intPartyType);
                        Session["ActiveIntPartyData"] = dtFilterData;
                        BindGrid(dtFilterData);
                        btnPayeeDetails.Visible = true;
                        btnCourtesyDetails.Visible = false;
                        ddlIntPartyType.Items.FindByValue(intPartyType).Selected = true;
                        hdnIntPartyType.Value = intPartyType;
                    }
                    else if (intPartyType == "C")
                    {
                        DataTable dtFilterData = FilterData(initialData.Tables[0], intPartyType);
                        Session["ActiveIntPartyData"] = dtFilterData;
                        BindGrid(dtFilterData);
                        btnPayeeDetails.Visible = false;
                        btnCourtesyDetails.Visible = true;
                        ddlIntPartyType.Items.FindByValue(intPartyType).Selected = true;
                        hdnIntPartyType.Value = intPartyType;
                    }
                    else
                    {
                        BindGrid(initialData.Tables[0]);
                    }

                    //load grid as per the search criteria held. This to be done only when navigating from Audit screen
                    if (Request.QueryString["fromAudit"] == "Y")
                    {
                        if (Session["IntPartyMaintFilters"] != null)
                        {
                            DataTable dtSearchedFilters = Session["IntPartyMaintFilters"] as DataTable;
                            foreach (DataRow dRow in dtSearchedFilters.Rows)
                            {
                                if (dRow["filter_name"].ToString() == "txtInterestedPartySearch")
                                {
                                    txtInterestedPartySearch.Text = dRow["filter_value"].ToString();
                                }
                                else if (dRow["filter_name"].ToString() == "ddlIntPartyType")
                                {
                                    ddlIntPartyType.SelectedValue = dRow["filter_value"].ToString();
                                }
                            }
                            PopulateGrid();
                        }
                    }

                }
                else
                {
                    BindGrid(initialData.Tables[0]);
                }
            }
            else
            {
                ExceptionHandler("Error in fetching data", string.Empty);
            }

        }

        private DataTable FilterData(DataTable acvtiveData, string intPartyType)
        {
            DataTable dtSearched = acvtiveData.Clone();

            DataRow[] foundRows = acvtiveData.Select("int_party_type = '" + intPartyType + "'");
            if (foundRows.Length != 0)
            {
                dtSearched = foundRows.CopyToDataTable();
            }

            return dtSearched;
        }

        private void BindGrid(DataTable gridData)
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            DataView dv = gridData.DefaultView;
            dv.Sort = "int_party_type asc, int_party_name asc";
            gridData = dv.ToTable();
            Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), gridData, gridDefaultPageSize, gvInterestedPartyDetails, dtEmpty, rptPager);
            UserAuthorization();
        }

        private void IntPartyAuditSession()
        {
            //Create a table to hold the filter values
            DataTable dtSearchedFilters = new DataTable();
            dtSearchedFilters.Columns.Add("filter_name", typeof(string));
            dtSearchedFilters.Columns.Add("filter_value", typeof(string));

            //Add the filter values to the above created table
            dtSearchedFilters.Rows.Add("txtInterestedPartySearch", txtInterestedPartySearch.Text.Trim());
            dtSearchedFilters.Rows.Add("ddlIntPartyType", ddlIntPartyType.SelectedValue);

            Session["IntPartyMaintFilters"] = dtSearchedFilters;
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);
        }

        private void PopulateGrid()
        {
            if (Session["InterestedPartyList"] != null)
            {
                DataTable dtInterestedPartyList = Session["InterestedPartyList"] as DataTable;
                DataTable dtSearched = dtInterestedPartyList.Clone();
                string searchQuery = "1=1";

                if (!string.IsNullOrWhiteSpace(txtInterestedPartySearch.Text))
                {
                    //JIRA-1048 Changes to handle single quote --Start
                    searchQuery = searchQuery + " AND (int_party_name like '%" + txtInterestedPartySearch.Text.Replace("'", "").Trim().ToUpper() + "%' OR Convert(int_party_id,System.String) like '%" +
                                        txtInterestedPartySearch.Text.Replace("'", "").Trim().ToUpper() + "%')";
                }

                if (ddlIntPartyType.SelectedIndex > 0)
                {
                    searchQuery = searchQuery + " AND int_party_type = '" + ddlIntPartyType.SelectedValue + "'";
                }

                if (!string.IsNullOrWhiteSpace(txtIpNumber.Text))
                {
                    searchQuery = searchQuery + " AND ip_number like '%" + txtIpNumber.Text.Replace("'", "").Trim().ToUpper() + "%'";
                }
                //JIRA-1048 Changes to handle single quote --End
                DataRow[] foundRows = dtInterestedPartyList.Select(searchQuery);
                if (foundRows.Length != 0)
                {
                    dtSearched = foundRows.CopyToDataTable();
                }

                Session["ActiveIntPartyData"] = dtSearched;
                BindGrid(dtSearched);
            }
        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                imgBtnInsert.Enabled = false;
                imgBtnCancel.Enabled = false;
                btnSaveChanges.Enabled = false;
                //disable grid buttons
                foreach (GridViewRow rows in gvInterestedPartyDetails.Rows)
                {
                    (rows.FindControl("imgBtnSave") as ImageButton).Enabled = false;
                    (rows.FindControl("imgBtnUndo") as ImageButton).Enabled = false;
                    (rows.FindControl("imgBtnDelete") as ImageButton).Enabled = false;
                }
            }
        }


        #endregion Methods

    }
}
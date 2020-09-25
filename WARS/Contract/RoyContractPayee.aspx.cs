/*
File Name   :   RoyContractPayee.cs
Purpose     :   to add/edit payee details of royaltor contract

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     03-Apr-2017     Harish(Infosys Limited)   Initial Creation
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
using System.Text.RegularExpressions;

namespace WARS.Contract
{
    public partial class RoyContractPayee : System.Web.UI.Page
    {
        #region Global Declarations
        RoyContractPayeeBL royContractPayeeBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        DataTable dtPayeeType;
        string loggedUserID;
        string royaltorId = string.Empty;
        string isNewRoyaltor = string.Empty;
        #endregion Global Declarations

        #region Sorting
        string ascending = "Asc";
        string descending = "Desc";
        string sort_Up = "~/Images/Sort_up.png";
        string sort_Down = "~/Images/Sort_down.png";
        #endregion Sorting

        #region EVENTS
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                royaltorId = Request.QueryString["RoyaltorId"];
                isNewRoyaltor = Request.QueryString["isNewRoyaltor"];

                if (royaltorId == null || isNewRoyaltor == null)
                {
                    msgView.SetMessage("Not a valid royaltor!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Contract - Payee Details";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Contract - Payee Details";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        //HtmlTableRow trPayeeDetails = (HtmlTableRow)contractNavigationButtons.FindControl("trPayeeDetails");
                        //trPayeeDetails.Visible = false;
                        Button btnPayee = (Button)contractNavigationButtons.FindControl("btnPayee");
                        btnPayee.Enabled = false;
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

                            //contractNavigationButtons.Disable();
                            contractNavigationButtons.EnableNewRoyNavButtons(ContractScreens.PayeeDetails.ToString());

                        }

                        txtRoyaltorId.Text = royaltorId;
                        LoadPayeeData();

                        //WUIN-450 - If a new Royaltor is set up with Lock checked it should continue to allow entry of the details (the Royaltor set up needs to be completed!)
                        if (isNewRoyaltor != "Y" && contractNavigationButtons.IsRoyaltorLocked(royaltorId))
                        {
                            btnSave.ToolTip = "Royaltor Locked";
                        }

                        //WUIN-599 -- Only one user can use contract screens at the same time.
                        // If a contract is already using by another user then making the screen readonly.
                        if (isNewRoyaltor != "Y" && contractNavigationButtons.IsScreenLocked(royaltorId))
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

        protected void gvContPayee_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string intPartyType = (e.Row.FindControl("hdnIntPartyType") as HiddenField).Value;
                    string primaryPayee = (e.Row.FindControl("hdnPrimaryPayee") as HiddenField).Value;
                    string pay = (e.Row.FindControl("hdnPay") as HiddenField).Value;
                    string generateInvoice = (e.Row.FindControl("hdnGenerateInvoice") as HiddenField).Value;
                    string isModified = (e.Row.FindControl("hdnIsModified") as HiddenField).Value;
                    CheckBox cbPay = (e.Row.FindControl("cbPay") as CheckBox);
                    CheckBox cbPrimaryPayee = (e.Row.FindControl("cbPrimaryPayee") as CheckBox);
                    CheckBox cbGenerateInvoice = (e.Row.FindControl("cbGenerateInvoice") as CheckBox);//WUIN-1164
                    ImageButton imgBtnDelete = (e.Row.FindControl("imgBtnDelete") as ImageButton);
                    ImageButton imgBtnUndo = (e.Row.FindControl("imgBtnUndo") as ImageButton);
                    DropDownList ddlTaxType = (e.Row.FindControl("ddlTaxType") as DropDownList);
                    string hdnTaxType = (e.Row.FindControl("hdnTaxType") as HiddenField).Value;

                    if (intPartyType == "P")
                    {
                        DataTable dtTaxType = (DataTable)Session["RoyContPayeeTaxType"];
                        ddlTaxType.DataSource = dtTaxType;
                        ddlTaxType.DataTextField = "tax_type_desc";
                        ddlTaxType.DataValueField = "tax_type";
                        ddlTaxType.DataBind();
                        ddlTaxType.Items.Insert(0, new ListItem("-"));
                        ddlTaxType.SelectedValue = hdnTaxType == "" ? "-" : hdnTaxType;
                    }

                    if (intPartyType == "C")
                    {
                        (e.Row.FindControl("txtPercentShare") as TextBox).Visible = false;
                        (e.Row.FindControl("valtxtPercentShare") as CustomValidator).Visible = false;
                        (e.Row.FindControl("txtTaxNumber") as TextBox).Visible = false;
                        ddlTaxType.Visible = false;
                        cbPay.Visible = false;
                        cbPrimaryPayee.Visible = false;
                        imgBtnUndo.Visible = false;
                        cbGenerateInvoice.Visible = false;
                        return;
                    }

                    if (primaryPayee == "Y")
                    {
                        cbPrimaryPayee.Checked = true;
                    }
                    else
                    {
                        cbPrimaryPayee.Checked = false;
                    }

                    if (pay == "Y")
                    {
                        cbPay.Checked = true;
                    }
                    else
                    {
                        cbPay.Checked = false;
                    }
                    //WUIN-1164
                    if (generateInvoice == "Y")
                    {
                        cbGenerateInvoice.Checked = true;
                    }
                    else
                    {
                        cbGenerateInvoice.Checked = false;
                    }
                }

                //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    foreach (TableCell tc in e.Row.Cells)
                    {
                        if (tc.HasControls() && tc.Controls.Count == 1)
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
                ExceptionHandler("Error in binding data to grid", ex.Message);
            }
        }

        protected void gvContPayee_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "cancelRow")
                {
                    GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                    string intPartyId = (row.FindControl("hdnIntPartyId") as HiddenField).Value;
                    string intPartyType = (row.FindControl("hdnIntPartyType") as HiddenField).Value;
                    string isModified = (row.FindControl("hdnIsModified") as HiddenField).Value;
                    RowCancelChanges(intPartyId);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in cancelling grid data", ex.Message);
            }
        }

        protected void gvIntPartySearchList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            string intPartyId = (row.FindControl("hdnIntPartyId") as HiddenField).Value;
            string intPartyType = (row.FindControl("hdnIntPartyType") as HiddenField).Value;
            string email = (row.FindControl("hdnEmail") as HiddenField).Value;
            string TaxNumber = (row.FindControl("hdnTaxNum") as HiddenField).Value;
            string generateInvoice = (row.FindControl("hdnGenerateInv") as HiddenField).Value;
            string intPartyName = (row.FindControl("lblIntPartyName") as Label).Text;
            string address1 = (row.FindControl("lblAddress1") as Label).Text;
            string address2 = (row.FindControl("lblAddress2") as Label).Text;
            string address3 = (row.FindControl("lblAddress3") as Label).Text;
            string address4 = (row.FindControl("lblAddress4") as Label).Text;
            string applicableTax = (row.FindControl("hdnTaxType") as HiddenField).Value;

            if (e.CommandName == "dblClk")
            {
                hdnIntPartyIdAddRow.Value = intPartyId;
                ddlPayeeTypeAdd.SelectedValue = intPartyType;
                txtIntPartyAddRow.Text = intPartyName;
                lblAddress1AddRow.Text = address1;
                lblAddress2AddRow.Text = address2;
                lblAddress3AddRow.Text = address3;
                lblAddress4AddRow.Text = address4;
                lblEmailAddRow.Text = email;
                txtTaxNumAddRow.Text = TaxNumber;
                ddlTaxTypeAddRow.SelectedValue = applicableTax == "" ? "-" : applicableTax;
                if (intPartyType == "P")
                {
                    txtShareAddRow.Enabled = true;
                    cbPrimaryAddRow.Enabled = true;
                    cbPayAddRow.Enabled = true;
                    if (generateInvoice == "Y") //WUIN-1223
                    {
                        cbGenerateInvoiceAddRow.Checked = true;
                    }
                    else
                    {
                        cbGenerateInvoiceAddRow.Checked = false;
                    }
                }
                else
                {
                    txtShareAddRow.Text = string.Empty;
                    cbPayAddRow.Checked = false;
                    txtTaxNumAddRow.Enabled = false;
                    ddlTaxTypeAddRow.Enabled = false;
                    txtShareAddRow.Enabled = false;
                    cbPrimaryAddRow.Enabled = false;
                    cbPayAddRow.Enabled = false;
                    cbGenerateInvoiceAddRow.Checked = false;
                    cbGenerateInvoiceAddRow.Enabled = false; //WUIN-1164
                }

                ddlPayeeTypeAdd.Visible = false;//to handle disabled dropdown text not clearly visible case
                txtPayeeTypeAdd.Visible = true;
                txtPayeeTypeAdd.Text = ddlPayeeTypeAdd.SelectedItem.Text;
                txtIntPartyAddRow.ReadOnly = true;
                txtIntPartyAddRow.CssClass = "textboxStyle_readonly";
                txtTaxNumAddRow.Focus(); // JIRA -1027 Tab to focus on Tax Number after selecting Interested Party                
            }
        }

        protected void gvIntPartySearchList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[5].Visible = false;
                LinkButton _dblClickButton = e.Row.FindControl("lnkBtnDblClk") as LinkButton;
                string _jsDoubleClick = ClientScript.GetPostBackClientHyperlink(_dblClickButton, "");
                e.Row.Attributes.Add("ondblclick", _jsDoubleClick);
            }
        }

        protected void btnAppendAddRow_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                {
                    return;
                }

                if (hdnIntPartyIdAddRow.Value == null || hdnIntPartyIdAddRow.Value == string.Empty)
                {
                    rfvtxtIntPartyAddRow.IsValid = false;
                    rfvtxtIntPartyAddRow.ToolTip = "Please select Name from the interested party list";
                    msgView.SetMessage("Payee details not moved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (!ValidateAppendRow())
                {
                    msgView.SetMessage("Selected payee already exists!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                AppendRowToGrid();
                ClearAddRow();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding payee row to grid", ex.Message);
            }
        }

        protected void btnIntPartyPopup_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlPayeeTypeAdd.SelectedIndex == 0)
                {
                    msgView.SetMessage("Please select Payee or Courtesy", MessageType.Warning, PositionType.Auto);
                    return;
                }

                string intPartyIds = GetExcludeIntPartyList();
                royContractPayeeBL = new RoyContractPayeeBL();
                DataSet intPartyList = royContractPayeeBL.GetIntPartySearchList(ddlPayeeTypeAdd.SelectedValue, txtIntPartyAddRow.Text.Trim(), intPartyIds, out errorId);
                royContractPayeeBL = null;

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
                    ExceptionHandler("Error in loading interested party search data", string.Empty);
                }

                mpeIntPartySearch.Show();
            }
            catch (Exception ex)
            {
                mpeIntPartySearch.Hide();
                ExceptionHandler("Error in loading interested party search data", ex.Message);
            }
        }

        protected void btnUndoAddRow_Click(object sender, ImageClickEventArgs e)
        {
            ClearAddRow();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Array payeeList = PayeeDetailsList();
                List<string> deleteList = new List<string>();
                if (ViewState["vsDeleteIds"] != null)
                {
                    deleteList = (List<string>)ViewState["vsDeleteIds"];
                }

                //check if any changes to save
                if (payeeList.Length == 0 && deleteList.Count == 0)
                {
                    if (isNewRoyaltor == "N")
                    {
                        msgView.SetMessage("No changes made to save!", MessageType.Warning, PositionType.Auto);
                    }
                    else if (isNewRoyaltor == "Y")
                    {
                        if (gvContPayee.Rows.Count == 0)
                        {
                            msgView.SetMessage("Must be at least one payee!", MessageType.Warning, PositionType.Auto);
                        }
                        else
                        {
                            //WUIN-450
                            //set screen button enabled = Y
                            contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.PayeeDetails.ToString());

                            ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId + ");", true);
                        }

                    }
                    return;
                }

                //validate 
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Payee details not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                string royaltor;
                royContractPayeeBL = new RoyContractPayeeBL();
                DataSet payeeData = royContractPayeeBL.SavePayee(royaltorId, payeeList, deleteList.ToArray(), loggedUserID, out royaltor, out errorId);
                royContractPayeeBL = null;
                ViewState["vsDeleteIds"] = null;
                hdnGridDataDeleted.Value = "N";
                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                if (errorId == 0)
                {
                    txtRoyaltorId.Text = royaltor;
                    if (payeeData.Tables.Count != 0)
                    {
                        Session["RoyContPayeeGridDataInitial"] = payeeData.Tables[0];
                        Session["RoyContPayeeGridData"] = payeeData.Tables[0];                       

                        gvContPayee.DataSource = payeeData.Tables[0];
                        gvContPayee.DataBind();

                        if (payeeData.Tables[0].Rows.Count == 0)
                        {
                            gvContPayee.EmptyDataText = "No data found for the selected royaltor";
                        }
                        else
                        {
                            gvContPayee.EmptyDataText = string.Empty;
                        }

                    }
                    else if (payeeData.Tables.Count == 0)
                    {
                        dtEmpty = new DataTable();
                        gvContPayee.DataSource = dtEmpty;
                        gvContPayee.EmptyDataText = "No data found for the selected royaltor";
                        gvContPayee.DataBind();
                    }

                    //new royaltor - redirect to Bank details screen
                    //existing royaltor - remain in payee screen
                    if (isNewRoyaltor == "Y")
                    {
                        //WUIN-450
                        //set screen button enabled = Y
                        contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.PayeeDetails.ToString());

                        //redirect to Bank details screen                                  
                        //redirect in javascript so that issue of data not saved validation would be handled
                        ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId.Split('-')[0].Trim() + ");", true);
                    }
                    else
                    {
                        msgView.SetMessage("Payee details saved", MessageType.Warning, PositionType.Auto);
                    }

                }
                else if (errorId == 2)
                {
                    ExceptionHandler("Error in saving payee data", string.Empty);
                }
                else
                {
                    ExceptionHandler("Error in saving payee data", string.Empty);
                }



            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving payee data", ex.Message);
            }
        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {
            hdnIsAuditScreen.Value = "Y";
            //redirect in javascript so that issue of data not saved validation would be handled
            if (isNewRoyaltor == "Y")
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "PreviousScreen", "RedirectToPreviousScreen(" + royaltorId + ");", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "AuditScreen", "RedirectToAuditScreen(" + royaltorId + ");", true);
            }
        }

        //Change of Image button click to button click
        //JIRA-974 Changes by Ravi on 05/02/2019 -- Start
        protected void btnAddIntParty_Click(object sender, EventArgs e)
        {
            ClearAddIntPartyControls();
            mpeAddIntPartyPopup.Show();
        }
        //JIRA-974 Changes by Ravi on 05/02/2019 --End

        protected void btnAddIntPartySave_Click(object sender, EventArgs e)
        {
            try
            {
                string ipNumber;
                string intPartyId;

                loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                royContractPayeeBL = new RoyContractPayeeBL();
                royContractPayeeBL.AddInterestedParty(ddlAddIntPartyType.SelectedValue, txtAddIntPartyName.Text, txtAddIntPartyAddress1.Text, txtAddIntPartyAddress2.Text,
                    txtAddIntPartyAddress3.Text, txtAddIntPartyAddress4.Text, txtAddIntPartyPostCode.Text, txtAddIntPartyEmail.Text, txtAddIntPartyTaxNum.Text, ddlAddIntPartyTaxType.SelectedValue == "-" ? string.Empty : ddlAddIntPartyTaxType.SelectedValue, (cbAddIPGenerateInvoice.Checked ? "Y" : "N"),
                    loggedUserID, out intPartyId, out ipNumber, out errorId);
                royContractPayeeBL = null;

                if (errorId == 1)
                {
                    //validation - Name and Address is not already on Interested Party table for selected type

                    return;

                }
                else if (errorId == 2)
                {
                    ExceptionHandler("Error in saving Interested party", string.Empty);
                    return;
                }
                else
                {

                    hdnIntPartyIdAddRow.Value = intPartyId;
                    ddlPayeeTypeAdd.SelectedValue = ddlAddIntPartyType.SelectedValue;
                    txtIntPartyAddRow.Text = ipNumber + " - " + txtAddIntPartyName.Text;
                    lblAddress1AddRow.Text = txtAddIntPartyAddress1.Text;
                    lblAddress2AddRow.Text = txtAddIntPartyAddress2.Text;
                    lblAddress3AddRow.Text = txtAddIntPartyAddress3.Text;
                    lblAddress4AddRow.Text = txtAddIntPartyAddress4.Text;
                    lblPostCodeAddRow.Text = txtAddIntPartyPostCode.Text;
                    lblEmailAddRow.Text = txtAddIntPartyEmail.Text;
                    txtTaxNumAddRow.Text = txtAddIntPartyTaxNum.Text;
                    ddlTaxTypeAddRow.Text = ddlAddIntPartyTaxType.Text;
                    if (cbAddIPGenerateInvoice.Checked)//WUIN-1164
                    {
                        cbGenerateInvoiceAddRow.Checked = true;
                    }
                    else
                    {
                        cbGenerateInvoiceAddRow.Checked = false;
                    }
                    ClearAddIntPartyControls();
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving Interested party", ex.Message);
            }
        }

        protected void btnAddIntPartyCancel_Click(object sender, EventArgs e)
        {
            ClearAddIntPartyControls();
            mpeAddIntPartyPopup.Hide();
        }


        //JIRA-908 Changes by Ravi on 13/02/2019 -- Start
        protected void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                string intPartyId = hdnDeleteIntPartyId.Value;
                string intPartyType = hdnDeleteIntPartyType.Value;
                string isModified = hdnDeleteIsModified.Value;
                DeleteRowFromGrid(intPartyId, intPartyType, isModified);
                hdnDeleteIntPartyId.Value = "";
                hdnDeleteIntPartyType.Value = "";
                hdnDeleteIsModified.Value = "";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in deleting grid data", ex.Message);
            }
        }
        //JIRA-908 Changes by Ravi on 13/02/2019 -- End

        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvContPayee_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["RoyContPayeeGridDataInitial"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;

                gvContPayee.DataSource = dataView;
                gvContPayee.DataBind();
                Session["RoyContPayeeGridData"] = dataView.ToTable();
            }
            //WUIN-1096 -  ReadOnlyUser
            if ((Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower()))
            {
                EnableReadonly();
            }
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End


        #endregion EVENTS

        #region METHODS
        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

        }

        private void LoadPayeeData()
        {
            string royaltor;
            royContractPayeeBL = new RoyContractPayeeBL();
            DataSet payeeData = royContractPayeeBL.GetPayeeData(royaltorId, out royaltor, out errorId);
            royContractPayeeBL = null;

            if (payeeData.Tables.Count != 0 && errorId != 2)
            {
                txtRoyaltorId.Text = royaltor;
                dtPayeeType = payeeData.Tables[1];
                Session["RoyContPayeeGridDataInitial"] = payeeData.Tables[0];
                Session["RoyContPayeeGridData"] = payeeData.Tables[0];
                Session["RoyContPayeeTaxType"] = payeeData.Tables[2];

                ddlPayeeTypeAdd.DataSource = dtPayeeType;
                ddlPayeeTypeAdd.DataTextField = "dropdown_text";
                ddlPayeeTypeAdd.DataValueField = "dropdown_value";
                ddlPayeeTypeAdd.DataBind();
                ddlPayeeTypeAdd.Items.Insert(0, new ListItem("-"));

                ddlAddIntPartyType.DataSource = dtPayeeType;
                ddlAddIntPartyType.DataTextField = "dropdown_text";
                ddlAddIntPartyType.DataValueField = "dropdown_value";
                ddlAddIntPartyType.DataBind();
                ddlAddIntPartyType.Items.Insert(0, new ListItem("-"));

                //JIRA-1144 Changes -- Start
                ddlAddIntPartyTaxType.DataSource = payeeData.Tables[2]; ;
                ddlAddIntPartyTaxType.DataTextField = "tax_type_desc";
                ddlAddIntPartyTaxType.DataValueField = "tax_type";
                ddlAddIntPartyTaxType.DataBind();
                ddlAddIntPartyTaxType.Items.Insert(0, new ListItem("-"));
                //JIRA-1144 Changes -- End

                ddlTaxTypeAddRow.DataSource = payeeData.Tables[2]; ;
                ddlTaxTypeAddRow.DataTextField = "tax_type_desc";
                ddlTaxTypeAddRow.DataValueField = "tax_type";
                ddlTaxTypeAddRow.DataBind();
                ddlTaxTypeAddRow.Items.Insert(0, new ListItem("-"));

                if (payeeData.Tables[0].Rows.Count == 0)
                {
                    gvContPayee.DataSource = payeeData.Tables[0];
                    gvContPayee.EmptyDataText = "No data found for the selected royaltor";
                    gvContPayee.DataBind();
                }
                else
                {
                    gvContPayee.DataSource = payeeData.Tables[0];
                    gvContPayee.DataBind();
                }

            }
            else if (payeeData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvContPayee.DataSource = dtEmpty;
                gvContPayee.EmptyDataText = "No data found for the selected royaltor";
                gvContPayee.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data", string.Empty);
            }

            ViewState["vsDeleteIds"] = null;
            hdnGridDataDeleted.Value = "N";


        }

        private void ClearAddRow()
        {
            txtIntPartyAddRow.Text = string.Empty;
            lblAddress1AddRow.Text = string.Empty;
            lblAddress2AddRow.Text = string.Empty;
            lblAddress3AddRow.Text = string.Empty;
            lblAddress4AddRow.Text = string.Empty;
            lblEmailAddRow.Text = string.Empty;
            txtTaxNumAddRow.Text = string.Empty;
            ddlTaxTypeAddRow.SelectedIndex = 0;
            txtShareAddRow.Text = string.Empty;
            cbPrimaryAddRow.Checked = false;
            cbPayAddRow.Checked = false;
            txtPayeeTypeAdd.Text = string.Empty;
            txtPayeeTypeAdd.Visible = false;
            ddlPayeeTypeAdd.Visible = true;
            ddlPayeeTypeAdd.SelectedIndex = 0;
            txtIntPartyAddRow.ReadOnly = false;
            txtIntPartyAddRow.CssClass = "textboxStyle";
            txtTaxNumAddRow.Enabled = true;
            ddlTaxTypeAddRow.Enabled = true;
            txtShareAddRow.Enabled = true;
            cbPrimaryAddRow.Enabled = true;
            cbPayAddRow.Enabled = true;
            cbGenerateInvoiceAddRow.Enabled = true;
            cbGenerateInvoiceAddRow.Checked = false;
            hdnIntPartyIdAddRow.Value = null;
        }

        private Array PayeeDetailsList()
        {
            if (Session["RoyContPayeeGridDataInitial"] == null)
            {
                ExceptionHandler("Error in saving payee data", string.Empty);
            }

            DataTable dtPayeeData = (DataTable)Session["RoyContPayeeGridDataInitial"];
            List<string> payeeDetails = new List<string>();
            string intPartyId;
            string intPartyType;
            string taxNumber;
            string applicableTax;
            string sharePercentage;
            string isModified;
            CheckBox cbPrimaryPayee;
            CheckBox cbPay;
            CheckBox cbGenerateInvoice;

            foreach (GridViewRow gvr in gvContPayee.Rows)
            {
                intPartyId = (gvr.FindControl("hdnIntPartyId") as HiddenField).Value;
                intPartyType = (gvr.FindControl("hdnIntPartyType") as HiddenField).Value;
                isModified = (gvr.FindControl("hdnIsModified") as HiddenField).Value;
                taxNumber = (gvr.FindControl("txtTaxNumber") as TextBox).Text;
                applicableTax = (gvr.FindControl("ddlTaxType") as DropDownList).SelectedValue;
                if (applicableTax == "-")
                {
                    applicableTax = string.Empty;
                }
                sharePercentage = (gvr.FindControl("txtPercentShare") as TextBox).Text;
                cbPay = (gvr.FindControl("cbPay") as CheckBox);
                cbPrimaryPayee = (gvr.FindControl("cbPrimaryPayee") as CheckBox);
                cbGenerateInvoice = (gvr.FindControl("cbGenerateInvoice") as CheckBox);
                
                if (isModified == Global.DBNullParamValue)//new row                   
                {
                    isModified = Global.DBNullParamValue;
                    payeeDetails.Add(intPartyId + "(;||;)" + intPartyType + "(;||;)" + (taxNumber == string.Empty ? Global.DBNullParamValue : taxNumber) + "(;||;)" + (applicableTax == string.Empty ? Global.DBNullParamValue : applicableTax) + "(;||;)" + (intPartyType == "P" ? sharePercentage : "0") + "(;||;)" +
                                           (cbPrimaryPayee.Checked ? "Y" : "N") + "(;||;)" + (cbPay.Checked ? "Y" : "N") + "(;||;)" + (cbGenerateInvoice.Checked ? "Y" : "N") + "(;||;)" + isModified);
                }
                else
                {
                    DataTable dtPayee = dtPayeeData.Select("int_party_id=" + intPartyId).CopyToDataTable();
                    if (dtPayee.Rows.Count != 0)
                    {
                        if (intPartyType == "P" &&
                            (taxNumber != dtPayee.Rows[0]["vat_number"].ToString()
                            || applicableTax != dtPayee.Rows[0]["applicable_tax"].ToString()
                            || sharePercentage != (dtPayee.Rows[0]["payee_percentage"].ToString() == "" ? "" : Convert.ToDouble(dtPayee.Rows[0]["payee_percentage"]).ToString())
                            || dtPayee.Rows[0]["primary_payee"].ToString() != (cbPrimaryPayee.Checked ? "Y" : "N")
                            || dtPayee.Rows[0]["payee_pay"].ToString() != (cbPay.Checked ? "Y" : "N")
                            || dtPayee.Rows[0]["generate_invoice"].ToString() != (cbGenerateInvoice.Checked ? "Y" : "N")//WUIN-1164
                            ))//existing row - if payee - data changed 
                        {
                            //if only Tax number/applicable tax/generate invoice is modified and not the payee details, then update only int_party table and not the payee table
                            if (sharePercentage != (dtPayee.Rows[0]["payee_percentage"].ToString() == "" ? "" : Convert.ToDouble(dtPayee.Rows[0]["payee_percentage"]).ToString())
                            || dtPayee.Rows[0]["primary_payee"].ToString() != (cbPrimaryPayee.Checked ? "Y" : "N")
                            || dtPayee.Rows[0]["payee_pay"].ToString() != (cbPay.Checked ? "Y" : "N"))
                            {
                                isModified = "Y";
                            }
                            else if (taxNumber != dtPayee.Rows[0]["vat_number"].ToString() || applicableTax != dtPayee.Rows[0]["applicable_tax"].ToString() || dtPayee.Rows[0]["generate_invoice"].ToString() != (cbGenerateInvoice.Checked ? "Y" : "N"))//WUIN-1164
                            {
                                isModified = "VAT";
                            }

                            payeeDetails.Add(intPartyId + "(;||;)" + intPartyType + "(;||;)" + (taxNumber == string.Empty ? Global.DBNullParamValue : taxNumber) + "(;||;)" + (applicableTax == string.Empty ? Global.DBNullParamValue : applicableTax) + "(;||;)" + (intPartyType == "P" ? sharePercentage : "0") + "(;||;)" +
                                                   (cbPrimaryPayee.Checked ? "Y" : "N") + "(;||;)" + (cbPay.Checked ? "Y" : "N") + "(;||;)" + (cbGenerateInvoice.Checked ? "Y" : "N") + "(;||;)" + isModified);
                        }
                    }

                }


            }

            return payeeDetails.ToArray();
        }

        /// <summary>
        /// When returning results – exclude rows already in grid.
        /// when poping up the interested party search list in add row, exclude the one present in the grid
        /// </summary>
        /// <returns></returns>
        private string GetExcludeIntPartyList()
        {
            string intPartyIds = "-1";//to handle if no rows in grid for a new royaltor
            string intPartyId;

            foreach (GridViewRow gvr in gvContPayee.Rows)
            {
                intPartyId = (gvr.FindControl("hdnIntPartyId") as HiddenField).Value;
                if (intPartyIds == string.Empty)
                {
                    intPartyIds = intPartyId;
                }
                else
                {
                    intPartyIds = intPartyIds + "," + intPartyId;
                }

            }

            return intPartyIds;
        }

        private bool ValidateAppendRow()
        {
            //validate if duplicate rows are not being added to the grid
            bool isValid = true;

            if (hdnIntPartyIdAddRow.Value == null || hdnIntPartyIdAddRow.Value == string.Empty)
            {
                return false;
            }

            string intPartyId;

            foreach (GridViewRow gvr in gvContPayee.Rows)
            {
                intPartyId = (gvr.FindControl("hdnIntPartyId") as HiddenField).Value;
                if (intPartyId == hdnIntPartyIdAddRow.Value)
                {
                    isValid = false;
                    break;
                }

            }

            return isValid;
        }

        /*
        private bool ValidateSave()
        {
            //Validate Payees have total of Share %  = 100
            //Must be at least one Payee
            //Only one payee should be set as Primary

            bool isValid = true;
            int primaryPayeeCount = 0;
            int payeeCount = 0;
            double totalShare = 0;
            string payeeType;
            double percentageShare;
            CheckBox cbPrimaryPayee;

            foreach (GridViewRow gvr in gvContPayee.Rows)
            {
                cbPrimaryPayee = (gvr.FindControl("cbPrimaryPayee") as CheckBox);
                payeeType = (gvr.FindControl("hdnIntPartyType") as HiddenField).Value;                
                if (payeeType == "P")
                {
                    percentageShare = Convert.ToDouble((gvr.FindControl("txtPercentShare") as TextBox).Text);
                    payeeCount++;
                    totalShare = totalShare + percentageShare;                    
                }

                if (cbPrimaryPayee.Checked)
                {
                    primaryPayeeCount++;
                }

            }


            if (payeeCount == 0)
            {
                isValid = false;
                msgView.SetMessage("Must be at least one payee!", MessageType.Warning, PositionType.Auto);
            }
            else if (totalShare != Convert.ToDouble("100"))
            {
                isValid = false;
                msgView.SetMessage("Payees must have total of Share %  = 100", MessageType.Warning, PositionType.Auto);
            }
            else if (primaryPayeeCount == 0)
            {
                isValid = false;
                msgView.SetMessage("One Payee must be set as Primary for the Royaltor", MessageType.Warning, PositionType.Auto);                
            }
            else if (primaryPayeeCount > 1)
            {
                isValid = false;                
                msgView.SetMessage("Only one payee should be set as Primary", MessageType.Warning, PositionType.Auto);
            }


            return isValid;
        }
        */
        private void ClearAddIntPartyControls()
        {
            ddlAddIntPartyType.SelectedIndex = 0;
            txtAddIntPartyName.Text = string.Empty;
            txtAddIntPartyAddress1.Text = string.Empty;
            txtAddIntPartyAddress2.Text = string.Empty;
            txtAddIntPartyAddress3.Text = string.Empty;
            txtAddIntPartyAddress4.Text = string.Empty;
            txtAddIntPartyPostCode.Text = string.Empty;
            txtAddIntPartyEmail.Text = string.Empty;
            txtAddIntPartyTaxNum.Text = string.Empty;
            ddlAddIntPartyTaxType.SelectedIndex = 0;
            cbAddIPGenerateInvoice.Checked = false;
        }

        private void RowCancelChanges(string intPartyId)
        {
            if (Session["RoyContPayeeGridData"] == null || Session["RoyContPayeeGridDataInitial"] == null)
            {
                ExceptionHandler("Error in cancelling row changes", string.Empty);
            }

            GetGridData();
            DataTable dtGridData = (DataTable)Session["RoyContPayeeGridData"];
            DataTable dtOriginalGridData = (DataTable)Session["RoyContPayeeGridDataInitial"];
            //get original data row before changes for the selected row cancel
            DataTable dtOriginalGridDataRow = dtOriginalGridData.Select("int_party_id=" + intPartyId).CopyToDataTable();
            DataRow drOriginalGridDataRow;

            if (dtOriginalGridDataRow.Rows.Count != 0)
            {
                drOriginalGridDataRow = dtOriginalGridDataRow.Rows[0];

                foreach (DataRow dr in dtGridData.Rows)
                {
                    if (dr["int_party_id"].ToString() == intPartyId)
                    {
                        dr.Delete();
                        break;
                    }
                }

                dtGridData.ImportRow(drOriginalGridDataRow);
                DataView dv = dtGridData.DefaultView;
                dv.Sort = "display_order, int_party_name asc";
                DataTable dtGridChangedDataSorted = dv.ToTable();
                Session["RoyContPayeeGridData"] = dtGridChangedDataSorted;
                gvContPayee.DataSource = dtGridChangedDataSorted;
                gvContPayee.DataBind();

            }

        }

        private void DeleteRowFromGrid(string intPartyIdToDelete, string intPartyType, string isModified)
        {
            GetGridData();
            //add to delete list only if the row is not a new one 
            List<string> deleteList = new List<string>();
            if (isModified != Global.DBNullParamValue)
            {
                if (ViewState["vsDeleteIds"] != null)
                {
                    deleteList = (List<string>)ViewState["vsDeleteIds"];
                    deleteList.Add(intPartyIdToDelete + "(;||;)" + intPartyType);
                }
                else
                {
                    deleteList.Add(intPartyIdToDelete + "(;||;)" + intPartyType);
                }

                ViewState["vsDeleteIds"] = deleteList;

            }

            DataTable dtGridData = (DataTable)Session["RoyContPayeeGridData"];
            foreach (DataRow dr in dtGridData.Rows)
            {
                if (dr["int_party_id"].ToString() == intPartyIdToDelete)
                {
                    dr.Delete();
                    break;
                }

            }

            DataView dv = dtGridData.DefaultView;
            dv.Sort = "display_order, int_party_name asc";
            DataTable dtGridChangedDataSorted = dv.ToTable();
            Session["RoyContPayeeGridData"] = dtGridChangedDataSorted;
            gvContPayee.DataSource = dtGridChangedDataSorted;
            gvContPayee.DataBind();

            //delete row from initial grid data session
            DataTable dtOriginalGridData = (DataTable)Session["RoyContPayeeGridDataInitial"];
            foreach (DataRow dr in dtOriginalGridData.Rows)
            {
                if (dr["int_party_id"].ToString() == intPartyIdToDelete)
                {
                    dr.Delete();
                    break;
                }

            }

            DataView dvInitialData = dtOriginalGridData.DefaultView;
            dvInitialData.Sort = "display_order, int_party_name asc";
            DataTable dtOriginalGridDataSorted = dvInitialData.ToTable();
            Session["RoyContPayeeGridDataInitial"] = dtOriginalGridDataSorted;

        }

        private void GetGridData()
        {
            DataTable dtGridData = (DataTable)Session["RoyContPayeeGridData"];
            DataTable dtGridChangedData = dtGridData.Clone();
            CheckBox cbPrimaryPayee;
            CheckBox cbPay;
            CheckBox cbGenerateInvoice;
            string intPartyType;
            string percentShare;

            foreach (GridViewRow gvr in gvContPayee.Rows)
            {
                cbPrimaryPayee = (gvr.FindControl("cbPrimaryPayee") as CheckBox);
                cbPay = (gvr.FindControl("cbPay") as CheckBox);
                cbGenerateInvoice = (gvr.FindControl("cbGenerateInvoice") as CheckBox);
                intPartyType = (gvr.FindControl("hdnIntPartyType") as HiddenField).Value;
                DataRow drGridRow = dtGridChangedData.NewRow();
                drGridRow["display_order"] = (gvr.FindControl("hdnDisplayOrder") as HiddenField).Value;
                drGridRow["int_party_id"] = (gvr.FindControl("hdnIntPartyId") as HiddenField).Value;
                drGridRow["int_party_type"] = intPartyType;
                drGridRow["payee_type"] = (gvr.FindControl("lblPayeeType") as Label).Text;
                drGridRow["ip_number"] = (gvr.FindControl("lblIPNumber") as Label).Text;
                drGridRow["int_party_name"] = (gvr.FindControl("lblName") as Label).Text;
                drGridRow["int_party_add1"] = (gvr.FindControl("lblAddress1") as Label).Text;
                drGridRow["int_party_add2"] = (gvr.FindControl("lblAddress2") as Label).Text;
                drGridRow["int_party_add3"] = (gvr.FindControl("lblAddress3") as Label).Text;
                drGridRow["int_party_add4"] = (gvr.FindControl("lblAddress4") as Label).Text;
                drGridRow["int_party_postcode"] = (gvr.FindControl("lblPostCode") as Label).Text;
                drGridRow["email"] = (gvr.FindControl("lblEmail") as Label).Text;
                drGridRow["primary_payee"] = cbPrimaryPayee.Checked ? "Y" : "N";
                drGridRow["payee_pay"] = cbPay.Checked ? "Y" : "N";
                drGridRow["generate_invoice"] = cbGenerateInvoice.Checked ? "Y" : "N";//WUIN-1164
                drGridRow["is_modified"] = (gvr.FindControl("hdnIsModified") as HiddenField).Value;
                if (intPartyType == "P")
                {
                    if ((gvr.FindControl("txtPercentShare") as TextBox).Text == string.Empty)
                    {
                        drGridRow["payee_percentage"] = DBNull.Value;
                    }
                    else
                    {
                        //to handle an invalid data on delete-
                        percentShare = (gvr.FindControl("txtPercentShare") as TextBox).Text;
                        //RegularExpressionValidator revtxtPercentShare = (gvr.FindControl("revtxtPercentShare") as RegularExpressionValidator);      

                        if (Regex.IsMatch(percentShare, @"^100(\.000?)?|^\d{0,2}(\.\d{1,3})? *%?$"))
                        {
                            drGridRow["payee_percentage"] = percentShare;
                        }
                        else
                        {
                            drGridRow["payee_percentage"] = DBNull.Value;
                        }

                    }

                    if ((gvr.FindControl("txtTaxNumber") as TextBox).Text == string.Empty)
                    {
                        drGridRow["vat_number"] = DBNull.Value;
                    }
                    else
                    {
                        drGridRow["vat_number"] = (gvr.FindControl("txtTaxNumber") as TextBox).Text;
                    }

                    if ((gvr.FindControl("ddlTaxType") as DropDownList).SelectedValue == "-")
                    {
                        drGridRow["applicable_tax"] = DBNull.Value;
                    }
                    else
                    {
                        drGridRow["applicable_tax"] = (gvr.FindControl("ddlTaxType") as DropDownList).SelectedValue;
                    }


                }
                else
                {
                    drGridRow["payee_percentage"] = DBNull.Value;
                    drGridRow["vat_number"] = DBNull.Value;
                }
                dtGridChangedData.Rows.Add(drGridRow);

            }

            Session["RoyContPayeeGridData"] = dtGridChangedData;

        }

        private void AppendRowToGrid()
        {
            if (Session["RoyContPayeeGridData"] == null || Session["RoyContPayeeGridDataInitial"] == null)
            {
                ExceptionHandler("Error in adding royalty rate row to grid", string.Empty);
            }

            GetGridData();
            DataTable dtGridData = (DataTable)Session["RoyContPayeeGridData"];
            DataTable dtOriginalGridData = (DataTable)Session["RoyContPayeeGridDataInitial"];
            DataRow drNewRow = dtGridData.NewRow();
            DataRow drNewRowOriginal = dtOriginalGridData.NewRow();

            drNewRow["display_order"] = (ddlPayeeTypeAdd.SelectedValue == "P" ? (cbPrimaryAddRow.Checked ? "1" : "2") : "3");
            drNewRow["int_party_id"] = hdnIntPartyIdAddRow.Value;
            drNewRow["int_party_type"] = ddlPayeeTypeAdd.SelectedValue;
            drNewRow["payee_type"] = ddlPayeeTypeAdd.SelectedItem.Text;
            drNewRow["ip_number"] = txtIntPartyAddRow.Text.Split('-')[0].Trim();
            drNewRow["int_party_name"] = txtIntPartyAddRow.Text.Split('-')[1].Trim();
            drNewRow["int_party_add1"] = lblAddress1AddRow.Text;
            drNewRow["int_party_add2"] = lblAddress2AddRow.Text;
            drNewRow["int_party_add3"] = lblAddress3AddRow.Text;
            drNewRow["int_party_add4"] = lblAddress4AddRow.Text;
            drNewRow["int_party_postcode"] = lblPostCodeAddRow.Text;
            drNewRow["email"] = lblEmailAddRow.Text;
            drNewRow["vat_number"] = txtTaxNumAddRow.Text;
            drNewRow["applicable_tax"] = ddlTaxTypeAddRow.SelectedValue;
            drNewRow["primary_payee"] = cbPrimaryAddRow.Checked == true ? "Y" : "N";
            drNewRow["payee_pay"] = cbPayAddRow.Checked == true ? "Y" : "N";
            drNewRow["generate_invoice"] = cbGenerateInvoiceAddRow.Checked == true ? "Y" : "N";
            drNewRow["is_modified"] = "-";
            if (ddlPayeeTypeAdd.SelectedValue == "P" && txtShareAddRow.Text != string.Empty)
            {

                drNewRow["payee_percentage"] = txtShareAddRow.Text;
            }
            else
            {
                drNewRow["payee_percentage"] = DBNull.Value;
            }
            dtGridData.Rows.Add(drNewRow);

            DataView dv = dtGridData.DefaultView;
            dv.Sort = "display_order, int_party_name asc";
            DataTable dtGridDataSorted = dv.ToTable();
            Session["RoyContPayeeGridData"] = dtGridDataSorted;
            gvContPayee.DataSource = dtGridDataSorted;
            gvContPayee.DataBind();

            drNewRowOriginal.ItemArray = drNewRow.ItemArray.Clone() as object[];
            dtOriginalGridData.Rows.Add(drNewRowOriginal);
            DataView dvInitialData = dtOriginalGridData.DefaultView;
            dvInitialData.Sort = "display_order, int_party_name asc";
            DataTable dtOriginalGridDataSorted = dvInitialData.ToTable();
            Session["RoyContPayeeGridDataInitial"] = dtOriginalGridDataSorted;

        }

        private void EnableReadonly()
        {
            btnSave.Enabled = false;
            btnAppendAddRow.Enabled = false;
            btnUndoAddRow.Enabled = false;
            btnAddIntParty.Enabled = false;

            //disable grid buttons
            foreach (GridViewRow gvr in gvContPayee.Rows)
            {
                (gvr.FindControl("imgBtnDelete") as ImageButton).Enabled = false;
                (gvr.FindControl("imgBtnUndo") as ImageButton).Enabled = false;
            }
        }



        #endregion METHODS

        #region Save all functionality

        #endregion Save all functionality



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
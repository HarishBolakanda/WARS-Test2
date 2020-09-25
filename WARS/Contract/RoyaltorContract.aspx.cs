/*
File Name   :   RoyaltorContract.cs
Purpose     :   to create/edit contracts of royaltors

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     20-Mar-2017     Harish(Infosys Limited)   Initial Creation
        21-09-2017      Harish                    WUIN-286 changes 
2.0     26-Mar-2018     Harish                    WUIN-517 - Disable Contract edit if Locked
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
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace WARS
{
    public partial class RoyaltorContract : System.Web.UI.Page
    {
        #region Global Declarations
        RoyaltorContractBL royContractBL;
        Utilities util;
        Int32 errorId;
        string wildCardChar = ".";
        string royaltorId = string.Empty;
        string isNewRoyaltor = string.Empty;
        string isRespChanged = string.Empty;
        string isOwnerChanged = string.Empty;
        #endregion Global Declarations

        #region EVENTS

        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {

                royaltorId = Request.QueryString["RoyaltorId"];
                isNewRoyaltor = Request.QueryString["isNewRoyaltor"];
                //royaltorId = "12340";
                //isNewRoyaltor = "N";

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Contract";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Contract";
                }



                txtRoyPLGId.Focus();//tabbing sequence starts here

                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        PopulateDropDowns();
                        //HtmlTableRow trContractDetails = (HtmlTableRow)contractNavigationButtons.FindControl("trContractDetails");
                        //trContractDetails.Visible = false;

                        HiddenField hdnRoyaltorId = (HiddenField)contractNavigationButtons.FindControl("hdnRoyaltorId");
                        hdnRoyaltorId.Value = royaltorId;
                        HiddenField hdnRoyaltorIdHdr = (HiddenField)contractHdrNavigation.FindControl("hdnRoyaltorId");
                        hdnRoyaltorIdHdr.Value = royaltorId;

                        //WUIN-599 - resetting the flags
                        ((HiddenField)Master.FindControl("hdnIsContractScreen")).Value = "N";
                        ((HiddenField)Master.FindControl("hdnIsNotContractScreen")).Value = "N";
                        this.Master.FindControl("lnkBtnHome").Visible = false;

                        if (royaltorId == null && isNewRoyaltor == null)
                        {
                            txtRoyaltorId.Focus();
                            btnSave.Text = "Save & Continue";
                            btnAudit.Text = "Cancel";
                            btnCopyContractPopup.Enabled = false;
                            btnLock.Enabled = false;

                            //WUIN-450 changes
                            //disable screen navigation buttons for new royaltor                            
                            //contractNavigationButtons.Disable();
                            //disable royaltor search navigation button for contract screen
                            //Button btnRoySearch = (Button)contractNavigationButtons.FindControl("btnRoySearch");
                            //btnRoySearch.Enabled = false;              

                            contractNavigationButtons.EnableNewRoyNavButtons(ContractScreens.ContractDetails.ToString());

                        }
                        else
                        {
                            LoadContractData();
                            txtRoyaltorId.Font.Bold = true;
                            txtRoyaltorId.ReadOnly = true;
                            txtRoyaltorId.Attributes.Add("onFocus", "MoveRoyaltorIdFocus()");
                            txtRoyaltorId.CssClass = "textboxStyle_readonly";

                            if (isNewRoyaltor == "Y")
                            {
                                txtRoyaltorId.Focus();
                                btnSave.Text = "Save & Continue";
                                btnAudit.Text = "Cancel";
                                btnCopyContractPopup.Enabled = false;

                                //WUIN-450 changes
                                //disable screen navigation buttons for new royaltor                            
                                //contractNavigationButtons.Disable();
                                //disable royaltor search navigation button for contract screen
                                //Button btnRoySearch = (Button)contractNavigationButtons.FindControl("btnRoySearch");
                                //btnRoySearch.Enabled = false;    

                                contractNavigationButtons.EnableNewRoyNavButtons(ContractScreens.ContractDetails.ToString());

                            }
                        }

                        UserAuthorization();
                        Button btnRoyContract = (Button)contractNavigationButtons.FindControl("btnRoyContract");
                        btnRoyContract.Enabled = false;

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

        protected void btnOwnerFuzzySearch_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchOwner();
                double searchHeight = hdnWindowHeight.Value == string.Empty ? 300 : Convert.ToDouble(hdnWindowHeight.Value) * 0.3;
                lbFuzzySearch.Style.Add("height", searchHeight.ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in owner fuzzy search", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "Owner")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtOwnerSearch.Text = string.Empty;
                        return;
                    }

                    txtOwnerSearch.Text = lbFuzzySearch.SelectedValue.ToString();
                }
                else if (hdnFuzzySearchField.Value == "Royaltor")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtRoyaltorSearch.Text = string.Empty;
                        hdnRoyaltorType.Value = "A";
                        return;
                    }
                    else
                    {
                        hdnRoyaltorType.Value = "P";
                    }

                    txtRoyaltorSearch.Text = lbFuzzySearch.SelectedValue.ToString();
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fuzzy search selection", ex.Message);
            }
        }

        protected void btnRoyaltorSearch_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchRoyaltor();
                double searchHeight = hdnWindowHeight.Value == string.Empty ? 300 : Convert.ToDouble(hdnWindowHeight.Value) * 0.3;
                lbFuzzySearch.Style.Add("height", searchHeight.ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in owner fuzzy search", ex.Message);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //validate 
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Royaltor Contract details not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }
                isRespChanged = "N";
                isOwnerChanged = "N";

                if (royaltorId == null)//new royaltor
                {
                    if (txtOwnerSearch.Text != string.Empty)
                    {
                        isOwnerChanged = "C";
                    }

                }
                else//existing royaltor
                {
                    //if no changes made to save
                    if (hdnChangeNotSaved.Value != "Y")
                    {
                        if (isNewRoyaltor == "Y")
                        {
                            //WUIN-450
                            //set screen button enabled = Y
                            contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.ContractDetails.ToString());

                            ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId + ");", true);
                        }
                        else
                        {
                            msgView.SetMessage("No changes made to save!", MessageType.Warning, PositionType.Auto);
                        }

                        return;
                    }
                    else
                    {
                        // WUIN-1114 - Any changes made to the contract which is at manager sign off/Team sign off should move back to under review 
                        if ((hdnStatusCode.Value == "2" || hdnStatusCode.Value == "3") && hdnIsStatusChange.Value == "N")
                        {
                            ddlStatus.SelectedValue = "1";
                        }
                    }

                    if ((hdnOwner.Value == null && txtOwnerSearch.Text != string.Empty) ||
                        (txtOwnerSearch.Text != string.Empty && hdnOwner.Value != txtOwnerSearch.Text.Substring(0, txtOwnerSearch.Text.IndexOf("-") - 1)))
                    {
                        isOwnerChanged = "C";
                    }
                    else if (ddlResponsibility.SelectedValue != hdnResponsibility.Value && txtOwnerSearch.Text != string.Empty)
                    {
                        isRespChanged = "Y";
                    }


                }

                SaveContract();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving contract", ex.Message);
            }
        }

        protected void btnSaveOwner_Click(object sender, EventArgs e)
        {
            try
            {
                royContractBL = new RoyaltorContractBL();
                DataSet ownerData = royContractBL.AddOwnerDetails(txtOwnerCode.Text.Trim(), txtOwnerDesc.Text.Trim().ToUpper(), Convert.ToString(Session["UserCode"]), out errorId);
                royContractBL = null;

                if (errorId == 0)
                {
                    txtOwnerSearch.Text = txtOwnerCode.Text.ToString() + " - " + txtOwnerDesc.Text.Trim().ToUpper();
                    txtOwnerDesc.Text = string.Empty;
                    if (ownerData.Tables.Count != 0)
                    {
                        //WUIN-929 - set this to null so that newly added owner will be included in next fetch of the search list
                        Session["FuzzySearchAllOwnerList"] = ownerData.Tables[0];

                        ////WUIN-595 -To display the next max owner code when adding a new owner.
                        //if (ownerData.Tables[0].Rows.Count > 0)
                        //{
                        //    txtOwnerCode.Text = (Convert.ToInt32(ownerData.Tables[0].Compute("max([owner_code])", string.Empty)) + 1).ToString();
                        //}
                        //else
                        //{
                        //    txtOwnerCode.Text = "1";
                        //}

                    }

                    mpeAddOwner.Hide();
                    msgView.SetMessage("Owner created successfully.", MessageType.Success, PositionType.Auto);

                }
                else if (errorId == 1)
                {
                    msgView.SetMessage("Owner code already exists.", MessageType.Success, PositionType.Auto);
                }
                else
                {
                    ExceptionHandler("Error in saving owner", string.Empty);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving owner", ex.Message);
            }

        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {
            hdnIsAuditScreen.Value = "Y";

            if ((royaltorId == null && isNewRoyaltor == null) || (royaltorId != null && isNewRoyaltor == "Y"))
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "PreviousScreen", "RedirectToPreviousScreen(" + royaltorId + ");", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "AuditScreen", "RedirectToAuditScreen(" + royaltorId + ");", true);
            }

        }

        protected void btnUnSavedDataExit_Click(object sender, EventArgs e)
        {
            if (hdnButtonSelection.Value == "CopyContract")
            {
                btnCopyContractPopup_Click(sender, e);
            }
            else if (hdnButtonSelection.Value == "Lock")
            {
                btnLock_Click(sender, e);
            }
        }

        protected void valStartDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (txtStartDate.Text.Trim() != "__/__/____" && txtStartDate.Text.Trim() != string.Empty)
            {
                DateTime startDate = DateTime.MinValue;
                try
                {
                    startDate = Convert.ToDateTime(txtStartDate.Text);
                }
                catch
                {
                    args.IsValid = false;
                    valStartDate.ToolTip = "Please enter a valid date in DD/MM/YYYY format";
                    return;
                }
            }
            else
            {
                txtStartDate.Text = string.Empty;
            }
        }

        protected void valExpiryDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (txtExpiryDate.Text.Trim() != "__/__/____" && txtExpiryDate.Text.Trim() != string.Empty)
            {
                DateTime expiryDate = DateTime.MinValue;
                try
                {
                    expiryDate = Convert.ToDateTime(txtExpiryDate.Text);
                }
                catch
                {
                    args.IsValid = false;
                    valExpiryDate.ToolTip = "Please enter a valid date in DD/MM/YYYY format";
                    return;
                }

                //WUIN-1009 changes
                //Start date can be empty. No need to validate if start_date is earlier than the exipty_date in this case.
                if (txtStartDate.Text == string.Empty)
                {
                    return;
                }

                DateTime startDate = DateTime.MinValue;
                try
                {
                    startDate = Convert.ToDateTime(txtStartDate.Text);
                }
                catch
                {
                    return;
                }

                /* Harish: WUIN-800 - This is removed as discussed with Business that contract should be editable for which expiry date is past to current date
                //validate - expiry date should be a future date
                if (!(expiryDate > DateTime.Today))
                {
                    args.IsValid = false;
                    valExpiryDate.ToolTip = "Please enter a future date";
                    return;
                }
                 * */

                //validate - start_date should be earlier than the exipty_date 
                if (expiryDate < startDate)
                {
                    args.IsValid = false;
                    valExpiryDate.ToolTip = "Expiry date cannot be earlier to start date!";
                    return;
                }

            }

        }

        protected void valResvEndAte_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (txtResvEndDate.Text.Trim() != "__/____" && txtResvEndDate.Text.Trim() != string.Empty)
            {
                /* //WUIN-286 - Remove validation for Reserves End Date in future
                DateTime resvEndDate = Convert.ToDateTime(txtResvEndDate.Text);
                //validate - reserves en date should be a future date
                if (!(resvEndDate > DateTime.Today))
                {
                    args.IsValid = false;                    
                }
                 * */
                Int32 dateYear = Convert.ToInt32(txtResvEndDate.Text.Replace('_', ' ').Split('/')[1].Trim());
                Int32 dateMonth = Convert.ToInt32(txtResvEndDate.Text.Replace('_', ' ').Split('/')[0].Trim());
                if (!(dateMonth > 0 && dateMonth < 13) || !(dateYear > 1900))
                {
                    args.IsValid = false;
                    return;
                }


            }
        }

        protected void valtxtRoyaltorSearch_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (txtRoyaltorSearch.Text.Trim() != string.Empty)
            {
                //validate - royaltor should be selected from search list only
                try
                {
                    string royaltorId = txtRoyaltorSearch.Text.Substring(0, txtRoyaltorSearch.Text.IndexOf("-") - 1);
                }
                catch (Exception ex)
                {
                    args.IsValid = false;
                    valtxtRoyaltorSearch.ToolTip = "Please select royaltor from search list";
                    return;
                }

            }
            else if (txtRoyaltorSearch.Text.Trim() == string.Empty && txtChargeablePct.Text.Trim() != string.Empty)
            {
                args.IsValid = false;
                valtxtRoyaltorSearch.ToolTip = "Please select chargeable to";
                return;
            }
        }

        protected void valtxtChargeablePct_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (txtRoyaltorSearch.Text.Trim() != string.Empty && txtChargeablePct.Text.Trim() == string.Empty)
            {
                args.IsValid = false;
                return;
            }
        }

        protected void valLiqPeriodPct_ServerValidate(object source, ServerValidateEventArgs args)
        {
            //Liquidation periods are Mandatory only when Reserve % > 0 and reserves end date is not entered.
            //If Reserve % = null or 0, then no Reserve Period % should be set up
            //8 fields are % values – if entered, must sum up to 100%, do not need to be consecutive

            if (txtDefaultResvPct.Text != string.Empty && Convert.ToDouble(txtDefaultResvPct.Text) > 0 && (txtResvEndDate.Text.Trim() == "__/____" || txtResvEndDate.Text.Trim() == string.Empty) && (txtLiqPct1.Text == string.Empty && txtLiqPct2.Text == string.Empty && txtLiqPct3.Text == string.Empty &&
                 txtLiqPct4.Text == string.Empty && txtLiqPct5.Text == string.Empty && txtLiqPct6.Text == string.Empty && txtLiqPct7.Text == string.Empty &&
                 txtLiqPct8.Text == string.Empty))
            {
                args.IsValid = false;
                valLiqPeriodPct.ToolTip = "Please enter liquidation period % values summing to 100%";
            }
            else if ((txtDefaultResvPct.Text == string.Empty || Convert.ToDouble(txtDefaultResvPct.Text) == 0) &&
                     (txtLiqPct1.Text != string.Empty || txtLiqPct2.Text != string.Empty || txtLiqPct3.Text != string.Empty ||
                      txtLiqPct4.Text != string.Empty || txtLiqPct5.Text != string.Empty || txtLiqPct6.Text != string.Empty || txtLiqPct7.Text != string.Empty ||
                      txtLiqPct8.Text != string.Empty)
                )
            {
                args.IsValid = false;
                valLiqPeriodPct.ToolTip = "Liquidation period % cannot be entered without default reserve %";
            }
            else if (txtLiqPct1.Text != string.Empty || txtLiqPct2.Text != string.Empty || txtLiqPct3.Text != string.Empty ||
                     txtLiqPct4.Text != string.Empty || txtLiqPct5.Text != string.Empty || txtLiqPct6.Text != string.Empty || txtLiqPct7.Text != string.Empty ||
                     txtLiqPct8.Text != string.Empty)
            {
                decimal liqPeriodPct = (txtLiqPct1.Text == string.Empty ? 0 : Convert.ToDecimal(txtLiqPct1.Text)) +
                        (txtLiqPct2.Text == string.Empty ? 0 : Convert.ToDecimal(txtLiqPct2.Text)) +
                        (txtLiqPct3.Text == string.Empty ? 0 : Convert.ToDecimal(txtLiqPct3.Text)) +
                        (txtLiqPct4.Text == string.Empty ? 0 : Convert.ToDecimal(txtLiqPct4.Text)) +
                        (txtLiqPct5.Text == string.Empty ? 0 : Convert.ToDecimal(txtLiqPct5.Text)) +
                        (txtLiqPct6.Text == string.Empty ? 0 : Convert.ToDecimal(txtLiqPct6.Text)) +
                        (txtLiqPct7.Text == string.Empty ? 0 : Convert.ToDecimal(txtLiqPct7.Text)) +
                        (txtLiqPct8.Text == string.Empty ? 0 : Convert.ToDecimal(txtLiqPct8.Text));

                if (liqPeriodPct != 100)
                {
                    args.IsValid = false;
                    valLiqPeriodPct.ToolTip = "Please enter liquidation period % values summing to 100%";
                }
            }





        }

        protected void valddlResvTakenOn_ServerValidate(object source, ServerValidateEventArgs args)
        {
            //If Reserve % > 0 then other Reserve data should be set up (except End date which is optional)
            if ((txtDefaultResvPct.Text != string.Empty && Convert.ToDouble(txtDefaultResvPct.Text) > 0) && ddlResvTakenOn.SelectedValue == "-")
            {
                args.IsValid = false;
            }
        }

        protected void valcbSalesUnits_ServerValidate(object source, ServerValidateEventArgs args)
        {
            //If Reserve % > 0 then other Reserve data should be set up (except End date which is optional)            
            if ((txtDefaultResvPct.Text != string.Empty && Convert.ToDouble(txtDefaultResvPct.Text) > 0) && ddlNettOrSalesUnits.SelectedValue == "-")
            {
                valNettOrSalesUnits.ToolTip = "Please enter nett / sales units";
                args.IsValid = false;
            }
        }

        protected void ddlResponsibility_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (royaltorId != null && ddlResponsibility.SelectedValue != hdnResponsibility.Value && txtOwnerSearch.Text != string.Empty)
                {
                    lblConfirmMsg.Text = "This royaltor is in the owner group " + hdnOwner.Value + ", all the responsibility codes for the royaltors in this group will be altered!";
                    mpeConfirmRespChange.Show();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in validating responsibility change", ex.Message);
            }
        }

        protected void btnOkOwnChange_Click(object sender, EventArgs e)
        {

            isRespChanged = "N";
            isOwnerChanged = "Y";
            SaveContract();

        }

        protected void btnConfirmRespChangeCancel_Click(object sender, EventArgs e)
        {
            if (royaltorId != null && ddlResponsibility.SelectedValue != hdnResponsibility.Value)
            {
                ddlResponsibility.SelectedValue = hdnResponsibility.Value;
            }
        }

        protected void txtRoyaltorId_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int inputVal;
                if (!int.TryParse(txtRoyaltorId.Text, out inputVal))
                {
                    if (txtRoyaltorId.Text == string.Empty)
                        rfvtxtRoyaltorId.IsValid = false;
                    else
                        revtxtRoyaltorId.IsValid = false;

                    return;
                }

                //The hundred column of the royalty number in (0,1,2,3) = 1
                //The hundred column of the royalty number in (4,5,6) = 2
                //The hundred column of the royalty number in (7,8,9) = 3
                int printGroup = 1;
                string royaltorId = txtRoyaltorId.Text;

                if (royaltorId.Length > 2)
                {
                    printGroup = Convert.ToInt16(royaltorId.Substring(royaltorId.Length - 3, 1));
                }

                //txtPrintGrp is removed WUIN-377
                //if (printGroup <= 3)
                //    txtPrintGrp.Text = "1";
                //else if (printGroup >= 4 && printGroup <= 6)
                //    txtPrintGrp.Text = "2";
                //else if (printGroup >= 7 && printGroup <= 9)
                //    txtPrintGrp.Text = "3";

                if (printGroup <= 3)
                    hdntxtPrintGrp.Value = "1";
                else if (printGroup >= 4 && printGroup <= 6)
                    hdntxtPrintGrp.Value = "2";
                else if (printGroup >= 7 && printGroup <= 9)
                    hdntxtPrintGrp.Value = "3";

                txtRoyPLGId.Focus();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in calculating print group", ex.Message);
            }
        }

        protected void btnCopyContract_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnCopySelected.Value == "N")
                {
                    string optionCodes = "";
                    string royRates = "";
                    string subRates = "";
                    string packRates = "";
                    string escCodes = "";
                   
                    //WUIN-627 Looping copy options grid and formulating comma seperated string for all option periods and rates 
                    for (int i = 0; i < gvOptionCopy.Rows.Count; i++)
                    {
                        optionCodes = optionCodes + (gvOptionCopy.Rows[i].FindControl("lblOptionCode") as Label).Text + ",";
                    }

                    // WUIN-1265 - Looping copy options escrates grid and formulating comma seperated string for selected esc rates
                    for (int i = 0; i < gvEscCodes.Rows.Count; i++)
                    {
                        escCodes = escCodes + (gvEscCodes.Rows[i].FindControl("lblEscProfileId") as Label).Text + ",";
                    }

                    // for Copy All - rates are same as optioncodes
                    royRates = packRates = optionCodes.TrimEnd(',');

                    //WUIN-1265 - Including '.' to copy subartes which has no option period, as option period code is not mandatory for sub rates
                    subRates = optionCodes + wildCardChar;

                    escCodes = escCodes.TrimEnd(',');

                    CopyContract(optionCodes, royRates, subRates, packRates, escCodes);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in copying contract", ex.Message);
            }


        }


        protected void btnCopyContractSelected_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnCopySelected.Value == "Y")
                {
                    string optionCodes = "";
                    string royRates = "";
                    string subRates = "";
                    string packRates = "";
                    string escCodes = "";

                    CheckBox chkOptionCode;
                    CheckBox chkRoyRates;
                    CheckBox chkSubRates;
                    CheckBox chkPackRates;
                    CheckBox chkscCodes;
                    // WUIN-627 
                    // Looping copy options grid and formulating comma seperated string for selected option periods and rates 
                    for (int i = 0; i < gvOptionCopy.Rows.Count; i++)
                    {
                        chkOptionCode = (CheckBox)gvOptionCopy.Rows[i].FindControl("chkOptionCode");
                        chkRoyRates = (CheckBox)gvOptionCopy.Rows[i].FindControl("chkRoyRates");
                        chkSubRates = (CheckBox)gvOptionCopy.Rows[i].FindControl("chkSubRates");
                        chkPackRates = (CheckBox)gvOptionCopy.Rows[i].FindControl("chkPackRates");
                        if (chkOptionCode.Checked)
                        {
                            optionCodes = optionCodes + (gvOptionCopy.Rows[i].FindControl("lblOptionCode") as Label).Text + ",";
                        }
                        if (chkRoyRates.Checked)
                        {
                            royRates = royRates + (gvOptionCopy.Rows[i].FindControl("lblOptionCode") as Label).Text + ",";
                        }
                        if (chkSubRates.Checked)
                        {
                            subRates = subRates + (gvOptionCopy.Rows[i].FindControl("lblOptionCode") as Label).Text + ",";
                        }
                        if (chkPackRates.Checked)
                        {
                            packRates = packRates + (gvOptionCopy.Rows[i].FindControl("lblOptionCode") as Label).Text + ",";
                        }
                    }
                    // WUIN-627 - Looping copy options escrates grid and formulating comma seperated string for selected esc rates
                    for (int i = 0; i < gvEscCodes.Rows.Count; i++)
                    {
                        chkscCodes = (CheckBox)gvEscCodes.Rows[i].FindControl("chkEscCode");
                        if (chkscCodes.Checked)
                        {
                            escCodes = escCodes + (gvEscCodes.Rows[i].FindControl("lblEscProfileId") as Label).Text + ",";
                        }
                    }

                    optionCodes = optionCodes.TrimEnd(',');
                    royRates = royRates.TrimEnd(',');
                    subRates = subRates.TrimEnd(',');
                    packRates = packRates.TrimEnd(',');
                    escCodes = escCodes.TrimEnd(',');

                    CopyContract(optionCodes, royRates, subRates, packRates, escCodes);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in copying contract", ex.Message);
            }

        }

        protected void valOwner_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (hdnIsValidOwner.Value == "Y" || string.IsNullOrWhiteSpace(txtOwnerSearch.Text))
                {
                    args.IsValid = true;
                }
                else
                {
                    args.IsValid = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in validating owner.", ex.Message);
            }
        }

        protected void btnLock_Click(object sender, EventArgs e)
        {
            try
            {
                string loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                royContractBL = new RoyaltorContractBL();
                DataSet contractData = royContractBL.LockUnlockContact(royaltorId, (btnLock.Text == "Lock" ? "Y" : "N"), loggedUserID, out errorId);
                royContractBL = null;
                hdnChangeNotSaved.Value = string.Empty;
                hdnIsStatusChange.Value = "N";
                if (errorId == 0)
                {
                    if (contractData.Tables.Count != 0 && contractData.Tables[0].Rows.Count != 0)
                    {
                        PopulateData(contractData);
                    }

                    if (btnLock.Text == "Lock")
                    {
                        msgView.SetMessage("Royaltor Contract unlocked", MessageType.Warning, PositionType.Auto);
                    }
                    else
                    {
                        msgView.SetMessage("Royaltor Contract locked", MessageType.Warning, PositionType.Auto);
                    }

                }
                else if (errorId == 2)
                {
                    ExceptionHandler("Error in locking/unlocking contract", string.Empty);
                }
                else
                {
                    ExceptionHandler("Error in saving royaltor contract data", string.Empty);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in locking/unlocking contract", ex.Message);
            }
        }

        protected void btnAddOwner_Click(object sender, EventArgs e)
        {
            try
            {
                royContractBL = new RoyaltorContractBL();
                string newOwnerCode = royContractBL.GetNewOwnerCode(out errorId);
                royContractBL = null;
                if (errorId == 0)
                {
                    txtOwnerCode.Text = newOwnerCode;
                    mpeAddOwner.Show();
                    txtOwnerDesc.Focus();
                }
                else
                {
                    ExceptionHandler("Error in fetching owner code", string.Empty);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fetching owner code", ex.Message);
            }
        }

        protected void btnCopyContractPopup_Click(object sender, EventArgs e)
        {
            try
            {
                royContractBL = new RoyaltorContractBL();
                DataSet dsCopyDetails = royContractBL.GetOptionsForCopyContract(royaltorId, out errorId);
                royContractBL = null;

                var pnlCopyPopupHeight = Convert.ToInt32(hdnWindowHeight.Value) * 0.6;

                //WUIN-627 - Binding data for copy contract pop up
                if (dsCopyDetails.Tables[0].Rows.Count > 0)
                {
                    gvOptionCopy.DataSource = dsCopyDetails.Tables[0];
                    gvOptionCopy.DataBind();
                    if (dsCopyDetails.Tables[0].Rows.Count > 4)
                    {
                        pnlOptionCopy.Style.Add("height", Convert.ToString(pnlCopyPopupHeight * 0.4));
                    }
                }
                else
                {
                    gvOptionCopy.DataSource = null;
                    gvOptionCopy.DataBind();

                    trNoOptions.Visible = true;
                }
                if (dsCopyDetails.Tables[1].Rows.Count > 0)
                {
                    gvEscCodes.DataSource = dsCopyDetails.Tables[1];
                    gvEscCodes.DataBind();
                    if (dsCopyDetails.Tables[1].Rows.Count > 3)
                    {
                        pnlEscCodeCopy.Style.Add("height", Convert.ToString(pnlCopyPopupHeight * 0.2));
                    }
                }
                else
                {
                    pnlEscCodeCopy.Visible = false;
                }
                mpeCopyContract.Show();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fetching option code detials", ex.Message);
            }
        }

        protected void btnCloseScreenLockPopup_Click(object sender, EventArgs e)
        {
            txtScreenLockMsgPopup.Visible = false;
            mpeShowScreenLockMsg.Hide();
        }

        #endregion EVENTS

        #region METHODS

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

            //util = new Utilities();
            //util.GenericExceptionHandler(errorMsg + "<br />" + expMsg);
            //util = null;
        }

        private void PopulateDropDowns()
        {

            royContractBL = new RoyaltorContractBL();
            DataSet dropdownListData = royContractBL.GetDropdownData(out errorId);
            royContractBL = null;

            if (dropdownListData.Tables.Count != 0 && errorId != 2)
            {

                //WUIN-595 -To display the next max owner code when adding a new owner.
                if (Session["FuzzySearchAllOwnerList"] == null)
                {
                    DataSet dsList = new FuzzySearchBL().GetFuzzySearchList("GetAllOwnerList", out errorId);
                    Session["FuzzySearchAllOwnerList"] = dsList.Tables[0];
                }

                ddlCompany.DataTextField = "company_name";
                ddlCompany.DataValueField = "company_code";
                ddlCompany.DataSource = dropdownListData.Tables[0];
                ddlCompany.DataBind();
                ddlCompany.Items.Insert(0, new ListItem("-"));

                ddlLabel.DataTextField = "label";
                ddlLabel.DataValueField = "label_code";
                ddlLabel.DataSource = dropdownListData.Tables[1];
                ddlLabel.DataBind();
                ddlLabel.Items.Insert(0, new ListItem("-"));

                ddlResponsibility.DataTextField = "responsibility";
                ddlResponsibility.DataValueField = "responsibility_code";
                ddlResponsibility.DataSource = dropdownListData.Tables[2];
                ddlResponsibility.DataBind();
                ddlResponsibility.Items.Insert(0, new ListItem("-"));

                ddlReportingSch.DataTextField = "statement_type_code";
                ddlReportingSch.DataValueField = "statement_type_code";
                ddlReportingSch.DataSource = dropdownListData.Tables[3];
                ddlReportingSch.DataBind();
                ddlReportingSch.Items.Insert(0, new ListItem("-"));

                ddlStmtPriority.DataTextField = "priority_desc";
                ddlStmtPriority.DataValueField = "priority_code";
                ddlStmtPriority.DataSource = dropdownListData.Tables[4];
                ddlStmtPriority.DataBind();
                ddlStmtPriority.Items.Insert(0, new ListItem("-"));

                ddlResvTakenOn.DataTextField = "reserve_basis_desc";
                ddlResvTakenOn.DataValueField = "reserve_basis_code";
                ddlResvTakenOn.DataSource = dropdownListData.Tables[5];
                ddlResvTakenOn.DataBind();
                ddlResvTakenOn.Items.Insert(0, new ListItem("-"));

                ddlNettOrSalesUnits.DataTextField = "item_text";
                ddlNettOrSalesUnits.DataValueField = "item_value";
                ddlNettOrSalesUnits.DataSource = dropdownListData.Tables[6];
                ddlNettOrSalesUnits.DataBind();
                ddlNettOrSalesUnits.Items.Insert(0, new ListItem("-"));
                if (royaltorId == null && isNewRoyaltor == null)
                {
                    ddlNettOrSalesUnits.SelectedIndex = 2;
                    hdntxtNettOrSalesUnits.Value = "S";
                }

                ddlStatus.DataTextField = "status_desc";
                ddlStatus.DataValueField = "status_code";
                ddlStatus.DataSource = dropdownListData.Tables[7];
                ddlStatus.DataBind();

                ddlContractType.DataTextField = "contract_type";
                ddlContractType.DataValueField = "contract_type_code";
                ddlContractType.DataSource = dropdownListData.Tables[8];
                ddlContractType.DataBind();
                ddlContractType.Items.Insert(0, new ListItem("-"));
                ddlContractType.SelectedValue = "1";

                ddlStatementFormat.DataTextField = "statement_format_desc";
                ddlStatementFormat.DataValueField = "statement_format_code";
                ddlStatementFormat.DataSource = dropdownListData.Tables[9];
                ddlStatementFormat.DataBind();
                ddlStatementFormat.Items.Insert(0, new ListItem("-"));
                ddlStatementFormat.SelectedValue = "1";
            }
            else
            {
                ExceptionHandler("Error in loading the dropdown list values", string.Empty);
            }

        }

        private void LoadContractData()
        {
            royContractBL = new RoyaltorContractBL();
            DataSet contractData = royContractBL.GetSearchData(royaltorId, out errorId);
            royContractBL = null;

            if (contractData.Tables.Count != 0 && errorId != 2)
            {
                if (contractData.Tables[0].Rows.Count != 0)
                {
                    PopulateData(contractData);
                }


            }
            else
            {
                ExceptionHandler("Error in loading contract data", string.Empty);
            }

        }

        private void PopulateData(DataSet dsContract)
        {
            DataTable dtRoyContract = dsContract.Tables[0];
            DataTable dtResvPlan = dsContract.Tables[1];

            if (dtRoyContract.Rows.Count != 0)
            {
                //hold initial values to compare data changes in a javascript array            
                txtRoyaltorId.Text = hdntxtRoyaltorId.Value = Convert.ToString(dtRoyContract.Rows[0]["royaltor_id"]);
                txtRoyaltorName.Text = hdntxtRoyaltorName.Value = Convert.ToString(dtRoyContract.Rows[0]["royaltor_name"]);
                txtRoyPLGId.Text = hdntxtRoyPLGId.Value = Convert.ToString(dtRoyContract.Rows[0]["royaltor_plg_id"]);
                txtOwnerSearch.Text = hdntxtOwnerSearch.Value = Convert.ToString(dtRoyContract.Rows[0]["owner"]);
                txtExpiryDate.Text = hdntxtExpiryDate.Value = Convert.ToString(dtRoyContract.Rows[0]["contract_expiry_date"]);
                txtStartDate.Text = hdntxtStartDate.Value = Convert.ToString(dtRoyContract.Rows[0]["contract_start_date"]);
                txtRoyaltorSearch.Text = hdntxtRoyaltorSearch.Value = Convert.ToString(dtRoyContract.Rows[0]["chargeble_to"]);
                hdnRoyaltorType.Value = Convert.ToString(dtRoyContract.Rows[0]["royaltor_type_code"]);
                txtChargeablePct.Text = hdntxtChargeablePct.Value = Convert.ToString(dtRoyContract.Rows[0]["parent_contribution_pct"]);
                //txtPrintGrp.Text = hdntxtPrintGrp.Value = Convert.ToString(dtRoyContract.Rows[0]["print_stream"]);
                hdntxtPrintGrp.Value = Convert.ToString(dtRoyContract.Rows[0]["print_stream"]);
                txtResvEndDate.Text = hdntxtResvEndDate.Value = Convert.ToString(dtRoyContract.Rows[0]["reserve_end_date"]);
                txtDefaultResvPct.Text = hdntxtDefaultResvPct.Value = Convert.ToString(dtRoyContract.Rows[0]["reserve_pct"]);
                hdnOwner.Value = txtOwnerSearch.Text == string.Empty ? "" : txtOwnerSearch.Text.Substring(0, txtOwnerSearch.Text.IndexOf("-") - 1);
                ddlContractType.SelectedValue = hdnddlContractType.Value = Convert.ToString(dtRoyContract.Rows[0]["contract_type_code"]);
                ddlStatementFormat.SelectedValue = hdnddlStatementFormat.Value = Convert.ToString(dtRoyContract.Rows[0]["statement_format_code"]);
                //JIRA-970 Changes by ravi on 15/02/2019 -- Start
                txtSocSecNo.Text = hdnSocSecNo.Value = Convert.ToString(dtRoyContract.Rows[0]["soc_sec_no"]);
                //JIRA-970 Changes by ravi on 15/02/2019 -- End

                if (Convert.ToString(dtRoyContract.Rows[0]["reserve_sales_flag"]) != string.Empty)
                {
                    ddlNettOrSalesUnits.SelectedValue = hdntxtNettOrSalesUnits.Value = Convert.ToString(dtRoyContract.Rows[0]["reserve_sales_flag"]);
                }
                else
                {
                    ddlNettOrSalesUnits.SelectedIndex = 0;
                    hdntxtNettOrSalesUnits.Value = string.Empty;
                }

                if (Convert.ToString(dtRoyContract.Rows[0]["status_code"]) != string.Empty)
                {
                    ddlStatus.SelectedValue = hdnStatusCode.Value = Convert.ToString(dtRoyContract.Rows[0]["status_code"]);

                }
                else
                {
                    ddlStatus.SelectedIndex = 0;
                    hdnStatusCode.Value = string.Empty;
                }

                if (Convert.ToString(dtRoyContract.Rows[0]["company_code"]) != string.Empty)
                {
                    ddlCompany.SelectedValue = hdnddlCompany.Value = Convert.ToString(dtRoyContract.Rows[0]["company_code"]);
                }
                else
                {
                    ddlCompany.SelectedIndex = 0;
                    hdnddlCompany.Value = string.Empty;
                }

                if (Convert.ToString(dtRoyContract.Rows[0]["label_code"]) != string.Empty)
                {
                    ddlLabel.SelectedValue = hdnddlLabel.Value = Convert.ToString(dtRoyContract.Rows[0]["label_code"]);

                }
                else
                {
                    ddlLabel.SelectedIndex = 0;
                    hdnddlLabel.Value = string.Empty;
                }

                if (Convert.ToString(dtRoyContract.Rows[0]["responsibility_code"]) != string.Empty)
                {
                    ddlResponsibility.SelectedValue = hdnResponsibility.Value = hdnddlResponsibility.Value = Convert.ToString(dtRoyContract.Rows[0]["responsibility_code"]);
                }
                else
                {
                    ddlResponsibility.SelectedIndex = 0;
                    hdnResponsibility.Value = string.Empty;
                }

                if (Convert.ToString(dtRoyContract.Rows[0]["statement_type_code"]) != string.Empty)
                {
                    ddlReportingSch.SelectedValue = hdnddlReportingSch.Value = Convert.ToString(dtRoyContract.Rows[0]["statement_type_code"]);
                }
                else
                {
                    ddlReportingSch.SelectedIndex = 0;
                    hdnddlReportingSch.Value = string.Empty;
                }

                if (Convert.ToString(dtRoyContract.Rows[0]["priority_code"]) != string.Empty)
                {
                    ddlStmtPriority.SelectedValue = hdnddlStmtPriority.Value = Convert.ToString(dtRoyContract.Rows[0]["priority_code"]);
                }
                else
                {
                    ddlStmtPriority.SelectedIndex = 0;
                    hdnddlStmtPriority.Value = string.Empty;
                }

                if (Convert.ToString(dtRoyContract.Rows[0]["reserve_prod_type"]) != string.Empty)
                {
                    ddlResvTakenOn.SelectedValue = hdnddlResvTakenOn.Value = Convert.ToString(dtRoyContract.Rows[0]["reserve_prod_type"]);
                }
                else
                {
                    ddlResvTakenOn.SelectedIndex = 0;
                    hdnddlResvTakenOn.Value = string.Empty;
                }


                if (Convert.ToString(dtRoyContract.Rows[0]["royaltor_held"]) == "Y")
                {
                    cbHeld.Checked = true;
                    hdncbHeld.Value = "Y";
                }
                else
                {
                    cbHeld.Checked = false;
                    hdncbHeld.Value = "N";
                }

                if (Convert.ToString(dtRoyContract.Rows[0]["send_to_portal"]) == "Y")
                {
                    cbSendToPortal.Checked = true;
                    hdncbSendToPortal.Value = "Y";
                }
                else
                {
                    cbSendToPortal.Checked = false;
                    hdncbSendToPortal.Value = "N";
                }
                //JIRA-1006 CHanges by Ravi -- Start
                if (Convert.ToString(dtRoyContract.Rows[0]["exclude_from_accrual"]) == "Y")
                {
                    cbExcludeFromAccrual.Checked = true;
                    hdncbExcludeFromAccrual.Value = "Y";
                }
                else
                {
                    cbExcludeFromAccrual.Checked = false;
                    hdncbExcludeFromAccrual.Value = "N";
                }
                //JIRA-1006 CHanges by Ravi -- End

                string lockedSession = royaltorId + "-" + "RoyaltorLocked";
                if (Convert.ToString(dtRoyContract.Rows[0]["royaltor_locked"]) == "Y")
                {
                    cbLock.Checked = true;
                    hdncbLock.Value = "Y";
                    btnLock.Text = "Unlock";

                    //WUIN-517
                    btnSave.Enabled = false;
                    btnSave.ToolTip = "Royaltor Locked";

                    //WUIN-450 - If a new Royaltor is set up with Lock checked it should continue to allow entry of the details (the Royaltor set up needs to be completed!)
                    if (isNewRoyaltor == "Y")
                    {
                        btnSave.Enabled = true;
                        btnSave.ToolTip = string.Empty;

                        //WUIN-588
                        //btnCopyContractPopup.Enabled = false;
                        //btnCopyContractPopup.ToolTip = "Royaltor Locked";
                    }

                    //create a dynamic session variable to hold locked status for this royaltor
                    //this is needed to handle different royaltors opened in  different browser tabs
                    Session[lockedSession] = "Y";


                }
                else
                {
                    btnSave.Enabled = true;
                    cbLock.Checked = false;
                    hdncbLock.Value = "N";
                    btnLock.Text = "Lock";

                    if (Session[lockedSession] != null)
                    {
                        Session[lockedSession] = null;
                    }
                }

                if (Convert.ToString(dtRoyContract.Rows[0]["signed"]) == "Y")
                {
                    cbSigned.Checked = true;
                    hdncbSigned.Value = "Y";
                }
                else
                {
                    cbSigned.Checked = false;
                    hdncbSigned.Value = "N";
                }

                if (Convert.ToString(dtRoyContract.Rows[0]["stmt_display_zero"]) == "Y")
                {
                    cbDisplayZero.Checked = true;
                    hdncbDisplayZero.Value = "Y";
                }
                else
                {
                    cbDisplayZero.Checked = false;
                    hdncbDisplayZero.Value = "N";
                }

                if (Convert.ToString(dtRoyContract.Rows[0]["stmt_producer_report"]) == "Y")
                {
                    cbProducerSumm.Checked = true;
                    hdncbProducerSumm.Value = "Y";
                }
                else
                {
                    cbProducerSumm.Checked = false;
                    hdncbProducerSumm.Value = "N";
                }

                if (Convert.ToString(dtRoyContract.Rows[0]["stmt_cost_report"]) == "Y")
                {
                    cbCostSummary.Checked = true;
                    hdncbCostSummary.Value = "Y";
                }
                else
                {
                    cbCostSummary.Checked = false;
                    hdncbCostSummary.Value = "N";
                }



                //WUIN-599 -- Only one user can use contract screens on same time.
                // If a contract already using by another user then making the screen readonly.

                string isScreenLocked = "";
                string screenLockedUser = "";
                Session["ScreenLockedRoyaltorId"] = royaltorId; // To use in web method for unset the screen locked flag

                if (Session["ContractScreenLocked"] == null && Session["ContractScreenLockedUser"] == null)
                {
                    isScreenLocked = Convert.ToString(dtRoyContract.Rows[0]["screen_locked"]);
                    screenLockedUser = Convert.ToString(dtRoyContract.Rows[0]["screen_locked_user"]);
                    Session["ContractScreenLocked"] = isScreenLocked;
                    Session["ContractScreenLockedUser"] = screenLockedUser == "" ? Session["UserCode"].ToString() : screenLockedUser;
                }
                else
                {
                    isScreenLocked = Convert.ToString(Session["ContractScreenLocked"]);
                    screenLockedUser = Convert.ToString(Session["ContractScreenLockedUser"]);
                }

                if (isScreenLocked == "N")
                {
                    RoyaltorContractBL royContractBL = new RoyaltorContractBL();
                    royContractBL.UpdateScreenLockFlag(royaltorId, "Y", Session["UserCode"].ToString(), out errorId);

                    if (errorId == 2)
                    {
                        ExceptionHandler("Error in updating screen lock flag", string.Empty);
                    }
                }
                else if (isScreenLocked == "Y" && Session["UserCode"].ToString() != screenLockedUser)
                {
                    EnableReadonly();
                    hdnOtherUserScreenLocked.Value = "Y";
                    //Showing the screen lock message only for first time and not showing when redirect back from contract screens and audit screen.
                    string priviousUrl = Convert.ToString(Request.UrlReferrer);
                    if (!(priviousUrl.Contains("RoyContractAudit") || priviousUrl.Contains("RoyContractPayee") || priviousUrl.Contains("RoyContractPayeeSupp") ||
                        priviousUrl.Contains("RoyContractOptionPeriods") || priviousUrl.Contains("RoyContractRoyRates") || priviousUrl.Contains("RoyContractSubRates")
                        || priviousUrl.Contains("RoyContractPkgRates") || priviousUrl.Contains("RoyContractEscRates") || priviousUrl.Contains("RoyContractGrouping")
                        || priviousUrl.Contains("RoyContractTaxDetails") || priviousUrl.Contains("RoyContractNotes")))
                    {
                        lblScreenLockMessage.Text = "This contract cannot be edited as user '" + screenLockedUser + "' working on this.";
                        mpeShowScreenLockMsg.Show();
                        txtScreenLockMsgPopup.Visible = true;
                        txtScreenLockMsgPopup.Focus();

                    }
                }

            }

            //WUIN - 873
            hdntxtLiqPct1.Value = hdntxtLiqPct2.Value = hdntxtLiqPct3.Value = hdntxtLiqPct4.Value = hdntxtLiqPct5.Value = hdntxtLiqPct6.Value = hdntxtLiqPct7.Value = hdntxtLiqPct8.Value = null;

            if (dtResvPlan.Rows.Count != 0)
            {
                string resvInterval = string.Empty;
                foreach (DataRow row in dtResvPlan.Rows)
                {
                    if (Convert.ToString(row["reserve_interval"]) == "1")
                    {
                        txtLiqPct1.Text = hdntxtLiqPct1.Value = Convert.ToString(row["liquidation_pct"]);
                    }
                    else if (Convert.ToString(row["reserve_interval"]) == "2")
                    {
                        txtLiqPct2.Text = hdntxtLiqPct2.Value = Convert.ToString(row["liquidation_pct"]);
                    }
                    else if (Convert.ToString(row["reserve_interval"]) == "3")
                    {
                        txtLiqPct3.Text = hdntxtLiqPct3.Value = Convert.ToString(row["liquidation_pct"]);
                    }
                    else if (Convert.ToString(row["reserve_interval"]) == "4")
                    {
                        txtLiqPct4.Text = hdntxtLiqPct4.Value = Convert.ToString(row["liquidation_pct"]);
                    }
                    else if (Convert.ToString(row["reserve_interval"]) == "5")
                    {
                        txtLiqPct5.Text = hdntxtLiqPct5.Value = Convert.ToString(row["liquidation_pct"]);
                    }
                    else if (Convert.ToString(row["reserve_interval"]) == "6")
                    {
                        txtLiqPct6.Text = hdntxtLiqPct6.Value = Convert.ToString(row["liquidation_pct"]);
                    }
                    else if (Convert.ToString(row["reserve_interval"]) == "7")
                    {
                        txtLiqPct7.Text = hdntxtLiqPct7.Value = Convert.ToString(row["liquidation_pct"]);
                    }
                    else if (Convert.ToString(row["reserve_interval"]) == "8")
                    {
                        txtLiqPct8.Text = hdntxtLiqPct8.Value = Convert.ToString(row["liquidation_pct"]);
                    }
                }
            }

        }

        private void FuzzySearchOwner()
        {
            if (txtOwnerSearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text owner search field", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "Owner";

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllOwnerList(txtOwnerSearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void FuzzySearchRoyaltor()
        {
            if (txtRoyaltorSearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in royaltor search field", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "Royaltor";

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllRoyListWithOwnerCode(txtRoyaltorSearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private Array ContractDetails()
        {
            List<string> contractDetails = new List<string>();
            contractDetails.Add("royaltor_id" + Global.DBDelimiter + txtRoyaltorId.Text);
            contractDetails.Add("royaltor_name" + Global.DBDelimiter + txtRoyaltorName.Text);
            contractDetails.Add("royaltor_plg_id" + Global.DBDelimiter + txtRoyPLGId.Text);
            contractDetails.Add("status_code" + Global.DBDelimiter + (ddlStatus.SelectedValue == "-" ? "-" : ddlStatus.SelectedValue));
            contractDetails.Add("company_code" + Global.DBDelimiter + (ddlCompany.SelectedValue == "-" ? "-" : ddlCompany.SelectedValue));
            contractDetails.Add("owner_code" + Global.DBDelimiter + (txtOwnerSearch.Text == string.Empty ? "-" : txtOwnerSearch.Text.Substring(0, txtOwnerSearch.Text.IndexOf("-") - 1)));
            contractDetails.Add("label_code" + Global.DBDelimiter + (ddlLabel.SelectedValue == "-" ? "-" : ddlLabel.SelectedValue));
            contractDetails.Add("responsibility_code" + Global.DBDelimiter + (ddlResponsibility.SelectedValue == "-" ? "-" : ddlResponsibility.SelectedValue));
            contractDetails.Add("contract_type_code" + Global.DBDelimiter + (ddlContractType.SelectedValue == "-" ? "-" : ddlContractType.SelectedValue));
            contractDetails.Add("statement_format_code" + Global.DBDelimiter + (ddlStatementFormat.SelectedValue == "-" ? "-" : ddlStatementFormat.SelectedValue));
            contractDetails.Add("royaltor_held" + Global.DBDelimiter + (cbHeld.Checked == true ? "Y" : "N"));
            contractDetails.Add("send_to_portal" + Global.DBDelimiter + (cbSendToPortal.Checked == true ? "Y" : "N"));
            //JIRA-1006 Changes by Ravi -- Start
            contractDetails.Add("exclude_from_accrual" + Global.DBDelimiter + (cbExcludeFromAccrual.Checked == true ? "Y" : "N"));
            //JIRA-1006 Changes by Ravi -- Ends
            contractDetails.Add("royaltor_locked" + Global.DBDelimiter + (cbLock.Checked == true ? "Y" : "N"));
            contractDetails.Add("signed" + Global.DBDelimiter + (cbSigned.Checked == true ? "Y" : "N"));
            contractDetails.Add("contract_expiry_date" + Global.DBDelimiter + (txtExpiryDate.Text == string.Empty ? "-" : txtExpiryDate.Text));
            contractDetails.Add("contract_start_date" + Global.DBDelimiter + (txtStartDate.Text == string.Empty ? "-" : txtStartDate.Text));
            contractDetails.Add("parent_royaltor_id" + Global.DBDelimiter + (txtRoyaltorSearch.Text == string.Empty ? "-" : txtRoyaltorSearch.Text.Substring(0, txtRoyaltorSearch.Text.IndexOf("-") - 1)));
            contractDetails.Add("royaltor_type_code" + Global.DBDelimiter + hdnRoyaltorType.Value);
            contractDetails.Add("parent_contribution_pct" + Global.DBDelimiter + (txtChargeablePct.Text == string.Empty ? "-" : txtChargeablePct.Text));
            contractDetails.Add("statement_type_code" + Global.DBDelimiter + (ddlReportingSch.SelectedValue == "-" ? "-" : ddlReportingSch.SelectedValue));
            contractDetails.Add("priority_code" + Global.DBDelimiter + (ddlStmtPriority.SelectedValue == "-" ? "-" : ddlStmtPriority.SelectedValue));
            contractDetails.Add("print_stream" + Global.DBDelimiter + (hdntxtPrintGrp.Value == string.Empty ? "-" : hdntxtPrintGrp.Value));
            contractDetails.Add("stmt_display_zero" + Global.DBDelimiter + (cbDisplayZero.Checked == true ? "Y" : "N"));
            contractDetails.Add("stmt_producer_report" + Global.DBDelimiter + (cbProducerSumm.Checked == true ? "Y" : "N"));
            contractDetails.Add("stmt_cost_report" + Global.DBDelimiter + (cbCostSummary.Checked == true ? "Y" : "N"));
            contractDetails.Add("reserve_prod_type" + Global.DBDelimiter + (ddlResvTakenOn.SelectedValue == "-" ? "-" : ddlResvTakenOn.SelectedValue));
            contractDetails.Add("reserve_end_month_id" + Global.DBDelimiter + ((txtResvEndDate.Text == string.Empty || txtResvEndDate.Text == "__/____") ? "-" : txtResvEndDate.Text));
            contractDetails.Add("reserve_pct" + Global.DBDelimiter + (txtDefaultResvPct.Text == string.Empty ? "-" : txtDefaultResvPct.Text));
            contractDetails.Add("reserve_sales_flag" + Global.DBDelimiter + (ddlNettOrSalesUnits.SelectedValue == "-" ? "-" : ddlNettOrSalesUnits.SelectedValue));
            //JIRA-970 Changes by ravi on 15/02/2019 -- Start
            contractDetails.Add("soc_sec_no" + Global.DBDelimiter + txtSocSecNo.Text);
            //JIRA-970 Changes by ravi on 15/02/2019 -- End
            return contractDetails.ToArray();
        }

        private Array LiquidationDetails()
        {
            List<string> liquidationDetails = new List<string>();
            liquidationDetails.Add("1" + Global.DBDelimiter + (txtLiqPct1.Text.Trim() == string.Empty ? "-" : txtLiqPct1.Text.Trim()));
            liquidationDetails.Add("2" + Global.DBDelimiter + (txtLiqPct2.Text.Trim() == string.Empty ? "-" : txtLiqPct2.Text.Trim()));
            liquidationDetails.Add("3" + Global.DBDelimiter + (txtLiqPct3.Text.Trim() == string.Empty ? "-" : txtLiqPct3.Text.Trim()));
            liquidationDetails.Add("4" + Global.DBDelimiter + (txtLiqPct4.Text.Trim() == string.Empty ? "-" : txtLiqPct4.Text.Trim()));
            liquidationDetails.Add("5" + Global.DBDelimiter + (txtLiqPct5.Text.Trim() == string.Empty ? "-" : txtLiqPct5.Text.Trim()));
            liquidationDetails.Add("6" + Global.DBDelimiter + (txtLiqPct6.Text.Trim() == string.Empty ? "-" : txtLiqPct6.Text.Trim()));
            liquidationDetails.Add("7" + Global.DBDelimiter + (txtLiqPct7.Text.Trim() == string.Empty ? "-" : txtLiqPct7.Text.Trim()));
            liquidationDetails.Add("8" + Global.DBDelimiter + (txtLiqPct8.Text.Trim() == string.Empty ? "-" : txtLiqPct8.Text.Trim()));
            return liquidationDetails.ToArray();
        }

        private void SaveContract()
        {
            string loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
            royContractBL = new RoyaltorContractBL();
            DataSet contractData = royContractBL.AddUpateContract(royaltorId, isRespChanged, isOwnerChanged, ContractDetails(), LiquidationDetails(), loggedUserID, out errorId);
            royContractBL = null;
            hdnChangeNotSaved.Value = string.Empty;
            hdnIsStatusChange.Value = "N";
            if (errorId == 0)
            {
                

                //new royaltor - redirect to payee screen
                //existing royaltor - remain in contract screen
                if ((royaltorId == null || royaltorId == string.Empty) || (royaltorId != null && isNewRoyaltor == "Y"))//Add
                {
                    if (contractData.Tables.Count != 0 && contractData.Tables[0].Rows.Count != 0)
                    {
                        royaltorId = royaltorId == null ? contractData.Tables[0].Rows[0]["royaltor_id"].ToString() : royaltorId;
                        PopulateData(contractData);
                    }

                    //WUIN-450
                    //set screen button enabled = Y
                    contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.ContractDetails.ToString());

                    //redirect to payee screen                
                    //Response.Redirect(@"~/Contract/RoyContractPayee.aspx?RoyaltorId=" + Convert.ToString(contractData.Tables[0].Rows[0]["royaltor_id"]) + "&isNewRoyaltor=Y", false);
                    //redirect in javascript so that issue of data not saved validation would be handled
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + Convert.ToString(contractData.Tables[0].Rows[0]["royaltor_id"]) + ");", true);
                }
                else
                {
                    if (contractData.Tables.Count != 0 && contractData.Tables[0].Rows.Count != 0)
                    {
                        PopulateData(contractData);
                    }
                    msgView.SetMessage("Royaltor Contract details saved", MessageType.Warning, PositionType.Auto);
                }

            }
            else if (errorId == 1)
            {
                lblOwnChangeMsg.Text = "Selected owner has different responsibility. Selected responsibility will be overridden!";
                mpeConfirmOwnerChange.Show();
            }
            else if (errorId == 2)
            {
                ExceptionHandler("Error in saving royaltor contract data", string.Empty);
            }
            else if (errorId == 3)
            {
                msgView.SetMessage("Royaltors under selected owner have different responsibilities. Cannot proceed!", MessageType.Warning, PositionType.Auto);
            }
            else if (errorId == 4)
            {
                msgView.SetMessage("Royaltor number - " + royaltorId + " already exists!", MessageType.Warning, PositionType.Auto);
            }
            else
            {
                ExceptionHandler("Error in saving royaltor contract data", string.Empty);
            }

            //WUIN-1076 changes
            //reset fuzzy search sessions to null to fetch the new list on any changes to royaltor data
            Session["FuzzySearchAllRoyaltorList"] = null;
            Session["FuzzySearchAllRoyListWithOwnerCode"] = null;

        }



        public void CopyContract(string optionCodes, string royRates, string subRates, string packRates, string escCodes)
        {
            try
            {
                string loggedUserID = Convert.ToString(Session["UserCode"]);
                royContractBL = new RoyaltorContractBL();
                royContractBL.CopyContract(royaltorId, txtRoyaltorIdCopyCont.Text, txtRoyaltorNameCopyCont.Text, loggedUserID, optionCodes, royRates, subRates, packRates, escCodes, out errorId);
                royContractBL = null;

                if (errorId == 0)
                {
                    msgView.SetMessage("Contract copied", MessageType.Warning, PositionType.Auto);
                    ClearCopyContract();
                    mpeCopyContract.Hide();
                }
                else if (errorId == 1)
                {
                    mpeCopyContract.Show();
                    msgView.SetMessage("Royaltor number - " + txtRoyaltorIdCopyCont.Text + " already exists!", MessageType.Warning, PositionType.Auto);
                }
                else if (errorId == 2)
                {
                    txtRoyaltorIdCopyCont.Text = string.Empty;
                    txtRoyaltorNameCopyCont.Text = string.Empty;
                    mpeCopyContract.Hide();
                    ExceptionHandler("Error in copying contract", string.Empty);
                }
            }

            catch (Exception ex)
            {
                ExceptionHandler("Error in copying contract", ex.Message);
            }

        }


        private void ClearCopyContract()
        {
            txtRoyaltorIdCopyCont.Text = string.Empty;
            txtRoyaltorNameCopyCont.Text = string.Empty;
            hdnCopySelected.Value = "N";
            chkAllOptions.Checked = chkAllRoyRates.Checked = chkAllSubRates.Checked = chkAllPackRates.Checked = chkEscRates.Checked = false;
            for (int i = 0; i < gvOptionCopy.Rows.Count; i++)
            {
                ((CheckBox)gvOptionCopy.Rows[i].FindControl("chkOptionCode")).Checked = false;
                ((CheckBox)gvOptionCopy.Rows[i].FindControl("chkRoyRates")).Checked = false;
                ((CheckBox)gvOptionCopy.Rows[i].FindControl("chkSubRates")).Checked = false;
                ((CheckBox)gvOptionCopy.Rows[i].FindControl("chkPackRates")).Checked = false;

            }
            for (int i = 0; i < gvEscCodes.Rows.Count; i++)
            {
                ((CheckBox)gvEscCodes.Rows[i].FindControl("chkEscCode")).Checked = false;
            }

        }


        /// <summary>
        ///WUIN-568 - Only superusers should be able to unlock and lock contracts
        ///WUIN-589 - enable Lock button only for Super user and if not a new royaltor
        ///WUIN-1114 - Any user irrespective of the user role can amend changes on a contract that is at Manager sign off
        /// </summary>
        private void UserAuthorization()
        {
            btnLock.Enabled = false;
            cbLock.Enabled = false;

            hdnUserRole.Value = Session["UserRole"].ToString();

            if (Session["UserRole"].ToString().ToLower() == UserRole.SuperUser.ToString().ToLower() && hdnOtherUserScreenLocked.Value == "N")
            {
                cbLock.Enabled = true;
                if (royaltorId != null && isNewRoyaltor != "Y")
                {
                    btnLock.Enabled = true;
                }
            }
            else if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                EnableReadonly();
            }
            else
            {
                if (hdnStatusCode.Value == "3")
                {
                    btnAddOwner.Enabled = false;
                }

            }

        }

        private void EnableReadonly()
        {
            btnAddOwner.Enabled = false;
            btnSave.Enabled = false;
            btnLock.Enabled = false;
            btnCopyContractPopup.Enabled = false;
        }

        #endregion METHODS

        #region Web Methods

        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod(EnableSession = true)]
        public static void UpdateScreenLockFlag()
        {
            try
            {
                int errorId;
                RoyaltorContractBL royContractBL = new RoyaltorContractBL();
                royContractBL.UpdateScreenLockFlag(HttpContext.Current.Session["ScreenLockedRoyaltorId"].ToString(), "N", HttpContext.Current.Session["UserCode"].ToString(), out errorId);
                royContractBL = null;
            }
            catch { }


        }

        #endregion Web Methods

    }
}
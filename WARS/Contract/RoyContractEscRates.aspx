<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoyContractEscRates.aspx.cs" Inherits="WARS.Contract.RoyContractEscRates" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Royaltor Contract Escalation Rates" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>
<%@ Register TagPrefix="ContNav" TagName="ContractNavigation" Src="~/Contract/ContractNavigationButtons.ascx" %>
<%@ Register TagPrefix="ContHdrNav" TagName="ContractHdrNavigation" Src="~/Contract/ContractHdrLinkButtons.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <ContHdrNav:ContractHdrNavigation ID="contractHdrNavigation" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderBody">
    <script type="text/javascript">
        //Global variables
        var gridClientId = "ContentPlaceHolderBody_gvEscRates_";

        //probress bar and scroll position functionality - starts
        //to remain scroll position of grid panel and window
        var xPos, yPos;
        var scrollTop;
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        //Raised before processing of an asynchronous postback starts and the postback request is sent to the server.
        prm.add_beginRequest(BeginRequestHandler);
        // Raised after an asynchronous postback is finished and control has been returned to the browser.
        prm.add_endRequest(EndRequestHandler);

        function BeginRequestHandler(sender, args) {
            //Shows the modal popup - the update progress
            var popup = $find('<%= mPopupPageLevel.ClientID %>');
            if (popup != null) {
                popup.show();
            }

            //to maintain scroll position

        }

        function EndRequestHandler(sender, args) {
            //Hide the modal popup - the update progress
            var popup = $find('<%= mPopupPageLevel.ClientID %>');
            if (popup != null) {
                popup.hide();
            }

            //to maintain scroll position

        }
        //======================= End


        //grid panel height adjustment functioanlity - starts
        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
            document.getElementById("<%=PnlGrid.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
        }
        //======================= End

        //Tab key to remain only on Add row fields
        function OnAddRowUndoTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= txtRoyaltorId.ClientID %>").focus();
            }
        }

        //=============== End

        function OnRoyaltorIdTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= txtRoyaltorId.ClientID %>").focus();
            }
        }

        //Undo Grid changes
        function UndoGridChanges(gridRow) {
            gridRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);

            var hdnEscCode = document.getElementById(gridClientId + 'hdnEscCode' + '_' + gridRowIndex);
            var txtEscCode = document.getElementById(gridClientId + 'txtEscCode' + '_' + gridRowIndex);
            var hdnSellerGrp = document.getElementById(gridClientId + 'hdnSellerGrp' + '_' + gridRowIndex);
            var txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + gridRowIndex);
            var hdnConfigGrp = document.getElementById(gridClientId + 'hdnConfigGrp' + '_' + gridRowIndex);
            var txtConfig = document.getElementById(gridClientId + 'txtConfig' + '_' + gridRowIndex);
            var hdnPriceGrp = document.getElementById(gridClientId + 'hdnPriceGrp' + '_' + gridRowIndex);
            var txtSalesType = document.getElementById(gridClientId + 'txtSalesType' + '_' + gridRowIndex);
            var hdnSalesTrigger = document.getElementById(gridClientId + 'hdnSalesTrigger' + '_' + gridRowIndex);
            var txtSalesTrigger = document.getElementById(gridClientId + 'txtSalesTrigger' + '_' + gridRowIndex);
            var hdnValueTrigger = document.getElementById(gridClientId + 'hdnValueTrigger' + '_' + gridRowIndex);
            var txtValueTrigger = document.getElementById(gridClientId + 'txtValueTrigger' + '_' + gridRowIndex);
            var hdnSalesPct = document.getElementById(gridClientId + 'hdnSalesPct' + '_' + gridRowIndex);
            var txtPctSales = document.getElementById(gridClientId + 'txtPctSales' + '_' + gridRowIndex);
            var hdnRoyaltyRate = document.getElementById(gridClientId + 'hdnRoyaltyRate' + '_' + gridRowIndex);
            var txtRoyaltyRate = document.getElementById(gridClientId + 'txtRoyaltyRate' + '_' + gridRowIndex);
            var hdnUnitRate = document.getElementById(gridClientId + 'hdnUnitRate' + '_' + gridRowIndex);
            var txtUnitRate = document.getElementById(gridClientId + 'txtUnitRate' + '_' + gridRowIndex);
            var hdnRevenueRate = document.getElementById(gridClientId + 'hdnRevenueRate' + '_' + gridRowIndex);
            var txtRevenueRate = document.getElementById(gridClientId + 'txtRevenueRate' + '_' + gridRowIndex);
            var hdnIsModified = document.getElementById(gridClientId + 'hdnIsModified' + '_' + gridRowIndex);

            txtEscCode.value = hdnEscCode.value;
            txtTerritory.value = hdnSellerGrp.value;
            txtConfig.value = hdnConfigGrp.value;
            txtSalesType.value = hdnPriceGrp.value;
            txtSalesTrigger.value = hdnSalesTrigger.value;
            txtValueTrigger.value = hdnValueTrigger.value;
            txtPctSales.value = hdnSalesPct.value;
            txtRoyaltyRate.value = hdnRoyaltyRate.value;
            txtUnitRate.value = hdnUnitRate.value;
            txtRevenueRate.value = hdnRevenueRate.value;

            Page_ClientValidate('');//clear all validators of the page

            txtTerritory.style["width"] = '98%';
            txtConfig.style["width"] = '98%';
            txtSalesType.style["width"] = '98%';

            return false;

        }

        //============== End

        //clear add row data
        function ClearAddRow() {
            document.getElementById('<%=txtEscCodeAddRow.ClientID%>').value = "";
            document.getElementById('<%=txtTerritoryAddRow.ClientID%>').value = "";
            document.getElementById('<%=txtConfigAddRow.ClientID%>').value = "";
            document.getElementById('<%=txtSalesTypeAddRow.ClientID%>').value = "";
            document.getElementById('<%=txtSalesTriggerAddRow.ClientID%>').value = "";
            document.getElementById('<%=txtValueTriggerAddRow.ClientID%>').value = "";
            document.getElementById('<%=txtPctSalesAddRow.ClientID%>').value = "";
            document.getElementById('<%=txtRoyaltyRateAddRow.ClientID%>').value = "";
            document.getElementById('<%=txtUnitRateAddRow.ClientID%>').value = "";
            document.getElementById('<%=txtRevenueRateAddRow.ClientID%>').value = "";
            Page_ClientValidate('');//clear all validators of the page
            document.getElementById("<%= txtEscCodeAddRow.ClientID %>").focus();

            //reset field width
            txtTerritoryAddRow = document.getElementById("<%=txtTerritoryAddRow.ClientID %>");
            txtConfigAddRow = document.getElementById("<%=txtConfigAddRow.ClientID %>");
            txtSalesTypeAddRow = document.getElementById("<%=txtSalesTypeAddRow.ClientID %>");
            txtTerritoryAddRow.style["width"] = '98%';
            txtConfigAddRow.style["width"] = '98%';
            txtSalesTypeAddRow.style["width"] = '98%';

            return false;

        }
        //============== End  


        //Audit button navigation
        function RedirectToAuditScreen(royaltorId) {
            //debugger;        
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Audit/RoyaltorEscalationRatesAudit.aspx?RoyaltorId=" + royaltorId + "");
            }
            else {
                window.location = "../Audit/RoyaltorEscalationRatesAudit.aspx?RoyaltorId=" + royaltorId + "";
            }
        }

        function RedirectToPreviousScreen(royaltorId) {
            //debugger;        
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Contract/RoyContractPkgRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y");
            }
            else {
                window.location = "../Contract/RoyContractPkgRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y";
            }
        }
        //=================End

        //Confim delete
        function ConfirmDelete(row) {
            //JIRA-908 Changes by Ravi on 12/02/2019 -- Start
            gridRowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            hdnIsModified = document.getElementById(gridClientId + 'hdnIsModified' + '_' + gridRowIndex).value;
            hdnEscProfileId = document.getElementById(gridClientId + 'hdnEscProfileId' + '_' + gridRowIndex).value;
            hdnEscLevel = document.getElementById(gridClientId + 'hdnEscLevel' + '_' + gridRowIndex).value;
            if (hdnIsModified != "-") {
                document.getElementById("<%=hdnGridDataDeleted.ClientID %>").innerText = "Y";
            }
            document.getElementById("<%=hdnDeleteEscProfileId.ClientID %>").innerText = hdnEscProfileId;
            document.getElementById("<%=hdnDeleteEscLevel.ClientID %>").innerText = hdnEscLevel;
            document.getElementById("<%=hdnDeleteIsModified.ClientID %>").innerText = hdnIsModified;
            var popup = $find('<%= mpeConfirmDelete.ClientID %>');
            if (popup != null) {
                popup.show();
            }
            return false;
            //JIRA-908 Changes by Ravi on 12/02/2019 -- End
        }
        //============== End

        //Validate any unsaved data on browser window close/refresh
        //set flag value when data is changed

        function IsAddRowDataChanged() {
            var txtEscCodeAddRow = document.getElementById("<%=txtEscCodeAddRow.ClientID %>").value;
            var txtTerritoryAddRow = document.getElementById("<%=txtTerritoryAddRow.ClientID %>").value;
            var txtConfigAddRow = document.getElementById("<%=txtConfigAddRow.ClientID %>").value;
            var txtSalesTypeAddRow = document.getElementById("<%=txtSalesTypeAddRow.ClientID %>").value;
            var txtSalesTriggerAddRow = document.getElementById("<%=txtSalesTriggerAddRow.ClientID %>").value;
            var txtPctSalesAddRow = document.getElementById("<%=txtPctSalesAddRow.ClientID %>").value;
            var txtRoyaltyRateAddRow = document.getElementById("<%=txtRoyaltyRateAddRow.ClientID %>").value;
            var txtUnitRateAddRow = document.getElementById("<%=txtUnitRateAddRow.ClientID %>").value;
            var txtRevenueRateAddRow = document.getElementById("<%=txtRevenueRateAddRow.ClientID %>").value;

            if (txtEscCodeAddRow != '' || txtTerritoryAddRow != '' || txtConfigAddRow != '' || txtSalesTypeAddRow != '' || txtSalesTriggerAddRow != ''
                || txtPctSalesAddRow != '' || txtRoyaltyRateAddRow != '' || txtUnitRateAddRow != '' || txtRevenueRateAddRow != ''
                ) {
                return true;
            }
            else {
                return false;
            }

        }

        function IsGridDataChanged() {
            var gvEscRates = document.getElementById("<%= gvEscRates.ClientID %>");
            var hdnGridDataDeleted = document.getElementById("<%=hdnGridDataDeleted.ClientID %>").value;

            if (hdnGridDataDeleted == "Y") {
                return true;
            }


            if (gvEscRates != null) {
                var gvRows = gvEscRates.rows;  // WUIN-746 grid view rows including header row
                var isGridDataChanged = "N";
                var rowIndex;
                for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                    rowIndex = i - 1; //WUIN-746 row index start from 0
                    hdnIsModified = document.getElementById(gridClientId + 'hdnIsModified' + '_' + rowIndex);
                    //handling empty data row
                    if (gvRows.length == 2 && hdnIsModified == null) {
                        break;
                    }

                    if (hdnIsModified != null && hdnIsModified.value == "-") {
                        isGridDataChanged = "Y";
                        break;
                    }

                    var hdnEscCode = document.getElementById(gridClientId + 'hdnEscCode' + '_' + rowIndex).value;
                    var txtEscCode = document.getElementById(gridClientId + 'txtEscCode' + '_' + rowIndex).value;
                    var hdnSellerGrp = document.getElementById(gridClientId + 'hdnSellerGrp' + '_' + rowIndex).value;
                    var txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + rowIndex).value;
                    var hdnConfigGrp = document.getElementById(gridClientId + 'hdnConfigGrp' + '_' + rowIndex).value;
                    var txtConfig = document.getElementById(gridClientId + 'txtConfig' + '_' + rowIndex).value;
                    var hdnPriceGrp = document.getElementById(gridClientId + 'hdnPriceGrp' + '_' + rowIndex).value;
                    var txtSalesType = document.getElementById(gridClientId + 'txtSalesType' + '_' + rowIndex).value;
                    var hdnSalesTrigger = document.getElementById(gridClientId + 'hdnSalesTrigger' + '_' + rowIndex).value;
                    var txtSalesTrigger = document.getElementById(gridClientId + 'txtSalesTrigger' + '_' + rowIndex).value;
                    var hdnSalesPct = document.getElementById(gridClientId + 'hdnSalesPct' + '_' + rowIndex).value;
                    var txtPctSales = document.getElementById(gridClientId + 'txtPctSales' + '_' + rowIndex).value;
                    var hdnRoyaltyRate = document.getElementById(gridClientId + 'hdnRoyaltyRate' + '_' + rowIndex).value;
                    var txtRoyaltyRate = document.getElementById(gridClientId + 'txtRoyaltyRate' + '_' + rowIndex).value;
                    var hdnUnitRate = document.getElementById(gridClientId + 'hdnUnitRate' + '_' + rowIndex).value;
                    var txtUnitRate = document.getElementById(gridClientId + 'txtUnitRate' + '_' + rowIndex).value;
                    var hdnRevenueRate = document.getElementById(gridClientId + 'hdnRevenueRate' + '_' + rowIndex).value;
                    var txtRevenueRate = document.getElementById(gridClientId + 'txtRevenueRate' + '_' + rowIndex).value;
                    var hdnIsModified = document.getElementById(gridClientId + 'hdnIsModified' + '_' + rowIndex).value;

                    //Harish 02-01-2018 : changed as it is being fetched as '-' from database when '.'
                    //if (ddlTerritory == '-')
                    //    ddlTerritory = '.';
                    //if (ddlConfigCode == '-')
                    //    ddlConfigCode = '.';
                    //if (ddlSalesType == '-')
                    //    ddlSalesType = '.';

                    if (txtPctSales == '')
                        txtPctSales = '0';
                    if (txtRoyaltyRate == '')
                        txtRoyaltyRate = '0';
                    if (txtUnitRate == '')
                        txtUnitRate = '0';
                    if (txtRevenueRate == '')
                        txtRevenueRate = '0';

                    if (hdnEscCode != txtEscCode || hdnSellerGrp != txtTerritory || hdnConfigGrp != txtConfig || hdnPriceGrp != txtSalesType ||
                        hdnSalesTrigger != txtSalesTrigger || hdnSalesPct != txtPctSales || hdnRoyaltyRate != txtRoyaltyRate || hdnUnitRate != txtUnitRate
                        || hdnRevenueRate != txtRevenueRate) {
                        isGridDataChanged = "Y";
                        break;
                    }

                }

            }

            if (isGridDataChanged == "Y") {
                return true;
            }
            else {
                return false;
            }
        }
        var unSaveBrowserClose = false;
        function WarnOnUnSavedData() {
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            var isNewRoyaltorSaved = document.getElementById("<%=hdnNewRoyaltorSaved.ClientID %>").value;
            var isContractScreen = document.getElementById("hdnIsContractScreen").value;
            var isNotContractScreen = document.getElementById("hdnIsNotContractScreen").value;
            if (isExceptionRaised != "Y" && isNewRoyaltorSaved != "Y" && isContractScreen != "Y" && isNotContractScreen != "Y") {
                if (IsAddRowDataChanged() || IsGridDataChanged()) {
                    unSaveBrowserClose = true;
                    return warningMsgOnUnSavedData;
                }
            }
            UpdateScreenLockFlag();// WUIN-599 - Unset the screen lock flag If an user close the browser with out unsaved data or navigate to other than contract screens
        }
        window.onbeforeunload = WarnOnUnSavedData;


        //WUIN-599 Unset the screen lock flag If an user close the browser or navigate to other than contract screens
        window.onunload = function () {
            if (unSaveBrowserClose) {
                UpdateScreenLockFlag();
            }
        }

        function UpdateScreenLockFlag() {
            var isOtherUserScreenLocked = document.getElementById("<%=hdnOtherUserScreenLocked.ClientID %>").value;
            var isAuditScreen = document.getElementById("<%=hdnIsAuditScreen.ClientID %>").value;
            var isContractScreen = document.getElementById("hdnIsContractScreen").value;

            if (isOtherUserScreenLocked == "N" && isAuditScreen == "N" && isContractScreen == "N") {
                document.getElementById("<%=hdnIsAuditScreen.ClientID %>").value = "Y";
                PageMethods.UpdateScreenLockFlag();
            }
        }

        //used to check if any changes to allow navigation to other screen 
        function IsDataChanged() {
            if (IsAddRowDataChanged() || IsGridDataChanged()) {
                return true;
            }
            else {
                return false;
            }
        }

        //redirect to contract grouping screen on saving data of new royaltor so that issue of data not saved validation would be handled
        function RedirectOnNewRoyaltorSave(royaltorId) {
            document.getElementById("<%=hdnNewRoyaltorSaved.ClientID %>").value = "Y";
            window.location = "../Contract/RoyContractGrouping.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y";
        }

        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }


        //============== End

        //validations
        var txtEscCodeAddRowVal;//for profile level validations
        var txtTerritoryAddRowVal;
        var txtConfigAddRowVal;
        var txtSalesTypeAddRowVal;

        var txtSalesTriggerAddRowVal;
        var txtValueTriggerAddRowVal;
        var txtPctSalesAddRowVal;
        var txtRoyaltyRateAddRowVal;
        var txtUnitRateAddRowVal;
        var txtRevenueRateAddRowVal;

        var txtEscCodeVal;//for profile level validations
        var txtTerritoryVal;
        var txtConfigVal;
        var txtSalesTypeVal;

        var txtSalesTriggerVal;
        var txtValueTriggerVal;
        var txtPctSalesGridRowVal;
        var txtRoyaltyRateGridRowVal;
        var txtUnitRateGridRowVal;
        var txtRevenueRateGridRowVal;

        var valtxtSalesTrigger;
        var valtxtValueTrigger;
        var valtxtPctSales;
        var valtxtRoyaltyRate;
        var valtxtUnitRate;
        var valtxtRevenueRate;
        var gridRowIndex;
        var pctSalesRegex = new RegExp("^[0-9][0-9]?(\.[0-9]{1,2})?$|^100(\.0{1,2})?$"); //only positive number <= 100 upto 2 decimal places. zero is allowed
        var onlyPositiveNumberRegex = new RegExp("^[0-9]*$"); //only positive number. zero is allowed

        function GetAddRowValues() {

            txtEscCodeAddRowVal = document.getElementById("<%=txtEscCodeAddRow.ClientID %>").value.toUpperCase();
            txtTerritoryAddRowVal = document.getElementById("<%=txtTerritoryAddRow.ClientID %>").value;
            txtConfigAddRowVal = document.getElementById("<%=txtConfigAddRow.ClientID %>").value;
            txtSalesTypeAddRowVal = document.getElementById("<%=txtSalesTypeAddRow.ClientID %>").value;

            txtSalesTriggerAddRowVal = document.getElementById("<%=txtSalesTriggerAddRow.ClientID %>").value;
            txtValueTriggerAddRowVal = document.getElementById("<%=txtValueTriggerAddRow.ClientID %>").value;
            txtPctSalesAddRowVal = document.getElementById("<%=txtPctSalesAddRow.ClientID %>").value;
            txtRoyaltyRateAddRowVal = document.getElementById("<%=txtRoyaltyRateAddRow.ClientID %>").value;
            txtUnitRateAddRowVal = document.getElementById("<%=txtUnitRateAddRow.ClientID %>").value;
            txtRevenueRateAddRowVal = document.getElementById("<%=txtRevenueRateAddRow.ClientID %>").value;

        }

        function GetGridRowValues(rowIndex) {
            var str = "ContentPlaceHolderBody_gvEscRates_";

            txtEscCodeVal = document.getElementById(str + 'txtEscCode' + '_' + rowIndex).value;
            txtTerritoryVal = document.getElementById(str + 'txtTerritory' + '_' + rowIndex).value;
            txtConfigVal = document.getElementById(str + 'txtConfig' + '_' + rowIndex).value;
            txtSalesTypeVal = document.getElementById(str + 'txtSalesType' + '_' + rowIndex).value;

            txtSalesTriggerGridRowVal = document.getElementById(str + 'txtSalesTrigger' + '_' + rowIndex).value;
            txtValueTriggerGridRowVal = document.getElementById(str + 'txtValueTrigger' + '_' + rowIndex).value;
            txtPctSalesGridRowVal = document.getElementById(str + 'txtPctSales' + '_' + rowIndex).value;
            txtRoyaltyRateGridRowVal = document.getElementById(str + 'txtRoyaltyRate' + '_' + rowIndex).value;
            txtUnitRateGridRowVal = document.getElementById(str + 'txtUnitRate' + '_' + rowIndex).value;
            txtRevenueRateGridRowVal = document.getElementById(str + 'txtRevenueRate' + '_' + rowIndex).value;

            valtxtSalesTrigger = document.getElementById(str + 'valtxtSalesTrigger' + '_' + rowIndex);
            valtxtValueTrigger = document.getElementById(str + 'valtxtValueTrigger' + '_' + rowIndex);
            valtxtPctSales = document.getElementById(str + 'valtxtPctSales' + '_' + rowIndex);
            valtxtRoyaltyRate = document.getElementById(str + 'valtxtRoyaltyRate' + '_' + rowIndex);
            valtxtUnitRate = document.getElementById(str + 'valtxtUnitRate' + '_' + rowIndex);
            valtxtRevenueRate = document.getElementById(str + 'valtxtRevenueRate' + '_' + rowIndex);

        }

        //WUIN-378
        //validation: only positive number can be entered
        //can enter either sales trigger or value trigger for an escalation profile
        function ValSalesTriggerAddRow(sender, args) {
            GetAddRowValues();
            var valtxtSalesTriggerAddRow = document.getElementById("<%=valtxtSalesTriggerAddRow.ClientID %>");

            if (onlyPositiveNumberRegex.test(txtSalesTriggerAddRowVal)) {
                args.IsValid = true;
            }
            else {
                args.IsValid = false;
                valtxtSalesTriggerAddRow.title = "Please enter only positive number";
                return;
            }

            if (txtSalesTriggerAddRowVal != "" && txtValueTriggerAddRowVal != "") {
                args.IsValid = false;
                valtxtSalesTriggerAddRow.title = "Please enter either sales trigger or value trigger";
            }

            //check for other rows of same profile
            var gvEscRates = document.getElementById("<%= gvEscRates.ClientID %>");
                if (gvEscRates != null) {
                    var gvRows = gvEscRates.rows;  // WUIN-746 grid view rows including header row
                    var rowIndex;
                    for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                        rowIndex = i - 1; //WUIN-746 row index start from 0
                        hdnIsModified = document.getElementById(gridClientId + 'hdnIsModified' + '_' + rowIndex);
                        //handling empty data row
                        if (gvRows.length == 2 && hdnIsModified == null) {
                            break;
                        }

                        GetGridRowValues(rowIndex);
                        //check if profile is same as current row being added
                        if (txtEscCodeAddRowVal == txtEscCodeVal && txtTerritoryAddRowVal == txtTerritoryVal && txtConfigAddRowVal == txtConfigVal &&
                        txtSalesTypeAddRowVal == txtSalesTypeVal) {
                            if ((txtSalesTriggerAddRowVal != "" && txtValueTriggerGridRowVal != "") ||
                                (txtValueTriggerAddRowVal != "" && txtSalesTriggerGridRowVal != "")) {
                                args.IsValid = false;
                                valtxtSalesTriggerAddRow.title = "Please enter either sales trigger or value trigger for an escalation profile";
                                break;
                            }
                        }

                    }

                }


            }

            //WUIN-378
            //validation: only positive number can be entered
            //can enter either sales trigger or value trigger for an escalation profile
            function ValValueTriggerAddRow(sender, args) {
                GetAddRowValues();
                var valtxtValueTriggerAddRow = document.getElementById("<%=valtxtValueTriggerAddRow.ClientID %>");

                    if (onlyPositiveNumberRegex.test(txtValueTriggerAddRowVal)) {
                        args.IsValid = true;
                    }
                    else {
                        args.IsValid = false;
                        valtxtValueTriggerAddRow.title = "Please enter only positive number";
                        return;
                    }

                    if (txtSalesTriggerAddRowVal != "" && txtValueTriggerAddRowVal != "") {
                        args.IsValid = false;
                        valtxtValueTriggerAddRow.title = "Please enter either sales trigger or value trigger";
                    }

                    //check for other rows of same profile
                    var gvEscRates = document.getElementById("<%= gvEscRates.ClientID %>");
                if (gvEscRates != null) {
                    var gvRows = gvEscRates.rows;// WUIN-746 grid view rows including header row
                    var rowIndex;
                    for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                        rowIndex = i - 1; //WUIN-746 row index start from 0
                        //handling empty data row
                        hdnIsModified = document.getElementById(gridClientId + 'hdnIsModified' + '_' + rowIndex);
                        //handling empty data row
                        if (gvRows.length == 2 && hdnIsModified == null) {
                            break;
                        }

                        GetGridRowValues(rowIndex);
                        //check if profile is same as current row being added
                        if (txtEscCodeAddRowVal == txtEscCodeVal && txtTerritoryAddRowVal == txtTerritoryVal && txtConfigAddRowVal == txtConfigVal &&
                        txtSalesTypeAddRowVal == txtSalesTypeVal) {
                            if ((txtSalesTriggerAddRowVal != "" && txtValueTriggerGridRowVal != "") ||
                                (txtValueTriggerAddRowVal != "" && txtSalesTriggerGridRowVal != "")) {
                                args.IsValid = false;
                                valtxtValueTriggerAddRow.title = "Please enter either sales trigger or value trigger for an escalation profile";
                                break;
                            }
                        }

                    }

                }
            }

            //WUIN-384 - changes: The User should not have to enter all the zeros. Just set the database fields to zero when data not entered.
            //Do not display warning for Sales Pct if value is 0 or not entered
            //WUIN-378 - valid for Sales Trigger only - need to be entered only if sales trigger is entered
            function ValPctSalesAddRow(sender, args) {
                GetAddRowValues();
                var valtxtPctSalesAddRow = document.getElementById("<%=valtxtPctSalesAddRow.ClientID %>");
            //validation:valid for Sales Trigger only            
            if ((txtValueTriggerAddRowVal != "" && txtValueTriggerAddRowVal != "0") && (txtPctSalesAddRowVal != "" && txtPctSalesAddRowVal != "0")) {
                args.IsValid = false;
                valtxtPctSalesAddRow.title = "Valid for sales trigger only";
                return;
            }
            else if ((txtSalesTriggerAddRowVal == "" || txtSalesTriggerAddRowVal == "0") && (txtPctSalesAddRowVal != "" && txtPctSalesAddRowVal != "0")) {
                args.IsValid = false;
                valtxtPctSalesAddRow.title = "Please enter sales trigger";
                return;
            }

            //validation:mandatory field
            //only either % of sales or a rate is allowed            
            //debugger;
            if ((txtPctSalesAddRowVal == "0" || txtPctSalesAddRowVal == "") &&
                ((txtRoyaltyRateAddRowVal != "0" && txtRoyaltyRateAddRowVal != "") || (txtUnitRateAddRowVal != "0" && txtUnitRateAddRowVal != "") ||
                (txtRevenueRateAddRowVal != "0" && txtRevenueRateAddRowVal != ""))) {
                args.IsValid = true;
                return;
            }
            else if ((txtPctSalesAddRowVal == "0" || txtPctSalesAddRowVal == "") && (txtRoyaltyRateAddRowVal == "0" || txtRoyaltyRateAddRowVal == "") &&
                (txtUnitRateAddRowVal == "0" || txtUnitRateAddRowVal == "") && (txtRevenueRateAddRowVal == "0" || txtRevenueRateAddRowVal == "")) {
                args.IsValid = false;
                valtxtPctSalesAddRow.title = "Please enter either % of sales or a rate";
                return;
            }
            else if ((txtPctSalesAddRowVal != "0" && txtPctSalesAddRowVal != "") &&
                ((txtRoyaltyRateAddRowVal != "0" && txtRoyaltyRateAddRowVal != "") || (txtUnitRateAddRowVal != "0" && txtUnitRateAddRowVal != "") || (txtRevenueRateAddRowVal != "0" && txtRevenueRateAddRowVal != ""))) {
                args.IsValid = false;
                valtxtPctSalesAddRow.title = "Please enter either % of sales or a rate";
                return;
            }

            if (pctSalesRegex.test(txtPctSalesAddRowVal)) {
                args.IsValid = true;
            }
            else {
                args.IsValid = false;
                valtxtPctSalesAddRow.title = "Please enter only positive number <= 100 upto 2 decimal places";
            }

        }

        function ValRoyaltyRateAddRow(sender, args) {
            GetAddRowValues();
            var valtxtRoyaltyRateAddRow = document.getElementById("<%=valtxtRoyaltyRateAddRow.ClientID %>");

            //if (txtRoyaltyRateAddRowVal == "") {
            //    args.IsValid = false;
            //    valtxtRoyaltyRateAddRow.title = "Can not be empty. Please enter 0 if no value to be entered";
            //    return;
            //}
            //debugger;
            if ((txtPctSalesAddRowVal != "0" && txtPctSalesAddRowVal != "") && (txtRoyaltyRateAddRowVal != "0" && txtRoyaltyRateAddRowVal != "")) {
                //either % of sales or atleast one rate should be entered 
                args.IsValid = false;
                valtxtRoyaltyRateAddRow.title = "Please enter either % of sales or a rate";
                return;
            }

            if ((txtPctSalesAddRowVal == "0" || txtPctSalesAddRowVal == "") && (txtRoyaltyRateAddRowVal == "0" || txtRoyaltyRateAddRowVal == "") &&
                (txtUnitRateAddRowVal == "0" || txtUnitRateAddRowVal == "") && (txtRevenueRateAddRowVal == "0" || txtRevenueRateAddRowVal == "")) {
                //atleast one rate should be entered for a row
                args.IsValid = false;
                valtxtRoyaltyRateAddRow.title = "Please enter either royalty rate or unit rate or revenue rate";
                return;
            }

            if ((txtPctSalesAddRowVal == "0" || txtPctSalesAddRowVal == "") && (txtRoyaltyRateAddRowVal != "0" && txtRoyaltyRateAddRowVal != "") &&
                ((txtUnitRateAddRowVal != "0" && txtUnitRateAddRowVal != "") || (txtRevenueRateAddRowVal != "0" && txtRevenueRateAddRowVal != ""))) {
                //atleast one rate should be entered for a row
                args.IsValid = false;
                valtxtRoyaltyRateAddRow.title = "Please enter either royalty rate or unit rate or revenue rate";
                return;
            }

            if (txtRoyaltyRateAddRowVal == "0" || txtRoyaltyRateAddRowVal == "") {
                args.IsValid = true;
                return;
            }

            if (isNaN(txtRoyaltyRateAddRowVal)) {
                args.IsValid = false;
                valtxtRoyaltyRateAddRow.title = "Please enter a number up to 4 decimal places.";
            }
            else {
                var decPlaces = (txtRoyaltyRateAddRowVal.split('.')[1] || []).length;
                if (decPlaces > 4) {
                    args.IsValid = false;
                    valtxtRoyaltyRateAddRow.title = "Please enter a number up to 4 decimal places.";
                }
                else {
                    args.IsValid = true;
                }

            }


        }

        function ValUnitRateAddRow(sender, args) {
            GetAddRowValues();
            var valtxtUnitRateAddRow = document.getElementById("<%=valtxtUnitRateAddRow.ClientID %>");

            //if (txtUnitRateAddRowVal == "") {
            //    args.IsValid = false;
            //    valtxtUnitRateAddRow.title = "Can not be empty. Please enter 0 if no value to be entered";
            //    return;
            //}

            if ((txtPctSalesAddRowVal != "0" && txtPctSalesAddRowVal != "") && (txtUnitRateAddRowVal != "0" && txtUnitRateAddRowVal != "")) {
                //either % of sales or atleast one rate should be entered 
                args.IsValid = false;
                valtxtUnitRateAddRow.title = "Please enter either % of sales or a rate";
                return;
            }

            if ((txtPctSalesAddRowVal == "0" || txtPctSalesAddRowVal == "") && (txtRoyaltyRateAddRowVal == "0" || txtRoyaltyRateAddRowVal == "") &&
                (txtUnitRateAddRowVal == "0" || txtUnitRateAddRowVal == "") && (txtRevenueRateAddRowVal == "0" || txtRevenueRateAddRowVal == "")) {
                //atleast one rate should be entered for a row
                args.IsValid = false;
                valtxtUnitRateAddRow.title = "Please enter either royalty rate or unit rate or revenue rate";
                return;
            }

            if ((txtPctSalesAddRowVal == "0" || txtPctSalesAddRowVal == "") && (txtUnitRateAddRowVal != "0" && txtUnitRateAddRowVal != "") &&
                ((txtRoyaltyRateAddRowVal != "0" && txtRoyaltyRateAddRowVal != "") || (txtRevenueRateAddRowVal != "0" && txtRevenueRateAddRowVal != ""))) {
                //atleast one rate should be entered for a row
                args.IsValid = false;
                valtxtUnitRateAddRow.title = "Please enter either royalty rate or unit rate or revenue rate";
                return;
            }

            if (txtUnitRateAddRowVal == "0" || txtUnitRateAddRowVal == "") {
                args.IsValid = true;
                return;
            }

            if (isNaN(txtUnitRateAddRowVal)) {
                args.IsValid = false;
                valtxtUnitRateAddRow.title = "Please enter a number up to 4 decimal places.";
            }
            else {
                var decPlaces = (txtUnitRateAddRowVal.split('.')[1] || []).length;
                if (decPlaces > 4) {
                    args.IsValid = false;
                    valtxtUnitRateAddRow.title = "Please enter a number up to 4 decimal places.";
                }
                else {
                    args.IsValid = true;
                }

            }
        }

        function ValRevenueRateAddRow(sender, args) {
            GetAddRowValues();
            var valtxtRevenueRateAddRow = document.getElementById("<%=valtxtRevenueRateAddRow.ClientID %>");

            //if (txtRevenueRateAddRowVal == "") {
            //    args.IsValid = false;
            //    valtxtRevenueRateAddRow.title = "Can not be empty. Please enter 0 if no value to be entered";
            //    return;
            //}

            if ((txtPctSalesAddRowVal != "0" && txtPctSalesAddRowVal != "") && (txtRevenueRateAddRowVal != "0" && txtRevenueRateAddRowVal != "")) {
                //either % of sales or atleast one rate should be entered 
                args.IsValid = false;
                valtxtRevenueRateAddRow.title = "Please enter either % of sales or a rate";
                return;
            }

            if ((txtPctSalesAddRowVal == "0" || txtPctSalesAddRowVal == "") && (txtRoyaltyRateAddRowVal == "0" || txtRoyaltyRateAddRowVal == "") &&
                (txtUnitRateAddRowVal == "0" || txtUnitRateAddRowVal == "") && (txtRevenueRateAddRowVal == "0" || txtRevenueRateAddRowVal == "")) {
                //atleast one rate should be entered for a row
                args.IsValid = false;
                valtxtRevenueRateAddRow.title = "Please enter either royalty rate or unit rate or revenue rate";
                return;
            }

            if ((txtPctSalesAddRowVal == "0" || txtPctSalesAddRowVal == "") && (txtRevenueRateAddRowVal != "0" && txtRevenueRateAddRowVal != "") &&
                ((txtRoyaltyRateAddRowVal != "0" && txtRoyaltyRateAddRowVal != "") || (txtUnitRateAddRowVal != "0" && txtUnitRateAddRowVal != ""))) {
                //atleast one rate should be entered for a row
                args.IsValid = false;
                valtxtRevenueRateAddRow.title = "Please enter either royalty rate or unit rate or revenue rate";
                return;
            }

            if (txtRevenueRateAddRowVal == "0" || txtRevenueRateAddRowVal == "") {
                args.IsValid = true;
                return;
            }

            if (isNaN(txtRevenueRateAddRowVal)) {
                args.IsValid = false;
                valtxtRevenueRateAddRow.title = "Please enter a number up to 4 decimal places.";
            }
            else {
                var decPlaces = (txtRevenueRateAddRowVal.split('.')[1] || []).length;
                if (decPlaces > 4) {
                    args.IsValid = false;
                    valtxtRevenueRateAddRow.title = "Please enter a number up to 4 decimal places.";
                }
                else {
                    args.IsValid = true;
                }

            }
        }

        //WUIN-378
        //validation: only positive number can be entered
        //can enter either sales trigger or value trigger for an escalation profile
        function ValSalesTriggerGridRow(sender, args) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);

            //current row
            GetGridRowValues(gridRowIndex);
            var txtEscCodeValCurrentRow = txtEscCodeVal;
            var txtTerritoryValCurrentRow = txtTerritoryVal;
            var txtConfigValCurrentRow = txtConfigVal;
            var txtSalesTypeValCurrentRow = txtSalesTypeVal;
            var salesTriggerValCurrentRow = txtSalesTriggerGridRowVal;
            var valueTriggerValCurrentRow = txtValueTriggerGridRowVal;

            if (onlyPositiveNumberRegex.test(salesTriggerValCurrentRow)) {
                args.IsValid = true;
            }
            else {
                args.IsValid = false;
                valtxtSalesTrigger.title = "Please enter only positive number";
                return;
            }

            if (salesTriggerValCurrentRow != "" && valueTriggerValCurrentRow != "") {
                args.IsValid = false;
                valtxtSalesTrigger.title = "Please enter either sales trigger or value trigger";
                return;
            }

            //check for other rows of same profile
            var gvEscRates = document.getElementById("<%= gvEscRates.ClientID %>");
            if (gvEscRates != null) {
                var gvRows = gvEscRates.rows;// WUIN-746 grid view rows including header row
                var rowIndex;
                for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                    rowIndex = i - 1; //WUIN-746 row index start from 0
                    //handling empty data row
                    hdnIsModified = document.getElementById(gridClientId + 'hdnIsModified' + '_' + rowIndex);
                    //handling empty data row
                    if (gvRows.length == 2 && hdnIsModified == null) {
                        break;
                    }

                    GetGridRowValues(rowIndex);
                    //check if profile is same as current row
                    if (i != gridRowIndex && txtEscCodeValCurrentRow == txtEscCodeVal && txtTerritoryValCurrentRow == txtTerritoryVal && txtConfigValCurrentRow == txtConfigVal &&
                    txtSalesTypeValCurrentRow == txtSalesTypeVal) {

                        if ((salesTriggerValCurrentRow != "" && txtValueTriggerGridRowVal != "") ||
                            (valueTriggerValCurrentRow != "" && txtSalesTriggerGridRowVal != "")) {
                            args.IsValid = false;
                            valtxtSalesTrigger.title = "Please enter either sales trigger or value trigger for an escalation profile";
                            break;
                        }
                    }


                }

            }

        }

        //WUIN-378
        //validation: only positive number can be entered
        //can enter either sales trigger or value trigger for an escalation profile
        function ValValueTriggerGridRow(sender, args) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            GetGridRowValues(gridRowIndex);
            var txtEscCodeValCurrentRow = txtEscCodeVal;
            var txtTerritoryValCurrentRow = txtTerritoryVal;
            var txtConfigValCurrentRow = txtConfigVal;
            var txtSalesTypeValCurrentRow = txtSalesTypeVal;
            var salesTriggerValCurrentRow = txtSalesTriggerGridRowVal;
            var valueTriggerValCurrentRow = txtValueTriggerGridRowVal;

            if (onlyPositiveNumberRegex.test(txtValueTriggerGridRowVal)) {
                args.IsValid = true;
            }
            else {
                args.IsValid = false;
                valtxtValueTrigger.title = "Please enter only positive number";
                return;
            }

            if (txtSalesTriggerGridRowVal != "" && txtValueTriggerGridRowVal != "") {
                args.IsValid = false;
                valtxtValueTrigger.title = "Please enter either sales trigger or value trigger";
            }

            //check for other rows of same profile
            var gvEscRates = document.getElementById("<%= gvEscRates.ClientID %>");
            if (gvEscRates != null) {
                var gvRows = gvEscRates.rows;// WUIN-746 grid view rows including header row
                var rowIndex;
                for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                    rowIndex = i - 1; //WUIN-746 row index start from 0
                    //handling empty data row
                    hdnIsModified = document.getElementById(gridClientId + 'hdnIsModified' + '_' + rowIndex);
                    //handling empty data row
                    if (gvRows.length == 2 && hdnIsModified == null) {
                        break;
                    }

                    GetGridRowValues(rowIndex);
                    //check if profile is same as current row
                    if (i != gridRowIndex && txtEscCodeValCurrentRow == txtEscCodeVal && txtTerritoryValCurrentRow == txtTerritoryVal && txtConfigValCurrentRow == txtConfigVal &&
                    txtSalesTypeValCurrentRow == txtSalesTypeVal) {

                        //can enter either sales trigger or value trigger for an escalation profile
                        if ((salesTriggerValCurrentRow != "" && txtValueTriggerGridRowVal != "") ||
                            (valueTriggerValCurrentRow != "" && txtSalesTriggerGridRowVal != "")) {
                            args.IsValid = false;
                            valtxtValueTrigger.title = "Please enter either sales trigger or value trigger for an escalation profile";
                            break;
                        }

                    }
                }

            }

        }

        //WUIN-378 - valid for Sales Trigger only - need to be entered only if sales trigger is entered
        function ValPctSalesGridRow(sender, args) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            GetGridRowValues(gridRowIndex);
            //validation:valid for Sales Trigger only            
            if ((txtValueTriggerGridRowVal != "" && txtValueTriggerGridRowVal != "0") && (txtPctSalesGridRowVal != "" && txtPctSalesGridRowVal != "0")) {
                args.IsValid = false;
                valtxtPctSales.title = "Valid for sales trigger only";
                return;
            }
            else if ((txtSalesTriggerGridRowVal == "" || txtSalesTriggerGridRowVal == "0") && (txtPctSalesGridRowVal != "" && txtPctSalesGridRowVal != "0")
                ) {
                args.IsValid = false;
                valtxtPctSales.title = "Please enter sales trigger";
                return;
            }

            //validation:mandatory field
            //only either % of sales or a rate is allowed
            //0 is the default value if no value is entered

            //if (txtPctSalesGridRowVal == "") {
            //    args.IsValid = false;
            //    valtxtPctSales.title = "Can not be empty. Please enter 0 if no value to be entered";
            //    return;
            //}
            if ((txtPctSalesGridRowVal == "0" || txtPctSalesGridRowVal == "") &&
                ((txtRoyaltyRateGridRowVal != "0" && txtRoyaltyRateGridRowVal != "") || (txtUnitRateGridRowVal != "0" && txtUnitRateGridRowVal != "") ||
                (txtRevenueRateGridRowVal != "0" && txtRevenueRateGridRowVal != ""))) {
                args.IsValid = true;
                return;
            }
            else if ((txtPctSalesGridRowVal == "0" || txtPctSalesGridRowVal == "") && (txtRoyaltyRateGridRowVal == "0" || txtRoyaltyRateGridRowVal == "") &&
                (txtUnitRateGridRowVal == "0" || txtUnitRateGridRowVal == "") && (txtRevenueRateGridRowVal == "0" || txtRevenueRateGridRowVal == "")) {
                args.IsValid = false;
                valtxtPctSales.title = "Please enter either % of sales or a rate";
                return;
            }
            else if ((txtPctSalesGridRowVal != "0" && txtPctSalesGridRowVal != "") &&
                ((txtRoyaltyRateGridRowVal != "0" && txtRoyaltyRateGridRowVal != "") || (txtUnitRateGridRowVal != "0" && txtUnitRateGridRowVal != "") ||
                (txtRevenueRateGridRowVal != "0" && txtRevenueRateGridRowVal != ""))) {
                args.IsValid = false;
                valtxtPctSales.title = "Please enter either % of sales or a rate";
                return;
            }

            if (pctSalesRegex.test(txtPctSalesGridRowVal)) {
                args.IsValid = true;
            }
            else {
                args.IsValid = false;
                valtxtPctSales.title = "Please enter only positive number <= 100 upto 2 decimal places";
            }

        }

        function ValRoyaltyRateGridRow
            (sender, args) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            GetGridRowValues(gridRowIndex);

            //if (txtRoyaltyRateGridRowVal == "") {
            //    args.IsValid = false;
            //    valtxtRoyaltyRate.title = "Can not be empty. Please enter 0 if no value to be entered";
            //    return;
            //}

            if ((txtPctSalesGridRowVal != "0" && txtPctSalesGridRowVal != "") && (txtRoyaltyRateGridRowVal != "0" && txtRoyaltyRateGridRowVal != "")) {
                //either % of sales or atleast one rate should be entered 
                args.IsValid = false;
                valtxtRoyaltyRate.title = "Please enter either % of sales or a rate";
                return;
            }

            if ((txtPctSalesGridRowVal == "0" || txtPctSalesGridRowVal == "") && (txtRoyaltyRateGridRowVal == "0" || txtRoyaltyRateGridRowVal == "") &&
                (txtUnitRateGridRowVal == "0" || txtUnitRateGridRowVal == "") && (txtRevenueRateGridRowVal == "0" || txtRevenueRateGridRowVal == "")) {
                //atleast one rate should be entered for a row
                args.IsValid = false;
                valtxtRoyaltyRate.title = "Please enter either royalty rate or unit rate or revenue rate";
                return;
            }

            if ((txtPctSalesGridRowVal == "0" || txtPctSalesGridRowVal == "") && (txtRoyaltyRateGridRowVal != "0" && txtRoyaltyRateGridRowVal != "") &&
                ((txtUnitRateGridRowVal != "0" && txtUnitRateGridRowVal != "") || (txtRevenueRateGridRowVal != "0" && txtRevenueRateGridRowVal != ""))) {
                //atleast one rate should be entered for a row
                args.IsValid = false;
                valtxtRoyaltyRate.title = "Please enter either royalty rate or unit rate or revenue rate";
                return;
            }

            if (txtRoyaltyRateGridRowVal == "0" || txtRoyaltyRateGridRowVal == "") {
                args.IsValid = true;
                return;
            }

            if (isNaN(txtRoyaltyRateGridRowVal)) {
                args.IsValid = false;
                valtxtRoyaltyRate.title = "Please enter a number up to 4 decimal places.";
            }
            else {
                var decPlaces = (txtRoyaltyRateGridRowVal.split('.')[1] || []).length;
                if (decPlaces > 4) {
                    args.IsValid = false;
                    valtxtRoyaltyRate.title = "Please enter a number up to 4 decimal places.";
                }
                else {
                    args.IsValid = true;
                }

            }
        }

        function ValUnitRateGridRow(sender, args) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            GetGridRowValues(gridRowIndex);

            //if (txtUnitRateGridRowVal == "") {
            //    args.IsValid = false;
            //    valtxtUnitRate.title = "Can not be empty. Please enter 0 if no value to be entered";
            //    return;
            //}

            if ((txtPctSalesGridRowVal != "0" && txtPctSalesGridRowVal != "") && (txtUnitRateGridRowVal != "0" && txtUnitRateGridRowVal != "")) {
                //either % of sales or atleast one rate should be entered 
                args.IsValid = false;
                valtxtUnitRate.title = "Please enter either % of sales or a rate";
                return;
            }

            if ((txtPctSalesGridRowVal == "0" || txtPctSalesGridRowVal == "") && (txtRoyaltyRateGridRowVal == "0" || txtRoyaltyRateGridRowVal == "") &&
                (txtUnitRateGridRowVal == "0" || txtUnitRateGridRowVal == "") && (txtRevenueRateGridRowVal == "0" || txtRevenueRateGridRowVal == "")) {
                //atleast one rate should be entered for a row
                args.IsValid = false;
                valtxtUnitRate.title = "Please enter either royalty rate or unit rate or revenue rate";
                return;
            }

            if ((txtPctSalesGridRowVal == "0" || txtPctSalesGridRowVal == "") && (txtUnitRateGridRowVal != "0" && txtUnitRateGridRowVal != "") &&
                ((txtRoyaltyRateGridRowVal != "0" && txtRoyaltyRateGridRowVal != "") || (txtRevenueRateGridRowVal != "0" && txtRevenueRateGridRowVal != ""))) {
                //atleast one rate should be entered for a row
                args.IsValid = false;
                valtxtUnitRate.title = "Please enter either royalty rate or unit rate or revenue rate";
                return;
            }

            if (txtUnitRateGridRowVal == "0" || txtUnitRateGridRowVal == "") {
                args.IsValid = true;
                return;
            }

            if (isNaN(txtUnitRateGridRowVal)) {
                args.IsValid = false;
                valtxtUnitRate.title = "Please enter a number up to 4 decimal places.";
            }
            else {
                var decPlaces = (txtUnitRateGridRowVal.split('.')[1] || []).length;
                if (decPlaces > 4) {
                    args.IsValid = false;
                    valtxtUnitRate.title = "Please enter a number up to 4 decimal places.";
                }
                else {
                    args.IsValid = true;
                }

            }
        }

        function ValRevenueRateGridRow(sender, args) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            GetGridRowValues(gridRowIndex);

            //if (txtRevenueRateGridRowVal == "") {
            //    args.IsValid = false;
            //    valtxtRevenueRate.title = "Can not be empty. Please enter 0 if no value to be entered";
            //    return;
            //}

            if ((txtPctSalesGridRowVal != "0" && txtPctSalesGridRowVal != "") && (txtRevenueRateGridRowVal != "0" && txtRevenueRateGridRowVal != "")) {
                //either % of sales or atleast one rate should be entered 
                args.IsValid = false;
                valtxtRevenueRate.title = "Please enter either % of sales or a rate";
                return;
            }

            if ((txtPctSalesGridRowVal == "0" || txtPctSalesGridRowVal == "") && (txtRoyaltyRateGridRowVal == "0" || txtRoyaltyRateGridRowVal == "") &&
                (txtUnitRateGridRowVal == "0" || txtUnitRateGridRowVal == "") && (txtRevenueRateGridRowVal == "0" || txtRevenueRateGridRowVal == "")) {
                //atleast one rate should be entered for a row
                args.IsValid = false;
                valtxtRevenueRate.title = "Please enter either royalty rate or unit rate or revenue rate";
                return;
            }

            if ((txtPctSalesGridRowVal == "0" || txtPctSalesGridRowVal == "") && (txtRevenueRateGridRowVal != "0" && txtRevenueRateGridRowVal != "") &&
                ((txtRoyaltyRateGridRowVal != "0" && txtRoyaltyRateGridRowVal != "") || (txtUnitRateGridRowVal != "0" && txtUnitRateGridRowVal != ""))) {
                //atleast one rate should be entered for a row
                args.IsValid = false;
                valtxtRevenueRate.title = "Please enter either royalty rate or unit rate or revenue rate";
                return;
            }

            if (txtRevenueRateGridRowVal == "0" || txtRevenueRateGridRowVal == "") {
                args.IsValid = true;
                return;
            }

            if (isNaN(txtRevenueRateGridRowVal)) {
                args.IsValid = false;
                valtxtRevenueRate.title = "Please enter a number up to 4 decimal places.";
            }
            else {
                var decPlaces = (txtRevenueRateGridRowVal.split('.')[1] || []).length;
                if (decPlaces > 4) {
                    args.IsValid = false;
                    valtxtRevenueRate.title = "Please enter a number up to 4 decimal places.";
                }
                else {
                    args.IsValid = true;
                }

            }

        }

        function ValidatePopUpSave() {
            //warning on save validation fail
            //debugger;
            if (!Page_ClientValidate("valSave")) {
                Page_BlockSubmit = false;
                DisplayMessagePopup("Escalation rates not saved – invalid or missing data!");
                return false;
            }
            else {
                return true;
            }


        }

        function ValidatePopUpAddRow() {
            //warning on add row validation fail            
            if (!Page_ClientValidate("valGrpAppendAddRow")) {
                Page_BlockSubmit = false;
                DisplayMessagePopup("Invalid or missing data!");
                return false;
            }
            else {
                return true;
            }
        }

        //If sales pct value entered on new line or changed line (not for existing data) - display warning message
        function SalesPctWarningAddRow() {
            var txtPctSales = document.getElementById('<%=txtPctSalesAddRow.ClientID%>').value;
            if (txtPctSales != "") {
                DisplayMessagePopup("The Sales Pct value entered will be added as an increment to the standard Sales Pct");
            }

        }

        function SalesPctWarningGridRow(row, name) {
            var rowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            //validation - If sales pct value entered on new line or changed line (not for existing data) - display warning message
            var gridRowId = row.id.split('_');
            var changeControlId = gridRowId[gridRowId.length - 2];
            if (changeControlId == "txtPctSales") {
                var str = "ContentPlaceHolderBody_gvEscRates_";
                var hdnSalesPct = document.getElementById(str + 'hdnSalesPct' + '_' + rowIndex).value;
                var txtPctSales = document.getElementById(str + 'txtPctSales' + '_' + rowIndex).value;
                var hdnIsModified = document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).value;
                if (txtPctSales != "" && (hdnIsModified == "-" || hdnSalesPct == "")) {
                    //new row or changed one                    
                    DisplayMessagePopup("The Sales Pct value entered will be added as an increment to the standard Sales Pct");
                }

            }
        }



        //============== End

        //Territory fuzzy search

        function territoryListPopulating(sender, args) {
            selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
            txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + selectedRowIndex);
            txtTerritory.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtTerritory.style.backgroundRepeat = 'no-repeat';
            txtTerritory.style.backgroundPosition = 'right';

        }

        function territoryListPopulated(sender, args) {
            selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
            txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + selectedRowIndex);
            txtTerritory.style.backgroundImage = 'none';

        }

        function territoryListHidden(sender, args) {
            selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
            txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + selectedRowIndex);
            txtTerritory.style.backgroundImage = 'none';

        }

        function territoryListItemSelected(sender, args) {
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
                document.getElementById(gridClientId + 'txtTerritory' + '_' + selectedRowIndex).value = "";
            }

        }

        //Pop up fuzzy search list       
        function OntxtTerritoryKeyDown(sender) {
            if ((event.keyCode == 13)) {
                selectedRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
                //Enter key can be used to select the dropdown list item or to pop up the complete list
                //to know this, check if list item is selected or not
                var aceTerritory = $find(gridClientId + 'aceTerritory' + '_' + selectedRowIndex);
                if (aceTerritory._selectIndex == -1) {
                    txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + selectedRowIndex).value;
                    document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtTerritory;
                    document.getElementById("<%=hdnGridFuzzySearchRowId.ClientID %>").innerText = selectedRowIndex;
                    document.getElementById("<%=hdnFuzzySearchField.ClientID %>").innerText = "Territory";
                    document.getElementById('<%=btnFuzzyTerritoryListPopup.ClientID%>').click();
                }
            }

        }

        //============== End

        //Configuration fuzzy search

        function configListPopulating(sender, args) {
            selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
            txtConfig = document.getElementById(gridClientId + 'txtConfig' + '_' + selectedRowIndex);
            txtConfig.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtConfig.style.backgroundRepeat = 'no-repeat';
            txtConfig.style.backgroundPosition = 'right';

        }

        function configListPopulated(sender, args) {
            selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
            txtConfig = document.getElementById(gridClientId + 'txtConfig' + '_' + selectedRowIndex);
            txtConfig.style.backgroundImage = 'none';

        }

        function configListHidden(sender, args) {
            selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
            txtConfig = document.getElementById(gridClientId + 'txtConfig' + '_' + selectedRowIndex);
            txtConfig.style.backgroundImage = 'none';

        }

        function configListItemSelected(sender, args) {
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
                document.getElementById(gridClientId + 'txtConfig' + '_' + selectedRowIndex).value = "";
            }

        }

        //Pop up fuzzy search list       
        function OntxtConfigKeyDown(sender) {
            if ((event.keyCode == 13)) {
                selectedRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
                //Enter key can be used to select the dropdown list item or to pop up the complete list
                //to know this, check if list item is selected or not
                var aceConfig = $find(gridClientId + 'aceConfig' + '_' + selectedRowIndex);
                if (aceConfig._selectIndex == -1) {
                    txtConfig = document.getElementById(gridClientId + 'txtConfig' + '_' + selectedRowIndex).value;
                    document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtConfig;
                    document.getElementById("<%=hdnGridFuzzySearchRowId.ClientID %>").innerText = selectedRowIndex;
                    document.getElementById("<%=hdnFuzzySearchField.ClientID %>").innerText = "Config";
                    document.getElementById('<%=btnFuzzyConfigListPopup.ClientID%>').click();
                }
            }

        }

        //============== End

        //Sales Type fuzzy search

        function salesTypeListPopulating(sender, args) {
            selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
            txtSalesType = document.getElementById(gridClientId + 'txtSalesType' + '_' + selectedRowIndex);
            txtSalesType.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtSalesType.style.backgroundRepeat = 'no-repeat';
            txtSalesType.style.backgroundPosition = 'right';

        }

        function salesTypeListPopulated(sender, args) {
            selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
            txtSalesType = document.getElementById(gridClientId + 'txtSalesType' + '_' + selectedRowIndex);
            txtSalesType.style.backgroundImage = 'none';


        }

        function salesTypeListHidden(sender, args) {
            selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
            txtSalesType = document.getElementById(gridClientId + 'txtSalesType' + '_' + selectedRowIndex);
            txtSalesType.style.backgroundImage = 'none';

        }

        function salesTypeListItemSelected(sender, args) {
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
                document.getElementById(gridClientId + 'txtSalesType' + '_' + selectedRowIndex).value = "";
            }

        }

        //Pop up fuzzy search list       
        function OntxtSalesTypeKeyDown(sender) {
            if ((event.keyCode == 13)) {
                selectedRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
                //Enter key can be used to select the dropdown list item or to pop up the complete list
                //to know this, check if list item is selected or not
                var aceSalesType = $find(gridClientId + 'aceSalesType' + '_' + selectedRowIndex);
                if (aceSalesType._selectIndex == -1) {
                    txtSalesType = document.getElementById(gridClientId + 'txtSalesType' + '_' + selectedRowIndex).value;
                    document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtSalesType;
                    document.getElementById("<%=hdnGridFuzzySearchRowId.ClientID %>").innerText = selectedRowIndex;
                    document.getElementById("<%=hdnFuzzySearchField.ClientID %>").innerText = "SalesType";
                    document.getElementById('<%=btnFuzzySalesTypeListPopup.ClientID%>').click();
                }
                else {
                    return false;
                }
            }

        }

        //============== End

        //Territory Add row fuzzy search

        function territoryAddRowListPopulating() {
            txtTerritoryAddRow = document.getElementById("<%= txtTerritoryAddRow.ClientID %>");
            txtTerritoryAddRow.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtTerritoryAddRow.style.backgroundRepeat = 'no-repeat';
            txtTerritoryAddRow.style.backgroundPosition = 'right';

        }

        function territoryAddRowListPopulated() {
            txtTerritoryAddRow = document.getElementById("<%= txtTerritoryAddRow.ClientID %>");
            txtTerritoryAddRow.style.backgroundImage = 'none';
        }

        function territoryAddRowListHidden() {
            txtTerritoryAddRow = document.getElementById("<%= txtTerritoryAddRow.ClientID %>");
            txtTerritoryAddRow.style.backgroundImage = 'none';

        }

        function territoryAddRowListItemSelected(sender, args) {
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                txtTerritoryAddRow = document.getElementById("<%= txtTerritoryAddRow.ClientID %>");
                txtTerritoryAddRow.value = "";
            }
        }

        //Pop up fuzzy search list       
        function OntxtTerritoryAddRowKeyDown() {
            if ((event.keyCode == 13)) {

                //Enter key can be used to select the dropdown list item or to pop up the complete list
                //to know this, check if list item is selected or not
                var aceTerritoryAddRow = $find('ContentPlaceHolderBody_' + 'aceTerritoryAddRow');
                if (aceTerritoryAddRow._selectIndex == -1) {
                    txtTerritoryAddRow = document.getElementById("<%= txtTerritoryAddRow.ClientID %>").value;
                    document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtTerritoryAddRow;
                    document.getElementById("<%=hdnFuzzySearchField.ClientID %>").innerText = "TerritoryAddRow";
                    document.getElementById('<%=btnFuzzyTerritoryListPopup.ClientID%>').click();
                }
            }

        }

        //============== End

        //Config Add row fuzzy search

        function configAddRowListPopulating() {
            txtConfigAddRow = document.getElementById("<%= txtConfigAddRow.ClientID %>");
            txtConfigAddRow.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtConfigAddRow.style.backgroundRepeat = 'no-repeat';
            txtConfigAddRow.style.backgroundPosition = 'right';
        }

        function configAddRowListPopulated() {
            txtConfigAddRow = document.getElementById("<%= txtConfigAddRow.ClientID %>");
            txtConfigAddRow.style.backgroundImage = 'none';
        }

        function configAddRowListHidden() {
            txtConfigAddRow = document.getElementById("<%= txtConfigAddRow.ClientID %>");
            txtConfigAddRow.style.backgroundImage = 'none';

        }

        function configAddRowListItemSelected(sender, args) {
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                document.getElementById("<%= txtConfigAddRow.ClientID %>").value = "";
            }
        }

        //Pop up fuzzy search list       
        function OntxtConfigAddRowKeyDown() {
            if ((event.keyCode == 13)) {
                //Enter key can be used to select the dropdown list item or to pop up the complete list
                //to know this, check if list item is selected or not
                var aceConfigAddRow = $find('ContentPlaceHolderBody_' + 'aceConfigAddRow');
                if (aceConfigAddRow._selectIndex == -1) {
                    txtConfigAddRow = document.getElementById("<%= txtConfigAddRow.ClientID %>").value;
                    document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtConfigAddRow;
                    document.getElementById("<%=hdnFuzzySearchField.ClientID %>").innerText = "ConfigAddRow";
                    document.getElementById('<%=btnFuzzyConfigListPopup.ClientID%>').click();
                }
            }

        }

        //============== End

        //Sales Type Add row fuzzy search

        function salesTypeAddRowListPopulating() {
            txtSalesTypeAddRow = document.getElementById("<%= txtSalesTypeAddRow.ClientID %>");
            txtSalesTypeAddRow.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtSalesTypeAddRow.style.backgroundRepeat = 'no-repeat';
            txtSalesTypeAddRow.style.backgroundPosition = 'right';

        }

        function salesTypeAddRowListPopulated() {
            txtSalesTypeAddRow = document.getElementById("<%= txtSalesTypeAddRow.ClientID %>");
            txtSalesTypeAddRow.style.backgroundImage = 'none';
        }

        function salesTypeAddRowListHidden() {
            txtSalesTypeAddRow = document.getElementById("<%= txtSalesTypeAddRow.ClientID %>");
            txtSalesTypeAddRow.style.backgroundImage = 'none';

        }

        function salesTypeAddRowListItemSelected(sender, args) {
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                document.getElementById("<%= txtSalesTypeAddRow.ClientID %>").value = "";
            }
        }

        //Pop up fuzzy search list       
        function OntxtSalesTypeAddRowKeyDown() {
            if ((event.keyCode == 13)) {

                //Enter key can be used to select the dropdown list item or to pop up the complete list
                //to know this, check if list item is selected or not
                var aceSalesTypeAddRow = $find('ContentPlaceHolderBody_' + 'aceSalesTypeAddRow');
                if (aceSalesTypeAddRow._selectIndex == -1) {
                    txtSalesTypeAddRow = document.getElementById("<%= txtSalesTypeAddRow.ClientID %>").value;
                    document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtSalesTypeAddRow;
                    document.getElementById("<%=hdnFuzzySearchField.ClientID %>").innerText = "SalesTypeAddRow";
                    document.getElementById('<%=btnFuzzySalesTypeListPopup.ClientID%>').click();
                }
            }

        }

        //============== End

        //================================End

        //Validate if the field value is a valid one from fuzzy search list
        function ValTerritoryGridRow(sender, args) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + gridRowIndex);

            if (txtTerritory.value == "") {
                args.IsValid = true;
                txtTerritory.style["width"] = '98%';
            }
            else if (txtTerritory.value == "No results found") {
                args.IsValid = true;
                txtTerritory.value = "";
                txtTerritory.style["width"] = '98%';
            }
            else if (txtTerritory.value != "" && txtTerritory.value.indexOf('-') == -1) {
                args.IsValid = false;
                //adjust width of the textbox to display error
                fieldWidth = txtTerritory.offsetWidth;
                txtTerritory.style["width"] = (fieldWidth - 20);
            }
            else if (args.IsValid == true) {
                txtTerritory.style["width"] = '98%';
            }


        }

        //Validate if the field value is a valid one from fuzzy search list
        function ValConfigGridRow(sender, args) {

            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtConfig = document.getElementById(gridClientId + 'txtConfig' + '_' + gridRowIndex);

            if (txtConfig.value == "") {
                args.IsValid = true;
                txtConfig.style["width"] = '98%';
            }
            else if (txtConfig.value == "No results found") {
                args.IsValid = true;
                txtConfig.value = "";
                txtConfig.style["width"] = '98%';
            }
            else if (txtConfig.value != "" && txtConfig.value.indexOf('-') == -1) {
                args.IsValid = false;
                //adjust width of the textbox to display error
                fieldWidth = txtConfig.offsetWidth;
                txtConfig.style["width"] = (fieldWidth - 20);
            }
            else if (args.IsValid == true) {
                txtConfig.style["width"] = '98%';
            }

        }

        //Validate if the field value is a valid one from fuzzy search list
        function ValSalesTypeGridRow(sender, args) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtSalesType = document.getElementById(gridClientId + 'txtSalesType' + '_' + gridRowIndex);

            if (txtSalesType.value == "") {
                args.IsValid = true;
                txtSalesType.style["width"] = '97%';
            }
            else if (txtSalesType.value == "No results found") {
                args.IsValid = true;
                txtSalesType.value = "";
                txtSalesType.style["width"] = '97%';
            }
            else if (txtSalesType.value != "" && txtSalesType.value.indexOf('-') == -1) {
                args.IsValid = false;
                //adjust width of the textbox to display error
                fieldWidth = txtSalesType.offsetWidth;
                txtSalesType.style["width"] = (fieldWidth - 20);
            }
            else if (args.IsValid == true) {
                txtSalesType.style["width"] = '97%';
            }


        }

        //reset field width when empty
        function OntxtTerritoryChange(sender) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + gridRowIndex);

            if (txtTerritory.value == "") {
                txtTerritory.style["width"] = '98%';
            }
        }

        //reset field width when empty
        function OntxtConfigChange(sender) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtConfig = document.getElementById(gridClientId + 'txtConfig' + '_' + gridRowIndex);

            if (txtConfig.value == "") {
                txtConfig.style["width"] = '98%';
            }
        }

        //reset field width when empty
        function OntxtSalesTypeChange(sender) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtSalesType = document.getElementById(gridClientId + 'txtSalesType' + '_' + gridRowIndex);

            if (txtSalesType.value == "") {
                txtSalesType.style["width"] = '97%';
            }
        }

        //*****************************
        //Validate if the field value is a valid one from fuzzy search list
        function ValTerritoryAddRow(sender, args) {
            txtTerritoryAddRow = document.getElementById("<%=txtTerritoryAddRow.ClientID %>");
            if (txtTerritoryAddRow.value == "") {
                args.IsValid = true;
                txtTerritoryAddRow.style["width"] = '98%';
            }
            else if (txtTerritoryAddRow.value == "No results found") {
                args.IsValid = true;
                txtTerritoryAddRow.value = "";
                txtTerritoryAddRow.style["width"] = '98%';
            }
            else if (txtTerritoryAddRow.value != "" && txtTerritoryAddRow.value.indexOf('-') == -1) {
                args.IsValid = false;
                //adjust width of the textbox to display error
                fieldWidth = txtTerritoryAddRow.offsetWidth;
                txtTerritoryAddRow.style["width"] = (fieldWidth - 20);
            }
            else if (args.IsValid == true) {
                txtTerritoryAddRow.style["width"] = '98%';
            }


        }

        //Validate if the field value is a valid one from fuzzy search list
        function ValConfigAddRow(sender, args) {
            txtConfigAddRow = document.getElementById("<%=txtConfigAddRow.ClientID %>");
            if (txtConfigAddRow.value == "") {
                args.IsValid = true;
                txtConfigAddRow.style["width"] = '98%';
            }
            else if (txtConfigAddRow.value == "No results found") {
                args.IsValid = true;
                txtConfigAddRow.value = "";
                txtConfigAddRow.style["width"] = '98%';
            }
            else if (txtConfigAddRow.value != "" && txtConfigAddRow.value.indexOf('-') == -1) {
                args.IsValid = false;
                //adjust width of the textbox to display error
                fieldWidth = txtConfigAddRow.offsetWidth;
                txtConfigAddRow.style["width"] = (fieldWidth - 20);
            }
            else if (args.IsValid == true) {
                txtConfigAddRow.style["width"] = '98%';
            }

        }

        //Validate if the field value is a valid one from fuzzy search list
        function ValSalesTypeAddRow(sender, args) {
            txtSalesTypeAddRow = document.getElementById("<%=txtSalesTypeAddRow.ClientID %>");
            if (txtSalesTypeAddRow.value == "") {
                args.IsValid = true;
                txtSalesTypeAddRow.style["width"] = '98%';
            }
            else if (txtSalesTypeAddRow.value == "No results found") {
                args.IsValid = true;
                txtSalesTypeAddRow.value = "";
                txtSalesTypeAddRow.style["width"] = '98%';
            }
            else if (txtSalesTypeAddRow.value != "" && txtSalesTypeAddRow.value.indexOf('-') == -1) {
                args.IsValid = false;
                //adjust width of the textbox to display error
                fieldWidth = txtSalesTypeAddRow.offsetWidth;
                txtSalesTypeAddRow.style["width"] = (fieldWidth - 20);
            }
            else if (args.IsValid == true) {
                txtSalesTypeAddRow.style["width"] = '98%';
            }

        }


        //reset field width when empty
        function OntxtTerritoryAddRowChange() {
            txtTerritoryAddRow = document.getElementById("<%=txtTerritoryAddRow.ClientID %>");
            if (txtTerritoryAddRow.value == "") {
                txtTerritoryAddRow.style["width"] = '98%';
            }
        }

        //reset field width when empty
        function OntxtConfigChangeAddRow(sender) {
            txtConfigAddRow = document.getElementById("<%=txtConfigAddRow.ClientID %>");
            if (txtConfigAddRow.value == "") {
                txtConfigAddRow.style["width"] = '98%';
            }
        }

        //reset field width when empty
        function OntxtSalesTypeAddRowChange(sender) {
            txtSalesTypeAddRow = document.getElementById("<%=txtSalesTypeAddRow.ClientID %>");
            if (txtSalesTypeAddRow.value == "") {
                txtSalesTypeAddRow.style["width"] = '98%';
            }
        }

        //validate sales prorata save
        function ValidateSaveProRata() {
            if (!Page_ClientValidate("valGrpSaveProRata")) {
                Page_BlockSubmit = false;
                DisplayMessagePopup("Prorata not saved – invalid or missing data!");
                return false;
            }
            else {
                return true;
            }

        }


        //Validations=================End

        //check if changes made and not saved in proRata popup
        //close popup if no changes made
        function CancelProRata() {
            //check if changes made and not saved
            var gvProRataClientId = "ContentPlaceHolderBody_gvSalesCategoryProRata_";
            var dataChanged = false;
            var gvSalesCategoryProRata = document.getElementById("<%= gvSalesCategoryProRata.ClientID %>");

            if (gvSalesCategoryProRata != null) {
                var gvRows = gvSalesCategoryProRata.rows;

                for (var i = 0; i < gvRows.length; i++) {
                    var hdnProRata = document.getElementById(gvProRataClientId + 'hdnProRata' + '_' + i);
                    //handling empty data row
                    if (gvRows.length == 1 && hdnProRata == null) {
                        break;
                    }

                    var txtProRata = document.getElementById(gvProRataClientId + 'txtProRata' + '_' + i).value;

                    if (hdnProRata.value != txtProRata) {
                        dataChanged = true;
                        break;
                    }
                }

            }

            if (dataChanged == true) {
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = "";//WUIN-932 - set this to null when prorata popup is open.
                OpenOnUnSavedData();
                return false;
            }

            document.getElementById('<%= gvSalesCategoryProRata.ClientID%>').innerText = null;

            var popup = $find('<%= mpeProRataPopup.ClientID %>');
            if (popup != null) {
                popup.hide();
            }

            return false;
        }
        //End

        // WUIN-662 - confirmation on un saved data
        function OpenOnUnSavedData() {
            var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
            if (warnPopup != null) {
                document.getElementById("<%=lblUnSavedWarnMsg.ClientID %>").innerText = warningMsgOnUnSavedData;
                warnPopup.show();

            }
        }

        function OnUnSavedDataReturn() {
            var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
            if (warnPopup != null) {
                warnPopup.hide();
            }

            window.onbeforeunload = WarnOnUnSavedData;
            return false;
        }

        function OnUnSavedDataExit() {
            hdnButtonSelection = document.getElementById("<%=hdnButtonSelection.ClientID %>").value;
            var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
            var proRataPopup = $find('<%= mpeProRataPopup.ClientID %>');

            if (warnPopup != null) {
                warnPopup.hide();
            }

            if (proRataPopup != null) {
                proRataPopup.hide();
                if (hdnButtonSelection == "SalesProRata") {
                    return true;
                }
                else {
                    window.onbeforeunload = WarnOnUnSavedData;
                    return false;
                }

            }

            window.onbeforeunload = WarnOnUnSavedData;
        }
        //============== End
        //JIRA-451 CHanges -- Start
        function OnAppendAddRowKeyDown() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnAppendAddRow.ClientID%>').click();
            }
        }
        //JIRA-451 Changes -- ENd

        //WUIN-932
        function ValidateProRata(button) {
            if (IsDataChanged()) {
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
                return false;
            }
            return true;
        }


    </script>
    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="4">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    ROYALTOR CONTRACT - ESCALATION RATES
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="4"></td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td valign="top">
                        <table width="99%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="10%" class="identifierLable_large_bold">Current Royaltor</td>
                                <td>
                                    <asp:TextBox ID="txtRoyaltorId" runat="server" Width="25%" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" TabIndex="100"
                                        onkeydown="OnRoyaltorIdTabPress();"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td width="1%"></td>
                    <td width="10%" rowspan="4" valign="top" align="right">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:Button ID="btnSave" runat="server" CssClass="ButtonStyle" OnClientClick="if (!ValidatePopUpSave()) { return false;};" OnClick="btnSave_Click"
                                                    Text="Save Changes" UseSubmitBehavior="false" Width="90%" ValidationGroup="valSave" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Button ID="btnAudit" runat="server" CssClass="ButtonStyle" OnClick="btnAudit_Click"
                                                    Text="Audit" UseSubmitBehavior="false" Width="90%" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Button ID="btnProRata" runat="server" CssClass="ButtonStyle" OnClick="btnProRata_Click" OnClientClick="if (!ValidateProRata('SalesProRata')) { return false;};"
                                                    Text="Sales Category ProRata" UseSubmitBehavior="false" Width="90%" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    <br />
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table width="100%" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td align="right">
                                                <ContNav:ContractNavigation ID="contractNavigationButtons" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td class="table_header_with_border" valign="top">Escalation Rates</td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td valign="top">
                        <table width="100%" class="table_with_border">
                            <tr>
                                <td>
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvEscRates" runat="server" AutoGenerateColumns="False" Width="98.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found" OnRowDataBound="gvEscRates_RowDataBound" AllowSorting="true" OnSorting="gvEscRates_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <Columns>
                                                            <asp:TemplateField HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Esc #" SortExpression="esc_code" ItemStyle-Width="4%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtEscCode" runat="server" Width="70%" Text='<%#Bind("esc_code")%>' CssClass="gridTextField"
                                                                        Style="text-align: center; text-transform: uppercase;" MaxLength="2"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvtxtEscCode" ControlToValidate="txtEscCode" ValidationGroup="valSave"
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Escalation code" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Territory" SortExpression="seller_group" ItemStyle-Width="21%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtTerritory" runat="server" Width="98%" Text='<%#Bind("seller_group")%>' CssClass="textbox_FuzzySearch"
                                                                        ToolTip='<%#Bind("seller_group")%>' onkeydown="OntxtTerritoryKeyDown(this);" onchange="OntxtTerritoryChange(this);"></asp:TextBox>
                                                                    <ajaxToolkit:AutoCompleteExtender ID="aceTerritory" runat="server"
                                                                        ServiceMethod="FuzzySearchAllSellerGroupList"
                                                                        ServicePath="~/Services/FuzzySearch.asmx"
                                                                        MinimumPrefixLength="1"
                                                                        CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                                        TargetControlID="txtTerritory"
                                                                        FirstRowSelected="true"
                                                                        OnClientPopulating="territoryListPopulating"
                                                                        OnClientPopulated="territoryListPopulated"
                                                                        OnClientHidden="territoryListHidden"
                                                                        OnClientItemSelected="territoryListItemSelected"
                                                                        CompletionListElementID="pnlTerritoryFuzzySearch" />
                                                                    <asp:Panel ID="pnlTerritoryFuzzySearch" runat="server" CssClass="identifierLable" />
                                                                    <asp:CustomValidator ID="valtxtTerritory" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator"
                                                                        ClientValidationFunction="ValTerritoryGridRow" ToolTip="Please select valid territory from the search list"
                                                                        ControlToValidate="txtTerritory" ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Configuration Code" SortExpression="config_group" ItemStyle-Width="20%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtConfig" runat="server" Width="98%" Text='<%#Bind("config_group")%>' CssClass="textbox_FuzzySearch"
                                                                        ToolTip='<%#Bind("config_group")%>' onkeydown="OntxtConfigKeyDown(this);" onchange="OntxtConfigChange(this);"></asp:TextBox>
                                                                    <ajaxToolkit:AutoCompleteExtender ID="aceConfig" runat="server"
                                                                        ServiceMethod="FuzzySearchAllConfigGroupList"
                                                                        ServicePath="~/Services/FuzzySearch.asmx"
                                                                        MinimumPrefixLength="1"
                                                                        CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                                        TargetControlID="txtConfig"
                                                                        FirstRowSelected="true"
                                                                        OnClientPopulating="configListPopulating"
                                                                        OnClientPopulated="configListPopulated"
                                                                        OnClientHidden="configListHidden"
                                                                        OnClientItemSelected="configListItemSelected"
                                                                        CompletionListElementID="pnlConfigFuzzySearch" />
                                                                    <asp:Panel ID="pnlConfigFuzzySearch" runat="server" CssClass="identifierLable" />
                                                                    <asp:CustomValidator ID="valtxtConfig" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator"
                                                                        ClientValidationFunction="ValConfigGridRow" ToolTip="Please select valid configuration from the search list"
                                                                        ControlToValidate="txtConfig" ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Sales Type" SortExpression="price_group" ItemStyle-Width="15%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtSalesType" runat="server" Width="97%" Text='<%#Bind("price_group")%>' CssClass="textbox_FuzzySearch"
                                                                        ToolTip='<%#Bind("price_group")%>' onkeydown="OntxtSalesTypeKeyDown(this);" onchange="OntxtSalesTypeChange(this);"></asp:TextBox>
                                                                    <ajaxToolkit:AutoCompleteExtender ID="aceSalesType" runat="server"
                                                                        ServiceMethod="FuzzySearchAllPriceGroupList"
                                                                        ServicePath="~/Services/FuzzySearch.asmx"
                                                                        MinimumPrefixLength="1"
                                                                        CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                                        TargetControlID="txtSalesType"
                                                                        FirstRowSelected="true"
                                                                        OnClientPopulating="salesTypeListPopulating"
                                                                        OnClientPopulated="salesTypeListPopulated"
                                                                        OnClientHidden="salesTypeListHidden"
                                                                        OnClientItemSelected="salesTypeListItemSelected"
                                                                        CompletionListElementID="pnlSalesTypeFuzzySearch" />
                                                                    <asp:Panel ID="pnlSalesTypeFuzzySearch" runat="server" CssClass="identifierLable" />
                                                                    <asp:CustomValidator ID="valtxtSalesType" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator"
                                                                        ClientValidationFunction="ValSalesTypeGridRow" ToolTip="Please select valid sales type from the search list"
                                                                        ControlToValidate="txtSalesType" ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Sales Trigger" SortExpression="sales_trigger" ItemStyle-Width="6%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtSalesTrigger" runat="server" Width="70%" Text='<%#Bind("sales_trigger")%>' CssClass="gridTextField" Style="text-align: center"></asp:TextBox>
                                                                    <asp:CustomValidator ID="valtxtSalesTrigger" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator"
                                                                        ClientValidationFunction="ValSalesTriggerGridRow" ToolTip="Please enter either sales trigger or value trigger"
                                                                        ErrorMessage="*"></asp:CustomValidator>
                                                                    <ajaxToolkit:FilteredTextBoxExtender ID="ftetxtSalesTrigger" runat="server"
                                                                        Enabled="True" TargetControlID="txtSalesTrigger" FilterType="Numbers">
                                                                    </ajaxToolkit:FilteredTextBoxExtender>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Value Trigger" SortExpression="value_trigger" ItemStyle-Width="6%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtValueTrigger" runat="server" Width="70%" Text='<%#Bind("value_trigger")%>' CssClass="gridTextField" Style="text-align: center"></asp:TextBox>
                                                                    <asp:CustomValidator ID="valtxtValueTrigger" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator"
                                                                        ClientValidationFunction="ValValueTriggerGridRow" ToolTip="Please enter either sales trigger or value trigger"
                                                                        ErrorMessage="*"></asp:CustomValidator>
                                                                    <ajaxToolkit:FilteredTextBoxExtender ID="ftetxtValueTrigger" runat="server"
                                                                        Enabled="True" TargetControlID="txtValueTrigger" FilterType="Numbers">
                                                                    </ajaxToolkit:FilteredTextBoxExtender>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="% of Sales" SortExpression="sales_pct" ItemStyle-Width="6%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtPctSales" runat="server" Width="70%" Text='<%#Bind("sales_pct")%>' CssClass="gridTextField" Style="text-align: center"
                                                                        onchange="SalesPctWarningGridRow(this,'');"></asp:TextBox>
                                                                    <asp:CustomValidator ID="valtxtPctSales" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator"
                                                                        ClientValidationFunction="ValPctSalesGridRow" ToolTip="Please enter either % of sales or a rate"
                                                                        ErrorMessage="*"></asp:CustomValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Royalty Rate" SortExpression="royalty_rate" ItemStyle-Width="6%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtRoyaltyRate" runat="server" Width="70%" Text='<%#Bind("royalty_rate","{0:0.####}")%>' CssClass="gridTextField" Style="text-align: center"
                                                                        ToolTip="Please enter a number up to 4 decimal places and either royalty rate or unit rate or revenue rate"></asp:TextBox>
                                                                    <asp:CustomValidator ID="valtxtRoyaltyRate" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator"
                                                                        ClientValidationFunction="ValRoyaltyRateGridRow" ToolTip="Please enter either royalty rate or unit rate or revenue rate"
                                                                        ErrorMessage="*"></asp:CustomValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Unit Rate" SortExpression="unit_rate" ItemStyle-Width="6%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtUnitRate" runat="server" Width="70%" Text='<%#Bind("unit_rate","{0:0.####}")%>' CssClass="gridTextField" Style="text-align: center"
                                                                        ToolTip="Please enter a number up to 4 decimal places and either royalty rate or unit rate or revenue rate"></asp:TextBox>
                                                                    <asp:CustomValidator ID="valtxtUnitRate" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator"
                                                                        ClientValidationFunction="ValUnitRateGridRow" ToolTip="Please enter either royalty rate or unit rate or revenue rate"
                                                                        ErrorMessage="*"></asp:CustomValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Revenue Rate" SortExpression="revenue_rate" ItemStyle-Width="6%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtRevenueRate" runat="server" Width="70%" Text='<%#Bind("revenue_rate","{0:0.####}")%>' CssClass="gridTextField" Style="text-align: center"
                                                                        ToolTip="Please enter a number up to 4 decimal places and either royalty rate or unit rate or revenue rate"></asp:TextBox>
                                                                    <asp:CustomValidator ID="valtxtRevenueRate" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator"
                                                                        ClientValidationFunction="ValRevenueRateGridRow" ToolTip="Please enter either royalty rate or unit rate or revenue rate"
                                                                        ErrorMessage="*"></asp:CustomValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="" ItemStyle-Width="4%">
                                                                <ItemTemplate>
                                                                    <table width="95%" style="float: right; table-layout: fixed">
                                                                        <tr style="float: right">
                                                                            <td align="center">
                                                                                <asp:ImageButton ID="imgBtnDelete" runat="server" ImageUrl="../Images/Delete.gif"
                                                                                    ToolTip="Delete" OnClientClick="return ConfirmDelete(this);" />
                                                                            </td>
                                                                            <td align="center">
                                                                                <asp:ImageButton ID="imgBtnUndo" runat="server" ImageUrl="../Images/cancel_row3.png"
                                                                                    ToolTip="Cancel" OnClientClick="return UndoGridChanges(this);" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                    <asp:HiddenField ID="hdnEscProfileId" runat="server" Value='<%# Bind("escalation_profile_id") %>' />
                                                                    <asp:HiddenField ID="hdnEscLevel" runat="server" Value='<%# Bind("escalation_level") %>' />
                                                                    <asp:HiddenField ID="hdnEscCode" runat="server" Value='<%# Bind("esc_code") %>' />
                                                                    <asp:HiddenField ID="hdnSellerGrp" runat="server" Value='<%# Bind("seller_group") %>' />
                                                                    <asp:HiddenField ID="hdnSellerGrpCode" runat="server" Value='<%# Bind("seller_group_code") %>' />
                                                                    <asp:HiddenField ID="hdnSellerGrpCodeOrder" runat="server" Value='<%# Bind("seller_group_code_order") %>' />
                                                                    <asp:HiddenField ID="hdnConfigGrp" runat="server" Value='<%# Bind("config_group") %>' />
                                                                    <asp:HiddenField ID="hdnConfigGrpCode" runat="server" Value='<%# Bind("config_group_code") %>' />
                                                                    <asp:HiddenField ID="hdnPriceGrp" runat="server" Value='<%# Bind("price_group") %>' />
                                                                    <asp:HiddenField ID="hdnPriceGrpCode" runat="server" Value='<%# Bind("price_group_code") %>' />
                                                                    <asp:HiddenField ID="hdnSalesTrigger" runat="server" Value='<%# Bind("sales_trigger") %>' />
                                                                    <asp:HiddenField ID="hdnValueTrigger" runat="server" Value='<%# Bind("value_trigger") %>' />
                                                                    <asp:HiddenField ID="hdnSalesPct" runat="server" Value='<%# Bind("sales_pct") %>' />
                                                                    <asp:HiddenField ID="hdnRoyaltyRate" runat="server" Value='<%# Bind("royalty_rate","{0:0.####}") %>' />
                                                                    <asp:HiddenField ID="hdnUnitRate" runat="server" Value='<%# Bind("unit_rate","{0:0.####}") %>' />
                                                                    <asp:HiddenField ID="hdnRevenueRate" runat="server" Value='<%# Bind("revenue_rate","{0:0.####}") %>' />
                                                                    <asp:HiddenField ID="hdnIsModified" runat="server" Value='<%# Bind("is_modified") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table width="98.5%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td width="4%" class="gridHeaderStyle_1row">Esc #</td>
                                            <td width="21%" class="gridHeaderStyle_1row">Territory</td>
                                            <td width="20%" class="gridHeaderStyle_1row">Configuration Code</td>
                                            <td width="15%" class="gridHeaderStyle_1row">Sales Type</td>
                                            <td width="6%" class="gridHeaderStyle_1row">Sales Trigger</td>
                                            <td width="6%" class="gridHeaderStyle_1row">Value Trigger</td>
                                            <td width="6%" class="gridHeaderStyle_1row">% of Sales</td>
                                            <td width="6%" class="gridHeaderStyle_1row">Royalty Rate</td>
                                            <td width="6%" class="gridHeaderStyle_1row">Unit Rate</td>
                                            <td width="6%" class="gridHeaderStyle_1row">Revenue Rate</td>
                                            <td width="4%" class="gridHeaderStyle_1row">&nbsp</td>
                                        </tr>
                                        <tr>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:TextBox ID="txtEscCodeAddRow" runat="server" Width="70%" CssClass="textboxStyle" MaxLength="2"
                                                    ToolTip="Please enter alphanumeric text upto 2 characters" Style="text-align: center; text-transform: uppercase;" TabIndex="101">
                                                </asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvtxtEscCodeAddRow" ControlToValidate="txtEscCodeAddRow" ValidationGroup="valGrpAppendAddRow"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Escalation code" Display="Dynamic"></asp:RequiredFieldValidator>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:TextBox ID="txtTerritoryAddRow" runat="server" Width="98%" CssClass="textbox_FuzzySearch" TabIndex="102"
                                                    onkeydown="OntxtTerritoryAddRowKeyDown(this);" onchange="OntxtTerritoryAddRowChange();"></asp:TextBox>
                                                <ajaxToolkit:AutoCompleteExtender ID="aceTerritoryAddRow" runat="server"
                                                    ServiceMethod="FuzzySearchAllSellerGroupList"
                                                    ServicePath="~/Services/FuzzySearch.asmx"
                                                    MinimumPrefixLength="1"
                                                    CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                    TargetControlID="txtTerritoryAddRow"
                                                    FirstRowSelected="true"
                                                    OnClientPopulating="territoryAddRowListPopulating"
                                                    OnClientPopulated="territoryAddRowListPopulated"
                                                    OnClientHidden="territoryAddRowListHidden"
                                                    OnClientItemSelected="territoryAddRowListItemSelected"
                                                    CompletionListElementID="pnlTerritoryAddRowFuzzySearch" />
                                                <asp:Panel ID="pnlTerritoryAddRowFuzzySearch" runat="server" CssClass="identifierLable"
                                                    Style="bottom: 0px; top: 1000px; position: absolute; height: 200px;" />
                                                <asp:CustomValidator ID="valtxtTerritoryAddRow" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator"
                                                    ClientValidationFunction="ValTerritoryAddRow" ToolTip="Please select valid territory from the search list"
                                                    ControlToValidate="txtTerritoryAddRow" ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:TextBox ID="txtConfigAddRow" runat="server" Width="98%" CssClass="textbox_FuzzySearch" TabIndex="103"
                                                    onkeydown="OntxtConfigAddRowKeyDown(this);" onchange="OntxtConfigChangeAddRow();"></asp:TextBox>
                                                <ajaxToolkit:AutoCompleteExtender ID="aceConfigAddRow" runat="server"
                                                    ServiceMethod="FuzzySearchAllConfigGroupList"
                                                    ServicePath="~/Services/FuzzySearch.asmx"
                                                    MinimumPrefixLength="1"
                                                    CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                    TargetControlID="txtConfigAddRow"
                                                    FirstRowSelected="true"
                                                    OnClientPopulating="configAddRowListPopulating"
                                                    OnClientPopulated="configAddRowListPopulated"
                                                    OnClientHidden="configAddRowListHidden"
                                                    OnClientItemSelected="configAddRowListItemSelected"
                                                    CompletionListElementID="pnlConfigAddRowFuzzySearch" />
                                                <asp:Panel ID="pnlConfigAddRowFuzzySearch" runat="server" CssClass="identifierLable"
                                                    Style="bottom: 0px; top: 1000px; position: absolute; height: 200px;" />
                                                <asp:CustomValidator ID="valtxtConfigAddRow" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator"
                                                    ClientValidationFunction="ValConfigAddRow" ToolTip="Please select valid configuration from the search list"
                                                    ControlToValidate="txtConfigAddRow" ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:TextBox ID="txtSalesTypeAddRow" runat="server" Width="98%" CssClass="textbox_FuzzySearch" TabIndex="104"
                                                    onkeydown="OntxtSalesTypeAddRowKeyDown(this);" onchange="OntxtSalesTypeAddRowChange();"></asp:TextBox>
                                                <ajaxToolkit:AutoCompleteExtender ID="aceSalesTypeAddRow" runat="server"
                                                    ServiceMethod="FuzzySearchAllPriceGroupList"
                                                    ServicePath="~/Services/FuzzySearch.asmx"
                                                    MinimumPrefixLength="1"
                                                    CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                    TargetControlID="txtSalesTypeAddRow"
                                                    FirstRowSelected="true"
                                                    OnClientPopulating="salesTypeAddRowListPopulating"
                                                    OnClientPopulated="salesTypeAddRowListPopulated"
                                                    OnClientHidden="salesTypeAddRowListHidden"
                                                    OnClientItemSelected="salesTypeAddRowListItemSelected"
                                                    CompletionListElementID="pnlSalesTypeAddRowFuzzySearch" />
                                                <asp:Panel ID="pnlSalesTypeAddRowFuzzySearch" runat="server" CssClass="identifierLable"
                                                    Style="bottom: 0px; top: 1000px; position: absolute; height: 200px;" />
                                                <asp:CustomValidator ID="valtxtSalesTypeAddRow" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator"
                                                    ClientValidationFunction="ValSalesTypeAddRow" ToolTip="Please select valid sales type from the search list"
                                                    ControlToValidate="txtSalesTypeAddRow" ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:TextBox ID="txtSalesTriggerAddRow" runat="server" Width="70%" CssClass="textboxStyle"
                                                    ToolTip="Please enter only positive number" Style="text-align: center" TabIndex="105"></asp:TextBox>
                                                <asp:CustomValidator ID="valtxtSalesTriggerAddRow" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator"
                                                    ClientValidationFunction="ValSalesTriggerAddRow" ToolTip="Please enter either sales trigger or value trigger"
                                                    ErrorMessage="*"></asp:CustomValidator>
                                                <ajaxToolkit:FilteredTextBoxExtender ID="ftetxtSalesTriggerAddRow" runat="server"
                                                    Enabled="True" TargetControlID="txtSalesTriggerAddRow" FilterType="Numbers">
                                                </ajaxToolkit:FilteredTextBoxExtender>
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:TextBox ID="txtValueTriggerAddRow" runat="server" Width="70%" CssClass="textboxStyle"
                                                    ToolTip="Please enter only positive number" Style="text-align: center" TabIndex="106"></asp:TextBox>
                                                <asp:CustomValidator ID="valtxtValueTriggerAddRow" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator"
                                                    ClientValidationFunction="ValValueTriggerAddRow" ToolTip="Please enter either sales trigger or value trigger"
                                                    ErrorMessage="*"></asp:CustomValidator>
                                                <ajaxToolkit:FilteredTextBoxExtender ID="ftetxtValueTriggerAddRow" runat="server"
                                                    Enabled="True" TargetControlID="txtValueTriggerAddRow" FilterType="Numbers">
                                                </ajaxToolkit:FilteredTextBoxExtender>
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:TextBox ID="txtPctSalesAddRow" runat="server" Width="70%" CssClass="textboxStyle" MaxLength="6" onchange="SalesPctWarningAddRow();"
                                                    ToolTip="Please enter only positive number <= 100 upto 2 decimal places" Style="text-align: center" TabIndex="107"></asp:TextBox>
                                                <asp:CustomValidator ID="valtxtPctSalesAddRow" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator"
                                                    ClientValidationFunction="ValPctSalesAddRow" ToolTip="Please enter either % of sales or a rate"
                                                    ErrorMessage="*"></asp:CustomValidator>
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:TextBox ID="txtRoyaltyRateAddRow" runat="server" Width="75%" CssClass="textboxStyle"
                                                    ToolTip="Please enter a number up to 4 decimal places." Style="text-align: center" TabIndex="108"></asp:TextBox>
                                                <asp:CustomValidator ID="valtxtRoyaltyRateAddRow" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator"
                                                    ClientValidationFunction="ValRoyaltyRateAddRow" ToolTip="Please enter either royalty rate or unit rate or revenue rate"
                                                    ErrorMessage="*"></asp:CustomValidator>

                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:TextBox ID="txtUnitRateAddRow" runat="server" Width="75%" CssClass="textboxStyle"
                                                    ToolTip="Please enter a number up to 4 decimal places." Style="text-align: center" TabIndex="109"></asp:TextBox>
                                                <asp:CustomValidator ID="valtxtUnitRateAddRow" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator"
                                                    ClientValidationFunction="ValUnitRateAddRow" ToolTip="Please enter either royalty rate or unit rate or revenue rate"
                                                    ErrorMessage="*"></asp:CustomValidator>

                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:TextBox ID="txtRevenueRateAddRow" runat="server" Width="70%" CssClass="textboxStyle"
                                                    ToolTip="Please enter a number up to 4 decimal places." Style="text-align: center" TabIndex="110"></asp:TextBox>
                                                <asp:CustomValidator ID="valtxtRevenueRateAddRow" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator"
                                                    ClientValidationFunction="ValRevenueRateAddRow" ToolTip="Please enter either royalty rate or unit rate or revenue rate"
                                                    ErrorMessage="*"></asp:CustomValidator>

                                            </td>
                                            <td class="insertBoxStyle">
                                                <table width="95%">
                                                    <tr>
                                                        <td align="center">
                                                            <asp:ImageButton ID="btnAppendAddRow" runat="server" ImageUrl="../Images/add_row.png"
                                                                ToolTip="Add Rate" OnClientClick="if (!ValidatePopUpAddRow()) { return false;};" OnClick="btnAppendAddRow_Click" ValidationGroup="valGrpAppendAddRow" TabIndex="111"
                                                                onkeydown="OnAppendAddRowKeyDown(this);" />
                                                        </td>
                                                        <td align="center">
                                                            <asp:ImageButton ID="btnUndoAddRow" runat="server" ImageUrl="../Images/cancel_row3.png"
                                                                ToolTip="Clear Add row" CausesValidation="false" OnClientClick="return ClearAddRow();"
                                                                TabIndex="112" onkeydown="OnAddRowUndoTabPress();" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td></td>
                </tr>
            </table>

            <asp:UpdateProgress ID="progressBarPageLevel" runat="server" AssociatedUpdatePanelID="updPnlPageLevel" DisplayAfter="100">
                <ProgressTemplate>
                    <div id="Search" style="font-weight: bold; color: Black; z-index: 2">
                        <img src="../Images/InProgress2.gif" alt="" />
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <ajaxToolkit:ModalPopupExtender ID="mPopupPageLevel" runat="server" PopupControlID="progressBarPageLevel" TargetControlID="progressBarPageLevel"
                BackgroundCssClass="progressBar" RepositionMode="RepositionOnWindowResize">
            </ajaxToolkit:ModalPopupExtender>

            <%--fuzzy search - full search - Ends--%>
            <asp:Button ID="dummyFuzzySearch" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeFuzzySearch" runat="server" PopupControlID="pnlFuzzySearch" TargetControlID="dummyFuzzySearch"
                CancelControlID="btnCloseFuzzySearchPopup" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlFuzzySearch" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td>
                                        <asp:Label ID="lblFuzzySearchPopUp" runat="server" Text="Complete Search List" CssClass="identifierLable"></asp:Label>
                                    </td>
                                    <td align="right" style="vertical-align: top;">
                                        <asp:ImageButton ID="btnCloseFuzzySearchPopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                                    </td>
                                </tr>
                            </table>
                        </td>

                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:ListBox ID="lbFuzzySearch" runat="server" Width="95%" CssClass="ListBox"
                                OnSelectedIndexChanged="lbFuzzySearch_SelectedIndexChanged" AutoPostBack="True"></asp:ListBox>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--fuzzy search - full search - Ends--%>

            <%--Sales category ProRata popup--%>
            <asp:Button ID="dummyProRataPopup" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeProRataPopup" runat="server" PopupControlID="pnlProRataPopup" TargetControlID="dummyProRataPopup"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlProRataPopup" runat="server" align="left" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td width="10%"></td>
                                    <td class="identifierLable" align="center">Sales Category ProRata</td>
                                    <td align="right" style="vertical-align: top;" width="10%">
                                        <asp:ImageButton ID="btnCloseProrataPopup" ImageUrl="../Images/CloseIcon.png" OnClientClick="return CancelProRata();" runat="server" Style="cursor: pointer" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <tr>
                            <td>
                                <table width="98%" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <table width="95.75%" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td width="30%" class="gridHeaderStyle_1row">Esc Code</td>
                                                    <td width="30%" class="gridHeaderStyle_1row">Sales Category</td>
                                                    <td width="40%" class="gridHeaderStyle_1row">ProRata Value</td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Panel ID="plnGridProRataPopup" runat="server" ScrollBars="Auto">
                                                <asp:GridView ID="gvSalesCategoryProRata" runat="server" AutoGenerateColumns="False" Width="95.75%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                    BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                    EmptyDataText="No data found" ShowHeader="false" RowStyle-CssClass="dataRow">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align"
                                                            ItemStyle-Width="30%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="ldlEscCode" runat="server" Width="99%" Text='<%# Bind("esc_code") %>' CssClass="identifierLable"></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align"
                                                            ItemStyle-Width="30%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lenSalesCategory" runat="server" Width="99%" Text='<%# Bind("price_type") %>' CssClass="identifierLable"></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Right_Align"
                                                            ItemStyle-Width="40%">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="txtProRata" runat="server" Width="70%" Text='<%#Bind("escalation_prorata")%>' CssClass="gridTextField" Style="text-align: right"></asp:TextBox>
                                                                <asp:RequiredFieldValidator runat="server" ID="rfvtxtProRata" ControlToValidate="txtProRata" ValidationGroup="valGrpSaveProRata"
                                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter ProRata value" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                <asp:RegularExpressionValidator ID="revtxtProRata" runat="server" Text="*" ControlToValidate="txtProRata" ValidationGroup="valGrpSaveProRata"
                                                                    ValidationExpression="^[0-9]*$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                    ToolTip="Please enter only positive number" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                                <asp:HiddenField ID="hdnEscCode" runat="server" Value='<%# Bind("esc_code") %>' />
                                                                <asp:HiddenField ID="hdnProRata" runat="server" Value='<%# Bind("escalation_prorata") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <table width="30%">
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="btnSaveProRata" runat="server" CssClass="ButtonStyle"
                                                            OnClientClick="if (!ValidateSaveProRata()) { return false;};" OnClick="btnSaveProRata_Click"
                                                            Text="Save" UseSubmitBehavior="false" Width="90%" ValidationGroup="valGrpSaveProRata" />
                                                    </td>
                                                    <td width="10%"></td>
                                                    <td>
                                                        <asp:Button ID="btnCancelProRata" runat="server" CssClass="ButtonStyle"
                                                            OnClientClick="return CancelProRata();" Text="Cancel" UseSubmitBehavior="false" Width="90%" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                </table>
            </asp:Panel>
            <%--Interested party search list popup-- Ends%>--%>

            <%--Add default prorata confirmation popup--%>
            <asp:Button ID="dummyProrataConfirm" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeProrataConfirm" runat="server" PopupControlID="pnlProrataConfirm" TargetControlID="dummyProrataConfirm"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlProrataConfirm" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td class="identifierLable" align="center" style="height: 20px">
                        Confirmation
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblmpeProrataConfirm" runat="server" CssClass="identifierLable"
                                Text="Do you want to set up default prorata for the royaltor escalation codes not been set up?"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYesProrataConfirm" runat="server" Text="Yes" CssClass="ButtonStyle" OnClick="btnYesProrataConfirm_Click" CausesValidation="false" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNoProrataConfirm" runat="server" Text="No" CssClass="ButtonStyle" OnClick="btnNoProrataConfirm_Click" CausesValidation="false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Copy participant popup--%>

            <%--Warning on unsaved data popup--%>
            <asp:Button ID="dummyUnsavedWarnMsg" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeUnSavedWarning" runat="server" PopupControlID="pnlUnsavedWarnMsgPopup" TargetControlID="dummyUnsavedWarnMsg"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlUnsavedWarnMsgPopup" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmOnUnsavedData" runat="server" Text="Unsaved Data Warning" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblUnSavedWarnMsg" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td width="48%" align="right">
                                        <asp:Button ID="btnUnSavedDataReturn" runat="server" Text="Return" CssClass="ButtonStyle" Width="30%" OnClientClick="return OnUnSavedDataReturn();" />
                                    </td>
                                    <td width="4%"></td>
                                    <td width="48%" align="left">
                                        <asp:Button ID="btnUnSavedDataExit" runat="server" Text="Exit" CssClass="ButtonStyle" Width="30%" OnClick="btnUnSavedDataExit_Click" OnClientClick="if (!OnUnSavedDataExit()) { return false;};" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- Start--%>
            <asp:Button ID="dummyConfirmDelete" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmDelete" runat="server" PopupControlID="pnlConfirmDelete" TargetControlID="dummyConfirmDelete"
                CancelControlID="btnNo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlConfirmDelete" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmation" runat="server" Text="Delete Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblText" runat="server"
                                CssClass="identifierLable" Text="Are you sure you want to delete this row?"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYes" runat="server" Text="Yes" CssClass="ButtonStyle" OnClick="btnYes_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNo" runat="server" Text="No" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- End--%>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnGridDataDeleted" runat="server" Value="N" />
            <asp:HiddenField ID="hdnNewRoyaltorSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" />
            <asp:HiddenField ID="hdnFuzzySearchText" runat="server" />
            <asp:HiddenField ID="hdnGridFuzzySearchRowId" runat="server" />
            <asp:Button ID="btnFuzzyTerritoryListPopup" runat="server" Style="display: none;" OnClick="btnFuzzyTerritoryListPopup_Click" CausesValidation="false" />
            <asp:Button ID="btnFuzzyConfigListPopup" runat="server" Style="display: none;" OnClick="btnFuzzyConfigListPopup_Click" CausesValidation="false" />
            <asp:Button ID="btnFuzzySalesTypeListPopup" runat="server" Style="display: none;" OnClick="btnFuzzySalesTypeListPopup_Click" CausesValidation="false" />
            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- Start--%>
            <asp:HiddenField ID="hdnDeleteEscProfileId" runat="server" Value="N" />
            <asp:HiddenField ID="hdnDeleteEscLevel" runat="server" Value="N" />
            <asp:HiddenField ID="hdnDeleteIsModified" runat="server" Value="N" />
            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- End--%>
            <asp:HiddenField ID="hdnIsAuditScreen" runat="server" Value="N" />
            <asp:HiddenField ID="hdnOtherUserScreenLocked" runat="server" Value="N" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
            <asp:HiddenField ID="hdnButtonSelection" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

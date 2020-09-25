<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoyContractSubRates.aspx.cs" Inherits="WARS.Contract.RoyContractSubRates" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Royaltor Contract Subsidiary Rates" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

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
        var gridClientId = "ContentPlaceHolderBody_gvSubRates_";

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

        //Confim delete
        function ConfirmDelete(row) {
            //debugger;
            //JIRA-908 Changes by Ravi on 13/02/2019 -- Start
            gridRowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            hdnIsModified = document.getElementById(gridClientId + 'hdnIsModified' + '_' + gridRowIndex).value;
            hdnRateId = document.getElementById(gridClientId + 'hdnRateId' + '_' + gridRowIndex).value;
            if (hdnIsModified != "-") {
                document.getElementById("<%=hdnGridDataDeleted.ClientID %>").innerText = "Y";
            }
            document.getElementById("<%=hdnDeleteRateId.ClientID %>").innerText = hdnRateId;
            document.getElementById("<%=hdnDeleteIsModified.ClientID %>").innerText = hdnIsModified;
            var popup = $find('<%= mpeConfirmDelete.ClientID %>');
            if (popup != null) {
                popup.show();
            }
            return false;
            //JIRA-908 Changes by Ravi on 13/02/2019 -- End

        }
        //============== End

        //Undo Grid changes
        function UndoGridChanges(gridRow) {
            gridRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);

            hdnOptionPeriod = document.getElementById(gridClientId + 'hdnOptionPeriod' + '_' + gridRowIndex);
            ddlOptionPeriod = document.getElementById(gridClientId + 'ddlOptionPeriod' + '_' + gridRowIndex);
            hdnSellerGrp = document.getElementById(gridClientId + 'hdnSellerGrp' + '_' + gridRowIndex);
            txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + gridRowIndex);
            hdnConfigGrp = document.getElementById(gridClientId + 'hdnConfigGrp' + '_' + gridRowIndex);
            txtConfig = document.getElementById(gridClientId + 'txtConfig' + '_' + gridRowIndex);
            hdnPriceGrp = document.getElementById(gridClientId + 'hdnPriceGrp' + '_' + gridRowIndex);
            txtSalesType = document.getElementById(gridClientId + 'txtSalesType' + '_' + gridRowIndex);
            hdnRevenueRate = document.getElementById(gridClientId + 'hdnRevenueRate' + '_' + gridRowIndex);
            txtRevenueRate = document.getElementById(gridClientId + 'txtRevenueRate' + '_' + gridRowIndex);
            hdnRoyaltyRate = document.getElementById(gridClientId + 'hdnRoyaltyRate' + '_' + gridRowIndex);
            txtRoyaltyRate = document.getElementById(gridClientId + 'txtRoyaltyRate' + '_' + gridRowIndex);

            if (hdnOptionPeriod.value == ".") {
                ddlOptionPeriod.value = "-";
            }
            else {
                ddlOptionPeriod.value = hdnOptionPeriod.value;
            }

            txtTerritory.value = hdnSellerGrp.value;
            txtConfig.value = hdnConfigGrp.value;
            txtSalesType.value = hdnPriceGrp.value;
            txtRevenueRate.value = hdnRevenueRate.value;
            txtRoyaltyRate.value = hdnRoyaltyRate.value;

            Page_ClientValidate('');//clear all validators of the page

            txtTerritory.style["width"] = '98%';
            txtConfig.style["width"] = '98%';
            txtSalesType.style["width"] = '97%';

            return false;

        }

        //============== End

        //clear add row data
        function ClearAddRow() {
            document.getElementById('<%=ddlOptionPeriodAddRow.ClientID%>').selectedIndex = 0;
            txtTerritoryAddRow = document.getElementById('<%=txtTerritoryAddRow.ClientID%>');
            txtConfigAddRow = document.getElementById('<%=txtConfigAddRow.ClientID%>');
            txtSalesTypeAddRow = document.getElementById('<%=txtSalesTypeAddRow.ClientID%>');
            document.getElementById('<%=txtRevenueRateAddRow.ClientID%>').value = "";
            document.getElementById('<%=txtRoyaltyRateAddRow.ClientID%>').value = "";
            Page_ClientValidate('');//clear all validators of the page

            txtTerritoryAddRow.value = "";
            txtConfigAddRow.value = "";
            txtSalesTypeAddRow.value = "";
            txtTerritoryAddRow.style["width"] = '98%';
            txtConfigAddRow.style["width"] = '98%';
            txtSalesTypeAddRow.style["width"] = '98%';

            return false;

        }
        //============== End  

        //Validate any unsaved data on browser window close/refresh
        //set flag value when data is changed

        function IsAddRowDataChanged() {
            var ddlOptionPeriodAddRow = document.getElementById("<%=ddlOptionPeriodAddRow.ClientID %>").value;
            var txtTerritoryAddRow = document.getElementById("<%=txtTerritoryAddRow.ClientID %>").value;
            var txtConfigAddRow = document.getElementById("<%=txtConfigAddRow.ClientID %>").value;
            var txtSalesTypeAddRow = document.getElementById("<%=txtSalesTypeAddRow.ClientID %>").value;
            var txtRoyaltyRateAddRow = document.getElementById("<%=txtRoyaltyRateAddRow.ClientID %>").value;
            var txtRevenueRateAddRow = document.getElementById("<%=txtRevenueRateAddRow.ClientID %>").value;

            if (ddlOptionPeriodAddRow != '-' || txtTerritoryAddRow != '' || txtConfigAddRow != '' || txtSalesTypeAddRow != '' ||
                txtRoyaltyRateAddRow != '' || txtRevenueRateAddRow != '') {
                return true;
            }
            else {
                return false;
            }


        }

        function IsGridDataChanged() {
            var gvSubRates = document.getElementById("<%= gvSubRates.ClientID %>");
            var hdnGridDataDeleted = document.getElementById("<%=hdnGridDataDeleted.ClientID %>").value;

            if (gvSubRates != null) {
                var gvRows = gvSubRates.rows; // WUIN-746 grid view rows including header row
                var isGridDataChanged = "N";

                var rowIndex;
                for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                    rowIndex = i - 1; //WUIN-746 row index start from 0
                    hdnIsModified = document.getElementById(gridClientId + 'hdnIsModified' + '_' + rowIndex);
                    //handling empty data row
                    if (gvRows.length == 2 && hdnIsModified == null) {
                        break;
                    }

                    if (hdnIsModified.value == "-") {
                        isGridDataChanged = "Y";
                        break;
                    }

                    var hdnOptionPeriod = document.getElementById(gridClientId + 'hdnOptionPeriod' + '_' + rowIndex).value;
                    var ddlOptionPeriod = document.getElementById(gridClientId + 'ddlOptionPeriod' + '_' + rowIndex).value;
                    var hdnSellerGrp = document.getElementById(gridClientId + 'hdnSellerGrp' + '_' + rowIndex).value;
                    var txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + rowIndex).value;
                    var hdnConfigGrp = document.getElementById(gridClientId + 'hdnConfigGrp' + '_' + rowIndex).value;
                    var txtConfig = document.getElementById(gridClientId + 'txtConfig' + '_' + rowIndex).value;
                    var hdnPriceGrp = document.getElementById(gridClientId + 'hdnPriceGrp' + '_' + rowIndex).value;
                    var txtSalesType = document.getElementById(gridClientId + 'txtSalesType' + '_' + rowIndex).value;
                    var hdnRevenueRate = document.getElementById(gridClientId + 'hdnRevenueRate' + '_' + rowIndex).value;
                    var txtRevenueRate = document.getElementById(gridClientId + 'txtRevenueRate' + '_' + rowIndex).value;
                    var hdnRoyaltyRate = document.getElementById(gridClientId + 'hdnRoyaltyRate' + '_' + rowIndex).value;
                    var txtRoyaltyRate = document.getElementById(gridClientId + 'txtRoyaltyRate' + '_' + rowIndex).value;
                    var hdnIsModified = document.getElementById(gridClientId + 'hdnIsModified' + '_' + rowIndex).value;
                    //debugger;
                    if (ddlOptionPeriod == '-')
                        ddlOptionPeriod = '.';

                    if (hdnOptionPeriod != ddlOptionPeriod || hdnSellerGrp != txtTerritory || hdnConfigGrp != txtConfig || hdnPriceGrp != txtSalesType
                        || hdnRoyaltyRate != txtRoyaltyRate || hdnRevenueRate != txtRevenueRate) {
                        isGridDataChanged = "Y";
                        break;
                    }

                }

                if (isGridDataChanged == "Y") {
                    return true;
                }
                else {
                    return false;
                }
            }
        }


        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
                window.location = "../Common/ExceptionPage.aspx";
            }

            //redirect to bank details screen on saving data of new royaltor so that issue of data not saved validation would be handled
            function RedirectOnNewRoyaltorSave(royaltorId) {
                //debugger;
                document.getElementById("<%=hdnNewRoyaltorSaved.ClientID %>").value = "Y";
            window.location = "../Contract/RoyContractPkgRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y";
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

        var unSaveBrowserClose = false;

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

        //============== End

        //Audit button navigation
        function RedirectToAuditScreen(royaltorId) {
            //debugger;      

            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Audit/RoyaltorSubsidiaryRatesAudit.aspx?RoyaltorId=" + royaltorId + "");
            }
            else {
                window.location = "../Audit/RoyaltorSubsidiaryRatesAudit.aspx?RoyaltorId=" + royaltorId + "";
            }
        }

        function RedirectToPreviousScreen(royaltorId) {
            //debugger;        
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Contract/RoyContractRoyRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y");
            }
            else {
                window.location = "../Contract/RoyContractRoyRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y";
            }
        }
        //=================End

        //Validations==============Begin
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

        function ValidatePopUpSave() {
            //warning on save validation fail        
            if (!Page_ClientValidate("valSave")) {
                Page_BlockSubmit = false;
                DisplayMessagePopup("Subsidiary rates not saved – invalid or missing data!");
                return false;
            }
            else {
                return true;
            }

        }

        var txtRoyaltyRate;
        var txtRevenueRate;
        var valtxtRoyaltyRateAddRow;
        var valtxtRevenueRateAddRow;
        var valtxtRoyaltyRate;
        var valtxtRevenueRate;
        var strGridId = "ContentPlaceHolderBody_gvSubRates_";
        //var rateRegex = /^100$|^\d{0,2}(\.\d{1,4})? *%?$/;//only positive number <= 100 up to 4 decimal places  
        var rateRegex = /^\-?\d+(\.\d{1,4})? *%?$/;//only number up to 4 decimal places  

        function GetAddRowValues() {
            txtRoyaltyRate = document.getElementById("<%=txtRoyaltyRateAddRow.ClientID %>").value;
            txtRevenueRate = document.getElementById("<%=txtRevenueRateAddRow.ClientID %>").value;
            valtxtRoyaltyRateAddRow = document.getElementById("<%=valtxtRoyaltyRateAddRow.ClientID %>");
            valtxtRevenueRateAddRow = document.getElementById("<%=valtxtRevenueRateAddRow.ClientID %>");
        }

        function GetGridRowValues(rowIndex) {
            txtRoyaltyRate = document.getElementById(strGridId + 'txtRoyaltyRate' + '_' + rowIndex).value;
            txtRevenueRate = document.getElementById(strGridId + 'txtRevenueRate' + '_' + rowIndex).value;
            valtxtRoyaltyRate = document.getElementById(strGridId + 'valtxtRoyaltyRate' + '_' + rowIndex);
            valtxtRevenueRate = document.getElementById(strGridId + 'valtxtRevenueRate' + '_' + rowIndex);
        }

        function ValRoyaltyRateAddRow(sender, args) {
            GetAddRowValues();
            if (txtRoyaltyRate == "") {
                args.IsValid = true;
                return;
            }
            else if (txtRoyaltyRate != "" && txtRevenueRate != "") {
                //validate either royalty rate or unit rate
                args.IsValid = false;
                valtxtRoyaltyRateAddRow.title = "Please enter either royalty rate or revenue rate";
                return;
            }

            if (rateRegex.test(txtRoyaltyRate)) {
                args.IsValid = true;
            }
            else {
                args.IsValid = false;
                valtxtRoyaltyRateAddRow.title = "Please enter a number up to 4 decimal places";
            }
        }

        function ValRevenueRateAddRow(sender, args) {
            GetAddRowValues();
            if (txtRevenueRate == "") {
                args.IsValid = true;
                return;
            }
            else if (txtRoyaltyRate != "" && txtRevenueRate != "") {
                //validate either royalty rate or unit rate
                args.IsValid = false;
                valtxtRevenueRateAddRow.title = "Please enter either royalty rate or revenue rate";
                return;
            }

            if (rateRegex.test(txtRevenueRate)) {
                args.IsValid = true;
            }
            else {
                args.IsValid = false;
                valtxtRevenueRateAddRow.title = "Please enter a number up to 4 decimal places";
            }
        }

        function ValRoyaltyRateGridRow(sender, args) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            GetGridRowValues(gridRowIndex);

            if (txtRoyaltyRate == "") {
                args.IsValid = true;
                return;
            }
            else if (txtRoyaltyRate != "" && txtRevenueRate != "") {
                //validate either royalty rate or unit rate
                args.IsValid = false;
                valtxtRoyaltyRate.title = "Please enter either royalty rate or revenue rate";
                return;
            }

            if (rateRegex.test(txtRoyaltyRate)) {
                args.IsValid = true;
            }
            else {
                args.IsValid = false;
                valtxtRoyaltyRate.title = "Please enter a number up to 4 decimal places";
            }
        }

        function ValRevenueRateGridRow(sender, args) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            GetGridRowValues(gridRowIndex);

            if (txtRevenueRate == "") {
                args.IsValid = true;
                return;
            }
            else if (txtRoyaltyRate != "" && txtRevenueRate != "") {
                //validate either royalty rate or unit rate
                args.IsValid = false;
                valtxtRevenueRate.title = "Please enter either royalty rate or revenue rate";
                return;
            }

            if (rateRegex.test(txtRevenueRate)) {
                args.IsValid = true;
            }
            else {
                args.IsValid = false;
                valtxtRevenueRate.title = "Please enter a number up to 4 decimal places";
            }
        }

        //Validations=================End

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

        //Validate if the field value is a valid one from fuzzy search list
        function ValTerritoryGridRow(sender, args) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtTerritory = document.getElementById(strGridId + 'txtTerritory' + '_' + gridRowIndex);

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
            txtConfig = document.getElementById(strGridId + 'txtConfig' + '_' + gridRowIndex);

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
            txtSalesType = document.getElementById(strGridId + 'txtSalesType' + '_' + gridRowIndex);
            txtSalesType.style["width"] = '97%';
            if (txtSalesType.value == "") {
                args.IsValid = false;
                //adjust width of the textbox to display error
                fieldWidth = txtSalesType.offsetWidth;
                txtSalesType.style["width"] = (fieldWidth - 20);
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
            txtTerritory = document.getElementById(strGridId + 'txtTerritory' + '_' + gridRowIndex);

            if (txtTerritory.value == "") {
                txtTerritory.style["width"] = '98%';
            }
        }

        //reset field width when empty
        function OntxtConfigChange(sender) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtConfig = document.getElementById(strGridId + 'txtConfig' + '_' + gridRowIndex);

            if (txtConfig.value == "") {
                txtConfig.style["width"] = '98%';
            }
        }

        //reset field width when empty
        function OntxtSalesTypeChange(sender) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtSalesType = document.getElementById(strGridId + 'txtSalesType' + '_' + gridRowIndex);

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
            txtSalesTypeAddRow.style["width"] = '98%';
            if (txtSalesTypeAddRow.value == "") {
                args.IsValid = false;
                //adjust width of the textbox to display error
                fieldWidth = txtSalesTypeAddRow.offsetWidth;
                txtSalesTypeAddRow.style["width"] = (fieldWidth - 20);
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

        //Validations=================End

        //================================End

        //JIRA-451 CHanges -- Start
        function OnAppendAddRowKeyDown() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnAppendAddRow.ClientID%>').click();
            }
        }
        //JIRA-451 Changes -- ENd

        function ValidateDisplayOrder(button) {
            if (IsDataChanged()) {
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
                return false;
            }
            return true;
        }


        //WUIN-932 Unsaved Pop-up implementation
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
            window.onbeforeunload = WarnOnUnSavedData;

            var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
            if (warnPopup != null) {
                warnPopup.hide();
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
                                    ROYALTOR CONTRACT - SUBSIDIARY RATES
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
                    <td width="15%" rowspan="4" valign="top" align="right">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td width="30%"></td>
                                            <td align="right" width="70%">
                                                <asp:Button ID="btnSave" runat="server" CssClass="ButtonStyle" OnClientClick="if (!ValidatePopUpSave()) { return false;};"
                                                    OnClick="btnSave_Click" Text="Save Changes" UseSubmitBehavior="false" Width="90%" ValidationGroup="valSave" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td align="right">
                                                <asp:Button ID="btnAudit" runat="server" CssClass="ButtonStyle" OnClick="btnAudit_Click"
                                                    Text="Audit" UseSubmitBehavior="false" Width="90%" />
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
                                            <td width="30%"></td>
                                            <td width="70%">
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
                    <td class="table_header_with_border" valign="top">Subsidiary Royalty Rates</td>
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
                                                <table width="98.5%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td colspan="7" align="right">
                                                            <asp:Button ID="btnDisplayOrder" runat="server" CssClass="ButtonStyle" OnClick="btnDisplayOrder_Click"  OnClientClick="if (!ValidateDisplayOrder('HierarchyOrder')) { return false;};"
                                                                Text="Hierarchy Order" UseSubmitBehavior="false" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="7" style="height: 5px"></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvSubRates" runat="server" AutoGenerateColumns="False" Width="98.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found" OnRowDataBound="gvSubRates_RowDataBound" AllowSorting="true" OnSorting="gvSubRates_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Option Period" SortExpression="option_period_code" ItemStyle-Width="12%">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlOptionPeriod" runat="server" Width="98%" CssClass="ddlStyle"></asp:DropDownList>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Territory" SortExpression="seller_group" ItemStyle-Width="25%">
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
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Sales Type" SortExpression="price_group" ItemStyle-Width="18%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtSalesType" runat="server" Width="97%" Text='<%#Bind("price_group")%>' CssClass="textbox_FuzzySearch"
                                                                        ToolTip='<%#Bind("price_group")%>' onkeydown="OntxtSalesTypeKeyDown(this);" onchange="OntxtSalesTypeChange(this);"></asp:TextBox>
                                                                    <ajaxToolkit:AutoCompleteExtender ID="aceSalesType" runat="server"
                                                                        ServiceMethod="FuzzySearchContSubRatesSalesTypeList"
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
                                                                        ControlToValidate="txtSalesType" ErrorMessage="*" Display="Dynamic" ValidateEmptyText="true"></asp:CustomValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Configuration Code" SortExpression="config_group" ItemStyle-Width="25%">
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
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Revenue Rate" SortExpression="revenue_rate" ItemStyle-Width="8%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtRevenueRate" runat="server" Width="70%" Text='<%#Bind("revenue_rate","{0:0.####}")%>' CssClass="gridTextField" Style="text-align: center"
                                                                        ToolTip="Please enter a number up to 4 decimal places and either revenue rate or royalty rate"></asp:TextBox>
                                                                    <asp:CustomValidator ID="valtxtRevenueRate" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator"
                                                                        ClientValidationFunction="ValRevenueRateGridRow" ToolTip="Please enter either revenue rate or royalty rate"
                                                                        ErrorMessage="*"></asp:CustomValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Royalty Rate" SortExpression="royalty_rate" ItemStyle-Width="8%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtRoyaltyRate" runat="server" Width="70%" Text='<%#Bind("royalty_rate","{0:0.####}")%>' CssClass="gridTextField" Style="text-align: center"
                                                                        ToolTip="Please enter a number up to 4 decimal places and either revenue rate or royalty rate"></asp:TextBox>
                                                                    <asp:CustomValidator ID="valtxtRoyaltyRate" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator"
                                                                        ClientValidationFunction="ValRoyaltyRateGridRow" ToolTip="Please enter either revenue rate or royalty rate"
                                                                        ControlToValidate="txtRoyaltyRate" ErrorMessage="*"></asp:CustomValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" ItemStyle-Width="4%">
                                                                <ItemTemplate>
                                                                    <table width="95%" style="float: right; table-layout: fixed">
                                                                        <tr style="float: right">
                                                                            <td align="center">
                                                                                <asp:ImageButton ID="imgBtnDelete" runat="server" CommandName="deleteRow" ImageUrl="../Images/Delete.gif"
                                                                                    ToolTip="Delete" OnClientClick="return ConfirmDelete(this);" />
                                                                            </td>
                                                                            <td align="center">
                                                                                <asp:ImageButton ID="imgBtnUndo" runat="server" ImageUrl="../Images/cancel_row3.png"
                                                                                    ToolTip="Cancel" OnClientClick="return UndoGridChanges(this);" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                    <asp:HiddenField ID="hdnRateId" runat="server" Value='<%# Bind("subsid_rate_id") %>' />
                                                                    <asp:HiddenField ID="hdnOptionPeriod" runat="server" Value='<%# Bind("option_period_code") %>' />
                                                                    <%--<asp:HiddenField ID="hdnSellerGrpCode" runat="server" Value='<%# Bind("seller_group_code") %>' />--%>
                                                                    <asp:HiddenField ID="hdnSellerGrp" runat="server" Value='<%# Bind("seller_group") %>' />
                                                                    <asp:HiddenField ID="hdnSellerGrpCodeOrder" runat="server" Value='<%# Bind("seller_group_code_order") %>' />
                                                                    <%--<asp:HiddenField ID="hdnConfigGrpCode" runat="server" Value='<%# Bind("config_group_code") %>' />
                                                                    <asp:HiddenField ID="hdnPriceGrpCode" runat="server" Value='<%# Bind("price_group_code") %>' />--%>
                                                                    <asp:HiddenField ID="hdnConfigGrp" runat="server" Value='<%# Bind("config_group") %>' />
                                                                    <asp:HiddenField ID="hdnPriceGrp" runat="server" Value='<%# Bind("price_group") %>' />
                                                                    <asp:HiddenField ID="hdnRevenueRate" runat="server" Value='<%# Bind("revenue_rate","{0:0.####}") %>' />
                                                                    <asp:HiddenField ID="hdnRoyaltyRate" runat="server" Value='<%# Bind("royalty_rate","{0:0.####}") %>' />
                                                                    <asp:HiddenField ID="hdnRateType" runat="server" Value='<%# Bind("rate_type") %>' />
                                                                    <asp:HiddenField ID="hdnScore" runat="server" Value='<%# Bind("score") %>' />
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
                                            <td width="12%" class="gridHeaderStyle_1row">Option Period</td>
                                            <td width="25%" class="gridHeaderStyle_1row">Territory</td>
                                            <td width="18%" class="gridHeaderStyle_1row">Sales Type</td>
                                            <td width="25%" class="gridHeaderStyle_1row">Configuration Code</td>
                                            <td width="8%" class="gridHeaderStyle_1row">Revenue Rate</td>
                                            <td width="8%" class="gridHeaderStyle_1row">Royalty Rate</td>
                                            <td width="4%" class="gridHeaderStyle_1row">&nbsp</td>
                                        </tr>
                                        <tr>
                                            <td class="insertBoxStyle">
                                                <asp:DropDownList ID="ddlOptionPeriodAddRow" runat="server" Width="96%" CssClass="ddlStyle" TabIndex="101">
                                                </asp:DropDownList>
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
                                                <asp:TextBox ID="txtSalesTypeAddRow" runat="server" Width="98%" CssClass="textbox_FuzzySearch" TabIndex="103"
                                                    onkeydown="OntxtSalesTypeAddRowKeyDown(this);" onchange="OntxtSalesTypeAddRowChange();"></asp:TextBox>
                                                <ajaxToolkit:AutoCompleteExtender ID="aceSalesTypeAddRow" runat="server"
                                                    ServiceMethod="FuzzySearchContSubRatesSalesTypeList"
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
                                                    ControlToValidate="txtSalesTypeAddRow" ErrorMessage="*" Display="Dynamic" ValidateEmptyText="true"></asp:CustomValidator>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:TextBox ID="txtConfigAddRow" runat="server" Width="98%" CssClass="textbox_FuzzySearch" TabIndex="104"
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
                                            <td class="insertBoxStyle" align="center">
                                                <asp:TextBox ID="txtRevenueRateAddRow" runat="server" Width="70%" CssClass="textboxStyle"
                                                    ToolTip="Please enter a number up to 4 decimal places and either revenue rate or royalty rate"
                                                    Style="text-align: center" TabIndex="105"></asp:TextBox>
                                                <asp:CustomValidator ID="valtxtRevenueRateAddRow" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator"
                                                    ClientValidationFunction="ValRevenueRateAddRow" ToolTip="Please enter either revenue rate or royalty rate"
                                                    ErrorMessage="*"></asp:CustomValidator>
                                            </td>

                                            <td class="insertBoxStyle" align="center">
                                                <asp:TextBox ID="txtRoyaltyRateAddRow" runat="server" Width="75%" CssClass="textboxStyle"
                                                    ToolTip="Please enter a number up to 4 decimal places and either revenue rate or royalty rate"
                                                    Style="text-align: center" TabIndex="106"></asp:TextBox>
                                                <asp:CustomValidator ID="valtxtRoyaltyRateAddRow" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator"
                                                    ClientValidationFunction="ValRoyaltyRateAddRow" ToolTip="Please enter either revenue rate or royalty rate"
                                                    ErrorMessage="*"></asp:CustomValidator>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <table width="95%">
                                                    <tr>
                                                        <td align="center">
                                                            <%--JIRA-451 Changes -- STart--%>
                                                            <asp:ImageButton ID="btnAppendAddRow" runat="server" ImageUrl="../Images/add_row.png"
                                                                ToolTip="Add Rate" OnClientClick="if (!ValidatePopUpAddRow()) { return false;};"
                                                                OnClick="btnAppendAddRow_Click" ValidationGroup="valGrpAppendAddRow" TabIndex="107" onkeydown="OnAppendAddRowKeyDown(this);" />
                                                            <%--JIRA-451 Changes -- End--%>
                                                        </td>
                                                        <td align="center">
                                                            <asp:ImageButton ID="btnUndoAddRow" runat="server" ImageUrl="../Images/cancel_row3.png"
                                                                ToolTip="Clear Add row" CausesValidation="false" OnClientClick="return ClearAddRow();" TabIndex="108" onkeydown="OnAddRowUndoTabPress();" />
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
                <tr>
                    <td></td>
                    <td>
                        <br />
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <%--JIRA-1013 Changes by Ravi on 24/05/2019 - Start--%>
                <%-- <tr>
                    <td></td>
                    <td class="table_header_with_border" valign="top">Calculation Check:</td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="3">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td width="90%">
                                    <table width="100%" class="table_with_border">
                                        <tr>
                                            <td>
                                                <table width="99%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="25%" class="gridHeaderStyle_1row">Territory</td>
                                                        <td width="25%" class="gridHeaderStyle_1row">Catalogue No.</td>
                                                        <td width="25%" class="gridHeaderStyle_1row">Configuration Code</td>
                                                        <td width="15%" class="gridHeaderStyle_1row">Sales Type</td>
                                                    </tr>
                                                    <tr>
                                                        <td class="insertBoxStyle">
                                                            <asp:DropDownList ID="DropDownList2" runat="server" Width="98%" CssClass="ddlStyle" TabIndex="112">
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td class="insertBoxStyle">
                                                            <asp:TextBox ID="txtCatNoCalCheck" runat="server" Width="98%" TabIndex="113"></asp:TextBox>
                                                        </td>
                                                        <td class="insertBoxStyle">
                                                            <asp:DropDownList ID="ddlConfigCodeCalCheck" runat="server" Width="98%" CssClass="ddlStyle" TabIndex="114">
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td class="insertBoxStyle">
                                                            <asp:DropDownList ID="ddlSalesTypeCalCheck" runat="server" Width="98%" CssClass="ddlStyle" TabIndex="115">
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                                                                    </table>
                                            </td>
                                            <td width="30%">
                                                <table width="100%">
                                                    <tr>
                                                        <td>
                                                            <table width="100%" cellpadding="0" cellspacing="0" style="border-collapse: collapse">
                                                                <tr>
                                                                    <td class="identifierLable_large_bold" width="30%" style="border: 1px solid black">Calculation Type:</td>
                                                                    <td width="30%" style="border: 1px solid black">
                                                                        <asp:Label ID="lblCalCheckCalType" runat="server" Text="Royalty" CssClass="identifierLable"></asp:Label>
                                                                    </td>
                                                                    <td></td>
                                                                    <td></td>
                                                                </tr>
                                                            </table>

                                                        </td>

                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <table width="100%" cellpadding="0" cellspacing="0" style="border-collapse: collapse">
                                                                <tr>
                                                                    <td class="identifierLable_large_bold" width="20%" style="border: 1px solid black" align="center">% of Sales</td>
                                                                    <td width="15%" style="border: 1px solid black" align="center">
                                                                        <asp:TextBox ID="TextBox1" runat="server" Width="90%" TabIndex="115"></asp:TextBox>
                                                                    </td>
                                                                    <td class="identifierLable_large_bold" width="20%" style="border: 1px solid black" align="center">Royalty Rate</td>
                                                                    <td width="15%" style="border: 1px solid black" align="center">
                                                                        <asp:TextBox ID="TextBox2" runat="server" Width="90%" align="center" TabIndex="116"></asp:TextBox>
                                                                    </td>
                                                                    <td class="identifierLable_large_bold" width="20%" style="border: 1px solid black" align="center">Subsid Rate</td>
                                                                    <td width="10%" style="border: 1px solid black" align="center">
                                                                        <asp:TextBox ID="TextBox3" runat="server" Width="80%" TabIndex="117" onkeydown="OnTabPress();"></asp:TextBox>
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

                                <td align="right" width="10%" valign="top">
                                    <asp:Button ID="Button1" runat="server" CssClass="ButtonStyle"
                                        Text="Calculation Check" UseSubmitBehavior="false" Width="90%" TabIndex="118" onkeydown="OnTabPress();" Enabled="false" />
                                </td>
                            </tr>
                        </table>
                    </td>

                </tr>--%>
                <%--JIRA-1013 Changes by Ravi on 24/05/2019 - End--%>
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
                                        <asp:Button ID="btnUnSavedDataExit" runat="server" Text="Exit" CssClass="ButtonStyle" Width="30%" OnClick="btnUnSavedDataExit_Click" OnClientClick="OnUnSavedDataExit();" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>


            <asp:HiddenField ID="hdnRateIdAddRow" runat="server" />
            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnNewRoyaltorSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnGridDataDeleted" runat="server" Value="N" />
            <asp:HiddenField ID="hdnDefaultDisplayOrder" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" />
            <asp:HiddenField ID="hdnFuzzySearchText" runat="server" />
            <asp:HiddenField ID="hdnGridFuzzySearchRowId" runat="server" />
            <%--<asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField" onkeydown="FocusLblKeyPress();"></asp:TextBox>--%>
            <asp:Label ID="lblTab" runat="server" TabIndex="99"></asp:Label>
            <asp:Button ID="btnFuzzyTerritoryListPopup" runat="server" Style="display: none;" OnClick="btnFuzzyTerritoryListPopup_Click" CausesValidation="false" />
            <asp:Button ID="btnFuzzyConfigListPopup" runat="server" Style="display: none;" OnClick="btnFuzzyConfigListPopup_Click" CausesValidation="false" />
            <asp:Button ID="btnFuzzySalesTypeListPopup" runat="server" Style="display: none;" OnClick="btnFuzzySalesTypeListPopup_Click" CausesValidation="false" />
            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- Start--%>
            <asp:HiddenField ID="hdnDeleteRateId" runat="server" Value="N" />
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


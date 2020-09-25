<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoyContractPkgRates.aspx.cs" Inherits="WARS.Contract.RoyContractPkgRates" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Royaltor Contract Packaging Rates" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

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
        var gridClientId = "ContentPlaceHolderBody_gvPkgRates_";

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


        //grid panel height adjustment functioanlity - starts
        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
            document.getElementById("<%=PnlGrid.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
        }
        //======================= End

        //Tab key to remain only on add row fields
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
            //JIRA-908 Changes by Ravi on 12/02/2019 -- Start
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

            //JIRA-908 Changes by Ravi on 12/02/2019 -- End

        }

        //============== End

        //Undo Grid changes
        function UndoGridChanges(gridRow) {
            gridRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);

            hdnSellerGrp = document.getElementById(gridClientId + 'hdnSellerGrp' + '_' + gridRowIndex);
            txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + gridRowIndex);
            hdnCatNo = document.getElementById(gridClientId + 'hdnCatNo' + '_' + gridRowIndex);
            txtCatNo = document.getElementById(gridClientId + 'txtCatNo' + '_' + gridRowIndex);
            hdnConfigGrp = document.getElementById(gridClientId + 'hdnConfigGrp' + '_' + gridRowIndex);
            txtConfig = document.getElementById(gridClientId + 'txtConfig' + '_' + gridRowIndex);
            hdnPkgRate = document.getElementById(gridClientId + 'hdnRateValue' + '_' + gridRowIndex);
            txtPkgRate = document.getElementById(gridClientId + 'txtPkgRate' + '_' + gridRowIndex);

            txtTerritory.value = hdnSellerGrp.value;
            txtCatNo.value = hdnCatNo.value;
            txtConfig.value = hdnConfigGrp.value;
            txtPkgRate.value = hdnPkgRate.value;

            Page_ClientValidate('');//clear all validators of the page

            txtTerritory.style["width"] = '98%';
            txtConfig.style["width"] = '98%';

            return false;

        }

        //============== End


        //clear add row data
        function ClearAddRow() {
            //debugger;
            document.getElementById('<%=txtTerritoryAddRow.ClientID%>').value = "";
            document.getElementById('<%=txtCatNoAddRow.ClientID%>').value = "";
            document.getElementById('<%=txtConfigAddRow.ClientID%>').value = "";
            document.getElementById('<%=txtPkgRateAddRow.ClientID%>').value = "";
            Page_ClientValidate('');//clear all validators of the page
            return false;

        }
        //============== End  

        //Validate any unsaved data on browser window close/refresh
        //set flag value when data is changed

        function IsAddRowDataChanged() {
            var txtTerritoryAddRow = document.getElementById("<%=txtTerritoryAddRow.ClientID %>").value;
            var txtCatNoAddRow = document.getElementById("<%=txtCatNoAddRow.ClientID %>").value;
            var txtConfigAddRow = document.getElementById("<%=txtConfigAddRow.ClientID %>").value;
            var txtPkgRateAddRow = document.getElementById("<%=txtPkgRateAddRow.ClientID %>").value;
            if (txtTerritoryAddRow != '' || txtConfigAddRow != '' || txtCatNoAddRow != '' || txtPkgRateAddRow != '') {
                document.getElementById("<%=hdnAddRowDataChanged.ClientID %>").innerText = "Y";
                    return true;
                }
                else {
                    document.getElementById("<%=hdnAddRowDataChanged.ClientID %>").innerText = "N";
                    return false;
                }


            }

            function IsGridDataChanged() {
                //debugger;            
                var gvPkgRates = document.getElementById("<%= gvPkgRates.ClientID %>");
            var hdnGridDataDeleted = document.getElementById("<%=hdnGridDataDeleted.ClientID %>").value;

            if (hdnGridDataDeleted == "Y") {
                return true;
            }

            if (gvPkgRates != null) {
                var gvRows = gvPkgRates.rows; // WUIN-746 grid view rows including header row
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

                    hdnSellerGrp = document.getElementById(gridClientId + 'hdnSellerGrp' + '_' + rowIndex).value;
                    txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + rowIndex).value;
                    hdnCatNo = document.getElementById(gridClientId + 'hdnCatNo' + '_' + rowIndex).value;
                    txtCatNo = document.getElementById(gridClientId + 'txtCatNo' + '_' + rowIndex).value;
                    hdnConfigGrp = document.getElementById(gridClientId + 'hdnConfigGrp' + '_' + rowIndex).value;
                    txtConfig = document.getElementById(gridClientId + 'txtConfig' + '_' + rowIndex).value;
                    hdnPkgRate = document.getElementById(gridClientId + 'hdnRateValue' + '_' + rowIndex).value;
                    txtPkgRate = document.getElementById(gridClientId + 'txtPkgRate' + '_' + rowIndex).value;

                    if (hdnSellerGrp != txtTerritory || hdnCatNo != txtCatNo || hdnConfigGrp != txtConfig || hdnPkgRate != txtPkgRate) {
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

        //redirect to Escalation rates screen on saving data of new royaltor so that issue of data not saved validation would be handled
        function RedirectOnNewRoyaltorSave(royaltorId) {
            //debugger;
            document.getElementById("<%=hdnNewRoyaltorSaved.ClientID %>").value = "Y";
            window.location = "../Contract/RoyContractEscRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y";
        }

        function WarnOnUnSavedData() {
            //debugger;            
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
        function RedirectToAuditScreen(royaltorId, optionPeriodCode) {
            //debugger;        
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Audit/RoyaltorPackagingRatesAudit.aspx?RoyaltorId=" + royaltorId + "&optionPeriodCode=" + optionPeriodCode + "");
            }
            else {
                window.location = "../Audit/RoyaltorPackagingRatesAudit.aspx?RoyaltorId=" + royaltorId + "&optionPeriodCode=" + optionPeriodCode + "";
            }
        }

        function RedirectToPreviousScreen(royaltorId) {
            //debugger;        
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Contract/RoyContractSubRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y");
            }
            else {
                window.location = "../Contract/RoyContractSubRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y";
            }
        }
        //=================End

        //validations ========== Begin

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

        //validations ========== End


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
        //JIRA-451 CHanges -- Start
        function OnAppendAddRowKeyDown() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnAppendAddRow.ClientID%>').click();
            }
        }
        //JIRA-451 Changes -- ENd

        function ValidateUnsavedData(button) {
            if (IsDataChanged()) {
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
                document.getElementById('<%=btnUnSavedDataReturn.ClientID%>').focus();
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
                                    ROYALTOR CONTRACT - PACKAGING RATES
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
                                                <asp:Button ID="btnSave" runat="server" CssClass="ButtonStyle" OnClick="btnSave_Click"
                                                    Text="Save Changes" UseSubmitBehavior="false" Width="90%" ValidationGroup="valSave" />
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
                    <td class="table_header_with_border" valign="top">Packaging Rates</td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td valign="top">
                        <table width="100%" class="table_with_border">
                            <tr>
                                <td></td>
                            </tr>
                            <tr>
                                <td>
                                    <table width="98.5%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td width="10%" class="identifierLable_large_bold">Option Period</td>
                                            <td width="50%">
                                                <div style="position: relative; left: -3px;">
                                                    <asp:DropDownList ID="ddlOptionPeriod" runat="server" CssClass="ddlStyle" Width="30%" OnSelectedIndexChanged="ddlOptionPeriod_SelectedIndexChanged"
                                                        onfocus="if (!ValidateUnsavedData('OptionPeriod')) { return false;};" AutoPostBack="true">
                                                    </asp:DropDownList>
                                                </div>
                                            </td>
                                            <td width="40%" align="right">
                                                <asp:Button ID="btnDisplayOrder" runat="server" CssClass="ButtonStyle" OnClick="btnDisplayOrder_Click" OnClientClick="if (!ValidateUnsavedData('HierarchyOrder')) { return false;};"
                                                    Text="Hierarchy Order" UseSubmitBehavior="false" />
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
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvPkgRates" runat="server" AutoGenerateColumns="False" Width="98.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found" OnRowDataBound="gvPkgRates_RowDataBound" AllowSorting="true" OnSorting="gvPkgRates_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <Columns>
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
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Catalogue No." SortExpression="catno" ItemStyle-Width="25%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtCatNo" runat="server" Width="95%" Text='<%#Bind("catno")%>' CssClass="gridTextField"
                                                                        MaxLength="30"></asp:TextBox>
                                                                    <asp:CustomValidator ID="valtxtCatNo" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator"
                                                                        OnServerValidate="valtxtCatNo_ServerValidate" ToolTip="Not a valid catalogue number" Display="Dynamic"
                                                                        ErrorMessage="*"></asp:CustomValidator>
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
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Packaging Rate" SortExpression="rate_value" ItemStyle-Width="10%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtPkgRate" runat="server" Width="70%" Text='<%#Bind("rate_value","{0:0.###}")%>' CssClass="gridTextField" Style="text-align: center"
                                                                        ToolTip="Please enter a number up to 3 decimal places"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvtxtPkgRate" ControlToValidate="txtPkgRate" ValidationGroup="valSave"
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter packaging rate" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                    <asp:RegularExpressionValidator ID="revtxtPkgRate" runat="server" Text="*" ControlToValidate="txtPkgRate" ValidationGroup="valSave"
                                                                        ValidationExpression="^\-?\d+(\.\d{1,3})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                        ToolTip="Please enter a number up to 3 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" ItemStyle-Width="5%">
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
                                                                    <asp:HiddenField ID="hdnRateId" runat="server" Value='<%# Bind("pack_rate_id") %>' />
                                                                    <asp:HiddenField ID="hdnSellerGrp" runat="server" Value='<%# Bind("seller_group") %>' />
                                                                    <asp:HiddenField ID="hdnSellerGrpCodeOrder" runat="server" Value='<%# Bind("seller_group_code_order") %>' />
                                                                    <asp:HiddenField ID="hdnCatNo" runat="server" Value='<%# Bind("catno") %>' />
                                                                    <asp:HiddenField ID="hdnConfigGrp" runat="server" Value='<%# Bind("config_group") %>' />
                                                                    <asp:HiddenField ID="hdnRateValue" runat="server" Value='<%# Bind("rate_value","{0:0.###}") %>' />
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
                                            <td width="25%" class="gridHeaderStyle_1row">Territory</td>
                                            <td width="25%" class="gridHeaderStyle_1row">Catalogue No.</td>
                                            <td width="25%" class="gridHeaderStyle_1row">Configuration Code</td>
                                            <td width="10%" class="gridHeaderStyle_1row">Packaging Rate</td>
                                            <td width="5%" class="gridHeaderStyle_1row">&nbsp</td>
                                        </tr>
                                        <tr>
                                            <td class="insertBoxStyle">
                                                <asp:TextBox ID="txtTerritoryAddRow" runat="server" Width="98%" CssClass="textbox_FuzzySearch" TabIndex="101"
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
                                                <asp:TextBox ID="txtCatNoAddRow" runat="server" Width="95%" CssClass="textboxStyle" MaxLength="30" TabIndex="102"></asp:TextBox>
                                                <asp:CustomValidator ID="valtxtCatNoAddRow" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator"
                                                    OnServerValidate="valtxtCatNoAddRow_ServerValidate" ToolTip="Not a valid catalogue number" Display="Dynamic"
                                                    ErrorMessage="*"></asp:CustomValidator>
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
                                            <td class="insertBoxStyle" align="center">
                                                <asp:TextBox ID="txtPkgRateAddRow" runat="server" Width="70%" CssClass="textboxStyle"
                                                    ToolTip="Please enter a number up to 3 decimal places"
                                                    Style="text-align: center" TabIndex="104"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvtxtPkgRateAddRow" ControlToValidate="txtPkgRateAddRow" ValidationGroup="valGrpAppendAddRow"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter packaging rate" Display="Dynamic"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="revtxtPkgRateAddRow" runat="server" Text="*" ControlToValidate="txtPkgRateAddRow" ValidationGroup="valGrpAppendAddRow"
                                                    ValidationExpression="^\-?\d+(\.\d{1,3})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                    ToolTip="Please enter a number up to 3 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <table width="95%">
                                                    <tr>
                                                        <td align="center">
                                                            <%--JIRA-451 Changes -- Start--%>
                                                            <asp:ImageButton ID="btnAppendAddRow" runat="server" ImageUrl="../Images/add_row.png"
                                                                ToolTip="Add Rate" OnClientClick="if (!ValidatePopUpAddRow()) { return false;};"
                                                                OnClick="btnAppendAddRow_Click" ValidationGroup="valGrpAppendAddRow" TabIndex="105" onkeydown="OnAppendAddRowKeyDown(this);" />
                                                            <%--JIRA-451 Changes -- End--%>
                                                        </td>
                                                        <td align="center">
                                                            <asp:ImageButton ID="btnUndoAddRow" runat="server" ImageUrl="../Images/cancel_row3.png"
                                                                ToolTip="Clear Add row" CausesValidation="false" OnClientClick="return ClearAddRow();" TabIndex="106" onkeydown="OnAddRowUndoTabPress();" />
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdnRateIdAddRow" runat="server" />
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


            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnNewRoyaltorSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnGridDataChanged" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridDataDeleted" runat="server" Value="N" />
            <asp:HiddenField ID="hdnAddRowDataChanged" runat="server" Value="N" />
            <asp:HiddenField ID="hdnDefaultDisplayOrder" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnOptPeriodSelected" runat="server" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" />
            <asp:HiddenField ID="hdnFuzzySearchText" runat="server" />
            <asp:HiddenField ID="hdnGridFuzzySearchRowId" runat="server" />
            <asp:Button ID="btnFuzzyTerritoryListPopup" runat="server" Style="display: none;" OnClick="btnFuzzyTerritoryListPopup_Click" CausesValidation="false" />
            <asp:Button ID="btnFuzzyConfigListPopup" runat="server" Style="display: none;" OnClick="btnFuzzyConfigListPopup_Click" CausesValidation="false" />
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


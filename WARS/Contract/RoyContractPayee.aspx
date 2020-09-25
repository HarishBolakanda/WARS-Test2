<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoyContractPayee.aspx.cs" Inherits="WARS.Contract.RoyContractPayee" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Royaltor Contract Payee" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

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

        //Pop up interested party list       
        function OntxtIntPartyAddRowKeyDown() {
            var txtIntPartyAddRow = document.getElementById("<%= txtIntPartyAddRow.ClientID %>");
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnIntPartyPopup.ClientID%>').click();
            }

        }
        //============== End

        //Confim delete
        function ConfirmDelete(row) {
            //JIRA-908 Changes by Ravi on 13/02/2019 -- Start
            //set if this is not a newly added row

            var selectedRowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            var str = "ContentPlaceHolderBody_gvContPayee_";
            var hdnIntPartyId = document.getElementById(str + 'hdnIntPartyId' + '_' + selectedRowIndex).value;
            var hdnIntPartyType = document.getElementById(str + 'hdnIntPartyType' + '_' + selectedRowIndex).value;
            var hdnIsModified = document.getElementById(str + 'hdnIsModified' + '_' + selectedRowIndex).value;
            if (hdnIsModified != "-") {
                document.getElementById("<%=hdnGridDataDeleted.ClientID %>").innerText = "Y";
            }
            document.getElementById("<%=hdnDeleteIntPartyId.ClientID %>").innerText = hdnIntPartyId;
            document.getElementById("<%=hdnDeleteIntPartyType.ClientID %>").innerText = hdnIntPartyType;
            document.getElementById("<%=hdnDeleteIsModified.ClientID %>").innerText = hdnIsModified;
            var popup = $find('<%= mpeConfirmDelete.ClientID %>');
            if (popup != null) {
                popup.show();
            }
            return false;

            //JIRA-908 Changes by Ravi on 13/02/2019 -- End
        }
        //============== End

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                if (ddlPayeeType != null) {
                    document.getElementById("<%= ddlPayeeTypeAdd.ClientID %>").focus();
                }
            }
        }

        function FocusLblKeyPress() {
            document.getElementById("<%= txtRoyaltorId.ClientID %>").focus();
        }

        //=============== End

        //Validation: warning message if changes made and not saved or on page change                                
        //set flag value when data is changed

        function IsAddRowDataChanged() {

            var ddlPayeeTypeAdd = document.getElementById("<%=ddlPayeeTypeAdd.ClientID %>");
            if (ddlPayeeTypeAdd != null) {
                ddlPayeeTypeAdd = document.getElementById("<%=ddlPayeeTypeAdd.ClientID %>").value;
            }

            var txtIntPartyAddRow = document.getElementById("<%=txtIntPartyAddRow.ClientID %>").value;
            var txtTaxNumAddRow = document.getElementById("<%=txtTaxNumAddRow.ClientID %>").value;
            var ddlTaxTypeAddRow = document.getElementById("<%=ddlTaxTypeAddRow.ClientID %>").value;
            var txtShareAddRow = document.getElementById("<%=txtShareAddRow.ClientID %>").value;
            var cbPayAddRow = document.getElementById("<%=cbPayAddRow.ClientID %>");

            if (ddlPayeeTypeAdd == null || ddlPayeeTypeAdd != '-' || txtTaxNumAddRow != '' || txtIntPartyAddRow != '' || txtShareAddRow != '' || cbPayAddRow.checked || ddlTaxTypeAddRow != '-') {
                return true;
            }
            else {
                return false;
            }
        }

        function IsGridDataChanged() {

            if (document.getElementById("<%=hdnGridDataDeleted.ClientID %>").value == "Y") {
                return true;
            }

            var gridDataChanged = "N";
            var hdnIntPartyType;
            var hdnPayeePct;
            var txtPercentShare;
            var hdnPay;
            var txtTaxNumber;
            var hdnTaxNumber;
            var ddlTaxtype;
            var hdnTaxType;
            var hdnIsModified;
            var hdnPrimaryPayee;
            var cbPay;
            var cbPrimaryPayee;
            var isPayeePay;
            var cbGenerateInvoice;
            var isGenerateInvoice;
            var hdnGenerateInvoice;

            var str = "ContentPlaceHolderBody_gvContPayee_";
            var gvRoyaltyRates = document.getElementById("<%= gvContPayee.ClientID %>");
            if (gvRoyaltyRates != null) {
                var gvRows = gvRoyaltyRates.rows;  // WUIN-746 grid view rows including header row
                var rowIndex;
                for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                    rowIndex = i - 1; //WUIN-746 row index start from 0
                    //handling empty data row
                    if (gvRows.length == 2 && document.getElementById(str + 'hdnIsModified' + '_' + rowIndex) == null) {
                        break;
                    }

                    hdnIsModified = document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).value;
                    if (hdnIsModified == "-") {
                        gridDataChanged = "Y";
                        break;
                    }

                    hdnIntPartyType = document.getElementById(str + 'hdnIntPartyType' + '_' + rowIndex).value;
                    if (hdnIntPartyType == 'P') {
                        hdnPayeePct = document.getElementById(str + 'hdnPayeePct' + '_' + rowIndex).value;
                        txtPercentShare = document.getElementById(str + 'txtPercentShare' + '_' + rowIndex).value;
                        txtTaxNumber = document.getElementById(str + 'txtTaxNumber' + '_' + rowIndex).value;
                        hdnTaxNumber = document.getElementById(str + 'hdnTaxNumber' + '_' + rowIndex).value;
                        ddlTaxType = document.getElementById(str + 'ddlTaxType' + '_' + rowIndex).value;
                        ddlTaxType = ddlTaxType == "-" ? "" : ddlTaxType;
                        hdnTaxType = document.getElementById(str + 'hdnTaxType' + '_' + rowIndex).value;
                        hdnPay = document.getElementById(str + 'hdnPay' + '_' + rowIndex).value;
                        cbPay = document.getElementById(str + 'cbPay' + '_' + rowIndex);
                        hdnPrimaryPayee = document.getElementById(str + 'hdnPrimaryPayee' + '_' + rowIndex).value;
                        cbPrimaryPayee = document.getElementById(str + 'cbPrimaryPayee' + '_' + rowIndex);
                        isPayeePay;

                        if (cbPay.checked) {
                            isPayeePay = 'Y';
                        }
                        else {
                            isPayeePay = 'N';
                        }

                        hdnGenerateInvoice = document.getElementById(str + 'hdnGenerateInvoice' + '_' + rowIndex).value;
                        cbGenerateInvoice = document.getElementById(str + 'cbGenerateInvoice' + '_' + rowIndex);
                        isGenerateInvoice;

                        if (cbGenerateInvoice.checked) {
                            isGenerateInvoice = 'Y';
                        }
                        else {
                            isGenerateInvoice = 'N';
                        }

                        if (hdnPayeePct != txtPercentShare || hdnPay != isPayeePay || hdnTaxNumber != txtTaxNumber || hdnTaxType != ddlTaxType || hdnGenerateInvoice != isGenerateInvoice
                            || (hdnPrimaryPayee == "Y" && !cbPrimaryPayee.checked) || (hdnPrimaryPayee == "N" && cbPrimaryPayee.checked)) {
                            gridDataChanged = "Y";
                            break;
                        }
                    }
                }


            }

            if (gridDataChanged == "Y") {
                return true;
            }
            else {
                return false;
            }

        }

        //Validate any unsaved data on browser window close/refresh
        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        //redirect to payee supplier details screen on saving data of new royaltor so that issue of data not saved validation would be handled
        function RedirectOnNewRoyaltorSave(royaltorId) {

            document.getElementById("<%=hdnNewRoyaltorSaved.ClientID %>").value = "Y";
            window.location = "../Contract/RoyContractPayeeSupp.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y";
        }

        function WarnOnUnSavedData() {
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            var isNewRoyaltorSaved = document.getElementById("<%=hdnNewRoyaltorSaved.ClientID %>").value;
            var isContractScreen = document.getElementById("hdnIsContractScreen").value;
            var isNotContractScreen = document.getElementById("hdnIsNotContractScreen").value;
            if (isExceptionRaised != "Y" && isNewRoyaltorSaved != "Y" && isContractScreen != "Y" && isNotContractScreen != "Y") {
                if (IsGridDataChanged() || IsAddRowDataChanged()) {
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
            if (IsGridDataChanged() || IsAddRowDataChanged()) {
                return true;
            }
            else {
                return false;
            }
        }

        //=================End

        //Audit button navigation
        function RedirectToAuditScreen(royaltorId) {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Audit/RoyaltorPayeeAudit.aspx?RoyaltorId=" + royaltorId + "");
            }
            else {
                window.location = "../Audit/RoyaltorPayeeAudit.aspx?RoyaltorId=" + royaltorId + "";
            }
        }

        function RedirectToPreviousScreen(royaltorId) {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Contract/RoyaltorContract.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y");
            }
            else {
                window.location = "../Contract/RoyaltorContract.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y";
            }
        }
        //=================End

        //Validations
        var txtShareVal;
        var ddlPayeeType;
        var payeeType;
        var cbPrimaryPayee;
        var cbPay;
        var valtxtPercentShare;
        var strGridId = "ContentPlaceHolderBody_gvContPayee_";
        //var pctShareRegex = /^100$|^\d{0,2}(\.\d{1,2})? *%?$/; //only positive number <= 100 upto 2 decimal places
        var pctShareRegex = /^100(\.000?)?|^\d{0,2}(\.\d{1,3})? *%?$/; //only positive number <= 100 upto 3 decimal places

        function GetGridRowValues(rowIndex) {
            payeeType = null;
            txtShareVal = null;
            cbPrimaryPayee = null;
            cbPay = null;
            valtxtPercentShare = null;

            if (document.getElementById(strGridId + 'hdnIntPartyType' + '_' + rowIndex) != null) {
                payeeType = document.getElementById(strGridId + 'hdnIntPartyType' + '_' + rowIndex).value;
            }

            if (payeeType == "P") {
                txtShareVal = document.getElementById(strGridId + 'txtPercentShare' + '_' + rowIndex).value;
                cbPrimaryPayee = document.getElementById(strGridId + 'cbPrimaryPayee' + '_' + rowIndex);
                cbPay = document.getElementById(strGridId + 'cbPay' + '_' + rowIndex);
                valtxtPercentShare = document.getElementById(strGridId + 'valtxtPercentShare' + '_' + rowIndex);
            }


        }


        function ValidatePopUpAddRow() {
            //warning on add row validation fail    
            if (!Page_ClientValidate("valGrpAppendAddRow")) {
                DisplayMessagePopup("Payee details not moved – invalid or missing data!");
                Page_BlockSubmit = false;
                return false;
            }
            else {
                return true;
            }
        }

        function ValidatePopUpSave() {
            //warning on save validation fail
            if (!Page_ClientValidate("valSave")) {
                DisplayMessagePopup("Payee details not saved – invalid or missing data!");
                Page_BlockSubmit = false;
                return false;
            }
            else {
                return ValidateSave();
            }

        }


        function ValidateShareAddRow(sender, args) {
            txtShareVal = document.getElementById("<%=txtShareAddRow.ClientID %>").value;
            ddlPayeeType = document.getElementById("<%=ddlPayeeTypeAdd.ClientID %>");
            var valtxtShareAddRow = document.getElementById("<%=valtxtShareAddRow.ClientID %>");
            var txtPayeeTypeAdd = document.getElementById("<%=txtPayeeTypeAdd.ClientID %>");
            if (ddlPayeeType == null && txtPayeeTypeAdd == null) {
                return;
            }

            //share % is mandatory if type is payee
            if ((txtPayeeTypeAdd != null && txtPayeeTypeAdd.value == "Payee") || (ddlPayeeType != null && ddlPayeeType.options[ddlPayeeType.selectedIndex].value == "P")) {
                if (txtShareVal == "") {
                    args.IsValid = false;
                    valtxtShareAddRow.title = "Please enter share %";
                }
                else {
                    if (pctShareRegex.test(txtShareVal)) {
                        args.IsValid = true;
                    }
                    else {
                        args.IsValid = false;
                        valtxtShareAddRow.title = "Please enter only positive number <= 100 upto 3 decimal places";
                    }
                }
            }
            else {
                args.IsValid = true;
            }

        }

        function ValPctShareGridRow(sender, args) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            GetGridRowValues(gridRowIndex);
            if (txtShareVal == "") {
                args.IsValid = false;
                valtxtPercentShare.title = "Please enter share %";
            }
            else {
                if (pctShareRegex.test(txtShareVal)) {
                    args.IsValid = true;
                }

                else {
                    args.IsValid = false;
                    valtxtPercentShare.title = "Please enter only positive number <= 100 upto 3 decimal places";
                }
            }

        }

        function ValidateSave() {
            //Validate Payees have total of Share %  = 100
            //Must be at least one Payee
            //Only one payee should be set as Primary

            var primaryPayeeCount = 0;
            var payeeCount = 0;
            var totalShare = 0;
            var gvRoyaltyRates = document.getElementById("<%= gvContPayee.ClientID %>");
            if (gvRoyaltyRates != null) {
                var gvRows = gvRoyaltyRates.rows;
                for (var i = 0; i < gvRows.length; i++) {
                    GetGridRowValues(i);
                    if (payeeType == "P") {
                        payeeCount++;
                        totalShare = +totalShare + +txtShareVal;//+ added to total share to convert to number from string
                        totalShare = totalShare.toFixed(3);  // Changes as per JIRA-1139 -- Used to Round Off the Extra digits to 3 places after decimal point 
                        if (cbPrimaryPayee.checked) {
                            primaryPayeeCount++;
                        }
                    }
                }

            }

            if (payeeCount == 0) {
                DisplayMessagePopup("Must be at least one payee!");
                return false;
            }
            else if (totalShare != "100.000") { // Changes as per JIRA-1139 
                DisplayMessagePopup("Payees must have total of Share %  = 100");
                return false;
            }
            else if (primaryPayeeCount == 0) {
                DisplayMessagePopup("One Payee must be set as Primary for the Royaltor");
                return false;

            }
            else if (primaryPayeeCount > 1) {
                DisplayMessagePopup("Only one payee should be set as Primary");
                return false;
            }
            else {
                return true;
            }

        }


        //=================End

        //WUIN-1164 - Auto Populate Generate Invoice on adding Tax number

        function checkGenerateInvoiceAddRow() {
            var txtTaxNumAddRow = document.getElementById("<%=txtTaxNumAddRow.ClientID %>").value;
            var applicableTax = document.getElementById("<%=ddlTaxTypeAddRow.ClientID %>").value;
            var cbGenerateInvoiceAddRow = document.getElementById("<%=cbGenerateInvoiceAddRow.ClientID %>");
            if (txtTaxNumAddRow != "" && applicableTax != "-") {
                cbGenerateInvoiceAddRow.checked = true;
            }
            else {
                cbGenerateInvoiceAddRow.checked = false;
            }

        }

        function checkGenerateInvoiceNewIP() {
            var txtAddIntPartyTaxNum = document.getElementById("<%=txtAddIntPartyTaxNum.ClientID %>").value;
            var cbAddIPGenerateInvoice = document.getElementById("<%=cbAddIPGenerateInvoice.ClientID %>");
            var applicableTax = document.getElementById("<%=ddlAddIntPartyTaxType.ClientID %>").value;
            if (txtAddIntPartyTaxNum != "" && applicableTax != "-") {
                cbAddIPGenerateInvoice.checked = true;
            }
            else {
                cbAddIPGenerateInvoice.checked = false;
            }
        }

        function checkGenerateInvoice(row) {
            var rowIndex = row.id.substring(row.id.lastIndexOf('_') + 1);
            var tempstr = row.id.substring(0, row.id.lastIndexOf('_'));
            var str = tempstr.substring(0, tempstr.lastIndexOf('_') + 1);
            var txtTaxNumber = document.getElementById(str + 'txtTaxNumber_' + rowIndex).value;
            var applicableTax = document.getElementById(str + 'ddlTaxType_' + rowIndex).value;
            var cbGenerateInvoice = document.getElementById(str + 'cbGenerateInvoice_' + rowIndex);
            if (txtTaxNumber != "" && applicableTax != "-") {
                cbGenerateInvoice.checked = true;
            }
            else {
                cbGenerateInvoice.checked = false;
            }
        }


        function validateGenerateInvoiceNewIP() {
            var txtTaxNumber = document.getElementById("<%=txtAddIntPartyTaxNum.ClientID %>").value;
            var applicableTax = document.getElementById("<%=ddlAddIntPartyTaxType.ClientID %>").value;
            var cbGenerateInvoiceInsert = document.getElementById("<%=cbAddIPGenerateInvoice.ClientID %>");
            if (txtTaxNumber == "" || applicableTax == "-") {
                cbGenerateInvoiceInsert.checked = false;
                DisplayMessagePopup("Tax number and applicabe tax are mandatory for generating invoice");
            }
        }
        //=================End

        //WUIN-1181 changes
        function OnAppendAddRowKeyDown() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnAppendAddRow.ClientID%>').click();
            }
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
                                    ROYALTOR CONTRACT - PAYEE DETAILS
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="4"></td>
                </tr>
                <tr>
                    <td width="0.5%"></td>
                    <td>
                        <table width="99%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="10%" class="identifierLable_large_bold">Current Royaltor</td>
                                <td>
                                    <asp:TextBox ID="txtRoyaltorId" runat="server" Width="25%" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" TabIndex="100"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <%--<td width="1%"></td>--%>
                    <td width="10%" rowspan="4" valign="top" align="right">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td align="right" width="70%">
                                                <asp:Button ID="btnSave" runat="server" CssClass="ButtonStyle" OnClientClick="if (!ValidatePopUpSave()) { return false;};" OnClick="btnSave_Click"
                                                    Text="Save Changes" UseSubmitBehavior="false" Width="90%" ValidationGroup="valSave" TabIndex="110" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <asp:Button ID="btnAudit" runat="server" CssClass="ButtonStyle" OnClick="btnAudit_Click"
                                                    Text="Audit" UseSubmitBehavior="false" Width="90%" TabIndex="111" onkeydown="OnTabPress();" />
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
                    <td class="table_header_with_border">Payee / Courtesy Details</td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <table width="100%" class="table_with_border">
                            <tr>
                                <td>
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvContPayee" runat="server" AutoGenerateColumns="False" Width="98.75%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found" OnRowDataBound="gvContPayee_RowDataBound" OnRowCommand="gvContPayee_RowCommand" AllowSorting="true" 
                                                        OnSorting="gvContPayee_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="Payee / Courtesy" SortExpression="payee_type"
                                                                ItemStyle-Width="6%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblPayeeType" runat="server" Width="75%" Text='<%# Bind("payee_type") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="IP Number" SortExpression="ip_number"
                                                                ItemStyle-Width="6%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblIPNumber" runat="server" Width="95%" Text='<%# Bind("ip_number") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="Name" SortExpression="int_party_name"
                                                                ItemStyle-Width="12%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblName" runat="server" Width="95%" Text='<%# Bind("int_party_name") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="Address 1" SortExpression="int_party_add1"
                                                                ItemStyle-Width="8%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblAddress1" runat="server" Width="95%" Text='<%# Bind("int_party_add1") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="Address 2" SortExpression="int_party_add2"
                                                                ItemStyle-Width="8%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblAddress2" runat="server" Width="95%" Text='<%# Bind("int_party_add2") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="Address 3" SortExpression="int_party_add3"
                                                                ItemStyle-Width="8%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblAddress3" runat="server" Width="95%" Text='<%# Bind("int_party_add3") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="Address 4" SortExpression="int_party_add4"
                                                                ItemStyle-Width="8%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblAddress4" runat="server" Width="95%" Text='<%# Bind("int_party_add4") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="Postcode" SortExpression="int_party_postcode"
                                                                ItemStyle-Width="5%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblPostCode" runat="server" Width="95%" Text='<%# Bind("int_party_postcode") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="Email" SortExpression="email"
                                                                ItemStyle-Width="9%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblEmail" runat="server" Width="98%" Text='<%# Bind("email") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="Tax Number" SortExpression="vat_number"
                                                                ItemStyle-Width="5%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtTaxNumber" runat="server" Width="95%" Text='<%#Bind("vat_number")%>' CssClass="gridTextField" MaxLength="12" onchange="checkGenerateInvoice(this);"></asp:TextBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="Applicable Tax" SortExpression="applicable_tax"
                                                                ItemStyle-Width="6%">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlTaxType" runat="server" Width="86%" CssClass="ddlStyle" onchange="checkGenerateInvoice(this);">
                                                                    </asp:DropDownList>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="Share %" SortExpression="payee_percentage"
                                                                ItemStyle-Width="4%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtPercentShare" runat="server" Width="55%" Text='<%#Bind("payee_percentage","{0:0.###}")%>' CssClass="gridTextField" Style="text-align: center" MaxLength="7"></asp:TextBox>
                                                                    <asp:CustomValidator ID="valtxtPercentShare" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator"
                                                                        ClientValidationFunction="ValPctShareGridRow" ToolTip="Please enter share %" Display="Dynamic"
                                                                        ErrorMessage="*"></asp:CustomValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="Primary" SortExpression="primary_payee"
                                                                ItemStyle-Width="4%">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="cbPrimaryPayee" runat="server" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="Pay" SortExpression="payee_pay"
                                                                ItemStyle-Width="3%">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="cbPay" runat="server" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="Generate Invoice" SortExpression="generate_invoice"
                                                                ItemStyle-Width="5%">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="cbGenerateInvoice" runat="server" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"
                                                                ItemStyle-Width="3%">
                                                                <ItemTemplate>
                                                                    <table width="100%" style="float: right; table-layout: fixed">
                                                                        <tr style="float: right">

                                                                            <td align="right" style="float: right">
                                                                                <asp:ImageButton ID="imgBtnDelete" runat="server" CommandName="deleteRow" ImageUrl="../Images/Delete.gif"
                                                                                    ToolTip="Delete" OnClientClick="return ConfirmDelete(this);" />
                                                                            </td>
                                                                            <td align="right" style="float: right">
                                                                                <asp:ImageButton ID="imgBtnUndo" runat="server" CommandName="cancelRow" ImageUrl="../Images/cancel_row3.png"
                                                                                    ToolTip="Cancel" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                    <asp:HiddenField ID="hdnDisplayOrder" runat="server" Value='<%# Bind("display_order") %>' />
                                                                    <asp:HiddenField ID="hdnIntPartyId" runat="server" Value='<%# Bind("int_party_id") %>' />
                                                                    <asp:HiddenField ID="hdnIntPartyType" runat="server" Value='<%# Bind("int_party_type") %>' />
                                                                    <asp:HiddenField ID="hdnPayeePct" runat="server" Value='<%# Bind("payee_percentage","{0:0.###}") %>' />
                                                                    <asp:HiddenField ID="hdnPrimaryPayee" runat="server" Value='<%# Bind("primary_payee") %>' />
                                                                    <asp:HiddenField ID="hdnPay" runat="server" Value='<%# Bind("payee_pay") %>' />
                                                                    <asp:HiddenField ID="hdnTaxNumber" runat="server" Value='<%# Bind("vat_number") %>' />
                                                                    <asp:HiddenField ID="hdnTaxType" runat="server" Value='<%# Bind("applicable_tax") %>' />
                                                                    <asp:HiddenField ID="hdnGenerateInvoice" runat="server" Value='<%# Bind("generate_invoice") %>' />
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
                                    <table width="99%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td width="6%" class="gridHeaderStyle_1row">Payee / Courtesy</td>
                                            <td width="14%" class="gridHeaderStyle_1row">Interested Party</td>
                                            <td width="9%" class="gridHeaderStyle_1row">Address 1</td>
                                            <td width="9%" class="gridHeaderStyle_1row">Address 2</td>
                                            <td width="9%" class="gridHeaderStyle_1row">Address 3</td>
                                            <td width="9%" class="gridHeaderStyle_1row">Address 4</td>
                                            <td width="5%" class="gridHeaderStyle_1row">Postcode</td>
                                            <td width="9%" class="gridHeaderStyle_1row">Email</td>
                                            <td width="5%" class="gridHeaderStyle_1row">Tax Number</td>
                                            <td width="6%" class="gridHeaderStyle_1row">Applicable Tax</td>
                                            <td width="4%" class="gridHeaderStyle_1row">Share %</td>
                                            <td width="4%" class="gridHeaderStyle_1row">Primary</td>
                                            <td width="3%" class="gridHeaderStyle_1row">Pay</td>
                                            <td width="5%" class="gridHeaderStyle_1row">Generate Invoice</td>
                                            <td width="3%" class="gridHeaderStyle_1row">&nbsp</td>
                                        </tr>
                                        <tr>
                                            <td class="insertBoxStyle">
                                                <asp:DropDownList ID="ddlPayeeTypeAdd" runat="server" Width="85%" CssClass="ddlStyle" TabIndex="101"></asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvddlPayeeTypeAdd" ControlToValidate="ddlPayeeTypeAdd" ValidationGroup="valGrpAppendAddRow"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please select payee type" InitialValue="-" Display="Dynamic"></asp:RequiredFieldValidator>
                                                <asp:TextBox ID="txtPayeeTypeAdd" runat="server" Width="85%" CssClass="textboxStyle_readonly" ReadOnly="true" TabIndex="101" Visible="false"></asp:TextBox>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:TextBox ID="txtIntPartyAddRow" runat="server" Width="90%" CssClass="textboxStyle"
                                                    onkeydown="javascript: OntxtIntPartyAddRowKeyDown();" TabIndex="102"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvtxtIntPartyAddRow" ControlToValidate="txtIntPartyAddRow" ValidationGroup="valGrpAppendAddRow"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Interested Party" Display="Dynamic"></asp:RequiredFieldValidator>
                                            </td>
                                            <td class="insertBoxStyle">&nbsp
                                                <asp:Label ID="lblAddress1AddRow" runat="server" CssClass="identifierLable" Width="93%"></asp:Label>
                                            </td>

                                            <td class="insertBoxStyle">&nbsp
                                                <asp:Label ID="lblAddress2AddRow" runat="server" Width="93%" CssClass="identifierLable"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle">&nbsp
                                                <asp:Label ID="lblAddress3AddRow" runat="server" Width="93%" CssClass="identifierLable"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle">&nbsp
                                                <asp:Label ID="lblAddress4AddRow" runat="server" Width="93%" CssClass="identifierLable"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:Label ID="lblPostCodeAddRow" runat="server" Width="90%" CssClass="identifierLable"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle">&nbsp
                                                <asp:Label ID="lblEmailAddRow" runat="server" Width="93%" CssClass="identifierLable"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:TextBox ID="txtTaxNumAddRow" runat="server" Width="90%" CssClass="textboxStyle" MaxLength="12" TabIndex="103" onchange="checkGenerateInvoiceAddRow();"></asp:TextBox>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:DropDownList ID="ddlTaxTypeAddRow" runat="server" Width="90%" CssClass="ddlStyle" TabIndex="104" onchange="checkGenerateInvoiceAddRow();">
                                                </asp:DropDownList>
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:TextBox ID="txtShareAddRow" runat="server" Width="65%" CssClass="textboxStyle" Style="text-align: center" TabIndex="105" MaxLength="7"></asp:TextBox>
                                                <asp:CustomValidator ID="valtxtShareAddRow" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator"
                                                    ClientValidationFunction="ValidateShareAddRow" ToolTip="Please enter share %" Display="Dynamic"
                                                    ErrorMessage="*"></asp:CustomValidator>
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:CheckBox ID="cbPrimaryAddRow" runat="server" TabIndex="106" />
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:CheckBox ID="cbPayAddRow" runat="server" TabIndex="107" />
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:CheckBox ID="cbGenerateInvoiceAddRow" runat="server" TabIndex="108" />
                                            </td>
                                            <td class="insertBoxStyle">
                                                <table width="95%">
                                                    <tr style="float: right">
                                                        <td align="right" style="float: right">
                                                            <%--JIRA-974 Changes by Ravi on 05/02/2019 -- Start--%>
                                                            <asp:ImageButton ID="btnAppendAddRow" runat="server" ImageUrl="../Images/add_row.png" ToolTip="Add row"
                                                                OnClientClick="if (!ValidatePopUpAddRow()) { return false;};" OnClick="btnAppendAddRow_Click" onkeydown="OnAppendAddRowKeyDown();" 
                                                                ValidationGroup="valGrpAppendAddRow" TabIndex="109" Height="16px" Width="16px" />
                                                        </td>
                                                        <td align="right" style="float: right">
                                                            <asp:ImageButton ID="btnUndoAddRow" runat="server" ImageUrl="../Images/cancel_row3.png"
                                                                ToolTip="Clear Add row" TabIndex="110" OnClick="btnUndoAddRow_Click" />
                                                        </td>
                                                        <td align="right" style="float: right">
                                                            <asp:Button ID="btnAddIntParty" runat="server" CssClass="ButtonStyle"
                                                                ToolTip="Add Interested Party" OnClick="btnAddIntParty_Click" Text="New" TabIndex="111" />
                                                            <%--JIRA-974 Changes by Ravi on 05/02/2019 -- End--%>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <asp:HiddenField ID="hdnIntPartyIdAddRow" runat="server" />
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
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

            <%--Add Interested party--%>
            <asp:Button ID="dummyAddIntPartyPopup" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeAddIntPartyPopup" runat="server" PopupControlID="pnlAddIntPartyPopup" TargetControlID="dummyAddIntPartyPopup"
                CancelControlID="btnCloseAddIntPartyPopup" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlAddIntPartyPopup" runat="server" align="left" Width="30%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="identifierLable" align="center">Enter New Interested Party details
                                    </td>
                                    <td align="right" style="vertical-align: top;">
                                        <asp:ImageButton ID="btnCloseAddIntPartyPopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td width="3%"></td>
                                    <td width="25%" class="identifierLable_large_bold">Type</td>
                                    <td>
                                        <asp:DropDownList ID="ddlAddIntPartyType" runat="server" Width="25%" CssClass="ddlStyle"></asp:DropDownList>
                                        <asp:RequiredFieldValidator runat="server" ID="valddlAddIntPartyType" ControlToValidate="ddlAddIntPartyType" ValidationGroup="valGrpAddIntParty"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please select type" InitialValue="-" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold">Name</td>
                                    <td>
                                        <asp:TextBox ID="txtAddIntPartyName" runat="server" Width="90%" CssClass="textboxStyle" MaxLength="60"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="valtxtAddIntPartyName" ControlToValidate="txtAddIntPartyName" ValidationGroup="valGrpAddIntParty"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter name" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold">Address 1</td>
                                    <td>
                                        <asp:TextBox ID="txtAddIntPartyAddress1" runat="server" Width="90%" CssClass="textboxStyle" MaxLength="50"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold">Address 2</td>
                                    <td>
                                        <asp:TextBox ID="txtAddIntPartyAddress2" runat="server" Width="90%" CssClass="textboxStyle" MaxLength="50"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold">Address 3</td>
                                    <td>
                                        <asp:TextBox ID="txtAddIntPartyAddress3" runat="server" Width="90%" CssClass="textboxStyle" MaxLength="50"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold">Address 4</td>
                                    <td>
                                        <asp:TextBox ID="txtAddIntPartyAddress4" runat="server" Width="90%" CssClass="textboxStyle" MaxLength="50"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold">Postcode</td>
                                    <td>
                                        <asp:TextBox ID="txtAddIntPartyPostCode" runat="server" Width="90%" CssClass="textboxStyle" MaxLength="20"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold">Email</td>
                                    <td>
                                        <asp:TextBox ID="txtAddIntPartyEmail" runat="server" Width="90%" CssClass="textboxStyle" MaxLength="254"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold">Tax Number</td>
                                    <td>
                                        <asp:TextBox ID="txtAddIntPartyTaxNum" runat="server" Width="90%" CssClass="textboxStyle" MaxLength="12" onchange="checkGenerateInvoiceNewIP();"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold">Applicable Tax</td>
                                    <td>
                                        <asp:DropDownList ID="ddlAddIntPartyTaxType" runat="server" Width="91%" CssClass="ddlStyle" onchange="checkGenerateInvoiceNewIP();">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold">Generate Invoice</td>
                                    <td>
                                        <div style="position: relative; left: -3px;">
                                            <asp:CheckBox ID="cbAddIPGenerateInvoice" runat="server" onclick="validateGenerateInvoiceNewIP();" />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3" align="center">
                                        <table width="30%">
                                            <tr>
                                                <td width="50%">
                                                    <asp:Button ID="btnAddIntPartySave" runat="server" CssClass="ButtonStyle" OnClick="btnAddIntPartySave_Click"
                                                        Text="Save" UseSubmitBehavior="false" Width="90%" ValidationGroup="valGrpAddIntParty" />
                                                </td>
                                                <td>
                                                    <asp:Button ID="btnAddIntPartyCancel" runat="server" CssClass="ButtonStyle" OnClick="btnAddIntPartyCancel_Click"
                                                        Text="Cancel" UseSubmitBehavior="false" Width="90%" />
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

            <%--Add Interested party-- Ends%>


            <%--Interested party search list popup--%>
            <asp:Button ID="dummyIntPartySearch" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeIntPartySearch" runat="server" PopupControlID="pnlIntPartyPopup" TargetControlID="dummyIntPartySearch"
                CancelControlID="btnClosePopupSaveMsg" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlIntPartyPopup" runat="server" align="left" Width="60%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="identifierLable" align="center">Interested party search List
                                    </td>
                                    <td align="right" style="vertical-align: top;">
                                        <asp:ImageButton ID="btnClosePopupSaveMsg" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <table width="97.75%" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td width="15%" class="gridHeaderStyle_1row">Interested Party</td>
                                                <td width="10%" class="gridHeaderStyle_1row">Address 1</td>
                                                <td width="10%" class="gridHeaderStyle_1row">Address 2</td>
                                                <td width="10%" class="gridHeaderStyle_1row">Address 3</td>
                                                <td width="10%" class="gridHeaderStyle_1row">Address 4</td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <asp:Panel ID="plnGridIntPartySearch" runat="server" ScrollBars="Auto">
                                            <asp:GridView ID="gvIntPartySearchList" runat="server" AutoGenerateColumns="False" Width="97.75%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                CssClass="gridStyle_hover" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                EmptyDataText="No data found" ShowHeader="false" OnRowCommand="gvIntPartySearchList_RowCommand" OnRowDataBound="gvIntPartySearchList_RowDataBound"
                                                RowStyle-CssClass="dataRow">
                                                <Columns>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align"
                                                        ItemStyle-Width="15%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblIntPartyName" runat="server" Width="99%" Text='<%# Bind("int_party_name") %>' CssClass="identifierLable"></asp:Label>
                                                            <asp:HiddenField ID="hdnIntPartyId" runat="server" Value='<%# Bind("int_party_id") %>' />
                                                            <asp:HiddenField ID="hdnIntPartyType" runat="server" Value='<%# Bind("int_party_type") %>' />
                                                            <asp:HiddenField ID="hdnEmail" runat="server" Value='<%# Bind("email") %>' />
                                                            <asp:HiddenField ID="hdnTaxNum" runat="server" Value='<%# Bind("vat_number") %>' />
                                                            <asp:HiddenField ID="hdnTaxType" runat="server" Value='<%# Bind("applicable_tax") %>' />
                                                            <asp:HiddenField ID="hdnGenerateInv" runat="server" Value='<%# Bind("generate_invoice") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align"
                                                        ItemStyle-Width="10%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblAddress1" runat="server" Width="99%" Text='<%# Bind("int_party_add1") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align"
                                                        ItemStyle-Width="10%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblAddress2" runat="server" Width="99%" Text='<%# Bind("int_party_add2") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align"
                                                        ItemStyle-Width="10%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblAddress3" runat="server" Width="99%" Text='<%# Bind("int_party_add3") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align"
                                                        ItemStyle-Width="10%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblAddress4" runat="server" Width="99%" Text='<%# Bind("int_party_add4") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkBtnDblClk" runat="server" CommandName="dblClk" Text="dblClick">
                                                            </asp:LinkButton>
                                                        </ItemTemplate>
                                                        <ItemStyle CssClass="hide" />
                                                    </asp:TemplateField>
                                                </Columns>

                                            </asp:GridView>
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Interested party search list popup-- Ends%>--%>

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
            <asp:HiddenField ID="hdnNewRoyaltorSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnGridDataDeleted" runat="server" Value="N" />
            <asp:Button ID="btnIntPartyPopup" runat="server" Style="display: none;" OnClick="btnIntPartyPopup_Click" CausesValidation="false" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" onkeydown="FocusLblKeyPress();"></asp:Label>
            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- Start--%>
            <asp:HiddenField ID="hdnDeleteIntPartyId" runat="server" />
            <asp:HiddenField ID="hdnDeleteIntPartyType" runat="server" />
            <asp:HiddenField ID="hdnDeleteIsModified" runat="server" />
            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- End--%>
            <asp:HiddenField ID="hdnIsAuditScreen" runat="server" Value="N" />
            <asp:HiddenField ID="hdnOtherUserScreenLocked" runat="server" Value="N" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

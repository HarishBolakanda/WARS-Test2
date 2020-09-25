<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaymentApproval.aspx.cs" Inherits="WARS.Payment.PaymentApproval" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Payment Approval" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">

        function GeneratePayment(buttonId) {
            if (IsGridDataChanged()) {
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = buttonId;
                OpenOnUnSavedData();
            }
            else {
                var popup = $find('<%= mpeConfirmGeneratePayment.ClientID %>');
                if (popup != null) {
                    popup.show();
                }
            }

            return false;
        }


        //================================End

    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnGeneratePaymentDetails" runat="server" Text="Generate Payment Details"
                            CssClass="LinkButtonStyle" Width="98%" OnClientClick="return GeneratePayment('btnGeneratePaymentDetails');" UseSubmitBehavior="false" />
                    </td>
                </tr>
            </table>
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
        var gridClientId = "ContentPlaceHolderBody_gvPaymentApproval_";
        var selectedRowIndex;
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

        //Owner fuzzy search functionalities        
        var txtOwnSrch;

        function ownerListPopulating() {
            txtOwnSrch = document.getElementById("<%= txtOwnFuzzySearch.ClientID %>");
            txtOwnSrch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtOwnSrch.style.backgroundRepeat = 'no-repeat';
            txtOwnSrch.style.backgroundPosition = 'right';
            document.getElementById('<%= hdnIsValidOwner.ClientID%>').value = "N";
        }

        function ownerListPopulated() {
            txtOwnSrch = document.getElementById("<%= txtOwnFuzzySearch.ClientID %>");
            txtOwnSrch.style.backgroundImage = 'none';
        }

        function ownerScrollPosition(sender, args) {
            var autoCompPnl = document.getElementById("<%= acePanelOwner.ClientID %>");
            autoCompPnl.scrollTop = 1;

        }

        function ownerListItemSelected(sender, args) {            
            var ownSrchVal = args.get_value();
            if (ownSrchVal == 'No results found') {
                document.getElementById("<%= txtOwnFuzzySearch.ClientID %>").value = "";
            }
            else {
                document.getElementById('<%= hdnIsValidOwner.ClientID%>').value = "Y";
            }
    }

    //================================End

    //Payee fuzzy search functionalities        
    var txtPayeeSrch;

    function payeeListPopulating() {
        txtPayeeSrch = document.getElementById("<%= txtPayeeFuzzySearch.ClientID %>");
        txtPayeeSrch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
        txtPayeeSrch.style.backgroundRepeat = 'no-repeat';
        txtPayeeSrch.style.backgroundPosition = 'right';
        document.getElementById('<%= hdnIsValidPayee.ClientID%>').value = "N";
    }

    function payeeListPopulated() {
        txtPayeeSrch = document.getElementById("<%= txtPayeeFuzzySearch.ClientID %>");
        txtPayeeSrch.style.backgroundImage = 'none';
    }

    function payeeScrollPosition(sender, args) {
        var autoCompPnl = document.getElementById("<%= acePanelPayee.ClientID %>");
        autoCompPnl.scrollTop = 1;
    }

    function payeeListItemSelected(sender, args) {        
        var payeeSrchVal = args.get_value();
        if (payeeSrchVal == 'No results found') {
            document.getElementById("<%= txtPayeeFuzzySearch.ClientID %>").value = "";
        }
        else {
            document.getElementById('<%= hdnIsValidPayee.ClientID%>').value = "Y";
        }

}
//================================End

//Royaltor fuzzy search functionalities        
var txtRoyaltorSrch;

function royaltorListPopulating() {
    txtRoyaltorSrch = document.getElementById("<%= txtRoyaltorFuzzySearch.ClientID %>");
    txtRoyaltorSrch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
    txtRoyaltorSrch.style.backgroundRepeat = 'no-repeat';
    txtRoyaltorSrch.style.backgroundPosition = 'right';
    document.getElementById('<%= hdnIsValidRoyaltor.ClientID%>').value = "N";
}

function royaltorListPopulated() {
    txtRoyaltorSrch = document.getElementById("<%= txtRoyaltorFuzzySearch.ClientID %>");
    txtRoyaltorSrch.style.backgroundImage = 'none';
}

function royaltorScrollPosition(sender, args) {
    var autoCompPnl = document.getElementById("<%= acePanelroyaltor.ClientID %>");
    autoCompPnl.scrollTop = 1;
}

function royaltorListItemSelected(sender, args) {    
    var royaltorSrchVal = args.get_value();
    if (royaltorSrchVal == 'No results found') {
        document.getElementById("<%= txtRoyaltorFuzzySearch.ClientID %>").value = "";
    }
    else {
        document.getElementById('<%= hdnIsValidRoyaltor.ClientID%>').value = "Y";
    }

}
//================================End



//validations===========Begin
//===========Warn on changes made and not saved === Begin
function WarnOnUnSavedData() {
    var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
    if (isExceptionRaised != "Y") {
        if (IsGridDataChanged()) {
            return warningMsgOnUnSavedData;
        }
    }

}
window.onbeforeunload = WarnOnUnSavedData;

function IsGridDataChanged() {
    //debugger;
    var gvPartSummary = document.getElementById("<%= gvPaymentApproval.ClientID %>");
        if (gvPartSummary != null) {
            var gvRows = gvPartSummary.rows;
            for (var i = 0; i < gvRows.length; i++) {

                //handling empty data row
                if (gvRows.length == 1 && document.getElementById(gridClientId + 'hdnPaymentId' + '_' + i) == null) {
                    break;
                }

                hdnAppUserCode1 = document.getElementById(gridClientId + 'hdnAppUserCode1' + '_' + i).value;
                hdnAppUserCode2 = document.getElementById(gridClientId + 'hdnAppUserCode2' + '_' + i).value;
                hdnAppUserCode3 = document.getElementById(gridClientId + 'hdnAppUserCode3' + '_' + i).value;
                hdnAppUserCode4 = document.getElementById(gridClientId + 'hdnAppUserCode4' + '_' + i).value;
                hdnAppUserCode5 = document.getElementById(gridClientId + 'hdnAppUserCode5' + '_' + i).value;
                hdnCanUserCode = document.getElementById(gridClientId + 'hdnCanUserCode' + '_' + i).value;

                cbApproval1 = document.getElementById(gridClientId + 'cbApproval1' + '_' + i);
                cbApproval2 = document.getElementById(gridClientId + 'cbApproval2' + '_' + i);
                cbApproval3 = document.getElementById(gridClientId + 'cbApproval3' + '_' + i);
                cbApproval4 = document.getElementById(gridClientId + 'cbApproval4' + '_' + i);
                cbApproval5 = document.getElementById(gridClientId + 'cbApproval5' + '_' + i);
                cbCancelPayment = document.getElementById(gridClientId + 'cbCancelPayment' + '_' + i);

                //debugger;

                if (((hdnAppUserCode1 == "" && cbApproval1.checked) || (hdnAppUserCode1 != "" && !cbApproval1.checked)) ||
                    ((hdnAppUserCode2 == "" && cbApproval2.checked) || (hdnAppUserCode2 != "" && !cbApproval2.checked)) ||
                    ((hdnAppUserCode3 == "" && cbApproval3.checked) || (hdnAppUserCode3 != "" && !cbApproval3.checked)) ||
                    ((hdnAppUserCode4 == "" && cbApproval4.checked) || (hdnAppUserCode4 != "" && !cbApproval4.checked)) ||
                    ((hdnAppUserCode5 == "" && cbApproval5.checked) || (hdnAppUserCode5 != "" && !cbApproval5.checked)) ||
                    ((hdnCanUserCode == "" && cbCancelPayment.checked) || (hdnCanUserCode != "" && !cbCancelPayment.checked))
                    ) {
                    return true;
                }

            }
        }

        return false;
    }

    function RedirectToErrorPage() {
        document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
        window.location = "../Common/ExceptionPage.aspx";
    }


    //used to check if any changes to allow navigation to other screen 
    function IsDataChanged() {

        if (IsGridDataChanged()) {
            return true;
        }
        else {
            return false;
        }
    }

    //=================End

    //===========Warn on changes made and not saved === End

    function ValidateSave() {
        if (!IsGridDataChanged()) {
            DisplayMessagePopup("No changes made to save");
            return false;
        }
        else {
            return true;
        }
    }

    function ValBalanceThreshold(sender, args) {
        txtBalThreshold = document.getElementById("<%=txtBalThreshold.ClientID %>").value;

    if (isNaN(txtBalThreshold)) {
        args.IsValid = false;
    }
    else {
        args.IsValid = true;
    }

}

function ValTxtStmtEndPeriod(sender, args) {
    var StmtEndPeriodRegex = /^((0[1-9])|(1[0-2]))\/((19|20)\d\d)$/; //valid month and year in MM/YYYY format

    txtStmtEndPeriod = document.getElementById("<%=txtStmtEndPeriod.ClientID %>").value;
    if (txtStmtEndPeriod == "__/____" || StmtEndPeriodRegex.test(txtStmtEndPeriod)) {
        args.IsValid = true;
    }
    else {
        args.IsValid = false;
    }

}

function ValidateSearch(button) {
    if (document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value == "Y") {
        return true;
    }
    document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "N";

    if (IsGridDataChanged()) {
        OpenOnUnSavedData();
        document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
        return false;
    }

    if (!Page_ClientValidate("valGrpSearch")) {
        Page_BlockSubmit = false;
        DisplayMessagePopup("Invalid or missing data");
        return false;
    }
    else {
        return true;
    }

}

//Harish: 11-12-2017: validation to allow approval if Below threshold
//Enable the first approval. Display a warning if selected 
var valBelowThresholdControlId;
function ValidateBelowThreshold(gridRow) {
    var selectedRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
    hdnPayBelowThreshold = document.getElementById(gridClientId + 'hdnPayBelowThreshold' + '_' + selectedRowIndex).value;
    valBelowThresholdControlId = document.getElementById(gridClientId + 'cbApproval1' + '_' + selectedRowIndex)

    if (valBelowThresholdControlId.checked && hdnPayBelowThreshold == "Y") {

        var popup = $find('<%= mpeBelowThresholdPopup.ClientID %>');
        if (popup != null) {
            popup.show();
        }
    }

    return false;

}

function ValidateBulk(button) {
    if (document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value == "Y") {
        return true;
    }
    document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "N";
    if (IsGridDataChanged()) {
        OpenOnUnSavedData();
        document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
    }
    else {
        return true;
    }
}

function ValidateBelowThresholdNo() {
    valBelowThresholdControlId.checked = false;
    var popup = $find('<%= mpeBelowThresholdPopup.ClientID %>');
    if (popup != null) {
        popup.hide();
    }
    return false;

}
//======================End

function ValidateUpdateInvoices(button) {
    if (document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value == "Y") {
        return true;
    }
    document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "N";
    if (IsGridDataChanged()) {
        OpenOnUnSavedData();
        document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
    }
    else {
        return true;
    }
}


function ValidateClear(button) {
    //debbuger;
    if (document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value == "Y") {
        return true;
    }
    document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "N";

    if (IsGridDataChanged()) {
        OpenOnUnSavedData();
        document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
    }
    else {
        return true;
    }
}

//validate if changes made not saved on page change
function ValidatePageChange(obj, button) {
    //on Exit button from data unsaved pop up
    if (document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value == "Y") {
        return true;
    }
    document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "N";

    if (IsGridDataChanged()) {
        //populate hdnPageIndex with the selected page number
        document.getElementById("<%=hdnPageIndex.ClientID %>").value = obj.innerHTML;
        document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
        if ((document.getElementById('<%=hdnSelectedBulkApproval.ClientID%>').value == "") && (document.getElementById('<%=hdnBulkCancelSelected.ClientID%>').value == "N")) {
            window.onbeforeunload = null;
            OpenOnUnSavedData();
        }
        else {
            window.onbeforeunload = null;
            return true;
        }

    }
    else {
        return true;
    }
}

//validations ================================End

//Enter Key Search functionality
function LoadSearchData() {
    if ((event.keyCode == 13)) {
        document.getElementById('<%= btnGo.ClientID%>').click();
    }
}

        function OntxtRoyaltorKeyDown() {
            var txtRoyaltor = document.getElementById("<%= txtRoyaltorFuzzySearch.ClientID %>").value;
            if ((event.keyCode == 13)) {                
                if (txtRoyaltor == "") {
                    document.getElementById('<%=btnGo.ClientID%>').click();
                }
                else {
                    if (document.getElementById("<%= hdnIsValidRoyaltor.ClientID %>").value == "Y") {
                        document.getElementById('<%=btnGo.ClientID%>').click();
                    }
                    else {
                        return false;
                    }
                }
            }
        }

        function OntxtOwnerKeyDown() {
            var txtOwner = document.getElementById("<%= txtOwnFuzzySearch.ClientID %>").value;
            if ((event.keyCode == 13)) {                
                if (txtOwner == "") {
                    document.getElementById('<%=btnGo.ClientID%>').click();
                }
                else {
                    if (document.getElementById("<%= hdnIsValidOwner.ClientID %>").value == "Y") {
                        document.getElementById('<%=btnGo.ClientID%>').click();
                    }
                    else {
                        return false;
                    }
                }
            }
        }

        function OntxtPayeeKeyDown() {
            var txtPayee = document.getElementById("<%= txtPayeeFuzzySearch.ClientID %>").value;
            if ((event.keyCode == 13)) {                
                if (txtPayee == "") {
                    document.getElementById('<%=btnGo.ClientID%>').click();
                }
                else {
                    if (document.getElementById("<%= hdnIsValidPayee.ClientID %>").value == "Y") {
                        document.getElementById('<%=btnGo.ClientID%>').click();
                    }
                    else {
                        return false;
                    }
                }
            }
        }
        //end of enter key functionality

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }
        //=============== End

        //Navigate to payment details screen
        function PaymentDetailsScreen() {
            if (IsGridDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Payment/PaymentDetails.aspx");
            }
            else {
                window.location = "../Payment/PaymentDetails.aspx";
            }
        }
        //============== End

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
            document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "Y";
        window.onbeforeunload = WarnOnUnSavedData;

        var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
        if (warnPopup != null) {
            warnPopup.hide();
        }

        //confirmation if Generate payments is requested
        if (document.getElementById("<%=hdnButtonSelection.ClientID %>").value == "btnGeneratePaymentDetails") {
        //JIRA-908 CHanges by Ravi on 12/02/2019 -- STart
        var popup = $find('<%= mpeConfirmGeneratePayment.ClientID %>');
        if (popup != null) {
            popup.show();
            //JIRA-908 CHanges by Ravi on 12/02/2019 -- End
        }

        return false;
    }

    return true;
}
//============== End

// WUIN-1018 Bulk Approval and cancel  - Start

//Validation to allow 4th and 5th approval only if 3rd approval approved
var val3rdApprovalControlId;
function ValidateHigherApproval(gridRow) {
    var selectedRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
    val3rdApprovalControlId = document.getElementById(gridClientId + 'cbApproval3' + '_' + selectedRowIndex)
    if (document.getElementById(gridRow.id).checked) {
        if (!(val3rdApprovalControlId.disabled) && val3rdApprovalControlId.checked) {
            return true;
        }
        else {
            DisplayMessagePopup("The selected payments require a 3rd approval level before they can be approved.");
            return false;
        }
    }

}


function ValidateBulkConfirmNo() {
    var popup = $find('<%= mpeBulkConfirmPopup.ClientID %>');
    if (popup != null) {
        popup.hide();

    }
    return true;

}


function RowLevelValidationOnBulkUpdate() {
    hdnSelectedBulkApproval = document.getElementById("<%=hdnSelectedBulkApproval.ClientID %>").value;
        hdnBulkCancelSelected = document.getElementById("<%=hdnBulkCancelSelected.ClientID %>").value;
        if (hdnSelectedBulkApproval != "") {
            DisplayMessagePopup("No row level approval/cancel is allowed on bulk Approval.");
            return false;
        }
        else if (hdnBulkCancelSelected == "Y") {
            DisplayMessagePopup("No row level approval/cancel is allowed on bulk Cancel.");
            return false;
        }
        else { return true; }

    }

    function ValidateBulkUpdate(id) {
        hdnSelectedBulkApproval = document.getElementById("<%=hdnSelectedBulkApproval.ClientID %>").value;
        hdnBulkCancelSelected = document.getElementById("<%=hdnBulkCancelSelected.ClientID %>").value;
        if (document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value == "Y") {
            return true;
        }
        document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "N";
        if (document.getElementById('<%=hdnBulkDataModified.ClientID%>').value == "Y") {
            if (!((hdnSelectedBulkApproval != "" && id.indexOf("cbHdrApproval" + hdnSelectedBulkApproval) != -1) || (hdnBulkCancelSelected == "Y" && id.indexOf("cbHdrCancel") != -1))) {
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = "BulkUpdate";
                return false;
            }
            else {
                return true;
            }

        }
        else {
            return true;
        }


    }


    // ============== End
    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="14">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    PAYMENT APPROVAL
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="14"></td>
                </tr>
                <tr>
                    <td colspan="14"></td>
                </tr>
                <tr>
                    <td colspan="14"></td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="9%" class="identifierLable_large_bold">Company</td>
                    <td width="14%">
                        <asp:DropDownList ID="ddlCompany" runat="server" Width="95%" CssClass="ddlStyle" TabIndex="100" onkeyup="LoadSearchData();">
                        </asp:DropDownList>
                    </td>
                    <td width="2%"></td>
                    <td width="8%" class="identifierLable_large_bold">Statement Status</td>
                    <td width="10%">
                        <asp:DropDownList ID="ddlStmtStatus" runat="server" Width="90%" CssClass="ddlStyle" TabIndex="101" onkeyup="LoadSearchData();">
                        </asp:DropDownList>
                    </td>
                    <td width="2%"></td>
                    <td width="4%" class="identifierLable_large_bold">Owner</td>
                    <td width="15%">
                        <asp:TextBox ID="txtOwnFuzzySearch" runat="server" Width="98%" CssClass="identifierLable"
                            TabIndex="102" OnKeyDown="OntxtOwnerKeyDown();"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="aceOwnFuzzySearch" runat="server"
                            ServiceMethod="FuzzySearchAllOwnerList"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtOwnFuzzySearch"
                            FirstRowSelected="true"
                            OnClientPopulating="ownerListPopulating"
                            OnClientPopulated="ownerListPopulated"
                            OnClientHidden="ownerListPopulated"
                            OnClientShown="ownerScrollPosition"
                            OnClientItemSelected="ownerListItemSelected"
                            CompletionListElementID="acePanelOwner" />
                        <asp:Panel ID="acePanelOwner" runat="server" CssClass="identifierLable" />
                    </td>
                    <td width="4%" valign="middle">
                        <asp:ImageButton ID="btnFuzzySearchOwner" ImageUrl="../Images/Search.png" runat="server" Style="cursor: pointer"
                            OnClick="btnFuzzySearchOwner_Click" ToolTip="Search Owner" CssClass="FuzzySearch_Button" />
                    </td>
                    <td width="8%" class="identifierLable_large_bold">Balance Threshold</td>
                    <td width="10%">
                        <asp:TextBox ID="txtBalThreshold" runat="server" Width="40%" CssClass="identifierLable" OnKeyDown="LoadSearchData();" TabIndex="103"
                            ToolTip="Please enter only numeric value"></asp:TextBox>
                        <asp:CustomValidator ID="valtxtBalThreshold" runat="server" ValidationGroup="valGrpSearch" CssClass="requiredFieldValidator"
                            ClientValidationFunction="ValBalanceThreshold" ToolTip="Please enter only numeric value"
                            ErrorMessage="*"></asp:CustomValidator>
                    </td>
                    <td width="2%"></td>
                    <td rowspan="3" valign="top">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td align="right">
                                    <asp:Button ID="btnSave" runat="server" CssClass="ButtonStyle" OnClick="btnSave_Click"
                                        Text="Save Changes" UseSubmitBehavior="false" Width="85%" OnClientClick="if (!ValidateSave()) { return false;};" TabIndex="113" />
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                            </tr>

                            <tr>
                                <td></td>
                            </tr>

                            <tr>
                                <td align="right">
                                    <asp:Button ID="btnUpdateInvoice" runat="server" CssClass="ButtonStyle" OnClientClick="if (!ValidateUpdateInvoices('PaymentReady')) { return false;};" OnClick="btnUpdateInvoice_Click"
                                        Text="Payment Ready" UseSubmitBehavior="false" Width="85%" TabIndex="115" />
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                            </tr>
                            <tr>
                                <td></td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Button ID="btnPaymentDetails" runat="server" CssClass="ButtonStyle"
                                        Text="Payment Details" UseSubmitBehavior="false" Width="85%" TabIndex="116" OnClientClick="if (!PaymentDetailsScreen()) { return false;};" onkeydown="OnTabPress();" />
                                </td>
                            </tr>
                        </table>
                    </td>

                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">Statement End Period</td>
                    <td>
                        <asp:TextBox ID="txtStmtEndPeriod" runat="server" Width="30%" CssClass="identifierLable" OnKeyDown="LoadSearchData();" TabIndex="104" ToolTip="MM/YYYY"></asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmeStmtEndPeriod" runat="server" TargetControlID="txtStmtEndPeriod"
                            WatermarkText="MM/YYYY" WatermarkCssClass="waterMarkText">
                        </ajaxToolkit:TextBoxWatermarkExtender>
                        <ajaxToolkit:MaskedEditExtender ID="mteStmtEndPeriod" runat="server"
                            TargetControlID="txtStmtEndPeriod" Mask="99/9999" AcceptNegative="None"
                            ClearMaskOnLostFocus="false" />
                        <asp:CustomValidator ID="valtxtStmtEndPeriod" runat="server" ValidationGroup="valGrpSearch" CssClass="requiredFieldValidator"
                            ClientValidationFunction="ValTxtStmtEndPeriod" ToolTip="Please enter valid month and year in MM/YYYY format"
                            ErrorMessage="*"></asp:CustomValidator>
                    </td>
                    <td></td>
                    <td class="identifierLable_large_bold">Payee Status</td>
                    <td>
                        <asp:DropDownList ID="ddlPayeeStatus" runat="server" Width="90%" CssClass="ddlStyle" TabIndex="105" OnKeyup="LoadSearchData();">
                        </asp:DropDownList>
                    </td>
                    <td></td>
                    <td class="identifierLable_large_bold">Payee</td>
                    <td>
                        <asp:TextBox ID="txtPayeeFuzzySearch" runat="server" Width="98%" CssClass="identifierLable" TabIndex="106" OnKeyDown="OntxtPayeeKeyDown();"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="acePayeeFuzzySearch" runat="server"
                            ServiceMethod="FuzzySearchAllPayeeList"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtPayeeFuzzySearch"
                            FirstRowSelected="true"
                            OnClientPopulating="payeeListPopulating"
                            OnClientPopulated="payeeListPopulated"
                            OnClientHidden="payeeListPopulated"
                            OnClientShown="payeeScrollPosition"
                            OnClientItemSelected="payeeListItemSelected"
                            CompletionListElementID="acePanelPayee" />
                        <asp:Panel ID="acePanelPayee" runat="server" CssClass="identifierLable" />
                    </td>
                    <td valign="middle">
                        <asp:ImageButton ID="btnFuzzySearchPayee" ImageUrl="../Images/Search.png" runat="server" Style="cursor: pointer"
                            OnClick="btnFuzzySearchPayee_Click1" ToolTip="Search Payee" CssClass="FuzzySearch_Button" />
                    </td>
                    <td class="identifierLable_large_bold">Responsibility</td>
                    <td>
                        <asp:DropDownList ID="ddlResponsibility" runat="server" Width="90%" CssClass="ddlStyle" TabIndex="107" onkeyup="LoadSearchData();">
                        </asp:DropDownList>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">Reported Days</td>
                    <td>
                        <asp:TextBox ID="txtReportedDays" runat="server" Width="30%" CssClass="identifierLable" OnKeyDown="LoadSearchData();" TabIndex="108"
                            ToolTip="Please enter only numeric value without decimals" TextMode="Number"></asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmeReportedDays" runat="server" TargetControlID="txtReportedDays"
                            WatermarkText="NN" WatermarkCssClass="waterMarkText">
                        </ajaxToolkit:TextBoxWatermarkExtender>
                        <ajaxToolkit:FilteredTextBoxExtender ID="ftetxtReportedDays" runat="server"
                            Enabled="True" TargetControlID="txtReportedDays" FilterType="Numbers">
                        </ajaxToolkit:FilteredTextBoxExtender>
                    </td>
                    <td></td>
                    <td class="identifierLable_large_bold">Payment Status</td>
                    <td>
                        <asp:DropDownList ID="ddlPaymentStatus" runat="server" Width="90%" CssClass="ddlStyle" TabIndex="109" onkeyup="LoadSearchData();">
                        </asp:DropDownList>
                    </td>
                    <td></td>
                    <td class="identifierLable_large_bold">Royaltor</td>
                    <td>
                        <asp:TextBox ID="txtRoyaltorFuzzySearch" runat="server" Width="98%" CssClass="identifierLable"
                            TabIndex="110" OnKeyDown="OntxtRoyaltorKeyDown();"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="aceRoyaltorFuzzySearch" runat="server"
                            ServiceMethod="FuzzySearchAllRoyListWithOwnerCode"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtRoyaltorFuzzySearch"
                            FirstRowSelected="true"
                            OnClientPopulating="royaltorListPopulating"
                            OnClientPopulated="royaltorListPopulated"
                            OnClientHidden="royaltorListPopulated"
                            OnClientShown="royaltorScrollPosition"
                            OnClientItemSelected="royaltorListItemSelected"
                            CompletionListElementID="acePanelroyaltor" />
                        <asp:Panel ID="acePanelroyaltor" runat="server" CssClass="identifierLable" />
                    </td>
                    <td valign="middle">
                        <asp:ImageButton ID="btnFuzzySearchRoyaltor" ImageUrl="../Images/Search.png" runat="server" Style="cursor: pointer"
                            OnClick="btnFuzzySearchRoyaltor_Click" ToolTip="Search Royaltor" CssClass="FuzzySearch_Button" />
                    </td>
                    <td colspan="2" rowspan="2" valign="top" style="padding-top: 3px">
                        <table width="50%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="40%">
                                    <asp:Button ID="btnGo" runat="server" CssClass="ButtonStyle" Text="Go" UseSubmitBehavior="false" Width="80%" TabIndex="111"
                                        ValidationGroup="valGrpSearch" OnClientClick="if (!ValidateSearch('GoButton')) { return false;};" OnClick="btnGo_Click" />
                                </td>
                                <td width="5%"></td>
                                <td>
                                    <asp:Button ID="btnClear" runat="server" CssClass="ButtonStyle" Text="Clear" UseSubmitBehavior="false" Width="80%"
                                        OnClientClick="if (!ValidateClear('ClearButton')) { return false;};" OnClick="btnClear_Click" TabIndex="112" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td></td>

                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>

                </tr>
                <tr>
                    <td colspan="14">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td colspan="14">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td></td>
                            </tr>
                            <tr>
                                <td>
                                    <table width="98.75%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td colspan="6" class="gridTitle_Bold" align="center">Statement Details</td>
                                            <td width="1px"></td>
                                            <td colspan="3" class="gridTitle_Bold" align="center">Payee Details</td>
                                            <td width="1px"></td>
                                            <td colspan="8" class="gridTitle_Bold" align="center">Payment Details</td>
                                        </tr>
                                        <tr>
                                            <td width="14%" class="gridHeaderStyle_1row">Royaltor</td>
                                            <td width="8%" class="gridHeaderStyle_1row">Reporting Schedule</td>
                                            <td width="5%" class="gridHeaderStyle_1row">Payment Date</td>
                                            <td width="5%" class="gridHeaderStyle_1row">Status</td>
                                            <td width="6%" class="gridHeaderStyle_1row">Responsibility</td>
                                            <td width="5%" class="gridHeaderStyle_1row">Statement Balance</td>
                                            <td width="1px">&nbsp</td>
                                            <td width="8%" class="gridHeaderStyle_1row">Payee</td>
                                            <td width="5%" class="gridHeaderStyle_1row">Status</td>
                                            <td width="5%" class="gridHeaderStyle_1row">Payee Share</td>
                                            <td width="1px">&nbsp</td>
                                            <td width="7%" class="gridHeaderStyle_1row">Status</td>
                                            <td width="4%" class="gridHeaderStyle_1row">Currency</td>
                                            <td width="3%" class="gridHeaderStyle_1row">VAT</td>
                                            <td width="3%" class="gridHeaderStyle_1row">Value in GBP</td>
                                            <td width="4%" class="gridHeaderStyle_1row">Exchange Rate</td>
                                            <td width="4%" class="gridHeaderStyle_1row">Payment Value</td>
                                            <td width="10%" class="gridHeaderStyle_1row">
                                                <table width="98.25%" align="left">
                                                    <tr>
                                                        <td colspan="5">Approval
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>1
                                                            <br />
                                                            <asp:CheckBox ID="cbHdrApproval1" runat="server" OnCheckedChanged="cbHdrApproval_OnCheckedChanged" AutoPostBack="true" onclick="if (!ValidateBulkUpdate(this.id)) { return false;};" />
                                                        </td>
                                                        <td>2
                                                            <br />
                                                            <asp:CheckBox ID="cbHdrApproval2" runat="server" OnCheckedChanged="cbHdrApproval_OnCheckedChanged" AutoPostBack="true" onclick="if (!ValidateBulkUpdate(this.id)) { return false;};" />
                                                        </td>
                                                        <td>3
                                                              <br />
                                                            <asp:CheckBox ID="cbHdrApproval3" runat="server" OnCheckedChanged="cbHdrApproval_OnCheckedChanged" AutoPostBack="true" onclick="if (!ValidateBulkUpdate(this.id)) { return false;};" />
                                                        </td>
                                                        <td>4
                                                            <br />
                                                            <asp:CheckBox ID="cbHdrApproval4" runat="server" OnCheckedChanged="cbHdrApproval_OnCheckedChanged" AutoPostBack="true" onclick="if (!ValidateBulkUpdate(this.id)) { return false;};" />
                                                        </td>
                                                        <td>5
                                                            <br />
                                                            <asp:CheckBox ID="cbHdrApproval5" runat="server" OnCheckedChanged="cbHdrApproval_OnCheckedChanged" AutoPostBack="true" onclick="if (!ValidateBulkUpdate(this.id)) { return false;};" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td width="4%" class="gridHeaderStyle_1row">
                                                <table width="100%" align="left">
                                                    <tr>
                                                        <td>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Cancel
                                                            <br />
                                                            <asp:CheckBox ID="cbHdrCancel" runat="server" OnCheckedChanged="cbHdrCancel_OnCheckedChanged" AutoPostBack="true" onclick="if (!ValidateBulkUpdate(this.id)) { return false;};" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="100%">
                                        <asp:GridView ID="gvPaymentApproval" runat="server" AutoGenerateColumns="False" Width="98.78%" AlternatingRowStyle-BackColor="#E3EFFF"
                                            CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" ShowHeaderWhenEmpty="true" EmptyDataText="No Data Found"
                                            EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" OnRowDataBound="gvPaymentApproval_RowDataBound" ShowHeader="false">
                                            <Columns>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="14%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRoyaltor" runat="server" Text='<%#Bind("royaltor")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="8%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRepSchedule" runat="server" Text='<%#Bind("rep_schedule")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPaymentDate" runat="server" Text='<%#Bind("payment_date")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStmtStatus" runat="server" Text='<%#Bind("statement_status")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="6%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblResponsibility" runat="server" Text='<%#Bind("responsibility")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Right_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="lblStmtBalance" runat="server" Width="95%" Text='<%#Bind("payable_amount")%>' CssClass="gridTextField" ReadOnly="true"
                                                            Style="text-align: right"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="2px" ItemStyle-CssClass="gridItemStyle_No_Border">
                                                    <ItemTemplate>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="8%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPayee" runat="server" Text='<%#Bind("int_party_name")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPayeeStatus" runat="server" Text='<%#Bind("payee_status")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPayeeShare" runat="server" Text='<%#Bind("payee_share")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="2px" ItemStyle-CssClass="gridItemStyle_No_Border">
                                                    <ItemTemplate>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="7%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPaymentStatus" runat="server" Text='<%#Bind("payment_status")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPaymentCurrency" runat="server" Text='<%#Bind("currency_code")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="3%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblVAT" runat="server" Text='<%#Bind("VAT")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="3%" ItemStyle-CssClass="gridItemStyle_Right_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="lblPaymentValueInGBP" runat="server" Width="95%" Text='<%#Bind("value_in_GBP")%>' CssClass="gridTextField" ReadOnly="true"
                                                            Style="text-align: right"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPaymentExchRate" runat="server" Text='<%#Bind("exchange_rate")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Right_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="lblPaymentValue" runat="server" Width="95%" Text='<%#Bind("payment_value")%>' CssClass="gridTextField" ReadOnly="true"
                                                            Style="text-align: right"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <table width="100%">
                                                            <tr>
                                                                <td>
                                                                    <asp:CheckBox ID="cbApproval1" runat="server" onclick="ValidateBelowThreshold(this);" onfocusin="return RowLevelValidationOnBulkUpdate();" />
                                                                </td>
                                                                <td>
                                                                    <asp:CheckBox ID="cbApproval2" runat="server" onfocusin="return RowLevelValidationOnBulkUpdate();" />
                                                                </td>
                                                                <td>
                                                                    <asp:CheckBox ID="cbApproval3" runat="server" onfocusin="return RowLevelValidationOnBulkUpdate();" />
                                                                </td>
                                                                <td>
                                                                    <asp:CheckBox ID="cbApproval4" runat="server" onclick="return ValidateHigherApproval(this);" onfocusin="return RowLevelValidationOnBulkUpdate();" />
                                                                </td>
                                                                <td>
                                                                    <asp:CheckBox ID="cbApproval5" runat="server" onclick="return ValidateHigherApproval(this);" onfocusin="return RowLevelValidationOnBulkUpdate();" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" ItemStyle-Width="4%">
                                                    <ItemTemplate>
                                                        <table width="100%" align="center">
                                                            <tr>
                                                                <td>
                                                                    <asp:CheckBox ID="cbCancelPayment" Text="" runat="server" onfocusin="return RowLevelValidationOnBulkUpdate();" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <asp:HiddenField ID="hdnPaymentId" runat="server" Value='<%# Bind("payment_id") %>' />
                                                        <asp:HiddenField ID="hdnApprovalLevel" runat="server" Value='<%# Bind("approval_level") %>' />
                                                        <asp:HiddenField ID="hdnAppUserCode1" runat="server" Value='<%# Bind("approval_user_code_1") %>' />
                                                        <asp:HiddenField ID="hdnAppUserCode2" runat="server" Value='<%# Bind("approval_user_code_2") %>' />
                                                        <asp:HiddenField ID="hdnAppUserCode3" runat="server" Value='<%# Bind("approval_user_code_3") %>' />
                                                        <asp:HiddenField ID="hdnAppUserCode4" runat="server" Value='<%# Bind("approval_user_code_4") %>' />
                                                        <asp:HiddenField ID="hdnAppUserCode5" runat="server" Value='<%# Bind("approval_user_code_5") %>' />
                                                        <asp:HiddenField ID="hdnCanUserCode" runat="server" Value='<%# Bind("cancelled_user_code") %>' />
                                                        <asp:HiddenField ID="hdnIsApprovalEnabled" runat="server" Value='<%# Bind("is_approval_enabled") %>' />
                                                        <asp:HiddenField ID="hdnRoyaltorId" runat="server" Value='<%# Bind("royaltor_id") %>' />
                                                        <asp:HiddenField ID="hdnPayBelowThreshold" runat="server" Value='<%# Bind("payment_below_threshold") %>' />
                                                        <asp:HiddenField ID="hdnPaymentStatusCode" runat="server" Value='<%# Bind("payment_status_code") %>' />
                                                        <asp:HiddenField ID="hdnBulkApproval1" runat="server" Value='<%# Bind("bulk_approval_1") %>' />
                                                        <asp:HiddenField ID="hdnBulkApproval2" runat="server" Value='<%# Bind("bulk_approval_2") %>' />
                                                        <asp:HiddenField ID="hdnBulkApproval3" runat="server" Value='<%# Bind("bulk_approval_3") %>' />
                                                        <asp:HiddenField ID="hdnBulkApproval4" runat="server" Value='<%# Bind("bulk_approval_4") %>' />
                                                        <asp:HiddenField ID="hdnBulkApproval5" runat="server" Value='<%# Bind("bulk_approval_5") %>' />
                                                        <asp:HiddenField ID="hdnBulkCancel" runat="server" Value='<%# Bind("bulk_cancel") %>' />
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
                    <td colspan="14">
                        <div align="center">
                            <asp:Repeater ID="rptPager" runat="server">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>'
                                        OnClientClick="if (!ValidatePageChange(this,'OnPageChange')) { return false;};" OnClick="lnkPage_Click"
                                        ClientIDMode="AutoID" Enabled='<%# Eval("Enabled") %>' CssClass="gridPager"> </asp:LinkButton>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
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

            <asp:Button ID="dummyFuzzySearch" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeFuzzySearch" runat="server" PopupControlID="pnlFuzzySearch" TargetControlID="dummyFuzzySearch"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlFuzzySearch" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="identifierLable">Complete Search List
                                    </td>
                                    <td align="right" style="vertical-align: top;">
                                        <asp:ImageButton ID="btnCloseFuzzySearchPopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" OnClick="btnCloseFuzzySearchPopup_Click" />
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

            <%--Cat status change pop up--%>
            <asp:Button ID="dummyBelowThresholdPopup" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeBelowThresholdPopup" runat="server" PopupControlID="pnlBelowThresholdPopup" TargetControlID="dummyBelowThresholdPopup"
                CancelControlID="btnYesBelowThresholdPopup" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlBelowThresholdPopup" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblHdrBelowThresholdPopup" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMsgBelowThresholdPopup" runat="server" Text="Payment is below threshold - do you want to approve this payment?"
                                CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYesBelowThresholdPopup" runat="server" Text="Yes" CssClass="ButtonStyle" CausesValidation="false" />

                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNoBelowThresholdPopup" runat="server" Text="No" CssClass="ButtonStyle" OnClientClick="return ValidateBelowThresholdNo();" CausesValidation="false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Cat status change pop up - Ends--%>



            <%--Bulk Approval/Cancel confirmation pop up--%>
            <asp:Button ID="dummyBulkConfirmPopup" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeBulkConfirmPopup" runat="server" PopupControlID="pnlBulkConfirmPopup" TargetControlID="dummyBulkConfirmPopup"
                CancelControlID="btnYesBulkConfirmPopup" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlBulkConfirmPopup" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblHdrBulkConfirmPopup" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMsgBulkConfirmPopup" runat="server" Text=""
                                CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYesBulkConfirmPopup" runat="server" CssClass="ButtonStyle" OnClick="btnBulkConfirmYes_Click"
                                            Text="Yes" UseSubmitBehavior="false" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNoBulkConfirmPopup" runat="server" Text="No" CssClass="ButtonStyle" OnClientClick="return ValidateBulkConfirmNo();" CausesValidation="false" OnClick="btnBulkConfirmNo_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Bulk Approval/Cancel confirmation pop up - Ends--%>

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
                                        <asp:Button ID="btnUnSavedDataExit" runat="server" Text="Exit" CssClass="ButtonStyle" Width="30%" OnClientClick="if (!OnUnSavedDataExit()) { return false;};"
                                            OnClick="btnUnSavedDataExit_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- Start--%>
            <asp:Button ID="dummyConfirmGeneratePayment" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmGeneratePayment" runat="server" PopupControlID="pnlConfirmGeneratePayment" TargetControlID="dummyConfirmGeneratePayment"
                CancelControlID="btnNo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlConfirmGeneratePayment" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmation" runat="server" Text="Generate Payment Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblText" runat="server"
                                CssClass="identifierLable" Text="Do you want to generate payment details?"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYes" runat="server" Text="Yes" CssClass="ButtonStyle" OnClick="btnGeneratePayment_Click" />
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
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" Value="" />
            <asp:HiddenField ID="hdnUserRoleApprovalLevel" runat="server" Value="" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsConfirmPopup" runat="server" Value="N" />
            <asp:HiddenField ID="hdnButtonSelection" runat="server" />
            <asp:HiddenField ID="hdnPageIndex" runat="server" Value="" />
            <asp:HiddenField ID="hdnPaymentRoleId" runat="server" />
            <asp:HiddenField ID="hdnSelectedBulkApproval" runat="server" />
            <asp:HiddenField ID="hdnBulkCancelSelected" runat="server" Value="N" />
            <asp:HiddenField ID="hdnBulkDataModified" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsValidRoyaltor" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsValidOwner" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsValidPayee" runat="server" Value="N" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" />
            <asp:Button ID="btnGeneratePayment" runat="server" OnClick="btnGeneratePayment_Click" Style="display: none" />

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

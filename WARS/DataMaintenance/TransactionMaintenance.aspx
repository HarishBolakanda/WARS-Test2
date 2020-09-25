<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="TransactionMaintenance.aspx.cs" Inherits="WARS.TransactionMaintenance" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Transaction Maintenance" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>
<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        function MissingParticipScreen() {
            if (IsGridDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../Participants/MissingParticipants.aspx?isNewRequest=N');
            }
            else {
                window.location = '../Participants/MissingParticipants.aspx?isNewRequest=N';
            }
        }
    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnMissingParticip" runat="server" CssClass="LinkButtonStyle"
                            Text="Missing Participants" UseSubmitBehavior="false" Width="98%" OnClientClick="if (!MissingParticipScreen()) { return false;};" />
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
            postBackElementID = args.get_postBackElement().id.substring(args.get_postBackElement().id.lastIndexOf("_") + 1);
            if (postBackElementID == 'btnSaveChanges') {
                xPos = sender._scrollPosition.x;
                yPos = sender._scrollPosition.y;

                //hold scroll position on selecting expand/collapse
                var PnlReference = document.getElementById("<%=PnlTransactions.ClientID %>");
                scrollTop = PnlReference.scrollTop;
            }
        }

        function EndRequestHandler(sender, args) {
            //Hide the modal popup - the update progress            
            var popup = $find('<%= mPopupPageLevel.ClientID %>');
            if (popup != null) {
                popup.hide();
            }

            //to maintain scroll position
            postBackElementID = sender._postBackSettings.sourceElement.id.substring(sender._postBackSettings.sourceElement.id.lastIndexOf("_") + 1);
            if (postBackElementID == 'btnSaveChanges') {
                window.scrollTo(xPos, yPos);

                //set scroll position on selecting expand/collapse
                var PnlReference = document.getElementById("<%=PnlTransactions.ClientID %>");
                PnlReference.scrollTop = scrollTop;

            }
        }
        //======================= End

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        //grid panel height adjustment functioanlity - starts
        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
            if (document.getElementById("<%=PnlTransactions.ClientID %>") != null) {
                document.getElementById("<%=PnlTransactions.ClientID %>").style.height = gridPanelHeight + "px";
            }
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
        }

        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";

        }

        //SalesType

        function salesTypeListPopulating() {
            txtSalesTypeAddRow = document.getElementById("<%= txtSalesTypeSearch.ClientID %>");
            txtSalesTypeAddRow.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtSalesTypeAddRow.style.backgroundRepeat = 'no-repeat';
            txtSalesTypeAddRow.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidSalesType.ClientID %>").value = "N";

        }

        function salesTypeListPopulated() {
            txtSalesTypeAddRow = document.getElementById("<%= txtSalesTypeSearch.ClientID %>");
            txtSalesTypeAddRow.style.backgroundImage = 'none';
        }

        function salesTypeListHidden() {
            txtSalesTypeAddRow = document.getElementById("<%= txtSalesTypeSearch.ClientID %>");
            txtSalesTypeAddRow.style.backgroundImage = 'none';

        }

        function salesTypeListItemSelected(sender, args) {          
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                document.getElementById("<%= txtSalesTypeSearch.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnIsValidSalesType.ClientID %>").value = "Y";
            }
        }

        //Seller

        function sellerListPopulating() {
            txtSeller = document.getElementById("<%= txtSellerSearch.ClientID %>");
            txtSeller.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtSeller.style.backgroundRepeat = 'no-repeat';
            txtSeller.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidSeller.ClientID %>").value = "N";

        }

        function sellerListPopulated() {
            txtSeller = document.getElementById("<%= txtSellerSearch.ClientID %>");
            txtSeller.style.backgroundImage = 'none';
        }

        function sellerListHidden() {
            txtSeller = document.getElementById("<%= txtSellerSearch.ClientID %>");
            txtSeller.style.backgroundImage = 'none';

        }

        function sellerListItemSelected(sender, args) {            
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                document.getElementById("<%= txtSellerSearch.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnIsValidSeller.ClientID %>").value = "Y";
            }
        }

        //Warn on Unsaved change incase of redirect or error message    

        function WarnOnUnSavedData() {
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            if (isExceptionRaised != "Y") {
                if (IsDataChanged()) {
                    return warningMsgOnUnSavedData;
                }
            }
        }
        window.onbeforeunload = WarnOnUnSavedData;

        function IsDataChanged() {
            //debugger;
            if (IsGridDataChanged()) {
                return true;
            }
            else {
                return false;
            }
        }

        function OnCatNoKeyDown() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnHdnCatNoSearch.ClientID%>').click();
            }
        }

        function OnGridDataChanges(row, name) {
            var selectedRowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            CompareGridData(selectedRowIndex);
        }

        function ValidateChanges() {
            //debugger;
            IsGridDataChanged();
            if (document.getElementById("<%=hdnGridDataChanged.ClientID %>").value != "Y") {
                eval(this.href);
                return true;
            }
            else {
                if (IsDataChanged()) {
                    return false;
                }
                else {
                    document.getElementById("<%=hdnGridDataChanged.ClientID %>").value = "N"
                    eval(this.href);
                    return true;
                }
            }
        }

        function ValidateTransaction(button) {
            if (document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value == "Y") {
                return true;
            }

            document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "N";

            if (IsGridDataChanged()) {
                window.onbeforeunload = null;
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
            }
            else { return true; }
        }

        function CompareGridData(rowIndex) {
            var str = "ContentPlaceHolderBody_gvTransactions_";
            var hdnReceivedDate = document.getElementById(str + 'hdnReceivedDate' + '_' + rowIndex).value;
            var txtReceivedDate = document.getElementById(str + 'txtReceivedDate' + '_' + rowIndex).value;
            var hdnReportedDate = document.getElementById(str + 'hdnReportedDate' + '_' + rowIndex).value;
            var txtReportedDate = document.getElementById(str + 'txtReportedDate' + '_' + rowIndex).value;
            var hdnCatNo = document.getElementById(str + 'hdnCatNo' + '_' + rowIndex).value;
            var txtCatNo = document.getElementById(str + 'txtCatNo' + '_' + rowIndex).value;
            var hdnSeller = document.getElementById(str + 'hdnSeller' + '_' + rowIndex).value;
            var hdnSalesType = document.getElementById(str + 'hdnSalesType' + '_' + rowIndex).value;
            var txtSalesType = document.getElementById(str + 'txtSalesType' + '_' + rowIndex).value;
            var hdnPrice1 = document.getElementById(str + 'hdnPrice1' + '_' + rowIndex).value;
            var txtPrice1 = document.getElementById(str + 'txtPrice1' + '_' + rowIndex).value;
            var hdnPrice2 = document.getElementById(str + 'hdnPrice2' + '_' + rowIndex).value;
            var txtPrice2 = document.getElementById(str + 'txtPrice2' + '_' + rowIndex).value;
            var hdnPrice3 = document.getElementById(str + 'hdnPrice3' + '_' + rowIndex).value;
            var txtPrice3 = document.getElementById(str + 'txtPrice3' + '_' + rowIndex).value;
            var hdnSales1 = document.getElementById(str + 'hdnSales1' + '_' + rowIndex).value;
            var txtSales1 = document.getElementById(str + 'txtSales1' + '_' + rowIndex).value;
            var hdnSales2 = document.getElementById(str + 'hdnSales2' + '_' + rowIndex).value;
            var txtSales2 = document.getElementById(str + 'txtSales2' + '_' + rowIndex).value;
            var hdnSales3 = document.getElementById(str + 'hdnSales3' + '_' + rowIndex).value;
            var txtSales3 = document.getElementById(str + 'txtSales3' + '_' + rowIndex).value;
            var hdnReceipts = document.getElementById(str + 'hdnReceipts' + '_' + rowIndex).value;
            var txtReceipts = document.getElementById(str + 'txtReceipts' + '_' + rowIndex).value;
            var hdnReceipts2 = document.getElementById(str + 'hdnReceipts2' + '_' + rowIndex).value;
            var txtReceipts2 = document.getElementById(str + 'txtReceipts2' + '_' + rowIndex).value;
            var hdnReceipts3 = document.getElementById(str + 'hdnReceipts3' + '_' + rowIndex).value;
            var txtReceipts3 = document.getElementById(str + 'txtReceipts3' + '_' + rowIndex).value;
            var hdnRepDolExchRate = document.getElementById(str + 'hdnDolExchRate' + '_' + rowIndex).value;
            var txtRepoDolExchRate = document.getElementById(str + 'txtDolExchRate' + '_' + rowIndex).value;
            var hdnCurrencyCode = document.getElementById(str + 'hdnCurrencyCode' + '_' + rowIndex).value;
            var txtCurrencyCode = document.getElementById(str + 'txtCurrencyCode' + '_' + rowIndex).value;
            var hdnWhtMultiplier = document.getElementById(str + 'hdnWhtTax' + '_' + rowIndex).value;
            var txtWhtMultiplier = document.getElementById(str + 'txtWhtTax' + '_' + rowIndex).value;
            var hdnTransactionId = document.getElementById(str + 'hdnTransactionId' + '_' + rowIndex).value;
            var txtDestinationCountry = document.getElementById(str + 'txtDestinationCountry' + '_' + rowIndex).value;
            var hdnDestinationCountry = document.getElementById(str + 'hdnDestinationCountry' + '_' + rowIndex).value;
            var hdnCompanyCode = document.getElementById(str + 'hdnCompanyCodeGrid' + '_' + rowIndex).value;
            var ddlCompanyCode = document.getElementById(str + 'ddlCompanyCodeGrid' + '_' + rowIndex).value;
            var hdnIsModified = document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).value;

            if (hdnReceivedDate != txtReceivedDate || hdnReportedDate != txtReportedDate
                || hdnCatNo != txtCatNo || hdnSalesType != txtSalesType || hdnPrice1 != txtPrice1
                || hdnPrice2 != txtPrice2 || hdnPrice3 != txtPrice3 || hdnReceipts != txtReceipts || hdnReceipts2 != txtReceipts2
                || hdnReceipts3 != txtReceipts3 || hdnSales1 != txtSales1 || hdnSales2 != txtSales2 || hdnSales3 != txtSales3
                || hdnRepDolExchRate != txtRepoDolExchRate || hdnCurrencyCode != txtCurrencyCode || hdnWhtMultiplier != txtWhtMultiplier || txtDestinationCountry != hdnDestinationCountry
                || hdnCompanyCode != ddlCompanyCode) {
                document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).innerText = "Y";
            }

            else {
                document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).innerText = "N";
            }
        }

        function ConfirmDelete(row) {
            //debugger;
            var rowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            var str = "ContentPlaceHolderBody_gvTransactions_";
            if (document.getElementById(str + 'chkDelete' + '_' + rowIndex).checked == true) {
                //JIRA-908 changes by Ravi on 14/02/2019 -- Start
                document.getElementById("<%= hdnDeleteRowIndex.ClientID %>").value = rowIndex;
                var popup = $find('<%= mpeConfirmDelete.ClientID %>');
                if (popup != null) {
                    popup.show();
                }
                return false;
                //JIRA-908 changes by Ravi on 14/02/2019 -- End
            }
        }


        function ConfirmDeleteYes() {
            rowIndex = document.getElementById("<%= hdnDeleteRowIndex.ClientID %>").value;
            var str = "ContentPlaceHolderBody_gvTransactions_";
            document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).value = "D";
            var popup = $find('<%= mpeConfirmDelete.ClientID %>');
            if (popup != null) {
                popup.hide();
            }
            return false;

        }
        function ConfirmDeleteNo() {
            rowIndex = document.getElementById("<%= hdnDeleteRowIndex.ClientID %>").value;
            var str = "ContentPlaceHolderBody_gvTransactions_";
            document.getElementById(str + 'chkDelete' + '_' + rowIndex).checked = false;
            return false;

        }

        function ValidateReset(button) {
            if (document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value == "Y") {
                return true;
            }
            document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "N";

            if (IsGridDataChanged()) {
                window.onbeforeunload = null;
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
                eval(this.href);
                return false;
            }
        }


        function OnFuzzySearchKeyDown(sender, name) {
            var str = "ContentPlaceHolderBody_gvTransactions_";
            if ((event.keyCode == 13)) {
                selectedRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
                //Enter key can be used to select the dropdown list item or to pop up the complete list
                //to know this, check if list item is selected or not
                if (name == 'SalesType') {
                    var aceSalesType = $find(str + 'aceSalesType' + '_' + selectedRowIndex);
                    if (aceSalesType._selectIndex == -1) {
                        txtSalesType = document.getElementById(str + 'txtSalesType' + '_' + selectedRowIndex).value;
                        document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtSalesType;
                        document.getElementById("<%=hdnFuzzySearchField.ClientID %>").innerText = 'SalesType';
                        document.getElementById("<%=hdnGridFuzzySearchRowId.ClientID %>").innerText = selectedRowIndex;
                        document.getElementById('<%=btnFuzzySearch.ClientID%>').click();
                    }
                }

            }
        }

        function UndoChanges(gridRow) {
            var str = "ContentPlaceHolderBody_gvTransactions_";
            var rowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
            document.getElementById(str + 'txtReceivedDate' + '_' + rowIndex).value = document.getElementById(str + 'hdnReceivedDate' + '_' + rowIndex).value;
            document.getElementById(str + 'txtReportedDate' + '_' + rowIndex).value = document.getElementById(str + 'hdnReportedDate' + '_' + rowIndex).value;
            document.getElementById(str + 'txtCatNo' + '_' + rowIndex).innerText = document.getElementById(str + 'hdnCatNo' + '_' + rowIndex).value;
            document.getElementById(str + 'txtSalesType' + '_' + rowIndex).innerText = document.getElementById(str + 'hdnSalesType' + '_' + rowIndex).value;
            document.getElementById(str + 'txtPrice1' + '_' + rowIndex).innerText = document.getElementById(str + 'hdnPrice1' + '_' + rowIndex).value
            document.getElementById(str + 'txtPrice2' + '_' + rowIndex).innerText = document.getElementById(str + 'hdnPrice2' + '_' + rowIndex).value;
            document.getElementById(str + 'txtPrice3' + '_' + rowIndex).innerText = document.getElementById(str + 'hdnPrice3' + '_' + rowIndex).value;
            document.getElementById(str + 'txtSales1' + '_' + rowIndex).innerText = document.getElementById(str + 'hdnSales1' + '_' + rowIndex).value;
            document.getElementById(str + 'txtSales2' + '_' + rowIndex).innerText = document.getElementById(str + 'hdnSales2' + '_' + rowIndex).value;
            document.getElementById(str + 'txtSales3' + '_' + rowIndex).innerText = document.getElementById(str + 'hdnSales3' + '_' + rowIndex).value;
            document.getElementById(str + 'txtReceipts' + '_' + rowIndex).innerText = document.getElementById(str + 'hdnReceipts' + '_' + rowIndex).value;
            document.getElementById(str + 'txtReceipts2' + '_' + rowIndex).innerText = document.getElementById(str + 'hdnReceipts2' + '_' + rowIndex).value;
            document.getElementById(str + 'txtReceipts3' + '_' + rowIndex).innerText = document.getElementById(str + 'hdnReceipts3' + '_' + rowIndex).value;
            document.getElementById(str + 'txtDolExchRate' + '_' + rowIndex).innerText = document.getElementById(str + 'hdnDolExchRate' + '_' + rowIndex).value;
            document.getElementById(str + 'txtCurrencyCode' + '_' + rowIndex).innerText = document.getElementById(str + 'hdnCurrencyCode' + '_' + rowIndex).value;
            document.getElementById(str + 'txtWhtTax' + '_' + rowIndex).innerText = document.getElementById(str + 'hdnWhtTax' + '_' + rowIndex).value;
            document.getElementById(str + 'txtDestinationCountry' + '_' + rowIndex).innerText = document.getElementById(str + 'hdnDestinationCountry' + '_' + rowIndex).value;
            document.getElementById(str + 'ddlCompanyCodeGrid' + '_' + rowIndex).value = document.getElementById(str + 'hdnCompanyCodeGrid' + '_' + rowIndex).value;
            document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).value = "N";
            document.getElementById(str + 'chkDelete' + '_' + rowIndex).checked = false;
            Page_ClientValidate("valSave");
            return false;
        }

        function IsGridDataChanged() {
            //debugger;
            var gridId;
            var gridType;
            gridId = "ContentPlaceHolderBody_gvTransactions_";
            gridType = document.getElementById("<%= gvTransactions.ClientID %>");
            if (gridType != null) {
                var gvRows = gridType.rows;
                var isModified;
                var isGridDataChanged = "N";
                for (var i = 0; i < gvRows.length; i++) {
                    if (document.getElementById(gridId + 'hdnIsModified' + '_' + i) != null) {
                        isModified = document.getElementById(gridId + 'hdnIsModified' + '_' + i).value;
                        if (isModified == "Y" || isModified == "D") {
                            isGridDataChanged = "Y";
                            break;
                        }
                    }
                }

                if (isGridDataChanged == "Y") {
                    document.getElementById("<%=hdnGridDataChanged.ClientID %>").value = "Y";
                    return true;
                }
                else {
                    document.getElementById("<%=hdnGridDataChanged.ClientID %>").value = "N";
                    return false;
                }
            }
        }
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
}

//Territory Add row fuzzy search -- Start

function territoryAddRowListPopulating() {
    txtTerritoryAddRow = document.getElementById("<%= txtAddTransTerritory.ClientID %>");
    txtTerritoryAddRow.style.backgroundImage = 'url(Images/textbox_loader.gif)';
    txtTerritoryAddRow.style.backgroundRepeat = 'no-repeat';
    txtTerritoryAddRow.style.backgroundPosition = 'right';
}

function territoryAddRowListPopulated() {
    txtTerritoryAddRow = document.getElementById("<%= txtAddTransTerritory.ClientID %>");
    txtTerritoryAddRow.style.backgroundImage = 'none';
}

function territoryAddRowListHidden() {
    txtTerritoryAddRow = document.getElementById("<%= txtAddTransTerritory.ClientID %>");
    txtTerritoryAddRow.style.backgroundImage = 'none';
}

function territoryAddRowListItemSelected(sender, args) {
    var roySrchVal = args.get_value();
    if (roySrchVal == 'No results found') {
        txtTerritoryAddRow = document.getElementById("<%= txtAddTransTerritory.ClientID %>");
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
            txtTerritoryAddRow = document.getElementById("<%= txtAddTransTerritory.ClientID %>").value;
            document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtTerritoryAddRow;
            document.getElementById('<%=btnFuzzyTerritoryListPopupAddTrans.ClientID%>').click();
        }
    }
}


//Validate if the field value is a valid one from fuzzy search list
function ValTerritoryAddRow(sender, args) {
    txtTerritoryAddRow = document.getElementById("<%=txtAddTransTerritory.ClientID %>");
    if (txtTerritoryAddRow.value == "") {
        args.IsValid = true;
        txtTerritoryAddRow.style["width"] = '86%';
    }
    else if (txtTerritoryAddRow.value == "No results found") {
        args.IsValid = true;
        txtTerritoryAddRow.value = "";
        txtTerritoryAddRow.style["width"] = '86%';
    }
    else if (txtTerritoryAddRow.value != "" && txtTerritoryAddRow.value.indexOf('-') == -1) {
        args.IsValid = false;
        //adjust width of the textbox to display error
        fieldWidth = txtTerritoryAddRow.offsetWidth;
        txtTerritoryAddRow.style["width"] = '86%';
    }
    else if (args.IsValid == true) {
        txtTerritoryAddRow.style["width"] = '86%';
    }
}

function salesTypeAddRowListPopulating() {
    salesTypeAddRow = document.getElementById("<%= txtAddTransSalesType.ClientID %>");
    salesTypeAddRow.style.backgroundImage = 'url(Images/textbox_loader.gif)';
    salesTypeAddRow.style.backgroundRepeat = 'no-repeat';
    salesTypeAddRow.style.backgroundPosition = 'right';
}

function salesTypeAddRowListPopulated() {
    salesTypeAddRow = document.getElementById("<%= txtAddTransSalesType.ClientID %>");
    salesTypeAddRow.style.backgroundImage = 'none';
}

function salesTypeAddRowListHidden() {
    salesTypeAddRow = document.getElementById("<%= txtAddTransSalesType.ClientID %>");
    salesTypeAddRow.style.backgroundImage = 'none';

}

function salesTypeAddRowListItemSelected(sender, args) {
    var roySrchVal = args.get_value();
    if (roySrchVal == 'No results found') {
        salesTypeAddRow = document.getElementById("<%= txtAddTransSalesType.ClientID %>");
        salesTypeAddRow.value = "";
    }
}

//Pop up fuzzy search list       
function OntxtSalesTypeAddRowKeyDown() {
    if ((event.keyCode == 13)) {
        //Enter key can be used to select the dropdown list item or to pop up the complete list
        //to know this, check if list item is selected or not
        var aceSalesTypeAddRow = $find('ContentPlaceHolderBody_' + 'aceSalesTypeAddRow');
        if (aceSalesTypeAddRow._selectIndex == -1) {
            txtSalesTypeAddRow = document.getElementById("<%= txtAddTransSalesType.ClientID %>").value;
            document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtSalesTypeAddRow;
            document.getElementById('<%=btnFuzzySalesTypeListPopupAddTrans.ClientID%>').click();
        }
    }
}
//Validate if the field value is a valid one from fuzzy search list
function ValSalesAddRow(sender, args) {
    txtSalesAddRow = document.getElementById("<%=txtAddTransSalesType.ClientID %>");
        if (txtSalesAddRow.value == "") {
            args.IsValid = true;
            txtSalesAddRow.style["width"] = '86%';
        }
        else if (txtSalesAddRow.value == "No results found") {
            args.IsValid = true;
            txtSalesAddRow.value = "";
            txtSalesAddRow.style["width"] = '86%';
        }
        else if (txtSalesAddRow.value != "" && txtSalesAddRow.value.indexOf('-') == -1) {
            args.IsValid = false;
            //adjust width of the textbox to display error
            fieldWidth = txtSalesAddRow.offsetWidth;
            txtSalesAddRow.style["width"] = '86%';
        }
        else if (args.IsValid == true) {
            txtSalesAddRow.style["width"] = '86%';
        }
    }

        function OnSellerKeyDown() {
            var txtSeller = document.getElementById("<%= txtSellerSearch.ClientID %>").value;
            if ((event.keyCode == 13)) {               
                if (txtSeller == "") {
                    document.getElementById('<%=btnHdnSearch.ClientID%>').click();
                }
                else {
                    if (document.getElementById("<%= hdnIsValidSeller.ClientID %>").value == "Y") {
                        document.getElementById('<%=btnHdnSearch.ClientID%>').click();
                    }
                    else {
                        return false;
                    }
                }
            }
        }

        function OnSalesTypeKeyDown() {
            var txtSalesType = document.getElementById("<%= txtSalesTypeSearch.ClientID %>").value;
            if ((event.keyCode == 13)) {               
                if (txtSalesType == "") {
                    document.getElementById('<%=btnHdnSearch.ClientID%>').click();
                }
                else {
                    if (document.getElementById("<%= hdnIsValidSalesType.ClientID %>").value == "Y") {
                        document.getElementById('<%=btnHdnSearch.ClientID%>').click();
                    }
                    else {
                        return false;
                    }
                }
            }
        }

        function OnReceivedDateKeyDown() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnHdnSearch.ClientID%>').click();
    }
}

function OnReportedDateKeyDown() {
    if ((event.keyCode == 13)) {
        document.getElementById('<%=btnHdnSearch.ClientID%>').click();
    }
}

function OnCompanyKeyDown() {
    if ((event.keyCode == 13)) {
        document.getElementById('<%=btnHdnSearch.ClientID%>').click();
    }
}

function CloseAddTransPopup() {
    var popup = $find('<%= mpeAddTransactionDetailsPopup.ClientID %>');
    if (popup != null) {
        popup.hide();
    }

    return false;
}

        function ValidateUnsavedData(button) {            
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
                    <td colspan="9">
                        <asp:Panel ID="PnlScreenName" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    TRANSACTION MAINTENANCE 
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="1%"></td>
                    <td class="identifierLable_large_bold" width="7%">Seller</td>
                    <td width="20%">
                        <asp:TextBox ID="txtSellerSearch" runat="server" CssClass="textbox_FuzzySearch" Width="95%" TabIndex="101" OnKeyDown="OnSellerKeyDown();"  Onfocus="if (!ValidateUnsavedData('Search')) { return false;};"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="aceSeller" runat="server"
                            ServiceMethod="FuzzySearchSellerGroupListTypeC"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtSellerSearch"
                            FirstRowSelected="true"
                            OnClientPopulating="sellerListPopulating"
                            OnClientPopulated="sellerListPopulated"
                            OnClientHidden="sellerListHidden"
                            OnClientItemSelected="sellerListItemSelected"
                            CompletionListElementID="pnlSellerFuzzySearch" />
                        <asp:Panel ID="pnlSellerFuzzySearch" runat="server" CssClass="identifierLable" />
                    </td>
                    <td align="left">
                        <asp:ImageButton ID="fuzzySearchSeller" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                            OnClick="fuzzySearchSeller_Click" TabIndex="102" ToolTip="Search Seller" CssClass="FuzzySearch_Button" />

                    </td>
                    <td class="identifierLable_large_bold" width="7%">SalesType</td>
                    <td width="20%">
                        <asp:TextBox ID="txtSalesTypeSearch" runat="server" CssClass="textbox_FuzzySearch" Width="95%" TabIndex="103" OnKeyDown="OnSalesTypeKeyDown();"  Onfocus="if (!ValidateUnsavedData('Search')) { return false;};"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="aceSalesType" runat="server"
                            ServiceMethod="FuzzySearchPriceGroupListTypeC"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtSalesTypeSearch"
                            FirstRowSelected="true"
                            OnClientPopulating="salesTypeListPopulating"
                            OnClientPopulated="salesTypeListPopulated"
                            OnClientHidden="salesTypeListHidden"
                            OnClientItemSelected="salesTypeListItemSelected"
                            CompletionListElementID="pnlSalesTypeFuzzySearch" />
                        <asp:Panel ID="pnlSalesTypeFuzzySearch" runat="server" CssClass="identifierLable" />
                    </td>
                    <td align="left" width="5%">
                        <asp:ImageButton ID="fuzzySearchSales" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                            OnClick="fuzzySearchSales_Click" TabIndex="104" ToolTip="Search Sales" CssClass="FuzzySearch_Button" />
                    </td>
                    <td width="5%"></td>
                    <td width="12%" align="right">
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="ButtonStyle" TabIndex="109"
                            Width="98%" OnClick="btnSearch_Click" UseSubmitBehavior="false" OnClientClick="if (!ValidateTransaction('Search')) { return false;};" /></td>

                </tr>
                <tr>
                    <td width="1%"></td>
                    <td class="identifierLable_large_bold" width="7%">Received Date</td>
                    <td>
                        <asp:TextBox ID="txtReceivedDateSearch" runat="server" Width="65" ToolTip="MM/YYYY" CssClass="textboxStyle"
                            TabIndex="105" OnKeyDown="OnReceivedDateKeyDown();"  Onfocus="if (!ValidateUnsavedData('Search')) { return false;};"></asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmetxtReceivedDate" runat="server" TargetControlID="txtReceivedDateSearch"
                            WatermarkText="MM/YYYY" WatermarkCssClass="waterMarkText">
                        </ajaxToolkit:TextBoxWatermarkExtender>
                        <ajaxToolkit:MaskedEditExtender ID="mtetxtReceivedDate" runat="server"
                            TargetControlID="txtReceivedDateSearch" Mask="99/9999" AcceptNegative="None" ClearMaskOnLostFocus="false" />
                        <asp:CustomValidator ID="valReceivedDate" runat="server" ValidationGroup="valGrpSearch" CssClass="requiredFieldValidator" Display="Dynamic"
                            OnServerValidate="valReceivedDate_ServerValidate" ToolTip="Please enter a valid date in MM/YYYY format"
                            ErrorMessage="*"></asp:CustomValidator>
                    </td>
                    <td width="5%"></td>
                    <td class="identifierLable_large_bold" width="7%">Reported Date</td>
                    <td>
                        <asp:TextBox ID="txtReportedDateSearch" runat="server" Width="65" ToolTip="MM/YYYY" CssClass="textboxStyle"
                            TabIndex="106" OnKeyDown="OnReportedDateKeyDown();"  Onfocus="if (!ValidateUnsavedData('Search')) { return false;};"></asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtReportedDateSearch"
                            WatermarkText="MM/YYYY" WatermarkCssClass="waterMarkText">
                        </ajaxToolkit:TextBoxWatermarkExtender>
                        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender1" runat="server"
                            TargetControlID="txtReportedDateSearch" Mask="99/9999" AcceptNegative="None" ClearMaskOnLostFocus="false" />

                        <asp:CustomValidator ID="valReportedDate" runat="server" ValidationGroup="valGrpSearch" CssClass="requiredFieldValidator" Display="Dynamic"
                            OnServerValidate="valReportedDate_ServerValidate" ToolTip="Please enter a valid date in MM/YYYY format"
                            ErrorMessage="*"></asp:CustomValidator>
                    </td>
                    <td></td>
                    <td width="5%"></td>
                    <td width="12%" align="right">
                        <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="ButtonStyle" TabIndex="110" OnClientClick="return ValidateReset('Reset');"
                            Width="98%" OnClick="btnReset_Click" />
                    </td>

                </tr>
                <tr>
                    <td width="1%"></td>
                    <td class="identifierLable_large_bold" width="7%">Catalogue No</td>
                    <td>
                        <asp:TextBox ID="txtCatalogueNumber" runat="server" Width="50%" OnKeyDown="OnCatNoKeyDown();" CssClass="textbox_FuzzySearch" TabIndex="107"  Onfocus="if (!ValidateUnsavedData('Search')) { return false;};"></asp:TextBox>
                    </td>
                    <td width="5%"></td>
                    <td width="7%" class="identifierLable_large_bold">Company</td>
                    <td width="20%">
                        <asp:DropDownList ID="ddlCompany" runat="server" Width="50%" CssClass="ddlStyle" AutoPostBack="false" TabIndex="108" OnKeyDown="OnCompanyKeyDown();"  Onfocus="if (!ValidateUnsavedData('Search')) { return false;};">
                        </asp:DropDownList>
                    </td>
                    <td></td>
                    <td width="5%"></td>
                    <td align="right">
                        <asp:Button ID="btnSaveChanges" runat="server" Text="Save Changes" CssClass="ButtonStyle" TabIndex="111"
                            Width="98%" UseSubmitBehavior="false" OnClick="btnSaveChanges_Click" ValidationGroup="valSave" />
                    </td>

                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td width="5%"></td>
                    <td align="right">
                        <asp:Button ID="btnAddTransaction" runat="server" Text="Add Transaction" CssClass="ButtonStyle" TabIndex="112"  OnClientClick="if (!ValidateUnsavedData('AddTransaction')) { return false;};"
                            Width="98%" UseSubmitBehavior="false" CausesValidation="false" OnClick="btnAddTransaction_Click" OnKeyDown="OnTabPress()" />
                    </td>
                </tr>
                <tr>
                    <td id="tdTrans" runat="server" colspan="7">
                        <asp:Panel ID="PnlTransactions" runat="server" ScrollBars="Auto" Width="100%">
                            <asp:GridView ID="gvTransactions" runat="server" AutoGenerateColumns="False" Width="98.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                EmptyDataText="No data found." ShowHeader="False" OnRowDataBound="gvTransactions_RowDataBound">
                                <Columns>
                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                        <ItemTemplate>
                                            <table width="98.5%" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td width="10%"></td>
                                                    <td width="15%"></td>
                                                    <td width="1%"></td>
                                                    <td width="7%"></td>
                                                    <td width="15%"></td>
                                                    <td width="1%"></td>
                                                    <td width="5%"></td>
                                                    <td width="6%"></td>
                                                    <td width="1%"></td>
                                                    <td width="5%"></td>
                                                    <td width="7%"></td>
                                                    <td width="1%"></td>
                                                    <td width="10%"></td>
                                                    <td width="10%"></td>
                                                    <td width="1%"></td>
                                                    <td width="5%"></td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">Received Date                                                                           
                                                    </td>
                                                    <td align="left">
                                                        <asp:HiddenField ID="hdnTransactionId" runat="server" Value='<%# Bind("transaction_id") %>' />
                                                        <asp:HiddenField ID="hdnReceivedDate" runat="server" Value='<%# Bind("recdate") %>' />
                                                        <asp:TextBox ID="txtReceivedDate" runat="server" Text='<%# Bind("recdate") %>' onchange="OnGridDataChanges(this,'ReceivedDate');" CssClass="textbox_FuzzySearch" Width="80%"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ID="rfvReceivedDateEdit" ControlToValidate="txtReceivedDate" ValidationGroup="valSave"
                                                            Text="*" CssClass="requiredFieldValidator" InitialValue="" ToolTip="Please enter a valid date in MM/DD/YYYY format" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="regDateReceivedEdit" runat="server" ControlToValidate="txtReceivedDate"
                                                            ValidationExpression="^(?=\d{2}([\/])\d{2}\1\d{4}$)(?:0[1-9]|1\d|[2][0-8]|29(?!.02.(?!(?!(?:[02468][1-35-79]|[13579][0-13-57-9])00)\d{2}(?:[02468][048]|[13579][26])))|30(?!.02)|31(?=.(?:0[13578]|10|12))).(?:0[1-9]|1[012]).(19|20)\d{2}$"
                                                            ErrorMessage="*" ToolTip="Please enter valid date in DD/MM/YYYY format" ValidationGroup="valSave" CssClass="requiredFieldValidator"
                                                            Display="Dynamic" />
                                                        <ajaxToolkit:MaskedEditExtender ID="maskEditReceivedDate" runat="server"
                                                            TargetControlID="txtReceivedDate" Mask="99/99/9999" AcceptNegative="None"
                                                            ClearMaskOnLostFocus="true" MaskType="Date" />
                                                    </td>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold" align="left">Sales Type
                                                    </td>
                                                    <td align="left">
                                                        <asp:HiddenField ID="hdnSalesType" runat="server" Value='<%# Bind("price_group") %>' />
                                                        <asp:TextBox ID="txtSalesType" Width="80%" onkeydown="OnFuzzySearchKeyDown(this,'SalesType');" onchange="OnGridDataChanges(this,'SalesType');" runat="server" Text='<%# Bind("price_group") %>' CssClass="textbox_FuzzySearch"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator6" ControlToValidate="txtSalesType" ValidationGroup="valSave"
                                                            Text="*" CssClass="requiredFieldValidator" InitialValue="" ToolTip="Please select valid Sales Type" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:CustomValidator ID="valSalesEdit" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator" Display="Dynamic"
                                                            OnServerValidate="valSalesEdit_ServerValidate" ErrorMessage="*" ToolTip="Please select valid sales type"></asp:CustomValidator>
                                                        <ajaxToolkit:AutoCompleteExtender ID="aceSalesType" runat="server"
                                                            ServiceMethod="FuzzySearchPriceGroupListTypeC"
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
                                                    </td>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold" align="left">Sales1
                                                    </td>
                                                    <td align="left">
                                                        <asp:HiddenField ID="hdnSales1" runat="server" Value='<%# Bind("sales1") %>' />
                                                        <asp:TextBox ID="txtSales1" runat="server" Text='<%# Bind("sales1") %>' onchange="OnGridDataChanges(this,'Sales1');" CssClass="textbox_FuzzySearch" Width="72%"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ID="rfvtxtSales1" ControlToValidate="txtSales1" ValidationGroup="valSave"
                                                            Text="*" CssClass="requiredFieldValidator" InitialValue="" ToolTip="Please enter Sales1" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="revSales1" runat="server" Text="*" ControlToValidate="txtSales1" ValidationGroup="valSave"
                                                            ValidationExpression="^-?[0-9]*$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                            ToolTip="Please enter only number" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                    </td>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold" align="left">Receipts
                                                    </td>
                                                    <td align="left">
                                                        <asp:HiddenField ID="hdnReceipts" runat="server" Value='<%# Bind("receipts") %>' />
                                                        <asp:TextBox ID="txtReceipts" CssClass="textbox_FuzzySearch" Width="78%" runat="server" Text='<%# Bind("receipts") %>' onchange="OnGridDataChanges(this,'Receipts');"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ID="rfvtxtReceipts" ControlToValidate="txtReceipts" ValidationGroup="valSave"
                                                            Text="*" CssClass="requiredFieldValidator" InitialValue="" ToolTip="Please enter Receipts" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="revReceipts" runat="server" Text="*" ControlToValidate="txtReceipts" ValidationGroup="valSave"
                                                            ValidationExpression="^-?\d{1,14}(\.\d{1,8})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                            ToolTip="Please enter only number upto 8 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                    </td>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold" align="left">Dollar Exchange Rate
                                                    </td>
                                                    <td align="left">
                                                        <asp:HiddenField ID="hdnDolExchRate" runat="server" Value='<%# Bind("repdol_exchrate") %>' />
                                                        <asp:TextBox ID="txtDolExchRate" CssClass="textbox_FuzzySearch" Width="80%" runat="server" Text='<%# Bind("repdol_exchrate") %>' onchange="OnGridDataChanges(this,'ExchangeRate');"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ID="rfvtxtDolExchRate" ControlToValidate="txtDolExchRate" ValidationGroup="valSave"
                                                            Text="*" CssClass="requiredFieldValidator" InitialValue="" ToolTip="Please enter Dollar Exchange Rate" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="revDolExchRate" runat="server" Text="*" ControlToValidate="txtDolExchRate" ValidationGroup="valSave"
                                                            ValidationExpression="^\d{1,14}(\.\d{1,6})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                            ToolTip="Please enter only number upto 6 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                    </td>
                                                    <td></td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">Reported Date                                                                           
                                                    </td>
                                                    <td align="left">
                                                        <asp:HiddenField ID="hdnReportedDate" runat="server" Value='<%# Bind("repdate") %>' />
                                                        <asp:TextBox ID="txtReportedDate" runat="server" Text='<%# Bind("repdate") %>' onchange="OnGridDataChanges(this,'ReportedDate');" CssClass="textbox_FuzzySearch" Width="80%"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ID="rfvReportedDateEdit" ControlToValidate="txtReportedDate" ValidationGroup="valSave"
                                                            Text="*" CssClass="requiredFieldValidator" InitialValue="" ToolTip="Please enter a valid date in MM/DD/YYYY format" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="regDateReportedEdit" runat="server" ControlToValidate="txtReportedDate"
                                                            ValidationExpression="^(?=\d{2}([\/])\d{2}\1\d{4}$)(?:0[1-9]|1\d|[2][0-8]|29(?!.02.(?!(?!(?:[02468][1-35-79]|[13579][0-13-57-9])00)\d{2}(?:[02468][048]|[13579][26])))|30(?!.02)|31(?=.(?:0[13578]|10|12))).(?:0[1-9]|1[012]).(19|20)\d{2}$"
                                                            ErrorMessage="*" ToolTip="Please enter valid date in DD/MM/YYYY format" ValidationGroup="valSave" CssClass="requiredFieldValidator"
                                                            Display="Dynamic" />
                                                        <ajaxToolkit:MaskedEditExtender ID="maskEditReportedDate" runat="server"
                                                            TargetControlID="txtReportedDate" Mask="99/99/9999" AcceptNegative="None"
                                                            ClearMaskOnLostFocus="true" MaskType="Date" />
                                                    </td>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold" align="left">Price1
                                                    </td>
                                                    <td align="left">
                                                        <asp:HiddenField ID="hdnPrice1" runat="server" Value='<%# Bind("price1") %>' />
                                                        <asp:TextBox ID="txtPrice1" Width="80%" Style="text-align: right" runat="server" Text='<%# Bind("price1") %>' onchange="OnGridDataChanges(this,'Price1');" CssClass="textbox_FuzzySearch"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ID="rfvtxtPrice1" ControlToValidate="txtPrice1" ValidationGroup="valSave"
                                                            Text="*" CssClass="requiredFieldValidator" InitialValue="" ToolTip="Please enter Price1" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="revPrice1" runat="server" Text="*" ControlToValidate="txtPrice1" ValidationGroup="valSave"
                                                            ValidationExpression="^-?\d{1,14}(\.\d{1,8})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                            ToolTip="Please enter only number upto 8 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                    </td>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold" align="left">Sales2
                                                    </td>
                                                    <td align="left">
                                                        <asp:HiddenField ID="hdnSales2" runat="server" Value='<%# Bind("sales2") %>' />
                                                        <asp:TextBox ID="txtSales2" CssClass="textbox_FuzzySearch" Width="72%" runat="server" Text='<%# Bind("sales2") %>' onchange="OnGridDataChanges(this,'Sales2');"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ID="rfvtxtSales2" ControlToValidate="txtSales2" ValidationGroup="valSave"
                                                            Text="*" CssClass="requiredFieldValidator" InitialValue="" ToolTip="Please enter Sales2" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="revSales2" runat="server" Text="*" ControlToValidate="txtSales2" ValidationGroup="valSave"
                                                            ValidationExpression="^-?[0-9]*$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                            ToolTip="Please enter only number" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                    </td>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold" align="left">Receipts2
                                                    </td>
                                                    <td align="left">
                                                        <asp:HiddenField ID="hdnReceipts2" runat="server" Value='<%# Bind("receipts2") %>' />
                                                        <asp:TextBox ID="txtReceipts2" CssClass="textbox_FuzzySearch" Width="78%" runat="server" Text='<%# Bind("receipts2") %>' onchange="OnGridDataChanges(this,'Receipts2');"></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="revReceipts2" runat="server" Text="*" ControlToValidate="txtReceipts2" ValidationGroup="valSave"
                                                            ValidationExpression="^-?\d{1,14}(\.\d{1,8})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                            ToolTip="Please enter only number upto 8 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                    </td>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold" align="left">Company Code
                                                    </td>
                                                    <td align="left">
                                                        <asp:HiddenField ID="hdnCompanyCodeGrid" runat="server" Value='<%# Bind("company_code") %>' />
                                                        <asp:DropDownList ID="ddlCompanyCodeGrid" runat="server" Width="84%" CssClass="ddlStyle" onchange="OnGridDataChanges(this,'CompanyCode');">
                                                        </asp:DropDownList>
                                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator5" ControlToValidate="ddlCompanyCodeGrid" ValidationGroup="valSave"
                                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please select Company Code" Display="Dynamic" InitialValue="-">
                                                        </asp:RequiredFieldValidator>
                                                    </td>
                                                    <td></td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">Catalogue Number
                                                    </td>
                                                    <td align="left">
                                                        <asp:HiddenField ID="hdnCatNo" runat="server" Value='<%# Bind("catno") %>' />
                                                        <asp:TextBox ID="txtCatNo" runat="server" MaxLength="30" Text='<%# Bind("catno") %>' onchange="OnGridDataChanges(this,'CatNo');" CssClass="textbox_FuzzySearch" Width="80%"></asp:TextBox>
                                                        <asp:CustomValidator ID="valCatNoEdit" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator" Display="Dynamic"
                                                            OnServerValidate="valCatNoEdit_ServerValidate" ErrorMessage="*" ToolTip="Please enter valid catno"></asp:CustomValidator>
                                                    </td>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold" align="left">Price2
                                                    </td>
                                                    <td align="left">
                                                        <asp:HiddenField ID="hdnPrice2" runat="server" Value='<%# Bind("price2") %>' />
                                                        <asp:TextBox ID="txtPrice2" Width="80%" Style="text-align: right" runat="server" Text='<%# Bind("price2") %>' onchange="OnGridDataChanges(this,'Price2');" CssClass="textbox_FuzzySearch"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ID="rfvtxtPrice2" ControlToValidate="txtPrice2" ValidationGroup="valSave"
                                                            Text="*" CssClass="requiredFieldValidator" InitialValue="" ToolTip="Please enter Price2" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="revPrice2" runat="server" Text="*" ControlToValidate="txtPrice2" ValidationGroup="valSave"
                                                            ValidationExpression="^-?\d{1,14}(\.\d{1,8})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                            ToolTip="Please enter only number upto 8 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                    </td>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold" align="left">Sales3
                                                    </td>
                                                    <td align="left">
                                                        <asp:HiddenField ID="hdnSales3" runat="server" Value='<%# Bind("sales3") %>' />
                                                        <asp:TextBox ID="txtSales3" CssClass="textbox_FuzzySearch" Width="72%" runat="server" Text='<%# Bind("sales3") %>' onchange="OnGridDataChanges(this,'Sales3');"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ID="rfvtxtSales3" ControlToValidate="txtSales3" ValidationGroup="valSave"
                                                            Text="*" CssClass="requiredFieldValidator" InitialValue="" ToolTip="Please enter Sales3" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="revSales3" runat="server" Text="*" ControlToValidate="txtSales3" ValidationGroup="valSave"
                                                            ValidationExpression="^-?[0-9]*$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                            ToolTip="Please enter only number" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                    </td>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold" align="left">Receipts3
                                                    </td>
                                                    <td align="left">
                                                        <asp:HiddenField ID="hdnReceipts3" runat="server" Value='<%# Bind("receipts3") %>' />
                                                        <asp:TextBox ID="txtReceipts3" CssClass="textbox_FuzzySearch" Width="78%" runat="server" Text='<%# Bind("receipts3") %>' onchange="OnGridDataChanges(this,'Receipts3');"></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="revReceipts3" runat="server" Text="*" ControlToValidate="txtReceipts3" ValidationGroup="valSave"
                                                            ValidationExpression="^-?\d{1,14}(\.\d{1,8})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                            ToolTip="Please enter only number upto 8 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                    </td>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold" align="left">Currency Code
                                                    </td>
                                                    <td align="left">
                                                        <asp:HiddenField ID="hdnCurrencyCode" runat="server" Value='<%# Bind("currency_code") %>' />
                                                        <asp:TextBox ID="txtCurrencyCode" CssClass="textbox_FuzzySearch" Width="80%" MaxLength="3" runat="server" Text='<%# Bind("currency_code") %>' Style="text-transform: uppercase;" onchange="OnGridDataChanges(this,'CurrencyCode');"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ID="rfvtxtCurrencyCode" ControlToValidate="txtCurrencyCode" ValidationGroup="valSave"
                                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Currency Code" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:CustomValidator ID="valCurrencyEdit" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator" Display="Dynamic"
                                                            OnServerValidate="valCurrencyEdit_ServerValidate" ErrorMessage="*" ToolTip="Please enter valid currency code"></asp:CustomValidator>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">Territory
                                                    </td>
                                                    <td align="left">
                                                        <asp:HiddenField ID="hdnSeller" runat="server" Value='<%# Bind("seller_group") %>' />
                                                        <asp:Label ID="lblSeller" CssClass="textbox_FuzzySearch" Width="80%" runat="server" Text='<%# Bind("seller_group") %>' onchange="OnGridDataChanges(this,'Seller');"></asp:Label>
                                                    </td>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold" align="left">Price3
                                                    </td>
                                                    <td align="left">
                                                        <asp:HiddenField ID="hdnPrice3" runat="server" Value='<%# Bind("price3") %>' />
                                                        <asp:TextBox ID="txtPrice3" Width="80%" Style="text-align: right" runat="server" Text='<%# Bind("price3") %>' onchange="OnGridDataChanges(this,'Price3');" CssClass="textbox_FuzzySearch"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ID="rfvtxtPrice3" ControlToValidate="txtPrice3" ValidationGroup="valSave"
                                                            Text="*" CssClass="requiredFieldValidator" InitialValue="" ToolTip="Please enter Price3" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="revPrice3" runat="server" Text="*" ControlToValidate="txtPrice3" ValidationGroup="valSave"
                                                            ValidationExpression="^-?\d{1,14}(\.\d{1,8})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                            ToolTip="Please enter only number upto 8 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                        <asp:HiddenField ID="hdnIsModified" runat="server" Value="N" />
                                                    </td>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold" align="left">Destination Country 
                                                    </td>
                                                    <td align="left">
                                                        <asp:HiddenField ID="hdnDestinationCountry" runat="server" Value='<%# Bind("destination_country_code") %>' />
                                                        <asp:TextBox ID="txtDestinationCountry" runat="server" Width="72%" CssClass="textbox_FuzzySearch" Text='<%# Bind("destination_country_code") %>' onchange="OnGridDataChanges(this,'Country');"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ID="rfvtxtDestinationCountry" ControlToValidate="txtDestinationCountry" ValidationGroup="valSave"
                                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Destination Country" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:CustomValidator ID="CustomValidator3" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator" Display="Dynamic"
                                                            OnServerValidate="valCountryEdit_ServerValidate" ErrorMessage="*" ToolTip="Please enter valid Country code"></asp:CustomValidator>
                                                    </td>
                                                    <td></td>
                                                    <td></td>
                                                    <td></td>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold" align="left">WHT TAX %
                                                    </td>
                                                    <td align="left">
                                                        <asp:HiddenField ID="hdnWhtTax" runat="server" Value='<%# Bind("wht_multiplier") %>' />
                                                        <asp:TextBox ID="txtWhtTax" CssClass="textbox_FuzzySearch" Width="80%" runat="server" Text='<%# Bind("wht_multiplier") %>' onchange="OnGridDataChanges(this,'WhtTax');"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator15" ControlToValidate="txtWhtTax" ValidationGroup="valSave"
                                                            Text="*" CssClass="requiredFieldValidator" InitialValue="" ToolTip="Please enter WHT Tax %" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="revTxtWhtTax" runat="server" Text="*" ControlToValidate="txtWhtTax" ValidationGroup="valSave"
                                                            ValidationExpression="^100(\.00?)?|^\d{0,2}(\.\d{1,2})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                            ToolTip="Please enter only positive number <= 100 upto 2 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                    </td>
                                                    <td class="identifierLable_large_bold" align="right">Delete
                                                    </td>
                                                    <td align="center">
                                                        <asp:CheckBox ID="chkDelete" runat="server" onclick="ConfirmDelete(this);" />
                                                        <asp:ImageButton ID="imgBtnUndo" CausesValidation="false" runat="server" ImageUrl="../Images/cancel_row3.png" OnClientClick="return UndoChanges(this);"
                                                            ToolTip="Cancel" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td align="center" colspan="7">
                        <asp:Repeater ID="rptPager" runat="server">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>'
                                    OnClientClick="if (!ValidateChanges()) { return false;};" ClientIDMode="AutoID" Enabled='<%# Eval("Enabled") %>' OnClick="lnkPage_Click" CssClass="gridPager"> </asp:LinkButton>
                            </ItemTemplate>
                        </asp:Repeater>
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
            <asp:Button ID="dummyFuzzySales" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeFuzzySales" runat="server" PopupControlID="pnlFuzzySales" TargetControlID="dummyFuzzySales"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlFuzzySales" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="identifierLable">Complete Search List
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
                            <asp:ListBox ID="lbFuzzySales" runat="server" Width="95%" CssClass="ListBox"
                                OnSelectedIndexChanged="lbFuzzySales_SelectedIndexChanged" AutoPostBack="True"></asp:ListBox>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Button ID="dummyFuzzySeller" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeFuzzySeller" runat="server" PopupControlID="pnlFuzzySeller" TargetControlID="dummyFuzzySeller"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlFuzzySeller" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="identifierLable">Complete Search List
                                    </td>
                                    <td align="right" style="vertical-align: top;">
                                        <asp:ImageButton ID="ImageButton1" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
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
                            <asp:ListBox ID="lbFuzzySeller" runat="server" Width="95%" CssClass="ListBox"
                                OnSelectedIndexChanged="lbFuzzySeller_SelectedIndexChanged" AutoPostBack="True"></asp:ListBox>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Button ID="btnUnsavedChanges" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeUnsavedChanges" runat="server" PopupControlID="pnlUnsavedChanges" TargetControlID="btnUnsavedChanges"
                CancelControlID="btnExit" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlUnsavedChanges" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td align="right" style="vertical-align: top;">
                            <asp:ImageButton ID="imgClose" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblWarningMessage" runat="server" CssClass="identifierLable"
                                Text="You have made changes that are not saved. Return to Save or Exit without saving."></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnReturn" OnClientClick="javascript: Confirmation('Y');" runat="server" Text="Return" CssClass="ButtonStyle" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnExit" OnClientClick="javascript: Confirmation('N');" runat="server" Text="Exit" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Button ID="dummyFuzzySaleType" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeFuzzySearch" runat="server" PopupControlID="pnlFuzzySearch" TargetControlID="dummyFuzzySaleType"
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
                                        <asp:ImageButton ID="imgFuzzyClose" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
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
                                        <asp:Button ID="btnUnSavedDataExit" runat="server" Text="Exit" CssClass="ButtonStyle" Width="30%" OnClientClick="OnUnSavedDataExit();"
                                            OnClick="btnUnSavedDataExit_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <%--Add New Transaction Details--%>
            <asp:Button ID="dummyAddTransactionDetailsPopup" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeAddTransactionDetailsPopup" runat="server" PopupControlID="pnlAddTransactionDetailsPopup" TargetControlID="dummyAddTransactionDetailsPopup"
                CancelControlID="btnCloseAddTransactionDetailsPopup" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlAddTransactionDetailsPopup" runat="server" align="left" Width="98%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td class="identifierLable" align="center">Add New Transaction
                                    </td>
                                    <td align="right" style="vertical-align: top;">
                                        <asp:ImageButton ID="btnCloseAddTransactionDetailsPopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="identifierLable_large_bold" width="8%">Received Date</td>
                                    <td>
                                        <asp:TextBox ID="txtReceivedDateAddTrans" runat="server" Width="85%" ToolTip="MM/DD/YYYY" CssClass="textboxStyle"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rfvReceivedDateAddTrans" ControlToValidate="txtReceivedDateAddTrans" ValidationGroup="valGrpAddTransactions"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter a valid date in MM/DD/YYYY format" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="regReceivedDateAddTrans" runat="server" ControlToValidate="txtReceivedDateAddTrans"
                                            ValidationExpression="^(?=\d{2}([\/])\d{2}\1\d{4}$)(?:0[1-9]|1\d|[2][0-8]|29(?!.02.(?!(?!(?:[02468][1-35-79]|[13579][0-13-57-9])00)\d{2}(?:[02468][048]|[13579][26])))|30(?!.02)|31(?=.(?:0[13578]|10|12))).(?:0[1-9]|1[012]).(19|20)\d{2}$"
                                            ErrorMessage="*" ToolTip="Please enter valid date in DD/MM/YYYY format" ValidationGroup="valGrpAddTransactions" CssClass="requiredFieldValidator"
                                            Display="Dynamic" />
                                        <ajaxToolkit:MaskedEditExtender ID="maskEditReportedDate" runat="server"
                                            TargetControlID="txtReceivedDateAddTrans" Mask="99/99/9999" AcceptNegative="None"
                                            ClearMaskOnLostFocus="true" MaskType="Date" />
                                    </td>
                                    <td class="identifierLable_large_bold">Sales Type</td>
                                    <td>
                                        <asp:TextBox ID="txtAddTransSalesType" runat="server" Width="85%" CssClass="textboxStyle" onkeydown="OntxtSalesTypeAddRowKeyDown(this);"></asp:TextBox>
                                        <ajaxToolkit:AutoCompleteExtender ID="aceSalesTypeAddRow" runat="server"
                                            ServiceMethod="FuzzySearchPriceGroupListTypeC"
                                            ServicePath="~/Services/FuzzySearch.asmx"
                                            MinimumPrefixLength="1"
                                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                            TargetControlID="txtAddTransSalesType"
                                            FirstRowSelected="true"
                                            OnClientPopulating="salesTypeAddRowListPopulating"
                                            OnClientPopulated="salesTypeAddRowListPopulated"
                                            OnClientHidden="salesTypeAddRowListHidden"
                                            OnClientItemSelected="salesTypeAddRowListItemSelected"
                                            CompletionListElementID="pnlSalesTypeAddTransFuzzySearch" />
                                        <asp:Panel ID="pnlSalesTypeAddTransFuzzySearch" runat="server" CssClass="identifierLable"
                                            Style="bottom: 0px; top: 1000px; position: absolute; height: 200px;" />
                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator6" ControlToValidate="txtAddTransSalesType" ValidationGroup="valGrpAddTransactions"
                                            Text="*" CssClass="requiredFieldValidator" InitialValue="" ToolTip="Please select valid Sales Type" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="valSalesEdit" runat="server" ValidationGroup="valGrpAddTransactions" CssClass="requiredFieldValidator" Display="Dynamic"
                                            ClientValidationFunction="ValSalesAddRow" ErrorMessage="*" ToolTip="Please select valid sales type" ControlToValidate="txtAddTransSalesType"></asp:CustomValidator>
                                    </td>
                                    <td class="identifierLable_large_bold">Sales1</td>
                                    <td>
                                        <asp:TextBox ID="txtAddTransSales1" runat="server" CssClass="textboxStyle" Width="85%"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rfvAddTransSales1" ControlToValidate="txtAddTransSales1" ValidationGroup="valGrpAddTransactions"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Sales1" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revAddTransSales1" runat="server" Text="*" ControlToValidate="txtAddTransSales1" ValidationExpression="^-?[0-9]*$" CssClass="requiredFieldValidator" ForeColor="Red"
                                            ToolTip="Please enter only number" Display="Dynamic"> </asp:RegularExpressionValidator>
                                    </td>
                                    <td class="identifierLable_large_bold" width="5%">Receipts</td>
                                    <td>
                                        <asp:TextBox ID="txtAddTransReceipts" CssClass="textboxStyle" Width="85%" runat="server"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rfvAddTransReceipts" ControlToValidate="txtAddTransReceipts" ValidationGroup="valGrpAddTransactions"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Receipts" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revAddTransReceipts" runat="server" Text="*" ControlToValidate="txtAddTransReceipts"
                                            ValidationExpression="^-?\d{1,14}(\.\d{1,8})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                            ToolTip="Please enter only number upto 8 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                    </td>
                                    <td class="identifierLable_large_bold" width="10%">Dollar Exchange Rate</td>
                                    <td>
                                        <asp:TextBox ID="txtAddTransDolExchRate" CssClass="textboxStyle" Width="85%" runat="server"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rfvAddTransDolExchRate" ControlToValidate="txtAddTransDolExchRate" ValidationGroup="valGrpAddTransactions"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Dollar Exchange Rate" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revAddTransDolExchRate" runat="server" Text="*" ControlToValidate="txtAddTransDolExchRate"
                                            ValidationExpression="^\d{1,14}(\.\d{1,6})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                            ToolTip="Please enter only number upto 6 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                    </td>

                                </tr>
                                <tr>
                                    <td class="identifierLable_large_bold" width="5%">Reported Date</td>
                                    <td>
                                        <asp:TextBox ID="txtAddTransReportedDate" runat="server" Width="85%" ToolTip="MM/DD/YYYY" CssClass="textboxStyle"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="txtAddTransReportedDate" ValidationGroup="valGrpAddTransactions"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter a valid date in MM/DD/YYYY format" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="regAddTransReportedDate" runat="server" ControlToValidate="txtAddTransReportedDate"
                                            ValidationExpression="^(?=\d{2}([\/])\d{2}\1\d{4}$)(?:0[1-9]|1\d|[2][0-8]|29(?!.02.(?!(?!(?:[02468][1-35-79]|[13579][0-13-57-9])00)\d{2}(?:[02468][048]|[13579][26])))|30(?!.02)|31(?=.(?:0[13578]|10|12))).(?:0[1-9]|1[012]).(19|20)\d{2}$"
                                            ErrorMessage="*" ToolTip="Please enter valid date in DD/MM/YYYY format" ValidationGroup="valGrpAddTransactions" CssClass="requiredFieldValidator"
                                            Display="Dynamic" />
                                        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender2" runat="server"
                                            TargetControlID="txtAddTransReportedDate" Mask="99/99/9999" AcceptNegative="None"
                                            ClearMaskOnLostFocus="true" MaskType="Date" />
                                    </td>
                                    <td class="identifierLable_large_bold" width="5%">Price1</td>
                                    <td>
                                        <asp:TextBox ID="txtAddTransPrice1" Width="85%" runat="server" CssClass="textboxStyle"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rfvAddTransPrice1" ControlToValidate="txtAddTransPrice1" ValidationGroup="valGrpAddTransactions"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Price1" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revAddTransPrice1" runat="server" Text="*" ControlToValidate="txtAddTransPrice1"
                                            ValidationExpression="^-?\d{1,14}(\.\d{1,8})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                            ToolTip="Please enter only number upto 8 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                    </td>
                                    <td class="identifierLable_large_bold">Sales2</td>
                                    <td>
                                        <asp:TextBox ID="txtAddTransSales2" CssClass="textboxStyle" Width="85%" runat="server"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rfvAddTransSales2" ControlToValidate="txtAddTransSales2" ValidationGroup="valGrpAddTransactions"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Sales2" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revAddTransSales2" runat="server" Text="*" ControlToValidate="txtAddTransSales2"
                                            ValidationExpression="^-?[0-9]*$" CssClass="requiredFieldValidator" ForeColor="Red"
                                            ToolTip="Please enter only number" Display="Dynamic"> </asp:RegularExpressionValidator>
                                    </td>
                                    <td class="identifierLable_large_bold" width="5%">Receipts2</td>
                                    <td>
                                        <asp:TextBox ID="txtAddTransReceipts2" CssClass="textboxStyle" Width="85%" runat="server"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="revAddTransReceipts2" runat="server" Text="*" ControlToValidate="txtAddTransReceipts2" ValidationExpression="^-?\d{1,14}(\.\d{1,8})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                            ToolTip="Please enter only number upto 8 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                    </td>
                                    <td class="identifierLable_large_bold" width="5%">Company Code</td>
                                    <td>
                                        <asp:DropDownList ID="ddlAddTransCompanyCode" runat="server" Width="87%" CssClass="ddlStyle">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator15" ControlToValidate="ddlAddTransCompanyCode" ValidationGroup="valGrpAddTransactions"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please select Company Code" Display="Dynamic" InitialValue="-">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="identifierLable_large_bold" width="5%">Catalogue Number</td>
                                    <td>
                                        <asp:TextBox ID="txtAddTransCatNo" runat="server" MaxLength="30" CssClass="textboxStyle" Width="85%"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator8" ControlToValidate="txtAddTransCatNo" ValidationGroup="valGrpAddTransactions"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter valid catno" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="valCatNoEdit" runat="server" ValidationGroup="valGrpAddTransactions" CssClass="requiredFieldValidator" Display="Dynamic"
                                            OnServerValidate="valCatNoAddTrans_ServerValidate" ErrorMessage="*" ToolTip="Please enter valid catno"></asp:CustomValidator>
                                    </td>
                                    <td class="identifierLable_large_bold" width="5%">Price2</td>
                                    <td>
                                        <asp:TextBox ID="txtAddTransPrice2" Width="85%" runat="server" CssClass="textboxStyle"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rfvAddTransPrice2" ControlToValidate="txtAddTransPrice2" ValidationGroup="valGrpAddTransactions"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Price2" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revAddTransPrice2" runat="server" Text="*" ControlToValidate="txtAddTransPrice2"
                                            ValidationExpression="^-?\d{1,14}(\.\d{1,8})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                            ToolTip="Please enter only number upto 8 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                    </td>
                                    <td class="identifierLable_large_bold">Sales3</td>
                                    <td>
                                        <asp:TextBox ID="txtAddTransSales3" CssClass="textboxStyle" Width="85%" runat="server"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rfvAddTransSales3" ControlToValidate="txtAddTransSales3" ValidationGroup="valGrpAddTransactions"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Sales3" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revAddTransSales3" runat="server" Text="*" ControlToValidate="txtAddTransSales3" ValidationExpression="^-?[0-9]*$" CssClass="requiredFieldValidator" ForeColor="Red"
                                            ToolTip="Please enter only number" Display="Dynamic"> </asp:RegularExpressionValidator>
                                    </td>
                                    <td class="identifierLable_large_bold" width="5%">Receipts3</td>
                                    <td>
                                        <asp:TextBox ID="txtAddTransReceipts3" CssClass="textboxStyle" Width="85%" runat="server"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="revAddTransReceipts3" runat="server" Text="*" ControlToValidate="txtAddTransReceipts3"
                                            ValidationExpression="^-?\d{1,14}(\.\d{1,8})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                            ToolTip="Please enter only number upto 8 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                    </td>
                                    <td class="identifierLable_large_bold" width="5%">Currency Code</td>
                                    <td>
                                        <asp:TextBox ID="txtAddTransCurrencyCode" CssClass="textboxStyle" Width="85%" MaxLength="3" runat="server" Style="text-transform: uppercase;"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator9" ControlToValidate="txtAddTransCurrencyCode" ValidationGroup="valGrpAddTransactions"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Currency Code" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="valCurrencyAdd" runat="server" CssClass="requiredFieldValidator" ValidationGroup="valGrpAddTransactions" Display="Dynamic"
                                            OnServerValidate="valCurrencyAddTrans_ServerValidate" ControlToValidate="txtAddTransCurrencyCode" ErrorMessage="*" ToolTip="Please enter valid Currency Code"></asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="identifierLable_large_bold" width="5%">Territory</td>
                                    <td>
                                        <asp:TextBox ID="txtAddTransTerritory" runat="server" Width="85%" CssClass="textbox_FuzzySearch"
                                            onkeydown="OntxtTerritoryAddRowKeyDown(this);"></asp:TextBox>
                                        <ajaxToolkit:AutoCompleteExtender ID="aceTerritoryAddRow" runat="server"
                                            ServiceMethod="FuzzySearchSellerGroupListTypeC"
                                            ServicePath="~/Services/FuzzySearch.asmx"
                                            MinimumPrefixLength="1"
                                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                            TargetControlID="txtAddTransTerritory"
                                            FirstRowSelected="true"
                                            OnClientPopulating="territoryAddRowListPopulating"
                                            OnClientPopulated="territoryAddRowListPopulated"
                                            OnClientHidden="territoryAddRowListHidden"
                                            OnClientItemSelected="territoryAddRowListItemSelected"
                                            CompletionListElementID="pnlTerritoryAddRowFuzzySearch" />
                                        <asp:Panel ID="pnlTerritoryAddRowFuzzySearch" runat="server" CssClass="identifierLable"
                                            Style="bottom: 0px; top: 1000px; position: absolute; height: 200px;" />
                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="txtAddTransTerritory" ValidationGroup="valGrpAddTransactions"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please select Territory" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="valtxtTerritoryAddRow" runat="server" ValidationGroup="valGrpAddTransactions" CssClass="requiredFieldValidator"
                                            ClientValidationFunction="ValTerritoryAddRow" ToolTip="Please select valid Territory from the search list"
                                            ControlToValidate="txtAddTransTerritory" ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>
                                    </td>
                                    <td class="identifierLable_large_bold" width="5%">Price3</td>
                                    <td>
                                        <asp:TextBox ID="txtAddTransPrice3" Width="85%" runat="server" CssClass="textboxStyle"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rfvAddTransPrice3" ControlToValidate="txtAddTransPrice3" ValidationGroup="valGrpAddTransactions"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Price3" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revAddTransPrice3" runat="server" Text="*" ControlToValidate="txtAddTransPrice3"
                                            ValidationExpression="^-?\d{1,14}(\.\d{1,8})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                            ToolTip="Please enter only number upto 8 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                    </td>
                                    <td class="identifierLable_large_bold" width="10%">Destination Country</td>
                                    <td>
                                        <asp:TextBox ID="txtAddTransDestinationCountry" runat="server" Width="85%" CssClass="textboxStyle"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator11" ControlToValidate="txtAddTransDestinationCountry" ValidationGroup="valGrpAddTransactions"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Destination Country" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="CustomValidator3" runat="server" ValidationGroup="valGrpAddTransactions" CssClass="requiredFieldValidator" Display="Dynamic"
                                            OnServerValidate="valCountryAddTrans_ServerValidate" ErrorMessage="*" ToolTip="Please enter valid Country code"></asp:CustomValidator>
                                    </td>
                                    <td></td>
                                    <td></td>
                                    <td class="identifierLable_large_bold" width="5%">WHT TAX %</td>
                                    <td>
                                        <asp:TextBox ID="txtAddTransWhtTax" CssClass="textboxStyle" Width="85%" runat="server"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ControlToValidate="txtAddTransWhtTax" ValidationGroup="valGrpAddTransactions"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter WHT Tax %" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revAddTransTxtWhtTax" runat="server" Text="*" ControlToValidate="txtAddTransWhtTax" ValidationGroup="valGrpAddTransactions"
                                            ValidationExpression="^100(\.00?)?|^\d{0,2}(\.\d{1,2})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                            ToolTip="Please enter only positive number <= 100 upto 2 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                    </td>
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
                                    <td class="identifierLable_large_bold" width="5%">Owner Share</td>
                                    <td>
                                        <asp:TextBox ID="txtAddTransOwnerShare" Width="85%" runat="server" CssClass="textboxStyle"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator7" ControlToValidate="txtAddTransOwnerShare" ValidationGroup="valGrpAddTransactions"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Owner Share" Display="Dynamic" InitialValue="">
                                        </asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revShare" runat="server" Text="*" ControlToValidate="txtAddTransOwnerShare"
                                            ValidationExpression="^[+]?\d+$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valGrpAddTransactions"
                                            ToolTip="Please enter only integers" Display="Dynamic">
                                        </asp:RegularExpressionValidator>
                                    </td>
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
                                    <td class="identifierLable_large_bold" width="5%">Total Share</td>
                                    <td>
                                        <asp:TextBox ID="txtAddTransTotalShare" Width="85%" runat="server" CssClass="textboxStyle"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator10" ControlToValidate="txtAddTransTotalShare" ValidationGroup="valGrpAddTransactions"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Total Share" Display="Dynamic" InitialValue="">
                                        </asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" Text="*" ControlToValidate="txtAddTransTotalShare"
                                            ValidationExpression="^[+]?\d+$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valGrpAddTransactions"
                                            ToolTip="Please enter only integers" Display="Dynamic">
                                        </asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td colspan="3" align="center">
                                        <table width="50%">
                                            <tr>
                                                <td>
                                                    <asp:Button ID="btnSaveTransactions" runat="server" CssClass="ButtonStyle" OnClick="btnSaveTransactions_Click"
                                                        Text="Save" UseSubmitBehavior="false" Width="90%" ValidationGroup="valGrpAddTransactions" />
                                                </td>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                                <td>
                                                    <asp:Button ID="btnCancelTransactions" runat="server" CssClass="ButtonStyle" OnClientClick="return CloseAddTransPopup();"
                                                        Text="Cancel" UseSubmitBehavior="false" Width="90%" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="dummyFuzzySellerAddTrans" runat="server" Style="display: none" />
                                        <ajaxToolkit:ModalPopupExtender ID="mpeFuzzySellerAddTrans" runat="server" PopupControlID="pnlFuzzySellerAddTrans" TargetControlID="dummyFuzzySellerAddTrans"
                                            CancelControlID="btnCloseFuzzySellerAddTransPopup" BackgroundCssClass="popupBox">
                                        </ajaxToolkit:ModalPopupExtender>
                                        <asp:Panel ID="pnlFuzzySellerAddTrans" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                                            <table width="100%">
                                                <tr class="ScreenName">
                                                    <td>
                                                        <table width="100%">
                                                            <tr>
                                                                <td class="identifierLable">Complete Search List
                                                                </td>
                                                                <td align="right" style="vertical-align: top;">
                                                                    <asp:ImageButton ID="btnCloseFuzzySellerAddTransPopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
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
                                                        <asp:ListBox ID="lbFuzzySellerAddTrans" runat="server" Width="95%" CssClass="ListBox"
                                                            OnSelectedIndexChanged="lbFuzzySellerAddTrans_SelectedIndexChanged" AutoPostBack="True"></asp:ListBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>

                                        <asp:Button ID="dummyFuzzySalesTypeAddTrans" runat="server" Style="display: none" />
                                        <ajaxToolkit:ModalPopupExtender ID="mpeFuzzySalesTypeAddTrans" runat="server" PopupControlID="pnlFuzzySalesTypeAddTrans" TargetControlID="dummyFuzzySalesTypeAddTrans"
                                            CancelControlID="btnCloseFuzzySalesTypeAddTransPopup" BackgroundCssClass="popupBox">
                                        </ajaxToolkit:ModalPopupExtender>
                                        <asp:Panel ID="pnlFuzzySalesTypeAddTrans" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                                            <table width="100%">
                                                <tr class="ScreenName">
                                                    <td>
                                                        <table width="100%">
                                                            <tr>
                                                                <td class="identifierLable">Complete Search List
                                                                </td>
                                                                <td align="right" style="vertical-align: top;">
                                                                    <asp:ImageButton ID="btnCloseFuzzySalesTypeAddTransPopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
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
                                                        <asp:ListBox ID="lbFuzzySalesTypeAddTrans" runat="server" Width="95%" CssClass="ListBox"
                                                            OnSelectedIndexChanged="lbFuzzySalesTypeAddTrans_SelectedIndexChanged" AutoPostBack="True"></asp:ListBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>

                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <%--Add New Transaction Details--%>


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
                                CssClass="identifierLable" Text="Are you sure you want to delete this transaction?"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYes" runat="server" Text="Yes" CssClass="ButtonStyle" OnClientClick="return ConfirmDeleteYes();" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNo" runat="server" Text="No" CssClass="ButtonStyle" OnClientClick="return ConfirmDeleteNo();" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- End--%>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnPageNumber" runat="server" Value="1" />
            <asp:HiddenField ID="hdnValidDate" runat="server" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" />
            <asp:HiddenField ID="hdnFuzzySearchText" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnGridDataChanged" runat="server" />
            <asp:HiddenField ID="hdnGridFuzzySearchRowId" runat="server" />
            <asp:HiddenField ID="hdnIsConfirmPopup" runat="server" Value="N" />
            <asp:HiddenField ID="hdnButtonSelection" runat="server" />
            <asp:Button ID="btnFuzzySearch" runat="server" Style="display: none;" OnClick="btnFuzzySearch_Click" CausesValidation="false" />
            <asp:Button ID="btnHdnCatNoSearch" runat="server" Style="display: none;" OnClick="btnHdnCatNoSearch_Click" OnClientClick="return ValidateTransaction();" CausesValidation="false" />
            <asp:Button ID="btnHdnSearch" runat="server" Style="display: none;" OnClick="btnHdnSearch_Click" CausesValidation="false" OnClientClick="return ValidateTransaction();" />
            <asp:Button ID="btnFuzzyTerritoryListPopup" runat="server" Style="display: none;" OnClick="btnFuzzyTerritoryListPopup_Click" CausesValidation="false" />
            <asp:Button ID="btnFuzzySalesTypeListPopup" runat="server" Style="display: none;" OnClick="btnFuzzySalesTypeListPopup_Click" CausesValidation="false" />
            <asp:Button ID="btnFuzzyTerritoryListPopupAddTrans" runat="server" Style="display: none;" OnClick="btnFuzzyTerritoryListPopupAddTrans_Click" CausesValidation="false" />
            <asp:Button ID="btnFuzzySalesTypeListPopupAddTrans" runat="server" Style="display: none;" OnClick="btnFuzzySalesTypeListPopupAddTrans_Click" CausesValidation="false" />
            <asp:HiddenField ID="hdnSearchText" runat="server" Value="" />
            <asp:HiddenField ID="hdnSearchAttr" runat="server" Value="" />
            <asp:HiddenField ID="hdnCompanyCode" runat="server" Value="" />
            <asp:HiddenField ID="hdnPrimaryCompanyCode" runat="server" Value="" />
            <asp:HiddenField ID="hdnDeleteRowIndex" runat="server" Value="" />
            <asp:HiddenField ID="hdnIsValidSeller" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsValidSalesType" runat="server" Value="N" />                      
            <asp:Label ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:Label>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>







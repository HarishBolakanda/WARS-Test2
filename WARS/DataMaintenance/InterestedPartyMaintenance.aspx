<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InterestedPartyMaintenance.aspx.cs" Inherits="WARS.InterestedPartyMaintenance" MasterPageFile="~/MasterPage.Master"
    Title="WARS - InterestedPartyMaintenance " MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

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
            postBackElementID = args.get_postBackElement().id;
            if (postBackElementID.lastIndexOf('imgBtnSave') > 0 || postBackElementID.lastIndexOf('imgBtnDelete') > 0 || postBackElementID.lastIndexOf('imgBtnUndo') > 0 ||
                postBackElementID.lastIndexOf('btnSaveChanges') > 0 || postBackElementID.lastIndexOf('btnUndoChanges') > 0 || postBackElementID.lastIndexOf('btnLinkedRoyaltors') > 0 ||
                postBackElementID.lastIndexOf('btnBankDetails') > 0 || postBackElementID.lastIndexOf('imgBtnInsert') > 0 || postBackElementID.lastIndexOf('imgBtnCancel') > 0) {
                xPos = sender._scrollPosition.x;
                yPos = sender._scrollPosition.y;

                //hold scroll position on selecting expand/collapse
                var PnlInterestedPartyDetails = document.getElementById("<%=PnlInterestedPartyDetails.ClientID %>");
                scrollTop = PnlInterestedPartyDetails.scrollTop;

            }
        }

        function EndRequestHandler(sender, args) {
            //Hide the modal popup - the update progress
            var popup = $find('<%= mPopupPageLevel.ClientID %>');
            if (popup != null) {
                popup.hide();
            }

            //to maintain scroll position
            postBackElementID = sender._postBackSettings.sourceElement.id;
            if (postBackElementID.lastIndexOf('imgBtnSave') > 0 || postBackElementID.lastIndexOf('imgBtnDelete') > 0 || postBackElementID.lastIndexOf('imgBtnUndo') > 0 || postBackElementID.lastIndexOf('btnSaveChanges') > 0 || postBackElementID.lastIndexOf('btnUndoChanges') > 0 || postBackElementID.lastIndexOf('btnLinkedRoyaltors') > 0 || postBackElementID.lastIndexOf('btnBankDetails') > 0 || postBackElementID.lastIndexOf('imgBtnInsert') > 0 || postBackElementID.lastIndexOf('imgBtnCancel') > 0) {
                window.scrollTo(xPos, yPos);

                //set scroll position on selecting expand/collapse
                var PnlInterestedPartyDetails = document.getElementById("<%=PnlInterestedPartyDetails.ClientID %>");
                PnlInterestedPartyDetails.scrollTop = scrollTop;
            }


        }
        //======================= End


        //Fuzzy search filters
        var txtInterestedPartySearch;
        function InterestedPartySelected(sender, args) {

            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtInterestedPartySearch.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnIsValidSearch.ClientID %>").value = "Y";
                document.getElementById('<%=btnHdnInterestedPartySearch.ClientID%>').click();
            }
        }

        function InterestedPartyListPopulating() {
            txtInterestedPartySearch = document.getElementById("<%= txtInterestedPartySearch.ClientID %>");
            txtInterestedPartySearch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtInterestedPartySearch.style.backgroundRepeat = 'no-repeat';
            txtInterestedPartySearch.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidSearch.ClientID %>").value = "N";
        }

        function InterestedPartyListPopulated() {
            txtInterestedPartySearch = document.getElementById("<%= txtInterestedPartySearch.ClientID %>");
            txtInterestedPartySearch.style.backgroundImage = 'none';
        }

        //=============== End

        //set flag value when data is changed in grid 
        //debugger;
        function OnDataChange() {
            document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        }

        function OnkeyDown() {
            if (event.keyCode == 8 || event.keyCode == 46) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
            }
        }

        function OnClickCheckbox(row) {
            OnGridRowSelected(row);
            document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        }

        //set flag value when data is changed in textboxes for inserting new row

        function OnDataChangeInsert() {
            document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").innerText = "Y";
        }

        function OnkeyDownInsert() {
            if (event.keyCode == 8 || event.keyCode == 46) {
                document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").innerText = "Y";
            }
        }

        function OnClickCheckboxInsert() {
            if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
                var popup = $find('<%= mpeSaveUndo.ClientID %>');
                if (popup != null) {
                    popup.show();
                    $get("<%=btnUndoChanges.ClientID%>").focus();
                }
                return false;
            }
            else {
                document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").innerText = "Y";
                return true;
            }

        }

        //Show warning while closing the window if changed data not saved 
        function WarnOnUnSavedData() {
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            var isDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
            var isDataChangedInsert = document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value;
            if (isExceptionRaised != "Y" && (isDataChanged == "Y" || isDataChangedInsert == "Y")) {
                return warningMsgOnUnSavedData;
            }
        }
        window.onbeforeunload = WarnOnUnSavedData;
        function ValidateChanges() {
            if (!(WarnOnUnSavedData.length > 0)) {
                eval(this.href);
            }
        }
        //used to check if any changes to allow navigation to other screen 
        function IsDataChanged() {
            var isDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
            var isDataChangedInsert = document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value;
            if ((isDataChanged == "Y" || isDataChangedInsert == "Y")) {
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

        //Validation: warning message if changes made and not saved

        function OnGridRowSelected(row) {
            var rowData = row.parentNode.parentNode;
            var rowIndex = rowData.rowIndex - 1;

            if (document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value == "Y") {
                var popup = $find('<%= mpeSaveUndo.ClientID %>');
                if (popup != null) {
                    document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new interested party. Save or Undo changes";
                    popup.show();
                    $get("<%=btnUndoChanges.ClientID%>").focus();
                }
            }
            else {
                if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value == "") {
                    document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").innerText = rowIndex;
                }
                else if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value != rowIndex) {
                    if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
                        var popup = $find('<%= mpeSaveUndo.ClientID %>');
                        if (popup != null) {
                            popup.show();
                            $get("<%=btnUndoChanges.ClientID%>").focus();
                        }
                    }
                    else {
                        document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").innerText = rowIndex;
                    }
                }
        }
    }

    function ConfirmSearch() {
        if (document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value == "Y") {
            var popup = $find('<%= mpeSaveUndo.ClientID %>');
            if (popup != null) {
                document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new interested party. Save or Undo changes";
                popup.show();
                $get("<%=btnUndoChanges.ClientID%>").focus();
                return false;
            }
        }
        else {
            if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
                var popup = $find('<%= mpeSaveUndo.ClientID %>');
                if (popup != null) {
                    popup.show();
                    $get("<%=btnUndoChanges.ClientID%>").focus();
                }
                return false;
            }
            else {
                return true;
            }
        }
    }

    function ConfirmInsert() {
        if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
            var popup = $find('<%= mpeSaveUndo.ClientID %>');
            if (popup != null) {
                popup.show();
                $get("<%=btnUndoChanges.ClientID%>").focus();
            }
            return false;
        }
        else {
            return true;
        }
    }

    function ConfirmUpdate(row) {
        var rowData = row.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
        var rowIndex = rowData.rowIndex - 1;

        if (document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value == "Y") {
            var popup = $find('<%= mpeSaveUndo.ClientID %>');
            if (popup != null) {
                document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new interested party. Save or Undo changes";
                popup.show();
                $get("<%=btnUndoChanges.ClientID%>").focus();
                return false;
            }
        }
        else {
            if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value == "") {
                return true;
            }
            else if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value != rowIndex) {
                if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
                    var popup = $find('<%= mpeSaveUndo.ClientID %>');
                    if (popup != null) {
                        popup.show();
                        $get("<%=btnUndoChanges.ClientID%>").focus();
                    }
                    return false;
                }
                else {
                    return true;
                }
            }
            else if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value == rowIndex) {
                return true;
            }
    }
}

function ConfirmDelete(row) {
    var rowData = row.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
    var rowIndex = rowData.rowIndex - 1;
    var tempstr = row.id.substring(0, row.id.lastIndexOf('_'));
    var str = tempstr.substring(0, tempstr.lastIndexOf('_') + 1);
    var interestedPartyCode = document.getElementById(str + 'hdnIntPartyId' + '_' + rowIndex).value;
    if (document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value == "Y") {
        var popup = $find('<%= mpeSaveUndo.ClientID %>');
        if (popup != null) {
            document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new interested party. Save or Undo changes";
            popup.show();
            $get("<%=btnUndoChanges.ClientID%>").focus();
            return false;
        }
    }
    else {
        if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value == "") {
            document.getElementById("<%=hdninterestedPartyCode.ClientID %>").innerText = interestedPartyCode;
            var popup = $find('<%= mpeConfirmDelete.ClientID %>');
            if (popup != null) {
                popup.show();
            }
            return false; //JIRA-908 Changes
        }
        else if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value != rowIndex) {
            if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
                var popup = $find('<%= mpeSaveUndo.ClientID %>');
                if (popup != null) {
                    popup.show();
                    $get("<%=btnUndoChanges.ClientID%>").focus();
                }
                return false;
            }
            else {
                document.getElementById("<%=hdninterestedPartyCode.ClientID %>").innerText = interestedPartyCode;
                var popup = $find('<%= mpeConfirmDelete.ClientID %>');
                if (popup != null) {
                    popup.show();
                }
                return false;//JIRA-908 Changes
            }
        }
        else if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value == rowIndex) {
            document.getElementById("<%=hdninterestedPartyCode.ClientID %>").innerText = interestedPartyCode;
            var popup = $find('<%= mpeConfirmDelete.ClientID %>');
            if (popup != null) {
                popup.show();
            }
            return false;//JIRA-908 Changes
        }
}
}

//grid panel height adjustment functioanlity - starts

function SetGrdPnlHeightOnLoad() {
    var windowHeight = window.screen.availHeight;
    var gridPanelHeight = windowHeight * 0.5;
    document.getElementById("<%=PnlInterestedPartyDetails.ClientID %>").style.height = gridPanelHeight + "px";
    document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

}

//======================= End

//Tab key to remain only on screen fields
function OnTabPress() {
    if (event.keyCode == 9) {
        document.getElementById("<%= lblTab.ClientID %>").focus();
    }
}

//=============== End

//On press of Enter key in search textbox
function OnInterestedPartyKeyDown() {
    if ((event.keyCode == 13)) {
        document.getElementById('<%=btnHdnInterestedPartySearch.ClientID%>').click();
    }
}
//=============== End

//open Audit screen
function OpenAuditScreen() {
    //debugger;
    if (IsDataChanged()) {
        window.onbeforeunload = null;
        OpenPopupOnUnSavedData('../Audit/InterestedPartyAudit.aspx');
    }
    else {
        return true;
    }
}
//=============End

//WUIN-1164 - Auto Populate Generate Invoice on adding Tax number

function checkGenerateInvoiceAddRow() {
    var txtTaxNumber = document.getElementById("<%=txtTaxNumber.ClientID %>").value;
    var applicableTax = document.getElementById("<%=ddlTaxType.ClientID %>").value;
    var cbGenerateInvoiceInsert = document.getElementById("<%=cbGenerateInvoiceInsert.ClientID %>");
    if (txtTaxNumber != "" && applicableTax != "-") {
        cbGenerateInvoiceInsert.checked = true;
    }
    else {
        cbGenerateInvoiceInsert.checked = false;
    }


}


function checkGenerateInvoiceGridRow(row) {
    var rowIndex = row.id.substring(row.id.lastIndexOf('_') + 1);
    var tempstr = row.id.substring(0, row.id.lastIndexOf('_'));
    var str = tempstr.substring(0, tempstr.lastIndexOf('_') + 1);

    var txtTaxNumber = document.getElementById(str + 'txtTaxNumber_' + rowIndex).value;
    var cbGenerateInvoice = document.getElementById(str + 'cbGenerateInvoice_' + rowIndex);
    var applicableTax = document.getElementById(str + 'ddlTaxType_' + rowIndex).value;
    if (txtTaxNumber != "" && applicableTax != "-") {
        cbGenerateInvoice.checked = true;
    }
    else {
        cbGenerateInvoice.checked = false;
    }

}

//WUIN-1144 - Validataion -Tax number and applicable tax are mandatory to check generate invoice
function validateGenerateInvoiceGrid(row) {
    OnGridRowSelected(row);
    var rowIndex = row.id.substring(row.id.lastIndexOf('_') + 1);
    var tempstr = row.id.substring(0, row.id.lastIndexOf('_'));
    var str = tempstr.substring(0, tempstr.lastIndexOf('_') + 1);
    var hdnInsertDataNotSaved = document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value;
    var hdnChangeNotSaved = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
    var hdnGridRowSelectedPrvious = document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value;
    if (hdnInsertDataNotSaved != "Y" && (hdnChangeNotSaved != "Y" || (hdnChangeNotSaved == "Y" && hdnGridRowSelectedPrvious == rowIndex))) {

        var txtTaxNumber = document.getElementById(str + 'txtTaxNumber_' + rowIndex).value;
        var applicableTax = document.getElementById(str + 'ddlTaxType_' + rowIndex).value;
        var cbGenerateInvoice = document.getElementById(str + 'cbGenerateInvoice_' + rowIndex);
        if (txtTaxNumber == "" || applicableTax == "-") {
            cbGenerateInvoice.checked = false;
            DisplayMessagePopup("Tax number and applicabe tax are mandatory for generating invoice");

        }
        else { document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y"; }
    }
}


function validateGenerateInvoiceAdd() {

    var txtTaxNumber = document.getElementById("<%=txtTaxNumber.ClientID %>").value;
    var applicableTax = document.getElementById("<%=ddlTaxType.ClientID %>").value;
    var cbGenerateInvoiceInsert = document.getElementById("<%=cbGenerateInvoiceInsert.ClientID %>");

    if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
        var popup = $find('<%= mpeSaveUndo.ClientID %>');
        if (popup != null) {
            popup.show();
            $get("<%=btnUndoChanges.ClientID%>").focus();
        }
        cbGenerateInvoiceInsert.checked = false;
        return false;
    }
    else {
        if (txtTaxNumber == "" || applicableTax == "-") {
            cbGenerateInvoiceInsert.checked = false;
            DisplayMessagePopup("Tax number and applicabe tax are mandatory for generating invoice");
            return false;
        }
        else {
            document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").innerText = "Y";
                    return true;
                }

            }
        }
    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="9">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    INTERESTED PARTY MAINTENANCE
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="9">
                        <table width="100%">
                            <tr>
                                <td width="1%"></td>
                                <td width="10%" class="identifierLable_large_bold" align="center">Interested Party</td>
                                <td width="20%">
                                    <asp:TextBox ID="txtInterestedPartySearch" runat="server" Width="90%" CssClass="textboxStyle"
                                        TabIndex="100" onfocus="return ConfirmSearch();" onkeydown="OnInterestedPartyKeyDown();"></asp:TextBox>
                                    <ajaxToolkit:TextBoxWatermarkExtender ID="tbweInterestedPartySearch" runat="server"
                                        TargetControlID="txtInterestedPartySearch"
                                        WatermarkText="Enter Search Text"
                                        WatermarkCssClass="watermarked" />
                                </td>
                                <td width="10%" class="identifierLable_large_bold" align="center">Interested Party Type</td>
                                <td width="15%" align="right">
                                    <asp:DropDownList ID="ddlIntPartyType" runat="server" CssClass="ddlStyle" TabIndex="101" Width="90%" AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlIntPartyType_SelectedIndexChanged" onfocus="return ConfirmSearch();" />
                                </td>
                                <td width="10%" class="identifierLable_large_bold" align="center">IP Number</td>
                                <td width="15%">
                                    <asp:TextBox ID="txtIpNumber" runat="server" Width="90%" CssClass="textboxStyle"
                                        TabIndex="102" onfocus="return ConfirmSearch();" onkeydown="OnInterestedPartyKeyDown();"></asp:TextBox>
                                    <ajaxToolkit:TextBoxWatermarkExtender ID="tbwetxtIpNumber" runat="server"
                                        TargetControlID="txtIpNumber"
                                        WatermarkText="Enter Search IP Number"
                                        WatermarkCssClass="watermarked" />
                                </td>
                                <td align="right">
                                    <asp:Button ID="btnReset" runat="server" CssClass="ButtonStyle" OnClick="btnReset_Click" TabIndex="117" Text="Reset" UseSubmitBehavior="false" Width="50%" onfocus="return ConfirmSearch();" />
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
                                <td align="right">
                                    <asp:Button ID="btnAudit" runat="server" CssClass="ButtonStyle" OnClick="btnAudit_Click" TabIndex="118" Text="Audit" OnClientClick="if (!OpenAuditScreen()) { return false;};" UseSubmitBehavior="false" Width="50%" onkeydown="OnTabPress();" onfocus="return ConfirmSearch();" />
                                </td>

                            </tr>
                        </table>

                    </td>


                </tr>
                <tr>
                    <td colspan="7">
                        <%--<br />--%>
                    </td>
                    <td align="right" colspan="2">
                        <asp:Button ID="btnPayeeDetails" runat="server" CssClass="ButtonStyle" OnClick="btnPayeeDetails_Click" TabIndex="119" Text="Back to Payee Details" UseSubmitBehavior="false" Width="98%" />
                        <asp:Button ID="btnCourtesyDetails" runat="server" CssClass="ButtonStyle" OnClick="btnCourtesyDetails_Click" TabIndex="119" Text="Back to Courtesy Details" UseSubmitBehavior="false" Width="98%" />
                    </td>
                </tr>
                <tr>
                    <td colspan="7"></td>
                    <td width="11%"></td>
                    <td width="1%"></td>
                </tr>
                <tr>
                    <td colspan="9" class="table_with_border">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="1%">
                                    <br />
                                </td>
                                <td width="99%"></td>
                                <%--<td width="1%"></td>--%>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlInterestedPartyDetails" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvInterestedPartyDetails" runat="server" AutoGenerateColumns="False" Width="98.75%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." OnRowDataBound="gvInterestedPartyDetails_RowDataBound" OnRowCommand="gvInterestedPartyDetails_RowCommand" AllowSorting="true" OnSorting="gvInterestedPartyDetails_Sorting" HeaderStyle-CssClass="FixedHeader">

                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="IP Number" SortExpression="ip_number">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblIPNumber" runat="server" Text='<%# Bind("ip_number") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="5%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Type" SortExpression="int_party_type_desc">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnIntPartyId" runat="server" Value='<%# Bind("int_party_id") %>'></asp:HiddenField>

                                                                    <asp:Label ID="lblInterestedPartyCode" runat="server" Text='<%# Bind("int_party_id") %>' CssClass="identifierLable" Visible="false"></asp:Label>
                                                                    <asp:Label ID="lblInterestedPartyType" runat="server" Text='<%# Bind("int_party_type_desc") %>' CssClass="identifierLable"></asp:Label>
                                                                    <%--<asp:DropDownList ID="ddlIntPartyTypeGrid" runat="server" Width="86%" CssClass="ddlStyle" onchange="javascript: OnDataChange();" onfocus="OnGridRowSelected(this)"></asp:DropDownList>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvIntPartyTypeGrid" ControlToValidate="ddlIntPartyTypeGrid" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>'
                                                                        Text="*" InitialValue="-" CssClass="requiredFieldValidator" ToolTip="Please select interested party type" Display="Dynamic"></asp:RequiredFieldValidator>--%>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="4%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Name" SortExpression="int_party_name">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblInterestedPartyName" runat="server" Text='<%# Bind("int_party_name") %>' CssClass="identifierLable" Visible="false"></asp:Label>
                                                                    <asp:TextBox ID="txtInterestedPartyName" runat="server" Text='<%# Eval("int_party_name") %>'
                                                                        CssClass="gridTextField" Width="90%" MaxLength="60" onchange="javascript: OnDataChange();" onKeyPress="javascript: OnDataChange();"
                                                                        onkeydown="javascript: OnkeyDown();" onfocus="OnGridRowSelected(this)"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvInterestedPartyName" ControlToValidate="txtInterestedPartyName" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>'
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter interested party name" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Address 1" SortExpression="int_party_add1">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblInterestedPartyAdd1" runat="server" Text='<%# Bind("int_party_add1") %>' CssClass="identifierLable" Visible="false"></asp:Label>
                                                                    <asp:TextBox ID="txtInterestedPartyAdd1" runat="server" Text='<%# Eval("int_party_add1") %>'
                                                                        CssClass="gridTextField" Width="97%" MaxLength="50" onchange="javascript: OnDataChange();" onKeyPress="javascript: OnDataChange();"
                                                                        onkeydown="javascript: OnkeyDown();" onfocus="OnGridRowSelected(this)"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="9%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Address 2" SortExpression="int_party_add2">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblInterestedPartyAdd2" runat="server" Text='<%# Bind("int_party_add2") %>' CssClass="identifierLable" Visible="false"></asp:Label>
                                                                    <asp:TextBox ID="txtInterestedPartyAdd2" runat="server" Text='<%# Eval("int_party_add2") %>'
                                                                        CssClass="gridTextField" Width="97%" MaxLength="50" onchange="javascript: OnDataChange();" onKeyPress="javascript: OnDataChange();"
                                                                        onkeydown="javascript: OnkeyDown();" onfocus="OnGridRowSelected(this)"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="9%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Address 3" SortExpression="int_party_add3">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblInterestedPartyAdd3" runat="server" Text='<%# Bind("int_party_add3") %>' CssClass="identifierLable" Visible="false"></asp:Label>
                                                                    <asp:TextBox ID="txtInterestedPartyAdd3" runat="server" Text='<%# Eval("int_party_add3") %>'
                                                                        CssClass="gridTextField" Width="97%" MaxLength="50" onchange="javascript: OnDataChange();" onKeyPress="javascript: OnDataChange();"
                                                                        onkeydown="javascript: OnkeyDown();" onfocus="OnGridRowSelected(this)"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="9%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Address 4" SortExpression="int_party_add4">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblInterestedPartyAdd4" runat="server" Text='<%# Bind("int_party_add4") %>' CssClass="identifierLable" Visible="false"></asp:Label>
                                                                    <asp:TextBox ID="txtInterestedPartyAdd4" runat="server" Text='<%# Eval("int_party_add4") %>'
                                                                        CssClass="gridTextField" Width="97%" MaxLength="50" onchange="javascript: OnDataChange();" onKeyPress="javascript: OnDataChange();"
                                                                        onkeydown="javascript: OnkeyDown();" onfocus="OnGridRowSelected(this)"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="9%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Postcode" SortExpression="int_party_postcode">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblInterestedPartyPostcode" runat="server" Text='<%# Bind("int_party_postcode") %>' CssClass="identifierLable" Visible="false"></asp:Label>
                                                                    <asp:TextBox ID="txtInterestedPartyPostcode" runat="server" Text='<%# Eval("int_party_postcode") %>'
                                                                        CssClass="gridTextField" Width="97%" MaxLength="20" onchange="javascript: OnDataChange();" onKeyPress="javascript: OnDataChange();"
                                                                        onkeydown="javascript: OnkeyDown();" onfocus="OnGridRowSelected(this)"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="5%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Email" SortExpression="email">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblInterestedPartyEmail" runat="server" Text='<%# Bind("email") %>' CssClass="identifierLable" Visible="false"></asp:Label>
                                                                    <asp:TextBox ID="txtInterestedPartyEmail" runat="server" Text='<%# Eval("email") %>'
                                                                        CssClass="gridTextField" Width="89%" MaxLength="254" onchange="javascript: OnDataChange();" onKeyPress="javascript: OnDataChange();"
                                                                        onkeydown="javascript: OnkeyDown();" onfocus="OnGridRowSelected(this)"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="9%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Tax Number" SortExpression="vat_number">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblTaxNumber" runat="server" Text='<%# Bind("vat_number") %>' CssClass="identifierLable" Visible="false"></asp:Label>
                                                                    <asp:TextBox ID="txtTaxNumber" runat="server" Text='<%# Eval("vat_number") %>'
                                                                        CssClass="gridTextField" Width="97%" MaxLength="20" onchange="javascript: OnDataChange();checkGenerateInvoiceGridRow(this);" onKeyPress="javascript: OnDataChange();"
                                                                        onkeydown="javascript: OnkeyDown();" onfocus="OnGridRowSelected(this)"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="4%" />
                                                            </asp:TemplateField>
                                                            <%--JIRA-1144 Changes -- Start--%>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Applicable Tax" SortExpression="applicable_tax">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnApplicableTax" runat="server" Value='<%# Bind("applicable_tax") %>'></asp:HiddenField>
                                                                    <asp:DropDownList ID="ddlTaxType" runat="server" Width="86%" CssClass="ddlStyle" onfocus="OnGridRowSelected(this)" onchange="javascript: OnDataChange();checkGenerateInvoiceGridRow(this);" onKeyPress="javascript: OnDataChange();" onkeydown="javascript: OnkeyDown();">
                                                                    </asp:DropDownList>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="5%" />
                                                            </asp:TemplateField>
                                                            <%--JIRA-1144 Changes -- End--%>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Generate Invoice?" SortExpression="generate_invoice">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnGenerateInvoice" runat="server" Value='<%# Bind("generate_invoice") %>' />
                                                                    <asp:CheckBox ID="cbGenerateInvoice" runat="server" onclick="validateGenerateInvoiceGrid(this);" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="4%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Send Stmt?" SortExpression="send_statement">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnSendStatement" runat="server" Value='<%# Bind("send_statement") %>' />
                                                                    <asp:CheckBox ID="cbSendStatement" runat="server" onclick="javascript: OnClickCheckbox(this);" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="3%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Right_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <table width="100%" style="float: right; table-layout: fixed">
                                                                        <tr style="float: right">
                                                                            <td align="right" style="float: right" width="33%">
                                                                                <asp:ImageButton ID="imgBtnSave" runat="server" CommandName="saverow" ImageUrl="../Images/save.png" ToolTip="Save"
                                                                                    ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>' onfocus="return ConfirmUpdate(this);" />
                                                                            </td>
                                                                            <td align="right" style="float: right" width="33%">
                                                                                <asp:ImageButton ID="imgBtnDelete" runat="server" CommandName="deleterow" ImageUrl="../Images/Delete.gif"
                                                                                    ToolTip="Delete" OnClientClick="return ConfirmDelete(this);" />
                                                                            </td>
                                                                            <td align="right" style="float: right" width="33%">
                                                                                <asp:ImageButton ID="imgBtnUndo" runat="server" CommandName="cancelrow" ImageUrl="../Images/cancel_row3.png"
                                                                                    ToolTip="Cancel" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="8%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <table width="100%" style="float: right; table-layout: fixed">
                                                                        <tr style="float: right">
                                                                            <td align="right" style="float: right" width="50%">
                                                                                <asp:Button ID="btnLinkedRoyaltors" runat="server" CommandName="linkedroyaltor" Width="99%" Text="Royaltors" CssClass="GridButtonStyle" />
                                                                            </td>
                                                                            <td align="right" style="float: right" width="50%">
                                                                                <asp:Button ID="btnBankDetails" runat="server" CommandName="bankdetails" Width="99%" Text="Supplier" CssClass="GridButtonStyle" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </asp:Panel>
                                                <div align="center">
                                                    <asp:Repeater ID="rptPager" runat="server">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>'
                                                                OnClientClick="return ValidateChanges();" ClientIDMode="AutoID" CausesValidation="false" Enabled='<%# Eval("Enabled") %>' OnClick="lnkPage_Click" CssClass="gridPager"> </asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>
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
                                                        <td width="6%" class="gridHeaderStyle_1row">Type</td>
                                                        <td width="12%" class="gridHeaderStyle_1row">Name</td>
                                                        <td width="11%" class="gridHeaderStyle_1row">Address 1</td>
                                                        <td width="11%" class="gridHeaderStyle_1row">Address 2</td>
                                                        <td width="11%" class="gridHeaderStyle_1row">Address 3</td>
                                                        <td width="11%" class="gridHeaderStyle_1row">Address 4</td>
                                                        <td width="6%" class="gridHeaderStyle_1row">Postcode</td>
                                                        <td width="10%" class="gridHeaderStyle_1row">Email</td>
                                                        <td width="6%" class="gridHeaderStyle_1row">Tax Number</td>
                                                        <td width="5%" class="gridHeaderStyle_1row">Applicable Tax</td>
                                                        <%--JIRA-1144 CHanges--%>
                                                        <td width="4%" class="gridHeaderStyle_1row">Generate Invoice?</td>
                                                        <td width="3%" class="gridHeaderStyle_1row">Send Stmt?</td>
                                                        <td width="3%" class="gridHeaderStyle_1row">&nbsp</td>
                                                    </tr>
                                                    <tr>
                                                        <td class="insertBoxStyle">
                                                            <asp:DropDownList ID="ddlIntPartyTypeInsert" runat="server" CssClass="ddlStyle" Width="84%" onfocus="return ConfirmInsert();"
                                                                onchange="javascript: OnDataChangeInsert();" TabIndex="103" />
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvIntPartyTypeInsert" ControlToValidate="ddlIntPartyTypeInsert" ValidationGroup="valInsertInterestedParty"
                                                                Text="*" InitialValue="-" CssClass="requiredFieldValidator" ToolTip="Please select interested party type" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td class="insertBoxStyle">
                                                            <asp:TextBox ID="txtInterestedPartyName" runat="server" CssClass="textboxStyle" TabIndex="104" Width="90%" MaxLength="60" onfocus="return ConfirmInsert();"
                                                                onchange="javascript: OnDataChangeInsert();" onKeyPress="javascript: OnDataChangeInsert();" onkeydown="javascript: OnkeyDownInsert();"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvInterestedPartyName" ControlToValidate="txtInterestedPartyName" ValidationGroup="valInsertInterestedParty"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter interested party name" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td class="insertBoxStyle">
                                                            <asp:TextBox ID="txtAddress1" runat="server" CssClass="textboxStyle" TabIndex="105" Width="95%" MaxLength="50" onfocus="return ConfirmInsert();"
                                                                onchange="javascript: OnDataChangeInsert();" onKeyPress="javascript: OnDataChangeInsert();" onkeydown="javascript: OnkeyDownInsert();"></asp:TextBox>
                                                        </td>
                                                        <td class="insertBoxStyle">
                                                            <asp:TextBox ID="txtAddress2" runat="server" CssClass="textboxStyle" TabIndex="106" Width="95%" MaxLength="50" onfocus="return ConfirmInsert();"
                                                                onchange="javascript: OnDataChangeInsert();" onKeyPress="javascript: OnDataChangeInsert();" onkeydown="javascript: OnkeyDownInsert();"></asp:TextBox>
                                                        </td>
                                                        <td class="insertBoxStyle">
                                                            <asp:TextBox ID="txtAddress3" runat="server" CssClass="textboxStyle" TabIndex="107" Width="95%" MaxLength="50" onfocus="return ConfirmInsert();"
                                                                onchange="javascript: OnDataChangeInsert();" onKeyPress="javascript: OnDataChangeInsert();" onkeydown="javascript: OnkeyDownInsert();"></asp:TextBox>
                                                        </td>
                                                        <td class="insertBoxStyle">
                                                            <asp:TextBox ID="txtAddress4" runat="server" CssClass="textboxStyle" TabIndex="108" Width="95%" MaxLength="50" onfocus="return ConfirmInsert();"
                                                                onchange="javascript: OnDataChangeInsert();" onKeyPress="javascript: OnDataChangeInsert();" onkeydown="javascript: OnkeyDownInsert();"></asp:TextBox>
                                                        </td>
                                                        <td class="insertBoxStyle">
                                                            <asp:TextBox ID="txtPostcode" runat="server" CssClass="textboxStyle" TabIndex="109" Width="95%" MaxLength="20" onfocus="return ConfirmInsert();"
                                                                onchange="javascript: OnDataChangeInsert();" onKeyPress="javascript: OnDataChangeInsert();" onkeydown="javascript: OnkeyDownInsert();"></asp:TextBox>
                                                        </td>
                                                        <td class="insertBoxStyle">
                                                            <asp:TextBox ID="txtEmail" runat="server" CssClass="textboxStyle" TabIndex="110" Width="87%" MaxLength="254" onfocus="return ConfirmInsert();"
                                                                onchange="javascript: OnDataChangeInsert();" onKeyPress="javascript: OnDataChangeInsert();" onkeydown="javascript: OnkeyDownInsert();"></asp:TextBox>
                                                        </td>
                                                        <td class="insertBoxStyle">
                                                            <asp:TextBox ID="txtTaxNumber" runat="server" CssClass="textboxStyle" TabIndex="111" Width="95%" MaxLength="12" onfocus="return ConfirmInsert();"
                                                                onchange="javascript: OnDataChangeInsert(); checkGenerateInvoiceAddRow();" onKeyPress="javascript: OnDataChangeInsert();" onkeydown="javascript: OnkeyDownInsert();"></asp:TextBox>
                                                        </td>
                                                        <%--JIRA-1144 Changes -- Start--%>
                                                        <td class="insertBoxStyle">
                                                            <asp:DropDownList ID="ddlTaxType" runat="server" Width="86%" CssClass="ddlStyle" TabIndex="112" onchange="javascript: OnDataChangeInsert(); checkGenerateInvoiceAddRow();" onKeyPress="javascript: OnDataChangeInsert();" onkeydown="javascript: OnkeyDownInsert();"  onfocus="return ConfirmInsert();">
                                                            </asp:DropDownList>
                                                        </td>
                                                        <%--JIRA-1144 Changes -- End--%>
                                                        <td class="insertBoxStyle" align="center">
                                                            <asp:CheckBox ID="cbGenerateInvoiceInsert" runat="server" onclick="javascript: validateGenerateInvoiceAdd();" TabIndex="113" />
                                                        </td>
                                                        <td class="insertBoxStyle" align="center">
                                                            <asp:CheckBox ID="cbSendStmtInsert" runat="server" onclick="javascript: OnClickCheckboxInsert();" TabIndex="114" />
                                                        </td>
                                                        <td class="insertBoxStyle_No_Padding">
                                                            <table width="100%" style="float: right; table-layout: fixed">
                                                                <tr style="float: right">
                                                                    <td align="right" style="float: right" width="50%">
                                                                        <asp:ImageButton ID="imgBtnInsert" runat="server" CommandName="saverow" ImageUrl="../Images/save.png" ToolTip="Insert Interested Party"
                                                                            OnClick="imgBtnInsert_Click" ValidationGroup="valInsertInterestedParty" TabIndex="115" />
                                                                    </td>
                                                                    <td align="right" style="float: right" width="50%">
                                                                        <asp:ImageButton ID="imgBtnCancel" runat="server" CommandName="cancelrow" ImageUrl="../Images/cancel_row3.png"
                                                                            ToolTip="Cancel" OnClick="imgBtnCancel_Click" TabIndex="116" />
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
                                                <br />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <%--<td></td>--%>
                            </tr>
                        </table>

                    </td>
                </tr>
            </table>

            <%--<asp:Button ID="dummyFuzzySearch" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeFuzzySearch" runat="server" PopupControlID="pnlFuzzySearch" TargetControlID="dummyFuzzySearch"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlFuzzySearch" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid">
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
            </asp:Panel>--%>

            <%--Save/Undo changes popup--%>
            <asp:Button ID="dummySaveUndo" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeSaveUndo" runat="server" PopupControlID="pnlSaveUndo" TargetControlID="dummySaveUndo"
                CancelControlID="btnClosePopupSaveUndo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlSaveUndo" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td align="right" style="vertical-align: top;">
                            <asp:ImageButton ID="btnClosePopupSaveUndo" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMessage" runat="server" CssClass="identifierLable"
                                Text="You have made changes which are not saved. Save or Undo changes"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnSaveChanges" runat="server" Text="Save" CssClass="ButtonStyle"
                                            OnClick="btnSaveChanges_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnUndoChanges" runat="server" Text="Undo" CssClass="ButtonStyle" OnClick="btnUndoChanges_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:Button ID="dummyButton1" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeLinkedRoyaltors" runat="server" PopupControlID="pnlLinkedRoyaltors" TargetControlID="dummyButton1"
                CancelControlID="btnClosePopupLinkedRoy" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlLinkedRoyaltors" runat="server" align="center" Width="30%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td width="10%"></td>
                                    <td class="identifierLable" align="center">Linked Royaltors
                                    </td>
                                    <td align="right" style="vertical-align: top;" width="10%">
                                        <asp:ImageButton ID="btnClosePopupLinkedRoy" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td width="5%"></td>
                                    <td align="left">
                                        <table width="95%" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td width="40%" class="gridHeaderStyle_1row">Royaltor Id</td>
                                                <td width="60%" class="gridHeaderStyle_1row">Royaltor Name</td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td align="left">
                                        <asp:Panel ID="pnlLinkedRoyaltorsPopUp" runat="server" ScrollBars="Auto" Width="100%">
                                            <asp:GridView ID="gvLinkedRoyaltors" runat="server" AutoGenerateColumns="False" Width="95%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                EmptyDataText="No data found." ShowHeader="False">
                                                <Columns>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblRoyaltorId" runat="server" Text='<%# Bind("royaltor_id") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="40%" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblRoyaltorName" runat="server" Text='<%# Bind("royaltor_name") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="60%" />
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

            <asp:Button ID="dummyButton3" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeSuppliers" runat="server" PopupControlID="pnlSuppliers" TargetControlID="dummyButton3"
                CancelControlID="btnClosePopupSuppliers" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlSuppliers" runat="server" align="center" Width="40%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td width="10%"></td>
                                    <td class="identifierLable" align="center">Supplier
                                    </td>
                                    <td align="right" style="vertical-align: top;" width="10%">
                                        <asp:ImageButton ID="btnClosePopupSuppliers" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td width="5%"></td>
                                    <td align="left">
                                        <table width="95%" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td width="30%" class="gridHeaderStyle_1row">Supplier Number</td>
                                                <td width="30%" class="gridHeaderStyle_1row">Supplier Site Name</td>
                                                <td width="40%" class="gridHeaderStyle_1row">Supplier Name</td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td align="left">
                                        <asp:Panel ID="pnlSuppliersPopUp" runat="server" ScrollBars="Auto" Width="100%">
                                            <asp:GridView ID="gvSupplierDetails" runat="server" AutoGenerateColumns="False" Width="95%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                EmptyDataText="No data found." ShowHeader="False">
                                                <Columns>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSupplierNumber" runat="server" Text='<%# Bind("supplier_number") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="30%" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSupplierSiteName" runat="server" Text='<%# Bind("supplier_site_name") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="30%" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSupplierName" runat="server" Text='<%# Bind("supplier_name") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="40%" />
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

            <%--<asp:Button ID="dummyButton2" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpePayeeBankDetails" runat="server" PopupControlID="pnlmpePayeeBankDetails" TargetControlID="dummyButton2"
                CancelControlID="btnClosePopupBankDetails" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlmpePayeeBankDetails" runat="server" align="center" Width="40%" CssClass="popupPanel">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td width="10%"></td>
                                    <td class="identifierLable" align="center">Bank Details
                                    </td>
                                    <td align="right" style="vertical-align: top;" width="10%">
                                        <asp:ImageButton ID="btnClosePopupBankDetails" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Panel ID="pnlmpePayeeBankDetailsPopUp" runat="server" ScrollBars="Auto" Width="100%">
                                <table width="90%" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <table width="100%">
                                                <tr>
                                                    <td width="40%"></td>
                                                    <td width="60%"></td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">VAT Number</td>
                                                    <td>
                                                        <asp:TextBox ID="txtVatNumber" runat="server" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" Width="99%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">Supplier Number</td>
                                                    <td>
                                                        <asp:TextBox ID="txtSupplierNumber" runat="server" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" Width="99%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">Domestic / Foreign Payment</td>
                                                    <td>
                                                        <asp:TextBox ID="txtPaymentType" runat="server" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" Width="99%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">Payment Method</td>
                                                    <td>
                                                        <asp:TextBox ID="txtPaymentMethod" runat="server" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" Width="99%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr runat="server" id="trDomestic">
                                        <td>
                                            <table width="100%">
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left" width="40%">Bank Name</td>
                                                    <td width="60%">
                                                        <asp:TextBox ID="txtBankNameDom" runat="server" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" Width="99%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">Bank Address</td>
                                                    <td>
                                                        <asp:TextBox ID="txtBankAddressDom" runat="server" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" Width="99%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">Account Name</td>
                                                    <td>
                                                        <asp:TextBox ID="txtAccountNameDom" runat="server" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" Width="99%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">Sort Code</td>
                                                    <td>
                                                        <asp:TextBox ID="txtSortCodeDom" runat="server" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" Width="99%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">Account Number</td>
                                                    <td>
                                                        <asp:TextBox ID="txtAccountNumberDom" runat="server" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" Width="99%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">Vendor Site Code</td>
                                                    <td>
                                                        <asp:TextBox ID="txtVendorSiteCodeDom" runat="server" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" Width="99%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr runat="server" id="trForeign">
                                        <td>
                                            <table width="100%">
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left" width="40%">Account Name</td>
                                                    <td width="60%">
                                                        <asp:TextBox ID="txtAccountNameFrgn" runat="server" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" Width="99%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">Account Number</td>
                                                    <td>
                                                        <asp:TextBox ID="txtAccountNumberFrgn" runat="server" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" Width="99%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">IBAN</td>
                                                    <td>
                                                        <asp:TextBox ID="txtIBANFrgn" runat="server" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" Width="99%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">Swift Code</td>

                                                    <td>
                                                        <asp:TextBox ID="txtSwiftCodeFrgn" runat="server" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" Width="99%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">ABA Routing No.</td>
                                                    <td>
                                                        <asp:TextBox ID="txtABARoutingFrgn" runat="server" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" Width="99%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">Bank Address</td>
                                                    <td>
                                                        <asp:TextBox ID="txtBankAddressFrgn" runat="server" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" Width="99%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">Currency</td>
                                                    <td>
                                                        <asp:TextBox ID="txtCurrencyFrgn" runat="server" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" Width="99%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="identifierLable_large_bold" align="left">Vendor Site Code</td>
                                                    <td>
                                                        <asp:TextBox ID="txtVendorSiteCodeFrgn" runat="server" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" Width="99%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </asp:Panel>--%>

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

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnPageNumber" runat="server" Value="" />
            <asp:HiddenField ID="hdnIsValidSearch" runat="server" Value="N" />
            <asp:HiddenField ID="hdnChangeNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnInsertDataNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridRowSelectedPrvious" runat="server" />
            <asp:HiddenField ID="hdnIntPartyType" runat="server" Value="" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:Button ID="btnHdnInterestedPartySearch" runat="server" Style="display: none;" OnClick="btnHdnInterestedPartySearch_Click" CausesValidation="false" />
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" ReadOnly="true" CssClass="gridTextField"></asp:TextBox>
            <asp:HiddenField ID="hdninterestedPartyCode" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

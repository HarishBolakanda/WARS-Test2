<%@ Page Title="WARS - Tax Rates" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="TaxRateMaintanance.aspx.cs" Inherits="WARS.DataMaintenance.TaxRateMaintanance" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //Navigation button functionality - Start
        //to open Royaltor Search
        function OpenContractMaintenance() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../Contract/RoyaltorSearch.aspx');

            }
            else {
                var win = window.open('../Contract/RoyaltorSearch.aspx', '_self');
                win.focus();
                return true;
            }

        }
        //================================End


    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnContractMaintenance" runat="server" Text="Contract Maintenance"
                            CssClass="LinkButtonStyle" Width="98%" OnClientClick="if (!OpenContractMaintenance()) { return false;};" UseSubmitBehavior="false" />
                    </td>
                </tr>

            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>



<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">

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
            if (postBackElementID.lastIndexOf('imgBtnSave') > 0 || postBackElementID.lastIndexOf('imgBtnUndo') > 0 || postBackElementID.lastIndexOf('btnSaveChanges') > 0 || postBackElementID.lastIndexOf('btnUndoChanges') > 0) {
                xPos = sender._scrollPosition.x;
                yPos = sender._scrollPosition.y;

                //hold scroll position on selecting expand/collapse
                var PnlExchangeRateDetails = document.getElementById("<%=PnlTaxRateDetails.ClientID %>");
                scrollTop = PnlExchangeRateDetails.scrollTop;

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
            if (postBackElementID.lastIndexOf('imgBtnSave') > 0 || postBackElementID.lastIndexOf('imgBtnUndo') > 0 || postBackElementID.lastIndexOf('btnSaveChanges') > 0 || postBackElementID.lastIndexOf('btnUndoChanges') > 0) {
                window.scrollTo(xPos, yPos);

                //set scroll position on selecting expand/collapse
                var PnlExchangeRateDetails = document.getElementById("<%=PnlTaxRateDetails.ClientID %>");
                PnlExchangeRateDetails.scrollTop = scrollTop;
            }


        }
        //======================= End


        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.50;
            document.getElementById("<%=PnlTaxRateDetails.ClientID %>").style.height = gridPanelHeight + "px";
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

        //Validate any unsaved data on browser window close/refresh
        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        //=============== End

        //Audit button navigation
        function RedirectToAuditScreen() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                alert("Audit Screen to be developed");
            }
            else {
                alert("Audit Screen to be developed");
            }
        }


        var taxRateRegex = /^(100(\.0{1,2})?|^\d{0,2}(\.\d{1,2})?) *%?$/; //only positive number <= 100 upto 2 decimal places

        function ValidateTaxRate(sender, args) {
            var txtTaxRate = document.getElementById("<%=txtTaxRate.ClientID %>").value;
            var valtxtTaxRate = document.getElementById("<%=valtxtTaxRate.ClientID %>");
            //share % is mandatory if type is payee
            if (txtTaxRate == "") {
                args.IsValid = false;
                valtxtTaxRate.title = "Please enter tax rate %";
            }
            else {
                if (taxRateRegex.test(txtTaxRate)) {
                    args.IsValid = true;
                }
                else {
                    args.IsValid = false;
                    valtxtTaxRate.title = "Please enter only positive number <= 100 upto 2 decimal places";
                }
            }
        }

        function ValidateTaxRateGriRow(sender, args) {
            var gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            var tempstr = sender.id.substring(0, sender.id.lastIndexOf('_'));
            var str = tempstr.substring(0, tempstr.lastIndexOf('_') + 1);
            var txtTaxRateGridRow = document.getElementById(str + 'txtTaxRateGridRow' + '_' + gridRowIndex).value;
            var valTaxRateGridRow = document.getElementById(str + 'valTaxRateGridRow' + '_' + gridRowIndex);
            if (txtTaxRateGridRow == "") {
                args.IsValid = false;
                valTaxRateGridRow.title = "Please enter tax rate %";
            }
            else {
                if (taxRateRegex.test(txtTaxRateGridRow)) {
                    args.IsValid = true;
                }

                else {
                    args.IsValid = false;
                    valTaxRateGridRow.title = "Please enter only positive number <= 100 upto 2 decimal places";
                }
            }

        }

        //set flag value when data is changed in textboxes for inserting new row

        function OnDataChangeInsert() {
            CompareRowInsert();
        }

        function CompareRowInsert() {
            var txtStartDate = document.getElementById("<%=txtStartDate.ClientID %>").value;
            var txtEndDate = document.getElementById("<%=txtEndDate.ClientID %>").value;
            var ddlTaxType = document.getElementById("<%=ddlTaxType.ClientID %>").value;
            var txtTaxRate = document.getElementById("<%=txtTaxRate.ClientID %>").value;

            if (txtStartDate != "__/____" || ddlTaxType != "-" || txtTaxRate != "") {
                document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").innerText = "Y";
            }
            else {
                document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").innerText = "N";
            }

        }

        //clear add row data
        function ClearAddRow() {
            //debugger;
            document.getElementById('<%=txtStartDate.ClientID%>').value = "";
            document.getElementById('<%=txtEndDate.ClientID%>').value = "";
            document.getElementById('<%=ddlTaxType.ClientID%>').value = "-";
            document.getElementById('<%=txtTaxRate.ClientID%>').value = "";
            document.getElementById('<%=hdnInsertDataNotSaved.ClientID%>').value = 'N';
            Page_ClientValidate('');//clear all validators of the page
            document.getElementById("<%= txtStartDate.ClientID %>").focus();
            return false;

        }


        function UndoTaxRateGridRow(row) {
            var rowIndex = row.id.substring(row.id.lastIndexOf('_') + 1);
            var tempstr = row.id.substring(0, row.id.lastIndexOf('_'));
            var str = tempstr.substring(0, tempstr.lastIndexOf('_') + 1);
            var hdnEndDateGridRow = document.getElementById(str + 'hdnEndDateGridRow' + '_' + rowIndex).value;
            var txtEndDateGridRow = document.getElementById(str + 'txtEndDateGridRow' + '_' + rowIndex);
            var hdnTaxRateGridRow = document.getElementById(str + 'hdnTaxRateGridRow' + '_' + rowIndex).value;
            var txtTaxRateGridRow = document.getElementById(str + 'txtTaxRateGridRow' + '_' + rowIndex);
            if (txtEndDateGridRow.value != hdnEndDateGridRow || txtTaxRateGridRow.value != hdnTaxRateGridRow) {
                if (hdnEndDateGridRow != "") {
                    txtEndDateGridRow.value = hdnEndDateGridRow;
                }
                else {
                    txtEndDateGridRow.value = "__/____";
                }
                txtTaxRateGridRow.value = hdnTaxRateGridRow;
                ValidatorValidate(document.getElementById(str + 'valTaxRateGridRow' + '_' + rowIndex));
                ValidatorValidate(document.getElementById(str + 'valEndDateGridRow' + '_' + rowIndex));
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value = "N";
                document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value = "";
            }
            return false;

        }


        //set flag value when data is changed in grid 
        function OnDataChange(row) {
            CompareRow(row);
        }


        function CompareRow(row) {
            var rowIndex = row.id.substring(row.id.lastIndexOf('_') + 1);
            var tempstr = row.id.substring(0, row.id.lastIndexOf('_'));
            var str = tempstr.substring(0, tempstr.lastIndexOf('_') + 1);

            var hdnTaxRateGridRow = document.getElementById(str + 'hdnTaxRateGridRow_' + rowIndex).value;
            var txtTaxRateGridRow = document.getElementById(str + 'txtTaxRateGridRow_' + rowIndex).value;

            var hdnEndDateGridRow = document.getElementById(str + 'hdnEndDateGridRow_' + rowIndex).value;
            var txtEndDateGridRow = document.getElementById(str + 'txtEndDateGridRow_' + rowIndex).value;

            if ((hdnTaxRateGridRow != txtTaxRateGridRow) || (hdnEndDateGridRow != txtEndDateGridRow)) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
            }
            else {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "N";
            }

        }





        //Validation: warning message if changes made and not saved

        function OnGridRowSelected(row) {
            var rowData = row.parentNode.parentNode;
            var rowIndex = rowData.rowIndex - 1;
            if (document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value == "Y") {
                var popup = $find('<%= mpeSaveUndo.ClientID %>');
                if (popup != null) {
                    document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new tax rate. Save or Undo changes";
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
        var rowIndex = rowData.rowIndex -1;

        if (document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value == "Y") {
            var popup = $find('<%= mpeSaveUndo.ClientID %>');
            if (popup != null) {
                document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new tax rate. Save or Undo changes";
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

function ValidateSave() {
    var hdnInsertDataNotSaved = document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value;
    var hdnChangeNotSaved = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
    var hdnGridRowSelectedPrvious = document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value;
    if (hdnInsertDataNotSaved == "Y") {
        if (!Page_ClientValidate("valInsertTaxRate")) {
            Page_BlockSubmit = false;
            var popup = $find('<%= mpeSaveUndo.ClientID %>');
            if (popup != null) {
                popup.hide();
            }
            DisplayMessagePopup("Tax rate details not saved  – invalid or missing data!");

            return false;
        }
        else {
            return true;
        }
    }
    if (hdnChangeNotSaved == "Y") {
        if (!Page_ClientValidate("GroupUpdate_" + hdnGridRowSelectedPrvious + "")) {
            Page_BlockSubmit = false;
            DisplayMessagePopup("Tax rate details not saved  – invalid or missing data!");
            var popup = $find('<%= mpeSaveUndo.ClientID %>');
            if (popup != null) {
                popup.hide();
            }
            return false;
        }
        else {
            return true;
        }
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
//============== End  
    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="14">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    TAX RATES
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="1%"></td>
                    <td width="6%" class="identifierLable_large_bold">Company</td>
                    <td colspan="10">
                        <asp:DropDownList ID="ddlCompany" runat="server" Width="18%" CssClass="ddlStyle" AutoPostBack="true"
                            OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged" TabIndex="100">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator runat="server" ID="rfddlCompany"
                            ControlToValidate="ddlCompany" ValidationGroup="valInsertTaxRate"
                            Text="*" CssClass="requiredFieldValidator" InitialValue="-" ToolTip="Please select company from list" Display="Dynamic"></asp:RequiredFieldValidator>
                    </td>
                    <td></td>
                    <td width="40%" align="right">
                        <asp:Button ID="btnAudit" runat="server" CssClass="ButtonStyle" OnClientClick="if (!RedirectToAuditScreen()) { return false;};"
                            Text="Audit" Width="35%" UseSubmitBehavior="false" />

                    </td>
                </tr>
                <tr>
                    <td width="1%"></td>
                    <td width="6%" class="identifierLable_large_bold">Start Date</td>
                    <td width="8%">
                        <asp:TextBox ID="txtStartDate" runat="server" Width="72" CssClass="textboxStyle" TabIndex="101" onchange="javascript: OnDataChangeInsert();"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ID="rfStartDate" ControlToValidate="txtStartDate" ValidationGroup="valInsertTaxRate" InitialValue="__/____"
                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter a valid date in MM/YYYY format" Display="Dynamic"></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="valStartDate" runat="server" ValidationGroup="valInsertTaxRate" CssClass="requiredFieldValidator"
                            OnServerValidate="valStartDate_ServerValidate" ErrorMessage="*" ToolTip="Please enter a valid date in MM/YYYY format">
                        </asp:CustomValidator>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmeStartDate" runat="server" TargetControlID="txtStartDate"
                            WatermarkText="mm/yyyy" WatermarkCssClass="waterMarkText">
                        </ajaxToolkit:TextBoxWatermarkExtender>
                        <ajaxToolkit:MaskedEditExtender ID="mteStartDate" runat="server"
                            TargetControlID="txtStartDate" Mask="99/9999" AcceptNegative="None"
                            ClearMaskOnLostFocus="false" />

                    </td>
                    <td width="1%"></td>
                    <td width="6%" class="identifierLable_large_bold">End Date</td>
                    <td width="8%">
                        <asp:TextBox ID="txtEndDate" runat="server" Width="72" CssClass="textboxStyle" onchange="javascript: OnDataChangeInsert();" onfocus="return ConfirmInsert();"
                            TabIndex="102"></asp:TextBox>
                        <asp:CustomValidator ID="valEndDate" runat="server" ValidationGroup="valInsertTaxRate" CssClass="requiredFieldValidator"
                            OnServerValidate="valEndDate_ServerValidate" ErrorMessage="*" ToolTip="Please enter a valid date in MM/YYYY format">
                        </asp:CustomValidator>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmeEndDate" runat="server" TargetControlID="txtEndDate"
                            WatermarkText="mm/yyyy" WatermarkCssClass="waterMarkText">
                        </ajaxToolkit:TextBoxWatermarkExtender>
                        <ajaxToolkit:MaskedEditExtender ID="mteEndDate" runat="server"
                            TargetControlID="txtEndDate" Mask="99/9999" AcceptNegative="None"
                            ClearMaskOnLostFocus="false" />

                    </td>
                    <td width="1%"></td>
                    <td width="6%" class="identifierLable_large_bold">Tax Type</td>
                    <td width="8%">
                        <asp:DropDownList ID="ddlTaxType" runat="server" Width="80%" CssClass="ddlStyle" onchange="javascript: OnDataChangeInsert();" onfocus="return ConfirmInsert();"
                            TabIndex="103">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator runat="server" ID="rfddlTaxType"
                            ControlToValidate="ddlTaxType" ValidationGroup="valInsertTaxRate"
                            Text="*" CssClass="requiredFieldValidator" InitialValue="-" ToolTip="Please select tax type from list" Display="Dynamic"></asp:RequiredFieldValidator>

                    </td>
                    <td width="1%"></td>
                    <td width="6%" class="identifierLable_large_bold">Tax Rate</td>
                    <td width="8%">
                        <asp:TextBox ID="txtTaxRate" runat="server" Width="80%" TabIndex="104" onchange="javascript: OnDataChangeInsert();" onfocus="return ConfirmInsert();"
                            CssClass="textboxStyle" MaxLength="6"></asp:TextBox>
                        <asp:CustomValidator ID="valtxtTaxRate" runat="server" ValidationGroup="valInsertTaxRate" CssClass="requiredFieldValidator"
                            ClientValidationFunction="ValidateTaxRate" ToolTip="Please enter tax rate %" Display="Dynamic"
                            ErrorMessage="*"></asp:CustomValidator>
                    </td>
                    <td width="3%">
                        <table width="100%" style="float: right; table-layout: fixed">
                            <tr style="float: right">
                                <td align="right" style="float: right" width="50%">
                                    <asp:ImageButton ID="imgBtnInsert" runat="server" CommandName="saverow" TabIndex="105" ImageUrl="../Images/save.png" ToolTip="Insert tax rate" ValidationGroup="valInsertTaxRate" OnClick="imgBtnInsert_Click" onfocus="return ConfirmInsert();" />
                                </td>
                                <td align="right" style="float: right" width="50%">
                                    <asp:ImageButton ID="imgBtnCancel" runat="server" CommandName="cancelrow" TabIndex="106" ImageUrl="../Images/cancel_row3.png"
                                        ToolTip="Cancel" OnClientClick="return ClearAddRow();" OnKeyDown="OnTabPress()" onfocus="return ConfirmInsert();" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="14">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td colspan="11" runat="server" id="tdData">
                        <table width="60%" cellpadding="0" cellspacing="0" align="left">
                            <tr>
                                <td>
                                    <asp:Panel ID="PnlTaxRateDetails" runat="server" ScrollBars="Auto" Width="100%">
                                        <asp:GridView ID="gvTaxRateDetails" runat="server" AutoGenerateColumns="False" Width="97.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                            CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                            EmptyDataText="No data found." OnRowDataBound="gvTaxRateDetails_RowDataBound" OnRowCommand="gvTaxRateDetails_RowCommand" AllowSorting="true" OnSorting="gvTaxRateDetails_Sorting" HeaderStyle-CssClass="FixedHeader">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Start Date" SortExpression="start_date">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnCompanyCode" runat="server" Value='<%# Bind("company_code") %>' />
                                                        <asp:HiddenField ID="hdnTaxNo" runat="server" Value='<%# Bind("tax_no") %>' />
                                                        <asp:Label ID="lblStartDateGridRow" runat="server" Text='<%# Bind("start_date") %>' CssClass="identifierLable"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="20%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="End Date" SortExpression="end_date">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnEndDateGridRow" runat="server" Value='<%# Bind("end_date") %>' />
                                                        <asp:TextBox ID="txtEndDateGridRow" runat="server" Width="72" CssClass="gridTextField" onfocus="OnGridRowSelected(this)" onchange="javascript: OnDataChange(this);"
                                                            Text='<%# Bind("end_date") %>' Style="text-align: center"></asp:TextBox>
                                                        <asp:CustomValidator ID="valEndDateGridRow" ControlToValidate="txtEndDateGridRow" runat="server" CssClass="requiredFieldValidator" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>'
                                                            OnServerValidate="valEndDateGridRow_ServerValidate" ErrorMessage="*" ToolTip="Please enter a valid date in MM/YYYY format">
                                                        </asp:CustomValidator>
                                                        <ajaxToolkit:MaskedEditExtender ID="mteEndDateGridRow" runat="server"
                                                            TargetControlID="txtEndDateGridRow" Mask="99/9999" AcceptNegative="None"
                                                            ClearMaskOnLostFocus="false" />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="20%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Tax Type" SortExpression="tax_type">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTaxTypeGridRow" runat="server" Text='<%# Bind("tax_type") %>' CssClass="identifierLable"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="20%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Tax Rate" SortExpression="tax_rate">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnTaxRateGridRow" runat="server" Value='<%# Bind("tax_rate") %>' />
                                                        <asp:TextBox ID="txtTaxRateGridRow" runat="server" Text='<%# Eval("tax_rate") %>'
                                                            CssClass="gridTextField" Width="30%" Style="text-align: center" MaxLength="6" onchange="javascript: OnDataChange(this);" onfocus="OnGridRowSelected(this)"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ID="rfTaxRateGridRow"
                                                            ControlToValidate="txtTaxRateGridRow" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>'
                                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter tax rate %" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:CustomValidator ID="valTaxRateGridRow" runat="server" CssClass="requiredFieldValidator" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>'
                                                            ClientValidationFunction="ValidateTaxRateGriRow" ControlToValidate="txtTaxRateGridRow" ToolTip="Please enter tax rate %" Display="Static"
                                                            ErrorMessage="*"></asp:CustomValidator>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="30%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Right_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderStyle-Width="10%">
                                                    <ItemTemplate>
                                                        <table width="100%" style="float: right; table-layout: fixed">
                                                            <tr style="float: right">
                                                                <td align="right" style="float: right" width="50%">
                                                                    <asp:ImageButton ID="imgBtnSave" runat="server" CommandName="saverow" ImageUrl="../Images/save.png" ToolTip="Save" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>' CausesValidation="true" onfocus="return ConfirmUpdate(this)" />
                                                                </td>
                                                                <td align="right" style="float: right" width="50%">
                                                                    <asp:ImageButton ID="imgBtnUndo" runat="server" ImageUrl="../Images/cancel_row3.png" OnClientClick="return UndoTaxRateGridRow(this);" onfocus="return ConfirmUpdate(this)"
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
                                <td align="center">
                                    <asp:Repeater ID="rptPager" runat="server">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>'
                                                ClientIDMode="AutoID" CausesValidation="false" Enabled='<%# Eval("Enabled") %>' CssClass="gridPager" OnClick="lnkPage_Click" OnClientClick="return ValidateChanges();" onfocus="return ConfirmUpdate(this)"> </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

            <msg:MsgControl ID="msgView" runat="server" />
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
                                        <asp:Button ID="btnSaveChanges" runat="server" Text="Save" CssClass="ButtonStyle" OnClick="btnSaveChanges_Click" OnClientClick="if (!ValidateSave()) { return false;};" />
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

            <asp:HiddenField ID="hdnPageNumber" runat="server" Value="" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnChangeNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnInsertDataNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridRowSelectedPrvious" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99"></asp:Label>
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

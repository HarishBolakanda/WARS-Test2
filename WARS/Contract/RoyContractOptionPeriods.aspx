<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoyContractOptionPeriods.aspx.cs" Inherits="WARS.Contract.RoyContractOptionPeriods" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Royaltor Contract Option Periods" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

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

        //clear add row data
        function ClearAddRow() {
            //debugger;
            document.getElementById('<%=txtDescAddRow.ClientID%>').value = "";
            document.getElementById('<%=txtPLGConAddRow.ClientID%>').value = "";
            document.getElementById('<%=ddlUnitFieldAddRow.ClientID%>').selectedIndex = 1;
            document.getElementById('<%=ddlPriceFieldAddRow.ClientID%>').selectedIndex = 2;
            document.getElementById('<%=ddlReceiptFieldAddRow.ClientID%>').selectedIndex = 1;
            document.getElementById('<%=rfvddlUnitFieldAddRow.ClientID%>').IsValid = true;
            document.getElementById('<%=rfvddlPriceFieldAddRow.ClientID%>').IsValid = true;
            document.getElementById('<%=rfvddlReceiptFieldAddRow.ClientID%>').IsValid = true;


        }
        //============== End

        //Confim delete
        function ConfirmDelete(row) {
            //debugger;
            //JIRA -908 CHanges by Ravi on 12/02/2019 -- Start
            gridRowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            document.getElementById("<%= hdnOptionDeleteRowIndex.ClientID %>").value = gridRowIndex;
            var popup = $find('<%= mpeConfirmDeletePopup.ClientID %>');
            if (popup != null) {
                popup.show();
            }
            return false;
            //JIRA -908 CHanges by Ravi on 12/02/2019 -- End

        }
        //============== End

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= txtOptionAddRow.ClientID %>").focus();
            }
        }

        function FocusLblKeyPress() {
            document.getElementById("<%= txtRoyaltorId.ClientID %>").focus();
        }

        //=============== End

        //Validate any unsaved data on browser window close/refresh
        //set flag value when data is changed
        // discard these fields with default values
        function IsAddRowDataChanged() {
            //debugger;
            var txtDescAddRow = document.getElementById("<%=txtDescAddRow.ClientID %>").value;
            var txtPLGConAddRow = document.getElementById("<%=txtPLGConAddRow.ClientID %>").value;

            if (txtDescAddRow != '' || txtPLGConAddRow != '') {
                document.getElementById("<%=hdnAddRowDataChanged.ClientID %>").innerText = "Y";
            }
            else {
                document.getElementById("<%=hdnAddRowDataChanged.ClientID %>").innerText = "N";
            }
        }

        function OnGridDataChange(row, name) {
            //debugger;
            //var selectedRowIndex = row.id.substring(row.id.length - 1);
            var selectedRowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            CompareGridData(selectedRowIndex);
        }

        function CompareGridData(rowIndex) {
            var str = "ContentPlaceHolderBody_gvContOptionPeriod_";
            var hdnOptPeriodDesc = document.getElementById(str + 'hdnOptPeriodDesc' + '_' + rowIndex).value;
            var txtDescription = document.getElementById(str + 'txtDescription' + '_' + rowIndex).value;
            var hdnPlgOptPeriod = document.getElementById(str + 'hdnPlgOptPeriod' + '_' + rowIndex).value;
            var txtPlgContractVersion = document.getElementById(str + 'txtPlgContractVersion' + '_' + rowIndex).value;
            var hdnUnitField = document.getElementById(str + 'hdnUnitField' + '_' + rowIndex).value;
            var ddlUnitField = document.getElementById(str + 'ddlUnitField' + '_' + rowIndex).value;
            var hdnPriceField = document.getElementById(str + 'hdnPriceField' + '_' + rowIndex).value;
            var ddlPriceField = document.getElementById(str + 'ddlPriceField' + '_' + rowIndex).value;
            var hdnReceiptField = document.getElementById(str + 'hdnReceiptField' + '_' + rowIndex).value;
            var ddlReceiptField = document.getElementById(str + 'ddlReceiptField' + '_' + rowIndex).value;
            var hdnIsModified = document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).value;

            //debugger;
            if (hdnOptPeriodDesc != txtDescription || hdnPlgOptPeriod != txtPlgContractVersion || hdnUnitField != ddlUnitField || hdnPriceField != ddlPriceField || hdnReceiptField != ddlReceiptField) {
                if (hdnIsModified != "-") {
                    document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).innerText = "Y";
                }
            }
            else {
                document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).innerText = "N";
            }

        }

        function IsGridDataChanged() {
            var str = "ContentPlaceHolderBody_gvContOptionPeriod_";
            var gvContOptionPeriod = document.getElementById("<%= gvContOptionPeriod.ClientID %>");
            if (gvContOptionPeriod != null) {
                var gvRows = gvContOptionPeriod.rows; // WUIN-746 grid view rows including header row
                var isModified;
                var isGridDataChanged = "N";
                var rowIndex;
                for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                    rowIndex = i - 1; //WUIN-746 row index start from 0
                    if (document.getElementById(str + 'hdnIsModified' + '_' + rowIndex) != null) {
                        isModified = document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).value;
                        if (isModified == "Y" || isModified == "-" || isModified == "C") {
                            isGridDataChanged = "Y";
                            break;
                        }
                    }

                }

                if (isGridDataChanged == "Y") {
                    document.getElementById("<%=hdnGridDataChanged.ClientID %>").value = "Y";
                }
                else {
                    if (document.getElementById("<%=hdnGridDataDeleted.ClientID %>").value != "Y") {
                        document.getElementById("<%=hdnGridDataChanged.ClientID %>").value = "N";
                    }
                    else {
                        document.getElementById("<%=hdnGridDataChanged.ClientID %>").value = "Y";
                    }
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
            window.location = "../Contract/RoyContractRoyRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y";
        }

        function WarnOnUnSavedData() {
            //debugger;
            IsGridDataChanged();
            IsAddRowDataChanged();
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            var isGridDataChanged = document.getElementById("<%=hdnGridDataChanged.ClientID %>").value;
            var isAddRowDataChanged = document.getElementById("<%=hdnAddRowDataChanged.ClientID %>").value;
            var isNewRoyaltorSaved = document.getElementById("<%=hdnNewRoyaltorSaved.ClientID %>").value;
            var isContractScreen = document.getElementById("hdnIsContractScreen").value;
            var isNotContractScreen = document.getElementById("hdnIsNotContractScreen").value;
            if (isExceptionRaised != "Y" && isNewRoyaltorSaved != "Y" && isContractScreen != "Y" && isNotContractScreen != "Y") {
                if (isGridDataChanged == "Y" || isAddRowDataChanged == "Y") {
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
            //debugger;
            IsGridDataChanged();
            IsAddRowDataChanged();
            var isGridDataChanged = document.getElementById("<%=hdnGridDataChanged.ClientID %>").value;
            var isAddRowDataChanged = document.getElementById("<%=hdnAddRowDataChanged.ClientID %>").value;
            if (isGridDataChanged == "Y" || isAddRowDataChanged == "Y") {
                return true;
            }
            else {
                return false;
            }
        }

        //=================End

        //Audit button navigation
        function RedirectToAuditScreen() {
            document.getElementById('<%=hdnIsAuditScreen.ClientID%>').value = "Y";
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                // OpenPopupOnUnSavedData("../Contract/Audit.aspx"); -- to be developed
                alert("Audit Screen to be developed");
            }
            else {
                //window.location = "../Contract/Audit.aspx";
                alert("Audit Screen to be developed");
            }
        }

        function RedirectToPreviousScreen(royaltorId) {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Contract/RoyContractPayeeSupp.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y");
            }
            else {
                window.location = "../Contract/RoyContractPayeeSupp.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y";
            }
        }
        //=================End

        //prevent page navigation to previous page on back space key press
        function preventBackspace() {
            var evt = window.event;
            var keyCode = evt.keyCode;
            if (keyCode === 8) {
                evt.returnValue = false;
            }
        }

        // WUIN-1181 Changes
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
                                    ROYALTOR CONTRACT - OPTION PERIODS
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
                                                    Text="Save Changes" UseSubmitBehavior="false" Width="90%" ValidationGroup="valSave" TabIndex="109" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td align="right">
                                                <asp:Button ID="btnAudit" runat="server" CssClass="ButtonStyle" OnClick="btnAudit_Click"
                                                    Text="Audit" UseSubmitBehavior="false" Width="90%" TabIndex="110" onkeydown="OnTabPress();" />
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
                    <td class="table_header_with_border">Option Periods</td>
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
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvContOptionPeriod" runat="server" AutoGenerateColumns="False" Width="98.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found" OnRowDataBound="gvContOptionPeriod_RowDataBound"
                                                        OnRowCommand="gvContOptionPeriod_RowCommand" AllowSorting="true" OnSorting="gvContOptionPeriod_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Option" SortExpression="option_period_code"
                                                                ItemStyle-Width="7%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblOption" runat="server" Width="75%" Text='<%# Bind("option_period_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Description" SortExpression="option_period_desc"
                                                                ItemStyle-Width="23%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtDescription" runat="server" Width="95%" Text='<%#Bind("option_period_desc")%>' CssClass="gridTextField"
                                                                        MaxLength="30" onchange="OnGridDataChange(this,'');"></asp:TextBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="PLG Contract Version" SortExpression="plg_option_period"
                                                                ItemStyle-Width="14%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtPlgContractVersion" runat="server" Width="95%" Text='<%#Bind("plg_option_period")%>' CssClass="gridTextField"
                                                                        MaxLength="30" onchange="OnGridDataChange(this,'');"></asp:TextBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Unit field" SortExpression="unit_type"
                                                                ItemStyle-Width="14%">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlUnitField" runat="server" Width="85%" CssClass="ddlStyle" onchange="OnGridDataChange(this,'');"></asp:DropDownList>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvddlUnitField" ControlToValidate="ddlUnitField" ValidationGroup="valGrpSaveCopy"
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please select unit field" InitialValue="-" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Price field" SortExpression="price_type"
                                                                ItemStyle-Width="14%">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlPriceField" runat="server" Width="85%" CssClass="ddlStyle" onchange="OnGridDataChange(this,'');"></asp:DropDownList>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvddlPriceField" ControlToValidate="ddlPriceField" ValidationGroup="valGrpSaveCopy"
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please select price field" InitialValue="-" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Receipt field" SortExpression="receipt_type"
                                                                ItemStyle-Width="14%">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlReceiptField" runat="server" Width="85%" CssClass="ddlStyle" onchange="OnGridDataChange(this,'');"></asp:DropDownList>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvddlReceiptField" ControlToValidate="ddlReceiptField" ValidationGroup="valGrpSaveCopy"
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please select receipt field" InitialValue="-" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"
                                                                ItemStyle-Width="5%" HeaderStyle-Width="5%">
                                                                <ItemTemplate>
                                                                    <table width="100%" style="float: left; table-layout: fixed">
                                                                        <tr style="float: right">
                                                                            <td align="center">
                                                                                <asp:ImageButton ID="imgBtnDelete" runat="server" ImageUrl="../Images/Delete.gif"
                                                                                    ToolTip="Delete" OnClientClick="return ConfirmDelete(this);" />
                                                                            </td>
                                                                            <td align="center">
                                                                                <asp:ImageButton ID="imgBtnUndo" runat="server" CommandName="cancelRow" ImageUrl="../Images/cancel_row3.png"
                                                                                    ToolTip="Cancel" />
                                                                            </td>
                                                                            <td align="center">
                                                                                <asp:ImageButton ID="ImageCopy" runat="server" CommandName="copy" ImageUrl="../Images/Copy.png"
                                                                                    ToolTip="Copy" ValidationGroup="valGrpSaveCopy" OnClientClick="OnGridDataChange(this,'');" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                    <asp:HiddenField ID="hdnOptPeriodCode" runat="server" Value='<%# Bind("option_period_code") %>' />
                                                                    <asp:HiddenField ID="hdnOptPeriodCodeCopy" runat="server" Value='<%# Bind("option_period_code_copy") %>' />
                                                                    <asp:HiddenField ID="hdnOptPeriodDesc" runat="server" Value='<%# Bind("option_period_desc") %>' />
                                                                    <asp:HiddenField ID="hdnPlgOptPeriod" runat="server" Value='<%# Bind("plg_option_period") %>' />
                                                                    <asp:HiddenField ID="hdnUnitField" runat="server" Value='<%# Bind("unit_type") %>' />
                                                                    <asp:HiddenField ID="hdnPriceField" runat="server" Value='<%# Bind("price_type") %>' />
                                                                    <asp:HiddenField ID="hdnReceiptField" runat="server" Value='<%# Bind("receipt_type") %>' />
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
                                            <td width="7%" class="gridHeaderStyle_1row">Option</td>
                                            <td width="23%" class="gridHeaderStyle_1row">Description</td>
                                            <td width="13%" class="gridHeaderStyle_1row">PLG Contract Version</td>
                                            <td width="14%" class="gridHeaderStyle_1row">Unit field</td>
                                            <td width="14%" class="gridHeaderStyle_1row">Price field</td>
                                            <td width="14%" class="gridHeaderStyle_1row">Receipt Field</td>
                                            <td width="5%" class="gridHeaderStyle_1row">&nbsp</td>
                                        </tr>
                                        <tr>
                                            <td class="insertBoxStyle">
                                                <asp:TextBox ID="txtOptionAddRow" runat="server" Width="75%" CssClass="textboxStyle" TabIndex="100" MaxLength="2" ReadOnly="true" onkeydown="preventBackspace();"></asp:TextBox>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:TextBox ID="txtDescAddRow" runat="server" Width="90%" CssClass="textboxStyle" MaxLength="30" TabIndex="101"></asp:TextBox>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:TextBox ID="txtPLGConAddRow" runat="server" Width="90%" CssClass="textboxStyle" MaxLength="30" TabIndex="102"></asp:TextBox>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:DropDownList ID="ddlUnitFieldAddRow" runat="server" Width="85%" CssClass="ddlStyle" TabIndex="103">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvddlUnitFieldAddRow" ControlToValidate="ddlUnitFieldAddRow" ValidationGroup="valGrpAppendAddRow"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please select unit field" InitialValue="-" Display="Dynamic"></asp:RequiredFieldValidator>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:DropDownList ID="ddlPriceFieldAddRow" runat="server" Width="85%" CssClass="ddlStyle" TabIndex="104">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvddlPriceFieldAddRow" ControlToValidate="ddlPriceFieldAddRow" ValidationGroup="valGrpAppendAddRow"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please select price field" InitialValue="-" Display="Dynamic"></asp:RequiredFieldValidator>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:DropDownList ID="ddlReceiptFieldAddRow" runat="server" Width="85%" CssClass="ddlStyle" TabIndex="105">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvddlReceiptFieldAddRow" ControlToValidate="ddlReceiptFieldAddRow" ValidationGroup="valGrpAppendAddRow"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please select receipt field" InitialValue="-" Display="Dynamic"></asp:RequiredFieldValidator>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <table width="60%">
                                                    <tr>
                                                        <td>
                                                            <asp:ImageButton ID="btnAppendAddRow" runat="server" ImageUrl="../Images/add_row.png" Onkeydown="OnAppendAddRowKeyDown();" 
                                                                ToolTip="Add Option Period" OnClick="btnAppendAddRow_Click" ValidationGroup="valGrpAppendAddRow" TabIndex="107" />
                                                        </td>
                                                        <td>
                                                            <asp:ImageButton ID="btnUndoAddRow" runat="server" ImageUrl="../Images/cancel_row3.png"
                                                                ToolTip="Clear Add row" OnClientClick="ClearAddRow();" CausesValidation="false" TabIndex="108" Height="16px" />
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

            <%--Copy optin--%>
            <asp:Button ID="dummyCopyOptionPopup" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeCopyOptionPopup" runat="server" PopupControlID="pnlCopyOptionPopup" TargetControlID="dummyCopyOptionPopup"
                CancelControlID="btnCloseCopyOptionPopup" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlCopyOptionPopup" runat="server" align="left" Width="30%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="identifierLable" align="center">Copy Option
                                    </td>
                                    <td align="right" style="vertical-align: top;" width="10%">
                                        <asp:ImageButton ID="btnCloseCopyOptionPopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
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
                                    <td width="30%" class="identifierLable_large_bold">Description</td>
                                    <td>
                                        <asp:TextBox ID="txtDescriptionCopyOptPrd" runat="server" Width="90%" CssClass="textboxStyle" MaxLength="30"></asp:TextBox>
                                    </td>
                                </tr>

                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold">PLG Contract Version</td>
                                    <td>
                                        <asp:TextBox ID="txtPLGContCopyOptPrd" runat="server" Width="90%" CssClass="textboxStyle" MaxLength="30"></asp:TextBox>
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
                                                    <asp:Button ID="btnCopyOptPrdContinue" runat="server" CssClass="ButtonStyle" OnClick="btnCopyOptPrdContinue_Click"
                                                        Text="Continue" UseSubmitBehavior="false" Width="90%" ValidationGroup="valGrpCopyOptPrd" />
                                                </td>
                                                <td>
                                                    <asp:Button ID="btnCopyOptPrdCancel" runat="server" CssClass="ButtonStyle" OnClick="btnCopyOptPrdCancel_Click"
                                                        Text="Cancel" UseSubmitBehavior="false" Width="90%" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <asp:HiddenField ID="hdnCopyRowOptPrdCodeCopy" runat="server" />
                            <asp:HiddenField ID="hdnCopyRowUnitField" runat="server" />
                            <asp:HiddenField ID="hdnCopyRowPriceField" runat="server" />
                            <asp:HiddenField ID="hdnCopyRowReceiptField" runat="server" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Copy optin - Ends--%>

            <%--Warning popup--%>
            <asp:Button ID="dummyConfirmDelete" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmDelete" runat="server" PopupControlID="pnlPopupConfirmDelete" TargetControlID="dummyConfirmDelete"
                CancelControlID="btnConfirmDeleteCancel" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlPopupConfirmDelete" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmMsgHdr" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblConfirmMsg" runat="server" Text="Option Period has rates set up - do you want to continue?" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnConfirmDelete" runat="server" Text="Confirm" OnClick="btnConfirmDelete_Click" CssClass="ButtonStyle" />

                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnConfirmDeleteCancel" runat="server" Text="Cancel" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Warning popup - Ends--%>


            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- Start--%>
            <asp:Button ID="dummyConfirmDeletePopup" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmDeletePopup" runat="server" PopupControlID="pnlConfirmDeletePopup" TargetControlID="dummyConfirmDeletePopup"
                CancelControlID="btnNo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlConfirmDeletePopup" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
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
            <asp:HiddenField ID="hdnGridDataChanged" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridDataDeleted" runat="server" Value="N" />
            <asp:HiddenField ID="hdnAddRowDataChanged" runat="server" Value="N" />
            <asp:HiddenField ID="hdnOptPrdToDelete" runat="server" />
            <asp:HiddenField ID="hdnOptPrdToDeleteIsModified" runat="server" />
            <asp:HiddenField ID="hdnOptionPeriodCode" runat="server" />
            <asp:HiddenField ID="hdnOptionDeleteRowIndex" runat="server" />
            <asp:HiddenField ID="hdnIsAuditScreen" runat="server" Value="N" />
            <asp:HiddenField ID="hdnOtherUserScreenLocked" runat="server" Value="N" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
            <%--<asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField" onkeydown="FocusLblKeyPress();"></asp:TextBox>--%>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

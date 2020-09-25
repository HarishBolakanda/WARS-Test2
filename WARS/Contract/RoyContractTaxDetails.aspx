<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoyContractTaxDetails.aspx.cs" Inherits="WARS.Contract.RoyContractTaxDetails" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Royaltor Contract Tax Details" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

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
        var gridClientId = "ContentPlaceHolderBody_gvContTaxDetails_";
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
            if (postBackElementID.lastIndexOf('imgBtnDelete') > 0 || postBackElementID.lastIndexOf('imgBtnUndo') > 0 ||
                postBackElementID.lastIndexOf('btnSave') > 0 || postBackElementID.lastIndexOf('btnUndoAddRow') > 0) {
                xPos = sender._scrollPosition.x;
                yPos = sender._scrollPosition.y;

                //hold scroll position on selecting expand/collapse
                var PnlRoyContTaxDetails = document.getElementById("<%=PnlGrid.ClientID %>");
                scrollTop = PnlRoyContTaxDetails.scrollTop;

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
            if (postBackElementID.lastIndexOf('imgBtnDelete') > 0 || postBackElementID.lastIndexOf('imgBtnUndo') > 0 ||
              postBackElementID.lastIndexOf('btnSave') > 0 || postBackElementID.lastIndexOf('btnUndoAddRow') > 0) {
                window.scrollTo(xPos, yPos);

                //set scroll position on selecting expand/collapse
                var PnlRoyContTaxDetails = document.getElementById("<%=PnlGrid.ClientID %>");
                PnlRoyContTaxDetails.scrollTop = scrollTop;
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
            document.getElementById('<%=ddlIPNumber.ClientID%>').selectedIndex = 0;
            document.getElementById('<%=txtInterestedPartyName.ClientID%>').value = "";
            document.getElementById('<%=ddlTaxType.ClientID%>').selectedIndex = 0;
            document.getElementById('<%=txtTaxRate.ClientID%>').value = "";
            document.getElementById('<%=hdnInsertDataNotSaved.ClientID%>').value = "N";
            Page_ClientValidate('');//clear all validators of the page
            return false;
        }
        //============== End

        //Confim delete
        function ConfirmDelete(row) {
            //JIRA-908 Changes by Ravi on 01/01/2020 -- Start
            //set if this is not a newly added row
            var selectedRowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            var str = "ContentPlaceHolderBody_gvContTaxDetails_";
            var hdnRoyaltorTaxIntPartyId = document.getElementById(str + 'hdnRoyaltorTaxIntPartyId' + '_' + selectedRowIndex).value;
            var hdnRoyaltorTaxType = document.getElementById(str + 'hdnRoyaltorTaxType' + '_' + selectedRowIndex).value;
            var hdnIsModified = document.getElementById(str + 'hdnIsModified' + '_' + selectedRowIndex).value;
            if (hdnIsModified != "-") {
                document.getElementById("<%=hdnGridDataDeleted.ClientID %>").innerText = "Y";
            }
            document.getElementById("<%=hdnDeleteIntPartyId.ClientID %>").innerText = hdnRoyaltorTaxIntPartyId;
            document.getElementById("<%=hdnDeleteRoyTaxType.ClientID %>").innerText = hdnRoyaltorTaxType;
            document.getElementById("<%=hdnDeleteIsModified.ClientID %>").innerText = hdnIsModified;
            var popup = $find('<%= mpeConfirmDelete.ClientID %>');
            if (popup != null) {
                popup.show();
            }
            return false;
            //JIRA-908 Changes by Ravi on 01/01/2020 -- ENd
        }

        //============== End

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= ddlIPNumber.ClientID %>").focus();
            }
        }

        function FocusLblKeyPress() {
            document.getElementById("<%= txtRoyaltorId.ClientID %>").focus();
        }

        //=============== End
        function IsGridDataChanged() {

            if (document.getElementById("<%=hdnGridDataDeleted.ClientID %>").value == "Y") {
                return true;
            }

            var gridDataChanged = "N";
            var hdnRoyTaxRate;
            var txtRoyTaxRate;

            var str = "ContentPlaceHolderBody_gvContTaxDetails_";
            var gvContTaxDetails = document.getElementById("<%= gvContTaxDetails.ClientID %>");
            if (gvContTaxDetails != null) {
                var gvRows = gvContTaxDetails.rows; // WUIN-746 grid view rows including header row
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
                    hdnRoyTaxRate = document.getElementById(str + 'hdnTaxRate' + '_' + rowIndex).value;
                    txtRoyTaxRate = document.getElementById(str + 'txtTaxRate' + '_' + rowIndex).value;
                    if (hdnRoyTaxRate != txtRoyTaxRate) {
                        gridDataChanged = "Y";
                        break;
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
        //set flag value when data is changed
        // discard these fields with default values
        function IsAddRowDataChanged() {
            //debugger;
            var ddlIpNumber = document.getElementById("<%=ddlIPNumber.ClientID %>").value;
            var ddlTaxType = document.getElementById("<%=ddlTaxType.ClientID %>").value;
            var txtTaxRate = document.getElementById("<%=txtTaxRate.ClientID %>").value;

            if (ddlIpNumber != '-' || ddlTaxType != '-' || txtTaxRate != '') {
                document.getElementById("<%=hdnAddRowDataChanged.ClientID %>").innerText = "Y";
            }
            else {
                document.getElementById("<%=hdnAddRowDataChanged.ClientID %>").innerText = "N";
            }
        }

        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        //redirect to Royaltor Notes screen on saving data of new royaltor so that issue of data not saved validation would be handled
        function RedirectOnNewRoyaltorSave(royaltorId) {
            //debugger;
            document.getElementById("<%=hdnNewRoyaltorSaved.ClientID %>").value = "Y";
            window.location = "../Contract/RoyContractNotes.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y";
        }

        function WarnOnUnSavedData() {
            //debugger;
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            var isDataChangedInsert = document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value;
            var isNewRoyaltorSaved = document.getElementById("<%=hdnNewRoyaltorSaved.ClientID %>").value;
            var isContractScreen = document.getElementById("hdnIsContractScreen").value;
            var isNotContractScreen = document.getElementById("hdnIsNotContractScreen").value;
            if (isExceptionRaised != "Y" && (IsGridDataChanged() || IsAddRowDataChanged() || IsDataChanged() || isDataChangedInsert == "Y") && isNewRoyaltorSaved != "Y" && isContractScreen != "Y" && isNotContractScreen != "Y") {
                unSaveBrowserClose = true;
                return warningMsgOnUnSavedData;
            }
            UpdateScreenLockFlag();// WUIN-599 - Unset the screen lock flag If an user close the browser with out unsaved data or navigate to other than contract screens


        }
        window.onbeforeunload = WarnOnUnSavedData;

        var unSaveBrowserClose = false;
        function UpdateScreenLockFlag() {
            var isOtherUserScreenLocked = document.getElementById("<%=hdnOtherUserScreenLocked.ClientID %>").value;
            var isAuditScreen = document.getElementById("<%=hdnIsAuditScreen.ClientID %>").value;
            var isContractScreen = document.getElementById("hdnIsContractScreen").value;
            if (isOtherUserScreenLocked == "N" && isAuditScreen == "N" && isContractScreen == "N") {
                document.getElementById("<%=hdnIsAuditScreen.ClientID %>").value = "Y";
                    PageMethods.UpdateScreenLockFlag();
                }
            }


            //WUIN-599 Unset the screen lock flag If an user close the browser or navigate to other than contract screens
            window.onunload = function () {
                if (unSaveBrowserClose) {
                    UpdateScreenLockFlag();
                }
            }

            //used to check if any changes to allow navigation to other screen 
            function IsDataChanged() {
                //debugger;
                IsAddRowDataChanged();
                var isDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
            var isDataChangedInsert = document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value;
            if (isDataChanged == "Y" || isDataChangedInsert == "Y") {
                return true;
            }
            else {
                return false;
            }
        }

        //Audit button navigation
        function RedirectToAuditScreen(button) {

            royaltorId = document.getElementById("<%=hdnRoyaltorId.ClientID %>").value;
            if (button.value == "Audit") {
                DisplayMessagePopup("To be developed!");
                return false;
            }
            else {
                document.getElementById("<%=hdnIsAuditScreen.ClientID %>").value = "Y";
                if (IsDataChanged()) {
                    window.onbeforeunload = null;
                    OpenPopupOnUnSavedData("../Contract/RoyContractGrouping.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y");
                }
                else {
                    window.location = "../Contract/RoyContractGrouping.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y";
                }
            }
        }

        //=================End

        function OnDataChange() {
            document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
            }

            function OnkeyDown() {
                if (event.keyCode == 8 || event.keyCode == 46) {
                    document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
            }
        }

        function OnDataChangeInsert() {
            document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").innerText = "Y";
        }

        function OnkeyDownInsert() {
            if (event.keyCode == 8 || event.keyCode == 46) {
                document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").innerText = "Y";
            }
        }

        //Validation: warning message if changes made and not saved

        //Undo Grid changes
        function UndoGridChanges(gridRow) {
            gridRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);

            hdnRoyTaxRate = document.getElementById(gridClientId + 'hdnTaxRate' + '_' + gridRowIndex);
            txtRoyTaxRate = document.getElementById(gridClientId + 'txtTaxRate' + '_' + gridRowIndex);

            txtRoyTaxRate.value = hdnRoyTaxRate.value;
            document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "N";

            Page_ClientValidate('');//clear all validators of the page

            return false;
        }

        //============== End

        //WUIN-1181 Changes
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
                                    ROYALTOR CONTRACT - TAX DETAILS
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
                                                    Text="Save Changes" UseSubmitBehavior="false" Width="90%" ValidationGroup="valUpdateTaxRate" TabIndex="109" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="30%"></td>
                                            <td align="right" width="70%">
                                                <asp:Button ID="btnAudit" runat="server" CssClass="ButtonStyle" OnClientClick="if (!RedirectToAuditScreen(this)){return false;}"
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
                    <td class="table_header_with_border">Tax Details</td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td valign="top">
                        <table width="90%" class="table_with_border">
                            <tr>
                                <td></td>
                            </tr>
                            <tr>
                                <td>
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvContTaxDetails" runat="server" AutoGenerateColumns="False" Width="98%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found" AllowSorting="true" OnRowDataBound="gvContTaxDetails_RowDataBound" OnSorting="gvContTaxDetails_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="IP Number" SortExpression="ip_number" 
                                                                ItemStyle-Width="15%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblIpNumber" runat="server" Width="75%" Text='<%# Bind("ip_number") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="Type" SortExpression="ip_type"
                                                                ItemStyle-Width="12%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblPayee" runat="server" Width="75%" Text='<%# Bind("ip_type") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="Interested Party Name" SortExpression="ip_name"
                                                                ItemStyle-Width="32%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblInterestedPartyName" runat="server" Width="95%" Text='<%#Bind("ip_name")%>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="Tax Type" SortExpression="royaltor_tax_type"
                                                                ItemStyle-Width="10%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblTaxType" runat="server" Width="95%" Text='<%#Bind("royaltor_tax_type")%>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"  HeaderText="Tax Rate" SortExpression="royaltor_tax_Rate"
                                                                ItemStyle-Width="13%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtTaxRate" runat="server" Width="87%" Text='<%#Bind("royaltor_tax_Rate")%>' CssClass="textboxStyle"
                                                                        MaxLength="8" onchange="javascript: OnDataChange();" onKeyPress="javascript: OnDataChange();"
                                                                        onkeydown="javascript: OnkeyDown();"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ControlToValidate="txtTaxRate" ValidationGroup="valUpdateTaxRate"
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Tax Rate" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                    <asp:RegularExpressionValidator ID="revtxtTaxRate" runat="server" Text="*" ValidationGroup="valUpdateTaxRate" ControlToValidate="txtTaxRate" ValidationExpression="^\d{0,3}(\.\d{1,2})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                        ToolTip="Please enter only positive number less than 1000 up to 2 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"
                                                                ItemStyle-Width="6%">
                                                                <ItemTemplate>
                                                                    <table width="100%" style="float: left; table-layout: fixed">
                                                                        <tr style="float: right">

                                                                            <td align="center">
                                                                                <asp:ImageButton ID="imgBtnUndo" runat="server" OnClientClick="return UndoGridChanges(this);" ImageUrl="../Images/cancel_row3.png"
                                                                                    ToolTip="Cancel" />
                                                                            </td>
                                                                            <td align="center">
                                                                                <asp:ImageButton ID="imgBtnDelete" runat="server" CommandName="deleteRow" ImageUrl="../Images/Delete.gif"
                                                                                    ToolTip="Delete" OnClientClick="return ConfirmDelete(this);" />
                                                                            </td>

                                                                        </tr>
                                                                    </table>
                                                                    <asp:HiddenField ID="hdnTaxRate" runat="server" Value='<%# Bind("royaltor_tax_rate") %>' />
                                                                    <asp:HiddenField ID="hdnRoyaltorTaxIntPartyId" runat="server" Value='<%# Bind("rt_int_party_id") %>' />
                                                                    <asp:HiddenField ID="hdnIsModified" runat="server" Value='<%# Bind("is_modified") %>' />
                                                                    <asp:HiddenField ID="hdnRoyaltorTaxType" runat="server" Value='<%# Bind("royaltor_tax_type") %>' />
                                                                    <asp:HiddenField ID="hdnIPNumber" runat="server" Value='<%# Bind("ip_number") %>' />
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
                                    <table width="98%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td width="15%" class="gridHeaderStyle_1row">IP Number</td>
                                            <td width="12%" class="gridHeaderStyle_1row">Type</td>
                                            <td width="32%" class="gridHeaderStyle_1row">Interested Party Name</td>
                                            <td width="10%" class="gridHeaderStyle_1row">Tax Type</td>
                                            <td width="13%" class="gridHeaderStyle_1row">Tax Rate</td>
                                            <td width="6%" class="gridHeaderStyle_1row">&nbsp</td>
                                        </tr>
                                        <tr>
                                            <td class="insertBoxStyle">
                                                <asp:DropDownList ID="ddlIPNumber" runat="server" Width="90%" CssClass="ddlStyle" TabIndex="103" OnSelectedIndexChanged="ddlIPNumber_SelectedIndexChanged" AutoPostBack="true" onchange="javascript: OnDataChangeInsert();" onKeyPress="javascript: OnDataChangeInsert();" onkeydown="javascript: OnkeyDownInsert();">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="ddlIPNumber" ValidationGroup="valGrpAppendAddRow"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please select IpNumber" InitialValue="-" Display="Dynamic"></asp:RequiredFieldValidator>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:TextBox ID="txtPayeeType" runat="server" Width="90%" CssClass="textboxStyle" TabIndex="104" Text="Payee" ReadOnly="true">
                                                </asp:TextBox>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:TextBox ID="txtInterestedPartyName" runat="server" Width="88%" CssClass="textboxStyle" TabIndex="105" ReadOnly="true">
                                                </asp:TextBox>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:DropDownList ID="ddlTaxType" runat="server" Width="86%" CssClass="ddlStyle" TabIndex="106" onchange="javascript: OnDataChangeInsert();" onKeyPress="javascript: OnDataChangeInsert();" onkeydown="javascript: OnkeyDownInsert();">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="ddlTaxType" ValidationGroup="valGrpAppendAddRow"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please select Tax Type" InitialValue="-" Display="Dynamic"></asp:RequiredFieldValidator>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:TextBox ID="txtTaxRate" runat="server" Width="87%" CssClass="textboxStyle" MaxLength="8" TabIndex="107" onchange="javascript: OnDataChangeInsert();" onKeyPress="javascript: OnDataChangeInsert();" onkeydown="javascript: OnkeyDownInsert();"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtTaxRate" ValidationGroup="valGrpAppendAddRow"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Tax Rate" Display="Dynamic"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="revAddtxtTaxRate" runat="server" Text="*" ValidationGroup="valGrpAppendAddRow" ControlToValidate="txtTaxRate" ValidationExpression="^\d{0,3}(\.\d{1,2})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                    ToolTip="Please enter only positive number less than 1000 up to 2 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <table width="60%">
                                                    <tr>
                                                        <td>
                                                            <asp:ImageButton ID="btnAppendAddRow" runat="server" ImageUrl="../Images/add_row.png" Onkeydown="OnAppendAddRowKeyDown();"
                                                                ToolTip="Append Tax Details" OnClick="btnAppendAddRow_Click" ValidationGroup="valGrpAppendAddRow" TabIndex="108" />
                                                        </td>
                                                        <td>
                                                            <asp:ImageButton ID="btnUndoAddRow" runat="server" ImageUrl="../Images/cancel_row3.png"
                                                                ToolTip="Clear Add row" OnClientClick="return ClearAddRow();" TabIndex="109" CausesValidation="false" />
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

            <%--JIRA-908 Changes by Ravi on 01/01/2020 -- Start--%>
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
            <%--JIRA-908 Changes by Ravi on 01/01/2020 -- End--%>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnInterestedPartyID" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnAddRowDataChanged" runat="server" Value="N" />
            <asp:HiddenField ID="hdnNewRoyaltorSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnChangeNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnInsertDataNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridRowSelectedPrvious" runat="server" />
            <asp:HiddenField ID="hdnRoyaltorId" runat="server" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" onkeydown="FocusLblKeyPress();"></asp:Label>
            <asp:HiddenField ID="hdnGridDataDeleted" runat="server" Value="N" />
            <%--JIRA-908 Changes by Ravi on 01/01/2020 -- Start--%>
            <asp:HiddenField ID="hdnDeleteIntPartyId" runat="server" Value="N" />
            <asp:HiddenField ID="hdnDeleteIsModified" runat="server" Value="N" />
            <asp:HiddenField ID="hdnDeleteRoyTaxType" runat="server" Value="N" />
            <%--JIRA-908 Changes by Ravi on 01/01/2020 -- End--%>
            <asp:HiddenField ID="hdnIsAuditScreen" runat="server" Value="N" />
            <asp:HiddenField ID="hdnOtherUserScreenLocked" runat="server" Value="N" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

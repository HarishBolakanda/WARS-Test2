<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoyaltorReserves.aspx.cs" Inherits="WARS.RoyaltorReserves" MasterPageFile="~/MasterPage.Master"
    Title="WARS - RoyaltorReserves" MaintainScrollPositionOnPostback="true" %>

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

        //probress bar functionality - starts        
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
        }

        function EndRequestHandler(sender, args) {
            //Hide the modal popup - the update progress
            var popup = $find('<%= mPopupPageLevel.ClientID %>');
            if (popup != null) {
                popup.hide();
            }


        }

        //probress bar functionality - ends

        //grid panel height adjustment functioanlity - starts
        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.53;
            document.getElementById("<%=hdnBalGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
            document.getElementById("<%=hdnRsvGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
            document.getElementById("<%=PnlRsvGrid.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=PnlBalGrid.ClientID %>").style.height = gridPanelHeight + "px";
        }

        //grid panel height adjustment functioanlity - ends    


        //Royaltor auto populate search functionalities
        //var txtRoy;
        var txtRoySrch;




        function royaltorListPopulating() {
            txtRoySrch = document.getElementById("<%= txtRoyaltor.ClientID %>");
            txtRoySrch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoySrch.style.backgroundRepeat = 'no-repeat';
            txtRoySrch.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnRoySearchSelected.ClientID %>").value = "N";
        }

        function royaltorListPopulated() {
            txtRoySrch = document.getElementById("<%= txtRoyaltor.ClientID %>");
            txtRoySrch.style.backgroundImage = 'none';
        }

        function royaltorListItemSelected(sender, args) {
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                document.getElementById("<%= txtRoyaltor.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnRoySearchSelected.ClientID %>").value = "Y";
            }

        }

        function resetScrollPosition(sender, args) {
            var autoCompPnl = document.getElementById("<%= autocompleteDropDownPanel1.ClientID %>");
            autoCompPnl.scrollTop = 1;

        }


        //================================End

        //Validation: warning message if changes made and not saved or on page change             

        function OnGridRowSelected(row, name) {
            var rowData = row.parentNode.parentNode;
            var rowIndex = rowData.rowIndex - 1;

            if (document.getElementById("<%=hdnPreviousSelectedGrid.ClientID %>").value == "") {
                document.getElementById("<%=hdnPreviousSelectedGrid.ClientID %>").innerText = name;
            }

            if (document.getElementById("<%=hdnPreviousSelectedGrid.ClientID %>").value != name) {
                //document.getElementById("<%=hdnPreviousSelectedGrid.ClientID %>").innerText = name;
                if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
                    var popup = $find('<%= mpeSaveUndo.ClientID %>');
                    if (popup != null) {
                        popup.show();
                        $get("<%=BtnUndoChanges.ClientID%>").focus();
                    }
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
                            $get("<%=BtnUndoChanges.ClientID%>").focus();
                        }
                    }
                }
        }
    }

    //set flag value when grid data is changed
    function OnGridDataChange() {
        document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
    }

    //Validate any unsaved data on browser window close/refresh
    function RedirectToErrorPage() {
        document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
        window.location = "../Common/ExceptionPage.aspx";
    }

    var unSaveBrowserClose = false;
    function WarnOnUnSavedData() {
        var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
        var isGridDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
        var isContractScreen = document.getElementById("hdnIsContractScreen").value;
        var isNotContractScreen = document.getElementById("hdnIsNotContractScreen").value;
        var isRoyaltorNull = document.getElementById("<%=hdnIsRoyaltorNull.ClientID %>").value;

        if (isExceptionRaised != "Y" && isContractScreen != "Y" && isNotContractScreen != "Y") {
            if ((isGridDataChanged == "Y")) {
                unSaveBrowserClose = true;
                return warningMsgOnUnSavedData;
            }
        }
        if (isRoyaltorNull != "Y") {
            UpdateScreenLockFlag();// WUIN-599 - Unset the screen lock flag If an user close the browser with out unsaved data or navigate to other than contract screens
        }
    }
    window.onbeforeunload = WarnOnUnSavedData;

    //WUIN-599 Unset the screen lock flag If an user close the browser or navigate to other than contract screens
    window.onunload = function () {
        var isRoyaltorNull = document.getElementById("<%=hdnIsRoyaltorNull.ClientID %>").value;
        if (unSaveBrowserClose) {
            if (isRoyaltorNull != "Y") {
                UpdateScreenLockFlag();
            }
        }
    }

        function UpdateScreenLockFlag() {
            var isOtherUserScreenLocked = document.getElementById("<%=hdnOtherUserScreenLocked.ClientID %>").value;
            var isContractScreen = document.getElementById("hdnIsContractScreen").value;
            if (isOtherUserScreenLocked == "N" && isContractScreen == "N") {
                PageMethods.UpdateScreenLockFlag();
            }
        }


        //used to check if any changes to allow navigation to other screen 
        function IsDataChanged() {
            var isGridDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
            if (isGridDataChanged == "Y") {
                return true;
            }
            else {
                return false;
            }
        }


        function ConfirmSearch() {
            var isGridDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
            if (isGridDataChanged == "Y") {
                var popup = $find('<%= mpeConfirmation.ClientID %>');
                if (popup != null) {
                    popup.show();
                    document.getElementById("<%=lblConfirmMsg.ClientID %>").innerText = "You have made changes which are not saved. Continue or cancel";
            }
            return false;
        }
        else {
            return true;
        }
    }
    //================================End

    //Tab key to remain only on screen fields
    function OnTabPress() {
        if (event.keyCode == 9) {
            document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        //=============== End


        function RoyRsvConfirmDelete(row) {
            gridRowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            document.getElementById("<%= hdnRsvDeleteRowIndex.ClientID %>").value = gridRowIndex;
        var popup = $find('<%= mpeConfirmDelete.ClientID %>');
        if (popup != null) {
            popup.show();
        }
        return false;
    }

    </script>
    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="5">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    ROYALTOR BALANCE AND RESERVE MAINTENANCE
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="5%" class="identifierLable_large_bold">Royaltor
                    </td>
                    <td width="25%">
                        <asp:TextBox ID="txtRoyaltor" runat="server" Width="98%" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" OnTextChanged="txtRoyaltor_TextChanged" AutoPostBack="true" OnFocus="return ConfirmSearch();"
                            TabIndex="101" onkeydown="OnTabPress();"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="royaltorFilterExtender" runat="server"
                            ServiceMethod="FuzzySearchAllRoyListWithOwnerCode"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtRoyaltor"
                            FirstRowSelected="true"
                            OnClientPopulating="royaltorListPopulating"
                            OnClientPopulated="royaltorListPopulated"
                            OnClientHidden="royaltorListPopulated"
                            OnClientShown="resetScrollPosition"
                            OnClientItemSelected="royaltorListItemSelected"
                            CompletionListElementID="autocompleteDropDownPanel1" />
                        <asp:Panel ID="autocompleteDropDownPanel1" runat="server" CssClass="identifierLable" />
                    </td>
                    <td valign="top">
                        <asp:ImageButton ID="fuzzySearchRoyaltor" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                            OnClick="fuzzySearchRoyaltor_Click" ToolTip="Search Royaltor" CssClass="FuzzySearch_Button" />
                    </td>
                    <td></td>

                </tr>
                <tr>
                    <td colspan="5"></td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="4">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <table width="94%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td class="gridTitle" style="cursor: none; vertical-align: middle; padding: 1%" width="21.62%">Royaltor Balance                                                  
                                            </td>
                                        </tr>
                                    </table>

                                </td>
                                <td width="1.5%"></td>
                                <td>
                                    <table width="99.2%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td class="gridTitle" style="cursor: none; vertical-align: middle; padding: 0.3%" width="89%">Royaltor Reserves
                                            </td>
                                            <td width="1%"></td>
                                            <td align="left">
                                                <asp:ImageButton ID="imgBtnAddRsvRow" runat="server" ImageUrl="../Images/Add.gif" OnClientClick="javascript: OnGridDataChange();" OnClick="imgBtnAddRsvRow_Click" Style="height: 16px; width: 16px; padding: 0" ValidationGroup="valResEdit" ToolTip="Insert reserve row"
                                                    UseSubmitBehavior="false" TabIndex="100" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="right">
                                    <%--<asp:ImageButton ID="imgBtnAddRsvRow" runat="server" ImageUrl="../Images/Add.gif" OnClientClick="javascript: OnGridDataChange();" OnClick="imgBtnAddRsvRow_Click" Style="height: 90%; width: 85%; padding: 0" ValidationGroup="valResEdit" ToolTip="Insert reserve row"
                                        UseSubmitBehavior="false" TabIndex="100"/>--%>
                                </td>
                            </tr>
                            <tr>
                                <td width="22%" valign="top">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlBalGrid" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvRoyBal" runat="server" AutoGenerateColumns="False" Width="93.8%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True" EmptyDataText="No Data Found"
                                                        OnRowCommand="gvRoyBal_RowCommand" OnRowDataBound="gvRoyBal_RowDataBound" AllowSorting="true" OnSorting="gvRoyBal_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <AlternatingRowStyle BackColor="#E3EFFF" />
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-Width="58%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Balance Period" SortExpression="STATEMENT_PERIOD_ID">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblBalPeriodId" runat="server" Text='<%# Bind("STATEMENT_PERIOD_ID") %>' Visible="false"></asp:Label>
                                                                    <asp:Label ID="lblBalancePeriod" runat="server" Text='<%# Bind("balance_period") %>'
                                                                        CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="27%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Balance" SortExpression="closing_balance">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtBalance" runat="server" Text='<%# Eval("closing_balance") %>' TextMode="Number"
                                                                        CssClass="gridTextField" Width="75%" onchange="javascript: OnGridDataChange();"
                                                                        onclick="OnGridRowSelected(this,'Bal')"
                                                                        Style="text-align: right"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvBalEdit" ControlToValidate="txtBalance" ValidationGroup="valBalEdit"
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter balance amount" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                    <asp:RegularExpressionValidator ID="revBalance" runat="server" Text="*" ControlToValidate="txtBalance" ValidationGroup="valBalEdit"
                                                                        ValidationExpression="^[-+]?\d*\.{0,1}\d+$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                        ToolTip="Please enter only number" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="15%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderStyle-Width="15%">
                                                                <ItemTemplate>
                                                                    <asp:ImageButton ID="btnUpdate" runat="server" CommandName="saverow" ImageUrl="../Images/save.png" ValidationGroup="valBalEdit" ToolTip="Save" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td width="1%"></td>
                                <td width="72%" valign="top">
                                    <table width="90%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlRsvGrid" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvRoyRsv" runat="server" AutoGenerateColumns="False" Width="98%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" ShowHeaderWhenEmpty="True" EmptyDataText="No Data Found"
                                                        EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" OnRowDataBound="gvRoyRsv_RowDataBound"
                                                        OnRowCommand="gvRoyRsv_RowCommand">
                                                        <AlternatingRowStyle BackColor="#E3EFFF" />
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-Width="22%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Period Reserve Taken" SortExpression="RSV_PERIOD_VALUE">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblflag" runat="server" Text='<%# Eval("flag") %>' Visible="false" />
                                                                    <asp:Label ID="lblRsvPeriodValue" runat="server" Text='<%# Eval("RSV_PERIOD_VALUE") %>' Visible="false" />
                                                                    <asp:DropDownList ID="ddlRsvPeriod" runat="server" Width="94%" CssClass="identifierLable"
                                                                        onchange="javascript: OnGridDataChange();" onclick="OnGridRowSelected(this,'Resv')">
                                                                    </asp:DropDownList>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvRsvPeriod" ControlToValidate="ddlRsvPeriod" ValidationGroup="valResEdit"
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please select reserve period"
                                                                        InitialValue="-"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Reserve Taken" SortExpression="RESERVE_TAKEN">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRsvTaken" runat="server" Text='<%# Bind("RESERVE_TAKEN") %>' Visible="false"></asp:Label>
                                                                    <asp:TextBox ID="txtRsvTaken" runat="server" Text='<%# Eval("RESERVE_TAKEN") %>'
                                                                        CssClass="identifierLable" Width="65px" ItemStyle-CssClass="gridItemStyle_Center_Align" TextMode="Number"
                                                                        Style="text-align: center; font-weight: bold" onchange="javascript: OnGridDataChange();" onclick="OnGridRowSelected(this,'Resv')"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvResEdit" ControlToValidate="txtRsvTaken" ValidationGroup="valResEdit"
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter reserve amount" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                    <asp:RegularExpressionValidator ID="revRsvTaken" runat="server" Text="*" ControlToValidate="txtRsvTaken"
                                                                        ValidationExpression="^[+]?\d*\.{0,1}\d+$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valResEdit"
                                                                        ToolTip="Please enter only positive number" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Liquidation Period" SortExpression="liquidation_period">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtLqPeriod" runat="server" Text='<%# Eval("liquidation_period") %>'
                                                                        CssClass="identifierLable" Width="65px" ItemStyle-CssClass="gridItemStyle_Center_Align" TextMode="Number"
                                                                        Style="text-align: center" onchange="javascript: OnGridDataChange();" onclick="OnGridRowSelected(this,'Resv')"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvLqdInterval" ControlToValidate="txtLqPeriod" ValidationGroup="valResEdit"
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter liquidation period" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                    <asp:RegularExpressionValidator ID="revLqdInterval" runat="server" Text="*" ControlToValidate="txtLqPeriod"
                                                                        ValidationExpression="^[0-9]\d*$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valResEdit"
                                                                        ToolTip="Please enter only positive integer number" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="22%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Period Reserve Released" SortExpression="Liquidation_period_value">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblLqdtPeriod" runat="server" Text='<%# Eval("Liquidation_period_value") %>' Visible="false" />
                                                                    <asp:DropDownList ID="ddlLqdPeriod" runat="server" Width="93%" CssClass="identifierLable"
                                                                        onchange="javascript: OnGridDataChange();" onclick="OnGridRowSelected(this,'Resv')">
                                                                    </asp:DropDownList>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvStmtPeriod" ControlToValidate="ddlLqdPeriod" ValidationGroup="valResEdit"
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please select liquidation period"
                                                                        InitialValue="-"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Liquidated Amount" SortExpression="LIQUIDATED_AMOUNT">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblLqdAmount" runat="server" Text='<%# Bind("LIQUIDATED_AMOUNT") %>'
                                                                        Visible="false"></asp:Label>
                                                                    <asp:TextBox ID="txtLqAmount" runat="server" Text='<%# Eval("LIQUIDATED_AMOUNT") %>'
                                                                        CssClass="identifierLable" Width="65px" ItemStyle-CssClass="gridItemStyle_Center_Align"
                                                                        Style="text-align: center" onchange="javascript: OnGridDataChange();" onclick="OnGridRowSelected(this,'Resv')"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvLqAmount" ControlToValidate="txtLqAmount" ValidationGroup="valResEdit"
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter liquidation amount" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                    <asp:RegularExpressionValidator ID="revLqAmount" runat="server" Text="*" ControlToValidate="txtLqAmount"
                                                                        ValidationExpression="^[+]?\d*\.{0,1}\d+$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valResEdit"
                                                                        ToolTip="Please enter only positive number" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Reserve Held Balance" SortExpression="RESERVE_HELD_BALANCE">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRsvHeld" runat="server" Text='<%# Eval("RESERVE_HELD_BALANCE") %>'
                                                                        CssClass="identifierLable" Font-Bold="true"
                                                                        Style="display: block; width: 85%; text-align: right;"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="6%" ItemStyle-CssClass="gridItemStyle_Right_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderStyle-Width="6%">
                                                                <ItemTemplate>
                                                                    <table width="100%" style="float: right; table-layout: fixed">
                                                                        <tr style="float: right">
                                                                            <td align="right" style="float: right;" width="33%">
                                                                                <asp:ImageButton ID="btnInsert" runat="server" CommandName="insertrow" ImageUrl="../Images/newrow.png" ToolTip="Add"
                                                                                    OnClientClick="javascript: OnGridDataChange();" />
                                                                            </td>
                                                                            <td align="right" style="float: right" width="33%">
                                                                                <asp:ImageButton ID="btnUpdate" runat="server" CommandName="saverow" ImageUrl="../Images/save.png" ValidationGroup="valResEdit" ToolTip="Save" />
                                                                            </td>
                                                                            <td align="right" style="float: right">
                                                                                <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="../Images/Delete.gif" OnClientClick="return RoyRsvConfirmDelete(this);"
                                                                                    ToolTip="Delete" />
                                                                                <%--JIRA-908 CHanges--%>
                                                                                <asp:ImageButton ID="imgBtnCancel" runat="server" CommandName="cancelrow" ImageUrl="../Images/cancel_row3.png" Visible="false"
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
                                    </table>
                                </td>
                                <td width="5%" valign="top" id="tdConNavBtns" runat="server">
                                    <ContNav:ContractNavigation ID="contractNavigationButtons" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <asp:UpdateProgress ID="progressBarPageLevel" runat="server" AssociatedUpdatePanelID="updPnlPageLevel" DisplayAfter="100">
                <ProgressTemplate>
                    <div id="Search" style="font-weight: bold; color: Black">
                        <img src="../Images/InProgress2.gif" alt="" />
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <ajaxToolkit:ModalPopupExtender ID="mPopupPageLevel" runat="server" PopupControlID="progressBarPageLevel" TargetControlID="progressBarPageLevel"
                BackgroundCssClass="progressBar" RepositionMode="RepositionOnWindowResize">
            </ajaxToolkit:ModalPopupExtender>

            <%--Warning popup--%>
            <asp:Button ID="dummyConfirm" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmation" runat="server" PopupControlID="pnlPopup" TargetControlID="dummyConfirm"
                CancelControlID="btnNoConfirm" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlPopup" runat="server" align="center" Width="30%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="Label1" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblConfirmMsg" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYesSearch" runat="server" Text="OK" CssClass="ButtonStyle" Width="60px"
                                            OnClick="btnYesSearch_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNoConfirm" runat="server" Text="Cancel" Width="60px" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <%--Save/Undo changes popup--%>
            <asp:Button ID="dummySaveUndo" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeSaveUndo" runat="server" PopupControlID="pnlSaveUndo" TargetControlID="dummySaveUndo"
                CancelControlID="btnClosePopupSaveUndo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlSaveUndo" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td align="right" style="vertical-align: top;">
                            <asp:ImageButton ID="btnClosePopupSaveUndo" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" CssClass="identifierLable"
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
                                        <asp:Button ID="BtnUndoChanges" runat="server" Text="Undo" CssClass="ButtonStyle" OnClick="BtnUndoChanges_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

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
            <asp:HiddenField ID="hdnBalGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnRsvGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnChangeNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridRowSelectedPrvious" runat="server" />
            <asp:HiddenField ID="hdnPreviousSelectedGrid" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnRoySearchSelected" runat="server" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" />
            <asp:HiddenField ID="hdnRsvDeleteRowIndex" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
            <asp:HiddenField ID="hdnOtherUserScreenLocked" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsRoyaltorNull" runat="server" Value="N" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoyaltorGroupings.aspx.cs" Inherits="WARS.RoyaltorGroupings" MasterPageFile="~/MasterPage.Master"
    Title="WARS - RoyaltorGroupings" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

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
            document.getElementById("<%=hdnGrpInGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
            document.getElementById("<%=hdnGrpOutGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        //grid panel height adjustment functioanlity - ends  


        //Royaltor auto populate search functionalities

        function royaltorGroupListPopulating() {
            txtRoy = document.getElementById("<%= txtRoyaltorGroupType.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "N";
        }

        function royaltorGroupListPopulated() {
            txtRoy = document.getElementById("<%= txtRoyaltorGroupType.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function royaltorGroupItemSelected(sender, args) {
            var royGrpSrchVal = args.get_value();
            if (royGrpSrchVal == 'No results found') {
                document.getElementById("<%= txtRoyaltorGroupType.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "Y";
            }

        }

        function royaltorGroupOutListPopulating() {
            txtRoy = document.getElementById("<%= txtGroupOutBox.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "N";
        }

        function royaltorGroupOutListPopulated() {
            txtRoy = document.getElementById("<%= txtGroupOutBox.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function royaltorGroupOutItemSelected(sender, args) {
            var royGrpOutSrchVal = args.get_value();
            if (royGrpOutSrchVal == 'No results found') {
                document.getElementById("<%= txtGroupOutBox.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "Y";
            }

        }

        function royaltorGroupInListPopulating() {
            txtRoy = document.getElementById("<%= txtGroupInBox.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "N";
        }

        function royaltorGroupInListPopulated() {
            txtRoy = document.getElementById("<%= txtGroupInBox.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function royaltorGroupInItemSelected(sender, args) {
            var royGrpInSrchVal = args.get_value();
            if (royGrpInSrchVal == 'No results found') {
                document.getElementById("<%= txtGroupInBox.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "Y";
            }

        }
        //================================End
        function ValidateChanges() {
            eval(this.href);
        }


        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }
        //=============== End

        function IsDataChanged() {
            if (IsGridDataChanged()) {
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

        //Warn on changes made and not saved
        function WarnOnUnSavedData() {
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            if (isExceptionRaised != "Y" && IsDataChanged()) {
                return warningMsgOnUnSavedData;
            }
        }
        window.onbeforeunload = WarnOnUnSavedData;

        function IsGridDataChanged() {
            gvGroupOut = document.getElementById("<%= gvGroupOut.ClientID %>");
            gvGroupIn = document.getElementById("<%= gvGroupIn.ClientID %>");
            var checkedAddCount = 0;
            var checkedRemoveCount = 0;
            if (gvGroupOut != null) {
                var gvRows = gvGroupOut.rows;
                for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                    rowIndex = i - 1; //WUIN-746 row index start from 0
                    cbAddRoyaltors = document.getElementById("ContentPlaceHolderBody_gvGroupOut_" + 'cbAddRoyaltors' + '_' + rowIndex);

                    if (cbAddRoyaltors != null && !(cbAddRoyaltors.disabled) && cbAddRoyaltors.checked) {
                        checkedAddCount = checkedAddCount + 1;
                        break;
                    }
                }
            }

            if (gvGroupIn != null) {
                var gvRows = gvGroupIn.rows;
                for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                    rowIndex = i - 1; //WUIN-746 row index start from 0

                    cbRemoveRoyaltors = document.getElementById("ContentPlaceHolderBody_gvGroupIn_" + 'cbRemoveRoyaltors' + '_' + rowIndex);

                    if (cbRemoveRoyaltors != null && !(cbRemoveRoyaltors.disabled) && cbRemoveRoyaltors.checked) {
                        checkedRemoveCount = checkedRemoveCount + 1;
                        break;
                    }
                }
            }

            if (checkedAddCount > 0 || checkedRemoveCount > 0) {
                return true;
            }
            else {
                return false;
            }
        }

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
                    <td colspan="6">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    ROYALTOR GROUPINGS MAINTENANCE
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="8%" class="identifierLable_large_bold">Grouping Type</td>
                    <td width="20%">
                        <asp:DropDownList ID="ddlRoyaltorGroupType" runat="server" Width="99.5%" CssClass="ddlStyle" AutoPostBack="true" OnSelectedIndexChanged="ddlRoyaltorGroupType_SelectedIndexChanged"
                            TabIndex="100" onfocus="if (!ValidateUnsavedData('GroupType')) { return false;};">
                        </asp:DropDownList></td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="7%" class="identifierLable_large_bold">Royaltor Group</td>
                    <td width="20%">
                        <asp:TextBox ID="txtRoyaltorGroupType" runat="server" Width="98%" CssClass="identifierLable"
                            OnTextChanged="txtRoyaltorGroupType_TextChanged" AutoPostBack="true" TabIndex="101"
                            onfocus="if (!ValidateUnsavedData('RoyaltorGroup')) { return false;};"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="royaltorGroupFilterExtender" runat="server"
                            ServiceMethod="FuzzyRoyGrpMaintRoyaltorList"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtRoyaltorGroupType"
                            FirstRowSelected="true"
                            OnClientPopulating="royaltorGroupListPopulating"
                            OnClientPopulated="royaltorGroupListPopulated"
                            OnClientHidden="royaltorGroupListPopulated"
                            OnClientItemSelected="royaltorGroupItemSelected"
                            CompletionListElementID="autocompleteDropDownPanel1" />
                        <asp:Panel ID="autocompleteDropDownPanel1" runat="server" CssClass="identifierLable" />
                    </td>
                    <td width="2%">
                        <asp:ImageButton ID="fuzzySearchRoyaltorGroup" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                            OnClientClick="if (!ValidateUnsavedData('fuzzySearchRoyaltorGroup')) { return false;};"
                            ToolTip="Search Royaltor Group" OnClick="fuzzySearchRoyaltorGroup_Click" CssClass="FuzzySearch_Button" />
                    </td>
                    <td width="35%">
                        <table width="100%" id="tdSummStmts" runat="server">
                            <tr>
                                <td width="2%"></td>
                                <td>
                                    <asp:Button ID="btnGenSummForGroup" runat="server" Text="Generate Summary for Group" Width="90%" CssClass="ButtonStyle"
                                        OnClick="btnGenSummForGroup_Click" UseSubmitBehavior="false" />
                                </td>
                                <td>
                                    <asp:Button ID="btnGenAllSummaries" runat="server" Text="Generate Summaries for all Groups" Width="92%" CssClass="ButtonStyle"
                                        OnClick="btnGenAllSummaries_Click" UseSubmitBehavior="false" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="6">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td colspan="6">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="2%"></td>
                                <td width="30%" valign="top">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td colspan="3">
                                                <div class="gridTitle_Bold" style="cursor: none; vertical-align: middle; padding-left: 1%; width: 94.9%; text-align: center">
                                                    Out of the Group
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="19%" class="identifierLable_large_bold">Royaltor</td>
                                            <td width="72%">
                                                <asp:TextBox ID="txtGroupOutBox" runat="server" Width="97%" CssClass="identifierLable"
                                                    OnTextChanged="txtGroupOutBox_TextChanged" AutoPostBack="true" TabIndex="102"></asp:TextBox>
                                                <ajaxToolkit:AutoCompleteExtender ID="txtGroupOutBoxExtender" runat="server"
                                                    ServiceMethod="FuzzyRoyGrpMaintGroupOutBoxList"
                                                    ServicePath="~/Services/FuzzySearch.asmx"
                                                    MinimumPrefixLength="1"
                                                    CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                    TargetControlID="txtGroupOutBox"
                                                    FirstRowSelected="true"
                                                    OnClientPopulating="royaltorGroupOutListPopulating"
                                                    OnClientPopulated="royaltorGroupOutListPopulated"
                                                    OnClientHidden="royaltorGroupOutListPopulated"
                                                    OnClientItemSelected="royaltorGroupOutItemSelected"
                                                    CompletionListElementID="autocompleteDropDownPanel2" />
                                                <asp:Panel ID="autocompleteDropDownPanel2" runat="server" CssClass="identifierLable" />
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="fuzzySearchRoyaltorOut" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                                                    ToolTip="Search Royaltor" OnClick="fuzzySearchRoyaltorOut_Click" CssClass="FuzzySearch_Button" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <asp:Panel ID="PnlGroupOut" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvGroupOut" runat="server" AutoGenerateColumns="False" Width="95.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found for the selected royaltor group." OnRowDataBound="gvGroupOut_RowDataBound" AllowSorting="true" OnSorting="gvGroupOut_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <SelectedRowStyle BackColor="#99b8fa" Font-Bold="true" />
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Royaltor Id" SortExpression="royaltor_id">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRoyaltorId" runat="server" Text='<%# Bind("royaltor_id") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Royaltor Name" SortExpression="royaltor_name">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRoyaltorName" runat="server" Text='<%# Bind("royaltor_name") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="75%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="cbAddRoyaltors" runat="server" CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="5%" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <EmptyDataRowStyle CssClass="gridEmptyDataRowStyle" />
                                                    </asp:GridView>
                                                </asp:Panel>
                                                <asp:Repeater ID="rptPager" runat="server">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>'
                                                            OnClientClick="return ValidateChanges();" ClientIDMode="AutoID" CausesValidation="false" Enabled='<%# Eval("Enabled") %>' OnClick="lnkPage_Click" CssClass="gridPager"> </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td width="20%" valign="top">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="100%" align="center">
                                                <asp:ImageButton ID="btnRemoveRoyaltor" runat="server" ImageUrl="~/Images/groupOut.png" OnClick="btnRemoveRoyaltor_Click"
                                                    ToolTip="Remove royaltor from the group" />
                                                &nbsp;
                                                &nbsp;
                                                <asp:ImageButton ID="btnAddRoyaltor" runat="server" ImageUrl="~/Images/groupIn.png" OnClick="btnAddRoyaltor_Click"
                                                    ToolTip="Add royaltor to the group" />
                                            </td>
                                        </tr>
                                    </table>

                                </td>
                                <td width="30%" valign="top">

                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td colspan="3">
                                                <div class="gridTitle_Bold" style="cursor: none; vertical-align: middle; padding-left: 1%; width: 94.9%; text-align: center">
                                                    In the Group
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="19%" class="identifierLable_large_bold">Royaltor</td>
                                            <td width="72%">
                                                <asp:TextBox ID="txtGroupInBox" runat="server" Width="97%" CssClass="identifierLable"
                                                    OnTextChanged="txtGroupInBox_TextChanged" AutoPostBack="true" TabIndex="103" onkeydown="OnTabPress();"></asp:TextBox>
                                                <ajaxToolkit:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server"
                                                    ServiceMethod="FuzzyRoyGrpMaintGroupInBoxList"
                                                    ServicePath="~/Services/FuzzySearch.asmx"
                                                    MinimumPrefixLength="1"
                                                    CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                    TargetControlID="txtGroupInBox"
                                                    FirstRowSelected="true"
                                                    OnClientPopulating="royaltorGroupInListPopulating"
                                                    OnClientPopulated="royaltorGroupInListPopulated"
                                                    OnClientHidden="royaltorGroupInListPopulated"
                                                    OnClientItemSelected="royaltorGroupInItemSelected"
                                                    CompletionListElementID="autocompleteDropDownPanel3" />
                                                <asp:Panel ID="autocompleteDropDownPanel3" runat="server" CssClass="identifierLable" />
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="fuzzySearchRoyaltorIn" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                                                    ToolTip="Search Royaltor" OnClick="fuzzySearchRoyaltorIn_Click" CssClass="FuzzySearch_Button" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <asp:Panel ID="PnlGroupIn" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvGroupIn" runat="server" AutoGenerateColumns="False" Width="95.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found for the selected royaltor group."
                                                        OnRowDataBound="gvGroupIn_RowDataBound" AllowSorting="true" OnSorting="gvGroupIn_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <SelectedRowStyle BackColor="#99b8fa" Font-Bold="true" />
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Royaltor Id" SortExpression="royaltor_id">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRoyaltorId" runat="server" Text='<%# Bind("royaltor_id") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Royaltor Name" SortExpression="royaltor_name">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRolaytorName" runat="server" Text='<%# Bind("royaltor_name") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="75%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="cbRemoveRoyaltors" runat="server" CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="5%" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <EmptyDataRowStyle CssClass="gridEmptyDataRowStyle" />
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td width="18%"></td>
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
                                        <asp:Button ID="btnUnSavedDataExit" runat="server" Text="Exit" CssClass="ButtonStyle" Width="30%" OnClick="btnUnSavedDataExit_Click"
                                            OnClientClick="if (!OnUnSavedDataExit()) { return false;};" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnPageNumber" runat="server" Value="" />
            <asp:HiddenField ID="hdnGrpInGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnGrpOutGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnSearchListItemSelected" runat="server" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" Value="" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
            <asp:HiddenField ID="hdnButtonSelection" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoyaltorStatementChanges.aspx.cs" Inherits="WARS.RoyaltorStatementChanges" MasterPageFile="~/MasterPage.Master"
    Title="WARS - RoyaltorStatementChanges" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>
<%--<%@ Register TagPrefix="nextRun" TagName="StmtNextRunDetails" Src="~/StmtNextRunDetails.ascx" %>--%>
<%@ Register TagPrefix="activityScreen" TagName="ActivityScreen" Src="~/UserControls/RoyaltorActivity.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //to open royaltor activity screen
        function OpenActivityScreen() {
            var win = window.open('../StatementProcessing/RoyaltorActivity.aspx', '_self');
            win.focus();
        }

        //to open workflow screen
        function OpenWorkflowScreen() {
            var win = window.open('../StatementProcessing/WorkFlow.aspx', '_self');
            win.focus();
        }
    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 1px;">
                        <asp:Button ID="btnOpenStatementActivity" runat="server" CssClass="LinkButtonStyle"
                            OnClientClick="OpenActivityScreen();" Text="Statement Activity" UseSubmitBehavior="false"
                            Width="98%" CausesValidation="false" />
                    </td>
                </tr>
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 1px;">
                        <asp:Button ID="btnOpenWorkflow" runat="server" CssClass="LinkButtonStyle"
                            OnClientClick="OpenWorkflowScreen();" Text="Workflow"
                            UseSubmitBehavior="false" Width="98%" CausesValidation="false" />
                    </td>
                </tr>
                <%--<tr>
                    <td class="dropdown">
                        <asp:Button ID="btnOpenStatementActivity" runat="server" CssClass="dropbtn"
                            Text="Shortcuts" UseSubmitBehavior="false"
                            Width="40%" CausesValidation="false" />
                        <div class="dropdown-content">
                            <a href="RoyaltorActivity.aspx">Statement Activity</a>
                            <a href="WorkFlow.aspx">Workflow</a>
                        </div>
                    </td>
                </tr>--%>
            </table>
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
            var gridPanelHeight = windowHeight * 0.58;
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        //grid panel height adjustment functioanlity - ends   


        //Royaltor auto populate search functionalities

        function royaltorListPopulating() {
            txtRoy = document.getElementById("<%= txtRoyaltor.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnRoySearchSelected.ClientID %>").value = "N";
        }

        function royaltorListPopulated() {
            txtRoy = document.getElementById("<%= txtRoyaltor.ClientID %>");
            txtRoy.style.backgroundImage = 'none';

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

        function ownerListPopulating() {
            txtOwner = document.getElementById("<%= txtOwner.ClientID %>");
            txtOwner.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtOwner.style.backgroundRepeat = 'no-repeat';
            txtOwner.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnOwnerSearchSelected.ClientID %>").value = "N";
        }

        function ownerListPopulated() {
            txtOwner = document.getElementById("<%= txtOwner.ClientID %>");
            txtOwner.style.backgroundImage = 'none';

        }

        function ownerListItemSelected(sender, args) {
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                document.getElementById("<%= txtOwner.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnOwnerSearchSelected.ClientID %>").value = "Y";
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

        //function OpenActivityScreen() {
        //    var win = window.open('RoyaltorActivity.aspx', '_self');
        //    win.focus();
        //}

        //function OpenWorkflowScreen() {
        //    var win = window.open('WorkFlow.aspx', '_self');
        //    win.focus();
        //}

    </script>
    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="5">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    ROYALTOR STATEMENT CHANGES
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="5%" class="identifierLable_large_bold">Royaltor
                    </td>
                    <td width="30%">
                        <table width="100%">
                            <tr>
                                <td width="90%">
                                    <asp:TextBox ID="txtRoyaltor" runat="server" Width="99%" CssClass="identifierLable"
                                        OnTextChanged="txtRoyaltor_TextChanged" AutoPostBack="true" TabIndex="100" onkeydown="OnTabPress();"></asp:TextBox>
                                    <ajaxToolkit:AutoCompleteExtender ID="royaltorFilterExtender" runat="server"
                                        ServiceMethod="FuzzySearchAllRoyaltorList"
                                        ServicePath="~/Services/FuzzySearch.asmx"
                                        MinimumPrefixLength="1"
                                        CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                        TargetControlID="txtRoyaltor"
                                        FirstRowSelected="true"
                                        OnClientPopulating="royaltorListPopulating"
                                        OnClientPopulated="royaltorListPopulated"
                                        OnClientHidden="royaltorListPopulated"
                                        OnClientItemSelected="royaltorListItemSelected"
                                        CompletionListElementID="autocompleteDropDownPanel1" />
                                    <asp:Panel ID="autocompleteDropDownPanel1" runat="server" CssClass="identifierLable" />
                                </td>
                                <td valign="top">
                                    <asp:ImageButton ID="fuzzySearchRoyaltor" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button" Style="cursor: pointer"
                                        OnClick="fuzzySearchRoyaltor_Click" ToolTip="Search Royaltor" />
                                </td>
                            </tr>
                        </table>

                    </td>
                    <td width="37.9%" align="left" rowspan="2" runat="server" id="tdButtons">
                        <table width="98%">
                            <tr>
                                <td align="right">
                                    <asp:Button ID="btnRemove" runat="server" Text="Remove" CssClass="ButtonStyle" OnClick="btnRemove_Click"
                                        UseSubmitBehavior="false" TabIndex="103" Width="40%" />
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Button ID="btnNextRunStmtActvitiy" runat="server" Text="Update Statement Activity List" CssClass="ButtonStyle"
                                        UseSubmitBehavior="false" OnClick="btnNextRunStmtActvitiy_Click" TabIndex="101" Width="40%" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td valign="top">
                        <%--<nextRun:StmtNextRunDetails ID="stmtNextRun" runat="server" />--%>
                        <table width="100%">
                            <tr>
                                <td width="80%"></td>
                                <td width="20%" align="right">
                                    <%--<asp:Button ID="btnOpenStatementActivity" runat="server" Text="Statement Activity" CssClass="ButtonStyle"
                                        Width="80%" UseSubmitBehavior="false" OnClientClick="OpenActivityScreen();" />--%>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td align="right">
                                    <%--<asp:Button ID="btnOpenWorkflow" runat="server" Text="Workflow" CssClass="ButtonStyle"
                                        Width="80%" UseSubmitBehavior="false" OnClientClick="OpenWorkflowScreen();" />--%>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">Owner
                    </td>
                    <td>
                        <table width="100%">
                            <tr>
                                <td width="90%">
                                    <asp:TextBox ID="txtOwner" runat="server" Width="99%" CssClass="identifierLable"
                                        OnTextChanged="txtOwner_TextChanged" AutoPostBack="true" TabIndex="101"></asp:TextBox>
                                    <ajaxToolkit:AutoCompleteExtender ID="ownerFilterExtender" runat="server"
                                        ServiceMethod="FuzzyStmtChangesOwnerList"
                                        ServicePath="~/Services/FuzzySearch.asmx"
                                        MinimumPrefixLength="1"
                                        CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                        TargetControlID="txtOwner"
                                        FirstRowSelected="true"
                                        OnClientPopulating="ownerListPopulating"
                                        OnClientPopulated="ownerListPopulated"
                                        OnClientHidden="ownerListPopulated"
                                        OnClientItemSelected="ownerListItemSelected"
                                        CompletionListElementID="autocompleteDropDownPanel2" />
                                    <asp:Panel ID="autocompleteDropDownPanel2" runat="server" CssClass="identifierLable" />
                                </td>
                                <td valign="top">
                                    <asp:ImageButton ID="fuzzySearchOwner" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button" Style="cursor: pointer"
                                        OnClick="fuzzySearchOwner_Click" ToolTip="Search Royaltor" />
                                </td>
                            </tr>
                        </table>

                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td align="right">&nbsp;</td>
                    <td></td>

                    <td></td>
                </tr>
                <%--<tr>
                    <td width="2%"></td>
                    <td align="right">&nbsp;</td>
                    <td></td>
                    <td></td>
                </tr>--%>
                <tr>
                    <%--<td width="2%"></td>--%>
                    <td colspan="5">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td align="right"></td>
                                <td width="1%"></td>
                                <%--<td>
                                    <asp:Button ID="btnAdd" runat="server" Text="Add" CssClass="ButtonStyle" OnClick="btnAdd_Click" UseSubmitBehavior="false" TabIndex="102"/>
                                </td>
                                <td></td>--%>
                                <td width="5%">
                                    <%--<asp:Button ID="btnRemove" runat="server" Text="Remove" CssClass="ButtonStyle" OnClick="btnRemove_Click" UseSubmitBehavior="false" TabIndex="103" />--%>
                                </td>
                                <%--<td width="20%">
                                    <asp:Button ID="btnClose" runat="server" Text="Exit" CssClass="ButtonStyle"
                                        OnClientClick="Javascript:window.close(); return false" UseSubmitBehavior="false" />
                                </td>--%>
                                <td width="26.7%"></td>
                            </tr>
                        </table>
                    </td>
                    <%--<td colspan="2"></td>--%>
                </tr>
                <tr>
                    <td colspan="4" align="left" runat="server" id="tdGrid">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td colspan="7">
                                    <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="100%">
                                        <asp:GridView ID="gvRoyStmt" runat="server" AutoGenerateColumns="False" Width="98.75%" AlternatingRowStyle-BackColor="#E3EFFF"
                                            CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" ShowHeaderWhenEmpty="true" EmptyDataText="No Data Found"
                                            OnRowDataBound="gvRoyStmt_RowDataBound" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" AllowSorting="true" OnSorting="gvRoyStmt_Sorting"
                                            HeaderStyle-CssClass="FixedHeader">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-Width="8%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Royaltor" SortExpression="royaltor_id">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRoyaltor" runat="server" Text='<%#Bind("royaltor_id")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Name" SortExpression="royaltor_name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblName" runat="server" Text='<%#Bind("royaltor_name")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="8%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Reporting Schedule" SortExpression="statement_type_code">
                                                    <ItemTemplate>
                                                        <asp:Label ID="hdnStmtPeriodID" runat="server" Text='<%#Bind("statement_period_id")%>' Visible="false"></asp:Label>
                                                        <asp:Label ID="lblRepSchedule" runat="server" Text='<%#Bind("statement_type_code")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Statement Period" SortExpression="stmt_period">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStmtPeriod" runat="server" Text='<%#Bind("stmt_period")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="8%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Status" SortExpression="status_desc">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStatus" runat="server" Text='<%#Bind("status_desc")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Add?">
                                                    <ItemTemplate>
                                                        <asp:Label ID="hdnAddRoy" runat="server" Text='<%#Bind("add_roy")%>' Visible="false" />
                                                        <asp:CheckBox ID="cbAdd" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Remove">
                                                    <ItemTemplate>
                                                        <asp:Label ID="hdnRemoveRoy" runat="server" Text='<%#Bind("remove_roy")%>' Visible="false" />
                                                        <asp:CheckBox ID="cbRemove" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="4" align="left" runat="server" id="tdOwner">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td>
                                    <asp:Panel ID="pnlOwnerGrid" runat="server" ScrollBars="Auto" Width="100%">
                                        <asp:GridView ID="gvOwnerStmt" runat="server" AutoGenerateColumns="False" Width="98.75%" AlternatingRowStyle-BackColor="#E3EFFF"
                                            CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" ShowHeaderWhenEmpty="true" EmptyDataText="No Data Found"
                                            OnRowDataBound="gvOwnerStmt_RowDataBound" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" AllowSorting="true" OnSorting="gvOwnerStmt_Sorting" HeaderStyle-CssClass="FixedHeader">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Owner" SortExpression="owner_code">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOwnerCode" runat="server" Text='<%#Bind("owner_code")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="40%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Name" SortExpression="owner_name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOwnerName" runat="server" Text='<%#Bind("owner_name")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="15%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Reporting Schedule" SortExpression="statement_type_code">
                                                    <ItemTemplate>
                                                        <asp:Label ID="hdnStmtPeriodID" runat="server" Text='<%#Bind("statement_period_id")%>' Visible="false"></asp:Label>
                                                        <asp:Label ID="lblRepSchedule" runat="server" Text='<%#Bind("statement_type_code")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="15%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Statement Period" SortExpression="stmt_period">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStmtPeriod" runat="server" Text='<%#Bind("stmt_period")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Add?">
                                                    <ItemTemplate>
                                                        <asp:Label ID="hdnAddOwner" runat="server" Text='<%#Bind("add_owner")%>' Visible="false" />
                                                        <asp:CheckBox ID="cbAdd" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Remove">
                                                    <ItemTemplate>
                                                        <asp:Label ID="hdnRemoveOwner" runat="server" Text='<%#Bind("remove_owner")%>' Visible="false" />
                                                        <asp:CheckBox ID="cbRemove" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td></td>
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

            <asp:Button ID="dummyConfirm" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirm" runat="server" PopupControlID="pnlPopup" TargetControlID="dummyConfirm"
                CancelControlID="btnNo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlPopup" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
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
                            <asp:ListBox ID="lbFuzzySearch" runat="server" Width="95%" CssClass="ListBox"
                                OnSelectedIndexChanged="lbFuzzySearch_SelectedIndexChanged" AutoPostBack="True"></asp:ListBox>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnAddBtnEnable" runat="server" />
            <asp:HiddenField ID="hdnRemoveBtnEnable" runat="server" />
            <asp:HiddenField ID="hdnRoySearchSelected" runat="server" />
            <asp:HiddenField ID="hdnOwnerSearchSelected" runat="server" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" Value="" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>
    <activityScreen:ActivityScreen ID="actScreen" runat="server" />


</asp:Content>

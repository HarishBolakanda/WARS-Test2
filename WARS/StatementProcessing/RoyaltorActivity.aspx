<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoyaltorActivity.aspx.cs" Inherits="WARS.RoyaltorActivity1" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Royaltor Activity" MaintainScrollPositionOnPostback="true" ClientIDMode="AutoID" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
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
                        <asp:Button ID="btnOpenWorkflow" runat="server" CssClass="LinkButtonStyle"
                            OnClientClick="OpenWorkflowScreen();" Text="Workflow"
                            UseSubmitBehavior="false" Width="98%" CausesValidation="false" />
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
            if (postBackElementID == 'btnRemove') {
                xPos = sender._scrollPosition.x;
                yPos = sender._scrollPosition.y;

                //hold scroll position on selecting expand/collapse
                var PnlReference = document.getElementById("<%=PnlActivity.ClientID %>");
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
            if (postBackElementID == 'btnRemove') {
                window.scrollTo(xPos, yPos);

                //set scroll position on selecting expand/collapse
                var PnlReference = document.getElementById("<%=PnlActivity.ClientID %>");
                PnlReference.scrollTop = scrollTop;
            }


        }
        //probress bar and scroll position functionality - ends

        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.55;
            document.getElementById("<%=PnlActivity.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }
        function ValidateChanges() {
            eval(this.href);
        }

        //grid panel height adjustment functioanlity - ends


        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        //=============== End

        //WUIN-908 Open Confirmation pop up on Run Requested Statements
        function OpenConfirmPopup() {
            var popup = $find('<%= mpeRoyaltyEngine.ClientID %>');
             if (popup != null) {
                 popup.show();
             }
             return false;
         }


    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table id="tblMain" style="width: 100%;">
                <tr>
                    <td>
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    STATEMENT ACTIVITY LIST
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>

                <tr>
                    <td align="left">
                        <table width="100%">
                            <tr>
                                <td width="5%"></td>
                                <td>
                                    <table width="100%" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>
                                                <table width="98.5%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Button ID="btnRefresh" runat="server" CssClass="ButtonStyle" OnClick="btnRefresh_Click" Text="Refresh"
                                                                UseSubmitBehavior="false" Width="15%" TabIndex="101" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Button ID="btnRoyaltyEngine" runat="server" CssClass="ButtonStyle" Text="Run Requested Statements"
                                                                UseSubmitBehavior="false" Width="15%" TabIndex="102" onkeydown="OnTabPress();" OnClientClick="return OpenConfirmPopup();" />
                                                            <%--JIRA-908 CHanges--%>
                                                        </td>
                                                    </tr>
                                                </table>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <asp:Panel ID="PnlActivity" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvRoyActivity" runat="server" AutoGenerateColumns="False" Width="98.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" ShowHeaderWhenEmpty="true" EmptyDataText="No Data Found"
                                                        EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" OnRowDataBound="gvRoyActivity_RowDataBound" AllowSorting="true" OnSorting="gvRoyActivity_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-Width="9%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Type" SortExpression="type">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRoyType" runat="server" Text='<%#Bind("type")%>' CssClass="identifierLable" />
                                                                    <asp:HiddenField ID="hdnLevelFlag" runat="server" Value='<%#Bind("level_flag")%>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="9%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Code" SortExpression="code">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblCode" runat="server" Text='<%#Bind("code")%>' CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="19%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Name" SortExpression="name">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblName" runat="server" Text='<%#Bind("name")%>' CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="14%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Reporting Schedule" SortExpression="rep_schedule">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRepSchedule" runat="server" Text='<%#Bind("rep_schedule")%>' CssClass="identifierLable" />
                                                                    <asp:Label ID="lblStmtPeriodId" runat="server" Text='<%#Bind("stmt_period_id")%>' Visible="false" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="9%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="User" SortExpression="user_code">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblUserCode" runat="server" Text='<%#Bind("user_code")%>' CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Date Added" SortExpression="last_modified">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblDateAdded" runat="server" Text='<%#Bind("date_added")%>' CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="15%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Run Status" SortExpression="run_status">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRunStatus" runat="server" Text='<%#Bind("run_status")%>' CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <HeaderTemplate>
                                                                    Remove
                                                                      <asp:ImageButton ID="btnRemoveAll" ImageUrl="../Images/Delete.gif" runat="server" ToolTip="Remove all from run"
                                                                          OnClick="btnRemoveAll_Click" />
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:ImageButton ID="btnRemove" ImageUrl="../Images/Delete.gif" runat="server" ToolTip="Remove from run"
                                                                        OnClick="btnRemove_Click" />
                                                                    <asp:Label ID="lblCanBeRemoved" runat="server" Text='<%#Bind("remove_from_run")%>' Visible="false" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <HeaderTemplate>
                                                                    Retry
                                                                    <asp:ImageButton ID="btnRetryAll" ImageUrl="../Images/Retry.jpg" runat="server" ToolTip="Retry all"
                                                                        OnClick="btnRetryAll_Click" />

                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:ImageButton ID="btnRetry" ImageUrl="../Images/Retry.jpg" runat="server" ToolTip="Retry"
                                                                        OnClick="btnRetry_Click" />
                                                                    <asp:Label ID="lblRetry" runat="server" Text='<%#Bind("royaltor_stmt_flag")%>' Visible="false" />
                                                                    <asp:HiddenField ID="hdnDetailFlag" runat="server" Value='<%#Bind("dtl_file_flag")%>' />
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
                        </table>
                    </td>
                </tr>
            </table>
            <div align="center">
                <asp:Repeater ID="rptPager" runat="server">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>'
                            OnClientClick="return ValidateChanges();" ClientIDMode="AutoID" Enabled='<%# Eval("Enabled") %>' OnClick="lnkPage_Click" CssClass="gridPager"> </asp:LinkButton>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <asp:UpdateProgress ID="progressBarPageLevel" runat="server" AssociatedUpdatePanelID="updPnlPageLevel" DisplayAfter="100">
                <ProgressTemplate>
                    <div id="Search" style="font-weight: bold; color: Black">
                        <table>
                            <tr>
                                <td>
                                    <img src="../Images/InProgress2.gif" alt="" />
                                </td>
                            </tr>
                            <tr>
                                <td class="identifierLable">Please Wait...
                                </td>
                            </tr>
                        </table>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <ajaxToolkit:ModalPopupExtender ID="mPopupPageLevel" runat="server" PopupControlID="progressBarPageLevel" TargetControlID="progressBarPageLevel"
                BackgroundCssClass="progressBar" RepositionMode="RepositionOnWindowResize">
            </ajaxToolkit:ModalPopupExtender>

            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- Start--%>
            <asp:Button ID="dummyRoyaltyEngine" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeRoyaltyEngine" runat="server" PopupControlID="pnlRoyaltyEngine" TargetControlID="dummyRoyaltyEngine"
                CancelControlID="btnNo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlRoyaltyEngine" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmation" runat="server" Text="Run RoyaltyEngine Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblText" runat="server"
                                CssClass="identifierLable" Text="Do you want to run the Royalty Engine?"></asp:Label>
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
            <asp:HiddenField ID="hdnPageNumber" runat="server" Value="" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

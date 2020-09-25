<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OwnerAudit.aspx.cs" Inherits="WARS.OwnerAudit" MasterPageFile="~/MasterPage.Master"
    Title="WARS - OwnerAudit " MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<%--<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //to open Configuration search screen in same tab
        function OpenOwnerMaintScreen() {
            var win = window.open('OwnerMaintenance.aspx', '_self');
            win.focus();
        }

    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnOwnerMaint" runat="server" Text="Owner Maintenance" CssClass="LinkButtonStyle" 
                            width="98%"  UseSubmitBehavior="false" OnClientClick="OpenOwnerMaintScreen();" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>--%>

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
            if (postBackElementID.lastIndexOf('gvOwnerDetails') > 0 || postBackElementID.lastIndexOf('btnSaveChanges') > 0) {
                xPos = sender._scrollPosition.x;
                yPos = sender._scrollPosition.y;

                //hold scroll position on selecting expand/collapse
                var PnlOwnerDetails = document.getElementById("<%=PnlOwnerDetails.ClientID %>");
                scrollTop = PnlOwnerDetails.scrollTop;

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
            if (postBackElementID.lastIndexOf('gvOwnerDetails') > 0 || postBackElementID.lastIndexOf('btnSaveChanges') > 0) {
                window.scrollTo(xPos, yPos);

                //set scroll position on selecting expand/collapse
                var PnlOwnerDetails = document.getElementById("<%=PnlOwnerDetails.ClientID %>");
                PnlOwnerDetails.scrollTop = scrollTop;
            }


        }
        //======================= End


        //Fuzzy search filters
        var txtOwnerSearch;
        function OwnerSelected(sender, args) {

            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtOwnerSearch.ClientID %>").value = "";
                document.getElementById("<%= hdnIsValidSearch.ClientID %>").value = "N";
            }
            else {
                document.getElementById('<%=btnHdnOwnerSearch.ClientID%>').click();
            }
        }

        function OwnerListPopulating() {
            txtOwnerSearch = document.getElementById("<%= txtOwnerSearch.ClientID %>");
            txtOwnerSearch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtOwnerSearch.style.backgroundRepeat = 'no-repeat';
            txtOwnerSearch.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidSearch.ClientID %>").value = "N";
        }

        function OwnerListPopulated() {
            txtOwnerSearch = document.getElementById("<%= txtOwnerSearch.ClientID %>");
            txtOwnerSearch.style.backgroundImage = 'none';
        }

        //=============== End



        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
            document.getElementById("<%=PnlOwnerDetails.ClientID %>").style.height = gridPanelHeight + "px";
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
    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="7">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    OWNER AUDIT
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="8%"></td>
                    <td width="5%" class="identifierLable_large_bold">Owner</td>
                    <td width="24%">
                        <asp:TextBox ID="txtOwnerSearch" runat="server" Width="99%" CssClass="textboxStyle"
                            TabIndex="100" onkeydown="OnTabPress();"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="aceOwnerSearch" runat="server"
                            ServiceMethod="FuzzySearchAllOwnerList"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtOwnerSearch"
                            FirstRowSelected="true"
                            OnClientItemSelected="OwnerSelected"
                            OnClientPopulating="OwnerListPopulating"
                            OnClientPopulated="OwnerListPopulated"
                            OnClientHidden="OwnerListPopulated"
                            CompletionListElementID="acePnlOwner" />
                        <asp:Panel ID="acePnlOwner" runat="server" CssClass="identifierLable" />
                    </td>
                    <td width="3%" align="left">
                        <asp:ImageButton ID="fuzzySearchOwner" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button"
                            OnClick="fuzzySearchOwner_Click" ToolTip="Search owner code/name" />
                    </td>
                    <td></td>
                    <td width="3%"></td>
                    <td align="right" width="12%">
                        <asp:Button ID="btnOwnerMaint" runat="server" Text="Owner Maintenance" CssClass="ButtonStyle"
                            Width="98%" UseSubmitBehavior="false" OnClick="btnOwnerMaint_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="7">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="5" class="table_with_border" runat="server" id="tdData">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="3%">
                                    <br />
                                </td>
                                <td width="94%"></td>
                                <td width="3%"></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlOwnerDetails" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvOwnerDetails" runat="server" AutoGenerateColumns="False" Width="98%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." AllowSorting="true" OnSorting="gvOwnerDetails_Sorting" HeaderStyle-CssClass="FixedHeader" OnRowDataBound="gvOwnerDetails_RowDataBound">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Owner" SortExpression="owner_code">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Bind("owner_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="15%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Description" SortExpression="owner_name">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblOwnerDesc" runat="server" Text='<%# Bind("owner_name") %>' CssClass="identifierLable"></asp:Label>
                                                                    <asp:HiddenField ID="hdnClrOwnerDesc" runat="server" Value='<%# Bind("owner_name_color") %>'></asp:HiddenField>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="55%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Update By" SortExpression="user_code">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblUserCode" runat="server" Text='<%# Bind("user_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="15%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Update On" SortExpression="last_modified">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblLastModified" runat="server" Text='<%# Bind("last_modified") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="15%" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </asp:Panel>
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
                                    </table>
                                </td>
                                <td></td>
                            </tr>
                        </table>
                    </td>
                    <td colspan="2"></td>
                </tr>
            </table>

            <asp:Button ID="dummyFuzzySearch" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeFuzzySearch" runat="server" PopupControlID="pnlFuzzySearch" TargetControlID="dummyFuzzySearch"
                BackgroundCssClass="popupBox" CancelControlID="btnCloseFuzzySearchPopup">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlFuzzySearch" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
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

            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnIsValidSearch" runat="server" Value="N" />
            <asp:Button ID="btnHdnOwnerSearch" runat="server" Style="display: none;" OnClick="btnHdnOwnerSearch_Click" CausesValidation="false" />
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:TextBox>
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

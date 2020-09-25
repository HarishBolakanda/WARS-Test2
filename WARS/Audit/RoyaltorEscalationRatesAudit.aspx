<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoyaltorEscalationRatesAudit.aspx.cs" Inherits="WARS.Audit.RoyaltorEscalationRatesAudit" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Royaltor Escalation Rates Audit" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>
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

        }

        function EndRequestHandler(sender, args) {
            //Hide the modal popup - the update progress
            var popup = $find('<%= mPopupPageLevel.ClientID %>');
            if (popup != null) {
                popup.hide();
            }

        }
        //======================= End


        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        function FocusLblKeyPress() {
            document.getElementById("<%= txtRoyaltorSearch.ClientID %>").focus();
        }
        //Fuzzy search filters
        var txtRoyaltorSearch;
        function RoyaltorSelected(sender, args) {

            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtRoyaltorSearch.ClientID %>").value = "";
                document.getElementById("<%= hdnIsValidSearch.ClientID %>").value = "N";
            }
            else {
                document.getElementById("<%= hdnIsValidSearch.ClientID %>").value = "Y";
            }
        }

        function RoyaltorListPopulating() {
            txtRoyaltorSearch = document.getElementById("<%= txtRoyaltorSearch.ClientID %>");
            txtRoyaltorSearch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoyaltorSearch.style.backgroundRepeat = 'no-repeat';
            txtRoyaltorSearch.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidSearch.ClientID %>").value = "N";
        }

        function RoyaltorListPopulated() {
            txtRoyaltorSearch = document.getElementById("<%= txtRoyaltorSearch.ClientID %>");
            txtRoyaltorSearch.style.backgroundImage = 'none';
        }

        //=============== End

        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
            document.getElementById("<%=PnlEscalationRatesAudit.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }


        function RedirectToMaintScreen(royaltorId) {
            window.location = "../Contract/RoyContractEscRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=N";
        }


        //WUIN-599 Unset the screen lock flag If an user close the browser or navigate to other than contract screens
        window.onbeforeunload = function () {
            var isOtherUserScreenLocked = document.getElementById("<%=hdnOtherUserScreenLocked.ClientID %>").value;
            var isMaintScreen = document.getElementById("<%=hdnIsMaintScreen.ClientID %>").value;

            if (isOtherUserScreenLocked == "N" && isMaintScreen == "N") {
                PageMethods.UpdateScreenLockFlag();
            }
        }
    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="10">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    ESCALATION RATES AUDIT 
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="10"></td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="6%" class="identifierLable_large_bold">Royaltor</td>
                    <td colspan="3" style="padding: 2px; padding-right: 4px;">
                        <asp:TextBox ID="txtRoyaltorSearch" runat="server" Width="99%" CssClass="textboxStyle"
                            TabIndex="100"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="aceRoyaltorSearch" runat="server"
                            ServiceMethod="FuzzySearchAllRoyaltorList"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtRoyaltorSearch"
                            FirstRowSelected="true"
                            OnClientItemSelected="RoyaltorSelected"
                            OnClientPopulating="RoyaltorListPopulating"
                            OnClientPopulated="RoyaltorListPopulated"
                            OnClientHidden="RoyaltorListPopulated"
                            CompletionListElementID="acePnlRoyaltor" />
                        <asp:Panel ID="acePnlRoyaltor" runat="server" CssClass="identifierLable" />
                    </td>
                    <td width="3%" align="left">
                        <asp:CustomValidator ID="valRoyaltor" runat="server" ValidationGroup="valSearch" CssClass="requiredFieldValidator" Display="Dynamic"
                            OnServerValidate="valRoyaltor_ServerValidate" ErrorMessage="*" ToolTip="Please select a royaltor from list"></asp:CustomValidator>
                        <asp:ImageButton ID="fuzzySearchRoyaltor" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button"
                            OnClick="fuzzySearchRoyaltor_Click" ToolTip="Search royaltor code/name" />
                    </td>
                    <td></td>
                    <td></td>
                    <td align="right" colspan="2">
                        <asp:Button ID="btnEscalationRates" runat="server" Text="Escalation Rates" CssClass="ButtonStyle"
                            Width="98%" UseSubmitBehavior="false" OnClick="btnEscalationRates_Click" TabIndex="104" onkeydown="OnTabPress();" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">From Date</td>
                    <td width="8%" style="padding: 2px">
                        <asp:TextBox ID="txtFromDate" runat="server" Width="65" CssClass="identifierLable"
                            ValidationGroup="valSave" TabIndex="101"></asp:TextBox>
                        <ajaxToolkit:MaskedEditExtender ID="mteFromDate" runat="server"
                            TargetControlID="txtFromDate" Mask="99/99/9999" AcceptNegative="None"
                            ClearMaskOnLostFocus="false" />
                        <asp:CustomValidator ID="valFromDate" runat="server" ValidationGroup="valSearch" CssClass="requiredFieldValidator"
                            OnServerValidate="valFromDate_ServerValidate" ErrorMessage="*" ToolTip="Please enter a valid date in DD/MM/YYYY format"></asp:CustomValidator>
                    </td>
                    <td class="identifierLable_large_bold" width="4%" align="right">To Date</td>
                    <td width="6%" align="right" style="padding: 2px;">
                        <asp:TextBox ID="txtToDate" runat="server" Width="65" CssClass="identifierLable"
                            ValidationGroup="valSave" TabIndex="102"></asp:TextBox>
                        <ajaxToolkit:MaskedEditExtender ID="mteToDate" runat="server"
                            TargetControlID="txtToDate" Mask="99/99/9999" AcceptNegative="None"
                            ClearMaskOnLostFocus="false" />
                    </td>
                    <td>
                        <asp:CustomValidator ID="valToDate" runat="server" ValidationGroup="valSearch" CssClass="requiredFieldValidator"
                            OnServerValidate="valToDate_ServerValidate" ErrorMessage="*" ToolTip="Please enter a valid date in DD/MM/YYYY format"></asp:CustomValidator>
                    </td>
                    <td width="10%"></td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td style="padding: 2px">
                        <asp:Button ID="btnSearch" runat="server" Text="Go" CssClass="ButtonStyle"
                            Width="50%" TabIndex="103" UseSubmitBehavior="false" OnClick="btnSearch_Click" />
                    </td>
                    <td colspan="5">
                        <br />
                    </td>
                    <td width="10%"></td>
                    <td width="2%"></td>
                </tr>
                <tr>
                    <td colspan="10">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td class="table_header_with_border" colspan="9" runat="server" id="tdDataHeader">Royalty Rates</td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="8" class="table_with_border" runat="server" id="tdData">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="1%"></td>
                                <td width="8%"></td>
                                <td width="15%"></td>
                                <td></td>
                                <td width="1%"></td>
                            </tr>
                            <tr>
                                <td colspan="5"></td>
                            </tr>
                            <tr>
                                <td colspan="5"></td>
                            </tr>
                            <tr>
                                <td colspan="5"></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td colspan="3">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr runat="server" id="trRoyAudit">
                                            <td>
                                                <asp:Panel ID="PnlEscalationRatesAudit" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvEscalationRatesAudit" runat="server" AutoGenerateColumns="False" Width="98.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." OnRowDataBound="gvEscalationRatesAudit_RowDataBound" AllowSorting="true" OnSorting="gvEscalationRatesAudit_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Esc #" SortExpression="esc_code">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnRateId" runat="server" Value='<%# Bind("escalation_profile_id") %>' />
                                                                    <asp:Label ID="lblEscCode" runat="server" Text='<%# Bind("esc_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="15%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Territory" SortExpression="seller">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblTerritory" runat="server" Text='<%# Bind("seller") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="13%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Configuration Code" SortExpression="config">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblConfigCode" runat="server" Text='<%# Bind("config") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="15%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Sales Type" SortExpression="price">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSalesType" runat="server" Text='<%# Bind("price") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Sales Above" SortExpression="threshold_units">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSalesAbove" runat="server" Text='<%# Bind("threshold_units") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="% of Sales" SortExpression="sales_pct">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSalesPerc" runat="server" Text='<%# Bind("sales_pct") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Royalty Rate" SortExpression="royalty_rate">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRoyaltyRate" runat="server" Text='<%# Bind("royalty_rate","{0:0.####}") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Unit Rate" SortExpression="unit_rate">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblUnitRate" runat="server" Text='<%# Bind("unit_rate","{0:0.####}") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Revenue Rate" SortExpression="revenue_rate">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRevenueRate" runat="server" Text='<%# Bind("revenue_rate","{0:0.####}") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="7%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Updated by" SortExpression="user_code">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblUpdatedby" runat="server" Text='<%# Bind("user_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="9%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Updated on" SortExpression="last_modified">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblUpdatedon" runat="server" Text='<%# Bind("last_modified") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="7%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Deleted by" SortExpression="deleted_by">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblDeletedby" runat="server" Text='<%# Bind("deleted_by") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="9%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Deleted on" SortExpression="deleted_on">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnChangeType" runat="server" Value='<%# Bind("change_type") %>' />
                                                                    <asp:Label ID="lblDeletedon" runat="server" Text='<%# Bind("deleted_on") %>' CssClass="identifierLable"></asp:Label>
                                                                     <asp:HiddenField ID="hdnClrEscCode" runat="server" Value='<%# Bind("clr_esc_code") %>' />
                                                                     <asp:HiddenField ID="hdnClrSeller" runat="server" Value='<%# Bind("clr_seller") %>' />
                                                                     <asp:HiddenField ID="hdnClrConfig" runat="server" Value='<%# Bind("clr_config") %>' />
                                                                     <asp:HiddenField ID="hdnClrPrice" runat="server" Value='<%# Bind("clr_price") %>' />
                                                                     <asp:HiddenField ID="hdnClrThreshUnits" runat="server" Value='<%# Bind("clr_threshold_units") %>' />
                                                                     <asp:HiddenField ID="hdnClrSalesPct" runat="server" Value='<%# Bind("clr_sales_pct") %>' />
                                                                     <asp:HiddenField ID="hdnClrRoyRate" runat="server" Value='<%# Bind("clr_royalty_rate") %>' />
                                                                     <asp:HiddenField ID="hdnClrUnitRate" runat="server" Value='<%# Bind("clr_unit_rate") %>' />
                                                                     <asp:HiddenField ID="hdnClrRevenueRate" runat="server" Value='<%# Bind("clr_revenue_rate") %>' />
                                                                </ItemTemplate>
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
                                    </table>
                                </td>
                                <td></td>
                            </tr>
                        </table>
                    </td>
                    <td></td>
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
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnRoyaltorId" runat="server" />
            <asp:HiddenField ID="hdnIsValidSearch" runat="server" Value="N" />
            <asp:HiddenField ID="hdnOtherUserScreenLocked" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsMaintScreen" runat="server" Value="N" />
            <asp:Label ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:Label>
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>



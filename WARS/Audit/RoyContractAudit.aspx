<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoyContractAudit.aspx.cs" Inherits="WARS.Contract.RoyContractAudit" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Royaltor Contract Audit" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

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
            }
            else {
                document.getElementById('<%=btnHdnRoyaltorSearch.ClientID%>').click();
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
            var gridPanelHeight = windowHeight * 0.6;
            document.getElementById("<%=PnlRoyaltorAuditDetails.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=pnlRoyaltorRsvAuditDetails.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        function RedirectToMaintScreen(royaltorId) {
            window.location = "../Contract/RoyaltorContract.aspx?RoyaltorId=" + royaltorId;
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
                    <td colspan="7">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    ROYALTOR CONTRACT - AUDIT DETAILS
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="7"></td>
                </tr>
                <tr>
                    <td width="6%"></td>
                    <td width="5%" class="identifierLable_large_bold">Royaltor</td>
                    <td width="24%">
                        <asp:TextBox ID="txtRoyaltorSearch" runat="server" Width="99%" CssClass="textboxStyle"
                            TabIndex="100" onkeydown="OnTabPress();"></asp:TextBox>
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
                        <asp:ImageButton ID="fuzzySearchRoyaltor" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button"
                            OnClick="fuzzySearchRoyaltor_Click" ToolTip="Search royaltor code/name" />
                    </td>
                    <td></td>
                    <td align="right" colspan="2">
                        <asp:Button ID="btnReserveAudit" runat="server" Text="Reserves Audit" CssClass="ButtonStyle"
                            Width="98%" UseSubmitBehavior="false" OnClick="btnReserveAudit_Click" />
                        <asp:Button ID="btnContractAudit" runat="server" Text="Contract Audit" CssClass="ButtonStyle"
                            Width="98%" UseSubmitBehavior="false" OnClick="btnContractAudit_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="5"></td>
                    <td colspan="2" align="right">
                        <asp:Button ID="btnContractMaint" runat="server" Text="Contract Maintenance" CssClass="ButtonStyle"
                            Width="98%" UseSubmitBehavior="false" OnClick="btnContractMaint_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="5"></td>
                    <td width="6%"></td>
                    <td width="6%"></td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="5" class="table_with_border" runat="server" id="tdData">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="3%">
                                    <br />
                                </td>
                                <td width="95%"></td>
                                <td width="2%"></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td></td>
                                        </tr>
                                        <tr runat="server" id="trRoyAudit">
                                            <td>
                                                <asp:Panel ID="PnlRoyaltorAuditDetails" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvRoyaltorAuditDetails" runat="server" AutoGenerateColumns="False" Width="98%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." ShowHeader="False">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <table width="100%" cellspacing="0">
                                                                        <tr>
                                                                            <td width="10%"></td>
                                                                            <td width="18%"></td>
                                                                            <td width="13%"></td>
                                                                            <td width="10%"></td>
                                                                            <td width="15%"></td>
                                                                            <td width="12%"></td>
                                                                            <td width="10%"></td>
                                                                            <td width="12%"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Royaltor Name
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblRoyaltorName" runat="server" Text='<%# Bind("royaltor_name") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">PLG Royaltor Number
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblPLGRoyaltorNumber" runat="server" Text='<%# Bind("royaltor_plg_id") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Reporting Schedule
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblReportingSchedule" runat="server" Text='<%# Bind("statement_type_code") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Updated by
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblUserCode" runat="server" Text='<%# Bind("user_code") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Company
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblCompany" runat="server" Text='<%# Bind("company_name") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Status
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("status") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Statement Priority
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblStatementPriority" runat="server" Text='<%# Bind("priority_desc") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Updated on
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblLastModified" runat="server" Text='<%# Bind("last_modified") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Owner
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblOwner" runat="server" Text='<%# Bind("owner_name") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Lock
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblLock" runat="server" Text='<%# Bind("royaltor_locked") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Print Group
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblPrintGroup" runat="server" Text='<%# Bind("print_stream") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                            <td></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Responsibility
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblResponsibility" runat="server" Text='<%# Bind("responsibility_desc") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Signed
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblSigned" runat="server" Text='<%# Bind("signed") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Display 0 Values on Stmts
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblZeroValue" runat="server" Text='<%# Bind("stmt_display_zero") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                            <td></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Label
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblLabel" runat="server" Text='<%# Bind("label_description") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Held
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblHeld" runat="server" Text='<%# Bind("royaltor_held") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>

                                                                            <td class="identifierLable_large_bold" align="left">Producer Summary
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblProducerSummary" runat="server" Text='<%# Bind("stmt_producer_report") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                            <td></td>
                                                                        </tr>

                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Contract Type
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblContractTypeDesc" runat="server" Text='<%# Bind("contract_type_desc") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Statement Format
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblStatementFormatDesc" runat="server" Text='<%# Bind("statement_format_desc") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Cost Report
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblCostReport" runat="server" Text='<%# Bind("stmt_cost_report") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                            <td></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Start Date
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblStartDate" runat="server" Text='<%# Bind("contract_start_date") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Expiry Date
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblExpiryDate" runat="server" Text='<%# Bind("contract_expiry_date") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>

                                                                            <td class="identifierLable_large_bold" align="left">Chargeable %
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblChargeablePercent" runat="server" Text='<%# Bind("parent_contribution_pct") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>

                                                                            <td></td>
                                                                            <td></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Chargeable To
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblChargeableTo" runat="server" Text='<%# Bind("chargeble_to") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Royaltor Type
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblRoyaltorType" runat="server" Text='<%# Bind("royaltor_type") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Send to Portal
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblSendtoPortal" runat="server" Text='<%# Bind("send_to_portal") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                            <td></td>
                                                                        </tr>
                                                                        <tr>

                                                                            <%--JIRA-970 CHanges by Ravi on 14/02/2019 -- Start--%>
                                                                            <td class="identifierLable_large_bold" align="left">Soc Sec No
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblSocSecNo" runat="server" Text='<%# Bind("soc_sec_no") %>' CssClass="identifierLable" Style="text-transform: uppercase"></asp:Label>
                                                                            </td>
                                                                            <%--JIRA-970 CHanges by Ravi on 14/02/2019 -- End--%>
                                                                            <td></td>
                                                                            <td></td>
                                                                            <%--JIRA-1006 CHanges by Ravi on 29/04/2019 -- Start--%>
                                                                            <td class="identifierLable_large_bold" align="left">Exclude from Accrual
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblExcludeFromAccrual" runat="server" Text='<%# Bind("exclude_from_accrual") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <%--JIRA-1006 CHanges by Ravi on 29/04/2019 -- End--%>
                                                                        </tr>
                                                                    </table>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="95%" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <tr runat="server" id="trRsvAudit">
                                            <td>
                                                <asp:Panel ID="pnlRoyaltorRsvAuditDetails" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvRoyaltorRsvAuditDetails" runat="server" AutoGenerateColumns="False" Width="98%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." ShowHeader="False">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <table width="100%" cellspacing="0">
                                                                        <tr>
                                                                            <td width="20%"></td>
                                                                            <td width="15%"></td>
                                                                            <td width="20%"></td>
                                                                            <td width="10%"></td>
                                                                            <td width="15%"></td>
                                                                            <td width="10%"></td>
                                                                            <td width="10%"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Products Reserves Taken On
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblRsvTakenOn" runat="server" Text='<%# Bind("reserve_prod_type") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Default Royaltor Reserve %
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblRsvPercent" runat="server" Text='<%# Bind("reserve_pct") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                            <td class="identifierLable_large_bold" align="left">Updated by
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblUserCode" runat="server" Text='<%# Bind("user_code") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Reserves End Date
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblRsvEndDate" runat="server" Text='<%# Bind("reserve_end_date") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Nett units / Sales units (N/S)
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblUnits" runat="server" Text='<%# Bind("reserve_sales_flag") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                            <td class="identifierLable_large_bold" align="left">Updated on
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblLastModified" runat="server" Text='<%# Bind("last_modified") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Liquidation Period %
                                                                            </td>
                                                                            <td colspan="4" align="left">
                                                                                <table width="100%" cellpadding="0" cellspacing="0">
                                                                                    <tr>
                                                                                        <td class="identifierLable_large_bold" align="center" width="6%">1:</td>
                                                                                        <td align="left" width="6.5%">
                                                                                            <asp:Label ID="lblInterval1" runat="server" Text='<%# Bind("1") %>' CssClass="identifierLable"></asp:Label>
                                                                                        </td>
                                                                                        <td class="identifierLable_large_bold" align="center" width="6%">2:</td>
                                                                                        <td align="left" width="6.5%">
                                                                                            <asp:Label ID="lblInterval2" runat="server" Text='<%# Bind("2") %>' CssClass="identifierLable"></asp:Label>
                                                                                        </td>
                                                                                        <td class="identifierLable_large_bold" align="center" width="6%">3:</td>
                                                                                        <td align="left" width="6.5%">
                                                                                            <asp:Label ID="lblInterval3" runat="server" Text='<%# Bind("3") %>' CssClass="identifierLable"></asp:Label>
                                                                                        </td>
                                                                                        <td class="identifierLable_large_bold" align="center" width="6%">4:</td>
                                                                                        <td align="left" width="6.5%">
                                                                                            <asp:Label ID="lblInterval4" runat="server" Text='<%# Bind("4") %>' CssClass="identifierLable"></asp:Label>
                                                                                        </td>
                                                                                        <td class="identifierLable_large_bold" align="center" width="6%">5:</td>
                                                                                        <td align="left" width="6.5%">
                                                                                            <asp:Label ID="lblInterval5" runat="server" Text='<%# Bind("5") %>' CssClass="identifierLable"></asp:Label>
                                                                                        </td>
                                                                                        <td class="identifierLable_large_bold" align="center" width="6%">6:</td>
                                                                                        <td align="left" width="6.5%">
                                                                                            <asp:Label ID="lblInterval6" runat="server" Text='<%# Bind("6") %>' CssClass="identifierLable"></asp:Label>
                                                                                        </td>
                                                                                        <td class="identifierLable_large_bold" align="center" width="6%">7:</td>
                                                                                        <td align="left" width="6.5%">
                                                                                            <asp:Label ID="lblInterval7" runat="server" Text='<%# Bind("7") %>' CssClass="identifierLable"></asp:Label>
                                                                                        </td>
                                                                                        <td class="identifierLable_large_bold" align="center" width="6%">8:</td>
                                                                                        <td align="left" width="6.5%">
                                                                                            <asp:Label ID="lblInterval8" runat="server" Text='<%# Bind("8") %>' CssClass="identifierLable"></asp:Label>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                            <td></td>
                                                                            <td></td>
                                                                        </tr>
                                                                    </table>

                                                                </ItemTemplate>
                                                                <ItemStyle Width="95%" />
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
            <asp:HiddenField ID="hdnIsValidSearch" runat="server" Value="N" />
            <asp:HiddenField ID="hdnOtherUserScreenLocked" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsMaintScreen" runat="server" Value="N" />
            <asp:Button ID="btnHdnRoyaltorSearch" runat="server" Style="display: none;" OnClick="btnHdnRoyaltorSearch_Click" CausesValidation="false" />
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField" onkeydown="FocusLblKeyPress();"></asp:TextBox>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

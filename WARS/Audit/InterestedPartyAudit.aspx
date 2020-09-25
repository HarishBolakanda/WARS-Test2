<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InterestedPartyAudit.aspx.cs" Inherits="WARS.InterestedPartyAudit" MasterPageFile="~/MasterPage.Master"
    Title="WARS - InterestedPartyAudit " MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

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

        //Validate any unsaved data on browser window close/refresh
        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.6;
            document.getElementById("<%=PnlInterestedPartyDetails.ClientID %>").style.height = gridPanelHeight + "px";
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

        //open Audit screen
        function OpenAuditScreen() {
            //debugger;
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../Audit/InterestedPartyAudit.aspx');
            }
            else {
                return true;
            }
        }
        //==============End


        //Fuzzy search filters
        var txtInterestedPartySearch;
        function InterestedPartySelected(sender, args) {

            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtInterestedPartySearch.ClientID %>").value = "";
            }
            else {
                document.getElementById('<%=btnHdnInterestedPartySearch.ClientID%>').click();
            }
        }

        function InterestedPartyListPopulating() {
            txtInterestedPartySearch = document.getElementById("<%= txtInterestedPartySearch.ClientID %>");
            txtInterestedPartySearch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtInterestedPartySearch.style.backgroundRepeat = 'no-repeat';
            txtInterestedPartySearch.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidSearch.ClientID %>").value = "N";
        }

        function InterestedPartyListPopulated() {
            txtInterestedPartySearch = document.getElementById("<%= txtInterestedPartySearch.ClientID %>");
            txtInterestedPartySearch.style.backgroundImage = 'none';
        }

        //=============== End
    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="10">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    INTERESTED PARTY AUDIT
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="1%"></td>
                    <td width="10%" class="identifierLable_large_bold">Interested Party</td>
                    <td width="24%">
                        <asp:TextBox ID="txtInterestedPartySearch" runat="server" Width="99%" CssClass="textboxStyle"
                            TabIndex="100"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="aceInterestedPartySearch" runat="server"
                            ServiceMethod="FuzzySearchIntPartyList"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtInterestedPartySearch"
                            FirstRowSelected="true"
                            OnClientItemSelected="InterestedPartySelected"
                            OnClientPopulating="InterestedPartyListPopulating"
                            OnClientPopulated="InterestedPartyListPopulated"
                            OnClientHidden="InterestedPartyListPopulated"
                            CompletionListElementID="acePnlInterestedParty" />
                        <asp:Panel ID="acePnlInterestedParty" runat="server" CssClass="identifierLable" />
                    </td>
                    <td width="3%" align="left">
                        <asp:ImageButton ID="fuzzySearchIntParty" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button"
                            OnClick="fuzzySearchIntParty_Click" ToolTip="Search interested party code/name" TabIndex="101" />
                    </td>

                    <td colspan="5"></td>
                    <td align="right" width="12%">
                        <asp:Button ID="btnInterestedPartyMaintenance" runat="server" CssClass="ButtonStyle" OnClick="btnInterestedPartyMaint_Click" TabIndex="112" Text="Interested Party Maintenance" UseSubmitBehavior="false" />
                    </td>
                </tr>


                <tr>
                    <td colspan="7">
                        <br />
                    </td>

                </tr>
                <tr>
                    <td colspan="7"></td>
                    <td width="11%"></td>
                    <td width="1%"></td>
                </tr>
                <tr>
                    <td width="1%"></td>
                    <td colspan="3" class="table_header_with_border" id="txtData" runat="server">Intereseted Party details </td>

                </tr>
                <tr>
                    <td colspan="10" class="table_with_border" id="tdData" runat="server">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="1%">
                                    <br />
                                </td>
                                <td width="98%"></td>
                                <td width="1%"></td>

                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlInterestedPartyDetails" runat="server" Style="overflow-x: hidden" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvInterestedPartyDetails" runat="server" AutoGenerateColumns="False" Width="98.85%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." OnRowDataBound="gvInterestedPartyDetails_RowDataBound"
                                                        AllowSorting="true" OnSorting="gvInterestedPartyDetails_Sorting" HeaderStyle-CssClass="FixedHeader">

                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Type" SortExpression="int_party_type_desc">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblInterestedPartyType" runat="server" Text='<%# Bind("int_party_type_desc") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="4%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Name" SortExpression="int_party_name">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblInterestedPartyName" runat="server" Text='<%# Bind("int_party_name") %>' CssClass="identifierLable" Style="word-wrap: normal; word-break: break-all;"></asp:Label>

                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Address 1" SortExpression="int_party_add1">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblInterestedPartyAdd1" runat="server" Text='<%# Bind("int_party_add1") %>' CssClass="identifierLable" Style="word-wrap: normal; word-break: break-all;"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Address 2" SortExpression="int_party_add2">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblInterestedPartyAdd2" runat="server" Text='<%# Bind("int_party_add2") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Address 3" SortExpression="int_party_add3">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblInterestedPartyAdd3" runat="server" Text='<%# Bind("int_party_add3") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Address 4" SortExpression="int_party_add4">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblInterestedPartyAdd4" runat="server" Text='<%# Bind("int_party_add4") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="6%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Postcode" SortExpression="int_party_postcode">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblInterestedPartyPostcode" runat="server" Text='<%# Bind("int_party_postcode") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="6%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Email" SortExpression="email">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblInterestedPartyEmail" runat="server" Text='<%# Bind("email") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Tax Number" SortExpression="vat_number">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblTaxNumber" runat="server" Text='<%# Bind("vat_number") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="6%" />
                                                            </asp:TemplateField>
                                                            <%--JIRA-1144 CHanges -- Start--%>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Applicable Tax" SortExpression="applicable_tax_type">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblApplicableTax" runat="server" Text='<%# Bind("applicable_tax_type") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="6%" />
                                                            </asp:TemplateField>
                                                            <%--JIRA-1144 CHanges -- ENd --%>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Generate Invoice?" SortExpression="generate_invoice">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblGenerateInvoice" runat="server" Text='<%# Bind("generate_invoice") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="6%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Send Stmt?" SortExpression="send_statement">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSendStatement" runat="server" Text='<%# Bind("send_statement") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="6%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="MisMatch Address?" SortExpression="mismatch_address">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblMisMatchAddress" runat="server" Text='<%# Bind("mismatch_address") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="6%" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Updated By" SortExpression="user_code">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblUpdatedBy" runat="server" Text='<%# Bind("user_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="6%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Updated On" SortExpression="last_modified">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnChangeType" runat="server" Value='<%# Bind("change_type") %>' />
                                                                    <asp:Label ID="lblUpdatedOn" runat="server" Text='<%# Bind("last_modified") %>' CssClass="identifierLable"></asp:Label>
                                                                    <asp:HiddenField ID="hdnDisplayOrder" runat="server" Value='<%# Bind("display_order") %>' />
                                                                    <asp:HiddenField ID="hdnClrIntPartName" runat="server" Value='<%# Bind("clr_int_party_name") %>'></asp:HiddenField>
                                                                    <asp:HiddenField ID="hdnClrIntPartyAdd1" runat="server" Value='<%# Bind("clr_int_party_add1") %>'></asp:HiddenField>
                                                                    <asp:HiddenField ID="hdnClrIntPartyAdd2" runat="server" Value='<%# Bind("clr_int_party_add2") %>'></asp:HiddenField>
                                                                    <asp:HiddenField ID="hdnClrIntPartyAdd3" runat="server" Value='<%# Bind("clr_int_party_add3") %>'></asp:HiddenField>
                                                                    <asp:HiddenField ID="hdnClrIntPartyAdd4" runat="server" Value='<%# Bind("clr_int_party_add4") %>'></asp:HiddenField>
                                                                    <asp:HiddenField ID="hdnClrPostCode" runat="server" Value='<%# Bind("clr_int_party_postcode") %>'></asp:HiddenField>
                                                                    <asp:HiddenField ID="hdnClrEmail" runat="server" Value='<%# Bind("clr_email") %>'></asp:HiddenField>
                                                                    <asp:HiddenField ID="hdnClrTaxNumber" runat="server" Value='<%# Bind("clr_vat_number") %>'></asp:HiddenField>
                                                                    <asp:HiddenField ID="hdnClrApplicalbeTax" runat="server" Value='<%# Bind("clr_applicable_tax_type") %>'></asp:HiddenField>
                                                                    <asp:HiddenField ID="hdnClrMismatchAdd" runat="server" Value='<%# Bind("clr_mismatch_address") %>'></asp:HiddenField>
                                                                    <asp:HiddenField ID="hdnClrSendStmt" runat="server" Value='<%# Bind("clr_send_statement") %>'></asp:HiddenField>
                                                                    <asp:HiddenField ID="hdnClrGenerateInv" runat="server" Value='<%# Bind("clr_generate_invoice") %>'></asp:HiddenField>

                                                                </ItemTemplate>
                                                                <ItemStyle Width="6%" />
                                                            </asp:TemplateField>

                                                        </Columns>
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                    </table>
                                </td>
                                <td></td>
                            </tr>
                        </table>
                    </td>
                    <td colspan="1"></td>
                </tr>
            </table>

            <asp:Button ID="dummyFuzzySearch" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeFuzzySearch" runat="server" PopupControlID="pnlFuzzySearch" TargetControlID="dummyFuzzySearch"
                CancelControlID="btnCloseFuzzySearchPopup" BackgroundCssClass="popupBox">
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
            <asp:HiddenField ID="hdnPageNumber" runat="server" Value="" />
            <asp:HiddenField ID="hdnIsValidSearch" runat="server" Value="N" />
            <asp:HiddenField ID="hdnChangeNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnInsertDataNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridRowSelectedPrvious" runat="server" />
            <asp:HiddenField ID="hdnIntPartyType" runat="server" Value="" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:Button ID="btnHdnInterestedPartySearch" runat="server" Style="display: none;" OnClick="btnHdnInterestedPartySearch_Click" CausesValidation="false" />
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" ReadOnly="true" CssClass="gridTextField"></asp:TextBox>
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

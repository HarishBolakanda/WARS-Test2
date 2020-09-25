<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoyaltorPayeeAudit.aspx.cs" Inherits="WARS.Audit.RoyaltorPayeeAudit" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Royaltor Payee Audit" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

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
            document.getElementById("<%=PnlRoyaltorPayeeAudit.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        function RedirectToMaintScreen(royaltorId) {
            window.location = "../Contract/RoyContractPayee.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=N";
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
                    <td colspan="8">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    ROYALTOR PAYEE - AUDIT DETAILS
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="8"></td>
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
                    <td></td>
                    <td align="right" colspan="2">
                        <asp:Button ID="btnPayeeDetails" runat="server" Text="Payee / Courtesy Details" CssClass="ButtonStyle"
                            Width="98%" UseSubmitBehavior="false" OnClick="btnPayeeDetails_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="8">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td colspan="5"></td>
                    <td width="6%"></td>
                    <td width="6%"></td>
                    <td width="6%"></td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="4" class="table_with_border" runat="server" id="tdData">
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
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlRoyaltorPayeeAudit" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvRoyaltorPayeeAudit" runat="server" AutoGenerateColumns="False" Width="98%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." ShowHeader="False" OnRowDataBound="gvRoyaltorPayeeAudit_RowDataBound">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <table width="100%" cellspacing="0">
                                                                        <tr>
                                                                            <td width="15%"></td>
                                                                            <td width="30%"></td>
                                                                            <td width="15%"></td>
                                                                            <td width="15%"></td>
                                                                            <td width="10%"></td>
                                                                            <td width="15%"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Type
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:HiddenField ID="hdnIntPartyId" runat="server" Value='<%# Bind("int_party_id") %>' />
                                                                                <asp:HiddenField ID="hdnIntPartyType" runat="server" Value='<%# Bind("int_party_type") %>' />
                                                                                <asp:HiddenField ID="hdnChangeType" runat="server" Value='<%# Bind("change_type") %>' />
                                                                                <asp:Label ID="lblType" runat="server" Text='<%# Bind("int_party_type_desc") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">
                                                                                <asp:Label ID="lblPrimaryHdr" runat="server" Text="Primary" CssClass="identifierLable_large_bold"></asp:Label>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblPrimary" runat="server" Text='<%# Bind("primary_payee") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">
                                                                                <asp:Label ID="lblUpdatedByHrd" runat="server" Text="Updated by" CssClass="identifierLable_large_bold"></asp:Label>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblUpdatedBy" runat="server" Text='<%# Bind("user_code") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Interested Party
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblIntPartyName" runat="server" Text='<%# Bind("int_party_name") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">
                                                                                <asp:Label ID="lblPercentageShareHdr" runat="server" Text="Percentage Share" CssClass="identifierLable_large_bold"></asp:Label>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblPercentageShare" runat="server" Text='<%# Bind("payee_percentage","{0:0.###}") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">
                                                                                <asp:Label ID="lblUpdatedOnHdr" runat="server" Text="Updated on" CssClass="identifierLable_large_bold"></asp:Label>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblUpdatedOn" runat="server" Text='<%# Bind("last_modified") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Address 1
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblAddress1" runat="server" Text='<%# Bind("int_party_add1") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">
                                                                                <asp:Label ID="lblPayHdr" runat="server" Text="Pay" CssClass="identifierLable_large_bold"></asp:Label>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblPay" runat="server" Text='<%# Bind("payee_pay") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                            <td></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Postcode
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblPostcode" runat="server" Text='<%# Bind("int_party_postcode") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">
                                                                                <asp:Label ID="lblSupplierNumberHdr" runat="server" Text="Supplier Number" CssClass="identifierLable_large_bold"></asp:Label>

                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblSupplierNumber" runat="server" Text='<%# Bind("supplier_number") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                            <td></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">
                                                                                <asp:Label ID="lblSupplierSiteNameHdr" runat="server" Text="Supplier Site Name" CssClass="identifierLable_large_bold"></asp:Label>

                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblSupplierSiteName" runat="server" Text='<%# Bind("supplier_site_name") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>

                                                                            <td></td>
                                                                            <td></td>
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
                    <td></td>
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

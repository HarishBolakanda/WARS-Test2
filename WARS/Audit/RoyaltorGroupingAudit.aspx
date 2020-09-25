<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="RoyaltorGroupingAudit.aspx.cs" Inherits="WARS.Audit.RoyaltorGroupingAudit" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Royaltor Contract Grouping" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

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

        var txtRoySrch;

        function royaltorListPopulating() {
            txtRoySrch = document.getElementById("<%= txtRoyaltor.ClientID %>");
            txtRoySrch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoySrch.style.backgroundRepeat = 'no-repeat';
            txtRoySrch.style.backgroundPosition = 'right';
        }

        function royaltorListPopulated() {
            txtRoySrch = document.getElementById("<%= txtRoyaltor.ClientID %>");
            txtRoySrch.style.backgroundImage = 'none';
        }

        function resetScrollPosition(sender, args) {
            var autoCompPnl = document.getElementById("<%= autocompleteDropDownPanel1.ClientID %>");
            autoCompPnl.scrollTop = 1;

        }

        function royaltorListItemSelected(sender, args) {
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                document.getElementById("<%= txtRoyaltor.ClientID %>").value = "";

            }
            else {

                document.getElementById('<%=btnHdnRoyaltor.ClientID%>').click();
            }
        }



        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        function MoveCatNoFocus() {
            document.getElementById("<%= lblTab.ClientID %>").focus();
        }

        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.6;
            document.getElementById("<%=PnlParticipantDetailsAudit.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        function RedirectToMaintScreen(royaltorId) {
            window.location = "../Contract/RoyContractGrouping.aspx?RoyaltorID=" + royaltorId + "&isNewRoyaltor=N";
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
                        <asp:Panel ID="PnlScreenName" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    ROYALTOR CONTRACT GROUPING
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="10"></td>
                </tr>
                <tr>
                    <td width="2%" class="auto-style1"></td>
                    <td width="9%" class="auto-style2">Current Royaltor</td>
                    <td width="20%" colspan="3" style="padding: 2px; padding-right: 4px;" class="auto-style1">
                        <asp:TextBox ID="txtRoyaltor" runat="server" Width="99%" CssClass="textboxStyle"
                            TabIndex="100"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="aceRoyFuzzySearch" runat="server"
                            ServiceMethod="FuzzySearchAllRoyaltorList"
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
                    <td width="3%" align="left" class="auto-style1">
                        <asp:CustomValidator ID="valCatalogue" runat="server" ValidationGroup="valSearch" CssClass="requiredFieldValidator"
                            ToolTip="Not a valid Royaltor. Please select from the search list." Display="Dynamic"
                            ErrorMessage="*"></asp:CustomValidator>
                        <asp:ImageButton ID="fuzzySearchCatalogue" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                            OnClick="fuzzySearchRoyaltor_Click" TabIndex="101" ToolTip="Search Royaltor" CssClass="FuzzySearch_Button" />
                    </td>
                    <td class="auto-style1"></td>
                    <td class="auto-style1"></td>
                    <td align="right" colspan="2" class="auto-style1">
                        <asp:Button ID="btnRoyaltor" runat="server" Text="Royaltor Grouping" CssClass="ButtonStyle"
                            UseSubmitBehavior="false" TabIndex="104" OnClick="btnRoyaltorDetails_Click" onkeydown="OnTabPress();" />
                    </td>
                </tr>
                <tr>
                    <td colspan="10"></td>

                </tr>
                <tr>
                    <td></td>
                    <td colspan="8" class="table_with_border" runat="server" id="tdData">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="2%">
                                    <br />
                                </td>
                                <td width="97%"></td>
                                <td width="1%"></td>
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
                                                <asp:Panel ID="PnlParticipantDetailsAudit" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvRoyaltorGrpAudit" runat="server" AutoGenerateColumns="False" Width="98.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." ShowHeader="False">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <table width="100%" cellspacing="0">
                                                                        <tr>
                                                                            <td width="12%"></td>
                                                                            <td width="12%"></td>
                                                                            <td width="16%"></td>
                                                                            <td width="22%"></td>
                                                                            <td width="15%"></td>
                                                                            <td width="21%"></td>
                                                                            <td width="1%"></td>
                                                                            <td width="1%"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Accrual Attributes:                                                                            
                                                                            </td>
                                                                            <td align="left"></td>
                                                                            <td class="identifierLable_large_bold" align="left">Statement Groupings:
                                                                            </td>
                                                                            <td align="left"></td>

                                                                            <td class="identifierLable_large_bold" align="left">Updated By
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblUserCode" runat="server" Text='<%# Bind("user_code") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left"></td>
                                                                            <td align="left"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">GFS Grouping Royaltor
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblRoyaltor" runat="server" Text='<%# Bind("royaltor") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Summary Statements</td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblSummary" runat="server" Text='<%# Bind("summary_master_royaltor") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Updated On                                                                       </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblUpdatedOn" runat="server" Text='<%# Bind("last_modified") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left"></td>
                                                                            <td align="left"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">GFS Label                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblGfsLabel" runat="server" Text='<%# Bind("gfs_label") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">TXT Detail Statements</td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblTxtDetail" runat="server" Text='<%# Bind("txt_master_royaltor") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left"></td>
                                                                            <td align="left"></td>
                                                                            <td class="identifierLable_large_bold" align="left"></td>
                                                                            <td align="left"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">GFS Company
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblGfsCompany" runat="server" Text='<%# Bind("gfs_company") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">DSP Analytics</td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblDspAnalytics" runat="server" Text='<%# Bind("dsp_analytics_royaltor") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left"></td>
                                                                            <td align="left"></td>
                                                                            <td class="identifierLable_large_bold" align="left"></td>
                                                                            <td align="left"></td>
                                                                        </tr>
                                                                    </table>
                                                                </ItemTemplate>
                                                                <%--<ItemStyle Width="100%" />--%>
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
            <asp:Button ID="dummyFuzzySearch" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeFuzzySearch" runat="server" PopupControlID="pnlFuzzySearch" TargetControlID="dummyFuzzySearch"
                BackgroundCssClass="popupBox" CancelControlID="btnCloseFuzzySearchPopup">
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
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnOtherUserScreenLocked" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsMaintScreen" runat="server" Value="N" />
            <asp:Label ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:Label>
            <asp:Button ID="btnHdnRoyaltor" runat="server" Style="display: none;" OnClick="btnHdnRoyaltor_Click" CausesValidation="false" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="head">
    <style type="text/css">
        .auto-style1 {
            height: 40px;
        }

        .auto-style2 {
            font-family: Calibri, Verdana, Arial, Serif;
            font-size: 14px;
            font-weight: bold;
            height: 40px;
        }
    </style>
</asp:Content>




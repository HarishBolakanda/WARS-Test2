<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="RoyaltorBalance.aspx.cs" Inherits="WARS.RoyaltorBalance" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Catalogue Details Audit" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>
<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //to open Configuration group screen in same tab
        function OpenContractMaintenance() {
            var win = window.open('../Contract/RoyaltorSearch.aspx', '_self');
            win.focus();
        }

        function OpenCatalogueSearch() {
            window.location = '../Participants/CatalogueSearch.aspx?isNewRequest=N';
        }

        //================================End
        function OpenReservesScreen() {
            //debugger;        
            window.location = '../DataMaintenance/RoyaltorReserves.aspx';
        }

        function OpenEscHisScreen() {
            royaltorId = document.getElementById("<%=hdnRoyaltorId.ClientID %>").value;

            window.location = "../Contract/RoyContractEscHistory.aspx?RoyaltorId=" + royaltorId;
        }


    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr id="trReservesHistory">
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnOpenBalAndResv" runat="server" Text="Balance and Reserve History" CssClass="LinkButtonStyle"
                            Width="98%" UseSubmitBehavior="false" OnClientClick="if (!OpenReservesScreen()) { return false;};" />
                    </td>
                </tr>
                <tr id="trEscHitory">
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnEscHistory" runat="server" Text="Escalation History" CssClass="LinkButtonStyle"
                            Width="98%" UseSubmitBehavior="false" OnClientClick="if (!OpenEscHisScreen()) { return false;};" />
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
            document.getElementById("<%= hdnIsvalidRoyaltor.ClientID %>").value = "N";
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
                document.getElementById("<%= txtBroughtForwardDate.ClientID %>").value = "";
                document.getElementById("<%= txtBroughtForward.ClientID %>").value = "";
                document.getElementById("<%= txtRoyaltyEarnings.ClientID %>").value = "";
                document.getElementById("<%= txtRoyaltyReserves.ClientID %>").value = "";
                document.getElementById("<%= txtCosts.ClientID %>").value = "";
                document.getElementById("<%= txtBalanceFinalDate.ClientID %>").value = "";
                document.getElementById("<%= txtBalanceFinal.ClientID %>").value = "";
                document.getElementById("<%= hdnRoyaltorId.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnIsvalidRoyaltor.ClientID %>").value = "Y";
                document.getElementById('<%=btnHdnRoyaltor.ClientID%>').click();
            }
        }

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        function OnBalanceKeyDown() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnHdnRoyaltor.ClientID%>').click();
            }
        }
        function OnRoyaltorKeyUp() {

            var textBox = document.getElementById('<%=txtRoyaltor.ClientID%>')
            var textLength = textBox.value.length;
            if (textLength == 0) {
                document.getElementById("<%= txtBroughtForwardDate.ClientID %>").value = "";
                document.getElementById("<%= txtBroughtForward.ClientID %>").value = "";
                document.getElementById("<%= txtRoyaltyEarnings.ClientID %>").value = "";
                document.getElementById("<%= txtRoyaltyReserves.ClientID %>").value = "";
                document.getElementById("<%= txtCosts.ClientID %>").value = "";
                document.getElementById("<%= txtBalanceFinalDate.ClientID %>").value = "";
                document.getElementById("<%= txtBalanceFinal.ClientID %>").value = "";
                document.getElementById("<%= hdnRoyaltorId.ClientID %>").value = "";

            }
        }

        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.6;
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }
    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="8">
                        <asp:Panel ID="PnlScreenName" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    ROYALTOR EARNINGS ENQUIRY 
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="1%"></td>
                    <td width="10%" class="identifierLable_large_bold">Royaltor</td>
                    <td width="20%">
                        <asp:TextBox ID="txtRoyaltor" runat="server" Width="98%" CssClass="identifierLable"
                            onkeyup="javascript: OnRoyaltorKeyUp();" TabIndex="105"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server"
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
                    <td align="left">
                        <asp:CustomValidator ID="valRoyaltor" runat="server" ValidationGroup="valSearch" CssClass="requiredFieldValidator"
                            ToolTip="Not a valid Royaltor. Please select from the search list." Display="Dynamic"
                            ErrorMessage="*"></asp:CustomValidator>
                        <asp:ImageButton ID="fuzzySearchRoyaltor" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                            OnClick="fuzzySearchRoyaltor_Click" TabIndex="106" ToolTip="Search Royaltor" CssClass="FuzzySearch_Button" />
                    </td>
                </tr>
                <tr>
                    <td width="1%"></td>
                    <td width="10%" class="identifierLable_large_bold">Balance As At</td>
                    <td>
                        <asp:TextBox ID="txtBalanceDate" runat="server" Width="65" ToolTip="MM/YYYY" CssClass="textboxStyle"
                            onkeydown="javascript: OnBalanceKeyDown();" TabIndex="107"></asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmetxtBalanceDate" runat="server" TargetControlID="txtBalanceDate"
                            WatermarkText="MM/YYYY" WatermarkCssClass="waterMarkText">
                        </ajaxToolkit:TextBoxWatermarkExtender>
                        <ajaxToolkit:MaskedEditExtender ID="mtetxtBalanceDate" runat="server"
                            TargetControlID="txtBalanceDate" Mask="99/9999" AcceptNegative="None" ClearMaskOnLostFocus="false" />
                        <asp:CustomValidator ID="valBalanceDate" runat="server" ValidationGroup="valGrpSave" CssClass="requiredFieldValidator" Display="Dynamic"
                            OnServerValidate="valBalanceDate_ServerValidate" ToolTip="Please enter a valid date in MM/YYYY format"
                            ErrorMessage="*"></asp:CustomValidator>
                    </td>
                    <td align="left"></td>
                    <td></td>
                    <td></td>
                    <td align="right"></td>
                </tr>
                <tr>
                    <td colspan="8">
                        <br />
                    </td>
                </tr>
                <tr>

                    <td width="2%"></td>
                    <td valign="top" colspan="7">
                        <table width="99%" cellpadding="0" cellspacing="0">

                            <tr>
                                <td>
                                    <table width="35%" class="table_with_border">
                                        <tr>
                                            <td>
                                                <table width="100%">

                                                    <tr>

                                                        <td class="identifierLable_large_bold">Balance Brought Forward</td>
                                                        <td>
                                                            <asp:TextBox ID="txtBroughtForwardDate" runat="server" ReadOnly="true" CssClass="textboxStyle_readonly"></asp:TextBox></td>
                                                        <td>
                                                            <asp:TextBox ID="txtBroughtForward" runat="server" ReadOnly="true" CssClass="textboxStyle_readonly" Style="text-align: right"></asp:TextBox></td>
                                                        <td align="right" colspan="5"></td>
                                                    </tr>
                                                    <tr>

                                                        <td class="identifierLable_large_bold">Royalty Earnings</td>
                                                        <td></td>
                                                        <td>
                                                            <asp:TextBox ID="txtRoyaltyEarnings" runat="server" ReadOnly="true" CssClass="textboxStyle_readonly" Style="text-align: right"></asp:TextBox></td>
                                                        <td align="right" colspan="5"></td>
                                                    </tr>
                                                    <tr>

                                                        <td class="identifierLable_large_bold">Royalty Reserves</td>
                                                        <td></td>
                                                        <td>
                                                            <asp:TextBox ID="txtRoyaltyReserves" runat="server" ReadOnly="true" CssClass="textboxStyle_readonly" Style="text-align: right"></asp:TextBox></td>
                                                        <td align="right" colspan="5"></td>
                                                    </tr>
                                                    <tr>

                                                        <td class="identifierLable_large_bold">Costs</td>
                                                        <td></td>
                                                        <td>
                                                            <asp:TextBox ID="txtCosts" runat="server" ReadOnly="true" CssClass="textboxStyle_readonly" Style="text-align: right"></asp:TextBox></td>
                                                        <td align="right" colspan="5"></td>
                                                    </tr>
                                                    <tr>

                                                        <td class="identifierLable_large_bold">Balance As At</td>
                                                        <td>
                                                            <asp:TextBox ID="txtBalanceFinalDate" runat="server" ReadOnly="true" CssClass="textboxStyle_readonly"></asp:TextBox></td>
                                                        <td>
                                                            <asp:TextBox ID="txtBalanceFinal" runat="server" ReadOnly="true" CssClass="textboxStyle_readonly" Style="text-align: right"></asp:TextBox></td>
                                                        <td align="right" colspan="5"></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
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
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlFuzzySearch" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"  Style="z-index: 1; display: none">
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
            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnRoyaltorId" runat="server" />
            <asp:HiddenField ID="hdnValidDate" runat="server" />
            <asp:HiddenField ID="hdnIsvalidRoyaltor" runat="server" Value="N" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:Label ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:Label>
            <asp:Button ID="btnHdnRoyaltor" runat="server" Style="display: none;" OnClick="btnHdnRoyaltor_Click" CausesValidation="false" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>






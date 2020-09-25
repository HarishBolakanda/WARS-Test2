<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="RoyContractGrouping.aspx.cs" Inherits="WARS.Contract.RoyContractGrouping" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Royaltor Contract Groupings" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

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
        //JIRA-1146 CHanges -- Start
        //redirect to Tax details screen on saving data of new royaltor so that issue of data not saved validation would be handled
        //Harish 08-12-2017: once contract creation is complete and contract maintenance screen is opened, enable the navigation buttons
        //Harish 01-03-2018: WUIN-367: open contract Notes screen from grouping 
        function RedirectOnNewRoyaltorSave(royaltorId) {
            //debugger;
            document.getElementById("<%=hdnNewRoyaltorSaved.ClientID %>").value = "Y";
            window.location = "../Contract/RoyContractTaxDetails.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y";
        }
        //JIRA-1146 CHanges -- End
        //used to check if any changes to allow navigation to other screen 
        function IsDataChanged() {
            IsDataModified();
            var hdnDataChanged = document.getElementById("<%=hdnDataChanged.ClientID %>").value;
            if (hdnDataChanged == "Y") {
                return true;
            }
            else {
                return false;
            }
        }

        //Audit button navigation

        function RedirectToAuditScreen(royaltorId) {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Audit/RoyaltorGroupingAudit.aspx?RoyaltorId=" + royaltorId);
            }
            else {
                window.location = "../Audit/RoyaltorGroupingAudit.aspx?RoyaltorId=" + royaltorId;
                return true;
            }
        }


        function RedirectToPreviousScreen(royaltorId) {
            //debugger; 
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Contract/RoyContractEscRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y");
            }
            else {
                window.location = "../Contract/RoyContractEscRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y";
            }


        }
        //=================End

        //Auto populate search functionalities     

        function GFSRoyaltorListPopulating() {
            txtRoyaltorInsert = document.getElementById("<%= txtGFSGroupingRoyaltor.ClientID %>");
            txtRoyaltorInsert.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoyaltorInsert.style.backgroundRepeat = 'no-repeat';
            txtRoyaltorInsert.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidRoyaltorGFS.ClientID %>").value = 'N';
        }

        function GFSRoyaltorListPopulated() {
            txtRoyaltorInsert = document.getElementById("<%= txtGFSGroupingRoyaltor.ClientID %>");
            txtRoyaltorInsert.style.backgroundImage = 'none';
        }

        function GFSRoyaltorListItemSelected(sender, args) {
            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtGFSGroupingRoyaltor.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnIsValidRoyaltorGFS.ClientID %>").value = 'Y';
            }
        }

        //-----------------------------//

        function DSPRoyaltorListPopulating() {
            txtRoyaltorInsert = document.getElementById("<%= txtDSPAnalytics.ClientID %>");
            txtRoyaltorInsert.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoyaltorInsert.style.backgroundRepeat = 'no-repeat';
            txtRoyaltorInsert.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidRoyaltorDSP.ClientID %>").value = 'N';
        }

        function DSPRoyaltorListPopulated() {
            txtRoyaltorInsert = document.getElementById("<%= txtDSPAnalytics.ClientID %>");
            txtRoyaltorInsert.style.backgroundImage = 'none';
        }

        function DSPRoyaltorListItemSelected(sender, args) {
            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtDSPAnalytics.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnIsValidRoyaltorDSP.ClientID %>").value = 'Y';
            }
        }

        //-----------------------------//

        function TXTRoyaltorListPopulating() {
            txtRoyaltorInsert = document.getElementById("<%= txtTXTDetailStatements.ClientID %>");
            txtRoyaltorInsert.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoyaltorInsert.style.backgroundRepeat = 'no-repeat';
            txtRoyaltorInsert.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidRoyaltorTXT.ClientID %>").value = 'N';
        }

        function TXTRoyaltorListPopulated() {
            txtRoyaltorInsert = document.getElementById("<%= txtTXTDetailStatements.ClientID %>");
            txtRoyaltorInsert.style.backgroundImage = 'none';
        }

        function TXTRoyaltorListItemSelected(sender, args) {
            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtTXTDetailStatements.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnIsValidRoyaltorTXT.ClientID %>").value = 'Y';
            }
        }

        //-----------------------------//

        function SummaryRoyaltorListPopulating() {
            txtRoyaltorInsert = document.getElementById("<%= txtSummaryStatements.ClientID %>");
            txtRoyaltorInsert.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoyaltorInsert.style.backgroundRepeat = 'no-repeat';
            txtRoyaltorInsert.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidRoyaltorSummary.ClientID %>").value = 'N';
        }

        function SummaryRoyaltorListPopulated() {
            txtRoyaltorInsert = document.getElementById("<%= txtSummaryStatements.ClientID %>");
            txtRoyaltorInsert.style.backgroundImage = 'none';
        }

        function SummaryRoyaltorListItemSelected(sender, args) {
            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtSummaryStatements.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnIsValidRoyaltorSummary.ClientID %>").value = 'Y';
            }
        }
        //====================End

        //Check if data changed
        function IsDataModified() {
            //debugger;           
            var txtGFSGroupingRoyaltor = document.getElementById("<%=txtGFSGroupingRoyaltor.ClientID %>").value;
            var txtGFSLabel = document.getElementById("<%=txtGFSLabel.ClientID %>").value;
            var ddlGFSCompany = document.getElementById("<%=ddlGFSCompany.ClientID %>").value;
            var txtSummaryStatements = document.getElementById("<%=txtSummaryStatements.ClientID %>").value;
            var txtTXTDetailStatements = document.getElementById("<%=txtTXTDetailStatements.ClientID %>").value;
            var txtDSPAnalytics = document.getElementById("<%=txtDSPAnalytics.ClientID %>").value;
            var txtPrintGrp = document.getElementById("<%=txtPrintGrp.ClientID %>").value;

            var hdnGFSGroupingRoyaltor = document.getElementById("<%=hdnGFSGroupingRoyaltor.ClientID %>").value;
            var hdnGFSLabel = document.getElementById("<%=hdnGFSLabel.ClientID %>").value;
            var hdnGFSCompany = document.getElementById("<%=hdnGFSCompany.ClientID %>").value;
            var hdnSummaryStatements = document.getElementById("<%=hdnSummaryStatements.ClientID %>").value;
            var hdnTXTDetailStatements = document.getElementById("<%=hdnTXTDetailStatements.ClientID %>").value;
            var hdnDSPAnalytics = document.getElementById("<%=hdnDSPAnalytics.ClientID %>").value;
            var hdnPrintGrp = document.getElementById("<%=hdnPrintGrp.ClientID %>").value;

            if (txtGFSGroupingRoyaltor != hdnGFSGroupingRoyaltor || txtGFSLabel != hdnGFSLabel || ddlGFSCompany != hdnGFSCompany ||
                txtSummaryStatements != hdnSummaryStatements || txtTXTDetailStatements != hdnTXTDetailStatements || txtDSPAnalytics != hdnDSPAnalytics
                || txtPrintGrp != hdnPrintGrp) {
                document.getElementById("<%=hdnDataChanged.ClientID %>").innerText = "Y";
            }
            else {
                document.getElementById("<%=hdnDataChanged.ClientID %>").innerText = "N";
            }


        }

        function WarnOnUnSavedData() {
            //debugger;
            IsDataModified();
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            var isNewRoyaltorSaved = document.getElementById("<%=hdnNewRoyaltorSaved.ClientID %>").value;
            var isDataChanged = document.getElementById("<%=hdnDataChanged.ClientID %>").value;
            var isContractScreen = document.getElementById("hdnIsContractScreen").value;
            var isNotContractScreen = document.getElementById("hdnIsNotContractScreen").value;
            if (isExceptionRaised != "Y" && isNewRoyaltorSaved != "Y" && isContractScreen != "Y" && isNotContractScreen != "Y") {
                if (isDataChanged == "Y") {
                    unSaveBrowserClose = true;
                    return warningMsgOnUnSavedData;
                }
            }
            UpdateScreenLockFlag();// WUIN-599 - Unset the screen lock flag If an user close the browser with out unsaved data or navigate to other than contract screens


        }
        window.onbeforeunload = WarnOnUnSavedData;

        var unSaveBrowserClose = false;

        //WUIN-599 Unset the screen lock flag If an user close the browser or navigate to other than contract screens
        window.onunload = function () {
            if (unSaveBrowserClose) {
                UpdateScreenLockFlag();
            }
        }


        function UpdateScreenLockFlag() {
            var isOtherUserScreenLocked = document.getElementById("<%=hdnOtherUserScreenLocked.ClientID %>").value;
            var isAuditScreen = document.getElementById("<%=hdnIsAuditScreen.ClientID %>").value;
            var isContractScreen = document.getElementById("hdnIsContractScreen").value;
            if (isOtherUserScreenLocked == "N" && isAuditScreen == "N" && isContractScreen == "N") {
                document.getElementById("<%=hdnIsAuditScreen.ClientID %>").value = "Y";
                PageMethods.UpdateScreenLockFlag();
            }
        }

        //Check if any data changed before saving
        function ValidateSaveChanges() {
            IsDataModified();
            var hdnDataChanged = document.getElementById("<%=hdnDataChanged.ClientID %>").value;
            if (hdnDataChanged == "Y") {
                //warning on validation fail            
                if (!Page_ClientValidate("valUpdate")) {
                    Page_BlockSubmit = false;
                    DisplayMessagePopup("Royaltor grouping deatils not saved – invalid or missing data!");
                    return false;
                }
                else {
                    return true;
                }
            }
            else {
                var hdnIsNewRoyaltor = document.getElementById("<%=hdnIsNewRoyaltor.ClientID %>").value;
                if (hdnIsNewRoyaltor != "Y") {
                    DisplayMessagePopup("No changes made to save");
                    return false;
                }
                else {
                    return true;
                }
            }
        }
        //============END

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }
        //============END

        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

    </script>
    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="4">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    ROYALTOR CONTRACT GROUPING
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="4"></td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td valign="top">
                        <table width="99%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="10%" class="identifierLable_large_bold">Current Royaltor</td>
                                <td colspan="2" align="left">
                                    <asp:TextBox ID="txtRoyaltorId" runat="server" Width="25%" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" TabIndex="100"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td class="table_header_with_border" colspan="2" align="left">Contract Groupings</td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <table class="table_with_border" width="50%">
                                        <tr>
                                            <td colspan="3"></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3"></td>
                                        </tr>
                                        <tr>
                                            <td width="1%"></td>
                                            <td style="border: solid 1px black;">
                                                <table width="100%">
                                                    <tr>
                                                        <td width="5%"></td>
                                                        <td width="30%" class="identifierLable_large_bold">Accrual Attributes:</td>
                                                        <td width="56%"></td>
                                                        <td width="6%"></td>
                                                        <td width="3%"></td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                        <td class="identifierLable_large_bold">GFS Grouping Royaltor</td>
                                                        <td>
                                                            <asp:TextBox ID="txtGFSGroupingRoyaltor" runat="server" Width="98%" CssClass="textboxStyle" TabIndex="101"></asp:TextBox>
                                                            <ajaxToolkit:AutoCompleteExtender ID="GFSRoyaltorFilterExtender" runat="server"
                                                                ServiceMethod="FuzzySearchAllRoyaltorList"
                                                                ServicePath="~/Services/FuzzySearch.asmx"
                                                                MinimumPrefixLength="1"
                                                                CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                                TargetControlID="txtGFSGroupingRoyaltor"
                                                                FirstRowSelected="true"
                                                                OnClientPopulating="GFSRoyaltorListPopulating"
                                                                OnClientPopulated="GFSRoyaltorListPopulated"
                                                                OnClientHidden="GFSRoyaltorListPopulated"
                                                                OnClientItemSelected="GFSRoyaltorListItemSelected"
                                                                CompletionListElementID="pnlGFSRoyFuzzySearch" />
                                                            <asp:Panel ID="pnlGFSRoyFuzzySearch" runat="server" CssClass="identifierLable" />
                                                        </td>
                                                        <td>
                                                            <asp:ImageButton ID="fuzzyGFSGroupingRoyaltor" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button" Style="cursor: pointer"
                                                                OnClick="fuzzyGFSGroupingRoyaltor_Click" ToolTip="Search Royaltor" TabIndex="-1" />
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvGFSGroupingRoyaltor" ControlToValidate="txtGFSGroupingRoyaltor" ValidationGroup="valUpdate"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please select a royaltor from list" Display="Dynamic">
                                                            </asp:RequiredFieldValidator>
                                                        </td>
                                                        <td align="left">
                                                            <asp:HiddenField ID="hdnGFSGroupingRoyaltor" runat="server" Value="" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                        <td class="identifierLable_large_bold">GFS Label</td>
                                                        <td>
                                                            <asp:TextBox ID="txtGFSLabel" runat="server" Width="60.3%" CssClass="textboxStyle" MaxLength="3" TabIndex="102"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvGFSLabel" ControlToValidate="txtGFSLabel" ValidationGroup="valUpdate"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter GFS Label" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td></td>
                                                        <td>
                                                            <asp:HiddenField ID="hdnGFSLabel" runat="server" Value="" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                        <td class="identifierLable_large_bold">GFS Company</td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlGFSCompany" runat="server" Width="61.5%" CssClass="ddlStyle" TabIndex="103"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvddlGFSCompany" ControlToValidate="ddlGFSCompany" ValidationGroup="valUpdate"
                                                                InitialValue="-" Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter GFS Company" Display="Dynamic">
                                                            </asp:RequiredFieldValidator>
                                                        </td>
                                                        <td></td>
                                                        <td>
                                                            <asp:HiddenField ID="hdnGFSCompany" runat="server" Value="" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td width="1%"></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td style="border: solid 1px black;">
                                                <table width="100%">
                                                    <tr>
                                                        <td width="5%"></td>
                                                        <td width="30%" class="identifierLable_large_bold">Statement Groupings:</td>
                                                        <td width="56%"></td>
                                                        <td width="6%"></td>
                                                        <td width="3%"></td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                        <td class="identifierLable_large_bold">Summary Statements</td>
                                                        <td>
                                                            <asp:TextBox ID="txtSummaryStatements" runat="server" Width="98%" CssClass="textboxStyle" TabIndex="104"></asp:TextBox>
                                                            <ajaxToolkit:AutoCompleteExtender ID="SummaryRoyaltorFilterExtender" runat="server"
                                                                ServiceMethod="FuzzySearchAllRoyaltorList"
                                                                ServicePath="~/Services/FuzzySearch.asmx"
                                                                MinimumPrefixLength="1"
                                                                CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                                TargetControlID="txtSummaryStatements"
                                                                FirstRowSelected="true"
                                                                OnClientPopulating="SummaryRoyaltorListPopulating"
                                                                OnClientPopulated="SummaryRoyaltorListPopulated"
                                                                OnClientHidden="SummaryRoyaltorListPopulated"
                                                                OnClientItemSelected="SummaryRoyaltorListItemSelected"
                                                                CompletionListElementID="pnlSummaryRoyFuzzySearch" />
                                                            <asp:Panel ID="pnlSummaryRoyFuzzySearch" runat="server" CssClass="identifierLable" />
                                                        </td>
                                                        <td>
                                                            <asp:ImageButton ID="fuzzySummaryGroupingRoyaltor" runat="server" CssClass="FuzzySearch_Button" ImageUrl="../Images/search.png"
                                                                OnClick="fuzzySummaryGroupingRoyaltor_Click" Style="cursor: pointer" TabIndex="-1" ToolTip="Search Royaltor" />
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvSummaryStatements" ControlToValidate="txtSummaryStatements" ValidationGroup="valUpdate"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please select a royaltor from list" Display="Dynamic">
                                                            </asp:RequiredFieldValidator>
                                                        </td>
                                                        <td align="left">
                                                            <asp:HiddenField ID="hdnSummaryStatements" runat="server" Value="" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                        <td class="identifierLable_large_bold">TXT Detail Statements</td>
                                                        <td>
                                                            <asp:TextBox ID="txtTXTDetailStatements" runat="server" Width="98%" CssClass="textboxStyle" TabIndex="105"></asp:TextBox>
                                                            <ajaxToolkit:AutoCompleteExtender ID="TXTRoyaltorFilterExtender" runat="server"
                                                                ServiceMethod="FuzzySearchAllRoyaltorList"
                                                                ServicePath="~/Services/FuzzySearch.asmx"
                                                                MinimumPrefixLength="1"
                                                                CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                                TargetControlID="txtTXTDetailStatements"
                                                                FirstRowSelected="true"
                                                                OnClientPopulating="TXTRoyaltorListPopulating"
                                                                OnClientPopulated="TXTRoyaltorListPopulated"
                                                                OnClientHidden="TXTRoyaltorListPopulated"
                                                                OnClientItemSelected="TXTRoyaltorListItemSelected"
                                                                CompletionListElementID="pnlTXTRoyFuzzySearch" />
                                                            <asp:Panel ID="pnlTXTRoyFuzzySearch" runat="server" CssClass="identifierLable" />
                                                        </td>
                                                        <td>
                                                            <asp:ImageButton ID="fuzzyTXTGroupingRoyaltor" runat="server" CssClass="FuzzySearch_Button" ImageUrl="../Images/search.png"
                                                                OnClick="fuzzyTXTGroupingRoyaltor_Click" Style="cursor: pointer" TabIndex="-1" ToolTip="Search Royaltor" />
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvTXTDetailStatements" ControlToValidate="txtTXTDetailStatements" ValidationGroup="valUpdate"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please select a royaltor from list" Display="Dynamic">
                                                            </asp:RequiredFieldValidator>
                                                        </td>
                                                        <td align="left">
                                                            <asp:HiddenField ID="hdnTXTDetailStatements" runat="server" Value="" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                        <td class="identifierLable_large_bold">DSP Analytics</td>
                                                        <td>
                                                            <asp:TextBox ID="txtDSPAnalytics" runat="server" Width="98%" CssClass="textboxStyle" TabIndex="106"></asp:TextBox>
                                                            <ajaxToolkit:AutoCompleteExtender ID="DSPRoyaltorFilterExtender" runat="server"
                                                                ServiceMethod="FuzzySearchAllRoyaltorList"
                                                                ServicePath="~/Services/FuzzySearch.asmx"
                                                                MinimumPrefixLength="1"
                                                                CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                                TargetControlID="txtDSPAnalytics"
                                                                FirstRowSelected="true"
                                                                OnClientPopulating="DSPRoyaltorListPopulating"
                                                                OnClientPopulated="DSPRoyaltorListPopulated"
                                                                OnClientHidden="DSPRoyaltorListPopulated"
                                                                OnClientItemSelected="DSPRoyaltorListItemSelected"
                                                                CompletionListElementID="pnlDSPRoyFuzzySearch" />
                                                            <asp:Panel ID="pnlDSPRoyFuzzySearch" runat="server" CssClass="identifierLable" />
                                                        </td>
                                                        <td>
                                                            <asp:ImageButton ID="fuzzyDSPGroupingRoyaltor" runat="server" CssClass="FuzzySearch_Button" ImageUrl="../Images/search.png"
                                                                OnClick="fuzzyDSPGroupingRoyaltor_Click" Style="cursor: pointer" TabIndex="-1" ToolTip="Search Royaltor" />
                                                        </td>
                                                        <td>
                                                            <asp:HiddenField ID="hdnDSPAnalytics" runat="server" Value="" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="1%"></td>
                                            <td style="border: solid 1px black;">
                                                <table width="100%">
                                                    <tr>
                                                        <td width="5%"></td>
                                                        <td width="30%"></td>
                                                        <td width="56%"></td>
                                                        <td width="6%"></td>
                                                        <td width="3%"></td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                        <td class="identifierLable_large_bold">Print Group</td>
                                                        <td>
                                                            <asp:TextBox ID="txtPrintGrp" runat="server" Width="60.3%" CssClass="textboxStyle" MaxLength="3" TabIndex="102"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvtxtPrintGrp" ControlToValidate="txtPrintGrp" ValidationGroup="valUpdate"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter print group" Display="Dynamic"></asp:RequiredFieldValidator>
                                                            <asp:RegularExpressionValidator ID="revtxtPrintGrp" runat="server" Text="*" ControlToValidate="txtPrintGrp" ValidationGroup="valUpdate"
                                                                ValidationExpression="^([1-3]|3)$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                ToolTip="Please enter only numbers 1,2 or 3 " Display="Dynamic"> </asp:RegularExpressionValidator>
                                                        </td>
                                                        <td></td>
                                                        <td>
                                                            <asp:HiddenField ID="hdnPrintGrp" runat="server" Value="" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td width="1%"></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3"></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3"></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td width="1%"></td>
                    <td width="15%" rowspan="4" valign="top" align="right">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td width="30%"></td>
                                            <td align="right" width="70%">
                                                <asp:Button ID="btnSave" runat="server" CssClass="ButtonStyle" OnClick="btnSave_Click"
                                                    Text="Save Changes" UseSubmitBehavior="false" Width="90%" OnClientClick="if (!ValidateSaveChanges()) { return false;};" TabIndex="107" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td align="right">
                                                <asp:Button ID="btnAudit" runat="server" CssClass="ButtonStyle" OnClick="btnAudit_Click"
                                                    Text="Audit" UseSubmitBehavior="false" Width="90%" TabIndex="108" onkeydown="OnTabPress();" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    <br />
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table width="100%" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td width="30%"></td>
                                            <td width="70%">
                                                <ContNav:ContractNavigation ID="contractNavigationButtons" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <%--<tr>
                    <td></td>
                    <td class="table_header_with_border" valign="top">Contract Groupings</td>
                    <td></td>
                </tr>--%>
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

            <%--Royaltor fuzzy search - full search - Ends--%>
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
            <%--Royaltor fuzzy search - full search - Ends--%>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnDataChanged" runat="server" Value="N" />
            <asp:HiddenField ID="hdnNewRoyaltorSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsNewRoyaltor" runat="server" Value="N" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" Value="" />
            <asp:HiddenField ID="hdnIsValidRoyaltorGFS" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnIsValidRoyaltorSummary" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnIsValidRoyaltorTXT" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnIsValidRoyaltorDSP" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnOtherUserScreenLocked" runat="server" Value="N" />
            <asp:Label ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:Label>
            <asp:HiddenField ID="hdnIsAuditScreen" runat="server" Value="N" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

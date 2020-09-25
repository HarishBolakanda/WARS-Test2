<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoyContractPayeeSupp.aspx.cs" Inherits="WARS.Contract.RoyContractPayeeSupp" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Royaltor Contract Payee Supplier" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>
<%@ Register TagPrefix="ContNav" TagName="ContractNavigation" Src="~/Contract/ContractNavigationButtons.ascx" %>
<%@ Register TagPrefix="ContHdrNav" TagName="ContractHdrNavigation" Src="~/Contract/ContractHdrLinkButtons.ascx" %>
<%@ Register TagPrefix="sao" TagName="SupplierAddressOverwrite" Src="~/UserControls/SupplierAddressOverwrite.ascx" %>

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

            //to keep the address overwrite open
            postBackElementID = args.get_postBackElement().id;
            if (postBackElementID.lastIndexOf('btnAddressOverwrite') > 0 || postBackElementID.lastIndexOf('btnContinue') > 0 || postBackElementID.lastIndexOf('btnCancel') > 0) {
                var popupAddressOverwrite = $find('<%= mpeAddressOverwrite.ClientID %>');
                if (popupAddressOverwrite != null) {
                    popupAddressOverwrite.show();
                }
            }
        }

        function EndRequestHandler(sender, args) {
            //Hide the modal popup - the update progress
            var popup = $find('<%= mPopupPageLevel.ClientID %>');
            if (popup != null) {
                popup.hide();
            }

            //to keep the address overwrite open
            postBackElementID = sender._postBackSettings.sourceElement.id;
            if (postBackElementID.lastIndexOf('btnAddressOverwrite') > 0 || postBackElementID.lastIndexOf('btnContinue') > 0 || postBackElementID.lastIndexOf('btnCancel') > 0) {
                var popupAddressOverwrite = $find('<%= mpeAddressOverwrite.ClientID %>');
                if (popupAddressOverwrite != null) {
                    popupAddressOverwrite.show();
                }
            }

        }
        //======================= End       

        //used to check if any changes to allow navigation to other screen 
        function IsDataChanged() {
            IsSuppDataChanged();
        }

        //Supplier auto populate search functionalities        
        var txtOwnSrch;

        function suppListPopulating() {
            txtSupplierSearch = document.getElementById("<%= txtSupplierSearch.ClientID %>");
            txtSupplierSearch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtSupplierSearch.style.backgroundRepeat = 'no-repeat';
            txtSupplierSearch.style.backgroundPosition = 'right';
        }

        function suppListPopulated() {
            txtSupplierSearch = document.getElementById("<%= txtSupplierSearch.ClientID %>");
            txtSupplierSearch.style.backgroundImage = 'none';
        }

        function suppScrollPosition(sender, args) {
            var autoCompPnl = document.getElementById("<%= acePnlsuppFuzzySearch.ClientID %>");
            autoCompPnl.scrollTop = 1;

        }

        function suppListItemSelected(sender, args) {
            var ownSrchVal = args.get_value();
            if (ownSrchVal == 'No results found') {
                document.getElementById("<%= txtSupplierSearch.ClientID %>").value = "";
            }
            else {
                document.getElementById('<%=btnSupplierSearch.ClientID%>').click();
            }


        }
        //================================End


        //Validate any unsaved data on browser window close/refresh -- start

        function WarnOnUnSavedData() {
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            var isNewRoyaltorSaved = document.getElementById("<%=hdnNewRoyaltorSaved.ClientID %>").value;
            var isContractScreen = document.getElementById("hdnIsContractScreen").value;
            var isNotContractScreen = document.getElementById("hdnIsNotContractScreen").value;
            if (isExceptionRaised != "Y" && isNewRoyaltorSaved != "Y" && isContractScreen != "Y" && isNotContractScreen != "Y") {
                if (IsSuppDataChanged()) {
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


        function IsSuppDataChanged() {

            hdnPayeeCurrency = document.getElementById("<%=hdnPayeeCurrency.ClientID %>").value;
                ddlCurrency = document.getElementById("<%=ddlCurrency.ClientID %>").value;

                hdnSupplierNumber = document.getElementById("<%=hdnSupplierNumber.ClientID %>").value;
                txtSupplier = document.getElementById("<%=txtSupplier.ClientID %>").value;
                hdnSupplierSiteName = document.getElementById("<%=hdnSupplierSiteName.ClientID %>").value;
                txtSuppSiteName = document.getElementById("<%=txtSuppSiteName.ClientID %>").value;

                if ((hdnPayeeCurrency != ddlCurrency) || (hdnSupplierNumber != txtSupplier) || (hdnSupplierSiteName != txtSuppSiteName)) {
                    return true;
                }
                else {
                    return false;
                }

            }

            //used to check if any changes to allow navigation to other screen 
            function IsDataChanged() {
                if (IsSuppDataChanged()) {
                    return true;
                }
                else {
                    return false;
                }
            }

            function RedirectToErrorPage() {
                document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        //redirect to contract option periods screen on saving data of new royaltor so that issue of data not saved validation would be handled
        function RedirectOnNewRoyaltorSave(royaltorId) {

            document.getElementById("<%=hdnNewRoyaltorSaved.ClientID %>").value = "Y";
            window.location = "../Contract/RoyContractOptionPeriods.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y";
        }

        //========Validate any unsaved data on browser window close/refresh -- End

        //Audit button navigation        
        function RedirectToAuditScreen(royaltorId, intPartyId) {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Audit/RoyaltorPayeeSuppAudit.aspx?RoyaltorId=" + royaltorId + "&IntPartyId=" + intPartyId + "");
            }
            else {
                window.location = "../Audit/RoyaltorPayeeSuppAudit.aspx?RoyaltorId=" + royaltorId + "&IntPartyId=" + intPartyId + "";
            }
        }

        function RedirectToPreviousScreen(royaltorId) {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Contract/RoyContractPayee.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y");
            }
            else {
                window.location = "../Contract/RoyContractPayee.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y";
            }
        }
        //=================End

        //Data validations - start

        function ValidateSuppNumber(sender, args) {
            hdnSupplierNumber = document.getElementById("<%=hdnSupplierNumber.ClientID %>").value;
            txtSupplier = document.getElementById("<%=txtSupplier.ClientID %>").value;
            if (hdnSupplierNumber != txtSupplier) {
                args.IsValid = false;
            }
        }

        function ValidateSave() {
            hdnPayeeCurrency = document.getElementById("<%=hdnPayeeCurrency.ClientID %>").value;
            ddlCurrency = document.getElementById("<%=ddlCurrency.ClientID %>").value;
            if (ddlCurrency == "-") {
                ddlCurrency = "";
            }

            hdnSupplierNumber = document.getElementById("<%=hdnSupplierNumber.ClientID %>").value;
            txtSupplier = document.getElementById("<%=txtSupplier.ClientID %>").value;

            hdnSupplierSiteName = document.getElementById("<%=hdnSupplierSiteName.ClientID %>").value;
            txtSuppSiteName = document.getElementById("<%=txtSuppSiteName.ClientID %>").value;

            //warning on add row validation fail - for an existing royaltor                       
            if (hdnPayeeCurrency == ddlCurrency && hdnSupplierNumber == txtSupplier && hdnSupplierSiteName == txtSuppSiteName) {
                Page_BlockSubmit = false;
                DisplayMessagePopup("No changes made to save!");
                return false;
            }
            else {
                return true;
            }
        }

        //===========Data validations - End

        function AuditClick() {
            var intPartyId = document.getElementById("ContentPlaceHolderBody_ddlPayee").value;
            var royaltorId = getParameterByName("RoyaltorId");
            var isNewRoyaltor = getParameterByName("isNewRoyaltor");
            document.getElementById("<%=hdnIsAuditScreen.ClientID %>").value = "Y";
            if (isNewRoyaltor == "Y") {
                RedirectToPreviousScreen(royaltorId);
            }
            else {
                RedirectToAuditScreen(royaltorId, intPartyId)
            }

        }

        function getParameterByName(name) {
            var url = window.location.href;
            name = name.replace(/[\[\]]/g, "\\$&");
            var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
                results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, " "));
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
                                    ROYALTOR CONTRACT - SUPPLIER DETAILS
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
                    <td colspan="2" valign="top">
                        <table width="100%">
                            <tr>
                                <td width="10%" class="identifierLable_large_bold">Current Royaltor</td>
                                <td width="26%">
                                    <asp:TextBox ID="txtRoyaltor" runat="server" Width="99%" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true"></asp:TextBox>
                                </td>
                                <td></td>
                                <td width="1%"></td>
                            </tr>
                            <tr>
                                <td class="identifierLable_large_bold">Payee</td>
                                <td>
                                    <asp:DropDownList ID="ddlPayee" runat="server" CssClass="ddlStyle" Width="99.9%" OnSelectedIndexChanged="ddlPayee_SelectedIndexChanged"
                                        AutoPostBack="true">
                                    </asp:DropDownList>
                                </td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td class="identifierLable_large_bold">Currency</td>
                                <td>
                                    <asp:DropDownList ID="ddlCurrency" runat="server" CssClass="ddlStyle" Width="99.9%">
                                    </asp:DropDownList>
                                </td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td class="identifierLable_large_bold">Supplier</td>
                                <td>
                                    <asp:TextBox ID="txtSupplierSearch" runat="server" Width="99%" CssClass="textboxStyle"></asp:TextBox>
                                    <ajaxToolkit:AutoCompleteExtender ID="aceSuppFuzzySearch" runat="server"
                                        ServiceMethod="FuzzySearchContPayeeSuppSupplierList"
                                        ServicePath="~/Services/FuzzySearch.asmx"
                                        MinimumPrefixLength="1"
                                        CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                        TargetControlID="txtSupplierSearch"
                                        FirstRowSelected="true"
                                        OnClientPopulating="suppListPopulating"
                                        OnClientPopulated="suppListPopulated"
                                        OnClientHidden="suppListPopulated"
                                        OnClientShown="suppScrollPosition"
                                        OnClientItemSelected="suppListItemSelected"
                                        CompletionListElementID="acePnlsuppFuzzySearch" />
                                    <asp:Panel ID="acePnlsuppFuzzySearch" runat="server" CssClass="identifierLable" />

                                </td>
                                <td>

                                    <asp:ImageButton ID="btnSupplierFuzzySearch" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button"
                                        OnClick="btnSupplierFuzzySearch_Click" ToolTip="Search by supplier number/name" />
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td class="identifierLable_large_bold">Supplier Site Name</td>
                                <td>
                                    <asp:DropDownList ID="ddlSupplierSite" runat="server" CssClass="ddlStyle" Width="99.9%"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlSupplierSite_OnSelectedIndexChanged">
                                    </asp:DropDownList>
                                </td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="4">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" class="table_header_with_border" valign="top">Supplier Details</td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <table width="100%" class="table_with_border">
                                        <tr>
                                            <td width="10%"></td>
                                            <td width="12%"></td>
                                            <td width="30%"></td>
                                            <td width="2%"></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Supplier</td>
                                            <td>
                                                <asp:TextBox ID="txtSupplier" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true"></asp:TextBox>

                                            </td>
                                            <td>
                                                <%--<asp:CustomValidator ID="valSuppNumber" runat="server" ValidationGroup="valGrpSave" CssClass="requiredFieldValidator"
                                                    ClientValidationFunction="ValidateSuppNumber" ToolTip="Not a valid supplier number. Please select from the search list."
                                                    ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>--%>
                                            </td>
                                            <td style="padding-left: 50px">
                                                <asp:Button ID="btnClearSupplier" runat="server" CssClass="ButtonStyle" OnClick="btnClearSupplier_Click"
                                                    Text="Clear Supplier Details" UseSubmitBehavior="false" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Supplier Name</td>
                                            <td>
                                                <asp:TextBox ID="txtSuppName" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true"></asp:TextBox>
                                            </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Supplier Address</td>
                                            <td>
                                                <asp:TextBox ID="txtSuppAdd1" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true"></asp:TextBox>
                                            </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td></td>
                                            <td>
                                                <asp:TextBox ID="txtSuppAdd2" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true"></asp:TextBox>
                                            </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td></td>
                                            <td>
                                                <asp:TextBox ID="txtSuppAdd3" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true"></asp:TextBox>
                                            </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td></td>
                                            <td>
                                                <asp:TextBox ID="txtSuppAdd4" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true"></asp:TextBox>
                                            </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Postcode</td>
                                            <td>
                                                <asp:TextBox ID="txtPostCode" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true"></asp:TextBox>
                                            </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Account Company</td>
                                            <td>
                                                <asp:TextBox ID="txtAccCompany" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true"></asp:TextBox>
                                            </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Supplier Site Name</td>
                                            <td>
                                                <asp:TextBox ID="txtSuppSiteName" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true"></asp:TextBox>
                                            </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Currency</td>
                                            <td>
                                                <asp:TextBox ID="txtCurrency" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true"></asp:TextBox>
                                            </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Payment Terms</td>
                                            <td>
                                                <asp:TextBox ID="txtPaymentTerms" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true"></asp:TextBox>
                                            </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Active</td>
                                            <td>
                                                <div style="position: relative; left: -3px;">
                                                    <asp:CheckBox ID="cbActive" runat="server" Enabled="false" />
                                                </div>
                                            </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Maintenance Expected</td>
                                            <td>
                                                <div style="position: relative; left: -3px;">
                                                    <asp:CheckBox ID="cbMaintExpected" runat="server" Enabled="false" />
                                                </div>
                                            </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                    </table>
                                    <%--<asp:HiddenField ID="hdnSupplierNumberOld" runat="server" />--%>
                                    <asp:HiddenField ID="hdnSupplierNumber" runat="server" />
                                    <asp:HiddenField ID="hdnSupplierSiteName" runat="server" />
                                </td>
                                <td></td>
                            </tr>
                        </table>
                    </td>
                    <td width="15%" valign="top" align="right">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td width="30%"></td>
                                            <td align="right" width="70%">
                                                <asp:Button ID="btnSave" runat="server" CssClass="ButtonStyle" OnClick="btnSave_Click"
                                                    Text="Save Changes" UseSubmitBehavior="false" Width="90%" TabIndex="119"
                                                    OnClientClick="if (!ValidateSave()) { return false;};" />
                                                <%--ValidationGroup="valGrpSave" OnClientClick="if (!ValidateSave()) { return false;};"--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td align="right">
                                                <asp:Button ID="btnClearSearch" runat="server" CssClass="ButtonStyle" OnClick="btnClearSearch_Click"
                                                    Text="Reset" UseSubmitBehavior="false" Width="90%" TabIndex="120" />
                                               
                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td align="right">
                                                <asp:Button ID="btnAudit" runat="server" CssClass="ButtonStyle" OnClientClick="AuditClick();"
                                                    Text="Audit" UseSubmitBehavior="false" Width="90%" TabIndex="121" onkeydown="OnTabPress();" />
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
                                <td align="right">
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

            <%--Fuzzy search popup--%>
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
            <%--Fuzzy search popup-- Ends%>

            <%--Supplier address overwrite popup--%>
            <asp:Button ID="dummyAddressOverwrite" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeAddressOverwrite" runat="server" PopupControlID="pnlSupplierAddressOverwrite" TargetControlID="dummyAddressOverwrite"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlSupplierAddressOverwrite" runat="server" align="left" Width="80%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="identifierLable" align="center">
                                        <asp:Panel ID="Panel1" runat="server" CssClass="ScreenName">
                                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                                <div style="float: left;">
                                                    PAYEE DETAILS - ADDRESS OVERWRITE
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </td>
                                    <td align="right" style="vertical-align: top;">
                                        <asp:ImageButton ID="btnClosePopupAddressOverwrite" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" OnClick="btnClosePopupAddressOverwrite_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:Panel ID="Panel3" runat="server" ScrollBars="Auto" Width="100%">
                                            <sao:SupplierAddressOverwrite ID="sao1" runat="server" />
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Supplier address overwrite popup-- Ends%>--%>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnNewRoyaltorSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnMismatchFlag" runat="server" Value="N" />
            <asp:HiddenField ID="hdnSupplierDelete" runat="server" Value="N" />
            <asp:HiddenField ID="hdnPayeeName" runat="server" Value="N" />
            <asp:HiddenField ID="hdnPayeeAddress1" runat="server" Value="N" />
            <asp:HiddenField ID="hdnPayeePostcode" runat="server" Value="N" />
            <%--            <asp:HiddenField ID="hdnIsSupplierLinked" runat="server" Value="N" />--%>
            <asp:HiddenField ID="hdnSupplierSiteNameRegValue" runat="server" />
            <asp:HiddenField ID="hdnPayeeCurrency" runat="server" />
            <asp:Button ID="btnSupplierSearch" runat="server" Style="display: none;" OnClick="btnSupplierSearch_Click" CausesValidation="false" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" onkeydown="FocusLblKeyPress();"></asp:Label>
            <asp:HiddenField ID="hdnIsAuditScreen" runat="server" Value="N" />
            <asp:HiddenField ID="hdnOtherUserScreenLocked" runat="server" Value="N" />

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

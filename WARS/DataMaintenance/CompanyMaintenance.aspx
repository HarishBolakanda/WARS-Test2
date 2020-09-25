<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyMaintenance.aspx.cs" Inherits="WARS.CompanyMaintenance" MasterPageFile="~/MasterPage.Master"
    Title="WARS - CompanyMaintenance " MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<%--<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">    
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnCompanyAudit" runat="server" Text="Company Audit" CssClass="LinkButtonStyle" 
                            width="98%"  UseSubmitBehavior="false" OnClick="btnCompanyAudit_Click"/>
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
        var scrollTopNotSel;
        var scrollTopSel;
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


        //Fuzzy search filters
        var txtCompanySearch;
        function CompanySelected(sender, args) {
            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtCompanySearch.ClientID %>").value = "";
            }
            else {
                if (IsDataChanged()) {
                    OpenOnUnSavedData();
                    document.getElementById("<%=hdnButtonSelection.ClientID %>").value = "btnHdnCompanySearch";
                }
                else {
                    document.getElementById("<%= hdnIsValidSearch.ClientID %>").value = "Y";
                    document.getElementById('<%=btnHdnCompanySearch.ClientID%>').click();
                }
            }
        }

        function CompanyListPopulating() {
            txtCompanySearch = document.getElementById("<%= txtCompanySearch.ClientID %>");
            txtCompanySearch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtCompanySearch.style.backgroundRepeat = 'no-repeat';
            txtCompanySearch.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidSearch.ClientID %>").value = "N";
        }

        function CompanyListPopulated() {
            txtCompanySearch = document.getElementById("<%= txtCompanySearch.ClientID %>");
            txtCompanySearch.style.backgroundImage = 'none';
        }

        //=============== End

        //set flag value when data is changed
        function OnDataChange() {
            document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        }

        function OnkeyDown() {
            if (event.keyCode == 8 || event.keyCode == 46) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
            }
        }

        function WarnOnUnSavedData() {
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            var isDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
            if (isExceptionRaised != "Y" && isDataChanged == "Y") {
                return warningMsgOnUnSavedData;
            }
        }
        window.onbeforeunload = WarnOnUnSavedData;

        //used to check if any changes to allow navigation to other screen 
        function IsDataChanged() {
            var isDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
            if ((isDataChanged == "Y")) {
                return true;
            }
            else {
                return false;
            }
        }

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        //Validate any unsaved data on browser window close/refresh
        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        //=============== End

        //grid panel height adjustment functioanlity - starts
        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
        }
        //=============== End

        //open Audit screen
        function OpenAuditScreen() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Audit/CompanyAudit.aspx");
            }
            else {
                return true;
            }
        }
        //==============End

        // WUIN-846 - confirmation on un saved data
        function OnNewCompanyClick(button) {
            if (IsDataChanged()) {
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
                return false;
            }
            else {
                return true;
            }
        }

        function OnFuzzySearchCompanyClick(button) {
            if (IsDataChanged()) {
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
                return false;
            }
            else {
                return true;
            }
        }

        function OpenOnUnSavedData() {
            var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
            if (warnPopup != null) {
                document.getElementById("<%=lblUnSavedWarnMsg.ClientID %>").innerText = warningMsgOnUnSavedData;
                warnPopup.show();
            }
        }

        function OnUnSavedDataReturn() {
            var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
            if (warnPopup != null) {
                warnPopup.hide();
            }

            window.onbeforeunload = WarnOnUnSavedData;

            //reset field values
            document.getElementById("<%= txtCompanySearch.ClientID %>").value = document.getElementById("<%= hdnCompanySearch.ClientID %>").value;

            return false;
        }

        function OnUnSavedDataExit() {
            document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "Y";
            window.onbeforeunload = WarnOnUnSavedData;
            return true;
        }
        //==============End

    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="6">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    COMPANY MAINTENANCE
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="8%"></td>
                    <td width="8%" class="identifierLable_large_bold">Company Code</td>
                    <td width="24%">
                        <asp:TextBox ID="txtCompanySearch" runat="server" Width="99%" CssClass="textboxStyle"
                            TabIndex="100"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="aceAccCompany" runat="server"
                            ServiceMethod="FuzzySearchAllCompanyList"     
                            ServicePath="~/Services/FuzzySearch.asmx"                       
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtCompanySearch"
                            FirstRowSelected="true"
                            OnClientItemSelected="CompanySelected"
                            OnClientPopulating="CompanyListPopulating"
                            OnClientPopulated="CompanyListPopulated"
                            OnClientHidden="CompanyListPopulated"
                            CompletionListElementID="acePnlCompany" />
                        <asp:Panel ID="acePnlCompany" runat="server" CssClass="identifierLable" />
                    </td>
                    <td width="3%" align="left">
                        <asp:ImageButton ID="fuzzySearchCompany" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button"
                            OnClientClick="if (!OnFuzzySearchCompanyClick('fuzzySearchCompany')) { return false;};" OnClick="fuzzySearchCompany_Click" ToolTip="Search company code/name" />
                    </td>
                    <td></td>
                    <td align="right" width="32%">
                        <asp:Button ID="btnNewCompany" runat="server" CssClass="ButtonStyle"
                            OnClientClick="if (!OnNewCompanyClick('btnNewCompany')) { return false;};" OnClick="btnNewCompany_Click" Width="30%" TabIndex="115" Text="New Company" UseSubmitBehavior="false" />
                    </td>
                </tr>
                <tr>
                    <td colspan="5"></td>
                    <td align="right">
                        <asp:Button ID="btnSaveChanges" runat="server" CssClass="ButtonStyle" OnClick="btnSaveChanges_Click" Width="30%" TabIndex="116" Text="Save Changes" UseSubmitBehavior="false" ValidationGroup="valEditCompanyData" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="4" class="table_with_border">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="3%">
                                    <br />
                                </td>
                                <td width="15%"></td>
                                <td width="18%"></td>
                                <td width="12%"></td>
                                <td width="22%"></td>
                                <td width="10%"></td>
                                <td width="15%"></td>
                                <td width="8%"></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td class="identifierLable_large_bold">Name</td>
                                <td colspan="4">
                                    <asp:TextBox ID="txtCompanyName" runat="server" Width="96%" CssClass="textboxStyle" TabIndex="101"
                                        onchange="javascript: OnDataChange();" onKeyPress="javascript: OnDataChange();" onkeydown="javascript: OnkeyDown();" MaxLength="30"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ID="rfvCompanyName" ControlToValidate="txtCompanyName" ValidationGroup="valEditCompanyData"
                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter company name" Display="Dynamic"></asp:RequiredFieldValidator>
                                </td>
                                <td class="identifierLable_large_bold">Primary Company</td>
                                <td align="left">
                                    <asp:CheckBox ID="cbPrimaryCompany" runat="server" onclick="javascript: OnDataChange();" TabIndex="102" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="8">
                                    <br />
                                </td>
                            </tr>
                              <tr>
                                <td></td>
                                <td class="identifierLable_large_bold">Description</td>
                                <td colspan="4">
                                    <asp:TextBox ID="txtDescription" runat="server" Width="96%" CssClass="textboxStyle" TabIndex="103"
                                        onchange="javascript: OnDataChange();" onKeyPress="javascript: OnDataChange();" onkeydown="javascript: OnkeyDown();" MaxLength="30"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtDescription" ValidationGroup="valEditCompanyData"
                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter description" Display="Dynamic"></asp:RequiredFieldValidator>
                                </td>
                                <td class="identifierLable_large_bold">Display VAT number</td>
                                <td align="left">
                                    <asp:CheckBox ID="cbDisplayVat" runat="server" onclick="javascript: OnDataChange();" TabIndex="104" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="8">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td class="identifierLable_large_bold">Address</td>
                                <td colspan="4">
                                    <asp:TextBox ID="txtAddress1" runat="server" Width="96%" CssClass="textboxStyle"
                                        TabIndex="105" onchange="javascript: OnDataChange();" onKeyPress="javascript: OnDataChange();" onkeydown="javascript: OnkeyDown();" MaxLength="50"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ID="rfvAddress1" ControlToValidate="txtAddress1" ValidationGroup="valEditCompanyData"
                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter address1" Display="Dynamic"></asp:RequiredFieldValidator>
                                </td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td></td>
                                <td colspan="4">
                                    <asp:TextBox ID="txtAddress2" runat="server" Width="96%" CssClass="textboxStyle"
                                        TabIndex="106" onchange="javascript: OnDataChange();" onKeyPress="javascript: OnDataChange();" onkeydown="javascript: OnkeyDown();" MaxLength="50"></asp:TextBox>
                                </td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td></td>
                                <td colspan="4">
                                    <asp:TextBox ID="txtAddress3" runat="server" Width="96%" CssClass="textboxStyle"
                                        TabIndex="107" onchange="javascript: OnDataChange();" onKeyPress="javascript: OnDataChange();" onkeydown="javascript: OnkeyDown();" MaxLength="50"></asp:TextBox>
                                </td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td></td>
                                <td colspan="4">
                                    <asp:TextBox ID="txtAddress4" runat="server" Width="96%" CssClass="textboxStyle"
                                        TabIndex="108" onchange="javascript: OnDataChange();" onKeyPress="javascript: OnDataChange();" onkeydown="javascript: OnkeyDown();" MaxLength="50"></asp:TextBox>
                                </td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="8">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td class="identifierLable_large_bold">Currency</td>
                                <td colspan="2">
                                    <asp:DropDownList ID="ddlCurrency" runat="server" Width="93%" CssClass="ddlStyle" onchange="javascript: OnDataChange();" TabIndex="109"></asp:DropDownList>
                                    <asp:RequiredFieldValidator runat="server" ID="rfvCurrency" ControlToValidate="ddlCurrency" ValidationGroup="valEditCompanyData"
                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please select currency" InitialValue="-" Display="Dynamic"></asp:RequiredFieldValidator>
                                </td>
                                <td class="identifierLable_large_bold" align="left">Domestic Currency Grouping</td>
                                <td>
                                    <asp:TextBox ID="txtDomesticCurrency" runat="server" Width="78%" CssClass="textboxStyle"
                                        TabIndex="110" onchange="javascript: OnDataChange();" onKeyPress="javascript: OnDataChange();" onkeydown="javascript: OnkeyDown();" MaxLength="3"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ID="rfvDomesticCurrency" ControlToValidate="txtDomesticCurrency" ValidationGroup="valEditCompanyData"
                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter domestic currency grouping" Display="Dynamic"></asp:RequiredFieldValidator>
                                </td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="8">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td class="identifierLable_large_bold">Threshold values :</td>
                                <td class="identifierLable_large_bold" align="left">Payment Threshold</td>
                                <td align="left">
                                    <asp:TextBox ID="txtPaymentThreshold" runat="server" Width="78%" CssClass="textboxStyle"
                                        TabIndex="111" onchange="javascript: OnDataChange();" onKeyPress="javascript: OnDataChange();" onkeydown="javascript: OnkeyDown();"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ID="rfvPaymentThreshold" ControlToValidate="txtPaymentThreshold" ValidationGroup="valEditCompanyData"
                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter payment threshold" Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revPaymentThreshold" runat="server" Text="*" ControlToValidate="txtPaymentThreshold"
                                        ValidationExpression="^[+]?\d+$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valEditCompanyData"
                                        ToolTip="Please enter only integers" Display="Dynamic"> </asp:RegularExpressionValidator>
                                </td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td></td>
                                <td class="identifierLable_large_bold" align="left">Recouped Statements</td>
                                <td align="left">
                                    <asp:TextBox ID="txtRecoupedStmt" runat="server" Width="78%" CssClass="textboxStyle"
                                        TabIndex="112" onchange="javascript: OnDataChange();" onKeyPress="javascript: OnDataChange();" onkeydown="javascript: OnkeyDown();"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ID="rfvRecoupedStmt" ControlToValidate="txtRecoupedStmt" ValidationGroup="valEditCompanyData"
                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter recouped statements" Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revRecoupedStmt" runat="server" Text="*" ControlToValidate="txtRecoupedStmt"
                                        ValidationExpression="^[+]?\d+$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valEditCompanyData"
                                        ToolTip="Please enter only integers" Display="Dynamic"> </asp:RegularExpressionValidator>
                                </td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td></td>
                                <td class="identifierLable_large_bold" align="left">Unrecouped Statements</td>
                                <td>
                                    <asp:TextBox ID="txtUnrecoupedStmt" runat="server" Width="78%" CssClass="textboxStyle"
                                        TabIndex="113" onchange="javascript: OnDataChange();" onKeyPress="javascript: OnDataChange();" onkeydown="javascript: OnkeyDown();"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ID="rfvUnrecoupedStmt" ControlToValidate="txtUnrecoupedStmt" ValidationGroup="valEditCompanyData"
                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter unrecouped statements" Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revUnrecoupedStmt" runat="server" Text="*" ControlToValidate="txtUnrecoupedStmt"
                                        ValidationExpression="^[+]?\d+$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valEditCompanyData"
                                        ToolTip="Please enter only integers" Display="Dynamic"> </asp:RegularExpressionValidator>
                                </td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="8">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td class="identifierLable_large_bold">Account Company</td>
                                <td colspan="2">
                                    <asp:DropDownList ID="ddlAccountCompany" runat="server" Width="93%" CssClass="ddlStyle" onchange="javascript: OnDataChange();" TabIndex="114"></asp:DropDownList>
                                </td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="8">
                                    <br />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td valign="top" align="right">
                        <asp:Button ID="btnCancel" runat="server" CssClass="ButtonStyle" OnClick="btnCancel_Click" Width="30%" Text="Cancel" UseSubmitBehavior="false" Visible="false" />
                        <asp:Button ID="btnCompanyAudit" runat="server" Text="Audit" CssClass="ButtonStyle" TabIndex="117" onkeydown="OnTabPress();"
                            Width="30%" UseSubmitBehavior="false" OnClick="btnCompanyAudit_Click" OnClientClick="if (!OpenAuditScreen()) { return false;};" />
                    </td>
                </tr>
            </table>

            <asp:Button ID="dummyFuzzySearch" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeFuzzySearch" runat="server" PopupControlID="pnlFuzzySearch" TargetControlID="dummyFuzzySearch"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlFuzzySearch" runat="server" align="center" Width="25%" CssClass="popupPanel"  Style="z-index: 1; display: none">
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

            <%--Warning on unsaved data popup--%>
            <asp:Button ID="dummyUnsavedWarnMsg" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeUnSavedWarning" runat="server" PopupControlID="pnlUnsavedWarnMsgPopup" TargetControlID="dummyUnsavedWarnMsg"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlUnsavedWarnMsgPopup" runat="server" align="center" Width="25%" CssClass="popupPanel"  Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmOnUnsavedData" runat="server" Text="Unsaved Data Warning" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblUnSavedWarnMsg" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td width="48%" align="right">
                                        <asp:Button ID="btnUnSavedDataReturn" runat="server" Text="Return" CssClass="ButtonStyle" Width="30%" OnClientClick="return OnUnSavedDataReturn();" />
                                    </td>
                                    <td width="4%"></td>
                                    <td width="48%" align="left">
                                        <asp:Button ID="btnUnSavedDataExit" runat="server" Text="Exit" CssClass="ButtonStyle" Width="30%" OnClientClick="if (!OnUnSavedDataExit()) { return false;};"
                                            OnClick="btnUnSavedDataExit_Click" />
                                    </td>
                                </tr>
                            </table>
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
            <asp:HiddenField ID="hdnIsValidSearch" runat="server" Value="N" />
            <asp:HiddenField ID="hdnChangeNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsConfirmPopup" runat="server" Value="N" />
            <asp:HiddenField ID="hdnButtonSelection" runat="server" />
            <asp:HiddenField ID="hdnCompanySearch" runat="server" />
            <asp:Button ID="btnHdnCompanySearch" runat="server" Style="display: none;" OnClick="btnHdnCompanySearch_Click" CausesValidation="false" />
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:TextBox>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

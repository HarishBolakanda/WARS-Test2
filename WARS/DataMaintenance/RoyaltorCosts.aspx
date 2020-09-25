<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoyaltorCosts.aspx.cs" Inherits="WARS.RoyaltorCosts" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Royaltor Costs" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //to open workflow screen
        function OpenWorkflowScreen() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../StatementProcessing/WorkFlow.aspx');
            }
            else {
                var win = window.open('../StatementProcessing/WorkFlow.aspx', '_self');
                win.focus();
            }


        }
    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 1px;">
                        <asp:Button ID="btnOpenWorkflow" runat="server" CssClass="LinkButtonStyle"
                            OnClientClick="if (!OpenWorkflowScreen()) { return false;};" Text="Workflow"
                            UseSubmitBehavior="false" Width="98%" CausesValidation="false" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderBody">
    <script type="text/javascript">
        //Global variables
        var gridClientId = "ContentPlaceHolderBody_gvRoyCosts_";

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
            postBackElementID = args.get_postBackElement().id.substring(args.get_postBackElement().id.lastIndexOf("_") + 1);
            if (postBackElementID == 'btnAppendAddRow' || (args.get_postBackElement().id.indexOf('imgBtnCancel') != -1)) {
                xPos = sender._scrollPosition.x;
                yPos = sender._scrollPosition.y;

                //hold scroll position 
                var PnlReference = document.getElementById("<%=PnlGrid.ClientID %>");
                scrollTop = PnlReference.scrollTop;
            }

        }

        function EndRequestHandler(sender, args) {
            //Hide the modal popup - the update progress
            var popup = $find('<%= mPopupPageLevel.ClientID %>');
            if (popup != null) {
                popup.hide();
            }

            //to maintain scroll position
            postBackElementID = sender._postBackSettings.sourceElement.id.substring(sender._postBackSettings.sourceElement.id.lastIndexOf("_") + 1);
            if (postBackElementID == 'btnAppendAddRow' || (sender._postBackSettings.sourceElement.id.indexOf('imgBtnCancel') != -1)) {
                window.scrollTo(xPos, yPos);

                //set scroll position 
                var PnlReference = document.getElementById("<%=PnlGrid.ClientID %>");
                PnlReference.scrollTop = scrollTop;
            }


        }

        //probress bar and scroll position functionality - starts

        //grid panel height adjustment functioanlity - starts
        function SetGrdPnlHeightOnLoad() {
            var pnlGrid = document.getElementById("<%=PnlGrid.ClientID %>");
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
            if (pnlGrid != null) {
                pnlGrid.style.height = gridPanelHeight + "px";
            }
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
        }

        //End

        //Royaltor auto populate search functionalities
        var txtRoy;
        function royaltorSelected(sender, args) {            
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                document.getElementById("<%= txtRoyaltor.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%=hdnRoyaltorSelected.ClientID %>").value = "Y";
            }
        }

        function royaltorListPopulating() {           
            txtRoy = document.getElementById("<%= txtRoyaltor.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%=hdnRoyaltorSelected.ClientID %>").value = "N";
        }

        function royaltorListPopulated() {
            txtRoy = document.getElementById("<%= txtRoyaltor.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function resetScrollPosition(sender, args) {
            var autoCompPnl = document.getElementById("<%= autocompleteDropDownPanel1.ClientID %>");
            autoCompPnl.scrollTop = 1;

        }

        //================================End

        //Validation: warning message if changes made and not saved or on page change                                

        //Validate any unsaved data on browser window close/refresh
        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        function WarnOnUnSavedData() {
            var tdGrid = document.getElementById("<%=tdGrid.ClientID %>");
            if (tdGrid != null) {
                var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
                if (isExceptionRaised != "Y") {
                    if (IsDataChanged()) {
                        return "You have made changes which are not saved. Do you want to proceed by discarding the changes?";
                    }
                }
            }
        }
        window.onbeforeunload = WarnOnUnSavedData;

        function IsAddRowDataChanged() {

            //handling initial page load whre ther is no add row
            if (document.getElementById('<%=ddlAccTypeAdd.ClientID%>') == null) {
                return false;
            }

            ddlAccTypeAdd = document.getElementById('<%=ddlAccTypeAdd.ClientID%>').selectedIndex;
            txtDescAdd = document.getElementById('<%=txtDescAdd.ClientID%>').value;
            txtDateAdd = document.getElementById('<%=txtDateAdd.ClientID%>').value;
            txtAmountAdd = document.getElementById('<%=txtAmountAdd.ClientID%>').value;
            txtSuppNameAdd = document.getElementById('<%=txtSuppNameAdd.ClientID%>').value;
            txtProjCodeAdd = document.getElementById('<%=txtProjCodeAdd.ClientID%>').value;
            txtInvNumAdd = document.getElementById('<%=txtInvNumAdd.ClientID%>').value;

            if (ddlAccTypeAdd != 0 || txtDescAdd != "" || (txtDateAdd != "dd/mm/yyyy" && txtDateAdd != "__/__/____" && txtDateAdd != "") || txtAmountAdd != "" || txtSuppNameAdd != "" || txtProjCodeAdd != "" || txtInvNumAdd != "") {
                return true;
            }
            else {
                return false;
            }
        }

        function IsGridDataChanged() {
            //debugger;
            var gridDataChanged = "N";
            var str = "ContentPlaceHolderBody_gvRoyCosts_";
            var gvRoyaltyRates = document.getElementById("<%= gvRoyCosts.ClientID %>");
            if (gvRoyaltyRates != null) {
                var gvRows = gvRoyaltyRates.rows;
                for (var i = 0; i < gvRows.length; i++) {
                    //handling empty data row
                    if (gvRows.length == 1 && document.getElementById(str + 'hdnIsModified' + '_' + i) == null) {
                        break;
                    }

                    hdnIsModified = document.getElementById(str + 'hdnIsModified' + '_' + i).value;
                    if (hdnIsModified == "-") {
                        gridDataChanged = "Y";
                        break;
                    }

                    //check only for child rows
                    hdnAccTypeId = document.getElementById(str + 'hdnAccTypeId' + '_' + i).value;
                    if (hdnAccTypeId == "") {
                        continue;
                    }

                    var hdnTranDesc = document.getElementById(str + 'hdnTranDesc' + '_' + i).value;
                    var txtTranDesc = document.getElementById(str + 'txtTranDesc' + '_' + i).value;
                    var hdnDate = document.getElementById(str + 'hdnDate' + '_' + i).value;
                    var txtDate = document.getElementById(str + 'txtDate' + '_' + i).value;
                    var hdnAmount = document.getElementById(str + 'hdnAmount' + '_' + i).value;
                    var txtAmount = document.getElementById(str + 'txtAmount' + '_' + i).value;
                    var hdnSuppName = document.getElementById(str + 'hdnSuppName' + '_' + i).value;
                    var txtSuppName = document.getElementById(str + 'txtSuppName' + '_' + i).value;
                    var hdnProjCode = document.getElementById(str + 'hdnProjCode' + '_' + i).value;
                    var txtProjCode = document.getElementById(str + 'txtProjCode' + '_' + i).value;
                    var hdnInvoiceNum = document.getElementById(str + 'hdnInvoiceNum' + '_' + i).value;
                    var txtInvoiceNum = document.getElementById(str + 'txtInvoiceNum' + '_' + i).value;

                    var hdnIsDeleted = document.getElementById(str + 'hdnIsDeleted' + '_' + i).value;
                    var isDeleted;
                    var cbDisplay = document.getElementById(str + 'cbDisplay' + '_' + i);
                    if (cbDisplay.checked == true) {
                        isDeleted = 'N';
                    }
                    else {
                        isDeleted = 'Y';
                    }

                    if (hdnTranDesc != txtTranDesc || hdnDate != txtDate
                        || hdnAmount != txtAmount || hdnSuppName != txtSuppName ||
                        hdnProjCode != txtProjCode || hdnInvoiceNum != txtInvoiceNum || hdnIsDeleted != isDeleted) {
                        gridDataChanged = "Y";
                        break;
                    }

                }
            }

            if (gridDataChanged == "Y") {
                return true;
            }
            else {
                return false;
            }

        }

        //used to check if any changes to allow navigation to other screen 
        function IsDataChanged() {
            var tdGrid = document.getElementById("<%=tdGrid.ClientID %>");
            if (tdGrid != null) {
                if (IsGridDataChanged() || IsAddRowDataChanged()) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }

        function ConfirmSearch() {
            if (Page_ClientValidate("valSearch")) {
                if (IsDataChanged()) {
                    var popup = $find('<%= mpeConfirmation.ClientID %>');
                    if (popup != null) {
                        popup.show();
                        document.getElementById("<%=lblConfirmMsg.ClientID %>").innerText = "You have made changes which are not saved. Do you want to proceed by discarding the changes?";
                    }
                    return false;
                }
                else {
                    return true;
                }
            }
            else {
                return false;
            }
        }

        //Validation: warning message if changes made and not saved

        function ConfirmChangeSearchFields() {
            if (IsGridDataChanged()) {
                var popup = $find('<%= mpeSaveUndo.ClientID %>');
                if (popup != null) {
                    popup.show();
                    $get("<%=btnUndoChanges.ClientID%>").focus();
                }
                return false;
            }
            else {
                return true;
            }
        }

        //================================End

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        //=============== End

        //to open dashboard screen 

        //clear add row data
        function ClearAddRow() {

            document.getElementById('<%=ddlAccTypeAdd.ClientID%>').selectedIndex = 0;
            document.getElementById('<%=txtDescAdd.ClientID%>').value = "";
            document.getElementById('<%=txtDateAdd.ClientID%>').value = "";
            document.getElementById('<%=txtAmountAdd.ClientID%>').value = "";
            document.getElementById('<%=txtSuppNameAdd.ClientID%>').value = "";
            document.getElementById('<%=txtProjCodeAdd.ClientID%>').value = "";
            document.getElementById('<%=txtInvNumAdd.ClientID%>').value = "";
            Page_ClientValidate('');//clear all validators of the page
            document.getElementById("<%= ddlAccTypeAdd.ClientID %>").focus();
            return false;

        }
        //============== End  

        //Validations ============= Start
        function ValidateSave() {

            //check if no changes made to save
            if (!IsGridDataChanged()) {
                Page_ClientValidate('');
                DisplayMessagePopup("No changes made to save!");
                Page_BlockSubmit = false;
                return false;
            }

            //warning on save validation fail       
            if (!Page_ClientValidate("valSave")) {
                DisplayMessagePopup("Cost details not saved – invalid or missing data!");
                Page_BlockSubmit = false;
                return false;
            }
            else {
                return true;
            }

        }

        //WUIN-511 - Validation: do not allow negative values of amount field when account type is payment        
        var onlyPositiveNumberRegex = new RegExp(/^(\d*\.)?\d+$/); //only positive number with decimal places. Zero is allowed       


        function ValidateAmountEdit(sender, args) {
            var selectedRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            var valAmountEdit = document.getElementById(gridClientId + 'valAmountEdit' + '_' + selectedRowIndex);
            var hdnAccTypeIdVal = document.getElementById(gridClientId + 'hdnAccTypeId' + '_' + selectedRowIndex).value;
            var txtAmountVal = document.getElementById(gridClientId + 'txtAmount' + '_' + selectedRowIndex).value;

            if (txtAmountVal == "") {
                //mandatory validtion
                args.IsValid = false;
                valAmountEdit.title = "Please enter amount";
            }
            else if (isNaN(txtAmountVal)) {
                //only number validtion
                args.IsValid = false;
                valAmountEdit.title = "Please enter only number";
            }
            else if (hdnAccTypeIdVal == "1" &&
                (!onlyPositiveNumberRegex.test(txtAmountVal) || parseFloat(txtAmountVal) <= 0)) {
                //only positive and > 0 for account type payments                
                args.IsValid = false;
                valAmountEdit.title = "Payments should be greater than zero";

            }
            else {
                args.IsValid = true;
            }

        }

        //WUIN-511 - Validation: do not allow negative values of amount field when account type is payment
        function ValidateAmountAdd(sender, args) {
            var valAmountAdd = document.getElementById("<%=valAmountAdd.ClientID %>");
            var ddlAccTypeAddVal = document.getElementById("<%=ddlAccTypeAdd.ClientID %>").value;
            var txtAmountAddVal = document.getElementById("<%=txtAmountAdd.ClientID %>").value;

            if (txtAmountAddVal == "") {
                //mandatory validtion
                args.IsValid = false;
                valAmountAdd.title = "Please enter amount";
            }
            else if (isNaN(txtAmountAddVal)) {
                //only number validtion
                args.IsValid = false;
                valAmountAdd.title = "Please enter only number";
            }
            else if (ddlAccTypeAddVal == "1-Payments" &&
                        (!onlyPositiveNumberRegex.test(txtAmountAddVal) || parseFloat(txtAmountAddVal) <= 0)) {
                //only positive and > 0 for account type payments                
                args.IsValid = false;
                valAmountAdd.title = "Payments should be greater than zero";
            }
            else {
                args.IsValid = true;
            }

        }

        //Validations ============= End

        //Undo Grid changes
        function UndoGridChanges(gridRow) {
            gridRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);

            var hdnTranDesc = document.getElementById(gridClientId + 'hdnTranDesc' + '_' + gridRowIndex);
            var txtTranDesc = document.getElementById(gridClientId + 'txtTranDesc' + '_' + gridRowIndex);
            var hdnDate = document.getElementById(gridClientId + 'hdnDate' + '_' + gridRowIndex);
            var txtDate = document.getElementById(gridClientId + 'txtDate' + '_' + gridRowIndex);
            var hdnAmount = document.getElementById(gridClientId + 'hdnAmount' + '_' + gridRowIndex);
            var txtAmount = document.getElementById(gridClientId + 'txtAmount' + '_' + gridRowIndex);
            var hdnSuppName = document.getElementById(gridClientId + 'hdnSuppName' + '_' + gridRowIndex);
            var txtSuppName = document.getElementById(gridClientId + 'txtSuppName' + '_' + gridRowIndex);
            var hdnProjCode = document.getElementById(gridClientId + 'hdnProjCode' + '_' + gridRowIndex);
            var txtProjCode = document.getElementById(gridClientId + 'txtProjCode' + '_' + gridRowIndex);
            var hdnInvoiceNum = document.getElementById(gridClientId + 'hdnInvoiceNum' + '_' + gridRowIndex);
            var txtInvoiceNum = document.getElementById(gridClientId + 'txtInvoiceNum' + '_' + gridRowIndex);
            var hdnIsDeleted = document.getElementById(gridClientId + 'hdnIsDeleted' + '_' + gridRowIndex);
            var cbDisplay = document.getElementById(gridClientId + 'cbDisplay' + '_' + gridRowIndex);

            txtTranDesc.value = hdnTranDesc.value;
            txtDate.value = hdnDate.value;
            txtAmount.value = hdnAmount.value;
            txtSuppName.value = hdnSuppName.value;
            txtProjCode.value = hdnProjCode.value;
            txtInvoiceNum.value = hdnInvoiceNum.value;

            if (hdnIsDeleted.value == "N") {
                cbDisplay.checked = true;
            }
            else {
                cbDisplay.checked = false;
            }

            Page_ClientValidate('');//clear all validators of the page

            return false;

        }

        //Search by enter key
        function SearchByEnterKey() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnGo.ClientID%>').click();
            }
        }

        function OntxtRoyaltorKeyDown() {
            var txtRoyaltor = document.getElementById("<%= txtRoyaltor.ClientID %>").value;
            if ((event.keyCode == 13)) {                
                if (txtRoyaltor == "") {
                    document.getElementById('<%=btnGo.ClientID%>').click();
                }
                else {
                    if (document.getElementById("<%= hdnRoyaltorSelected.ClientID %>").value == "Y") {
                        document.getElementById('<%=btnGo.ClientID%>').click();
                    }
                    else {
                        return false;
                    }
                }
            }
        }

                //============== End
             
        // WUIN-1181 Changes
        function OnAppendAddRowKeyDown() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnAppendAddRow.ClientID%>').click();
            }
        }

    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="5">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    ROYALTOR COSTS
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="9%" class="identifierLable_large_bold">Royaltor
                    </td>
                    <td width="30%" style="padding-right: 0;">
                        <table width="100%">
                            <tr>
                                <td width="70%">
                                    <asp:TextBox ID="txtRoyaltor" runat="server" Width="98%" CssClass="identifierLable" onfocus="return ConfirmChangeSearchFields();"
                                        OnTextChanged="txtRoyaltor_TextChanged" AutoPostBack="true" onkeydown="OntxtRoyaltorKeyDown();" TabIndex="100"></asp:TextBox>
                                    <ajaxToolkit:AutoCompleteExtender ID="royaltorFilterExtender" runat="server"
                                        ServiceMethod="FuzzySearchAllRoyListWithOwnerCode"
                                        ServicePath="~/Services/FuzzySearch.asmx"
                                        MinimumPrefixLength="1"
                                        CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                        TargetControlID="txtRoyaltor"
                                        FirstRowSelected="true"
                                        OnClientItemSelected="royaltorSelected"
                                        OnClientPopulating="royaltorListPopulating"
                                        OnClientPopulated="royaltorListPopulated"
                                        OnClientHidden="royaltorListPopulated"
                                        OnClientShown="resetScrollPosition"
                                        CompletionListElementID="autocompleteDropDownPanel1" />
                                    <asp:Panel ID="autocompleteDropDownPanel1" runat="server" CssClass="identifierLable" />
                                </td>
                                <td align="left" valign="top" style="padding-left: 0;">
                                    <asp:ImageButton ID="fuzzySearchRoyaltor" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                                        ToolTip="Search Royaltor" OnClick="fuzzySearchRoyaltor_Click" CssClass="FuzzySearch_Button" />
                                    <asp:RequiredFieldValidator runat="server" ID="rfvRoyaltor" ControlToValidate="txtRoyaltor" ValidationGroup="valSearch"
                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please select royaltor"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                    </td>

                    <td align="right">
                        <table width="25%">
                            <tr>
                                <td align="right">
                                    <asp:Button ID="btnSave" runat="server" CssClass="ButtonStyle" OnClientClick="if (!ValidateSave()) { return false;};" OnClick="btnSave_Click"
                                        Text="Save Changes" UseSubmitBehavior="false" Width="90%" TabIndex="105" ValidationGroup="valSave" />
                                </td>
                                <td width="35%"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">Statement Period
                    </td>
                    <td>
                        <table width="100%">
                            <tr>
                                <td>
                                    <asp:DropDownList ID="ddlStmtPeriod" runat="server" Width="50%" CssClass="ddlStyle" CausesValidation="true" onfocus="return ConfirmChangeSearchFields();"
                                        onkeydown="SearchByEnterKey();" TabIndex="101">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td colspan="2"></td>
                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">From Date</td>
                    <td>
                        <table width="100%">
                            <tr>
                                <td width="25%">
                                    <asp:TextBox ID="txtFromDate" runat="server" Width="65" CssClass="identifierLable" onfocus="return ConfirmChangeSearchFields();"
                                        ValidationGroup="valSave" onkeydown="SearchByEnterKey();" TabIndex="102"></asp:TextBox>
                                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmeFromDate" runat="server" TargetControlID="txtFromDate"
                                        WatermarkText="mm/yyyy" WatermarkCssClass="waterMarkText">
                                    </ajaxToolkit:TextBoxWatermarkExtender>
                                    <ajaxToolkit:MaskedEditExtender ID="mteFromDate" runat="server"
                                        TargetControlID="txtFromDate" Mask="99/9999" AcceptNegative="None"
                                        ClearMaskOnLostFocus="false" />
                                    <%--<asp:RegularExpressionValidator ID="regFromDate" runat="server" ControlToValidate="txtFromDate" ValidationExpression="((0[1-9]|1[0-2])\/((19|20)\d\d))$"
                                        ErrorMessage="*" ToolTip="Please enter valid date in mm/yyyy format" ValidationGroup="valSearch" CssClass="requiredFieldValidator" />--%>
                                </td>
                                <td width="1%"></td>
                                <td width="15%" class="identifierLable_large_bold" align="center">To Date</td>
                                <td width="25%">
                                    <asp:TextBox ID="txtToDate" runat="server" Width="65" CssClass="identifierLable" onfocus="return ConfirmChangeSearchFields();"
                                        ValidationGroup="valSave" onkeydown="SearchByEnterKey();" TabIndex="103"></asp:TextBox>
                                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmeToDate" runat="server" TargetControlID="txtToDate"
                                        WatermarkText="mm/yyyy" WatermarkCssClass="waterMarkText">
                                    </ajaxToolkit:TextBoxWatermarkExtender>
                                    <ajaxToolkit:MaskedEditExtender ID="mteToDate" runat="server"
                                        TargetControlID="txtToDate" Mask="99/9999" AcceptNegative="None"
                                        ClearMaskOnLostFocus="false" />
                                    <%--<asp:RegularExpressionValidator ID="regToDate" runat="server" ControlToValidate="txtToDate" ValidationExpression="((0[1-9]|1[0-2])\/((19|20)\d\d))$"
                                        ErrorMessage="*" ToolTip="Please enter valid date in mm/yyyy format" ValidationGroup="valSearch" CssClass="requiredFieldValidator" />--%>
                                </td>
                                <td></td>
                            </tr>
                        </table>
                    </td>

                    <td colspan="2">
                        <asp:CustomValidator ID="valFrmToDates" runat="server" ValidationGroup="valSearch" CssClass="errorMessage"
                            OnServerValidate="valFrmToDates_ServerValidate"
                            ErrorMessage="From date should be earlier than the To date!"></asp:CustomValidator>
                    </td>

                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="8%"></td>
                    <td width="15%">
                        <table width="100%">
                            <tr>
                                <td>
                                    <asp:Button ID="btnGo" runat="server" Text="Go" CssClass="ButtonStyle" Width="10%" ValidationGroup="valSearch"
                                        OnClick="btnGo_Click" OnClientClick="if (!ConfirmSearch()) { return false;};" CausesValidation="true" UseSubmitBehavior="false"
                                        TabIndex="104" onkeydown="OnTabPress();" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td colspan="2"></td>

                </tr>
                <tr>
                    <td colspan="5"></td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="4" align="left" runat="server" id="tdGrid">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td colspan="5" align="left">
                                    <table width="93.75%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td width="10%" class="gridHeaderStyle_2rows">&nbsp</td>
                                            <td width="24%" class="gridHeaderStyle_2rows">Transaction Description</td>
                                            <td width="9%" class="gridHeaderStyle_2rows">Date</td>
                                            <td width="10%" class="gridHeaderStyle_2rows">Amount</td>
                                            <td width="14%" class="gridHeaderStyle_2rows">Supplier Name</td>
                                            <td width="10%" class="gridHeaderStyle_2rows">Project Code</td>
                                            <td width="14%" class="gridHeaderStyle_2rows">Invoice No.</td>
                                            <td width="5%" class="gridHeaderStyle_2rows">Display?</td>
                                            <td width="4%" class="gridHeaderStyle_2rows">&nbsp</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="5">
                                    <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="95%">
                                        <asp:GridView ID="gvRoyCosts" runat="server" AutoGenerateColumns="False" Width="98.65%" AlternatingRowStyle-BackColor="#E3EFFF"
                                            CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" ShowHeaderWhenEmpty="true" EmptyDataText="No Data Found"
                                            OnRowDataBound="gvRoyCosts_RowDataBound" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" ShowHeader="false">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Center_Align" ItemStyle-Font-Bold="true">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAccType" runat="server" Text='<%#Bind("account_type_desc")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="24%" ItemStyle-CssClass="gridItemStyle_Left_Align">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtTranDesc" runat="server" Text='<%#Bind("journal_voucher_desc")%>' CssClass="gridTextField"
                                                            MaxLength="42" ToolTip="upto 42 chars" Width="90%"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ID="rfvDescEdit" ControlToValidate="txtTranDesc" ValidationGroup="valSave"
                                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter description" Display="Dynamic"></asp:RequiredFieldValidator>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="9%" ItemStyle-CssClass="gridItemStyle_Left_Align">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtDate" runat="server" Text='<%#Bind("journal_voucher_date")%>' CssClass="gridTextField"
                                                            Width="65"></asp:TextBox>
                                                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmeTxtDate" runat="server" TargetControlID="txtDate"
                                                            WatermarkText="dd/mm/yyyy" WatermarkCssClass="waterMarkText">
                                                        </ajaxToolkit:TextBoxWatermarkExtender>
                                                        <ajaxToolkit:MaskedEditExtender ID="maskEditDate" runat="server"
                                                            TargetControlID="txtDate" Mask="99/99/9999" AcceptNegative="None"
                                                            ClearMaskOnLostFocus="true" MaskType="Date" />
                                                        <asp:RequiredFieldValidator runat="server" ID="rfvDateEdit" ControlToValidate="txtDate" ValidationGroup="valSave"
                                                            Text="*" CssClass="requiredFieldValidator" InitialValue="" ToolTip="Please enter date" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="regDateEdit" runat="server" ControlToValidate="txtDate"
                                                            ValidationExpression="^(?=\d{2}([\/])\d{2}\1\d{4}$)(?:0[1-9]|1\d|[2][0-8]|29(?!.02.(?!(?!(?:[02468][1-35-79]|[13579][0-13-57-9])00)\d{2}(?:[02468][048]|[13579][26])))|30(?!.02)|31(?=.(?:0[13578]|10|12))).(?:0[1-9]|1[012]).(19|20)\d{2}$"
                                                            ErrorMessage="*" ToolTip="Please enter valid date in DD/MM/YYYY format" ValidationGroup="valSave" CssClass="requiredFieldValidator"
                                                            Display="Dynamic" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Right_Align">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtAmount" runat="server" Style="text-align: right;" Text='<%#Bind("journal_voucher_amount")%>'
                                                            MaxLength="14" ToolTip="integer/decimal numbers upto 14 numerics" CssClass="gridTextField"
                                                            Width="50%"></asp:TextBox>
                                                        <asp:CustomValidator ID="valAmountEdit" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator" Display="Dynamic"
                                                            ClientValidationFunction="ValidateAmountEdit" ErrorMessage="*"></asp:CustomValidator>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="14%" ItemStyle-CssClass="gridItemStyle_Left_Align">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtSuppName" runat="server" Text='<%#Bind("journal_supplier_name")%>' CssClass="gridTextField"
                                                            Width="90%" MaxLength="50" ToolTip="upto 50 chars"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Left_Align">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtProjCode" runat="server" Text='<%#Bind("project_code")%>' CssClass="gridTextField"
                                                            Width="85%" MaxLength="11" ToolTip="upto 11 chars"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="14%" ItemStyle-CssClass="gridItemStyle_Left_Align">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtInvoiceNum" runat="server" Text='<%#Bind("invoice_no")%>' CssClass="gridTextField"
                                                            Width="90%" MaxLength="50" ToolTip="upto 50 chars"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cbDisplay" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Left_Align">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imgBtnCancel" runat="server" ImageUrl="../Images/cancel_row3.png"
                                                            ToolTip="Cancel" OnClientClick="return UndoGridChanges(this);" />
                                                        <asp:HiddenField ID="hdnSno" runat="server" Value='<%#Bind("sno")%>' />
                                                        <asp:HiddenField ID="hdnDisplayOrder" runat="server" Value='<%#Bind("display_order")%>' />
                                                        <asp:HiddenField ID="hdnJournalEntryId" runat="server" Value='<%#Bind("journal_entry_id")%>' />
                                                        <asp:HiddenField ID="hdnAccTypeId" runat="server" Value='<%#Bind("account_type_id")%>' />
                                                        <asp:HiddenField ID="hdnRoyaltorId" runat="server" Value='<%#Bind("royaltor_id")%>' />
                                                        <asp:HiddenField ID="hdnTranDesc" runat="server" Value='<%#Bind("journal_voucher_desc")%>' />
                                                        <asp:HiddenField ID="hdnDate" runat="server" Value='<%#Bind("journal_voucher_date")%>' />
                                                        <asp:HiddenField ID="hdnAmount" runat="server" Value='<%#Bind("journal_voucher_amount")%>' />
                                                        <asp:HiddenField ID="hdnSuppName" runat="server" Value='<%#Bind("journal_supplier_name")%>' />
                                                        <asp:HiddenField ID="hdnProjCode" runat="server" Value='<%#Bind("project_code")%>' />
                                                        <asp:HiddenField ID="hdnInvoiceNum" runat="server" Value='<%#Bind("invoice_no")%>' />
                                                        <asp:HiddenField ID="hdnIsDeleted" runat="server" Value='<%#Bind("is_deleted")%>' />
                                                        <asp:HiddenField ID="hdnIsModified" runat="server" Value='<%# Bind("is_modified") %>' />
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
                            <tr>
                                <td colspan="5" align="left">
                                    <table width="93.75%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td width="10%" class="gridHeaderStyle_2rows">Account Type</td>
                                            <td width="24%" class="gridHeaderStyle_2rows">Transaction Description</td>
                                            <td width="9%" class="gridHeaderStyle_2rows">Date</td>
                                            <td width="10%" class="gridHeaderStyle_2rows">Amount</td>
                                            <td width="14%" class="gridHeaderStyle_2rows">Supplier Name</td>
                                            <td width="10%" class="gridHeaderStyle_2rows">Project Code</td>
                                            <td width="14%" class="gridHeaderStyle_2rows">Invoice No.</td>
                                            <td width="9%" class="gridHeaderStyle_2rows">&nbsp</td>
                                        </tr>
                                        <tr>
                                            <td class="insertBoxStyle">
                                                <asp:DropDownList ID="ddlAccTypeAdd" runat="server" CssClass="ddlStyle" Width="90%">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvAccTypeAdd" ControlToValidate="ddlAccTypeAdd" ValidationGroup="valAdd"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please select account type"
                                                    InitialValue="-"></asp:RequiredFieldValidator>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:TextBox ID="txtDescAdd" runat="server" CssClass="identifierLable" Width="90%"
                                                    MaxLength="42" ToolTip="upto 42 chars"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvDescAdd" ControlToValidate="txtDescAdd" ValidationGroup="valAdd"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter description"></asp:RequiredFieldValidator>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:TextBox ID="txtDateAdd" runat="server" CssClass="identifierLable" Width="65"
                                                    MaxLength="10" ToolTip="DD/MM/YYYY"></asp:TextBox>
                                                <ajaxToolkit:TextBoxWatermarkExtender ID="wmeTxtDateAdd" runat="server" TargetControlID="txtDateAdd"
                                                    WatermarkText="dd/mm/yyyy" WatermarkCssClass="waterMarkText">
                                                </ajaxToolkit:TextBoxWatermarkExtender>
                                                <ajaxToolkit:MaskedEditExtender ID="maskEditDateAdd" runat="server"
                                                    TargetControlID="txtDateAdd" Mask="99/99/9999" AcceptNegative="None"
                                                    ClearMaskOnLostFocus="true" MaskType="Date" />
                                                <asp:RequiredFieldValidator runat="server" ID="rfvDateAdd" ControlToValidate="txtDateAdd" ValidationGroup="valAdd"
                                                    Text="*" CssClass="requiredFieldValidator" InitialValue="" ToolTip="Please enter date"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="regDateAdd" runat="server" ControlToValidate="txtDateAdd"
                                                    ValidationExpression="^(?=\d{2}([\/])\d{2}\1\d{4}$)(?:0[1-9]|1\d|[2][0-8]|29(?!.02.(?!(?!(?:[02468][1-35-79]|[13579][0-13-57-9])00)\d{2}(?:[02468][048]|[13579][26])))|30(?!.02)|31(?=.(?:0[13578]|10|12))).(?:0[1-9]|1[012]).(19|20)\d{2}$"
                                                    ErrorMessage="*" ToolTip="Please enter valid date in DD/MM/YYYY format" ValidationGroup="valAdd" CssClass="requiredFieldValidator"
                                                    Display="Dynamic" />
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:TextBox ID="txtAmountAdd" runat="server" CssClass="identifierLable" Width="80%"
                                                    MaxLength="14" ToolTip="integer/decimal numbers upto 14 numerics"></asp:TextBox>
                                                <asp:CustomValidator ID="valAmountAdd" runat="server" ValidationGroup="valAdd" CssClass="requiredFieldValidator" Display="Dynamic"
                                                    ClientValidationFunction="ValidateAmountAdd" ErrorMessage="*"></asp:CustomValidator>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:TextBox ID="txtSuppNameAdd" runat="server" CssClass="identifierLable" Width="90%"
                                                    MaxLength="50" ToolTip="upto 50 chars"></asp:TextBox>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:TextBox ID="txtProjCodeAdd" runat="server" CssClass="identifierLable" Width="85%"
                                                    MaxLength="11" ToolTip="upto 11 chars"></asp:TextBox>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:TextBox ID="txtInvNumAdd" runat="server" CssClass="identifierLable" Width="90%"
                                                    MaxLength="50" ToolTip="upto 50 chars"></asp:TextBox>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <%--JIRA-974 Changes by Ravi on 05/02/2019 -- Start--%>
                                                            <asp:ImageButton ID="btnAppendAddRow" ImageUrl="../Images/add_row.png" runat="server" ToolTip="Add Cost"
                                                                OnClick="btnAppendAddRow_Click" ValidationGroup="valAdd"  Onkeydown="OnAppendAddRowKeyDown();" />
                                                            <%--JIRA-974 Changes by Ravi on 05/02/2019 -- End--%>
                                                        </td>
                                                        <td>
                                                            <asp:ImageButton ID="imgBtnCancel" runat="server" ImageUrl="../Images/cancel_row3.png" OnClientClick="return ClearAddRow();" ToolTip="Cancel" />
                                                        </td>
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
                    <div id="Search" style="font-weight: bold; color: Black">
                        <table>
                            <tr>
                                <td>
                                    <img src="../Images/InProgress2.gif" alt="" />
                                </td>
                            </tr>
                            <tr>
                                <td class="identifierLable">Please Wait... </td>
                            </tr>
                        </table>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <ajaxToolkit:ModalPopupExtender ID="mPopupPageLevel" runat="server" BackgroundCssClass="progressBar" PopupControlID="progressBarPageLevel" RepositionMode="RepositionOnWindowResize" TargetControlID="progressBarPageLevel">
            </ajaxToolkit:ModalPopupExtender>

            <%--Warning popup--%>
            <asp:Button ID="dummyConfirm" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmation" runat="server" PopupControlID="pnlPopup" TargetControlID="dummyConfirm"
                CancelControlID="btnNoConfirm" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlPopup" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="Label1" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblConfirmMsg" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYesSearch" runat="server" Text="Yes" CssClass="ButtonStyle"
                                            OnClick="btnYesSearch_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNoConfirm" runat="server" Text="No" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

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

            <%--Save/Undo changes popup--%>
            <asp:Button ID="dummySaveUndo" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeSaveUndo" runat="server" PopupControlID="pnlSaveUndo" TargetControlID="dummySaveUndo"
                CancelControlID="btnClosePopupSaveUndo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlSaveUndo" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td align="right" style="vertical-align: top;">
                            <asp:ImageButton ID="btnClosePopupSaveUndo" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMessage" runat="server" CssClass="identifierLable"
                                Text="You have made changes which are not saved. Save or Undo changes"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnConfirmSaveChanges" runat="server" Text="Save" CssClass="ButtonStyle"
                                            OnClientClick="if (!ValidateSave()) { return false;};" OnClick="btnSave_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnUndoChanges" runat="server" Text="Undo" CssClass="ButtonStyle" OnClick="btnUndoChanges_Click" CausesValidation="false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnUserRole" runat="server" />
            <asp:HiddenField ID="hdnGridRowSelectedPrvious" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnRoyaltorSelected" runat="server" Value="N" />            
            <asp:Label ID="lblTab" runat="server" TabIndex="99" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

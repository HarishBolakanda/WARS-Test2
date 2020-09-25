<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoyContractEscHistory.aspx.cs" Inherits="WARS.Contract.RoyContractEscHistory" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Royaltor Contract Escalation History" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

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
        var gridClientId = "ContentPlaceHolderBody_gvEscHistory_";
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
            //debugger;
            //to maintain scroll position
            var postBackElement = args.get_postBackElement().id;
            postBackElementID = args.get_postBackElement().id.substring(args.get_postBackElement().id.lastIndexOf("_") + 1);
            if (postBackElement.indexOf('imgBtnSave') != -1 || postBackElement.indexOf('imgBtnDelete') != -1) {
                xPos = sender._scrollPosition.x;
                yPos = sender._scrollPosition.y;

                //hold scroll position on selecting expand/collapse
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
            //debugger;
            //to maintain scroll position
            var postBackElement = sender._postBackSettings.sourceElement.id;
            postBackElementID = sender._postBackSettings.sourceElement.id.substring(sender._postBackSettings.sourceElement.id.lastIndexOf("_") + 1);
            if (postBackElement.indexOf('imgBtnSave') != -1 || postBackElement.indexOf('imgBtnDelete') != -1) {
                window.scrollTo(xPos, yPos);

                //set scroll position on selecting expand/collapse
                var PnlReference = document.getElementById("<%=PnlGrid.ClientID %>");
                PnlReference.scrollTop = scrollTop;
            }


        }
        //======================= End



        //grid panel height adjustment functioanlity - starts
        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
            document.getElementById("<%=PnlGrid.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
        }
        //======================= End

        //Royaltor fuzzy search functionalities

        function royaltorListPopulating() {
            txtRoy = document.getElementById("<%= txtRoyaltor.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnRoySearchSelected.ClientID %>").value = "N";
        }

        function royaltorListPopulated() {
            txtRoy = document.getElementById("<%= txtRoyaltor.ClientID %>");
            txtRoy.style.backgroundImage = 'none';

        }

        function royaltorListItemSelected(sender, args) {
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                document.getElementById("<%= txtRoyaltor.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnRoySearchSelected.ClientID %>").value = "Y";
                document.getElementById('<%=btnRoyaltorSearch.ClientID%>').click();
            }

        }

        function RoyaltorSearchChanged() {
            //on search field cleared
            if (document.getElementById("<%= txtRoyaltor.ClientID %>").value == "") {
                document.getElementById("<%= ddlEscCode.ClientID %>").innerText = null;
                document.getElementById("<%= gvEscHistory.ClientID %>").innerText = null;
                document.getElementById("<%= lblTotalSales.ClientID %>").innerText = "0";
                document.getElementById("<%= lblTotalAdjSales.ClientID %>").innerText = "0";
            }
        }

        //================================End

        function UndoGridChanges(gridRow) {
            var selectedRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
            var hdnUnits = document.getElementById(gridClientId + 'hdnUnits' + '_' + selectedRowIndex).value;
            var hdnAdjUnits = document.getElementById(gridClientId + 'hdnAdjUnits' + '_' + selectedRowIndex).value;
            var txtSales = document.getElementById(gridClientId + 'txtSales' + '_' + selectedRowIndex);
            var txtAdjSales = document.getElementById(gridClientId + 'txtAdjSales' + '_' + selectedRowIndex);

            txtSales.value = hdnUnits;
            txtAdjSales.value = hdnAdjUnits;
            Page_ClientValidate('');
            return false;
        }


        //Confim delete
        function ConfirmDelete(row) {
            //JIRA-908 Changes by Ravi on 13/02/2019 -- Start
            //set if this is not a newly added row

            var selectedRowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            var hdnRoyaltorId = document.getElementById(gridClientId + 'hdnRoyaltorId' + '_' + selectedRowIndex).value;
            var hdnEscCode = document.getElementById(gridClientId + 'hdnEscCode' + '_' + selectedRowIndex).value;
            var hdnSellerGrpCode = document.getElementById(gridClientId + 'hdnSellerGrpCode' + '_' + selectedRowIndex).value;
            var hdnConfigGrpCode = document.getElementById(gridClientId + 'hdnConfigGrpCode' + '_' + selectedRowIndex).value;
            var hdnPriceGrpCode = document.getElementById(gridClientId + 'hdnPriceGrpCode' + '_' + selectedRowIndex).value;

            document.getElementById("<%=hdnDeleteRoyaltorId.ClientID %>").innerText = hdnRoyaltorId;
            document.getElementById("<%=hdnDeleteEscCode.ClientID %>").innerText = hdnEscCode;
            document.getElementById("<%=hdnDeleteSellerGrpCode.ClientID %>").innerText = hdnSellerGrpCode;
            document.getElementById("<%=hdnDeleteConfigGrpCode.ClientID %>").innerText = hdnConfigGrpCode;
            document.getElementById("<%=hdnDeletePriceGrpCode.ClientID %>").innerText = hdnPriceGrpCode;
            var popup = $find('<%= mpeConfirmDelete.ClientID %>');
            if (popup != null) {
                popup.show();
            }
            return false;

            //JIRA-908 Changes by Ravi on 13/02/2019 -- End
        }

        //Grid previous row changes save/undo -- Start

        function OnGridRowSelected(row) {
            var rowData = row.parentNode.parentNode;
            var rowIndex = rowData.rowIndex - 1;
            var hdnGridRowSelectedPrvious = document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value;
            if (hdnGridRowSelectedPrvious == "") {
                document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").innerText = rowIndex;
            }
            else if (hdnGridRowSelectedPrvious != rowIndex) {
                if (IsPreviousRowChangesSaved(hdnGridRowSelectedPrvious)) {
                    var popup = $find('<%= mpeSaveUndo.ClientID %>');
                    if (popup != null) {
                        popup.show();
                        $get("<%=btnUndoChanges.ClientID%>").focus();
                    }
                }
                else {
                    document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").innerText = "";
                }

            }
    }

    function IsPreviousRowChangesSaved(prevRowIndex) {
        hdnUnits = document.getElementById(gridClientId + 'hdnUnits' + '_' + prevRowIndex).value;
        hdnAdjUnits = document.getElementById(gridClientId + 'hdnAdjUnits' + '_' + prevRowIndex).value;
        txtSales = document.getElementById(gridClientId + 'txtSales' + '_' + prevRowIndex).value;
        txtAdjSales = document.getElementById(gridClientId + 'txtAdjSales' + '_' + prevRowIndex).value;

        if (hdnUnits != txtSales || hdnAdjUnits != txtAdjSales) {
            return true;
        }
        else {
            return false;
        }
    }

    function UndoPreviousRowChanges() {
        hdnGridRowSelectedPrvious = document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value;
        hdnUnits = document.getElementById(gridClientId + 'hdnUnits' + '_' + hdnGridRowSelectedPrvious).value;
        hdnAdjUnits = document.getElementById(gridClientId + 'hdnAdjUnits' + '_' + hdnGridRowSelectedPrvious).value;
        txtSales = document.getElementById(gridClientId + 'txtSales' + '_' + hdnGridRowSelectedPrvious);
        txtAdjSales = document.getElementById(gridClientId + 'txtAdjSales' + '_' + hdnGridRowSelectedPrvious);

        txtSales.value = hdnUnits;
        txtAdjSales.value = hdnAdjUnits;

        var popup = $find('<%= mpeSaveUndo.ClientID %>');
        if (popup != null) {
            popup.hide();
        }

        document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").innerText = "";
        Page_ClientValidate('');
        return false;

    }

    //=============Grid previous row changes save/undo -- End

    //Validate any unsaved data on browser window close/refresh -- start

    var unSaveBrowserClose = false;
    function WarnOnUnSavedData() {
        var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
        var isContractScreen = document.getElementById("hdnIsContractScreen").value;
        var isNotContractScreen = document.getElementById("hdnIsNotContractScreen").value;
        var isRoyaltorNull = document.getElementById("<%=hdnIsRoyaltorNull.ClientID %>").value;
        if (isExceptionRaised != "Y" && isContractScreen != "Y" && isNotContractScreen != "Y") {
            if (IsGridDataChanged()) {
                unSaveBrowserClose = true;
                return warningMsgOnUnSavedData;
            }
        }

        if (isRoyaltorNull != "Y") {
            UpdateScreenLockFlag();// WUIN-599 - Unset the screen lock flag If an user close the browser with out unsaved data or navigate to other than contract screens
        }
    }
    window.onbeforeunload = WarnOnUnSavedData;

    //WUIN-599 Unset the screen lock flag If an user close the browser or navigate to other than contract screens
    window.onunload = function () {
        var isRoyaltorNull = document.getElementById("<%=hdnIsRoyaltorNull.ClientID %>").value;
        if (unSaveBrowserClose) {
            if (isRoyaltorNull != "Y") {
                UpdateScreenLockFlag();
            }
        }
    }

        function UpdateScreenLockFlag() {
            var isOtherUserScreenLocked = document.getElementById("<%=hdnOtherUserScreenLocked.ClientID %>").value;
        var isContractScreen = document.getElementById("hdnIsContractScreen").value;

        if (isOtherUserScreenLocked == "N" && isContractScreen == "N") {
            PageMethods.UpdateScreenLockFlag();
        }
    }

    function RedirectToErrorPage() {
        document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        function IsGridDataChanged() {
            gvEscHistory = document.getElementById("<%= gvEscHistory.ClientID %>");

            if (gvEscHistory != null) {
                var gvRows = gvEscHistory.rows;// WUIN-746 grid view rows including header row
                var rowIndex;
                for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                    rowIndex = i - 1; //WUIN-746 row index start from 0

                    //handling empty data row
                    if (gvRows.length == 2 && document.getElementById(gridClientId + 'hdnRoyaltorId' + '_' + rowIndex) == null) {
                        break;
                    }

                    hdnUnits = document.getElementById(gridClientId + 'hdnUnits' + '_' + rowIndex).value;
                    hdnAdjUnits = document.getElementById(gridClientId + 'hdnAdjUnits' + '_' + rowIndex).value;
                    txtSales = document.getElementById(gridClientId + 'txtSales' + '_' + rowIndex).value;
                    txtAdjSales = document.getElementById(gridClientId + 'txtAdjSales' + '_' + rowIndex).value;

                    if (hdnUnits != txtSales || hdnAdjUnits != txtAdjSales) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }

        }

        //used to check if any changes to allow navigation to other screen 
        function IsDataChanged() {
            //debugger;            
            if (IsGridDataChanged()) {
                return true;
            }
            else {
                return false;
            }
        }

        //========Validate any unsaved data on browser window close/refresh -- End

        //Data field validations  - start

        var numberOnlyRegex = /^[+\-]?\d+$/; //only positive/negative numbers without decimal places

        function ValidateSales(sender, args) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtSales = document.getElementById(gridClientId + 'txtSales' + '_' + gridRowIndex).value;
            valTxtSales = document.getElementById(gridClientId + 'valTxtSales' + '_' + gridRowIndex);

            //mandatory
            if (txtSales == "") {
                args.IsValid = false;
                valTxtSales.title = "Please enter a value";
                return;
            }

            //only positive or nagative numbers without decimal places allowed

            if (numberOnlyRegex.test(txtSales)) {
                args.IsValid = true;
            }
            else {
                args.IsValid = false;
                valTxtSales.title = "Please enter only positive or negative numbers without decimal places";
            }
        }

        function ValidateAdjSales(sender, args) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtAdjSales = document.getElementById(gridClientId + 'txtAdjSales' + '_' + gridRowIndex).value;
            valTxtAdjSales = document.getElementById(gridClientId + 'valTxtAdjSales' + '_' + gridRowIndex);

            //mandatory
            if (txtAdjSales == "") {
                args.IsValid = false;
                valTxtAdjSales.title = "Please enter a value";
                return;
            }

            //only positive or nagative numbers without decimal places allowed

            if (numberOnlyRegex.test(txtAdjSales)) {
                args.IsValid = true;
            }
            else {
                args.IsValid = false;
                valTxtAdjSales.title = "Please enter only positive or negative numbers without decimal places";
            }
        }

        function ValidatePopUpSave() {
            //warning on save validation fail
            //debugger;
            if (!Page_ClientValidate("valGrpSave")) {
                Page_BlockSubmit = false;
                DisplayMessagePopup("invalid or missing data!");
                return false;
            }
            else {
                return true;
            }


        }

        //==========Data field validations  - End

        //Navigation buttons        
        function OpenAuditScreen() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                alert("Audit Screen to be developed");
                // OpenPopupOnUnSavedData(); -- Audit Screen to be developed
            }
            else {
                alert("Audit Screen to be developed");
            }
        }

        //Navigation buttons -- Ends

        function ValidateUnsavedData(button) {
            if (IsDataChanged()) {
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
                document.getElementById('<%=btnUnSavedDataReturn.ClientID%>').focus();
                return false;
            }
            return true;
        }


        //WUIN-932 Unsaved Pop-up implementation
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
            return false;
        }

        function OnUnSavedDataExit() {
            window.onbeforeunload = WarnOnUnSavedData;

            var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
            if (warnPopup != null) {
                warnPopup.hide();
            }
            return true;
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
                                    ROYALTOR CONTRACT - ESCALATION HISTORY
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
                                <td width="30%">
                                    <asp:TextBox ID="txtRoyaltor" runat="server" Width="98%" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" TabIndex="100"
                                        onchange="RoyaltorSearchChanged();"></asp:TextBox>
                                    <ajaxToolkit:AutoCompleteExtender ID="royaltorFilterExtender" runat="server"
                                        ServiceMethod="FuzzySearchAllRoyListWithOwnerCode"
                                        ServicePath="~/Services/FuzzySearch.asmx"
                                        MinimumPrefixLength="1"
                                        CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                        TargetControlID="txtRoyaltor"
                                        FirstRowSelected="true"
                                        OnClientPopulating="royaltorListPopulating"
                                        OnClientPopulated="royaltorListPopulated"
                                        OnClientHidden="royaltorListPopulated"
                                        OnClientItemSelected="royaltorListItemSelected"
                                        CompletionListElementID="autocompleteDropDownPanel1" />
                                    <asp:Panel ID="autocompleteDropDownPanel1" runat="server" CssClass="identifierLable" />
                                </td>
                                <td valign="top" width="60%">
                                    <asp:ImageButton ID="fuzzySearchRoyaltor" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button" Style="cursor: pointer"
                                        OnClick="fuzzySearchRoyaltor_Click" ToolTip="Search Royaltor" />
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
                                            <td></td>
                                            <td>
                                                <asp:Button ID="btnAudit" runat="server" CssClass="ButtonStyle" OnClientClick="return OpenAuditScreen();"
                                                    Text="Audit" UseSubmitBehavior="false" Width="90%" TabIndex="111" onkeydown="OnTabPress();" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <br />
                                            </td>
                                        </tr>
                                        <tr id="trConNavBtns" runat="server">
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
                <tr>
                    <td></td>
                    <td class="table_header_with_border" valign="top">Escalation History</td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td valign="top">
                        <table width="100%" class="table_with_border">
                            <tr>
                                <td></td>
                            </tr>
                            <tr>
                                <td>
                                    <table width="98.5%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td width="10%" class="identifierLable_large_bold">Escalation Code</td>
                                            <td width="50%">
                                                <div style="position: relative; left: -3px;">
                                                    <asp:DropDownList ID="ddlEscCode" runat="server" CssClass="ddlStyle" Width="30%" TabIndex="101" OnSelectedIndexChanged="ddlEscCode_SelectedIndexChanged"  onfocus="if (!ValidateUnsavedData('EscalationCode')) { return false;};"
                                                        AutoPostBack="true">
                                                    </asp:DropDownList>
                                                </div>
                                            </td>
                                            <td width="40%" align="right">
                                                <asp:Button ID="btnHistorySummary" runat="server" CssClass="ButtonStyle" OnClick="btnHistorySummary_Click"  OnClientClick="if (!ValidateUnsavedData('HistorySummary')) { return false;};"
                                                    Text="History Summary" UseSubmitBehavior="false" TabIndex="102" />
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
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvEscHistory" runat="server" AutoGenerateColumns="False" Width="98.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found" OnRowDataBound="gvEscHistory_RowDataBound" OnRowCommand="gvEscHistory_RowCommand" AllowSorting="true" OnSorting="gvEscHistory_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Territory" SortExpression="territory" ItemStyle-Width="25%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblTerritory" runat="server" Text='<%#Bind("territory")%>' CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Configuration" SortExpression="configuration" ItemStyle-Width="25%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblConfiguration" runat="server" Text='<%#Bind("configuration")%>' CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Sales Type" SortExpression="pricegroup" ItemStyle-Width="15%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSalesType" runat="server" Text='<%#Bind("pricegroup")%>' CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Sales" SortExpression="units" ItemStyle-Width="10%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtSales" runat="server" Width="60px" Text='<%#Bind("units")%>' CssClass="gridTextField" Style="text-align: center"
                                                                        ToolTip="Please enter only numeric value without decimals" ValidationGroup="valGrpSave"
                                                                        onfocus="OnGridRowSelected(this)"></asp:TextBox>
                                                                    <asp:CustomValidator ID="valTxtSales" runat="server" ValidationGroup="valGrpSave" CssClass="requiredFieldValidator"
                                                                        ClientValidationFunction="ValidateSales" ToolTip="Please enter a value"
                                                                        ErrorMessage="*"></asp:CustomValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Adjusted Sales" SortExpression="adjusted_units" ItemStyle-Width="10%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtAdjSales" runat="server" Width="60px" Text='<%#Bind("adjusted_units")%>' CssClass="gridTextField" Style="text-align: center"
                                                                        ToolTip="Please enter only numeric value without decimals" ValidationGroup="valSave" onfocus="OnGridRowSelected(this)"></asp:TextBox>
                                                                    <asp:CustomValidator ID="valTxtAdjSales" runat="server" ValidationGroup="valGrpSave" CssClass="requiredFieldValidator"
                                                                        ClientValidationFunction="ValidateAdjSales" ToolTip="Please enter a value"
                                                                        ErrorMessage="*"></asp:CustomValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" ItemStyle-Width="5%">
                                                                <ItemTemplate>
                                                                    <table width="95%" style="float: right; table-layout: fixed">
                                                                        <tr style="float: right">
                                                                            <td align="center">
                                                                                <asp:ImageButton ID="imgBtnSave" runat="server" CommandName="SaveRow" ImageUrl="../Images/save.png"
                                                                                    ToolTip="Save" ValidationGroup="valGrpSave" TabIndex="112"
                                                                                    OnClientClick="if (!ValidatePopUpSave()) { return false;};" />
                                                                            </td>
                                                                            <td align="center">
                                                                                <asp:ImageButton ID="imgBtnDelete" runat="server" ImageUrl="../Images/Delete.gif"
                                                                                    ToolTip="Delete" OnClientClick="return ConfirmDelete(this);" />
                                                                            </td>
                                                                            <td align="center">
                                                                                <asp:ImageButton ID="imgBtnUndo" runat="server" CommandName="cancelRow" ImageUrl="../Images/cancel_row3.png"
                                                                                    ToolTip="Cancel" OnClientClick="return UndoGridChanges(this);" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                    <asp:HiddenField ID="hdnRoyaltorId" runat="server" Value='<%# Bind("royaltor_id") %>' />
                                                                    <asp:HiddenField ID="hdnEscCode" runat="server" Value='<%# Bind("esc_code") %>' />
                                                                    <asp:HiddenField ID="hdnSellerGrpCode" runat="server" Value='<%# Bind("seller_group_code") %>' />
                                                                    <asp:HiddenField ID="hdnConfigGrpCode" runat="server" Value='<%# Bind("config_group_code") %>' />
                                                                    <asp:HiddenField ID="hdnPriceGrpCode" runat="server" Value='<%# Bind("price_group_code") %>' />
                                                                    <asp:HiddenField ID="hdnUnits" runat="server" Value='<%# Bind("units") %>' />
                                                                    <asp:HiddenField ID="hdnAdjUnits" runat="server" Value='<%# Bind("adjusted_units") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table width="98.5%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td width="25%"></td>
                                            <td width="25%"></td>
                                            <td width="15%" class="identifierLable_large_bold" align="right" style="padding-right: 5px">Total Sales</td>
                                            <td class="insertBoxStyle" width="10%" align="center" style="background-color: #E3EFFF">
                                                <asp:Label ID="lblTotalSales" runat="server" CssClass="identifierLable" Style="text-align: center" Font-Bold="true"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle" width="10%" align="center" style="background-color: #E3EFFF">
                                                <asp:Label ID="lblTotalAdjSales" runat="server" CssClass="identifierLable" Style="text-align: center" Font-Bold="true"></asp:Label>
                                            </td>
                                            <td width="5%"></td>
                                        </tr>
                                    </table>
                                </td>
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

            <%--Save/Undo changes popup--%>
            <asp:Button ID="dummySaveUndo" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeSaveUndo" runat="server" PopupControlID="pnlSaveUndo" TargetControlID="dummySaveUndo"
                CancelControlID="btnClosePopupSaveUndo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlSaveUndo" runat="server" align="center" Width="25%" BackColor="Window" CssClass="popupPanel" Style="z-index: 1; display: none">
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
                                        <asp:Button ID="btnSaveChanges" runat="server" Text="Save" CssClass="ButtonStyle"
                                            OnClick="btnSaveChanges_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnUndoChanges" runat="server" Text="Undo" CssClass="ButtonStyle" OnClientClick="return UndoPreviousRowChanges();" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

              <%--Warning on unsaved data popup--%>
            <asp:Button ID="dummyUnsavedWarnMsg" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeUnSavedWarning" runat="server" PopupControlID="pnlUnsavedWarnMsgPopup" TargetControlID="dummyUnsavedWarnMsg"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlUnsavedWarnMsgPopup" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
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
                                        <asp:Button ID="btnUnSavedDataExit" runat="server" Text="Exit" CssClass="ButtonStyle" Width="30%" OnClientClick="OnUnSavedDataExit();" OnClick="btnUnSavedDataExit_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>           

            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- Start--%>
            <asp:Button ID="dummyConfirmDelete" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmDelete" runat="server" PopupControlID="pnlConfirmDelete" TargetControlID="dummyConfirmDelete"
                CancelControlID="btnNo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlConfirmDelete" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmation" runat="server" Text="Delete Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblText" runat="server" Text="Are you sure you want to delete the row?"
                                CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYes" runat="server" Text="Yes" CssClass="ButtonStyle" OnClick="btnYes_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNo" runat="server" Text="No" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- End--%>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnRoySearchSelected" runat="server" />
            <asp:HiddenField ID="hdnGridRowSelectedPrvious" runat="server" />
            <asp:Button ID="btnRoyaltorSearch" runat="server" Style="display: none;" OnClick="btnRoyaltorSearch_Click" CausesValidation="false" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" onkeydown="FocusLblKeyPress();"></asp:Label>
            <asp:HiddenField ID="hdnDeleteRoyaltorId" runat="server" />
            <asp:HiddenField ID="hdnDeleteEscCode" runat="server" />
            <asp:HiddenField ID="hdnDeleteSellerGrpCode" runat="server" />
            <asp:HiddenField ID="hdnDeleteConfigGrpCode" runat="server" />
            <asp:HiddenField ID="hdnDeletePriceGrpCode" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
            <asp:HiddenField ID="hdnOtherUserScreenLocked" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsRoyaltorNull" runat="server" Value="N" />            
            <asp:HiddenField ID="hdnButtonSelection" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

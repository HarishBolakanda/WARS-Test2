<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TerritorySearch.aspx.cs" Inherits="WARS.TerritorySearch" MasterPageFile="~/MasterPage.Master"
    Title="WARS - TerritorySearch" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //to open territory group screen in same tab
        function OpenTerritoryGroupScreen() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../DataMaintenance/TerritoryGroup.aspx', '_self');
                return false;
            }
            else {
                var win = window.open('../DataMaintenance/TerritoryGroup.aspx', '_self');
                win.focus();
                return true;
            }
        }

        //=============== End
    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnTerritoryGroup" runat="server" CssClass="LinkButtonStyle" Text="Territory Group Maintenance"
                            OnClientClick="if (!OpenTerritoryGroupScreen()) { return false;};"
                            UseSubmitBehavior="false" Width="98%" onkeydown="OnTabPress();" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderBody">
    <script type="text/javascript">
        //probress bar functionality - starts                
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

        //probress bar functionality - ends

        //grid panel height adjustment functioanlity - starts
        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelTerritoryHeight = windowHeight * 0.04;
            var gridPanelGroupHeight = windowHeight * 0.45;

            document.getElementById("<%=hdnGridPnlTerritoryHeight.ClientID %>").innerText = gridPanelTerritoryHeight;
            document.getElementById("<%=hdnGridPnlGroupHeight.ClientID %>").innerText = gridPanelGroupHeight;
        }

        //grid panel height adjustment functioanlity - ends


        //Territory auto populate search functionalities

        function territorySearchListPopulating() {
            txtRoy = document.getElementById("<%= txtTerritorySearch.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "N";
        }

        function territorySearchListPopulated() {
            txtRoy = document.getElementById("<%= txtTerritorySearch.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function territorySearchItemSelected(sender, args) {
            var territorySrchVal = args.get_value();
            if (territorySrchVal == 'No results found') {
                document.getElementById("<%= txtTerritorySearch.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "Y";
            }

        }

        function territoryLocSearchListPopulating() {
            txtRoy = document.getElementById("<%= txtTerritoryLocSearch.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "N";
        }

        function territoryLocSearchListPopulated() {
            txtRoy = document.getElementById("<%= txtTerritoryLocSearch.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function territoryLocItemSelected(sender, args) {
            var territoryLocSrchVal = args.get_value();
            if (territoryLocSrchVal == 'No results found') {
                document.getElementById("<%= txtTerritoryLocSearch.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "Y";
            }

        }

        function newTerritoryListPopulating() {
            txtRoy = document.getElementById("<%= txtNewTerritory.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "N";
        }

        function newTerritoryListPopulated() {
            txtRoy = document.getElementById("<%= txtNewTerritory.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function NewterritorySearchItemSelected(sender, args) {
            var territoryNewSrchVal = args.get_value();
            if (territoryNewSrchVal == 'No results found') {
                document.getElementById("<%= txtNewTerritory.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "Y";
            }

        }

        function newTerritoryLocListPopulating() {
            txtRoy = document.getElementById("<%= txtNewTerritoryLoc.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "N";
        }

        function newTerritoryLocListPopulated() {
            txtRoy = document.getElementById("<%= txtNewTerritoryLoc.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function territoryNewLocItemSelected(sender, args) {
            var territoryNewLocSrchVal = args.get_value();
            if (territoryNewLocSrchVal == 'No results found') {
                document.getElementById("<%= txtNewTerritoryLoc.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "Y";
            }

        }

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        function OpenAddTerritoryPopup(button) {
            if (IsDataChanged()) {
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
                return false;
            }

            document.getElementById("<%= txtTerritoryCode.ClientID %>").focus()
            var popup = $find('<%= mpeAddTerritoryCode.ClientID %>');
            if (popup != null) {
                popup.show();
            }
            return false;
        }

        function UndoTerritoryGroup() {
            var txtTerritoryName = document.getElementById("<%= txtGridTerritoryName.ClientID %>");
            var hdnTerritoryName = document.getElementById("<%= hdnGridTerritoryName.ClientID %>");
            var txtTerritoryLocation = document.getElementById("<%= txtGridTerritoryLocation.ClientID %>");
            var hdnTerritoryLocation = document.getElementById("<%= hdnGridTerritoryLocation.ClientID %>");
            var txtCountryCode = document.getElementById("<%= txtGridCountryCode.ClientID %>");
            var hdnCountryCode = document.getElementById("<%= hdnGridCountryCode.ClientID %>");
            var ddlTerritoryType = document.getElementById("<%= ddlGridTerritoryType.ClientID %>");
            var hdnTerritoryType = document.getElementById("<%= hdnGridTerritoryType.ClientID %>");

            txtTerritoryName.value = hdnTerritoryName.value;
            txtTerritoryLocation.value = hdnTerritoryLocation.value;
            txtCountryCode.value = hdnCountryCode.value;
            ddlTerritoryType.value = hdnTerritoryType.value;
            ValidatorValidate(document.getElementById("<%= rfGridTerritoryName.ClientID %>"));
            ValidatorValidate(document.getElementById("<%= rfGridTerritoryLocation.ClientID %>"));
            ValidatorValidate(document.getElementById("<%= cvGridCountryCode.ClientID %>"));
            ValidatorValidate(document.getElementById("<%= rfGridCountryCode.ClientID %>"));
            ValidatorValidate(document.getElementById("<%= rfGridTerritoryType.ClientID %>"));
            return false;

        }

        function CompareTerritoryCodeChange() {
            if (!Page_ClientValidate("valUpdateTerritoryGroup")) {
                Page_BlockSubmit = false;
                return false;
            }
            else {
                var txtTerritoryName = document.getElementById("<%= txtGridTerritoryName.ClientID %>");
                var hdnTerritoryName = document.getElementById("<%= hdnGridTerritoryName.ClientID %>");
                var txtTerritoryLocation = document.getElementById("<%= txtGridTerritoryLocation.ClientID %>");
                var hdnTerritoryLocation = document.getElementById("<%= hdnGridTerritoryLocation.ClientID %>");
                var txtCountryCode = document.getElementById("<%= txtGridCountryCode.ClientID %>");
                var hdnCountryCode = document.getElementById("<%= hdnGridCountryCode.ClientID %>");
                var ddlTerritoryType = document.getElementById("<%= ddlGridTerritoryType.ClientID %>");
                var hdnTerritoryType = document.getElementById("<%= hdnGridTerritoryType.ClientID %>");

                txtTerritoryName.value = hdnTerritoryName.value;
                txtTerritoryLocation.value = hdnTerritoryLocation.value;
                txtCountryCode.value = hdnCountryCode.value;
                ddlTerritoryType.value = hdnTerritoryType.value;

                if (hdnTerritoryName != txtTerritoryName || hdnTerritoryLocation != txtTerritoryLocation
                    || hdnCountryCode != txtCountryCode || hdnTerritoryType != ddlTerritoryType) {
                    return true;
                }
                else {
                    DisplayMessagePopup("No changes made to save!");
                    return false;
                }
            }

        }

        function CloseAddTerritoryPopUp() {
            document.getElementById("<%= txtTerritoryCode.ClientID %>").value = "";
            document.getElementById("<%= txtTerritoryName.ClientID %>").value = "";
            document.getElementById("<%= txtTerritoryLocation.ClientID %>").value = "";
            document.getElementById("<%= txtCountryCode.ClientID %>").value = "";
            document.getElementById("<%= ddlTerritoryType.ClientID %>").value = "-";
            document.getElementById("<%= rfvtxtTerritoryCode.ClientID %>").style.display = "none";
            document.getElementById("<%= rftxtTerritoryName.ClientID %>").style.display = "none";
            document.getElementById("<%= rftxtTerritoryLocation.ClientID %>").style.display = "none";
            document.getElementById("<%= rftxtCountryCode.ClientID %>").style.display = "none";
            document.getElementById("<%= cvCountryCode.ClientID %>").style.display = "none";
            document.getElementById("<%= rfvddlTerritoryType.ClientID %>").style.display = "none";
            var popup = $find('<%= mpeAddTerritoryCode.ClientID %>');
            if (popup != null) {
                popup.hide();
            }
            return false;
        }

        //Validate any unsaved data on browser window close/refresh
        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        function WarnOnUnSavedData() {
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            if (isExceptionRaised != "Y" && IsDataChanged()) {
                return warningMsgOnUnSavedData;
            }
        }
        window.onbeforeunload = WarnOnUnSavedData;

        function IsDataChanged() {
            if (IsDataModified()) {
                return true;
            }
            else {
                return false;
            }
        }

        //Check if data changed
        function IsDataModified() {
            if ((document.getElementById("<%=txtTerritorySearch.ClientID %>").value != "" || document.getElementById("<%=txtTerritoryLocSearch.ClientID %>").value != "")
                && document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value == "Y") {
                var txtTerritoryName = document.getElementById("<%=txtGridTerritoryName.ClientID %>").value;
                var txtTerritoryLocation = document.getElementById("<%=txtGridTerritoryLocation.ClientID %>").value;
                var txtCountryCode = document.getElementById("<%=txtGridCountryCode.ClientID %>").value;
                var ddlTerritoryType = document.getElementById("<%=ddlGridTerritoryType.ClientID %>").value;

                var hdnTerritoryName = document.getElementById("<%=hdnGridTerritoryName.ClientID %>").value;
                var hdnTerritoryLocation = document.getElementById("<%=hdnGridTerritoryLocation.ClientID %>").value;
                var hdnCountryCode = document.getElementById("<%=hdnGridCountryCode.ClientID %>").value;
                var hdnTerritoryType = document.getElementById("<%=hdnGridTerritoryType.ClientID %>").value;

                if (hdnTerritoryName != txtTerritoryName || hdnTerritoryLocation != txtTerritoryLocation
                    || hdnCountryCode != txtCountryCode || hdnTerritoryType != ddlTerritoryType) {
                    return true;
                }

                return false;
            }
        }

        function ValidateUnsavedData(button) {            
            if (IsDataChanged()) {                
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
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
            var hdnButtonSelection = document.getElementById("<%=hdnButtonSelection.ClientID %>").value;
            var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
            if (warnPopup != null) {
                warnPopup.hide();
            }

            if (hdnButtonSelection == "AddNewTerritory") {
                document.getElementById("<%= txtTerritoryCode.ClientID %>").focus()
                var popup = $find('<%= mpeAddTerritoryCode.ClientID %>');
                if (popup != null) {
                    popup.show();
                }
                return false;
            }

            return true;
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
                                    TERRITORY SEARCH
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="8%" class="identifierLable_large_bold">Territory</td>
                    <td width="24%">
                        <asp:TextBox ID="txtTerritorySearch" runat="server" Width="99%" CssClass="identifierLable" Onfocus="if (!ValidateUnsavedData('TerritorySearch')) { return false;};"
                            OnTextChanged="txtTerritorySearch_TextChanged" AutoPostBack="true" TabIndex="100"></asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="tbweTerritorySearch" runat="server"
                            TargetControlID="txtTerritorySearch"
                            WatermarkText="Search by territory code/name"
                            WatermarkCssClass="watermarked" />
                        <ajaxToolkit:AutoCompleteExtender ID="territorySearchFilterExtender" runat="server"
                            ServiceMethod="FuzzyTerritorySearchSellerGrpList"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtTerritorySearch"
                            FirstRowSelected="true"
                            OnClientPopulating="territorySearchListPopulating"
                            OnClientPopulated="territorySearchListPopulated"
                            OnClientHidden="territorySearchListPopulated"
                            OnClientItemSelected="territorySearchItemSelected"
                            CompletionListElementID="autocompleteDropDownPanel1" />
                        <asp:Panel ID="autocompleteDropDownPanel1" runat="server" CssClass="identifierLable" />
                    </td>
                    <td width="3%" align="left">
                        <asp:ImageButton ID="fuzzyTerritorySearch" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                            ToolTip="Search Territory " OnClientClick="if (!ValidateUnsavedData('fuzzyTerritorySearch')) { return false;};"
                            OnClick="fuzzyTerritorySearch_Click" CssClass="FuzzySearch_Button" />
                    </td>
                    <td width="24%">
                        <asp:TextBox ID="txtTerritoryLocSearch" runat="server" Width="99%" CssClass="identifierLable" Onfocus="if (!ValidateUnsavedData('TerritoryLocSearch')) { return false;};"
                            OnTextChanged="txtTerritoryLocSearch_TextChanged" AutoPostBack="true" TabIndex="101"></asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="tbweTerritoryLocSearch" runat="server"
                            TargetControlID="txtTerritoryLocSearch"
                            WatermarkText="Search by territory location"
                            WatermarkCssClass="watermarked" />
                        <ajaxToolkit:AutoCompleteExtender ID="territortLocSearchFilterExtender" runat="server"
                            ServiceMethod="FuzzyTerritorySearchSellerGrpList"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtTerritoryLocSearch"
                            FirstRowSelected="true"
                            OnClientPopulating="territoryLocSearchListPopulating"
                            OnClientPopulated="territoryLocSearchListPopulated"
                            OnClientHidden="territoryLocSearchListPopulated"
                            OnClientItemSelected="territoryLocItemSelected"
                            CompletionListElementID="autocompleteDropDownPanel2" />
                        <asp:Panel ID="autocompleteDropDownPanel2" runat="server" CssClass="identifierLable" />
                    </td>
                    <td align="left" width="3%">
                        <asp:ImageButton ID="fuzzyTerritoryLocSearch" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                            ToolTip="Search Territory " OnClick="fuzzyTerritoryLocSearch_Click" OnClientClick="if (!ValidateUnsavedData('fuzzyTerritoryLocSearch')) { return false;};"
                            CssClass="FuzzySearch_Button" />
                    </td>
                    <td align="right">
                        <asp:Button ID="btnAddTerritories" runat="server" CssClass="ButtonStyle" onkeydown="return OnTabPress();"
                            OnClientClick="if (!OpenAddTerritoryPopup('AddTerritories')) { return false;};"
                            OnClick="btnAddTerritories_Click" Text="Add Territory to Groups" UseSubmitBehavior="false" TabIndex="104" Width="39.2%" />
                    </td>
                </tr>
                <tr>
                    <td colspan="6"></td>
                    <td align="right">
                        <asp:Button ID="btnAddNewTerritory" runat="server" CssClass="ButtonStyle" Text="Add Territory Code" Width="39.2%" OnClientClick="if (!OpenAddTerritoryPopup('AddNewTerritory')) { return false;};" />
                    </td>
                </tr>
                <tr>
                    <td colspan="7">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td colspan="7" id="tdGrid" runat="server">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="10%"></td>
                                <td width="50%" valign="top">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <table width="98%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="14%" class="gridHeaderStyle_1row">Territory</td>
                                                        <td width="20%" class="gridHeaderStyle_1row">Territory Name</td>
                                                        <td width="25%" class="gridHeaderStyle_1row">Territory Location</td>
                                                        <td width="15%" class="gridHeaderStyle_1row">Country Code</td>
                                                        <td width="20%" class="gridHeaderStyle_1row">Territory Type</td>
                                                        <td width="6%" class="gridHeaderStyle_1row">&nbsp</td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <table width="98%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="14%" class="gridItemStyle_Center_Align">
                                                            <asp:Label ID="lblTerritoryCode" runat="server" Text="" CssClass="identifierLable"></asp:Label>

                                                        </td>
                                                        <td width="20%" class="gridItemStyle_Left_Align">
                                                            <asp:HiddenField ID="hdnGridTerritoryName" runat="server"></asp:HiddenField>
                                                            <asp:TextBox ID="txtGridTerritoryName" runat="server" Width="80%" CssClass="gridTextField" MaxLength="200" Style="text-transform: uppercase"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfGridTerritoryName" ControlToValidate="txtGridTerritoryName" ValidationGroup="valUpdateTerritoryGroup"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter territory name" Display="Dynamic"></asp:RequiredFieldValidator>

                                                        </td>
                                                        <td width="25%" class="gridItemStyle_Left_Align">
                                                            <asp:HiddenField ID="hdnGridTerritoryLocation" runat="server"></asp:HiddenField>
                                                            <asp:TextBox ID="txtGridTerritoryLocation" runat="server" Width="80%" CssClass="gridTextField" MaxLength="40" Style="text-transform: uppercase"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfGridTerritoryLocation" ControlToValidate="txtGridTerritoryLocation" ValidationGroup="valUpdateTerritoryGroup"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter territory location" Display="Dynamic"></asp:RequiredFieldValidator>

                                                        </td>
                                                        <td width="15%" class="gridItemStyle_Left_Align">
                                                            <asp:HiddenField ID="hdnGridCountryCode" runat="server"></asp:HiddenField>
                                                            <asp:TextBox ID="txtGridCountryCode" runat="server" Width="75%" Text='<%#Bind("country_code")%>' CssClass="gridTextField" MaxLength="2" Style="text-transform: uppercase"></asp:TextBox>
                                                            <asp:CustomValidator ID="cvGridCountryCode" runat="server" ValidationGroup="valUpdateTerritoryGroup" CssClass="requiredFieldValidator" Display="Dynamic"
                                                                OnServerValidate="valGridCountryCode_ServerValidate" ErrorMessage="*" ToolTip="Please enter valid Country code"></asp:CustomValidator>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfGridCountryCode" ControlToValidate="txtGridCountryCode" ValidationGroup="valUpdateTerritoryGroup"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter country code" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td width="20%" class="gridItemStyle_Left_Align">

                                                            <asp:HiddenField ID="hdnGridTerritoryType" runat="server"></asp:HiddenField>
                                                            <asp:DropDownList ID="ddlGridTerritoryType" runat="server" CssClass="ddlStyle" Width="80%" />
                                                            <asp:RequiredFieldValidator runat="server" ID="rfGridTerritoryType" ControlToValidate="ddlGridTerritoryType" ValidationGroup="valUpdateTerritoryGroup" InitialValue="-"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please select territory type" Display="Dynamic"></asp:RequiredFieldValidator>

                                                        </td>
                                                        <td width="6%" class="gridItemStyle_Left_Align">
                                                            <table width="100%" style="float: right; table-layout: fixed">
                                                                <tr style="float: right">
                                                                    <td align="right" style="float: right" width="50%">
                                                                        <asp:ImageButton ID="imgBtnSave" runat="server" TabIndex="106" ImageUrl="../Images/save.png" ValidationGroup="valUpdateTerritoryGroup" OnClientClick="return CompareTerritoryCodeChange();" OnClick="imgBtnTerritoryUpdate_Click" />
                                                                    </td>
                                                                    <td align="right" style="float: right" width="50%">
                                                                        <asp:ImageButton ID="imgBtnCancel" runat="server" TabIndex="107" ImageUrl="../Images/cancel_row3.png" OnClientClick="return UndoTerritoryGroup();"
                                                                            ToolTip="Cancel" />
                                                                    </td>
                                                                </tr>
                                                            </table>
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
                                                <asp:Panel ID="PnlTerritoryGroup" runat="server" ScrollBars="Auto" Width="60%">
                                                    <asp:GridView ID="gvTerritoryGroup" runat="server" AutoGenerateColumns="False" Width="95.6%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found for the selected territory." OnRowDataBound="gvTerritoryGroup_RowDataBound" AllowSorting="true" OnSorting="gvTerritoryGroup_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Territory Group" SortExpression="seller_group_code">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblTerritoryGroupCode" runat="server" Text='<%# Bind("seller_group_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="30%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Territory Group Name" SortExpression="group_name">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblTerritoryGroupName" runat="server" Text='<%# Bind("group_name") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="70%" />
                                                            </asp:TemplateField>
                                                            <%--<asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Owner" SortExpression="OwnerCode">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblTerritotyGroupLocation" runat="server" Text='<%# Bind("group_location") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="40%" />
                                                            </asp:TemplateField>--%>
                                                        </Columns>
                                                        <EmptyDataRowStyle CssClass="gridEmptyDataRowStyle" />
                                                    </asp:GridView>
                                                </asp:Panel>
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
                                <td colspan="3">
                                    <table width="100%">
                                        <tr>
                                            <td width="2%"></td>
                                            <td width="8%" class="identifierLable_large_bold"></td>
                                            <td width="24%"></td>
                                            <td width="3%" align="left"></td>
                                            <td width="24%"></td>
                                            <td width="3%" align="left"></td>
                                            <td></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

            <asp:Button ID="dummySave" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeInsertGroup" runat="server" PopupControlID="pnlInsertGroup" TargetControlID="dummySave"
                CancelControlID="btnNo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlInsertGroup" runat="server" align="center" Width="30%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="top: 100px; z-index: 1; display: none;">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="Label2" runat="server" Text="Add Territory To Groups" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMessage" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td width="10%"></td>
                                    <td class="identifierLable_large_bold" align="left">New Territory</td>
                                    <td></td>
                                    <td width="10%"></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td width="75%">
                                        <asp:TextBox ID="txtNewTerritory" runat="server" Width="99%" CssClass="identifierLable"
                                            OnTextChanged="txtNewTerritory_TextChanged" AutoPostBack="true" TabIndex="102"></asp:TextBox>
                                        <ajaxToolkit:TextBoxWatermarkExtender ID="tbweNewTerritory" runat="server"
                                            TargetControlID="txtNewTerritory"
                                            WatermarkText="Search by territory code/name"
                                            WatermarkCssClass="watermarked" />
                                        <ajaxToolkit:AutoCompleteExtender ID="newTerritorySearchFilterExtender" runat="server"
                                            ServiceMethod="FuzzyTerritorySearchSellerGrpList"
                                            ServicePath="~/Services/FuzzySearch.asmx"
                                            MinimumPrefixLength="1"
                                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                            TargetControlID="txtNewTerritory"
                                            FirstRowSelected="true"
                                            OnClientPopulating="newTerritoryListPopulating"
                                            OnClientPopulated="newTerritoryListPopulated"
                                            OnClientHidden="newTerritoryListPopulated"
                                            OnClientItemSelected="NewterritorySearchItemSelected"
                                            CompletionListElementID="autocompleteDropDownPanel3" />
                                        <asp:Panel ID="autocompleteDropDownPanel3" runat="server" CssClass="identifierLable" />
                                    </td>
                                    <td align="left">
                                        <asp:ImageButton ID="fuzzySearchNewTerritory" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                                            ToolTip="Search Territory " OnClick="fuzzySearchNewTerritory_Click" CssClass="FuzzySearch_Button" />
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:TextBox ID="txtNewTerritoryLoc" runat="server" Width="99%" CssClass="identifierLable"
                                            OnTextChanged="txtNewTerritoryLoc_TextChanged" AutoPostBack="true" TabIndex="103"></asp:TextBox>
                                        <ajaxToolkit:TextBoxWatermarkExtender ID="tbweNewTerritoryLoc" runat="server"
                                            TargetControlID="txtNewTerritoryLoc"
                                            WatermarkText="Search by territory location"
                                            WatermarkCssClass="watermarked" />
                                        <ajaxToolkit:AutoCompleteExtender ID="newTerritoryLocFilterExtender" runat="server"
                                            ServiceMethod="FuzzyTerritorySearchSellerGrpList"
                                            ServicePath="~/Services/FuzzySearch.asmx"
                                            MinimumPrefixLength="1"
                                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                            TargetControlID="txtNewTerritoryLoc"
                                            FirstRowSelected="true"
                                            OnClientPopulating="newTerritoryLocListPopulating"
                                            OnClientPopulated="newTerritoryLocListPopulated"
                                            OnClientHidden="newTerritoryLocListPopulated"
                                            OnClientItemSelected="territoryNewLocItemSelected"
                                            CompletionListElementID="autocompleteDropDownPanel4" />
                                        <asp:Panel ID="autocompleteDropDownPanel4" runat="server" CssClass="identifierLable" />
                                    </td>
                                    <td align="left">
                                        <asp:ImageButton ID="ImageButton1" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                                            ToolTip="Search Territory " OnClick="fuzzySearchNewTerritoryLoc_Click" CssClass="FuzzySearch_Button" />
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <table width="100%" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td width="10%"></td>
                                                <td align="left" width="50%" style="padding-left: 2px;">
                                                    <asp:Button ID="btnAdd" runat="server" Text="Add Territory to Groups" CssClass="ButtonStyle"
                                                        OnClick="btnAdd_Click" Width="99%" />
                                                </td>
                                                <td align="left" style="padding-left: 2px;">
                                                    <asp:Button ID="btnNo" runat="server" Text="Cancel" CssClass="ButtonStyle" />
                                                </td>
                                                <td width="10%"></td>
                                            </tr>
                                        </table>

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
            <asp:Panel ID="pnlFuzzySearch" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="identifierLable">Complete Search List
                                    </td>
                                    <td align="right" style="vertical-align: top;">
                                        <asp:ImageButton ID="btnCloseFuzzySearchPopup" ImageUrl="../Images/CloseIcon.png" runat="server" OnClick="btnCloseFuzzySearchPopup_Click" Style="cursor: pointer" />
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



            <%--popup to Add Territory Code--%>
            <asp:Button ID="dummyBtnAddTerritoryCode" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeAddTerritoryCode" runat="server" PopupControlID="pnlAddTerritoryCode" TargetControlID="dummyBtnAddTerritoryCode"
                CancelControlID="btnCloseAddTerritoryPopup" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlAddTerritoryCode" runat="server" align="center" Width="30%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="identifierLable" align="center">New Territory Code
                                    </td>
                                    <td align="right" style="vertical-align: top;">
                                        <asp:ImageButton ID="btnCloseAddTerritoryPopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" OnClientClick="return CloseAddTerritoryPopUp();" />
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
                                    <td width="5%"></td>
                                    <td width="35%" class="identifierLable_large_bold" align="left">Territory Code</td>
                                    <td width="55%">
                                        <asp:TextBox ID="txtTerritoryCode" runat="server" Width="95%" CssClass="textboxStyle" Style="text-transform: uppercase"
                                            MaxLength="5"></asp:TextBox>
                                    </td>
                                    <td align="left">
                                        <asp:RequiredFieldValidator runat="server" ID="rfvtxtTerritoryCode" ControlToValidate="txtTerritoryCode" ValidationGroup="valAddTerritoryGroup"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter territory code" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold" align="left">Territory Name</td>
                                    <td>
                                        <asp:TextBox ID="txtTerritoryName" runat="server" Width="95%" CssClass="textboxStyle" MaxLength="200" Style="text-transform: uppercase"></asp:TextBox>
                                    </td>
                                    <td align="left">
                                        <asp:RequiredFieldValidator runat="server" ID="rftxtTerritoryName" ControlToValidate="txtTerritoryName" ValidationGroup="valAddTerritoryGroup"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter territory name" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold" align="left">Territory Location</td>
                                    <td>
                                        <asp:TextBox ID="txtTerritoryLocation" runat="server" Width="95%" CssClass="textboxStyle" MaxLength="40" Style="text-transform: uppercase"></asp:TextBox>
                                    </td>
                                    <td align="left">
                                        <asp:RequiredFieldValidator runat="server" ID="rftxtTerritoryLocation" ControlToValidate="txtTerritoryLocation" ValidationGroup="valAddTerritoryGroup"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter territory location" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold" align="left">Country Code</td>
                                    <td>
                                        <asp:TextBox ID="txtCountryCode" runat="server" Width="95%" CssClass="textboxStyle" MaxLength="2" Style="text-transform: uppercase"></asp:TextBox>

                                    </td>
                                    <td align="left">
                                        <asp:RequiredFieldValidator runat="server" ID="rftxtCountryCode" ControlToValidate="txtCountryCode" ValidationGroup="valAddTerritoryGroup"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter country code" Display="Dynamic"></asp:RequiredFieldValidator>

                                        <asp:CustomValidator ID="cvCountryCode" runat="server" ValidationGroup="valAddTerritoryGroup" CssClass="requiredFieldValidator" Display="Dynamic"
                                            OnServerValidate="valCountryCode_ServerValidate" ErrorMessage="*" ToolTip="Please enter valid Country code"></asp:CustomValidator>

                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold" align="left">Territory Type</td>
                                    <td>
                                        <asp:DropDownList ID="ddlTerritoryType" runat="server" CssClass="ddlStyle" Width="97%" />
                                    </td>
                                    <td align="left">
                                        <asp:RequiredFieldValidator runat="server" ID="rfvddlTerritoryType" ControlToValidate="ddlTerritoryType" ValidationGroup="valAddTerritoryGroup" InitialValue="-"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please select territory type" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <table align="center">
                                            <tr>
                                                <td align="right" width="75%">
                                                    <asp:Button ID="btnAddTerritory" runat="server" Text="Add New Territory" CssClass="ButtonStyle" OnClick="btnAddTerritory_Click"
                                                        ValidationGroup="valAddTerritoryGroup" />
                                                </td>
                                                <td width="1%"></td>
                                                <td align="left">
                                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="ButtonStyle" OnClientClick="return CloseAddTerritoryPopUp();" CausesValidation="false" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>


            <msg:MsgControl ID="msgView" runat="server" />
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
                                        <asp:Button ID="btnUnSavedDataExit" runat="server" Text="Exit" CssClass="ButtonStyle" Width="30%" OnClientClick="if (!OnUnSavedDataExit()) { return false;};"
                                            OnClick="btnUnSavedDataExit_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:HiddenField ID="hdnGridPnlTerritoryHeight" runat="server" Value="0" />
            <asp:HiddenField ID="hdnGridPnlGroupHeight" runat="server" Value="0" />
            <asp:HiddenField ID="hdnSearchListItemSelected" runat="server" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" Value="" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
            <asp:HiddenField ID="hdnDataChanged" runat="server" Value="N" />
            <asp:HiddenField ID="hdnButtonSelection" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

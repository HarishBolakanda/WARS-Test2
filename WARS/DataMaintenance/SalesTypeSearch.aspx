<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SalesTypeSearch.aspx.cs" Inherits="WARS.SalesTypeSearch" MasterPageFile="~/MasterPage.Master"
    Title="WARS - SalesTypeSearch" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //to open salestype group screen in same tab
        function OpenSalestypeGroupScreen() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../DataMaintenance/SalesTypeGroupMaintenance.aspx', '_self');
                return false;
            }
            else {
                var win = window.open('../DataMaintenance/SalesTypeGroupMaintenance.aspx', '_self');
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
                        <asp:Button ID="btnSalestypeGroup" runat="server" CssClass="LinkButtonStyle" Text="Sales Type Group Maintenance"
                            OnClientClick="if (!OpenSalestypeGroupScreen()) { return false;};"
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
            var gridPanelSalestypeHeight = windowHeight * 0.04;
            var gridPanelGroupHeight = windowHeight * 0.45;

            document.getElementById("<%=hdnGridPnlSalesTypeHeight.ClientID %>").innerText = gridPanelSalestypeHeight;
            document.getElementById("<%=hdnGridPnlGroupHeight.ClientID %>").innerText = gridPanelGroupHeight;
        }

        //grid panel height adjustment functioanlity - ends

        //Salestype auto populate search functionalities
        function salestypeSearchListPopulating() {
            txtRoy = document.getElementById("<%= txtSalestypeSearch.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
        }

        function salestypeSearchListPopulated() {
            txtRoy = document.getElementById("<%= txtSalestypeSearch.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function salestypeSearchItemSelected(sender, args) {
            var salestypeSrchVal = args.get_value();
            if (salestypeSrchVal == 'No results found') {
                document.getElementById("<%= txtSalestypeSearch.ClientID %>").value = "";
            }
            else {
                document.getElementById('<%=btnHdnSalesTypeSearch.ClientID%>').click();
            }
        }

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }
        function OpenAddSalesTypePopup(button) {
            if (IsDataChanged()) {
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
                return false;
            }

            document.getElementById("<%= txtSalesTypeCode.ClientID %>").focus()
            var popup = $find('<%= mpeAddSalesTypeCode.ClientID %>');
            if (popup != null) {
                popup.show();
            }
            return false;
        }

        function UndoSalesTypeGroup() {
            var txtSalesTypeName = document.getElementById("<%= txtGridSalesTypeName.ClientID %>");
            var hdnSalesTypeName = document.getElementById("<%= hdnGridSalesTypeName.ClientID %>");
            var ddlSalesTypeType = document.getElementById("<%= ddlGridSalesTypeType.ClientID %>");
            var hdnSalesTypeType = document.getElementById("<%= hdnGridSalesTypeType.ClientID %>");
            var txtEscalationProrata = document.getElementById("<%= txtGridEscalationProrata.ClientID %>");
            var hdnEscalationProrata = document.getElementById("<%= hdnGridEscalationProrata.ClientID %>");

            txtSalesTypeName.value = hdnSalesTypeName.value;
            ddlSalesTypeType.value = hdnSalesTypeType.value;
            txtEscalationProrata.value = hdnEscalationProrata.value;
            ValidatorValidate(document.getElementById("<%= rfGridSalesTypeName.ClientID %>"));
            ValidatorValidate(document.getElementById("<%= rfGridSalesTypeType.ClientID %>"));
            return false;

        }

        function ComputeSalesTypeChange() {
            if (!Page_ClientValidate("valUpdateSalesTypeGroup")) {
                Page_BlockSubmit = false;
                return false;
            }
            else {
                var txtSalesTypeName = document.getElementById("<%= txtGridSalesTypeName.ClientID %>").value;
                var hdnSalesTypeName = document.getElementById("<%= hdnGridSalesTypeName.ClientID %>").value;
                var ddlSalesTypeType = document.getElementById("<%= ddlGridSalesTypeType.ClientID %>").value;
                var hdnSalesTypeType = document.getElementById("<%= hdnGridSalesTypeType.ClientID %>").value;
                var txtEscalationProrata = document.getElementById("<%= txtGridEscalationProrata.ClientID %>").value;
                var hdnEscalationProrata = document.getElementById("<%= hdnGridEscalationProrata.ClientID %>").value;

                if (hdnSalesTypeName != txtSalesTypeName || hdnSalesTypeType != ddlSalesTypeType || hdnEscalationProrata != txtEscalationProrata) {
                    return true;
                }

                else {
                    DisplayMessagePopup("No changes made to save!");
                    return false;
                }
            }
        }

        function CloseAddSalesTypePopUp() {
            document.getElementById("<%= txtSalesTypeCode.ClientID %>").value = "";
            document.getElementById("<%= txtSalesTypeName.ClientID %>").value = "";
            document.getElementById("<%= ddlSalesTypeType.ClientID %>").value = "-";
            document.getElementById("<%= txtEscalationProrata.ClientID %>").value = "";
            document.getElementById("<%= rfvtxtSalesTypeCode.ClientID %>").style.display = "none";
            document.getElementById("<%= rftxtSalesTypeName.ClientID %>").style.display = "none";
            document.getElementById("<%= rfvddlSalesTypeType.ClientID %>").style.display = "none";

            var popup = $find('<%= mpeAddSalesTypeCode.ClientID %>');
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

        //Check if data is changed
        function IsDataModified() {
            if (document.getElementById("<%=txtSalestypeSearch.ClientID %>").value != "") {
                var txtGridSalesTypeName = document.getElementById("<%=txtGridSalesTypeName.ClientID %>").value;
                var ddlGridSalesTypeType = document.getElementById("<%=ddlGridSalesTypeType.ClientID %>").value;
                var txtGridEscalationProrata = document.getElementById("<%=txtGridEscalationProrata.ClientID %>").value;

                var hdnGridSalesTypeName = document.getElementById("<%=hdnGridSalesTypeName.ClientID %>").value;
                var hdnGridSalesTypeType = document.getElementById("<%=hdnGridSalesTypeType.ClientID %>").value;
                var hdnGridEscalationProrata = document.getElementById("<%=hdnGridEscalationProrata.ClientID %>").value;

                if (txtGridSalesTypeName != hdnGridSalesTypeName || ddlGridSalesTypeType != hdnGridSalesTypeType || txtGridEscalationProrata != hdnGridEscalationProrata) {
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

            if (hdnButtonSelection == "AddSalesType") {
                document.getElementById("<%= txtSalesTypeCode.ClientID %>").focus()
                var popup = $find('<%= mpeAddSalesTypeCode.ClientID %>');
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
                                    SALES TYPE SEARCH
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="4%"></td>
                    <td width="6%" class="identifierLable_large_bold">Sales Type </td>
                    <td width="20%">
                        <asp:TextBox ID="txtSalestypeSearch" runat="server" Width="99%" CssClass="identifierLable" TabIndex="100" Onfocus="if (!ValidateUnsavedData('SalesTypeSearch')) { return false;};"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="salestypeSearchFilterExtender" runat="server"
                            ServiceMethod="FuzzySearchPriceGroupListTypeC"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtSalestypeSearch"
                            FirstRowSelected="true"
                            OnClientPopulating="salestypeSearchListPopulating"
                            OnClientPopulated="salestypeSearchListPopulated"
                            OnClientHidden="salestypeSearchListPopulated"
                            OnClientItemSelected="salestypeSearchItemSelected"
                            CompletionListElementID="autocompleteDropDownPanel1" />
                        <asp:Panel ID="autocompleteDropDownPanel1" runat="server" CssClass="identifierLable" />
                    </td>
                    <td width="3%" align="left">
                        <asp:ImageButton ID="fuzzySalestypeSearch" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                            OnClientClick="if (!ValidateUnsavedData('fuzzySalestypeSearch')) { return false;};"
                            ToolTip="Search Salestype " OnClick="fuzzySalestypeSearch_Click" CssClass="FuzzySearch_Button" />
                    </td>
                    <td align="right">
                        <asp:Button ID="btnAddNewSalesType" runat="server" CssClass="ButtonStyle" Text="Add Sales Type" Width="22%" OnClientClick="if (!OpenAddSalesTypePopup('AddSalesType')) { return false;};" TabIndex="106" />
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
                                                        <td width="20%" class="gridHeaderStyle_1row">Sales Type</td>
                                                        <td width="35%" class="gridHeaderStyle_1row">Description</td>
                                                        <td width="25%" class="gridHeaderStyle_1row">Sales Type-Type</td>
                                                        <td width="12%" class="gridHeaderStyle_1row">Escalation ProRata</td>
                                                        <td width="8%" class="gridHeaderStyle_1row">&nbsp</td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <table width="98%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="20%" class="gridItemStyle_Center_Align">
                                                            <asp:Label ID="lblSalesTypeCode" runat="server" Text="" CssClass="identifierLable"></asp:Label>

                                                        </td>
                                                        <td width="35%" class="gridItemStyle_Center_Align">
                                                            <asp:HiddenField ID="hdnGridSalesTypeName" runat="server"></asp:HiddenField>
                                                            <asp:TextBox ID="txtGridSalesTypeName" runat="server" Width="80%" CssClass="gridTextField" MaxLength="200" Style="text-transform: uppercase" TabIndex="101"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfGridSalesTypeName" ControlToValidate="txtGridSalesTypeName" ValidationGroup="valUpdateSalesTypeGroup"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter sales type name" Display="Dynamic"></asp:RequiredFieldValidator>

                                                        </td>
                                                        <td width="25%" class="gridItemStyle_Center_Align">

                                                            <asp:HiddenField ID="hdnGridSalesTypeType" runat="server"></asp:HiddenField>
                                                            <asp:DropDownList ID="ddlGridSalesTypeType" runat="server" CssClass="ddlStyle" Width="80%" TabIndex="102" />
                                                            <asp:RequiredFieldValidator runat="server" ID="rfGridSalesTypeType" ControlToValidate="ddlGridSalesTypeType" ValidationGroup="valUpdateSalesTypeGroup" InitialValue="-"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please select sales type" Display="Dynamic"></asp:RequiredFieldValidator>

                                                        </td>
                                                        <td width="12%" class="gridItemStyle_Center_Align">
                                                            <asp:HiddenField ID="hdnGridEscalationProrata" runat="server"></asp:HiddenField>
                                                            <asp:TextBox ID="txtGridEscalationProrata" align="Center" runat="server" Width="80%" CssClass="gridTextField" MaxLength="40" Style="text-transform: uppercase" TabIndex="103"></asp:TextBox>

                                                        </td>
                                                        <td width="8%" class="gridItemStyle_Center_Align">
                                                            <table width="100%" style="float: right; table-layout: fixed">
                                                                <tr style="float: right">
                                                                    <td align="right" style="float: right" width="50%">
                                                                        <asp:ImageButton ID="imgBtnSave" runat="server" TabIndex="104" ImageUrl="../Images/save.png" ValidationGroup="valUpdateSalesTypeGroup" OnClientClick="return ComputeSalesTypeChange();" OnClick="imgBtnSalesTypeUpdate_Click" />
                                                                    </td>
                                                                    <td align="right" style="float: right" width="50%">
                                                                        <asp:ImageButton ID="imgBtnCancel" runat="server" TabIndex="105" ImageUrl="../Images/cancel_row3.png" OnClientClick="return UndoSalesTypeGroup();"
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
                                                <asp:Panel ID="PnlSalestypeGroup" runat="server" ScrollBars="Auto" Width="58%">
                                                    <asp:GridView ID="gvSalestypeGroup" runat="server" AutoGenerateColumns="False" Width="93%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." OnRowDataBound="gvSalestypeGroup_RowDataBound" AllowSorting="true" OnSorting="gvSalestypeGroup_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Sales Type Group" SortExpression="price_group_code">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSalestypeGroupCode" runat="server" Text='<%# Bind("price_group_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="36.9%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Sales Type Group Desc" SortExpression="price_name">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSalestypeGroupName" runat="server" Text='<%# Bind("price_name") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="63.1%" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <EmptyDataRowStyle CssClass="gridEmptyDataRowStyle" />
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td width="45%"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>


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

            <%--popup to Add Sales Type Code--%>
            <asp:Button ID="dummyBtnAddSalesTypeCode" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeAddSalesTypeCode" runat="server" PopupControlID="pnlAddSalesTypeCode" TargetControlID="dummyBtnAddSalesTypeCode"
                CancelControlID="btnCloseAddSalesTypePopup" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlAddSalesTypeCode" runat="server" align="center" Width="30%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td width="95%" class="identifierLable" align="center">New Sales Type
                                    </td>
                                    <td width="5%" align="right" style="vertical-align: top;">
                                        <asp:ImageButton ID="btnCloseAddSalesTypePopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" OnClientClick="CloseAddSalesTypePopUp();" />
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
                                    <td width="35%" class="identifierLable_large_bold" align="left">Sales Type</td>
                                    <td width="55%">
                                        <asp:TextBox ID="txtSalesTypeCode" runat="server" Width="95%" CssClass="textboxStyle" Style="text-transform: uppercase"
                                            MaxLength="3"></asp:TextBox>
                                    </td>
                                    <td align="left">
                                        <asp:RequiredFieldValidator runat="server" ID="rfvtxtSalesTypeCode" ControlToValidate="txtSalesTypeCode" ValidationGroup="valAddSalesTypeGroup"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Sales Type code" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold" align="left">Sales Type Name</td>
                                    <td>
                                        <asp:TextBox ID="txtSalesTypeName" runat="server" Width="95%" CssClass="textboxStyle" MaxLength="200" Style="text-transform: uppercase"></asp:TextBox>
                                    </td>
                                    <td align="left">
                                        <asp:RequiredFieldValidator runat="server" ID="rftxtSalesTypeName" ControlToValidate="txtSalesTypeName" ValidationGroup="valAddSalesTypeGroup"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Sales Type name" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold" align="left">Sales Type Type</td>
                                    <td>
                                        <asp:DropDownList ID="ddlSalesTypeType" runat="server" CssClass="ddlStyle" Width="97%" />
                                    </td>
                                    <td align="left">
                                        <asp:RequiredFieldValidator runat="server" ID="rfvddlSalesTypeType" ControlToValidate="ddlSalesTypeType" ValidationGroup="valAddSalesTypeGroup" InitialValue="-"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please select Sales type" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold" align="left">Escalation Prorata </td>
                                    <td>
                                        <asp:TextBox ID="txtEscalationProrata" runat="server" Width="95%" CssClass="textboxStyle"></asp:TextBox>
                                    </td>
                                    <td align="left"></td>
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
                                                    <asp:Button ID="BtnAddSalesType" runat="server" Text="Add New Sales Type" CssClass="ButtonStyle" OnClick="BtnAddSalesType_Click"
                                                        ValidationGroup="valAddSalesTypeGroup" />
                                                </td>
                                                <td width="1%"></td>
                                                <td align="left">
                                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="ButtonStyle" OnClientClick="return CloseAddSalesTypePopUp();" CausesValidation="false" />
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
                                        <asp:Button ID="btnUnSavedDataExit" runat="server" Text="Exit" CssClass="ButtonStyle" Width="30%" OnClientClick="if (!OnUnSavedDataExit()) { return false;};" OnClick="btnUnSavedDataExit_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>



            <asp:HiddenField ID="hdnGridPnlSalesTypeHeight" runat="server" Value="0" />
            <asp:HiddenField ID="hdnGridPnlGroupHeight" runat="server" Value="0" />
            <asp:Button ID="btnHdnSalesTypeSearch" runat="server" Style="display: none;" OnClick="btnHdnSalesTypeSearch_Click" CausesValidation="false" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnButtonSelection" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>


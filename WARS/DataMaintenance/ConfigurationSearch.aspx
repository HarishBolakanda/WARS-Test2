<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfigurationSearch.aspx.cs" Inherits="WARS.ConfigurationSearch" MasterPageFile="~/MasterPage.Master"
    Title="WARS - ConfigurationSearch" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //to open Configuration group screen in same tab
        function OpenConfigGroupScreen() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../DataMaintenance/ConfigurationGrouping.aspx', '_self');
                return false;
            }
            else {
                var win = window.open('../DataMaintenance/ConfigurationGrouping.aspx', '_self');
                win.focus();
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

    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnConfigurationGroupMaint" runat="server" Text="Configuration Group Maintenance"
                            CssClass="LinkButtonStyle" Width="98%" OnClientClick="if (!OpenConfigGroupScreen()) { return false;};" UseSubmitBehavior="false" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderBody">

    <script type="text/javascript">

        var xPos, yPos;
        var scrollTopIn;
        var scrollTopOut
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
            var gridPanelConfigHeight = windowHeight * 0.04;
            var gridPanelGroupHeight = windowHeight * 0.52;

            document.getElementById("<%=hdnGridPnlConfigurationHeight.ClientID %>").innerText = gridPanelConfigHeight;
            document.getElementById("<%=hdnGridPnlGroupHeight.ClientID %>").innerText = gridPanelGroupHeight;
        }

        //grid panel height adjustment functioanlity - ends    


        //Auto populate search functionalities

        function configurationListPopulating() {
            txtRoy = document.getElementById("<%= txtConfiguration.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnconfSearchSelected.ClientID %>").value = "N";
        }

        function configurationListPopulated() {
            txtRoy = document.getElementById("<%= txtConfiguration.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function configListItemSelected(sender, args) {
            var confSrchVal = args.get_value();
            if (confSrchVal == 'No results found') {
                document.getElementById("<%= txtConfiguration.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnconfSearchSelected.ClientID %>").value = "Y";
            }
        }

        function OpenAddConfigTypePopup(button) {
            if (IsDataChanged()) {
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
                return false;
            }

            document.getElementById("<%= txtConfigTypeCode.ClientID %>").focus()
            var popup = $find('<%= mpeAddConfigTypeCode.ClientID %>');
            if (popup != null) {
                popup.show();
            }
            return false;
        }

        function UndoConfigTypeGroup() {
            var txtConfigTypeName = document.getElementById("<%= txtGridConfigTypeName.ClientID %>");
            var hdnConfigTypeName = document.getElementById("<%= hdnGridConfigTypeName.ClientID %>");
            var ddlConfigType = document.getElementById("<%= ddlGridConfigType.ClientID %>");
            var hdnConfigType = document.getElementById("<%= hdnGridConfigType.ClientID %>");

            txtConfigTypeName.value = hdnConfigTypeName.value;
            ddlConfigType.value = hdnConfigType.value;
            ValidatorValidate(document.getElementById("<%= rfGridConfigTypeName.ClientID %>"));
            ValidatorValidate(document.getElementById("<%= rfGridConfigType.ClientID %>"));
            return false;

        }

        function ComputeConfigTypeChange() {
            if (!Page_ClientValidate("valUpdateConfigTypeGroup")) {
                Page_BlockSubmit = false;
                return false;
            }
            else {
                var txtConfigTypeName = document.getElementById("<%= txtGridConfigTypeName.ClientID %>").value;
                var hdnConfigTypeName = document.getElementById("<%= hdnGridConfigTypeName.ClientID %>").value;
                var ddlConfigType = document.getElementById("<%= ddlGridConfigType.ClientID %>").value;
                var hdnConfigType = document.getElementById("<%= hdnGridConfigType.ClientID %>").value;


                if (hdnConfigTypeName != txtConfigTypeName || hdnConfigType != ddlConfigType) {
                    return true;
                }
                else {
                    DisplayMessagePopup("No changes made to save!");
                    return false;
                }
            }
        }

        function CloseAddConfigTypePopUp() {
            document.getElementById("<%= txtConfigTypeCode.ClientID %>").value = "";
            document.getElementById("<%= txtConfigTypeName.ClientID %>").value = "";
            document.getElementById("<%= ddlConfigType.ClientID %>").value = "-";
            document.getElementById("<%= rfvtxtConfigTypeCode.ClientID %>").style.display = "none";
            document.getElementById("<%= rftxtConfigTypeName.ClientID %>").style.display = "none";
            document.getElementById("<%= rfvddlConfigType.ClientID %>").style.display = "none";

            var popup = $find('<%= mpeAddConfigTypeCode.ClientID %>');
            if (popup != null) {
                popup.hide();
            }
            return false;
        }

        //=============== End

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
            //debugger;
            if (document.getElementById("<%=txtConfiguration.ClientID %>").value != "") {
                var txtGridConfigTypeName = document.getElementById("<%=txtGridConfigTypeName.ClientID %>").value;
                var ddlGridConfigType = document.getElementById("<%=ddlGridConfigType.ClientID %>").value;

                var hdnGridConfigTypeName = document.getElementById("<%=hdnGridConfigTypeName.ClientID %>").value;
                var hdnGridConfigType = document.getElementById("<%=hdnGridConfigType.ClientID %>").value;

                if (txtGridConfigTypeName != hdnGridConfigTypeName || ddlGridConfigType != hdnGridConfigType) {
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
            hdnButtonSelection = document.getElementById("<%=hdnButtonSelection.ClientID %>").value;
            var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
            if (warnPopup != null) {
                warnPopup.hide();
            }
            
            if (hdnButtonSelection == "NewConfigType")
            {
                var popup = $find('<%= mpeAddConfigTypeCode.ClientID %>');
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
                                    CONFIGURATION SEARCH
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="7%" class="identifierLable_large_bold">Configuration </td>
                    <td width="25%">
                        <table width="100%">
                            <tr>
                                <td width="72%">
                                    <asp:TextBox ID="txtConfiguration" runat="server" Width="98%" CssClass="identifierLable" Onfocus="if (!ValidateUnsavedData('ConfigSearch')) { return false;};"
                                        OnTextChanged="txtConfiguration_TextChanged" AutoPostBack="true" TabIndex="100"></asp:TextBox>
                                    <ajaxToolkit:AutoCompleteExtender ID="configurationFilterExtender" runat="server"
                                        ServiceMethod="FuzzySearchConfigGroupListTypeC"
                                        ServicePath="~/Services/FuzzySearch.asmx"
                                        MinimumPrefixLength="1"
                                        CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                        TargetControlID="txtConfiguration"
                                        FirstRowSelected="true"
                                        OnClientPopulating="configurationListPopulating"
                                        OnClientPopulated="configurationListPopulated"
                                        OnClientHidden="configurationListPopulated"
                                        OnClientItemSelected="configListItemSelected"
                                        CompletionListElementID="autocompleteDropDownPanel1" />
                                    <asp:Panel ID="autocompleteDropDownPanel1" runat="server" CssClass="identifierLable" />
                                </td>
                                <td width="10%">
                                    <asp:ImageButton ID="fuzzySearchConfig" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"  
                                        OnClientClick="if (!ValidateUnsavedData('fuzzySearchConfig')) { return false;};"                                      
                                        OnClick="fuzzySearchConfig_Click" ToolTip="Search configuration" CssClass="FuzzySearch_Button" Width="17px" />
                                </td>
                                <td width="18%"></td>
                            </tr>
                        </table>

                    </td>
                    <td width="2%"></td>
                    <td align="right">
                        <asp:Button ID="btnAddNewConfigType" runat="server" CssClass="ButtonStyle" Text="Add Configuration" Width="23%" 
                            TabIndex="105" OnClientClick="if (!OpenAddConfigTypePopup('NewConfigType')) { return false;};" />                         
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
                                                <table width="95%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="35%" class="gridHeaderStyle_1row">Configuration</td>
                                                        <td width="30%" class="gridHeaderStyle_1row">Description</td>
                                                        <td width="25%" class="gridHeaderStyle_1row">Configuration Type</td>
                                                        <td width="10%" class="gridHeaderStyle_1row">&nbsp</td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <table width="95%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="35%" class="gridItemStyle_Center_Align">
                                                            <asp:Label ID="lblConfigTypeCode" runat="server" Text="" CssClass="identifierLable"></asp:Label>

                                                        </td>
                                                        <td width="30%" class="gridItemStyle_Center_Align">
                                                            <asp:HiddenField ID="hdnGridConfigTypeName" runat="server"></asp:HiddenField>
                                                            <asp:TextBox ID="txtGridConfigTypeName" runat="server" Width="80%" CssClass="gridTextField" MaxLength="50" Style="text-transform: uppercase" TabIndex="101"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfGridConfigTypeName" ControlToValidate="txtGridConfigTypeName" ValidationGroup="valUpdateConfigTypeGroup"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter configuration name" Display="Dynamic"></asp:RequiredFieldValidator>

                                                        </td>
                                                        <td width="25%" class="gridItemStyle_Center_Align">

                                                            <asp:HiddenField ID="hdnGridConfigType" runat="server"></asp:HiddenField>
                                                            <asp:DropDownList ID="ddlGridConfigType" runat="server" CssClass="ddlStyle" TabIndex="102" Width="80%" />
                                                            <asp:RequiredFieldValidator runat="server" ID="rfGridConfigType" ControlToValidate="ddlGridConfigType" ValidationGroup="valUpdateConfigTypeGroup" InitialValue="-"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please select configuration type" Display="Dynamic"></asp:RequiredFieldValidator>

                                                        </td>
                                                        <td width="10%" class="gridItemStyle_Center_Align">
                                                            <table width="100%" style="float: right; table-layout: fixed">
                                                                <tr style="float: right">
                                                                    <td align="right" style="float: right" width="50%">
                                                                        <asp:ImageButton ID="imgBtnSave" runat="server" TabIndex="103" ImageUrl="../Images/save.png" ValidationGroup="valUpdateConfigTypeGroup" OnClientClick="return ComputeConfigTypeChange();" OnClick="imgBtnConfigUpdate_Click" />
                                                                    </td>
                                                                    <td align="right" style="float: right" width="50%">
                                                                        <asp:ImageButton ID="imgBtnCancel" runat="server" TabIndex="104" ImageUrl="../Images/cancel_row3.png" OnClientClick="return UndoConfigTypeGroup();"
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
                                                <asp:Panel ID="PnlConfigurationGroup" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvConfigurationGroup" runat="server" AutoGenerateColumns="False" Width="33%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found for the selected Configuration." OnRowDataBound="gvConfigurationGroup_RowDataBound" AllowSorting="true" OnSorting="gvConfigurationGroup_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Configuration Groups" SortExpression="config_group_code">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblConfigurationGroupCode" runat="server" Text='<%# Bind("config_group_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="33%" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <EmptyDataRowStyle CssClass="gridEmptyDataRowStyle" />
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td width="63%"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <asp:UpdateProgress ID="progressBarPageLevel" runat="server" AssociatedUpdatePanelID="updPnlPageLevel" DisplayAfter="100">
                <ProgressTemplate>
                    <div id="Search" style="font-weight: bold; color: Black">
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


            <%--popup to Add New Configuration Group Code--%>
            <asp:Button ID="dummyBtnAddConfigTypeCode" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeAddConfigTypeCode" runat="server" PopupControlID="pnlAddConfigTypeCode" TargetControlID="dummyBtnAddConfigTypeCode"
                CancelControlID="btnCloseAddConfigTypePopup" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlAddConfigTypeCode" runat="server" align="center" Width="30%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td width="95%" class="identifierLable" align="center">New Configuration
                                    </td>
                                    <td width="5%" align="right" style="vertical-align: top;">
                                        <asp:ImageButton ID="btnCloseAddConfigTypePopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" OnClientClick=" return CloseAddConfigTypePopUp();" />
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
                                    <td width="35%" class="identifierLable_large_bold" align="left">Configuration</td>
                                    <td width="55%">
                                        <asp:TextBox ID="txtConfigTypeCode" runat="server" Width="95%" CssClass="textboxStyle" Style="text-transform: uppercase"
                                            MaxLength="10"></asp:TextBox>
                                    </td>
                                    <td align="left">
                                        <asp:RequiredFieldValidator runat="server" ID="rfvtxtConfigTypeCode" ControlToValidate="txtConfigTypeCode" ValidationGroup="valAddConfigTypeGroup"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Configuration code" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold" align="left">Configuration Name</td>
                                    <td>
                                        <asp:TextBox ID="txtConfigTypeName" runat="server" Width="95%" CssClass="textboxStyle" MaxLength="50" Style="text-transform: uppercase"></asp:TextBox>
                                    </td>
                                    <td align="left">
                                        <asp:RequiredFieldValidator runat="server" ID="rftxtConfigTypeName" ControlToValidate="txtConfigTypeName" ValidationGroup="valAddConfigTypeGroup"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter Configuration name" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold" align="left">Configuration Type</td>
                                    <td>
                                        <asp:DropDownList ID="ddlConfigType" runat="server" CssClass="ddlStyle" Width="97%" />
                                    </td>
                                    <td align="left">
                                        <asp:RequiredFieldValidator runat="server" ID="rfvddlConfigType" ControlToValidate="ddlConfigType" ValidationGroup="valAddConfigTypeGroup" InitialValue="-"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please select Configuration type" Display="Dynamic"></asp:RequiredFieldValidator>
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
                                                    <asp:Button ID="BtnAddConfigType" runat="server" Text="Add New Configuration" CssClass="ButtonStyle" OnClick="BtnAddConfigType_Click"
                                                        ValidationGroup="valAddConfigTypeGroup" />
                                                </td>
                                                <td width="1%"></td>
                                                <td align="left">
                                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="ButtonStyle" OnClientClick="return CloseAddConfigTypePopUp();" CausesValidation="false" />
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

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlConfigurationHeight" runat="server" />
            <asp:HiddenField ID="hdnGridPnlGroupHeight" runat="server" />
            <asp:HiddenField ID="hdnconfSearchSelected" runat="server" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>            
            <asp:HiddenField ID="hdnButtonSelection" runat="server" />            
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>


<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfigurationGrouping.aspx.cs" Inherits="WARS.ConfigurationGrouping" MasterPageFile="~/MasterPage.Master"
    Title="WARS - ConfigurationGrouping" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //to open Configuration search screen in same tab
        function OpenConfigSearchScreen() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../DataMaintenance/ConfigurationSearch.aspx');

            }
            else {
                var win = window.open('../DataMaintenance/ConfigurationSearch.aspx', '_self');
                win.focus();
            }
        }

    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnSearchConfiguration" runat="server" Text="Configuration Search" CssClass="LinkButtonStyle"
                            Width="98%" UseSubmitBehavior="false" OnClientClick="if (!OpenConfigSearchScreen()) { return false;};"
                            onkeydown="OnTabPress();" />
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
        var scrollTopOut;
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
            var gridPanelHeight = windowHeight * 0.56;
            document.getElementById("<%=PnlGroupIn.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=PnlGroupOut.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGrpInGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
            document.getElementById("<%=hdnGrpOutGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        //grid panel height adjustment functioanlity - ends    


        //Royaltor auto populate search functionalities

        function configurationGroupListPopulating() {
            txtRoy = document.getElementById("<%= txtConfigurationGroup.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnconfGrpSearchSelected.ClientID %>").value = "N";
        }

        function configurationGroupListPopulated() {
            txtRoy = document.getElementById("<%= txtConfigurationGroup.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function configGrpListItemSelected(sender, args) {
            var confGrpSrchVal = args.get_value();
            if (confGrpSrchVal == 'No results found') {
                document.getElementById("<%= txtConfigurationGroup.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnconfGrpSearchSelected.ClientID %>").value = "Y";
            }

        }

        function configurationGroupOutListPopulating() {
            txtRoy = document.getElementById("<%= txtGroupOutBox.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
        }

        function configurationGroupOutListPopulated() {
            txtRoy = document.getElementById("<%= txtGroupOutBox.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function configurationGroupInListPopulating() {
            txtRoy = document.getElementById("<%= txtGroupInBox.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
        }

        function configurationGroupInListPopulated() {
            txtRoy = document.getElementById("<%= txtGroupInBox.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }



        //================================End

        function OntxtConfigKeyDownOut() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnConfigSearchOut.ClientID%>').click();
            }
        }

        function OntxtConfigKeyDownIn() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnConfigSearchIn.ClientID%>').click();
            }
        }
        //=============== End

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        //For handeling select all checkboxes fuctionality
        function SelectAllout() {
            var checkcount = 0;
            var gridView = document.getElementById("<%=gvGroupOut.ClientID %>");
            var checkBoxes = gridView.getElementsByTagName("input");
            if (document.getElementById("<%=cbOutSelectAll.ClientID %>").checked == true) {
                document.getElementById("<%= hdnisItemSelectedOut.ClientID %>").value = "Y";
                for (var i = 0; i < checkBoxes.length; i++) {
                    if (checkBoxes[i].type == "checkbox") {
                        checkBoxes[i].checked = true;
                    }
                }
            }
            else {
                document.getElementById("<%= hdnisItemSelectedOut.ClientID %>").value = "N";
                for (var i = 0; i < checkBoxes.length; i++) {
                    if (checkBoxes[i].type == "checkbox") {
                        checkBoxes[i].checked = false;
                    }
                }
            }

        }

        function ToggleSelectAllout() {
            var checkcount = 0;
            var gridView = document.getElementById("<%=gvGroupOut.ClientID %>");
            var checkBoxes = gridView.getElementsByTagName("input");
            for (var i = 0; i < checkBoxes.length; i++) {
                if (checkBoxes[i].type == "checkbox" && checkBoxes[i].checked) {
                    checkcount++;
                }
            }
            var rowscount = gridView.rows.length;

            if (rowscount == checkcount) {
                document.getElementById("<%=cbOutSelectAll.ClientID %>").checked = true;
            }
            else {
                document.getElementById("<%=cbOutSelectAll.ClientID %>").checked = false;
            }

            if (checkcount > 0) {
                document.getElementById("<%= hdnisItemSelectedOut.ClientID %>").value = "Y";
            }
            else {
                document.getElementById("<%= hdnisItemSelectedOut.ClientID %>").value = "N";
            }

        }

        function SelectAllIn() {
            var checkcount = 0;
            var gridView = document.getElementById("<%=gvGroupIn.ClientID %>");
            var checkBoxes = gridView.getElementsByTagName("input");
            if (document.getElementById("<%=cbInSelectAll.ClientID %>").checked == true) {
                document.getElementById("<%= hdnisItemSelectedIn.ClientID %>").value = "Y";
                for (var i = 0; i < checkBoxes.length; i++) {
                    if (checkBoxes[i].type == "checkbox") {
                        checkBoxes[i].checked = true;
                    }
                }
            }
            else {
                document.getElementById("<%= hdnisItemSelectedIn.ClientID %>").value = "N";
                for (var i = 0; i < checkBoxes.length; i++) {
                    if (checkBoxes[i].type == "checkbox") {
                        checkBoxes[i].checked = false;
                    }
                }
            }

        }

        function ToggleSelectAllIn() {
            var checkcount = 0;
            var gridView = document.getElementById("<%=gvGroupIn.ClientID %>");
            var checkBoxes = gridView.getElementsByTagName("input");
            for (var i = 0; i < checkBoxes.length; i++) {
                if (checkBoxes[i].type == "checkbox" && checkBoxes[i].checked) {
                    checkcount++;
                }
            }
            var rowscount = gridView.rows.length;

            if (rowscount == checkcount) {
                document.getElementById("<%=cbInSelectAll.ClientID %>").checked = true;
            }
            else {
                document.getElementById("<%=cbInSelectAll.ClientID %>").checked = false;
            }

            if (checkcount > 0) {
                document.getElementById("<%= hdnisItemSelectedIn.ClientID %>").value = "Y";
                    }
                    else {
                        document.getElementById("<%= hdnisItemSelectedIn.ClientID %>").value = "N";
                    }

                }

                //Show warning while closing the window if changed data not saved 
                function WarnOnUnSavedData() {
                    //Harish 29-08-2018: corrected this
                    //if (document.getElementById("<%=hdnButtonSelection.ClientID %>").value != '') {
            //  document.getElementById("<%=hdnisItemSelectedIn.ClientID %>").value = 'N';
            // document.getElementById("<%=hdnisItemSelectedOut.ClientID %>").value = 'N';
            //}
            //var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            //var isItemSelectedIn = document.getElementById("<%=hdnisItemSelectedIn.ClientID %>").value;
            //var isItemSelectedOut = document.getElementById("<%=hdnisItemSelectedOut.ClientID %>").value;
            //if (isExceptionRaised != "Y" && (isItemSelectedIn == "Y" || isItemSelectedOut == "Y")) {
            //  return "There are some items selected which are not moved. Do you want to proceed?";
            //}

            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            var hdnButtonSelection = document.getElementById("<%=hdnButtonSelection.ClientID %>").value;
            if (isExceptionRaised != "Y" && IsDataChanged() && (hdnButtonSelection == "")) {
                return "There are some items selected which are not moved. Do you want to proceed by discarding the changes?";
            }
        }

        window.onbeforeunload = WarnOnUnSavedData;

        //used to check if any changes to allow navigation to other screen 
        function IsDataChanged() {
            var isItemSelectedIn = document.getElementById("<%=hdnisItemSelectedIn.ClientID %>").value;
            var isItemSelectedOut = document.getElementById("<%=hdnisItemSelectedOut.ClientID %>").value;
            if ((isItemSelectedIn == "Y" || isItemSelectedOut == "Y")) {
                return true;
            }
            else {
                return false;
            }
        }

        //Validate any unsaved data on browser window close/refresh
        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        function ResetHiddenSelection() {
            document.getElementById("<%=hdnButtonSelection.ClientID %>").value = '';
            return false;
        }

        function ConfirmSearch(button) {
            var isItemSelectedIn = document.getElementById("<%=hdnisItemSelectedIn.ClientID %>").value;
            var isItemSelectedOut = document.getElementById("<%=hdnisItemSelectedOut.ClientID %>").value;

            if (button != undefined) {
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
            }
            else {
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = "";
            }

            if ((isItemSelectedIn == "Y" || isItemSelectedOut == "Y")) {
                var popup = $find('<%= mpeConfirmation.ClientID %>');
                if (popup != null) {
                    document.getElementById("<%=lblConfirmMsg.ClientID %>").innerText = "There are some items selected which are not moved. Do you want to proceed by discarding the changes?";
                    popup.show();
                    $get("<%=btnProceed.ClientID%>").focus();
                    return false;
                }
            }
            else {
                return true;
            }
        }

    </script>
    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>

            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="5">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    CONFIGURATION GROUP MAINTENANCE
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="10%" class="identifierLable_large_bold">Configuration Group</td>
                    <td width="25%">
                        <table width="100%">
                            <tr>
                                <td width="90%">
                                    <asp:TextBox ID="txtConfigurationGroup" runat="server" Width="98%" CssClass="identifierLable"
                                        OnTextChanged="txtConfigurationGroup_TextChanged" AutoPostBack="true" TabIndex="100" onfocus="return ConfirmSearch();"></asp:TextBox>
                                    <ajaxToolkit:AutoCompleteExtender ID="configurationGroupFilterExtender" runat="server"
                                        ServiceMethod="FuzzySearchConfigGroupListTypeP"
                                        ServicePath="~/Services/FuzzySearch.asmx"
                                        MinimumPrefixLength="1"
                                        CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                        TargetControlID="txtConfigurationGroup"
                                        FirstRowSelected="true"
                                        OnClientPopulating="configurationGroupListPopulating"
                                        OnClientPopulated="configurationGroupListPopulated"
                                        OnClientHidden="configurationGroupListPopulated"
                                        OnClientItemSelected="configGrpListItemSelected"
                                        CompletionListElementID="autocompleteDropDownPanel1" />
                                    <asp:Panel ID="autocompleteDropDownPanel1" runat="server" CssClass="identifierLable" />
                                </td>
                                <td>
                                    <asp:ImageButton ID="fuzzySearchConfigGrp" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                                        OnClick="fuzzySearchConfigGrp_Click" ToolTip="Search configuration group" CssClass="FuzzySearch_Button" onfocus="return ConfirmSearch('FuzzySearchConfig');" />
                                </td>
                            </tr>
                        </table>

                    </td>
                    <td width="2%"></td>
                    <td align="right" style="padding-right: 2px">
                        <asp:Button ID="btnAddConfigurationGroup" runat="server" Text="Create Configuration Group" CssClass="ButtonStyle"
                            Width="23%" OnClick="btnAddConfigurationGroup_Click" UseSubmitBehavior="false"
                            TabIndex="103" onfocus="return ConfirmSearch('AddConfiguration');" /></td>
                </tr>
                <tr>
                    <td colspan="4"></td>
                    <td align="right" style="padding-right: 2px">
                        <asp:Button ID="btnConfigurationGroupAudit" runat="server" Text="Configuration Group Audit" CssClass="ButtonStyle"
                            Width="23%" OnClick="btnConfigurationGroupAudit_Click" UseSubmitBehavior="false"
                            TabIndex="104" onkeydown="return OnTabPress();" onfocus="return ConfirmSearch('ConfigurationAudit');" /></td>

                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td width="2%"></td>
                    <td align="right">
                        <%--<asp:Button ID="btnSearchConfiguration" runat="server" Text="Configuration Search" CssClass="ButtonStyle" width="20%"  UseSubmitBehavior="false" OnClientClick="OpenConfigSearchScreen();" 
                            TabIndex="104" onkeydown="OnTabPress();"/>--%>
                    </td>

                </tr>
                <tr>
                    <td colspan="5">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="12%"></td>
                                <td width="28%" valign="top">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td colspan="2">
                                                <div class="gridTitle_Bold" style="cursor: none; vertical-align: middle; padding: 1%; width: 93.4%">
                                                    Out of the Group
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="24%" class="identifierLable_large_bold">Configuration</td>
                                            <td width="76%">
                                                <asp:TextBox ID="txtGroupOutBox" runat="server" Width="92.8%" CssClass="identifierLable"
                                                    onkeydown="javascript: OntxtConfigKeyDownOut();" AutoPostBack="true" TabIndex="101"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <table width="95.5%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="19%" class="gridHeaderStyle_1row" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                            onclick="document.getElementById('<%=btnOutCodeSort.ClientID%>').click();">Code</td>
                                                        <td width="74%" class="gridHeaderStyle_1row" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                            onclick="document.getElementById('<%=btnOutDescSort.ClientID%>').click();">Description</td>
                                                        <td width="7%" class="gridHeaderStyle_1row">
                                                            <asp:CheckBox ID="cbOutSelectAll" runat="server" onclick="return SelectAllout();" />
                                                        </td>
                                                    </tr>
                                                </table>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Panel ID="PnlGroupOut" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvGroupOut" runat="server" AutoGenerateColumns="False" Width="95.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." ShowHeader="False" OnDataBound="gvGroupOut_DataBound">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblConfigCode" runat="server" Text='<%# Bind("config_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblConfigDesc" runat="server" Text='<%# Bind("config_name") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="75%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="cbAddConfiguration" runat="server" CssClass="identifierLable" OnClick="return ToggleSelectAllout();" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="5%" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <EmptyDataRowStyle CssClass="gridEmptyDataRowStyle" />
                                                    </asp:GridView>
                                                    <asp:Button ID="btnOutCodeSort" runat="server" Style="display: none;" OnClick="btnOutCodeSort_Click" CausesValidation="false" />
                                                    <asp:Button ID="btnOutDescSort" runat="server" Style="display: none;" OnClick="btnOutDescSort_Click" CausesValidation="false" />
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td width="20%" valign="top">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="100%" align="center">
                                                <asp:ImageButton ID="btnRemoveConfiguration" runat="server" ImageUrl="~/Images/groupOut.png" OnClick="btnRemoveConfiguration_Click"
                                                    ToolTip="Remove configuration from the group" />
                                                &nbsp;
                                                &nbsp;
                                                <asp:ImageButton ID="btnAddConfiguration" runat="server" ImageUrl="~/Images/groupIn.png" OnClick="btnAddConfiguration_Click"
                                                    ToolTip="Add configuration to the group" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td width="28%" valign="top">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td colspan="2">
                                                <div class="gridTitle_Bold" style="cursor: none; vertical-align: middle; padding: 1%; width: 93.4%">
                                                    In the Group
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="24%" class="identifierLable_large_bold">Configuration</td>
                                            <td width="76%">
                                                <asp:TextBox ID="txtGroupInBox" runat="server" Width="92.8%" CssClass="identifierLable"
                                                    onkeydown="javascript: OntxtConfigKeyDownIn();" AutoPostBack="true" TabIndex="102"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <table width="95.5%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="19%" class="gridHeaderStyle_1row" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                            onclick="document.getElementById('<%=btnInCodeSort.ClientID%>').click();">Code</td>
                                                        <td width="74%" class="gridHeaderStyle_1row" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                            onclick="document.getElementById('<%=btnInDescSort.ClientID%>').click();">Description</td>
                                                        <td width="7%" class="gridHeaderStyle_1row">
                                                            <asp:CheckBox ID="cbInSelectAll" runat="server" onclick="return SelectAllIn();" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Panel ID="PnlGroupIn" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvGroupIn" runat="server" AutoGenerateColumns="False" Width="95.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." ShowHeader="False" OnDataBound="gvGroupIn_DataBound">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblConfigCode" runat="server" Text='<%# Bind("config_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblConfigDesc" runat="server" Text='<%# Bind("config_name") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="75%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="cbRemoveConfiguration" runat="server" CssClass="identifierLable" onclick="return ToggleSelectAllIn();" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="5%" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <EmptyDataRowStyle CssClass="gridEmptyDataRowStyle" />
                                                    </asp:GridView>
                                                    <asp:Button ID="btnInCodeSort" runat="server" Style="display: none;" OnClick="btnInCodeSort_Click" CausesValidation="false" />
                                                    <asp:Button ID="btnInDescSort" runat="server" Style="display: none;" OnClick="btnInDescSort_Click" CausesValidation="false" />
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td width="12%"></td>
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

            <%--popup to insert configuration groups--%>
            <asp:Button ID="dummySave" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeInsertGroup" runat="server" PopupControlID="pnlInsertGroup" TargetControlID="dummySave"
                CancelControlID="btnNo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlInsertGroup" runat="server" align="center" Width="30%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="Label2" runat="server" Text="Add Configuration Group" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMessage" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td width="5%"></td>
                                    <td class="identifierLable" width="40%" align="left">Configuration Group Code</td>
                                    <td width="55%" align="left" style="padding-left: 10px;">
                                        <asp:TextBox ID="txtgroupCode" runat="server" Width="90%" CssClass="textboxStyle"
                                            MaxLength="10" ToolTip="Group code upto 10 characters"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rfvGrpCode" ControlToValidate="txtgroupCode" ValidationGroup="addGroup"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter group code" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable" align="left">Configuration Group Name</td>
                                    <td align="left" style="padding-left: 10px;">
                                        <asp:TextBox ID="txtGroupName" runat="server" Width="90%" CssClass="textboxStyle" MaxLength="50" ToolTip="Group name upto 50 characters"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rdvGrpName" ControlToValidate="txtGroupName" ValidationGroup="addGroup"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter group name" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
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
                                                <td align="right" width="49%">
                                                    <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="ButtonStyle"
                                                        OnClick="btnSave_Click" ValidationGroup="addGroup" UseSubmitBehavior="false" />
                                                </td>
                                                <td width="2%"></td>
                                                <td align="left" width="49%">
                                                    <asp:Button ID="btnNo" runat="server" Text="Cancel" CssClass="ButtonStyle" UseSubmitBehavior="false" />
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

            <%--Warning popup--%>
            <asp:Button ID="dummyConfirm" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmation" runat="server" PopupControlID="pnlPopup" TargetControlID="dummyConfirm"
                CancelControlID="btnCancel" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlPopup" runat="server" align="center" Width="25%" CssClass="popupPanel"  Style="z-index: 1; display: none">
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
                            <table width="100%">
                                <tr>
                                    <td width="48%" align="right">
                                        <asp:Button ID="btnProceed" runat="server" Text="Proceed" CssClass="ButtonStyle" OnClick="btnProceed_Click" />
                                    </td>
                                    <td width="4%"></td>
                                    <td width="48%" align="left">
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="ButtonStyle" OnClientClick="ResetHiddenSelection();" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnButtonSelection" runat="server" />
            <asp:HiddenField ID="hdnGrpInGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnGrpOutGridPnlHeight" runat="server" />
            <asp:HiddenField ID="throwWarning" runat="server" Value="N" />
            <asp:HiddenField ID="hdnconfGrpSearchSelected" runat="server" />
            <asp:HiddenField ID="hdnGridSortDirOut" runat="server" />
            <asp:HiddenField ID="hdnGridSortColumnOut" runat="server" />
            <asp:HiddenField ID="hdnGridSortDirIn" runat="server" />
            <asp:HiddenField ID="hdnGridSortColumnIn" runat="server" />
            <asp:HiddenField ID="hdnisItemSelectedIn" runat="server" />
            <asp:HiddenField ID="hdnisItemSelectedOut" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" />
            <asp:Button ID="btnConfigSearchOut" runat="server" Style="display: none;" OnClick="btnConfigSearchOut_Click" CausesValidation="false" />
            <asp:Button ID="btnConfigSearchIn" runat="server" Style="display: none;" OnClick="btnConfigSearchIn_Click" CausesValidation="false" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

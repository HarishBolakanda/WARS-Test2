<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SalesTypeGroupMaintenance.aspx.cs" Inherits="WARS.PriceGroupMaintenance" MasterPageFile="~/MasterPage.Master"
    Title="WARS - SalesTypeGroupMaintenance" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //to open SalesType search screen in same tab
        function OpenSalesTypeSearchScreen() {           
            //debugger;        
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../DataMaintenance/SalesTypeSearch.aspx');
            }
            else {
                var win = window.open('../DataMaintenance/SalesTypeSearch.aspx', '_self');
                win.focus();
            }
        }

    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnSearchSalesType" runat="server" Text="Sales Type Search" CssClass="LinkButtonStyle"
                            Width="98%" UseSubmitBehavior="false" OnClientClick="if (!OpenSalesTypeSearchScreen()) { return false;};"/>
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
            var gridPanelHeight = windowHeight * 0.56;
            document.getElementById("<%=hdnGrpInGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
            document.getElementById("<%=hdnGrpOutGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        //grid panel height adjustment functioanlity - ends    

       
        //Royaltor auto populate search functionalities

        function salesTypeGroupListPopulating() {
            txtRoy = document.getElementById("<%= txtSalesTypeGroup.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidSearch.ClientID %>").value = "N";
        }

        function salesTypeGroupListPopulated() {
            txtRoy = document.getElementById("<%= txtSalesTypeGroup.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function salesTypeGrpListItemSelected(sender, args) {
            var confGrpSrchVal = args.get_value();
            if (confGrpSrchVal == 'No results found') {
                document.getElementById("<%= txtSalesTypeGroup.ClientID %>").value = "";
            }
            else {
                document.getElementById('<%=btnHdnSalesTypeSearch.ClientID%>').click();
            }

        }

        function salesTypeGroupOutListPopulating() {
            txtRoy = document.getElementById("<%= txtGroupOutBox.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
        }

        function salesTypeGroupOutListPopulated() {
            txtRoy = document.getElementById("<%= txtGroupOutBox.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function salesTypeGroupInListPopulating() {
            txtRoy = document.getElementById("<%= txtGroupInBox.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
        }

        function salesTypeGroupInListPopulated() {
            txtRoy = document.getElementById("<%= txtGroupInBox.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }



        //================================End

        function OntxtSalesTypeKeyDownOut() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnSalesTypeSearchOut.ClientID%>').click();
            }
        }

        function OntxtSalesTypeKeyDownIn() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnSalesTypeSearchIn.ClientID%>').click();
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
        //end

        //Show warning while closing the window if changed data not saved 
        function WarnOnUnSavedData() {
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            var isItemSelectedIn = document.getElementById("<%=hdnisItemSelectedIn.ClientID %>").value;
            var isItemSelectedOut = document.getElementById("<%=hdnisItemSelectedOut.ClientID %>").value;
            if (isExceptionRaised != "Y" && (isItemSelectedIn == "Y" || isItemSelectedOut == "Y")) {
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

        function ConfirmSearch() {
            var isItemSelectedIn = document.getElementById("<%=hdnisItemSelectedIn.ClientID %>").value;
            var isItemSelectedOut = document.getElementById("<%=hdnisItemSelectedOut.ClientID %>").value;
            if ((isItemSelectedIn == "Y" || isItemSelectedOut == "Y")) {
                var popup = $find('<%= mpeConfirmation.ClientID %>');
                if (popup != null) {
                    document.getElementById("<%=lblConfirmMsg.ClientID %>").innerText = "There are some items selected which are not moved. Do you want to proceed by discarding the changes?";
                    popup.show();
                    $get("<%=btnDiscard.ClientID%>").focus();
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
                                    SALES TYPE GROUP MAINTENANCE
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="10%" class="identifierLable_large_bold">Sales Type Group</td>
                    <td width="25%">
                        <table width="100%">
                            <tr>
                                <td width="90%">
                                    <asp:TextBox ID="txtSalesTypeGroup" runat="server" Width="98%" CssClass="identifierLable" TabIndex="100" onfocus="return ConfirmSearch();"></asp:TextBox>
                                    <ajaxToolkit:AutoCompleteExtender ID="salesTypeGroupFilterExtender" runat="server"
                                        ServiceMethod="FuzzySearchPriceGroupListTypeP"
                                        ServicePath="~/Services/FuzzySearch.asmx"
                                        MinimumPrefixLength="1"
                                        CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                        TargetControlID="txtSalesTypeGroup"
                                        FirstRowSelected="true"
                                        OnClientPopulating="salesTypeGroupListPopulating"
                                        OnClientPopulated="salesTypeGroupListPopulated"
                                        OnClientHidden="salesTypeGroupListPopulated"
                                        OnClientItemSelected="salesTypeGrpListItemSelected"
                                        CompletionListElementID="autocompleteDropDownPanel1" />
                                    <asp:Panel ID="autocompleteDropDownPanel1" runat="server" CssClass="identifierLable" />
                                </td>
                                <td>
                                    <asp:ImageButton ID="fuzzySearchSalesTypeGrp" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                                        OnClick="fuzzySearchSalesTypeGrp_Click" ToolTip="Search salesType group" CssClass="FuzzySearch_Button" Onfocus="return ConfirmSearch();" />
                                </td>
                            </tr>
                        </table>

                    </td>
                    <td width="2%"></td>
                    <td align="right" style="padding-right: 2px">
                        <asp:Button ID="btnAddSalesTypeGroup" runat="server" Text="Add Sales Type Group" CssClass="ButtonStyle"
                            Width="23%" OnClick="btnAddSalesTypeGroup_Click" UseSubmitBehavior="false" Onfocus="return ConfirmSearch();"
                            TabIndex="103" onkeydown="return OnTabPress();" /></td>

                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                    <td></td>
                    <td></td>
                    <td width="2%"></td>
                    <td align="right"></td>

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
                                            <td width="24%" class="identifierLable_large_bold">Sales Type</td>
                                            <td width="76%">
                                                <asp:TextBox ID="txtGroupOutBox" runat="server" Width="92.8%" CssClass="identifierLable"
                                                    onkeydown="javascript: OntxtSalesTypeKeyDownOut();" AutoPostBack="true" TabIndex="101" Onfocus="return ConfirmSearch();"></asp:TextBox>
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
                                                                    <asp:Label ID="lblSalesTypeCode" runat="server" Text='<%# Bind("price_group_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="19%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSalesTypeDesc" runat="server" Text='<%# Bind("price_name") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="74%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="cbAddSalesType" runat="server" CssClass="identifierLable" OnClick="return ToggleSelectAllout();" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="7%" />
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
                                                <asp:ImageButton ID="btnRemoveSalesType" runat="server" ImageUrl="~/Images/groupOut.png" OnClick="btnRemoveSalesType_Click"
                                                    ToolTip="Remove salesType from the group" />
                                                &nbsp;
                                                &nbsp;
                                                <asp:ImageButton ID="btnAddSalesType" runat="server" ImageUrl="~/Images/groupIn.png" OnClick="btnAddSalesType_Click"
                                                    ToolTip="Add salesType to the group" />
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
                                            <td width="24%" class="identifierLable_large_bold">Sales Type</td>
                                            <td width="76%">
                                                <asp:TextBox ID="txtGroupInBox" runat="server" Width="92.8%" CssClass="identifierLable"
                                                    onkeydown="javascript: OntxtSalesTypeKeyDownIn();" AutoPostBack="true" TabIndex="102" Onfocus="return ConfirmSearch();"></asp:TextBox>
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
                                                                    <asp:Label ID="lblSalesTypeCode" runat="server" Text='<%# Bind("price_group_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="19%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSalesTypeDesc" runat="server" Text='<%# Bind("price_name") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="74%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="cbRemoveSalesType" runat="server" CssClass="identifierLable" onclick="return ToggleSelectAllIn();" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="7%" />
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

            <%--popup to insert salesType groups--%>
            <asp:Button ID="dummySave" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeInsertGroup" runat="server" PopupControlID="pnlInsertGroup" TargetControlID="dummySave"
                CancelControlID="btnNo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlInsertGroup" runat="server" align="center" Width="30%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="Label2" runat="server" Text="Add Sales Type Group" CssClass="identifierLable"></asp:Label>
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
                                    <td class="identifierLable" width="20%" align="left" style="padding-left: 10px;">Sales Type Group Code</td>
                                    <td width="25%" align="left">
                                        <asp:TextBox ID="txtGroupCode" runat="server" Width="90%" CssClass="textboxStyle"
                                            MaxLength="3" ToolTip="Group code upto 3 characters"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rfvGrpCode" ControlToValidate="txtGroupCode" ValidationGroup="addGroup"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter group code" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="identifierLable" width="20%" align="left" style="padding-left: 10px;">Sales Type Group Name</td>
                                    <td align="left">
                                        <asp:TextBox ID="txtGroupDesc" runat="server" Width="90%" CssClass="textboxStyle" MaxLength="100" ToolTip="Group name upto 100 characters"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rdvGrpName" ControlToValidate="txtGroupDesc" ValidationGroup="addGroup"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter group name" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <table>
                                            <tr>
                                                <td align="right" width="49.5%">
                                                    <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="ButtonStyle"
                                                        OnClick="btnSave_Click" ValidationGroup="addGroup" UseSubmitBehavior="false" />
                                                </td>
                                                <td width="1%"></td>
                                                <td align="left">
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
            <asp:Panel ID="pnlFuzzySearch" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
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
            <asp:Panel ID="pnlPopup" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
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
                                        <asp:Button ID="btnDiscard" runat="server" Text="Discard" CssClass="ButtonStyle" OnClick="btnDiscard_Click" />
                                    </td>
                                    <td width="4%"></td>
                                    <td width="48%" align="left">
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnGrpInGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnGrpOutGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnisItemSelectedIn" runat="server" />
            <asp:HiddenField ID="hdnisItemSelectedOut" runat="server" />
            <asp:HiddenField ID="hdnGridSortDirOut" runat="server" />
            <asp:HiddenField ID="hdnGridSortColumnOut" runat="server" />
            <asp:HiddenField ID="hdnGridSortDirIn" runat="server" />
            <asp:HiddenField ID="hdnGridSortColumnIn" runat="server" />
            <asp:HiddenField ID="hdnIsValidSearch" runat="server" Value="N" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" />
            <asp:Button ID="btnHdnSalesTypeSearch" runat="server" Style="display: none;" OnClick="btnHdnSalesTypeSearch_Click" CausesValidation="false" />
            <asp:Button ID="btnSalesTypeSearchOut" runat="server" Style="display: none;" OnClick="btnSalesTypeSearchOut_Click" CausesValidation="false" />
            <asp:Button ID="btnSalesTypeSearchIn" runat="server" Style="display: none;" OnClick="btnSalesTypeSearchIn_Click" CausesValidation="false" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


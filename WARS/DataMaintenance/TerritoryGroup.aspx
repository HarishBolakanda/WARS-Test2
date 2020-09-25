<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TerritoryGroup.aspx.cs" Inherits="WARS.SellerGroup" MasterPageFile="~/MasterPage.Master"
    Title="WARS - SellerGroup" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //to open territory group screen in same tab
        function OpenTerritorySearchScreen() {
            //debugger;        
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../DataMaintenance/TerritorySearch.aspx');              
            }
            else {
                var win = window.open('../DataMaintenance/TerritorySearch.aspx', '_self');
                win.focus();
            }

        }

        //=============== End
    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnSearchTerritory" runat="server" Text="Territory Search" CssClass="LinkButtonStyle"
                            Width="98%" UseSubmitBehavior="false" OnClientClick="if (!OpenTerritorySearchScreen()) { return false;};"
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

            //to maintain scroll position
            //postBackElementID = args.get_postBackElement().id.substring(args.get_postBackElement().id.lastIndexOf("_") + 1);
            postBackElementID = args.get_postBackElement().id;
            if (postBackElementID.lastIndexOf('imgExpand') > 0 || postBackElementID.lastIndexOf('imgCollapse') > 0 || postBackElementID.lastIndexOf('gvGroupOut') > 0 || postBackElementID.lastIndexOf('gvGroupIn') > 0 || postBackElementID.lastIndexOf('updPnlPageLevel') > 0) {
                xPos = sender._scrollPosition.x;
                yPos = sender._scrollPosition.y;

                //hold scroll position on selecting expand/collapse
                var PnlReferenceIn = document.getElementById("<%=PnlGroupIn.ClientID %>");
                scrollTopIn = PnlReferenceIn.scrollTop;

                var PnlReferenceOut = document.getElementById("<%=PnlGroupOut.ClientID %>");
                scrollTopOut = PnlReferenceOut.scrollTop;

            }

        }

        function EndRequestHandler(sender, args) {
            //Hide the modal popup - the update progress
            var popup = $find('<%= mPopupPageLevel.ClientID %>');
            if (popup != null) {
                popup.hide();
            }

            //to maintain scroll position
            postBackElementID = sender._postBackSettings.sourceElement.id;//.substring(sender._postBackSettings.sourceElement.id.lastIndexOf("_") + 1);
            if (postBackElementID.lastIndexOf('imgExpand') > 0 || postBackElementID.lastIndexOf('imgCollapse') > 0 || postBackElementID.lastIndexOf('gvGroupOut') > 0 || postBackElementID.lastIndexOf('gvGroupIn') > 0 || postBackElementID.lastIndexOf('updPnlPageLevel') > 0) {
                window.scrollTo(xPos, yPos);

                //set scroll position on selecting expand/collapse
                var PnlReferenceIn = document.getElementById("<%=PnlGroupIn.ClientID %>");
                PnlReferenceIn.scrollTop = scrollTopIn;

                var PnlReferenceOut = document.getElementById("<%=PnlGroupOut.ClientID %>");
                PnlReferenceOut.scrollTop = scrollTopOut;

            }

         
        }

        //probress bar functionality - ends

        //grid panel height adjustment functioanlity - starts
        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.57;
            document.getElementById("<%=PnlGroupIn.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=PnlGroupOut.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGrpInGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
            document.getElementById("<%=hdnGrpOutGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        //grid panel height adjustment functioanlity - ends    

        
        //Royaltor auto populate search functionalities

        function sellerGroupListPopulating() {
            txtRoy = document.getElementById("<%= txtSellerGroup.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "N";
        }

        function sellerGroupListPopulated() {
            txtRoy = document.getElementById("<%= txtSellerGroup.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function territoryGroupListItemSelected(sender, args) {
            var territoryGrpSrchVal = args.get_value();
            if (territoryGrpSrchVal == 'No results found') {
                document.getElementById("<%= txtSellerGroup.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "Y";
            }

        }
        //------------------------------//

        function sellerGroupOutListPopulating() {
            txtRoy = document.getElementById("<%= txtGroupOutBox.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "N";
        }

        function sellerGroupOutListPopulated() {
            txtRoy = document.getElementById("<%= txtGroupOutBox.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function territoryGroupOutItemSelected(sender, args) {
            var territoryGrpOutSrchVal = args.get_value();
            if (territoryGrpOutSrchVal == 'No results found') {
                document.getElementById("<%= txtGroupOutBox.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "Y";
            }

        }
        //------------------------------//
        function sellerGroupOutLocListPopulating() {
            txtRoy = document.getElementById("<%= txtGroupOutLoc.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "N";
        }

        function sellerGroupOutLocListPopulated() {
            txtRoy = document.getElementById("<%= txtGroupOutLoc.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function territoryGroupOutLocItemSelected(sender, args) {
            var territoryGrpOutLocSrchVal = args.get_value();
            if (territoryGrpOutLocSrchVal == 'No results found') {
                document.getElementById("<%= txtGroupOutLoc.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "Y";
            }

        }
        //------------------------------//
        function sellerGroupInListPopulating() {
            txtRoy = document.getElementById("<%= txtGroupInBox.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "N";
        }

        function sellerGroupInListPopulated() {
            txtRoy = document.getElementById("<%= txtGroupInBox.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function territoryGroupInItemSelected(sender, args) {
            var territoryGrpInSrchVal = args.get_value();
            if (territoryGrpInSrchVal == 'No results found') {
                document.getElementById("<%= txtGroupInBox.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "Y";
            }

        }
        //------------------------------//
        function sellerGroupInLocListPopulating() {
            txtRoy = document.getElementById("<%= txtGroupInLoc.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "N";
        }

        function sellerGroupInLocListPopulated() {
            txtRoy = document.getElementById("<%= txtGroupInLoc.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function territoryGroupInLocItemSelected(sender, args) {
            var territoryGrpInLocSrchVal = args.get_value();
            if (territoryGrpInLocSrchVal == 'No results found') {
                document.getElementById("<%= txtGroupInLoc.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "Y";
            }

        }
        //------------------------------//

        //Show warning while closing the window if changed data not saved 
        function WarnOnUnSavedData() {
            //Harish 29-08-2018: corrected this
            //if (document.getElementById("<%=hdnButtonSelection.ClientID %>").value != '') {
            //  document.getElementById("<%=hdnisItemSelectedIn.ClientID %>").value = 'N';
            //  document.getElementById("<%=hdnisItemSelectedOut.ClientID %>").value = 'N';
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
            if (isItemSelectedIn == "Y" || isItemSelectedOut == "Y") {
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

        //Validate if any location is present both in In & Out grid

        function ConfirmSearch(button) {
            var isItemSelectedIn = document.getElementById("<%=hdnisItemSelectedIn.ClientID %>").value;
            var isItemSelectedOut = document.getElementById("<%=hdnisItemSelectedOut.ClientID %>").value;
            var txtSellerGroup = document.getElementById("<%=txtSellerGroup.ClientID %>").value;
            
            if (button != undefined) {
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
            }
            else {
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = "";
            }

            if (txtSellerGroup.value != '') {
                if (isItemSelectedIn == "Y" || isItemSelectedOut == "Y") {
                    var popup = $find('<%= mpeConfirmation.ClientID %>');
                    if (popup != null) {
                        popup.show();
                        $get("<%=btnCancel.ClientID%>").focus();
                        document.getElementById("<%=lblConfirmMsg.ClientID %>").innerText = "There are some items selected which are not moved. Do you want to proceed?";

                    }
                    return false;
                }
                else {
                    return true;
                }
            }
            else {
                return true;
            }
        }
        //================================End

        //On press of Enter key in search textboxes
        function OnTerritoryNameKeyDownOut() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnTerritoryNameSearchOut.ClientID%>').click();
            }
        }

        function OnTerritoryLocKeyDownOut() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnTerritoryLocSearchOut.ClientID%>').click();
            }
        }

        function OnTerritoryNameKeyDownIn() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnTerritoryNameSearchIn.ClientID%>').click();
            }
        }

        function OnTerritoryLocKeyDownIn() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnTerritoryLocSearchIn.ClientID%>').click();
        }
    }

    //Tab key to remain only on screen fields
    function OnTabPress() {
        if (event.keyCode == 9) {
            document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

    </script>
    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>

            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="6">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    TERRITORY GROUP MAINTENANCE
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="8%" class="identifierLable_large_bold">Territory Group</td>
                    <td width="24%">
                        <asp:TextBox ID="txtSellerGroup" runat="server" Width="99%" CssClass="identifierLable" OnFocus="return ConfirmSearch();"
                            OnTextChanged="txtSellerGroup_TextChanged" AutoPostBack="true" TabIndex="100"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="sellerGroupFilterExtender" runat="server"
                            ServiceMethod="FuzzySearchSellerGroupListTypeP"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtSellerGroup"
                            FirstRowSelected="true"
                            OnClientPopulating="sellerGroupListPopulating"
                            OnClientPopulated="sellerGroupListPopulated"
                            OnClientHidden="sellerGroupListPopulated"
                            OnClientItemSelected="territoryGroupListItemSelected"
                            CompletionListElementID="autocompleteDropDownPanel1" />
                        <asp:Panel ID="autocompleteDropDownPanel1" runat="server" CssClass="identifierLable" />
                    </td>
                    <td width="5%" align="left">
                        <asp:ImageButton ID="fuzzySearchTerritoryGroup" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                            ToolTip="Search Territory Group" OnClick="fuzzySearchTerritoryGroup_Click" OnFocus="return ConfirmSearch('FuzzySearchTerritory');" CssClass="FuzzySearch_Button" />
                    </td>
                    <td align="right" style="padding-right: 0;">
                        <asp:Button ID="btnAddTerritoryGroup" runat="server" Text="Create Territory Group" CssClass="ButtonStyle"
                            Width="18%" OnClick="btnAddTerritoryGroup_Click" UseSubmitBehavior="false" onkeydown="return OnTabPress();" OnFocus="return ConfirmSearch('AddConfiguration');"
                            TabIndex="105" /></td>
                    <td width="2%"></td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td align="right" style="padding-right: 0;"></td>
                    <td align="right" style="padding-right: 0;">
                        <asp:Button ID="btnCheckRoyaltors" runat="server" Text="Check Royaltors" CssClass="ButtonStyle"
                            Width="18%" OnFocus="return ConfirmSearch('CheckRoyaltors');" OnClick="btnCheckRoyaltors_Click" UseSubmitBehavior="false"
                            TabIndex="105" /></td>
                    </td>
                    <td width="2%"></td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td align="right" style="padding-right: 0;"></td>
                    <td align="right" style="padding-right: 0;">
                        <asp:Button ID="Button1" runat="server" Text="Territory Group Audit" CssClass="ButtonStyle"
                            Width="18%" OnClick="btnAudit_Click" UseSubmitBehavior="false" onkeydown="return OnTabPress();" OnFocus="return ConfirmSearch('TerritoryAudit');"
                            TabIndex="106" /></td>
                    </td>
                    <td width="2%"></td>
                </tr>
                <tr>
                    <td colspan="6">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="2%"></td>
                                <td width="35%" valign="top">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td colspan="5">
                                                <div class="gridTitle_Bold" style="cursor: none; vertical-align: middle; padding-left: 1%; width: 94.4%">
                                                    Out of the Group
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="10%" class="identifierLable_large_bold">Territory</td>
                                            <td width="42.5%">
                                                <asp:TextBox ID="txtGroupOutLoc" runat="server" Width="97%" CssClass="identifierLable"
                                                    onkeydown="javascript: OnTerritoryLocKeyDownOut();" AutoPostBack="true" TabIndex="101"></asp:TextBox>
                                                <ajaxToolkit:TextBoxWatermarkExtender ID="tbweGrpLocOut" runat="server"
                                                    TargetControlID="txtGroupOutLoc"
                                                    WatermarkText="Search by territory location"
                                                    WatermarkCssClass="watermarked" />                                               
                                            </td>
                                            <td width="1%">                                               
                                            </td>
                                            <td width="42.5%">
                                                <asp:TextBox ID="txtGroupOutBox" runat="server" Width="97.35%" CssClass="identifierLable"
                                                    onkeydown="javascript: OnTerritoryNameKeyDownOut();" AutoPostBack="true" TabIndex="103"></asp:TextBox>
                                                <ajaxToolkit:TextBoxWatermarkExtender ID="tbweGrpOutBox" runat="server"
                                                    TargetControlID="txtGroupOutBox"
                                                    WatermarkText="Search by territory code/name"
                                                    WatermarkCssClass="watermarked" />                                             
                                            </td>
                                            <td width="4%">                                               
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="5">
                                                <table width="95.5%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="42%" class="gridHeaderStyle_1row" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                            onclick="document.getElementById('<%=btnTerritoryLocOutSort.ClientID%>').click();">Territory Location</td>
                                                        <td width="14%" class="gridHeaderStyle_1row">Territory Code</td>
                                                        <td width="39%" class="gridHeaderStyle_1row">Territory Name</td>
                                                        <td width="5%" class="gridHeaderStyle_1row">
                                                            <asp:CheckBox ID="cbOutSelectAll" runat="server" OnCheckedChanged="cbOutSelectAll_CheckedChanged"
                                                                AutoPostBack="true" />
                                                        </td>
                                                    </tr>
                                                </table>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="5">
                                                <asp:Panel ID="PnlGroupOut" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvGroupOut" runat="server" AutoGenerateColumns="False" Width="95.5%" OnDataBound="gvGroupOut_DataBound"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found for the selected territory group." ShowHeader="False" OnRowDataBound="gvGroupOut_RowDataBound" OnRowCommand="gvGroupOut_RowCommand">
                                                        <%--<SelectedRowStyle BackColor="#99b8fa" Font-Bold="true" />--%>
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="" ItemStyle-Width="1%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                                <ItemTemplate>
                                                                    <asp:ImageButton ID="imgExpand" runat="server" ImageUrl="../Images/Plus.gif" CommandName="Expand" Visible="false"
                                                                        CommandArgument='<%# Container.DataItemIndex %>' />
                                                                    <asp:ImageButton ID="imgCollapse" runat="server" ImageUrl="../Images/Minus.gif" CommandName="Collapse" Visible="false"
                                                                        CommandArgument='<%# Container.DataItemIndex %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSellerLocation" runat="server" Text='<%# Bind("seller_location") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="40%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSellerCode" runat="server" Text='<%# Bind("seller_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="14%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSellerName" runat="server" Text='<%# Bind("seller_name") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="39%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="cbAddSeller" runat="server" CssClass="identifierLable" AutoPostBack="true" OnCheckedChanged="cbAddSeller_CheckedChanged" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="5%" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <EmptyDataRowStyle CssClass="gridEmptyDataRowStyle" />
                                                    </asp:GridView>
                                                    <asp:Button ID="btnTerritoryLocOutSort" runat="server" Style="display: none;" OnClick="btnTerritoryLocOutSort_Click" CausesValidation="false" />
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td width="10%" valign="top">
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
                                                <asp:ImageButton ID="btnRemoveTerritory" runat="server" ImageUrl="~/Images/groupOut.png" OnClick="btnRemoveTerritory_Click"
                                                    ToolTip="Remove territory from the group" />
                                                &nbsp;
                                                &nbsp;
                                                <asp:ImageButton ID="btnAddTerritory" runat="server" ImageUrl="~/Images/groupIn.png" OnClick="btnAddTerritory_Click"
                                                    ToolTip="Add territory to the group" />
                                            </td>
                                        </tr>
                                    </table>

                                </td>
                                <td width="35%" valign="top">

                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td colspan="5">
                                                <div class="gridTitle_Bold" style="cursor: none; vertical-align: middle; padding-left: 1%; width: 94.4%">
                                                    In the Group
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="10%" class="identifierLable_large_bold">Territory</td>
                                            <td width="42.5%">
                                                <asp:TextBox ID="txtGroupInLoc" runat="server" Width="97%" CssClass="identifierLable"
                                                    onkeydown="javascript: OnTerritoryLocKeyDownIn();" AutoPostBack="true" TabIndex="103"></asp:TextBox>
                                                <ajaxToolkit:TextBoxWatermarkExtender ID="tbweGrpInLoc" runat="server"
                                                    TargetControlID="txtGroupInLoc"
                                                    WatermarkText="Search by territory location"
                                                    WatermarkCssClass="watermarked" />                                             
                                            </td>
                                            <td width="1%" align="left">                                            
                                            </td>
                                            <td width="42.5%">
                                                <asp:TextBox ID="txtGroupInBox" runat="server" Width="97.35%" CssClass="identifierLable"
                                                    onkeydown="javascript: OnTerritoryNameKeyDownIn();" AutoPostBack="true" TabIndex="104"></asp:TextBox>
                                                <ajaxToolkit:TextBoxWatermarkExtender ID="tbweGrpIn" runat="server"
                                                    TargetControlID="txtGroupInBox"
                                                    WatermarkText="Search by territory code/name"
                                                    WatermarkCssClass="watermarked" />                                          
                                            </td>
                                            <td width="4%">                                              
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="5">
                                                <table width="95.5%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="42%" class="gridHeaderStyle_1row" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                            onclick="document.getElementById('<%=btnTerritoryLocSort.ClientID%>').click();">Territory Location</td>
                                                        <td width="14%" class="gridHeaderStyle_1row">Territory Code</td>
                                                        <td width="39%" class="gridHeaderStyle_1row">Territory Name</td>
                                                        <td width="5%" class="gridHeaderStyle_1row">
                                                            <asp:CheckBox ID="cbInSelectAll" runat="server" OnCheckedChanged="cbInSelectAll_CheckedChanged"
                                                                AutoPostBack="true" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="5">
                                                <asp:Panel ID="PnlGroupIn" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvGroupIn" runat="server" AutoGenerateColumns="False" Width="95.5%" OnDataBound="gvGroupIn_DataBound"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found for the selected territory group." ShowHeader="False" OnRowDataBound="gvGroupIn_RowDataBound"
                                                        OnRowCommand="gvGroupIn_RowCommand">
                                                        <%--<SelectedRowStyle BackColor="#99b8fa" Font-Bold="true" />--%>
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="" ItemStyle-Width="1%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                                <ItemTemplate>
                                                                    <asp:ImageButton ID="imgExpand" runat="server" ImageUrl="../Images/Plus.gif" CommandName="Expand" Visible="false"
                                                                        CommandArgument='<%# Container.DataItemIndex %>' />
                                                                    <asp:ImageButton ID="imgCollapse" runat="server" ImageUrl="../Images/Minus.gif" CommandName="Collapse" Visible="false"
                                                                        CommandArgument='<%# Container.DataItemIndex %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSellerLocation" runat="server" Text='<%# Bind("seller_location") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="40%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSellerCode" runat="server" Text='<%# Bind("seller_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="14%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSellerName" runat="server" Text='<%# Bind("seller_name") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="39%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="cbRemoveSeller" runat="server" CssClass="identifierLable" AutoPostBack="true" OnCheckedChanged="cbRemoveSeller_CheckedChanged" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="5%" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <EmptyDataRowStyle CssClass="gridEmptyDataRowStyle" />
                                                    </asp:GridView>
                                                    <asp:Button ID="btnTerritoryLocSort" runat="server" Style="display: none;" OnClick="btnTerritoryLocSort_Click" CausesValidation="false" />
                                                </asp:Panel>
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
                        <img src="../Images/InProgress2.gif" alt="" />
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <ajaxToolkit:ModalPopupExtender ID="mPopupPageLevel" runat="server" PopupControlID="progressBarPageLevel" TargetControlID="progressBarPageLevel"
                BackgroundCssClass="progressBar" RepositionMode="RepositionOnWindowResize">
            </ajaxToolkit:ModalPopupExtender>

            <%--Warning popup--%>
            <asp:Button ID="dummyConfirm" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmation" runat="server" PopupControlID="pnlPopup" TargetControlID="dummyConfirm"
                CancelControlID="btnCancel" BackgroundCssClass="popupBox">
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
                                        <asp:Button ID="btnContinue" runat="server" Text="Proceed" CssClass="ButtonStyle"
                                            OnClick="btnContinue_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <%--popup to insert territory groups--%>
            <asp:Button ID="dummySave" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeInsertGroup" runat="server" PopupControlID="pnlInsertGroup" TargetControlID="dummySave"
                CancelControlID="btnNo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlInsertGroup" runat="server" align="center" Width="30%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="Label2" runat="server" Text="Add Territory Group" CssClass="identifierLable"></asp:Label>
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
                                    <td width="10%"></td>
                                    <td class="identifierLable" width="40%" align="left">Territory Group Code</td>
                                    <td width="40%" align="left">
                                        <asp:TextBox ID="txtgroupCode" runat="server" Width="90%" CssClass="textboxStyle"
                                            MaxLength="5" ToolTip="Group code upto 5 characters"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rfvGrpCode" ControlToValidate="txtgroupCode" ValidationGroup="addGroup"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter group code" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                    <td width="10%"></td>
                                    <tr>
                                        <td></td>
                                        <td class="identifierLable" align="left">Territory Group Name</td>
                                        <td align="left">
                                            <asp:TextBox ID="txtGroupName" runat="server" Width="90%" CssClass="textboxStyle" MaxLength="200" ToolTip="Group name upto 200 characters"></asp:TextBox>
                                            <asp:RequiredFieldValidator runat="server" ID="rdvGrpName" ControlToValidate="txtGroupName" ValidationGroup="addGroup"
                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter group name" Display="Dynamic"></asp:RequiredFieldValidator>
                                        </td>
                                        <td></td>
                                    </tr>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="5">
                                        <table>
                                            <tr>
                                                <td align="right" width="49.5%">
                                                    <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="ButtonStyle"
                                                        OnClick="btnSave_Click" ValidationGroup="addGroup" />
                                                </td>
                                                <td width="1%"></td>
                                                <td align="left">
                                                    <asp:Button ID="btnNo" runat="server" Text="Cancel" CssClass="ButtonStyle" />
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


            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnGrpInGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnGrpOutGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnSearchListItemSelected" runat="server" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" Value="" />
            <asp:HiddenField ID="hdnGridSortDirOut" runat="server" />
            <asp:HiddenField ID="hdnGridSortColumnOut" runat="server" />
            <asp:HiddenField ID="hdnGridSortDirIn" runat="server" />
            <asp:HiddenField ID="hdnGridSortColumnIn" runat="server" />
            <asp:HiddenField ID="hdnHearderLoc" runat="server" />
            <asp:HiddenField ID="hdnisItemSelectedIn" runat="server" />
            <asp:HiddenField ID="hdnisItemSelectedOut" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnButtonSelection" runat="server" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" />
            <asp:Button ID="btnTerritoryNameSearchOut" runat="server" Style="display: none;" OnClick="btnTerritoryNameSearchOut_Click" CausesValidation="false" />
            <asp:Button ID="btnTerritoryLocSearchOut" runat="server" Style="display: none;" OnClick="btnTerritoryLocSearchOut_Click" CausesValidation="false" />
            <asp:Button ID="btnTerritoryNameSearchIn" runat="server" Style="display: none;" OnClick="btnTerritoryNameSearchIn_Click" CausesValidation="false" />
            <asp:Button ID="btnTerritoryLocSearchIn" runat="server" Style="display: none;" OnClick="btnTerritoryLocSearchIn_Click" CausesValidation="false" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

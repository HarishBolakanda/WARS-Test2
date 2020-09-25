<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TerritoryGroupRoyaltors.aspx.cs" Inherits="WARS.DataMaintenance.TerritoryGroupRoyaltors" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Royaltors for Territory Group" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //to open territory group screen in same tab
        function OpenContractMaintenanceScreen() {
            var win = window.open('../Contract/RoyaltorSearch.aspx', '_self');
            win.focus();
        }

        //=============== End
    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnContractMaintenance" runat="server" CssClass="LinkButtonStyle" Text="Contract Maintenance"
                            OnClientClick="OpenContractMaintenanceScreen();"
                            UseSubmitBehavior="false" Width="98%" />
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

            document.getElementById("<%=PnlTerritoryGroup.ClientID %>").style.height = gridPanelTerritoryHeight + "px";
            document.getElementById("<%=PnlRoyaltors.ClientID %>").style.height = gridPanelGroupHeight + "px";
            document.getElementById("<%=hdnGridPnlTerritoryHeight.ClientID %>").innerText = gridPanelTerritoryHeight;
            document.getElementById("<%=hdnGridPnlGroupHeight.ClientID %>").innerText = gridPanelGroupHeight;
        }

        //grid panel height adjustment functioanlity - ends
      

        //Territory auto populate search functionalities

        function territorySearchListPopulating() {
            txtRoy = document.getElementById("<%= txtTerritoryGroupSearch.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidSearch.ClientID %>").value = "N";
        }

        function territorySearchListPopulated() {
            txtRoy = document.getElementById("<%= txtTerritoryGroupSearch.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function territorySearchItemSelected(sender, args) {
            var territorySrchVal = args.get_value();
            if (territorySrchVal == 'No results found') {
                document.getElementById("<%= txtTerritoryGroupSearch.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnIsValidSearch.ClientID %>").value = "Y";
                document.getElementById('<%=btnHdnTerritoryGroupSearch.ClientID%>').click();
            }

        }


        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }


        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        function ValidateChanges() {
            eval(this.href);
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
                                    ROYALTORS FOR TERRITORY GROUP
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="8%" class="identifierLable_large_bold">Territory Group</td>
                    <td width="24%">
                        <asp:TextBox ID="txtTerritoryGroupSearch" runat="server" Width="99%" CssClass="identifierLable"
                            TabIndex="100"></asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="tbweTerritorySearch" runat="server"
                            TargetControlID="txtTerritoryGroupSearch"
                            WatermarkText="Territory Group fuzzy search"
                            WatermarkCssClass="watermarked" />
                        <ajaxToolkit:AutoCompleteExtender ID="territorySearchFilterExtender" runat="server"
                            ServiceMethod="FuzzySearchSellerGroupListTypeP"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtTerritoryGroupSearch"
                            FirstRowSelected="true"
                            OnClientPopulating="territorySearchListPopulating"
                            OnClientPopulated="territorySearchListPopulated"
                            OnClientHidden="territorySearchListPopulated"
                            OnClientItemSelected="territorySearchItemSelected"
                            CompletionListElementID="autocompleteDropDownPanel1" />
                        <asp:Panel ID="autocompleteDropDownPanel1" runat="server" CssClass="identifierLable" />
                    </td>
                    <td width="3%" align="left">
                        <asp:ImageButton ID="fuzzyTerritoryGroupSearch" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                            ToolTip="Search Territory " OnClick="fuzzyTerritoryGroupSearch_Click" CssClass="FuzzySearch_Button" />
                    </td>
                    <td width="24%"></td>
                    <td align="left" width="3%"></td>
                    <td align="right">
                        <asp:Button ID="btnTerritoryGroupMaint" runat="server" CssClass="ButtonStyle" onkeydown="return OnTabPress();"
                            OnClick="btnTerritoryGroupMaint_Click" Text="Territory Group Maintenance" UseSubmitBehavior="false" TabIndex="101" Width="39.2%" />
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
                                                <table width="54.5%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="22%" class="gridHeaderStyle_1row">Territory Group</td>
                                                        <td width="78%" class="gridHeaderStyle_1row">Territory Group Name</td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlTerritoryGroup" runat="server" ScrollBars="Auto" Width="57%">
                                                    <asp:GridView ID="gvTerritoryGroup" runat="server" AutoGenerateColumns="False" Width="95.6%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found for the selected territory." ShowHeader="False">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblTerritoryGroupCode" runat="server" Text='<%# Bind("seller_group_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="22%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblTerritoryGroupName" runat="server" Text='<%# Bind("seller_name") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="78%" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <EmptyDataRowStyle CssClass="gridEmptyDataRowStyle" />
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
                                            <td>
                                                <asp:Panel ID="PnlRoyaltors" runat="server" ScrollBars="Auto" Width="82%">
                                                    <asp:GridView ID="gvRoyaltors" runat="server" AutoGenerateColumns="False" Width="96.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found for the selected territory." OnRowDataBound="gvRoyaltors_RowDataBound" AllowSorting="true" OnSorting="gvRoyaltors_Sorting" HeaderStyle-CssClass="FixedHeader"
>
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Royaltor" SortExpression="royaltor_id">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRoyaltor" runat="server" Text='<%# Bind("royaltor_id") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="15%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Royaltor Name" SortExpression="royaltor_name">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRoyaltorName" runat="server" Text='<%# Bind("royaltor_name") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="54%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Rate Type" SortExpression="rate_type">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRateType" runat="server" Text='<%# Bind("rate_type") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="31%" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <EmptyDataRowStyle CssClass="gridEmptyDataRowStyle" />
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:Repeater ID="rptPager" runat="server">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>'
                                                OnClientClick="return ValidateChanges();" ClientIDMode="AutoID" CausesValidation="false" Enabled='<%# Eval("Enabled") %>' OnClick="lnkPage_Click" CssClass="gridPager"> </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <br />
                                </td>
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
            <asp:HiddenField ID="hdnPageNumber" runat="server" Value="" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlTerritoryHeight" runat="server" Value="0" />
            <asp:HiddenField ID="hdnGridPnlGroupHeight" runat="server" Value="0" />
            <asp:HiddenField ID="hdnIsValidSearch" runat="server" Value="N" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" />
              <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
            <asp:Button ID="btnHdnTerritoryGroupSearch" runat="server" Style="display: none;" OnClick="btnHdnTerritoryGroupSearch_Click" CausesValidation="false" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

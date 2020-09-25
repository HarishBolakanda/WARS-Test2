<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MissingParticipants.aspx.cs" Inherits="WARS.MissingParticipants" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Missing Participants " MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //to open Configuration group screen in same tab
        function OpenContractMaintenance() {
            var win = window.open('../Contract/RoyaltorSearch.aspx', '_self');
            win.focus();
        }

        function OpenCatalogueSearch() {
            window.location = '../Participants/CatalogueSearch.aspx?isNewRequest=N';
        }

        //================================End

    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnContractMaintenance" runat="server" Text="Contract Maintenance"
                            CssClass="LinkButtonStyle" Width="98%" OnClientClick="OpenContractMaintenance();" UseSubmitBehavior="false" />
                    </td>
                </tr>
                <tr>
                    <td align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnCatalogueSearch" runat="server" Text="Catalogue Search"
                            CssClass="LinkButtonStyle" Width="98%" OnClientClick="OpenCatalogueSearch();" UseSubmitBehavior="false" />
                    </td>
                </tr>
            </table>
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


        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
            document.getElementById("<%=PnlCatalogueDetails.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        //======================= End

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        //=============== End

        //Validate any unsaved data on browser window close/refresh
        function RedirectToErrorPage() {
            window.location = "../Common/ExceptionPage.aspx";
        }

        //=========Search by Enter key - Starts

        function SearchByEnterKey() {            
            if ((event.keyCode == 13)) {                
                document.getElementById('<%=btnSearch.ClientID%>').click();
            }
        }       

        //=========Search by Enter key - Ends

    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="12">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    MISSING PARTICIPANTS CATALOGUE SEARCH
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="1%"></td>
                    <td width="12%" class="identifierLable_large_bold">Catalogue No.</td>
                    <td width="16%">
                        <asp:TextBox ID="txtCatalogueNo" runat="server" Width="95%" CssClass="textboxStyle" onkeydown="SearchByEnterKey();"
                            TabIndex="100"></asp:TextBox>
                    </td>
                    <td width="1%"></td>
                    <td width="12%" class="identifierLable_large_bold">Title</td>
                    <td width="16%">
                        <asp:TextBox ID="txtTitle" runat="server" Width="95%" CssClass="textboxStyle" onkeydown="SearchByEnterKey();"
                            TabIndex="101"></asp:TextBox>
                    </td>
                    <td width="1%"></td>
                    <td width="8%" class="identifierLable_large_bold">ISRC</td>
                    <td width="10%">
                        <asp:TextBox ID="txtISRC" runat="server" Width="88%" CssClass="textboxStyle" onkeydown="SearchByEnterKey();"
                            TabIndex="102"></asp:TextBox>
                    </td>
                    <td></td>
                    <td align="right" colspan="2">
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="ButtonStyle"
                            Width="98%" UseSubmitBehavior="false" ValidationGroup="valSearch" TabIndex="115" OnClick="btnSearch_Click" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">Artist</td>
                    <td>
                        <asp:TextBox ID="txtArtist" runat="server" Width="95%" CssClass="textboxStyle" onkeydown="SearchByEnterKey();"
                            TabIndex="103"></asp:TextBox>
                    </td>
                    <td></td>
                    <td class="identifierLable_large_bold">Configuration</td>
                    <td>
                        <asp:DropDownList ID="ddlConfiguration" runat="server" Width="97%" CssClass="ddlStyle" onkeydown="SearchByEnterKey();"
                            TabIndex="104">
                        </asp:DropDownList>
                    </td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td colspan="2" align="right">
                        <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="ButtonStyle"
                            Width="98%" UseSubmitBehavior="false" OnClick="btnReset_Click" CausesValidation="false" TabIndex="116" OnKeyDown="OnTabPress()" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">Team Responsibility by Product</td>
                    <td>
                        <asp:DropDownList ID="ddlTeamResponsibility" runat="server" Width="97%" CssClass="ddlStyle" TabIndex="105" onkeydown="SearchByEnterKey();"></asp:DropDownList>
                    </td>
                    <td></td>
                    <td class="identifierLable_large_bold">Manager Responsibility by Product</td>
                    <td>
                        <asp:DropDownList ID="ddlManagerResponsibility" runat="server" Width="97%" CssClass="ddlStyle" TabIndex="106" onkeydown="SearchByEnterKey();"></asp:DropDownList>
                    </td>
                    <td></td>
                    <td class="identifierLable_large_bold">Product Status</td>
                    <td>
                        <asp:DropDownList ID="ddlCatnoStatus" runat="server" Width="91%" CssClass="ddlStyle" TabIndex="107" onkeydown="SearchByEnterKey();"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td ></td>
                    <td class="identifierLable_large_bold">Team Responsibility by Track</td>
                    <td>
                        <asp:DropDownList ID="ddlTeamResponsibilitybyTrackLevel" runat="server" Width="97%" CssClass="ddlStyle" TabIndex="108" onkeydown="SearchByEnterKey();"></asp:DropDownList>
                    </td>
                    <td></td>
                    <td class="identifierLable_large_bold">Manager Responsibility by Track</td>
                    <td>
                        <asp:DropDownList ID="ddlManagerResponsibilitybyTrackLevel" runat="server" Width="97%" CssClass="ddlStyle" TabIndex="109" onkeydown="SearchByEnterKey();"></asp:DropDownList>
                    </td>
                    <td></td>
                    <td class="identifierLable_large_bold">Track Status</td>
                    <td>
                        <asp:DropDownList ID="ddlTrackStatus" runat="server" Width="91%" CssClass="ddlStyle" TabIndex="110" onkeydown="SearchByEnterKey(this.id);"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">Transaction Start Date</td>
                    <td>
                         <asp:DropDownList ID="ddlCompareStartDate" runat="server" Width="20%" TabIndex="110" CssClass="ddlStyle">
                                        <asp:ListItem Text="=" Value="="></asp:ListItem>
                                        <asp:ListItem Text="<=" Value="<="></asp:ListItem>
                                        <asp:ListItem Text=">=" Value=">="></asp:ListItem>
                                    </asp:DropDownList>
                        <asp:TextBox ID="txtStartDate" runat="server" Width="65px" CssClass="textboxStyle" onkeydown="SearchByEnterKey();"
                            TabIndex="111"></asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmeStartDate" runat="server" TargetControlID="txtStartDate"
                            WatermarkText="mm/yyyy" WatermarkCssClass="waterMarkText">
                        </ajaxToolkit:TextBoxWatermarkExtender>
                        <ajaxToolkit:MaskedEditExtender ID="mteStartDate" runat="server"
                            TargetControlID="txtStartDate" Mask="99/9999" AcceptNegative="None"
                            ClearMaskOnLostFocus="false" />
                        <%--<ajaxToolkit:MaskedEditValidator ID="valMaskEditStartDate" runat="server" ControlExtender="mteStartDate" ControlToValidate="txtStartDate"
                            ValidationExpression="^(([0-2][1-9])|(3[0-1]))\/((0[1-9])|(1[0-2]))\/(\d{4})$" CssClass="identifierLable"
                            InvalidValueMessage="*" ToolTip="Please enter valid month and year in DD/MM/YY format" ForeColor="Red">
                        </ajaxToolkit:MaskedEditValidator>--%>
                        <asp:CustomValidator ID="valStartDate" runat="server" ValidationGroup="valSearch" CssClass="requiredFieldValidator"
                            OnServerValidate="valStartDate_ServerValidate" ErrorMessage="*" ToolTip="Please enter a valid date in MM/YYYY format"></asp:CustomValidator>
                    </td>
                    <td></td>
                    <td class="identifierLable_large_bold">Transaction End Date</td>
                    <td>
                         <asp:DropDownList ID="ddlCompareEndDate" runat="server" Width="20%" TabIndex="112" CssClass="ddlStyle">
                                        <asp:ListItem Text="=" Value="="></asp:ListItem>
                                        <asp:ListItem Text="<=" Value="<="></asp:ListItem>
                                        <asp:ListItem Text=">=" Value=">="></asp:ListItem>
                                    </asp:DropDownList>
                        <asp:TextBox ID="txtEndDate" runat="server" Width="65px" CssClass="textboxStyle" onkeydown="SearchByEnterKey();"
                            TabIndex="113"></asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmeEndDate" runat="server" TargetControlID="txtEndDate"
                            WatermarkText="mm/yyyy" WatermarkCssClass="waterMarkText">
                        </ajaxToolkit:TextBoxWatermarkExtender>
                        <ajaxToolkit:MaskedEditExtender ID="mteEndDate" runat="server"
                            TargetControlID="txtEndDate" Mask="99/9999" AcceptNegative="None"
                            ClearMaskOnLostFocus="false" />
                        <asp:CustomValidator ID="valEndDate" runat="server" ValidationGroup="valSearch" CssClass="requiredFieldValidator"
                            OnServerValidate="valEndDate_ServerValidate" ErrorMessage="*" ToolTip="Please enter a valid date in MM/YYYY format"></asp:CustomValidator>
                        <%--<ajaxToolkit:MaskedEditValidator ID="valMaskEditEndDate" runat="server" ControlExtender="mteEndDate" ControlToValidate="txtEndDate"
                            ValidationExpression="^(([0-2][1-9])|(3[0-1]))\/((0[1-9])|(1[0-2]))\/(\d{4})$" CssClass="identifierLable"
                            InvalidValueMessage="*" ToolTip="Please enter valid month and year in DD/MM/YY format" ForeColor="Red">
                        </ajaxToolkit:MaskedEditValidator>--%>
                    </td>
                    <td></td>
                    <td class="identifierLable_large_bold">Value Threshold</td>
                    <td>
                        <asp:TextBox ID="txtValueThreshold" runat="server" Width="88%" CssClass="textboxStyle" onkeydown="SearchByEnterKey();"
                            TabIndex="114"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="revValueThreshold" runat="server" Text="*" ControlToValidate="txtValueThreshold" ValidationGroup="valSearch" EnableClientScript="false"
                            ValidationExpression="^[-+]?\d*\.{0,1}\d+$" CssClass="requiredFieldValidator" ForeColor="Red"
                            ToolTip="Please enter only number" Display="Dynamic"> </asp:RegularExpressionValidator>
                    </td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="10"></td>
                    <td width="11%"></td>
                    <td width="1%"></td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="10" class="table_with_border" runat="server" id="tdData">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="2%">
                                    <br />
                                </td>
                                <td width="96%"></td>
                                <td width="2%"></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlCatalogueDetails" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvCatalogueDetails" runat="server" AutoGenerateColumns="False" Width="98%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle_hover" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." OnRowDataBound="gvCatalogueDetails_RowDataBound" OnRowCommand="gvCatalogueDetails_RowCommand" RowStyle-CssClass="dataRow"
                                                        AllowSorting="true" OnSorting="gvCatalogueDetails_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <%--AllowPaging="true" OnPageIndexChanging="gvCatalogueDetails_PageIndexChanging" PageSize="20">
                                                        <PagerStyle HorizontalAlign="Center" CssClass="pagerStyle" />
                                                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />--%>
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Catalogue No." SortExpression="catno">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblCatNo" runat="server" Text='<%# Bind("catno") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="15%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Artist" SortExpression="artist_name">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblArtistName" runat="server" Text='<%# Bind("artist_name") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Title" SortExpression="catno_title">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblCatnoTitle" runat="server" Text='<%# Bind("catno_title") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Configuration" SortExpression="config_code">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblConfigCode" runat="server" Text='<%# Bind("config_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="9%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="First Transactions" SortExpression="first_recdate">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblFirstRecDate" runat="server" Text='<%# Bind("first_recdate") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Last Transactions" SortExpression="last_recdate">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblLastRecDate" runat="server" Text='<%# Bind("last_recdate") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Product Status" SortExpression="status_desc">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblStatusDesc" runat="server" Text='<%# Bind("status_desc") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Right_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Value" SortExpression="transaction_value">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblTransactionValue" runat="server" Text='<%# Bind("transaction_value") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="lnkBtnDblClk" runat="server" CommandName="dblClk" Text="dblClick" CausesValidation="false"> </asp:LinkButton>
                                                                    <%--<asp:HiddenField ID="hdnTrackListingId" runat="server" Value='<%# Bind("track_listing_id") %>' />--%>
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="hide" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:Repeater ID="rptPager" runat="server">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>'
                                                            Enabled='<%# Eval("Enabled") %>' OnClick="lnkPage_Click" CssClass="gridPager"> </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:Repeater>
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
                                    </table>
                                </td>
                                <td></td>
                            </tr>
                        </table>
                    </td>
                    <td></td>
                </tr>
            </table>

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

            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnIsValidSearch" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsNewRequest" runat="server" Value="Y" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99"></asp:Label>
              <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

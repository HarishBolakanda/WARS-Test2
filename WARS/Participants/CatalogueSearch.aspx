<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="CatalogueSearch.aspx.cs" Inherits="WARS.CatalogueSearch" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Catalogue Search " MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">

        function MissingParticipScreen() {
            window.location = '../Participants/MissingParticipants.aspx?isNewRequest=N';
        }

        function OpenContractMaintenance() {
            var win = window.open('../Contract/RoyaltorSearch.aspx', '_self');
            win.focus();
        }

    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnContractMaintenance" runat="server" Text="Contract Maintenance"
                            CssClass="LinkButtonStyle" Width="98%" OnClientClick="OpenContractMaintenance();" UseSubmitBehavior="false" />
                    </td>
                </tr>
                <tr>

                    <td align="right">
                        <asp:Button ID="btnMissingParticip" runat="server" CssClass="LinkButtonStyle"
                            Text="Missing Participants" UseSubmitBehavior="false" Width="98%" OnClientClick="if (!MissingParticipScreen()) { return false;};" />
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
            var gridPanelHeight = windowHeight * 0.65;
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

        //=============== End

        //=========Search by Enter key - Starts

        function SearchByEnterKey() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnSearch.ClientID%>').click();
            }
        }


        //=========Search by Enter key - Ends

        function OpenUploadPrdListPopUp() {
            var popup = $find('<%= mpeUploadPrdList.ClientID %>');
            if (popup != null) {
                popup.show();

                if (document.getElementById('<%=lblUploadPrdListError.ClientID%>') != null) {
                    document.getElementById('<%=lblUploadPrdListError.ClientID%>').style.display = 'none';
                }

                document.getElementById('<%=txtUploadPrdList.ClientID%>').value = "";
                document.getElementById('<%=txtUploadPrdList.ClientID%>').focus();
            }
            return false;
        }

    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="10">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    CATALOGUE SEARCH
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="1%"></td>
                    <td width="7%" class="identifierLable_large_bold">Catalogue No.</td>
                    <td width="16%">
                        <asp:TextBox ID="txtCatalogueNo" runat="server" Width="95%" CssClass="textboxStyle"
                            TabIndex="100" onkeydown="SearchByEnterKey();"></asp:TextBox>
                    </td>
                    <td width="1%"></td>
                    <td width="7%" class="identifierLable_large_bold">Title</td>
                    <td width="16%">
                        <asp:TextBox ID="txtTitle" runat="server" Width="95%" CssClass="textboxStyle"
                            TabIndex="101" onkeydown="SearchByEnterKey();"></asp:TextBox>
                    </td>
                    <td width="1%"></td>
                    <td width="3%" class="identifierLable_large_bold">ISRC</td>
                    <td width="16%">
                        <asp:TextBox ID="txtISRC" runat="server" Width="95%" CssClass="textboxStyle"
                            TabIndex="102" onkeydown="SearchByEnterKey();"></asp:TextBox>
                    </td>
                    <td width="12%" align="right" rowspan="5" valign="top">
                        <table width="85%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td style="padding-bottom: 5px">
                                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="ButtonStyle" TabIndex="110"
                                        Width="98%" UseSubmitBehavior="false" OnClick="btnSearch_Click" CausesValidation="false" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-bottom: 5px">
                                    <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="ButtonStyle"
                                        Width="98%" UseSubmitBehavior="false" OnClick="btnReset_Click" CausesValidation="false" TabIndex="111" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-bottom: 5px">
                                    <asp:Button ID="btnAddCatDetails" runat="server" Text="Add Catalogue Details" CssClass="ButtonStyle" TabIndex="112"
                                        Width="98%" UseSubmitBehavior="false" OnClick="btnAddCatDetails_Click" CausesValidation="false" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-bottom: 5px">
                                    <asp:Button ID="btnUpdateStatus" runat="server" Text="Update Status" CssClass="ButtonStyle" TabIndex="113"
                                        Width="98%" UseSubmitBehavior="false" OnClick="btnUpdateStatus_Click" CausesValidation="false" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-bottom: 5px">
                                    <asp:Button ID="btnUpdatePrdList" runat="server" Text="Upload Product List" CssClass="ButtonStyle" TabIndex="114"
                                        Width="98%" UseSubmitBehavior="false" CausesValidation="false" ToolTip="To search from list of products"
                                        OnClientClick="return OpenUploadPrdListPopUp();" OnKeyDown="OnTabPress()" />
                                </td>
                            </tr>
                        </table>

                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">Artist</td>
                    <td>
                        <asp:TextBox ID="txtArtist" runat="server" Width="95%" CssClass="textboxStyle"
                            TabIndex="103" onkeydown="SearchByEnterKey();"></asp:TextBox>
                    </td>
                    <td></td>
                    <td class="identifierLable_large_bold">Configuration</td>
                    <td>
                        <asp:DropDownList ID="ddlConfiguration" runat="server" Width="96%" CssClass="ddlStyle" onkeydown="SearchByEnterKey();" TabIndex="104">
                        </asp:DropDownList>
                    </td>

                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">Team Responsibility by Product</td>
                    <td>
                        <asp:DropDownList ID="ddlTeamResponsibility" runat="server" Width="96%" CssClass="ddlStyle" TabIndex="105" onkeydown="SearchByEnterKey();"></asp:DropDownList></td>
                    <td></td>
                    <td class="identifierLable_large_bold">Manager Responsibility by Product</td>
                    <td>
                        <asp:DropDownList ID="ddlManagerResponsibility" runat="server" Width="96%" CssClass="ddlStyle" TabIndex="106" onkeydown="SearchByEnterKey();"></asp:DropDownList></td>
                    <td></td>
                    <td class="identifierLable_large_bold"> Product Status</td>
                    <td>
                        <asp:DropDownList ID="ddlCatnoStatus" runat="server" Width="50%" CssClass="ddlStyle" TabIndex="107" onkeydown="SearchByEnterKey();"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold" width="8%">Team Responsibility by Track</td>
                    <td>
                        <asp:DropDownList ID="ddlTeamResponsibilitybyTrack" runat="server" Width="96%" CssClass="ddlStyle" TabIndex="108" onkeydown="SearchByEnterKey();"></asp:DropDownList></td>
                    <td></td>
                    <td class="identifierLable_large_bold" width="8%">Manager Responsibility by Track</td>
                    <td>
                        <asp:DropDownList ID="ddlManagerResponsibilitybyTrack" runat="server" Width="96%" CssClass="ddlStyle" TabIndex="109" onkeydown="SearchByEnterKey();"></asp:DropDownList></td>
                    <td></td>
                    <td class="identifierLable_large_bold"> Track Status</td>
                    <td>
                        <asp:DropDownList ID="ddlTrackStatus" runat="server" Width="50%" CssClass="ddlStyle" TabIndex="110" onkeydown="SearchByEnterKey()"></asp:DropDownList></td>
                </tr>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="8" runat="server" id="tdData">
                        <table width="80%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:Panel ID="PnlCatalogueDetails" runat="server" ScrollBars="Auto" Width="100%">
                                        <asp:GridView ID="gvCatalogueDetails" runat="server" AutoGenerateColumns="False" Width="98%" AlternatingRowStyle-BackColor="#E3EFFF"
                                            CssClass="gridStyle_hover" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                            EmptyDataText="No data found." OnRowDataBound="gvCatalogueDetails_RowDataBound" OnRowCommand="gvCatalogueDetails_RowCommand"
                                            AllowPaging="true" OnPageIndexChanging="gvCatalogueDetails_PageIndexChanging" PageSize="25" RowStyle-CssClass="dataRow" AllowSorting="true" OnSorting="gvCatalogueDetails_Sorting" HeaderStyle-CssClass="FixedHeader">
                                            <PagerStyle HorizontalAlign="Center" CssClass="pagerStyle" />
                                            <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
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
                                                    <ItemStyle Width="30%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Title" SortExpression="catno_title">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCatnoTitle" runat="server" Text='<%# Bind("catno_title") %>' CssClass="identifierLable"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="30%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Configuration" SortExpression="config_code">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblConfigCode" runat="server" Text='<%# Bind("config_code") %>' CssClass="identifierLable"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="10%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Product Status" SortExpression="status_desc">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnIsValid" runat="server" Value="Y" />
                                                        <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("status_desc") %>' CssClass="identifierLable"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="15%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkBtnDblClk" runat="server" CommandName="dblClk" Text="dblClick" CausesValidation="false"> </asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle CssClass="hide" />
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
                                <td>
                                    <br />
                                </td>
                            </tr>
                        </table>
                    </td>

                </tr>
            </table>


            <ajaxToolkit:ModalPopupExtender ID="mPopupPageLevel" runat="server" PopupControlID="progressBarPageLevel" TargetControlID="progressBarPageLevel"
                BackgroundCssClass="progressBar" RepositionMode="RepositionOnWindowResize">
            </ajaxToolkit:ModalPopupExtender>
            <asp:UpdateProgress ID="progressBarPageLevel" runat="server" AssociatedUpdatePanelID="updPnlPageLevel" DisplayAfter="100">
                <ProgressTemplate>
                    <div id="Search" style="font-weight: bold; color: Black; z-index: 2">
                        <img src="../Images/InProgress2.gif" alt="" />
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>

            <asp:Button ID="dummyConfirm" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeUpdateStatus" runat="server" PopupControlID="pnlPopup" TargetControlID="dummyConfirm"
                CancelControlID="btnCancel" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlPopup" runat="server" align="center" Width="30%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr align="justify" id="screen">
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblUpdateStatus" runat="server" Width="100%" Text="Update Catalogue Status" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="50%" id="status">
                                <tr align="center">
                                    <td width="30%" class="identifierLable_large_bold">New Status</td>
                                    <td width="10%"></td>
                                    <td width="50%" align="right">
                                        <asp:DropDownList ID="ddlUpdateStatus" runat="server" CssClass="ddlStyle" />
                                    </td>
                                    <td width="10%" align="left">
                                        <asp:RequiredFieldValidator runat="server" ID="rfvddlUpdateStatus" ControlToValidate="ddlUpdateStatus"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please select status" InitialValue="-" Display="Dynamic"
                                            ValidationGroup="valGrpUpdateStatus"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr align="justify">
                        <td>
                            <asp:Label ID="lblConfirmMsg" Text="Update Status of selected Catalogue Numbers" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr align="justify">
                        <td>
                            <table id="button">
                                <tr>
                                    <td>
                                        <asp:Button ID="btnUpdate" OnClick="bthUpdateCatStatusPopup_Click" UseSubmitBehavior="false" runat="server" Text="Update" CssClass="ButtonStyle"
                                            ValidationGroup="valGrpUpdateStatus" />
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

            <asp:Button ID="dummyConfirmation" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmation" runat="server" PopupControlID="pnlConfirmation" TargetControlID="dummyConfirmation"
                CancelControlID="btnNoConfirm" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlConfirmation" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmation" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMessage" Text="One or more catalogue status cannot be updated. Do you want to continue for the rest?" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYesConfirm" runat="server" OnClick="btnUpdateCatStatus_Click" Text="Yes" CssClass="ButtonStyle" />
                                        <asp:HiddenField ID="hdnOverrideInvalidCatnoUpdate" runat="server" Value="N" />
                                        <asp:HiddenField ID="hdnOverrideParticipUpdate" runat="server" Value="N" />
                                        <asp:HiddenField ID="hdnOverrideTrackStatus0" runat="server" Value="N" />
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


            <asp:Button ID="dummyMpeUploadPrdList" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeUploadPrdList" runat="server" PopupControlID="pnlMpeUploadPrdList" TargetControlID="dummyMpeUploadPrdList"
                CancelControlID="btnCloseUploadPrdListPopup" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlMpeUploadPrdList" runat="server" align="center" Width="30%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="identifierLable">Upload Product List</td>
                                    <td align="right" style="vertical-align: top;" width="5%">
                                        <asp:ImageButton ID="btnCloseUploadPrdListPopup" runat="server" ImageUrl="../Images/CloseIcon.png" Style="cursor: pointer" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table width="90%">
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtUploadPrdList" runat="server" TextMode="MultiLine" Width="99%" Height="100px" class="identifierLable" Style="word-break: break-all"></asp:TextBox>
                                    </td>
                                    <td width="5%">
                                        <asp:RequiredFieldValidator runat="server" ID="rvTxtUploadPrdList" ControlToValidate="txtUploadPrdList" ValidationGroup="valGrpUploadPrdList"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter semicolon(;) separated Catalogue Numbers" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                            </table>

                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Label ID="lblUploadPrdListError" Text="Invalid Catalogue Numbers entered" runat="server" CssClass="errorMessage" Visible="false"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Label ID="lblUploadPrdList" Text="Upload semicolon(;) separated Catalogue Numbers" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding-bottom: 5px">
                            <asp:Button ID="btnSearchBulkUpload" OnClick="btnSearchBulkUpload_Click" UseSubmitBehavior="false" runat="server" Text="Search" CssClass="ButtonStyle"
                                ValidationGroup="valGrpUploadPrdList" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <msg:MsgControl ID="msgView" runat="server" />

            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnIsValidSearch" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsNewRequest" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnIsSuperUser" runat="server" Value="N" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99"></asp:Label>
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>


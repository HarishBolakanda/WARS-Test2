<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ArtistAudit.aspx.cs" Inherits="WARS.ArtistAudit" MasterPageFile="~/MasterPage.Master"
    Title="WARS - ArtistAudit " MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

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
            document.getElementById("<%=PnlArtistDetails.ClientID %>").style.height = gridPanelHeight + "px";
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

        //Fuzzy search filters
        var txtArtist;
        function ArtistSelected(sender, args) {

            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtArtist.ClientID %>").value = "";                
            }
            else {
                document.getElementById('<%=btnHdnArtistSearch.ClientID%>').click();
            }
        }

        function ArtistListPopulating() {            
            txtArtist = document.getElementById("<%= txtArtist.ClientID %>");
            txtArtist.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtArtist.style.backgroundRepeat = 'no-repeat';
            txtArtist.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidSearch.ClientID %>").value = "N";
        }

        function ArtistListPopulated() {
            txtArtist = document.getElementById("<%= txtArtist.ClientID %>");
            txtArtist.style.backgroundImage = 'none';
        }

        //=============== End


    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="7">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    Artist Audit
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="8%"></td>
                    <td width="3%" class="identifierLable_large_bold">Artist</td>
                    <td width="20%">
                        <asp:TextBox ID="txtArtist" runat="server" Width="99%" CssClass="textboxStyle"
                            TabIndex="100"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="aceArtistSearch" runat="server"                            
                            ServiceMethod="FuzzySearchAllArtisList"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtArtist"
                            FirstRowSelected="true"
                            OnClientItemSelected="ArtistSelected"
                            OnClientPopulating="ArtistListPopulating"
                            OnClientPopulated="ArtistListPopulated"
                            OnClientHidden="ArtistListPopulated"
                            CompletionListElementID="acePnlArtist" />
                        <asp:Panel ID="acePnlArtist" runat="server" CssClass="identifierLable" />
                    </td>
                    <td width="3%" align="left">
                        <asp:ImageButton ID="fuzzySearchArtist" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button"
                            OnClick="fuzzySearchArtist_Click" ToolTip="Search Artist code/name" TabIndex="101" />
                    </td>
                    <td></td>
                    <td width="3%"></td>
                    <td align="right" width="15%">
                        <asp:Button ID="btnArtistMaint" runat="server" Text="Artist Maintenance" CssClass="ButtonStyle"
                            Width="98%" UseSubmitBehavior="false" OnClick="btnArtistMaint_Click" TabIndex="102" onkeydown="OnTabPress();" />
                    </td>
                </tr>
                <tr>
                    <td colspan="7">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="5" class="table_with_border" runat="server" id="tdData">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="3%">
                                    <br />
                                </td>
                                <td width="94%"></td>
                                <td width="3%"></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <table width="98%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="20%" class="gridHeaderStyle_1row">Artist Name</td>
                                                        <td width="20%" class="gridHeaderStyle_1row">Deal Type</td>
                                                        <td width="15%" class="gridHeaderStyle_1row">Team Responsibility</td>
                                                        <td width="15%" class="gridHeaderStyle_1row">Manager Responsibility</td>
                                                        <td width="15%" class="gridHeaderStyle_1row">Update by</td>
                                                        <td width="15%" class="gridHeaderStyle_1row">Update on</td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlArtistDetails" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvArtistDetails" runat="server" AutoGenerateColumns="False" Width="98%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." ShowHeader="False" OnRowDataBound="gvAuditDetails_RowDataBound">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblArtistDesc" runat="server" Text='<%# Bind("Artist_name") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblDealType" runat="server" Text='<%# Bind("deal_type_desc") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblTeamResponsibility" runat="server" Text='<%# Bind("team_responsibility") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="15%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblManagerResponsibility" runat="server" Text='<%# Bind("manager_responsibility") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="15%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblUpdatedby" runat="server" Text='<%# Bind("user_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="15%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnChangeType" runat="server" Value='<%# Bind("change_type") %>' />
                                                                    <asp:Label ID="lblUpdatedon" runat="server" Text='<%# Bind("last_modified") %>' CssClass="identifierLable"></asp:Label>
                                                                    <asp:HiddenField ID="hdnDisplayOrder" runat="server" Value='<%# Bind("display_order") %>' />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="15%" />
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
                                <td></td>
                            </tr>
                        </table>
                    </td>
                    <td colspan="2"></td>
                </tr>
            </table>

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
                                        <asp:ImageButton ID="btnCloseFuzzySearchPopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer"/>
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
            <asp:Button ID="btnHdnArtistSearch" runat="server" Style="display: none;" OnClick="btnHdnArtistSearch_Click" CausesValidation="false" />
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:TextBox>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

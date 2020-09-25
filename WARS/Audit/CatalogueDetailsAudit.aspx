<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CatalogueDetailsAudit.aspx.cs" Inherits="WARS.Audit.CatalogueDetailsAudit" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Catalogue Details Audit" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

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



        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        function MoveCatNoFocus() {
            document.getElementById("<%= lblTab.ClientID %>").focus();
        }

        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.6;
            document.getElementById("<%=PnlCatalogueDetailsAudit.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }
    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="8">
                        <asp:Panel ID="PnlScreenName" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    CATALOGUE DETAILS AUDIT
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="8"></td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="10%" class="identifierLable_large_bold">Catalogue Number</td>
                    <td width="10%">
                        <asp:TextBox ID="txtCatalogueSearch" runat="server" Width="95%" ReadOnly="true" CssClass="textboxStyle_readonly"
                            TabIndex="100" OnFocus="MoveCatNoFocus();"></asp:TextBox>
                    </td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td align="right" colspan="2">
                        <asp:Button ID="btnCatalogue" runat="server" Text="Catalogue Maintenance" CssClass="ButtonStyle"
                            Width="98%" UseSubmitBehavior="false" TabIndex="101" OnClick="btnCatalogueDetails_Click" onkeydown="OnTabPress();" />
                    </td>
                </tr>
                <tr>
                    <td colspan="8">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td colspan="5"></td>
                    <td width="6%"></td>
                    <td width="6%"></td>
                    <td width="6%"></td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="6" class="table_with_border" runat="server" id="tdData">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="2%">
                                    <br />
                                </td>
                                <td width="97%"></td>
                                <td width="1%"></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td></td>
                                        </tr>
                                        <tr runat="server" id="trRoyAudit">
                                            <td>
                                                <asp:Panel ID="PnlCatalogueDetailsAudit" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvCatalogueDetailsAudit" runat="server" AutoGenerateColumns="False" Width="98.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." ShowHeader="False">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <table width="100%" cellspacing="0">
                                                                        <tr>
                                                                            <td width="12%"></td>
                                                                            <td width="20%"></td>
                                                                            <td width="13%"></td>
                                                                            <td width="20%"></td>
                                                                            <td width="12%"></td>
                                                                            <td width="3%"></td>
                                                                            <td width="8%"></td>
                                                                            <td width="12%"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Title
                                                                            <asp:HiddenField ID="hdnChangeType" runat="server" Value='<%# Bind("change_type") %>' />
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblTitle" runat="server" Text='<%# Bind("catno_title") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Status
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("status_desc") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Compilation?
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblCompilation" runat="server" Text='<%# Bind("is_compilation") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">
                                                                                <asp:Label ID="lblUserCodeHdr" runat="server" Text="Updated by" CssClass="identifierLable_large_bold"></asp:Label>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblUserCode" runat="server" Text='<%# Bind("user_code") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Artist
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblArtistName" runat="server" Text='<%# Bind("artist_name") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Configuration
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblConfigName" runat="server" Text='<%# Bind("config_name") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Complicated?
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblComplicated" runat="server" Text='<%# Bind("is_complicated") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">
                                                                                <asp:Label ID="lblUpdatedOnHdr" runat="server" Text="Updated on" CssClass="identifierLable_large_bold"></asp:Label>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblUpdatedOn" runat="server" Text='<%# Bind("last_modified") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">MUR Owner
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblMurOwner" runat="server" Text='<%# Bind("mur_owner_name") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Project
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblProject" runat="server" Text='<%# Bind("project_title") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Licensed Out?
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblLicensedOut" runat="server" Text='<%# Bind("is_licensed_out") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Marketing Owner

                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblMarketingOwner" runat="server" Text='<%# Bind("marketing_owner_code") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Exception Rate Project
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblExceptionRate" runat="server" Text='<%# Bind("exception_rate_project") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Unlisted Components
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblUnlistedComponents" runat="server" Text='<%# Bind("unlisted_components") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                            <td></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">WEA Sales Label

                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblWeaSalesLabel" runat="server" Text='<%# Bind("wea_sales_label_code") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Label
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblLabel" runat="server" Text='<%# Bind("label") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Legacy
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblLegacy" runat="server" Text='<%# Bind("legacy") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                            <td></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Total Tracks
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblTotalTracks" runat="server" Text='<%# Bind("total_tracks") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">First Sale Date

                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblFirstSaleDate" runat="server" Text='<%# Bind("first_sale_date") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                            <td></td>
                                                                            <td></td>
                                                                            <td></td>
                                                                        </tr>

                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Total Play Length
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblPlayLength" runat="server" Text='<%# Bind("total_play_length") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Time / Track Share
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblTotalPlay" runat="server" Text='<%# Bind("track_time_flag") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                            <td></td>
                                                                            <td></td>
                                                                            <td></td>
                                                                        </tr>
                                                                    </table>
                                                                </ItemTemplate>
                                                                <%--<ItemStyle Width="100%" />--%>
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
                                    </table>
                                </td>
                                <td></td>
                            </tr>
                        </table>
                    </td>
                    <td></td>
                </tr>
            </table>
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
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:Label ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:Label>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>



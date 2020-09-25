<%@ Page Title="WARS - Auto Participant Audit" Language="C#" MasterPageFile="~/MasterPage.Master" 
    AutoEventWireup="true" CodeBehind="AutoParticipantAudit.aspx.cs" Inherits="WARS.Audit.AutoParticipantAudit" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
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
            var gridPanelHeight = windowHeight * 0.6;
            document.getElementById("<%=PnlAutoParticipantAudit.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        function OpenAutoParticipantSearch() {
            window.location = "../Participants/AutoParticipantSearch.aspx?isNewRequest=N";

        }
    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="3">
                        <asp:Panel ID="PnlScreenName" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    AUTO PARTICIPANTS AUDIT

                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td align="right" style="width: 25%">
                        <asp:Button ID="btnAutoParticipantSearch" Width="50%" runat="server" CausesValidation="false" CssClass="ButtonStyle"
                            OnClientClick="if (!OpenAutoParticipantSearch()) { return false;};" TabIndex="101" Text="Auto Participant Search"
                            UseSubmitBehavior="false" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td class="table_with_border">
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
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlAutoParticipantAudit" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvAutoParticipantAudit" runat="server" AutoGenerateColumns="False" Width="98.2%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." ShowHeader="False">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <table width="100%" cellspacing="0">
                                                                        <tr>
                                                                            <td width="15%"></td>
                                                                            <td width="15%"></td>
                                                                            <td width="15%"></td>
                                                                            <td width="25%"></td>
                                                                            <td width="10%"></td>
                                                                            <td width="20%"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Marketing Owner

                                                                            <asp:HiddenField ID="hdnChangeType" runat="server" Value='<%# Bind("change_type") %>' />
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblMarketingOwner" runat="server" Text='<%# Bind("marketing_owner_code") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Artist

                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblArtist" runat="server" Text='<%# Bind("artist_name") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>

                                                                            <td class="identifierLable_large_bold" align="left">
                                                                                <asp:Label ID="lblUserCodeHdr" runat="server" Text="Updated by" CssClass="identifierLable_large_bold"></asp:Label>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblUserCode" runat="server" Text='<%# Bind("user_code") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">WEA Sales Label
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblWeaSalesLabel" runat="server" Text='<%# Bind("wea_sales_label_code") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Project Title
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblProjectTitle" runat="server" Text='<%# Bind("project_title") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>

                                                                            <td class="identifierLable_large_bold" align="left">
                                                                                <asp:Label ID="lblUpdatedOnHdr" runat="server" Text="Updated on" CssClass="identifierLable_large_bold"></asp:Label>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblUpdatedOn" runat="server" Text='<%# Bind("last_modified") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="6"></td>
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
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

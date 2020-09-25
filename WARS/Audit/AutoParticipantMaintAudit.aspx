<%@ Page Title="WARS - Auto Participant Audit Details" Language="C#" MasterPageFile="~/MasterPage.Master"
    AutoEventWireup="true" CodeBehind="AutoParticipantMaintAudit.aspx.cs" Inherits="WARS.Audit.AutoParticipantMaintAudit" %>

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

            //Handling Session timeout 
            // - Reset session timeout on each request end            
            ResetSession();
        }
        //======================= End


        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.6;
            document.getElementById("<%=PnlAutoPartDetailsAudit.ClientID %>").style.height = gridPanelHeight + "px";
             document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

         }

         function RedirectToErrorPage() {
             document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        function OpenAutoParticipantMaint() {
            autoPartId = document.getElementById("<%=hdnAutoPartId.ClientID %>").value;
            window.location = "../Participants/AutoParticipantMaintenance.aspx?autoPartId=" + autoPartId;

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
                                    AUTO PARTICIPANT DETAILS AUDIT
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="3"></td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td  align="right" style="width: 15%">
                        <asp:Button ID="btnAutoParticipantMaint" Width="90%" runat="server" Text="Auto Participant Maintenance" CssClass="ButtonStyle" OnClientClick="if (!OpenAutoParticipantMaint()) { return false;};"
                            UseSubmitBehavior="false" />
                    </td>
                </tr>

                <tr>
                    <td colspan="3"></td>

                </tr>
                <tr>
                    <td></td>
                    <td class="table_with_border" runat="server" id="tdData">
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
                                                <asp:Panel ID="PnlAutoPartDetailsAudit" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvAutoPartDetailsAudit" runat="server" AutoGenerateColumns="False" Width="98.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        OnRowDataBound="gvAutoPartDetailsAudit_RowDataBound" CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." ShowHeader="False">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <table width="100%" cellspacing="0">
                                                                        <tr>
                                                                            <td width="12%"></td>
                                                                            <td width="20%"></td>
                                                                            <td width="13%"></td>
                                                                            <td width="15%"></td>
                                                                            <td width="13%"></td>
                                                                            <td width="8%"></td>
                                                                            <td width="9%"></td>
                                                                            <td width="10%"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Royaltor                                                                            
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblRoyaltor" runat="server" Text='<%# Bind("royaltor") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">OptionPeriod
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblOptionPeriod" runat="server" Text='<%# Bind("option_period") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Active
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblIsActive" runat="server" Text='<%# Bind("is_active") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">
                                                                                <asp:Label ID="lblUserCodeHdr" runat="server" Text="Updated by" CssClass="identifierLable_large_bold"></asp:Label>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblUserCode" runat="server" Text='<%# Bind("user_code") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Territory
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblTerritory" runat="server" Text='<%# Bind("territory") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Escalation Code
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblEscCode" runat="server" Text='<%# Bind("esc_code") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Escalation Include Units
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblIncludeEscalation" runat="server" Text='<%# Bind("inc_in_escalation") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">
                                                                                <asp:Label ID="lblUpdatedOnHdr" runat="server" Text="Updated on" CssClass="identifierLable_large_bold"></asp:Label>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblUpdatedOn" runat="server" Text='<%# Bind("last_modified") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Track Share
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblShareTracks" runat="server" Text='<%# Bind("share_tracks") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Total Tracks
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblShareTotalTracks" runat="server" Text='<%# Bind("share_total_tracks") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Track Title
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblTrackTitle" runat="server" Text='<%# Bind("track_title") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Time Share
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblShareTime" runat="server" Text='<%# Bind("share_time") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Total Time
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblShareTotalTime" runat="server" Text='<%# Bind("share_total_time") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                            <td></td>
                                                                            <td></td>
                                                                            <td></td>
                                                                            <td></td>
                                                                        </tr>
                                                                    </table>
                                                                    <asp:HiddenField ID="hdnDisplayOrder" runat="server" Value='<%# Bind("display_order") %>' />
                                                                    <asp:HiddenField ID="hdnAutoPartId" runat="server" Value='<%# Bind("auto_participant_id") %>' />
                                                                    <asp:HiddenField ID="hdnAutoPartDetailId" runat="server" Value='<%# Bind("auto_participant_detail_id") %>' />
                                                                    <asp:HiddenField ID="hdnRoyaltorId" runat="server" Value='<%# Bind("royaltor_id") %>' />
                                                                    <asp:HiddenField ID="hdnOptionPeriodCode" runat="server" Value='<%# Bind("option_period_code") %>' />
                                                                    <asp:HiddenField ID="hdnSellerGroupCode" runat="server" Value='<%# Bind("seller_group_code") %>' />
                                                                    <asp:HiddenField ID="hdnTuneId" runat="server" Value='<%# Bind("tune_code") %>' />
                                                                     <asp:HiddenField ID="hdnEscCode" runat="server" Value='<%# Bind("esc_code") %>' />
                                                                     <asp:HiddenField ID="hdnIncludeEscalation" runat="server" Value='<%# Bind("inc_in_escalation") %>' />
                                                                    <asp:HiddenField ID="hdnParticipationType" runat="server" Value='<%# Bind("participation_type") %>' />
                                                                </ItemTemplate>
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
            <asp:HiddenField ID="hdnAutoPartId" runat="server" />
            </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

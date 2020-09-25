<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RoyaltorActivity.ascx.cs" Inherits="WARS.RoyaltorActivity" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<script type="text/javascript">

    //WUIN-609 - To close the message pop up on Enter key
    //Add keypress event handler when pop up is shown
    //Remove keypress event handler when pop up is closed
    function pageLoadRoyActivity() {
        if ($find('behaviourRoyActivityMsgPopup') != null) {
            $find('behaviourRoyActivityMsgPopup').add_shown(function () {
                $addHandler(document, "keydown", ClosePopupRoyActivity);
            });

            $find('behaviourRoyActivityMsgPopup').add_hiding(function () {
                $removeHandler(document, "keydown", ClosePopupRoyActivity);
            });
        }
    }

    function ClosePopupRoyActivity() {
        //close pop up on Enter key        
        if (event.keyCode == 13) {
            $find("behaviourRoyActivityMsgPopup").hide();
        }
    }

    //======= WUIN-609 -- Ends


    //WUIN-908 Open Confirmation pop up on Run Requested Statements
    function OpenConfirmPopup() {
        var popup = $find('<%= mpeRoyaltyEngine.ClientID %>');
        if (popup != null) {
            popup.show();
        }
        return false;
    }



</script>
<asp:UpdatePanel ID="updPnlActivity" runat="server">
    <ContentTemplate>
        <%--Activity pop up - Begin--%>
        <asp:Button runat="server" ID="btnMpeRoyActivity" Style="display: none" />
        <ajaxToolkit:ModalPopupExtender ID="mpeRoyActivity" BehaviorID="modalPopupBehavior2" runat="server" PopupControlID="pnlRoyaltorActivity" TargetControlID="btnMpeRoyActivity"
            BackgroundCssClass="messageBackground" RepositionMode="RepositionOnWindowResize">
        </ajaxToolkit:ModalPopupExtender>
        <asp:Panel ID="pnlRoyaltorActivity" runat="server" CssClass="Popup" align="center" Width="85%" Style="display: none" BackColor="Window">
            <table id="tblMain" style="width: 100%;">
                <tr>
                    <td colspan="3">
                        <table style="background-image: url(../Images/bg5.png); color: #ffffff; font-weight: bold; height: auto; width: 100%">
                            <tr>
                                <td style="width: 98%; vertical-align: top; text-align: left">STATEMENT ACTIVITY LIST                                                              
                                </td>
                                <td style="vertical-align: top; text-align: right">
                                    <asp:ImageButton ID="btnClosePopupAct" ImageUrl="../Images/CloseIcon.png" runat="server" OnClick="btnClosePopupAct_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="3"></td>
                </tr>
                <tr>
                    <td colspan="3" align="left">
                        <table width="100%">
                            <tr>
                                <td width="2%"></td>
                                <td>
                                    <table width="100%" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>
                                                <table width="98.5%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Button ID="btnRefresh" runat="server" CssClass="ButtonStyle" OnClick="btnRefresh_Click" Text="Refresh"
                                                                UseSubmitBehavior="false" Width="18%" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Button ID="btnRoyaltyEngine" runat="server" CssClass="ButtonStyle" Text="Run Requested Statements"
                                                                UseSubmitBehavior="false" Width="18%" OnClientClick="return OpenConfirmPopup();" />
                                                            <%--JIRA-908 CHanges--%>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- Start--%>
                                                            <asp:Button ID="dummyRoyaltyEngine" runat="server" Style="display: none" />
                                                            <ajaxToolkit:ModalPopupExtender ID="mpeRoyaltyEngine" runat="server" PopupControlID="pnlRoyaltyEngine" TargetControlID="dummyRoyaltyEngine"
                                                                CancelControlID="btnNo" BackgroundCssClass="popupBox">
                                                            </ajaxToolkit:ModalPopupExtender>
                                                            <asp:Panel ID="pnlRoyaltyEngine" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                                                                Style="z-index: 1; display: none">
                                                                <table width="100%">
                                                                    <tr>
                                                                        <td class="ScreenName" style="align-content: center">
                                                                            <asp:Label ID="lblConfirmation" runat="server" Text="Run RoyaltyEngine Confirmation" CssClass="identifierLable"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="lblText" runat="server"
                                                                                CssClass="identifierLable" Text="Do you want to run the Royalty Engine?"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <table>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Button ID="btnYes" runat="server" Text="Yes" CssClass="ButtonStyle" OnClick="btnRoyaltyEngine_Click" />
                                                                                    </td>
                                                                                    <td></td>
                                                                                    <td>
                                                                                        <asp:Button ID="btnNo" runat="server" Text="No" CssClass="ButtonStyle" />
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </asp:Panel>
                                                            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- End--%>

                                                        </td>
                                                    </tr>
                                                </table>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <table width="98.5%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="9%" class="gridHeaderStyle_1row">Type</td>
                                                        <td width="9%" class="gridHeaderStyle_1row">Code</td>
                                                        <td width="19%" class="gridHeaderStyle_1row">Name</td>
                                                        <td width="14%" class="gridHeaderStyle_1row">Reporting Schedule</td>
                                                        <td width="9%" class="gridHeaderStyle_1row">User</td>
                                                        <td width="10%" class="gridHeaderStyle_1row">Date Added</td>
                                                        <td width="15%" class="gridHeaderStyle_1row">Run Status</td>
                                                        <td width="5%" class="gridHeaderStyle_1row">Remove
                                                            <asp:ImageButton ID="btnRemoveAll" ImageUrl="../Images/Delete.gif" runat="server" ToolTip="Remove all from run"
                                                                OnClick="btnRemoveAll_Click" />
                                                        </td>
                                                        <td width="5%" class="gridHeaderStyle_1row">Retry
                                                             <asp:ImageButton ID="ImageButton1" ImageUrl="../Images/Retry.jpg" runat="server" ToolTip="Retry all"
                                                                 OnClick="btnRetryAll_Click" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:Panel ID="PnlActivity" runat="server" Height="200px" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvRoyActivity" runat="server" AutoGenerateColumns="False" Width="98.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" ShowHeaderWhenEmpty="true" EmptyDataText="No Data Found"
                                                        ShowHeader="false" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" OnRowDataBound="gvRoyActivity_RowDataBound">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-Width="9%" ItemStyle-CssClass="gridItemStyle_Left_Align">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRoyType" runat="server" Text='<%#Bind("type")%>' CssClass="identifierLable" />
                                                                    <asp:HiddenField ID="hdnLevelFlag" runat="server" Value='<%#Bind("level_flag")%>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="9%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblCode" runat="server" Text='<%#Bind("code")%>' CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="19%" ItemStyle-CssClass="gridItemStyle_Left_Align">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblName" runat="server" Text='<%#Bind("name")%>' CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="14%" ItemStyle-CssClass="gridItemStyle_Left_Align">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRepSchedule" runat="server" Text='<%#Bind("rep_schedule")%>' CssClass="identifierLable" />
                                                                    <asp:Label ID="lblStmtPeriodId" runat="server" Text='<%#Bind("stmt_period_id")%>' Visible="false" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="9%" ItemStyle-CssClass="gridItemStyle_Left_Align">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblUserCode" runat="server" Text='<%#Bind("user_code")%>' CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblDateAdded" runat="server" Text='<%#Bind("date_added")%>' CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="15%" ItemStyle-CssClass="gridItemStyle_Left_Align">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRunStatus" runat="server" Text='<%#Bind("run_status")%>' CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                                <ItemTemplate>
                                                                    <asp:ImageButton ID="btnRemove" ImageUrl="../Images/Delete.gif" runat="server" ToolTip="Remove from run"
                                                                        OnClick="btnRemove_Click" />
                                                                    <asp:Label ID="lblCanBeRemoved" runat="server" Text='<%#Bind("remove_from_run")%>' Visible="false" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                                <ItemTemplate>
                                                                    <asp:ImageButton ID="btnRetry" ImageUrl="../Images/Retry.jpg" runat="server" ToolTip="Retry"
                                                                        OnClick="btnRetry_Click" />
                                                                    <asp:Label ID="lblRetry" runat="server" Text='<%#Bind("royaltor_stmt_flag")%>' Visible="false" />
                                                                    <asp:HiddenField ID="hdnDetailFlag" runat="server" Value='<%#Bind("dtl_file_flag")%>' />                                                                    
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </asp:Panel>
                                                <asp:Repeater ID="rptPager" runat="server">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>'
                                                            OnClientClick="eval(this.href);" ClientIDMode="AutoID" CausesValidation="false" Enabled='<%# Eval("Enabled") %>' OnClick="lnkPage_Click" CssClass="gridPager"> </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="hdnIsActivityChanged" runat="server" />
            <asp:HiddenField ID="hdnPageNumber" runat="server" Value="" />
        </asp:Panel>
        <%--Activity pop up - End--%>

        <%--Message pop up - Begin--%>
        <%--WUIN-609 - Added this to fix issue of closing message pop up on Esc key is not working in this pop up--%>
        <asp:Button runat="server" ID="btnMpeRoyActivityMsgPopup" Style="display: none" />
        <ajaxToolkit:ModalPopupExtender ID="mpeRoyActivityMsgPopup" runat="server" BehaviorID="behaviourRoyActivityMsgPopup" PopupControlID="pnlRoyActivityMsgPopup" TargetControlID="btnMpeRoyActivityMsgPopup"
            CancelControlID="btnClosePopup" RepositionMode="RepositionOnWindowResize" BackgroundCssClass="messageBackground" PopupDragHandleControlID="programmaticMessageDragHandle">
        </ajaxToolkit:ModalPopupExtender>
        <asp:Panel ID="pnlRoyActivityMsgPopup" runat="server" align="center" Width="30%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="display: none">
            <asp:Panel ID="programmaticMessageDragHandle" runat="server" Style="cursor: move">
                <table id="Table1" style="width: 100%;">
                    <tr class="ScreenName">
                        <td align="right" style="vertical-align: top;">
                            <asp:ImageButton ID="btnClosePopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMessageRoyActivity" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </asp:Panel>
        <%--Message pop up - End--%>
    </ContentTemplate>
</asp:UpdatePanel>

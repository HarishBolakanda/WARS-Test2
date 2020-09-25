<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BOReports.aspx.cs" Inherits="WARS.BOReports" MasterPageFile="~/MasterPage.Master"
    Title="WARS - BO Reports" MaintainScrollPositionOnPostback="true" %>

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
        //probress bar and scroll position functionality - ends       

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        //=============== End

    </script>
    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>

            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="2">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    <%--BO REPORTS--%>
                                    <asp:Label ID="lblScreenName" runat="server" Text="BO REPORTS"></asp:Label>
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <br />
                        <br />
                    </td>
                </tr>
                <tr>
                    <td width="35%"></td>
                    <td align="left">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td>
                                    <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="30%">
                                        <asp:GridView ID="gvBOReports" runat="server" AutoGenerateColumns="False" Width="98.2%" AlternatingRowStyle-BackColor="#E3EFFF"
                                            CssClass="gridStyle_hover" BackColor="White" HorizontalAlign="Left" EmptyDataText="No Data Found" RowStyle-CssClass="dataRow"
                                            EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" AllowSorting="true" OnRowDataBound="gvBOReports_RowDataBound" OnSorting="gvBOReports_Sorting" HeaderStyle-CssClass="FixedHeader">
                                            <%--AlternatingRowStyle-BackColor="#E3EFFF"--%>

                                            <Columns>
                                                <asp:TemplateField ItemStyle-Width="65%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="GridHeaderLargeFont" HeaderText="Report Name" SortExpression="name">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnCUID" runat="server" Value='<%#Bind("cuid")%>' />
                                                        <%-- <asp:LinkButton ID="btnOpenRpt" Text='<%#Bind("name")%>' runat="server" CssClass="Hyperlink_largeFont" 
                                                            OnClick="btnOpenRpt_Click" TabIndex="100" onkeydown="OnTabPress();"></asp:LinkButton>--%> <%--ForeColor="Black"--%>

                                                        <asp:LinkButton ID="btnOpenRpt" Text='<%#Bind("name")%>' runat="server" CssClass="Hyperlink_largeFont" ForeColor="Black"
                                                            OnClick="btnOpenRpt_Click" TabIndex="100" onkeydown="OnTabPress();"></asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

            <asp:UpdateProgress ID="progressBarPageLevel" runat="server" AssociatedUpdatePanelID="updPnlPageLevel" DisplayAfter="100">
                <ProgressTemplate>
                    <div id="Search" style="font-weight: bold; color: Black">
                        <table>
                            <tr>
                                <td>
                                    <img src="../Images/InProgress2.gif" alt="" />
                                </td>
                            </tr>
                            <tr>
                                <td class="identifierLable">Please Wait...
                                </td>
                            </tr>
                        </table>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <ajaxToolkit:ModalPopupExtender ID="mPopupPageLevel" runat="server" PopupControlID="progressBarPageLevel" TargetControlID="progressBarPageLevel"
                BackgroundCssClass="progressBar" RepositionMode="RepositionOnWindowResize">
            </ajaxToolkit:ModalPopupExtender>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:TextBox>
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

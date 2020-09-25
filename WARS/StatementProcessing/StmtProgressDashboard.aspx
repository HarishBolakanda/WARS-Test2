<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StmtProgressDashboard.aspx.cs" Inherits="WARS.StmtProgressDashboard" MasterPageFile="~/MasterPage.Master"
    Title="WARS - StmtProgressDashboard" MaintainScrollPositionOnPostback="true" %>

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

        //probress bar and scroll position functionality - starts


        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
            document.getElementById("<%=PnlGrid.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        //grid panel height adjustment functioanlity - ends

        //allow postback on enter key press
        function ExcecuteEnterPress() {
            if (event.keyCode == 13) {
                document.body.setActive();
                document.body.focus();
                window.event.srcElement.focus();
            }
        }

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        //=============== End

        //Wuin-1022 Enter key functionality
        function SearchByEnterKey() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnGo.ClientID%>').click();
            }
            else {
                return false;
            }
        }

        //Reset Earnings compare ddl when Earning field is empty
        function OnTxtEarningsBlur() {
            txtEarnings = document.getElementById("<%=txtEarnings.ClientID %>").value;
            ddlEarningsCompare = document.getElementById("<%=ddlEarningsCompare.ClientID %>");
            if (txtEarnings == "") {
                ddlEarningsCompare.selectedIndex = 0;
            }
        }


    </script>
    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="4">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    STATEMENT PROGRESS DASHBOARD
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="4"></td>
                </tr>
                <tr>
                    <td width="15%"></td>
                    <td width="70%">
                        <table width="100%">
                            <tr>
                                <td align="left">
                                    <table width="100%" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td width="2%"></td>
                                            <td width="15%" class="identifierLable_large_bold">Company</td>
                                            <td width="20%">
                                                <asp:DropDownList ID="ddlCompany" runat="server" Width="85%" CssClass="ddlStyle" onkeydown="SearchByEnterKey();" TabIndex="100">
                                                </asp:DropDownList>
                                            </td>
                                            <td width="3%"></td>
                                            <td width="20%" class="identifierLable_large_bold">Manager Responsibility
                                            </td>
                                            <td width="20%">
                                                <asp:DropDownList ID="ddlMngrResponsibility" runat="server" Width="85%" CssClass="ddlStyle"
                                                    onkeydown="SearchByEnterKey();" TabIndex="101">
                                                </asp:DropDownList>
                                            </td>
                                            <td align="right">
                                                <asp:Button ID="btnGo" runat="server" Text="Search" CssClass="ButtonStyle" OnClick="btnGo_Click" ValidationGroup="valGrpGo"
                                                    UseSubmitBehavior="false" Width="40%" TabIndex="106" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Reporting Schedule</td>
                                            <td>
                                                <asp:DropDownList ID="ddlReportingSch" runat="server" Width="85%" CssClass="ddlStyle" onkeydown="SearchByEnterKey();" TabIndex="102">
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Team Responsibility
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlTeamResponsibility" runat="server" Width="85%" CssClass="ddlStyle"
                                                   onkeydown="SearchByEnterKey();" TabIndex="103">
                                                </asp:DropDownList>
                                            </td>
                                            <td align="right" style="padding-top: 5px">
                                                <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="ButtonStyle" OnClick="btnReset_Click"
                                                    UseSubmitBehavior="false" TabIndex="107" Width="40%" onkeydown="OnTabPress();"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold" valign="top">Earnings</td>
                                            <td colspan="2" valign="top">
                                                <asp:DropDownList ID="ddlEarningsCompare" runat="server" Width="20%" onkeydown="SearchByEnterKey();" TabIndex="104" CssClass="ddlStyle">
                                                    <asp:ListItem Text="=" Value="=" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="<=" Value="<="></asp:ListItem>
                                                    <asp:ListItem Text=">=" Value=">="></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:TextBox ID="txtEarnings" runat="server" Width="50%" CssClass="identifierLable" TabIndex="105" onblur="OnTxtEarningsBlur();" onkeydown="SearchByEnterKey();"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="revtxtEarnings" runat="server" Text="*" ControlToValidate="txtEarnings" ValidationGroup="valGrpGo"
                                                    ValidationExpression="^-?(\d*\.)?\d+$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                    ToolTip="Please enter only numeric values" Display="Dynamic"> </asp:RegularExpressionValidator>
                                            </td>
                                            <td colspan="3" valign="top" align="right">
                                                <table width="85%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td id="tdGvRemainingDaysHdr" runat="server" style="padding-right: 6%;">
                                                            <table width="60%" cellpadding="0" cellspacing="0">
                                                                <tr>
                                                                    <td width="50%" class="gridHeaderStyle_1row">Reported day
                                                                    </td>
                                                                    <td width="50%" class="gridHeaderStyle_1row">No of days to go</td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td id="tdGvRemainingDays" runat="server" style="padding-right: 6%;">
                                                            <asp:GridView ID="gvRemainingDays" runat="server" AutoGenerateColumns="False" Width="60%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                                CssClass="gridStyle" BackColor="White" HorizontalAlign="Right" EmptyDataText="No Data Found"
                                                                EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" ShowHeader="false" ShowHeaderWhenEmpty="true">
                                                                <Columns>
                                                                    <asp:TemplateField ItemStyle-Width="50%" ItemStyle-CssClass="gridItemStyle_Center_Align"
                                                                        HeaderStyle-CssClass="GridHeaderLargeFont" HeaderText="Reported day">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblDaysAfter" runat="server" Text='<%#Bind("days_after")%>' CssClass="dashboardGridText" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField ItemStyle-Width="50%" ItemStyle-CssClass="gridItemStyle_Center_Align"
                                                                        HeaderStyle-CssClass="GridHeaderLargeFont" HeaderText="No of days to go">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblRemainingDays" runat="server" Text='<%#Bind("remaining_days")%>' CssClass="dashboardGridText" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="7" align="right"></td>
                                        </tr>
                                    </table>
                                </td>

                            </tr>
                            <tr>
                               <td></td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <table width="100%" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td align="left">
                                                <table width="97.8%" cellpadding="0" cellspacing="0">

                                                    <tr>
                                                        <td colspan="6">
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="11%" class="gridHeaderStyle_1row">
                                                            <%--<table width="100%">
                                                                <tr>
                                                                    <td>Status
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td></td>
                                                                </tr>
                                                            </table>--%>
                                                            Status
                                                        </td>
                                                        <td width="15%" class="gridHeaderStyle_1row">Team Responsibility</td>
                                                        <td width="8%" class="gridHeaderStyle_1row">No. of stmts</td>
                                                        <td width="8%" class="gridHeaderStyle_1row">Owners</td>
                                                        <td width="8%" class="gridHeaderStyle_1row">Royaltors</td>
                                                        <td width="8%" class="gridHeaderStyle_1row">Earnings</td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvStatements" runat="server" AutoGenerateColumns="False" Width="97.8%"
                                                        CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" EmptyDataText="No Data Found"
                                                        OnRowDataBound="gvStatements_RowDataBound" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" ShowHeader="false"
                                                        OnRowCommand="gvStatements_RowCommand">
                                                        <%--AlternatingRowStyle-BackColor="#E3EFFF"--%>
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="" ItemStyle-Width="1%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                                <ItemTemplate>
                                                                    <asp:ImageButton ID="imgExpand" runat="server" ImageUrl="../Images/Plus.gif" CommandName="Expand" Visible="false"
                                                                        CommandArgument='<%# Container.DataItemIndex %>' />
                                                                    <asp:ImageButton ID="imgCollapse" runat="server" ImageUrl="../Images/Minus.gif" CommandName="Collapse" Visible="false"
                                                                        CommandArgument='<%# Container.DataItemIndex %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Left_Align" ItemStyle-Font-Bold="true">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblStatus" runat="server" Text='<%#Bind("status_desc")%>' CssClass="dashboardGridText" />
                                                                    <asp:HiddenField ID="hdnStatusCode" runat="server" Value='<%#Bind("status_code")%>' />
                                                                    <asp:HiddenField ID="hdnRowNum" runat="server" Value='<%#Bind("row_num")%>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="15%" ItemStyle-CssClass="gridItemStyle_Left_Align">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblresp" runat="server" Text='<%#Bind("responsibility_desc")%>' CssClass="dashboardGridText" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="8%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblNumOfStmts" runat="server" Text='<%#Bind("num_of_stmts")%>' CssClass="dashboardGridText" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="8%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblOwners" runat="server" Text='<%#Bind("owners")%>' CssClass="dashboardGridText" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="8%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRoyaltors" runat="server" Text='<%#Bind("royaltors")%>' CssClass="dashboardGridText" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="8%" ItemStyle-CssClass="gridItemStyle_Right_Align">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblEarnings" runat="server" Text='<%#Bind("earnings")%>' CssClass="dashboardGridText" />
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
                    </td>
                    <td colspan="2" width="15%"></td>

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
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


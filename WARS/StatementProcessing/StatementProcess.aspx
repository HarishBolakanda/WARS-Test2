<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StatementProcess.aspx.cs" Inherits="WARS.StatementProcess" MasterPageFile="~/MasterPage.Master"
    Title="WARS - StatementRun" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>
<%--<%@ Register TagPrefix="nextRun" TagName="StmtNextRunDetails" Src="~/StmtNextRunDetails.ascx" %>--%>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //to open royaltor activity screen
        function OpenActivityScreen() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../StatementProcessing/RoyaltorActivity.aspx', '_self');
            }
            else {
                var win = window.open('../StatementProcessing/RoyaltorActivity.aspx', '_self');
                win.focus();
            }
        }

        //to open workflow screen
        function OpenWorkflowScreen() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../StatementProcessing/WorkFlow.aspx', '_self');
            }
            else {
                var win = window.open('../StatementProcessing/WorkFlow.aspx', '_self');
                win.focus();
            }
        }

    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 1px;">
                        <asp:Button ID="btnOpenStatementActivity" runat="server" CssClass="LinkButtonStyle"
                            OnClientClick="OpenActivityScreen();" Text="Statement Activity" UseSubmitBehavior="false"
                            Width="98%" CausesValidation="false" />
                    </td>
                </tr>
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 1px;">
                        <asp:Button ID="btnOpenWorkflow" runat="server" CssClass="LinkButtonStyle"
                            OnClientClick="OpenWorkflowScreen();" Text="Workflow"
                            UseSubmitBehavior="false" Width="98%" CausesValidation="false" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderBody">
    <script type="text/javascript">
        var gridClientId = "ContentPlaceHolderBody_gvStmtRun_";
        //debugger;
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
            var gridPanelHeight = windowHeight * 0.58;
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
            else if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
    }

    //Tab key to remain only on screen fields
    function OnTabPress() {
        if (event.keyCode == 9) {
            document.getElementById("<%= lblTab.ClientID %>").focus();
        }
    }

    //=============== End
    var valStatementRunControlId;
    function ValidateStatementRun(gridRow) {
        var selectedRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
        hdnStatementRun = document.getElementById(gridClientId + 'hdnStatementRun' + '_' + selectedRowIndex).value;
        valStatementRun = document.getElementById(gridClientId + 'cbRunStmt' + '_' + selectedRowIndex)

        if (valStatementRun.checked && hdnStatementRun == 'Y') {

            var popup = $find('<%= mpeStatementRunPopup.ClientID %>');
            if (popup != null) {
                popup.show();
            }
        }

        return false;

    }

    function ValidateStatementRunNo() {
        valStatementRun.checked = false;
        var popup = $find('<%= mpeStatementRunPopup.ClientID %>');
            if (popup != null) {
                popup.hide();
            }
            return false;

        }
        //======================End
        function FocusLblKeyPress() {
            document.getElementById("<%= txtStmtEndPeriod.ClientID %>").focus();
        }

        function IsDataChanged() {
            ValidateGridDataChanged();
            var hdnDataChanged = document.getElementById("<%=hdnGridDataChanged.ClientID %>").value;
            if (hdnDataChanged == "Y") {
                return true;
            }
            else {
                return false;
            }
        }

        //Warn on changes made and not saved

        function WarnOnUnSavedData() {
            ValidateGridDataChanged();
            var hdnGridDataChanged = document.getElementById("<%=hdnGridDataChanged.ClientID %>").value;
            if (hdnGridDataChanged == "Y") {
                return warningMsgOnUnSavedData;
            }
        }
        window.onbeforeunload = WarnOnUnSavedData;

        function ValidateGridDataChanged() {
            gvStmtRun = document.getElementById("<%= gvStmtRun.ClientID %>");            
            var checkedCount = 0;            
            if (gvStmtRun != null) {
                var gvRows = gvStmtRun.rows;
                for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                    rowIndex = i - 1; //WUIN-746 row index start from 0
                    cbRunStmt = document.getElementById("ContentPlaceHolderBody_gvStmtRun_" + 'cbRunStmt' + '_' + rowIndex);
                    cbRerunStmt = document.getElementById("ContentPlaceHolderBody_gvStmtRun_" + 'cbRerunStmt' + '_' + rowIndex);
                    cbReprintDetail = document.getElementById("ContentPlaceHolderBody_gvStmtRun_" + 'cbReprintDetail' + '_' + rowIndex);
                    cbReprintSummary = document.getElementById("ContentPlaceHolderBody_gvStmtRun_" + 'cbReprintSummary' + '_' + rowIndex);
                    cbArchiveStmt = document.getElementById("ContentPlaceHolderBody_gvStmtRun_" + 'cbArchiveStmt' + '_' + rowIndex);

                    if (cbRunStmt != null && !(cbRunStmt.disabled) && cbRunStmt.checked) {
                        checkedCount = checkedCount + 1;
                        break;
                    }

                    if (cbRerunStmt != null && !(cbRerunStmt.disabled) && cbRerunStmt.checked) {
                        checkedCount = checkedCount + 1;
                        break;
                    }

                    if (cbReprintDetail != null && !(cbReprintDetail.disabled) && cbReprintDetail.checked) {
                        checkedCount = checkedCount + 1;
                        break;
                    }

                    if (cbReprintSummary != null && !(cbReprintSummary.disabled) && cbReprintSummary.checked) {
                        checkedCount = checkedCount + 1;
                        break;
                    }

                    if (cbArchiveStmt != null && !(cbArchiveStmt.disabled) && cbArchiveStmt.checked) {
                        checkedCount = checkedCount + 1;
                        break;
                    }
                }
            }


            if (checkedCount > 0 ) {
                document.getElementById("<%= hdnGridDataChanged.ClientID %>").value = "Y";
            }
        }


    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="6">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    STATEMENT PROCESS
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="12%">
                        <asp:Label ID="Label8" runat="server" Text="Statement End Period" CssClass="identifierLable_large_bold" TabIndex="99"></asp:Label>
                    </td>
                    <td width="15%">
                        <asp:TextBox ID="txtStmtEndPeriod" runat="server" Width="65px" CssClass="identifierLable" OnTextChanged="txtStmtEndPeriod_TextChanged"
                            placeholder="mm/yyyy" AutoPostBack="true" onkeydown="ExcecuteEnterPress();"></asp:TextBox>
                        <ajaxToolkit:MaskedEditExtender ID="maskEditStmtPeriod" runat="server"
                            TargetControlID="txtStmtEndPeriod" Mask="99/9999" AcceptNegative="None"
                            ClearMaskOnLostFocus="false" />
                        <ajaxToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtStmtEndPeriod"
                            WatermarkText="MM/YYYY">
                        </ajaxToolkit:TextBoxWatermarkExtender>
                    </td>
                    <td width="8%"></td>
                    <td align="right" width="42%">
                        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CssClass="ButtonStyle" OnClick="btnRefresh_Click" Width="27%" UseSubmitBehavior="false"
                            TabIndex="101" />
                    </td>
                    <td width="21%" align="right">&nbsp;</td>
                </tr>
                <tr>
                    <td width="2%"></td>

                    <td colspan="2" align="right">
                        <ajaxToolkit:MaskedEditValidator ID="valMaskEditStmtPeriod" runat="server" ControlExtender="maskEditStmtPeriod" ControlToValidate="txtStmtEndPeriod"
                            ValidationExpression="^((0[1-9])|(1[0-2]))\/(\d{4})$" CssClass="identifierLable"
                            InvalidValueMessage="Please enter valid month and year in MM/YYYY format" IsValidEmpty="False" ForeColor="Red"></ajaxToolkit:MaskedEditValidator>
                    </td>
                    <td></td>
                    <td align="right">
                        <asp:Button ID="btnArchive" runat="server" CssClass="ButtonStyle" OnClick="btnArchive_Click" Text="Request Archive"
                            UseSubmitBehavior="false" Width="27%" TabIndex="102" />

                    </td>
                    <td width="21%" align="right">&nbsp;</td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td colspan="3"></td>
                    <td runat="server" id="tdButtonStmt" align="right">
                        <asp:Button ID="btnProcessStmts" runat="server" Text="Schedule Statement Run" CssClass="ButtonStyle" OnClick="btnProcessStmts_Click"
                            UseSubmitBehavior="false" Width="27%" TabIndex="103" />
                    </td>
                    <td width="21%"></td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td colspan="3"></td>
                    <td align="right">
                        <asp:Button ID="btnReprint" runat="server" Text="Start Reprint" CssClass="ButtonStyle" OnClick="btnReprint_Click"
                            UseSubmitBehavior="false" Width="27%" TabIndex="104" onkeydown="OnTabPress();" />
                    </td>
                    <td width="21%"></td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="4" align="left" runat="server" id="tdGrid">
                        <table width="101.5%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td >
                                    <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="100%">
                                        <asp:GridView ID="gvStmtRun" runat="server" AutoGenerateColumns="False" Width="98.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                            CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" ShowHeaderWhenEmpty="true" EmptyDataText="No Data Found"
                                            OnRowDataBound="gvStmtRun_RowDataBound" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" AllowSorting="true" OnSorting="gvStmtRun_Sorting" HeaderStyle-CssClass="FixedHeader">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Reporting Schedule" SortExpression="statement_type_code">
                                                    <ItemTemplate>
                                                        <asp:Label ID="hdnStmtPeriodID" runat="server" Text='<%#Bind("statement_period_id")%>' Visible="false"></asp:Label>
                                                        <asp:Label ID="lblRepSchedule" runat="server" Text='<%#Bind("statement_type_code")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="15%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Statement Period" SortExpression="stmt_period">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStmtPeriod" runat="server" Text='<%#Bind("stmt_period")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="8%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Run Statements" SortExpression="run_stmt">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnStatementRun" runat="server" Value='<%# Bind("statement_status") %>' />
                                                        <asp:Label ID="hdnRunStmt" runat="server" Text='<%#Bind("run_stmt")%>' Visible="false" />
                                                        <asp:CheckBox ID="cbRunStmt" runat="server" onclick="ValidateStatementRun(this);" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="8%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Rerun Statements" SortExpression="rerun_stmt">
                                                    <ItemTemplate>
                                                        <asp:Label ID="hdnRerunStmt" runat="server" Text='<%#Bind("rerun_stmt")%>' Visible="false" />
                                                        <asp:CheckBox ID="cbRerunStmt" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="8%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Reprint Detail PDFs" SortExpression="rerun_stmt">
                                                    <ItemTemplate>
                                                        <asp:Label ID="hdnReprintDetail" runat="server" Text='<%#Bind("rerun_stmt")%>' Visible="false" />
                                                        <asp:CheckBox ID="cbReprintDetail" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="8%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Reprint Summary PDFs" SortExpression="rerun_stmt">
                                                    <ItemTemplate>
                                                        <asp:Label ID="hdnReprintSummary" runat="server" Text='<%#Bind("rerun_stmt")%>' Visible="false" />
                                                        <asp:CheckBox ID="cbReprintSummary" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="8%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Archive Statements" SortExpression="archive_stmt">
                                                    <ItemTemplate>
                                                        <asp:Label ID="hdnArchiveStmt" runat="server" Text='<%#Bind("archive_stmt")%>' Visible="false" />
                                                        <asp:CheckBox ID="cbArchiveStmt" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Scheduled Date" SortExpression="trigger_date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTriggerDate" runat="server" Text='<%#Bind("trigger_date")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="12%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="User" SortExpression="user_code">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblUser" runat="server" Text='<%#Bind("user_code")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="12%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Status" SortExpression="status_desc">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStatus" runat="server" Text='<%#Bind("status_desc")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
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

            <asp:Button ID="dummyConfirm" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirm" runat="server" PopupControlID="pnlPopup" TargetControlID="dummyConfirm"
                CancelControlID="btnNo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlPopup" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%" style="z-index: -1">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="Label1" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblConfirmMsg" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYes" runat="server" Text="Yes" CssClass="ButtonStyle" OnClick="btnYes_Click" />
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

            <asp:Button ID="dummyConfirmReprint" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmReprint" runat="server" PopupControlID="pnlPopupConfirmReprint" TargetControlID="dummyConfirmReprint"
                CancelControlID="btnNoConfirmReprint" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlPopupConfirmReprint" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%" style="z-index: -1">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="Label2" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblConfirmMessage" runat="server" Text="Do you want to reprint the selected statements?" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYesConfirmReprint" runat="server" Text="Yes" CssClass="ButtonStyle" OnClick="btnYesConfirmReprint_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNoConfirmReprint" runat="server" Text="No" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:Button ID="dummyStatementRunPopup" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeStatementRunPopup" runat="server" PopupControlID="pnlStatementRunPopup" TargetControlID="dummyStatementRunPopup"
                CancelControlID="btnYesStatementRunPopup" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlStatementRunPopup" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblHdrStatementRunPopup" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMsgStatementRunPopup" runat="server" Text="There is an existing Statement run for this Reporting Schedule that has not been archived. Do you want to continue?"
                                CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYesStatementRunPopup" runat="server" Text="Continue" CssClass="ButtonStyle" CausesValidation="false" />

                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNoStatementRunPopup" runat="server" Text="Cancel" CssClass="ButtonStyle" OnClientClick="return ValidateStatementRunNo();" CausesValidation="false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <%-- WOS-384 - changes - Harish - 10-11-16           
    <asp:Button ID="dummyContinue" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeRunArchive" runat="server" PopupControlID="pnlPopupArchive" TargetControlID="dummyContinue"
                CancelControlID="btnCancel" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlPopupArchive" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid">
                <table width="100%" style="z-index: -1">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="Label2" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblConfirmMessage" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnContinue" runat="server" Text="Yes" CssClass="ButtonStyle" OnClick="btnContinue_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnCancel" runat="server" Text="No" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>--%>
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField" onkeydown="FocusLblKeyPress();"></asp:TextBox>
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />          
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
            <asp:HiddenField ID="hdnGridDataChanged" runat="server" Value="N" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

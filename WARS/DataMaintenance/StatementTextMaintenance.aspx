<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="StatementTextMaintenance.aspx.cs" Inherits="WARS.StatementTextMaintenance" MasterPageFile="~/MasterPage.Master"
    Title="WARS - StatementTextMaintenance" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>


<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderBody">

    <script type="text/javascript">
        var gridClientId = "ContentPlaceHolderBody_gvStatementTextDetails_";

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
            var pnlGrid = document.getElementById("<%=PnlStatementTextDetails.ClientID %>");
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
            if (pnlGrid != null) {
                pnlGrid.style.height = gridPanelHeight + "px";
            }
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
        }
        //======================= End     

        //Show warning while closing the window if changed data not saved 
        function WarnOnUnSavedData() {
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            if (isExceptionRaised != "Y") {
                if (IsDataChanged()) {
                    return warningMsgOnUnSavedData;
                }
            }
        }
        window.onbeforeunload = WarnOnUnSavedData;

        function IsDataChanged() {
            if (IsGridDataChanged()) {
                return true;
            }
            else {
                return false;
            }
        }

        //Validate any unsaved data on browser window close/refresh
        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }
        //======================= End

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        //=============== End

        function IsGridDataChanged() {
            var isGridDataChanged = "N";
            var gvStatementText = document.getElementById("<%= gvStatementTextDetails.ClientID %>");
            if (gvStatementText != null) {
                var gvRows = gvStatementText.rows; // WUIN-746 grid view rows including header row
                var rowIndex;
                for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                    rowIndex = i - 1; //WUIN-746 row index start from 0

                    //handling empty data row
                    if (gvRows.length == 2 && document.getElementById(gridClientId + 'txtStatementText' + '_' + rowIndex) == null) {
                        break;
                    }

                    var hdnFieldText = document.getElementById(gridClientId + 'hdnFieldText' + '_' + rowIndex).value;
                    var txtStatementText = document.getElementById(gridClientId + 'txtStatementText' + '_' + rowIndex).value;

                    if (hdnFieldText != txtStatementText) {
                        isGridDataChanged = "Y";
                        break;
                    }
                }
            }

            if (isGridDataChanged == "Y") {
                return true;
            }
            else {
                return false;
            }

        }
        //Undo Grid changes
        function UndoChanges(gridRow) {
            gridRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
            document.getElementById(gridClientId + 'txtStatementText' + '_' + gridRowIndex).innerText = document.getElementById(gridClientId + 'hdnFieldText' + '_' + gridRowIndex).value;

            return false;
        }

        function ValidateReset(button) {
            if (document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value == "Y") {
                return true;
            }
            document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "N";

            if (IsGridDataChanged()) {
                window.onbeforeunload = null;
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
                return false;
            }
            else {
                return true;
            }
        }

        function OnCompanyChange() {
            if (IsGridDataChanged()) {
                OpenOnUnSavedData();
                return false;
            }
            else {
                document.getElementById('<%=btnCompanyChange.ClientID%>').click();
            }
        }

        function OpenOnUnSavedData() {
            var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
            if (warnPopup != null) {
                document.getElementById("<%=lblUnSavedWarnMsg.ClientID %>").innerText = warningMsgOnUnSavedData;
                warnPopup.show();

            }
        }

        function OnUnSavedDataReturn() {

            var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
            if (warnPopup != null) {
                warnPopup.hide();
            }
            window.onbeforeunload = WarnOnUnSavedData;
            return false;
        }

        function OnUnSavedDataExit() {
            document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "Y";
            window.onbeforeunload = WarnOnUnSavedData;
        }

        //Validations ============= Start
        function ValidateSave() {
            //check if no changes made to save
            if (!IsGridDataChanged()) {
                DisplayMessagePopup("No changes made to save!");
                return false;
            }
            else {
                return true;
            }
        }

    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="7">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    STATEMENT TEXT MAINTENANCE
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="8%"></td>
                    <td width="5%" class="identifierLable_large_bold">Company</td>
                    <td width="15%">
                        <asp:DropDownList ID="ddlCompany" runat="server" Width="75%" CssClass="ddlStyle" onchange="return OnCompanyChange();" TabIndex="100">
                        </asp:DropDownList>
                    </td>
                    <td width="59%"></td>
                    <td width="3%"></td>
                    <td align="right" width="10%">
                        <asp:Button ID="btnReset" runat="server" CssClass="ButtonStyle" TabIndex="101" OnClientClick="if(!ValidateReset('Reset')) {return false;};" Text="Reset" Width="98%" UseSubmitBehavior="false" OnClick="btnReset_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="5"></td>
                    <td align="right" width="12%">
                        <asp:Button ID="btnSave" runat="server" CssClass="ButtonStyle" OnClientClick="if(!ValidateSave()) {return false;};" OnClick="btnSaveChanges_Click"
                            Text="Save Changes" UseSubmitBehavior="false" Width="98%" TabIndex="102" onkeydown="OnTabPress();" />
                    </td>
                </tr>
                <tr>
                    <td width="8%"></td>
                    <td colspan="3" class="table_header_with_border">Statement Text details </td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="3" class="table_with_border" align="center">
                        <table width="80%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlStatementTextDetails" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvStatementTextDetails" runat="server" AutoGenerateColumns="False" Width="98%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True" OnRowDataBound="gvStatementTextDetails_RowDataBound"
                                                        EmptyDataText="No data found." AllowSorting="true" OnSorting="gvStatementTextDetails_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-Width="7%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Field ID" SortExpression="field_id">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblFieldID" runat="server" Text='<%# Bind("field_id") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="35%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Field Description" SortExpression="field_description">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblFieldDescription" runat="server" Text='<%# Bind("field_description") %>' CssClass="identifierLable">
                                                                    </asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="52%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Statement Text" SortExpression="field_text">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnFieldText" runat="server" Value='<%# Bind("field_text")%>' />
                                                                    <asp:TextBox ID="txtStatementText" runat="server" Text='<%# Bind("field_text") %>'
                                                                        CssClass="gridTextField" Width="97%" MaxLength="150"></asp:TextBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="6%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:ImageButton ID="imgBtnUndo" CausesValidation="false" runat="server" OnClientClick="return UndoChanges(this);" ImageUrl="../Images/cancel_row3.png"
                                                                        ToolTip="Cancel" />
                                                                    <asp:HiddenField ID="hdnFieldId" runat="server" Value='<%# Bind("field_id") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
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

            <%--Warning on unsaved data popup--%>
            <asp:Button ID="dummyUnsavedWarnMsg" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeUnSavedWarning" runat="server" PopupControlID="pnlUnsavedWarnMsgPopup" TargetControlID="dummyUnsavedWarnMsg"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlUnsavedWarnMsgPopup" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmOnUnsavedData" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblUnSavedWarnMsg" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td width="48%" align="right">
                                        <asp:Button ID="btnUnSavedDataReturn" runat="server" Text="Return" CssClass="ButtonStyle" Width="30%" OnClientClick="return OnUnSavedDataReturn();" />
                                    </td>
                                    <td width="4%"></td>
                                    <td width="48%" align="left">
                                        <asp:Button ID="btnUnSavedDataExit" runat="server" Text="Exit" CssClass="ButtonStyle" Width="30%" OnClientClick="OnUnSavedDataExit();"
                                            OnClick="btnUnSavedDataExit_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsConfirmPopup" runat="server" Value="N" />
            <asp:HiddenField ID="hdnButtonSelection" runat="server" />
            <asp:HiddenField ID="hdnGridRowSelectedPrvious" runat="server" />
            <asp:Button ID="btnCompanyChange" runat="server" Style="display: none;" OnClick="ddlCompany_SelectedIndexChanged" CausesValidation="false" />
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:TextBox>
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExchangeRateFactors.aspx.cs" Inherits="WARS.ExchangeRateFactors" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Exchange Rates" MaintainScrollPositionOnPostback="true" %>

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

            //to maintain scroll position
            postBackElementID = args.get_postBackElement().id;
            if (postBackElementID.lastIndexOf('imgBtnSave') > 0 || postBackElementID.lastIndexOf('imgBtnUndo') > 0 || postBackElementID.lastIndexOf('btnSaveChanges') > 0 || postBackElementID.lastIndexOf('btnUndoChanges') > 0) {
                xPos = sender._scrollPosition.x;
                yPos = sender._scrollPosition.y;

                //hold scroll position on selecting expand/collapse
                var PnlExchangeRateDetails = document.getElementById("<%=PnlExchangeRateDetails.ClientID %>");
                scrollTop = PnlExchangeRateDetails.scrollTop;

            }
        }

        function EndRequestHandler(sender, args) {
            //Hide the modal popup - the update progress
            var popup = $find('<%= mPopupPageLevel.ClientID %>');
            if (popup != null) {
                popup.hide();
            }

            //to maintain scroll position
            postBackElementID = sender._postBackSettings.sourceElement.id;
            if (postBackElementID.lastIndexOf('imgBtnSave') > 0 || postBackElementID.lastIndexOf('imgBtnUndo') > 0 || postBackElementID.lastIndexOf('btnSaveChanges') > 0 || postBackElementID.lastIndexOf('btnUndoChanges') > 0) {
                window.scrollTo(xPos, yPos);

                //set scroll position on selecting expand/collapse
                var PnlExchangeRateDetails = document.getElementById("<%=PnlExchangeRateDetails.ClientID %>");
                PnlExchangeRateDetails.scrollTop = scrollTop;
            }


        }
        //======================= End       

        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.55;
            document.getElementById("<%=PnlExchangeRateDetails.ClientID %>").style.height = gridPanelHeight + "px";
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
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        //=============== End

        //set flag value when data is changed in grid 
        function OnDataChange(row) {
            CompareRow(row);
        }


        function CompareRow(row) {
            //debugger;
            var rowIndex = row.id.substring(row.id.lastIndexOf('_') + 1);
            var tempstr = row.id.substring(0, row.id.lastIndexOf('_'));
            var str = tempstr.substring(0, tempstr.lastIndexOf('_') + 1);

            var hdnExchangeRateFactor = document.getElementById(str + 'hdnExchangeRateFactor_' + rowIndex).value;
            var txtExchangeRateFactor = document.getElementById(str + 'txtExchangeRateFactor_' + rowIndex).value;

            if (hdnExchangeRateFactor != txtExchangeRateFactor) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
            }
            else {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "N";
            }

        }

        //set flag value when data is changed in textboxes for inserting new row

        function OnDataChangeInsert() {
            CompareRowInsert();
        }

        function CompareRowInsert() {
            //debugger;
            var txtTransactionReceiptDate = document.getElementById("<%=txtTransactionReceiptDate.ClientID %>").value;
            var txtExchangeRateFactor = document.getElementById("<%=txtExchangeRateFactor.ClientID %>").value;

            if (txtTransactionReceiptDate != "__/____" || txtExchangeRateFactor != "") {
                document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").innerText = "Y";
            }
            else {
                document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").innerText = "N";
            }
        }

        //Show warning while closing the window if changed data not saved 
        function WarnOnUnSavedData() {
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            var isDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
            var isDataChangedInsert = document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value;
            if (isExceptionRaised != "Y" && (isDataChanged == "Y" || isDataChangedInsert == "Y")) {
                return warningMsgOnUnSavedData;
            }
        }
        window.onbeforeunload = WarnOnUnSavedData;
        function ValidateChanges() {
            if (!(WarnOnUnSavedData.length > 0)) {
                eval(this.href);
            }
        }
        //used to check if any changes to allow navigation to other screen 
        function IsDataChanged() {
            var isDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
            var isDataChangedInsert = document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value;
            if ((isDataChanged == "Y" || isDataChangedInsert == "Y")) {
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

        //Validation: warning message if changes made and not saved

        function OnGridRowSelected(row) {
            var rowData = row.parentNode.parentNode;
            var rowIndex = rowData.rowIndex - 1;

            if (document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value == "Y") {
                var popup = $find('<%= mpeSaveUndo.ClientID %>');
                if (popup != null) {
                    document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new exchange rate factor. Save or Undo changes";
                    popup.show();
                    $get("<%=btnUndoChanges.ClientID%>").focus();
                }
            }
            else {
                if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value == "") {
                    document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").innerText = rowIndex;
                }
                else if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value != rowIndex) {
                    if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
                        var popup = $find('<%= mpeSaveUndo.ClientID %>');
                        if (popup != null) {
                            popup.show();
                            $get("<%=btnUndoChanges.ClientID%>").focus();
                        }
                    }
                }
        }
    }

    function ConfirmSearch() {
        if (document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value == "Y") {
            var popup = $find('<%= mpeSaveUndo.ClientID %>');
            if (popup != null) {
                document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new exchange rate factor. Save or Undo changes";
                popup.show();
                $get("<%=btnUndoChanges.ClientID%>").focus();
                return false;
            }
        }
        else {
            if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
                var popup = $find('<%= mpeSaveUndo.ClientID %>');
                if (popup != null) {
                    popup.show();
                    $get("<%=btnUndoChanges.ClientID%>").focus();
                }
                return false;
            }
            else {
                return true;
            }
        }
    }

    function ConfirmInsert() {
        if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
            var popup = $find('<%= mpeSaveUndo.ClientID %>');
            if (popup != null) {
                popup.show();
                $get("<%=btnUndoChanges.ClientID%>").focus();
            }
            return false;
        }
        else {
            return true;
        }
    }

    function ConfirmUpdate(row) {
        var rowData = row.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
        var rowIndex = rowData.rowIndex - 1;

        if (document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value == "Y") {
            var popup = $find('<%= mpeSaveUndo.ClientID %>');
            if (popup != null) {
                document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new exchange rate factor. Save or Undo changes";
                popup.show();
                $get("<%=btnUndoChanges.ClientID%>").focus();
                return false;
            }
        }
        else {
            if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value == "") {
                return true;
            }
            else if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value != rowIndex) {
                if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
                    var popup = $find('<%= mpeSaveUndo.ClientID %>');
                    if (popup != null) {
                        popup.show();
                        $get("<%=btnUndoChanges.ClientID%>").focus();
                    }
                    return false;
                }
                else {
                    return true;
                }
            }
            else if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value == rowIndex) {
                return true;
            }
    }
}

function ConfirmSearch() {
    if (document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value == "Y") {
        var popup = $find('<%= mpeSaveUndo.ClientID %>');
        if (popup != null) {
            document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new exchange rate factor. Save or Undo changes";
                    popup.show();
                    $get("<%=btnUndoChanges.ClientID%>").focus();
                return false;
            }
        }
        else {
            if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
            var popup = $find('<%= mpeSaveUndo.ClientID %>');
                if (popup != null) {
                    popup.show();
                    $get("<%=btnUndoChanges.ClientID%>").focus();
                }
                return false;
            }
            else {
                return true;
            }
        }
    }

    //clear add row data
    function ClearAddRow() {
        //debugger;
        document.getElementById('<%=txtTransactionReceiptDate.ClientID%>').value = "";
        document.getElementById('<%=txtExchangeRateFactor.ClientID%>').value = "";
        document.getElementById('<%=hdnInsertDataNotSaved.ClientID%>').value = 'N';
        Page_ClientValidate('');//clear all validators of the page
        document.getElementById("<%= txtTransactionReceiptDate.ClientID %>").focus();
    return false;

}
//============== End  

//Disable back button on royaltor id textbox for existing royaltor
function MoveFocus() {
    document.getElementById("<%=txtTransactionReceiptDate.ClientID %>").focus();;
}

//=================End
    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="11">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    EXCHANGE RATES
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="1%"></td>
                    <td width="7%" class="identifierLable_large_bold">Company</td>
                    <td width="16%">
                        <asp:DropDownList ID="ddlCompany" runat="server" Width="96%" CssClass="ddlStyle" AutoPostBack="true"
                            OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged" OnFocus="return ConfirmSearch();">
                        </asp:DropDownList>
                        <asp:TextBox ID="txtCompany" runat="server" Width="96%" CssClass="textboxStyle_readonly"
                            ReadOnly="true" OnFocus="MoveFocus();"></asp:TextBox>
                    </td>
                    <td width="1%"></td>
                    <td width="12%" class="identifierLable_large_bold">Domestic Currency</td>
                    <td width="8%">
                        <asp:TextBox ID="txtDomesticCurrency" runat="server" Width="65px" CssClass="textboxStyle_readonly"
                            ReadOnly="true" OnFocus="MoveFocus();"></asp:TextBox>
                    </td>
                    <td width="14%" class="identifierLable_large_bold">Domestic Currency Grouping</td>
                    <td width="6.5%">
                        <asp:TextBox ID="txtDomesticCurrencyGrouping" runat="server" Width="65px" CssClass="textboxStyle_readonly"
                            ReadOnly="true" OnFocus="MoveFocus();"></asp:TextBox>
                    </td>
                    <td width="3%"></td>
                    <td></td>
                    <td width="12%"></td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td class="identifierLable_large_bold">Transaction Receipt Date</td>
                    <td>
                        <asp:TextBox ID="txtTransactionReceiptDate" runat="server" Width="65px" CssClass="textboxStyle"
                            TabIndex="100" onchange="javascript: OnDataChangeInsert();" onfocus="return ConfirmInsert();"></asp:TextBox>
                        <asp:CustomValidator ID="valTransactionReceiptDate" runat="server" ValidationGroup="valInsertExchangeRates" CssClass="requiredFieldValidator"
                            OnServerValidate="valTransactionReceiptDate_ServerValidate" ErrorMessage="*" ToolTip="Please enter a valid date in DD/MM/YYYY format">
                        </asp:CustomValidator>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmeTransactionReceiptDate" runat="server" TargetControlID="txtTransactionReceiptDate"
                            WatermarkText="mm/yyyy" WatermarkCssClass="waterMarkText">
                        </ajaxToolkit:TextBoxWatermarkExtender>
                        <ajaxToolkit:MaskedEditExtender ID="mteTransactionReceiptDate" runat="server"
                            TargetControlID="txtTransactionReceiptDate" Mask="99/9999" AcceptNegative="None"
                            ClearMaskOnLostFocus="false" />
                    </td>
                    <td class="identifierLable_large_bold">Exchange Rate Factor</td>
                    <td>
                        <asp:TextBox ID="txtExchangeRateFactor" runat="server" Width="65px" CssClass="textboxStyle"
                            TabIndex="101" onchange="javascript: OnDataChangeInsert();" onfocus="return ConfirmInsert();"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ID="rfvExchangeRateFactor" ControlToValidate="txtExchangeRateFactor" ValidationGroup="valInsertExchangeRates"
                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter exchange rate factor" Display="Dynamic"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="revExchangeRateFactor" runat="server" Text="*" ControlToValidate="txtExchangeRateFactor" ValidationGroup="valInsertExchangeRates"
                            ValidationExpression="^[-+]?\d*\.{0,1}\d+$" CssClass="requiredFieldValidator" ForeColor="Red"
                            ToolTip="Please enter only number" Display="Dynamic"> </asp:RegularExpressionValidator>
                    </td>
                    <td align="center" valign="middle">
                        <table width="100%" style="float: right; table-layout: fixed">
                            <tr style="float: right">
                                <td align="right" style="float: right" width="50%">
                                    <asp:ImageButton ID="imgBtnInsert" runat="server" CommandName="saverow" TabIndex="102" ImageUrl="../Images/save.png" ToolTip="Insert exchange rate factor" OnClick="imgBtnInsert_Click" OnClientClick="return ConfirmInsert();" />
                                </td>
                                <td align="right" style="float: right" width="50%">
                                    <asp:ImageButton ID="imgBtnCancel" runat="server" CommandName="cancelrow" TabIndex="103" ImageUrl="../Images/cancel_row3.png"
                                        ToolTip="Cancel" OnClientClick="return ClearAddRow();" OnKeyDown="OnTabPress()" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="12">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td colspan="6" runat="server" id="tdData">
                        <table width="60%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:Panel ID="PnlExchangeRateDetails" runat="server" ScrollBars="Auto" Width="100%">
                                        <asp:GridView ID="gvExchangeRateDetails" runat="server" AutoGenerateColumns="False" Width="96%" AlternatingRowStyle-BackColor="#E3EFFF"
                                            CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                            EmptyDataText="No data found." OnRowDataBound="gvExchangeRateDetails_RowDataBound" OnRowCommand="gvExchangeRateDetails_RowCommand"  AllowSorting="true" OnSorting="gvExchangeRateDetails_Sorting" HeaderStyle-CssClass="FixedHeader">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Transaction Receipt Date" SortExpression="recdate_month_id">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnCompanyCode" runat="server" Value='<%# Bind("company_code") %>' />
                                                        <asp:HiddenField ID="hdnMonthId" runat="server" Value='<%# Bind("recdate_month_id") %>' />
                                                        <asp:Label ID="lblTransactionReceiptDate" runat="server" Text='<%# Bind("month_name") %>' CssClass="identifierLable"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="40%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Exchange Rate Factor" SortExpression="exchrate_factor_sort">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnExchangeRateFactor" runat="server" Value='<%# Bind("exchrate_factor") %>' />
                                                        <asp:TextBox ID="txtExchangeRateFactor" runat="server" Text='<%# Eval("exchrate_factor") %>'
                                                            CssClass="gridTextField" Width="25%" onchange="javascript: OnDataChange(this);" onfocus="OnGridRowSelected(this)"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ID="rfExchangeRateFactor" ControlToValidate="txtExchangeRateFactor" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>'
                                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter exchange rate factor" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="revExchangeRateFactor" runat="server" Text="*" ControlToValidate="txtExchangeRateFactor" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>'
                                                            ValidationExpression="^[-+]?\d*\.{0,1}\d+$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                            ToolTip="Please enter only number" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="50%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Right_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderStyle-Width="10%">
                                                    <ItemTemplate>
                                                        <table width="100%" style="float: right; table-layout: fixed">
                                                            <tr style="float: right">
                                                                <td align="right" style="float: right" width="50%">
                                                                    <asp:ImageButton ID="imgBtnSave" runat="server" CommandName="saverow" ImageUrl="../Images/save.png" ToolTip="Save" CausesValidation="true"
                                                                        OnClientClick="return ConfirmUpdate(this)" />
                                                                </td>
                                                                <td align="right" style="float: right" width="50%">
                                                                    <asp:ImageButton ID="imgBtnUndo" runat="server" CommandName="cancelrow" ImageUrl="../Images/cancel_row3.png" OnClientClick="return ConfirmInsert();"
                                                                        ToolTip="Cancel" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:Repeater ID="rptPager" runat="server">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>'
                                                OnClientClick="return ValidateChanges();" ClientIDMode="AutoID" CausesValidation="false" Enabled='<%# Eval("Enabled") %>' OnClick="lnkPage_Click" CssClass="gridPager"> </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <br />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td align="right" valign="top">
                        <%--<asp:Button ID="btnUpdateTransactions" runat="server" CausesValidation="false" CssClass="ButtonStyle" OnClick="btnUpdateTransactions_Click" OnKeyDown="OnTabPress()" TabIndex="104" Text="Update Transactions" UseSubmitBehavior="false" Width="98%" />--%>
                    </td>
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

            <%--Save/Undo changes popup--%>
            <asp:Button ID="dummySaveUndo" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeSaveUndo" runat="server" PopupControlID="pnlSaveUndo" TargetControlID="dummySaveUndo"
                CancelControlID="btnClosePopupSaveUndo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlSaveUndo" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td align="right" style="vertical-align: top;">
                            <asp:ImageButton ID="btnClosePopupSaveUndo" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMessage" runat="server" CssClass="identifierLable"
                                Text="You have made changes which are not saved. Save or Undo changes"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnSaveChanges" runat="server" Text="Save" CssClass="ButtonStyle"
                                            OnClick="btnSaveChanges_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnUndoChanges" runat="server" Text="Undo" CssClass="ButtonStyle" OnClick="btnUndoChanges_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:HiddenField ID="hdnPageNumber" runat="server" Value="" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnIsValidSearch" runat="server" Value="N" />
            <asp:HiddenField ID="hdnChangeNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnInsertDataNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridRowSelectedPrvious" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99"></asp:Label>
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

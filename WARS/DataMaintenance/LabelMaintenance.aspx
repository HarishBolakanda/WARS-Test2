<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LabelMaintenance.aspx.cs" Inherits="WARS.LabelMaintenance" MasterPageFile="~/MasterPage.Master"
    Title="WARS - LabelMaintenance " MaintainScrollPositionOnPostback="true" %>

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
            if (postBackElementID.lastIndexOf('gvLabelDetails') > 0 || postBackElementID.lastIndexOf('btnSaveChanges') > 0) {
                xPos = sender._scrollPosition.x;
                yPos = sender._scrollPosition.y;

                //hold scroll position on selecting expand/collapse
                var PnlLabelDetails = document.getElementById("<%=PnlLabelDetails.ClientID %>");
                scrollTop = PnlLabelDetails.scrollTop;

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
            if (postBackElementID.lastIndexOf('gvLabelDetails') > 0 || postBackElementID.lastIndexOf('btnSaveChanges') > 0) {
                window.scrollTo(xPos, yPos);

                //set scroll position on selecting expand/collapse
                var PnlLabelDetails = document.getElementById("<%=PnlLabelDetails.ClientID %>");
                PnlLabelDetails.scrollTop = scrollTop;
            }


        }
        //======================= End       

        //Fuzzy search filters
        var txtLabelSearch;
        function LabelSelected(sender, args) {

            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtLabelSearch.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnIsValidSearch.ClientID %>").value = "Y";
                document.getElementById('<%=btnHdnLabelSearch.ClientID%>').click();
            }
        }

        function LabelListPopulating() {
            txtLabelSearch = document.getElementById("<%= txtLabelSearch.ClientID %>");
            txtLabelSearch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtLabelSearch.style.backgroundRepeat = 'no-repeat';
            txtLabelSearch.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidSearch.ClientID %>").value = "N";
        }

        function LabelListPopulated() {
            txtLabelSearch = document.getElementById("<%= txtLabelSearch.ClientID %>");
            txtLabelSearch.style.backgroundImage = 'none';
        }

        //=============== End

        //set flag value when data is changed in grid 
        //debugger;
        function OnDataChange() {
            document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        }

        function OnkeyDown() {
            if (event.keyCode == 8 || event.keyCode == 46) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
            }
        }

        //set flag value when data is changed in textboxes for inserting new row

        function OnDataChangeInsert() {
            document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").innerText = "Y";
        }

        function OnkeyDownInsert() {
            if (event.keyCode == 8 || event.keyCode == 46) {
                document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").innerText = "Y";
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
                    document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new label. Save or Undo changes";
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
                document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new label. Save or Undo changes";
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
                    document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new label. Save or Undo changes";
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

        function ConfirmDelete(row) {
            var rowData = row.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
            var rowIndex = rowData.rowIndex - 1;
            var tempstr = row.id.substring(0, row.id.lastIndexOf('_'));
            var str = tempstr.substring(0, tempstr.lastIndexOf('_') + 1);
            var labelCode = document.getElementById(str + 'lblLabelCode' + '_' + rowIndex).innerText;
            if (document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value == "Y") {
                var popup = $find('<%= mpeSaveUndo.ClientID %>');
                if (popup != null) {
                    document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new label. Save or Undo changes";
                popup.show();
                $get("<%=btnUndoChanges.ClientID%>").focus();
                return false;
            }
        }
        else {
            if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value == "") {
                    document.getElementById("<%=hdnLabelCode.ClientID %>").innerText = labelCode;
                    var popup = $find('<%= mpeConfirmDelete.ClientID %>');
                    if (popup != null) {
                        popup.show();
                    }
                    return false; //JIRA-908 Changes
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
                        document.getElementById("<%=hdnLabelCode.ClientID %>").innerText = labelCode;
                        var popup = $find('<%= mpeConfirmDelete.ClientID %>');
                        if (popup != null) {
                            popup.show();
                        }
                        return false; //JIRA-908 Changes
                    }
                }
                else if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value == rowIndex) {
                    document.getElementById("<%=hdnLabelCode.ClientID %>").innerText = labelCode;
                    var popup = $find('<%= mpeConfirmDelete.ClientID %>');
                    if (popup != null) {
                        popup.show();
                    }
                    return false; //JIRA-908 Changes
                }
    }
}

//grid panel height adjustment functioanlity - starts

function SetGrdPnlHeightOnLoad() {
    var windowHeight = window.screen.availHeight;
    var gridPanelHeight = windowHeight * 0.5;
    document.getElementById("<%=PnlLabelDetails.ClientID %>").style.height = gridPanelHeight + "px";
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
    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="7">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    LABEL MAINTENANCE
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="8%"></td>
                    <td width="5%" class="identifierLable_large_bold">Label</td>
                    <td width="24%">
                        <asp:TextBox ID="txtLabelSearch" runat="server" Width="99%" CssClass="textboxStyle"
                            TabIndex="100" onfocus="return ConfirmSearch();"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="aceLabelSearch" runat="server"
                            ServiceMethod="FuzzySearchAllLabelList"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtLabelSearch"
                            FirstRowSelected="true"
                            OnClientItemSelected="LabelSelected"
                            OnClientPopulating="LabelListPopulating"
                            OnClientPopulated="LabelListPopulated"
                            OnClientHidden="LabelListPopulated"
                            CompletionListElementID="acePnlLabel" />
                        <asp:Panel ID="acePnlLabel" runat="server" CssClass="identifierLable" />
                    </td>
                    <td width="3%" align="left">
                        <asp:ImageButton ID="fuzzySearchLabel" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button"
                            OnClientClick="if (!ConfirmSearch()) { return false;};" OnClick="fuzzySearchLabel_Click" ToolTip="Search label code/name" />
                    </td>
                    <td></td>
                    <td width="3%"></td>
                    <td align="right" width="12%">
                        <asp:Button ID="btnReset" runat="server" CssClass="ButtonStyle" OnClick="btnReset_Click" TabIndex="101" Text="Reset" Width="98%" UseSubmitBehavior="false"
                            OnClientClick="if (!ConfirmSearch()) { return false;};" />
                    </td>
                </tr>
                <tr>
                    <td colspan="7">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="4" class="table_with_border">
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
                                                <asp:Panel ID="PnlLabelDetails" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvLabelDetails" runat="server" AutoGenerateColumns="False" Width="97%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." OnRowCommand="gvLabelDetails_RowCommand" OnRowDataBound="gvLabelDetails_RowDataBound" AllowSorting="true" OnSorting="gvLabelDetails_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Label" SortExpression="label_code">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblLabelCode" runat="server" Text='<%# Bind("label_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Description" SortExpression="label_description">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblLabelName" runat="server" Text='<%# Bind("label_description") %>' CssClass="identifierLable" Visible="false"></asp:Label>
                                                                    <asp:TextBox ID="txtLabelName" runat="server" Text='<%# Eval("label_description") %>'
                                                                        CssClass="gridTextField" Width="97%" MaxLength="50" onchange="javascript: OnDataChange();" onKeyPress="javascript: OnDataChange();"
                                                                        onkeydown="javascript: OnkeyDown();" onfocus="OnGridRowSelected(this)"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvOwneName" ControlToValidate="txtLabelName" ValidationGroup="valUpdateLabel"
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter label description" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="70%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Right_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <table width="100%" style="float: right; table-layout: fixed">
                                                                        <tr style="float: right">
                                                                            <td align="right" style="float: right" width="33%">
                                                                                <asp:ImageButton ID="imgBtnSave" runat="server" CommandName="saverow" ImageUrl="../Images/save.png" ToolTip="Save"
                                                                                    ValidationGroup="valUpdateLabel" OnClientClick="return ConfirmUpdate(this);" />
                                                                            </td>
                                                                            <td align="right" style="float: right" width="33%">
                                                                                <asp:ImageButton ID="imgBtnDelete" runat="server" CommandName="deleterow" ImageUrl="../Images/Delete.gif"
                                                                                    ToolTip="Delete" OnClientClick="return ConfirmDelete(this);" />
                                                                            </td>
                                                                            <td align="right" style="float: right" width="33%">
                                                                                <asp:ImageButton ID="imgBtnUndo" runat="server" CommandName="cancelrow" ImageUrl="../Images/cancel_row3.png"
                                                                                    ToolTip="Cancel" OnClientClick="if (!ConfirmUpdate(this)) { return false;};" />
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
                                            <td>
                                                <br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <table width="97%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="20%" class="gridHeaderStyle_1row">Label</td>
                                                        <td width="70%" class="gridHeaderStyle_1row">Description</td>
                                                        <td width="10%" class="gridHeaderStyle_1row">&nbsp</td>
                                                    </tr>
                                                    <tr>
                                                        <td width="20%" align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:TextBox ID="txtLabelCode" runat="server" CssClass="textboxStyle" TabIndex="102" Width="50%" MaxLength="5" onfocus="return ConfirmInsert();"
                                                                onchange="javascript: OnDataChangeInsert();" onKeyPress="javascript: OnDataChangeInsert();" onkeydown="javascript: OnkeyDownInsert();"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvLabeCode" ControlToValidate="txtLabelCode" ValidationGroup="valInsertLabel"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter label code" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td width="70%" class="insertBoxStyle">
                                                            <asp:TextBox ID="txtLabelDesc" runat="server" CssClass="textboxStyle" TabIndex="102" Width="97%" MaxLength="50" onfocus="return ConfirmInsert();"
                                                                onchange="javascript: OnDataChangeInsert();" onKeyPress="javascript: OnDataChangeInsert();" onkeydown="javascript: OnkeyDownInsert();"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvOwneDesc" ControlToValidate="txtLabelDesc" ValidationGroup="valInsertLabel"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter label description" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td width="10%" class="insertBoxStyle_No_Padding">
                                                            <table width="99%" style="float: right; table-layout: fixed">
                                                                <tr style="float: right">
                                                                    <td align="right" style="float: right" width="31%">
                                                                        <asp:ImageButton ID="imgBtnInsert" runat="server" CommandName="saverow" ImageUrl="../Images/save.png" ToolTip="Insert Label" OnClientClick="if (!ConfirmInsert()) { return false;};"
                                                                            OnClick="imgBtnInsert_Click" ValidationGroup="valInsertLabel" />
                                                                    </td>
                                                                    <td align="right" style="float: right" width="33%">
                                                                        <asp:ImageButton ID="imgBtnCancel" runat="server" CommandName="cancelrow" ImageUrl="../Images/cancel_row3.png" OnClientClick="if (!ConfirmInsert()) { return false;};"
                                                                            ToolTip="Cancel" OnClick="imgBtnCancel_Click" />
                                                                    </td>
                                                                    <td align="right" style="float: right" width="36%"></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
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
                BackgroundCssClass="popupBox">
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
                                        <asp:ImageButton ID="btnCloseFuzzySearchPopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" OnClick="btnCloseFuzzySearchPopup_Click" />
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

            <%--Save/Undo changes popup--%>
            <asp:Button ID="dummySaveUndo" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeSaveUndo" runat="server" PopupControlID="pnlSaveUndo" TargetControlID="dummySaveUndo"
                CancelControlID="btnClosePopupSaveUndo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlSaveUndo" runat="server" align="center" Width="25%" BackColor="Window" CssClass="popupPanel" Style="z-index: 1; display: none">
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

            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- Start--%>
            <asp:Button ID="dummyConfirmDelete" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmDelete" runat="server" PopupControlID="pnlConfirmDelete" TargetControlID="dummyConfirmDelete"
                CancelControlID="btnNo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlConfirmDelete" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmation" runat="server" Text="Delete Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblText" runat="server"
                                CssClass="identifierLable" Text="Are you sure you want to delete this row?"></asp:Label>
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
            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- End--%>

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
            <asp:HiddenField ID="hdnIsValidSearch" runat="server" Value="N" />
            <asp:HiddenField ID="hdnChangeNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnInsertDataNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridRowSelectedPrvious" runat="server" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:Button ID="btnHdnLabelSearch" runat="server" Style="display: none;" OnClick="btnHdnLabelSearch_Click" CausesValidation="false" />
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:TextBox>
            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- Start--%>
            <asp:HiddenField ID="hdnLabelCode" runat="server" />
            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- End--%>
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

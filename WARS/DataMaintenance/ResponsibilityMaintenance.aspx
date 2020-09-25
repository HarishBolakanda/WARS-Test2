<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResponsibilityMaintenance.aspx.cs" Inherits="WARS.ResponsibilityMaintenance" MasterPageFile="~/MasterPage.Master"
    Title="WARS - ResponsibilityMaintenance " MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderBody">

    <script type="text/javascript">


        //probress bar and scroll position functionality - starts
        //to remain scroll position of grid panel and window
        var xPos, yPos;
        var scrollTop;
        var gridClientID = "ContentPlaceHolderBody_gvResponsibilityDetails_";
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
            if (postBackElementID.lastIndexOf('gvResponsibilityDetails') > 0 || postBackElementID.lastIndexOf('btnSaveChanges') > 0) {
                xPos = sender._scrollPosition.x;
                yPos = sender._scrollPosition.y;

                //hold scroll position on selecting expand/collapse
                var PnlResponsibilityDetails = document.getElementById("<%=PnlResponsibilityDetails.ClientID %>");
                scrollTop = PnlResponsibilityDetails.scrollTop;

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
            if (postBackElementID.lastIndexOf('gvResponsibilityDetails') > 0 || postBackElementID.lastIndexOf('btnSaveChanges') > 0) {
                window.scrollTo(xPos, yPos);

                //set scroll position on selecting expand/collapse
                var PnlResponsibilityDetails = document.getElementById("<%=PnlResponsibilityDetails.ClientID %>");
                PnlResponsibilityDetails.scrollTop = scrollTop;
            }

        }
        //======================= End


        //Fuzzy search filters
        var txtResponsibilitySearch;
        function ResponsibilitySelected(sender, args) {
            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtResponsibilitySearch.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnIsValidSearch.ClientID %>").value = "Y";
                document.getElementById('<%=btnHdnResponsibilitySearch.ClientID%>').click();
            }
        }

        function ResponsibilityListPopulating() {
            txtResponsibilitySearch = document.getElementById("<%= txtResponsibilitySearch.ClientID %>");
            txtResponsibilitySearch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtResponsibilitySearch.style.backgroundRepeat = 'no-repeat';
            txtResponsibilitySearch.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidSearch.ClientID %>").value = "N";
        }

        function ResponsibilityListPopulated() {
            txtResponsibilitySearch = document.getElementById("<%= txtResponsibilitySearch.ClientID %>");
            txtResponsibilitySearch.style.backgroundImage = 'none';
        }

        //=============== End

        //set flag value when data is changed in grid 
        //debugger;
        function OnDataChange(row) {
            var hdnIsSuperUser = document.getElementById("<%=hdnIsSuperUser.ClientID %>").value;
            var selectedRowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";

            if (document.getElementById(gridClientID + 'lblResponsibilityType' + '_' + selectedRowIndex).innerHTML != "" && hdnIsSuperUser != "Y") {
                DisplayMessagePopup("This can be edited only by super user!");
                document.getElementById(gridClientID + 'txtResponsibilityName' + '_' + selectedRowIndex).value = document.getElementById(gridClientID + 'hdnResponsibilityName' + '_' + selectedRowIndex).value;
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "N";
            }
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
                    document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new responsibility. Save or Undo changes";
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
                document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new responsibility. Save or Undo changes";
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
                document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new responsibility. Save or Undo changes";
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
    var responsibilityCode = document.getElementById(str + 'lblResponsibilityCode' + '_' + rowIndex).innerText;
    if (document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value == "Y") {
        var popup = $find('<%= mpeSaveUndo.ClientID %>');
        if (popup != null) {
            document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new responsibility. Save or Undo changes";
            popup.show();
            $get("<%=btnUndoChanges.ClientID%>").focus();
            return false;
        }
    }
    else {
        if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value == "") {
            document.getElementById("<%=hdnRespCode.ClientID %>").innerText = responsibilityCode;
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
                    document.getElementById("<%=hdnRespCode.ClientID %>").innerText = responsibilityCode;
                    var popup = $find('<%= mpeConfirmDelete.ClientID %>');
                    if (popup != null) {
                        popup.show();
                    }
                    return false; //JIRA-908 Changes
                }
            }
            else if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value == rowIndex) {
                document.getElementById("<%=hdnRespCode.ClientID %>").innerText = responsibilityCode;
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
    document.getElementById("<%=PnlResponsibilityDetails.ClientID %>").style.height = gridPanelHeight + "px";
    document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

}

//======================= End

//Tab key to remain only on screen fields
function OnTabPress() {
    if (event.keyCode == 9) {
        document.getElementById("<%= lblTab.ClientID %>").focus();
    }
}

// Enter Key search functionality for filters        
function SearchByEnterKey() {
    if ((event.keyCode == 13)) {
        document.getElementById("<%= btnSearch.ClientID %>").click();
    }
}

function OntxtRespKeyDown() {
    var txtResponsibilitySearch = document.getElementById("<%= txtResponsibilitySearch.ClientID %>").value;
    if ((event.keyCode == 13)) {
        if (txtResponsibilitySearch == "") {
            document.getElementById('<%=btnHdnResponsibilitySearch.ClientID%>').click();
        }
        else {
            if (document.getElementById("<%= hdnIsValidSearch.ClientID %>").value == "Y") {
                document.getElementById('<%=btnHdnResponsibilitySearch.ClientID%>').click();
            }
            else {
                return false;
            }
        }
    }
}

//=============== End
    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="8">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    RESPONSIBILITY MAINTENANCE
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="8%"></td>
                    <td width="5%" class="identifierLable_large_bold">Responsibility</td>
                    <td width="24%">
                        <asp:TextBox ID="txtResponsibilitySearch" runat="server" Width="99%" CssClass="textboxStyle"
                            TabIndex="100" onfocus="return ConfirmSearch();" onkeydown="OntxtRespKeyDown();"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="aceResponsibilitySearch" runat="server"
                            ServiceMethod="FuzzySearchAllResponsibilityList"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtResponsibilitySearch"
                            FirstRowSelected="true"
                            OnClientItemSelected="ResponsibilitySelected"
                            OnClientPopulating="ResponsibilityListPopulating"
                            OnClientPopulated="ResponsibilityListPopulated"
                            OnClientHidden="ResponsibilityListPopulated"
                            CompletionListElementID="acePnlResponsibility" />
                        <asp:Panel ID="acePnlResponsibility" runat="server" CssClass="identifierLable" />
                    </td>
                    <td width="3%" align="left">
                        <asp:ImageButton ID="fuzzySearchResponsibility" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button"
                            OnClientClick="if (!ConfirmSearch()) { return false;};" OnClick="fuzzySearchResponsibility_Click" ToolTip="Search responsibility code/name" />
                    </td>
                    <td width="10%" class="identifierLable_large_bold" align="center">Manager Responsibility</td>
                    <td width="12%">
                        <asp:DropDownList ID="ddlManagerResponsibility" runat="server" Width="98%" CssClass="ddlStyle" TabIndex="101" onfocus="return ConfirmSearch();" onkeydown="SearchByEnterKey()">
                        </asp:DropDownList>
                    </td>
                    <td width="10%"></td>
                    <td align="right" width="12%">
                        <asp:Button ID="btnSearch" runat="server" CssClass="ButtonStyle" OnClick="btnSearch_Click" TabIndex="102" Text="Search" Width="98%" UseSubmitBehavior="false" OnClientClick="if (!ConfirmSearch()) { return false;};" />
                    </td>
                </tr>
                <tr>
                    <td colspan="7">
                        <br />
                    </td>
                    <td align="right" width="12%">
                        <asp:Button ID="btnReset" runat="server" CssClass="ButtonStyle" OnClick="btnReset_Click" TabIndex="103" Text="Reset" Width="98%" UseSubmitBehavior="false" OnClientClick="if (!ConfirmSearch()) { return false;};" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="5" class="table_with_border">
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
                                                <asp:Panel ID="PnlResponsibilityDetails" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvResponsibilityDetails" runat="server" AutoGenerateColumns="False" Width="97%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." OnRowDataBound="gvResponsibilityDetails_RowDataBound" OnRowCommand="gvResponsibilityDetails_RowCommand" AllowSorting="true" OnSorting="gvResponsibilityDetails_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Responsibility" SortExpression="responsibility_code">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblResponsibilityCode" runat="server" Text='<%# Bind("responsibility_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="13%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Name" SortExpression="responsibility_desc">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnResponsibilityName" runat="server" Value='<%# Eval("responsibility_desc") %>' />
                                                                    <asp:Label ID="lblResponsibilityName" runat="server" Text='<%# Bind("responsibility_desc") %>' CssClass="identifierLable" Visible="false"></asp:Label>
                                                                    <asp:TextBox ID="txtResponsibilityName" runat="server" Text='<%# Eval("responsibility_desc") %>'
                                                                        CssClass="gridTextField" Width="97%" MaxLength="50" onchange="javascript: OnDataChange(this);" onKeyPress="javascript: OnDataChange(this);"
                                                                        onkeydown="javascript: OnkeyDown();" onfocus="OnGridRowSelected(this)"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvOwneName" ControlToValidate="txtResponsibilityName" ValidationGroup="valUpdateResponsibility"
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter label description" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="48%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Type" SortExpression="responsibility_type">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblResponsibilityType" runat="server" Text='<%# Bind("responsibility_type") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="12%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Manager Responsibility" SortExpression="manager_responsibility">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnManagerResp" runat="server" Value='<%# Bind("manager_responsibility") %>' />
                                                                    <asp:DropDownList ID="ddlGridManagerResp" runat="server" Width="90%" CssClass="ddlStyle" onchange="javascript: OnDataChange(this);"
                                                                        onKeyPress="javascript: OnDataChange(this);" onkeydown="javascript: OnkeyDown();" onfocus="OnGridRowSelected(this)">
                                                                    </asp:DropDownList>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvGridManagerResp" ControlToValidate="ddlGridManagerResp" ValidationGroup="valUpdateResponsibility"
                                                                        Text="*" InitialValue="-" CssClass="requiredFieldValidator" ToolTip="Please select manager responsibility" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="15%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="12%" ItemStyle-CssClass="gridItemStyle_Right_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <table width="100%" style="float: right; table-layout: fixed">
                                                                        <tr style="float: right">
                                                                            <td align="right" style="float: right" width="33%">
                                                                                <asp:ImageButton ID="imgBtnSave" runat="server" CommandName="saverow" ImageUrl="../Images/save.png" ToolTip="Save"
                                                                                    ValidationGroup="valUpdateResponsibility" OnClientClick="return ConfirmUpdate(this);" />
                                                                            </td>
                                                                            <td align="right" style="float: right" width="33%">
                                                                                <asp:ImageButton ID="imgBtnDelete" runat="server" CommandName="deleterow" ImageUrl="../Images/Delete.gif"
                                                                                    ToolTip="Delete" OnClientClick="return ConfirmDelete(this);" />
                                                                            </td>
                                                                            <td align="right" style="float: right" width="33%">
                                                                                <asp:ImageButton ID="imgBtnUndo" runat="server" CommandName="cancelrow" ImageUrl="../Images/cancel_row3.png"
                                                                                    OnClientClick="if (!ConfirmUpdate(this)) { return false;};" ToolTip="Cancel" />
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
                                                        <td width="13%" class="gridHeaderStyle_1row">Responsibility</td>
                                                        <td width="48%" class="gridHeaderStyle_1row">Name</td>
                                                        <td width="12%" class="gridHeaderStyle_1row">&nbsp</td>
                                                        <td width="15%" class="gridHeaderStyle_1row">Manager Responsibility</td>
                                                        <td width="12%" class="gridHeaderStyle_1row">&nbsp</td>
                                                    </tr>
                                                    <tr>
                                                        <td width="13%" align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:TextBox ID="txtResponsibilityCode" runat="server" CssClass="textboxStyle" TabIndex="104" Width="50%" MaxLength="2" onfocus="return ConfirmInsert();"
                                                                onchange="javascript: OnDataChangeInsert();" onKeyPress="javascript: OnDataChangeInsert();" onkeydown="javascript: OnkeyDownInsert();"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvLabeCode" ControlToValidate="txtResponsibilityCode" ValidationGroup="valInsertResponsibility"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter responsibility code" Display="Dynamic"></asp:RequiredFieldValidator>
                                                            <asp:RegularExpressionValidator ID="revRespCode" runat="server" Text="*" ControlToValidate="txtResponsibilityCode"
                                                                ValidationExpression="^[1-9]\d*$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valInsertResponsibility"
                                                                ToolTip="Please enter only numbers" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                        </td>
                                                        <td width="48%" class="insertBoxStyle">
                                                            <asp:TextBox ID="txtResponsibilityDesc" runat="server" CssClass="textboxStyle" TabIndex="105" Width="96%" MaxLength="50" onfocus="return ConfirmInsert();"
                                                                onchange="javascript: OnDataChangeInsert();" onKeyPress="javascript: OnDataChangeInsert();" onkeydown="javascript: OnkeyDownInsert();"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvOwneDesc" ControlToValidate="txtResponsibilityDesc" ValidationGroup="valInsertResponsibility"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter responsibility description" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td width="12%" class="insertBoxStyle">
                                                            <label id="lblTypeAddRow" runat="server">&nbsp</label>
                                                        </td>
                                                        <td width="15%" class="insertBoxStyle">
                                                            <asp:DropDownList ID="ddlNewManagerResp" runat="server" Width="90%" CssClass="ddlStyle" TabIndex="106" onfocus="return ConfirmInsert();"
                                                                onchange="javascript: OnDataChangeInsert();" onKeyPress="javascript: OnDataChangeInsert();" onkeydown="javascript: OnkeyDownInsert();">
                                                            </asp:DropDownList>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvNewManagerResp" ControlToValidate="ddlNewManagerResp" ValidationGroup="valInsertResponsibility"
                                                                Text="*" InitialValue="-" CssClass="requiredFieldValidator" ToolTip="Please select manager responsibility" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td width="12%" class="insertBoxStyle_No_Padding">
                                                            <table width="99%" style="float: right; table-layout: fixed">
                                                                <tr style="float: right">
                                                                    <td align="right" style="float: right" width="31%">
                                                                        <asp:ImageButton ID="imgBtnInsert" runat="server" TabIndex="107" CommandName="saverow" ImageUrl="../Images/save.png" ToolTip="Insert Responsibility"
                                                                            OnClientClick="if (!ConfirmInsert()) { return false;};" OnClick="imgBtnInsert_Click" ValidationGroup="valInsertResponsibility" />
                                                                    </td>
                                                                    <td align="right" style="float: right" width="33%">
                                                                        <asp:ImageButton ID="imgBtnCancel" runat="server" TabIndex="108" CommandName="cancelrow" ImageUrl="../Images/cancel_row3.png"
                                                                            OnClientClick="if (!ConfirmInsert()) { return false;};" ToolTip="Cancel" OnClick="imgBtnCancel_Click" />
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
                    <td colspan="3"></td>
                </tr>
            </table>

            <asp:Button ID="dummyFuzzySearch" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeFuzzySearch" runat="server" PopupControlID="pnlFuzzySearch" TargetControlID="dummyFuzzySearch"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlFuzzySearch" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
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
            <asp:Panel ID="pnlSaveUndo" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
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
            <asp:HiddenField ID="hdnIsSuperUser" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsValidSearch" runat="server" Value="N" />
            <asp:HiddenField ID="hdnChangeNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnInsertDataNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridRowSelectedPrvious" runat="server" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:Button ID="btnHdnResponsibilitySearch" runat="server" Style="display: none;" OnClick="btnHdnResponsibilitySearch_Click" CausesValidation="false" />
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:TextBox>
            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- Start--%>
            <asp:HiddenField ID="hdnRespCode" runat="server" />
            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- End--%>
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

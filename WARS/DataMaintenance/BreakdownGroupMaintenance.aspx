<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BreakdownGroupMaintenance.aspx.cs" Inherits="WARS.BreakdownGroupMaintenance" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Breakdown Group Maintenance " MaintainScrollPositionOnPostback="true" %>


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
                var PnlBreakdownGroupDetails = document.getElementById("<%=PnlBreakdownGroupDetails.ClientID %>");
                scrollTop = PnlBreakdownGroupDetails.scrollTop;

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
                var PnlBreakdownGroupDetails = document.getElementById("<%=PnlBreakdownGroupDetails.ClientID %>");
                PnlBreakdownGroupDetails.scrollTop = scrollTop;
            }


        }
        //======================= End


        //set flag value when data is changed in grid 
        function OnDataChange(row) {
            CompareRow(row);
        }

        function CompareRow(row) {
            //debugger;
            var rowIndex = row.id.substring(row.id.lastIndexOf('_') + 1);
            var tempstr = row.id.substring(0, row.id.lastIndexOf('_'));
            var str = tempstr.substring(0, tempstr.lastIndexOf('_') + 1);

            var hdnBreakdownGroupDesc = document.getElementById(str + 'hdnBreakdownGroupDesc_' + rowIndex).value;
            var txtBreakdownGroupDesc = document.getElementById(str + 'txtBreakdownGroupDesc_' + rowIndex).value;
            var hdnTerritory = document.getElementById(str + 'hdnTerritory_' + rowIndex).value;
            var ddlTerritory = document.getElementById(str + 'ddlTerritory_' + rowIndex).value;
            var hdnConfiguration = document.getElementById(str + 'hdnConfiguration_' + rowIndex).value;
            var ddlConfiguration = document.getElementById(str + 'ddlConfiguration_' + rowIndex).value;
            var hdnSalesType = document.getElementById(str + 'hdnSalesType_' + rowIndex).value;
            var ddlSalesType = document.getElementById(str + 'ddlSalesType_' + rowIndex).value;
            var hdnGFSPLAccount = document.getElementById(str + 'hdnGFSPLAccount_' + rowIndex).value;
            var txtGFSPLAccount = document.getElementById(str + 'txtGFSPLAccount_' + rowIndex).value;
            var hdnGFSBLAccount = document.getElementById(str + 'hdnGFSBLAccount_' + rowIndex).value;
            var txtGFSBLAccount = document.getElementById(str + 'txtGFSBLAccount_' + rowIndex).value;

            if (hdnBreakdownGroupDesc != txtBreakdownGroupDesc || hdnTerritory != ddlTerritory || hdnConfiguration != ddlConfiguration
                || hdnSalesType != ddlSalesType || hdnGFSPLAccount != txtGFSPLAccount || hdnGFSBLAccount != txtGFSBLAccount) {
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
            var txtBreakdownGroupCodeInsert = document.getElementById("<%=txtBreakdownGroupCodeInsert.ClientID %>").value;
            var txtBreakdownGroupDescInsert = document.getElementById("<%=txtBreakdownGroupDescInsert.ClientID %>").value;
            var ddlTerritoryInsert = document.getElementById("<%=ddlTerritoryInsert.ClientID %>").value;
            var ddlConfigurationInsert = document.getElementById("<%=ddlConfigurationInsert.ClientID %>").value;
            var ddlSalesTypeInsert = document.getElementById("<%=ddlSalesTypeInsert.ClientID %>").value;
            var txtGFSPLAccountInsert = document.getElementById("<%=txtGFSPLAccountInsert.ClientID %>").value;
            var txtGFSBLAccountInsert = document.getElementById("<%=txtGFSBLAccountInsert.ClientID %>").value;

            if (txtBreakdownGroupCodeInsert != "" || txtBreakdownGroupDescInsert != "" || ddlTerritoryInsert != "-" || ddlConfigurationInsert != "-" || ddlSalesTypeInsert != "-" || txtGFSPLAccountInsert != "" || txtGFSBLAccountInsert != "") {
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
                    document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new breakdown group. Save or Undo changes";
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

        function ValidateUnsavedData() {
        if (document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value == "Y") {
                var popup = $find('<%= mpeSaveUndo.ClientID %>');
            if (popup != null) {
                document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new breakdown group. Save or Undo changes";
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
                document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new breakdown group. Save or Undo changes";
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
    var breakDownGrpCode = document.getElementById(str + 'lblBreakdownGroupCode' + '_' + rowIndex).innerText;
    if (document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value == "Y") {
        var popup = $find('<%= mpeSaveUndo.ClientID %>');
        if (popup != null) {
            document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new breakdown group. Save or Undo changes";
            popup.show();
            $get("<%=btnUndoChanges.ClientID%>").focus();
            return false;
        } hdnBreakDownGrpCode
    }
    else {
        if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value == "") {
            document.getElementById("<%=hdnDeleteBreakDownGrpCode.ClientID %>").innerText = breakDownGrpCode;
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
                document.getElementById("<%=hdnDeleteBreakDownGrpCode.ClientID %>").innerText = breakDownGrpCode;
                var popup = $find('<%= mpeConfirmDelete.ClientID %>');
                if (popup != null) {
                    popup.show();
                }
                return false; //JIRA-908 Changes
            }
        }
        else if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value == rowIndex) {
            document.getElementById("<%=hdnDeleteBreakDownGrpCode.ClientID %>").innerText = breakDownGrpCode;
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
    document.getElementById("<%=PnlBreakdownGroupDetails.ClientID %>").style.height = gridPanelHeight + "px";
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

//On press of Enter key in search textbox
function OnBreakdownGroupKeyDown() {
    if ((event.keyCode == 13)) {
        document.getElementById('<%=btnHdnBreakdownGroupSearch.ClientID%>').click();
    }
}

//==============End

//clear add row data
function ClearAddRow() {
    if (!ConfirmInsert()) {
        return false;
    }
    document.getElementById('<%=txtBreakdownGroupCodeInsert.ClientID%>').value = "";
    document.getElementById('<%=txtBreakdownGroupDescInsert.ClientID%>').value = "";
            document.getElementById('<%=ddlTerritoryInsert.ClientID%>').selectedIndex = 0;
            document.getElementById('<%=ddlConfigurationInsert.ClientID%>').selectedIndex = 0;
            document.getElementById('<%=ddlSalesTypeInsert.ClientID%>').selectedIndex = 0;
            document.getElementById('<%=txtGFSPLAccountInsert.ClientID%>').value = "";
            document.getElementById('<%=txtGFSBLAccountInsert.ClientID%>').value = "";
            document.getElementById('<%=hdnInsertDataNotSaved.ClientID%>').value = 'N';
            Page_ClientValidate('');//clear all validators of the page
            document.getElementById("<%= txtBreakdownGroupCodeInsert.ClientID %>").focus();
    return false;

}
//============== End
    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="8">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    BREAKDOWN GROUP MAINTENANCE
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="10%" class="identifierLable_large_bold">Breakdown Group</td>
                    <td width="20%">
                        <asp:TextBox ID="txtBreakdownGroupSearch" runat="server" Width="99%" CssClass="textboxStyle"
                            TabIndex="100" onfocus="return ValidateUnsavedData();" onkeydown="OnBreakdownGroupKeyDown();"></asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="tbweBreakdownGroupSearch" runat="server"
                            TargetControlID="txtBreakdownGroupSearch"
                            WatermarkText="Enter Search Text"
                            WatermarkCssClass="watermarked" />
                    </td>
                    <td width="3%" align="left"></td>
                    <td></td>
                    <td width="3%"></td>
                    <td align="right" colspan="2">
                        <asp:Button ID="btnReset" runat="server" CssClass="ButtonStyle" OnClick="btnReset_Click" TabIndex="109" Text="Reset" UseSubmitBehavior="false" Width="98%" OnKeyDown="OnTabPress()"
                            OnClientClick="if (!ValidateUnsavedData()) { return false;};" />
                    </td>
                </tr>
                <tr>
                    <td colspan="6">
                        <br />
                    </td>
                    <td width="10%"></td>
                    <td width="2%"></td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="6" class="table_with_border">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="2%">
                                    <br />
                                </td>
                                <td width="96%"></td>
                                <td width="2%"></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlBreakdownGroupDetails" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvBreakdownGroupDetails" runat="server" AutoGenerateColumns="False" Width="98%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." OnRowDataBound="gvBreakdownGroupDetails_RowDataBound" OnRowCommand="gvBreakdownGroupDetails_RowCommand"
                                                        AllowSorting="true" OnSorting="gvBreakdownGroupDetails_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Group" SortExpression="breakdown_group">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblBreakdownGroupCode" runat="server" Text='<%# Bind("breakdown_group") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Description" SortExpression="breakdown_desc">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnBreakdownGroupDesc" runat="server" Value='<%# Bind("breakdown_desc") %>' />
                                                                    <asp:TextBox ID="txtBreakdownGroupDesc" runat="server" Text='<%# Eval("breakdown_desc") %>'
                                                                        CssClass="gridTextField" Width="92%" MaxLength="50" onchange="javascript: OnDataChange(this);" onfocus="OnGridRowSelected(this)"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvBreakdownGroupDesc" ControlToValidate="txtBreakdownGroupDesc" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>'
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter breakdown group desc" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Territory" SortExpression="seller_group_code">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnTerritory" runat="server" Value='<%# Bind("seller_group_code") %>' />
                                                                    <asp:DropDownList ID="ddlTerritory" runat="server" Width="94%" CssClass="ddlStyle" onchange="javascript: OnDataChange(this);" onfocus="OnGridRowSelected(this)"></asp:DropDownList>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvTerritory" ControlToValidate="ddlTerritory" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>'
                                                                        Text="*" InitialValue="-" CssClass="requiredFieldValidator" ToolTip="Please select territory" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="19%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Configuration" SortExpression="config_group_code">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnConfiguration" runat="server" Value='<%# Bind("config_group_code") %>' />
                                                                    <asp:DropDownList ID="ddlConfiguration" runat="server" Width="92%" CssClass="ddlStyle" onchange="javascript: OnDataChange(this);" onfocus="OnGridRowSelected(this)"></asp:DropDownList>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvConfiguration" ControlToValidate="ddlConfiguration" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>'
                                                                        Text="*" InitialValue="-" CssClass="requiredFieldValidator" ToolTip="Please select configuration" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="15%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Sales Type" SortExpression="price_group_code">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnSalesType" runat="server" Value='<%# Bind("price_group_code") %>' />
                                                                    <asp:DropDownList ID="ddlSalesType" runat="server" Width="92%" CssClass="ddlStyle" onchange="javascript: OnDataChange(this);" onfocus="OnGridRowSelected(this)"></asp:DropDownList>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvSalesType" ControlToValidate="ddlSalesType" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>'
                                                                        Text="*" InitialValue="-" CssClass="requiredFieldValidator" ToolTip="Please select sales type" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="15%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="GFS PL Account" SortExpression="gfs_pl_account">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnGFSPLAccount" runat="server" Value='<%# Bind("gfs_pl_account") %>' />
                                                                    <asp:TextBox ID="txtGFSPLAccount" runat="server" Text='<%# Eval("gfs_pl_account") %>'
                                                                        CssClass="gridTextField" Width="40%" MaxLength="6" onchange="javascript: OnDataChange(this);" onfocus="OnGridRowSelected(this)"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvGFSPLAccount" ControlToValidate="txtGFSPLAccount" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>'
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter GFS PL account" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="GFS BL Account" SortExpression="gfs_bl_account">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnGFSBLAccount" runat="server" Value='<%# Bind("gfs_bl_account") %>' />
                                                                    <asp:TextBox ID="txtGFSBLAccount" runat="server" Text='<%# Eval("gfs_bl_account") %>'
                                                                        CssClass="gridTextField" Width="40%" MaxLength="6" onchange="javascript: OnDataChange(this);" onfocus="OnGridRowSelected(this)"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvGFSBLAccount" ControlToValidate="txtGFSBLAccount" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>'
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter GFS BL account" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Right_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <table width="100%" style="float: right; table-layout: fixed">
                                                                        <tr style="float: right">
                                                                            <td align="right" style="float: right" width="33%">
                                                                                <asp:ImageButton ID="imgBtnSave" runat="server" CommandName="saverow" ImageUrl="../Images/save.png" ToolTip="Save"
                                                                                    onfocus="return ConfirmUpdate(this);" />
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
                                                <table width="98%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="10%" class="gridHeaderStyle_1row">Group</td>
                                                        <td width="20%" class="gridHeaderStyle_1row">Description</td>
                                                        <td width="19%" class="gridHeaderStyle_1row">Territory</td>
                                                        <td width="15%" class="gridHeaderStyle_1row">Configuration</td>
                                                        <td width="15%" class="gridHeaderStyle_1row">Sales Type</td>
                                                        <td width="8%" class="gridHeaderStyle_1row">GFS PL Account</td>
                                                        <td width="8%" class="gridHeaderStyle_1row">GFS BL Account</td>
                                                        <td width="5%" class="gridHeaderStyle_1row">&nbsp</td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:TextBox ID="txtBreakdownGroupCodeInsert" runat="server" CssClass="textboxStyle" TabIndex="101" Width="60%" MaxLength="5" onchange="javascript: OnDataChangeInsert();"
                                                                onfocus="return ConfirmInsert();"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvBreakdownGroupCodeInsert" ControlToValidate="txtBreakdownGroupCodeInsert" ValidationGroup="valInsertBreakdownGroup"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter breakdown group code" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:TextBox ID="txtBreakdownGroupDescInsert" runat="server" CssClass="textboxStyle" TabIndex="102" Width="90%" MaxLength="50" onchange="javascript: OnDataChangeInsert();"
                                                                onfocus="return ConfirmInsert();"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvBreakdownGroupDescInsert" ControlToValidate="txtBreakdownGroupDescInsert" ValidationGroup="valInsertBreakdownGroup"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter breakdown group desc" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td class="insertBoxStyle" align="left">
                                                            <asp:DropDownList ID="ddlTerritoryInsert" runat="server" Width="94%" CssClass="ddlStyle" TabIndex="102" onchange="javascript: OnDataChangeInsert();" onfocus="ConfirmInsert()"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvTerritoryInsert" ControlToValidate="ddlTerritoryInsert" ValidationGroup="valInsertBreakdownGroup"
                                                                Text="*" InitialValue="-" CssClass="requiredFieldValidator" ToolTip="Please select territory" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td class="insertBoxStyle" align="left">
                                                            <asp:DropDownList ID="ddlConfigurationInsert" runat="server" Width="92%" CssClass="ddlStyle" TabIndex="103" onchange="javascript: OnDataChangeInsert();" onfocus="ConfirmInsert()"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvConfigurationInsert" ControlToValidate="ddlConfigurationInsert" ValidationGroup="valInsertBreakdownGroup"
                                                                Text="*" InitialValue="-" CssClass="requiredFieldValidator" ToolTip="Please select configuration" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td class="insertBoxStyle" align="left">
                                                            <asp:DropDownList ID="ddlSalesTypeInsert" runat="server" Width="92%" CssClass="ddlStyle" TabIndex="104" onchange="javascript: OnDataChangeInsert();" onfocus="ConfirmInsert()"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvSalesTypeInsert" ControlToValidate="ddlSalesTypeInsert" ValidationGroup="valInsertBreakdownGroup"
                                                                Text="*" InitialValue="-" CssClass="requiredFieldValidator" ToolTip="Please select sales type" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:TextBox ID="txtGFSPLAccountInsert" runat="server" CssClass="textboxStyle" TabIndex="105" Width="60%" MaxLength="6" onchange="javascript: OnDataChangeInsert();"
                                                                onfocus="return ConfirmInsert();"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvGFSPLAccountInsert" ControlToValidate="txtGFSPLAccountInsert" ValidationGroup="valInsertBreakdownGroup"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter GFS PL account" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:TextBox ID="txtGFSBLAccountInsert" runat="server" CssClass="textboxStyle" TabIndex="106" Width="60%" MaxLength="6" onchange="javascript: OnDataChangeInsert();"
                                                                onfocus="return ConfirmInsert();"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvGFSBLAccountInsert" ControlToValidate="txtGFSBLAccountInsert" ValidationGroup="valInsertBreakdownGroup"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter GFS BL account" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td class="insertBoxStyle_No_Padding">
                                                            <table width="100%" style="float: right; table-layout: fixed">
                                                                <tr style="float: right">
                                                                    <td align="right" style="float: right" width="33%">
                                                                        <asp:ImageButton ID="imgBtnInsert" runat="server" CommandName="saverow" TabIndex="107" ImageUrl="../Images/save.png" ToolTip="Insert breakdown group" onfocus="return ConfirmInsert();" OnClick="imgBtnInsert_Click" />
                                                                    </td>
                                                                    <td align="right" style="float: right" width="33%">
                                                                        <asp:ImageButton ID="imgBtnCancel" runat="server" CommandName="cancelrow" TabIndex="108" ImageUrl="../Images/cancel_row3.png"
                                                                            ToolTip="Cancel" OnClientClick="if (!ClearAddRow()) { return false;};" />
                                                                    </td>
                                                                    <td align="right" style="float: right" width="34%"></td>
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
                                <td colspan="2"></td>
                            </tr>
                        </table>
                    </td>
                    <td valign="top" align="right"></td>
                </tr>
            </table>

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
            <asp:HiddenField ID="hdnSearchText" runat="server" Value="" />
            <asp:HiddenField ID="hdnChangeNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnInsertDataNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridRowSelectedPrvious" runat="server" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:Button ID="btnHdnBreakdownGroupSearch" runat="server" Style="display: none;" OnClick="btnHdnBreakdownGroupSearch_Click" CausesValidation="false" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99"></asp:Label>
            <%--JIRA-908 Changes -- Start--%>
            <asp:HiddenField ID="hdnDeleteBreakDownGrpCode" runat="server" Value="N" />
            <%--JIRA-908 Changes -- Start--%>
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserAccountMaint.aspx.cs" Inherits="WARS.UserAccountMaint" MasterPageFile="~/MasterPage.Master"
    Title="WARS - User Account Maintenance" MaintainScrollPositionOnPostback="true" ClientIDMode="AutoID" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderBody">
    <script type="text/javascript">
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

            //to maintain scroll position
            postBackElementID = args.get_postBackElement().id.substring(args.get_postBackElement().id.lastIndexOf("_") + 1);
            if (postBackElementID == 'btnUpdate' || postBackElementID == 'btnAdd' || postBackElementID == 'btnYesConfirm' || postBackElementID == 'btnNoConfirm') {
                xPos = sender._scrollPosition.x;
                yPos = sender._scrollPosition.y;
                //hold scroll position on selecting expand/collapse
                var PnlReference = document.getElementById("<%=PnlGrid.ClientID %>");
                scrollTop = PnlReference.scrollTop;
            }
        }

        function EndRequestHandler(sender, args) {
            //Hide the modal popup - the update progress
            var popup = $find('<%= mPopupPageLevel.ClientID %>');
            if (popup != null) {
                popup.hide();
            }

            //to maintain scroll position
            postBackElementID = sender._postBackSettings.sourceElement.id.substring(sender._postBackSettings.sourceElement.id.lastIndexOf("_") + 1);
            if (postBackElementID == 'btnUpdate' || postBackElementID == 'btnAdd' || postBackElementID == 'btnYesConfirm' || postBackElementID == 'btnNoConfirm') {
                window.scrollTo(xPos, yPos);
                //set scroll position on selecting expand/collapse
                var PnlReference = document.getElementById("<%=PnlGrid.ClientID %>");
                PnlReference.scrollTop = scrollTop;
            }


        }

        //probress bar functionality - ends

        //grid panel height adjustment functioanlity - starts
        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.53;
            document.getElementById("<%=PnlGrid.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        //grid panel height adjustment functioanlity - ends    

        //User filter auto populate search functionalities        
        var txtUserSrch;

        function userSelected(sender, args) {
            document.getElementById("<%=hdnUserSearchSelected.ClientID %>").innerText = "Y";

            var userSrchVal = args.get_value();
            if (userSrchVal == 'No results found') {
                document.getElementById("<%= txtUserSearch.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "Y";
            }

        }

        function userListPopulating() {
            document.getElementById("<%=hdnUserSearchSelected.ClientID %>").innerText = "N";
            txtUserSrch = document.getElementById("<%= txtUserSearch.ClientID %>");
            txtUserSrch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtUserSrch.style.backgroundRepeat = 'no-repeat';
            txtUserSrch.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "N";
        }

        function userListPopulated() {
            txtUserSrch = document.getElementById("<%= txtUserSearch.ClientID %>");
            txtUserSrch.style.backgroundImage = 'none';
        }


        function resetScrollPosition(sender, args) {
            var autoCompPnl = document.getElementById("<%= userSearchPnl.ClientID %>");
            autoCompPnl.scrollTop = 1;

        }
        //================================End

        //Validation: warning message if changes made and not saved or on page change                                
        //set flag value when grid data is changed
        function OnGridDataChange(row, name) {
            CompareRow(row);
            if (name == "Chkbox") {
                OnGridRowSelected(row);
            }
        }

        //set flag value when grid data is changed in Insert panel
        function OnGridDataChangeInsert(row, name) {
            if (name == "Chkbox") {
                OnGridRowSelectedAdd(row);
            }

            var txtUserNameAdd = document.getElementById("<%=txtUserNameAdd.ClientID %>").value;
            var txtUserCodeAdd = document.getElementById("<%=txtUserCodeAdd.ClientID %>").value;
            var txtUserAccIdAdd = document.getElementById("<%=txtUserAccIdAdd.ClientID %>").value;
            var ddlResponsibilityAdd = document.getElementById("<%=ddlResponsibilityAdd.ClientID %>").value;
            var ddlroleAdd = document.getElementById("<%=ddlroleAdd.ClientID %>").value;
            var ddlPaymentRoleAdd = document.getElementById("<%=ddlPaymentRoleAdd.ClientID %>").value;
            var cbActiveAdd = document.getElementById("<%=cbActiveAdd.ClientID %>");

            if (txtUserNameAdd != '' || txtUserCodeAdd != '' || txtUserAccIdAdd != '' || ddlResponsibilityAdd != '-' || ddlroleAdd != '-' ||
                ddlPaymentRoleAdd != '-' || cbActiveAdd.checked == true) {
                document.getElementById("<%=hdnChangeNotSavedAdd.ClientID %>").innerText = "Y";
            }
            else {
                document.getElementById("<%=hdnChangeNotSavedAdd.ClientID %>").innerText = "N";
            }


        }

        function OnkeyDown(row) {
            if (event.keyCode == 8 || event.keyCode == 46) {
                //document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                CompareRow(row);
            }
        }

        function CompareRow(row) {
            var str = row.id.substring(0, row.id.lastIndexOf('_') + 1);
            var hdnUserName = document.getElementById(str + 'hdnUserName').value;
            var txtUserName = document.getElementById(str + 'txtUserName').value;
            var hdnUserCode = document.getElementById(str + 'hdnUserCode').value;
            var txtUserCode = document.getElementById(str + 'txtUserCode').value;
            var hdnUserAccId = document.getElementById(str + 'hdnUserAccId').value;
            var txtUserAccId = document.getElementById(str + 'txtUserAccId').value;
            var hdnRespCode = document.getElementById(str + 'hdnRespCode').value;
            var ddlResponsibility = document.getElementById(str + 'ddlResponsibility').value;
            var hdnRoleId = document.getElementById(str + 'hdnRoleId').value;
            var ddlrole = document.getElementById(str + 'ddlrole').value;
            var hdnPaymentRoleId = document.getElementById(str + 'hdnPaymentRoleId').value;
            var ddlPaymentRole = document.getElementById(str + 'ddlPaymentRole').value;
            var hdnIsEnabled = document.getElementById(str + 'hdnIsEnabled').value;
            var isEnabled;

            var cbActive = document.getElementById(str + 'cbActive');
            if (cbActive.checked == true) {
                isEnabled = 'Y';
            }
            else {
                isEnabled = 'N';
            }

            if (ddlPaymentRole == "-") {
                ddlPaymentRole = "";
            }

            if (hdnUserName != txtUserName || hdnUserCode != txtUserCode || hdnUserAccId != txtUserAccId || hdnRespCode != ddlResponsibility || hdnRoleId != ddlrole ||
               hdnPaymentRoleId != ddlPaymentRole || hdnIsEnabled != isEnabled) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
            }
            else {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "N";
            }

        }

        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        function WarnOnUnSavedData() {
            if (DataChanged()) {
                return warningMsgOnUnSavedData;
            }

        }
        window.onbeforeunload = WarnOnUnSavedData;

        function DataChanged() {
            var tdGrid = document.getElementById("<%=tdGrid.ClientID %>");
            if (tdGrid != null) {
                var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
                var isGridDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
                var isGridDataChangedAdd = document.getElementById("<%=hdnChangeNotSavedAdd.ClientID %>").value;
                if (isExceptionRaised != "Y") {
                    if (isGridDataChanged == "Y" || isGridDataChangedAdd == "Y") {
                        return true;
                    }
                }
            }

            return false;
        }

        //used to check if any changes to allow navigation to other screen 
        function IsDataChanged() {
            return DataChanged();
        }

        function ConfirmSearch() {
            var isGridDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
            var isGridDataChangedAdd = document.getElementById("<%=hdnChangeNotSavedAdd.ClientID %>").value;
            if (isGridDataChanged == "Y" || isGridDataChangedAdd == "Y") {
                var popup = $find('<%= mpeConfirmation.ClientID %>');
                if (popup != null) {
                    popup.show();
                    document.getElementById("<%=lblConfirmMsg.ClientID %>").innerText = warningMsgOnUnSavedData;
                }
                return false;
            }
            else {
                return true;
            }
        }

        function OnGridRowSelected(row) {
            var str = row.id.substring(0, row.id.lastIndexOf('_') + 1);
            var hdnIsEnabled = document.getElementById(str + 'hdnIsEnabled').value;
            var cbActive = document.getElementById(str + 'cbActive');
            var rowData = row.parentNode.parentNode;
            var rowIndex = rowData.rowIndex - 1;
            if (document.getElementById("<%=hdnChangeNotSavedAdd.ClientID %>").value == "Y") {
                var popup = $find('<%= mpeConfirmation.ClientID %>');
                if (popup != null) {
                    popup.show();
                    document.getElementById("<%=btnNoConfirm.ClientID %>").focus();
                    document.getElementById("<%=lblConfirmMsg.ClientID %>").innerText = warningMsgOnUnSavedData;
                    //reset checkbox value
                    if (hdnIsEnabled == "Y") {
                        cbActive.checked = true;
                    }
                    else {
                        cbActive.checked = false;
                    }
                    return false;
                }
            }
            else {
                if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value == "") {
                    document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").innerText = rowIndex;
                }
                else if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value != rowIndex) {
                    if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
                        var popup = $find('<%= mpeConfirmation.ClientID %>');
                        if (popup != null) {
                            popup.show();
                            document.getElementById("<%=btnNoConfirm.ClientID %>").focus();
                            document.getElementById("<%=lblConfirmMsg.ClientID %>").innerText = warningMsgOnUnSavedData;
                            //reset checkbox value
                            if (hdnIsEnabled == "Y") {
                                cbActive.checked = true;
                            }
                            else {
                                cbActive.checked = false;
                            }
                            return false;
                        }
                    }
                    else {
                        document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").innerText = rowIndex;
                    }
                }
        }

        return true;
    }

    function OnGridRowSelectedAdd(row) {
        if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
            var popup = $find('<%= mpeConfirmation.ClientID %>');
            if (popup != null) {
                popup.show();
                document.getElementById("<%=btnNoConfirm.ClientID %>").focus();
                document.getElementById("<%=lblConfirmMsg.ClientID %>").innerText = warningMsgOnUnSavedData;
                document.getElementById("<%=cbActiveAdd.ClientID %>").checked = false;
                return false;
            }
        }

        return true;
    }

    function ConfirmUpdate(row) {
        var rowData = row.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
        var rowIndex = rowData.rowIndex - 1;

        if (document.getElementById("<%=hdnChangeNotSavedAdd.ClientID %>").value == "Y") {
            var popup = $find('<%= mpeConfirmation.ClientID %>');
            if (popup != null) {
                popup.show();
                document.getElementById("<%=lblConfirmMsg.ClientID %>").innerText = warningMsgOnUnSavedData;
            }
            return false;
        }
        else {
            if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value == "") {
                return true;
            }
            else if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value != rowIndex) {
                if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
                    var popup = $find('<%= mpeConfirmation.ClientID %>');
                    if (popup != null) {
                        popup.show();
                        document.getElementById("<%=lblConfirmMsg.ClientID %>").innerText = warningMsgOnUnSavedData;
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

function ConfirmInsert() {
    if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
        var popup = $find('<%= mpeConfirmation.ClientID %>');
        if (popup != null) {
            popup.show();
            document.getElementById("<%=lblConfirmMsg.ClientID %>").innerText = warningMsgOnUnSavedData;
        }
        return false;
    }
    else {
        return true;
    }
}
//================================End

//allow search text field enter key postback

function OnUserSearchEnter() {
    if ((event.keyCode == 13)) {
        var test = 1;
        if (document.getElementById("<%=txtUserSearch.ClientID %>").value == "") {
            __doPostBack("txtUserSearch", "txtUserSearch_TextChanged");
        }
    }
    else if (event.keyCode == 9) {
        document.getElementById("<%= lblTab.ClientID %>").focus();
    }
    else {
        return false;
    }

}

//================================= End

    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="6">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    USER ACCOUNT MAINTENANCE
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="9%" class="identifierLable_large_bold">User Search
                    </td>
                    <td width="30%">
                        <table width="100%">
                            <tr>
                                <td width="70%">
                                    <asp:TextBox ID="txtUserSearch" runat="server" Width="98%" CssClass="identifierLable"
                                        onfocus="if (!ConfirmSearch()) { return false;};" onkeydown="javascript: OnUserSearchEnter();"
                                        OnTextChanged="txtUserSearch_TextChanged" AutoPostBack="true" TabIndex="100"></asp:TextBox>
                                    <ajaxToolkit:AutoCompleteExtender ID="royaltorFilterExtender" runat="server"
                                        ServiceMethod="FuzzyUserMaintUserList"
                                        ServicePath="~/Services/FuzzySearch.asmx"
                                        MinimumPrefixLength="1"
                                        CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                        FirstRowSelected="true"
                                        TargetControlID="txtUserSearch"
                                        OnClientItemSelected="userSelected"
                                        OnClientPopulating="userListPopulating"
                                        OnClientPopulated="userListPopulated"
                                        OnClientHidden="userListPopulated"
                                        OnClientShown="resetScrollPosition"
                                        CompletionListElementID="userSearchPnl" />
                                    <asp:Panel ID="userSearchPnl" runat="server" CssClass="identifierLable" />
                                </td>
                                <td align="left">
                                    <asp:ImageButton ID="fuzzySearchUser" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                                        ToolTip="Search User" OnClientClick="if (!ConfirmSearch()) { return false;};" OnClick="fuzzySearchUser_Click" CssClass="FuzzySearch_Button" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td></td>
                    <td align="right" width="12%">
                        <asp:Button ID="btnReset" runat="server" CssClass="ButtonStyle" OnClick="btnReset_Click" TabIndex="101" Text="Reset" UseSubmitBehavior="false" Width="98%"
                            OnClientClick="if (!ConfirmSearch()) { return false;};" />
                    </td>
                </tr>
                <tr>
                    <td colspan="6">
                        <br />
                    </td>
                </tr>

                <tr>
                    <td></td>
                    <td colspan="5" align="left" runat="server" id="tdGrid">
                        <table width="80%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td colspan="5">
                                    <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="100%">
                                        <table width="98.3%" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td colspan="7">
                                                    <asp:GridView ID="gvUserAccount" runat="server" AutoGenerateColumns="False" Width="100%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" ShowHeaderWhenEmpty="true" EmptyDataText="No Data Found"
                                                        OnRowDataBound="gvUserAccount_RowDataBound" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" AllowSorting="true" OnSorting="gvUserAccount_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="User Name" SortExpression="user_name">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnUserName" runat="server" Value='<%#Bind("user_name")%>' />
                                                                    <asp:TextBox ID="txtUserName" runat="server" Text='<%#Bind("user_name")%>' CssClass="gridTextField"
                                                                        MaxLength="200" ToolTip="upto 200 chars" Width="90%" onchange="javascript: OnGridDataChange(this,'');" onclick="OnGridRowSelected(this)"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvUserNameEdit" ControlToValidate="txtUserName" ValidationGroup="valUpdate"
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter user name" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="User Code" SortExpression="user_code">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtUserCode" runat="server" Text='<%#Bind("user_code")%>' CssClass="gridTextField"
                                                                        MaxLength="10" ToolTip="upto 10 chars" Width="50%" onchange="javascript: OnGridDataChange(this,'');" onclick="OnGridRowSelected(this)"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvUsercode" ControlToValidate="txtUserCode" ValidationGroup="valUpdate"
                                                                        Text="*" CssClass="requiredFieldValidator" InitialValue="" ToolTip="Please enter user code" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="15%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="User Account Id" SortExpression="user_account_id">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtUserAccId" runat="server" Text='<%#Bind("user_account_id")%>'
                                                                        MaxLength="100" ToolTip="Domain\account id upto 100 chars" CssClass="gridTextField"
                                                                        Width="90%" onchange="javascript: OnGridDataChange(this,'');" onclick="OnGridRowSelected(this)"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvUserAccId" ControlToValidate="txtUserAccId" ValidationGroup="valUpdate"
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter user account id" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Responsibility" SortExpression="responsibility_code">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlResponsibility" runat="server" CssClass="ddlStyle" Width="90%"
                                                                        onchange="javascript: OnGridDataChange(this,'');" onclick="OnGridRowSelected(this)">
                                                                    </asp:DropDownList>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvResponsibility" ControlToValidate="ddlResponsibility" ValidationGroup="valUpdate"
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please select responsibility"
                                                                        InitialValue="-"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Role" SortExpression="role_id">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnRoleId" runat="server" Value='<%#Bind("role_id")%>' />
                                                                    <asp:DropDownList ID="ddlrole" runat="server" CssClass="ddlStyle" Width="90%"
                                                                        onchange="javascript: OnGridDataChange(this,'');" onclick="OnGridRowSelected(this)">
                                                                    </asp:DropDownList>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvRole" ControlToValidate="ddlrole" ValidationGroup="valUpdate"
                                                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please select role"
                                                                        InitialValue="-"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Payment Role" SortExpression="payment_role_id">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnPaymentRoleId" runat="server" Value='<%#Bind("payment_role_id")%>' />
                                                                    <asp:DropDownList ID="ddlPaymentRole" runat="server" CssClass="ddlStyle" Width="90%"
                                                                        onchange="javascript: OnGridDataChange(this,'');" onclick="OnGridRowSelected(this)">
                                                                    </asp:DropDownList>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Active" SortExpression="is_enabled">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnIsEnabled" runat="server" Value='<%#Bind("is_enabled")%>' />
                                                                    <asp:CheckBox ID="cbActive" runat="server" onclick="javascript: OnGridDataChange(this,'Chkbox');" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="3%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <table width="100%" style="float: right; table-layout: fixed">
                                                                        <tr style="float: right">
                                                                            <td align="right" style="float: right" width="50%">
                                                                                <asp:ImageButton ID="btnUpdate" ImageUrl="../Images/Save.png" runat="server" ToolTip="Edit user" OnClick="btnUpdate_Click"
                                                                                    ValidationGroup="valUpdate" OnClientClick="return ConfirmUpdate(this)" />
                                                                            </td>
                                                                            <td align="right" style="float: right" width="50%">
                                                                                <asp:ImageButton ID="imgBtnUndo" runat="server" ImageUrl="../Images/cancel_row3.png" OnClick="imgBtnUndo_Click"
                                                                                    OnClientClick="if (!ConfirmUpdate(this)) { return false;};"
                                                                                    ToolTip="Cancel" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                    <asp:HiddenField ID="hdnUserAccId" runat="server" Value='<%#Bind("user_account_id")%>' />
                                                                    <asp:HiddenField ID="hdnUserCode" runat="server" Value='<%#Bind("user_code")%>' />
                                                                    <asp:HiddenField ID="hdnRespCode" runat="server" Value='<%#Bind("responsibility_code")%>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="5">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="5">
                                    <table width="98.3%" cellpadding="0" cellspacing="0" style="border: solid 1px #C6D7ED;">
                                        <tr>
                                            <td width="10%" class="gridHeaderStyle_2rows">User Name</td>
                                            <td width="10%" class="gridHeaderStyle_2rows">User Code</td>
                                            <td width="15%" class="gridHeaderStyle_2rows">User Account Id</td>
                                            <td width="10%" class="gridHeaderStyle_2rows">Responsibility</td>
                                            <td width="10%" class="gridHeaderStyle_2rows">Role</td>
                                            <td width="10%" class="gridHeaderStyle_2rows">Payment Role</td>
                                            <td width="4%" class="gridHeaderStyle_2rows">Active</td>
                                            <td width="3%" class="gridHeaderStyle_2rows">&nbsp
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="10%" class="gridItemStyle_Left_Align">
                                                <asp:TextBox ID="txtUserNameAdd" runat="server" CssClass="identifierLable"
                                                    MaxLength="200" ToolTip="upto 200 chars" Width="88%" onchange="javascript: OnGridDataChangeInsert(this,'');"
                                                    onclick="OnGridRowSelectedAdd(this)"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvUserNameAdd" ControlToValidate="txtUserNameAdd" ValidationGroup="valAdd"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter user name" Display="Dynamic"></asp:RequiredFieldValidator>
                                            </td>
                                            <td width="10%" class="gridItemStyle_Center_Align">
                                                <asp:TextBox ID="txtUserCodeAdd" runat="server" CssClass="identifierLable"
                                                    MaxLength="10" ToolTip="upto 10 chars" Width="50%" onchange="javascript: OnGridDataChangeInsert(this,'');"
                                                    onclick="OnGridRowSelectedAdd(this)"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvUsercodeAdd" ControlToValidate="txtUserCodeAdd" ValidationGroup="valAdd"
                                                    Text="*" CssClass="requiredFieldValidator" InitialValue="" ToolTip="Please enter user code" Display="Dynamic"></asp:RequiredFieldValidator>
                                            </td>
                                            <td width="15%" class="gridItemStyle_Left_Align">
                                                <asp:TextBox ID="txtUserAccIdAdd" runat="server"
                                                    MaxLength="100" ToolTip="Domain\account id upto 100 chars" CssClass="identifierLable"
                                                    Width="90%" onchange="javascript: OnGridDataChangeInsert(this,'');"
                                                    onclick="OnGridRowSelectedAdd(this)"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvUserAccIdAdd" ControlToValidate="txtUserAccIdAdd" ValidationGroup="valAdd"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter user account id" Display="Dynamic"></asp:RequiredFieldValidator>
                                            </td>
                                            <td width="10%" class="gridItemStyle_Left_Align">
                                                <asp:DropDownList ID="ddlResponsibilityAdd" runat="server" CssClass="ddlStyle" Width="90%"
                                                    onchange="javascript: OnGridDataChangeInsert(this,'');" onclick="OnGridRowSelectedAdd(this)">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvResponsibilityAdd" ControlToValidate="ddlResponsibilityAdd" ValidationGroup="valAdd"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please select responsibility"
                                                    InitialValue="-"></asp:RequiredFieldValidator>
                                            </td>
                                            <td width="10%" class="gridItemStyle_Left_Align">
                                                <asp:DropDownList ID="ddlroleAdd" runat="server" CssClass="ddlStyle" Width="90%"
                                                    onchange="javascript: OnGridDataChangeInsert(this,'');" onclick="OnGridRowSelectedAdd(this)">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvRoleAdd" ControlToValidate="ddlroleAdd" ValidationGroup="valAdd"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please select role"
                                                    InitialValue="-"></asp:RequiredFieldValidator>
                                            </td>
                                            <td width="10%" class="gridItemStyle_Left_Align">
                                                <asp:DropDownList ID="ddlPaymentRoleAdd" runat="server" CssClass="ddlStyle" Width="90%"
                                                    onchange="javascript: OnGridDataChangeInsert(this,'');" onclick="OnGridRowSelectedAdd(this)">
                                                </asp:DropDownList>
                                            </td>
                                            <td width="4%" class="gridItemStyle_Center_Align">
                                                <asp:CheckBox ID="cbActiveAdd" runat="server" onclick="javascript: OnGridDataChangeInsert(this,'Chkbox');" />
                                            </td>
                                            <td width="3%" class="gridItemStyle_Center_Align">
                                                <table width="99%" style="float: right; table-layout: fixed">
                                                    <tr style="float: right">
                                                        <td align="right" style="float: right" width="50%">
                                                            <asp:ImageButton ID="btnAdd" ImageUrl="../Images/Save.png" runat="server" ToolTip="Add user" OnClick="btnAdd_Click"
                                                                OnClientClick="if (!ConfirmInsert()) { return false;};" ValidationGroup="valAdd" />
                                                        </td>
                                                        <td align="right" style="float: right" width="50%">
                                                            <asp:ImageButton ID="imgBtnCancel" runat="server" ImageUrl="../Images/cancel_row3.png" OnClientClick="if (!ConfirmInsert()) { return false;};"
                                                                ToolTip="Cancel" OnClick="imgBtnCancel_Click" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

            <asp:UpdateProgress ID="progressBarPageLevel" runat="server" AssociatedUpdatePanelID="updPnlPageLevel" DisplayAfter="100">
                <ProgressTemplate>
                    <div id="Search" style="font-weight: bold; color: Black">
                        <img src="../Images/InProgress2.gif" alt="" />
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <ajaxToolkit:ModalPopupExtender ID="mPopupPageLevel" runat="server" PopupControlID="progressBarPageLevel" TargetControlID="progressBarPageLevel"
                BackgroundCssClass="progressBar" RepositionMode="RepositionOnWindowResize">
            </ajaxToolkit:ModalPopupExtender>

            <%--Responsibility update popup--%>
            <asp:Button ID="dummyRespUpdate" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeRespUpdate" runat="server" PopupControlID="pnlRespUpdatePopup" TargetControlID="dummyRespUpdate"
                CancelControlID="btnSkipRespUpdate" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlRespUpdatePopup" runat="server" align="left" Width="30%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName">
                            <asp:Label ID="Label1" runat="server" Text="Responsibility update" CssClass="identifierLable_large"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="90%">
                                <tr>
                                    <td width="5%"></td>
                                    <td width="45%" class="identifierLable">New Responsibility
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtNewResp" runat="server" CssClass="identifierLable"
                                            Width="75%" ReadOnly="true"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3"></td>
                                </tr>
                                <tr>
                                    <td colspan="3"></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable">Responsibility to replace
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlRespToReplace" runat="server" CssClass="ddlStyle" Width="77%">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator runat="server" ID="rfvRespToReplace" ControlToValidate="ddlRespToReplace" ValidationGroup="valRespUpdate"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please select new responsibility"
                                            InitialValue="-"></asp:RequiredFieldValidator>

                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3"></td>
                                </tr>
                                <tr>
                                    <td colspan="3"></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td></td>
                                    <td>
                                        <table width="25%" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td>
                                                    <asp:Button ID="btnRespUpdate" runat="server" Text="Update" CssClass="ButtonStyle"
                                                        OnClick="btnRespUpdate_Click" />
                                                </td>
                                                <td width="5%">&nbsp </td>
                                                <td width="5%">&nbsp </td>
                                                <td>
                                                    <asp:Button ID="btnSkipRespUpdate" runat="server" Text="Skip" CssClass="ButtonStyle" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <br />
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <%--Warning popup--%>
            <asp:Button ID="dummyConfirm" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmation" runat="server" PopupControlID="pnlPopup" TargetControlID="dummyConfirm"
                CancelControlID="btnNoConfirm" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlPopup" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="Label2" runat="server" Text="Unsaved Data Warning" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblConfirmMsg" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td width="48%" align="right">
                                        <asp:Button ID="btnNoConfirm" runat="server" Text="Return" CssClass="ButtonStyle" Width="30%" />
                                    </td>
                                    <td width="4%"></td>
                                    <td width="48%" align="left">
                                        <asp:Button ID="btnYesConfirm" runat="server" Text="Exit" CssClass="ButtonStyle" Width="30%" OnClick="btnYesConfirm_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

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

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnUserRole" runat="server" />
            <asp:HiddenField ID="hdnChangeNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnChangeNotSavedAdd" runat="server" Value="N" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridRowSelectedPrvious" runat="server" />
            <asp:HiddenField ID="hdnUserSearchSelected" runat="server" Value="N" />
            <asp:HiddenField ID="hdnSearchListItemSelected" runat="server" />
            <asp:HiddenField ID="hdnNewResp" runat="server" />
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:TextBox>
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>



</asp:Content>



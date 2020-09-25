<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountTypeMaintenance.aspx.cs" Inherits="WARS.AccountTypeMaintenance" MasterPageFile="~/MasterPage.Master"
    Title="WARS - AccountTypeMaintenance " MaintainScrollPositionOnPostback="true" %>

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
                var PnlAccountTypeDetails = document.getElementById("<%=PnlAccountTypeDetails.ClientID %>");
                scrollTop = PnlAccountTypeDetails.scrollTop;

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
                var PnlAccountTypeDetails = document.getElementById("<%=PnlAccountTypeDetails.ClientID %>");
                PnlAccountTypeDetails.scrollTop = scrollTop;
            }


        }
        //======================= End


        //set flag value when data is changed in grid 
        function OnDataChange(row) {
            CompareRow(row);
        }


        function OnClickCheckbox(row) {
            OnGridRowSelected(row);
            CompareRow(row);
        }

        function CompareRow(row) {
            //debugger;
            var rowIndex = row.id.substring(row.id.lastIndexOf('_') + 1);
            var tempstr = row.id.substring(0, row.id.lastIndexOf('_'));
            var str = tempstr.substring(0, tempstr.lastIndexOf('_') + 1);

            //Harish 23-08-2018: Added _ to control to fix the issue of control not available
            var hdnSourceType = document.getElementById(str + 'hdnSourceType_' + rowIndex).value;
            var ddlSourceType = document.getElementById(str + 'ddlSourceType_' + rowIndex).value;
            var hdnConsolid = document.getElementById(str + 'hdnConsolid_' + rowIndex).value;
            var hdnIsInclude = document.getElementById(str + 'hdnIsInclude_' + rowIndex).value;

            var isConsolid;
            var cbConsolid = document.getElementById(str + 'cbConsolid_' + rowIndex);
            if (cbConsolid.checked == true) {
                isConsolid = 'Y';
            }
            else {
                isConsolid = 'N';
            }

            var isInclude;
            var cbIsInclude = document.getElementById(str + 'cbIsInclude_' + rowIndex);
            if (cbIsInclude.checked == true) {
                isInclude = 'Y';
            }
            else {
                isInclude = 'N';
            }

            if (hdnSourceType != ddlSourceType || hdnConsolid != isConsolid || hdnIsInclude != isInclude) {
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

        function OnClickCheckboxInsert() {
            ConfirmInsert();
            CompareRowInsert();
        }

        function CompareRowInsert() {
            var accountTypeCode = document.getElementById("<%=txtAccountTypeCode.ClientID %>").value;
            var accountTypedesc = document.getElementById("<%=ddlAccountTypeDescInsert.ClientID %>").value;
            var sourceType = document.getElementById("<%=ddlSourceTypeInsert.ClientID %>").value;
            var isConsolid = document.getElementById("<%=cbConsolidInsert.ClientID %>");
            var isInclude = document.getElementById("<%=cbInIncludeInsert.ClientID %>");

            if (accountTypeCode != "" || accountTypedesc != "-" || sourceType != "-" || isConsolid.checked == true || isInclude.checked == true) {
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
                    document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new account type. Save or Undo changes";
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
                document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new account type. Save or Undo changes";
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
                document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new account type. Save or Undo changes";
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

//grid panel height adjustment functioanlity - starts

function SetGrdPnlHeightOnLoad() {
    var windowHeight = window.screen.availHeight;
    var gridPanelHeight = windowHeight * 0.5;
    document.getElementById("<%=PnlAccountTypeDetails.ClientID %>").style.height = gridPanelHeight + "px";
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

//open Audit screen
function OpenAuditScreen() {
    //debugger;
    if (IsDataChanged()) {
        window.onbeforeunload = null;
        //OpenPopupOnUnSavedData('Audit Screen to be developed');
        alert("Audit Screen to be developed");
    }
    else {
        return true;
    }
}

//==============End

//On press of Enter key in search textbox
function OnAccountTypeKeyDown() {
    if ((event.keyCode == 13)) {
        document.getElementById('<%=btnHdnAccountTypeSearch.ClientID%>').click();
    }
}

//==============End

    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="7">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    COST ACCOUNT TYPE MAP
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="8%"></td>
                    <td width="8%" class="identifierLable_large_bold">Account Code</td>
                    <td width="24%">
                        <asp:TextBox ID="txtAccountTypeSearch" runat="server" Width="99%" CssClass="textboxStyle"
                            TabIndex="100" onfocus="return ConfirmSearch();" onkeydown="OnAccountTypeKeyDown();"></asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="tbweAccountTypeSearch" runat="server"
                            TargetControlID="txtAccountTypeSearch"
                            WatermarkText="Enter Search Text"
                            WatermarkCssClass="watermarked" />
                    </td>
                    <td width="3%" align="left"></td>
                    <td></td>
                    <td width="3%"></td>
                    <td align="right" width="12%">
                        <asp:Button ID="btnReset" runat="server" CssClass="ButtonStyle" OnClick="btnReset_Click" TabIndex="108" Text="Reset" UseSubmitBehavior="false" Width="98%"  onfocus="return ConfirmSearch();" />
                    </td>
                </tr>
                <tr>
                    <td colspan="6"></td>
                    <td align="right">
                        <asp:Button ID="btnAddAccountType" runat="server" Text="Add Account Type" CssClass="ButtonStyle" TabIndex="109"
                            Width="98%" UseSubmitBehavior="false" OnClick="btnAddAccountType_Click" onfocus="return ConfirmSearch();" />
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
                                                <asp:Panel ID="PnlAccountTypeDetails" runat="server" ScrollBars="Auto" Width="100%">
                                                    <div style="overflow-x: hidden; overflow: scroll; width: 100%; height: 100%" id="gridviewContainer">
                                                        <asp:GridView ID="gvAccountTypeDetails" runat="server" AutoGenerateColumns="False" Width="97%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                            CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                            EmptyDataText="No data found." OnRowDataBound="gvAccountTypeDetails_RowDataBound" OnRowCommand="gvAccountTypeDetails_RowCommand"
                                                            AllowSorting="true" OnSorting="gvAccountTypeDetails_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                            <Columns>
                                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Account Code" SortExpression="account_code_type">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblAccountTypeCode" runat="server" Text='<%# Bind("account_code_type") %>' CssClass="identifierLable"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="10%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Account Type Description" SortExpression="account_type_desc">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblAccountTypeDesc" runat="server" Text='<%# Bind("account_type_desc") %>' CssClass="identifierLable"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="25%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Source Type" SortExpression="account_type_cat">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hdnSourceType" runat="server" Value='<%# Bind("account_type_cat") %>' />
                                                                        <asp:DropDownList ID="ddlSourceType" runat="server" Width="80%" CssClass="ddlStyle" onchange="javascript: OnDataChange(this);" onfocus="OnGridRowSelected(this)"></asp:DropDownList>
                                                                        <asp:RequiredFieldValidator runat="server" ID="rfvSourceType" ControlToValidate="ddlSourceType" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>'
                                                                            Text="*" InitialValue="-" CssClass="requiredFieldValidator" ToolTip="Please enter source type" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="15%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Consolidate?" SortExpression="consolid_level">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hdnConsolid" runat="server" Value='<%# Bind("consolid_level") %>' />
                                                                        <asp:CheckBox ID="cbConsolid" runat="server" onclick="javascript: OnClickCheckbox(this);" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="15%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Include?" SortExpression="is_include">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hdnIsInclude" runat="server" Value='<%# Bind("is_include") %>' />
                                                                        <asp:CheckBox ID="cbIsInclude" runat="server" onclick="javascript: OnClickCheckbox(this);" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="15%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Display Order" SortExpression="display_order">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblDisplayOrder" runat="server" Text='<%# Bind("display_order") %>' CssClass="identifierLable"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="15%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Right_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                    <ItemTemplate>
                                                                        <table width="100%" style="float: right; table-layout: fixed">
                                                                            <tr style="float: right">
                                                                                <td align="right" style="float: right" width="50%">
                                                                                    <asp:ImageButton ID="imgBtnSave" runat="server" CommandName="saverow" ImageUrl="../Images/save.png" ToolTip="Save" CausesValidation="true"
                                                                                        ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>' OnClientClick="return ConfirmUpdate(this)" />
                                                                                </td>
                                                                                <td align="right" style="float: right" width="50%">
                                                                                    <asp:ImageButton ID="imgBtnUndo" runat="server" CommandName="cancelrow" ImageUrl="../Images/cancel_row3.png"
                                                                                        ToolTip="Cancel"  OnClientClick="return ConfirmUpdate(this)" />
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
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
                                                        <td width="10%" class="gridHeaderStyle_1row">Account Code</td>
                                                        <td width="25%" class="gridHeaderStyle_1row">Account Type Description</td>
                                                        <td width="15%" class="gridHeaderStyle_1row">Source Type</td>
                                                        <td width="15%" class="gridHeaderStyle_1row">Consolidate?</td>
                                                        <td width="15%" class="gridHeaderStyle_1row">Include?</td>
                                                        <td width="15%" class="gridHeaderStyle_1row">Display Order</td>
                                                        <td width="5%" class="gridHeaderStyle_1row">&nbsp</td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:TextBox ID="txtAccountTypeCode" runat="server" CssClass="textboxStyle" TabIndex="101" Width="70%" MaxLength="3" onchange="javascript: OnDataChangeInsert();"
                                                                onfocus="return ConfirmInsert();"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvAccountTypeCode" ControlToValidate="txtAccountTypeCode" ValidationGroup="valInsertAccountType"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter account code" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td class="insertBoxStyle" align="left">
                                                            <asp:DropDownList ID="ddlAccountTypeDescInsert" runat="server" Width="98%" CssClass="ddlStyle" TabIndex="102" onchange="javascript: OnDataChangeInsert();" AutoPostBack="true" onfocus="ConfirmInsert()" OnSelectedIndexChanged="ddlAccountTypeDescInsert_SelectedIndexChanged"></asp:DropDownList>
                                                        </td>
                                                        <td class="insertBoxStyle" align="left">
                                                            <asp:DropDownList ID="ddlSourceTypeInsert" runat="server" Width="90%" CssClass="ddlStyle" TabIndex="103" onchange="javascript: OnDataChangeInsert();" onfocus="ConfirmInsert()"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvSourceTypeInsert" ControlToValidate="ddlSourceTypeInsert" ValidationGroup="valInsertAccountType"
                                                                Text="*" InitialValue="-" CssClass="requiredFieldValidator" ToolTip="Please enter source type" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td class="insertBoxStyle_No_Padding" align="center">
                                                            <asp:CheckBox ID="cbConsolidInsert" runat="server" TabIndex="103" onclick="javascript: OnClickCheckboxInsert();" />
                                                        </td>
                                                        <td class="insertBoxStyle_No_Padding" align="center">
                                                            <asp:CheckBox ID="cbInIncludeInsert" runat="server" TabIndex="104" onclick="javascript: OnClickCheckboxInsert();" />
                                                        </td>
                                                        <td class="insertBoxStyle" align="center">
                                                            <%--<asp:TextBox ID="txtDisplayOrderInsert" runat="server" CssClass="textboxStyle_readonly" Width="90%" ReadOnly="true" OnkeyDown="return OnKeyDown();"></asp:TextBox>--%>
                                                            <asp:Label ID="lblDisplayOrderInsert" runat="server" CssClass="textboxStyle_readonly" Width="90%" Height="17px" Style="vertical-align: middle" />
                                                        </td>
                                                        <td class="insertBoxStyle_No_Padding">
                                                            <table width="100%" style="float: right; table-layout: fixed">
                                                                <tr style="float: right">
                                                                    <td align="right" style="float: right" width="50%">
                                                                        <asp:ImageButton ID="imgBtnInsert" runat="server" CommandName="saverow" TabIndex="106" ImageUrl="../Images/save.png" ToolTip="Insert account type" OnClick="imgBtnInsert_Click" ValidationGroup="valInsertAccountType" />
                                                                    </td>
                                                                    <td align="right" style="float: right" width="50%">
                                                                        <asp:ImageButton ID="imgBtnCancel" runat="server" CommandName="cancelrow" TabIndex="107" ImageUrl="../Images/cancel_row3.png"
                                                                            ToolTip="Cancel" OnClick="imgBtnCancel_Click" />
                                                                    </td>
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
                    <td></td>
                    <td valign="top" align="right">
                        <asp:Button ID="btnAccountTypeAudit" runat="server" Text="Audit" CssClass="ButtonStyle" TabIndex="110"
                            Width="98%" UseSubmitBehavior="false" onkeydown="OnTabPress();" OnClick="btnAccountTypeAudit_Click" OnClientClick="if (!OpenAuditScreen()) { return false;};" />
                    </td>
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

            <%--popup to insert configuration groups--%>
            <asp:Button ID="dummySave" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeAddAccountType" runat="server" PopupControlID="pnlInsertGroup" TargetControlID="dummySave"
                CancelControlID="btnNo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlInsertGroup" runat="server" align="center" Width="30%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="Label2" runat="server" Text="Add Account Type" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td></td>
                                    <td colspan="3" align="left">
                                        <asp:Label ID="Label1" runat="server" CssClass="identifierLable" Text="Enter New Account Type details:"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td width="12.5%"></td>
                                    <td class="identifierLable" width="25%" align="left">Description</td>
                                    <td width="50%" align="left" style="padding-left: 10px;">
                                        <asp:TextBox ID="txtDesciption" runat="server" Width="90%" CssClass="textboxStyle"
                                            MaxLength="20" ToolTip="Description upto 10 characters"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rfvDesciption" ControlToValidate="txtDesciption" ValidationGroup="ValAddAccoutType"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please descpription" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                    <td width="12.5%"></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="identifierLable" align="left">Display Order</td>
                                    <td align="left" style="padding-left: 10px;">
                                        <asp:TextBox ID="txtDisplayOrder" runat="server" Width="90%" CssClass="textboxStyle"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rfvDisplayOrder" ControlToValidate="txtDisplayOrder" ValidationGroup="ValAddAccoutType"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter display order" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revDisplayOrder" runat="server" Text="*" ControlToValidate="txtDisplayOrder"
                                            ValidationExpression="^[+]?\d+$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="ValAddAccoutType"
                                            ToolTip="Please enter only integers" Display="Dynamic"> </asp:RegularExpressionValidator>
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <table width="100%">
                                            <tr>
                                                <td align="right" width="49%">
                                                    <asp:Button ID="btnInsertAccountType" runat="server" Text="Save" CssClass="ButtonStyle"
                                                        OnClick="btnInsertAccountType_Click" ValidationGroup="ValAddAccoutType" UseSubmitBehavior="false" />
                                                </td>
                                                <td width="2%"></td>
                                                <td align="left" width="49%">
                                                    <asp:Button ID="btnNo" runat="server" Text="Cancel" CssClass="ButtonStyle" UseSubmitBehavior="false" />
                                                </td>
                                            </tr>
                                        </table>

                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

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
            <asp:HiddenField ID="hdnSearchText" runat="server" Value="" />
            <asp:HiddenField ID="hdnChangeNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnInsertDataNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridRowSelectedPrvious" runat="server" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:Button ID="btnHdnAccountTypeSearch" runat="server" Style="display: none;" OnClick="btnHdnAccountTypeSearch_Click" CausesValidation="false" />
            <%--<asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField" ReadOnly="true" onFocus="MoveHiddenTextboxFocus();"></asp:TextBox>--%>
            <asp:Label ID="lblTab" runat="server" TabIndex="99"></asp:Label>
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomerMaintenance.aspx.cs" Inherits="WARS.CustomerMaintenance" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Customer Maintenance " MaintainScrollPositionOnPostback="true" %>


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
                var PnlCustomerDetails = document.getElementById("<%=PnlCustomerDetails.ClientID %>");
                scrollTop = PnlCustomerDetails.scrollTop;

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
                var PnlCustomerDetails = document.getElementById("<%=PnlCustomerDetails.ClientID %>");
                PnlCustomerDetails.scrollTop = scrollTop;
            }

        }
        //======================= End             

        //set flag value when data is changed in grid 
        //debugger;
        function OnDataChange(row) {
            CompareRow(row);
        }


        function OnClickCheckbox(row) {
            OnGridRowSelected(row);
            CompareRow(row);
        }

        function CompareRow(row) {
            //var rowIndex = row.id.substring(row.id.lastIndexOf('_'));
            var rowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            var tempstr = row.id.substring(0, row.id.lastIndexOf('_'));
            var str = tempstr.substring(0, tempstr.lastIndexOf('_') + 1);

            var hdnFixedMobile = document.getElementById(str + 'hdnFixedMobile' + "_" + rowIndex).value;
            var ddlFixedMobile = document.getElementById(str + 'ddlFixedMobile' + "_" + rowIndex).value;
            var hdnDisplayOnGlobalStmt = document.getElementById(str + 'hdnDisplayOnGlobalStmt' + "_" + rowIndex).value;
            var hdnDisplayOnAccountStmt = document.getElementById(str + 'hdnDisplayOnAccountStmt' + "_" + rowIndex).value;

            var isDisplayOnGlobalStmt;
            var cbDisplayOnGlobalStmt = document.getElementById(str + 'cbDisplayOnGlobalStmt' + "_" + rowIndex);
            if (cbDisplayOnGlobalStmt.checked == true) {
                isDisplayOnGlobalStmt = 'Y';
            }
            else {
                isDisplayOnGlobalStmt = 'N';
            }

            var isDisplayOnAccountStmt;
            var cbDisplayOnAccountStmt = document.getElementById(str + 'cbDisplayOnAccountStmt' + "_" + rowIndex);
            if (cbDisplayOnAccountStmt.checked == true) {
                isDisplayOnAccountStmt = 'Y';
            }
            else {
                isDisplayOnAccountStmt = 'N';
            }

            if (hdnFixedMobile != ddlFixedMobile || hdnDisplayOnGlobalStmt != isDisplayOnGlobalStmt || hdnDisplayOnAccountStmt != isDisplayOnAccountStmt) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
            }
            else {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "N";
            }

        }

        //Show warning while closing the window if changed data not saved 
        function WarnOnUnSavedData() {
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            var isDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
            if (isExceptionRaised != "Y" && isDataChanged == "Y") {
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
            if (isDataChanged == "Y") {
                return true;
            }
            else {
                return false;
            }
        }

        //Validate any unsaved data on browser window close/refresh
        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "ExceptionPage.aspx";
        }

        //Validation: warning message if changes made and not saved

        function OnGridRowSelected(row) {
            var rowData = row.parentNode.parentNode;
            var rowIndex = rowData.rowIndex - 1;

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
                    else {
                        document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").innerText = rowIndex;
                    }
                }
        }

        function ConfirmSearch() {
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


//grid panel height adjustment functioanlity - starts

function SetGrdPnlHeightOnLoad() {
    var windowHeight = window.screen.availHeight;
    var gridPanelHeight = windowHeight * 0.6;
    document.getElementById("<%=PnlCustomerDetails.ClientID %>").style.height = gridPanelHeight + "px";
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
function OnCustomerKeyDown() {
    if ((event.keyCode == 13)) {
        document.getElementById('<%=btnHdnSearch.ClientID%>').click();
    }
}

function OnLocalCustomerKeyDown() {
    if ((event.keyCode == 13)) {
        document.getElementById('<%=btnHdnSearch.ClientID%>').click();
    }
}

function OnSourceCountryKeyDown() {
    if ((event.keyCode == 13)) {
        document.getElementById('<%=btnHdnSearch.ClientID%>').click();
    }
}
//=============== End
    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="12">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    CUSTOMER MAINTENANCE
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="1%"></td>
                    <td width="5%" class="identifierLable_large_bold">Customer</td>
                    <td width="18%">
                        <asp:TextBox ID="txtCustomerSearch" runat="server" Width="97%" CssClass="textboxStyle"
                            TabIndex="100" onfocus="return ConfirmSearch();" onkeydown="OnCustomerKeyDown();"></asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="tbweCustomerSearch" runat="server"
                            TargetControlID="txtCustomerSearch"
                            WatermarkText="Enter Search Text"
                            WatermarkCssClass="watermarked" />
                    </td>
                    <td width="2%"></td>
                    <td width="9%" class="identifierLable_large_bold">Local Customer No</td>
                    <td width="12%">
                        <asp:TextBox ID="txtLocalCustomerNo" runat="server" Width="97%" CssClass="textboxStyle"
                            TabIndex="101" onfocus="return ConfirmSearch();" onkeydown="OnLocalCustomerKeyDown();"></asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="tbweLocalCustomerNo" runat="server"
                            TargetControlID="txtLocalCustomerNo"
                            WatermarkText="Enter Search Text"
                            WatermarkCssClass="watermarked" />
                    </td>
                    <td width="2%"></td>
                    <td width="8%" class="identifierLable_large_bold">Source Country</td>
                    <td width="12%">
                        <asp:TextBox ID="txtSourceCountry" runat="server" Width="97%" CssClass="textboxStyle"
                            TabIndex="102" onfocus="return ConfirmSearch();" onkeydown="OnSourceCountryKeyDown();"></asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="tbweSourceCountry" runat="server"
                            TargetControlID="txtSourceCountry"
                            WatermarkText="Enter Search Text"
                            WatermarkCssClass="watermarked" />
                    </td>
                    <td></td>
                    <td align="right" colspan="2">
                        <asp:Button ID="btnReset" runat="server" CssClass="ButtonStyle" OnClick="btnReset_Click" TabIndex="103" Text="Reset" UseSubmitBehavior="false" Width="98%"  onfocus="return ConfirmSearch();"/>
                    </td>
                </tr>
                <tr>
                    <td colspan="10"></td>
                    <td align="right" colspan="2">
                        <asp:Button ID="btnAudit" runat="server" CssClass="ButtonStyle" OnClick="btnAudit_Click" TabIndex="104" Text="Audit" UseSubmitBehavior="false" Width="98%" onkeydown="OnTabPress();" />
                    </td>
                </tr>
                <tr>
                    <td colspan="10"></td>
                    <td width="11%"></td>
                    <td width="1%"></td>
                </tr>
                <tr>
                    <td colspan="12" class="table_with_border">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="1%">
                                    <br />
                                </td>
                                <td width="99%"></td>
                                <%--<td width="1%"></td>--%>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlCustomerDetails" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvCustomerDetails" runat="server" AutoGenerateColumns="False" Width="98.85%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." OnRowDataBound="gvCustomerDetails_RowDataBound" OnRowCommand="gvCustomerDetails_RowCommand"
                                                        AllowSorting="true" OnSorting="gvCustomerDetails_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Customer Id" SortExpression="customer_id">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblCustomerId" runat="server" Text='<%# Bind("customer_id") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="6%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Local Customer No." SortExpression="local_customer_code">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblLocalCustomerNo" runat="server" Text='<%# Bind("local_customer_code") %>' CssClass="identifierLable" ToolTip='<%# Bind("local_customer_code") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="7%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Account Name" SortExpression="account_name">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblAccountName" runat="server" Text='<%# Bind("account_name") %>' CssClass="identifierLable" ToolTip='<%# Bind("account_name") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="14%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Source Country" SortExpression="source_country_code">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblSourceCountry" runat="server" Text='<%# Bind("source_country_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="6%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Global Customer Code" SortExpression="global_customer_code">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblGlobalCustomerCode" runat="server" Text='<%# Bind("global_customer_code") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="6%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Global Customer Name" SortExpression="global_customer_name">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblGlobalCustomerName" runat="server" Text='<%# Bind("global_customer_name") %>' CssClass="identifierLable" ToolTip='<%# Bind("global_customer_name") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="14%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Group Name" SortExpression="group_name">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblGroupName" runat="server" Text='<%# Bind("group_name") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Customer Type" SortExpression="customer_desc">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblCustomerType" runat="server" Text='<%# Bind("customer_desc") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="6%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Central DSP" SortExpression="is_central_dsp">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblIsCentralDSP" runat="server" Text='<%# Bind("is_central_dsp") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="5%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Fixed / Mobile" SortExpression="fixed_mobile">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnFixedMobile" runat="server" Value='<%# Bind("fixed_mobile") %>' />
                                                                    <asp:DropDownList ID="ddlFixedMobile" runat="server" Width="82%" CssClass="ddlStyle" onchange="javascript: OnDataChange(this);" onfocus="OnGridRowSelected(this)"></asp:DropDownList>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvFixedMobile" ControlToValidate="ddlFixedMobile" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>'
                                                                        Text="*" InitialValue="-" CssClass="requiredFieldValidator" ToolTip="Please select fixed/mobile" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="6%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Display On Global Statement" SortExpression="display_on_statement_global">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnDisplayOnGlobalStmt" runat="server" Value='<%# Bind("display_on_statement_global") %>' />
                                                                    <asp:CheckBox ID="cbDisplayOnGlobalStmt" runat="server" onclick="javascript: OnClickCheckbox(this);" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="7%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Display On Account Statement" SortExpression="display_on_statement_account">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnDisplayOnAccountStmt" runat="server" Value='<%# Bind("display_on_statement_account") %>' />
                                                                    <asp:CheckBox ID="cbDisplayOnAccountStmt" runat="server" onclick="javascript: OnClickCheckbox(this);" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="7%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Account Sales Channel" SortExpression="account_sales_channel">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblAccountSalesChannel" runat="server" Text='<%# Bind("account_sales_channel") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="5%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="3%" ItemStyle-CssClass="gridItemStyle_Right_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <table width="100%" style="float: right; table-layout: fixed">
                                                                        <tr style="float: right">
                                                                            <td align="right" style="float: right" width="50%">
                                                                                <asp:ImageButton ID="imgBtnSave" runat="server" CommandName="saverow" ImageUrl="../Images/save.png" ToolTip="Save"
                                                                                    ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>' onfocus="return ConfirmUpdate(this);" />
                                                                            </td>
                                                                            <td align="right" style="float: right" width="50%">
                                                                                <asp:ImageButton ID="imgBtnUndo" runat="server" CommandName="cancelrow" ImageUrl="../Images/cancel_row3.png"  onfocus="return ConfirmSearch();"
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
                                                            ClientIDMode="AutoID" Enabled='<%# Eval("Enabled") %>' OnClick="lnkPage_Click" CssClass="gridPager"> </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <%--<td></td>--%>
                            </tr>
                        </table>

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
            <asp:HiddenField ID="hdnPageNumber" runat="server" Value="" />
            <asp:HiddenField ID="hdnIsValidSearch" runat="server" Value="N" />
            <asp:HiddenField ID="hdnChangeNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridRowSelectedPrvious" runat="server" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:Button ID="btnHdnSearch" runat="server" Style="display: none;" OnClick="btnHdnSearch_Click" CausesValidation="false" />
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:TextBox>
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>


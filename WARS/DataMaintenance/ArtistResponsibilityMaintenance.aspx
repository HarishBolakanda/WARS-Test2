<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="ArtistResponsibilityMaintenance.aspx.cs" Inherits="WARS.DataMaintenance.ArtistResponsibilityMaintenance" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Artist Maintenance " MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>


<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderBody">

    <script type="text/javascript">


        //probress bar and scroll position functionality - starts
        //to remain scroll position of grid panel and window
        var xPos, yPos;
        var scrollTop;
        var gridClientId = "ContentPlaceHolderBody_gvArtistDetails_";
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
                var PnlArtistDetails = document.getElementById("<%=PnlArtistDetails.ClientID %>");
                scrollTop = PnlArtistDetails.scrollTop;

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
                var PnlArtistDetails = document.getElementById("<%=PnlArtistDetails.ClientID %>");
                PnlArtistDetails.scrollTop = scrollTop;
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

            var hdnTeamResponsibility = document.getElementById(gridClientId + 'hdnTeamResponsibility_' + rowIndex).value;
            var ddlTeamResponsibility = document.getElementById(gridClientId + 'ddlTeamResponsibility_' + rowIndex).value;
            var hdnManagerResponsibility = document.getElementById(gridClientId + 'hdnManagerResponsibility_' + rowIndex).value;
            var ddlManagerResponsibility = document.getElementById(gridClientId + 'ddlManagerResponsibility_' + rowIndex).value;

            if (hdnTeamResponsibility != ddlTeamResponsibility || hdnManagerResponsibility != ddlManagerResponsibility) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
            }
            else {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "N";
            }

        }

        function CompareRowInsert() {
            var artistName = document.getElementById("<%=txtArtistNameInsert.ClientID %>").value;
            var dealType = document.getElementById("<%=ddlDealTypeInsert.ClientID %>").value;
            var teamResponsibility = document.getElementById("<%=ddlTeamRespInsert.ClientID %>").value;
            var ManagerResponsibility = document.getElementById("<%=ddlManagerRespInsert.ClientID %>").value;

            if (artistName != "" || dealType != "-" || teamResponsibility != "-" || ManagerResponsibility != "-") {
                document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").innerText = "Y";
            }
            else {
                document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").innerText = "N";
            }
        }

        function ClearArtistAddRow() {
            document.getElementById("<%=txtArtistNameInsert.ClientID %>").value = "";
            document.getElementById("<%=ddlDealTypeInsert.ClientID %>").value = "-";
            document.getElementById("<%=ddlTeamRespInsert.ClientID %>").value = "-";
            document.getElementById("<%=ddlManagerRespInsert.ClientID %>").value = "-";
            document.getElementById("<%=rfvArtistNameInsert.ClientID %>").style.display = "none";
            document.getElementById("<%=rfvDealTypeInsert.ClientID %>").style.display = "none";
            document.getElementById("<%=rfvTeamRespInsert.ClientID %>").style.display = "none";
            document.getElementById("<%=rfvManagerRespInsert.ClientID %>").style.display = "none";
            document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value = "N";
            return false;
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
            if ((isDataChanged == "Y")) {
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
                    document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new artist. Save or Undo changes";
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
                document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new artist. Save or Undo changes";
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

        function ConfirmUpdate(row) {
            var rowData = row.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
            var rowIndex = rowData.rowIndex - 1;

            if (document.getElementById("<%=hdnInsertDataNotSaved.ClientID %>").value == "Y") {
                var popup = $find('<%= mpeSaveUndo.ClientID %>');
                if (popup != null) {
                    document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new artist. Save or Undo changes";
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


    //grid panel height adjustment functioanlity - starts

    function SetGrdPnlHeightOnLoad() {
        var windowHeight = window.screen.availHeight;
        var gridPanelHeight = windowHeight * 0.5;
        document.getElementById("<%=PnlArtistDetails.ClientID %>").style.height = gridPanelHeight + "px";
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
        function OnArtistKeyDown() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnHdnArtistSearch.ClientID%>').click();
        }
    }

    //==============End

    //open Audit screen
    function OpenAuditScreen() {
        //debugger;
        if (IsDataChanged()) {
            window.onbeforeunload = null;
            OpenPopupOnUnSavedData('../Audit/ArtistAudit.aspx');
        }
        else {
            return true;
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
                                    ARTIST MAINTENANCE
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="5%"></td>
                    <td width="5%" class="identifierLable_large_bold">Artist</td>
                    <td width="24%">
                        <asp:TextBox ID="txtArtist" runat="server" Width="86%" CssClass="textboxStyle"
                            TabIndex="100" onfocus="return ConfirmSearch();" onkeydown="OnArtistKeyDown();"></asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="tbweArtist" runat="server"
                            TargetControlID="txtArtist"
                            WatermarkText="Artist Name Search"
                            WatermarkCssClass="watermarked" />
                    </td>
                    <td width="3%" align="left"></td>
                    <td></td>
                    <td width="10%"></td>
                    <td align="right" width="12%">
                        <asp:Button ID="btnReset" runat="server" CssClass="ButtonStyle" OnClick="btnReset_Click" TabIndex="109" Text="Reset" UseSubmitBehavior="false" Width="100%" onfocus="return ConfirmSearch();" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">Deal Type</td>
                    <td colspan="3">
                        <table cellpadding="0" cellspacing="0" width="50%">
                            <tr>
                                <td width="30%">
                                    <asp:DropDownList ID="ddlDealType" TabIndex="101" AutoPostBack="true" OnSelectedIndexChanged="ddlDropdown_SelectedIndexChanged" CssClass="ddlStyle" runat="server" onfocus="return ConfirmSearch();"
                                        Width="90%">
                                    </asp:DropDownList>
                                </td>
                                <td width="5%"></td>
                                <td width="22%" class="identifierLable_large_bold">Team Responsibility
                                </td>
                                <td width="30%">
                                    <asp:DropDownList ID="ddlResponsibility" AutoPostBack="true" TabIndex="102" OnSelectedIndexChanged="ddlDropdown_SelectedIndexChanged" CssClass="ddlStyle" Width="90%" runat="server" onfocus="return ConfirmSearch();"></asp:DropDownList>
                                </td>
                                <td></td>
                            </tr>
                        </table>
                    </td>
                    <td></td>
                    <td align="right" width="12%">
                        <asp:Button ID="btnAudit" runat="server" CssClass="ButtonStyle" OnClick="btnAudit_Click" onfocus="return ConfirmSearch();" TabIndex="110" Text="Audit" OnClientClick="if (!OpenAuditScreen()) { return false;};" UseSubmitBehavior="false" Width="100%" OnKeyDown="OnTabPress();" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                        <br />
                    </td>
                    <td class="table_header_with_border" colspan="2" runat="server" id="tdDataHeader">Artist Details</td>
                    <td colspan="4"></td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="4" class="table_with_border">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="1%">
                                    <br />
                                </td>
                                <td width="98%"></td>
                                <td width="%1"></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlArtistDetails" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvArtistDetails" runat="server" AutoGenerateColumns="False" Width="98.25%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." OnRowDataBound="gvArtistDetails_RowDataBound" OnRowCommand="gvArtistDetails_RowCommand"
                                                        AllowSorting="true" OnSorting="gvArtistDetails_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Artist Name" SortExpression="artist_name">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnArtistId" runat="server" Value='<%# Bind("artist_id") %>' />
                                                                    <asp:Label ID="lblArtistName" runat="server" Text='<%# Bind("artist_name") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="30%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Deal Type" SortExpression="deal_type_desc">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblDealType" runat="server" Text='<%# Bind("deal_type_desc") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="25%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Team Responsibility" SortExpression="team_responsibility">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnTeamResponsibility" runat="server" Value='<%# Bind("team_responsibility") %>' />
                                                                    <asp:DropDownList ID="ddlTeamResponsibility" runat="server" Width="93%" CssClass="ddlStyle" onchange="javascript: OnDataChange(this);" onfocus="OnGridRowSelected(this)"></asp:DropDownList>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvTeamResponsibility" ControlToValidate="ddlTeamResponsibility" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>'
                                                                        Text="*" InitialValue="-" CssClass="requiredFieldValidator" ToolTip="Please select responsibility" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Manager Responsibility" SortExpression="manager_responsibility">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdnManagerResponsibility" runat="server" Value='<%# Bind("manager_responsibility") %>' />
                                                                    <asp:DropDownList ID="ddlManagerResponsibility" runat="server" Width="93%" CssClass="ddlStyle" onchange="javascript: OnDataChange(this);" onfocus="OnGridRowSelected(this)"></asp:DropDownList>
                                                                    <asp:RequiredFieldValidator runat="server" ID="rfvManagerResponsibility" ControlToValidate="ddlManagerResponsibility" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>'
                                                                        Text="*" InitialValue="-" CssClass="requiredFieldValidator" ToolTip="Please select responsibility" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="18%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="7%" ItemStyle-CssClass="gridItemStyle_Right_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <table width="100%" style="float: right; table-layout: fixed">
                                                                        <tr style="float: right">
                                                                            <td align="right" style="float: right" width="50%">
                                                                                <asp:ImageButton ID="imgBtnSave" runat="server" CommandName="saverow" ImageUrl="../Images/save.png" ToolTip="Save" CausesValidation="true"  
                                                                                    ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>' OnClientClick="return ConfirmUpdate(this)" />
                                                                            </td>
                                                                            <td align="right" style="float: right" width="50%">
                                                                                <asp:ImageButton ID="imgBtnUndo" runat="server" CommandName="cancelrow" ImageUrl="../Images/cancel_row3.png" OnClientClick="return ConfirmUpdate(this)" 
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
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <table width="98.25%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="30%" class="gridHeaderStyle_1row">Artist Name</td>
                                                        <td width="25%" class="gridHeaderStyle_1row">Deal Type</td>
                                                        <td width="20%" class="gridHeaderStyle_1row">Team Responsibility</td>
                                                        <td width="18%" class="gridHeaderStyle_1row">Manager Responsibility</td>
                                                        <td width="7%" class="gridHeaderStyle_1row">&nbsp</td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:TextBox ID="txtArtistNameInsert" runat="server" CssClass="textboxStyle" style="text-transform:uppercase" TabIndex="103" Width="93%" MaxLength="30" onchange="javascript: CompareRowInsert();"
                                                                onfocus="return ConfirmInsert();"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvArtistNameInsert" ControlToValidate="txtArtistNameInsert" ValidationGroup="valInsertArtist"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter artist name" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td class="insertBoxStyle" align="left">
                                                            <asp:DropDownList ID="ddlDealTypeInsert" runat="server" Width="93%" CssClass="ddlStyle" TabIndex="104" onchange="javascript: CompareRowInsert();" onfocus="return ConfirmInsert();"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvDealTypeInsert" ControlToValidate="ddlDealTypeInsert" ValidationGroup="valInsertArtist"
                                                                Text="*" InitialValue="-" CssClass="requiredFieldValidator" ToolTip="Please select deal type" Display="Dynamic"></asp:RequiredFieldValidator>

                                                        </td>
                                                        <td class="insertBoxStyle" align="left">
                                                            <asp:DropDownList ID="ddlTeamRespInsert" runat="server" Width="93%" CssClass="ddlStyle" TabIndex="105" onchange="javascript: CompareRowInsert();" onfocus="return ConfirmInsert();"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvTeamRespInsert" ControlToValidate="ddlTeamRespInsert" ValidationGroup="valInsertArtist"
                                                                Text="*" InitialValue="-" CssClass="requiredFieldValidator" ToolTip="Please select team responsibility" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td class="insertBoxStyle" align="left">

                                                            <asp:DropDownList ID="ddlManagerRespInsert" runat="server" Width="93%" CssClass="ddlStyle" TabIndex="106" onchange="javascript: CompareRowInsert();" onfocus="return ConfirmInsert();"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvManagerRespInsert" ControlToValidate="ddlManagerRespInsert" ValidationGroup="valInsertArtist"
                                                                Text="*" InitialValue="-" CssClass="requiredFieldValidator" ToolTip="Please select manager responsibility" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td class="insertBoxStyle_No_Padding">
                                                            <table width="100%" style="float: right; table-layout: fixed">
                                                                <tr style="float: right">
                                                                    <td align="right" style="float: right" width="50%">
                                                                        <asp:ImageButton ID="imgBtnInsert" runat="server" TabIndex="107" ImageUrl="../Images/save.png" ToolTip="Insert artist" ValidationGroup="valInsertArtist" OnClick="imgBtnArtistInsert_Click" />
                                                                    </td>
                                                                    <td align="right" style="float: right" width="50%">
                                                                        <asp:ImageButton ID="imgBtnCancel" runat="server" TabIndex="108" ImageUrl="../Images/cancel_row3.png" OnClientClick="return ClearArtistAddRow();" 
                                                                            ToolTip="Cancel" />
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
                                <td colspan="2"></td>
                            </tr>
                        </table>
                    </td>
                    <td></td>
                    <td valign="top" align="right"></td>
                </tr>
            </table>

            <%--Save/Undo changes popup--%>
            <asp:Button ID="dummySaveUndo" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeSaveUndo" runat="server" PopupControlID="pnlSaveUndo" TargetControlID="dummySaveUndo"
                CancelControlID="btnClosePopupSaveUndo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlSaveUndo" runat="server" align="center" Width="25%" CssClass="popupPanel"  Style="z-index: 1; display: none">
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
            <asp:HiddenField ID="hdnSearchText" runat="server" Value="" />
            <asp:HiddenField ID="hdnChangeNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridRowSelectedPrvious" runat="server" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnInsertDataNotSaved" runat="server" Value="N" />
            <asp:Button ID="btnHdnArtistSearch" runat="server" Style="display: none;" OnClick="btnHdnArtistSearch_Click" CausesValidation="false" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99"></asp:Label>
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

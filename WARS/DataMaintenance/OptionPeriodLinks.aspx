<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OptionPeriodLinks.aspx.cs" Inherits="WARS.OperationPeriodLinks" MasterPageFile="~/MasterPage.Master"
    Title="WARS - OptionPeriodLinks" MaintainScrollPositionOnPostback="true" %>

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
            var gridPanelHeight = windowHeight * 0.53;
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
        }

        //grid panel height adjustment functioanlity - ends    

        //Royaltor auto populate search functionalities

        function royaltorPopulating() {
            txtRoy = document.getElementById("<%= txtRoyaltor.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "N";
        }

        function royaltorPopulated() {
            txtRoy = document.getElementById("<%= txtRoyaltor.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function royaltorListItemSelected(sender, args) {
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                document.getElementById("<%= txtRoyaltor.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "Y";
            }

        }

        function artistPopulating() {
            txtRoy = document.getElementById("<%= txtArtist.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "N";
        }

        function artistPopulated() {
            txtRoy = document.getElementById("<%= txtArtist.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function artistListItemSelected(sender, args) {
            var artistSrchVal = args.get_value();
            if (artistSrchVal == 'No results found') {
                document.getElementById("<%= txtArtist.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "Y";
            }

        }
        //================================End

        //Validation: warning message if changes made and not saved or on page change             
        //set flag value when grid data is changed
        function OnGridDataChange() {
            document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";

        }


        //Validate any unsaved data on browser window close/refresh
        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        function WarnOnUnSavedData() {
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            var isGridDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
            if (isExceptionRaised != "Y") {
                if ((isGridDataChanged == "Y")) {
                    return warningMsgOnUnSavedData;
                }
            }

        }
        window.onbeforeunload = WarnOnUnSavedData;

        //used to check if any changes to allow navigation to other screen 
        function IsDataChanged() {
            var isGridDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
            if (isGridDataChanged == "Y") {
                return true;
            }
            else {
                return false;
            }
        }

        function ConfirmSearch() {
            var checkcount = 0;
            var isGridPopulated = document.getElementById("<%=isGridPopulated.ClientID %>").value;
            var txtRoyaltor = document.getElementById("<%= txtRoyaltor.ClientID %>");

            if (txtRoyaltor.value != '') {
                if (isGridPopulated == "Y") {
                    var gridView = document.getElementById("<%=gvArtistLinks.ClientID %>");
                    var checkBoxes = gridView.getElementsByTagName("input");
                    for (var i = 0; i < checkBoxes.length; i++) {
                        if (checkBoxes[i].type == "checkbox" && checkBoxes[i].checked) {
                            checkcount++;
                        }
                    }
                    if (checkcount > 0) {
                        var popup = $find('<%= mpeConfirmation.ClientID %>');
                        if (popup != null) {
                            popup.show();
                            $get("<%=btnDiscard.ClientID%>").focus();
                            document.getElementById("<%=lblConfirmMsg.ClientID %>").innerText = WarnOnUnSavedData;
                        }
                        return false;
                    }
                    else {
                        return true;
                    }
                }
                else {
                    return true;
                }
            }
            else {
                return true;
            }
        }

        function ConfirmOptionSelection() {
            var checkcount = 0;
            var isGridDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;

            var txtRoyaltor = document.getElementById("<%= txtRoyaltor.ClientID %>");

            if (txtRoyaltor.value != '') {

                var ddlOptionPeriod = document.getElementById("<%= ddlOptionPeriod.ClientID %>");
                var selectedText = ddlOptionPeriod.options[ddlOptionPeriod.selectedIndex].innerHTML;

                if (selectedText != "-") {

                    var isGridPopulated = document.getElementById("<%=isGridPopulated.ClientID %>").value;

                    if (isGridPopulated == "Y") {
                        var gridView = document.getElementById("<%=gvArtistLinks.ClientID %>");
                        var checkBoxes = gridView.getElementsByTagName("input");
                        for (var i = 0; i < checkBoxes.length; i++) {
                            if (checkBoxes[i].type == "checkbox" && checkBoxes[i].checked) {
                                checkcount++;
                            }
                        }

                        if (checkcount > 0) {
                            var popup = $find('<%= mpeConfirmation.ClientID %>');
                            if (popup != null) {
                                popup.show();
                                $get("<%=btnDiscard.ClientID%>").focus();
                                document.getElementById("<%=lblConfirmMsg.ClientID %>").innerText = WarnOnUnSavedData;
                            }
                            return false;
                        }
                        else {
                            return true;
                        }
                    }
                    else {
                        return true;
                    }
                }
                else {
                    return true;
                }
            }
            else {
                return true;
            }
        }

        function ConfirmArtistSearch() {
            var checkcount = 0;
            var isGridDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;

            var isGridPopulated = document.getElementById("<%=isGridPopulated.ClientID %>").value;

            if (isGridPopulated == "Y") {
                var gridView = document.getElementById("<%=gvArtistLinks.ClientID %>");
                var checkBoxes = gridView.getElementsByTagName("input");
                for (var i = 0; i < checkBoxes.length; i++) {
                    if (checkBoxes[i].type == "checkbox" && checkBoxes[i].checked) {
                        checkcount++;
                    }
                }

                if (checkcount > 0) {
                    var popup = $find('<%= mpeConfirmation.ClientID %>');
                    if (popup != null) {
                        popup.show();
                        $get("<%=btnDiscard.ClientID%>").focus();
                        document.getElementById("<%=lblConfirmMsg.ClientID %>").innerText = WarnOnUnSavedData;
                    }
                    return false;
                }
                else {
                    return true;
                }
            }
            else {
                return true;
            }
        }
        //End - Validation: warning message if changes made and not saved or on page change       

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
                    <td colspan="6">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    ARTIST ROYALTOR OPTION LINKS
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="7%" class="identifierLable_large_bold">Royaltor</td>
                    <td width="20%">
                        <asp:TextBox ID="txtRoyaltor" runat="server" Width="98.6%" CssClass="identifierLable" OnFocus="return ConfirmSearch();"
                            OnTextChanged="txtRoyaltor_TextChanged" AutoPostBack="true" TabIndex="100"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="royaltorFilterExtender" runat="server"
                            ServiceMethod="FuzzySearchAllRoyaltorList"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtRoyaltor"
                            FirstRowSelected="true"
                            OnClientPopulating="royaltorPopulating"
                            OnClientPopulated="royaltorPopulated"
                            OnClientHidden="royaltorPopulated"
                            OnClientItemSelected="royaltorListItemSelected"
                            CompletionListElementID="autocompleteDropDownPanel1" />
                        <asp:Panel ID="autocompleteDropDownPanel1" runat="server" CssClass="identifierLable" />
                    </td>
                    <td>
                        <asp:ImageButton ID="fuzzySearchRoyaltor" runat="server" CssClass="FuzzySearch_Button" ImageUrl="../Images/search.png" OnClick="fuzzySearchRoyaltor_Click" Style="cursor: pointer" ToolTip="Search Royaltor" />
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="7%" class="identifierLable_large_bold">Option Period</td>
                    <td width="20%">
                        <asp:DropDownList ID="ddlOptionPeriod" runat="server" Width="99.85%" CssClass="ddlStyle" AutoPostBack="true" OnFocus="return ConfirmOptionSelection();"
                            OnSelectedIndexChanged="ddlOptionPeriod_SelectedIndexChanged" Style="z-index: 9;" TabIndex="101">
                        </asp:DropDownList>
                    </td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="6">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="7%" class="identifierLable_large_bold">Artist</td>
                    <td width="20%">
                        <asp:TextBox ID="txtArtist" runat="server" Width="98.6%" CssClass="identifierLable" OnFocus="return ConfirmArtistSearch();"
                            OnTextChanged="txtArtist_TextChanged" AutoPostBack="true" TabIndex="102"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server"
                            ServiceMethod="FuzzySearchAllArtisList"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtArtist"
                            FirstRowSelected="true"
                            OnClientPopulating="artistPopulating"
                            OnClientPopulated="artistPopulated"
                            OnClientHidden="artistPopulated"
                            OnClientItemSelected="artistListItemSelected"
                            CompletionListElementID="autocompleteDropDownPanel2" />
                        <asp:Panel ID="autocompleteDropDownPanel2" runat="server" CssClass="identifierLable" />
                    </td>
                    <td width="3%">
                        <asp:ImageButton ID="fuzzySearchArtist" runat="server" CssClass="FuzzySearch_Button" ImageUrl="../Images/search.png" OnClick="fuzzySearchArtist_Click" Style="cursor: pointer" ToolTip="Search Artist" />
                    </td>
                    <td width="5%" align="center">
                        <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="ButtonStyle" OnClick="btnClear_Click"
                            UseSubmitBehavior="false" TabIndex="104" onkeydown="OnTabPress();" /></td>
                    <td align="left">
                        <asp:Button ID="btnSaveChanges" runat="server" Text="Save Changes" CssClass="ButtonStyle" OnClick="btnSaveChanges_Click"
                            UseSubmitBehavior="false" TabIndex="103" /></td>
                </tr>
                <tr>
                    <td colspan="6">
                        <table width="100%" cellpadding="0" cellspacing="0">
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="6">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:Panel ID="PnlArtistLinks" runat="server" ScrollBars="Auto" Width="100%">
                                        <asp:GridView ID="gvArtistLinks" runat="server" AutoGenerateColumns="False" Width="98%" AlternatingRowStyle-BackColor="#E3EFFF"
                                            CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                            EmptyDataText="No data found" OnRowDataBound="gvArtistLinks_RowDataBound" AllowSorting="true" OnSorting="gvArtistLinks_Sorting" HeaderStyle-CssClass="FixedHeader">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Artist Id" SortExpression="artist_id">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblArtistId" runat="server" Text='<%# Bind("artist_id") %>' CssClass="identifierLable"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="10%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Artist Name" SortExpression="art_name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblArtistName" runat="server" Text='<%# Bind("art_name") %>' CssClass="identifierLable"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="20%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Royaltor Id" SortExpression="royaltor_id">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRoyaltorId" runat="server" Text='<%# Bind("royaltor_id") %>' CssClass="identifierLable"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="10%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Royaltor Name" SortExpression="royaltor_name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRoyaltorName" runat="server" Text='<%# Bind("royaltor_name") %>' CssClass="identifierLable"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="20%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Option Code" SortExpression="option_period_code">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOptionCode" runat="server" Text='<%# Bind("option_period_code") %>' CssClass="identifierLable"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="7%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Option Desc" SortExpression="opd_description">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOptionDesc" runat="server" Text='<%# Bind("opd_description") %>' CssClass="identifierLable"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="18%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Add">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cbAddLinks" runat="server" CssClass="identifierLable" onclick="javascript: OnGridDataChange();" />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="5%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Remove">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cbRemoveLinks" runat="server" CssClass="identifierLable" onclick="javascript: OnGridDataChange();" />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="5%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Replace">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cbReplaceLinks" runat="server" CssClass="identifierLable" onclick="javascript: OnGridDataChange();" />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="5%" />
                                                </asp:TemplateField>
                                            </Columns>
                                            <EmptyDataRowStyle CssClass="gridEmptyDataRowStyle" />
                                        </asp:GridView>
                                    </asp:Panel>
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

            <%--Warning popup--%>
            <asp:Button ID="dummyConfirm" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmation" runat="server" PopupControlID="pnlPopup" TargetControlID="dummyConfirm"
                CancelControlID="btnCancel" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlPopup" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
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
                                        <asp:Button ID="btnSaveChangedData" runat="server" Text="Save" CssClass="ButtonStyle"
                                            OnClick="btnSaveChangedData_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnDiscard" runat="server" Text="Discard" CssClass="ButtonStyle" OnClick="btnDiscard_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="ButtonStyle" />
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

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnChangeNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="isGridPopulated" runat="server" Value="N" />
            <asp:HiddenField ID="hdnSearchListItemSelected" runat="server" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" Value="" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

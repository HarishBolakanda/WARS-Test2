<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoyaltorSearch.aspx.cs" Inherits="WARS.RoyaltorSearch" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Royaltor Search" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

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

        }

        function EndRequestHandler(sender, args) {
            //Hide the modal popup - the update progress
            var popup = $find('<%= mPopupPageLevel.ClientID %>');
            if (popup != null) {
                popup.hide();
            }

        }
        //======================= End

        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.6;
            document.getElementById("<%=PnlGrid.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
        }

        function OntxtOwnerKeyDown() {
            var txtOwner = document.getElementById("<%= txtOwner.ClientID %>").value;
            if ((event.keyCode == 13)) {
                if (txtOwner == "") {
                    document.getElementById('<%=btnSearch.ClientID%>').click();
                }
                else {
                    if (document.getElementById("<%= hdnIsValidOwner.ClientID %>").value == "Y") {
                        document.getElementById('<%=btnSearch.ClientID%>').click();
                    }
                    else {
                        return false;
                    }

                }

            }

        }

        //Owner auto populate search functionalities        
        var txtOwnSrch;

        function ownerListPopulating() {
            txtOwnSrch = document.getElementById("<%= txtOwner.ClientID %>");
            txtOwnSrch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtOwnSrch.style.backgroundRepeat = 'no-repeat';
            txtOwnSrch.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidOwner.ClientID %>").value = "N";
        }

        function ownerListPopulated() {
            txtOwnSrch = document.getElementById("<%= txtOwner.ClientID %>");
            txtOwnSrch.style.backgroundImage = 'none';
        }

        function ownerScrollPosition(sender, args) {
            var autoCompPnl = document.getElementById("<%= acePanelOwner.ClientID %>");
            autoCompPnl.scrollTop = 1;

        }

        function ownerListItemSelected(sender, args) {
            var ownSrchVal = args.get_value();
            if (ownSrchVal == 'No results found') {
                document.getElementById("<%= txtOwner.ClientID %>").value = "";
            }

            document.getElementById("<%= hdnIsValidOwner.ClientID %>").value = "Y";
        }


        //================================End

        //For handeling select all checkboxes fuctionality
        function GridSelectAll() {
            var checkcount = 0;
            var gridView = document.getElementById("<%=gvRoyaltors.ClientID %>");
            var checkBoxes = gridView.getElementsByTagName("input");
            if (document.getElementById("ContentPlaceHolderBody_gvRoyaltors_cbRoySelectAll").checked == true) {
                for (var i = 1; i < checkBoxes.length; i++) {
                    if (checkBoxes[i].type == "checkbox") {
                        checkBoxes[i].checked = true;
                    }
                }
            }
            else {
                for (var i = 0; i < checkBoxes.length; i++) {
                    if (checkBoxes[i].type == "checkbox") {
                        checkBoxes[i].checked = false;
                    }
                }
            }

        }

        function SetHdnValue() {
            document.getElementById("<%= hdnButtonText.ClientID %>").value = "";
        }
        function ToggleSelectAllout() {
            var checkcount = 0;
            var gridView = document.getElementById("<%=gvRoyaltors.ClientID %>");
            var checkBoxes = gridView.getElementsByTagName("input");
            for (var i = 1; i < checkBoxes.length; i++) {
                if (checkBoxes[i].type == "checkbox" && checkBoxes[i].checked) {
                    checkcount++;
                }
            }
            var rowscount = gridView.rows.length - 1;
            if (rowscount == checkcount) {
                document.getElementById("ContentPlaceHolderBody_gvRoyaltors_cbRoySelectAll").checked = true;
            }
            else {
                document.getElementById("ContentPlaceHolderBody_gvRoyaltors_cbRoySelectAll").checked = false;
            }

        }

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        function FocusLblKeyPress() {
            document.getElementById("<%= txtRoyaltor.ClientID %>").focus();
        }

        //=============== End

        //=========Search by Enter key - Starts

        function SearchByEnterKey() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnSearch.ClientID%>').click();
            }
        }

        //=========Search by Enter key - Ends

        function OpenUploadRoyListPopUp() {
            var popup = $find('<%= mpeUploadRoyList.ClientID %>');
            if (popup != null) {
                popup.show();

                if (document.getElementById('<%=lblUploadRoyListError.ClientID%>') != null) {
                    document.getElementById('<%=lblUploadRoyListError.ClientID%>').style.display = 'none';
                }

                document.getElementById('<%=txtUploadRoyList.ClientID%>').value = "";
                document.getElementById('<%=txtUploadRoyList.ClientID%>').focus();
            }
            return false;
        }


    </script>
    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="4">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    ROYALTOR SEARCH
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="4"></td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td valign="top">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td valign="top">
                                    <table width="99%" align="left">
                                        <tr>
                                            <td width="8%" class="identifierLable_large_bold">Royaltor</td>
                                            <td width="22%">
                                                <asp:TextBox ID="txtRoyaltor" runat="server" Width="98%" CssClass="textboxStyle" onkeydown="SearchByEnterKey();"
                                                    TabIndex="100"></asp:TextBox>
                                            </td>
                                            <td width="2%"></td>
                                            <td width="3%"></td>
                                            <td width="20%">
                                                <asp:TextBox ID="txtPlgRoyaltor" runat="server" Width="98%" CssClass="textboxStyle" onkeydown="SearchByEnterKey();"
                                                    TabIndex="101"></asp:TextBox>
                                                <ajaxToolkit:TextBoxWatermarkExtender ID="wmeTxtPlgRoyaltor" runat="server" TargetControlID="txtPlgRoyaltor"
                                                    WatermarkText="PLG Royaltor Search" WatermarkCssClass="waterMarkText">
                                                </ajaxToolkit:TextBoxWatermarkExtender>
                                            </td>
                                            <td width="3%"></td>
                                            <td width="12%" class="identifierLable_large_bold">Select by Company</td>

                                            <td width="5%" align="right">
                                                <asp:CheckBox ID="cbCompanySelected" runat="server" AutoPostBack="true" OnCheckedChanged="cbCompanySelected_CheckedChanged" TabIndex="102" />
                                            </td>
                                            <td width="3%"></td>
                                            <td width="10%" class="identifierLable_large_bold">Royaltor Held</td>

                                            <td width="10%" align="right">
                                                <asp:CheckBox ID="cbRoyaltorHeld" runat="server" AutoPostBack="true" Width="98%" OnCheckedChanged="cbRoyaltorHeld_CheckedChanged" TabIndex="102" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="identifierLable_large_bold">Owner</td>
                                            <td>
                                                <asp:TextBox ID="txtOwner" runat="server" Width="99%" CssClass="textboxStyle"
                                                    onkeydown="OntxtOwnerKeyDown();" TabIndex="103"></asp:TextBox>
                                                <ajaxToolkit:AutoCompleteExtender ID="ownerFilterExtender" runat="server"
                                                    ServiceMethod="FuzzySearchAllOwnerList"
                                                    ServicePath="~/Services/FuzzySearch.asmx"
                                                    MinimumPrefixLength="1"
                                                    CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                    TargetControlID="txtOwner"
                                                    FirstRowSelected="true"
                                                    OnClientPopulating="ownerListPopulating"
                                                    OnClientPopulated="ownerListPopulated"
                                                    OnClientHidden="ownerListPopulated"
                                                    OnClientShown="ownerScrollPosition"
                                                    OnClientItemSelected="ownerListItemSelected"
                                                    CompletionListElementID="acePanelOwner" />
                                                <asp:Panel ID="acePanelOwner" runat="server" CssClass="identifierLable" />
                                            </td>
                                            <td align="left">
                                                <asp:ImageButton ID="fuzzySearchOwner" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button"
                                                    OnClick="fuzzySearchOwner_Click" ToolTip="Search owner code/name" TabIndex="104" />
                                            </td>
                                            <td></td>
                                            <td>
                                                <table width="100%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td class="identifierLable_large_bold">Responsibility</td>
                                                        <td width="5%"></td>
                                                        <td width="65%" style="text-align: center">
                                                            <asp:DropDownList ID="ddlResponsibility" runat="server" Width="98%" CssClass="ddlStyle" TabIndex="105" onkeydown="SearchByEnterKey();">
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>

                                            </td>
                                            <td></td>
                                            <td colspan="2">
                                                <table width="100%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="30%" class="identifierLable_large_bold">Status</td>
                                                        <td width="70%">
                                                            <asp:DropDownList ID="ddlStatus" runat="server" Width="98%" CssClass="ddlStyle" TabIndex="106" onkeydown="SearchByEnterKey();">
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>

                                            </td>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Contract Type</td>
                                            <td>
                                                <asp:DropDownList ID="ddlContractType" runat="server" Width="98%" CssClass="ddlStyle" TabIndex="106" onkeydown="SearchByEnterKey();">
                                                </asp:DropDownList>
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
                            <tr>
                                <td>
                                    <table width="99%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvRoyaltors" runat="server" AutoGenerateColumns="False" Width="98.4%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle_hover" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found" OnRowDataBound="gvRoyaltors_RowDataBound" RowStyle-CssClass="dataRow"
                                                        OnRowCommand="gvRoyaltors_RowCommand" AllowPaging="true" OnPageIndexChanging="gvRoyaltors_PageIndexChanging" PageSize="25" AllowSorting="true" OnSorting="gvRoyaltors_Sorting" HeaderStyle-CssClass="FixedHeader">
                                                        <PagerStyle HorizontalAlign="Center" CssClass="pagerStyle" />
                                                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Royaltor" SortExpression="royaltor_id"
                                                                ItemStyle-Width="10%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRoyaltorId" runat="server" Text='<%# Bind("royaltor_id") %>' CssClass="identifierLable"></asp:Label>
                                                                    <asp:HiddenField ID="hdnRoyaltorId" runat="server" Value='<%# Bind("royaltor_id") %>'></asp:HiddenField>
                                                                    <asp:HiddenField ID="hdnRoyaltorLocked" runat="server" Value='<%# Bind("royaltor_locked") %>'></asp:HiddenField>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Name" SortExpression="royaltor_name"
                                                                ItemStyle-Width="18%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRoyaltorName" runat="server" Text='<%# Bind("royaltor_name") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Owner" SortExpression="owner_name"
                                                                ItemStyle-Width="18%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblOwner" runat="server" Text='<%# Bind("owner_name") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Contract Type" SortExpression="contract_type"
                                                                ItemStyle-Width="10%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblC" runat="server" Text='<%# Bind("contract_type") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="PLG Royaltor #" SortExpression="royaltor_plg_id"
                                                                ItemStyle-Width="10%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblPlgRoyaltor" runat="server" Text='<%# Bind("royaltor_plg_id") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Responsibility" SortExpression="responsibility_desc"
                                                                ItemStyle-Width="13%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblResponsibility" runat="server" Text='<%# Bind("responsibility_desc") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Status" SortExpression="status_desc"
                                                                ItemStyle-Width="13%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("status_desc") %>' CssClass="identifierLable"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"
                                                                ItemStyle-Width="4%" HeaderStyle-Width="4%">
                                                                <ItemTemplate>
                                                                    <asp:Image ID="imgLock" runat="server" ImageUrl="../Images/Lock.png" ToolTip="Locked" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row"
                                                                ItemStyle-Width="4%" HeaderStyle-Width="4%">
                                                                <HeaderTemplate>
                                                                    <asp:CheckBox ID="cbRoySelectAll" runat="server" OnClick="return GridSelectAll();" />
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="cbRoyChecked" runat="server" OnClick="return ToggleSelectAllout();" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="lnkBtnDblClk" runat="server" CommandName="dblClk" Text="dblClick">                                                                    
                                                                    </asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="hide" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td width="5%"></td>
                    <td width="20%" valign="top" align="right">
                        <table width="90%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="ButtonStyle" TabIndex="107"
                                        UseSubmitBehavior="false" Width="50%" OnClick="btnSearch_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-top: 4px">
                                    <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="ButtonStyle"
                                        UseSubmitBehavior="false" Width="50%" OnClick="btnReset_Click" TabIndex="108" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-top: 4px">
                                    <asp:Button ID="btnAddRoyaltor" runat="server" CssClass="ButtonStyle" OnClick="btnAddRoyaltor_Click"
                                        Text="Add Royaltor" UseSubmitBehavior="false" Width="50%" TabIndex="109" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-top: 4px">
                                    <asp:Button ID="btnLockContracts" runat="server" CssClass="ButtonStyle" OnClick="btnLockContracts_Click"
                                        Text="Lock Contracts" UseSubmitBehavior="false" Width="50%" TabIndex="110" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-top: 4px">
                                    <asp:Button ID="btnUnLockContracts" runat="server" CssClass="ButtonStyle" OnClick="btnUnLockContracts_Click"
                                        Text="Unlock Contracts" UseSubmitBehavior="false" Width="50%" TabIndex="111" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-top: 4px">
                                    <asp:Button ID="btnLockAllContracts" runat="server" CssClass="ButtonStyle" OnClick="btnLockAllContracts_Click"
                                        Text="Lock All Contracts" UseSubmitBehavior="false" Width="50%" TabIndex="112" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-top: 4px">
                                    <asp:Button ID="btnUnlockAllContracts" runat="server" CssClass="ButtonStyle" OnClick="btnUnlockAllContracts_Click"
                                        Text="Unlock All Contracts" UseSubmitBehavior="false" Width="50%" TabIndex="113" onkeydown="OnTabPress();" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-top: 4px">
                                    <asp:Button ID="btnUpdateStatus" runat="server" CssClass="ButtonStyle"
                                        Text="Update Status" UseSubmitBehavior="false" Width="50%" TabIndex="114" onkeydown="OnTabPress();" OnClick="btnUpdateStatus_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-top: 4px">
                                    <asp:Button ID="btnUploadRoyList" runat="server" CssClass="ButtonStyle"
                                        Text="Upload Royaltor List" UseSubmitBehavior="false" Width="50%" TabIndex="115" onkeydown="OnTabPress();" OnClientClick="return OpenUploadRoyListPopUp();" />
                                </td>
                            </tr>
                        </table>
                    </td>
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

            <asp:Button ID="dummyConfirm" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirm" runat="server" PopupControlID="pnlPopup" TargetControlID="dummyConfirm"
                CancelControlID="btnNo" BackgroundCssClass="popupBox">
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
                                        <asp:Button ID="btnYes" runat="server" Text="Yes" CssClass="ButtonStyle" OnClick="btnYes_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNo" runat="server" Text="No" OnClientClick="SetHdnValue();" CssClass="ButtonStyle" />
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
                                        <asp:ImageButton ID="btnCloseFuzzySearchPopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
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

            <asp:Button ID="dummyUpdateStatus" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeUpdateStatus" runat="server" PopupControlID="pnlUpdateStatus" TargetControlID="dummyUpdateStatus"
                CancelControlID="btnCancel" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlUpdateStatus" runat="server" align="center" Width="30%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr align="justify" id="screen">
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblUpdateStatus" runat="server" Width="100%" Text="Update Royaltor Status" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="50%" id="status">
                                <tr align="center">
                                    <td width="30%" class="identifierLable_large_bold">New Status</td>
                                    <td width="10%"></td>
                                    <td width="50%" align="right">
                                        <asp:DropDownList ID="ddlUpdateStatus" runat="server" CssClass="ddlStyle" />
                                    </td>
                                    <td width="10%" align="left">
                                        <asp:RequiredFieldValidator runat="server" ID="rfvddlUpdateStatus" ControlToValidate="ddlUpdateStatus"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please select status" InitialValue="-" Display="Dynamic"
                                            ValidationGroup="valGrpUpdateStatus"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr align="justify">
                        <td>
                            <asp:Label ID="Label2" Text="Update Status of selected Royatlors" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr align="justify">
                        <td>
                            <table id="button">
                                <tr>
                                    <td>
                                        <asp:Button ID="btnUpdate" OnClick="bthUpdateRoyStatusPopup_Click" UseSubmitBehavior="false" runat="server" Text="Update" CssClass="ButtonStyle"
                                            ValidationGroup="valGrpUpdateStatus" />
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

            <asp:Button ID="dummyMpeUploadRoyList" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeUploadRoyList" runat="server" PopupControlID="pnlMpeUploadRoyList" TargetControlID="dummyMpeUploadRoyList"
                CancelControlID="btnCloseUploadRoyListPopup" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlMpeUploadRoyList" runat="server" align="center" Width="30%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="identifierLable">Upload Royaltor List</td>
                                    <td align="right" style="vertical-align: top;" width="5%">
                                        <asp:ImageButton ID="btnCloseUploadRoyListPopup" runat="server" ImageUrl="../Images/CloseIcon.png" Style="cursor: pointer" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table width="90%">
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtUploadRoyList" runat="server" TextMode="MultiLine" Width="99%" Height="100px" class="identifierLable" Style="word-break: break-all"></asp:TextBox>
                                    </td>
                                    <td width="5%">
                                        <asp:RequiredFieldValidator runat="server" ID="rvTxtUploadRoyList" ControlToValidate="txtUploadRoyList" ValidationGroup="valGrpUploadRoyList"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter semicolon(;) separated Royaltors" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                            </table>

                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Label ID="lblUploadRoyListError" Text="Invalid Royaltors entered" runat="server" CssClass="errorMessage" Visible="false"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Label ID="lblUploadRoyList" Text="Upload semicolon(;) separated Royaltors" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding-bottom: 5px">
                            <asp:Button ID="btnSearchBulkUpload" OnClick="btnSearchBulkUpload_Click" UseSubmitBehavior="false" runat="server" Text="Search" CssClass="ButtonStyle"
                                ValidationGroup="valGrpUploadRoyList" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnLockUnlock" runat="server" />
            <asp:HiddenField ID="hdnButtonText" runat="server" />
            <asp:HiddenField ID="hdnIsNewRequest" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnIsValidOwner" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnUpdateStatus" runat="server" />
            <%--<asp:Button ID="btnRoyaltorSearch" runat="server" Style="display: none;" OnClick="btnRoyaltorSearch_Click" CausesValidation="false" />
            <asp:Button ID="btnPlgRoyaltorSearch" runat="server" Style="display: none;" OnClick="btnPlgRoyaltorSearch_Click" CausesValidation="false" />
            <asp:Button ID="btnOwnerSearch" runat="server" Style="display: none;" OnClick="btnOwnerSearch_Click" CausesValidation="false" />--%>
            <%--<asp:Button ID="btnSearch" runat="server" Style="display: none;" OnClick="btnSearch_Click" CausesValidation="false" />--%>
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField" onkeydown="FocusLblKeyPress();"></asp:TextBox>
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

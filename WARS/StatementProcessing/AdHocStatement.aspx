<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdHocStatement.aspx.cs" Inherits="WARS.AdHocStatement" MasterPageFile="~/MasterPage.Master"
    Title="WARS - AdHocStatement" MaintainScrollPositionOnPostback="true" %>

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

        //probress bar and scroll position functionality - starts

        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.4;
            document.getElementById("<%=PnlGrid.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        //grid panel height adjustment functioanlity - ends


        //window.onresize = function SetGrdPnlHeightOnResize() {
        // var windowHeight = window.screen.availHeight;
        // var gridPanelHeight = windowHeight * 0.4;
        // document.getElementById("<%=PnlGrid.ClientID %>").style.height = gridPanelHeight + "px";

        // }

       

        //Royaltor auto populate search functionalities        
        var txtRoySearch;

        function royaltorListPopulating() {
            txtRoySearch = document.getElementById("<%= txtRoySearch.ClientID %>");
            txtRoySearch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoySearch.style.backgroundRepeat = 'no-repeat';
            txtRoySearch.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "N";
        }

        function royaltorListPopulated() {
            txtRoySearch = document.getElementById("<%= txtRoySearch.ClientID %>");
            txtRoySearch.style.backgroundImage = 'none';
        }

        function royaltorListItemSelected(sender, args) {
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                document.getElementById("<%= txtRoySearch.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "Y";
            }

        }

        function resetScrollPosition(sender, args) {
            var autoCompPnl = document.getElementById("<%= autocompleteDropDownPanel1.ClientID %>");
            autoCompPnl.scrollTop = 1;

        }


        //================================End

        //Owner auto populate search functionalities        
        var txtOwn;

        function ownerListPopulating() {
            txtOwn = document.getElementById("<%= txtOwnSearch.ClientID %>");
            txtOwn.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtOwn.style.backgroundRepeat = 'no-repeat';
            txtOwn.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "N";
        }

        function ownerListPopulated() {
            txtOwn = document.getElementById("<%= txtOwnSearch.ClientID %>");
            txtOwn.style.backgroundImage = 'none';
        }

        function ownerListItemSelected(sender, args) {
            var ownSrchVal = args.get_value();
            if (ownSrchVal == 'No results found') {
                document.getElementById("<%= txtOwnSearch.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSearchListItemSelected.ClientID %>").value = "Y";
            }

        }

        function ownerScrollPosition(sender, args) {
            var autoCompPnl = document.getElementById("<%= acePanelOwner.ClientID %>");
            autoCompPnl.scrollTop = 1;

        }
        //================================End

        //Validation        
        function ValidateStmtDesc(sender, args) //Statement description
        {
            var txtStmtDescStartDateVal = document.getElementById("<%= txtStmtDescStartDate.ClientID %>").value;
            var txtStmtDescEndDateVal = document.getElementById("<%= txtStmtDescEndDate.ClientID %>").value;

            if (txtStmtDescStartDateVal != "" && txtStmtDescEndDateVal != "" && txtStmtDescStartDateVal != "__/____" && txtStmtDescEndDateVal != "__/____") {

                var splitValues = txtStmtDescStartDateVal.split("/");
                var frmDateYear = splitValues[1].trim();
                var frmDateMonth = splitValues[0].trim();
                var splitValues = txtStmtDescEndDateVal.split("/");
                var toDateYear = splitValues[1].trim();
                var toDateMonth = splitValues[0].trim();

                //validate - month and year are valid
                if (!(frmDateMonth > 0 && frmDateMonth < 13)) {
                    sender.innerText = "Not a valid start date!";
                    args.IsValid = false;
                }
                else if (!(toDateMonth > 0 && toDateMonth < 13)) {
                    sender.innerText = "Not a valid end date!";
                    args.IsValid = false;
                }//validate - from_date should be earlier than the to_date
                else if ((frmDateYear > toDateYear) || (frmDateYear = toDateYear && frmDateMonth > toDateMonth)) {
                    sender.innerText = "Start date should be earlier than the end date!";
                    args.IsValid = false;
                }
                else {
                    args.IsValid = true;
                }

            }
            else {
                args.IsValid = false;
            }


        }

        function ValidateStmtPriod(sender, args) //Statement period
        {
            var txtStmtDateStartDateVal = document.getElementById("<%= txtStmtDateStartDate.ClientID %>").value;
            var txtStmtDateEndDateVal = document.getElementById("<%= txtStmtDateEndDate.ClientID %>").value;
            if ((txtStmtDateStartDateVal != "" && txtStmtDateEndDateVal != "") && (txtStmtDateStartDateVal != "__/____" && txtStmtDateEndDateVal != "__/____")) {

                var splitValues = txtStmtDateStartDateVal.split("/");
                var frmDateYear = splitValues[1].trim();
                var frmDateMonth = splitValues[0].trim();
                var splitValues = txtStmtDateEndDateVal.split("/");
                var toDateYear = splitValues[1].trim();
                var toDateMonth = splitValues[0].trim();

                //validate - month and year are valid
                if (!(frmDateMonth > 0 && frmDateMonth < 13)) {
                    sender.innerText = "Not a valid start date!";
                    args.IsValid = false;
                }
                else if (!(toDateMonth > 0 && toDateMonth < 13)) {
                    sender.innerText = "Not a valid end date!";
                    args.IsValid = false;
                }//validate - from_date should be earlier than the to_date
                else if ((frmDateYear > toDateYear) || (frmDateYear = toDateYear && frmDateMonth > toDateMonth)) {
                    sender.innerText = "Start date should be earlier than the end date!";
                    args.IsValid = false;
                }
                else {
                    args.IsValid = true;
                }
            }
            else {
                args.IsValid = false;
            }

        }

        //======================== End

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        //=============== End

        //warning message if changes made and not saved and on page leave                                
        function WarnOnUnSavedData() {
            //debugger;
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            if (isExceptionRaised != "Y") {
                if (DataChanged()) {
                    return warningMsgOnUnSavedData;
                }
            }
        }
        window.onbeforeunload = WarnOnUnSavedData;

        function DataChanged() {
            var rowscount = document.getElementById("<%= gvStatements.ClientID %>").rows.length;
            var emptyText = document.getElementById("<%= gvStatements.ClientID %>").rows[1].cells[0].innerHTML;
            if (emptyText != "No Data Found") {
                return true;
            }
            else {
                var txtStmtDescStartDate = document.getElementById("<%= txtStmtDescStartDate.ClientID %>").value;
                var txtStmtDescEndDate = document.getElementById("<%= txtStmtDescEndDate.ClientID %>").value;
                var txtStmtDateStartDate = document.getElementById("<%= txtStmtDateStartDate.ClientID %>").value;
                var txtStmtDateEndDate = document.getElementById("<%= txtStmtDateEndDate.ClientID %>").value;
                var txtPaymentDate = document.getElementById("<%= txtPaymentDate.ClientID %>").value;
                var txtRoySearch = document.getElementById("<%= txtRoySearch.ClientID %>").value;
                var txtOwnSearch = document.getElementById("<%= txtOwnSearch.ClientID %>").value;
                if (((txtStmtDescStartDate != "__/____" && txtStmtDescStartDate != "") || (txtStmtDescEndDate != "__/____" && txtStmtDescEndDate != "") ||
                              (txtStmtDateStartDate != "__/____" && txtStmtDateStartDate != "") || (txtStmtDateEndDate != "__/____" && txtStmtDateEndDate != "")
                    || (txtPaymentDate != "__/__/____" && txtPaymentDate != "") || txtRoySearch != "" || txtOwnSearch != "")) {
                    return true;
                }
            }

            return false;
        }

        //Validate any unsaved data on browser window close/refresh
        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        //used to check if any changes to allow navigation to other screen 
        function IsDataChanged() {
            return DataChanged();
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
                                    AD HOC STATEMENTS
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="7"></td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="12%" class="identifierLable_large_bold">Add new Statement Details
                    </td>
                    <td width="2%"></td>
                    <td width="10%" class="identifierLable_large_bold">Statement Description
                    </td>
                    <td width="5%" class="identifierLable">
                        <asp:TextBox ID="txtStmtDescStartDate" runat="server" Width="60px" CssClass="identifierLable"
                            ToolTip="Start date in MM/YYYY format" TabIndex="100"></asp:TextBox>
                        <ajaxToolkit:MaskedEditExtender ID="maskEditStmtDescStartDate" runat="server"
                            TargetControlID="txtStmtDescStartDate" Mask="99/9999" AcceptNegative="None"
                            ClearMaskOnLostFocus="false" />
                    </td>
                    <td width="5%" class="identifierLable">
                        <asp:TextBox ID="txtStmtDescEndDate" runat="server" Width="60px" CssClass="identifierLable"
                            ToolTip="End date in MM/YYYY format" TabIndex="101"></asp:TextBox>
                        <ajaxToolkit:MaskedEditExtender ID="maskEditStmtPeriodEndDate" runat="server"
                            TargetControlID="txtStmtDescEndDate" Mask="99/9999" AcceptNegative="None"
                            ClearMaskOnLostFocus="false" />

                    </td>
                    <td>
                        <%--<table width="100%">
                            <tr>
                                <td class="identifierLable" width="4%">(or) </td>
                                <td width="30%">
                                    <asp:TextBox ID="txtStmtDesc" runat="server" Width="98%" CssClass="identifierLable"
                                        MaxLength="30" ToolTip="Free text upto 30 chars" TabIndex="103"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:CustomValidator ID="valStmtDesc" runat="server" ValidationGroup="valSave" CssClass="errorMessage" EnableClientScript="false"
                                        ClientValidationFunction="ValidateStmtDesc" OnServerValidate="valStmtDesc_ServerValidate"
                                        ErrorMessage="Please enter either start & end date or a free text."></asp:CustomValidator>
                                </td>
                            </tr>
                        </table>--%>
                        <asp:CustomValidator ID="valStmtDesc" runat="server" ValidationGroup="valSave" CssClass="errorMessage" EnableClientScript="false"
                            ClientValidationFunction="ValidateStmtDesc" OnServerValidate="valStmtDesc_ServerValidate"
                            ErrorMessage="Please enter both start & end dates."></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td class="identifierLable_large_bold">Statement Period
                    </td>
                    <td class="identifierLable">
                        <asp:TextBox ID="txtStmtDateStartDate" runat="server" Width="60px" CssClass="identifierLable"
                            ValidationGroup="valSave" ToolTip="Start date in MM/YYYY format" TabIndex="104"></asp:TextBox>
                        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender2" runat="server"
                            TargetControlID="txtStmtDateStartDate" Mask="99/9999" AcceptNegative="None"
                            ClearMaskOnLostFocus="false" />
                    </td>
                    <td class="identifierLable">
                        <asp:TextBox ID="txtStmtDateEndDate" runat="server" Width="60px" CssClass="identifierLable"
                            ValidationGroup="valSave" ToolTip="End date in MM/YYYY format" TabIndex="105"></asp:TextBox>
                        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender3" runat="server"
                            TargetControlID="txtStmtDateEndDate" Mask="99/9999" AcceptNegative="None"
                            ClearMaskOnLostFocus="false" />
                    </td>
                    <td>
                        <asp:CustomValidator ID="valStmtPriod" runat="server" ValidationGroup="valSave" CssClass="errorMessage" EnableClientScript="false"
                            ClientValidationFunction="ValidateStmtPriod" OnServerValidate="valStmtPriod_ServerValidate"
                            ErrorMessage="Please enter both start & end dates."></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td class="identifierLable_large_bold">Payment Date
                    </td>
                    <td class="identifierLable">
                        <asp:TextBox ID="txtPaymentDate" runat="server" Width="60px" CssClass="identifierLable"
                            ValidationGroup="valSave" ToolTip="Payment date in DD/MM/YYYY format" TabIndex="105"></asp:TextBox>
                        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender1" runat="server"
                            TargetControlID="txtPaymentDate" Mask="99/99/9999" AcceptNegative="None"
                            ClearMaskOnLostFocus="false" />
                    </td>
                    <td></td>
                    <td>
                        <asp:CustomValidator ID="valPaymentDate" runat="server" ValidationGroup="valSave" CssClass="errorMessage" EnableClientScript="false"
                            OnServerValidate="valPaymentDate_ServerValidate"
                            ErrorMessage="Please enter a valid date in DD/MM/YYYY format."></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td colspan="7"></td>
                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">Add Royaltors for Statement
                    </td>
                    <td></td>

                    <td colspan="4">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td width="25%">
                                    <asp:TextBox ID="txtRoySearch" runat="server" Width="98%" CssClass="identifierLable"
                                        OnTextChanged="txtRoySearch_TextChanged" AutoPostBack="true" TabIndex="106"></asp:TextBox>
                                    <ajaxToolkit:AutoCompleteExtender ID="royaltorFilterExtender" runat="server"
                                        ServiceMethod="FuzzyAdHocStmtRoyaltorList"
                                        ServicePath="~/Services/FuzzySearch.asmx"
                                        MinimumPrefixLength="1"
                                        CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                        TargetControlID="txtRoySearch"
                                        FirstRowSelected="true"
                                        OnClientPopulating="royaltorListPopulating"
                                        OnClientPopulated="royaltorListPopulated"
                                        OnClientHidden="royaltorListPopulated"
                                        OnClientShown="resetScrollPosition"
                                        OnClientItemSelected="royaltorListItemSelected"
                                        CompletionListElementID="autocompleteDropDownPanel1" />
                                    <asp:Panel ID="autocompleteDropDownPanel1" runat="server" CssClass="identifierLable" />
                                </td>
                                <td>
                                    <asp:ImageButton ID="fuzzySearchRoyaltor" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                                        ToolTip="Search Royaltor" OnClick="fuzzySearchRoyaltor_Click" CssClass="FuzzySearch_Button" />
                                </td>
                            </tr>
                        </table>
                    </td>

                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">Add Owners for Statement
                    </td>
                    <td></td>
                    <td colspan="4">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td width="25%">
                                    <asp:TextBox ID="txtOwnSearch" runat="server" Width="98%" CssClass="identifierLable"
                                        OnTextChanged="txtOwnerSearch_TextChanged" AutoPostBack="true" TabIndex="107"></asp:TextBox>
                                    <ajaxToolkit:AutoCompleteExtender ID="ownerFilterExtender" runat="server"
                                        ServiceMethod="FuzzySearchAllOwnerList"
                                        ServicePath="~/Services/FuzzySearch.asmx"
                                        MinimumPrefixLength="1"
                                        CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                        TargetControlID="txtOwnSearch"
                                        FirstRowSelected="true"
                                        OnClientPopulating="ownerListPopulating"
                                        OnClientPopulated="ownerListPopulated"
                                        OnClientHidden="ownerListPopulated"
                                        OnClientShown="ownerScrollPosition"
                                        OnClientItemSelected="ownerListItemSelected"
                                        CompletionListElementID="acePanelOwner" />
                                    <asp:Panel ID="acePanelOwner" runat="server" CssClass="identifierLable" />
                                </td>
                                <td>
                                    <asp:ImageButton ID="fuzzySearchOwner" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                                        ToolTip="Search Owner" OnClick="fuzzySearchOwner_Click" CssClass="FuzzySearch_Button" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="7" align="left">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td align="left">
                                    <table width="68.74%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td colspan="5" align="right">
                                                <table width="30%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="50%" align="left">
                                                            <asp:Button ID="btnRemove" runat="server" Text="Remove" CssClass="ButtonStyle" Width="95%"
                                                                OnClick="btnRemove_Click" UseSubmitBehavior="false" TabIndex="108" />
                                                        </td>
                                                        <td align="right">
                                                            <asp:Button ID="btnSave" runat="server" Text="Process Statement" CssClass="ButtonStyle" Width="95%"
                                                                OnClick="btnSave_Click" UseSubmitBehavior="false" ValidationGroup="valSave" TabIndex="109"
                                                                onkeydown="OnTabPress();" />
                                                        </td>
                                                    </tr>
                                                </table>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="5">
                                                <div style="height: 5px"></div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="70%">
                                        <asp:GridView ID="gvStatements" runat="server" AutoGenerateColumns="False" Width="98.2%" AlternatingRowStyle-BackColor="#E3EFFF"
                                            CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" EmptyDataText="No Data Found" ShowHeaderWhenEmpty="true"
                                            OnRowDataBound="gvStatements_RowDataBound" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle"  AllowSorting="true" OnSorting="gvStatements_Sorting" HeaderStyle-CssClass="FixedHeader">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Owner" SortExpression="OwnerCode">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOwner" runat="server" Text='<%#Bind("OwnerCode")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="15%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Owner Name" SortExpression="OwnerName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOwnerName" runat="server" Text='<%#Bind("OwnerName")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Royaltor" SortExpression="RoyaltorId">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRoyaltor" runat="server" Text='<%#Bind("RoyaltorId")%>' CssClass="identifierLable" />
                                                        <asp:Label ID="lblLelvelFlag" runat="server" Text='<%#Bind("LevelFlag")%>' Visible="false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="15%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Royaltor Name" SortExpression="RoyaltorName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRoyaltorName" runat="server" Text='<%#Bind("RoyaltorName")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cbRemoveFromStmt" runat="server" />
                                                    </ItemTemplate>
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

            <asp:UpdateProgress ID="progressBarPageLevel" runat="server" AssociatedUpdatePanelID="updPnlPageLevel" DisplayAfter="100">
                <ProgressTemplate>
                    <div id="Search" style="font-weight: bold; color: Black">
                        <table>
                            <tr>
                                <td>
                                    <img src="../Images/InProgress2.gif" alt="" />
                                </td>
                            </tr>
                            <tr>
                                <td class="identifierLable">Please Wait...
                                </td>
                            </tr>
                        </table>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <ajaxToolkit:ModalPopupExtender ID="mPopupPageLevel" runat="server" PopupControlID="progressBarPageLevel" TargetControlID="progressBarPageLevel"
                BackgroundCssClass="progressBar" RepositionMode="RepositionOnWindowResize">
            </ajaxToolkit:ModalPopupExtender>

            <asp:Button ID="dummyFuzzySearch" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeFuzzySearch" runat="server" PopupControlID="pnlFuzzySearch" TargetControlID="dummyFuzzySearch"
                CancelControlID="btnCloseFuzzySearchPopup" BackgroundCssClass="popupBox">
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

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
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



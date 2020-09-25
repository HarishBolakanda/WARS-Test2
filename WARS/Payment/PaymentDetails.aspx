<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaymentDetails.aspx.cs" Inherits="WARS.Payment.PaymentDetails" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Payment Details" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        <%--JIRA-908 Changes by Ravi on 13/02/2019 -- STart--%>
        
        function CreateAPInterface() {
            var popup = $find('<%= mpeConfirmAPInterface.ClientID %>');
            if (popup != null) {
                popup.show();
            }
            return false
        }
    <%--JIRA-908 Changes by Ravi on 13/02/2019 -- End--%>
    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 1px;">
                        <asp:Button ID="btnPaymentApproval" runat="server" CssClass="LinkButtonStyle" Width="98%"
                            Text="Payment Approval" UseSubmitBehavior="false" OnClientClick="PaymentApprovalScreen();" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderBody">
    <script type="text/javascript">

        //probress bar and scroll position functionality - starts
        //to remain scroll position of grid panel and window
        var xPos, yPos;
        var scrollTop;
        var gridClientId = "ContentPlaceHolderBody_gvPaymentDetails_";
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
            if (postBackElementID == 'imgExpand' || postBackElementID == 'imgCollapse' || postBackElementID == 'txtSupplier'
                || postBackElementID == 'ddlCompany' || postBackElementID == 'ddlStatus' || postBackElementID == 'fuzzySearchRoyaltor') {
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
            if (postBackElementID == 'imgExpand' || postBackElementID == 'imgCollapse' || postBackElementID == 'txtSupplier'
                || postBackElementID == 'ddlCompany' || postBackElementID == 'ddlStatus' || postBackElementID == 'fuzzySearchRoyaltor') {
                window.scrollTo(xPos, yPos);

                //set scroll position on selecting expand/collapse
                var PnlReference = document.getElementById("<%=PnlGrid.ClientID %>");
                PnlReference.scrollTop = scrollTop;
            }


        }

        //probress bar and scroll position functionality - starts

        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.40;
            document.getElementById("<%=PnlGrid.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
        }


        //Royaltor auto populate search functionalities        
        var txtRoySrch;

        function royaltorListPopulating() {
            txtRoySrch = document.getElementById("<%= txtRoyaltor.ClientID %>");
            txtRoySrch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoySrch.style.backgroundRepeat = 'no-repeat';
            txtRoySrch.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsvalidRoyaltor.ClientID %>").value = "N";
        }

        function royaltorListPopulated() {
            txtRoySrch = document.getElementById("<%= txtRoyaltor.ClientID %>");
            txtRoySrch.style.backgroundImage = 'none';
        }

        function resetScrollPosition(sender, args) {
            var autoCompPnl = document.getElementById("<%= autocompleteDropDownPanel1.ClientID %>");
            autoCompPnl.scrollTop = 1;

        }

        function royaltorListItemSelected(sender, args) {
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                document.getElementById("<%= txtRoyaltor.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnIsvalidRoyaltor.ClientID %>").value = "Y";                
            }
        }


        //================================End



        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }//=============== End


        //Validate any unsaved data on browser window close/refresh
        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        //============== End

        //Enter key Functionality on all fields      
        function SearchByEnterKey(){
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnGo.ClientID%>').click();
            }
        }

        function OntxtSupplierSearchKeyDown() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnSupplierSearch.ClientID%>').click();
            }
        }

        function OntxtRoyaltorKeyDown() {
            var txtRoyaltor = document.getElementById("<%= txtRoyaltor.ClientID %>").value;
            if ((event.keyCode == 13)) {                
                if (txtRoyaltor == "") {
                    document.getElementById('<%=btnGo.ClientID%>').click();
                }
                else {
                    if (document.getElementById("<%= hdnIsvalidRoyaltor.ClientID %>").value == "Y") {
                        document.getElementById('<%=btnGo.ClientID%>').click();
                    }
                    else {
                        return false;
                    }
                }
            }
        }

        //============== End

        //Navigate to payment approval screen
        function PaymentApprovalScreen() {
            window.location = '../Payment/PaymentApproval.aspx?RetainSearch=Y';
        }
        //============== End

    </script>
    <asp:UpdatePanel ID="updPnlPageLevel" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="11"></td>
                </tr>
                <tr>
                    <td colspan="11">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    PAYMENT DETAILS
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="11"></td>
                </tr>
                <tr>
                    <td colspan="11"></td>
                </tr>
                <tr>
                    <td colspan="11"></td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="8%" class="identifierLable_large_bold">Payment No
                    </td>
                    <td width="10%">
                        <asp:TextBox ID="txtPaymentNo" runat="server" Width="98%" CssClass="identifierLable" onkeydown="SearchByEnterKey();"
                            TabIndex="100"></asp:TextBox>
                    </td>
                    <td width="1%"></td>
                    <td width="5%" class="identifierLable_large_bold">Supplier
                    </td>
                    <td width="15%">
                        <asp:TextBox ID="txtSupplier" runat="server" Width="98%" CssClass="identifierLable" onkeydown="OntxtSupplierSearchKeyDown();"
                            TabIndex="101"></asp:TextBox>
                    </td>
                    <td width="3%" align="left">
                        <asp:CustomValidator ID="valSupplier" runat="server" ValidationGroup="valSearch" CssClass="requiredFieldValidator"
                            OnServerValidate="valSupplier_ServerValidate" ToolTip="Not a valid Supplier. Please select from the search list." Display="Dynamic"
                            ErrorMessage="*"></asp:CustomValidator>
                    </td>
                    <td class="identifierLable_large_bold" width="10%">Payment Threshold</td>
                    <td width="10%">
                        <asp:TextBox ID="txtPaymentThreshold" runat="server" Width="98%" CssClass="identifierLable" onkeydown="SearchByEnterKey();"
                            TabIndex="103"></asp:TextBox>
                    </td>
                    <td align="left">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td align="left">
                                    <asp:RegularExpressionValidator ID="revPaymentThreshold" runat="server" Text="*" ControlToValidate="txtPaymentThreshold" ValidationGroup="valSearch"
                                        ValidationExpression="^[-+]?\d*\.{0,1}\d+$" CssClass="requiredFieldValidator" ForeColor="Red"
                                        ToolTip="Please enter only number" Display="Dynamic"> </asp:RegularExpressionValidator>
                                </td>
                                <td align="right">
                                    <asp:Button ID="btnApInterface" runat="server" CssClass="ButtonStyle"
                                        OnClientClick="return CreateAPInterface();" Text="Create AP Interface"
                                        UseSubmitBehavior="false" Width="40%" CausesValidation="false" />
                                </td>
                            </tr>
                        </table>

                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">Payment Date
                    </td>
                    <td>
                        <asp:TextBox ID="txtPaymentDate" runat="server" Width="65px" CssClass="identifierLable" onkeydown="SearchByEnterKey();"
                            TabIndex="104"></asp:TextBox>
                        <ajaxToolkit:MaskedEditExtender ID="mtePaymentDate" runat="server"
                            TargetControlID="txtPaymentDate" Mask="99/99/9999" AcceptNegative="None"
                            ClearMaskOnLostFocus="false" />
                        <asp:CustomValidator ID="valPaymentDate" runat="server" ValidationGroup="valSearch" CssClass="requiredFieldValidator"
                            OnServerValidate="valPaymentDate_ServerValidate" ErrorMessage="*" ToolTip="Please enter a valid date in DD/MM/YYYY format"></asp:CustomValidator>
                    </td>
                    <td></td>
                    <td class="identifierLable_large_bold">Royaltor
                    </td>
                    <td>
                        <asp:TextBox ID="txtRoyaltor" runat="server" Width="98%" CssClass="identifierLable"
                            TabIndex="105" onkeydown="OntxtRoyaltorKeyDown();" ></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server"
                            ServiceMethod="FuzzySearchAllRoyListWithOwnerCode"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtRoyaltor"
                            FirstRowSelected="true"
                            OnClientPopulating="royaltorListPopulating"
                            OnClientPopulated="royaltorListPopulated"
                            OnClientHidden="royaltorListPopulated"
                            OnClientShown="resetScrollPosition"
                            OnClientItemSelected="royaltorListItemSelected"
                            CompletionListElementID="autocompleteDropDownPanel1" />
                        <asp:Panel ID="autocompleteDropDownPanel1" runat="server" CssClass="identifierLable" />
                    </td>
                    <td align="left">
                        <asp:CustomValidator ID="valRoyaltor" runat="server" ValidationGroup="valSearch" CssClass="requiredFieldValidator"
                            OnServerValidate="valRoyaltor_ServerValidate" ToolTip="Not a valid Royaltor. Please select from the search list." Display="Dynamic"
                            ErrorMessage="*"></asp:CustomValidator>
                        <asp:ImageButton ID="fuzzySearchRoyaltor" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                            OnClick="fuzzySearchRoyaltor_Click" ToolTip="Search Royaltor" CssClass="FuzzySearch_Button" />
                    </td>
                    <td class="identifierLable_large_bold">Payment Filename
                    </td>
                    <td>
                        <asp:TextBox ID="txtPaymentFilename" runat="server" Width="98%" CssClass="identifierLable" onkeydown="SearchByEnterKey();"
                            TabIndex="106"></asp:TextBox>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">Payment Status
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="ddlStyle" TabIndex="107" Width="99%" onkeydown="SearchByEnterKey();">
                        </asp:DropDownList>
                    </td>
                    <td></td>
                    <td></td>
                    <td>
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td width="22%">
                                    <asp:Button ID="btnGo" runat="server" CssClass="ButtonStyle" OnClick="btnGo_Click" TabIndex="108" Text="Go" UseSubmitBehavior="false" Width="80%" />
                                </td>
                                <td width="35%">
                                    <asp:Button ID="btnClear" runat="server" CssClass="ButtonStyle" OnClick="btnClear_Click" TabIndex="109" Text="Clear" UseSubmitBehavior="false" Width="70%" onkeydown="OnTabPress();" />
                                </td>
                                <td width="43%"></td>
                            </tr>
                        </table>
                    </td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="11"></td>
                </tr>
                <tr>
                    <td colspan="11"></td>
                </tr>
                <tr>
                    <td colspan="11"></td>
                </tr>
                <tr>
                    <td colspan="11" align="center">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td colspan="9">
                                    <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="100%">
                                        <asp:GridView ID="gvPaymentDetails" runat="server" AutoGenerateColumns="False" Width="98.72%"
                                            CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" ShowHeaderWhenEmpty="true" EmptyDataText="No Data Found"
                                            EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" OnRowDataBound="gvPaymentDetails_RowDataBound" AllowSorting="true" OnSorting="gvPaymentDetails_Sorting" HeaderStyle-CssClass="FixedHeader">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-Width="12%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Royaltor" SortExpression="royaltor_text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRoyaltor" runat="server" Text='<%#Bind("royaltor_text")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="12%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Supplier" SortExpression="supplier_text">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnInvoiceId" runat="server" Value='<%# Bind("invoice_id") %>' />
                                                        <asp:Label ID="lblSupplier" runat="server" Text='<%#Bind("supplier_text")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="6%" ItemStyle-CssClass="gridItemStyle_Right_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Value in GBP" SortExpression="payment_amount">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblValueInGBP" runat="server" Text='<%#Bind("payment_amount")%>' CssClass="identifierLable" Style="text-align: right" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Currency" SortExpression="currency_code">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCurrency" runat="server" Text='<%#Bind("currency_code")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="6%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Exchange Rate" SortExpression="exchange_rate">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblExchRate" runat="server" Text='<%#Bind("exchange_rate")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="6%" ItemStyle-CssClass="gridItemStyle_Right_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Payment Value" SortExpression="payment_value">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPaymentValue" runat="server" Text='<%#Bind("payment_value")%>' CssClass="identifierLable" Style="text-align: right" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="7%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Payment Date" SortExpression="payment_date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPaymentDate" runat="server" Text='<%#Bind("payment_date")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="6%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Payment Number" SortExpression="payment_number">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPaymentNumber" runat="server" Text='<%#Bind("payment_number")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Sent" SortExpression="sent_date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSent" runat="server" Text='<%#Bind("sent_date")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Filename" SortExpression="sent_filename">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFilename" runat="server" Text='<%#Bind("sent_filename")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="8%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Confirmed" SortExpression="paid_date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblConfirmed" runat="server" Text='<%#Bind("paid_date")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="6%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Payment Ref" SortExpression="payment_ref_no">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPaymentRef" runat="server" Text='<%#Bind("payment_ref_no")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="7%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Status" SortExpression="status_desc">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStatus" runat="server" Text='<%#Bind("status_desc")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                    <tr>
                                        <td align="center">
                                            <asp:Repeater ID="Repeater1" runat="server">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>'
                                                        ClientIDMode="AutoID" CausesValidation="false" Enabled='<%# Eval("Enabled") %>' OnClick="lnkPage_Click" CssClass="gridPager"> </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </td>
                                    </tr>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="9">
                                    <asp:Repeater ID="rptPager" runat="server">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>'
                                                Enabled='<%# Eval("Enabled") %>' OnClick="lnkPage_Click" CssClass="gridPager"> </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:Repeater>
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

            <%--Supplier search list popup--%>
            <asp:Button ID="dummySupplierSearch" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeSupplierSearch" runat="server" PopupControlID="pnlSupplierSearchPopup" TargetControlID="dummySupplierSearch"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlSupplierSearchPopup" runat="server" align="left" Width="60%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="identifierLable" align="center">Supplier search List
                                    </td>
                                    <td align="right" style="vertical-align: top;">
                                        <asp:ImageButton ID="btnClosePopupSupplierSearch" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" OnClick="btnClosePopupSupplierSearch_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <table width="97.75%" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td width="10%" class="gridHeaderStyle_1row">Supplier</td>
                                                <td width="15%" class="gridHeaderStyle_1row">Supplier Site Name</td>
                                                <td width="20%" class="gridHeaderStyle_1row">Supplier Name</td>
                                                <td width="15%" class="gridHeaderStyle_1row">Address 1</td>
                                                <td width="15%" class="gridHeaderStyle_1row">Address 2</td>
                                                <td width="15%" class="gridHeaderStyle_1row">Address 3</td>
                                                <td width="10%" class="gridHeaderStyle_1row">Address 4</td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <asp:Panel ID="plnGridSupplierSearch" runat="server" ScrollBars="Auto" Width="100%">
                                            <asp:GridView ID="gvSupplierSearchList" runat="server" AutoGenerateColumns="False" Width="97.75%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                CssClass="gridStyle_hover" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                EmptyDataText="No data found" ShowHeader="false" OnRowCommand="gvSupplierSearchList_RowCommand" OnRowDataBound="gvSupplierSearchList_RowDataBound"
                                                RowStyle-CssClass="dataRow">
                                                <Columns>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" ItemStyle-Width="10%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSupplierNumber" runat="server" Width="99%" Text='<%# Bind("supplier_number") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" ItemStyle-Width="15%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSupplierSiteName" runat="server" Width="99%" Text='<%# Bind("supplier_site_name") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" ItemStyle-Width="20%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSupplierName" runat="server" Width="99%" Text='<%# Bind("supplier_name") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" ItemStyle-Width="15%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblAddress1" runat="server" Width="99%" Text='<%# Bind("supplier_add1") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" ItemStyle-Width="15%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblAddress2" runat="server" Width="99%" Text='<%# Bind("supplier_add2") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" ItemStyle-Width="15%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblAddress3" runat="server" Width="99%" Text='<%# Bind("supplier_add3") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" ItemStyle-Width="10%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblAddress4" runat="server" Width="99%" Text='<%# Bind("supplier_add4") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkBtnDblClk" runat="server" CommandName="dblClk" Text="dblClick">
                                                            </asp:LinkButton>
                                                            <asp:HiddenField ID="hdnEscProfileId" runat="server" Value='<%# Bind("supplier_postcode") %>' />
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
            </asp:Panel>
            <%--Supplier search list popup-- Ends%>--%>

             <%--JIRA-908 Changes by Ravi on 12/02/2019 -- Start--%>
            <asp:Button ID="dummyConfirmAPInterface" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmAPInterface" runat="server" PopupControlID="pnlConfirmAPInterface" TargetControlID="dummyConfirmAPInterface"
                CancelControlID="btnNo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlConfirmAPInterface" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmation" runat="server" Text="API Interface Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblText" runat="server"
                                CssClass="identifierLable" Text="Do you want to create AP interface file?"></asp:Label>
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

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnHeaderInvoiceNo" runat="server" Value="N" />
            <asp:HiddenField ID="hdnPageIndex" runat="server" />
            <asp:HiddenField ID="hdnGridPageSize" runat="server" />
            <asp:HiddenField ID="hdnSupplier" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsvalidRoyaltor" runat="server" Value="N" />
            <asp:HiddenField ID="hdnPageNumber" runat="server" Value="" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" />
             <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
            <asp:Button ID="btnSupplierSearch" runat="server" Style="display: none;" OnClick="btnSupplierSearch_Click" CausesValidation="false" />            
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

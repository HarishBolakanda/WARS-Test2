<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SupplierAddressOverwrite.aspx.cs" Inherits="WARS.Contract.SupplierAddressOverwrite" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Supplier Address Overwrite" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>
<%@ Register TagPrefix="sao" TagName="SupplierAddressOverwrite" Src="~/UserControls/SupplierAddressOverwrite.ascx" %>

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

        //Supplier auto populate search functionalities        
        var txtOwnSrch;

        function overwriteSuppListPopulating() {
            txtSupplier = document.getElementById("<%= txtSupplier.ClientID %>");
            txtSupplier.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtSupplier.style.backgroundRepeat = 'no-repeat';
            txtSupplier.style.backgroundPosition = 'right';
        }

        function overwriteSuppListPopulated() {
            txtSupplier = document.getElementById("<%= txtSupplier.ClientID %>");
            txtSupplier.style.backgroundImage = 'none';
        }

        function overwriteSuppScrollPosition(sender, args) {
            var autoCompPnl = document.getElementById("<%= acePnlOverwriteSuppFuzzySearch.ClientID %>");
            autoCompPnl.scrollTop = 1;

        }

        function overwriteSuppListItemSelected(sender, args) {
            //debugger;
            var ownSrchVal = args.get_value();
            if (ownSrchVal == 'No results found') {
                document.getElementById("<%= txtSupplier.ClientID %>").value = "";
            }
            else {
                //document.getElementById("<%= txtSupplier.ClientID %>").value = ownSrchVal;
                document.getElementById('<%=btnOverwriteSupplierSearch.ClientID%>').click();
            }


        }
        //================================End


        //grid panel height adjustment functioanlity - starts
        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
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

        //Pop up interested party list       
        function OntxtPayeeKeyDown() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnOverwritePayeeSearch.ClientID%>').click();
            }

        }
        //============== End


        //Validate any unsaved data on browser window close/refresh
        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }
        //End
        //Data validations - start

        function ValidatePayee(sender, args) {
            hdnPayee = document.getElementById("<%=hdnPayee.ClientID %>").value;
            txtPayee = document.getElementById("<%=txtPayee.ClientID %>").value;
            if (hdnPayee != txtPayee || txtPayee == '') {
                args.IsValid = false;
            }
        }

        function ValidateSupplier(sender, args) {

            hdnSupplier = document.getElementById("<%=hdnSupplier.ClientID %>").value;
        txtSupplier = document.getElementById("<%=txtSupplier.ClientID %>").value;
        if (hdnSupplier != txtSupplier || txtSupplier == '') {
            args.IsValid = false;
        }
    }

    function ValidateSave() {
        //warning on save validation fail            
        if (!Page_ClientValidate("valGrpSave")) {
            Page_BlockSubmit = false;
            DisplayMessagePopup("Invalid or missing data!");
            return false;
        }
        else {
            return true;
        }
    }


    function ComfirmValidPayee() {
        //warning on invalid payee  
        hdnPayee = document.getElementById("<%=hdnPayee.ClientID %>").value;
        txtPayee = document.getElementById("<%=txtPayee.ClientID %>").value;
        if (hdnPayee != txtPayee || txtPayee == '') {
            DisplayMessagePopup("Please select a valid payee!");
            return false;
        }
        else {
            return true;
        }
    }
    //===========Data validations - End
    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="4">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    PAYEE DETAILS - ADDRESS OVERWRITE
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td colspan="3" valign="top">
                        <table width="100%">
                            <tr>
                                <td width="10%" class="identifierLable_large_bold">Payee</td>
                                <td width="20%">
                                    <asp:TextBox ID="txtPayee" runat="server" Width="99%" CssClass="textboxStyle" onkeydown="javascript: OntxtPayeeKeyDown();" TabIndex="100"></asp:TextBox>
                                    <ajaxToolkit:TextBoxWatermarkExtender ID="tbwePayee" runat="server"
                                        TargetControlID="txtPayee" WatermarkText="Enter Search Text" WatermarkCssClass="watermarked" />
                                    <asp:DropDownList ID="ddlPayee" runat="server" CssClass="ddlStyle" Width="99.9%" TabIndex="101" Visible="false"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlPayee_OnSelectedIndexChanged">
                                    </asp:DropDownList>

                                </td>
                                <td width="5%">
                                    <asp:CustomValidator ID="valPayee" runat="server" ValidationGroup="valGrpSave" CssClass="requiredFieldValidator"
                                        ClientValidationFunction="ValidatePayee" ToolTip="Not a valid payee. Please select from the search list."
                                        ErrorMessage="*"></asp:CustomValidator>
                                     <asp:RequiredFieldValidator runat="server" ID="rfPayee" ControlToValidate="ddlPayee" Visible="false"
                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please select payee" InitialValue="-" Display="Dynamic"
                                        ValidationGroup="valGrpSave"></asp:RequiredFieldValidator>
                                </td>
                                <td align="right">
                                    <asp:Button ID="btnAddressOverwrite" runat="server" CssClass="ButtonStyle" OnClick="btnAddressOverwrite_Click" ValidationGroup="valGrpSave"
                                        TabIndex="105" Text="Overwrite Payee Address" UseSubmitBehavior="false" Width="20%" OnClientClick="if (!ValidateSave()) { return false;};" />
                                </td>
                            </tr>
                            <tr>
                                <td width="10%" class="identifierLable_large_bold">Royaltor</td>
                                <td width="20%">
                                    <asp:DropDownList ID="ddlRoyaltor" runat="server" CssClass="ddlStyle" Width="99.9%" TabIndex="101"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlRoyaltor_OnSelectedIndexChanged">
                                    </asp:DropDownList></td>
                                <td width="5%">
                                    <asp:RequiredFieldValidator runat="server" ID="rfddlRoyaltor" ControlToValidate="ddlRoyaltor"
                                        Text="*" CssClass="requiredFieldValidator" ToolTip="Please select royaltor" InitialValue="-" Display="Dynamic"
                                        ValidationGroup="valGrpSave"></asp:RequiredFieldValidator></td>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnClear" runat="server" CssClass="ButtonStyle" OnClick="btnClear_Click"
                            TabIndex="106" Text="Clear" UseSubmitBehavior="false" Width="20%" onkeydown="OnTabPress();" />
                    </td>
                </tr>
                <tr>
                    <td class="identifierLable_large_bold">Supplier</td>
                    <td>
                        <asp:TextBox ID="txtSupplier" runat="server" Width="99%" CssClass="textboxStyle" TabIndex="102"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="aceSuppFuzzySearch" runat="server"
                            ServiceMethod="FuzzySuppAddOverwriteSupplierList"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtSupplier"
                            FirstRowSelected="true"
                            OnClientPopulating="overwriteSuppListPopulating"
                            OnClientPopulated="overwriteSuppListPopulated"
                            OnClientHidden="overwriteSuppListPopulated"
                            OnClientShown="overwriteSuppScrollPosition"
                            OnClientItemSelected="overwriteSuppListItemSelected"
                            CompletionListElementID="acePnlOverwriteSuppFuzzySearch" />
                        <asp:Panel ID="acePnlOverwriteSuppFuzzySearch" runat="server" CssClass="identifierLable" />
                    </td>
                    <td align="left">
                        <asp:CustomValidator ID="valSupplier" runat="server" ValidationGroup="valGrpSave" CssClass="requiredFieldValidator"
                            ClientValidationFunction="ValidateSupplier" ToolTip="Not a valid Supplier. Please select from the search list." Display="Dynamic"
                            ErrorMessage="*"></asp:CustomValidator>
                        <asp:ImageButton ID="btnSupplierFuzzySearch" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button"
                            OnClick="btnSupplierFuzzySearch_Click" ToolTip="Search by supplier number/name" TabIndex="-1" />
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td class="identifierLable_large_bold">Supplier Site Name</td>
                    <td>
                        <asp:DropDownList ID="ddlSupplierSite" runat="server" CssClass="ddlStyle" Width="99.9%" TabIndex="103"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlSupplierSite_OnSelectedIndexChanged">
                        </asp:DropDownList>

                    </td>
                    <td>
                        <asp:RequiredFieldValidator runat="server" ID="rfvddlSupplierSite" ControlToValidate="ddlSupplierSite"
                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please select Supplier Site Name" InitialValue="-" Display="Dynamic"
                            ValidationGroup="valGrpSave"></asp:RequiredFieldValidator></td>
                    <td></td>
                </tr>
                <td class="identifierLable_large_bold">Mismatch Flag</td>
                <td>
                    <asp:TextBox ID="txtMismatchFlag" runat="server" Width="15px" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                </td>
                <td></td>
                <td></td>
                </tr>
                <tr>
                    <td colspan="4">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <table width="80%">
                            <tr>
                                <td width="48%" valign="top" class="table_with_border">
                                    <table width="100%">
                                        <tr>
                                            <td width="5%"></td>
                                            <td width="20%"></td>
                                            <td width="70%"></td>
                                            <td width="5%"></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="table_header_with_border" colspan="3">Supplier Address</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Supplier Name</td>
                                            <td>
                                                <asp:TextBox ID="txtSuppName" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Address 1</td>
                                            <td>
                                                <asp:TextBox ID="txtSuppAdd1" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Address 2</td>
                                            <td>
                                                <asp:TextBox ID="txtSuppAdd2" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Address 3</td>
                                            <td>
                                                <asp:TextBox ID="txtSuppAdd3" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Address 4</td>
                                            <td>
                                                <asp:TextBox ID="txtSuppCity" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Postcode</td>
                                            <td>
                                                <asp:TextBox ID="txtSuppPostcode" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                    </table>
                                </td>
                                <td></td>
                                <td width="48%" valign="top" class="table_with_border">
                                    <table width="100%">
                                        <tr>
                                            <td width="5%"></td>
                                            <td width="20%"></td>
                                            <td width="70%"></td>
                                            <td width="5%"></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="table_header_with_border" colspan="3">Payee Address</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Payee Name</td>
                                            <td>
                                                <asp:TextBox ID="txtPayeeName" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Address 1</td>
                                            <td>
                                                <asp:TextBox ID="txtPayeeAdd1" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Address 2</td>
                                            <td>
                                                <asp:TextBox ID="txtPayeeAdd2" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Address 3</td>
                                            <td>
                                                <asp:TextBox ID="txtPayeeAdd3" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Address 4</td>
                                            <td>
                                                <asp:TextBox ID="txtPayeeAdd4" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Postcode</td>
                                            <td>
                                                <asp:TextBox ID="txtPayeePostcode" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                            </td>
                                            <td></td>
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
                    <div id="Search" style="font-weight: bold; color: Black; z-index: 2">
                        <img src="../Images/InProgress2.gif" alt="" />
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>

            <ajaxToolkit:ModalPopupExtender ID="mPopupPageLevel" runat="server" PopupControlID="progressBarPageLevel" TargetControlID="progressBarPageLevel"
                BackgroundCssClass="progressBar" RepositionMode="RepositionOnWindowResize">
            </ajaxToolkit:ModalPopupExtender>

            <%--Fuzzy search popup--%>
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
            <%--Fuzzy search popup-- Ends--%>

            <%--Interested party search list popup--%>
            <asp:Button ID="dummyIntPartySearch" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeIntPartySearch" runat="server" PopupControlID="pnlIntPartyPopup" TargetControlID="dummyIntPartySearch"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlIntPartyPopup" runat="server" align="left" Width="50%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="identifierLable" align="center">Interested party search List
                                    </td>
                                    <td align="right" style="vertical-align: top;">
                                        <asp:ImageButton ID="btnClosePopupPayeeSearch" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" OnClick="btnClosePopupPayeeSearch_Click" />
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
                                                <td width="15%" class="gridHeaderStyle_1row">Name</td>
                                                <td width="10%" class="gridHeaderStyle_1row">Address 1</td>
                                                <td width="10%" class="gridHeaderStyle_1row">Address 2</td>
                                                <td width="10%" class="gridHeaderStyle_1row">Address 3</td>
                                                <td width="10%" class="gridHeaderStyle_1row">Address 4</td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <asp:Panel ID="plnGridIntPartySearch" runat="server" ScrollBars="Auto">
                                            <asp:GridView ID="gvIntPartySearchList" runat="server" AutoGenerateColumns="False" Width="97.75%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                CssClass="gridStyle_hover" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                EmptyDataText="No data found" ShowHeader="false" OnRowCommand="gvIntPartySearchList_RowCommand" OnRowDataBound="gvIntPartySearchList_RowDataBound"
                                                RowStyle-CssClass="dataRow">
                                                <Columns>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align"
                                                        ItemStyle-Width="15%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblName" runat="server" Width="99%" Text='<%# Bind("int_party_name") %>' CssClass="identifierLable"></asp:Label>
                                                            <asp:HiddenField ID="hdnIntPartyId" runat="server" Value='<%# Bind("int_party_id") %>' />
                                                            <asp:HiddenField ID="hdnIntPartyType" runat="server" Value='<%# Bind("int_party_type") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align"
                                                        ItemStyle-Width="10%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblAddress1" runat="server" Width="99%" Text='<%# Bind("int_party_add1") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align"
                                                        ItemStyle-Width="10%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblAddress2" runat="server" Width="99%" Text='<%# Bind("int_party_add2") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align"
                                                        ItemStyle-Width="10%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblAddress3" runat="server" Width="99%" Text='<%# Bind("int_party_add3") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align"
                                                        ItemStyle-Width="10%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblAddress4" runat="server" Width="99%" Text='<%# Bind("int_party_add4") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
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
            </asp:Panel>
            <%--Interested party search list popup-- Ends%>--%>

            <%--Save/Cancel changes popup--%>
            <asp:Button ID="dummySaveCancel" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeSaveCancel" runat="server" PopupControlID="pnlSaveCancel" TargetControlID="dummySaveCancel"
                CancelControlID="btnClosePopupSaveCancel" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlSaveCancel" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td align="right" style="vertical-align: top;">
                            <asp:ImageButton ID="btnClosePopupSaveCancel" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMessage" runat="server" CssClass="identifierLable"
                                Text="This will overwrite the Payee name and address. Select Continue to Update or Cancel to exit."></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnContinue" runat="server" Text="Continue" CssClass="ButtonStyle"
                                            OnClick="btnContinue_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="ButtonStyle" OnClick="btnCancel_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Save/Cancel changes popup Ends--%>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnSelectedPayee" runat="server" Value="N" />
            <asp:HiddenField ID="hdnPayee" runat="server" Value="N" />
            <asp:HiddenField ID="hdnSupplier" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIntPartyId" runat="server" />
            <asp:HiddenField ID="hdnSupplierNumberSearch" runat="server" Value="N" />

            <asp:Label ID="lblTab" runat="server" Text="" TabIndex="99"></asp:Label>
            <asp:Button ID="btnOverwriteSupplierSearch" runat="server" Style="display: none;" OnClick="btnOverwriteSupplierSearch_Click" CausesValidation="false" />
            <asp:Button ID="btnOverwritePayeeSearch" runat="server" Style="display: none;" OnClick="btnOverwritePayeeSearch_Click" CausesValidation="false" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

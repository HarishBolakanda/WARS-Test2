<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TransactionRetrieval.aspx.cs" Inherits="WARS.TransactionRetrieval" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Transaction Retrieval" MaintainScrollPositionOnPostback="true" %>  <%--ClientIDMode="AutoID"--%>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>
<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //to open Transaction retrieval status screen 
        function OpenTransRetStatusScreen() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../StatementProcessing/TransactionRetrievalStatus.aspx', '_self');
            }
            else {
                var win = window.open('../StatementProcessing/TransactionRetrievalStatus.aspx', '_self');
                win.focus();
            }
        }


    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 1px;">
                        <asp:Button ID="btnOpenTransactionStatus" runat="server" CssClass="LinkButtonStyle"
                            OnClientClick="OpenTransRetStatusScreen();" Text="Transaction Retrieval Status" UseSubmitBehavior="false"
                            Width="98%" CausesValidation="false" />
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
        var scrollTopNotSel;
        var scrollTopSel;
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
            postBackElementID = document.getElementById('__EVENTTARGET').value.substring(document.getElementById('__EVENTTARGET').value.lastIndexOf("$") + 1);
            if (postBackElementID == '') {
                postBackElementID = args.get_postBackElement().id.substring(args.get_postBackElement().id.lastIndexOf("_") + 1);
            }

            if (postBackElementID == 'btnAddToSelected' || postBackElementID == 'btnRemoveFromSelected') {
                xPos = sender._scrollPosition.x;
                yPos = sender._scrollPosition.y;

                //hold scroll position 
                var PnlReferenceNotSel = document.getElementById("<%=PnlGridCatNotSelected.ClientID %>");
                scrollTopNotSel = PnlReferenceNotSel.scrollTop;

                var PnlReferenceSel = document.getElementById("<%=PnlGridCatSelected.ClientID %>");
                scrollTopSel = PnlReferenceSel.scrollTop;

            }

        }

        function EndRequestHandler(sender, args) {
            //Hide the modal popup - the update progress
            var popup = $find('<%= mPopupPageLevel.ClientID %>');
            if (popup != null) {
                popup.hide();
            }

            //to maintain scroll position
            postBackElementID = document.getElementById('__EVENTTARGET').value.substring(document.getElementById('__EVENTTARGET').value.lastIndexOf("$") + 1);
            if (postBackElementID == '') {
                postBackElementID = sender._postBackSettings.sourceElement.id.substring(sender._postBackSettings.sourceElement.id.lastIndexOf("_") + 1);
            }

            if (postBackElementID == 'btnAddToSelected' || postBackElementID == 'btnRemoveFromSelected') {
                window.scrollTo(xPos, yPos);

                //set scroll position 
                var PnlReferenceNotSel = document.getElementById("<%=PnlGridCatNotSelected.ClientID %>");
                PnlReferenceNotSel.scrollTop = scrollTopNotSel;

                var PnlReferenceSel = document.getElementById("<%=PnlGridCatSelected.ClientID %>");
                PnlReferenceSel.scrollTop = scrollTopSel;

            }


        }
        //======================= End

        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
            document.getElementById("<%=PnlGridCatNotSelected.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        //======================= End

        //Fuzzy search filters - Not selected grid
        //Royaltor & Option period
        var txtRoyOptPrd;
        function RoyOptPrdSelected(sender, args) {
            document.getElementById("<%=hdnNotSelGridFilterSelected.ClientID %>").innerText = "Y";
            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtRoyOptPrd.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value = "Y";
            }
        }

        function RoyOptPrdListPopulating() {
            document.getElementById("<%=hdnNotSelGridFilterSelected.ClientID %>").innerText = "N";
            document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value = "N";
            txtRoyOptPrd = document.getElementById("<%= txtRoyOptPrd.ClientID %>");
            txtRoyOptPrd.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoyOptPrd.style.backgroundRepeat = 'no-repeat';
            txtRoyOptPrd.style.backgroundPosition = 'right';
        }

        function RoyOptPrdListPopulated() {
            txtRoyOptPrd = document.getElementById("<%= txtRoyOptPrd.ClientID %>");
            txtRoyOptPrd.style.backgroundImage = 'none';
        }

        function RoyOptPrdResetScrollPosition(sender, args) {
            var acePnlRoyOptPrd = document.getElementById("<%= acePnlRoyOptPrd.ClientID %>");
            acePnlRoyOptPrd.scrollTop = 1;

        }

        //Cat Artist
        var txtCatArtist;
        function CatArtistSelected(sender, args) {
            document.getElementById("<%=hdnNotSelGridFilterSelected.ClientID %>").innerText = "Y";
            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtCatArtist.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value = "Y";
            }
        }

        function CatArtistListPopulating() {
            document.getElementById("<%=hdnNotSelGridFilterSelected.ClientID %>").innerText = "N";
            document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value = "N";
            txtCatArtist = document.getElementById("<%= txtCatArtist.ClientID %>");
            txtCatArtist.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtCatArtist.style.backgroundRepeat = 'no-repeat';
            txtCatArtist.style.backgroundPosition = 'right';
        }

        function CatArtistListPopulated() {
            txtCatArtist = document.getElementById("<%= txtCatArtist.ClientID %>");
            txtCatArtist.style.backgroundImage = 'none';
        }

        function CatArtistResetScrollPosition(sender, args) {
            var acePnlCatArtist = document.getElementById("<%= acePnlCatArtist.ClientID %>");
            acePnlCatArtist.scrollTop = 1;

        }


        //Project Code
        var txtProjCode;
        function ProjCodeSelected(sender, args) {
            document.getElementById("<%=hdnNotSelGridFilterSelected.ClientID %>").innerText = "Y";
            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtProjCode.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value = "Y";
            }
        }

        function ProjCodeListPopulating() {
            document.getElementById("<%=hdnNotSelGridFilterSelected.ClientID %>").innerText = "N";
            document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value = "N";
            txtProjCode = document.getElementById("<%= txtProjCode.ClientID %>");
            txtProjCode.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtProjCode.style.backgroundRepeat = 'no-repeat';
            txtProjCode.style.backgroundPosition = 'right';
        }

        function ProjCodeListPopulated() {
            txtProjCode = document.getElementById("<%= txtProjCode.ClientID %>");
            txtProjCode.style.backgroundImage = 'none';
        }

        function ProjCodeResetScrollPosition(sender, args) {
            var acePnlProjCode = document.getElementById("<%= acePnlProjCode.ClientID %>");
            acePnlProjCode.scrollTop = 1;
        }

        //====================== End     

        //allow search text field enter key postback

        function OntxtCatNumKeyDown() {
            var txtCatNum = document.getElementById("<%= txtCatNum.ClientID %>");
            if ((event.keyCode == 13)) {
                document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value = "Y";
                document.getElementById('<%=btnCatNumNotSelecSearch.ClientID%>').click();
            }
            else {
                document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value = "N";
                return false;
            }
        }

        function OntxtRoyOptPrdKeyDown() {
            var txtRoyOptPrd = document.getElementById("<%= txtRoyOptPrd.ClientID %>");
            if ((event.keyCode == 13)) {
                if (txtRoyOptPrd.value == "") {
                    document.getElementById("<%=hdnNotSelGridFilterSelected.ClientID %>").innerText = "Y";
                    document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value = "Y";
                    __doPostBack("txtRoyOptPrd", "txtRoyOptPrd_TextChanged");
                }
                else {
                    if (document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value == "Y") {
                        __doPostBack("txtRoyOptPrd", "txtRoyOptPrd_TextChanged");
                    }
                    else {
                        return false;
                    }
                }
            }
            else {
                if (txtRoyOptPrd.value == "") {
                    txtRoyOptPrd.style.backgroundImage = 'none';
                }
                return false;
            }
        }

        function OntxtCatArtistKeyDown() {
            var txtCatArtist = document.getElementById("<%= txtCatArtist.ClientID %>");
            if ((event.keyCode == 13)) {
                if (txtCatArtist.value == "") {
                    document.getElementById("<%=hdnNotSelGridFilterSelected.ClientID %>").innerText = "Y";
                    document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value = "Y";
                    __doPostBack("txtCatArtist", "txtCatArtist_TextChanged");
                }
                else {
                    if (document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value == "Y") {
                        __doPostBack("txtCatArtist", "txtCatArtist_TextChanged");
                    }
                    else {
                        return false;
                    }
                }
            }
            else {
                if (txtCatArtist.value == "") {
                    txtCatArtist.style.backgroundImage = 'none';
                }
                return false;
            }
        }

        function OntxtProjCodeKeyDown() {
            var txtProjCode = document.getElementById("<%= txtProjCode.ClientID %>");
            if ((event.keyCode == 13)) {
                if (txtProjCode.value == "") {
                    document.getElementById("<%=hdnNotSelGridFilterSelected.ClientID %>").innerText = "Y";
                    document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value = "Y";
                    __doPostBack("txtProjCode", "txtProjCode_TextChanged");
                }
                else {
                    if (document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value == "Y") {
                        __doPostBack("txtProjCode", "txtProjCode_TextChanged");
                    }
                    else {
                        return false;
                    }
                }
            }
            else {
                if (txtProjCode.value == "") {
                    txtProjCode.style.backgroundImage = 'none';
                }
                return false;
            }
        }




        //================================= End
        function ValidateChanges() {
            eval(this.href);
        }

        function IsDataChanged() {
            ValidateGridDataChanged();
            var hdnDataChanged = document.getElementById("<%=hdnGridDataChanged.ClientID %>").value;
            if (hdnDataChanged == "Y") {
                return true;
            }
            else {
                return false;
            }
        }

        //Warn on changes made and not saved

        function WarnOnUnSavedData() {
            ValidateGridDataChanged();
            var hdnGridDataChanged = document.getElementById("<%=hdnGridDataChanged.ClientID %>").value;
            if (hdnGridDataChanged == "Y") {
                return warningMsgOnUnSavedData;
            }
        }
        window.onbeforeunload = WarnOnUnSavedData;

        function ValidateGridDataChanged() {
            debugger;
            gvCatNotSelected = document.getElementById("<%= gvCatNotSelected.ClientID %>");
            gvCatSelected = document.getElementById("<%= gvCatSelected.ClientID %>");
            var checkedCatNotSel = 0;
            var checkedCatSel = 0;

            if (gvCatNotSelected != null) {
                var gvRows = gvCatNotSelected.rows;
                var rowIndex = 0;
                for (var i = 0; i < gvRows.length; i++) {                    
                    cbCatNotSelec = document.getElementById("ContentPlaceHolderBody_gvCatNotSelected_" + 'cbCatNotSelec' + '_' + i);                   
                    if (cbCatNotSelec != null && !(cbCatNotSelec.disabled) && cbCatNotSelec.checked) {
                        checkedCatNotSel = checkedCatNotSel + 1;
                        break;
                    }
                }
            }

            if (gvCatSelected != null) {
                var rowIndex = 0;
                var gvRows = gvCatSelected.rows;                
                for (var i = 0; i < gvRows.length; i++) {                    
                    cbCatSelec = document.getElementById("ContentPlaceHolderBody_gvCatSelected_" + 'cbCatSelec' + '_' + i);

                    if (cbCatSelec != null && !(cbCatSelec.disabled) && cbCatSelec.checked) {
                        checkedCatSel = checkedCatSel + 1;
                        break;
                    }
                }
            }

            if (checkedCatNotSel > 0 || checkedCatSel > 0) {
                document.getElementById("<%= hdnGridDataChanged.ClientID %>").value = "Y";
            }
        }


    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="7">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    TRANSACTION RETRIEVAL
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="1%"></td>
                    <td width="11%" class="identifierLable_large_bold">Received Date range &nbsp -
                    </td>
                    <td width="5%" class="identifierLable_large_bold">Start Date</td>
                    <td width="8%">
                        <asp:TextBox ID="txtFromDate" runat="server" Width="65" CssClass="identifierLable"
                            ValidationGroup="valSave" ToolTip="mm/yyyy" TabIndex="100"></asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmeFromDate" runat="server" TargetControlID="txtFromDate"
                            WatermarkText="From Date" WatermarkCssClass="waterMarkText">
                        </ajaxToolkit:TextBoxWatermarkExtender>
                        <ajaxToolkit:MaskedEditExtender ID="mteFromDate" runat="server"
                            TargetControlID="txtFromDate" Mask="99/9999" AcceptNegative="None"
                            ClearMaskOnLostFocus="false" />
                    </td>
                    <td width="5%" class="identifierLable_large_bold">End Date</td>
                    <td width="8%">
                        <asp:TextBox ID="txtToDate" runat="server" Width="65" CssClass="identifierLable"
                            ValidationGroup="valSave" ToolTip="mm/yyyy" TabIndex="101"></asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmeToDate" runat="server" TargetControlID="txtToDate"
                            WatermarkText="To Date" WatermarkCssClass="waterMarkText">
                        </ajaxToolkit:TextBoxWatermarkExtender>
                        <ajaxToolkit:MaskedEditExtender ID="mteToDate" runat="server"
                            TargetControlID="txtToDate" Mask="99/9999" AcceptNegative="None"
                            ClearMaskOnLostFocus="false" />

                    </td>
                    <td>
                        <table width="100%">
                            <tr>
                                <td width="50%">
                                    <asp:CustomValidator ID="valFrmToDates" runat="server" ValidationGroup="valSave" CssClass="errorMessage"
                                        OnServerValidate="valFrmToDates_ServerValidate"
                                        ErrorMessage="From date should be earlier than the To date!"></asp:CustomValidator>
                                </td>
                                <td width="3%"></td>
                                <td align="right">
                                    <asp:Button ID="btnRetrieveTrans" runat="server" Text="Retrieve Transactions" CssClass="ButtonStyle" UseSubmitBehavior="false"
                                        Width="38%" OnClick="btnRetrieveTrans_Click" CausesValidation="true" ValidationGroup="valSave" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="5"></td>
                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold" valign="top">Catalogue Search
                    </td>
                    <td colspan="5">
                        <table width="37%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="40%" valign="top">
                                    <asp:TextBox ID="txtRoyOptPrd" runat="server" Width="95%" CssClass="identifierLable"
                                        OnTextChanged="txtRoyOptPrd_TextChanged" AutoPostBack="true" onkeydown="javascript: OntxtRoyOptPrdKeyDown();"
                                        TabIndex="102"></asp:TextBox>
                                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmeTxtRoyOptPrd" runat="server" TargetControlID="txtRoyOptPrd"
                                        WatermarkText="Royaltor & Option period" WatermarkCssClass="waterMarkText">
                                    </ajaxToolkit:TextBoxWatermarkExtender>
                                    <ajaxToolkit:AutoCompleteExtender ID="aceRoyOptPrd" runat="server"
                                        ServiceMethod="FuzzyTransRetRoyOpPrdList"
                                        ServicePath="~/Services/FuzzySearch.asmx"
                                        MinimumPrefixLength="1"
                                        CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                        TargetControlID="txtRoyOptPrd"
                                        FirstRowSelected="true"
                                        OnClientItemSelected="RoyOptPrdSelected"
                                        OnClientPopulating="RoyOptPrdListPopulating"
                                        OnClientPopulated="RoyOptPrdListPopulated"
                                        OnClientHidden="RoyOptPrdListPopulated"
                                        OnClientShown="RoyOptPrdResetScrollPosition"
                                        CompletionListElementID="acePnlRoyOptPrd" />
                                    <asp:Panel ID="acePnlRoyOptPrd" runat="server" CssClass="identifierLable" />
                                </td>
                                <td valign="top" width="5%">
                                    <asp:ImageButton ID="fuzzySearchRoyOptPrd" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button"
                                        OnClick="fuzzySearchRoyOptPrd_Click" ToolTip="Search Royaltor & Option period" />
                                </td>
                                <td valign="top" width="40%">
                                    <asp:TextBox ID="txtCatArtist" runat="server" Width="95%" CssClass="identifierLable"
                                        OnTextChanged="txtCatArtist_TextChanged" AutoPostBack="true" onkeydown="javascript: OntxtCatArtistKeyDown();"
                                        TabIndex="103"></asp:TextBox>
                                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmeTxtCatArtist" runat="server" TargetControlID="txtCatArtist"
                                        WatermarkText="Catalogue Artist / Name" WatermarkCssClass="waterMarkText">
                                    </ajaxToolkit:TextBoxWatermarkExtender>
                                    <ajaxToolkit:AutoCompleteExtender ID="aceCatArtist" runat="server"
                                        ServiceMethod="FuzzyTransRetArtistList"
                                        ServicePath="~/Services/FuzzySearch.asmx"
                                        MinimumPrefixLength="1"
                                        CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                        TargetControlID="txtCatArtist"
                                        FirstRowSelected="true"
                                        OnClientItemSelected="CatArtistSelected"
                                        OnClientPopulating="CatArtistListPopulating"
                                        OnClientPopulated="CatArtistListPopulated"
                                        OnClientHidden="CatArtistListPopulated"
                                        OnClientShown="CatArtistResetScrollPosition"
                                        CompletionListElementID="acePnlCatArtist" />
                                    <asp:Panel ID="acePnlCatArtist" runat="server" CssClass="identifierLable" />
                                </td>
                                <td valign="top" width="5%">
                                    <asp:ImageButton ID="fuzzySearchCatArtist" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button"
                                        OnClick="fuzzySearchCatArtist_Click" ToolTip="Catalogue Artist / Name" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtCatNum" runat="server" Width="95%" CssClass="identifierLable"
                                        onkeydown="javascript: OntxtCatNumKeyDown();" TabIndex="105"></asp:TextBox>
                                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmeTxtCatNum" runat="server" TargetControlID="txtCatNum"
                                        WatermarkText="Catalogue No. / Title" WatermarkCssClass="waterMarkText">
                                    </ajaxToolkit:TextBoxWatermarkExtender>
                                </td>
                                <td></td>
                                <td>
                                    <asp:TextBox ID="txtProjCode" runat="server" Width="95%" CssClass="identifierLable"
                                        OnTextChanged="txtProjCode_TextChanged" AutoPostBack="true" onkeydown="javascript: OntxtProjCodeKeyDown();"
                                        TabIndex="104"></asp:TextBox>
                                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmeTxtProjCode" runat="server" TargetControlID="txtProjCode"
                                        WatermarkText="Project No. / Title" WatermarkCssClass="waterMarkText">
                                    </ajaxToolkit:TextBoxWatermarkExtender>
                                    <ajaxToolkit:AutoCompleteExtender ID="aceProjCode" runat="server"
                                        ServiceMethod="FuzzySearchAllProjectList"
                                        ServicePath="~/Services/FuzzySearch.asmx"
                                        MinimumPrefixLength="1"
                                        CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                        TargetControlID="txtProjCode"
                                        FirstRowSelected="true"
                                        OnClientItemSelected="ProjCodeSelected"
                                        OnClientPopulating="ProjCodeListPopulating"
                                        OnClientPopulated="ProjCodeListPopulated"
                                        OnClientHidden="ProjCodeListPopulated"
                                        OnClientShown="ProjCodeResetScrollPosition"
                                        CompletionListElementID="acePnlProjCode" />
                                    <asp:Panel ID="acePnlProjCode" runat="server" CssClass="identifierLable" />
                                </td>
                                <td>
                                    <asp:ImageButton ID="fuzzySearchProjCode" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button"
                                        OnClick="fuzzySearchProjCode_Click" ToolTip="Search Project No. / Title" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td width="1%"></td>
                    <td colspan="6">
                        <table width="100%">
                            <tr>
                                <td width="47%" valign="bottom">
                                    <table width="97.38%" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td width="100%" class="gridTitle_Bold" style="cursor: none; vertical-align: middle; text-align: center; padding: 0.5%; font-weight: bold;">Not Selected to Retrieve
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td width="6%"></td>
                                <td width="47%" valign="bottom">
                                    <table width="97%" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td width="100%" class="gridTitle_Bold" style="cursor: none; vertical-align: middle; text-align: center; padding: 0.5%; font-weight: bold;">Selected to Retrieve
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td align="left" valign="top">
                                    <table width="100%" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td align="left" valign="top">
                                                <table width="97.2%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="18%" class="gridHeaderStyle_2rows"
                                                            onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                            onclick="document.getElementById('<%=btnCatNoNotSelecSort.ClientID%>').click();">Catalogue no.</td>
                                                        <td width="28%" class="gridHeaderStyle_2rows"
                                                            onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                            onclick="document.getElementById('<%=btnTitleNotSelecSort.ClientID%>').click();">Title</td>
                                                        <td width="22%" class="gridHeaderStyle_2rows"
                                                            onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                            onclick="document.getElementById('<%=btnArtistNotSelecSort.ClientID%>').click();">Artist</td>
                                                        <td width="20%" class="gridHeaderStyle_2rows" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                            onclick="document.getElementById('<%=btnProjectNotSelecSort.ClientID%>').click();">Project Title</td>
                                                        <td width="7%" class="gridHeaderStyle_2rows" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                            onclick="document.getElementById('<%=btnConfigNotSelecSort.ClientID%>').click();">Config</td>
                                                        <td width="5%" class="gridHeaderStyle_2rows">
                                                            <asp:CheckBox ID="cbCatNotSelecSelectAll" runat="server" OnCheckedChanged="cbCatNotSelecSelectAll_CheckedChanged"
                                                                AutoPostBack="true" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="5" align="center">
                                                <asp:Panel ID="PnlGridCatNotSelected" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvCatNotSelected" runat="server" AutoGenerateColumns="False" Width="97.2%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" ShowHeaderWhenEmpty="true" EmptyDataText="No Data Found"
                                                        EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" ShowHeader="false" OnDataBound="gvCatNotSelected_DataBound" OnRowDataBound="gvCatNotSelected_RowDataBound">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-Width="18%" ItemStyle-CssClass="gridItemStyle_Left_Align_Padding">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblCatNoNotSelec" runat="server" Text='<%#Bind("catno")%>' ToolTip='<%#Bind("catno")%>' CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="28%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblTitleNotSelec" runat="server" Text='<%#Bind("catno_title")%>' ToolTip='<%#Bind("catno_title")%>' CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="22%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblArtistNotSelec" runat="server" Text='<%#Bind("artist_name")%>' ToolTip='<%#Bind("artist_name")%>' CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="20%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblProjCodeNotSelec" runat="server" Text='<%#Bind("project_title")%>' ToolTip='<%#Bind("project_title")%>' CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="7%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblConfigNotSelec" runat="server" Text='<%#Bind("config_code")%>' CssClass="identifierLable" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="cbCatNotSelec" runat="server" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                    <asp:Button ID="btnCatNoNotSelecSort" runat="server" Style="display: none;" OnClick="btnCatNoNotSelecSort_Click" CausesValidation="false" />
                                                    <asp:Button ID="btnTitleNotSelecSort" runat="server" Style="display: none;" OnClick="btnTitleNotSelecSort_Click" CausesValidation="false" />
                                                    <asp:Button ID="btnArtistNotSelecSort" runat="server" Style="display: none;" OnClick="btnArtistNotSelecSort_Click" CausesValidation="false" />
                                                    <asp:Button ID="btnProjectNotSelecSort" runat="server" Style="display: none;" OnClick="btnProjectNotSelecSort_Click" CausesValidation="false" />
                                                    <asp:Button ID="btnConfigNotSelecSort" runat="server" Style="display: none;" OnClick="btnConfigNotSelecSort_Click" CausesValidation="false" />
                                                </asp:Panel>
                                                <div>
                                                    <asp:Repeater ID="rptPager" runat="server">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>'
                                                                OnClientClick="return ValidateChanges();" ClientIDMode="AutoID" Enabled='<%# Eval("Enabled") %>' OnClick="lnkPage_Click" CssClass="gridPager"> </asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>
                                            </td>
                                </td>
                            </tr>
                        </table>
                    </td>

                    <td align="center">
                        <table width="100%">
                            <tr>
                                <td width="14%"></td>
                                <td width="30%">
                                    <asp:ImageButton ID="btnRemoveFromSelected" runat="server" ImageUrl="~/Images/groupOut.png" OnClick="btnRemoveFromSelected_Click"
                                        ToolTip="Remove from selected to retrieve" />
                                </td>
                                <td width="2%"></td>
                                <td width="30%">
                                    <asp:ImageButton ID="btnAddToSelected" runat="server" ImageUrl="~/Images/groupIn.png" OnClick="btnAddToSelected_Click"
                                        ToolTip="Add to selected to retrieve" />
                                </td>
                                <td width="14%"></td>
                            </tr>
                        </table>

                    </td>
                    <td align="left" valign="top">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td align="left" valign="top">
                                    <table width="97%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td width="18%" class="gridHeaderStyle_2rows"
                                                onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                onclick="document.getElementById('<%=btnCatNoSelecSort.ClientID%>').click();">Catalogue no.</td>
                                            <td width="28%" class="gridHeaderStyle_2rows"
                                                onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                onclick="document.getElementById('<%=btnTitleSelecSort.ClientID%>').click();">Title</td>
                                            <td width="22%" class="gridHeaderStyle_2rows"
                                                onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                onclick="document.getElementById('<%=btnArtistSelecSort.ClientID%>').click();">Artist</td>
                                            <td width="20%" class="gridHeaderStyle_2rows" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                onclick="document.getElementById('<%=btnProjectSelecSort.ClientID%>').click();">Project Title</td>
                                            <td width="7%" class="gridHeaderStyle_2rows" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                onclick="document.getElementById('<%=btnConfigSelecSort.ClientID%>').click();">Config</td>
                                            <td width="5%" class="gridHeaderStyle_2rows">
                                                <asp:CheckBox ID="cbCatSelecSelectAll" runat="server" OnCheckedChanged="cbCatSelecSelectAll_CheckedChanged"
                                                    AutoPostBack="true" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="5">
                                    <asp:Panel ID="PnlGridCatSelected" runat="server" ScrollBars="Auto" Width="100%">
                                        <asp:GridView ID="gvCatSelected" runat="server" AutoGenerateColumns="False" Width="97%" AlternatingRowStyle-BackColor="#E3EFFF"
                                            CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" ShowHeaderWhenEmpty="true" EmptyDataText="No Data Found"
                                            EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" ShowHeader="false" OnDataBound="gvCatSelected_DataBound" OnRowDataBound="gvCatSelected_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-Width="18%" ItemStyle-CssClass="gridItemStyle_Left_Align_Padding">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCatNoSelec" runat="server" Text='<%#Bind("catno")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="28%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTitleSelec" runat="server" Text='<%#Bind("catno_title")%>' ToolTip='<%#Bind("catno_title")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="22%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblArtistSelec" runat="server" Text='<%#Bind("artist_name")%>' ToolTip='<%#Bind("artist_name")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="20%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblProjCodeSelec" runat="server" Text='<%#Bind("project_title")%>' ToolTip='<%#Bind("project_title")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="7%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblConfigSelec" runat="server" Text='<%#Bind("config_code")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cbCatSelec" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <asp:Button ID="btnCatNoSelecSort" runat="server" Style="display: none;" OnClick="btnCatNoSelecSort_Click" CausesValidation="false" />
                                        <asp:Button ID="btnTitleSelecSort" runat="server" Style="display: none;" OnClick="btnTitleSelecSort_Click" CausesValidation="false" />
                                        <asp:Button ID="btnArtistSelecSort" runat="server" Style="display: none;" OnClick="btnArtistSelecSort_Click" CausesValidation="false" />
                                        <asp:Button ID="btnProjectSelecSort" runat="server" Style="display: none;" OnClick="btnProjectSelecSort_Click" CausesValidation="false" />
                                        <asp:Button ID="btnConfigSelecSort" runat="server" Style="display: none;" OnClick="btnConfigSelecSort_Click" CausesValidation="false" />


                                    </asp:Panel>
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
                        <table>
                            <tr>
                                <td>
                                    <img src="../Images/InProgress2.gif" alt="" />
                                </td>
                            </tr>
                            <tr>
                                <td class="identifierLable">Please Wait... </td>
                            </tr>
                        </table>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <ajaxToolkit:ModalPopupExtender ID="mPopupPageLevel" runat="server" BackgroundCssClass="progressBar" PopupControlID="progressBarPageLevel" RepositionMode="RepositionOnWindowResize" TargetControlID="progressBarPageLevel">
            </ajaxToolkit:ModalPopupExtender>

            <asp:Button ID="dummyConfirm" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirm" runat="server" PopupControlID="pnlPopup" TargetControlID="dummyConfirm"
                CancelControlID="btnCofirmNo" BackgroundCssClass="popupBox">
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
                                        <asp:Button ID="btnCofirmYes" runat="server" Text="Yes" CssClass="ButtonStyle" OnClick="btnCofirmYes_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnCofirmNo" runat="server" Text="No" CssClass="ButtonStyle" />
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
            <asp:HiddenField ID="hdnPageNumber" runat="server" Value="" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnNotSelGridSortDir" runat="server" />
            <asp:HiddenField ID="hdnNotSelGridSortColumn" runat="server" />
            <asp:HiddenField ID="hdnSelGridSortDir" runat="server" />
            <asp:HiddenField ID="hdnSelGridSortColumn" runat="server" />
            <asp:HiddenField ID="hdnNotSelGridFilterSelected" runat="server" />
            <asp:HiddenField ID="hdnSelGridFilterSelected" runat="server" />
            <asp:HiddenField ID="hdnSelToRetSearchSelected" runat="server" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" Value="" />
            <asp:HiddenField ID="hdnGridDataChanged" runat="server" Value="N" />
            <asp:Label ID="lblTab" runat="server" Text="" TabIndex="99"></asp:Label>
            <asp:Button ID="btnCatNumNotSelecSearch" runat="server" Style="display: none;" OnClick="btnCatNumNotSelecSearch_Click" CausesValidation="false" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

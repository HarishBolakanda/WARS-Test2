<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TransactionRetrievalStatus.aspx.cs" Inherits="WARS.TransactionRetrievalStatus" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Transaction Retrieval Status" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //to open transaction retrieval selection screen
        function OpenTransactionSelScreen() {
            var win = window.open('../StatementProcessing/TransactionRetrieval.aspx', '_self');
            win.focus();
        }

    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 1px;">
                        <asp:Button ID="btnOpenTransactionSel" runat="server" CssClass="LinkButtonStyle"
                            OnClientClick="OpenTransactionSelScreen();" Text="Transaction Retrieval" UseSubmitBehavior="false"
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
            var gridPanelHeight = windowHeight * 0.56;
            document.getElementById("<%=PnlGridTransRetStatus.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        //======================= End

        //Fuzzy search filters - Not selected grid
        //Royaltor & Option period
        var txtRoyOptPrd;
        function RoyOptPrdSelected(sender, args) {
            
            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtRoyOptPrd.ClientID %>").value = "";
                document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value = "Y";
            }
        }

        function RoyOptPrdListPopulating() {
            
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
            
            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtCatArtist.ClientID %>").value = "";
                document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value = "Y";
            }
        }

        function CatArtistListPopulating() {
            
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
            
            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtProjCode.ClientID %>").value = "";
                document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnSelToRetSearchSelected.ClientID %>").value = "Y";
            }
        }

        function ProjCodeListPopulating() {
            
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
                    __doPostBack("txtRoyOptPrd", "txtRoyOptPrd_TextChanged");
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
                    __doPostBack("txtCatArtist", "txtCatArtist_TextChanged");
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
                    __doPostBack("txtProjCode", "txtProjCode_TextChanged");
                }
            }
            else {
                if (txtProjCode.value == "") {
                    txtProjCode.style.backgroundImage = 'none';
                }
                return false;
            }
        }

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        //=============== End

    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="6">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    TRANSACTION RETRIEVAL REQUEST STATUS
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td class="identifierLable_large_bold" valign="top" width="10%">Catalogue Search
                    </td>
                    <td colspan="3">
                        <table width="37%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="40%" valign="top">
                                    <asp:TextBox ID="txtRoyOptPrd" runat="server" Width="95%" CssClass="identifierLable"
                                        OnTextChanged="txtRoyOptPrd_TextChanged" AutoPostBack="true" onkeydown="javascript: OntxtRoyOptPrdKeyDown();"
                                        TabIndex="100"></asp:TextBox>
                                    <ajaxToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtRoyOptPrd"
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
                                        TabIndex="101"></asp:TextBox>
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
                                        onkeydown="javascript: OntxtCatNumKeyDown();" TabIndex="102"></asp:TextBox>
                                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmeTxtCatNum" runat="server" TargetControlID="txtCatNum"
                                        WatermarkText="Catalogue No. / Title" WatermarkCssClass="waterMarkText">
                                    </ajaxToolkit:TextBoxWatermarkExtender>
                                </td>
                                <td></td>
                                <td>
                                    <asp:TextBox ID="txtProjCode" runat="server" Width="95%" CssClass="identifierLable"
                                        OnTextChanged="txtProjCode_TextChanged" AutoPostBack="true" onkeydown="javascript: OntxtProjCodeKeyDown();"
                                        TabIndex="103"></asp:TextBox>
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
                        <td width="10%" valign="bottom">
                            <table width="100%">
                                <tr>
                                    <td align="right">
                                        <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="ButtonStyle" OnClick="btnReset_Click" UseSubmitBehavior="false"
                                            TabIndex="104" onkeydown="OnTabPress();" />
                                    </td>
                                    <td width="14%"></td>
                                </tr>
                            </table>
                        </td>
                    </td>
                </tr>
                <tr>
                    <td colspan="6">
                    </td>
                </tr>
                <tr>
                    <td colspan="6">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <table width="98.5%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td width="10%" class="gridHeaderStyle_2rows"
                                                onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                onclick="document.getElementById('<%=btnCatNoSort.ClientID%>').click();">Catalogue no.</td>
                                            <td width="15%" class="gridHeaderStyle_2rows"
                                                onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                onclick="document.getElementById('<%=btnTitleSort.ClientID%>').click();">Title</td>
                                            <td width="15%" class="gridHeaderStyle_2rows"
                                                onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                onclick="document.getElementById('<%=btnArtistSort.ClientID%>').click();">Artist</td>
                                            <td width="13%" class="gridHeaderStyle_2rows" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                onclick="document.getElementById('<%=btnProject.ClientID%>').click();">Project Title</td>
                                            <td width="5%" class="gridHeaderStyle_2rows" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                onclick="document.getElementById('<%=btnConfig.ClientID%>').click();">Config</td>
                                            <td width="8%" class="gridHeaderStyle_2rows" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                onclick="document.getElementById('<%=btnStartDate.ClientID%>').click();">Received Start Date</td>
                                            <td width="8%" class="gridHeaderStyle_2rows" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                onclick="document.getElementById('<%=btnEndDate.ClientID%>').click();">Received End Date</td>
                                            <td width="7%" class="gridHeaderStyle_2rows" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                onclick="document.getElementById('<%=btnReqstedBy.ClientID%>').click();">Requested By</td>
                                            <td width="7%" class="gridHeaderStyle_2rows" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                onclick="document.getElementById('<%=btnRqstedOn.ClientID%>').click();">Requested on</td>
                                            <td width="7%" class="gridHeaderStyle_2rows" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                onclick="document.getElementById('<%=btnTrnxRcvd.ClientID%>').click();">Txns Retrieved</td>
                                            <td width="5%" class="gridHeaderStyle_2rows" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'"
                                                onclick="document.getElementById('<%=btnStatus.ClientID%>').click();">Status</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Panel ID="PnlGridTransRetStatus" runat="server" ScrollBars="Auto" Width="100%" Height="200px">
                                        <asp:GridView ID="gvTransRetStatus" runat="server" AutoGenerateColumns="False" Width="98.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                            CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" ShowHeaderWhenEmpty="true" EmptyDataText="No Data Found"
                                            EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" ShowHeader="false">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Left_Align_Padding">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCatNo" runat="server" Text='<%#Bind("catno")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="15%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTitle" runat="server" Text='<%#Bind("catno_title")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="15%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblArtist" runat="server" Text='<%#Bind("artist_name")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="13%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblProjCode" runat="server" Text='<%#Bind("project_title")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblConfig" runat="server" Text='<%#Bind("config_code")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="8%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRcvStartDate" runat="server" Text='<%#Bind("start_recdate")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="8%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRcvEndDate" runat="server" Text='<%#Bind("end_recdate")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="7%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRqstedBy" runat="server" Text='<%#Bind("user_name")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="7%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRqstedDate" runat="server" Text='<%#Bind("last_modified")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="7%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTrnxRetrieved" runat="server" Text='<%#Bind("txns_retrieved")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStatus" runat="server" Text='<%#Bind("status")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <asp:Button ID="btnCatNoSort" runat="server" Style="display: none;" OnClick="btnCatNoSort_Click" CausesValidation="false" />
                                        <asp:Button ID="btnTitleSort" runat="server" Style="display: none;" OnClick="btnTitleSort_Click" CausesValidation="false" />
                                        <asp:Button ID="btnArtistSort" runat="server" Style="display: none;" OnClick="btnArtistSort_Click" CausesValidation="false" />
                                        <asp:Button ID="btnProject" runat="server" Style="display: none;" OnClick="btnProject_Click" CausesValidation="false" />
                                        <asp:Button ID="btnConfig" runat="server" Style="display: none;" OnClick="btnConfig_Click" CausesValidation="false" />
                                        <asp:Button ID="btnStartDate" runat="server" Style="display: none;" OnClick="btnStartDate_Click" CausesValidation="false" />
                                        <asp:Button ID="btnEndDate" runat="server" Style="display: none;" OnClick="btnEndDate_Click" CausesValidation="false" />
                                        <asp:Button ID="btnReqstedBy" runat="server" Style="display: none;" OnClick="btnReqstedBy_Click" CausesValidation="false" />
                                        <asp:Button ID="btnRqstedOn" runat="server" Style="display: none;" OnClick="btnRqstedOn_Click" CausesValidation="false" />
                                        <asp:Button ID="btnTrnxRcvd" runat="server" Style="display: none;" OnClick="btnTrnxRcvd_Click" CausesValidation="false" />
                                        <asp:Button ID="btnStatus" runat="server" Style="display: none;" OnClick="btnStatus_Click" CausesValidation="false" />
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
                                <td class="identifierLable">Please Wait... </td>
                            </tr>
                        </table>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <ajaxToolkit:ModalPopupExtender ID="mPopupPageLevel" runat="server" BackgroundCssClass="progressBar" PopupControlID="progressBarPageLevel" RepositionMode="RepositionOnWindowResize" TargetControlID="progressBarPageLevel">
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

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnSelGridFilterSelected" runat="server" />
            <asp:HiddenField ID="hdnSelToRetSearchSelected" runat="server" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" Value="" />
            <asp:HiddenField ID="hdnGridSortDir" runat="server" />
            <asp:HiddenField ID="hdnGridSortColumn" runat="server" />
            <asp:Button ID="btnCatNumNotSelecSearch" runat="server" Style="display: none;" OnClick="btnCatNumNotSelecSearch_Click" CausesValidation="false" />
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:TextBox>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyAudit.aspx.cs" Inherits="WARS.CompanyAudit" MasterPageFile="~/MasterPage.Master"
    Title="WARS - CompanyAudit " MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<%--<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //to open Configuration search screen in same tab
        function OpenCompanyMaintScreen() {
            var win = window.open('CompanyMaintenance.aspx', '_self');
            win.focus();
        }

    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnCompanyMaint" runat="server" Text="Company Maintenance" CssClass="LinkButtonStyle" 
                            width="98%"  UseSubmitBehavior="false" OnClick="btnCompanyMaint_Click" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>--%>

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
            if (postBackElementID.lastIndexOf('gvCompanyDetails') > 0 || postBackElementID.lastIndexOf('btnSaveChanges') > 0) {
                xPos = sender._scrollPosition.x;
                yPos = sender._scrollPosition.y;

                //hold scroll position on selecting expand/collapse
                var PnlCompanyDetails = document.getElementById("<%=PnlCompanyDetails.ClientID %>");
                scrollTop = PnlCompanyDetails.scrollTop;

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
            if (postBackElementID.lastIndexOf('gvCompanyDetails') > 0 || postBackElementID.lastIndexOf('btnSaveChanges') > 0) {
                window.scrollTo(xPos, yPos);

                //set scroll position on selecting expand/collapse
                var PnlCompanyDetails = document.getElementById("<%=PnlCompanyDetails.ClientID %>");
                PnlCompanyDetails.scrollTop = scrollTop;
            }

        }
        //======================= End


        //Fuzzy search filters
        var txtCompanySearch;
        function CompanySelected(sender, args) {

            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtCompanySearch.ClientID %>").value = "";
            }
            else {
                document.getElementById('<%=btnHdnCompanySearch.ClientID%>').click();
            }
        }

        function CompanyListPopulating() {
            txtCompanySearch = document.getElementById("<%= txtCompanySearch.ClientID %>");
            txtCompanySearch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtCompanySearch.style.backgroundRepeat = 'no-repeat';
            txtCompanySearch.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidSearch.ClientID %>").value = "N";
        }

        function CompanyListPopulated() {
            txtCompanySearch = document.getElementById("<%= txtCompanySearch.ClientID %>");
            txtCompanySearch.style.backgroundImage = 'none';
        }

        //=============== End



        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
            document.getElementById("<%=PnlCompanyDetails.ClientID %>").style.height = gridPanelHeight + "px";
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
    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="7">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    COMPANY AUDIT
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="8%"></td>
                    <td width="5%" class="identifierLable_large_bold">Company</td>
                    <td width="24%">
                        <asp:TextBox ID="txtCompanySearch" runat="server" Width="99%" CssClass="textboxStyle"
                            TabIndex="100" onkeydown="OnTabPress();"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="aceCompanySearch" runat="server"
                            ServiceMethod="FuzzySearchAllCompanyList"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtCompanySearch"
                            FirstRowSelected="true"
                            OnClientItemSelected="CompanySelected"
                            OnClientPopulating="CompanyListPopulating"
                            OnClientPopulated="CompanyListPopulated"
                            OnClientHidden="CompanyListPopulated"
                            CompletionListElementID="acePnlCompany" />
                        <asp:Panel ID="acePnlCompany" runat="server" CssClass="identifierLable" />
                    </td>
                    <td width="3%" align="left">
                        <asp:ImageButton ID="fuzzySearchCompany" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button"
                            OnClick="fuzzySearchCompany_Click" ToolTip="Search company code/name" />
                    </td>
                    <td></td>
                    <td width="3%"></td>
                    <td align="right" width="12%">
                        <asp:Button ID="btnCompanyMaint" runat="server" Text="Company Maintenance" CssClass="ButtonStyle"
                            Width="98%" UseSubmitBehavior="false" OnClick="btnCompanyMaint_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="7">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="5" class="table_with_border" runat="server" id="tdData">
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
                                                <%--<table width="97%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="20%" class="gridHeaderStyle_1row">Label</td>
                                                        <td width="70%" class="gridHeaderStyle_1row">Description</td>
                                                        <td width="10%" class="gridHeaderStyle_1row">&nbsp</td>
                                                    </tr>
                                                </table>--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Panel ID="PnlCompanyDetails" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvCompanyDetails" runat="server" AutoGenerateColumns="False" Width="98%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." ShowHeader="False">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <table width="100%" cellspacing="0">
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" width="7%" align="left">Company</td>
                                                                            <td width="18%" align="left">
                                                                                <asp:Label ID="lblCompanyName" runat="server" Text='<%# Bind("company_name") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td width="12%"></td>
                                                                            <td width="10%"></td>
                                                                            <td width="20%"></td>
                                                                            <td width="5%"></td>
                                                                            <td width="10%"></td>
                                                                            <td width="18%"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" width="7%" align="left">Description</td>
                                                                            <td width="18%" align="left">
                                                                                <asp:Label ID="lblCompanyDesc" runat="server" Text='<%# Bind("company_desc") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Primary Company</td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblPrimary" runat="server" Text='<%# Bind("primary") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Display VAT number</td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblDisplayaVat" runat="server" Text='<%# Bind("display_vat") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>

                                                                            <td class="identifierLable_large_bold" align="left">Updated By</td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblUserCode" runat="server" Text='<%# Bind("user_code") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Address</td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblCompanyAdd1" runat="server" Text='<%# Bind("company_add1") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Account Company</td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblAccountCompany" runat="server" Text='<%# Bind("account_company") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Domestic Currency Grouping</td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblDomCurrencyGroup" runat="server" Text='<%# Bind("domestic_currency_group") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>

                                                                            <td class="identifierLable_large_bold" align="left">Updated On</td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblLastModified" runat="server" Text='<%# Bind("last_modified") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td></td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblCompanyAdd2" runat="server" Text='<%# Bind("company_add2") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Currency</td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblCurrency" runat="server" Text='<%# Bind("currency_code") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                            <td></td>
                                                                            <td></td>
                                                                            <td></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td></td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblCompanyAdd3" runat="server" Text='<%# Bind("company_add3") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left" valign="top">Threshold values:</td>
                                                                            <td colspan="3" align="left">
                                                                                <table width="100%" cellpadding="0" cellspacing="0">
                                                                                    <tr>
                                                                                        <td class="identifierLable_large_bold" align="left" width="50%">Recouped Statements</td>
                                                                                        <td width="20%">
                                                                                            <asp:Label ID="lblThresholdRecouped" runat="server" Text='<%# Bind("threshold_recouped") %>' CssClass="identifierLable"></asp:Label>
                                                                                        </td>
                                                                                        <td width="30%"></td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                            <td></td>
                                                                            <td></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td></td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblCompanyAdd4" runat="server" Text='<%# Bind("company_add4") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                            <td colspan="3" align="left">
                                                                                <table width="100%" cellpadding="0" cellspacing="0">
                                                                                    <tr>
                                                                                        <td class="identifierLable_large_bold" align="left" width="50%">Unrecouped Statements</td>
                                                                                        <td width="20%">
                                                                                            <asp:Label ID="lblThresholdUnrecouped" runat="server" Text='<%# Bind("threshold_unrecouped") %>' CssClass="identifierLable"></asp:Label>
                                                                                        </td>
                                                                                        <td width="30%"></td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                            <td></td>
                                                                            <td></td>
                                                                        </tr>
                                                                    </table>

                                                                </ItemTemplate>
                                                                <ItemStyle Width="95%" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
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
                                                <br />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td></td>
                            </tr>
                        </table>
                    </td>
                    <td colspan="2"></td>
                </tr>
            </table>

            <asp:Button ID="dummyFuzzySearch" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeFuzzySearch" runat="server" PopupControlID="pnlFuzzySearch" TargetControlID="dummyFuzzySearch"
                BackgroundCssClass="popupBox" CancelControlID="btnCloseFuzzySearchPopup">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlFuzzySearch" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
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
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnIsValidSearch" runat="server" Value="N" />
            <asp:Button ID="btnHdnCompanySearch" runat="server" Style="display: none;" OnClick="btnHdnCompanySearch_Click" CausesValidation="false" />
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:TextBox>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

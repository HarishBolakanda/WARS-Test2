<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="ParticipantAudit.aspx.cs" Inherits="WARS.Audit.ParticipantAudit" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Participant Audit Details" ValidateRequest="false" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

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


        function OnDateKeyDown() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnHdnCatalogue.ClientID%>').click();
            }
        }

        var txtCatSrch;

        function catalogueListPopulating() {
            txtCatSrch = document.getElementById("<%= txtCatalogueSearch.ClientID %>");
            txtCatSrch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtCatSrch.style.backgroundRepeat = 'no-repeat';
            txtCatSrch.style.backgroundPosition = 'right';
        }

        function catalogueListPopulated() {
            txtCatSrch = document.getElementById("<%= txtCatalogueSearch.ClientID %>");
            txtCatSrch.style.backgroundImage = 'none';
        }

        function resetScrollPosition(sender, args) {
            var autoCompPnl = document.getElementById("<%= autocompleteDropDownPanel1.ClientID %>");
            autoCompPnl.scrollTop = 1;

        }

        function catalogueListItemSelected(sender, args) {
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                document.getElementById("<%= txtCatalogueSearch.ClientID %>").value = "";
                document.getElementById('<%=btnHdnCatalogue.ClientID%>').click();
            }
            else {
                document.getElementById('<%=btnHdnCatalogue.ClientID%>').click();
            }
        }

        function OnCatalogueKeyDown() {

        }
        function OnCatalogueKeyUp() {
            if (document.getElementById('<%=txtCatalogueSearch.ClientID%>')) {
                var textBox = document.getElementById('<%=txtCatalogueSearch.ClientID%>').value.length;
                if (textBox == 0) {
                    document.getElementById("<%=txtCatalogueSearch.ClientID %>").value = "";
                    document.getElementById('<%=tdData.ClientID%>').style.display = "none";
                    document.getElementById('<%=trParAudit.ClientID%>').style.display = "none";
                }
            }
        }
        

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        function MoveCatNoFocus() {
            document.getElementById("<%= lblTab.ClientID %>").focus();
        }

        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.6;
            document.getElementById("<%=PnlParticipantDetailsAudit.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }
    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="10">
                        <asp:Panel ID="PnlScreenName" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    PARTICIPANT AUDIT DETAILS
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="10"></td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="6%" class="identifierLable_large_bold">Catalogue No.</td>
                    <td colspan="3" style="padding: 2px; padding-right: 4px;">
                        <asp:TextBox ID="txtCatalogueSearch" runat="server" Width="99%" CssClass="textboxStyle"
                            onkeyup="javascript: OnCatalogueKeyUp();" onkeydown="javascript: OnCatalogueKeyDown();" TabIndex="100"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="aceCatalogueList" runat="server"
                            ServiceMethod="FuzzySearchAllCatalogueList"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtCatalogueSearch"
                            FirstRowSelected="true"
                            OnClientPopulating="catalogueListPopulating"
                            OnClientPopulated="catalogueListPopulated"
                            OnClientHidden="catalogueListPopulated"
                            OnClientShown="resetScrollPosition"
                            OnClientItemSelected="catalogueListItemSelected"
                            CompletionListElementID="autocompleteDropDownPanel1" />
                        <asp:Panel ID="autocompleteDropDownPanel1" runat="server" CssClass="identifierLable" />
                    </td>
                    <td width="3%" align="left">
                        <asp:CustomValidator ID="valCatalogue" runat="server" ValidationGroup="valSearch" CssClass="requiredFieldValidator"
                            ToolTip="Not a valid Royaltor. Please select from the search list." Display="Dynamic"
                            ErrorMessage="*"></asp:CustomValidator>
                        <asp:ImageButton ID="fuzzySearchCatalogue" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                            OnClick="fuzzySearchCatalogue_Click" TabIndex="101" ToolTip="Search Catalogue" CssClass="FuzzySearch_Button" />
                    </td>
                    <td></td>
                    <td></td>
                    <td align="right" colspan="2">
                        <asp:Button ID="btnParticipant" runat="server" Text="Participant Details" CssClass="ButtonStyle"
                            UseSubmitBehavior="false" TabIndex="104" OnClick="btnParticipantDetails_Click" onkeydown="OnTabPress();" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">From Date</td>
                    <td width="8%" style="padding: 2px">
                        <asp:TextBox ID="txtFromDate" runat="server" Width="65" CssClass="identifierLable"
                            onkeydown="javascript: OnDateKeyDown();" ValidationGroup="valSave" TabIndex="102"></asp:TextBox>
                        <ajaxToolkit:MaskedEditExtender ID="mteFromDate" runat="server"
                            TargetControlID="txtFromDate" Mask="99/99/9999" AcceptNegative="None"
                            ClearMaskOnLostFocus="false" />
                        <asp:CustomValidator ID="valFromDate" runat="server" ValidationGroup="valSearch" CssClass="requiredFieldValidator"
                            OnServerValidate="valFromDate_ServerValidate" ErrorMessage="*" ToolTip="Please enter a valid date in DD/MM/YYYY format"></asp:CustomValidator>
                    </td>
                    <td class="identifierLable_large_bold" width="4%" align="right">To Date</td>
                    <td width="6%" align="right" style="padding: 2px;">
                        <asp:TextBox ID="txtToDate" runat="server" Width="65" CssClass="identifierLable"
                            onkeydown="javascript: OnDateKeyDown();" ValidationGroup="valSave" TabIndex="103"></asp:TextBox>
                        <ajaxToolkit:MaskedEditExtender ID="mteToDate" runat="server"
                            TargetControlID="txtToDate" Mask="99/99/9999" AcceptNegative="None"
                            ClearMaskOnLostFocus="false" />
                    </td>
                    <td>
                        <asp:CustomValidator ID="valToDate" runat="server" ValidationGroup="valSearch" CssClass="requiredFieldValidator"
                            OnServerValidate="valToDate_ServerValidate" ErrorMessage="*" ToolTip="Please enter a valid date in DD/MM/YYYY format"></asp:CustomValidator>
                    </td>
                    <td width="10%"></td>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="10"></td>

                </tr>
                <tr>
                    <td></td>
                    <td colspan="8" class="table_with_border" runat="server" id="tdData">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="2%">
                                    <br />
                                </td>
                                <td width="97%"></td>
                                <td width="1%"></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td></td>
                                        </tr>
                                        <tr runat="server" id="trParAudit">
                                            <td>
                                                <asp:Panel ID="PnlParticipantDetailsAudit" runat="server" ScrollBars="Auto" Width="100%">
                                                    <asp:GridView ID="gvPartAudit" runat="server" AutoGenerateColumns="False" Width="98.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                        OnRowDataBound="gvPartAudit_RowDataBound" CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                        EmptyDataText="No data found." ShowHeader="False">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                <ItemTemplate>
                                                                    <table width="100%" cellspacing="0">
                                                                        <tr>
                                                                            <td width="12%"></td>
                                                                            <td width="20%"></td>
                                                                            <td width="13%"></td>
                                                                            <td width="15%"></td>
                                                                            <td width="13%"></td>
                                                                            <td width="8%"></td>
                                                                            <td width="9%"></td>
                                                                            <td width="10%"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Royaltor                                                                            
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblRoyaltor" runat="server" Text='<%# Bind("royaltor") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">OptionPeriod
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblOptionPeriod" runat="server" Text='<%# Bind("option_period") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Active
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblIsActive" runat="server" Text='<%# Bind("is_active") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">
                                                                                <asp:Label ID="lblUserCodeHdr" runat="server" Text="Updated by" CssClass="identifierLable_large_bold"></asp:Label>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblUserCode" runat="server" Text='<%# Bind("user_code") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Territory
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblTerritory" runat="server" Text='<%# Bind("territory") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Escalation Code
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblEscCode" runat="server" Text='<%# Bind("esc_code") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Escalation Include Units
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblIncludeEscalation" runat="server" Text='<%# Bind("inc_in_escalation") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">
                                                                                <asp:Label ID="lblUpdatedOnHdr" runat="server" Text="Updated on" CssClass="identifierLable_large_bold"></asp:Label>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblUpdatedOn" runat="server" Text='<%# Bind("last_modified") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Track Share
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblShareTracks" runat="server" Text='<%# Bind("share_tracks") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Total Tracks
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblShareTotalTracks" runat="server" Text='<%# Bind("share_total_tracks") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Track Title
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblTrackTitle" runat="server" Text='<%# Bind("track_title") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="identifierLable_large_bold" align="left">Time Share
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblShareTime" runat="server" Text='<%# Bind("share_time") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Total Time
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblShareTotalTime" runat="server" Text='<%# Bind("share_total_time") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td class="identifierLable_large_bold" align="left">Status
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("status_desc") %>' CssClass="identifierLable"></asp:Label>
                                                                            </td>
                                                                            <td></td>
                                                                            <td></td>
                                                                            <td></td>
                                                                        </tr>
                                                                    </table>
                                                                    <asp:HiddenField ID="hdnDisplayOrder" runat="server" Value='<%# Bind("display_order") %>' />
                                                                    <asp:HiddenField ID="hdnParticipationId" runat="server" Value='<%# Bind("participation_id") %>' />
                                                                    <asp:HiddenField ID="hdnRoyaltorId" runat="server" Value='<%# Bind("royaltor_id") %>' />
                                                                    <asp:HiddenField ID="hdnOptionPeriodCode" runat="server" Value='<%# Bind("option_period_code") %>' />
                                                                    <asp:HiddenField ID="hdnSellerGroupCode" runat="server" Value='<%# Bind("seller_group_code") %>' />
                                                                    <asp:HiddenField ID="hdnTuneId" runat="server" Value='<%# Bind("tune_id") %>' />
                                                                    <asp:HiddenField ID="hdnParticipationType" runat="server" Value='<%# Bind("participation_type") %>' />
                                                                </ItemTemplate>
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
                                    </table>
                                </td>
                                <td></td>
                            </tr>
                        </table>
                    </td>
                    <td></td>
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
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:Label ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:Label>
            <asp:Button ID="btnHdnCatalogue" runat="server" Style="display: none;" OnClick="btnHdnCatalogue_Click" CausesValidation="false" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>



<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NotesOverview.aspx.cs" Inherits="WARS.NotesOverview.NotesOverview" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Notes Overview" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>


<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderBody">
    <script type="text/javascript">

        //progress bar and scroll position functionality - starts
        //to remain scroll position of grid panel and window

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

        //Royaltor auto populate search functionalities

        function royaltorListPopulating() {
            txtRoy = document.getElementById("<%= txtRoyaltor.ClientID %>");
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';
        }

        function royaltorListPopulated() {
            txtRoy = document.getElementById("<%= txtRoyaltor.ClientID %>");
            txtRoy.style.backgroundImage = 'none';
        }

        function royaltorListItemSelected(sender, args) {
            var roySrchVal = args.get_value();

            if (roySrchVal == 'No results found') {
                document.getElementById("<%= txtRoyaltor.ClientID %>").value = "";
            }
        }

        //Validate if the field value is a valid one from fuzzy search list
        function ValRoyaltorSearch(sender, args) {
            txtRoyaltor = document.getElementById("<%=txtRoyaltor.ClientID %>");

            if (txtRoyaltor.value == "") {
                args.IsValid = false;
            }
            else if (txtRoyaltor.value == "No results found") {
                args.IsValid = false;
            }
            else if (txtRoyaltor.value != "" && txtRoyaltor.value.indexOf('-') == -1) {
                args.IsValid = false;
            }
            else {
                args.IsValid = true;
            }

        }

        //Pop up fuzzy search list       
        function OntxtRoyaltorKeyDown(sender) {
            if ((event.keyCode == 13)) {
                var aceRoyaltorAddRow = $find('ContentPlaceHolderBody_' + 'aceRoyaltorAddRow');
                if (aceRoyaltorAddRow._selectIndex == -1) {
                    //Enter key can be used to select the dropdown list item or to pop up the complete list
                    //to know this, check if list item is selected or not
                    document.getElementById('<%=btnFuzzyRoyaltorListPopup.ClientID%>').click();
                }
            }
        }

        //grid panel height adjustment functioanlity - starts
        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").value = gridPanelHeight;
            document.getElementById("<%=txtNotes.ClientID %>").style.height = gridPanelHeight + "px";
        }

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= txtRoyaltor.ClientID %>").focus();
            }
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
                                    Notes Overview Screen
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="10%" align="center" class="identifierLable_large_bold">Royaltor Search</td>
                    <td>
                        <asp:TextBox ID="txtRoyaltor" runat="server" Width="20%" CssClass="textboxStyle"
                            TabIndex="100" onkeydown="OntxtRoyaltorKeyDown(this);"> </asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="aceRoyaltorAddRow" runat="server"
                            ServiceMethod="FuzzySearchAllRoyaltorList"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtRoyaltor"
                            FirstRowSelected="true"
                            OnClientPopulating="royaltorListPopulating"
                            OnClientPopulated="royaltorListPopulated"
                            OnClientHidden="royaltorListPopulated"
                            OnClientItemSelected="royaltorListItemSelected"
                            CompletionListElementID="pnlRoyFuzzySearchInsert" />
                        <asp:Panel ID="pnlRoyFuzzySearchInsert" runat="server" CssClass="identifierLable" />
                        <asp:CustomValidator ID="valtxtRoyaltorSearch" runat="server" CssClass="requiredFieldValidator"
                            ToolTip="Please select valid royaltor from the search list" ValidationGroup="valGrpRoySearch"
                            ControlToValidate="txtRoyaltor" ErrorMessage="*" Display="Dynamic" ClientValidationFunction="ValRoyaltorSearch"></asp:CustomValidator>
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtRoyaltor"
                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please select valid royaltor from the search list" InitialValue="" Display="Dynamic" ValidationGroup="valGrpRoySearch">
                        </asp:RequiredFieldValidator>
                    </td>
                    <td align="center" width="10%">
                        <asp:Button ID="btnReset" runat="server" OnClick="btnReset_Click" CssClass="ButtonStyle"
                            UseSubmitBehavior="false" Width="80%" Text="Reset" TabIndex="101" />
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="2">
                        <table runat="server" class="table_with_border" width="90%">
                            <tr>
                                <td width="10%"></td>
                                <td width="70%" class="table_header_with_border">Notes</td>
                            </tr>
                            <tr>
                                <td valign="top" align="center">
                                    <table width="100%">
                                        <tr>
                                            <td align="center">
                                                <asp:Button ID="btnWorkFlow" align="right" runat="server" CssClass="ButtonStyle" OnClick="btnWorkFlow_Click" TabIndex="102" Text="Work Flow"
                                                    UseSubmitBehavior="false" Width="80%" ValidationGroup="valGrpRoySearch" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:Button ID="btnContract" runat="server" CssClass="ButtonStyle" OnClick="btnContract_Click" TabIndex="103" Text="Contract" UseSubmitBehavior="false"
                                                    Width="80%" ValidationGroup="valGrpRoySearch" onkeydown="OnTabPress();" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td valign="top" class="table_header_with_border">
                                                <asp:TextBox ID="txtNotes" runat="server" CssClass="textboxStyle" ReadOnly="true" TextMode="MultiLine" Width="97%"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <br />
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


            <%--Fuzzy search pop up - starts --%>
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
                                    <td>
                                        <asp:Label ID="lblFuzzySearchPopUp" runat="server" Text="Complete Search List" CssClass="identifierLable"></asp:Label>
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
            <%--Fuzzy search pop up - Ends --%>

            <%--A Pop up to select Reporting Schedule for WorkFlow button- starts--%>
            <asp:Button ID="dummyReportingSchedule" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeReportingSchedule" runat="server" PopupControlID="pnlPopup" TargetControlID="dummyReportingSchedule"
                CancelControlID="btnCancel" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlPopup" runat="server" align="center" Width="30%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblReportingSchedule" runat="server" Width="100%" Text="Reporting Schedule" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="80%">
                                <tr>
                                    <td width="40%" class="identifierLable_large_bold">Reporting Schedule</td>
                                    <td width="5%"></td>
                                    <td width="50%" align="right">
                                        <asp:DropDownList ID="ddlReportingSchedule" runat="server" CssClass="ddlStyle" Width="100%"></asp:DropDownList>
                                    </td>
                                    <td width="5%" align="left">
                                        <asp:RequiredFieldValidator runat="server" ID="rfvddlReportingSchedule" ControlToValidate="ddlReportingSchedule"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please select Reporting Schedule" InitialValue="-" Display="Dynamic"
                                            ValidationGroup="valGrpReportingSchedule"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr align="center">
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnWorkflowProceed" OnClick="btnWorkflowProceed_click" runat="server" Text="Proceed" CssClass="ButtonStyle"
                                            ValidationGroup="valGrpReportingSchedule" />
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
            <%--A Pop up to select Reporting Schedule for WorkFlow button- ends--%>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:Button ID="btnFuzzyRoyaltorListPopup" runat="server" Style="display: none;" OnClick="btnFuzzyRoyaltorListPopup_Click" CausesValidation="false" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

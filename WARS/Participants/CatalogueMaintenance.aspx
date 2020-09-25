<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CatalogueMaintenance.aspx.cs" Inherits="WARS.CatalogueMaintenance" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Catalogue Maintenance " MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //to open Configuration group screen in same tab
        function OpenContractMaintenance() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../Contract/RoyaltorSearch.aspx');

            }
            else {
                var win = window.open('../Contract/RoyaltorSearch.aspx', '_self');
                win.focus();
                return true;
            }

        }
        //================================End


    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnContractMaintenance" runat="server" Text="Contract Maintenance"
                            CssClass="LinkButtonStyle" Width="98%" OnClientClick="if (!OpenContractMaintenance()) { return false;};" UseSubmitBehavior="false" />
                    </td>
                </tr>
                <tr>
                    <td align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnCatalogueSearch" runat="server" CssClass="LinkButtonStyle" OnClick="btnCatalogueSearch_Click" Text="Catalogue Search" Width="98%"
                            UseSubmitBehavior="false" OnKeyDown="OnTabPress()" OnClientClick="if (!OpenCatalogueSearch()) { return false;};" />
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
        //============probress bar and scroll position functionality - Ends


        //Fuzzy search filters - Start
        var txtArtistSearch;
        function ArtistSelected(sender, args) {
            //WUIN-540
            var hdnUserRole = document.getElementById("<%=hdnUserRole.ClientID %>").value;
            var hdnStatusCode = document.getElementById("<%=hdnStatusCode.ClientID %>").value;

            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtArtist.ClientID %>").value = "";

            }
            else {
                document.getElementById("<%= hdnIsValidArtist.ClientID %>").value = "Y";
                document.getElementById('<%=txtArtist.ClientID%>').click();
            }
        }

        function ArtistListPopulating() {
            txtArtistSearch = document.getElementById("<%= txtArtist.ClientID %>");
            txtArtistSearch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtArtistSearch.style.backgroundRepeat = 'no-repeat';
            txtArtistSearch.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidArtist.ClientID %>").value = "N";
        }

        function ArtistListPopulated() {
            txtArtistSearch = document.getElementById("<%= txtArtist.ClientID %>");
            txtArtistSearch.style.backgroundImage = 'none';
        }

        //----------------//

        var txtProjectSearch;
        function ProjectSelected(sender, args) {
            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtProject.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnIsValidProject.ClientID %>").value = "Y";
                document.getElementById('<%=txtProject.ClientID%>').click();
            }
        }

        function ProjectListPopulating() {
            txtProjectSearch = document.getElementById("<%= txtProject.ClientID %>");
            txtProjectSearch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtProjectSearch.style.backgroundRepeat = 'no-repeat';
            txtProjectSearch.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidProject.ClientID %>").value = "N";
        }

        function ProjectListPopulated() {
            txtProjectSearch = document.getElementById("<%= txtProject.ClientID %>");
            txtProjectSearch.style.backgroundImage = 'none';
        }

        //----------------//

        var txtExceptionRateProjectSearch;
        function ExceptionRateProjectSelected(sender, args) {
            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtExceptionRateProject.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnIsValidExcRateProject.ClientID %>").value = "Y";
                document.getElementById('<%=txtExceptionRateProject.ClientID%>').click();
            }
        }

        function ExceptionRateProjectListPopulating() {
            txtExceptionRateProjectSearch = document.getElementById("<%= txtExceptionRateProject.ClientID %>");
            txtExceptionRateProjectSearch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtExceptionRateProjectSearch.style.backgroundRepeat = 'no-repeat';
            txtExceptionRateProjectSearch.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidExcRateProject.ClientID %>").value = "N";
        }

        function ExceptionRateProjectListPopulated() {
            txtExceptionRateProjectSearch = document.getElementById("<%= txtExceptionRateProject.ClientID %>");
            txtExceptionRateProjectSearch.style.backgroundImage = 'none';
        }

        //=============== //Fuzzy search filters - End

        //Validations - Start

        //WUIN-698 - Status update validations:
        //1.No update allowed if (legacy = 'Y' and no active Participants (PARTICIPATION_TYPE = 'A' and end_date null))  
        //                    OR (legacy = 'N' and no active Track Participants(ISRC participants))
        //2. Allow maintenance of Status only If CATNO.LEGACY = 'Y' or (CATNO.LEGACY = 'N' and no Track Listing) 
        //3. Only SuperUser can change status to Manager Sign Off (3) 
        //   WUIN-1167 Any User can change status from Managaer Sing off (3)
        //4. If Status is moved from Manager Sign Off (3) then 						
        //      display warning 'This update will prevent the generation of Statement details for all Participants'	
        //5. For new catalogue. Status cannot be other than 'No Participants'
        //6. (WUIN-909) Cannot change status to 'No Participants' if there are active participants
        //      if legacy = 'Y' and active Participants (PARTICIPATION_TYPE = 'A' and end_date null)
        //      if legacy = 'N' and active Track Participants(ISRC participants)
        function ValidateStatus() {
            var hdnIsNewCatNo = document.getElementById("<%=hdnIsNewCatNo.ClientID %>").value;
            var hdnUserRole = document.getElementById("<%=hdnUserRole.ClientID %>").value;
            var hdnStatusCode = document.getElementById("<%=hdnStatusCode.ClientID %>").value;
            var hdnIsEditable = document.getElementById("<%=hdnIsEditable.ClientID %>").value;
            var ddlCatStatus = document.getElementById("<%=ddlCatStatus.ClientID %>");
            var isValid = true;

            //validation for new participant
            if (hdnIsNewCatNo == "Y" && ddlCatStatus.value != "0") {
                DisplayMessagePopup("Status should be 'No Participants' for a new catalogue!");
                //reset ddlCatStatus value
                for (var i = 0; i < ddlCatStatus.options.length; i++) {
                    if (ddlCatStatus.options[i].value == "0") {
                        ddlCatStatus.selectedIndex = i;
                        break;
                    }
                }

                //clear validator
                ValidatorValidate(document.getElementById("<%=rfvddlCatStatus.ClientID %>"));

                return false;
            }

            if (hdnIsEditable == "N") {
                DisplayMessagePopup("As there are no active participants, status cannot be changed!");
                isValid = false;
            }
            //JIRA-983 Changes by Ravi on 20/02/2019 -- Start
            if (hdnUserRole != "SuperUser" && hdnUserRole != "Supervisor" && hdnStatusCode != "3" && ddlCatStatus.value == "3") {
                DisplayMessagePopup("Only super user and Supervisor can change the status to 'Manager Sign Off'!");
                isValid = false;
            }
             //JIRA-983 Changes by Ravi on 20/02/2019 -- End
            else if (hdnIsEditable == "Y" && ddlCatStatus.value == "0") {
                //WUIN-909
                DisplayMessagePopup("As there are active participants, status cannot be set to 'No Participants'!");
                isValid = false;
            }
            else if (hdnStatusCode == "3" && ddlCatStatus.value != "3" && ddlCatStatus.selectedIndex != "0") {
                DisplayMessagePopup("This update will prevent the generation of Statement details for all Participants");
            }

            if (!isValid) {
                //reset ddlCatStatus value
                for (var i = 0; i < ddlCatStatus.options.length; i++) {
                    if (ddlCatStatus.options[i].value == hdnStatusCode) {
                        ddlCatStatus.selectedIndex = i;
                        break;
                    }
                }
            }
            else {
                if (hdnStatusCode != document.getElementById("<%=ddlCatStatus.ClientID %>").value) {
                    document.getElementById("<%=hdnIsStatusChange.ClientID %>").value = "Y";
                }
            }

        }

        //Compares control data with initial value
        //returns true if no changes else returns false
        function CompareData() {
            var txtCatalogueNumber = document.getElementById("<%=txtCatalogueNumber.ClientID %>").value;
            var hdnCatalogueNumber = document.getElementById("<%=hdnCatalogueNumber.ClientID %>").value;

            var txtTitle = document.getElementById("<%=txtTitle.ClientID %>").value;
            var hdnTitle = document.getElementById("<%=hdnTitle.ClientID %>").value;

            var ddlConfiguration = document.getElementById("<%=ddlConfiguration.ClientID %>").value;
            var hdnConfiguration = document.getElementById("<%=hdnConfiguration.ClientID %>").value;

            var txtArtist = document.getElementById("<%=txtArtist.ClientID %>").value;
            var hdnArtist = document.getElementById("<%=hdnArtist.ClientID %>").value;

            var txtProject = document.getElementById("<%=txtProject.ClientID %>").value;
            var hdnProject = document.getElementById("<%=hdnProject.ClientID %>").value;

            var txtExceptionRateProject = document.getElementById("<%=txtExceptionRateProject.ClientID %>").value;
            var hdnExceptionRateProject = document.getElementById("<%=hdnExceptionRateProject.ClientID %>").value;

            var ddlMurOwner = document.getElementById("<%=ddlMurOwner.ClientID %>").value;
            var hdnMurOwner = document.getElementById("<%=hdnMurOwner.ClientID %>").value;

            var txtTotalTracks = document.getElementById("<%=txtTotalTracks.ClientID %>").value;
            var hdnTotalTracks = document.getElementById("<%=hdnTotalTracks.ClientID %>").value;

            var txtUnlistedComponents = document.getElementById("<%=txtUnlistedComponents.ClientID %>").value;
            var hdnUnlistedComponents = document.getElementById("<%=hdnUnlistedComponents.ClientID %>").value;

            var txtTotalPlayLength = document.getElementById("<%=txtTotalPlayLength.ClientID %>").value;
            var hdnTotalPlayLength = document.getElementById("<%=hdnTotalPlayLength.ClientID %>").value;

            var ddlTimeTrackShare = document.getElementById("<%=ddlTimeTrackShare.ClientID %>").value;
            var hdnTimeTrackShare = document.getElementById("<%=hdnTimeTrackShare.ClientID %>").value;


            var hdnCompilation = document.getElementById("<%=hdnCompilation.ClientID %>").value;
            var isCompilation;
            if (document.getElementById("<%=cbCompilation.ClientID %>").checked == true) {
                isCompilation = "Y";
            }
            else {
                isCompilation = "N";
            }

            var hdnLicensedOut = document.getElementById("<%=hdnLicensedOut.ClientID %>").value;
            var isLicensedOut;
            if (document.getElementById("<%=cbLicensedOut.ClientID %>").checked == true) {
                isLicensedOut = "Y";
            }
            else {
                isLicensedOut = "N";
            }

            var hdnLegacy = document.getElementById("<%=hdnLegacy.ClientID %>").value;
            var isLegacy;
            if (document.getElementById("<%=cbLegacy.ClientID %>").checked == true) {
                isLegacy = "Y";
            }
            else {
                isLegacy = "N";
            }

            var ddlCatStatus = document.getElementById("<%=ddlCatStatus.ClientID %>").value;
            var hdnStatusCode = document.getElementById("<%=hdnStatusCode.ClientID %>").value;

            var txtMarketingOwner = document.getElementById("<%=txtMarketingOwner.ClientID %>").value;
            var hdnMarketingOwner = document.getElementById("<%=hdnMarketingOwner.ClientID %>").value;

            var txtFirstSaleDate = document.getElementById("<%=txtFirstSaleDate.ClientID %>").value;
            if (txtFirstSaleDate == "DD/MM/YYYY") {
                txtFirstSaleDate = "";
            }
            var hdnFirstSaleDate = document.getElementById("<%=hdnFirstSaleDate.ClientID %>").value;

            var txtWeaSaelsLabel = document.getElementById("<%=txtWeaSaelsLabel.ClientID %>").value;
            var hdnWeaSalesLabel = document.getElementById("<%=hdnWeaSalesLabel.ClientID %>").value;

            var txtLabel = document.getElementById("<%=txtLabel.ClientID %>").value;
            var hdnLabel = document.getElementById("<%=hdnLabel.ClientID %>").value;

            if (txtCatalogueNumber != hdnCatalogueNumber || txtTitle != hdnTitle || ddlConfiguration != hdnConfiguration || txtArtist != hdnArtist
                || txtProject != hdnProject || txtExceptionRateProject != hdnExceptionRateProject || ddlMurOwner != hdnMurOwner || txtTotalTracks != hdnTotalTracks
                || txtUnlistedComponents != hdnUnlistedComponents || txtTotalPlayLength != hdnTotalPlayLength || ddlTimeTrackShare != hdnTimeTrackShare
                || isCompilation != hdnCompilation || isLicensedOut != hdnLicensedOut || isLegacy != hdnLegacy || ddlCatStatus != hdnStatusCode || txtMarketingOwner != hdnMarketingOwner
                || txtFirstSaleDate != hdnFirstSaleDate || txtWeaSaelsLabel != hdnWeaSalesLabel || txtLabel != hdnLabel) {
                return false;
            }
            else {
                return true;
            }

        }

        //Show warning while closing the window if changed data not saved 
        function WarnOnUnSavedData() {
            //debugger;

            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;

            if (isExceptionRaised != "Y" && IsDataChanged()) {
                return warningMsgOnUnSavedData;
            }
        }
        window.onbeforeunload = WarnOnUnSavedData;

        //used to check if any changes to allow navigation to other screen 
        function IsDataChanged() {
            //debugger;
            if (!CompareData()) {
                return true;
            }
            else {
                return false;
            }
        }

        //to check if data is changed before saving
        function ValidateSave() {
            if (!Page_ClientValidate("valGrpSave")) {
                Page_BlockSubmit = false;
                DisplayMessagePopup("Invalid or missing data!");
                return false;
            }

            if (!IsDataChanged()) {
                DisplayMessagePopup("No changes made to save!");
                return false;
            }

            return true;
        }

        //=============Validations - End

        //Validate any unsaved data on browser window close/refresh
        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        //grid panel height adjustment functioanlity - starts
        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        //Disable back button on royaltor id textbox for existing royaltor
        function MoveCatNoFocus() {
            document.getElementById("<%=txtArtist.ClientID %>").focus();;
        }

        //Navigation button functionality - Start
        //open Catalog Search screen
        function OpenCatalogueSearch() {
            //debugger;
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Participants/CatalogueSearch.aspx?isNewRequest=N");
            }
            else {
                return true;
            }
        }

        //open Audit screen
        function OpenAuditScreen() {
            //debugger;
            catalogueNo = document.getElementById("<%=hdnCatalogueNo.ClientID %>").value;
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Audit/CatalogueDetailsAudit.aspx?CatNo=" + catalogueNo + "");
            }
            else {
                return true;
            }
        }

        //open Participant Maintenance screen
        function OpenParticipantMaintenance() {
            //debugger;
            catalogueNo = document.getElementById("<%=hdnCatalogueNo.ClientID %>").value;
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Participants/ParticipantMaintenance.aspx?CatNo=" + catalogueNo + "");
            }
            else {
                return true;
            }
        }

        //open Participant Summary screen
        function OpenParticipantSummary() {
            //debugger;
            catalogueNo = document.getElementById("<%=hdnCatalogueNo.ClientID %>").value;
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Participants/ParticipantSummary.aspx?CatNo=" + catalogueNo + "");
            }
            else {
                return true;
            }
        }

        //open Track Listing screen
        function OpenTrackListingScreen() {
            //debugger;
            catalogueNo = document.getElementById("<%=hdnCatalogueNo.ClientID %>").value;
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Participants/TrackListing.aspx?CatNo=" + catalogueNo + "");
            }
            else {
                return true;
            }
        }

        //open Missing Participants screen
        function OpenMissingParticipants() {
            //debugger;
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Participants/MissingParticipants.aspx?isNewRequest=N");
            }
            else {
                return true;
            }
        }

        //JIRA-961 Changes by Ravi -- Start
        //open Catalogue Notes screen
        function OpenCatalogueNotes() {
            //debugger;
            catalogueNo = document.getElementById("<%=hdnCatalogueNo.ClientID %>").value;
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Audit/CatalogueNotes.aspx?CatNo=" + catalogueNo + "");
            }
            else {
                return true;
            }
        }
        //JIRA-961 Changes by Ravi -- End
        //============Navigation button functionality - End
    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="4">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    CATALOGUE DETAILS
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td>
                        <br />
                        <br />
                        <br />
                    </td>
                    <td width="10%"></td>
                    <td align="right" width="12%" rowspan="2" valign="top">
                        <table width="100%">
                            <tr>
                                <td>
                                    <asp:Button ID="btnSave" runat="server" CssClass="ButtonStyle" OnClick="btnSave_Click" TabIndex="122"
                                        Text="Save Changes" Width="85%" CausesValidation="false" UseSubmitBehavior="false" OnClientClick="if (!ValidateSave()) { return false;};"
                                        ValidationGroup="valGrpSave" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="btnAudit" runat="server" CssClass="ButtonStyle" OnClick="btnAudit_Click" TabIndex="123" Text="Audit" Width="85%" UseSubmitBehavior="false" OnClientClick="if (!OpenAuditScreen()) { return false;};" />
                                </td>
                            </tr>
                            <tr>
                                <%--JIRA-961 Changes by Ravi -- Start--%>
                                <td>
                                    <asp:Button ID="btnCatNotes" runat="server" CssClass="ButtonStyle" OnClick="btnCatNotes_Click" TabIndex="124" Text="Notes" Width="85%" UseSubmitBehavior="false" OnClientClick="if (!OpenCatalogueNotes()) { return false;};" />
                                </td>
                                <%--JIRA-961 Changes by Ravi -- End--%>
                            </tr>
                            <tr>
                                <td>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                            </tr>
                            <tr>
                                <td></td>
                            </tr>
                            <tr>
                                <td></td>
                            </tr>
                            <tr>
                                <td></td>
                            </tr>
                            <tr>
                                <td></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="btnTrackListing" runat="server" CssClass="LinkButtonStyle" OnClick="btnTrackListing_Click" TabIndex="125" Text="Track Listing" Width="85%"
                                        UseSubmitBehavior="false" OnKeyDown="OnTabPress()" OnClientClick="if (!OpenTrackListingScreen()) { return false;};" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="btnPartSummary" runat="server" CssClass="LinkButtonStyle" OnClick="btnPartSummary_Click" TabIndex="126" Text="Participant Summary" Width="85%"
                                        UseSubmitBehavior="false" OnKeyDown="OnTabPress()" OnClientClick="if (!OpenParticipantSummary()) { return false;};" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="btnPartMaint" runat="server" CssClass="LinkButtonStyle" OnClick="btnPartMaint_Click" TabIndex="127" Text="Participant Maintenance" Width="85%"
                                        UseSubmitBehavior="false" OnKeyDown="OnTabPress()" OnClientClick="if (!OpenParticipantMaintenance()) { return false;};" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="btnMissingPart" runat="server" CssClass="LinkButtonStyle" OnClick="btnMissingPart_Click" TabIndex="128" Text="Missing Participants" Width="85%"
                                        UseSubmitBehavior="false" OnKeyDown="OnTabPress()" OnClientClick="if (!OpenMissingParticipants()) { return false;};" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td class="table_with_border">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="3%">
                                    <br />
                                </td>
                                <td width="97%"></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <table width="100%" cellpadding="1" cellspacing="1">
                                        <tr>
                                            <td width="14%"></td>
                                            <td width="8%"></td>
                                            <td width="16%"></td>
                                            <td width="8%"></td>
                                            <td width="3%"></td>
                                            <td width="2%"></td>
                                            <td width="14%"></td>
                                            <td width="10%"></td>
                                            <td width="12%"></td>
                                            <td width="10%"></td>
                                            <td width="3%"></td>
                                        </tr>
                                        <tr>
                                            <td class="identifierLable_large_bold">Catalogue Number</td>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtCatalogueNumber" runat="server" Width="93.8%" CssClass="textboxStyle" MaxLength="30" TabIndex="100"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvCatalogueNumber" ControlToValidate="txtCatalogueNumber" ValidationGroup="valGrpSave"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter catalogue number" Display="Dynamic"></asp:RequiredFieldValidator>
                                            </td>
                                            <td></td>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Status</td>
                                            <td colspan="3">
                                                <asp:DropDownList ID="ddlCatStatus" runat="server" Width="95%" CssClass="ddlStyle" TabIndex="111" onchange="ValidateStatus();">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvddlCatStatus" ControlToValidate="ddlCatStatus"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please select status" InitialValue="-" Display="Dynamic"
                                                    ValidationGroup="valGrpSave"></asp:RequiredFieldValidator>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td class="identifierLable_large_bold">Artist</td>
                                            <td colspan="4">
                                                <asp:TextBox ID="txtArtist" runat="server" Width="85.5%" CssClass="textboxStyle" TabIndex="101"></asp:TextBox>
                                                <ajaxToolkit:AutoCompleteExtender ID="aceArtistSearch" runat="server"
                                                    ServiceMethod="FuzzySearchAllArtisList"
                                                    ServicePath="~/Services/FuzzySearch.asmx"
                                                    MinimumPrefixLength="1"
                                                    CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                    TargetControlID="txtArtist"
                                                    FirstRowSelected="true"
                                                    OnClientItemSelected="ArtistSelected"
                                                    OnClientPopulating="ArtistListPopulating"
                                                    OnClientPopulated="ArtistListPopulated"
                                                    OnClientHidden="ArtistListPopulated"
                                                    CompletionListElementID="acePnlArtist" />
                                                <asp:Panel ID="acePnlArtist" runat="server" CssClass="identifierLable" />
                                                <asp:ImageButton ID="fuzzySearchArtist" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button"
                                                    OnClick="fuzzySearchArtist_Click" ToolTip="Search artist code/name" TabIndex="102" />
                                                <asp:RequiredFieldValidator runat="server" ID="rfvArtist" ControlToValidate="txtArtist" ValidationGroup="valGrpSave"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please select artist" Display="Dynamic">
                                                </asp:RequiredFieldValidator>
                                            </td>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Configuration</td>
                                            <td colspan="3">
                                                <asp:DropDownList ID="ddlConfiguration" runat="server" Width="95%" CssClass="ddlStyle" TabIndex="112"></asp:DropDownList>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td class="identifierLable_large_bold">Title</td>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtTitle" runat="server" Width="93.8%" CssClass="textboxStyle"
                                                    MaxLength="40" TabIndex="103"></asp:TextBox>
                                            </td>
                                            <td></td>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Project</td>
                                            <td colspan="4">
                                                <asp:TextBox ID="txtProject" runat="server" Width="85.5%" CssClass="textboxStyle"
                                                    TabIndex="113"></asp:TextBox>
                                                <ajaxToolkit:AutoCompleteExtender ID="aceProjectSearch" runat="server"
                                                    ServiceMethod="FuzzySearchAllProjectList"
                                                    ServicePath="~/Services/FuzzySearch.asmx"
                                                    MinimumPrefixLength="1"
                                                    CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                    TargetControlID="txtProject"
                                                    FirstRowSelected="true"
                                                    OnClientItemSelected="ProjectSelected"
                                                    OnClientPopulating="ProjectListPopulating"
                                                    OnClientPopulated="ProjectListPopulated"
                                                    OnClientHidden="ProjectListPopulated"
                                                    CompletionListElementID="acePnlProject" />
                                                <asp:Panel ID="acePnlProject" runat="server" CssClass="identifierLable" />
                                                <asp:ImageButton ID="fuzzySearchProject" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button"
                                                    OnClick="fuzzySearchProject_Click" ToolTip="Search project code/name" TabIndex="114" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="identifierLable_large_bold">MUR Owner</td>
                                            <td colspan="3">
                                                <asp:DropDownList ID="ddlMurOwner" runat="server" Width="95%" CssClass="ddlStyle" TabIndex="104"></asp:DropDownList>
                                            </td>
                                            <td></td>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Exception Rate Project</td>
                                            <td colspan="4">
                                                <asp:TextBox ID="txtExceptionRateProject" runat="server" Width="85.5%" CssClass="textboxStyle"
                                                    TabIndex="115"></asp:TextBox>
                                                <ajaxToolkit:AutoCompleteExtender ID="aceExcRateProjectSearch" runat="server"
                                                    ServiceMethod="FuzzySearchAllProjectList"
                                                    ServicePath="~/Services/FuzzySearch.asmx"
                                                    MinimumPrefixLength="1"
                                                    CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                    TargetControlID="txtExceptionRateProject"
                                                    FirstRowSelected="true"
                                                    OnClientItemSelected="ExceptionRateProjectSelected"
                                                    OnClientPopulating="ExceptionRateProjectListPopulating"
                                                    OnClientPopulated="ExceptionRateProjectListPopulated"
                                                    OnClientHidden="ExceptionRateProjectListPopulated"
                                                    CompletionListElementID="acePnlExceptionRateProject" />
                                                <asp:Panel ID="acePnlExceptionRateProject" runat="server" CssClass="identifierLable" />
                                                <asp:ImageButton ID="fuzzySearchExRatesProject" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button"
                                                    OnClick="fuzzySearchExRatesProject_Click" ToolTip="Search project code/name" TabIndex="116" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="identifierLable_large_bold">Marketing Owner</td>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtMarketingOwner" runat="server" Width="93.8%" CssClass="textboxStyle" MaxLength="50" TabIndex="105"
                                                    Style="text-transform: uppercase"></asp:TextBox>
                                            </td>
                                            <td></td>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Label</td>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtLabel" runat="server" Width="72" CssClass="textboxStyle" MaxLength="3" TabIndex="117" Style="text-transform: uppercase"></asp:TextBox>
                                            </td>
                                            <td></td>


                                        </tr>

                                        <tr>
                                            <td class="identifierLable_large_bold">WEA Sales Label
                                            </td>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtWeaSaelsLabel" runat="server" Width="93.8%" CssClass="textboxStyle" MaxLength="50" TabIndex="106"
                                                    Style="text-transform: uppercase"></asp:TextBox>
                                            </td>
                                            <td></td>
                                            <td></td>
                                            <td class="identifierLable_large_bold">First Sale Date</td>
                                            <td colspan="4">
                                                <asp:TextBox ID="txtFirstSaleDate" runat="server" Width="72" CssClass="textboxStyle" TabIndex="118"></asp:TextBox>
                                                <ajaxToolkit:TextBoxWatermarkExtender ID="wmetxtFirstSaleDate" runat="server" TargetControlID="txtFirstSaleDate"
                                                    WatermarkText="DD/MM/YYYY" WatermarkCssClass="waterMarkText">
                                                </ajaxToolkit:TextBoxWatermarkExtender>
                                                <ajaxToolkit:MaskedEditExtender ID="masktxtFirstSaleDate" runat="server"
                                                    TargetControlID="txtFirstSaleDate" Mask="99/99/9999" AcceptNegative="None"
                                                    ClearMaskOnLostFocus="true" MaskType="Date" />
                                                <asp:RegularExpressionValidator ID="regtxtFirstSaleDate" runat="server" ControlToValidate="txtFirstSaleDate"
                                                    ValidationExpression="((0[1-9]|1[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$"
                                                    ErrorMessage="*" ToolTip="Please enter valid date in DD/MM/YYYY format" ValidationGroup="valGrpSave" CssClass="requiredFieldValidator"
                                                    Display="Dynamic" />
                                                <asp:CustomValidator ID="valFirstSaleDate" runat="server" ValidationGroup="valGrpSave" CssClass="requiredFieldValidator" ControlToValidate="txtFirstSaleDate"
                                                    OnServerValidate="valFirstSaleDate_ServerValidate" ToolTip="Please enter a valid date" Display="Dynamic"
                                                    ErrorMessage="*"></asp:CustomValidator>

                                            </td>

                                        </tr>

                                        <tr>
                                            <td class="identifierLable_large_bold">Total Tracks</td>
                                            <td>
                                                <asp:TextBox ID="txtTotalTracks" runat="server" Width="75%" CssClass="textboxStyle"
                                                    TabIndex="107"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="revTotalTracks" runat="server" Text="*" ControlToValidate="txtTotalTracks" ValidationGroup="valGrpSave"
                                                    ValidationExpression="^[0-9]*$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                    ToolTip="Please enter only positive number" Display="Dynamic"> </asp:RegularExpressionValidator>
                                            </td>
                                            <td class="identifierLable_large_bold" align="center">Unlisted Components</td>
                                            <td align="left">
                                                <asp:TextBox ID="txtUnlistedComponents" runat="server" Width="75%" CssClass="textboxStyle"
                                                    TabIndex="108"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="revUnlistedComponents" runat="server" Text="*" ControlToValidate="txtUnlistedComponents" ValidationGroup="valGrpSave"
                                                    ValidationExpression="^[0-9]*$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                    ToolTip="Please enter only positive number" Display="Dynamic"> </asp:RegularExpressionValidator>
                                            </td>
                                            <td></td>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Compilation?</td>
                                            <td align="left">
                                                <div style="position: relative; left: -4px;">
                                                    <asp:CheckBox ID="cbCompilation" runat="server" TabIndex="119" />
                                                </div>
                                            </td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td class="identifierLable_large_bold">Total Play Length</td>
                                            <td>
                                                <asp:TextBox ID="txtTotalPlayLength" runat="server" Width="75%" CssClass="textboxStyle"
                                                    MaxLength="10" TabIndex="109"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="revTotalPlayLength" runat="server" Text="*" ControlToValidate="txtTotalPlayLength" ValidationGroup="valGrpSave"
                                                    ValidationExpression="^(___:__:__)|([0-9]{0,3}:[0-5][0-9]:[0-5][0-9])$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                    ToolTip="Please enter a valid time in hhh:mm:ss format" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                <ajaxToolkit:TextBoxWatermarkExtender ID="wmeTotalPlayLength" runat="server" TargetControlID="txtTotalPlayLength"
                                                    WatermarkText="hhh:mm:ss" WatermarkCssClass="waterMarkText">
                                                </ajaxToolkit:TextBoxWatermarkExtender>
                                                <ajaxToolkit:MaskedEditExtender ID="mteTotalPlayLength" runat="server"
                                                    TargetControlID="txtTotalPlayLength" Mask="999:99:99" AcceptNegative="None"
                                                    ClearMaskOnLostFocus="false" />
                                            </td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Licensed Out?</td>
                                            <td align="left">
                                                <div style="position: relative; left: -4px;">
                                                    <asp:CheckBox ID="cbLicensedOut" runat="server" TabIndex="120" />
                                                </div>
                                            </td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td class="identifierLable_large_bold">Time / Track Share</td>
                                            <td>
                                                <asp:DropDownList ID="ddlTimeTrackShare" runat="server" Width="80%" CssClass="ddlStyle" TabIndex="110"></asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvTimeTrackShare" ControlToValidate="ddlTimeTrackShare" ValidationGroup="valGrpSave"
                                                    Text="*" InitialValue="-" CssClass="requiredFieldValidator" ToolTip="Please select time / track share" Display="Dynamic"></asp:RequiredFieldValidator>
                                            </td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Legacy</td>
                                            <td align="left">
                                                <div style="position: relative; left: -4px;">
                                                    <asp:CheckBox ID="cbLegacy" runat="server" TabIndex="121" />
                                                </div>
                                            </td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                        </tr>

                                        <tr>
                                            <td colspan="11">
                                                <br />
                                                <asp:HiddenField ID="hdnCatalogueNumber" runat="server" />
                                                <asp:HiddenField ID="hdnArtist" runat="server" />
                                                <asp:HiddenField ID="hdnConfiguration" runat="server" />
                                                <asp:HiddenField ID="hdnTitle" runat="server" />
                                                <asp:HiddenField ID="hdnProject" runat="server" />
                                                <asp:HiddenField ID="hdnMurOwner" runat="server" />
                                                <asp:HiddenField ID="hdnExceptionRateProject" runat="server" />
                                                <asp:HiddenField ID="hdnTotalTracks" runat="server" />
                                                <asp:HiddenField ID="hdnUnlistedComponents" runat="server" />
                                                <asp:HiddenField ID="hdnCompilation" runat="server" />
                                                <asp:HiddenField ID="hdnTotalPlayLength" runat="server" />
                                                <asp:HiddenField ID="hdnLicensedOut" runat="server" />
                                                <asp:HiddenField ID="hdnLegacy" runat="server" />
                                                <asp:HiddenField ID="hdnTimeTrackShare" runat="server" />
                                                <asp:HiddenField ID="hdnIsEditable" runat="server" />
                                                <asp:HiddenField ID="hdnMarketingOwner" runat="server" />
                                                <asp:HiddenField ID="hdnFirstSaleDate" runat="server" />
                                                <asp:HiddenField ID="hdnWeaSalesLabel" runat="server" />
                                                <asp:HiddenField ID="hdnLabel" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td></td>
                    <td valign="top" align="right"></td>
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
            <asp:Panel ID="pnlFuzzySearch" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
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

            <asp:Button ID="dummyConfirm" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmation" BehaviorID="Confirmation" runat="server" PopupControlID="pnlPopup" TargetControlID="dummyConfirm"
                CancelControlID="btnNoConfirm" BackgroundCssClass="popupBox">
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
                            <asp:Label ID="lblConfirmMsg" Text="This will update the Status of all Participants and Tracks to this Catalogue Status" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYesConfirm" runat="server" Text="Confirm" CssClass="ButtonStyle" OnClick="btnYesConfirm_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNoConfirm" runat="server" Text="Cancel" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnTrackListingCount" runat="server" Value="0" />
            <asp:HiddenField ID="hdnStatusCode" runat="server" Value="" />
            <asp:HiddenField ID="hdnUserRole" runat="server" />
            <asp:HiddenField ID="hdnIsValidArtist" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnIsValidProject" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnIsValidExcRateProject" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnIsNewCatNo" runat="server" Value="N" />
            <asp:HiddenField ID="hdnCatalogueNo" runat="server" />
            <asp:HiddenField ID="hdnIsStatusChange" runat="server" Value="N" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" Value="" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99"></asp:Label>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>


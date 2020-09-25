<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AutoParticipantMaintenance.aspx.cs" Inherits="WARS.Participants.AutoParticipantMaintenance" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Auto Participants Maintenance" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">

        function OpenCatalogueSearchScreen() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../Participants/MissingParticipants.aspx?isNewRequest=N');
            }
            else {
                window.location = '../Participants/CatalogueSearch.aspx?isNewRequest=N', '_self';
            }
        }

        function OpenContractMaintenance() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../Contract/RoyaltorSearch.aspx');
            }
            else {
                window.location = '../Contract/RoyaltorSearch.aspx';
            }
        }


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
                        <asp:Button ID="btnCatalogueSearch" runat="server" CssClass="LinkButtonStyle"
                            Text="Catalogue Search" UseSubmitBehavior="false" Width="98%" OnClientClick="if (!OpenCatalogueSearchScreen()) { return false;};" />
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
        var gridClientId = "ContentPlaceHolderBody_gvAutoParticipantDetails_";
        var selectedRowIndex;
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
            //hold scroll position on selecting expand/collapse
            var PnlReference = document.getElementById("<%=PnlGrid.ClientID %>");
            scrollTop = PnlReference.scrollTop;

            //to maintain scroll position
            postBackElementID = args.get_postBackElement().id.substring(args.get_postBackElement().id.lastIndexOf("_") + 1);
            if (postBackElementID == 'btnSaveChanges' || postBackElementID == 'imgBtnInsert') {
                xPos = sender._scrollPosition.x;
                yPos = sender._scrollPosition.y;
            }
        }
        function EndRequestHandler(sender, args) {
            //Hide the modal popup - the update progress
            var popup = $find('<%= mPopupPageLevel.ClientID %>');
            if (popup != null) {
                popup.hide();
            }
            //To bring scroll position to bottom when a new row is added
            window.scrollTo(xPos, yPos);
            var PnlGrid = document.getElementById("<%=PnlGrid.ClientID %>");
            PnlGrid.scrollTop = PnlGrid.scrollHeight;

            var postBackElement = sender._postBackSettings.sourceElement.id;
            if (postBackElement.indexOf('btnHdnRoyaltorInsertSearch') != -1 || postBackElement.indexOf('imgBtnInsert') != -1) {
                document.getElementById("<%=cbEscIncludeUnitsInsert.ClientID %>").disabled = true;
                document.getElementById("<%=cbEscIncludeUnitsInsert.ClientID %>").checked = true;
            }

            //After adding a new row to the grid clear option period and esc code dropdown items
            if (postBackElement.indexOf('imgBtnInsert') != -1) {
                var ddlOptionPeriod = document.getElementById("<%= ddlOptionPeriodInsert.ClientID %>");
                ddlOptionPeriod.innerHTML = "";

                var listItem1 = document.createElement('option');
                listItem1.text = listItem1.value = "-";
                ddlOptionPeriod.add(listItem1);

                var ddlEscCodeInsert = document.getElementById("<%= ddlEscCodeInsert.ClientID %>");
                ddlEscCodeInsert.innerHTML = "";

                var listItem2 = document.createElement('option');
                listItem2.text = listItem2.value = "-";
                ddlEscCodeInsert.add(listItem2);
            }

            //to maintain scroll position
            postBackElementID = sender._postBackSettings.sourceElement.id.substring(sender._postBackSettings.sourceElement.id.lastIndexOf("_") + 1);
            if (postBackElementID == 'btnSaveChanges' || postBackElementID == 'imgBtnInsert') {
                window.scrollTo(xPos, yPos);
            }
        }
        //======================= End

        function OpenAutoParticipantSearch() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../Participants/AutoParticipantSearch.aspx?isNewRequest=N');
            }
            else {
                window.location = '../Participants/AutoParticipantSearch.aspx?isNewRequest=N';
            }
        }
        //on page load
        function SetGrdPnlHeightOnLoad() {
            //grid panel height adjustment functioanlity - starts
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.36;
            document.getElementById("<%=PnlGrid.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
            document.getElementById("<%=cbEscIncludeUnitsInsert.ClientID %>").disabled = true;
            document.getElementById("<%=cbEscIncludeUnitsInsert.ClientID %>").checked = true;
        }
        //============== End

        function OnGridDataChange(row, name) {
            if (name == "Territory") {
                OntxtTerritoryChange(row);
            }
            else if (name == "TrackTitle") {
                OntxtTrackTitleChange(row);
            }

            var selectedRowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            ValidateSuperUser(row, name);
            EnableDisableEscInludes(row, name);
            CompareGridData(selectedRowIndex);
        }

        //Added this to handle fuzzy search changes(with enter key select all search list) and Super user validations
        function OnGridRowSelected(row, name) {
            ValidateSuperUser(row, name);
        }

        function EnableDisableEscInludes(row, name) {
            if (name == "EscalationCode") {
                var rowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
                var str = "ContentPlaceHolderBody_gvAutoParticipantDetails_";
                var cbEscIncludeUnits = document.getElementById(str + 'cbEscIncludeUnits' + '_' + rowIndex);
                var ddlEscalationCode = document.getElementById(str + 'ddlEscalationCode' + '_' + rowIndex);

                if (ddlEscalationCode.value == '-') {
                    cbEscIncludeUnits.disabled = true;
                }
                else {
                    cbEscIncludeUnits.disabled = false;
                }
            }
        }

        //Validate super user
        function ValidateSuperUser(row, name) {
            var rowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            var str = "ContentPlaceHolderBody_gvAutoParticipantDetails_";
            var hdnIsSuperUser = document.getElementById("<%=hdnIsSuperUser.ClientID %>").value;
            if ((hdnIsSuperUser != 'Y')) {
                DisplayMessagePopup("This row can be edited only by super user!");
                if (name == "OptionPeriod") {
                    var hdnAutoParticipOptionPeriod = document.getElementById(str + 'hdnAutoParticipOptionPeriod' + '_' + rowIndex).value;
                    var ddlOptionPeriod = document.getElementById(str + 'ddlOptionPeriod' + '_' + rowIndex);

                    ddlOptionPeriod.value = hdnAutoParticipOptionPeriod;
                }
                else if (name == "Territory") {
                    var hdnSellerGrp = document.getElementById(str + 'hdnSellerGrp' + '_' + rowIndex).value;
                    var txtTerritory = document.getElementById(str + 'txtTerritory' + '_' + rowIndex);
                    txtTerritory.value = hdnSellerGrp;
                }
                else if (name == "Share") {
                    var hdnShare = document.getElementById(str + 'hdnShare' + '_' + rowIndex).value;
                    var txtShare = document.getElementById(str + 'txtShare' + '_' + rowIndex);

                    txtShare.value = hdnShare;
                }
                else if (name == "TotalShare") {
                    var hdnTotalShare = document.getElementById(str + 'hdnTotalShare' + '_' + rowIndex).value;
                    var txtTotalShare = document.getElementById(str + 'txtTotalShare' + '_' + rowIndex);

                    txtTotalShare.value = hdnTotalShare;
                }
                else if (name == "Time") {
                    var hdnTime = document.getElementById(str + 'hdnTime' + '_' + rowIndex).value;
                    var txtTime = document.getElementById(str + 'txtTime' + '_' + rowIndex);

                    txtTime.value = hdnTime.substring(0, 3) + ':' + hdnTime.substring(3, 5) + ':' + hdnTime.substring(5, 7);
                }
                else if (name == "TotalTime") {
                    var hdnTotalTime = document.getElementById(str + 'hdnTotalTime' + '_' + rowIndex).value;
                    var txtTotalTime = document.getElementById(str + 'txtTotalTime' + '_' + rowIndex);

                    txtTotalTime.value = hdnTotalTime.substring(0, 3) + ':' + hdnTotalTime.substring(3, 5) + ':' + hdnTotalTime.substring(5, 7);
                }
                else if (name == "TrackNo") {
                    var hdnTrackNo = document.getElementById(str + 'hdnTrackNo' + '_' + rowIndex).value;
                    var txtTrackNo = document.getElementById(str + 'txtTrackNo' + '_' + rowIndex);

                    txtTrackNo.value = hdnTrackNo;
                }
                else if (name == "TrackTitle") {
                    var hdnTrackTitle = document.getElementById(str + 'hdnTrackTitle' + '_' + rowIndex).value;
                    var txtTrackTitle = document.getElementById(str + 'txtTrackTitle' + '_' + rowIndex);

                    txtTrackTitle.value = hdnTrackTitle;
                }
                else if (name == "EscalationCode") {
                    var hdnEscalationCode = document.getElementById(str + 'hdnEscalationCode' + '_' + rowIndex).value;
                    var ddlEscalationCode = document.getElementById(str + 'ddlEscalationCode' + '_' + rowIndex);

                    ddlEscalationCode.value = hdnEscalationCode;
                }
                else if (name == "Active") {
                    var hdnAutoParticipantActive = document.getElementById(str + 'hdnAutoParticipantActive' + '_' + rowIndex).value;
                    var cbActive = document.getElementById(str + 'cbActive' + '_' + rowIndex);
                    if (hdnAutoParticipantActive == 'A') {
                        cbActive.checked = true;
                    }
                    else {
                        cbActive.checked = false;
                    }
                }
                else if (name == "EscIncludeUnits") {
                    var hdnEscIncludeUnits = document.getElementById(str + 'hdnEscIncludeUnits' + '_' + rowIndex).value;
                    var cbEscIncludeUnits = document.getElementById(str + 'cbEscIncludeUnits' + '_' + rowIndex);
                    if (hdnEscIncludeUnits == 'Y') {
                        cbEscIncludeUnits.checked = true;
                    }
                    else {
                        cbEscIncludeUnits.checked = false;
                    }
                }
                return false;
            }
        }

        function CompareGridData(rowIndex) {
            var str = "ContentPlaceHolderBody_gvAutoParticipantDetails_";
            var hdnAutoParticipOptionPeriod = document.getElementById(str + 'hdnAutoParticipOptionPeriod' + '_' + rowIndex).value;
            var ddlOptionPeriod = document.getElementById(str + 'ddlOptionPeriod' + '_' + rowIndex).value;
            var hdnSellerGrp = document.getElementById(str + 'hdnSellerGrp' + '_' + rowIndex).value;
            var txtTerritory = document.getElementById(str + 'txtTerritory' + '_' + rowIndex).value;
            var hdnShare = document.getElementById(str + 'hdnShare' + '_' + rowIndex).value;
            var txtShare = document.getElementById(str + 'txtShare' + '_' + rowIndex).value;
            var hdnTotalShare = document.getElementById(str + 'hdnTotalShare' + '_' + rowIndex).value;
            var txtTotalShare = document.getElementById(str + 'txtTotalShare' + '_' + rowIndex).value;
            var hdnTime = document.getElementById(str + 'hdnTime' + '_' + rowIndex).value;
            var txtTime = document.getElementById(str + 'txtTime' + '_' + rowIndex).value;
            var hdnTotalTime = document.getElementById(str + 'hdnTotalTime' + '_' + rowIndex).value;
            var txtTotalTime = document.getElementById(str + 'txtTotalTime' + '_' + rowIndex).value;
            var hdnTrackNo = document.getElementById(str + 'hdnTrackNo' + '_' + rowIndex).value;
            var txtTrackNo = document.getElementById(str + 'txtTrackNo' + '_' + rowIndex).value;
            var hdnTrackTitle = document.getElementById(str + 'hdnTrackTitle' + '_' + rowIndex).value;
            var txtTrackTitle = document.getElementById(str + 'txtTrackTitle' + '_' + rowIndex).value;
            var hdnEscalationCode = document.getElementById(str + 'hdnEscalationCode' + '_' + rowIndex).value;
            var ddlEscalationCode = document.getElementById(str + 'ddlEscalationCode' + '_' + rowIndex).value;
            var hdnAutoParticipantActive = document.getElementById(str + 'hdnAutoParticipantActive' + '_' + rowIndex).value;

            var hdnEscIncludeUnits = document.getElementById(str + 'hdnEscIncludeUnits' + '_' + rowIndex).value;
            var hdnIsModified = document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).value;

            var escIncludeUnits;
            var cbEscIncludeUnits = document.getElementById(str + 'cbEscIncludeUnits' + '_' + rowIndex);
            if (cbEscIncludeUnits.checked == true) {
                escIncludeUnits = 'Y';
            }
            else {
                escIncludeUnits = 'N';
            }

            var isActive;
            var cbActive = document.getElementById(str + 'cbActive' + '_' + rowIndex);
            if (cbActive.checked == true) {
                isActive = 'A';
            }
            else {
                isActive = 'I';
            }

            if (hdnAutoParticipOptionPeriod != ddlOptionPeriod || hdnSellerGrp != txtTerritory
                || hdnShare != txtShare || hdnTotalShare != txtTotalShare || hdnTime != txtTime.replace(':', '').replace(':', '').trim() || hdnTotalTime != txtTotalTime.replace(':', '').replace(':', '').trim()
                || hdnTrackNo != txtTrackNo || hdnTrackTitle != txtTrackTitle || hdnEscalationCode != ddlEscalationCode
                || hdnEscIncludeUnits != escIncludeUnits || hdnAutoParticipantActive != isActive) {
                if (hdnIsModified != "-") {
                    document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).innerText = "Y";
                }
            }
            else {
                document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).innerText = "N";
            }
        }

        //clear add row data
        function ClearAddRow() {
            //debugger;
            document.getElementById('<%=txtRoyaltorInsert.ClientID%>').value = "";

            document.getElementById('<%=ddlOptionPeriodInsert.ClientID%>').innerHTML = "";
            var listItem1 = document.createElement('option');
            listItem1.text = listItem1.value = "";
            document.getElementById('<%=ddlOptionPeriodInsert.ClientID%>').add(listItem1);

            var ddlEscCodeInsert = document.getElementById("<%= ddlEscCodeInsert.ClientID %>");
            ddlEscCodeInsert.innerHTML = "";

            var listItem2 = document.createElement('option');
            listItem2.text = listItem2.value = "";
            ddlEscCodeInsert.add(listItem2);

            document.getElementById('<%=txtTerritoryAddRow.ClientID%>').value = "";
            document.getElementById('<%=txtShareInsert.ClientID%>').value = document.getElementById('<%=hdnShareInsert.ClientID%>').value;
            document.getElementById('<%=txtTotalShareInsert.ClientID%>').value = document.getElementById('<%=hdnTotalShareInsert.ClientID%>').value;;
            document.getElementById('<%=txtTimeInsert.ClientID%>').value = '___:__:__';
            document.getElementById('<%=txtTotalTimeInsert.ClientID%>').value = '___:__:__';
            document.getElementById('<%=txtTrackNoInsert.ClientID%>').value = "";
            document.getElementById('<%=txtTrackTitleInsert.ClientID%>').value = "";
            document.getElementById('<%=ddlEscCodeInsert.ClientID%>').selectedIndex = 0;
            document.getElementById('<%=cbEscIncludeUnitsInsert.ClientID%>').checked = true;
            document.getElementById("<%=cbEscIncludeUnitsInsert.ClientID %>").disabled = true;
            document.getElementById("<%=hdnAddRowDataChanged.ClientID %>").innerText = "N";
            Page_ClientValidate('');//clear all validators of the page
            document.getElementById("<%= txtRoyaltorInsert.ClientID %>").focus();
            return false;
        }
        //============== End

        //validations ========== Begin
        function ValidatePopUpAddRow() {
            //warning on add row validation fail
            if (!Page_ClientValidate("valGrpAppendAddRow")) {
                Page_BlockSubmit = false;
                DisplayMessagePopup("Invalid or missing data!");
                return false;
            }
            else {
                return true;
            }
        }

        function ValidateSaveChanges() {
            //warning on add row validation fail                 
            if (!Page_ClientValidate("valUpdate")) {
                Page_BlockSubmit = false;
                DisplayMessagePopup("Auto Participant details not saved – invalid or missing data!");
                return false;
            }
            else {
                return true;
            }
        }

        //validations ========== End

        function IsGridDataChanged() {
            var str = "ContentPlaceHolderBody_gvAutoParticipantDetails_";
            var gvSubRates = document.getElementById("<%= gvAutoParticipantDetails.ClientID %>");
            if (gvSubRates != null) {
                var gvRows = gvSubRates.rows; // WUIN-746 grid view rows including header row
                var isModified;
                var isGridDataChanged = "N";
                var rowIndex;
                for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                    rowIndex = i - 1; //WUIN-746 row index start from 0
                    if (document.getElementById(str + 'hdnIsModified' + '_' + rowIndex) != null) {
                        isModified = document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).value;
                        if (isModified == "Y" || isModified == "-") {
                            isGridDataChanged = "Y";
                            break;
                        }
                    }
                }

                if (isGridDataChanged == "Y") {
                    document.getElementById("<%=hdnGridDataChanged.ClientID %>").value = "Y";
                }
                else {
                    document.getElementById("<%=hdnGridDataChanged.ClientID %>").value = "N";
                }

            }
        }

        function IsAddRowDataChanged() {

            var txtRoyaltorInsert = document.getElementById("<%=txtRoyaltorInsert.ClientID %>").value;
            var ddlOptionPeriodInsert = document.getElementById("<%=ddlOptionPeriodInsert.ClientID %>").value;
            var txtTerritoryAddRow = document.getElementById("<%=txtTerritoryAddRow.ClientID %>").value;
            var txtShareInsert = document.getElementById("<%=txtShareInsert.ClientID %>").value;
            var txtTotalShareInsert = document.getElementById("<%=txtTotalShareInsert.ClientID %>").value;
            var txtTimeInsert = document.getElementById("<%=txtTimeInsert.ClientID %>").value;
            var txtTotalTimeInsert = document.getElementById("<%=txtTotalTimeInsert.ClientID %>").value;
            var txtTrackNoInsert = document.getElementById("<%=txtTrackNoInsert.ClientID %>").value;
            var txtTrackTitleInsert = document.getElementById("<%=txtTrackTitleInsert.ClientID %>").value;
            var ddlEscCodeInsert = document.getElementById("<%=ddlEscCodeInsert.ClientID %>").value;

            var escIncludeUnitsInsert;
            var cbEscIncludeUnitsInsert = document.getElementById("<%=cbEscIncludeUnitsInsert.ClientID %>");
            if (cbEscIncludeUnitsInsert.checked == true) {
                escIncludeUnitsInsert = 'Y';
            }
            else {
                escIncludeUnitsInsert = 'N';
            }

            var hdnShareInsert = document.getElementById("<%=hdnShareInsert.ClientID %>").value;
            var hdnTotalShareInsert = document.getElementById("<%=hdnTotalShareInsert.ClientID %>").value;

            if (txtRoyaltorInsert != '' || ddlOptionPeriodInsert != '' || txtTerritoryAddRow != '' ||
                txtShareInsert != '1' || txtTotalShareInsert != '1' || txtTimeInsert != '___:__:__' || txtTotalTimeInsert != '___:__:__'
                || txtTrackNoInsert != '' || txtTrackTitleInsert != '' || ddlEscCodeInsert != '' || escIncludeUnitsInsert != 'Y') {
                document.getElementById("<%=hdnAddRowDataChanged.ClientID %>").innerText = "Y";
            }
            else {
                document.getElementById("<%=hdnAddRowDataChanged.ClientID %>").innerText = "N";
            }


        }

        function WarnOnUnSavedData() {
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            if (isExceptionRaised != "Y" && IsDataChanged()) {
                return warningMsgOnUnSavedData;
            }
        }
        window.onbeforeunload = WarnOnUnSavedData;

        //used to check if any changes to allow navigation to other screen 
        function IsDataChanged() {
            // debugger;
            IsGridDataChanged();
            IsAddRowDataChanged();
            var isGridDataChanged = document.getElementById("<%=hdnGridDataChanged.ClientID %>").value;
            var isAddRowDataChanged = document.getElementById("<%=hdnAddRowDataChanged.ClientID %>").value;
            if (isGridDataChanged == "Y" || isAddRowDataChanged == "Y") {
                return true;
            }
            else {
                return false;
            }
        }

        function OpenAuditScreen() {
            var autoPartId = document.getElementById("<%= hdnAutoPartId.ClientID %>").value;
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Audit/AutoParticipantMaintAudit.aspx?autoPartId=" + autoPartId);
            }
            else {
                window.location = "../Audit/AutoParticipantMaintAudit.aspx?autoPartId=" + autoPartId;
            }
        }
        //============== End

        //Enable track no and track title if share=1 and totalshare>1
        function OnFocusOutShare(row) {
            //debugger;
            var rowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            var str = "ContentPlaceHolderBody_gvAutoParticipantDetails_";
            var hdnShare = document.getElementById(str + 'hdnShare' + '_' + rowIndex).value;
            var hdnTotalShare = document.getElementById(str + 'hdnTotalShare' + '_' + rowIndex).value;
            var txtShare = document.getElementById(str + 'txtShare' + '_' + rowIndex).value;
            var txtTotalShare = document.getElementById(str + 'txtTotalShare' + '_' + rowIndex).value;
            var txtTrackNo = document.getElementById(str + 'txtTrackNo' + '_' + rowIndex);
            var txtTrackTitle = document.getElementById(str + 'txtTrackTitle' + '_' + rowIndex);
            var txtTime = document.getElementById(str + 'txtTime' + '_' + rowIndex).value;
            var txtTotalTime = document.getElementById(str + 'txtTotalTime' + '_' + rowIndex).value;
            var hdnTrackNo = document.getElementById(str + 'hdnTrackNo' + '_' + rowIndex).value;
            var hdnTrackTitle = document.getElementById(str + 'hdnTrackTitle' + '_' + rowIndex).value;

            if ((txtShare == 1 && txtTotalShare > 1) && (txtTime == '___:__:__' && txtTotalTime == '___:__:__')) {
                txtTrackNo.disabled = false;
                txtTrackTitle.disabled = false;

                txtTime.disabled = true;
                txtTotalTime.disabled = true;
            }
            else if ((txtShare == 1 && txtTotalShare == 1) && (txtTime != '___:__:__' || txtTotalTime != '___:__:__')) {
                document.getElementById(str + 'txtShare' + '_' + rowIndex).disabled = true;
                document.getElementById(str + 'txtTotalShare' + '_' + rowIndex).disabled = true;
                txtTrackNo.disabled = false;
                txtTrackTitle.disabled = false;
            }

            else if ((txtShare == 1 && txtTotalShare == 1) && (txtTime == '___:__:__' && txtTotalTime == '___:__:__')) {
                document.getElementById(str + 'txtShare' + '_' + rowIndex).disabled = false;
                document.getElementById(str + 'txtTotalShare' + '_' + rowIndex).disabled = false;
                txtTrackNo.disabled = true;
                txtTrackTitle.disabled = true;
                document.getElementById(str + 'txtTrackNo' + '_' + rowIndex).value = hdnTrackNo;
                document.getElementById(str + 'txtTrackTitle' + '_' + rowIndex).value = hdnTrackTitle;
            }
        }

        function OnFocusOutShareInsert() {
            var txtShareInsert = document.getElementById("<%=txtShareInsert.ClientID %>").value;
            var txtTotalShareInsert = document.getElementById("<%=txtTotalShareInsert.ClientID %>").value;
            var txtTrackNoInsert = document.getElementById("<%=txtTrackNoInsert.ClientID %>");
            var txtTrackTitleInsert = document.getElementById("<%=txtTrackTitleInsert.ClientID %>");
            var txtTime = document.getElementById("<%=txtTimeInsert.ClientID %>").value;
            var txtTotalTime = document.getElementById("<%=txtTotalTimeInsert.ClientID %>").value;

            if ((txtShareInsert == 1 && txtTotalShareInsert > 1) && (txtTime == '___:__:__' && txtTotalTime == '___:__:__')) {
                txtTrackNoInsert.disabled = false;
                txtTrackTitleInsert.disabled = false;
                document.getElementById("<%=txtTimeInsert.ClientID %>").disabled = true;
                document.getElementById("<%=txtTotalTimeInsert.ClientID %>").disabled = true;

                //To keep Enabled the Tracktitle and TrackNo fields when Track title fuzzy search popup is clicked
                document.getElementById("<%=hdnTrackTitleInsertEnabled.ClientID %>").innerText = "Y";
            }
            else if ((txtShareInsert == 1 && txtTotalShareInsert == 1) && (txtTime != '___:__:__' || txtTotalTime != '___:__:__')) {
                document.getElementById("<%=txtShareInsert.ClientID %>").disabled = true;
                document.getElementById("<%=txtTotalShareInsert.ClientID %>").disabled = true;
                document.getElementById("<%=txtShareInsert.ClientID %>").value = '';
                document.getElementById("<%=txtTotalShareInsert.ClientID %>").value = '';
                txtTrackNoInsert.disabled = false;
                txtTrackTitleInsert.disabled = false;

                document.getElementById("<%=hdnTrackTitleInsertEnabled.ClientID %>").innerText = "Y";
            }

            else if ((txtShareInsert == 1 && txtTotalShareInsert == 1) || (txtTime == '___:__:__' && txtTotalTime == '___:__:__')) {
                document.getElementById("<%=txtShareInsert.ClientID %>").disabled = false;
                document.getElementById("<%=txtTotalShareInsert.ClientID %>").disabled = false;
                document.getElementById("<%=txtTimeInsert.ClientID %>").disabled = false;
                document.getElementById("<%=txtTotalTimeInsert.ClientID %>").disabled = false;
                txtTrackNoInsert.disabled = true;
                txtTrackTitleInsert.disabled = true;
                document.getElementById("<%=txtTrackNoInsert.ClientID %>").value = "";
                document.getElementById("<%=txtTrackTitleInsert.ClientID %>").value = "";
                document.getElementById("<%=hdnTrackTitleInsertEnabled.ClientID %>").innerText = "N";
            }
        }

        //=========End

        //Enable Esc Include Units of Escalation code is selected
        function OnChangeEscCodeInsert() {
            //debugger;
            var ddlEscCodeInsert = document.getElementById("<%=ddlEscCodeInsert.ClientID %>");
            var cbEscIncludeUnitsInsert = document.getElementById("<%=cbEscIncludeUnitsInsert.ClientID %>");

            if (ddlEscCodeInsert.value == '-') {
                cbEscIncludeUnitsInsert.disabled = true;
            }
            else {
                cbEscIncludeUnitsInsert.disabled = false;
            }
        }

        //===========End

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }
        function FocusLblKeyPress() {
            document.getElementById("<%= txtRoyaltorInsert.ClientID %>").focus();
        }
        //=============== End
        //Track title fuzzy search -- Starts

        function trackTitleListPopulating(sender, args) {
            selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
            txtTrackTitle = document.getElementById(gridClientId + 'txtTrackTitle' + '_' + selectedRowIndex);
            txtTrackTitle.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtTrackTitle.style.backgroundRepeat = 'no-repeat';
            txtTrackTitle.style.backgroundPosition = 'right';
        }

        function trackTitleListPopulated(sender, args) {
            selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
            txtTrackTitle = document.getElementById(gridClientId + 'txtTrackTitle' + '_' + selectedRowIndex);
            txtTrackTitle.style.backgroundImage = 'none';
        }

        function trackTitleListHidden(sender, args) {
            selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
    txtTrackTitle = document.getElementById(gridClientId + 'txtTrackTitle' + '_' + selectedRowIndex);
    txtTrackTitle.style.backgroundImage = 'none';
        }

        function trackTitleListItemSelected(sender, args) {
            var trackTitleSrchVal = args.get_value();
            if (trackTitleSrchVal == 'No results found') {
                selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
                document.getElementById(gridClientId + 'txtTrackTitle' + '_' + selectedRowIndex).value = "";
            }
        }

        //Pop up fuzzy search list       
        function OntxtTrackTitleKeyDown(sender) {
            if ((event.keyCode == 13)) {
                selectedRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
                //Enter key can be used to select the dropdown list item or to pop up the complete list
                //to know this, check if list item is selected or not
                var aceTrackTitle = $find(gridClientId + 'aceTrackTitle' + '_' + selectedRowIndex);
                if (aceTrackTitle._selectIndex == -1) {
                    txtTrackTitle = document.getElementById(gridClientId + 'txtTrackTitle' + '_' + selectedRowIndex).value;
                    document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtTrackTitle;
                    document.getElementById("<%=hdnGridFuzzySearchRowId.ClientID %>").innerText = selectedRowIndex;
                    document.getElementById("<%=hdnFuzzySearchField.ClientID %>").innerText = "TrackTitle";
                    document.getElementById("<%=hdnGridRoyFuzzySearchRowId.ClientID %>").innerText = selectedRowIndex;
                    document.getElementById('<%=btnFuzzyTrackTitleListPopup.ClientID%>').click();
                }
            }
        }

        //Validate if the field value is a valid one from fuzzy search list
        function ValTrackTitleGridRow(sender, args) {
            //debugger;
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtTrackTitle = document.getElementById(gridClientId + 'txtTrackTitle' + '_' + gridRowIndex);

            if (txtTrackTitle.value == "") {
                args.IsValid = true;
        txtTrackTitle.style["width"] = '90%';
            }
            else if (txtTrackTitle.value == "No results found") {
                args.IsValid = true;
                txtTrackTitle.value = "";
        txtTrackTitle.style["width"] = '90%';
            }
                //Harish 18-06-18 - not required as Track title must not have '-'
                //else if (txtTrackTitleInsert.value != "" && txtTrackTitleInsert.value.indexOf('-') == -1) {
                //    args.IsValid = false;
                //    //adjust width of the textbox to display error		
                //    fieldWidth = txtTrackTitleInsert.offsetWidth;
                //    txtTrackTitleInsert.style["width"] = (fieldWidth - 20);
                //}
            else if (args.IsValid == true) {
        txtTrackTitle.style["width"] = '90%';
            }
        }

        //reset field width when empty
        function OntxtTrackTitleChange(sender) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtTrackTitle = document.getElementById(gridClientId + 'txtTrackTitle' + '_' + gridRowIndex);

            if (txtTrackTitle.value == "") {
        txtTrackTitle.style["width"] = '90%';
            }
        }

        //Track title fuzzy search -- Ends

        //Territory fuzzy search -- Starts

        function territoryListPopulating(sender, args) {
            selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
            txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + selectedRowIndex);
            txtTerritory.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtTerritory.style.backgroundRepeat = 'no-repeat';
            txtTerritory.style.backgroundPosition = 'right';

        }

        function territoryListPopulated(sender, args) {
            selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
            txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + selectedRowIndex);
            txtTerritory.style.backgroundImage = 'none';
        }

        function territoryListHidden(sender, args) {
            selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
            txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + selectedRowIndex);
            txtTerritory.style.backgroundImage = 'none';
        }

        function territoryListItemSelected(sender, args) {
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
                document.getElementById(gridClientId + 'txtTerritory' + '_' + selectedRowIndex).value = "";
            }
        }

        //Pop up fuzzy search list       
        function OntxtTerritoryKeyDown(sender) {
            if ((event.keyCode == 13)) {
                selectedRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
                //Enter key can be used to select the dropdown list item or to pop up the complete list
                //to know this, check if list item is selected or not
                var aceTerritory = $find(gridClientId + 'aceTerritory' + '_' + selectedRowIndex);
                if (aceTerritory._selectIndex == -1) {
                    txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + selectedRowIndex).value;
                    document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtTerritory;
                    document.getElementById("<%=hdnGridFuzzySearchRowId.ClientID %>").innerText = selectedRowIndex;
                    document.getElementById("<%=hdnFuzzySearchField.ClientID %>").innerText = "Territory";
                    document.getElementById("<%=hdnGridRoyFuzzySearchRowId.ClientID %>").innerText = selectedRowIndex;
                    document.getElementById('<%=btnFuzzyTerritoryListPopup.ClientID%>').click();
                }
            }
        }

        //Validate if the field value is a valid one from fuzzy search list
        function ValTerritoryGridRow(sender, args) {
            //debugger;
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + gridRowIndex);

            if (txtTerritory.value == "") {
                args.IsValid = true;
                txtTerritory.style["width"] = '90%';
            }
            else if (txtTerritory.value == "No results found") {
                args.IsValid = true;
                txtTerritory.value = "";
                txtTerritory.style["width"] = '90%';
            }
            else if (txtTerritory.value != "" && txtTerritory.value.indexOf('-') == -1) {
                args.IsValid = false;
                //adjust width of the textbox to display error
                fieldWidth = txtTerritory.offsetWidth;
                txtTerritory.style["width"] = (fieldWidth - 10);
            }
            else if (args.IsValid == true) {
                txtTerritory.style["width"] = '90%';
            }
        }

        //reset field width when empty
        function OntxtTerritoryChange(sender) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + gridRowIndex);

            if (txtTerritory.value == "") {
                txtTerritory.style["width"] = '90%';
            }
        }

        //===========Territory fuzzy search -- Ends

        //Royaltor Add row fuzzy search -- Start

        function royaltorListPopulatingInsert() {
            txtRoyaltorInsert = document.getElementById("<%= txtRoyaltorInsert.ClientID %>");
            txtRoyaltorInsert.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoyaltorInsert.style.backgroundRepeat = 'no-repeat';
            txtRoyaltorInsert.style.backgroundPosition = 'right';
        }

        function royaltorListPopulatedInsert() {
            txtRoyaltorInsert = document.getElementById("<%= txtRoyaltorInsert.ClientID %>");
            txtRoyaltorInsert.style.backgroundImage = 'none';
            var ddlOptionPeriod = document.getElementById("<%= ddlOptionPeriodInsert.ClientID %>");
            ddlOptionPeriod.innerHTML = "";

            var listItem1 = document.createElement('option');
            listItem1.text = listItem1.value = "-";
            ddlOptionPeriod.add(listItem1);
        }

        function royaltorListItemSelectedInsert(sender, args) {
            var srchVal = args.get_value();
            if (srchVal == 'No results found') {
                document.getElementById("<%= txtRoyaltorInsert.ClientID %>").value = "";
            }
            else {
                document.getElementById('<%=btnHdnRoyaltorInsertSearch.ClientID%>').click();
            }
        }

        //Pop up fuzzy search list       
        function OntxtRoyaltorInsertKeyDown() {
            if ((event.keyCode == 13)) {
                //Enter key can be used to select the dropdown list item or to pop up the complete list
                //to know this, check if list item is selected or not
                var aceRoyaltorAddRow = $find('ContentPlaceHolderBody_' + 'aceRoyaltorAddRow');
                if (aceRoyaltorAddRow._selectIndex == -1) {
                    txtRoyaltorInsert = document.getElementById("<%= txtRoyaltorInsert.ClientID %>").value;
                    document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtRoyaltorInsert;
                    document.getElementById("<%=hdnFuzzySearchField.ClientID %>").innerText = "RoyaltorAddRow";
                    document.getElementById('<%=btnHdnRoyaltorInsertSearch.ClientID%>').click();
                }
            }

        }

        //reset field width when empty
        function OntxtRoyaltorAddRowChange() {
            txtRoyaltorInsert = document.getElementById("<%=txtRoyaltorInsert.ClientID %>");
            if (txtRoyaltorInsert.value == "") {
                txtRoyaltorInsert.style["width"] = '90%';
            }
        }

        //Validate if the field value is a valid one from fuzzy search list
        function ValRoyaltorAddRow(sender, args) {
            txtRoyaltorInsert = document.getElementById("<%=txtRoyaltorInsert.ClientID %>");
            if (txtRoyaltorInsert.value == "") {
                args.IsValid = true;
                txtRoyaltorInsert.style["width"] = '90%';
            }
            else if (txtRoyaltorInsert.value == "No results found") {
                args.IsValid = true;
                txtRoyaltorInsert.value = "";
                txtRoyaltorInsert.style["width"] = '90%';
            }
            else if (txtRoyaltorInsert.value != "" && txtRoyaltorInsert.value.indexOf('-') == -1) {
                args.IsValid = false;
                //adjust width of the textbox to display error
                fieldWidth = txtRoyaltorInsert.offsetWidth;
                txtRoyaltorInsert.style["width"] = (fieldWidth - 15);
            }
            else if (args.IsValid == true) {
                txtRoyaltorInsert.style["width"] = '90%';
            }

        }

        function OnRoyFuzzySearchChangeInsert() {
            //debugger;
            var royaltorSearchText = document.getElementById("<%= txtRoyaltorInsert.ClientID %>").value;
            var ddlOptionPeriod = document.getElementById("<%= ddlOptionPeriodInsert.ClientID %>");
            ddlOptionPeriod.innerHTML = "";

            var listItem1 = document.createElement('option');
            listItem1.text = listItem1.value = "-";
            ddlOptionPeriod.add(listItem1);

            var ddlEscCodeInsert = document.getElementById("<%= ddlEscCodeInsert.ClientID %>");
            ddlEscCodeInsert.innerHTML = "";

            var listItem2 = document.createElement('option');
            listItem2.text = listItem2.value = "-";
            ddlEscCodeInsert.add(listItem2);

            OntxtRoyaltorAddRowChange();

            return false;
        }

        //Royaltor Add row fuzzy search ====== End ==

        //Territory Add row fuzzy search -- Start

        function territoryAddRowListPopulating() {
            txtTerritoryAddRow = document.getElementById("<%= txtTerritoryAddRow.ClientID %>");
            txtTerritoryAddRow.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtTerritoryAddRow.style.backgroundRepeat = 'no-repeat';
            txtTerritoryAddRow.style.backgroundPosition = 'right';

        }

        function territoryAddRowListPopulated() {
            txtTerritoryAddRow = document.getElementById("<%= txtTerritoryAddRow.ClientID %>");
            txtTerritoryAddRow.style.backgroundImage = 'none';
        }

        function territoryAddRowListHidden() {
            txtTerritoryAddRow = document.getElementById("<%= txtTerritoryAddRow.ClientID %>");
            txtTerritoryAddRow.style.backgroundImage = 'none';

        }

        function territoryAddRowListItemSelected(sender, args) {
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                txtTerritoryAddRow = document.getElementById("<%= txtTerritoryAddRow.ClientID %>");
                txtTerritoryAddRow.value = "";
            }
        }

        //Pop up fuzzy search list       
        function OntxtTerritoryAddRowKeyDown() {
            if ((event.keyCode == 13)) {
                //Enter key can be used to select the dropdown list item or to pop up the complete list
                //to know this, check if list item is selected or not
                var aceTerritoryAddRow = $find('ContentPlaceHolderBody_' + 'aceTerritoryAddRow');
                if (aceTerritoryAddRow._selectIndex == -1) {
                    txtTerritoryAddRow = document.getElementById("<%= txtTerritoryAddRow.ClientID %>").value;
                    document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtTerritoryAddRow;
                    document.getElementById("<%=hdnFuzzySearchField.ClientID %>").innerText = "TerritoryAddRow";
                    document.getElementById('<%=btnFuzzyTerritoryListPopup.ClientID%>').click();
                }
            }
        }

        //reset field width when empty
        function OntxtTerritoryAddRowChange() {
            txtTerritoryAddRow = document.getElementById("<%=txtTerritoryAddRow.ClientID %>");
            if (txtTerritoryAddRow.value == "") {
                txtTerritoryAddRow.style["width"] = '98%';
            }
        }

        //Validate if the field value is a valid one from fuzzy search list
        function ValTerritoryAddRow(sender, args) {
            txtTerritoryAddRow = document.getElementById("<%=txtTerritoryAddRow.ClientID %>");
            if (txtTerritoryAddRow.value == "") {
                args.IsValid = true;
                txtTerritoryAddRow.style["width"] = '98%';
            }
            else if (txtTerritoryAddRow.value == "No results found") {
                args.IsValid = true;
                txtTerritoryAddRow.value = "";
                txtTerritoryAddRow.style["width"] = '98%';
            }
            else if (txtTerritoryAddRow.value != "" && txtTerritoryAddRow.value.indexOf('-') == -1) {
                args.IsValid = false;
                //adjust width of the textbox to display error
                fieldWidth = txtTerritoryAddRow.offsetWidth;
                txtTerritoryAddRow.style["width"] = (fieldWidth - 10);
            }
            else if (args.IsValid == true) {
                txtTerritoryAddRow.style["width"] = '98%';
            }

        }

        //Territory Add row fuzzy search ======= End ====

        //Track title Add row fuzzy search -- Start
        function trackTitleListPopulatingInsert() {
            txtTrackTitleInsert = document.getElementById("<%= txtTrackTitleInsert.ClientID %>");
            txtTrackTitleInsert.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtTrackTitleInsert.style.backgroundRepeat = 'no-repeat';
            txtTrackTitleInsert.style.backgroundPosition = 'right';
        }

        function trackTitleListPopulatedInsert() {
            txtTrackTitleInsert = document.getElementById("<%= txtTrackTitleInsert.ClientID %>");
           txtTrackTitleInsert.style.backgroundImage = 'none';
       }

       function trackTitleListItemSelectedInsert(sender, args) {
           var srchVal = args.get_value();
           if (srchVal == 'No results found') {
               document.getElementById("<%= txtTrackTitleInsert.ClientID %>").value = "";
            }
        }

        //Pop up fuzzy search list       
        function OntxtTrackTitleAddRowKeyDown() {
            if ((event.keyCode == 13)) {
                //Enter key can be used to select the dropdown list item or to pop up the complete list
                //to know this, check if list item is selected or not
                var aceTrackTitleAddRow = $find('ContentPlaceHolderBody_' + 'aceTrackTitleAddRow');
                if (aceTrackTitleAddRow._selectIndex == -1) {
                    txtTrackTitleInsert = document.getElementById("<%= txtTrackTitleInsert.ClientID %>").value;
                    document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtTrackTitleInsert;
                    document.getElementById("<%=hdnFuzzySearchField.ClientID %>").innerText = "TrackTitleAddRow";
                    document.getElementById('<%=btnFuzzyTrackTitleListPopup.ClientID%>').click();
                }
            }
        }

        //reset field width when empty
        function OntxtTrackTitleAddRowChange() {
            txtTrackTitleInsert = document.getElementById("<%=txtTrackTitleInsert.ClientID %>");
            if (txtTrackTitleInsert.value == "") {
                txtTrackTitleInsert.style["width"] = '98%';
            }
        }

        //Validate if the field value is a valid one from fuzzy search list
        function ValTrackTitleAddRow(sender, args) {
            txtTrackTitleInsert = document.getElementById("<%=txtTrackTitleInsert.ClientID %>");

            if (txtTrackTitleInsert.value == "") {
                args.IsValid = true;
                txtTrackTitleInsert.style["width"] = '98%';
            }
            else if (txtTrackTitleInsert.value == "No results found") {
                args.IsValid = true;
                txtTrackTitleInsert.value = "";
                txtTrackTitleInsert.style["width"] = '98%';
            }
                //Harish 18-06-18 - not required as Track title must not have '-'
                //else if (txtTrackTitleInsert.value != "" && txtTrackTitleInsert.value.indexOf('-') == -1) {
                //    args.IsValid = false;
                //    //adjust width of the textbox to display error		
                //    fieldWidth = txtTrackTitleInsert.offsetWidth;
                //    txtTrackTitleInsert.style["width"] = (fieldWidth - 20);
                //}
            else if (args.IsValid == true) {
                txtTrackTitleInsert.style["width"] = '98%';
            }
        }

        //Time on each Participant must not be > Total Time of that Participant
        function ValTimeInsertRow(args) {
            txtTime = document.getElementById("<%= txtTimeInsert.ClientID %>").value;
            txtTotalTime = document.getElementById("<%= txtTotalTimeInsert.ClientID %>").value;

            args.IsValid = true;
            if (CompareTimes(txtTime, txtTotalTime)) {
                args.IsValid = false;
            }
        }

        //Time on each Participant must not be > Total Time of that Participant
        function ValTimeGridRow(sender, args) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtTime = document.getElementById(gridClientId + 'txtTime' + '_' + gridRowIndex);
            txtTotalTime = document.getElementById(gridClientId + 'txtTotalTime' + '_' + gridRowIndex);

            args.IsValid = true;

            if (txtTime.disabled == false && txtTotalTime.disabled == false) {
                if (CompareTimes(txtTime.value, txtTotalTime.value)) {
                    args.IsValid = false;
                }
            }
        }

        // compares two time values.
        // if time1 > time2 then returned true
        function CompareTimes(time1, time2) {
            var time1Array = time1.split(":");
            var time2Array = time2.split(":");

            time1Value = Number(time1Array[0]) * 3600 + Number(time1Array[1]) * 60 + Number(time1Array[2]);
            time2Value = Number(time2Array[0]) * 3600 + Number(time2Array[1]) * 60 + Number(time2Array[2]);

            if (time1Value > time2Value) {
                return true;
            }
            else if (time1Value == '0' && time2Value == '0') {
                return true;
            }
            else if ((time1 == '___:__:__' || time2 == '___:__:__')) {
                return true;
            }
            else {
                return false;
            }
        }

        //validation: share should not be > total share
        function ValShareGridRow(sender, args) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtShare = document.getElementById(gridClientId + 'txtShare' + '_' + gridRowIndex);
            txtTotalShare = document.getElementById(gridClientId + 'txtTotalShare' + '_' + gridRowIndex);

            args.IsValid = true;

            if (txtShare.disabled == false && txtTotalShare.disabled == false) {
                if (Number(txtShare.value) > Number(txtTotalShare.value)) {
                    args.IsValid = false;
                }
            }
        }
        function OpenOnUnSavedData() {
            var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
            if (warnPopup != null) {
                document.getElementById("<%=lblUnSavedWarnMsg.ClientID %>").innerText = warningMsgOnUnSavedData;
                warnPopup.show();
            }
        }

        function OnUnSavedDataReturn() {
            var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
            if (warnPopup != null) {
                warnPopup.hide();
            }
            window.onbeforeunload = WarnOnUnSavedData;
            return false;
        }

        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }
        //============== End

        function imgBtnInsertKeydown() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=imgBtnInsert.ClientID%>').click();
            }
        }

    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="2">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    AUTO PARTICIPANT MAINTENANCE
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="2"></td>
                </tr>
                <tr>
                    <td class="table_header_with_border" valign="top">Participation Group</td>
                </tr>
                <tr>
                    <td valign="top">
                        <table width="75%" class="table_with_border">
                            <tr>
                                <td style="padding: 10px">
                                    <table width="99.9%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td class="gridHeaderStyle_1row" width="12%">Marketing Owner</td>
                                            <td class="gridHeaderStyle_1row" width="12%">WEA Sales Label</td>
                                            <td class="gridHeaderStyle_1row" width="20%">Artist</td>
                                            <td class="gridHeaderStyle_1row" width="30%">Project Title</td>
                                        </tr>
                                        <tr>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:Label ID="lblMarketingOwner" runat="server" Width="48%" CssClass="identifierLable" ReadOnly="true"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:Label ID="lblWEASales" runat="server" Width="48%" CssClass="identifierLable" ReadOnly="true"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:Label ID="lblArtist" runat="server" Width="78%" CssClass="identifierLable" ReadOnly="true"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:TextBox ID="txtProjectTitle" runat="server" Width="99%" Style="border: none; text-align: center" CssClass="identifierLable" ReadOnly="true"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td width="12%" rowspan="2" valign="top" align="right">
                        <table width="100%">
                            <tr>
                                <td>
                                    <asp:Button ID="btnSaveChanges" runat="server" CssClass="ButtonStyle" OnClick="btnSaveChanges_Click"
                                        Text="Save Changes" UseSubmitBehavior="false" Width="96%" OnClientClick="if (!ValidateSaveChanges()) { return false;};" TabIndex="114" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="btnAudit" runat="server" CssClass="ButtonStyle" OnClientClick="if (!OpenAuditScreen()) { return false;};"
                                        OnClick="btnAudit_Click" Text="Audit" Width="96%" TabIndex="115" UseSubmitBehavior="false" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="btnAutoParticipantSearch" Width="96%" runat="server" CausesValidation="false" CssClass="ButtonStyle"
                                        OnClientClick="if (!OpenAutoParticipantSearch()) { return false;};" TabIndex="116" Text="Auto Participant Search"
                                        UseSubmitBehavior="false" />
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
                    <td class="table_header_with_border" valign="top" colspan="2" style="padding-left: 15px">Participant Summary
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table width="100%" class="table_with_border" cellpadding="0" cellspacing="0">
                            <tr>
                                <td style="padding-left: 10px; padding-bottom: 10px;">
                                    <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Vertical" Width="100%">
                                        <asp:GridView ID="gvAutoParticipantDetails" runat="server" AutoGenerateColumns="False" Width="98.5%" AlternatingRowStyle-BackColor="#E3EFFF"
                                            CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" ShowHeaderWhenEmpty="true" EmptyDataText="No Data Found"
                                            EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" OnRowDataBound="gvAutoParticipantDetails_RowDataBound"
                                             OnRowCommand="gvAutoParticipantDetails_RowCommand" AllowSorting="true" OnSorting="gvAutoParticipantDetails_Sorting" HeaderStyle-CssClass="FixedHeader">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-Width="13%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Royaltor" SortExpression="royaltor_id">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnAutoParticipantDetailId" runat="server" Value='<%#Bind("auto_participant_detail_id")%>' />
                                                        <asp:HiddenField ID="hdnAutoParticipantRoyId" runat="server" Value='<%# Bind("royaltor_id") %>' />
                                                        <asp:Label ID="lblRoyaltor" runat="server" Text='<%# Bind("royaltor_id") %>' CssClass="identifierLable"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="11%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Option Period" SortExpression="option_period_code">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnAutoParticipOptionPeriod" runat="server" Value='<%# Bind("option_period_code") %>' />
                                                        <asp:DropDownList ID="ddlOptionPeriod" runat="server" Width="87%" CssClass="ddlStyle" onchange="OnGridDataChange(this,'OptionPeriod');"></asp:DropDownList>
                                                        <asp:RequiredFieldValidator runat="server" ID="rfvOptionPeriod" ControlToValidate="ddlOptionPeriod" ValidationGroup="valUpdate"
                                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please select option period" InitialValue="-" Display="Dynamic">
                                                        </asp:RequiredFieldValidator>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="15%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Territory" SortExpression="seller_group">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnSellerGrp" runat="server" Value='<%# Bind("seller_group") %>' />
                                                        <asp:TextBox ID="txtTerritory" runat="server" Width="94%" Text='<%#Bind("seller_group")%>' CssClass="textbox_FuzzySearch"
                                                            ToolTip='<%#Bind("seller_group")%>' onkeydown="OntxtTerritoryKeyDown(this);" onchange="OnGridDataChange(this,'Territory');"
                                                            onfocus="OnGridRowSelected(this,'Territory');"></asp:TextBox>
                                                        <ajaxToolkit:AutoCompleteExtender ID="aceTerritory" runat="server"
                                                            ServiceMethod="FuzzyPartMaintSellerGrpList"
                                                            ServicePath="~/Services/FuzzySearch.asmx"
                                                            MinimumPrefixLength="1"
                                                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                            TargetControlID="txtTerritory"
                                                            FirstRowSelected="true"
                                                            OnClientPopulating="territoryListPopulating"
                                                            OnClientPopulated="territoryListPopulated"
                                                            OnClientHidden="territoryListHidden"
                                                            OnClientItemSelected="territoryListItemSelected"
                                                            CompletionListElementID="pnlTerritoryFuzzySearch" />
                                                        <asp:Panel ID="pnlTerritoryFuzzySearch" runat="server" CssClass="identifierLable" />
                                                        <asp:CustomValidator ID="CustomValidator1" runat="server" ValidationGroup="valUpdate" CssClass="requiredFieldValidator"
                                                            ClientValidationFunction="ValTerritoryGridRow" ToolTip="Please select valid territory from the search list"
                                                            ControlToValidate="txtTerritory" ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Share" SortExpression="share_tracks">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnShare" runat="server" Value='<%# Bind("share_tracks") %>' />
                                                        <asp:TextBox ID="txtShare" runat="server" Text='<%#Bind("share_tracks") %>'
                                                            CssClass="gridTextField" Width="70%" onchange="OnGridDataChange(this,'Share');" onkeyup="OnFocusOutShare(this);" Style="text-align: center;" Enabled="true"></asp:TextBox>
                                                        <asp:CustomValidator ID="valShare" runat="server" ValidationGroup="valUpdate" CssClass="requiredFieldValidator" Display="Dynamic"
                                                            ClientValidationFunction="ValShareGridRow" ErrorMessage="*" ToolTip="Share must not be greater than Total Share or participation multiplier should not be greater than 1"></asp:CustomValidator>
                                                        <asp:RegularExpressionValidator ID="revShare" runat="server" Text="*" ControlToValidate="txtShare"
                                                            ValidationExpression="^[+]?\d+$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valUpdate"
                                                            ToolTip="Please enter only integers" Display="Dynamic"> 
                                                        </asp:RegularExpressionValidator>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Total Share" SortExpression="share_total_tracks">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnTotalShare" runat="server" Value='<%# Bind("share_total_tracks") %>' />
                                                        <asp:TextBox ID="txtTotalShare" runat="server" Text='<%#Bind("share_total_tracks") %>'
                                                            CssClass="gridTextField" Width="70%" onchange="OnGridDataChange(this,'TotalShare');" onkeyup="OnFocusOutShare(this);" Style="text-align: center;" Enabled="true"></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="revTotalShare" runat="server" Text="*" ControlToValidate="txtTotalShare"
                                                            ValidationExpression="^[+]?\d+$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valUpdate"
                                                            ToolTip="Please enter only integers" Display="Dynamic"> 
                                                        </asp:RegularExpressionValidator>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Time" SortExpression="share_time">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnTime" runat="server" Value='<%# Bind("share_time") %>' />
                                                        <asp:TextBox ID="txtTime" runat="server" Text='<%# Bind("share_time") %>'
                                                            CssClass="gridTextField" Width="75%" MaxLength="7" onchange="OnGridDataChange(this,'Time');" onkeyup="OnFocusOutShare(this);" Style="text-align: center;" Enabled="true"></asp:TextBox>
                                                        <asp:CustomValidator ID="valTime" runat="server" ValidationGroup="valUpdate" CssClass="requiredFieldValidator" Display="Dynamic"
                                                            ClientValidationFunction="ValTimeGridRow" ErrorMessage="*" ToolTip="Time Share must not be greater than Total Time">
                                                        </asp:CustomValidator>
                                                        <asp:RegularExpressionValidator ID="revTime" runat="server" Text="*" ControlToValidate="txtTime" ValidationGroup="valUpdate"
                                                            ValidationExpression="^(___:__:__)|(000:00:00)|([0-9]{0,3}:[0-5][0-9]:[0-5][0-9])$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                            ToolTip="Please enter a valid time in hhh:mm:ss format" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                        <ajaxToolkit:MaskedEditExtender ID="mteTime" runat="server"
                                                            TargetControlID="txtTime" Mask="999:99:99" AcceptNegative="None"
                                                            ClearMaskOnLostFocus="false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Total Time" SortExpression="share_total_time">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnTotalTime" runat="server" Value='<%# Bind("share_total_time") %>' />
                                                        <asp:TextBox ID="txtTotalTime" runat="server" Text='<%# Bind("share_total_time") %>'
                                                            CssClass="gridTextField" Width="75%" MaxLength="10" onchange="OnGridDataChange(this,'TotalTime');" onkeyup="OnFocusOutShare(this);" Style="text-align: center;" Enabled="true"></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="revTotalTime" runat="server" Text="*" ControlToValidate="txtTotalTime" ValidationGroup="valUpdate"
                                                            ValidationExpression="^(___:__:__)|(000:00:00)|([0-9]{0,3}:[0-5][0-9]:[0-5][0-9])$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                            ToolTip="Please enter a valid time in hhh:mm:ss format" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                        <ajaxToolkit:MaskedEditExtender ID="mteTotalTime" runat="server"
                                                            TargetControlID="txtTotalTime" Mask="999:99:99" AcceptNegative="None"
                                                            ClearMaskOnLostFocus="false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Track No." SortExpression="tune_code">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnTrackNo" runat="server" Value='<%# Bind("tune_code") %>' />
                                                        <asp:TextBox ID="txtTrackNo" runat="server" Text='<%# Bind("tune_code") %>'
                                                            CssClass="gridTextField" Width="70%" onchange="OnGridDataChange(this,'TrackNo');" Style="text-align: center;" Enabled="false"></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="revTrackNo" runat="server" Text="*" ControlToValidate="txtTrackNo"
                                                            ValidationExpression="^[+]?\d+$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valUpdate"
                                                            ToolTip="Please enter only integers" Display="Dynamic"> 
                                                        </asp:RegularExpressionValidator>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="12%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Track Title" SortExpression="tune_title">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnTrackTitle" runat="server" Value='<%# Bind("tune_title") %>' />
                                                        <asp:TextBox ID="txtTrackTitle" runat="server" Width="95%" Text='<%#Bind("tune_title")%>' MaxLength="30" onchange="OnGridDataChange(this,'TrackTitle');"
                                                            CssClass="textbox_FuzzySearch" onkeydown="OntxtTrackTitleKeyDown(this);" onfocus="OnGridRowSelected(this,'TrackTitle');" Enabled="false"></asp:TextBox>
                                                        <ajaxToolkit:AutoCompleteExtender ID="aceTrackTitle" runat="server"
                                                            ServiceMethod="FuzzyPartMaintTrackTitleList"
                                                            ServicePath="~/Services/FuzzySearch.asmx"
                                                            MinimumPrefixLength="1"
                                                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                            TargetControlID="txtTrackTitle"
                                                            FirstRowSelected="true"
                                                            OnClientPopulating="trackTitleListPopulating"
                                                            OnClientPopulated="trackTitleListPopulated"
                                                            OnClientHidden="trackTitleListHidden"
                                                            OnClientItemSelected="trackTitleListItemSelected"
                                                            CompletionListElementID="pnlTitleFuzzySearch" />
                                                        <asp:Panel ID="pnlTitleFuzzySearch" runat="server" CssClass="identifierLable" Style="bottom: 0px; top: 1000px; position: absolute; height: 200px;" />
                                                        <asp:CustomValidator ID="valtxtTrackTitle" runat="server" ValidationGroup="valUpdate" CssClass="requiredFieldValidator"
                                                            ClientValidationFunction="ValTrackTitleGridRow" ToolTip="Please select valid track title from the search list"
                                                            ControlToValidate="txtTrackTitle" ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>

                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Escalation Code" SortExpression="esc_code">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnEscalationCode" runat="server" Value='<%# Bind("esc_code") %>' />
                                                        <asp:DropDownList ID="ddlEscalationCode" runat="server" Width="85%" CssClass="ddlStyle" onchange="OnGridDataChange(this,'EscalationCode');"></asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Esc Include Units" SortExpression="inc_in_escalation">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnEscIncludeUnits" runat="server" Value='<%# Bind("inc_in_escalation") %>' />
                                                        <asp:CheckBox ID="cbEscIncludeUnits" runat="server" onclick="OnGridDataChange(this,'EscIncludeUnits');" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Active?" SortExpression="participation_type">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnAutoParticipantActive" runat="server" Value='<%# Bind("participation_type") %>' />
                                                        <asp:CheckBox ID="cbActive" runat="server" onclick="OnGridDataChange(this,'Active');" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" ItemStyle-Width="3%" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imgBtnUndo" runat="server" CommandName="cancelRow" ImageUrl="../Images/cancel_row3.png"
                                                            ToolTip="Cancel" />
                                                        <asp:HiddenField ID="hdnIsModified" runat="server" Value='<%# Bind("is_modified") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-left: 10px; padding-top: 10px">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <table width="98%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="13%" class="gridHeaderStyle_1row">Royaltor</td>
                                                        <td width="11%" class="gridHeaderStyle_1row">Option Period</td>
                                                        <td width="15%" class="gridHeaderStyle_1row">Territory</td>
                                                        <td width="4%" class="gridHeaderStyle_1row">Share</td>
                                                        <td width="4%" class="gridHeaderStyle_1row">Total Share</td>
                                                        <td width="6%" class="gridHeaderStyle_1row">Time</td>
                                                        <td width="6%" class="gridHeaderStyle_1row">Total Time</td>
                                                        <td width="4%" class="gridHeaderStyle_1row">Track No.</td>
                                                        <td width="13%" class="gridHeaderStyle_1row">Track Title</td>
                                                        <td width="5%" class="gridHeaderStyle_1row">Escalation Code</td>
                                                        <td width="5%" class="gridHeaderStyle_1row">Esc Include Units</td>
                                                        <td width="4%" class="gridHeaderStyle_1row">&nbsp</td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:TextBox ID="txtRoyaltorInsert" runat="server" Width="90%" CssClass="textboxStyle" TabIndex="101"
                                                                onkeydown="OntxtRoyaltorInsertKeyDown(this);" OnChange="if (!OnRoyFuzzySearchChangeInsert()) { return false;};"></asp:TextBox>
                                                            <ajaxToolkit:AutoCompleteExtender ID="aceRoyaltorAddRow" runat="server"
                                                                ServiceMethod="FuzzySearchAllRoyaltorList"
                                                                ServicePath="~/Services/FuzzySearch.asmx"
                                                                MinimumPrefixLength="1"
                                                                CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                                TargetControlID="txtRoyaltorInsert"
                                                                FirstRowSelected="true"
                                                                OnClientPopulating="royaltorListPopulatingInsert"
                                                                OnClientPopulated="royaltorListPopulatedInsert"
                                                                OnClientHidden="royaltorListPopulatedInsert"
                                                                OnClientItemSelected="royaltorListItemSelectedInsert"
                                                                CompletionListElementID="pnlRoyFuzzySearchInsert" />
                                                            <asp:Panel ID="pnlRoyFuzzySearchInsert" runat="server" CssClass="identifierLable" Style="bottom: 0px; top: 1000px; position: absolute; height: 200px;" />
                                                            <asp:CustomValidator ID="valtxtRoyaltorInsert" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator"
                                                                ToolTip="Please select valid royaltor from the search list"
                                                                ControlToValidate="txtRoyaltorInsert" ErrorMessage="*" Display="Dynamic" ClientValidationFunction="ValRoyaltorAddRow"></asp:CustomValidator>
                                                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtRoyaltorInsert" ValidationGroup="valGrpAppendAddRow"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please select valid royaltor from the search list" InitialValue="" Display="Dynamic">
                                                            </asp:RequiredFieldValidator>
                                                        </td>
                                                        <td class="insertBoxStyle" align="left">
                                                            <asp:DropDownList ID="ddlOptionPeriodInsert" runat="server" Width="88%" CssClass="ddlStyle" TabIndex="102"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ControlToValidate="ddlOptionPeriodInsert" ValidationGroup="valGrpAppendAddRow"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please select option period" InitialValue="-" Display="Dynamic">
                                                            </asp:RequiredFieldValidator>
                                                        </td>
                                                        <td class="insertBoxStyle" align="left">
                                                            <asp:TextBox ID="txtTerritoryAddRow" runat="server" Width="94%" CssClass="textbox_FuzzySearch" TabIndex="103"
                                                                onkeydown="OntxtTerritoryAddRowKeyDown(this);" onchange="OntxtTerritoryAddRowChange();"></asp:TextBox>
                                                            <ajaxToolkit:AutoCompleteExtender ID="aceTerritoryAddRow" runat="server"
                                                                ServiceMethod="FuzzyPartMaintSellerGrpList"
                                                                ServicePath="~/Services/FuzzySearch.asmx"
                                                                MinimumPrefixLength="1"
                                                                CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                                TargetControlID="txtTerritoryAddRow"
                                                                FirstRowSelected="true"
                                                                OnClientPopulating="territoryAddRowListPopulating"
                                                                OnClientPopulated="territoryAddRowListPopulated"
                                                                OnClientHidden="territoryAddRowListHidden"
                                                                OnClientItemSelected="territoryAddRowListItemSelected"
                                                                CompletionListElementID="pnlTerritoryAddRowFuzzySearch" />
                                                            <asp:Panel ID="pnlTerritoryAddRowFuzzySearch" runat="server" CssClass="identifierLable"
                                                                Style="bottom: 0px; top: 1000px; position: absolute; height: 200px;" />
                                                            <asp:CustomValidator ID="CustomValidator1" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator"
                                                                ClientValidationFunction="ValTerritoryAddRow" ToolTip="Please select valid territory from the search list"
                                                                ControlToValidate="txtTerritoryAddRow" ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:HiddenField ID="hdnShareInsert" runat="server" Value="1" />
                                                            <asp:TextBox ID="txtShareInsert" runat="server" CssClass="textboxStyle"
                                                                onkeyup="OnFocusOutShareInsert();" TabIndex="104" Width="50%" Text="1" Enabled="true"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="revShareInsert" runat="server" Text="*" ControlToValidate="txtShareInsert"
                                                                ValidationExpression="^[+]?\d+$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valGrpAppendAddRow"
                                                                ToolTip="Please enter only integers" Display="Dynamic"> 
                                                            </asp:RegularExpressionValidator>
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:HiddenField ID="hdnTotalShareInsert" runat="server" Value="1" />
                                                            <asp:TextBox ID="txtTotalShareInsert" runat="server" CssClass="textboxStyle"
                                                                onkeyup="OnFocusOutShareInsert();" TabIndex="105" Width="50%" Text="1" Enabled="true"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="revTotalShareInsert" runat="server" Text="*" ControlToValidate="txtTotalShareInsert"
                                                                ValidationExpression="^[+]?\d+$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valGrpAppendAddRow"
                                                                ToolTip="Please enter only integers" Display="Dynamic"> 
                                                            </asp:RegularExpressionValidator>
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:TextBox ID="txtTimeInsert" runat="server" CssClass="textboxStyle" TabIndex="106" Width="70%" MaxLength="7" onkeyup="OnFocusOutShareInsert();" Enabled="true"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="revTimeInsert" runat="server" Text="*" ControlToValidate="txtTimeInsert" ValidationGroup="valGrpAppendAddRow"
                                                                ValidationExpression="^(___:__:__)|([0-9]{0,3}:[0-5][0-9]:[0-5][0-9])$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                ToolTip="Please enter a valid time in hhh:mm:ss format" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                            <asp:CustomValidator ID="valTimeInsert" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator" Display="Dynamic"
                                                                ClientValidationFunction="ValTimeInsertRow" ControlToValidate="txtTimeInsert" ErrorMessage="*" ToolTip="Time Share must not be greater than Total Time">
                                                            </asp:CustomValidator>
                                                            <ajaxToolkit:MaskedEditExtender ID="mteTimeInsert" runat="server"
                                                                TargetControlID="txtTimeInsert" Mask="999:99:99" AcceptNegative="None"
                                                                ClearMaskOnLostFocus="false" />
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:TextBox ID="txtTotalTimeInsert" runat="server" CssClass="textboxStyle" TabIndex="107" Width="70%" MaxLength="10" onkeyup="OnFocusOutShareInsert();" Enabled="true"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="revTotalTimeInsert" runat="server" Text="*" ControlToValidate="txtTotalTimeInsert" ValidationGroup="valGrpAppendAddRow"
                                                                ValidationExpression="^(___:__:__)|([0-9]{0,3}:[0-5][0-9]:[0-5][0-9])$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                ToolTip="Please enter a valid time in hhh:mm:ss format" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                            <asp:CustomValidator ID="CustomValidator2" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator" Display="Dynamic"
                                                                ClientValidationFunction="ValTimeInsertRow" ControlToValidate="txtTotalTimeInsert" ErrorMessage="*" ToolTip="Time Share must not be greater than Total Time">
                                                            </asp:CustomValidator>
                                                            <ajaxToolkit:MaskedEditExtender ID="mteTotalTimeInsert" runat="server"
                                                                TargetControlID="txtTotalTimeInsert" Mask="999:99:99" AcceptNegative="None"
                                                                ClearMaskOnLostFocus="false" />
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:TextBox ID="txtTrackNoInsert" runat="server" CssClass="textboxStyle" TabIndex="108" Width="50%" MaxLength="50" Enabled="false"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="revTrackNoInsert" runat="server" Text="*" ControlToValidate="txtTrackNoInsert"
                                                                ValidationExpression="^[+]?\d+$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valGrpAppendAddRow"
                                                                ToolTip="Please enter only integers" Display="Dynamic"> 
                                                            </asp:RegularExpressionValidator>
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:TextBox ID="txtTrackTitleInsert" runat="server" Width="95%" MaxLength="30" CssClass="textboxStyle" TabIndex="109"
                                                                onkeydown="OntxtTrackTitleAddRowKeyDown(this);" onchange="OntxtTrackTitleAddRowChange();" Enabled="false"></asp:TextBox>
                                                            <ajaxToolkit:AutoCompleteExtender ID="aceTrackTitleAddRow" runat="server"
                                                                ServiceMethod="FuzzyPartMaintTrackTitleList"
                                                                ServicePath="~/Services/FuzzySearch.asmx"
                                                                MinimumPrefixLength="1"
                                                                CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                                TargetControlID="txtTrackTitleInsert"
                                                                FirstRowSelected="true"
                                                                OnClientPopulating="trackTitleListPopulatingInsert"
                                                                OnClientPopulated="trackTitleListPopulatedInsert"
                                                                OnClientHidden="trackTitleListPopulatedInsert"
                                                                OnClientItemSelected="trackTitleListItemSelectedInsert"
                                                                CompletionListElementID="pnlTitleFuzzySearchInsert" />
                                                            <asp:Panel ID="pnlTitleFuzzySearchInsert" runat="server" CssClass="identifierLable" Style="bottom: 0px; top: 1000px; position: absolute; height: 200px;" />
                                                            <asp:CustomValidator ID="valtxtTrackTitleAddRow" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator"
                                                                ClientValidationFunction="ValTrackTitleAddRow" ToolTip="Please select valid track title from the search list"
                                                                ControlToValidate="txtTrackTitleInsert" ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:DropDownList ID="ddlEscCodeInsert" runat="server" Width="87%" CssClass="ddlStyle" TabIndex="110" onchange="OnChangeEscCodeInsert();"></asp:DropDownList>
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:CheckBox ID="cbEscIncludeUnitsInsert" runat="server" TabIndex="111" />
                                                        </td>
                                                        <td class="insertBoxStyle_No_Padding">
                                                            <table width="100%" style="float: right; table-layout: fixed">
                                                                <tr style="float: right">
                                                                    <td align="right" style="float: right" width="50%">
                                                                        <asp:ImageButton ID="imgBtnInsert" runat="server" CommandName="saverow" TabIndex="112" ImageUrl="../Images/newrow.png" ToolTip="Insert participant"
                                                                            OnClientClick="return ValidatePopUpAddRow()" OnClick="imgBtnInsert_Click" Onkeydown="imgBtnInsertKeydown();"/>
                                                                    </td>
                                                                    <td align="right" style="float: right" width="50%">
                                                                        <asp:ImageButton ID="imgBtnCancel" runat="server" CommandName="cancelrow" TabIndex="113" ImageUrl="../Images/cancel_row3.png"
                                                                            ToolTip="Cancel" OnClientClick="return ClearAddRow();" />
                                                                    </td>
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
                            <tr>
                                <td>
                                    <br />
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

            <%--Warning on unsaved data popup--%>
            <asp:Button ID="dummyUnsavedWarnMsg" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeUnSavedWarning" runat="server" PopupControlID="pnlUnsavedWarnMsgPopup" TargetControlID="dummyUnsavedWarnMsg"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlUnsavedWarnMsgPopup" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmOnUnsavedData" runat="server" Text="Unsaved Data Warning" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblUnSavedWarnMsg" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td width="48%" align="right">
                                        <asp:Button ID="btnUnSavedDataReturn" runat="server" Text="Return" CssClass="ButtonStyle" Width="30%" OnClientClick="return OnUnSavedDataReturn();" />
                                    </td>
                                    <td width="4%"></td>
                                    <td width="48%" align="left">
                                        <asp:Button ID="btnUnSavedDataExit" runat="server" Text="Exit" CssClass="ButtonStyle" Width="30%" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnGridDataChanged" runat="server" Value="N" />
            <asp:HiddenField ID="hdnAddRowDataChanged" runat="server" Value="N" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" Value="" />
            <asp:HiddenField ID="hdnGridRoyFuzzySearchRowId" runat="server" />
            <asp:HiddenField ID="hdnNewAutoParticipId" runat="server" Value="-99" />
            <asp:HiddenField ID="hdnIsSuperUser" runat="server" Value="N" />
            <asp:HiddenField ID="hdnTrackTitleInsertEnabled" runat="server" Value="N" />
            <asp:HiddenField ID="hdnFuzzySearchText" runat="server" />
            <asp:HiddenField ID="hdnGridFuzzySearchRowId" runat="server" />
            <asp:HiddenField ID="hdnAutoPartDetailId" runat="server" />
            <asp:HiddenField ID="hdnAutoPartId" runat="server" />
            <asp:Button ID="btnFuzzyTerritoryListPopup" runat="server" Style="display: none;" OnClick="btnFuzzyTerritoryListPopup_Click" CausesValidation="false" />
            <asp:Button ID="btnFuzzyTrackTitleListPopup" runat="server" Style="display: none;" OnClick="btnFuzzyTrackTitleListPopup_Click" CausesValidation="false" />
            <asp:Button ID="btnHdnRoyaltorInsertSearch" runat="server" Style="display: none;" OnClick="btnHdnRoyaltorInsertSearch_Click" CausesValidation="false" />
            <asp:Button ID="btnFuzzyRoyaltorListPopup" runat="server" Style="display: none;" OnClick="btnFuzzyRoyaltorListPopup_Click" CausesValidation="false" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" onkeydown="FocusLblKeyPress();"></asp:Label>
              <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>






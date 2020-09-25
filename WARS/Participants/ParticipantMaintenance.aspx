<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ParticipantMaintenance.aspx.cs" Inherits="WARS.ParticipantMaintenance" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Track Listing" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">

        function MissingParticipScreen() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../Participants/MissingParticipants.aspx?isNewRequest=N');
            }
            else {
                window.location = '../Participants/MissingParticipants.aspx?isNewRequest=N';
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

        //Validate any unsaved data on browser window close/refresh

        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        //End=================Validate any unsaved data on browser window close/refresh
        //================================End

        //WUIN-1181 changes
        function imgBtnInsertKeydown() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=imgBtnInsert.ClientID%>').click();
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
                        <asp:Button ID="btnMissingParticip" runat="server" CssClass="LinkButtonStyle"
                            Text="Missing Participants" UseSubmitBehavior="false" Width="98%" OnClientClick="if (!MissingParticipScreen()) { return false;};" />
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
        var gridClientId = "ContentPlaceHolderBody_gvParicipantDetails_";
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

        }

        function EndRequestHandler(sender, args) {
            //Hide the modal popup - the update progress
            var popup = $find('<%= mPopupPageLevel.ClientID %>');
            if (popup != null) {
                popup.hide();
            }

            //debugger;
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

        }
        //======================= End


        //on page load
        function SetGrdPnlHeightOnLoad() {
            //grid panel height adjustment functioanlity - starts
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.45;
            document.getElementById("<%=PnlGrid.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
            document.getElementById("<%=cbEscIncludeUnitsInsert.ClientID %>").disabled = true;
            document.getElementById("<%=cbEscIncludeUnitsInsert.ClientID %>").checked = true;
        }


        function OpenCatalogueSearch() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../Participants/CatalogueSearch.aspx?isNewRequest=N');
            }
            else {
                window.location = '../Participants/CatalogueSearch.aspx?isNewRequest=N';
            }
        }

        function OpenCatMaintenance() {
            // debugger;
            var catalogueNo = document.getElementById("<%=lblCatNo.ClientID %>").innerHTML;
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Participants/CatalogueMaintenance.aspx?catNo=" + catalogueNo + "")
            }
            else {
                return true;
            }
        }

        function DisplayActive(button) {
            // debugger;
            if (document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value == "Y") {
                return true;
            }
            document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "N";
            if (IsDataChanged()) {
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
            }
            else {
                return true;
            }
        }
        //============== End

        function OnGridDataChange(row, name) {
            //debugger;
            if (name == "Territory") {
                OntxtTerritoryChange(row);
            }

            //var selectedRowIndex = row.id.substring(row.id.length - 1);
            var selectedRowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            //ValidateSuperUser(row, name); //WUIN-1167
            EnableDisableEscInludes(row, name);
            ValidatePaticipantStatus(row, name);
            CompareGridData(selectedRowIndex);


        }

        function EnableDisableEscInludes(row, name) {
            if (name == "EscalationCode") {
                //debugger;
                var rowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
                var str = "ContentPlaceHolderBody_gvParicipantDetails_";
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

        //Validate status of participants
        function ValidatePaticipantStatus(row, name) {
            if (name == "Status") {
                //debugger;
                var rowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
                var str = "ContentPlaceHolderBody_gvParicipantDetails_";
                var ddlStatus = document.getElementById(str + 'ddlStatus' + '_' + rowIndex);
                var currentStatusCode = ddlStatus.value;
                var catStatusCode = document.getElementById("<%=ddlCatStatus.ClientID %>").value;
                var previousStatusCode = document.getElementById(str + 'hdnParticipStatus' + '_' + rowIndex).value;

                var hdnIsSuperUser = document.getElementById("<%=hdnIsSuperUser.ClientID %>").value;
                var hdnIsSupervisor = document.getElementById("<%=hdnIsSupervisor.ClientID %>").value;

                //if (ddlStatus == "3" && hdnIsSuperUser != 'Y') {
                if (((hdnIsSuperUser != 'Y') && (hdnIsSupervisor != 'Y')) && catStatusCode != 3 && currentStatusCode == "3") {
                    var ddlStatus = document.getElementById(str + 'ddlStatus' + '_' + rowIndex);
                    ddlStatus.value = previousStatusCode;
                    DisplayMessagePopup("Only super user and supervisor can change the status to 'Manager Sign Off'!");
                    return;
                }

                if (currentStatusCode == "-") {
                    DisplayMessagePopup("Please select a status");
                    ddlStatus.value = previousStatusCode;
                }
                else if (previousStatusCode == currentStatusCode) {
                }
                else if (previousStatusCode != currentStatusCode) {

                    if (previousStatusCode == "1" && currentStatusCode != "2") {
                        DisplayMessagePopup("Status 'Under Review' can only be changed to 'Team Sign Off'");
                        ddlStatus.value = previousStatusCode;
                    }
                    else if (previousStatusCode == "2" && currentStatusCode == "0") {
                        DisplayMessagePopup("Status 'Team Sign Off' can either be changed to 'Under Review' or 'Manager Sign Off'");
                        ddlStatus.value = previousStatusCode;
                    }
                    else if (previousStatusCode == "3" && currentStatusCode == "0") {
                        DisplayMessagePopup("Status 'Manager Sign Off' can either be changed to 'Under Review' or 'Team Sign Off'");
                        ddlStatus.value = previousStatusCode;
                    }

                }
            }
        }

        function ValidateClick(row) {

            var rowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            var str = "ContentPlaceHolderBody_gvParicipantDetails_";
            var hdnIsSuperUser = document.getElementById("<%=hdnIsSuperUser.ClientID %>").value;
            //JIRA-983 Changes by Ravi on 20/02/2019 -- Start
            var hdnIsSupervisor = document.getElementById("<%=hdnIsSupervisor.ClientID %>").value;
            var ddlStatus = document.getElementById(str + 'ddlStatus' + '_' + rowIndex).value;
            var ddlCatStatus = document.getElementById("<%=ddlCatStatus.ClientID %>").value;
            if ((ddlStatus == "3" || ddlCatStatus == "3") && ((hdnIsSuperUser != 'Y') && (hdnIsSupervisor != 'Y'))) {
                DisplayMessagePopup("This row can be edited only by super user and supervisor!");
                //JIRA-983 Changes by Ravi on 20/02/2019 -- End
                return false;
            }
            else { return true; }
        }

        //Validate super user if status is 'Manager Sign Off'
        //Harish 18-06-18: WUIN-696 - modified this as this is not working if catno status is < 3 and participant status is 3
        function ValidateSuperUser(row, name) {
            var rowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            var str = "ContentPlaceHolderBody_gvParicipantDetails_";
            var hdnCatStatus = document.getElementById("<%=hdnStatusCode.ClientID %>").value;
            var hdnParticipStatus = document.getElementById(str + 'hdnParticipStatus' + '_' + rowIndex).value;
            var hdnIsSuperUser = document.getElementById("<%=hdnIsSuperUser.ClientID %>").value;
            //JIRA-983 Changes by Ravi on 20/02/2019 -- Start
            var hdnIsSupervisor = document.getElementById("<%=hdnIsSupervisor.ClientID %>").value;
            var ddlStatus = document.getElementById(str + 'ddlStatus' + '_' + rowIndex).value;
            var ddlCatStatus = document.getElementById("<%=ddlCatStatus.ClientID %>").value;

            //debugger;
            //if (ddlStatus == "3" && hdnIsSuperUser != 'Y') {
            if (((hdnIsSuperUser != 'Y') && (hdnIsSupervisor != 'Y')) && hdnParticipStatus != 3 && ddlStatus == "3") {
                var ddlStatus = document.getElementById(str + 'ddlStatus' + '_' + rowIndex);
                ddlStatus.value = hdnParticipStatus;
                DisplayMessagePopup("Only super user and supervisor can change the status to 'Manager Sign Off'!");
                return;
            }

            //WUIN-540                        
            //if ((ddlStatus == "3" || ddlCatStatus == "3") && (hdnIsSuperUser != 'Y')) {
            if (((hdnIsSuperUser != 'Y') && (hdnIsSupervisor != 'Y')) && (hdnCatStatus == "3" || hdnParticipStatus == "3")) {
                DisplayMessagePopup("This row can be edited by super user and supervisor only!");
                //JIRA-983 Changes by Ravi on 20/02/2019 -- End
                if (name == "OptionPeriod") {
                    var hdnParticipOptionPeriod = document.getElementById(str + 'hdnParticipOptionPeriod' + '_' + rowIndex).value;
                    var ddlOptionPeriod = document.getElementById(str + 'ddlOptionPeriod' + '_' + rowIndex);

                    ddlOptionPeriod.value = hdnParticipOptionPeriod;
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
                else if (name == "Status") {
                    var ddlStatus = document.getElementById(str + 'ddlStatus' + '_' + rowIndex);
                    ddlStatus.value = hdnParticipStatus;
                }
                else if (name == "Active") {
                    var hdnParticipActive = document.getElementById(str + 'hdnParticipActive' + '_' + rowIndex).value;
                    var cbActive = document.getElementById(str + 'cbActive' + '_' + rowIndex);
                    if (hdnParticipActive == 'A') {
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
            //debugger;
            var str = "ContentPlaceHolderBody_gvParicipantDetails_";
            var hdnParticipOptionPeriod = document.getElementById(str + 'hdnParticipOptionPeriod' + '_' + rowIndex).value;
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
            var hdnParticipActive = document.getElementById(str + 'hdnParticipActive' + '_' + rowIndex).value;

            var hdnEscIncludeUnits = document.getElementById(str + 'hdnEscIncludeUnits' + '_' + rowIndex).value;
            var hdnParticipStatus = document.getElementById(str + 'hdnParticipStatus' + '_' + rowIndex).value;
            var ddlStatus = document.getElementById(str + 'ddlStatus' + '_' + rowIndex).value;
            var hdnIsModified = document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).value;
            var hdnConfigCode = document.getElementById("<%=hdnConfigCode.ClientID %>").value;
            var hdnTrackTime = document.getElementById("<%=hdnTimeTrack.ClientID %>").value;

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
            if (hdnIsModified != "-") {
                if (hdnParticipOptionPeriod != ddlOptionPeriod || hdnSellerGrp != txtTerritory
                    || hdnShare != txtShare || hdnTotalShare != txtTotalShare || hdnTime != txtTime.replace(':', '').replace(':', '').trim() || hdnTotalTime != txtTotalTime.replace(':', '').replace(':', '').trim()
                    || hdnTrackNo != txtTrackNo || hdnTrackTitle != txtTrackTitle || hdnEscalationCode != ddlEscalationCode
                    || hdnEscIncludeUnits != escIncludeUnits || hdnParticipActive != isActive || hdnParticipStatus != ddlStatus) {

                    document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).innerText = "Y";
                }
                else {
                    document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).innerText = "N";
                }
            }
        }

        //clear add row data
        function ClearAddRow() {
            document.getElementById('<%=txtRoyaltorInsert.ClientID%>').value = "";

            document.getElementById('<%=ddlOptionPeriodInsert.ClientID%>').innerHTML = "";
            var listItem1 = document.createElement('option');
            listItem1.text = listItem1.value = "-";
            document.getElementById('<%=ddlOptionPeriodInsert.ClientID%>').add(listItem1);

            var ddlEscCodeInsert = document.getElementById("<%= ddlEscCodeInsert.ClientID %>");
            ddlEscCodeInsert.innerHTML = "";

            var listItem2 = document.createElement('option');
            listItem2.text = listItem2.value = "-";
            ddlEscCodeInsert.add(listItem2);

            document.getElementById('<%=txtTerritoryAddRow.ClientID%>').value = "";
            document.getElementById('<%=txtShareInsert.ClientID%>').value = document.getElementById('<%=hdnShareInsert.ClientID%>').value;
            document.getElementById('<%=txtTotalShareInsert.ClientID%>').value = document.getElementById('<%=hdnTotalShareInsert.ClientID%>').value;;
            document.getElementById('<%=txtTimeInsert.ClientID%>').value = '___:__:__';
            document.getElementById('<%=txtTotalTimeInsert.ClientID%>').value = '___:__:__';
            document.getElementById('<%=txtTrackNoInsert.ClientID%>').value = "";
            //WUIN-1074 Changes by Rakesh - Start
            document.getElementById('<%=txtTrackTitleInsert.ClientID%>').disabled = true;
            document.getElementById('<%=txtTrackNoInsert.ClientID%>').disabled = true;
            //WUIN-1074 Changes by Rakesh - End
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
            var hdnConfigCode = document.getElementById("<%=hdnConfigCode.ClientID %>").value;
            var hdnTrackTime = document.getElementById("<%=hdnTimeTrack.ClientID %>").value;

            if (!Page_ClientValidate("valAddRow")) {
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
                DisplayMessagePopup("Paricipant details not saved – invalid or missing data!");
                return false;
            }
            else {
                return true;
            }
        }

        //validations ========== End

        function IsGridDataChanged() {
            //debugger;
            var str = "ContentPlaceHolderBody_gvParicipantDetails_";
            var gvParicipantDetails = document.getElementById("<%= gvParicipantDetails.ClientID %>");
            if (gvParicipantDetails != null) {
                var gvRows = gvParicipantDetails.rows;// WUIN-746 grid view rows including header row
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
            //debugger;           
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

            if (txtRoyaltorInsert != '' || ddlOptionPeriodInsert != '-' || txtTerritoryAddRow != '' ||
                txtShareInsert != hdnShareInsert || txtTotalShareInsert != hdnTotalShareInsert || txtTimeInsert != '___:__:__' || txtTotalTimeInsert != '___:__:__'
                || txtTrackNoInsert != '' || txtTrackTitleInsert != '' || ddlEscCodeInsert != '-' || escIncludeUnitsInsert != 'Y') {
                document.getElementById("<%=hdnAddRowDataChanged.ClientID %>").innerText = "Y";
            }
            else {
                document.getElementById("<%=hdnAddRowDataChanged.ClientID %>").innerText = "N";
            }


        }

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
            IsGridDataChanged();
            IsAddRowDataChanged();
            var isGridDataChanged = document.getElementById("<%=hdnGridDataChanged.ClientID %>").value;
            var isAddRowDataChanged = document.getElementById("<%=hdnAddRowDataChanged.ClientID %>").value;
            var ddlCatStatus = document.getElementById("<%=ddlCatStatus.ClientID %>").value;
            var hdnStatusCode = document.getElementById("<%=hdnStatusCode.ClientID %>").value;
            if (isGridDataChanged == "Y" || isAddRowDataChanged == "Y" || (ddlCatStatus != hdnStatusCode && ddlCatStatus != "-")) {
                return true;
            }
            else {
                return false;
            }
        }

        function OpenAuditScreen() {
            //debugger;
            var catalogueNo = document.getElementById("<%=lblCatNo.ClientID %>").innerHTML;
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Audit/ParticipantAudit.aspx?CatNo=" + catalogueNo + "&pageName=" + "ParticipantMaint" + "")

            }
            else {
                return true;
            }
        }
        //============== End

        //Enable track no and track title based on share and totalshare
        function OnFocusOutShare(row) {
            var rowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            var str = "ContentPlaceHolderBody_gvParicipantDetails_";
            var hdnShare = document.getElementById(str + 'hdnShare' + '_' + rowIndex).value;
            var hdnTotalShare = document.getElementById(str + 'hdnTotalShare' + '_' + rowIndex).value;
            var txtShare = document.getElementById(str + 'txtShare' + '_' + rowIndex).value;
            var txtTotalShare = document.getElementById(str + 'txtTotalShare' + '_' + rowIndex).value;
            var txtTrackNo = document.getElementById(str + 'txtTrackNo' + '_' + rowIndex);
            var txtTrackTitle = document.getElementById(str + 'txtTrackTitle' + '_' + rowIndex);
            var hdnTrackNo = document.getElementById(str + 'hdnTrackNo' + '_' + rowIndex).value;
            var hdnConfigCode = document.getElementById("<%=hdnConfigCode.ClientID %>").value;
            var hdnTrackTime = document.getElementById("<%=hdnTimeTrack.ClientID %>").value;
            var hdnTrackTitle = document.getElementById(str + 'hdnTrackTitle' + '_' + rowIndex).value;

            if ((txtShare == 1 && txtTotalShare > 1) || (hdnTrackTime == "M") || (hdnConfigCode == "S")) {
                txtTrackTitle.readOnly = true;
                document.getElementById(str + 'txtTrackNo' + '_' + rowIndex).value = "";
                document.getElementById(str + 'txtTrackTitle' + '_' + rowIndex).value = "";
                document.getElementById(str + 'hdnTrackNoTemp' + '_' + rowIndex).value = "";
                document.getElementById(str + 'txtTrackNo' + '_' + rowIndex).focus();

            }
            else if ((txtShare >= 1 && txtTotalShare >= 1) || (hdnTrackTime == "M") || (hdnConfigCode == "S")) {
                txtTrackTitle.readOnly = false;
                document.getElementById(str + 'txtTrackNo' + '_' + rowIndex).value = "";
                document.getElementById(str + 'txtTrackTitle' + '_' + rowIndex).value = "";
                document.getElementById(str + 'hdnTrackNoTemp' + '_' + rowIndex).value = "";
                document.getElementById(str + 'hdnISRCVal' + '_' + rowIndex).value = "";
                document.getElementById(str + 'hdnTrackListingId' + '_' + rowIndex).value = "";
                document.getElementById(str + 'hdnIsISRCTrackTitle' + '_' + rowIndex).value = "N";
                document.getElementById(str + 'txtTrackNo' + '_' + rowIndex).focus();
            }
        }

        function OnFocusOutShareInsert() {
            //debugger;
            var txtShareInsert = document.getElementById("<%=txtShareInsert.ClientID %>").value;
            var txtTotalShareInsert = document.getElementById("<%=txtTotalShareInsert.ClientID %>").value;
            var txtTrackNoInsert = document.getElementById("<%=txtTrackNoInsert.ClientID %>");
            var txtTrackTitleInsert = document.getElementById("<%=txtTrackTitleInsert.ClientID %>");
            var hdnTrackTime = document.getElementById("<%=hdnTimeTrack.ClientID %>").value;
            var hdnConfigCode = document.getElementById("<%=hdnConfigCode.ClientID %>").value;

            if ((txtShareInsert == 1 && txtTotalShareInsert > 1) || (hdnTrackTime == "M") || (hdnConfigCode == "S")) {
                txtTrackTitleInsert.readOnly = true;
                txtTrackNoInsert.value = "";
                txtTrackTitleInsert.value = "";
                document.getElementById("<%= hdnTrackNoChangeInsert.ClientID %>").value = "";
                txtTrackNoInsert.focus();
            }
            else if ((txtShareInsert >= 1 && txtTotalShareInsert >= 1) || (hdnTrackTime == "M") || (hdnConfigCode == "S")) {
                txtTrackNoInsert.value = "";
                txtTrackTitleInsert.value = "";
                txtTrackTitleInsert.readOnly = false;
                txtTrackNoInsert.focus();
                document.getElementById("<%= hdnTrackNoChangeInsert.ClientID %>").value = "";
                document.getElementById("<%=hdnISRCValInsert.ClientID %>").value = "";
                document.getElementById("<%=hdnTrackListingIdInsert.ClientID %>").value = "";
                document.getElementById("<%=hdnIsISRCTrackTitleInsert.ClientID %>").value = "N";

            }
    }

    //=========End
    function OnTrackNoEnterKeyInsert() {
        var txtTrackNoInsert = document.getElementById("<%= txtTrackNoInsert.ClientID %>").value;
        var txtShareInsert = document.getElementById("<%=txtShareInsert.ClientID %>").value;
        var txtTotalShareInsert = document.getElementById("<%=txtTotalShareInsert.ClientID %>").value;
        var txtTrackTitleInsert = document.getElementById("<%= txtTrackTitleInsert.ClientID %>").value;
        var hdnTrackNoChangeInsert = document.getElementById("<%= hdnTrackNoChangeInsert.ClientID %>").value;

        if (txtShareInsert == 1 && txtTotalShareInsert > 1) {
            if ((txtTrackNoInsert != hdnTrackNoChangeInsert) && txtTrackNoInsert != "") {
                if (event.keyCode == 13 || event.keyCode == 9) {
                    document.getElementById("<%=hdnTrackNoChangeInsert.ClientID %>").value = txtTrackNoInsert;
                    window.onbeforeunload = null;
                    __doPostBack("txtTrackNoInsert", "txtTrackNoInsert_OnTextChanged");
                }
            }
        }
    }
    //prevent page navigation to previous page on back space key press when track title is readonly.
    function OnTrackTitleInsertBackSapce() {
        var txtTrackTitleInsert = document.getElementById("<%= txtTrackTitleInsert.ClientID %>");
        if (txtTrackTitleInsert.readOnly && event.keyCode == 8) {
            event.returnValue = false;
        }
    }

    function ClearTrackTitleAddRow() {
        //WUIN-1074
        var txtTrackNoInsert = document.getElementById("<%= txtTrackNoInsert.ClientID %>").value;
        var txtTrackTitleInsert = document.getElementById("<%= txtTrackTitleInsert.ClientID %>").value;
        var hdnTrackNoChangeInsert = document.getElementById("<%= hdnTrackNoChangeInsert.ClientID %>").value;

        if (txtTrackNoInsert != hdnTrackNoChangeInsert) {
            document.getElementById("<%= txtTrackTitleInsert.ClientID %>").value = "";
        }
    }


    //prevent page navigation to previous page on back space key press when track title is readonly.
    function OnTrackTitleBackSapce(row) {
        var rowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
        var str = "ContentPlaceHolderBody_gvParicipantDetails_";
        var txtTrackTitle = document.getElementById(str + 'txtTrackTitle' + '_' + rowIndex);
        if (txtTrackTitle.readOnly && event.keyCode == 8) {
            event.returnValue = false;
        }
    }

    function OnTrackNoEnterKeyChange(row) {
        var rowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
        var str = "ContentPlaceHolderBody_gvParicipantDetails_";
        var txtTrackNo = document.getElementById(str + 'txtTrackNo' + '_' + rowIndex).value;
        var hdnTrackNoTemp = document.getElementById(str + 'hdnTrackNoTemp' + '_' + rowIndex).value;
        if (hdnTrackNoTemp == "") {
            hdnTrackNoTemp = document.getElementById(str + 'hdnTrackNo' + '_' + rowIndex).value;
        }
        var txtTrackTitle = document.getElementById(str + 'txtTrackTitle' + '_' + rowIndex);
        var txtShare = document.getElementById(str + 'txtShare' + '_' + rowIndex).value;
        var txtTotalShare = document.getElementById(str + 'txtTotalShare' + '_' + rowIndex).value;

        if (txtShare == 1 && txtTotalShare > 1) {
            if (txtTrackNo != hdnTrackNoTemp && txtTrackNo != "") {
                if (event.keyCode == 13 || event.keyCode == 9) {
                    document.getElementById("<%=hdnTrackNoChange.ClientID %>").value = txtTrackNo;
                document.getElementById("<%=hdnTrackNoRow.ClientID %>").value = rowIndex;
                window.onbeforeunload = null;
                __doPostBack("txtTrackNo", "txtTrackNo_OnTextChanged");
            }
        }
    }
}

function clearTrackTitleGridRow(row) {
    //WUIN-1074
    var rowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
    var str = "ContentPlaceHolderBody_gvParicipantDetails_";
    var hdnTrackNo = document.getElementById(str + 'hdnTrackNo' + '_' + rowIndex).value;
    var txtTrackNo = document.getElementById(str + 'txtTrackNo' + '_' + rowIndex).value;
    var txtTrackNoTemp = document.getElementById(str + 'hdnTrackNoTemp' + '_' + rowIndex).value;
    if (txtTrackNoTemp == "") {
        txtTrackNoTemp = document.getElementById(str + 'hdnTrackNo' + '_' + rowIndex).value;
    }
    if (txtTrackNo != txtTrackNoTemp && txtTrackNoTemp != "") {
        document.getElementById(str + 'txtTrackTitle' + '_' + rowIndex).value = "";
    }
}



function CloseShowNoTrackMsgPopup() {
    var str = "ContentPlaceHolderBody_gvParicipantDetails_";
    var rowIndex = document.getElementById("<%=hdnTrackNoRow.ClientID %>").value;
    var popup = $find('<%= mpeShowNoTrackMsg.ClientID %>');
    if (popup != null) {
        popup.hide();
    }
    if (document.getElementById("<%=hdnFocusControl.ClientID %>").value == "txtTrackNo") {
        document.getElementById(str + 'txtTrackNo' + '_' + rowIndex).focus();
    }
    else {
        document.getElementById("<%=txtTrackNoInsert.ClientID %>").focus();
    }
    return false;
}

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

//Validate super user if status of Catalogue is 'Manager Sign Off'
function ValidateCatalogueStatus() {
    var participantValue;
    var valueFound = false;
    var ddlCatStatus = document.getElementById("<%=ddlCatStatus.ClientID %>").value;
            var hdnStatusCode = document.getElementById("<%=hdnStatusCode.ClientID %>").value;
            var hdnIsSuperUser = document.getElementById("<%=hdnIsSuperUser.ClientID %>").value;
            //JIRA-983 Changes by Ravi on 20/02/2019 -- Start
            var hdnIsSupervisor = document.getElementById("<%=hdnIsSupervisor.ClientID %>").value;
            var str = "ContentPlaceHolderBody_gvParicipantDetails_";
            var gvParicipantDetails = document.getElementById("<%= gvParicipantDetails.ClientID %>");
            if (ddlCatStatus == "3" && ((hdnIsSuperUser != 'Y') && (hdnIsSupervisor != 'Y'))) {
                document.getElementById("<%=ddlCatStatus.ClientID %>").value = hdnStatusCode;
        DisplayMessagePopup("Only super user and supervisor can change the status to 'Manager Sign Off'!");
        //JIRA-983 Changes by Ravi on 20/02/2019 -- End
        return;
    }
    else {
        if (gvParicipantDetails != null) {
            var gvRows = gvParicipantDetails.rows;// WUIN-746 grid view rows including header row
            var rowIndex;
            for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                rowIndex = i - 1; //WUIN-746 row index start from 0
                if (document.getElementById(str + 'ddlStatus' + '_' + rowIndex) != null) {
                    participantValue = document.getElementById(str + 'ddlStatus' + '_' + rowIndex).value;
                    if (participantValue != ddlCatStatus) {
                        valueFound = true;
                        break;
                    }

                }
            }
            if (valueFound == true) {
                //JIRA-908 CHanges --Start
                var popup = $find('<%= mpeConfirmation.ClientID %>');
                if (popup != null) {
                    popup.show();
                }
            }
            //JIRA-908 CHanges --End


        }
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
    document.getElementById("<%= ddlCatStatus.ClientID %>").focus();
}

//=============== End

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
            document.getElementById('<%=btnFuzzyTerritoryListPopup.ClientID%>').click();
        }
    }
}

//Validate if the field value is a valid one from fuzzy search list
function ValTerritoryGridRow(sender, args) {
    gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
    txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + gridRowIndex);

    if (txtTerritory.value == "") {
        args.IsValid = true;
        txtTerritory.style["width"] = '98%';
    }
    else if (txtTerritory.value == "No results found") {
        args.IsValid = true;
        txtTerritory.value = "";
        txtTerritory.style["width"] = '98%';
    }
    else if (txtTerritory.value != "" && txtTerritory.value.indexOf('-') == -1) {
        args.IsValid = false;
        //adjust width of the textbox to display error
        fieldWidth = txtTerritory.offsetWidth;
        txtTerritory.style["width"] = (fieldWidth - 20);
    }
    else if (args.IsValid == true) {
        txtTerritory.style["width"] = '98%';
    }
}

//reset field width when empty
function OntxtTerritoryChange(sender) {
    gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
    txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + gridRowIndex);

    if (txtTerritory.value == "") {
        txtTerritory.style["width"] = '98%';
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
        txtRoyaltorInsert.style["width"] = '97%';
    }
}

//Validate if the field value is a valid one from fuzzy search list
function ValRoyaltorAddRow(sender, args) {
    txtRoyaltorInsert = document.getElementById("<%=txtRoyaltorInsert.ClientID %>");
        if (txtRoyaltorInsert.value == "") {
            args.IsValid = true;
            txtRoyaltorInsert.style["width"] = '97%';
        }
        else if (txtRoyaltorInsert.value == "No results found") {
            args.IsValid = true;
            txtRoyaltorInsert.value = "";
            txtRoyaltorInsert.style["width"] = '97%';
        }
        else if (txtRoyaltorInsert.value != "" && txtRoyaltorInsert.value.indexOf('-') == -1) {
            args.IsValid = false;
            //adjust width of the textbox to display error
            fieldWidth = txtRoyaltorInsert.offsetWidth;
            txtRoyaltorInsert.style["width"] = (fieldWidth - 20);
        }
        else if (args.IsValid == true) {
            txtRoyaltorInsert.style["width"] = '97%';
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

//Royaltor Add row fuxxy search ====== End ==

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
            txtTerritoryAddRow.style["width"] = (fieldWidth - 20);
        }
        else if (args.IsValid == true) {
            txtTerritoryAddRow.style["width"] = '98%';
        }
    }

    //Territory Add row fuzzy search ======= End ====

    //WUIN-745 - validation
    //Total time on each Participant must not be > Total Time on CATNO
    function ValTotalTimeGridRow(sender, args) {
        gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
        catnoTotalTime = "000:00:00";
        txtTotalTime = document.getElementById(gridClientId + 'txtTotalTime' + '_' + gridRowIndex);

        args.IsValid = true;

        if (txtTotalTime.disabled == false) {
            lblCatTotalTime = document.getElementById("<%=lblCatTotalTime.ClientID %>").innerHTML;
                if (lblCatTotalTime != "") {
                    catnoTotalTime = lblCatTotalTime;
                }

                if (CompareTimes(txtTotalTime.value, catnoTotalTime)) {
                    args.IsValid = false;
                }
            }
        }

        //WUIN-745 - validation        
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
        // WUIN-662 - confirmation on un saved data
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

    function OnUnSavedDataExit() {
        document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "Y";
        window.onbeforeunload = WarnOnUnSavedData;
    }
    //============== End

    function UpdateStatusConfirmYes() {
        var ddlCatStatus = document.getElementById("<%=ddlCatStatus.ClientID %>").value;
        var hdnStatusCode = document.getElementById("<%=hdnStatusCode.ClientID %>").value;
        var str = "ContentPlaceHolderBody_gvParicipantDetails_";
        var gvParicipantDetails = document.getElementById("<%= gvParicipantDetails.ClientID %>");
    if (gvParicipantDetails != null) {
        var gvRows = gvParicipantDetails.rows;// WUIN-746 grid view rows including header row
        var rowIndex;
        for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
            rowIndex = i - 1; //WUIN-746 row index start from 0
            if (document.getElementById(str + 'ddlStatus' + '_' + rowIndex).value != ddlCatStatus) { //WUIN-1286
                document.getElementById(str + 'ddlStatus' + '_' + rowIndex).value = ddlCatStatus;
                if (document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).value != "-") {
                    CompareGridData(rowIndex);
                }
            }

        }
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
                                    PARTICIPANT MAINTENANCE
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="2"></td>
                </tr>
                <tr>
                    <td valign="top">
                        <table width="100%" class="table_with_border">
                            <tr>
                                <td style="padding: 10px">
                                    <table width="99.9%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td width="10%" class="gridHeaderStyle_1row">Catalogue No</td>
                                            <td width="20%" class="gridHeaderStyle_1row">Title</td>
                                            <td width="15%" class="gridHeaderStyle_1row">Artist</td>
                                            <td width="15%" class="gridHeaderStyle_1row">Deal Type</td>
                                            <td width="6%" class="gridHeaderStyle_1row">Compilation?</td>
                                            <td width="6%" class="gridHeaderStyle_1row">Total Tracks</td>
                                            <td width="6%" class="gridHeaderStyle_1row">Total Time</td>
                                            <td width="10%" class="gridHeaderStyle_1row">Track / Time Share</td>
                                            <td width="12%" class="gridHeaderStyle_1row">Status</td>
                                        </tr>
                                        <tr>
                                            <td class="insertBoxStyle">
                                                <asp:Label ID="lblCatNo" runat="server" CssClass="identifierLable" Width="97%"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:Label ID="lblCatTitle" runat="server" CssClass="identifierLable" Width="97%"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:Label ID="lblCatArtist" runat="server" CssClass="identifierLable" Width="97%"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:Label ID="lblCatDealType" runat="server" CssClass="identifierLable" Width="97%"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:CheckBox ID="cbCatCompilation" runat="server" Enabled="false" />
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:Label ID="lblCatTotalTracks" runat="server" CssClass="identifierLable" Width="97%"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:Label ID="lblCatTotalTime" runat="server" CssClass="identifierLable" Width="97%"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:Label ID="lblTimeTrackShare" runat="server" CssClass="identifierLable" Width="97%"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle" style="padding-top: 3px; padding-bottom: 3px;">
                                                <asp:DropDownList ID="ddlCatStatus" runat="server" Width="90%" CssClass="ddlStyle" TabIndex="100" onChange="ValidateCatalogueStatus();">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvCatStatus" ControlToValidate="ddlCatStatus" ValidationGroup="valUpdate"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please select status" InitialValue="-" Display="Dynamic">
                                                </asp:RequiredFieldValidator>
                                            </td>

                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <asp:HiddenField ID="hdnStatusCode" runat="server" />
                        </table>
                    </td>
                    <td width="12%" rowspan="2" valign="top" align="right">
                        <table width="100%">
                            <tr>
                                <td>
                                    <asp:Button ID="btnSaveChanges" runat="server" CssClass="ButtonStyle" OnClick="btnSaveChanges_Click"
                                        Text="Save Changes" UseSubmitBehavior="false" Width="90%" OnClientClick="if (!ValidateSaveChanges()) { return false;};" TabIndex="115" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="btnAudit" runat="server" CssClass="ButtonStyle" OnClientClick="if (!OpenAuditScreen()) { return false;};"
                                        OnClick="btnAudit_Click" Text="Audit" Width="90%" TabIndex="116" UseSubmitBehavior="false" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="btnCatSearch" runat="server" CssClass="ButtonStyle" OnClientClick="if (!OpenCatalogueSearch()) { return false;};"
                                        Text="Catalogue Search" UseSubmitBehavior="false" Width="90%" TabIndex="117" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="btnCatMaintenance" runat="server" CssClass="ButtonStyle" OnClientClick="if (!OpenCatMaintenance()) { return false;};" OnClick="btnCatMaintenance_Click"
                                        Text="Catalogue Maintenance" UseSubmitBehavior="false" Width="90%" TabIndex="118" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="btnDisplayActive" runat="server" CssClass="ButtonStyle" OnClientClick="if (!DisplayActive('DisplayActive')) { return false;};" OnClick="btnDisplayActive_Click"
                                        Text="Display Active" UseSubmitBehavior="false" Width="90%" TabIndex="119" OnKeyDown="OnTabPress()" />
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
                    <td class="table_header_with_border" valign="top" colspan="2" style="padding-left: 15px">Participant details
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table width="100%" class="table_with_border" cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-left: 10px; padding-bottom: 10px">
                                    <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="100%">
                                        <asp:GridView ID="gvParicipantDetails" runat="server" AutoGenerateColumns="False" Width="98%" AlternatingRowStyle-BackColor="#E3EFFF"
                                            CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" ShowHeaderWhenEmpty="true" EmptyDataText="No Data Found"
                                            EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" OnRowDataBound="gvParicipantDetails_RowDataBound" OnRowCommand="gvParicipantDetails_RowCommand"
                                            AllowSorting="true" OnSorting="gvParicipantDetails_Sorting" HeaderStyle-CssClass="FixedHeader">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-Width="13%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Royaltor" SortExpression="royaltor">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnParticipId" runat="server" Value='<%# Bind("participation_id") %>' />
                                                        <asp:HiddenField ID="hdnParticipRoyId" runat="server" Value='<%# Bind("royaltor_id") %>' />
                                                        <asp:Label ID="lblRoyaltor" runat="server" Text='<%# Bind("royaltor") %>' CssClass="identifierLable"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="11%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Option Period" SortExpression="option_period_code">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnParticipOptionPeriod" runat="server" Value='<%# Bind("option_period_code") %>' />
                                                        <asp:DropDownList ID="ddlOptionPeriod" runat="server" Width="90%" TabIndex="101" CssClass="ddlStyle" onchange="OnGridDataChange(this,'OptionPeriod');"></asp:DropDownList>
                                                        <asp:RequiredFieldValidator runat="server" ID="rfvOptionPeriod" ControlToValidate="ddlOptionPeriod" ValidationGroup="valUpdate"
                                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please select option period" InitialValue="-" Display="Dynamic">
                                                        </asp:RequiredFieldValidator>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="15%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Territory" SortExpression="seller_group">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnSellerGrp" runat="server" Value='<%# Bind("seller_group") %>' />
                                                        <asp:TextBox ID="txtTerritory" runat="server" Width="98%" Text='<%#Bind("seller_group")%>' TabIndex="101" CssClass="textbox_FuzzySearch"
                                                            ToolTip='<%#Bind("seller_group")%>' onkeydown="OntxtTerritoryKeyDown(this);" onchange="OnGridDataChange(this,'Territory');"></asp:TextBox>
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
                                                        <asp:CustomValidator ID="valtxtTerritory" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator"
                                                            ClientValidationFunction="ValTerritoryGridRow" ToolTip="Please select valid territory from the search list"
                                                            ControlToValidate="txtTerritory" ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Share" SortExpression="share_tracks">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnShare" runat="server" Value='<%# Bind("share_tracks") %>' />
                                                        <asp:TextBox ID="txtShare" runat="server" Text='<%# Eval("share_tracks") %>'
                                                            CssClass="gridTextField" Width="70%" TabIndex="101" onchange="OnGridDataChange(this,'Share'); OnFocusOutShare(this);" Style="text-align: center;"></asp:TextBox>
                                                        <asp:CustomValidator ID="valShare" runat="server" ValidationGroup="valUpdate" CssClass="requiredFieldValidator" Display="Dynamic"
                                                            ClientValidationFunction="ValShareGridRow" ErrorMessage="*" ToolTip="Share must not be greater than Total Share or participation multiplier should not be greater than 1"></asp:CustomValidator>
                                                        <asp:RegularExpressionValidator ID="revShare" runat="server" Text="*" ControlToValidate="txtShare"
                                                            ValidationExpression="^[1-9][0-9]*$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valUpdate"
                                                            ToolTip="Please enter only integers" Display="Dynamic"> 
                                                        </asp:RegularExpressionValidator>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Total Share" SortExpression="share_total_tracks">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnTotalShare" runat="server" Value='<%# Bind("share_total_tracks") %>' />
                                                        <asp:TextBox ID="txtTotalShare" runat="server" Text='<%# Eval("share_total_tracks") %>'
                                                            CssClass="gridTextField" Width="70%" TabIndex="101" onchange="OnGridDataChange(this,'TotalShare'); OnFocusOutShare(this);" Style="text-align: center;"></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="revTotalShare" runat="server" Text="*" ControlToValidate="txtTotalShare"
                                                            ValidationExpression="^[1-9][0-9]*$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valUpdate"
                                                            ToolTip="Please enter only integers" Display="Dynamic"> 
                                                        </asp:RegularExpressionValidator>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Time" SortExpression="share_time">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnTime" runat="server" Value='<%# Bind("share_time") %>' />
                                                        <asp:TextBox ID="txtTime" runat="server" Text='<%# Eval("share_time") %>'
                                                            CssClass="gridTextField" Width="80%" MaxLength="7" TabIndex="101" onchange="OnGridDataChange(this,'Time');" Style="text-align: center;"></asp:TextBox>
                                                        <asp:CustomValidator ID="valTime" runat="server" ValidationGroup="valUpdate" CssClass="requiredFieldValidator" Display="Dynamic"
                                                            ClientValidationFunction="ValTimeGridRow" ErrorMessage="*" ToolTip="Time Share must not be greater than Total Time or participation multiplier should not be greater than 1">
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
                                                        <asp:TextBox ID="txtTotalTime" runat="server" Text='<%# Eval("share_total_time") %>'
                                                            CssClass="gridTextField" Width="80%" MaxLength="10" TabIndex="101" onchange="OnGridDataChange(this,'TotalTime');" Style="text-align: center;"></asp:TextBox>
                                                        <asp:CustomValidator ID="valTotalTime" runat="server" ValidationGroup="valUpdate" CssClass="requiredFieldValidator" Display="Dynamic"
                                                            ClientValidationFunction="ValTotalTimeGridRow" ErrorMessage="*" ToolTip="Total time of participant should not be greater than total time of catalogue">
                                                        </asp:CustomValidator>
                                                        <asp:RegularExpressionValidator ID="revTotalTime" runat="server" Text="*" ControlToValidate="txtTotalTime" ValidationGroup="valUpdate"
                                                            ValidationExpression="^(___:__:__)|(000:00:00)|([0-9]{0,3}:[0-5][0-9]:[0-5][0-9])$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                            ToolTip="Please enter a valid time in hhh:mm:ss format" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                        <ajaxToolkit:MaskedEditExtender ID="mteTotalTime" runat="server"
                                                            TargetControlID="txtTotalTime" Mask="999:99:99" AcceptNegative="None"
                                                            ClearMaskOnLostFocus="false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Track No." SortExpression="track_no">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnTrackNo" runat="server" Value='<%# Bind("track_no") %>' />
                                                        <asp:HiddenField ID="hdnTrackNoTemp" runat="server" Value='<%# Bind("track_no")%>' />
                                                        <asp:TextBox ID="txtTrackNo" runat="server" Text='<%# Eval("track_no")%>'
                                                            CssClass="gridTextField" Width="70%" TabIndex="101" onblur="clearTrackTitleGridRow(this);" onchange="OnGridDataChange(this,'TrackNo');" onkeydown="OnTrackNoEnterKeyChange(this);" Style="text-align: center;"
                                                            OnTextChanged="txtTrackNo_OnTextChanged"></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="revTrackNo" runat="server" Text="*" ControlToValidate="txtTrackNo"
                                                            ValidationExpression="^[+]?\d+$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valUpdate"
                                                            ToolTip="Please enter only integers" Display="Dynamic"> 
                                                        </asp:RegularExpressionValidator>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="12%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Track Title" SortExpression="track_title">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnTrackTitle" runat="server" Value='<%# Bind("track_title") %>' />
                                                        <asp:TextBox ID="txtTrackTitle" runat="server" Text='<%#Bind("track_title") %>' TabIndex="101" MaxLength="30" CssClass="identifierLable" onchange="OnGridDataChange(this,'TrackTitle');" onkeydown="OnTrackTitleBackSapce(this);" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Escalation Code" SortExpression="esc_code">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnEscalationCode" runat="server" Value='<%# Bind("esc_code") %>' />
                                                        <asp:DropDownList ID="ddlEscalationCode" runat="server" TabIndex="101" Width="90%" CssClass="ddlStyle" onchange="OnGridDataChange(this,'EscalationCode');"></asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Esc Include Units" SortExpression="inc_in_escalation">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnEscIncludeUnits" runat="server" Value='<%# Bind("inc_in_escalation") %>' />
                                                        <asp:CheckBox ID="cbEscIncludeUnits" runat="server" TabIndex="101" onclick="OnGridDataChange(this,'EscIncludeUnits');" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Active?" SortExpression="participation_type">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnParticipActive" runat="server" Value='<%# Bind("participation_type") %>' />
                                                        <asp:CheckBox ID="cbActive" runat="server" TabIndex="101" onclick="OnGridDataChange(this,'Active');" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Status" SortExpression="status_code">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnParticipStatus" runat="server" Value='<%# Bind("status_code") %>' />
                                                        <asp:DropDownList ID="ddlStatus" runat="server" Width="88%" CssClass="ddlStyle" TabIndex="101" onchange="OnGridDataChange(this,'Status');"></asp:DropDownList>
                                                        <asp:CustomValidator ID="valStatus" runat="server" ValidationGroup="valUpdate" CssClass="requiredFieldValidator"
                                                            ClientValidationFunction="ValidatePaticipantStatus" ErrorMessage="*"></asp:CustomValidator>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" ItemStyle-Width="3%" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imgBtnUndo" runat="server" CommandName="cancelRow" ImageUrl="../Images/cancel_row3.png"
                                                            ToolTip="Cancel" TabIndex="101" />
                                                        <asp:HiddenField ID="hdnEndDate" runat="server" Value='<%# Bind("end_date") %>' />
                                                        <asp:HiddenField ID="hdnIsModified" runat="server" Value='<%# Bind("is_modified") %>' />
                                                        <asp:HiddenField ID="hdnISRCVal" runat="server" Value='<%# Bind("isrc_val") %>' />
                                                        <asp:HiddenField ID="hdnTrackListingId" runat="server" Value='<%# Bind("tracklisting_id") %>' />
                                                        <asp:HiddenField ID="hdnIsISRCTrackTitle" runat="server" Value='<%# Bind("is_isrc_track_title") %>' />
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
                                                        <td width="5%" class="gridHeaderStyle_1row">Time</td>
                                                        <td width="5%" class="gridHeaderStyle_1row">Total Time</td>
                                                        <td width="4%" class="gridHeaderStyle_1row">Track No.</td>
                                                        <td width="12%" class="gridHeaderStyle_1row">Track Title</td>
                                                        <td width="5%" class="gridHeaderStyle_1row">Escalation Code</td>
                                                        <td width="5%" class="gridHeaderStyle_1row">Esc Include Units</td>
                                                        <td width="4%" class="gridHeaderStyle_1row">&nbsp</td>
                                                        <td width="10%" class="gridHeaderStyle_1row">&nbsp</td>
                                                        <td width="3%" class="gridHeaderStyle_1row">&nbsp</td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:TextBox ID="txtRoyaltorInsert" runat="server" Width="97%" CssClass="textboxStyle" TabIndex="102"
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
                                                                ClientValidationFunction="ValRoyaltorAddRow" ToolTip="Please select valid royaltor from the search list"
                                                                ControlToValidate="txtRoyaltorInsert" ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>
                                                        </td>
                                                        <td class="insertBoxStyle" align="left">
                                                            <asp:DropDownList ID="ddlOptionPeriodInsert" runat="server" Width="90%" CssClass="ddlStyle" TabIndex="103"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvOptionPeriodInsert" ControlToValidate="ddlOptionPeriodInsert" ValidationGroup="valAddRow"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please select option period" InitialValue="-" Display="Dynamic">
                                                            </asp:RequiredFieldValidator>
                                                        </td>
                                                        <td class="insertBoxStyle" align="left">
                                                            <asp:TextBox ID="txtTerritoryAddRow" runat="server" Width="98%" CssClass="textbox_FuzzySearch" TabIndex="104"
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
                                                            <asp:CustomValidator ID="valtxtTerritoryAddRow" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator"
                                                                ClientValidationFunction="ValTerritoryAddRow" ToolTip="Please select valid territory from the search list"
                                                                ControlToValidate="txtTerritoryAddRow" ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:HiddenField ID="hdnShareInsert" runat="server" Value="" />
                                                            <asp:TextBox ID="txtShareInsert" runat="server" CssClass="textboxStyle" onchange="OnFocusOutShareInsert();" TabIndex="105" Width="50%"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="revShareInsert" runat="server" Text="*" ControlToValidate="txtShareInsert"
                                                                ValidationExpression="^[1-9][0-9]*$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valAddRow"
                                                                ToolTip="Please enter only integers" Display="Dynamic"> 
                                                            </asp:RegularExpressionValidator>
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:HiddenField ID="hdnTotalShareInsert" runat="server" Value="" />
                                                            <asp:TextBox ID="txtTotalShareInsert" runat="server" CssClass="textboxStyle" onchange="OnFocusOutShareInsert();" TabIndex="106" Width="50%"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="revTotalShareInsert" runat="server" Text="*" ControlToValidate="txtTotalShareInsert"
                                                                ValidationExpression="^[1-9][0-9]*$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valAddRow"
                                                                ToolTip="Please enter only integers" Display="Dynamic"> 
                                                            </asp:RegularExpressionValidator>
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:TextBox ID="txtTimeInsert" runat="server" CssClass="textboxStyle" TabIndex="107" Width="70%" MaxLength="7"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="revTimeInsert" runat="server" Text="*" ControlToValidate="txtTimeInsert" ValidationGroup="valAddRow"
                                                                ValidationExpression="^(___:__:__)|([0-9]{0,3}:[0-5][0-9]:[0-5][0-9])$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                ToolTip="Please enter a valid time in hhh:mm:ss format" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                            <ajaxToolkit:MaskedEditExtender ID="mteTimeInsert" runat="server"
                                                                TargetControlID="txtTimeInsert" Mask="999:99:99" AcceptNegative="None"
                                                                ClearMaskOnLostFocus="false" />
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:TextBox ID="txtTotalTimeInsert" runat="server" CssClass="textboxStyle" TabIndex="108" Width="70%" MaxLength="10"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="revTotalTimeInsert" runat="server" Text="*" ControlToValidate="txtTotalTimeInsert" ValidationGroup="valAddRow"
                                                                ValidationExpression="^(___:__:__)|([0-9]{0,3}:[0-5][0-9]:[0-5][0-9])$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                ToolTip="Please enter a valid time in hhh:mm:ss format" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                            <ajaxToolkit:MaskedEditExtender ID="mteTotalTimeInsert" runat="server"
                                                                TargetControlID="txtTotalTimeInsert" Mask="999:99:99" AcceptNegative="None"
                                                                ClearMaskOnLostFocus="false" />
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">

                                                            <asp:TextBox ID="txtTrackNoInsert" runat="server" CssClass="textboxStyle" onblur="ClearTrackTitleAddRow();" onkeydown="javascript: OnTrackNoEnterKeyInsert();" OnTextChanged="txtTrackNoInsert_OnTextChanged" TabIndex="109" Width="50%" MaxLength="10"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="revTrackNoInsert" runat="server" Text="*" ControlToValidate="txtTrackNoInsert"
                                                                ValidationExpression="^[+]?\d+$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valAddRow"
                                                                ToolTip="Please enter only integers" Display="Dynamic"> 
                                                            </asp:RegularExpressionValidator>
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:TextBox ID="txtTrackTitleInsert" runat="server" Width="90%" MaxLength="30" CssClass="textboxStyle" TabIndex="110" onkeydown="OnTrackTitleInsertBackSapce();"></asp:TextBox>
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:DropDownList ID="ddlEscCodeInsert" runat="server" Width="90%" CssClass="ddlStyle" TabIndex="111" onchange="OnChangeEscCodeInsert();"></asp:DropDownList>
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">
                                                            <asp:CheckBox ID="cbEscIncludeUnitsInsert" runat="server" TabIndex="112" />
                                                        </td>
                                                        <td align="center" class="insertBoxStyle_No_Padding">&nbsp
                                                        </td>
                                                        <td class="insertBoxStyle" align="left">&nbsp
                                                        </td>
                                                        <td class="insertBoxStyle_No_Padding">
                                                            <table width="100%" style="float: right; table-layout: fixed">
                                                                <tr style="float: right">
                                                                    <td align="right" style="float: right" width="50%">
                                                                        <asp:ImageButton ID="imgBtnInsert" runat="server" CommandName="saverow" TabIndex="113" ImageUrl="../Images/add_row.png" ToolTip="Insert participant"
                                                                            OnClientClick="if (!ValidatePopUpAddRow()) { return false;};" OnClick="imgBtnInsert_Click" Onkeydown="imgBtnInsertKeydown();" />
                                                                    </td>
                                                                    <td align="right" style="float: right" width="50%">
                                                                        <asp:ImageButton ID="imgBtnCancel" runat="server" CommandName="cancelrow" TabIndex="114" ImageUrl="../Images/cancel_row3.png"
                                                                            ToolTip="Cancel" OnClientClick="return ClearAddRow();" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <asp:HiddenField ID="hdnISRCValInsert" runat="server" Value="" />
                                                        <asp:HiddenField ID="hdnTrackListingIdInsert" runat="server" Value="" />
                                                        <asp:HiddenField ID="hdnIsISRCTrackTitleInsert" runat="server" Value="N" />
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
                                        <asp:Button ID="btnUnSavedDataExit" runat="server" Text="Exit" CssClass="ButtonStyle" Width="30%" OnClientClick="OnUnSavedDataExit();"
                                            OnClick="btnUnSavedDataExit_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- Start--%>
            <asp:Button ID="dummyConfirmation" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmation" runat="server" PopupControlID="pnlConfirmation" TargetControlID="dummyConfirmation"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlConfirmation" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmation" runat="server" Text="Confirmation Box" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblText" runat="server" CssClass="identifierLable" Text="Do you want to update Status of all Participants to this Catalogue Status?"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYes" runat="server" Text="Yes" CssClass="ButtonStyle" OnClientClick="UpdateStatusConfirmYes();" />
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

            <asp:Button ID="btnShowNoTrackDummy" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeShowNoTrackMsg" runat="server" PopupControlID="pnlShowNoTrackMsg" TargetControlID="btnShowNoTrackDummy"
                BackgroundCssClass="popupBox" CancelControlID="btnCloseShowNoTrackMsg">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlShowNoTrackMsg" runat="server" align="center" Width="35%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <table width="100%">
                                <tr>
                                    <td align="right" style="vertical-align: top;">
                                        <asp:ImageButton ID="btnCloseShowNoTrackMsg" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" OnClientClick="return CloseShowNoTrackMsgPopup();" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMessage" runat="server" CssClass="identifierLable" Text="Track No doesn't exist. Please select another Track No!"></asp:Label>
                        </td>
                    </tr>

                </table>
            </asp:Panel>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnTimeTrack" runat="server" />
            <asp:HiddenField ID="hdnConfigCode" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnGridDataChanged" runat="server" Value="N" />
            <asp:HiddenField ID="hdnAddRowDataChanged" runat="server" Value="N" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" Value="" />
            <asp:HiddenField ID="hdnGridRoyFuzzySearchRowId" runat="server" />
            <asp:HiddenField ID="hdnNewParticipId" runat="server" Value="-99" />
            <asp:HiddenField ID="hdnIsSuperUser" runat="server" Value="N" />
            <asp:HiddenField ID="hdnTrackTitleInsertEnabled" runat="server" Value="N" />
            <asp:HiddenField ID="hdnFuzzySearchText" runat="server" />
            <asp:HiddenField ID="hdnGridFuzzySearchRowId" runat="server" />
            <asp:HiddenField ID="hdnIsConfirmPopup" runat="server" Value="N" />
            <asp:HiddenField ID="hdnButtonSelection" runat="server" />
            <asp:Button ID="btnFuzzyTerritoryListPopup" runat="server" Style="display: none;" OnClick="btnFuzzyTerritoryListPopup_Click" CausesValidation="false" />
            <asp:Button ID="btnHdnRoyaltorInsertSearch" runat="server" Style="display: none;" OnClick="btnHdnRoyaltorInsertSearch_Click" CausesValidation="false" />
            <asp:Button ID="btnFuzzyRoyaltorListPopup" runat="server" Style="display: none;" OnClick="btnFuzzyRoyaltorListPopup_Click" CausesValidation="false" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" onkeydown="FocusLblKeyPress();"></asp:Label>
            <%--JIRA-983 CHanges by Ravi on 26/02/2019 -- STart--%>
            <asp:HiddenField ID="hdnIsSupervisor" runat="server" Value="N" />
            <%--JIRA-983 CHanges by Ravi on 26/02/2019 -- End--%>
            <%--JIRA-1074 Changes -- Start--%>
            <asp:HiddenField ID="hdnTrackNoChange" runat="server" />
            <asp:HiddenField ID="hdnTrackNoRow" runat="server" />
            <asp:HiddenField ID="hdnTrackNoChangeInsert" runat="server" />
            <asp:HiddenField ID="hdnFocusControl" runat="server" />
            <%--JIRA-1074 Changes -- End--%>
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

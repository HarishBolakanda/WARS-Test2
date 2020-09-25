<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TrackListing.aspx.cs" Inherits="WARS.TrackListing" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Track Listing" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>


<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        function OpenContractMaintenance() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../Contract/RoyaltorSearch.aspx');
            }
            else {
                window.location = '../Contract/RoyaltorSearch.aspx';
            }
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
                        <asp:Button ID="btnCatalogueSearch" runat="server" Text="Catalogue Search"
                            CssClass="LinkButtonStyle" Width="98%" OnClientClick="return OpenCatalogueSearch();" UseSubmitBehavior="false" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderBody">
    <script type="text/javascript">

        function ParticipantSummaryScreen() {
            catNum = document.getElementById("<%=lblCatNo.ClientID %>").innerHTML;
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Participants/ParticipantSummary.aspx?CatNo=" + catNum);
            }
            else {
                window.location = "../Participants/ParticipantSummary.aspx?CatNo=" + catNum;
            }

        }

        function CatalogueMaintScreen() {
            catNum = document.getElementById("<%=lblCatNo.ClientID %>").innerHTML;
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Participants/CatalogueMaintenance.aspx?CatNo=" + catNum);
            }
            else {
                window.location = "../Participants/CatalogueMaintenance.aspx?CatNo=" + catNum;
            }

        }

        function MissingParticipScreen() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../Participants/MissingParticipants.aspx?isNewRequest=N');
            }
            else {
                window.location = '../Participants/MissingParticipants.aspx?isNewRequest=N';
            }

        }



        //probress bar and scroll position functionality - starts
        //to remain scroll position of grid panel and window
        var xPos, yPos;
        var scrollTop;
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        var gridClientId = "ContentPlaceHolderBody_gvTrackListing_";
        var gridClientIdCopy2 = "ContentPlaceHolderBody_gvCopyParticipTrackList_";
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

            //to maintain scroll position
            var postBackElement = args.get_postBackElement().id;
            postBackElementID = args.get_postBackElement().id.substring(args.get_postBackElement().id.lastIndexOf("_") + 1);
            if (postBackElementID == 'btnConsolidate' || postBackElementID == 'btnSaveComment' || postBackElementID == 'btnDeleteComment'
                || postBackElement.indexOf('imgBtnAddParticipant') != -1 ||
                document.getElementById("<%=hdnTxtRoyaltorSelected.ClientID %>").value == "Y") {
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

            //to maintain group expanded                        
            GridTrackLevelGrouping();

            //to maintain scroll position
            var postBackElement = sender._postBackSettings.sourceElement.id;
            postBackElementID = sender._postBackSettings.sourceElement.id.substring(sender._postBackSettings.sourceElement.id.lastIndexOf("_") + 1);
            if (postBackElementID == 'btnConsolidate' || postBackElementID == 'btnSaveComment' || postBackElementID == 'btnDeleteComment'
                || postBackElement.indexOf('imgBtnAddParticipant') != -1 || postBackElement.indexOf('imgBtnCopy') != -1 ||
                document.getElementById("<%=hdnTxtRoyaltorSelected.ClientID %>").value == "Y") {
                window.scrollTo(xPos, yPos);

                //set scroll position on selecting expand/collapse
                var PnlReference = document.getElementById("<%=PnlGrid.ClientID %>");
                PnlReference.scrollTop = scrollTop;

                //reset hiddenfield to maintain scroll position
                document.getElementById("<%=hdnTxtRoyaltorSelected.ClientID %>").innerText = "N";
            }


        }
        //======================= End


        //on page load        
        //**This function name should not be changed
        //This is being called on window.onload of Master page
        function SetGrdPnlHeightOnLoad() {
            //grid panel height adjustment functioanlity - starts
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
            document.getElementById("<%=PnlGrid.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

            GridTrackLevelGrouping();
        }

        //grid grouping - start
        var gvTrackListing;
        var isGroupExpand;
        var imgExpand;
        var imgCollapse;
        var displayOrder;
        var trackId;
        var trackIdSelectedRow;
        var hdnIsTrackEditable;
        var expandTrack;

        function GridTrackLevelGrouping() {

            gvTrackListing = document.getElementById("<%= gvTrackListing.ClientID %>");

            imgExpandAll = document.getElementById("<%=imgExpandAll.ClientID %>");
            imgCollapseAll = document.getElementById("<%=imgCollapseAll.ClientID %>");

            hdnExpandCollapseAll = document.getElementById("<%=hdnExpandCollapseAll.ClientID %>").value;
            if (hdnExpandCollapseAll == "Expand") {
                imgExpandAll.style.display = 'block';
                imgCollapseAll.style.display = 'none';
            }
            else {
                imgExpandAll.style.display = 'none';
                imgCollapseAll.style.display = 'block';
            }

            if (gvTrackListing != null) {
                var gvRows = gvTrackListing.rows;
                for (var i = 0; i < gvRows.length; i++) {

                    //handling empty data row
                    if (gvRows.length == 1 && document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + i) == null) {
                        break;
                    }

                    imgExpand = document.getElementById(gridClientId + 'imgExpand' + '_' + i);
                    imgCollapse = document.getElementById(gridClientId + 'imgCollapse' + '_' + i);
                    displayOrder = document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + i).value;
                    trackId = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + i).value;
                    hdnIsTrackEditable = document.getElementById(gridClientId + 'hdnIsTrackEditable' + '_' + i).value;

                    if (displayOrder == 1) {
                        //check if trackId is in the list of tracks to be expanded
                        hdnExpandedTrackId = document.getElementById("<%=hdnExpandedTrackId.ClientID %>").value;
                        var arrayValues = hdnExpandedTrackId.split(";");
                        expandTrack = "N";
                        for (var arrIndex = 0; arrIndex < arrayValues.length; arrIndex++) {
                            if (trackId == arrayValues[arrIndex]) {
                                expandTrack = "Y";
                                break;
                            }
                        }

                        //if (hdnExpandedTrackId.indexOf(trackId) != -1) {
                        //    expandTrack = "Y";
                        //}
                        //else {
                        //    expandTrack = "N";
                        //}
                    }

                    if (displayOrder == 1 && hdnIsTrackEditable != "N") {
                        if (expandTrack == "Y") {
                            imgExpand.style.display = 'none';
                            imgCollapse.style.display = 'block';
                        }
                        else {
                            imgExpand.style.display = 'block';
                            imgCollapse.style.display = 'none';
                        }
                    }
                    else if (displayOrder == 2) {
                        if (expandTrack == "Y" && hdnIsTrackEditable != "N") {
                            gvRows[i].style.display = 'block';
                        }
                        else {
                            gvRows[i].style.display = 'none';
                        }
                    }


                }

            }
        }

        function ExpandGridGroup(gridRow) {
            var selectedRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
            trackIdSelectedRow = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + selectedRowIndex).value;
            gvTrackListing = document.getElementById("<%= gvTrackListing.ClientID %>");

            //add the expanded track id to the list of tracks to be expanded            
            hdnExpandedTrackId = document.getElementById("<%=hdnExpandedTrackId.ClientID %>").value;
            document.getElementById("<%=hdnExpandedTrackId.ClientID %>").innerText = hdnExpandedTrackId + ";" + trackIdSelectedRow;
            if (gvTrackListing != null) {
                var gvRows = gvTrackListing.rows;
                for (var i = selectedRowIndex; i < gvRows.length; i++) {
                    trackId = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + i).value;
                    if (trackIdSelectedRow == trackId) {
                        imgExpand = document.getElementById(gridClientId + 'imgExpand' + '_' + i);
                        imgCollapse = document.getElementById(gridClientId + 'imgCollapse' + '_' + i);
                        displayOrder = document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + i).value;

                        if (displayOrder == 1) {
                            imgExpand.style.display = 'none';
                            imgCollapse.style.display = 'block';
                        }
                        else if (displayOrder == 2) {
                            gvRows[i].style.display = 'block';
                        }
                    }
                }

            }

            return false;
        }

        function CollapseGridGroup(gridRow) {
            var selectedRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
            trackIdSelectedRow = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + selectedRowIndex).value;
            gvTrackListing = document.getElementById("<%= gvTrackListing.ClientID %>");

            //remove the expanded track id to the list of tracks to be expanded            
            hdnExpandedTrackId = document.getElementById("<%=hdnExpandedTrackId.ClientID %>").value;

            //Harish 05-02-18: correcting this as it will not work properly when there are ids with single digit
            //if (hdnExpandedTrackId.indexOf(trackIdSelectedRow) != -1) {
            //  document.getElementById("<%=hdnExpandedTrackId.ClientID %>").innerText = hdnExpandedTrackId.replace(";" + trackIdSelectedRow, "");
            //}
            var arrayValues = hdnExpandedTrackId.split(";");
            for (var arrIndex = 0; arrIndex < arrayValues.length; arrIndex++) {
                if (trackIdSelectedRow == arrayValues[arrIndex]) {
                    arrayValues[arrIndex] = "";
                    break;
                }
            }

            document.getElementById("<%=hdnExpandedTrackId.ClientID %>").innerText = arrayValues.toString().replace(new RegExp(',', 'g'), ';');//replace the occurance globally

            if (gvTrackListing != null) {
                var gvRows = gvTrackListing.rows;
                for (var i = selectedRowIndex; i < gvRows.length; i++) {
                    trackId = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + i).value;

                    if (trackIdSelectedRow == trackId) {
                        imgExpand = document.getElementById(gridClientId + 'imgExpand' + '_' + i);
                        imgCollapse = document.getElementById(gridClientId + 'imgCollapse' + '_' + i);
                        displayOrder = document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + i).value;

                        if (displayOrder == 1) {
                            imgExpand.style.display = 'block';
                            imgCollapse.style.display = 'none';
                        }
                        else if (displayOrder == 2) {
                            gvRows[i].style.display = 'none';
                        }
                    }
                }

            }

            return false;
        }

        //WUIN-503
        function ExpandGridGroupAll() {

            gvTrackListing = document.getElementById("<%= gvTrackListing.ClientID %>");
            //empty the hdnExpandedTrackId 
            //add the expanded track id to the list of tracks to be expanded            
            document.getElementById("<%=hdnExpandedTrackId.ClientID %>").value = "";
            var expandedTrackId = null;
            imgExpandAll = document.getElementById("<%=imgExpandAll.ClientID %>");
            imgCollapseAll = document.getElementById("<%=imgCollapseAll.ClientID %>");

            imgExpandAll.style.display = 'none';
            imgCollapseAll.style.display = 'block';

            //to keep the expand/collapse status on postback
            document.getElementById("<%=hdnExpandCollapseAll.ClientID %>").value = "Collapse";

            if (gvTrackListing != null) {
                var gvRows = gvTrackListing.rows;
                for (var i = 0; i < gvRows.length; i++) {

                    //handling empty data row
                    if (gvRows.length == 1 && document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + i) == null) {
                        break;
                    }

                    trackId = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + i).value;
                    imgExpand = document.getElementById(gridClientId + 'imgExpand' + '_' + i);
                    imgCollapse = document.getElementById(gridClientId + 'imgCollapse' + '_' + i);
                    displayOrder = document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + i).value;

                    if (displayOrder == 1) {
                        //to handle when rows are grayed out
                        if (imgExpand == null && imgCollapse == null) {
                            continue;
                        }

                        imgExpand.style.display = 'none';
                        imgCollapse.style.display = 'block';

                        if (expandedTrackId == null) {
                            expandedTrackId = trackId;
                        }
                        else {
                            expandedTrackId = expandedTrackId + ";" + trackId;
                        }
                    }
                    else if (displayOrder == 2) {
                        gvRows[i].style.display = 'block';
                    }
                }

            }

            document.getElementById("<%=hdnExpandedTrackId.ClientID %>").innerText = expandedTrackId;

            //Harish:11-07-2018: to disable the browser window scrollbars.
            //This is not suggestable as it will impact if users view in smaller screens
            //document.body.scroll = "no";//ie browser
            //document.body.style.overflow = 'hidden';// firefox, chrome browsers

            return false;
        }

        function CollapseGridGroupAll() {

            gvTrackListing = document.getElementById("<%= gvTrackListing.ClientID %>");
            //empty the hdnExpandedTrackId 
            document.getElementById("<%=hdnExpandedTrackId.ClientID %>").value = "";

            imgExpandAll = document.getElementById("<%=imgExpandAll.ClientID %>");
            imgCollapseAll = document.getElementById("<%=imgCollapseAll.ClientID %>");

            imgExpandAll.style.display = 'block';
            imgCollapseAll.style.display = 'none';

            //to keep the expand/collapse status on postback
            document.getElementById("<%=hdnExpandCollapseAll.ClientID %>").value = "Expand";

            if (gvTrackListing != null) {
                var gvRows = gvTrackListing.rows;
                for (var i = 0; i < gvRows.length; i++) {

                    //handling empty data row
                    if (gvRows.length == 1 && document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + i) == null) {
                        break;
                    }

                    trackId = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + i).value;
                    imgExpand = document.getElementById(gridClientId + 'imgExpand' + '_' + i);
                    imgCollapse = document.getElementById(gridClientId + 'imgCollapse' + '_' + i);
                    displayOrder = document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + i).value;

                    if (displayOrder == 1) {
                        //to handle when rows are grayed out
                        if (imgExpand == null && imgCollapse == null) {
                            continue;
                        }

                        imgExpand.style.display = 'block';
                        imgCollapse.style.display = 'none';
                    }
                    else if (displayOrder == 2) {
                        gvRows[i].style.display = 'none';
                    }


                }

            }

            return false;
        }

        function SetTrackAdded(gridRow) {

            var selectedRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
            trackIdSelectedRow = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + selectedRowIndex).value;


            //add the track to the list of tracks to be expanded if not already present
            hdnExpandedTrackId = document.getElementById("<%=hdnExpandedTrackId.ClientID %>").value;
            if (hdnExpandedTrackId.indexOf(trackIdSelectedRow) == -1) {
                document.getElementById("<%=hdnExpandedTrackId.ClientID %>").innerText = hdnExpandedTrackId + ";" + trackIdSelectedRow;
            }

            Page_BlockSubmit = false;
            return true;

        }


        //============grid grouping - End

        //Comment popup
        var CommentPopup;
        function OpenCommentPopup(row, name) {

            var selectedRowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
            var hdnTrackListingId = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + selectedRowIndex).value;

            Page_BlockSubmit = false;

            if (IsGridDataChanged()) {
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = "Comment";
                trackIdOnUnSavedDataExit = hdnTrackListingId;
                rowIndexOnUnSavedDataExit = selectedRowIndex;
                OpenOnUnSavedData();
                return false
            }
            else {
                //WUIN-634 - validations
                //When Catalogue status is in Manager sign off, Editor user cannot edit any fields in the screen
                //when Track is in Manager sign off, Editor user cannot edit Participants of the track
                //if a product is auto sign off then allow Editor user to make changes(to participants) at any status
                var hdnUserRole = document.getElementById("<%=hdnUserRole.ClientID %>").value;
                var hdnCatStatusCode = document.getElementById("<%=hdnCatStatusCode.ClientID %>").value;
                var hdnCatAutoSign = document.getElementById("<%=hdnCatAutoSign.ClientID %>").value;
                var hdnTrackStatusCode = document.getElementById(gridClientId + 'hdnStatusCode' + '_' + selectedRowIndex).value;
                //debugger;

                CommentPopup = $find('<%= mpeCommentPopup.ClientID %>');
                if (CommentPopup != null) {
                    CommentPopup.show();
                }
                else {
                    DisplayMessagePopup("Error in opening comment");
                    return false;
                }

                document.getElementById("<%=hdnCommentISRCDealId.ClientID %>").innerText = document.getElementById(gridClientId + 'hdnISRCDealId' + '_' + selectedRowIndex).value;
                document.getElementById("<%=txtComment.ClientID %>").innerText = document.getElementById(gridClientId + 'hdnComments' + '_' + selectedRowIndex).value;

                return false;
            }


        }

        function CancelComment() {
            document.getElementById("<%=txtComment.ClientID %>").innerText = "";
            CommentPopup = $find('<%= mpeCommentPopup.ClientID %>');
            if (CommentPopup != null) {
                CommentPopup.hide();
            }
        }

        //Comment popup========== End

        function ValOptionPeriodGridRow(sender, args) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtRoyaltor = document.getElementById(gridClientId + 'txtRoyaltor' + '_' + gridRowIndex).value;
            ddlOptionPeriod = document.getElementById(gridClientId + 'ddlOptionPeriod' + '_' + gridRowIndex).value;
            if ((txtRoyaltor != "" && ddlOptionPeriod == "-") || (txtRoyaltor == "" && ddlOptionPeriod != "-")) {
                args.IsValid = false;
            }
            else {
                args.IsValid = true;
            }
        }

        //Royaltor auto populate search functionalities

        function royaltorListPopulating(sender, args) {
            selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
            txtRoy = document.getElementById(gridClientId + 'txtRoyaltor' + '_' + selectedRowIndex);
            txtRoy.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoy.style.backgroundRepeat = 'no-repeat';
            txtRoy.style.backgroundPosition = 'right';

        }

        function royaltorListPopulated(sender, args) {
            selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
            txtRoy = document.getElementById(gridClientId + 'txtRoyaltor' + '_' + selectedRowIndex);
            txtRoy.style.backgroundImage = 'none';

        }

        function royaltorListItemSelected(sender, args) {
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
                txtRoy = document.getElementById(gridClientId + 'txtRoyaltor' + '_' + selectedRowIndex).value = "";
            }

        }

        //Pop up fuzzy search list       
        function OntxtRoyaltorKeyDown(sender) {
            if ((event.keyCode == 13)) {
                selectedRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);

                //Enter key can be used to select the dropdown list item or to pop up the complete list
                //to know this, check if list item is selected or not
                //as txtroyaltor can be read only, Enter key press throws error in this case. checking if aceroyaltor is null or not
                var aceRoyaltor = $find(gridClientId + 'aceRoyaltor' + '_' + selectedRowIndex);
                if (aceRoyaltor != null && aceRoyaltor._selectIndex == -1) {
                    txtRoyaltor = document.getElementById(gridClientId + 'txtRoyaltor' + '_' + selectedRowIndex).value;
                    document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtRoyaltor;
                    document.getElementById("<%=hdnGridRoyFuzzySearchRowId.ClientID %>").innerText = selectedRowIndex;
                    document.getElementById("<%=hdnFuzzySearchField.ClientID %>").innerText = "Royaltor";
                    document.getElementById('<%=btnFuzzyRoyaltorListPopup.ClientID%>').click();
                }
            }

        }

        //Validate if the field value is a valid one from fuzzy search list
        function ValRoyaltorGridRow(sender, args) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtRoyaltor = document.getElementById(gridClientId + 'txtRoyaltor' + '_' + gridRowIndex);
            txtRoyaltor.style["width"] = '97%';

            if (txtRoyaltor.value == "") {
                args.IsValid = false;
                //adjust width of the textbox to display error
                fieldWidth = txtRoyaltor.offsetWidth;
                txtRoyaltor.style["width"] = (fieldWidth - 20);
            }
            else if (txtRoyaltor.value == "No results found") {
                args.IsValid = false;
                //adjust width of the textbox to display error
                fieldWidth = txtRoyaltor.offsetWidth;
                txtRoyaltor.style["width"] = (fieldWidth - 20);
            }
            else if (txtRoyaltor.value != "" && txtRoyaltor.value.indexOf('-') == -1) {
                args.IsValid = false;
                //adjust width of the textbox to display error
                fieldWidth = txtRoyaltor.offsetWidth;
                txtRoyaltor.style["width"] = (fieldWidth - 20);
            }
            else if (args.IsValid == true) {
                txtRoyaltor.style["width"] = '97%';
            }


        }



        //================================End

        //Territory fuzzy search

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
            txtTerritory.style["width"] = '97%';
            if (txtTerritory.value == "") {
                args.IsValid = true;
            }
            else if (txtTerritory.value == "No results found") {
                args.IsValid = true;
                txtTerritory.value = "";
            }
            else if (txtTerritory.value != "" && txtTerritory.value.indexOf('-') == -1) {
                args.IsValid = false;
                //adjust width of the textbox to display error
                fieldWidth = txtTerritory.offsetWidth;
                txtTerritory.style["width"] = (fieldWidth - 20);
            }


        }

        //reset field width when empty
        function OntxtTerritoryChange(sender) {
            gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
            txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + gridRowIndex);

            if (txtTerritory.value == "") {
                txtTerritory.style["width"] = '97%';
            }
        }
        //============== End

        //Undo button functionality - Begins
        var ddlOptionPeriod;
        var ddlEscCode;
        var hdnOptPeriodCode;
        var hdnEscCode;

        function UndoCatNoChanges() {
            var hdnTrackTimeFlag = document.getElementById("<%=hdnTrackTimeFlag.ClientID %>").value;
            var hdnStatusCode = document.getElementById("<%=hdnCatStatusCode.ClientID %>").value;

            if (hdnTrackTimeFlag == "T") {
                document.getElementById("<%=rbCatTrackShare.ClientID %>").checked = true;
            }
            else {
                document.getElementById("<%=rbCatTimeShare.ClientID %>").checked = true;
            }
            document.getElementById("<%=ddlCatStatus.ClientID %>").value = hdnStatusCode;

            return false;
        }

        function UndoTrackChanges(gridRow) {
            var selectedRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
            var hdnTrackListingId = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + selectedRowIndex).value;
            //Page_BlockSubmit = false;
            //Page_ClientValidate('');
            displayOrder = document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + selectedRowIndex).value;
            if (displayOrder == 1) {
                var cbExclude = document.getElementById(gridClientId + 'cbExclude' + '_' + selectedRowIndex);
                var hdnExclude = document.getElementById(gridClientId + 'hdnExclude' + '_' + selectedRowIndex).value;
                var hdnStatusCode = document.getElementById(gridClientId + 'hdnStatusCode' + '_' + selectedRowIndex).value;
                document.getElementById(gridClientId + 'ddlStatus' + '_' + selectedRowIndex).value = hdnStatusCode;
                if (hdnExclude == "Y") {
                    cbExclude.checked = true;
                }
                else {
                    cbExclude.checked = false;
                }

                Page_BlockSubmit = false;
                return false;
            }
            else {

                UndoTrackParticipChanges(selectedRowIndex);
                //Page_ClientValidate('');
                Page_BlockSubmit = false;
                return false;
            }



        }

        function UndoTrackParticipChanges(selectedRowIndex) {
            var hdnIsModified = document.getElementById(gridClientId + 'hdnIsModified' + '_' + selectedRowIndex).value;
            var txtRoyaltor = document.getElementById(gridClientId + 'txtRoyaltor' + '_' + selectedRowIndex);
            ddlOptionPeriod = document.getElementById(gridClientId + 'ddlOptionPeriod' + '_' + selectedRowIndex);
            txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + selectedRowIndex);
            ddlEscCode = document.getElementById(gridClientId + 'ddlEscCode' + '_' + selectedRowIndex);
            var cbActive = document.getElementById(gridClientId + 'cbActive' + '_' + selectedRowIndex);
            var cbIncInEsc = document.getElementById(gridClientId + 'cbIncInEsc' + '_' + selectedRowIndex);

            if (hdnIsModified == "-") {
                txtRoyaltor.innerText = "";
                ddlOptionPeriod.innerHTML = "";
                txtTerritory.innerText = "";
                ddlEscCode.innerHTML = "";
                cbActive.checked = true;
                cbIncInEsc.checked = false;

                var listItem1 = document.createElement('option');
                listItem1.text = listItem1.value = "-";
                ddlOptionPeriod.add(listItem1);

                var listItem2 = document.createElement('option');
                listItem2.text = listItem2.value = "-";
                ddlEscCode.add(listItem2);

            }
            else {
                var hdnRoyaltor = document.getElementById(gridClientId + 'hdnRoyaltor' + '_' + selectedRowIndex).value;
                hdnOptPeriodCode = document.getElementById(gridClientId + 'hdnOptPeriodCode' + '_' + selectedRowIndex).value;
                var hdnSellerGrpCode = document.getElementById(gridClientId + 'hdnSellerGrpCode' + '_' + selectedRowIndex).value;
                var hdnSellerGrp = document.getElementById(gridClientId + 'hdnSellerGrp' + '_' + selectedRowIndex).value;
                hdnEscCode = document.getElementById(gridClientId + 'hdnEscCode' + '_' + selectedRowIndex).value;
                var hdnActive = document.getElementById(gridClientId + 'hdnActive' + '_' + selectedRowIndex).value;
                var hdnIncInEsc = document.getElementById(gridClientId + 'hdnIncInEsc' + '_' + selectedRowIndex).value;

                //re populate option period and esc code dropdowns if royaltor was changed
                //do this only if royaltor and option period are editable
                if (ddlOptionPeriod != null && txtRoyaltor.value != hdnRoyaltor) {
                    ddlOptionPeriod.innerHTML = "";
                    var listItem1 = document.createElement('option');
                    listItem1.text = listItem1.value = "-";
                    ddlOptionPeriod.add(listItem1);

                    PageMethods.GetOptionPeriods(hdnRoyaltor, OnSuccessOptPeriod);

                    ddlEscCode.innerHTML = "";
                    var listItem2 = document.createElement('option');
                    listItem2.text = listItem2.value = "-";
                    ddlEscCode.add(listItem2);
                    PageMethods.GetEscCodes(hdnRoyaltor, OnSuccessEscCode);

                }

                txtRoyaltor.innerText = hdnRoyaltor;
                ValidatorValidate(document.getElementById(gridClientId + 'valtxtRoyaltor' + '_' + selectedRowIndex));

                if (ddlOptionPeriod != null) {
                    if (hdnOptPeriodCode != "") {
                        ddlOptionPeriod.value = hdnOptPeriodCode;
                    }
                    else {
                        ddlOptionPeriod.value = "-";
                    }
                }

                ValidatorValidate(document.getElementById(gridClientId + 'valddlOptionPeriod' + '_' + selectedRowIndex));

                txtTerritory.innerText = hdnSellerGrp;
                ValidatorValidate(document.getElementById(gridClientId + 'valtxtTerritory' + '_' + selectedRowIndex));

                if (hdnEscCode != "") {
                    ddlEscCode.value = hdnEscCode;
                }
                else {
                    ddlEscCode.value = "-";
                }

                if (hdnActive == "Y") {
                    cbActive.checked = true;
                }
                else {
                    cbActive.checked = false;
                }

                if (hdnIncInEsc == "Y") {
                    cbIncInEsc.checked = true;
                }
                else {
                    cbIncInEsc.checked = false;
                }

                //set is_modified flag to N
                document.getElementById(gridClientId + 'hdnIsModified' + '_' + selectedRowIndex).innerText = "N";
            }
        }

        function OnSuccessOptPeriod(response) {
            for (var i in response) {
                var listItem = document.createElement('option');
                listItem.text = response[i].Text;
                listItem.value = response[i].Value;
                ddlOptionPeriod.add(listItem);
            }

            if (hdnOptPeriodCode != "") {
                ddlOptionPeriod.value = hdnOptPeriodCode;
            }
            else {
                ddlOptionPeriod.value = "-";
            }
        }

        function OnSuccessEscCode(response) {
            for (var i in response) {
                var listItem = document.createElement('option');
                listItem.text = response[i].Text;
                listItem.value = response[i].Value;
                ddlEscCode.add(listItem);
            }

            if (hdnEscCode != "") {
                ddlEscCode.value = hdnEscCode;
            }
            else {
                ddlEscCode.value = "-";
            }
        }

        //Undo button functionality - Ends

        //Confim delete
        function ConfirmDelete(gridRow) {

            var selectedRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
            var hdnTrackListingId = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + selectedRowIndex).value;
            var hdnDisplayOrder = document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + selectedRowIndex).value;
            var hdnISRCDealId = document.getElementById(gridClientId + 'hdnISRCDealId' + '_' + selectedRowIndex).value;
            var hdnISRCPartId = document.getElementById(gridClientId + 'hdnISRCPartId' + '_' + selectedRowIndex).value;
            var hdnIsModified = document.getElementById(gridClientId + 'hdnIsModified' + '_' + selectedRowIndex).value;

            document.getElementById("<%=hdnDeleteISRCDealId.ClientID %>").innerText = hdnISRCDealId;
            document.getElementById("<%=hdnDeleteISRCPartId.ClientID %>").innerText = hdnISRCPartId;
            document.getElementById("<%=hdnDeletedisplayOrder.ClientID %>").innerText = hdnDisplayOrder;
            document.getElementById("<%=hdnDeleteisModified.ClientID %>").innerText = hdnIsModified;

            var popup = $find('<%= mpeConfirmDelete.ClientID %>');
            if (popup != null) {
                popup.show();
                return false;
            }


        }
        //============== End

        //Validations - Begin

        //triggers on catalogue status change
        function ValidateCatNoStatus() {
            //Validate Catalogue status update
            //If Status = 1  Allow update to 2        
            //If Status = 2  Allow update to 1        
            //If Status = 3  Allow update to 1,2  Display warning 'This update will prevent the generation of Statement details for all Participants.
            //WUIN-1167 Validations:
            //Any changes done on catalogue which is at manager sign off should move the status to under review irrespective of the User role.
            //only super user / supervisor can change status to Manger sign off.
            var hdnStatusCode = document.getElementById("<%=hdnCatStatusCode.ClientID %>").value;
            var selectedCode = document.getElementById("<%=ddlCatStatus.ClientID %>").value;
            var hdnUserRole = document.getElementById("<%=hdnUserRole.ClientID %>").value;
            var lblMsgCatStatusPopup = document.getElementById("<%=lblMsgCatStatusPopup.ClientID %>");
            lblMsgCatStatusPopup.innerText = "Do you want to update Status of all participants to this Catalogue Status?";

            if ((hdnUserRole != "SuperUser") && (hdnUserRole != "Supervisor")) {
                if (selectedCode == "3") {
                    DisplayMessagePopup("Only super user and Supervisor can change status to Manger Sign Off!");
                    document.getElementById("<%=ddlCatStatus.ClientID %>").value = hdnStatusCode;
                }
                return false;
            }

            if (hdnStatusCode == "1" && selectedCode != "2" && selectedCode != "1") {
                DisplayMessagePopup("Status can only be changed from Under Review to Team Sign Off!");
                document.getElementById("<%=ddlCatStatus.ClientID %>").value = hdnStatusCode;
            }
            else if (hdnStatusCode == "2" && selectedCode != "1" && selectedCode != "2") {
                DisplayMessagePopup("Status can only be changed from Team Sign Off to Under Review!");
                document.getElementById("<%=ddlCatStatus.ClientID %>").value = hdnStatusCode;
            }
            else if (hdnStatusCode == "3" && (selectedCode != "1" && selectedCode != "2") && selectedCode != "3") {
                DisplayMessagePopup("Status can only be changed from Manager Sign Off to either Team Sign Off or Under Review!");
                document.getElementById("<%=ddlCatStatus.ClientID %>").value = hdnStatusCode;
            }
            else if (hdnStatusCode == "3" && (selectedCode == "1" || selectedCode == "2") && selectedCode != "3") {

                document.getElementById("<%=pnlCatStatusPopup.ClientID %>").style.width = "35%";
                lblMsgCatStatusPopup.innerText = "This update will prevent the generation of Statement details for all Participants! \n" + lblMsgCatStatusPopup.innerText;
                var popup = $find('<%= mpeCatStatusPopup.ClientID %>');
                if (popup != null) {
                    popup.show();
                }
            }
            else {
                document.getElementById("<%=pnlCatStatusPopup.ClientID %>").style.width = "25%";
                var popup = $find('<%= mpeCatStatusPopup.ClientID %>');
                if (popup != null) {
                    popup.show();
                }
            }
    return false;
}

function UpdateStatusConfirmYes() {
    var ddlCatStatus = document.getElementById("<%=ddlCatStatus.ClientID %>").value;
    document.getElementById("<%=hdnBulkStatusUpdate.ClientID %>").value = "Y";
    var gvTrackListing = document.getElementById("<%= gvTrackListing.ClientID %>");
    if (gvTrackListing != null) {
        var gvRows = gvTrackListing.rows;
        for (var i = 0; i < gvRows.length; i++) {
            var ddlStatus = document.getElementById(gridClientId + 'ddlStatus' + '_' + i);
            if (document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + i).value == "1" && ddlStatus != null) {
                ddlStatus.value = ddlCatStatus;
            }
        }
    }
}


//triggers on grid track status change
function ValidateTrackStatus(gridRow) {
    //Validate - Track status update
    //If Status = 0 No manual update  
    //If Status = 1 Allow update to 2  
    //If Status = 2 Allow update to 1 or 3  
    //If Status = 3 Allow update to 1 or 2
    //WUIN-1167 Validations:
    //Any changes done on catalogue which is at manager sign off should move the status to under review irrespective of the User role.
    //only super user / supervisor can change status to Manger sign off
    var selectedRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
    var hdnStatusCode = document.getElementById(gridClientId + 'hdnStatusCode' + '_' + selectedRowIndex).value;
    var selectedCode = document.getElementById(gridClientId + 'ddlStatus' + '_' + selectedRowIndex).value;
    var hdnUserRole = document.getElementById("<%=hdnUserRole.ClientID %>").value;
    var hdnCatStatusCode = document.getElementById("<%=hdnCatStatusCode.ClientID %>").value;
    //JIRA-983 Changes by Ravi on 26/02/2019 -- Start
    if ((hdnUserRole != "SuperUser") && (hdnUserRole != "Supervisor")) {
        if (selectedCode == "3") {
            DisplayMessagePopup("Only super user and Supervisor can change the status to Manager Sign Off!");
            document.getElementById(gridClientId + 'ddlStatus' + '_' + selectedRowIndex).value = hdnStatusCode;
            return;
        }
        //JIRA-983 Changes by Ravi on 26/02/2019 -- End
    }
    if (hdnStatusCode == "0") {
        DisplayMessagePopup("Status cannot be changed manually at this stage!");
        document.getElementById(gridClientId + 'ddlStatus' + '_' + selectedRowIndex).value = hdnStatusCode;
    }
    else if (hdnStatusCode == "1" && selectedCode != "1" && selectedCode != "2") {
        DisplayMessagePopup("Status can only be changed from Under Review to Team Sign Off!");
        document.getElementById(gridClientId + 'ddlStatus' + '_' + selectedRowIndex).value = hdnStatusCode;
    }
    else if (hdnStatusCode == "2" && selectedCode != "1" && selectedCode != "2" && selectedCode != "3") {
        DisplayMessagePopup("Status can only be changed from Team Sign Off to either Under Review or Manager Sign Off!");
        document.getElementById(gridClientId + 'ddlStatus' + '_' + selectedRowIndex).value = hdnStatusCode;
    }
    else if (hdnStatusCode == "3" && selectedCode != "1" && selectedCode != "2" && selectedCode != "3") {
        DisplayMessagePopup("Status can only be changed from Manager Sign Off to either Team Sign Off or Under Review!");
        document.getElementById(gridClientId + 'ddlStatus' + '_' + selectedRowIndex).value = hdnStatusCode;
    }


}

//==============Begin - option period and Esc code population on Royaltor selection
var isRoyaltorPostBack = false;//used when dopostback is used for royaltor change    
var ddlOptionPeriodOnRoyChange;
var ddlEscCodeOnRoyChange;
var hdnOptPeriodCodeOnRoyChange;
function OnRoyFuzzySearchChange(gridRow) {
    var selectedRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
    OnRoyFuzzySearchSelected(selectedRowIndex);
}

function OnRoyFuzzySearchSelected(gridRowId) {
    var selectedRowIndex = gridRowId;
    var royaltorSearchText = document.getElementById(gridClientId + 'txtRoyaltor' + '_' + selectedRowIndex).value;
    ddlOptionPeriodOnRoyChange = document.getElementById(gridClientId + 'ddlOptionPeriod' + '_' + selectedRowIndex);
    ddlEscCodeOnRoyChange = document.getElementById(gridClientId + 'ddlEscCode' + '_' + selectedRowIndex);
    var hdnRoyaltorId = document.getElementById(gridClientId + 'hdnRoyaltorId' + '_' + selectedRowIndex);
    hdnOptPeriodCodeOnRoyChange = document.getElementById(gridClientId + 'hdnOptPeriodCode' + '_' + selectedRowIndex);

    //Harish: 11-01-2019 - correction as the hdn Esc code is not reset after a royaltor is changed
    var hdnOptPeriodCode = document.getElementById(gridClientId + 'hdnOptPeriodCode' + '_' + selectedRowIndex);
    var hdnEscCode = document.getElementById(gridClientId + 'hdnEscCode' + '_' + selectedRowIndex);

    //WUIN-258 changes - The option period is lost in the first participate added when the user presses + to add a second participate to an 
    //                   ISRC without saving the first one
    hdnRoyaltorId.value = "";

    if (royaltorSearchText.indexOf('-') != -1) {
        //reset field width 
        document.getElementById(gridClientId + 'txtRoyaltor' + '_' + selectedRowIndex).style["width"] = '97%';

        //Populate the Option periods
        ddlOptionPeriodOnRoyChange.innerHTML = "";
        var listItem1 = document.createElement('option');
        listItem1.text = listItem1.value = "-";
        ddlOptionPeriodOnRoyChange.add(listItem1);
        PageMethods.GetOptionPeriods(royaltorSearchText, OnSuccessOptPrdRoyChange);

        //Populate Esc codes
        ddlEscCodeOnRoyChange.innerHTML = "";
        var listItem2 = document.createElement('option');
        listItem2.text = listItem2.value = "-";
        ddlEscCodeOnRoyChange.add(listItem2);
        PageMethods.GetEscCodes(royaltorSearchText, OnSuccessEscCodeRoyChange);

        //set hiddenfield to maintain scroll position            
        document.getElementById("<%=hdnTxtRoyaltorSelected.ClientID %>").innerText = "Y";

        //WUIN-258 changes            
        hdnRoyaltorId.value = royaltorSearchText.split('-')[0].trim();

        //Harish: 11-01-2019 - correction as the hdn fields are not reset after a royaltor is changed
        hdnOptPeriodCode.value = "";
        hdnEscCode.value = "";

    }
    else {
        ddlOptionPeriodOnRoyChange.innerHTML = "";
        var listItem1 = document.createElement('option');
        listItem1.text = listItem1.value = "-";
        ddlOptionPeriodOnRoyChange.add(listItem1);

        ddlEscCodeOnRoyChange.innerHTML = "";
        var listItem2 = document.createElement('option');
        listItem2.text = listItem2.value = "-";
        ddlEscCodeOnRoyChange.add(listItem2);

        //DisplayMessagePopup("Not a valid royaltor");
        Page_BlockSubmit = false;

        //Harish: 11-01-2019 - correction as the hdn Esc code is not reset after a royaltor is changed
        hdnOptPeriodCode.value = "";
        hdnEscCode.value = "";
    }

    return false;
}

function OnSuccessOptPrdRoyChange(response) {
    var listCount = 0;
    for (var i in response) {
        var listItem = document.createElement('option');
        listItem.text = response[i].Text;
        listItem.value = response[i].Value;
        ddlOptionPeriodOnRoyChange.add(listItem);
        listCount++;
    }

    //Populate the Option period if only one option period for the selected royaltor
    if (listCount == 1) {
        ddlOptionPeriodOnRoyChange.selectedIndex = 1;
        hdnOptPeriodCodeOnRoyChange.value = ddlOptionPeriodOnRoyChange.options[ddlOptionPeriodOnRoyChange.selectedIndex].value;
    }
}

function OnSuccessEscCodeRoyChange(response) {
    for (var i in response) {
        var listItem = document.createElement('option');
        listItem.text = response[i].Text;
        listItem.value = response[i].Value;
        ddlEscCodeOnRoyChange.add(listItem);
    }

}

//to hold selected option period on new row added
function OnOptionPeriodChange(gridRow) {
    var selectedRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
    var ddlOptionPeriod = document.getElementById(gridClientId + 'ddlOptionPeriod' + '_' + selectedRowIndex);
    var hdnOptPeriodCodeChanged = document.getElementById(gridClientId + 'hdnOptPeriodCodeChanged' + '_' + selectedRowIndex);

    //WUIN-634 - validations
    //When Catalogue status is in Manager sign off, Editor user cannot edit any fields in the screen
    //when Track is in Manager sign off, Editor user cannot edit Participants of the track
    //if a product is auto sign off then allow Editor user to make changes(to participants) at any status
    var hdnUserRole = document.getElementById("<%=hdnUserRole.ClientID %>").value;
    var hdnCatStatusCode = document.getElementById("<%=hdnCatStatusCode.ClientID %>").value;
    var hdnCatAutoSign = document.getElementById("<%=hdnCatAutoSign.ClientID %>").value;
    var hdnTrackStatusCode = document.getElementById(gridClientId + 'hdnStatusCode' + '_' + selectedRowIndex).value;
    //debugger;
    //JIRA-983 Changes by Ravi on 26/02/2019 -- Start
    if (((hdnUserRole != "SuperUser") && (hdnUserRole != "Supervisor")) && hdnCatAutoSign != "Y") {
        if (hdnCatStatusCode == "3") {
            //reset selected row values to initial values
            ResetGridRowValues(selectedRowIndex);
            return;
            //JIRA-983 Changes by Ravi on 26/02/2019 -- End
        }

        //track status is null for participant row of the grid
        //get track status of selected grid row
        if (hdnTrackStatusCode == "") {
            var hdnTrackListingId = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + selectedRowIndex).value;
            gvTrackListing = document.getElementById("<%= gvTrackListing.ClientID %>");
            if (gvTrackListing != null) {
                var gvRows = gvTrackListing.rows;
                for (var i = 0; i < gvRows.length; i++) {
                    hdnDisplayOrder2 = document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + i).value;
                    hdnTrackListingId2 = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + i).value;
                    if (hdnDisplayOrder2 == "1" && hdnTrackListingId == hdnTrackListingId2) {
                        hdnTrackStatusCode = document.getElementById(gridClientId + 'hdnStatusCode' + '_' + i).value;
                        break;
                    }

                }
            }
        }

        if (hdnTrackStatusCode == "3") {
            //reset selected row values to initial values
            ResetGridRowValues(selectedRowIndex);
            return;
        }
    }

    hdnOptPeriodCodeChanged.value = ddlOptionPeriod.options[ddlOptionPeriod.selectedIndex].value;
}

//to hold selected Esc code on new row added
function OnEscCodeChange(gridRow) {
    var selectedRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
    var ddlEscCode = document.getElementById(gridClientId + 'ddlEscCode' + '_' + selectedRowIndex);
    var hdnEscCodeChanged = document.getElementById(gridClientId + 'hdnEscCodeChanged' + '_' + selectedRowIndex);

    //WUIN-634 - validations
    //When Catalogue status is in Manager sign off, Editor user cannot edit any fields in the screen
    //when Track is in Manager sign off, Editor user cannot edit Participants of the track
    //if a product is auto sign off then allow Editor user to make changes(to participants) at any status
    var hdnUserRole = document.getElementById("<%=hdnUserRole.ClientID %>").value;
    var hdnCatStatusCode = document.getElementById("<%=hdnCatStatusCode.ClientID %>").value;
    var hdnCatAutoSign = document.getElementById("<%=hdnCatAutoSign.ClientID %>").value;
    var hdnTrackStatusCode = document.getElementById(gridClientId + 'hdnStatusCode' + '_' + selectedRowIndex).value;
    if (hdnUserRole != "SuperUser" && hdnCatAutoSign != "Y") {
        if (hdnCatStatusCode == "3") {
            //reset selected row values to initial values
            ResetGridRowValues(selectedRowIndex);
            return;
        }

        //track status is null for participant row of the grid
        //get track status of selected grid row
        if (hdnTrackStatusCode == "") {
            var hdnTrackListingId = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + selectedRowIndex).value;
            gvTrackListing = document.getElementById("<%= gvTrackListing.ClientID %>");
            if (gvTrackListing != null) {
                var gvRows = gvTrackListing.rows;
                for (var i = 0; i < gvRows.length; i++) {
                    hdnDisplayOrder2 = document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + i).value;
                    hdnTrackListingId2 = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + i).value;
                    if (hdnDisplayOrder2 == "1" && hdnTrackListingId == hdnTrackListingId2) {
                        hdnTrackStatusCode = document.getElementById(gridClientId + 'hdnStatusCode' + '_' + i).value;
                        break;
                    }

                }
            }
        }

        if (hdnTrackStatusCode == "3") {
            //reset selected row values to initial values
            ResetGridRowValues(selectedRowIndex);
            return;
        }
    }

    hdnEscCodeChanged.value = ddlEscCode.options[ddlEscCode.selectedIndex].value;
}

//==============End - option period and Esc code population on Royaltor selection

//======== Begin - Avoid postback on fuzzy search pop up selection
function OnlbFuzzySearchSelected() {
    var hdnFuzzySearchField = document.getElementById("<%=hdnFuzzySearchField.ClientID %>");
    var lbFuzzySearch = document.getElementById("<%=lbFuzzySearch.ClientID %>");

    if (hdnFuzzySearchField.value == "Territory") {
        //territory fuzzy search
        var hdnGridFuzzySearchRowId = document.getElementById("<%=hdnGridFuzzySearchRowId.ClientID %>");
        var txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + hdnGridFuzzySearchRowId.value);
        var lbSelectedVal = lbFuzzySearch.options[lbFuzzySearch.selectedIndex].value;
        if (lbSelectedVal == "No results found") {
            txtTerritory.value = "";
        }
        else {
            txtTerritory.value = lbSelectedVal;
        }

        txtTerritory.title = txtTerritory.value;

        hdnFuzzySearchField.value = "";
    }
    else {
        //royaltor fuzzy search
        var hdnGridRoyFuzzySearchRowId = document.getElementById("<%=hdnGridRoyFuzzySearchRowId.ClientID %>");
        var txtRoyaltor = document.getElementById(gridClientId + 'txtRoyaltor' + '_' + hdnGridRoyFuzzySearchRowId.value);
        var lbSelectedVal = lbFuzzySearch.options[lbFuzzySearch.selectedIndex].value;
        if (lbSelectedVal == "No results found") {
            txtRoyaltor.value = "";
        }
        else {
            txtRoyaltor.value = lbSelectedVal;
        }

        //populate option period and esc code
        OnRoyFuzzySearchSelected(hdnGridRoyFuzzySearchRowId.value);

    }

    var FuzzySearchPopup = $find('<%= mpeFuzzySearch.ClientID %>');
    if (FuzzySearchPopup != null) {
        FuzzySearchPopup.hide();
    }
}

//======== End - Avoid postback on fuzzy search pop up selection


//to validate track changes before save
function ValidateSaveAllChanges() {
    var hdnDisplayOrder;
    var hdnTrackListingIdRow;
    var hdnRoyaltor;
    var txtRoyaltor;
    var ddlOptionPeriod;
    var cbActive;
    var cbExclude;
    var hdnExclude;
    var isExclude;
    var trackStatus;
    gvTrackListing = document.getElementById("<%= gvTrackListing.ClientID %>");
    //validate - input data 
    if (gvTrackListing != null) {
        var gvRows = gvTrackListing.rows;
        for (var i = 0; i < gvRows.length; i++) {
            hdnTrackListingIdRow = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + i).value;
            hdnDisplayOrder = document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + i).value;

            if (hdnDisplayOrder == "1") {
                //WUIN-601 - change
                //validation - cannot set Exclude to checked when there is active participant in the track
                //if it is 'No Participants' track and Exclude is checked,
                //  - if no participant data is entered then skip the validations and update
                //  - if participant data entered then perform Active participant validation and warning
                //  - otherwise do checks only for child rows 

                hdnRoyaltor = document.getElementById(gridClientId + 'hdnRoyaltor' + '_' + i).value;
                cbExclude = document.getElementById(gridClientId + 'cbExclude' + '_' + i);
                hdnExclude = document.getElementById(gridClientId + 'hdnExclude' + '_' + i).value;
                isExclude = cbExclude.checked ? "Y" : "N";
                txtRoyaltorParticip = document.getElementById(gridClientId + 'txtRoyaltor' + '_' + (Number(i) + 1));
                if (isExclude == hdnExclude && hdnExclude == "N") {
                    trackStatus = document.getElementById(gridClientId + 'ddlstatus' + '_' + i).value;
                }
                continue;

            }

            ddlOptionPeriodCtrl = document.getElementById(gridClientId + 'ddlOptionPeriod' + '_' + i);
            if (ddlOptionPeriodCtrl != null) {
                txtRoyaltor = document.getElementById(gridClientId + 'txtRoyaltor' + '_' + i).value;
                hdnRoyaltor = document.getElementById(gridClientId + 'hdnRoyaltor' + '_' + i).value;
                ddlOptionPeriod = document.getElementById(gridClientId + 'ddlOptionPeriod' + '_' + i).value;
                if ((txtRoyaltor == "" || ddlOptionPeriod == "-")) {
                    if (trackStatus != "0" || (trackStatus == "0" && ((txtRoyaltor == "" && ddlOptionPeriod != "-") || (txtRoyaltor != "" && ddlOptionPeriod == "-")))) {
                        if (gvTrackListing != null) {
                            var gvRows = gvTrackListing.rows;
                            for (var j = i; j < gvRows.length; j++) {
                                trackId = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + j).value;
                                if (hdnTrackListingIdRow == trackId) {
                                    imgExpand = document.getElementById(gridClientId + 'imgExpand' + '_' + j);
                                    imgCollapse = document.getElementById(gridClientId + 'imgCollapse' + '_' + j);
                                    displayOrder = document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + j).value;

                                    if (displayOrder == 1) {
                                        imgExpand.style.display = 'none';
                                        imgCollapse.style.display = 'block';
                                    }
                                    else if (displayOrder == 2) {
                                        gvRows[j].style.display = 'block';
                                    }
                                }
                            }

                        }
                        DisplayMessagePopup("Invalid or missing data!");
                        ValidatorValidate(document.getElementById(gridClientId + 'valtxtRoyaltor' + '_' + i));
                        ValidatorValidate(document.getElementById(gridClientId + 'valddlOptionPeriod' + '_' + i));

                        Page_BlockSubmit = false;
                        return false;
                    }
                    else if (trackStatus == "0" && txtRoyaltor == "" && ddlOptionPeriod == "-") {
                        document.getElementById(gridClientId + 'valtxtRoyaltor' + '_' + i).enabled = false;
                        document.getElementById(gridClientId + 'valddlOptionPeriod' + '_' + i).enabled = false;
                    }
                }
            }

            //WUIN-601
            //validation - cannot set Exclude to checked when there is active participant in the track
            cbActive = document.getElementById(gridClientId + 'cbActive' + '_' + i);
            if (cbExclude.checked && cbActive.checked) {
                DisplayMessagePopup("Cannot Exclude when there is an active participant in the track!");
                Page_BlockSubmit = false;
                return false;
            }
        }

        for (var j = 0; j < gvRows.length; j++) {
            hdnTrackListingIdRow = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + j).value;
            for (var j = 0; j < gvRows.length; j++) {
                hdnTrackListingIdSelected = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + j).value;
                if (hdnTrackListingIdSelected == hdnTrackListingIdRow) {
                    if (ValDuplicateParticipInTrack(hdnTrackListingIdRow, j) == false) {
                        DisplayMessagePopup("Royaltor, option period, territory and escalation code cannot be same for two active participants within a track!");
                        Page_BlockSubmit = false;
                        return false;
                    }
                }
            }
        }

    }


    Page_BlockSubmit = false;//to handle next valid post back correctly
    return true;
}



//to validate 
//  1. if any changes are un saved in track group(other than the participant being copied), and copy option is selected then warning message 
//  2. if any changes are un saved in other track group, and copy option is selected then warning message 
var trackIdOnUnSavedDataExit;//these are used in copy functionality when selected from unsaved pop up Exit
var rowIndexOnUnSavedDataExit;
function ValidateCopyParticipant(gridRow) {
    isValid = false;
    Page_BlockSubmit = false;
    var selectedRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
    var hdnTrackListingId = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + selectedRowIndex).value;
    //check if there are more than one track to copy the participant
    var numOfTracks = 0;
    gvTrackListing = document.getElementById("<%= gvTrackListing.ClientID %>");
    if (gvTrackListing != null) {
        var gvRows = gvTrackListing.rows;
        for (var i = 0; i < gvRows.length; i++) {
            hdnDisplayOrder = document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + i).value;
            if (hdnDisplayOrder == "1") {
                numOfTracks++;

                if (numOfTracks > 1)
                    break;
            }
        }
    }

    if (!(numOfTracks > 1)) {
        DisplayMessagePopup("There should be more than one track for a participant to be copied!");
        return false;
    }

    var hdnISRCPartId = document.getElementById(gridClientId + 'hdnISRCPartId' + '_' + selectedRowIndex).value;

    if (IsGridDataChanged()) {
        document.getElementById("<%=hdnButtonSelection.ClientID %>").value = "Copy";
        trackIdOnUnSavedDataExit = hdnTrackListingId;
        rowIndexOnUnSavedDataExit = selectedRowIndex;
        OpenOnUnSavedData();
        isValid = false;
    }
    else {
        isValid = true;
    }

    //WUIN-944 changes:cannot copy with duplicate participant with in track
    //validation: with in a track and for an active participant, royaltor & option period cannot be same for two particiipants
    if (ValDuplicateParticipInTrack(hdnTrackListingId, 0) == false) {
        DisplayMessagePopup("Royaltor, option period, territory and escalation code cannot be same for two active participants within a track!");
        Page_BlockSubmit = false;
        return false;
    }

    if (isValid == true) {
        //validations are ok, pop up copy notifications    
        var popup = $find('<%= mpeCopyParticip1.ClientID %>');
        if (popup != null) {
            //set copied grid row id and tracklisting id for copying
            document.getElementById("<%=hdnCopyParticipRowIndex.ClientID %>").innerText = selectedRowIndex;
            document.getElementById("<%=hdnCopyParticipTrackListingId.ClientID %>").innerText = hdnTrackListingId;
            popup.show();
        }
    }


    return false;

}

//WUIN-594
//when copy all is selected validate if some tracks have status not < 2, display a warning 
function ValidateCopyToAllTracks() {
    hdnCopyParticipTrackListingId = document.getElementById("<%=hdnCopyParticipTrackListingId.ClientID %>").value;
    //close copy popup1
    var popup1 = $find('<%= mpeCopyParticip1.ClientID %>');
    if (popup1 != null) {
        popup1.hide();
    }

    if (ValCurPageTrackStatusToCopy(hdnCopyParticipTrackListingId) == true) {
        //Harish: This will check for the tracks on current page only.
        //To avoid more work around on this validation here, this is also being validated on server side for all tracks of all pages
        return true;
    }
    else {
        //pop up confirmation
        var popup3 = $find('<%= mpeCopyParticip3.ClientID %>');
        if (popup3 != null) {
            document.getElementById("<%=hdnCopyParticip3PopUp.ClientID %>").innerText = "All";
            document.getElementById("<%=lblmpeCopyParticip3.ClientID %>").innerText =
                "Cannot copy to all tracks. Some tracks are signed off. Do you want to copy Participants to all other tracks?";
            popup3.show();
        }

        return false;
    }
}

//WUIN-713
//when copy to current page tracks is selected, validate if some tracks have status not < 2, display a warning 
function ValidateCopyToCurPageTracks() {
    hdnCopyParticipTrackListingId = document.getElementById("<%=hdnCopyParticipTrackListingId.ClientID %>").value;
    //close copy popup1
    var popup1 = $find('<%= mpeCopyParticip1.ClientID %>');
    if (popup1 != null) {
        popup1.hide();
    }

    if (ValCurPageTrackStatusToCopy(hdnCopyParticipTrackListingId) == true) {
        return true;
    }
    else {
        var popup3 = $find('<%= mpeCopyParticip3.ClientID %>');
        if (popup3 != null) {
            document.getElementById("<%=hdnCopyParticip3PopUp.ClientID %>").innerText = "CurrentPage";
            document.getElementById("<%=lblmpeCopyParticip3.ClientID %>").innerText =
                "Cannot copy to all tracks. Some tracks are signed off. Do you want to copy Participants to all other current page tracks?";
            popup3.show();
        }

        return false;
    }
}

//WUIN-594
//validate if there are tracks selected to be copied    
//check if any of selected tracks are signed off.
function ValidateCopyToSelectedTracks() {
    var trackSelected = false;
    gvCopyParticipTrackList = document.getElementById("<%= gvCopyParticipTrackList.ClientID %>");
    if (gvCopyParticipTrackList != null) {
        var gvRows = gvCopyParticipTrackList.rows;
        if (gvRows.length == 0) {
            DisplayMessagePopup("No tracks to copy");
            return false;
        }

        for (var i = 0; i < gvRows.length; i++) {
            cbCopyTrackSelect = document.getElementById(gridClientIdCopy2 + 'cbCopyTrackSelect' + '_' + i);
            if (cbCopyTrackSelect == null) {
                DisplayMessagePopup("No tracks to copy");
                return false;
            }

            if (cbCopyTrackSelect.checked) {
                trackSelected = true;
                break;
            }
        }
    }

    if (trackSelected) {
        if (ValidateSelectedTrackStatusToCopy() == false) {
            //some of selected tracks are signed off
            //pop up confirmatiom to proceed with copying to non signed off tracks
            //pop up confirmation
            var popup3 = $find('<%= mpeCopyParticip3.ClientID %>');
            if (popup3 != null) {
                document.getElementById("<%=hdnCopyParticip3PopUp.ClientID %>").innerText = "Selected";
                document.getElementById("<%=lblmpeCopyParticip3.ClientID %>").innerText =
                    "Cannot copy to all tracks. Some selected tracks are signed off. Do you want to copy Participants to all other selected tracks?";
                popup3.show();
            }

            return false;
        }
        else {
            return true;
        }
    }
    else {
        DisplayMessagePopup("Please select tracks to copy");
        return false;
    }
}

//WUIN-594
//check if any of selected tracks are signed off.
//If some selected tracks have status not < 2, display a warning 'Cannot copy to all selected tracks. 
function ValidateSelectedTrackStatusToCopy() {
    var isTrackSignedOff = false;
    gvCopyParticipTrackList = document.getElementById("<%= gvCopyParticipTrackList.ClientID %>");

        if (gvCopyParticipTrackList != null) {
            var gvRows = gvCopyParticipTrackList.rows;
            for (var i = 0; i < gvRows.length; i++) {
                cbCopyTrackSelect = document.getElementById(gridClientIdCopy2 + 'cbCopyTrackSelect' + '_' + i);
                if (cbCopyTrackSelect == null) {
                    DisplayMessagePopup("No tracks to copy");
                    return false;
                }

                if (cbCopyTrackSelect.checked) {
                    hdnStatusCode = document.getElementById(gridClientIdCopy2 + 'hdnStatusCode' + '_' + i).value;
                    if (hdnStatusCode >= 2) {
                        isTrackSignedOff = true;
                        break;
                    }

                }

            }
        }

        if (isTrackSignedOff == true) {
            return false;
        }
        else {
            return true;
        }
    }

    //WUIN-444 - When copy participants is selected for a track, only copy to other tracks with status < 2 (Under Review or No Participants). 
    //If some tracks have status not < 2, display a warning 'Cannot copy to all tracks. 
    //Some tracks are signed off. Do you want to copy Participants to all other tracks?' with Yes or No options to select.
    //if yes, then copy to all other tracks which are not signed off
    //if no, do not copy and close popup
    //WUIN-713 - Harish 18-07-18: This will check for tracks on current page only
    function ValCurPageTrackStatusToCopy(trkListIdSelectedRow) {
        var isTrackSignedOff = false;
        gvTrackListing = document.getElementById("<%= gvTrackListing.ClientID %>");

    if (gvTrackListing != null) {
        var gvRows = gvTrackListing.rows;
        for (var i = 0; i < gvRows.length; i++) {

            hdnDisplayOrder = document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + i).value;
            hdnTrackListingId = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + i).value;

            if (hdnDisplayOrder == "1") {
                if (document.getElementById(gridClientId + 'ddlStatus' + '_' + i) != null) {
                    ddlStatus = document.getElementById(gridClientId + 'ddlStatus' + '_' + i).value;

                    if ((trkListIdSelectedRow != hdnTrackListingId) && ddlStatus >= 2) {
                        isTrackSignedOff = true;
                        break;
                    }
                }

            }

        }
    }

    if (isTrackSignedOff == true) {
        return false;
    }
    else {
        return true;
    }
}


//If an existing PARTICIPATION has MANUAL_OVERRIDE = 'Y', then display message 'One or more Participants has a manual override, do you want to Continue?' Continue or Cancel
function ValConsolidate() {
    //validate for any changes made are not saved
    if (IsDataChanged()) {
        DisplayMessagePopup("Please save the changes made before consolidating!");
        return false;
    }

    //validation:Royaltor and option period and active combination cannot have different esc codes or seller group codes
    //Harish: 11-01-2019 : This validation is added as part of WUIN-110.
    //          as discussed with Susmitha, this validation is being removed as part of WUIN-920      
    //if (ValUniqueParticipants() == false) {
    //    DisplayMessagePopup("Royaltor and option period combination cannot have different esc codes or territories!");

    //    return false;
    //}

    var manualOverrideCount = document.getElementById("<%=hdnManualOverride.ClientID %>").value;
    if (parseInt(manualOverrideCount) > 0) {
        var popupConsolidate = $find('<%= mpeConfirmConsolidate.ClientID %>');
        if (popupConsolidate != null) {
            popupConsolidate.show();
        }

        return false;
    }
    else {
        return true;
    }


}

//validation:Royaltor and option period and active combination cannot have different esc codes or seller group codes across all tracks
//Harish: 11-01-2019 : This validation is added as part of WUIN-110.
//          as discussed with Susmitha, this validation is being removed as part of WUIN-920        
//function ValUniqueParticipants() {
//
//        var isDifferent = "N";
//
//        gvTrackListing1 = document.getElementById("<%= gvTrackListing.ClientID %>");
        //
        //        if (gvTrackListing1 != null) {
        //            var gvRows = gvTrackListing1.rows;
        //            for (var i = 0; i < gvRows.length; i++) {
        //
        //                hdnDisplayOrder1 = document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + i).value;
        //
        //                if (hdnDisplayOrder1 == "1") {
        //                    continue;
        //                }
        //
        //                txtRoyaltor1 = document.getElementById(gridClientId + 'txtRoyaltor' + '_' + i).value;
        //
        //                ddlOptionPeriodCtrl1 = document.getElementById(gridClientId + 'ddlOptionPeriod' + '_' + i);
        //                if (ddlOptionPeriodCtrl1 != null) {
        //                    ddlOptionPeriod1 = document.getElementById(gridClientId + 'ddlOptionPeriod' + '_' + i).value;
        //                }
        //                else {
        //                    txtOptionPeriod1 = document.getElementById(gridClientId + 'txtOptionPeriod' + '_' + i).value;
        //                    ddlOptionPeriod1 = txtOptionPeriod1.substring(0, txtOptionPeriod1.lastIndexOf("-") - 1);
        //                }
        //
        //                ddlEscCode1 = document.getElementById(gridClientId + 'ddlEscCode' + '_' + i).value;
        //                txtTerritory1 = document.getElementById(gridClientId + 'txtTerritory' + '_' + i).value;
        //                cbActive1 = document.getElementById(gridClientId + 'cbActive' + '_' + i);
        //
        //                //for each royaltor and option period combination and active check if either esc_code or seller_group_code is same. warning if not
        //                gvTrackListing2 = document.getElementById("<%= gvTrackListing.ClientID %>");
        //                    var gvRows2 = gvTrackListing2.rows;
        //                    for (var j = 0; j < gvRows2.length; j++) {
        //
        //                        hdnDisplayOrder2 = document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + j).value;
        //
        //                        if (hdnDisplayOrder2 == "1") {
        //                            continue;
        //                        }
        //
        //                        txtRoyaltor2 = document.getElementById(gridClientId + 'txtRoyaltor' + '_' + j).value;
        //
        //                        ddlOptionPeriodCtrl2 = document.getElementById(gridClientId + 'ddlOptionPeriod' + '_' + j);
        //                        if (ddlOptionPeriodCtrl2 != null) {
        //                            ddlOptionPeriod2 = document.getElementById(gridClientId + 'ddlOptionPeriod' + '_' + j).value;
        //                        }
        //                        else {
        //                            txtOptionPeriod2 = document.getElementById(gridClientId + 'txtOptionPeriod' + '_' + j).value;
        //                            ddlOptionPeriod2 = txtOptionPeriod2.substring(0, txtOptionPeriod2.lastIndexOf("-") - 1);
        //                        }
        //
        //                        ddlEscCode2 = document.getElementById(gridClientId + 'ddlEscCode' + '_' + j).value;
        //                        txtTerritory2 = document.getElementById(gridClientId + 'txtTerritory' + '_' + j).value;
        //                        cbActive2 = document.getElementById(gridClientId + 'cbActive' + '_' + j);
        //
        //                        if (txtRoyaltor1 == txtRoyaltor2 && ddlOptionPeriod1 == ddlOptionPeriod2 && cbActive1.checked == cbActive2.checked) {
        //                            if (ddlEscCode1 != ddlEscCode2 || txtTerritory1 != txtTerritory2) {
        //                                isDifferent = "Y";
        //                                break;
        //                            }
        //
        //                        }
        //
        //                    }
        //
        //                    if (isDifferent == "Y") {
        //                        break;
        //                    }
        //
        //
        //                }
        //            }
        //
        //            if (isDifferent == "Y") {
        //                return false;
        //            }
        //            else {
        //                return true;
        //            }
        //
        //
        //        }

        //Harish 08-01-2018 WUIN-365 changes:cannot add duplicate participant with in track
        //WUIN-944 - Validation: with in a track and for an active participant, royaltor & option period & Territory & Esc code cannot be same for two participants
        function ValDuplicateParticipInTrack(trkListIdSelectedRow, selectedRowIndex) {
            isDuplicateExist = "N";
            gvTrackListing1 = document.getElementById("<%= gvTrackListing.ClientID %>");
            if (gvTrackListing1 != null) {
                var gvRows = gvTrackListing1.rows;

                for (var i = selectedRowIndex; i < gvRows.length; i++) {

                    hdnDisplayOrder1 = document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + i).value;
                    hdnTrackListingIdRow1 = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + i).value;
                    cbActive1 = document.getElementById(gridClientId + 'cbActive' + '_' + i);

                    if (hdnTrackListingIdRow1 != trkListIdSelectedRow || hdnDisplayOrder1 == "1" || cbActive1.checked == false) {
                        //do checks only for the selected track and child rows and for active
                        continue;
                    }

                    txtRoyaltor1 = document.getElementById(gridClientId + 'txtRoyaltor' + '_' + i).value;

                    ddlOptionPeriodCtrl1 = document.getElementById(gridClientId + 'ddlOptionPeriod' + '_' + i);
                    if (ddlOptionPeriodCtrl1 != null) {
                        ddlOptionPeriod1 = document.getElementById(gridClientId + 'ddlOptionPeriod' + '_' + i).value;
                    }
                    else {
                        txtOptionPeriod1 = document.getElementById(gridClientId + 'txtOptionPeriod' + '_' + i).value;
                        ddlOptionPeriod1 = txtOptionPeriod1.substring(0, txtOptionPeriod1.lastIndexOf("-") - 1);
                    }

                    txtTerritory1 = document.getElementById(gridClientId + 'txtTerritory' + '_' + i).value;
                    ddlEscCode1 = document.getElementById(gridClientId + 'ddlEscCode' + '_' + i).value;

                    //check if there are duplicate royaltor & option period combination
                    royaltorOptionCount = 0;
                    gvTrackListing2 = document.getElementById("<%= gvTrackListing.ClientID %>");
                    var gvRows2 = gvTrackListing2.rows;
                    for (var j = selectedRowIndex; j < gvRows2.length; j++) {

                        hdnDisplayOrder2 = document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + j).value;
                        hdnTrackListingIdRow2 = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + j).value;
                        cbActive2 = document.getElementById(gridClientId + 'cbActive' + '_' + j);

                        if (hdnTrackListingIdRow2 != trkListIdSelectedRow || hdnDisplayOrder2 == "1" || cbActive2.checked == false) {
                            //do checks only for the selected track and child rows and for active
                            continue;
                        }

                        txtRoyaltor2 = document.getElementById(gridClientId + 'txtRoyaltor' + '_' + j).value;

                        ddlOptionPeriodCtrl2 = document.getElementById(gridClientId + 'ddlOptionPeriod' + '_' + j);
                        if (ddlOptionPeriodCtrl2 != null) {
                            ddlOptionPeriod2 = document.getElementById(gridClientId + 'ddlOptionPeriod' + '_' + j).value;
                        }
                        else {
                            txtOptionPeriod2 = document.getElementById(gridClientId + 'txtOptionPeriod' + '_' + j).value;
                            ddlOptionPeriod2 = txtOptionPeriod2.substring(0, txtOptionPeriod2.lastIndexOf("-") - 1);
                        }

                        txtTerritory2 = document.getElementById(gridClientId + 'txtTerritory' + '_' + j).value;
                        ddlEscCode2 = document.getElementById(gridClientId + 'ddlEscCode' + '_' + j).value;

                        if (txtRoyaltor1 == txtRoyaltor2 && ddlOptionPeriod1 == ddlOptionPeriod2 && cbActive1.checked == cbActive2.checked && txtTerritory1 == txtTerritory2 && ddlEscCode1 == ddlEscCode2) {
                            royaltorOptionCount = royaltorOptionCount + 1;
                            if (royaltorOptionCount == 2) {
                                isDuplicateExist = "Y";
                                break;
                            }
                        }


                    }

                    if (isDuplicateExist == "Y") {
                        break;
                    }


                }

            }


            if (isDuplicateExist == "Y") {
                return false;
            }
            else {
                return true;
            }


        }

        //==========Validations - End

        //Display control text on hover
        function DisplayDdlTextOnHover(control) {
            var ctrlId = control.id;
            var popUpText = control.options[control.selectedIndex].text;
            control.title = popUpText;
        }

        //Warn on changes made and not saved

        function WarnOnUnSavedData() {

            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            if (isExceptionRaised != "Y") {
                if (IsCatDataChanged() || IsGridDataChanged()) {
                    return warningMsgOnUnSavedData;
                }
            }
        }
        window.onbeforeunload = WarnOnUnSavedData;

        function IsCatDataChanged() {
            var hdnTrackTimeFlag = document.getElementById("<%=hdnTrackTimeFlag.ClientID %>").value;
            var hdnCatStatusCode = document.getElementById("<%=hdnCatStatusCode.ClientID %>").value;

            if ((hdnTrackTimeFlag == "T" && document.getElementById("<%=rbCatTimeShare.ClientID %>").checked) ||
                (hdnCatStatusCode != "" && document.getElementById("<%=ddlCatStatus.ClientID %>").value != hdnCatStatusCode)) {
                return true;
            }
            else {
                return false;
            }
        }

        function IsGridDataChanged() {
            var changesMade = "N";
            var hdnDisplayOrder;
            var hdnRoyaltor;
            var hdnOptPeriodCode;
            var hdnSellerGrp;
            var hdnEscCode;
            var hdnActive;
            var hdnIsModified;
            var txtRoyaltor;
            var hdnRoyaltor;
            var ddlOptionPeriod;
            var txtTerritory;
            var ddlEscCode;
            var cbActive;
            var cbExclude;
            var cbIncInEsc;
            var ddlStatus;
            var hdnStatusCode;
            var hdnTrackListingId;
            gvTrackListing = document.getElementById("<%= gvTrackListing.ClientID %>");
            var hdnGridDataDeleted = document.getElementById("<%=hdnGridDataDeleted.ClientID %>").value;

            if (hdnGridDataDeleted == "Y") {
                return true;
            }

            if (gvTrackListing != null) {
                var gvRows = gvTrackListing.rows;
                for (var i = 0; i < gvRows.length; i++) {

                    //handling empty data row
                    if (gvRows.length == 1 && document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + i) == null) {
                        break;
                    }

                    hdnDisplayOrder = document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + i).value;
                    hdnIsModified = document.getElementById(gridClientId + 'hdnIsModified' + '_' + i).value;
                    hdnTrackListingId = document.getElementById(gridClientId + 'hdnTrackListingId' + '_' + i).value;
                    hdnIsTrackEditable = document.getElementById(gridClientId + 'hdnIsTrackEditable' + '_' + i).value;

                    if (hdnDisplayOrder == "1") {
                        hdnRoyaltor = document.getElementById(gridClientId + 'hdnRoyaltor' + '_' + i).value;
                        if (document.getElementById(gridClientId + 'ddlStatus' + '_' + i) != null) {
                            //check if track status is changed                        
                            ddlStatus = document.getElementById(gridClientId + 'ddlStatus' + '_' + i).value;
                            hdnStatusCode = document.getElementById(gridClientId + 'hdnStatusCode' + '_' + i).value;
                            if (ddlStatus != hdnStatusCode) {
                                changesMade = "Y";
                                break;
                            }
                        }

                        if (document.getElementById(gridClientId + 'cbExclude' + '_' + i) != null) {
                            //check if exclude is changed 
                            cbExclude = document.getElementById(gridClientId + 'cbExclude' + '_' + i);
                            hdnExclude = document.getElementById(gridClientId + 'hdnExclude' + '_' + i).value;
                            if ((hdnExclude == "N" && cbExclude.checked) || (hdnExclude == "Y" && !cbExclude.checked)) {
                                changesMade = "Y";
                                break;
                            }
                        }

                        continue;

                    }
                    else if (hdnIsModified == "-" && (hdnIsTrackEditable == "Y" && hdnRoyaltor != "No Participants" && hdnStatusCode != "0")) {
                        changesMade = "Y";
                        break;
                    }

                    txtRoyaltor = document.getElementById(gridClientId + 'txtRoyaltor' + '_' + i).value;
                    hdnRoyaltor = document.getElementById(gridClientId + 'hdnRoyaltor' + '_' + i).value;

                    ddlOptionPeriodCtrl = document.getElementById(gridClientId + 'ddlOptionPeriod' + '_' + i);
                    if (ddlOptionPeriodCtrl != null) {
                        ddlOptionPeriod = document.getElementById(gridClientId + 'ddlOptionPeriod' + '_' + i).value;
                    }

                    hdnOptPeriodCode = document.getElementById(gridClientId + 'hdnOptPeriodCode' + '_' + i).value;
                    txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + i).value;
                    hdnSellerGrp = document.getElementById(gridClientId + 'hdnSellerGrp' + '_' + i).value;
                    ddlEscCode = document.getElementById(gridClientId + 'ddlEscCode' + '_' + i).value;
                    hdnEscCode = document.getElementById(gridClientId + 'hdnEscCode' + '_' + i).value;
                    cbActive = document.getElementById(gridClientId + 'cbActive' + '_' + i);
                    hdnActive = document.getElementById(gridClientId + 'hdnActive' + '_' + i).value;
                    cbIncInEsc = document.getElementById(gridClientId + 'cbIncInEsc' + '_' + i);
                    hdnIncInEsc = document.getElementById(gridClientId + 'hdnIncInEsc' + '_' + i).value;

                    if (ddlOptionPeriodCtrl != null && ddlOptionPeriod == '-')
                        ddlOptionPeriod = '';

                    if (ddlEscCode == '-')
                        ddlEscCode = '';

                    if (hdnSellerGrp == '.')
                        hdnSellerGrp = '';

                    if (cbActive.checked) {
                        cbActive = "Y";
                    }
                    else {
                        cbActive = "N";
                    }

                    if (cbIncInEsc.checked) {
                        cbIncInEsc = "Y";
                    }
                    else {
                        cbIncInEsc = "N";
                    }

                    if (txtRoyaltor != hdnRoyaltor || (ddlOptionPeriodCtrl != null && ddlOptionPeriod != hdnOptPeriodCode) || txtTerritory != hdnSellerGrp ||
                        ddlEscCode != hdnEscCode || cbActive != hdnActive || cbIncInEsc != hdnIncInEsc) {
                        document.getElementById(gridClientId + 'hdnIsModified' + '_' + i).innerText = "Y";
                        changesMade = "Y";
                        break;
                    }


                }

            }
            if (changesMade == "Y" && isRoyaltorPostBack == false) {
                document.getElementById("<%=hdnChangesMadeNotSavedTrackId.ClientID %>").innerText = hdnTrackListingId;
                return true;
            }
            else {
                return false;
            }
        }

        //used to check if any changes to allow navigation to other screen 
        function IsDataChanged() {
            if (IsCatDataChanged() || IsGridDataChanged()) {
                return true;
            }
            else {
                return false;
            }
        }


        //WUIN-634
        //Resets catalogue values to initial ones
        function ResetCatalogueValues() {
            var hdnTrackTimeFlag = document.getElementById("<%=hdnTrackTimeFlag.ClientID %>").value;
            var hdnStatusCode = document.getElementById("<%=hdnCatStatusCode.ClientID %>").value;

            if (hdnTrackTimeFlag == "T") {
                document.getElementById("<%=rbCatTrackShare.ClientID %>").checked = true;
            }
            else {
                document.getElementById("<%=rbCatTimeShare.ClientID %>").checked = true;
            }

            document.getElementById("<%=ddlCatStatus.ClientID %>").value = hdnStatusCode;
        }

        //WUIN-634
        //Resets grid row values to initial ones
        function ResetGridRowValues(selectedRowIndex) {
            var hdnDisplayOrder = document.getElementById(gridClientId + 'hdnDisplayOrder' + '_' + selectedRowIndex).value;
            var hdnRoyaltor = document.getElementById(gridClientId + 'hdnRoyaltor' + '_' + selectedRowIndex).value;
            var hdnOptPeriodCode = document.getElementById(gridClientId + 'hdnOptPeriodCode' + '_' + selectedRowIndex).value;
            var hdnSellerGrpCode = document.getElementById(gridClientId + 'hdnSellerGrpCode' + '_' + selectedRowIndex).value;
            var hdnSellerGrp = document.getElementById(gridClientId + 'hdnSellerGrp' + '_' + selectedRowIndex).value;
            var hdnEscCode = document.getElementById(gridClientId + 'hdnEscCode' + '_' + selectedRowIndex).value;
            var hdnActive = document.getElementById(gridClientId + 'hdnActive' + '_' + selectedRowIndex).value;
            var hdnIncInEsc = document.getElementById(gridClientId + 'hdnIncInEsc' + '_' + selectedRowIndex).value;

            var txtRoyaltor = document.getElementById(gridClientId + 'txtRoyaltor' + '_' + selectedRowIndex);
            var ddlOptionPeriod = document.getElementById(gridClientId + 'ddlOptionPeriod' + '_' + selectedRowIndex);
            var txtTerritory = document.getElementById(gridClientId + 'txtTerritory' + '_' + selectedRowIndex);
            var ddlEscCode = document.getElementById(gridClientId + 'ddlEscCode' + '_' + selectedRowIndex);
            var cbActive = document.getElementById(gridClientId + 'cbActive' + '_' + selectedRowIndex);
            var cbIncInEsc = document.getElementById(gridClientId + 'cbIncInEsc' + '_' + selectedRowIndex);

            if (hdnDisplayOrder == "1") {
                //Track header row
                var hdnStatusCode = document.getElementById(gridClientId + 'hdnStatusCode' + '_' + selectedRowIndex).value;
                var ddlStatus = document.getElementById(gridClientId + 'ddlStatus' + '_' + selectedRowIndex);
                ddlStatus.value = hdnStatusCode;

                var hdnExclude = document.getElementById(gridClientId + 'hdnExclude' + '_' + selectedRowIndex).value;
                var cbExclude = document.getElementById(gridClientId + 'cbExclude' + '_' + selectedRowIndex);
                if (hdnExclude == "Y") {
                    cbExclude.checked = true;
                }
                else {
                    cbExclude.checked = false;
                }

                return;
            }

            txtRoyaltor.innerText = hdnRoyaltor;

            if (ddlOptionPeriod != null) {
                if (hdnOptPeriodCode != "") {
                    ddlOptionPeriod.value = hdnOptPeriodCode;
                }
                else {
                    ddlOptionPeriod.value = "-";
                }
            }

            txtTerritory.innerText = hdnSellerGrp;

            if (hdnEscCode != "") {
                ddlEscCode.value = hdnEscCode;
            }
            else {
                ddlEscCode.value = "-";
            }

            if (hdnActive == "Y") {
                cbActive.checked = true;
            }
            else {
                cbActive.checked = false;
            }

            if (hdnIncInEsc == "Y") {
                cbIncInEsc.checked = true;
            }
            else {
                cbIncInEsc.checked = false;
            }

            //set is_modified flag to N
            document.getElementById(gridClientId + 'hdnIsModified' + '_' + selectedRowIndex).innerText = "N";
        }



        //Comment textbox max length 510
        function CommentBoxMaxLength() {
            var maxLength = 510;
            var txtComment = document.getElementById("<%=txtComment.ClientID %>");
            if (txtComment.value.length > maxLength) {
                txtComment.value = txtComment.value.substring(0, maxLength);
                return false;
            }
            return true;

        }


        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        //===========Warn on changes made and not saved === End

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= rbCatTrackShare.ClientID %>").focus();
            }
        }

        function FocusLblKeyPress() {
            document.getElementById("<%= rbCatTrackShare.ClientID %>").focus();
        }

        //=============== End

        //Generic function to ignore any client validation failure and do post back
        function SubmitPage() {
            Page_BlockSubmit = false;
            return true;
        }
        //=============== End

        //validate if changes made not saved on page change
        function ValidatePageChange(obj, button) {

            //on Exit button from data unsaved pop up
            if (document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value == "Y") {
                return true;
            }
            document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "N";

            if (IsGridDataChanged()) {
                //populate hdnPageIndex with the selected page number
                document.getElementById("<%=hdnPageIndex.ClientID %>").value = obj.innerHTML;

                window.onbeforeunload = null;
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
            }
            else {
                return true;
            }
        }

        //validate if changes made not saved on filter tracks
        function ValidateFilterTracks(button) {
            //validate filter fields        
            if (!Page_ClientValidate("ValGrpSearchFilters")) {
                Page_BlockSubmit = false;
                DisplayMessagePopup("Invalid filter Data!");
                return false;
            }

            //on Exit button from data unsaved pop up
            if (document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value == "Y") {
                return true;
            }
            document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "N";

            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
                return false;
            }
            else {
                return true;
            }
        }

        //validate if changes made not saved on clear filters
        function ValidateClearFilters(button) {

            //on Exit button from data unsaved pop up
            if (document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value == "Y") {
                return true;
            }
            document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "N";

            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
                return false;
            }
            else {
                return true;
            }
        }

        //Validate txtSideFilter filter field
        //only numbers are allowed. Can enter only when Unit is entered
        function ValtxtSideFilter(sender, args) {
            txtUnitFilter = document.getElementById("<%=txtUnitFilter.ClientID %>").value;
            txtSideFilter = document.getElementById("<%=txtSideFilter.ClientID %>").value;
            valtxtSideFilter = document.getElementById("<%=valtxtSideFilter.ClientID %>");
            if (txtSideFilter != "") {
                if (txtUnitFilter == "") {
                    args.IsValid = false;
                    valtxtSideFilter.title = "Can be entered only when Unit is entered";
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
            var hdnButtonSelection = document.getElementById("<%=hdnButtonSelection.ClientID %>").value;

            if (hdnButtonSelection == "Copy") {
                var popup = $find('<%= mpeCopyParticip1.ClientID %>');
                if (popup != null) {
                    //set copied grid row id and tracklisting id for copying
                    document.getElementById("<%=hdnCopyParticipRowIndex.ClientID %>").innerText = rowIndexOnUnSavedDataExit;
                    document.getElementById("<%=hdnCopyParticipTrackListingId.ClientID %>").innerText = trackIdOnUnSavedDataExit;
                    popup.show();
                }

                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = "";
                var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
                warnPopup.hide();
                return false;
            }
            else if (hdnButtonSelection == "Comment") {

                //WUIN-634 - validations
                //When Catalogue status is in Manager sign off, Editor user cannot edit any fields in the screen
                //when Track is in Manager sign off, Editor user cannot edit Participants of the track
                //if a product is auto sign off then allow Editor user to make changes(to participants) at any status
                var hdnUserRole = document.getElementById("<%=hdnUserRole.ClientID %>").value;
                var hdnCatStatusCode = document.getElementById("<%=hdnCatStatusCode.ClientID %>").value;
                var hdnCatAutoSign = document.getElementById("<%=hdnCatAutoSign.ClientID %>").value;
                var hdnTrackStatusCode = document.getElementById(gridClientId + 'hdnStatusCode' + '_' + rowIndexOnUnSavedDataExit).value;
                //debugger;
                CommentPopup = $find('<%= mpeCommentPopup.ClientID %>');
                if (CommentPopup != null) {
                    CommentPopup.show();
                }
                else {
                    DisplayMessagePopup("Error in opening comment");
                    return false;
                }

                document.getElementById("<%=hdnCommentISRCDealId.ClientID %>").innerText = document.getElementById(gridClientId + 'hdnISRCDealId' + '_' + rowIndexOnUnSavedDataExit).value;
                document.getElementById("<%=txtComment.ClientID %>").innerText = document.getElementById(gridClientId + 'hdnComments' + '_' + rowIndexOnUnSavedDataExit).value;

                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = "";
                var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
                warnPopup.hide();
                return false;
            }

        var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
            warnPopup.hide();

            return true;
        }
        //============== End

    </script>
    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>

            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="2">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    TRACK LISTING
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
                                            <td width="12%" class="gridHeaderStyle_1row">Deal Type</td>
                                            <td width="6%" class="gridHeaderStyle_1row">Compilation?</td>
                                            <td width="6%" class="gridHeaderStyle_1row">Total Tracks</td>
                                            <td width="6%" class="gridHeaderStyle_1row">Total Time</td>
                                            <td width="6%" class="gridHeaderStyle_1row">Track Share</td>
                                            <td width="6%" class="gridHeaderStyle_1row">Time Share</td>
                                            <td width="10%" class="gridHeaderStyle_1row">Status</td>
                                            <td width="3%" class="gridHeaderStyle_1row">&nbsp</td>
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
                                                <asp:RadioButton ID="rbCatTrackShare" runat="server" CssClass="identifierLable" GroupName="CatNoTrackTime" TabIndex="100" />
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:RadioButton ID="rbCatTimeShare" runat="server" CssClass="identifierLable" GroupName="CatNoTrackTime" TabIndex="101" />
                                            </td>
                                            <td class="insertBoxStyle" style="padding-top: 3px; padding-bottom: 3px;">
                                                <asp:DropDownList ID="ddlCatStatus" runat="server" Width="98%" CssClass="ddlStyle" TabIndex="102"
                                                    onchange="return ValidateCatNoStatus();">
                                                </asp:DropDownList>
                                            </td>

                                            <td class="insertBoxStyle" align="center">
                                                <asp:ImageButton ID="btnUndoSaveCat" runat="server" ImageUrl="../Images/cancel_row3.png"
                                                    ToolTip="Undo Changes" CausesValidation="false" TabIndex="103" OnClientClick="return UndoCatNoChanges();" />


                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <asp:HiddenField ID="hdnTrackTimeFlag" runat="server" />
                            <asp:HiddenField ID="hdnCatStatusCode" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnCatAutoSign" runat="server" />
                        </table>
                    </td>
                    <td width="11%" rowspan="3" valign="top" align="right">
                        <table width="100%">
                            <tr>
                                <td width="5%"></td>
                                <td align="right" width="70%">
                                    <asp:Button ID="btnSaveAllChanges" runat="server" CssClass="ButtonStyle" ValidationGroup="valSave" OnClick="btnSaveAllChanges_Click"
                                        Text="Save Changes" UseSubmitBehavior="false" Width="90%" TabIndex="109" OnClientClick="if (!ValidateSaveAllChanges()) { return false;};" />
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td align="right">
                                    <asp:Button ID="btnConsolidate" runat="server" CssClass="ButtonStyle"
                                        Text="Consolidate" UseSubmitBehavior="false" Width="90%" ValidationGroup="valSave" TabIndex="110"
                                        OnClientClick="if (!ValConsolidate()) { return false;};" OnClick="btnConsolidate_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td align="right">
                                    <asp:Button ID="btnPartSummary" runat="server" CssClass="ButtonStyle" OnClick="btnPartSummary_Click"
                                        Text="Participant Summary" UseSubmitBehavior="false" Width="90%" TabIndex="111" OnClientClick="if (!ParticipantSummaryScreen()) { return false;};" />
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <asp:Button ID="Button1" runat="server" CssClass="ButtonStyle"
                                        Text="Catalogue Maintenance" UseSubmitBehavior="false" Width="90%" TabIndex="112" OnClientClick="if (!CatalogueMaintScreen()) { return false;};" />
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td align="right">
                                    <asp:Button ID="btnMissingParticip" runat="server" CssClass="ButtonStyle"
                                        Text="Missing Participants" UseSubmitBehavior="false" Width="90%" TabIndex="113" OnClientClick="if (!MissingParticipScreen()) { return false;};"
                                        onkeydown="OnTabPress();" />
                                </td>
                            </tr>
                        </table>

                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td style="padding-left: 15px">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="10%" class="identifierLable_large_bold">Track Filters</td>
                                <td width="4%" class="identifierLable_large_bold">Status</td>
                                <td width="10%">
                                    <asp:DropDownList ID="ddlTrackStatusFilter" runat="server" Width="98%" CssClass="ddlStyle" TabIndex="104"></asp:DropDownList>
                                </td>
                                <td width="5%"></td>
                                <td width="4%" class="identifierLable_large_bold">Unit</td>
                                <td width="8%">
                                    <asp:TextBox ID="txtUnitFilter" runat="server" Width="95%" CssClass="textboxStyle" ValidationGroup="ValGrpSearchFilters" TabIndex="105"></asp:TextBox>
                                </td>
                                <td width="5%">
                                    <asp:RegularExpressionValidator ID="revTxtUnitFilter" runat="server" Text="*" ControlToValidate="txtUnitFilter"
                                        ValidationExpression="^[0-9]\d*$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="ValGrpSearchFilters"
                                        ToolTip="Please enter only numbers" Display="Dynamic"> </asp:RegularExpressionValidator>
                                </td>
                                <td width="4%" class="identifierLable_large_bold">Side</td>
                                <td width="8%">
                                    <asp:TextBox ID="txtSideFilter" runat="server" Width="95%" CssClass="textboxStyle" ValidationGroup="ValGrpSearchFilters"
                                        MaxLength="20" ToolTip="Can be entered only when Unit is entered" Style="text-transform: uppercase;" TabIndex="106"></asp:TextBox>
                                </td>
                                <td width="5%">
                                    <asp:CustomValidator ID="valtxtSideFilter" runat="server" ValidationGroup="ValGrpSearchFilters" CssClass="requiredFieldValidator"
                                        ClientValidationFunction="ValtxtSideFilter" ToolTip="Please enter upto 20 characters"
                                        ControlToValidate="txtSideFilter" ErrorMessage="*" Display="Dynamic" ValidateEmptyText="true"></asp:CustomValidator>
                                </td>
                                <td width="7%">
                                    <asp:Button ID="btnFilterTracks" runat="server" CssClass="ButtonStyle" OnClientClick="if (!ValidateFilterTracks('FilterTracks')) { return false;};" OnClick="btnFilterTracks_Click"
                                        Text="Filter Tracks" UseSubmitBehavior="false" ValidationGroup="ValGrpSearchFilters" Width="95%" TabIndex="107" />
                                </td>
                                <td width="3%"></td>
                                <td width="7%">
                                    <asp:Button ID="btnClearFilters" runat="server" CssClass="ButtonStyle" OnClientClick="if (!ValidateClearFilters('ClearFilters')) { return false;};" OnClick="btnClearFilters_Click"
                                        Text="Clear" UseSubmitBehavior="false" Width="50%" CausesValidation="false" TabIndex="108" />
                                </td>
                                <td width="27%"></td>
                            </tr>
                        </table>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td class="table_header_with_border" valign="top" colspan="2" style="padding-left: 15px">Track Listing</td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table width="100%" class="table_with_border" cellpadding="0" cellspacing="0">
                            <tr>
                                <td style="padding-left: 10px; padding-top: 10px">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <table width="98.75%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td width="3%" class="gridHeaderStyle_1row_Left_Align" style="padding-left: 3px">
                                                            <table width="98%" cellpadding="0" cellspacing="0" style="float: left; table-layout: fixed">
                                                                <tr>
                                                                    <td width="30%">
                                                                        <asp:ImageButton ID="imgExpandAll" runat="server" ImageUrl="../Images/Plus.gif" OnClientClick="return ExpandGridGroupAll();" />
                                                                        <asp:ImageButton ID="imgCollapseAll" runat="server" ImageUrl="../Images/Minus.gif" OnClientClick="return CollapseGridGroupAll();" />
                                                                    </td>
                                                                    <td width="70%">Track</td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td width="5%" class="gridHeaderStyle_1row">ISRC</td>
                                                        <td width="10%" class="gridHeaderStyle_1row">Track Title</td>
                                                        <td width="7%" class="gridHeaderStyle_1row">Artist</td>
                                                        <td width="5%" class="gridHeaderStyle_1row">Team Responsibility</td>
                                                        <td width="4%" class="gridHeaderStyle_1row">Track Time</td>
                                                        <td width="10%" class="gridHeaderStyle_1row">Royaltor</td>
                                                        <td width="7%" class="gridHeaderStyle_1row">Option Period</td>
                                                        <td width="9%" class="gridHeaderStyle_1row">Territory</td>
                                                        <td width="3%" class="gridHeaderStyle_1row">Esc Code</td>
                                                        <td width="3%" class="gridHeaderStyle_1row">Inc in Esc</td>
                                                        <td width="3%" class="gridHeaderStyle_1row">Active?</td>
                                                        <td width="3%" class="gridHeaderStyle_1row">Exclude?</td>
                                                        <td width="6%" class="gridHeaderStyle_1row">Status</td>
                                                        <td width="3%" class="gridHeaderStyle_1row">Comment</td>
                                                        <td width="3%" class="gridHeaderStyle_1row">&nbsp</td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-left: 10px; padding-bottom: 10px">
                                    <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="100%">
                                        <asp:GridView ID="gvTrackListing" runat="server" AutoGenerateColumns="False" Width="98.75%"
                                            CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" ShowHeaderWhenEmpty="true" EmptyDataText="No Data Found"
                                            EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" OnRowDataBound="gvTrackListing_RowDataBound" OnRowCommand="gvTrackListing_RowCommand" ShowHeader="false">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-Width="3%" ItemStyle-CssClass="gridItemStyle_Left_Align">
                                                    <ItemTemplate>
                                                        <table width="98%" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td width="30%" align="left">
                                                                    <asp:ImageButton ID="imgExpand" runat="server" ImageUrl="../Images/Plus.gif" OnClientClick="return ExpandGridGroup(this);" />
                                                                    <asp:ImageButton ID="imgCollapse" runat="server" ImageUrl="../Images/Minus.gif" OnClientClick="return CollapseGridGroup(this);" />
                                                                </td>
                                                                <td width="70%" align="left">
                                                                    <asp:Label ID="lblTrack" runat="server" Text='<%#Bind("seq_no")%>' CssClass="identifierLable" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="lblISRC" runat="server" Width="98%" Text='<%#Bind("isrc")%>' CssClass="gridTextField" ReadOnly="true"
                                                            ToolTip='<%#Bind("isrc")%>' Font-Bold="true"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTrackTitle" runat="server" Text='<%#Bind("track_title")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="7%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblArtistName" runat="server" Text='<%#Bind("artist_name")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblResponsibility" runat="server" Text='<%#Bind("responsibility_desc")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTrackTime" runat="server" Text='<%#Bind("play_length")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRoyaltor" runat="server" Text='<%#Bind("royaltor")%>' CssClass="identifierLable" Visible="false" />
                                                        <asp:TextBox ID="txtRoyaltor" runat="server" Width="97%" Text='<%#Bind("royaltor")%>' CssClass="textbox_FuzzySearch"
                                                            OnChange="if (!OnRoyFuzzySearchChange(this)) { return false;};" onkeydown="OntxtRoyaltorKeyDown(this);"
                                                            ToolTip='<%#Bind("royaltor")%>'></asp:TextBox>
                                                        <ajaxToolkit:AutoCompleteExtender ID="aceRoyaltor" runat="server"
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
                                                            CompletionListElementID="pnlRoyFuzzySearch" />
                                                        <asp:Panel ID="pnlRoyFuzzySearch" runat="server" CssClass="identifierLable" />
                                                        <asp:CustomValidator ID="valtxtRoyaltor" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator"
                                                            ClientValidationFunction="ValRoyaltorGridRow" ToolTip="Please select valid royaltor from the search list"
                                                            ControlToValidate="txtRoyaltor" ErrorMessage="*" Display="Dynamic" ValidateEmptyText="true"></asp:CustomValidator>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="7%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlOptionPeriod" runat="server" Width="90%" CssClass="ddlStyle" onmouseover="DisplayDdlTextOnHover(this);"
                                                            onchange="OnOptionPeriodChange(this);">
                                                        </asp:DropDownList>
                                                        <asp:TextBox ID="txtOptionPeriod" runat="server" Width="90%" Text="" ReadOnly="true" CssClass="textboxStyle"></asp:TextBox>
                                                        <asp:CustomValidator ID="valddlOptionPeriod" runat="server" ValidationGroup="valSave" CssClass="requiredFieldValidator"
                                                            ClientValidationFunction="ValOptionPeriodGridRow" ToolTip="Please select an option period"
                                                            ControlToValidate="ddlOptionPeriod" ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="9%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtTerritory" runat="server" Width="97%" Text='<%#Bind("seller_group")%>' CssClass="textbox_FuzzySearch"
                                                            ToolTip='<%#Bind("seller_group")%>' onkeydown="OntxtTerritoryKeyDown(this);" onchange="OntxtTerritoryChange(this);"></asp:TextBox>
                                                        <ajaxToolkit:AutoCompleteExtender ID="aceTerritory" runat="server"
                                                            ServiceMethod="FuzzyTrackListingSellerGrpList"
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
                                                <asp:TemplateField ItemStyle-Width="3%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlEscCode" runat="server" Width="96%" CssClass="ddlStyle"
                                                            onchange="OnEscCodeChange(this);">
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="3%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cbIncInEsc" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="3%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cbActive" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="3%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cbExclude" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="6%" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlStatus" runat="server" Width="95%" CssClass="ddlStyle" onchange="return ValidateTrackStatus(this);"
                                                            onmouseover="DisplayDdlTextOnHover(this);">
                                                        </asp:DropDownList>
                                                        <%--JIRA-953 Changes by Ravi on 01/03/2019 --Start--%>
                                                        <asp:Image ID="imgAutoSignoff" runat="server" ImageUrl="../Images/autosignoff.png" ToolTip="Auto Approved" Visible="false" />
                                                        <%--JIRA-953 Changes by Ravi on 01/03/2019 --End--%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="3%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imgBtnCommentWithLine" runat="server" CommandName="Comment" ImageUrl="../Images/Comment_with_lines.png"
                                                            ToolTip='<%#Bind("comments")%>' OnClientClick="return OpenCommentPopup(this,'');" CausesValidation="false" />
                                                        <asp:ImageButton ID="imgBtnCommentWithOutLine" runat="server" CommandName="Comment" ImageUrl="../Images/Comment_without_lines.png"
                                                            ToolTip="Comment" OnClientClick="return OpenCommentPopup(this,'');" CausesValidation="false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" ItemStyle-Width="3%">
                                                    <ItemTemplate>
                                                        <table width="95%" style="float: left; table-layout: fixed" cellpadding="0" cellspacing="0">
                                                            <tr style="float: left">
                                                                <td align="left">
                                                                    <asp:ImageButton ID="imgBtnDelete" runat="server" CommandName="deleteRow" ImageUrl="../Images/Delete.gif"
                                                                        ToolTip="Delete" OnClientClick="return ConfirmDelete(this);" />
                                                                </td>
                                                                <td align="left">
                                                                    <asp:ImageButton ID="imgBtnUndo" runat="server" ImageUrl="../Images/cancel_row3.png"
                                                                        ToolTip="Cancel" OnClientClick="if (!UndoTrackChanges(this)) { return false;};" CausesValidation="false" />
                                                                </td>
                                                                <td align="left">
                                                                    <asp:ImageButton ID="imgBtnAddParticipant" runat="server" CommandName="addParicipant" ImageUrl="../Images/add_row.png"
                                                                        ToolTip="Add Participant" Visible="false" CausesValidation="false"
                                                                        OnClientClick="if (!SetTrackAdded(this)) { return false;};" />
                                                                    <asp:ImageButton ID="imgBtnCopy" runat="server" CommandName="copyParticipant" ImageUrl="../Images/Copy.png"
                                                                        ToolTip="Copy" Visible="false" OnClientClick="if (!ValidateCopyParticipant(this)) { return false;};" CausesValidation="false" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <asp:HiddenField ID="hdnRowNum" runat="server" Value='<%# Bind("rownum") %>' />
                                                        <asp:HiddenField ID="hdnTrackListingId" runat="server" Value='<%# Bind("track_listing_id") %>' />
                                                        <asp:HiddenField ID="hdnDisplayOrder" runat="server" Value='<%# Bind("display_order") %>' />
                                                        <asp:HiddenField ID="hdnSeqNo" runat="server" Value='<%# Bind("seq_no") %>' />
                                                        <asp:HiddenField ID="hdnISRC" runat="server" Value='<%# Bind("isrc") %>' />
                                                        <asp:HiddenField ID="hdnRoyaltor" runat="server" Value='<%# Bind("royaltor") %>' />
                                                        <asp:HiddenField ID="hdnRoyaltorId" runat="server" Value='<%# Bind("royaltor_id") %>' />
                                                        <asp:HiddenField ID="hdnOptPeriodCode" runat="server" Value='<%# Bind("option_period_code") %>' />
                                                        <asp:HiddenField ID="hdnOptPeriodCodeChanged" runat="server" Value='' />
                                                        <asp:HiddenField ID="hdnSellerGrpCode" runat="server" Value='<%# Bind("seller_group_code") %>' />
                                                        <asp:HiddenField ID="hdnSellerGrp" runat="server" Value='<%# Bind("seller_group") %>' />
                                                        <asp:HiddenField ID="hdnEscCode" runat="server" Value='<%# Bind("esc_code") %>' />
                                                        <asp:HiddenField ID="hdnEscCodeChanged" runat="server" Value='' />
                                                        <asp:HiddenField ID="hdnIncInEsc" runat="server" Value='<%# Bind("inc_in_escalation") %>' />
                                                        <asp:HiddenField ID="hdnActive" runat="server" Value='<%# Bind("active") %>' />
                                                        <asp:HiddenField ID="hdnExclude" runat="server" Value='<%# Bind("exclude") %>' />
                                                        <asp:HiddenField ID="hdnStatusCode" runat="server" Value='<%# Bind("status_code") %>' />
                                                        <asp:HiddenField ID="hdnComments" runat="server" Value='<%# Bind("comments") %>' />
                                                        <asp:HiddenField ID="hdnIsModified" runat="server" Value='<%# Bind("is_modified") %>' />
                                                        <asp:HiddenField ID="hdnISRCPartId" runat="server" Value='<%# Bind("isrc_part_id") %>' />
                                                        <asp:HiddenField ID="hdnISRCDealId" runat="server" Value='<%# Bind("isrc_deal_id") %>' />
                                                        <asp:HiddenField ID="hdnParticipationId" runat="server" Value='<%# Bind("participation_id") %>' />
                                                        <asp:HiddenField ID="hdnManualOverride" runat="server" Value='<%# Bind("manual_override") %>' />
                                                        <asp:HiddenField ID="hdnIsTrackEditable" runat="server" Value='<%# Bind("is_track_editable") %>' />
                                                        <%--JIRA-953 Changes by Ravi on 01/03/2019 --Start--%>
                                                        <asp:HiddenField ID="hdnAutoSignoff" runat="server" Value='<%# Bind("auto_signoff") %>' />
                                                        <%--JIRA-953 Changes by Ravi on 01/03/2019 --End--%>
                                                        <%--JIRA-1095 Changes by Ravi on 14/10/2019 -- Start--%>
                                                        <asp:HiddenField ID="hdnIsConsolidated" runat="server" Value='<%# Bind("is_consolidated") %>' />
                                                        <%--JIRA-1095 Changes by Ravi on 14/10/2019 -- End--%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center" style="padding-bottom: 10px">
                        <asp:Repeater ID="rptPager" runat="server">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>'
                                    Enabled='<%# Eval("Enabled") %>' OnClientClick="if (!ValidatePageChange(this,'OnPageChange')) { return false;};"
                                    OnClick="lnkPage_Click" CssClass="gridPager" CausesValidation="false"> </asp:LinkButton>
                            </ItemTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
            </table>

            <asp:UpdateProgress ID="progressBarPageLevel" runat="server" AssociatedUpdatePanelID="updPnlPageLevel" DisplayAfter="100">
                <ProgressTemplate>
                    <div id="Search" style="font-weight: bold; color: Black; z-index: 2">
                        <img src="../../Images/InProgress2.gif" alt="" />
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <ajaxToolkit:ModalPopupExtender ID="mPopupPageLevel" runat="server" PopupControlID="progressBarPageLevel" TargetControlID="progressBarPageLevel"
                BackgroundCssClass="progressBar" RepositionMode="RepositionOnWindowResize">
            </ajaxToolkit:ModalPopupExtender>

            <%--Comments--%>
            <asp:Button ID="dummyCommentPopup" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeCommentPopup" runat="server" PopupControlID="pnlCommentPopup" TargetControlID="dummyCommentPopup"
                CancelControlID="btnCloseCommentPopup" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlCommentPopup" runat="server" align="left" Width="30%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="identifierLable" align="center">Comment
                                    </td>
                                    <td align="right" style="vertical-align: top;" width="10%">
                                        <asp:ImageButton ID="btnCloseCommentPopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" CausesValidation="false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td align="center">
                                        <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Width="80%" Height="100" CssClass="identifierLable" onkeydown="CommentBoxMaxLength();"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rfvtxtComment" ControlToValidate="txtComment" ValidationGroup="valGrpSaveComment"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter comment" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3" align="center">
                                        <table width="40%">
                                            <tr>
                                                <td width="30%">
                                                    <asp:Button ID="btnSaveComment" runat="server" CssClass="ButtonStyle" OnClick="btnSaveComment_Click"
                                                        Text="Save" UseSubmitBehavior="false" Width="90%" ValidationGroup="valGrpSaveComment" />
                                                </td>
                                                <td width="30%">
                                                    <asp:Button ID="btnDeleteComment" runat="server" CssClass="ButtonStyle" OnClick="btnSaveComment_Click"
                                                        Text="Delete" UseSubmitBehavior="false" Width="90%" ValidationGroup="valGrpSaveComment" />
                                                </td>
                                                <td width="30%">
                                                    <asp:Button ID="btnCancelComment" runat="server" CssClass="ButtonStyle" OnClientClick="return CancelComment();"
                                                        Text="Cancel" UseSubmitBehavior="false" Width="90%" CausesValidation="false" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <asp:HiddenField ID="hdnCommentISRCDealId" runat="server" />
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Comments - Ends--%>

            <%--Royaltor fuzzy search - full search - Ends--%>
            <asp:Button ID="dummyFuzzySearch" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeFuzzySearch" runat="server" PopupControlID="pnlFuzzySearch" TargetControlID="dummyFuzzySearch"
                CancelControlID="btnCloseFuzzySearchPopup" BackgroundCssClass="popupBox">
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
                                onchange="OnlbFuzzySearchSelected();"></asp:ListBox>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Royaltor fuzzy search - full search - Ends--%>

            <%--Confirm Consolidate--%>
            <asp:Button ID="dummyConfirmConsolidate" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmConsolidate" runat="server" PopupControlID="pnlPopupConsolidate" TargetControlID="dummyConfirmConsolidate"
                CancelControlID="btnCancelConsolidate" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlPopupConsolidate" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmConsolidate" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblConsolidatePopUpMsg" runat="server" Text="One or more Participants has a manual override, do you want to Continue?"
                                CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnCancelConsolidate" runat="server" Text="Cancel" CssClass="ButtonStyle" CausesValidation="false" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnConfirmConsolidate" runat="server" Text="Continue" CssClass="ButtonStyle" OnClick="btnConsolidate_Click" CausesValidation="false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Confirm Consolidate - Ends--%>

            <%--Cat status change pop up--%>
            <asp:Button ID="dummyCatStatusPopup" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeCatStatusPopup" runat="server" PopupControlID="pnlCatStatusPopup" TargetControlID="dummyCatStatusPopup"
                CancelControlID="btnNoCatStatusPopup" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlCatStatusPopup" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblHdrCatStatusPopup" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMsgCatStatusPopup" runat="server" Text="Do you want to update Status of all Tracks to this Catalogue Status?"
                                CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYesCatStatusPopup" runat="server" Text="Yes" CssClass="ButtonStyle" OnClientClick="UpdateStatusConfirmYes();" CausesValidation="false" />

                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNoCatStatusPopup" runat="server" Text="No" CssClass="ButtonStyle" CausesValidation="false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Cat status change pop up - Ends--%>

            <%--Copy participant popups--%>
            <%--popup 1 to select All/Selected tracks--%>
            <asp:Button ID="dummyCopyParticip1" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeCopyParticip1" runat="server" PopupControlID="pnlCopyParticip1" TargetControlID="dummyCopyParticip1"
                CancelControlID="btnCopyParticip1Cancel" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlCopyParticip1" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="Label5" runat="server" Text="Copy Participant details" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" CssClass="identifierLable"
                                Text="Copy Participant to All or Selected Tracks?"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table width="85%">
                                <tr>
                                    <td width="17%">
                                        <asp:Button ID="btnCopyParticip1All" runat="server" Text="All" CssClass="ButtonStyle"
                                            OnClientClick="if (!ValidateCopyToAllTracks()) { return false;};" OnClick="btnCopyParticip1All_Click" CausesValidation="false" Width="95%"
                                            ToolTip="Copies to tracks on all pages" />
                                    </td>
                                    <td width="5%"></td>
                                    <td width="28%">
                                        <asp:Button ID="btnCopyParticip1CurPage" runat="server" Text="Current Page" CssClass="ButtonStyle"
                                            OnClientClick="if (!ValidateCopyToCurPageTracks()) { return false;};" OnClick="btnCopyParticip1CurPage_Click" CausesValidation="false" Width="95%"
                                            ToolTip="Copies to tracks on current page" />
                                    </td>
                                    <td width="5%"></td>
                                    <td width="20%">
                                        <asp:Button ID="btnCopyParticip1Selected" runat="server" Text="Selected" CssClass="ButtonStyle" OnClick="btnCopyParticip1Selected_Click" CausesValidation="false" Width="95%"
                                            ToolTip="Copies to selected tracks on all pages" />
                                    </td>
                                    <td width="5%"></td>
                                    <td width="20%">
                                        <asp:Button ID="btnCopyParticip1Cancel" runat="server" Text="Cancel" CssClass="ButtonStyle" CausesValidation="false" Width="95%" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <%--popup 2 for user to select the tracks to be copied--%>
            <asp:Button ID="dummyCopyParticip2" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeCopyParticip2" runat="server" PopupControlID="pnlCopyParticip2" TargetControlID="dummyCopyParticip2"
                CancelControlID="btnCopyParticip2Cancel" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlCopyParticip2" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="Label3" runat="server" Text="Copy Participant details" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td align="left">
                                        <table width="95.75%" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td width="20%" class="gridHeaderStyle_1row">Track</td>
                                                <td width="70%" class="gridHeaderStyle_1row">Track Title</td>
                                                <td width="10%" class="gridHeaderStyle_1row">&nbsp</td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Panel ID="plnGridCopyParticipTrackList" runat="server" ScrollBars="Auto">
                                            <asp:GridView ID="gvCopyParticipTrackList" runat="server" AutoGenerateColumns="False" Width="95.75%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                EmptyDataText="No data found" ShowHeader="false" RowStyle-CssClass="dataRow">
                                                <Columns>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" ItemStyle-Width="20%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblTrack" runat="server" Width="98%" Text='<%# Bind("seq_no") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" ItemStyle-Width="70%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblTrackTitle" runat="server" Width="98%" Text='<%# Bind("track_title") %>' CssClass="identifierLable"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" ItemStyle-Width="10%">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="cbCopyTrackSelect" runat="server" />
                                                            <asp:HiddenField ID="hdnTrackListingId" runat="server" Value='<%# Bind("track_listing_id") %>' />
                                                            <asp:HiddenField ID="hdnStatusCode" runat="server" Value='<%# Bind("status_code") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table width="100%" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td width="30%" align="right">
                                        <asp:Button ID="btnCopyParticip2Copy" runat="server" Text="Copy" CssClass="ButtonStyle"
                                            OnClientClick="if (!ValidateCopyToSelectedTracks()) { return false;};" OnClick="btnCopyParticip2Copy_Click" CausesValidation="false" Width="50%" />
                                    </td>
                                    <td width="5%"></td>
                                    <td width="30%" align="left">
                                        <asp:Button ID="btnCopyParticip2Cancel" runat="server" Text="Cancel" CssClass="ButtonStyle" CausesValidation="false" Width="50%" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <%--popup 3 warning if some tracks are signed off--%>
            <asp:Button ID="dummyCopyParticip3" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeCopyParticip3" runat="server" PopupControlID="pnlCopyParticip3" TargetControlID="dummyCopyParticip3"
                CancelControlID="btnNoCopyParticip3PopUp" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlCopyParticip3" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="Label4" runat="server" Text="Copy Participant details" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblmpeCopyParticip3" runat="server" CssClass="identifierLable"
                                Text="Cannot copy to all tracks. Some tracks are signed off. Do you want to copy Participants to all other tracks?"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYesCopyParticip3PopUp" runat="server" Text="Yes" CssClass="ButtonStyle" OnClick="btnYesCopyParticip3PopUp_Click" CausesValidation="false" />
                                        <asp:HiddenField ID="hdnCopyParticip3PopUp" runat="server" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNoCopyParticip3PopUp" runat="server" Text="No" CssClass="ButtonStyle" CausesValidation="false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Copy participant popups --%>

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
                                        <asp:Button ID="btnUnSavedDataExit" runat="server" Text="Exit" CssClass="ButtonStyle" Width="30%" OnClientClick="if (!OnUnSavedDataExit()) { return false;};"
                                            OnClick="btnUnSavedDataExit_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- Start--%>
            <asp:Button ID="dummyConfirmDelete" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmDelete" runat="server" PopupControlID="pnlConfirmDelete" TargetControlID="dummyConfirmDelete"
                CancelControlID="btnNo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlConfirmDelete" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmation" runat="server" Text="Delete Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblText" runat="server"
                                CssClass="identifierLable" Text="Are you sure you want to delete this row?"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYes" runat="server" Text="Yes" CssClass="ButtonStyle" OnClick="btnYes_Click" CausesValidation="false" />
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
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnGridDataDeleted" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridRoyFuzzySearchRowId" runat="server" />
            <asp:HiddenField ID="hdnExpandedTrackId" runat="server" Value="N" />
            <asp:HiddenField ID="hdnExpandCollapseAll" runat="server" Value="Expand" />
            <asp:HiddenField ID="hdnManualOverride" runat="server" Value="0" />
            <asp:HiddenField ID="hdnUserRole" runat="server" />
            <asp:HiddenField ID="hdnISRCPartIdAddRow" runat="server" />
            <asp:HiddenField ID="hdnChangesMadeNotSavedTrackId" runat="server" />
            <asp:HiddenField ID="hdnTxtRoyaltorSelected" runat="server" Value="N" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" />
            <asp:HiddenField ID="hdnFuzzySearchText" runat="server" />
            <asp:HiddenField ID="hdnGridFuzzySearchRowId" runat="server" />
            <asp:HiddenField ID="hdnCopyParticipRowIndex" runat="server" Value="N" />
            <asp:HiddenField ID="hdnCopyParticipTrackListingId" runat="server" />
            <asp:HiddenField ID="hdnPageIndex" runat="server" />
            <asp:HiddenField ID="hdnGridPageSize" runat="server" />
            <asp:HiddenField ID="hdnIsConfirmPopup" runat="server" Value="N" />
            <asp:HiddenField ID="hdnButtonSelection" runat="server" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" onkeydown="FocusLblKeyPress();"></asp:Label>
            <asp:Button ID="btnFuzzyTerritoryListPopup" runat="server" Style="display: none;" OnClick="btnFuzzyTerritoryListPopup_Click" CausesValidation="false" />
            <asp:Button ID="btnFuzzyRoyaltorListPopup" runat="server" Style="display: none;" OnClick="btnFuzzyRoyaltorListPopup_Click" CausesValidation="false" />
            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- Start--%>
            <asp:HiddenField ID="hdnDeletedisplayOrder" runat="server" Value="N" />
            <asp:HiddenField ID="hdnDeleteisModified" runat="server" Value="N" />
            <asp:HiddenField ID="hdnDeleteISRCPartId" runat="server" Value="N" />
            <asp:HiddenField ID="hdnDeleteISRCDealId" runat="server" Value="N" />
            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- End--%>
            <asp:HiddenField ID="hdnBulkStatusUpdate" runat="server" Value="N" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

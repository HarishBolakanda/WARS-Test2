<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkFlow.aspx.cs" Inherits="WARS.WorkFlow" MasterPageFile="~/MasterPage.Master"
    Title="WARS - WorkFlow" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>
<%@ Register TagPrefix="activityScreen" TagName="ActivityScreen" Src="~/UserControls/RoyaltorActivity.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //to open costs screen 
        function OpenCostScreen() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../DataMaintenance/RoyaltorCosts.aspx");
                return false;
            }
            else {
                window.location = "../DataMaintenance/RoyaltorCosts.aspx";
                return true;
            }
        }

        //to open dashboard screen 
        function OpenDashboardScreen() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../StatementProcessing/StmtProgressDashboard.aspx");
                return false;
            }
            else {
                window.location = "../StatementProcessing/StmtProgressDashboard.aspx";
                return true;
            }
        }

    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 1px;">
                        <asp:Button ID="btnOpenDashboard" runat="server" Text="Statement Dashboard" CssClass="LinkButtonStyle"
                            Width="98%" UseSubmitBehavior="false" OnClientClick="if (!OpenDashboardScreen()) { return false;};" />
                    </td>
                </tr>
                <tr>
                    <td colspan="11" align="right" style="padding-right: 0; padding-left: 1px;">
                        <asp:Button ID="btnCostMaint" runat="server" Text="Cost Maintenance" CssClass="LinkButtonStyle"
                            Width="98%" UseSubmitBehavior="false" OnClientClick="if (!OpenCostScreen()) { return false;};" />
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

            //to maintain scroll position
            //postBackElementID = args.get_postBackElement().id.substring(args.get_postBackElement().id.lastIndexOf("_") + 1);
            postBackElementID = args.get_postBackElement().id;
            if (postBackElementID.indexOf('imgExpand') != -1 || postBackElementID.indexOf('imgCollapse') != -1
                || postBackElementID.indexOf('btnCreateFrontSheet') != -1 || postBackElementID.indexOf('cbReCreateStats') != -1
                || postBackElementID.indexOf('btnRbStatusClick') != -1 || postBackElementID.indexOf('rblistStatus') != -1 || postBackElementID.indexOf('rblistStatusHeader') != -1
                || postBackElementID.indexOf('btnYesStatusBulkUpdateConfirm') != -1 || postBackElementID.indexOf('btnNoStatusBulkUpdateConfirm') != -1
                || postBackElementID.indexOf('btnYesStatusUpdateConfirm') != -1 || postBackElementID.indexOf('OKFinalSignOffWarning') != -1
                || postBackElementID.indexOf('btnEditFrontSheet') != -1 || postBackElementID.indexOf('btnClosePopupAct') != -1
                || postBackElementID.indexOf('txtRoyaltor') != -1 || postBackElementID.indexOf('txtOwnSearch') != -1
                || postBackElementID.indexOf('updPnlPageLevel') != -1
                || postBackElementID.indexOf('cancelFinalSignOffWarning') != -1 || postBackElementID.indexOf('btnNoStatusUpdateConfirm') != -1
                || postBackElementID.indexOf('cbRecalFrntSht') != -1 || postBackElementID.indexOf('btnCbRecalFrntShtClick') != -1
                || postBackElementID.indexOf('btnSaveComment') != -1 || postBackElementID.indexOf('btnDeleteComment') != -1
                || postBackElementID.indexOf('imgBtnCommentWithLine') != -1 || postBackElementID.indexOf('imgBtnCommentWithOutLine') != -1
                || postBackElementID.indexOf('btnSaveComment') != -1 || postBackElementID.indexOf('btnDeleteComment') != -1
                || postBackElementID.indexOf('btnCommentUploadFile') != -1 || postBackElementID.indexOf('btnYesCommentDeleteFile') != -1
                ) {
                xPos = sender._scrollPosition.x;
                yPos = sender._scrollPosition.y;

                //hold scroll position on selecting expand/collapse
                var PnlReference = document.getElementById("<%=PnlGrid.ClientID %>");
                scrollTop = PnlReference.scrollTop;
            }
            else if (postBackElementID.indexOf('btnRemove') != -1) {
                //hold scroll position of Activity popup screen
                var PnlActivity = document.getElementById("<%=actScreen.FindControl("PnlActivity").ClientID %>");
                    scrollTop = PnlActivity.scrollTop;
                }

        }

        function EndRequestHandler(sender, args) {
            //Hide the modal popup - the update progress
            var popup = $find('<%= mPopupPageLevel.ClientID %>');
            if (popup != null) {
                popup.hide();
            }

            //to maintain scroll position
            //postBackElementID = sender._postBackSettings.sourceElement.id.substring(sender._postBackSettings.sourceElement.id.lastIndexOf("_") + 1);
            postBackElementID = sender._postBackSettings.sourceElement.id;
            if (postBackElementID.indexOf('imgExpand') != -1 || postBackElementID.indexOf('imgCollapse') != -1
                || postBackElementID.indexOf('btnCreateFrontSheet') != -1 || postBackElementID.indexOf('cbReCreateStats') != -1
                || postBackElementID.indexOf('btnRbStatusClick') != -1 || postBackElementID.indexOf('rblistStatus') != -1 || postBackElementID.indexOf('rblistStatusHeader') != -1
                || postBackElementID.indexOf('btnYesStatusBulkUpdateConfirm') != -1 || postBackElementID.indexOf('btnNoStatusBulkUpdateConfirm') != -1
                || postBackElementID.indexOf('btnYesStatusUpdateConfirm') != -1 || postBackElementID.indexOf('OKFinalSignOffWarning') != -1
                || postBackElementID.indexOf('btnEditFrontSheet') != -1 || postBackElementID.indexOf('btnClosePopupAct') != -1
                || postBackElementID.indexOf('txtRoyaltor') != -1 || postBackElementID.indexOf('txtOwnSearch') != -1
                || postBackElementID.indexOf('updPnlPageLevel') != -1
                || postBackElementID.indexOf('cancelFinalSignOffWarning') != -1 || postBackElementID.indexOf('btnNoStatusUpdateConfirm') != -1
                || postBackElementID.indexOf('cbRecalFrntSht') != -1 || postBackElementID.indexOf('btnCbRecalFrntShtClick') != -1
                || postBackElementID.indexOf('btnSaveComment') != -1 || postBackElementID.indexOf('btnDeleteComment') != -1
                || postBackElementID.indexOf('imgBtnCommentWithLine') != -1 || postBackElementID.indexOf('imgBtnCommentWithOutLine') != -1
                || postBackElementID.indexOf('btnSaveComment') != -1 || postBackElementID.indexOf('btnDeleteComment') != -1
                || postBackElementID.indexOf('btnCommentUploadFile') != -1 || postBackElementID.indexOf('btnYesCommentDeleteFile') != -1
                ) {
                window.scrollTo(xPos, yPos);

                //set scroll position on selecting expand/collapse
                var PnlReference = document.getElementById("<%=PnlGrid.ClientID %>");
                PnlReference.scrollTop = scrollTop;
            }
            else if (postBackElementID.indexOf('btnRemove') != -1) {
                //set scroll position of Activity popup screen
                var PnlActivity = document.getElementById("<%=actScreen.FindControl("PnlActivity").ClientID %>");
                    PnlActivity.scrollTop = scrollTop;
                }


        }

        //probress bar and scroll position functionality - starts

        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.50;
            document.getElementById("<%=PnlGrid.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        //grid panel height adjustment functioanlity - ends

        //used to check if any changes to allow navigation to other screen 
        function IsDataChanged() {
            var hdnIsConfirmPopup = document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value;
            var hdnCommentUpload = document.getElementById('<%=hdnCommentUpload.ClientID%>').value;
            var hdnCommentDownloadFile = document.getElementById('<%=hdnCommentDownloadFile.ClientID%>').value;
            if ((IsCommentChanged() && hdnIsConfirmPopup != "Y" && hdnCommentUpload != "Y" && hdnCommentDownloadFile != "Y")
                    || (IsRerunStmtGridHdrSelected() || IsRerunStmtSelected() || IsRecalStmtGridHdrSelected() || IsRecalStmtSelected())) {
                return true;
            }
            else {
                return false;
            }
        }

        //unsaved data warning - start
        function WarnOnUnSavedData() {
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            var hdnAllowGridPageChange = document.getElementById("<%=hdnAllowGridPageChange.ClientID %>").value;
            if (isExceptionRaised != "Y") {
                if (hdnAllowGridPageChange == "Y" && IsDataChanged()) {
                    return warningMsgOnUnSavedData;
                }
            }

        }
        window.onbeforeunload = WarnOnUnSavedData;

        function IsCommentChanged() {
            if (document.getElementById("<%=hdnIsCommentChanged.ClientID %>").value == "Y") {
                return true;
            }
            else {
                return false;
            }

        }



        //Validate any unsaved data on browser window close/refresh
        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        //unsaved data warning - end

        //Royaltor auto populate search functionalities        
        var txtRoySrch;

        function royaltorListPopulating() {
            txtRoySrch = document.getElementById("<%= txtRoyaltor.ClientID %>");
            txtRoySrch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoySrch.style.backgroundRepeat = 'no-repeat';
            txtRoySrch.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidRoyaltor.ClientID %>").value = "N";
        }

        function royaltorListPopulated() {
            txtRoySrch = document.getElementById("<%= txtRoyaltor.ClientID %>");
            txtRoySrch.style.backgroundImage = 'none';
        }

        function resetScrollPosition(sender, args) {
            var autoCompPnl = document.getElementById("<%= autocompleteDropDownPanel1.ClientID %>");
            autoCompPnl.scrollTop = 1;

        }

        function royaltorListItemSelected(sender, args) {
            var roySrchVal = args.get_value();
            if (roySrchVal == 'No results found') {
                document.getElementById("<%= txtRoyaltor.ClientID %>").value = "";
            }
            else if (roySrchVal != '') {
                document.getElementById("<%= txtOwnSearch.ClientID %>").value = "";
                document.getElementById("<%= hdnIsValidRoyaltor.ClientID %>").value = "Y";
            }
    }


    //================================End

    //Owner auto populate search functionalities        
    var txtOwnSrch;

    function ownerListPopulating() {
        txtOwnSrch = document.getElementById("<%= txtOwnSearch.ClientID %>");
        txtOwnSrch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
        txtOwnSrch.style.backgroundRepeat = 'no-repeat';
        txtOwnSrch.style.backgroundPosition = 'right';
        document.getElementById("<%= hdnIsValidOwner.ClientID %>").value = "N";
    }

    function ownerListPopulated() {
        txtOwnSrch = document.getElementById("<%= txtOwnSearch.ClientID %>");
        txtOwnSrch.style.backgroundImage = 'none';
    }

    function ownerScrollPosition(sender, args) {
        var autoCompPnl = document.getElementById("<%= acePanelOwner.ClientID %>");
        autoCompPnl.scrollTop = 1;

    }

    function ownerListItemSelected(sender, args) {
        var ownSrchVal = args.get_value();
        if (ownSrchVal == 'No results found') {
            document.getElementById("<%= txtOwnSearch.ClientID %>").value = "";
        }
        else if (ownSrchVal != '') {
            document.getElementById("<%= txtRoyaltor.ClientID %>").value = "";
            document.getElementById("<%= hdnIsValidOwner.ClientID %>").value = "Y";
        }
}

//================================End

//Tab key to remain only on screen fields
function OnTabPress() {
    if (event.keyCode == 9) {
        document.getElementById("<%= lblTab.ClientID %>").focus();
    }
}//=============== End

//WUIN-290 change begins
//WUIN-1057 - validations: 
//      warning message on selection of rerun/recalculate statements and not processed.
var gridClientId = "ContentPlaceHolderBody_gvRoyActivity_";
function OnStatusChange(gridRow) {
    document.getElementById("<%=hdnIgnoreStatusClick.ClientID %>").value = "N";//set to default value
    selectedRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
    rblistStatus = document.getElementById(gridClientId + 'rblistStatus' + '_' + selectedRowIndex);
    hdnStatus = document.getElementById(gridClientId + 'hdnStatus' + '_' + selectedRowIndex).value;
    hdnReCreateStats = document.getElementById(gridClientId + 'hdnReCreateStats' + '_' + selectedRowIndex).value;
    hdnOwnerLevelStatus = document.getElementById(gridClientId + 'hdnOwnerLevelStatus' + '_' + selectedRowIndex).value;
    hdnOwnerLevelReCreateStatus = document.getElementById(gridClientId + 'hdnOwnerLevelReCreateStatus' + '_' + selectedRowIndex).value;

    //warning message on selection of rerun/recalculate statements and not processed.
    if (IsRerunStmtGridHdrSelected() || IsRerunStmtSelected() || IsRecalStmtGridHdrSelected() || IsRecalStmtSelected()) {
        //reset the status to its initial value
        var rbListItems = rblistStatus.getElementsByTagName('input');
        var rbListItem;
        for (var i = 0; i < rbListItems.length; i++) {
            rbListItem = rbListItems[i];
            if (hdnStatus != "" && hdnStatus == rbListItem.value) {
                //royaltor level
                rbListItem.checked = true;
                break;
            }
            else if (hdnOwnerLevelStatus != "" && hdnOwnerLevelStatus == rbListItem.value) {
                //owner level
                rbListItem.checked = true;
                break;
            }
        }

        DisplayMessagePopup("Selected rerun/recalculate statements have not been processed. Please unselect to proceed.");
        return false;
    }

    //set this to differentiate between status update bulk and others
    document.getElementById("<%=hdnStatusUpdateGridHdr.ClientID %>").value = "N";

    //Holding previous status value. used in warning popup cancel
    document.getElementById("<%=hdnFinalSignOffWarningRowIndex.ClientID %>").innerText = selectedRowIndex;
    document.getElementById("<%=hdnFinalSignOffWarningReCreateSt.ClientID %>").innerText = hdnReCreateStats;

    if (hdnStatus == "") {
        //owner level
        document.getElementById("<%=hdnFinalSignOffWarningPrevStatus.ClientID %>").innerText = hdnOwnerLevelStatus;
    }
    else {
        //royaltor level
        document.getElementById("<%=hdnFinalSignOffWarningPrevStatus.ClientID %>").innerText = hdnStatus;
    }

    //check which status list item is selected
    var rbListItems = rblistStatus.getElementsByTagName('input');
    var rbListItem;
    for (var i = 0; i < rbListItems.length; i++) {
        if (rbListItems[i].checked) {
            rbListItem = rbListItems[i];
            break;
        }
    }

    //warning popup only when status is changed and final sign off is selected   
    if ((hdnStatus == "" && hdnOwnerLevelStatus == rbListItem.value) || (hdnReCreateStats != "A" && hdnStatus == rbListItem.value)) {
        return;
    }

    //display warning popup when final sign off is selected.
    //no warning is required when final sign off is selected from Archive requested
    if (rbListItem.value == "3" &&
        (
        (hdnStatus != "" && !(hdnReCreateStats = "A" && hdnStatus == "3")) //royaltor level
        ||
        (hdnStatus == "" && hdnOwnerLevelStatus != "4"
        //!(hdnOwnerLevelReCreateStatus == "A" && hdnOwnerLevelStatus == "4")
        )//owner level        
        )
        ) {
        var popup = $find('<%= mpeFinalSignOffWarning.ClientID %>');
        if (popup != null) {
            popup.show();
        }
    }
    else {
        CallStatusUpdateEvent();
    }

    return true;

}

function CallStatusUpdateEvent() {
    document.getElementById('<%=btnRbStatusClick.ClientID%>').click();
}

function CancelFinalSignOffWarning() {
    //if final sign of warning is cancelled from status bulk update
    hdnStatusUpdateGridHdr = document.getElementById("<%=hdnStatusUpdateGridHdr.ClientID %>");
    if (hdnStatusUpdateGridHdr.value == "Y") {
        //clear radio button selection
        rblistStatusHeader = document.getElementById("<%=rblistStatusHeader.ClientID %>");
        var rbHdrListItems = rblistStatusHeader.getElementsByTagName('input');
        var rbHdrListItem;
        for (var i = 0; i < rbHdrListItems.length; i++) {
            rbHdrListItems[i].checked = false;
        }
        return;
    }

    selectedRowIndex = document.getElementById("<%=hdnFinalSignOffWarningRowIndex.ClientID %>").value;
    hdnFinalSignOffWarningPrevStatus = document.getElementById("<%=hdnFinalSignOffWarningPrevStatus.ClientID %>").value;
    hdnFinalSignOffWarningReCreateSt = document.getElementById("<%=hdnFinalSignOffWarningReCreateSt.ClientID %>").value;
    rblistStatus = document.getElementById(gridClientId + 'rblistStatus' + '_' + selectedRowIndex);

    var rbListItems = rblistStatus.getElementsByTagName('input');
    var rbListItem;
    for (var i = 0; i < rbListItems.length; i++) {
        if (hdnFinalSignOffWarningPrevStatus == "") {
            rbListItems[i].checked = false;
        }
        else if (hdnFinalSignOffWarningPrevStatus == "1" && i == 0) {
            rbListItems[i].checked = true;
            break;
        }
        else if (hdnFinalSignOffWarningPrevStatus == "2" && i == 1) {
            rbListItems[i].checked = true;
            break;
        }
        else if (hdnFinalSignOffWarningPrevStatus == "8" && i == 2) {
            rbListItems[i].checked = true;
            break;
        }
        else if (hdnFinalSignOffWarningPrevStatus == "3" && i == 3) {
            rbListItems[i].checked = true;
            break;
        }
        else if (hdnFinalSignOffWarningPrevStatus == "4" && i == 4) {
            rbListItems[i].checked = true;
            break;
        }

    }

    //calling this to avoid status change event proceeds after selecting 'No' from pop up
    document.getElementById("<%=hdnIgnoreStatusClick.ClientID %>").value = "Y";
    CallStatusUpdateEvent();

}
//===================WUIN-290 change ends

//Status - bulk update start

//WUIN-1057 - validations: 
//      warning message on selection of rerun/recalculate statements and not processed.
function OnStatusChangeHeader() {
    //check if grid has any rows. Display pop up only if it has rows
    var gvRoyActivity = document.getElementById("<%= gvRoyActivity.ClientID %>");
    if (gvRoyActivity != null) {
        var gvRows = gvRoyActivity.rows;
        if (gvRows.length == 0 || gvRows.length == 1) {
            lblRoyaltor = document.getElementById(gridClientId + 'lblRoyaltor' + '_' + 1);
            if (lblRoyaltor == null) {
                //clear header row status radiobuttons
                //clear bulk status update radio buttons
                rblistStatusHeader = document.getElementById("<%=rblistStatusHeader.ClientID %>");
                var rbHdrListItems = rblistStatusHeader.getElementsByTagName('input');
                var rbHdrListItem;
                for (var i = 0; i < rbHdrListItems.length; i++) {
                    rbHdrListItems[i].checked = false;
                }
                return false;
            }
        }
    }

    //warning message on selection of rerun/recalculate statements and not processed.
    if (IsRerunStmtGridHdrSelected() || IsRerunStmtSelected() || IsRecalStmtGridHdrSelected() || IsRecalStmtSelected()) {
        DisplayMessagePopup("Selected rerun/recalculate statements have not been processed. Please unselect to proceed.");
        return false;
    }

    var popup = $find('<%= mpeBulkUpdateConfirm.ClientID %>');
    if (popup != null) {
        popup.show();
    }

    return true;
}

//WUIN-1089 - changes - Start
//on click of 'Yes' button of bulk status update confirmation pop up
function OnBulkUpdateConfirmYes() {
    rblistStatusHeader = document.getElementById("<%=rblistStatusHeader.ClientID %>");
            //check which status list item is selected
            var rbHdrListItems = rblistStatusHeader.getElementsByTagName('input');
            var rbHdrListItem;
            for (var i = 0; i < rbHdrListItems.length; i++) {
                if (rbHdrListItems[i].checked) {
                    rbHdrListItem = rbHdrListItems[i];
                    break;
                }
            }
            //display warning popup when final sign off is selected.            
            if (rbHdrListItem.value == "3") {
                var bulkUpdatePopup = $find('<%= mpeBulkUpdateConfirm.ClientID %>');
        var finalSignOffPopup = $find('<%= mpeFinalSignOffWarning.ClientID %>');
        if (bulkUpdatePopup != null) {
            bulkUpdatePopup.hide();
        }

        if (finalSignOffPopup != null) {
            document.getElementById("<%=hdnStatusUpdateGridHdr.ClientID %>").value = "Y";
            finalSignOffPopup.show();
        }
        return false;
    }
    else {
        CallStatusUpdateEvent();
    }

}

//on click of 'No' button of bulk status update confirmation pop up
function OnBulkUpdateConfirmNo() {
    //clear bulk status update radio buttons
    rblistStatusHeader = document.getElementById("<%=rblistStatusHeader.ClientID %>");
    var rbHdrListItems = rblistStatusHeader.getElementsByTagName('input');
    var rbHdrListItem;
    for (var i = 0; i < rbHdrListItems.length; i++) {
        rbHdrListItems[i].checked = false;
    }
    return false;
}

//WUIN-1089 - changes - End

//===================Status - bulk update End




//trigger search on Enter key of Producer filter field
function OnTxtProducerKeyDown() {
    if ((event.keyCode == 13)) {
        document.getElementById('<%=btnGo.ClientID%>').click();
    }
}
//Reset Earnings compare ddl when Earning field is empty
function OnTxtEarningsBlur() {
    txtEarnings = document.getElementById("<%=txtEarnings.ClientID %>").value;
    ddlEarningsCompare = document.getElementById("<%=ddlEarningsCompare.ClientID %>");
    if (txtEarnings == "") {
        ddlEarningsCompare.selectedIndex = 0;
    }
}

//Reset Earnings compare ddl when Earning field is empty
function OnTxtClosingBalanceBlur() {
    txtClosingBalance = document.getElementById("<%=txtClosingBalance.ClientID %>").value;
    ddlClosingBalCompare = document.getElementById("<%=ddlClosingBalCompare.ClientID %>");
    if (txtClosingBalance == "") {
        ddlClosingBalCompare.selectedIndex = 0;
    }
}

//Comment popup========== Start
function CancelComment(selectedButton) {
    //validate if comment changed and not saved
    if (IsCommentChanged()) {
        OpenOnUnSavedData(selectedButton);
        return false;
    }

    CommentPopup = $find('<%= mpeCommentPopup.ClientID %>');
    if (CommentPopup != null) {
        CommentPopup.hide();
    }

    return false;
}

function SaveComment() {
    var iframe = document.getElementById('iframe');
    var innerDoc = iframe.contentDocument || iframe.contentWindow.document;
    var txtWorkFlowComment = innerDoc.getElementById('txtWorkFlowComment');

    if (txtWorkFlowComment.value == "") {
        document.getElementById("<%= lblCommentError.ClientID %>").className = 'ErrorLabelVisible';
        return false;
    }

    document.getElementById("<%= txtHidCommentData.ClientID %>").value = txtWorkFlowComment.value;

    return true;
}

function OnBtnCommentUploadFileClick(selectedButton) {
    hdnCommentFileUploadDuplicateCheck = document.getElementById('<%=hdnCommentFileUploadDuplicateCheck.ClientID %>').value;
    hdnIsConfirmPopup = document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value;

    if (document.getElementById("<%= uploadCommentAttachment.ClientID %>").value == "") {
        document.getElementById('<%=lblCommentFileUploadError.ClientID %>').className = 'ErrorLabelVisible';
        return false;
    }

    //validate if comment changed and not saved
    if (hdnIsConfirmPopup == "N" && hdnCommentFileUploadDuplicateCheck == "Y" && IsCommentChanged()) {
        OpenOnUnSavedData(selectedButton);
        return false;
    }
    document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "N";//set to default

    //check if the file being uploaded already exist

    if (hdnCommentFileUploadDuplicateCheck == "Y") {
        var fileUpload = document.getElementById("<%= uploadCommentAttachment.ClientID %>").value;
        var fileName = fileUpload.split("\\").pop();//split the file full path and get last item of the split                        
        gvCommentDownloadFile = document.getElementById("<%= gvCommentDownloadFile.ClientID %>");
            if (gvCommentDownloadFile != null) {
                var gvRows = gvCommentDownloadFile.rows;
                for (var i = 0; i < gvRows.length; i++) {
                    if (document.getElementById('ContentPlaceHolderBody_gvCommentDownloadFile_lblFileName' + '_' + i) == null) {
                        continue;
                    }

                    lblFileName = document.getElementById('ContentPlaceHolderBody_gvCommentDownloadFile_lblFileName' + '_' + i).value;

                    if (fileName == lblFileName) {
                        var popup = $find('<%= mpeCommentUploadFileConfirm.ClientID %>');
                        if (popup != null) {
                            popup.show();
                        }
                        return false;

                    }

                }
            }
        }
        else {
            var popup = $find('<%= mpeCommentUploadFileConfirm.ClientID %>');
        if (popup != null) {
            popup.hide();
        }
    }

    document.getElementById('<%=hdnCommentFileUploadDuplicateCheck.ClientID %>').innerText = "Y";
    document.getElementById('<%=hdnCommentUpload.ClientID %>').innerText = "Y";

    //upload functionality is handled in btnCommentUploadFileHidden
    document.getElementById('<%=btnCommentUploadFileHidden.ClientID %>').click();

    return true;

}

function CallUploadFileEvent() {
    document.getElementById('<%=hdnCommentFileUploadDuplicateCheck.ClientID %>').innerText = "N";
    document.getElementById('<%=btnCommentUploadFile.ClientID %>').click();
}

function OnBtnUndoCommentUploadFile() {
    var uploadCommentAttachment = document.getElementById('<%=uploadCommentAttachment.ClientID %>');
    uploadCommentAttachment.select();
    textRange = uploadCommentAttachment.createTextRange();
    textRange.execCommand('delete');
    uploadCommentAttachment.focus();

    document.getElementById('<%=lblCommentFileUploadError.ClientID %>').className = 'ErrorLabelHidden';

    return false;
}

var gvCommentDownloadFileId = "ContentPlaceHolderBody_gvCommentDownloadFile_";
function GridCommentDownloadFileClick(gridRow) {
    document.getElementById('<%=hdnCommentDownloadFile.ClientID%>').value = "Y";
        var selectedRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
        lblFileName = document.getElementById(gvCommentDownloadFileId + 'lblFileName' + '_' + selectedRowIndex).value;
        document.getElementById("<%=hdnCommentSelectedFileName.ClientID %>").innerText = lblFileName;
        document.getElementById('<%=btnCommentDownloadFile.ClientID%>').click();
        document.getElementById('<%=hdnCommentDownloadFile.ClientID%>').value = "";//reset to default
        return false;
    }

    function ConfirmCommentFileDelete(gridRow) {
        var selectedRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
        lblFileName = document.getElementById(gvCommentDownloadFileId + 'lblFileName' + '_' + selectedRowIndex).value;
        document.getElementById("<%=hdnCommentSelectedFileName.ClientID %>").innerText = lblFileName;

        var popupCommentDeleteFile = $find('<%= mpeCommentDeleteFile.ClientID %>');
        if (popupCommentDeleteFile != null) {
            popupCommentDeleteFile.show();
        }

        return false;

    }

    function OnYesCommentDeleteFileClick(selectedButton) {
        hdnIsConfirmPopup = document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value;

    //validate if comment changed and not saved
    if (hdnIsConfirmPopup == "N" && IsCommentChanged()) {
        var popupCommentDeleteFile = $find('<%= mpeCommentDeleteFile.ClientID %>');
        if (popupCommentDeleteFile != null) {
            popupCommentDeleteFile.hide();
        }

        OpenOnUnSavedData(selectedButton);
        return false;
    }
    document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "N";//set default

    return true;
}

//to set comment related control height after file upload
function SetHeightAfterUpload() {
    hdnGridPnlHeight = document.getElementById("<%=hdnGridPnlHeight.ClientID %>").value;

    if (document.getElementById("<%=pnlCommentPopup.ClientID %>") != null && hdnGridPnlHeight != "") {
        document.getElementById("<%=pnlCommentPopup.ClientID %>").style.height = (hdnGridPnlHeight) + "px";
        document.getElementById("<%=iFrameComment.ClientID %>").style.height = (hdnGridPnlHeight * 0.5) + "px";
        document.getElementById("<%=plnGridCommentDownloadFile.ClientID %>").style.height = (hdnGridPnlHeight * 0.2 - 5) + "px";
    }
}

function SetCommentChangedFlag() {
    document.getElementById("<%=hdnIsCommentChanged.ClientID %>").value = "Y";
}

function OnCloseCommentPopup(selectedButton) {
    if (IsCommentChanged()) {
        OpenOnUnSavedData(selectedButton);
    }
    else {
        //close popup
        CommentPopup = $find('<%= mpeCommentPopup.ClientID %>');
                if (CommentPopup != null) {
                    CommentPopup.hide();
                }

            }

            return false;
        }
        //Comment popup========== End


        //WUIN-1057-validation
        //check if rerun/recalculate statements are selected and not processed and warning message if not processed.
        function OnGoBtnClick() {
            if (IsRerunStmtGridHdrSelected() || IsRerunStmtSelected() || IsRecalStmtGridHdrSelected() || IsRecalStmtSelected()) {
                //as DisplayMessagePopup() doesn't work here, need to implement a pop up in the screen for this .
                var popup = $find('<%= mpeValSearchMsgPopup.ClientID %>');
                if (popup != null) {
                    document.getElementById("<%=lblValSearchMsgPopup.ClientID %>").innerText = "Selected rerun/recalculate statements have not been processed. Please unselect to proceed.";
                    popup.show();
                }
                return false;
            }
            else {
                return true;
            }
        }

        //WUIN-1057- changes
        // implemented to close the explicit message pop up on enter key for validation message on search
        function CloseValSearchMsgPopup() {
            //close pop up on Enter key
            if (event.keyCode == 13) {
                debugger;
                document.getElementById("<%= btnCloseValSearchMsgPopup.ClientID %>").click();
            }
        }

        //validation on page change:
        //1.don't allow page change when 'rerun/recalculate statements option' is selected only at grid row level.
        //2.allow page change when 'rerun/recalculate statements option' is selected at grid header level.
        function OnPageChangeClick() {
            if (IsRerunStmtGridHdrSelected() || IsRecalStmtGridHdrSelected()) {
                //if ((!IsRerunStmtGridHdrSelected() && !IsRecalStmtGridHdrSelected()) && (IsRerunStmtSelected() || IsRecalStmtSelected())) {
                //set flag to N  so that paging is allowed when grid headers are selected
                document.getElementById("<%=hdnAllowGridPageChange.ClientID %>").innerText = "N";
            }
            else if (IsRerunStmtSelected() || IsRecalStmtSelected()) {
                document.getElementById("<%=hdnAllowGridPageChange.ClientID %>").innerText = "Y";//set to default
        //warning on page change
        DisplayMessagePopup("Selected rerun/recalculate statements have not been processed. Please unselect to proceed.");
        return false;
    }

    return true;
}

//check if Rerun statements at grid header level are selected
function IsRerunStmtGridHdrSelected() {
    cbRerunStmtHeader = document.getElementById("<%=cbRerunStmtHeader.ClientID %>");
    if (cbRerunStmtHeader.checked) {
        return true;
    }
    else {
        return false;
    }
}

//check if Recalculate statements at grid header level are selected
function IsRecalStmtGridHdrSelected() {
    cbRecalSummaryHeader = document.getElementById("<%=cbRecalSummaryHeader.ClientID %>");
    if (cbRecalSummaryHeader.checked) {
        return true;
    }
    else {
        return false;
    }
}

//check if Rerun statements at grid row level are selected
function IsRerunStmtSelected() {
    var gvRoyActivity = document.getElementById("<%= gvRoyActivity.ClientID %>");

    if (gvRoyActivity != null) {
        var gvRows = gvRoyActivity.rows;
        for (var i = 0; i < gvRows.length; i++) {
            //to handle empty grid row
            if (document.getElementById(str + 'cbReCreateStats' + '_' + i) == null) {
                continue;
            }

            cbReCreateStats = document.getElementById(str + 'cbReCreateStats' + '_' + i);
            if (!cbReCreateStats.disabled && cbReCreateStats.checked) {
                return true;
            }
        }
    }

    return false;
}

//check if Recalculate statements at grid row level are selected
function IsRecalStmtSelected() {
    var gvRoyActivity = document.getElementById("<%= gvRoyActivity.ClientID %>");

    if (gvRoyActivity != null) {
        var gvRows = gvRoyActivity.rows;
        for (var i = 0; i < gvRows.length; i++) {
            //to handle empty grid row
            if (document.getElementById(str + 'cbRecalFrntSht' + '_' + i) == null) {
                continue;
            }

            cbRecalFrntSht = document.getElementById(str + 'cbRecalFrntSht' + '_' + i);
            if (cbRecalFrntSht.checked) {
                return true;
            }
        }
    }

    return false;
}

// confirmation on un saved data
function OpenOnUnSavedData(selectedButton) {
    var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
    if (warnPopup != null) {
        //hold selected button to perform the selected action
        document.getElementById("<%=hdnButtonSelection.ClientID %>").value = selectedButton;

        //pop up 
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

    var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
    if (warnPopup != null) {
        warnPopup.hide();
    }

    hdnButtonSelection = document.getElementById("<%=hdnButtonSelection.ClientID %>").value;
    if (hdnButtonSelection == "btnCloseCommentPopup" || hdnButtonSelection == "btnCancelComment") {
        var commentPopup = $find('<%= mpeCommentPopup.ClientID %>');
                if (commentPopup != null) {
                    commentPopup.hide();
                }
            }
            else if (hdnButtonSelection == "btnCommentUploadFile") {
                document.getElementById('<%=btnCommentUploadFile.ClientID %>').click();
            }
            else if (hdnButtonSelection == "btnYesCommentDeleteFile") {
                document.getElementById('<%=btnYesCommentDeleteFileHidden.ClientID %>').click();
    }

    return false;

}

// Enter Key Functionality for all search filters.
function SearchByEnterKey() {
    if ((event.keyCode == 13)) {
        document.getElementById('<%=btnGo.ClientID%>').click();
    }
}

function OntxtRoyaltorKeyDown() {
    var txtRoyaltor = document.getElementById("<%= txtRoyaltor.ClientID %>").value;
    if ((event.keyCode == 13)) {
        if (txtRoyaltor == "") {
            document.getElementById('<%=btnGo.ClientID%>').click();
        }
        else {
            if (document.getElementById("<%= hdnIsValidRoyaltor.ClientID %>").value == "Y") {
                document.getElementById('<%=btnGo.ClientID%>').click();
            }
            else {
                return false;
            }
        }
    }
}


function OntxtOwnerKeyDown() {
    var txtOwner = document.getElementById("<%= txtOwnSearch.ClientID %>").value;
    if ((event.keyCode == 13)) {
        if (txtOwner == "") {
            document.getElementById('<%=btnGo.ClientID%>').click();
        }
        else {
            if (document.getElementById("<%= hdnIsValidOwner.ClientID %>").value == "Y") {
                document.getElementById('<%=btnGo.ClientID%>').click();
            }
            else {
                return false;
            }
        }
    }
}

//============== End

//WUIN-1057 changes - start
var str = "ContentPlaceHolderBody_gvRoyActivity_";//global declaration

//WUIN-1057 - validations: 
//      1.No row level change is allowed on bulk selection of Recalculate Stmt Summary/Rerun statement
function OnRerunStmtClick() {
    cbRerunStmtHeader = document.getElementById("<%=cbRerunStmtHeader.ClientID %>");
    cbRecalSummaryHeader = document.getElementById("<%=cbRecalSummaryHeader.ClientID %>");
    if (cbRerunStmtHeader.checked) {
        DisplayMessagePopup("No row level change is allowed on bulk selection.");
        return false;
    }
    else if (cbRecalSummaryHeader.checked) {
        DisplayMessagePopup("Selected recalculate statements have not been processed. Please unselect to proceed.");
        return false;
    }

}

// WUIN-920 - this has been added to retain grid scroll position on recalculate front sheet checkbox without autopostback true
//WUIN-1057 - validations: 
//      1.No row level change is allowed on bulk selection of Recalculate Stmt Summary/Rerun statement
function OnRecalFrntShtClick() {
    cbRerunStmtHeader = document.getElementById("<%=cbRerunStmtHeader.ClientID %>");
    cbRecalSummaryHeader = document.getElementById("<%=cbRecalSummaryHeader.ClientID %>");
    if (cbRecalSummaryHeader.checked) {
        DisplayMessagePopup("No row level change is allowed on bulk selection.");
        return false;
    }
    else if (cbRerunStmtHeader.checked) {
        DisplayMessagePopup("Selected rerun statements have not been processed. Please unselect to proceed.");
        return false;
    }
    else {
        document.getElementById('<%=btnCbRecalFrntShtClick.ClientID%>').click();
    }
}

//WUIN-1057 - validations: 
//1.No comments change is allowed on selection of Recalculate Stmt Summary/Rerun statement
function OnCommentsClick() {
    if (IsRerunStmtGridHdrSelected() || IsRerunStmtSelected() || IsRecalStmtGridHdrSelected() || IsRecalStmtSelected()) {
        DisplayMessagePopup("Selected rerun/recalculate statements have not been processed. Please unselect to proceed.");
        return false;
    }
    else {
        return true;
    }
}

//WUIN-1057 - validations: 
//1.No Update Stmt Activity Click is allowed on selection of Recalculate Stmt Summary(header/row level)
function OnUpdateStmtActivityClick() {
    if (IsRecalStmtGridHdrSelected() || IsRecalStmtSelected()) {
        DisplayMessagePopup("Selected recalculate statements have not been processed. Please unselect to proceed.");
        return false;
    }
    else {
        return true;
    }
}

//WUIN-1057 - validations: 
//1.No Recalculate statements Click is allowed on selection of Rerun Stmt(header/row level)
function OnRecalStmtClick() {
    if (IsRerunStmtGridHdrSelected() || IsRerunStmtSelected()) {
        DisplayMessagePopup("Selected rerun statements have not been processed. Please unselect to proceed.");
        return false;
    }
    else {
        return true;
    }
}


//Validations - On selecting of Rerun Stmt header checkbox
//1.don't select Rerun Stmt header checkbox when there are no rows in grid
//2.warning on selecting Rerun Stmt header checkbox when Recal Summary stmt (header/row level) checkbox is checked
function OnRerunStmtHeaderClick() {
    var gvRoyActivity = document.getElementById("<%= gvRoyActivity.ClientID %>");
    cbRerunStmtHeader = document.getElementById("<%=cbRerunStmtHeader.ClientID %>");
    document.getElementById("<%=hdnAllowGridPageChange.ClientID %>").innerText = "Y";//reset to default
    if (gvRoyActivity != null) {
        var gvRows = gvRoyActivity.rows;
        //1.don't select Rerun Stmt header checkbox when there are no rows in grid
        //if grid has no rows
        if (gvRows.length == 0 || gvRows.length == 1) {
            lblRoyaltor = document.getElementById(gridClientId + 'lblRoyaltor' + '_' + 1);
            if (lblRoyaltor == null) {
                //clear header row checkbox's                                        
                cbRerunStmtHeader.checked = false;
                return false;
            }
        }

        //2.warning on selecting Rerun Stmt header checkbox when Recal Summary stmt (header level) checkbox is checked
        if (IsRecalStmtGridHdrSelected()) {
            DisplayMessagePopup("Selected recalculate statements have not been processed. Please unselect to proceed.");
            return false;
        }

        //2.warning on selecting Rerun Stmt header checkbox when Recal Summary stmt (row level) checkbox is checked
        if (IsRecalStmtSelected()) {
            DisplayMessagePopup("Selected rerun statements have not been processed. Please unselect to proceed.");
            return false;
        }
    }

    return true;
}

//Validations - On selecting of Recal Summay stmt header checkbox
//1.don't select Recal Summary Stmt header checkbox when there are no rows in grid
//2.warning on selecting Rerun Stmt header checkbox when Recal Summary stmt (header/row level) checkbox is checked
function OnRecalSummHeaderClick() {
    var gvRoyActivity = document.getElementById("<%= gvRoyActivity.ClientID %>");
    document.getElementById("<%=hdnAllowGridPageChange.ClientID %>").innerText = "Y";//reset to default
    cbRecalSummaryHeader = document.getElementById("<%=cbRecalSummaryHeader.ClientID %>");
    if (gvRoyActivity != null) {
        var gvRows = gvRoyActivity.rows;
        //1.don't select Recal Summary Stmt header checkbox when there are no rows in grid
        if (gvRows.length == 0 || gvRows.length == 1) {
            lblRoyaltor = document.getElementById(gridClientId + 'lblRoyaltor' + '_' + 1);
            if (lblRoyaltor == null) {
                //clear header row checkbox                        
                cbRecalSummaryHeader.checked = false;
                return false;
            }
        }

        //2.warning on selecting Recal Summary Stmt header checkbox when Rerun Stmt (header level) checkbox is checked
        if (IsRerunStmtGridHdrSelected()) {
            DisplayMessagePopup("Selected rerun statements have not been processed. Please unselect to proceed.");
            return false;
        }

        //2.warning on selecting Recal Summary Stmt header checkbox when Rerun Stmt (row level) checkbox is checked
        if (IsRerunStmtSelected()) {
            DisplayMessagePopup("Selected rerun statements have not been processed. Please unselect to proceed.");
            return false;
        }
    }

    return true;
}


//WUIN-1057 changes - end


    </script>
    <asp:UpdatePanel ID="updPnlPageLevel" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="11"></td>
                </tr>
                <tr>
                    <td colspan="11">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    STATEMENT WORKFLOW
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="11"></td>
                </tr>
                <tr>
                    <td colspan="11"></td>
                </tr>
                <tr>
                    <td colspan="11"></td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td width="10%" class="identifierLable_large_bold">Company
                    </td>
                    <td width="15%">
                        <asp:DropDownList ID="ddlCompany" runat="server" Width="75%" CssClass="ddlStyle" onkeydown="SearchByEnterKey();" TabIndex="100">
                        </asp:DropDownList>
                    </td>
                    <td width="3%"></td>
                    <td width="10%" class="identifierLable_large_bold">Team Responsibility
                    </td>
                    <td width="15%">
                        <asp:DropDownList ID="ddlResponsibility" runat="server" Width="75%" CssClass="ddlStyle" OnSelectedIndexChanged="ddlResponsibility_SelectedIndexChanged"
                            AutoPostBack="true" onkeydown="SearchByEnterKey();" TabIndex="104">
                        </asp:DropDownList>
                    </td>
                    <td width="3%"></td>
                    <td class="identifierLable_large_bold" width="10%">Status</td>
                    <td width="13%">
                        <asp:DropDownList ID="ddlStatus" runat="server" Width="75%" onkeydown="SearchByEnterKey();" TabIndex="107" CssClass="ddlStyle"></asp:DropDownList>
                    </td>
                    <td rowspan="4" width="15%" valign="top">
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td align="right">
                                    <asp:Button ID="btnEditFrontSheet" runat="server" Text="Update Statement Activity List" CssClass="ButtonStyle" OnClientClick="if (!OnUpdateStmtActivityClick()) { return false;};" OnClick="btnEditFrontSheet_Click"
                                        Width="91%" UseSubmitBehavior="false" TabIndex="115" />
                                </td>
                            </tr>
                            <tr>
                                <td align="right" style="padding-top: 4px">
                                    <asp:Button ID="btnRecalFrntSht" runat="server" Text="Recalculate Front Sheets" CssClass="ButtonStyle" OnClick="btnRecalFrntSht_Click" Width="91%"
                                        UseSubmitBehavior="false" TabIndex="116" OnClientClick="if (!OnRecalStmtClick()) { return false;};" onkeydown="OnTabPress();" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">Reporting Schedule
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlReportingSch" runat="server" Width="75%" onkeydown="SearchByEnterKey();" CssClass="ddlStyle" TabIndex="101"></asp:DropDownList>
                    </td>
                    <td></td>
                    <td class="identifierLable_large_bold">Manager Responsibility
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlMngrResponsibility" runat="server" Width="75%" CssClass="ddlStyle" OnSelectedIndexChanged="ddlMngrResponsibility_SelectedIndexChanged"
                            AutoPostBack="true" onkeydown="SearchByEnterKey();" TabIndex="104">
                        </asp:DropDownList>
                    </td>
                    <td></td>
                    <td>
                        <table width="100%" cellspacing="0">
                            <tr>
                                <td width="75%" class="identifierLable_large_bold">Earnings</td>
                                <td>
                                    <asp:DropDownList ID="ddlEarningsCompare" runat="server" Width="95%" onkeydown="SearchByEnterKey();" TabIndex="108" CssClass="ddlStyle">
                                        <asp:ListItem Text="=" Value="=" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="<=" Value="<="></asp:ListItem>
                                        <asp:ListItem Text=">=" Value=">="></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        <asp:TextBox ID="txtEarnings" runat="server" Width="50%" CssClass="identifierLable" TabIndex="109" onblur="OnTxtEarningsBlur();" onkeydown="SearchByEnterKey();"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="revtxtEarnings" runat="server" Text="*" ControlToValidate="txtEarnings" ValidationGroup="valGrpGo"
                            ValidationExpression="^-?(\d*\.)?\d+$" CssClass="requiredFieldValidator" ForeColor="Red"
                            ToolTip="Please enter only numeric values" Display="Dynamic"> </asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">Owner
                    </td>
                    <td>
                        <asp:TextBox ID="txtOwnSearch" runat="server" Width="98%" CssClass="identifierLable" onkeydown="OntxtOwnerKeyDown();"
                            TabIndex="102"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="ownerFilterExtender" runat="server"
                            ServiceMethod="FuzzyWorkflowOwnerList"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtOwnSearch"
                            FirstRowSelected="true"
                            OnClientPopulating="ownerListPopulating"
                            OnClientPopulated="ownerListPopulated"
                            OnClientHidden="ownerListPopulated"
                            OnClientShown="ownerScrollPosition"
                            OnClientItemSelected="ownerListItemSelected"
                            CompletionListElementID="acePanelOwner" />
                        <asp:Panel ID="acePanelOwner" runat="server" CssClass="identifierLable" />
                    </td>
                    <td valign="middle">
                        <asp:ImageButton ID="fuzzySearchOwner" ImageUrl="../Images/Search.png" runat="server" Style="cursor: pointer"
                            OnClick="fuzzySearchOwner_Click" ToolTip="Search Owner" CssClass="FuzzySearch_Button" />
                    </td>
                    <td class="identifierLable_large_bold">Priority
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlPriority" runat="server" Width="75%" onkeydown="SearchByEnterKey();" CssClass="ddlStyle" TabIndex="105"></asp:DropDownList>

                    </td>
                    <td></td>
                    <td class="identifierLable_large_bold">
                        <table width="100%" cellspacing="0">
                            <tr>
                                <td width="75%" class="identifierLable_large_bold">Closing Balance</td>
                                <td>
                                    <asp:DropDownList ID="ddlClosingBalCompare" runat="server" Width="95%" onkeydown="SearchByEnterKey();" TabIndex="110" CssClass="ddlStyle">
                                        <asp:ListItem Value="=">=</asp:ListItem>
                                        <asp:ListItem Value="<="><=</asp:ListItem>
                                        <asp:ListItem Value=">=">>=</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        <asp:TextBox ID="txtClosingBalance" runat="server" CssClass="identifierLable" Width="50%" onblur="OnTxtClosingBalanceBlur();" onkeydown="SearchByEnterKey();"
                            TabIndex="111"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="revTxtClosingBalance" runat="server" Text="*" ControlToValidate="txtClosingBalance" ValidationGroup="valGrpGo"
                            ValidationExpression="^-?(\d*\.)?\d+$" CssClass="requiredFieldValidator" ForeColor="Red"
                            ToolTip="Please enter only numeric values" Display="Dynamic"> </asp:RegularExpressionValidator>
                    </td>

                </tr>
                <tr>
                    <td></td>
                    <td class="identifierLable_large_bold">Royaltor
                    </td>
                    <td>
                        <asp:TextBox ID="txtRoyaltor" runat="server" Width="98%" CssClass="identifierLable" onkeydown="OntxtRoyaltorKeyDown();" TabIndex="103"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="royaltorFilterExtender" runat="server"
                            ServiceMethod="FuzzyWorkflowRoyaltorList"
                            ServicePath="~/Services/FuzzySearch.asmx"
                            MinimumPrefixLength="1"
                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                            TargetControlID="txtRoyaltor"
                            FirstRowSelected="true"
                            OnClientPopulating="royaltorListPopulating"
                            OnClientPopulated="royaltorListPopulated"
                            OnClientHidden="royaltorListPopulated"
                            OnClientShown="resetScrollPosition"
                            OnClientItemSelected="royaltorListItemSelected"
                            CompletionListElementID="autocompleteDropDownPanel1" />
                        <asp:Panel ID="autocompleteDropDownPanel1" runat="server" CssClass="identifierLable" />
                    </td>
                    <td valign="middle">
                        <asp:ImageButton ID="fuzzySearchRoyaltor" ImageUrl="../Images/search.png" runat="server" Style="cursor: pointer"
                            OnClick="fuzzySearchRoyaltor_Click" ToolTip="Search Royaltor" CssClass="FuzzySearch_Button" />
                    </td>
                    <td class="identifierLable_large_bold">Producer</td>
                    <td>
                        <asp:TextBox ID="txtProducer" runat="server" CssClass="identifierLable" OnKeyDown="OnTxtProducerKeyDown();" Width="98%"
                            TabIndex="106"></asp:TextBox>
                    </td>
                    <td></td>
                    <td valign="bottom">
                        <table width="90%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="40%">
                                    <asp:Button ID="btnGo" runat="server" CssClass="ButtonStyle"
                                        OnClientClick="if (!OnGoBtnClick()) { return false;};" OnClick="btnGo_Click" TabIndex="112" Text="Go" UseSubmitBehavior="false" Width="80%" ValidationGroup="valGrpGo" />
                                </td>
                                <td width="5%"></td>
                                <td width="40%">
                                    <asp:Button ID="btnClear" runat="server" CssClass="ButtonStyle" OnClick="btnClear_Click" TabIndex="113" Text="Clear" UseSubmitBehavior="false" Width="80%" />
                                </td>

                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="11"></td>
                </tr>
                <tr>
                    <td colspan="11"></td>
                </tr>
                <tr>
                    <td colspan="11"></td>
                </tr>
                <tr>
                    <td colspan="11" align="center">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td colspan="9" align="left">
                                    <table width="98.72%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td width="13%" class="gridHeaderStyle_3rows">Reporting Schedule</td>
                                            <td width="5%" class="gridHeaderStyle_3rows">Owner</td>
                                            <td width="13%" class="gridHeaderStyle_3rows">Owner Name</td>
                                            <td width="5%" class="gridHeaderStyle_3rows">Royaltor</td>
                                            <td width="15%" class="gridHeaderStyle_3rows">Royaltor Name</td>
                                            <td width="8%" class="gridHeaderStyle_3rows">Team Responsibility</td>
                                            <td width="5%" class="gridHeaderStyle_3rows">Earnings</td>
                                            <td width="4%" class="gridHeaderStyle_3rows">Closing Balance</td>
                                            <td width="18%" id="tdStatusHdr" runat="server" class="gridHeaderStyle_3rows" align="center">
                                                <table width="99%" cellspacing="0">
                                                    <tr>
                                                        <td width="20%">Under Review</td>
                                                        <td width="20%" id="tdTeamSignOffHdr" runat="server">Team Sign Off</td>
                                                        <td width="20%" id="tdMngrSignOffHdr" runat="server">Manager Sign Off</td>
                                                        <td width="20%">Final Sign Off</td>
                                                        <td width="20%">Archive</td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="5" align="center">
                                                            <asp:RadioButtonList ID="rblistStatusHeader" runat="server" RepeatDirection="Horizontal"
                                                                RepeatLayout="Table" RepeatColumns="5" Width="100%" onclick="if (!OnStatusChangeHeader()) { return false;};"
                                                                OnSelectedIndexChanged="rblistStatusHeader_SelectedIndexChanged" CssClass="identifierLable">
                                                                <%--Harish: removed AutoPostBack="true" as this is being called from javascript"--%>
                                                                <asp:ListItem Value="1"></asp:ListItem>
                                                                <asp:ListItem Value="2"></asp:ListItem>
                                                                <asp:ListItem Value="8"></asp:ListItem>
                                                                <asp:ListItem Value="3"></asp:ListItem>
                                                                <asp:ListItem Value="4"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td width="4%" class="gridHeaderStyle_3rows">Comment
                                                <br />
                                            </td>
                                            <td width="5%" class="gridHeaderStyle_3rows" align="center">
                                                <table width="99%" cellspacing="0">
                                                    <tr>
                                                        <td>Re-run Statement
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:CheckBox ID="cbRerunStmtHeader" runat="server" OnCheckedChanged="cbRerunStmtHeader_CheckedChanged" AutoPostBack="true"
                                                                onclick="if (!OnRerunStmtHeaderClick()) { return false;};" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td width="5%" class="gridHeaderStyle_3rows">
                                                <table width="99%" cellspacing="0">
                                                    <tr>
                                                        <td>Recalculate Stmt Summary
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:CheckBox ID="cbRecalSummaryHeader" runat="server" OnCheckedChanged="cbRecalSummaryHeader_CheckedChanged" AutoPostBack="true"
                                                                onclick="if (!OnRecalSummHeaderClick()) { return false;};" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="9">
                                    <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="100%">
                                        <asp:GridView ID="gvRoyActivity" runat="server" AutoGenerateColumns="False" Width="98.72%" AlternatingRowStyle-BackColor="#E3EFFF"
                                            CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" ShowHeaderWhenEmpty="true" EmptyDataText="No Data Found"
                                            EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" OnRowDataBound="gvRoyActivity_RowDataBound" OnRowCommand="gvRoyActivity_RowCommand" ShowHeader="false">
                                            <Columns>
                                                <asp:TemplateField HeaderText="" ItemStyle-Width="1%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imgExpand" runat="server" ImageUrl="../Images/Plus.gif" CommandName="Expand" Visible="false"
                                                            CommandArgument='<%# Container.DataItemIndex %>' />
                                                        <asp:ImageButton ID="imgCollapse" runat="server" ImageUrl="../Images/Minus.gif" CommandName="Collapse" Visible="false"
                                                            CommandArgument='<%# Container.DataItemIndex %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="12%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">

                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRepSchedule" runat="server" Text='<%#Bind("rep_schedule")%>' CssClass="identifierLable" />
                                                        <asp:Label ID="lblStmtPeriodId" runat="server" Text='<%#Bind("stmt_period_id")%>' Visible="false" />
                                                        <asp:HiddenField ID="hdnStmtPeriodId" runat="server" Value='<%# Bind("stmt_period_id") %>' />
                                                        <%--This is used in comments--%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOwner" runat="server" Text='<%#Bind("owner_code")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="13%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOwnerName" runat="server" Text='<%#Bind("owner_name")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRoyaltor" runat="server" Text='<%#Bind("royaltor")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="15%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRoyaltorName" runat="server" Text='<%#Bind("royaltor_name")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="8%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblResponsibility" runat="server" Text='<%#Bind("responsibility")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEarnings" runat="server" Text='<%#Bind("earnings")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblClosingBal" runat="server" Text='<%#Bind("closing_balance")%>' CssClass="identifierLable" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="18%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStatus" runat="server" Text='<%#Bind("status_code")%>' Visible="false" />
                                                        <asp:HiddenField ID="hdnStatus" runat="server" Value='<%# Bind("status_code") %>' />
                                                        <asp:HiddenField ID="hdnOwnerLevelStatus" runat="server" />
                                                        <%-- WUIN-290 - used in final sign off warning--%>
                                                        <asp:RadioButtonList ID="rblistStatus" runat="server" RepeatDirection="Horizontal"
                                                            RepeatLayout="Table" RepeatColumns="5" Width="100%" onclick="if (!OnStatusChange(this)) { return false;};"
                                                            OnSelectedIndexChanged="rblistStatus_SelectedIndexChanged" CssClass="identifierLable">
                                                            <%--Harish 02-01-18: removed AutoPostBack="true" as this is being called from javascript"--%>
                                                            <asp:ListItem Value="1"></asp:ListItem>
                                                            <asp:ListItem Value="2"></asp:ListItem>
                                                            <asp:ListItem Value="8"></asp:ListItem>
                                                            <asp:ListItem Value="3"></asp:ListItem>
                                                            <asp:ListItem Value="4"></asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" ItemStyle-VerticalAlign="Middle"
                                                    HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnComments" runat="server" Value='<%# Bind("comments") %>' />
                                                        <asp:ImageButton ID="imgBtnCommentWithLine" runat="server" CommandName="Comment" ImageUrl="../Images/Comment_with_lines.png"
                                                            TabIndex="111" CausesValidation="false" CommandArgument='<%# Container.DataItemIndex %>'
                                                            OnClientClick="if (!OnCommentsClick()) { return false;};" />
                                                        <asp:ImageButton ID="imgBtnCommentWithOutLine" runat="server" CommandName="Comment" ImageUrl="../Images/Comment_without_lines.png"
                                                            ToolTip="" TabIndex="111" CausesValidation="false" CommandArgument='<%# Container.DataItemIndex %>'
                                                            OnClientClick="if (!OnCommentsClick()) { return false;};" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblReCreateStats" runat="server" Text='<%#Bind("royaltor_stmt_flag")%>' Visible="false" />
                                                        <asp:HiddenField ID="hdnReCreateStats" runat="server" Value='<%# Bind("royaltor_stmt_flag") %>' />
                                                        <asp:HiddenField ID="hdnOwnerLevelReCreateStatus" runat="server" />
                                                        <%-- WUIN-290 - used in final sign off warning--%>
                                                        <asp:Label ID="lblDtlFileFlag" runat="server" Text='<%#Bind("dtl_file_flag")%>' Visible="false" />
                                                        <asp:Label ID="lblRoyaltorHeld" runat="server" Text='<%#Bind("royaltor_held")%>' Visible="false" />
                                                        <asp:CheckBox ID="cbReCreateStats" runat="server" onfocusin="if (!OnRerunStmtClick()) { return false;};" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cbRecalFrntSht" runat="server" OnCheckedChanged="cbRecalFrntSht_CheckedChanged" onclick="if (!OnRecalFrntShtClick()) { return false;};" />
                                                        <%-- AutoPostBack="true"--%>
                                                        <asp:HiddenField ID="hdnStmtHeld" runat="server" Value='<%# Bind("statement_held") %>' />
                                                        <asp:HiddenField ID="hdnRerunStmt" runat="server" Value='<%# Bind("rerun_stmt") %>' />
                                                        <asp:HiddenField ID="hdnRecalStmt" runat="server" Value='<%# Bind("recalculate_stmt") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="9">
                                    <asp:Repeater ID="rptPager" runat="server">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>'
                                                Enabled='<%# Eval("Enabled") %>' CssClass="gridPager"
                                                OnClientClick="if (!OnPageChangeClick()) { return false;};" OnClick="lnkPage_Click"> </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:Repeater>
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
                                <td class="identifierLable">Please Wait...
                                </td>
                            </tr>
                        </table>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <ajaxToolkit:ModalPopupExtender ID="mPopupPageLevel" runat="server" PopupControlID="progressBarPageLevel" TargetControlID="progressBarPageLevel"
                BackgroundCssClass="progressBar" RepositionMode="RepositionOnWindowResize">
            </ajaxToolkit:ModalPopupExtender>

            <asp:Button ID="dummyRecalFrntSht" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeRecalFrntSht" runat="server" PopupControlID="pnlRecalFrntSht" TargetControlID="dummyRecalFrntSht"
                CancelControlID="btnNoRecalFrntSht" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlRecalFrntSht" runat="server" align="center" Width="35%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="Label1" runat="server" Text="Confirm" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="txtRecalFrntShtMsg" runat="server" CssClass="gridTextField" TextMode="MultiLine" Width="90%" Height="60"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYesRecalFrntSht" runat="server" Text="Yes" CssClass="ButtonStyle" OnClick="btnYesRecalFrntSht_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNoRecalFrntSht" runat="server" Text="No" CssClass="ButtonStyle" />
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
            <asp:Panel ID="pnlFuzzySearch" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="display: none">
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

            <%--WUIN-290 change: 3.	When Final Sign off is selected (at Owner or Royaltor level), display a warning popup --%>
            <asp:Button ID="dummyFinalSignOffWarning" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeFinalSignOffWarning" runat="server" PopupControlID="pnlFinalSignOffWarning" TargetControlID="dummyFinalSignOffWarning"
                CancelControlID="cancelFinalSignOffWarning" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlFinalSignOffWarning" runat="server" align="center" Width="35%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmMsgHdr" runat="server" Text="Confirm" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblConfirmMsg" runat="server" Text="This update cannot be reversed. Payment details will be generated if applicable. Select OK to Continue or Cancel"
                                CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="OKFinalSignOffWarning" runat="server" Text="OK" CssClass="ButtonStyle" OnClientClick="CallStatusUpdateEvent();" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="cancelFinalSignOffWarning" runat="server" Text="Cancel" CssClass="ButtonStyle" OnClientClick="CancelFinalSignOffWarning();" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <%--WUIN-1089 - Confirmation pop up on bulk update of status--%>
            <asp:Button ID="dummyBulkUpdateConfirm" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeBulkUpdateConfirm" runat="server" PopupControlID="pnlBulkUpdateConfirm" TargetControlID="dummyBulkUpdateConfirm"
                CancelControlID="btnNoBulkUpdateConfirm" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlBulkUpdateConfirm" runat="server" align="center" Width="35%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblFinalSignOffWarningHdr" runat="server" Text="Confirm" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblFinalSignOffWarningMsg" runat="server" Text="Changes will be applied to all the rows across the pages, Do you wish to continue?"
                                CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYesBulkUpdateConfirm" runat="server" Text="Yes" CssClass="ButtonStyle"
                                            OnClientClick="return OnBulkUpdateConfirmYes();" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNoBulkUpdateConfirm" runat="server" Text="No" CssClass="ButtonStyle" OnClientClick="return OnBulkUpdateConfirmNo();" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <%--Comments--%>
            <asp:Button ID="dummyCommentPopup" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeCommentPopup" runat="server" PopupControlID="pnlCommentPopup" TargetControlID="dummyCommentPopup"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlCommentPopup" runat="server" align="left" Width="50%" CssClass="popupPanel" Style="display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="identifierLable" align="center">Comment
                                    </td>
                                    <td align="right" style="vertical-align: top;" width="10%">
                                        <asp:ImageButton ID="btnCloseCommentPopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" CausesValidation="false"
                                            OnClientClick="return OnCloseCommentPopup('btnCloseCommentPopup');" />
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
                                        <iframe id="iFrameComment" name="iframe" runat="server" src="../StatementProcessing/WorkFlowCommentRichText.aspx" width="100%" frameborder="0"></iframe>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center"></td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <table width="100%">
                                            <tr>
                                                <td width="30%" align="right">
                                                    <asp:Label ID="lblCommentError" runat="server" Text="Please enter comment" CssClass="ErrorLabelHidden"></asp:Label>
                                                </td>
                                                <td width="30%" align="left">
                                                    <table width="80%">
                                                        <tr>
                                                            <td width="30%">
                                                                <asp:Button ID="btnSaveComment" runat="server" CssClass="ButtonStyle"
                                                                    OnClientClick="if (!SaveComment()) { return false;};" OnClick="btnSaveComment_Click"
                                                                    Text="Save" UseSubmitBehavior="false" Width="90%" />
                                                            </td>
                                                            <td width="30%">
                                                                <asp:Button ID="btnDeleteComment" runat="server" CssClass="ButtonStyle" OnClick="btnSaveComment_Click"
                                                                    Text="Delete" UseSubmitBehavior="false" Width="90%" ValidationGroup="valGrpSaveComment" />
                                                            </td>
                                                            <td width="30%">
                                                                <asp:Button ID="btnCancelComment" runat="server" CssClass="ButtonStyle" OnClientClick="return CancelComment('btnCancelComment');"
                                                                    Text="Cancel" UseSubmitBehavior="false" Width="90%" CausesValidation="false" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td width="30%"></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <table width="100%">
                                            <tr>
                                                <td align="left" class="identifierLable_large_bold" width="20%">Attachments:</td>
                                                <td width="80%" align="center"></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <table width="100%">
                                            <tr>
                                                <td width="50%" class="gridHeaderStyle_1row" align="center">Upload
                                                </td>
                                                <td width="50%" class="gridHeaderStyle_1row" align="center">Download
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top">
                                                    <table width="100%" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td width="78%">
                                                                <div style="position: relative; left: -2px;">
                                                                    <asp:FileUpload ID="uploadCommentAttachment" runat="server" CssClass="FileUpload" BackColor="White" Width="99%" />
                                                                </div>
                                                            </td>
                                                            <td width="15%">
                                                                <asp:Button ID="btnCommentUploadFile" runat="server" CssClass="ButtonStyle"
                                                                    OnClientClick="if (!OnBtnCommentUploadFileClick('btnCommentUploadFile')) { return false;};" Text="Upload" UseSubmitBehavior="false" Width="95%" />
                                                            </td>
                                                            <td width="7%" align="center">
                                                                <asp:ImageButton ID="btnUndoCommentUploadFile" runat="server" ImageUrl="../Images/cancel_row3.png" ToolTip="Cancel"
                                                                    OnClientClick="return OnBtnUndoCommentUploadFile();" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right">
                                                                <asp:Label ID="lblCommentFileUploadError" runat="server" Text="Please select a file to upload" CssClass="ErrorLabelHidden"></asp:Label>
                                                            </td>
                                                            <td></td>
                                                            <td></td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td>
                                                    <asp:Panel ID="plnGridCommentDownloadFile" runat="server" ScrollBars="Auto" Width="75%">
                                                        <asp:GridView ID="gvCommentDownloadFile" runat="server" AutoGenerateColumns="False" Width="92%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                            CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                            EmptyDataText="No data found" ShowHeader="false" RowStyle-CssClass="dataRow">
                                                            <Columns>
                                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align"
                                                                    ItemStyle-Width="80%">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="lblFileName" runat="server" Width="99%" Text='<%#Bind("FileName")%>' CssClass="gridTextField" ReadOnly="true"
                                                                            ToolTip='<%#Bind("FileName")%>'></asp:TextBox>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align"
                                                                    ItemStyle-Width="20%">
                                                                    <ItemTemplate>
                                                                        <table width="95%" style="float: right; table-layout: fixed">
                                                                            <tr style="float: right">
                                                                                <td align="center">
                                                                                    <asp:ImageButton ID="gridCommentDownloadFile" runat="server" ImageUrl="../Images/FileDownload.png"
                                                                                        ToolTip="Download File" OnClientClick="return GridCommentDownloadFileClick(this);" />
                                                                                </td>
                                                                                <td align="center">
                                                                                    <asp:ImageButton ID="btnCommentDeleteFile" runat="server" ImageUrl="../Images/Delete.gif"
                                                                                        ToolTip="Delete File" OnClientClick="return ConfirmCommentFileDelete(this);" />
                                                                                </td>
                                                                            </tr>
                                                                        </table>
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
                                <input type="hidden" id="txtHidCommentData" runat="server" />
                                <asp:HiddenField ID="hdnCommentRoyaltorId" runat="server" />
                                <asp:HiddenField ID="hdnCommentStmtPeriodId" runat="server" />
                                <asp:HiddenField ID="hdnCommentOwnerCode" runat="server" />
                                <asp:HiddenField ID="hdnCommentUpload" runat="server" />
                                <asp:HiddenField ID="hdnCommentDownloadFile" runat="server" />
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Comments - Ends--%>

            <%--Comments - Upload existing attachment confirmation -- Start--%>
            <asp:Button ID="dummyCommentUploadFile" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeCommentUploadFileConfirm" runat="server" PopupControlID="pnlCommentUploadFile" TargetControlID="dummyCommentUploadFile"
                CancelControlID="btnNoCommentUploadFile" BackgroundCssClass="messageBackground">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlCommentUploadFile" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="Label2" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="A file exists with the same name, Do you want to overwrite the existing file?"
                                CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYesCommentUploadFile" runat="server" Text="Yes" CssClass="ButtonStyle" OnClientClick="CallUploadFileEvent();" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNoCommentUploadFile" runat="server" Text="No" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Comments - Upload existing attachment confirmation -- End--%>

            <%--Comments - Delete attachment confirmation -- Start--%>
            <asp:Button ID="dummyCommentDeleteFile" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeCommentDeleteFile" runat="server" PopupControlID="pnlCommentDeleteFile" TargetControlID="dummyCommentDeleteFile"
                CancelControlID="btnNoCommentDeleteFile" BackgroundCssClass="messageBackground">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlCommentDeleteFile" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmCommentDeleteFile" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblCommentDeleteFilePopUpMsg" runat="server" Text="Do you want to delete the file?"
                                CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYesCommentDeleteFile" runat="server" Text="Yes" CssClass="ButtonStyle" OnClientClick="if (!OnYesCommentDeleteFileClick('btnYesCommentDeleteFile')) { return false;};"
                                            OnClick="btnYesCommentDeleteFile_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNoCommentDeleteFile" runat="server" Text="No" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Comments - Delete attachment confirmation -- End--%>

            <%--Status bulk update - confirmation -- Start--%>
            <asp:Button ID="dummyStatusBulkUpdateConfirm" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeStatusBulkUpdateConfirm" runat="server" PopupControlID="pnlStatusBulkUpdateConfirm" TargetControlID="dummyStatusBulkUpdateConfirm"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlStatusBulkUpdateConfirm" runat="server" align="center" Width="35%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblHdrStatusBulkUpdateConfirm" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMsgStatusBulkUpdateConfirm" runat="server" Text="" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYesStatusBulkUpdateConfirm" runat="server" Text="Yes" CssClass="ButtonStyle" OnClick="btnYesStatusBulkUpdateConfirm_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNoStatusBulkUpdateConfirm" runat="server" Text="No" CssClass="ButtonStyle" OnClick="btnNoStatusBulkUpdateConfirm_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Status bulk update - confirmation -- End--%>

            <%--Status update - confirmation -- Start--%>
            <asp:Button ID="dummyStatusUpdateConfirm" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeStatusUpdateConfirm" runat="server" PopupControlID="pnlStatusUpdateConfirm" TargetControlID="dummyStatusUpdateConfirm"
                CancelControlID="btnNoStatusUpdateConfirm" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlStatusUpdateConfirm" runat="server" align="center" Width="35%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblHdrStatusUpdateConfirm" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMsgStatusUpdateConfirm" runat="server" Text="" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYesStatusUpdateConfirm" runat="server" Text="Yes" CssClass="ButtonStyle" OnClick="btnYesStatusUpdateConfirm_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNoStatusUpdateConfirm" runat="server" Text="No" CssClass="ButtonStyle" OnClientClick="CancelFinalSignOffWarning();" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Status update - confirmation -- End--%>

            <%--Warning on unsaved data popup--%>
            <asp:Button ID="dummyUnsavedWarnMsg" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeUnSavedWarning" runat="server" PopupControlID="pnlUnsavedWarnMsgPopup" TargetControlID="dummyUnsavedWarnMsg"
                BackgroundCssClass="messageBackground">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlUnsavedWarnMsgPopup" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="display: none">
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
                                        <asp:Button ID="btnUnSavedDataExit" runat="server" Text="Exit" CssClass="ButtonStyle" Width="30%" OnClientClick="if (!OnUnSavedDataExit()) { return false;};" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:Button ID="btnValSearchMsgPopupDummy" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeValSearchMsgPopup" runat="server" PopupControlID="pnlValSearchMsgPopup" TargetControlID="btnValSearchMsgPopupDummy"
                BackgroundCssClass="popupBox" CancelControlID="imgbtnCloseValSearchMsgPopup">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlValSearchMsgPopup" runat="server" align="center" Width="35%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                Style="z-index: 1; display: none" onkeydown="CloseValSearchMsgPopup();">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <table width="100%">
                                <tr>
                                    <td align="right" style="vertical-align: top;">
                                        <asp:ImageButton ID="imgbtnCloseValSearchMsgPopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblValSearchMsgPopup" runat="server" CssClass="identifierLable" Text=""></asp:Label>
                        </td>
                    </tr>

                </table>
            </asp:Panel>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnOwnersToExpand" runat="server" />
            <asp:HiddenField ID="hdnPageMode" runat="server" />
            <asp:HiddenField ID="hdnUserRole" runat="server" />
            <asp:HiddenField ID="hdnUpdatedStmtPeriod" runat="server" />
            <asp:HiddenField ID="hdnPageIndex" runat="server" />
            <asp:HiddenField ID="hdnGridPageSize" runat="server" />
            <asp:HiddenField ID="hdnIsRecreateStmtUpdated" runat="server" Value="" />
            <asp:HiddenField ID="hdnReCalFrntShtErrRow" runat="server" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" Value="" />
            <asp:HiddenField ID="hdnFinalSignOffWarningPrevStatus" runat="server" Value="" />
            <asp:HiddenField ID="hdnFinalSignOffWarningRowIndex" runat="server" Value="" />
            <asp:HiddenField ID="hdnFinalSignOffWarningReCreateSt" runat="server" Value="" />
            <asp:HiddenField ID="hdnTeamSignOffVisible" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnMngrSignOffVisible" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnCommentSelectedFileName" runat="server" />
            <asp:HiddenField ID="hdnStatusUpdateGridHdr" runat="server" Value="N" />
            <asp:HiddenField ID="hdnCommentFileUploadDuplicateCheck" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnStatusUpdateConfirmOwnerCode" runat="server" />
            <asp:HiddenField ID="hdnStatusUpdateConfirmStmtPrdId" runat="server" />
            <asp:HiddenField ID="hdnStatusUpdateConfirmStatusCode" runat="server" />
            <asp:HiddenField ID="hdnIgnoreStatusClick" runat="server" />
            <asp:HiddenField ID="hdnIsCommentChanged" runat="server" Value="N" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsConfirmPopup" runat="server" Value="N" />
            <asp:HiddenField ID="hdnButtonSelection" runat="server" />
            <asp:HiddenField ID="hdnIsValidRoyaltor" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsValidOwner" runat="server" Value="N" />
            <asp:HiddenField ID="hdnAllowGridPageChange" runat="server" Value="Y" />

            <asp:Label ID="lblTab" runat="server" TabIndex="99" />
            <asp:Button ID="btnRbStatusClick" runat="server" Style="display: none;" OnClick="btnRbStatusClick_Click" CausesValidation="false" />
            <asp:Button ID="btnCbRecalFrntShtClick" runat="server" Style="display: none;" OnClick="btnCbRecalFrntShtClick_Click" CausesValidation="false" />
            <asp:Button ID="btnCommentDownloadFile" runat="server" Style="display: none;" OnClick="btnCommentDownloadFile_Click" CausesValidation="false" />
            <asp:Button ID="btnCommentUploadFileHidden" runat="server" Style="display: none;" OnClick="btnCommentUploadFileHidden_Click" CausesValidation="false" />
            <asp:Button ID="btnYesCommentDeleteFileHidden" runat="server" Style="display: none;" OnClick="btnYesCommentDeleteFile_Click" CausesValidation="false" />
            <asp:Button ID="btnCloseValSearchMsgPopup" runat="server" Style="display: none;" OnClick="btnCloseValSearchMsgPopup_Click" CausesValidation="false" />
            <asp:TextBox ID="txtValSearchMsgPopup" runat="server" Width="2" onkeydown="CloseValSearchMsgPopup();" Visible="false"></asp:TextBox>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnCommentUploadFileHidden" />
            <asp:PostBackTrigger ControlID="btnCommentDownloadFile" />
        </Triggers>
    </asp:UpdatePanel>
    <activityScreen:ActivityScreen ID="actScreen" runat="server" />
</asp:Content>

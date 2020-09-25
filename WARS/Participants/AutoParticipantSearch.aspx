<%@ Page Title="Auto Participant Search" Language="C#" MasterPageFile="~/MasterPage.Master" EnableEventValidation="false" ValidateRequest="false" AutoEventWireup="true" CodeBehind="AutoParticipantSearch.aspx.cs" Inherits="WARS.Participants.AutoParticipantSearch" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>



<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //Navigation button functionality - Start
        //to open Royaltor Search
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
        //open Catalog Search screen
        function OpenCatalogueSearch() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Participants/CatalogueSearch.aspx?isNewRequest=N");
            }
            else {
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
                            UseSubmitBehavior="false" OnClientClick="if (!OpenCatalogueSearch()) { return false;};" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">

    <script type="text/javascript">

        var gridClientId = "ContentPlaceHolderBody_gvAutoParticipantDtls_";
        var selectedRowIndex;
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
            if (postBackElementID.indexOf('imgBtnSave') != -1 || postBackElementID.indexOf('btnSaveChanges') != -1
                || postBackElementID.indexOf('btnUndoChanges') != -1) {
                xPos = sender._scrollPosition.x;
                yPos = sender._scrollPosition.y;

                //hold scroll position 
                var PnlReference = document.getElementById("<%=pnlAutoParticipantDtls.ClientID %>");
                scrollTop = PnlReference.scrollTop;
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
            if (postBackElementID.indexOf('imgBtnSave') != -1 || postBackElementID.indexOf('btnSaveChanges') != -1
                || postBackElementID.indexOf('btnUndoChanges') != -1) {
                window.scrollTo(xPos, yPos);

                //set scroll position 
                var PnlReference = document.getElementById("<%=pnlAutoParticipantDtls.ClientID %>");
                PnlReference.scrollTop = scrollTop;
            }

        }
        //============probress bar and scroll position functionality - Ends



        //Validate any unsaved data on browser window close/refresh
        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }


        //grid panel height adjustment functioanlity - starts

        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
            document.getElementById("<%=pnlAutoParticipantDtls.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;

        }

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        //Enter key Functionality on all fields      
        function SearchByEnterKey() {
            if ((event.keyCode == 13)) {
                document.getElementById('<%=btnSearch.ClientID%>').click();
            }
        }

        //Artist Add row fuzzy search -- Start

        function ArtistAddRowListPopulating() {
            txtArtistAddRow = document.getElementById("<%= txtArtistAddRow.ClientID %>");
            txtArtistAddRow.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtArtistAddRow.style.backgroundRepeat = 'no-repeat';
            txtArtistAddRow.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidArtist.ClientID %>").value = "N";
        }


        function ArtistAddRowListPopulated() {
            txtArtistAddRow = document.getElementById("<%= txtArtistAddRow.ClientID %>");
            txtArtistAddRow.style.backgroundImage = 'none';
        }

        function ArtistAddRowListHidden() {
            txtArtistAddRow = document.getElementById("<%= txtArtistAddRow.ClientID %>");
            txtArtistAddRow.style.backgroundImage = 'none';

        }

        function ArtistAddRowListItemSelected(sender, args) {
            var artistSrchVal = args.get_value();
            if (artistSrchVal == 'No results found') {
                txtArtistAddRow = document.getElementById("<%= txtArtistAddRow.ClientID %>");
                txtArtistAddRow.value = "";
            }
            else {
                document.getElementById("<%= hdnIsValidArtist.ClientID %>").value = "Y";
                document.getElementById('<%=txtArtistAddRow.ClientID%>').click();
            }
        }

        //Pop up fuzzy search list       
        function OntxtArtistAddRowKeyDown() {
            if ((event.keyCode == 13)) {
                //Enter key can be used to select the dropdown list item or to pop up the complete list
                //to know this, check if list item is selected or not
                var aceArtistAddRow = $find('ContentPlaceHolderBody_' + 'aceArtistAddRow');
                if (aceArtistAddRow._selectIndex == -1) {
                    txtArtistAddRow = document.getElementById("<%= txtArtistAddRow.ClientID %>").value;
                    document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtArtistAddRow;
                    document.getElementById("<%=hdnFuzzySearchField.ClientID %>").innerText = "ArtistAddRow";
                    document.getElementById('<%=btnFuzzyArtistListPopup.ClientID%>').click();
                }
            }

        }

        //reset field width when empty
        function OntxtArtistAddRowChange() {
            txtArtistAddRow = document.getElementById("<%=txtArtistAddRow.ClientID %>");
            if (txtArtistAddRow.value == "") {
                txtArtistAddRow.style["width"] = '98%';
            }
            AddParticipantCheck();
        }

        //Validate if the field value is a valid one from fuzzy search list
        function ValArtistAddRow(sender, args) {
            txtArtistAddRow = document.getElementById("<%=txtArtistAddRow.ClientID %>");
            if (txtArtistAddRow.value == "") {
                args.IsValid = true;
                txtArtistAddRow.style["width"] = '98%';
                document.getElementById("<%= hdnFuzzyAddRowValidator.ClientID %>").value = "N";
            }
            else if (txtArtistAddRow.value == "No results found") {
                args.IsValid = true;
                txtArtistAddRow.value = "";
                txtArtistAddRow.style["width"] = '98%';
                document.getElementById("<%= hdnFuzzyAddRowValidator.ClientID %>").value = "N";
            }
            else if (txtArtistAddRow.value != "" && txtArtistAddRow.value.indexOf('-') == -1) {
                args.IsValid = false;
                //adjust width of the textbox to display error
                txtArtistAddRow.style["width"] = '88%';
                document.getElementById("<%= hdnFuzzyAddRowValidator.ClientID %>").value = "Y";
                document.getElementById("<%= hdnIsValidArtist.ClientID %>").value = "N";
            }
            else if (args.IsValid == true) {
                txtArtistAddRow.style["width"] = '98%';
                document.getElementById("<%= hdnFuzzyAddRowValidator.ClientID %>").value = "N";
            }

}

//Artist Add row fuzzy search ====== End ==

//Project Title Add row fuzzy search -- Start

function ProjectTitleAddRowListPopulating() {
    txtProjectTitleAddRow = document.getElementById("<%= txtProjectTitleAddRow.ClientID %>");
    txtProjectTitleAddRow.style.backgroundImage = 'url(Images/textbox_loader.gif)';
    txtProjectTitleAddRow.style.backgroundRepeat = 'no-repeat';
    txtProjectTitleAddRow.style.backgroundPosition = 'right';
    document.getElementById("<%= hdnIsValidProject.ClientID %>").value = "N";
}

function ProjectTitleAddRowListPopulated() {
    txtProjectTitleAddRow = document.getElementById("<%= txtProjectTitleAddRow.ClientID %>");
    txtProjectTitleAddRow.style.backgroundImage = 'none';
}

function ProjectTitleAddRowListHidden() {
    txtProjectTitleAddRow = document.getElementById("<%= txtProjectTitleAddRow.ClientID %>");
    txtProjectTitleAddRow.style.backgroundImage = 'none';

}

function ProjectTitleAddRowListItemSelected(sender, args) {
    var artistSrchVal = args.get_value();
    if (artistSrchVal == 'No results found') {
        txtProjectTitleAddRow = document.getElementById("<%= txtProjectTitleAddRow.ClientID %>");
        txtProjectTitleAddRow.value = "";
    }
    else {
        document.getElementById("<%= hdnIsValidProject.ClientID %>").value = "Y";
        document.getElementById('<%=txtProjectTitleAddRow.ClientID%>').click();
    }
}

//Pop up fuzzy search list       
function OntxtProjectTitleAddRowKeyDown() {
    if ((event.keyCode == 13)) {
        //Enter key can be used to select the dropdown list item or to pop up the complete list
        //to know this, check if list item is selected or not
        var aceProjectTitleAddRow = $find('ContentPlaceHolderBody_' + 'aceProjectTitleAddRow');
        if (aceProjectTitleAddRow._selectIndex == -1) {
            txtProjectTitleAddRow = document.getElementById("<%= txtProjectTitleAddRow.ClientID %>").value;
            document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtProjectTitleAddRow;
            document.getElementById("<%=hdnFuzzySearchField.ClientID %>").innerText = "ProjectTitleAddRow";
            document.getElementById('<%=btnFuzzyProjectTitleListPopup.ClientID%>').click();
        }
    }

}

//reset field width when empty
function OntxtProjectTitleAddRowChange() {
    txtProjectTitleAddRow = document.getElementById("<%=txtProjectTitleAddRow.ClientID %>");
    if (txtProjectTitleAddRow.value == "") {
        txtProjectTitleAddRow.style["width"] = '98%';
    }
    AddParticipantCheck();

}

//Validate if the field value is a valid one from fuzzy search list
function ValProjectTitleAddRow(sender, args) {
    txtProjectTitleAddRow = document.getElementById("<%=txtProjectTitleAddRow.ClientID %>");
    if (txtProjectTitleAddRow.value == "") {
        args.IsValid = true;
        txtProjectTitleAddRow.style["width"] = '98%';
        document.getElementById("<%= hdnFuzzyAddRowValidator.ClientID %>").value = "N";
    }
    else if (txtProjectTitleAddRow.value == "No results found") {
        args.IsValid = true;
        txtProjectTitleAddRow.value = "";
        txtProjectTitleAddRow.style["width"] = '98%';
        document.getElementById("<%= hdnFuzzyAddRowValidator.ClientID %>").value = "N";
    }
    else if (txtProjectTitleAddRow.value != "" && txtProjectTitleAddRow.value.indexOf('-') == -1) {
        args.IsValid = false;
        //adjust width of the textbox to display error
        txtProjectTitleAddRow.style["width"] = '88%';
        document.getElementById("<%= hdnFuzzyAddRowValidator.ClientID %>").value = "Y";
        document.getElementById("<%= hdnIsValidProject.ClientID %>").value = "N";
    }
    else if (args.IsValid == true) {
        txtProjectTitleAddRow.style["width"] = '98%';
        document.getElementById("<%= hdnFuzzyAddRowValidator.ClientID %>").value = "N";
    }

}

//Project Add row fuzzy search ====== End ==

//Artist Grid row fuzzy search -- Starts

function ArtistGridRowListPopulating(sender, args) {
    selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
    txtArtistGridRow = document.getElementById(gridClientId + 'txtArtistGridRow' + '_' + selectedRowIndex);
    txtArtistGridRow.style.backgroundImage = 'url(Images/textbox_loader.gif)';
    txtArtistGridRow.style.backgroundRepeat = 'no-repeat';
    txtArtistGridRow.style.backgroundPosition = 'right';
    document.getElementById("<%= hdnIsValidArtistGridRow.ClientID %>").value = "N";

}

function ArtistGridRowListPopulated(sender, args) {
    selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
    txtArtistGridRow = document.getElementById(gridClientId + 'txtArtistGridRow' + '_' + selectedRowIndex);
    txtArtistGridRow.style.backgroundImage = 'none';

}

function ArtistGridRowListHidden(sender, args) {
    selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
    txtArtistGridRow = document.getElementById(gridClientId + 'txtArtistGridRow' + '_' + selectedRowIndex);
    txtArtistGridRow.style.backgroundImage = 'none';

}

function ArtistGridRowListItemSelected(sender, args) {
    var ArtistSrchVal = args.get_value();
    if (ArtistSrchVal == 'No results found') {
        selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
        document.getElementById(gridClientId + 'txtArtistGridRow' + '_' + selectedRowIndex).value = "";
    }
    else {
        document.getElementById("<%= hdnIsValidArtistGridRow.ClientID %>").value = "Y";
        document.getElementById(gridClientId + 'txtArtistGridRow' + '_' + selectedRowIndex).click();
    }

}

//Pop up fuzzy search list       
function OntxtArtistGridRowKeyDown(sender) {
    if ((event.keyCode == 13)) {
        selectedRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
        //Enter key can be used to select the dropdown list item or to pop up the complete list
        //to know this, check if list item is selected or not
        var aceArtistGridRow = $find(gridClientId + 'aceArtistGridRow' + '_' + selectedRowIndex);
        if (aceArtistGridRow._selectIndex == -1) {
            txtArtistGridRow = document.getElementById(gridClientId + 'txtArtistGridRow' + '_' + selectedRowIndex).value;
            document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtArtistGridRow;
            document.getElementById("<%=hdnGridFuzzySearchRowId.ClientID %>").innerText = selectedRowIndex;
            document.getElementById("<%=hdnFuzzySearchField.ClientID %>").innerText = "ArtistGridRow";
            document.getElementById('<%=btnFuzzyArtistListPopup.ClientID%>').click();
        }
    }

}

//Validate if the field value is a valid one from fuzzy search list
function ValArtistGridRow(sender, args) {
    gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
    txtArtistGridRow = document.getElementById(gridClientId + 'txtArtistGridRow' + '_' + gridRowIndex);
    if (txtArtistGridRow.value == "") {
        args.IsValid = true;
        txtArtistGridRow.style["width"] = '98%';
        document.getElementById("<%= hdnFuzzyGridRowValidator.ClientID %>").value = "N";
    }
    else if (txtArtistGridRow.value == "No results found") {
        args.IsValid = true;
        txtArtistGridRow.value = "";
        txtArtistGridRow.style["width"] = '98%';
        document.getElementById("<%= hdnFuzzyGridRowValidator.ClientID %>").value = "N";
    }
    else if (txtArtistGridRow.value != "" && txtArtistGridRow.value.indexOf('-') == -1) {
        args.IsValid = false;
        //adjust width of the textbox to display error
        txtArtistGridRow.style["width"] = '88%';
        document.getElementById("<%= hdnFuzzyGridRowValidator.ClientID %>").value = "Y";
        document.getElementById("<%= hdnIsValidArtistGridRow.ClientID %>").value = "N";
    }
    else if (args.IsValid == true) {
        txtArtistGridRow.style["width"] = '98%';
        document.getElementById("<%= hdnFuzzyGridRowValidator.ClientID %>").value = "N";
    }


}

//reset field width when empty
function OntxtArtistGridRowChange(sender) {
    gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
    txtArtistGridRow = document.getElementById(gridClientId + 'txtArtistGridRow' + '_' + gridRowIndex);
    if (txtArtistGridRow.value == "") {
        txtArtistGridRow.style["width"] = '98%';
    }
}

//===========Artist Grid Row fuzzy search -- Ends

//ProjectTitle Grid row fuzzy search -- Starts

function ProjectTitleGridRowListPopulating(sender, args) {
    selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
    txtProjectTitleGridRow = document.getElementById(gridClientId + 'txtProjectTitleGridRow' + '_' + selectedRowIndex);
    txtProjectTitleGridRow.style.backgroundImage = 'url(Images/textbox_loader.gif)';
    txtProjectTitleGridRow.style.backgroundRepeat = 'no-repeat';
    txtProjectTitleGridRow.style.backgroundPosition = 'right';
    document.getElementById("<%= hdnIsValidProjectGridRow.ClientID %>").value = "N";

}

function ProjectTitleGridRowListPopulated(sender, args) {
    selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
    txtProjectTitleGridRow = document.getElementById(gridClientId + 'txtProjectTitleGridRow' + '_' + selectedRowIndex);
    txtProjectTitleGridRow.style.backgroundImage = 'none';

}

function ProjectTitleGridRowListHidden(sender, args) {
    selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
    txtProjectTitleGridRow = document.getElementById(gridClientId + 'txtProjectTitleGridRow' + '_' + selectedRowIndex);
    txtProjectTitleGridRow.style.backgroundImage = 'none';

}

function ProjectTitleGridRowListItemSelected(sender, args) {
    var ProjectTitleSrchVal = args.get_value();
    if (ProjectTitleSrchVal == 'No results found') {
        selectedRowIndex = sender._id.substring(sender._id.lastIndexOf("_") + 1);
        document.getElementById(gridClientId + 'txtProjectTitleGridRow' + '_' + selectedRowIndex).value = "";
    }

    else {
        document.getElementById("<%= hdnIsValidProjectGridRow.ClientID %>").value = "Y";
        document.getElementById(gridClientId + 'txtProjectTitleGridRow' + '_' + selectedRowIndex).click();
    }

}

//Pop up fuzzy search list       
function OntxtProjectTitleGridRowKeyDown(sender) {
    if ((event.keyCode == 13)) {
        selectedRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
        //Enter key can be used to select the dropdown list item or to pop up the complete list
        //to know this, check if list item is selected or not
        var aceProjectTitleGridRow = $find(gridClientId + 'aceProjectTitleGridRow' + '_' + selectedRowIndex);
        if (aceProjectTitleGridRow._selectIndex == -1) {
            txtProjectTitleGridRow = document.getElementById(gridClientId + 'txtProjectTitleGridRow' + '_' + selectedRowIndex).value;
            document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtProjectTitleGridRow;
            document.getElementById("<%=hdnGridFuzzySearchRowId.ClientID %>").innerText = selectedRowIndex;
            document.getElementById("<%=hdnFuzzySearchField.ClientID %>").innerText = "ProjectTitleGridRow";
            document.getElementById('<%=btnFuzzyProjectTitleListPopup.ClientID%>').click();
        }
    }

}

//Validate if the field value is a valid one from fuzzy search list
function ValProjectTitleGridRow(sender, args) {
    gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
    txtProjectTitleGridRow = document.getElementById(gridClientId + 'txtProjectTitleGridRow' + '_' + gridRowIndex);
    if (txtProjectTitleGridRow.value == "") {
        args.IsValid = true;
        txtProjectTitleGridRow.style["width"] = '98%';
        document.getElementById("<%= hdnFuzzyGridRowValidator.ClientID %>").value = "N";
    }
    else if (txtProjectTitleGridRow.value == "No results found") {
        args.IsValid = true;
        txtProjectTitleGridRow.value = "";
        txtProjectTitleGridRow.style["width"] = '98%';
        document.getElementById("<%= hdnFuzzyGridRowValidator.ClientID %>").value = "N";
    }
    else if (txtProjectTitleGridRow.value != "" && txtProjectTitleGridRow.value.indexOf('-') == -1) {
        args.IsValid = false;
        //adjust width of the textbox to display error
        txtProjectTitleGridRow.style["width"] = '88%';
        document.getElementById("<%= hdnFuzzyGridRowValidator.ClientID %>").value = "Y";

    }
    else if (args.IsValid == true) {
        txtProjectTitleGridRow.style["width"] = '98%';
        document.getElementById("<%= hdnFuzzyGridRowValidator.ClientID %>").value = "N";
    }


}

//reset field width when empty
function OntxtProjectTitleGridRowChange(sender) {
    gridRowIndex = sender.id.substring(sender.id.lastIndexOf("_") + 1);
    txtProjectTitleGridRow = document.getElementById(gridClientId + 'txtProjectTitleGridRow' + '_' + gridRowIndex);
    if (txtProjectTitleGridRow.value == "") {
        txtProjectTitleGridRow.style["width"] = '98%';
    }
}

//===========ProjectTitle Grid Row fuzzy search -- Ends



function CompareGridData(rowIndex) {
    var str = "ContentPlaceHolderBody_gvAutoParticipantDtls_";
    var hdnAutoParticipantId = document.getElementById(str + 'hdnAutoParticipantId' + '_' + rowIndex).value;
    var hdnMarketingOwner = document.getElementById(str + 'hdnMarketingOwner' + '_' + rowIndex).value;
    var txtMarketingOwnerGridRow = document.getElementById(str + 'txtMarketingOwnerGridRow' + '_' + rowIndex).value.trim().toUpperCase();
    var hdnWEASaleslabel = document.getElementById(str + 'hdnWEASaleslabel' + '_' + rowIndex).value;
    var txtWEASaleslabelGridRow = document.getElementById(str + 'txtWEASaleslabelGridRow' + '_' + rowIndex).value.trim().toUpperCase();
    var hdnArtist = document.getElementById(str + 'hdnArtist' + '_' + rowIndex).value.trim();
    var txtArtistGridRow = document.getElementById(str + 'txtArtistGridRow' + '_' + rowIndex).value.trim();
    var hdnProjectTitle = document.getElementById(str + 'hdnProjectTitle' + '_' + rowIndex).value.trim();
    var txtProjectTitleGridRow = document.getElementById(str + 'txtProjectTitleGridRow' + '_' + rowIndex).value.trim();
    var hdnChangeNotSaved = document.getElementById("<%= hdnChangeNotSaved.ClientID %>").value;
    if (hdnMarketingOwner != txtMarketingOwnerGridRow || hdnWEASaleslabel != txtWEASaleslabelGridRow
        || hdnArtist != txtArtistGridRow || hdnProjectTitle != txtProjectTitleGridRow) {
        //debugger;
        document.getElementById("<%= hdnChangeNotSaved.ClientID %>").value = "Y";

    }
    else {
        //debugger;
        document.getElementById("<%= hdnChangeNotSaved.ClientID %>").value = "N";
    }
}

function AddParticipantCheck() {
    var txtMarketingOwnerAddRow = document.getElementById("<%=txtMarketingOwnerAddRow.ClientID %>").value;
    var txtWEASalesLabelAddRow = document.getElementById("<%=txtWEASalesLabelAddRow.ClientID %>").value;
    var txtArtistAddRow = document.getElementById("<%=txtArtistAddRow.ClientID %>").value;
    var txtProjectTitleAddRow = document.getElementById("<%=txtProjectTitleAddRow.ClientID %>").value;

    if (txtMarketingOwnerAddRow == "" && txtWEASalesLabelAddRow == "" && txtArtistAddRow == "" && txtProjectTitleAddRow == "") {

        document.getElementById("<%= hdnAddRowDataNotSaved.ClientID %>").value = "N";
    }
    else {
        // debugger;
        document.getElementById("<%= hdnAddRowDataNotSaved.ClientID %>").value = "Y";
    }
}


function OnGridDataChange(row, name) {
    //debugger;
    if (name == "Artist") {
        OntxtArtistGridRowChange(row);
    }
    if (name == "Project") {
        OntxtProjectTitleGridRowChange(row);
    }
    //var selectedRowIndex = row.id.substring(row.id.length - 1);
    var selectedRowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
    CompareGridData(selectedRowIndex);
}

function UndoGridRowParticipant(row) {
    var selectedRowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);

    var hdnAutoParticipantId = document.getElementById(gridClientId + 'hdnAutoParticipantId' + '_' + selectedRowIndex).value;
    var hdnMarketingOwner = document.getElementById(gridClientId + 'hdnMarketingOwner' + '_' + selectedRowIndex).value;
    var txtMarketingOwnerGridRow = document.getElementById(gridClientId + 'txtMarketingOwnerGridRow' + '_' + selectedRowIndex);
    var hdnWEASaleslabel = document.getElementById(gridClientId + 'hdnWEASaleslabel' + '_' + selectedRowIndex).value;
    var txtWEASaleslabelGridRow = document.getElementById(gridClientId + 'txtWEASaleslabelGridRow' + '_' + selectedRowIndex);
    var hdnArtist = document.getElementById(gridClientId + 'hdnArtist' + '_' + selectedRowIndex).value;
    var txtArtistGridRow = document.getElementById(gridClientId + 'txtArtistGridRow' + '_' + selectedRowIndex);
    var hdnProjectTitle = document.getElementById(gridClientId + 'hdnProjectTitle' + '_' + selectedRowIndex).value;
    var txtProjectTitleGridRow = document.getElementById(gridClientId + 'txtProjectTitleGridRow' + '_' + selectedRowIndex);
    txtMarketingOwnerGridRow.value = hdnMarketingOwner;
    txtWEASaleslabelGridRow.value = hdnWEASaleslabel;
    txtArtistGridRow.value = hdnArtist;
    txtProjectTitleGridRow.value = hdnProjectTitle;
    ValidatorValidate(document.getElementById(gridClientId + 'valtxtArtistGridRow' + '_' + selectedRowIndex));
    ValidatorValidate(document.getElementById(gridClientId + 'ValtxtProjectTitleGridRow' + '_' + selectedRowIndex));
    if (txtArtistGridRow.value == "") {
        txtArtistGridRow.style["width"] = '98%';
    }
    if (txtProjectTitleGridRow.value == "") {
        txtProjectTitleGridRow.style["width"] = '98%';
    }
    //debugger;
    document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value = "N";
    document.getElementById("<%=hdnFuzzyGridRowValidator.ClientID %>").value = "N";
    document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value = "";

    return false;

}

function UndoAddRowParticipant() {
    document.getElementById("<%=txtMarketingOwnerAddRow.ClientID %>").value = "";
    document.getElementById("<%=txtWEASalesLabelAddRow.ClientID %>").value = "";
    document.getElementById("<%=txtArtistAddRow.ClientID %>").value = "";
    document.getElementById("<%=txtProjectTitleAddRow.ClientID %>").value = "";
    ValidatorValidate(document.getElementById("<%=cvArtistAddRow.ClientID %>"));
    ValidatorValidate(document.getElementById("<%=cvProjectTitleAddRow.ClientID %>"));
    OntxtArtistAddRowChange();
    OntxtProjectTitleAddRowChange();
    document.getElementById("<%=hdnAddRowDataNotSaved.ClientID %>").value = "N";
    document.getElementById("<%=hdnFuzzyAddRowValidator.ClientID %>").value = "N";
}



//Show warning while closing the window if changed data not saved 
function WarnOnUnSavedData() {
    var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
    var isDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
    var isDataChangedAddRow = document.getElementById("<%=hdnAddRowDataNotSaved.ClientID %>").value;
    var hdnRedirectToMaintanance = document.getElementById("<%=hdnRedirectToMaintanance.ClientID %>").value;
    if (isExceptionRaised != "Y" && (isDataChanged == "Y" || isDataChangedAddRow == "Y") && hdnRedirectToMaintanance == "N") {
        return warningMsgOnUnSavedData;
    }
}
window.onbeforeunload = WarnOnUnSavedData;

//redirect to Auto participant maint screen on saving data of new participant group
function RedirectToAutoParticipMaint(newAutoPartId) {
    document.getElementById("<%=hdnRedirectToMaintanance.ClientID %>").value = "Y";
    window.location = "../Participants/AutoParticipantMaintenance.aspx?autoPartId=" + newAutoPartId;
}
//open  Auto participant maint screen on double click
function RedirectToAutoParticipMaintOnDblClick(autoPartId) {
    if (IsDataChanged()) {
        window.onbeforeunload = null;
        OpenPopupOnUnSavedData("../Participants/AutoParticipantMaintenance.aspx?autoPartId=" + autoPartId);
    }
    else {
        return true;
    }
}

//redirect to Auto participant Audit 
function RedirectToAuditScreen(row) {
    var selectedRowIndex = row.id.substring(row.id.lastIndexOf("_") + 1);
    var autoPartId = document.getElementById(gridClientId + 'hdnAutoParticipantId' + '_' + selectedRowIndex).value;
    if (IsDataChanged()) {
        window.onbeforeunload = null;
        OpenPopupOnUnSavedData("../Audit/AutoParticipantAudit.aspx?autoPartId=" + autoPartId);
    }
    else {
        window.location = "../Audit/AutoParticipantAudit.aspx?autoPartId=" + autoPartId;
    }

}


function ValidateChanges() {
    if (!(WarnOnUnSavedData.length > 0)) {
        eval(this.href);
    }
}
//used to check if any changes to allow navigation to other screen 
function IsDataChanged() {
    var hdnGridRowSelectedPrvious = document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value;
    if (hdnGridRowSelectedPrvious != "") {
        CompareGridData(hdnGridRowSelectedPrvious);
    }
    var isDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
    var isDataChangedAddRow = document.getElementById("<%=hdnAddRowDataNotSaved.ClientID %>").value;

    if ((isDataChanged == "Y" || isDataChangedAddRow == "Y")) {
        return true;
    }
    else {
        return false;
    }
}


//Validation: warning message if changes made and not saved

function OnGridRowSelected(row) {
    var rowData = row.parentNode.parentNode;
    var rowIndex = rowData.rowIndex - 1;
    if (document.getElementById("<%=hdnAddRowDataNotSaved.ClientID %>").value == "Y") {
        var popup = $find('<%= mpeSaveUndo.ClientID %>');
        if (popup != null) {
            document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new Particiaption group. Save or Undo changes";
            popup.show();
            $get("<%=btnUndoChanges.ClientID%>").focus();
        }
    }
    else {
        if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value == "") {
            document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").innerText = rowIndex;
        }
        else if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value != rowIndex) {
            if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
                var popup = $find('<%= mpeSaveUndo.ClientID %>');
                //  debugger;
                if (popup != null) {
                    popup.show();
                    $get("<%=btnUndoChanges.ClientID%>").focus();
                }
            }
            else {
                document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").innerText = rowIndex;
            }
        }
}

    return false;
}


function ConfirmSearch() {
    if (document.getElementById("<%=hdnAddRowDataNotSaved.ClientID %>").value == "Y") {
        var popup = $find('<%= mpeSaveUndo.ClientID %>');
        if (popup != null) {
            document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new Particiaption group. Save or Undo changes";
            popup.show();
            $get("<%=btnUndoChanges.ClientID%>").focus();
            return false;
        }
    }
    else {
        if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
            var popup = $find('<%= mpeSaveUndo.ClientID %>');
            if (popup != null) {
                popup.show();
                $get("<%=btnUndoChanges.ClientID%>").focus();
            }
            return false;
        }
        else {
            return true;
        }
    }
}

function ConfirmInsert() {
    var hdnGridRowSelectedPrvious = document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value;
    if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
        var popup = $find('<%= mpeSaveUndo.ClientID %>');
        if (popup != null) {
            popup.show();
            $get("<%=btnUndoChanges.ClientID%>").focus();
        }
        return false;
    }
    else {
        return true;
    }
}

function ConfirmUpdate(row) {
    var rowData = row.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
    var rowIndex = rowData.rowIndex - 1;
    var hdnGridRowSelectedPrvious = document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value;

    if (document.getElementById("<%=hdnAddRowDataNotSaved.ClientID %>").value == "Y") {
        var popup = $find('<%= mpeSaveUndo.ClientID %>');
        if (popup != null) {
            document.getElementById("<%=lblMessage.ClientID %>").innerText = "Looks like you were trying to create a new Particiaption group. Save or Undo changes";
            popup.show();
            $get("<%=btnUndoChanges.ClientID%>").focus();
            return false;
        }
    }
    else {
        if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value == "") {
            return true;
        }
        else if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value != rowIndex) {
            if (document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value == "Y") {
                var popup = $find('<%= mpeSaveUndo.ClientID %>');
                if (popup != null) {
                    popup.show();
                    $get("<%=btnUndoChanges.ClientID%>").focus();
                }
                return false;
            }
            else {
                return true;
            }
        }
        else if (document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value == rowIndex) {
            return true;
        }
}
}

function ValidateSaveParticipant(id) {
    var hdnAddRowDataNotSaved = document.getElementById("<%=hdnAddRowDataNotSaved.ClientID %>").value;
    var hdnChangeNotSaved = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
    var hdnGridRowSelectedPrvious = document.getElementById("<%=hdnGridRowSelectedPrvious.ClientID %>").value;
    var controlId = id;
    if (hdnChangeNotSaved == "Y") {

        var rowIndex = hdnGridRowSelectedPrvious;
        var str = "ContentPlaceHolderBody_gvAutoParticipantDtls_";
        var hdnAutoParticipantId = document.getElementById(str + 'hdnAutoParticipantId' + '_' + rowIndex).value;
        var txtMarketingOwnerGridRow = document.getElementById(str + 'txtMarketingOwnerGridRow' + '_' + rowIndex).value.trim();
        var txtWEASaleslabelGridRow = document.getElementById(str + 'txtWEASaleslabelGridRow' + '_' + rowIndex).value.trim();
        var txtArtistGridRow = document.getElementById(str + 'txtArtistGridRow' + '_' + rowIndex).value.trim();
        var txtProjectTitleGridRow = document.getElementById(str + 'txtProjectTitleGridRow' + '_' + rowIndex).value.trim();
        var hdnFuzzyGridRowValidator = document.getElementById("<%=hdnFuzzyGridRowValidator.ClientID %>").value;
        var hdnIsValidArtistGridRow = document.getElementById("<%=hdnIsValidArtistGridRow.ClientID %>").value;
        var hdnIsValidProjectGridRow = document.getElementById("<%=hdnIsValidProjectGridRow.ClientID %>").value;
        var popup = $find('<%= mpeSaveUndo.ClientID %>');
        if (popup != null) {
            popup.hide();
        }
        if (hdnFuzzyGridRowValidator == "Y") {
            DisplayMessagePopup("Invalid data entered.Please correct.");
        }
        else if (txtArtistGridRow != "" && hdnIsValidArtistGridRow != "Y") {

            DisplayMessagePopup("Please select valid artist from list.");
        }
        else if (txtProjectTitleGridRow != "" && hdnIsValidProjectGridRow != "Y") {

            DisplayMessagePopup("Please select valid project from list.");
        }
        else if (txtMarketingOwnerGridRow == "" && txtWEASaleslabelGridRow == "" && txtArtistGridRow == "" && txtProjectTitleGridRow == "") {
            DisplayMessagePopup("Atleast one field is mandatory to create Participation Group.");
            return false;
        }
        else {
            return true;
        }
    }
    else if (hdnChangeNotSaved == "N" && controlId.indexOf('imgBtnSave') != -1) {
        DisplayMessagePopup("No changes made to save");
        return false;
    }
    else if (hdnAddRowDataNotSaved == "Y") {
        var txtMarketingOwnerAddRow = document.getElementById("<%=txtMarketingOwnerAddRow.ClientID %>").value.trim();
        var txtWEASaleslabelAddRow = document.getElementById("<%=txtWEASalesLabelAddRow.ClientID %>").value.trim();
        var txtArtistAddRow = document.getElementById("<%=txtArtistAddRow.ClientID %>").value.trim();
        var txtProjectTitleAddRow = document.getElementById("<%=txtProjectTitleAddRow.ClientID %>").value.trim();
        var hdnFuzzyAddRowValidator = document.getElementById("<%=hdnFuzzyAddRowValidator.ClientID %>").value;
        var hdnIsValidArtist = document.getElementById("<%=hdnIsValidArtist.ClientID %>").value;
        var hdnIsValidProject = document.getElementById("<%=hdnIsValidProject.ClientID %>").value;
        var popup = $find('<%= mpeSaveUndo.ClientID %>');
        if (popup != null) {
            popup.hide();
        }
        if (hdnFuzzyAddRowValidator == "Y") {
            DisplayMessagePopup("Invalid data entered.Please correct.");
        }
        else if (txtArtistAddRow != "" && hdnIsValidArtist != "Y") {
            DisplayMessagePopup("Please select valid artist from list.");
        }
        else if (txtProjectTitleAddRow != "" && hdnIsValidProject != "Y") {
            DisplayMessagePopup("Please select valid project from list.");
        }
        else if (txtMarketingOwnerAddRow == "" && txtWEASaleslabelAddRow == "" && txtArtistAddRow == "" && txtProjectTitleAddRow == "") {
            DisplayMessagePopup("Atleast one field is mandatory to create Participation Group.");
            return false;
        }
        else {
            return true;
        }
    }
    else if (hdnChangeNotSaved == "N" && controlId.indexOf('imgBtnAddParticipant') != -1) {
        DisplayMessagePopup("Atleast one field is mandatory to create Participation Group.");
        return false;
    }
    else { return true; }

}




    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>

            <ajaxToolkit:ModalPopupExtender ID="mPopupPageLevel" runat="server" PopupControlID="progressBarPageLevel" TargetControlID="progressBarPageLevel"
                BackgroundCssClass="progressBar" RepositionMode="RepositionOnWindowResize">
            </ajaxToolkit:ModalPopupExtender>
            <asp:UpdateProgress ID="progressBarPageLevel" runat="server" AssociatedUpdatePanelID="updPnlPageLevel" DisplayAfter="100">
                <ProgressTemplate>
                    <div id="Search" style="font-weight: bold; color: Black; z-index: 2">
                        <img src="../Images/InProgress2.gif" alt="" />
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>


            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="9">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    AUTO PARTICIPANTS SEARCH
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="9">
                        <table width="100%">
                            <tr>
                                <td width="1%"></td>
                                <td width="10%" class="identifierLable_large_bold">Marketing Owner</td>
                                <td width="15%">
                                    <asp:TextBox ID="txtMarketingOwner" runat="server" Width="90%" CssClass="textboxStyle"
                                        TabIndex="100" onfocus="return ConfirmSearch();" MaxLength="50" onkeydown="SearchByEnterKey();"></asp:TextBox>
                                </td>
                                <td width="10%"></td>
                                <td width="10%" class="identifierLable_large_bold">WEA Sales Label</td>
                                <td width="15%" align="left">
                                    <asp:TextBox ID="txtWEASalesLabel" runat="server" Width="90%" CssClass="textboxStyle"
                                        TabIndex="101" onfocus="return ConfirmSearch();" MaxLength="50" onkeydown="SearchByEnterKey();"></asp:TextBox>
                                </td>
                                <td></td>
                                <td width="30%" align="right" rowspan="5" valign="top">
                                    <table width="80%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td style="padding-bottom: 5px">
                                                <asp:Button ID="btnSearch" runat="server" CssClass="ButtonStyle" Text="Search" onfocus="return ConfirmSearch();" OnClick="btnSearch_Click" UseSubmitBehavior="false" Width="50%" />

                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-bottom: 5px">
                                                <asp:Button ID="btnReset" runat="server" CssClass="ButtonStyle" Text="Reset" onfocus="return ConfirmSearch();" OnClick="btnReset_Click" UseSubmitBehavior="false" Width="50%" />

                                            </td>
                                        </tr>
                                    </table>
                                </td>

                            </tr>
                            <tr>
                                <td width="1%"></td>
                                <td width="10%" class="identifierLable_large_bold">Artist</td>
                                <td width="15%">
                                    <asp:TextBox ID="txtArtist" runat="server" Width="90%" CssClass="textboxStyle" TabIndex="103" onfocus="return ConfirmSearch();" onkeydown="SearchByEnterKey();"></asp:TextBox>
                                </td>
                                <td width="10%"></td>
                                <td width="10%" class="identifierLable_large_bold">Project Title</td>
                                <td width="15%" align="left">
                                    <asp:TextBox ID="txtProject" runat="server" Width="90%" CssClass="textboxStyle" TabIndex="104"
                                        onfocus="return ConfirmSearch();" onkeydown="SearchByEnterKey();"></asp:TextBox>
                                </td>
                                <td></td>


                            </tr>
                            <tr>
                                <td colspan="7">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td width="1%"></td>
                                <td colspan="6" class="table_header_with_border">Participation Group</td>

                            </tr>

                            <tr>
                                <td width="1%"></td>
                                <td colspan="6">
                                    <table width="89%" cellpadding="0" cellspacing="0" class="table_with_border">
                                        <tr>

                                            <td>
                                                <br />

                                            </td>
                                        </tr>
                                        <tr>

                                            <td>
                                                <table width="98%" cellpadding="0" cellspacing="0" align="center">
                                                    <tr>
                                                        <td>
                                                            <asp:Panel ID="pnlAutoParticipantDtls" runat="server" ScrollBars="Auto" Width="100%">
                                                                <table width="98.14%" cellpadding="0" cellspacing="0">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:GridView ID="gvAutoParticipantDtls" runat="server" AutoGenerateColumns="False" Width="100%" AlternatingRowStyle-BackColor="#E3EFFF"
                                                                                CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                                                EmptyDataText="No data found." RowStyle-CssClass="dataRow" OnRowCommand="gvAutoParticipantDtls_RowCommand"
                                                                                 OnRowDataBound="gvAutoParticipantDtls_RowDataBound" AllowSorting="true" OnSorting="gvAutoParticipantDtls_Sorting" HeaderStyle-CssClass="FixedHeader">

                                                                                <Columns>
                                                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Marketing Owner" SortExpression="marketing_owner">
                                                                                        <ItemTemplate>
                                                                                            <asp:HiddenField ID="hdnAutoParticipantId" runat="server" Value='<%# Bind("auto_participant_id") %>'></asp:HiddenField>
                                                                                            <asp:HiddenField ID="hdnMarketingOwner" runat="server" Value='<%# Bind("marketing_owner") %>'></asp:HiddenField>
                                                                                            <asp:TextBox ID="txtMarketingOwnerGridRow" runat="server" Text='<%# Eval("marketing_owner") %>' MaxLength="50"
                                                                                                CssClass="gridTextField" Width="90%" onchange="javascript: OnGridDataChange(this,'marketing');" onfocus="return OnGridRowSelected(this)" Style="text-transform: uppercase;"></asp:TextBox>
                                                                                        </ItemTemplate>
                                                                                        <ItemStyle Width="15%" />
                                                                                    </asp:TemplateField>
                                                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="WEA Sales Label" SortExpression="wea_sales_label">
                                                                                        <ItemTemplate>
                                                                                            <asp:HiddenField ID="hdnWEASaleslabel" runat="server" Value='<%# Bind("wea_sales_label") %>'></asp:HiddenField>
                                                                                            <asp:TextBox ID="txtWEASaleslabelGridRow" runat="server" Text='<%# Eval("wea_sales_label") %>'
                                                                                                CssClass="gridTextField" Width="90%" onchange="javascript:  OnGridDataChange(this,'wea');" MaxLength="50" onfocus="OnGridRowSelected(this)" Style="text-transform: uppercase;"></asp:TextBox>
                                                                                        </ItemTemplate>
                                                                                        <ItemStyle Width="15%" />
                                                                                    </asp:TemplateField>
                                                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Artist" SortExpression="artist_name">
                                                                                        <ItemTemplate>
                                                                                            <asp:HiddenField ID="hdnArtist" runat="server" Value='<%# Bind("artist_name") %>'></asp:HiddenField>

                                                                                            <asp:TextBox ID="txtArtistGridRow" runat="server" Width="98%" Text='<%#Bind("artist_name")%>' CssClass="textbox_FuzzySearch"
                                                                                                ToolTip='<%#Bind("artist_name")%>' onkeydown="OntxtArtistGridRowKeyDown(this);" onchange="OnGridDataChange(this,'Artist');"
                                                                                                onfocus="OnGridRowSelected(this)" onKeyPress="javascript: OnGridDataChange(this,'Artist');"></asp:TextBox>

                                                                                            <ajaxToolkit:AutoCompleteExtender ID="aceArtistGridRow" runat="server"
                                                                                                ServiceMethod="FuzzySearchAllArtisList"
                                                                                                ServicePath="~/Services/FuzzySearch.asmx"
                                                                                                MinimumPrefixLength="1"
                                                                                                CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                                                                TargetControlID="txtArtistGridRow"
                                                                                                FirstRowSelected="true"
                                                                                                OnClientPopulating="ArtistGridRowListPopulating"
                                                                                                OnClientPopulated="ArtistGridRowListPopulated"
                                                                                                OnClientHidden="ArtistGridRowListHidden"
                                                                                                OnClientItemSelected="ArtistGridRowListItemSelected"
                                                                                                CompletionListElementID="pnlArtistGridRowFuzzySearch" />
                                                                                            <asp:Panel ID="pnlArtistGridRowFuzzySearch" runat="server" CssClass="identifierLable" />
                                                                                            <asp:CustomValidator ID="valtxtArtistGridRow" runat="server" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>' CssClass="requiredFieldValidator"
                                                                                                ClientValidationFunction="ValArtistGridRow" ToolTip="Please select valid Artist from the search list"
                                                                                                ControlToValidate="txtArtistGridRow" ErrorMessage="*" Display="Dynamic" EnableClientScript="true"></asp:CustomValidator>
                                                                                        </ItemTemplate>
                                                                                        <ItemStyle Width="30%" />
                                                                                    </asp:TemplateField>
                                                                                    <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Project Title" SortExpression="project_title">
                                                                                        <ItemTemplate>
                                                                                            <asp:HiddenField ID="hdnProjectTitle" runat="server" Value='<%# Bind("project_title") %>'></asp:HiddenField>

                                                                                            <asp:TextBox ID="txtProjectTitleGridRow" runat="server" Width="98%" Text='<%#Bind("project_title")%>' CssClass="textbox_FuzzySearch"
                                                                                                ToolTip='<%#Bind("project_title")%>' onkeydown="OntxtProjectTitleGridRowKeyDown(this);" onchange="OnGridDataChange(this,'Project');" onfocus="OnGridRowSelected(this)"></asp:TextBox>

                                                                                            <ajaxToolkit:AutoCompleteExtender ID="aceProjectTitleGridRow" runat="server"
                                                                                                ServiceMethod="FuzzySearchAllProjectList"
                                                                                                ServicePath="~/Services/FuzzySearch.asmx"
                                                                                                MinimumPrefixLength="1"
                                                                                                CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                                                                TargetControlID="txtProjectTitleGridRow"
                                                                                                FirstRowSelected="true"
                                                                                                OnClientPopulating="ProjectTitleGridRowListPopulating"
                                                                                                OnClientPopulated="ProjectTitleGridRowListPopulated"
                                                                                                OnClientHidden="ProjectTitleGridRowListHidden"
                                                                                                OnClientItemSelected="ProjectTitleGridRowListItemSelected"
                                                                                                CompletionListElementID="pnlProjectTitleGridRowFuzzySearch" />
                                                                                            <asp:Panel ID="pnlProjectTitleGridRowFuzzySearch" runat="server" CssClass="identifierLable" />
                                                                                            <asp:CustomValidator ID="valtxtProjectTitleGridRow" runat="server" CssClass="requiredFieldValidator"
                                                                                                ClientValidationFunction="ValProjectTitleGridRow" ToolTip="Please select valid ProjectTitle from the search list"
                                                                                                ControlToValidate="txtProjectTitleGridRow" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>' ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>
                                                                                        </ItemTemplate>
                                                                                        <ItemStyle Width="33%" />
                                                                                    </asp:TemplateField>
                                                                                    <asp:TemplateField>
                                                                                        <ItemTemplate>
                                                                                            <asp:ImageButton ID="imgBtnDblClk" runat="server" CommandName="dblClk" Text="dblClick" CausesValidation="false"></asp:ImageButton>
                                                                                        </ItemTemplate>
                                                                                        <ItemStyle CssClass="hide" />
                                                                                    </asp:TemplateField>
                                                                                    <asp:TemplateField ItemStyle-Width="7%" ItemStyle-CssClass="gridItemStyle_Right_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                                        <ItemTemplate>
                                                                                            <table width="60%" align="center">
                                                                                                <tr style="float: right">
                                                                                                    <td align="right" style="float: right" width="50%">
                                                                                                        <asp:ImageButton ID="imgBtnSave" runat="server" CommandName="saverow" ImageUrl="../Images/save.png" ToolTip="Save" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>' onfocus="return ConfirmUpdate(this);" OnClientClick="if (!ValidateSaveParticipant(this.id)) { return false;};" />
                                                                                                    </td>
                                                                                                    <td align="right" style="float: right" width="50%">
                                                                                                        <asp:ImageButton ID="imgBtnUndo" runat="server" CausesValidation="false" ImageUrl="../Images/cancel_row3.png" ToolTip="Cancel" OnClientClick="return UndoGridRowParticipant(this);" onfocus="return ConfirmUpdate(this);" />
                                                                                                    </td>
                                                                                                    <td align="right" style="float: right" width="50%">
                                                                                                        <asp:ImageButton ID="imgAudit" runat="server" CommandName="audit" ImageUrl="../Images/audit.png" ToolTip="Audit" ValidationGroup='<%# "GroupUpdate_" + Container.DataItemIndex %>' onfocus="return ConfirmUpdate(this);" OnClientClick="if (!RedirectToAuditScreen(this)) { return false;};" />
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </ItemTemplate>
                                                                                    </asp:TemplateField>
                                                                                </Columns>
                                                                            </asp:GridView>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </asp:Panel>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <div align="center">
                                                                <asp:Repeater ID="rptPager" runat="server">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>'
                                                                            ClientIDMode="AutoID" CausesValidation="false" Enabled='<%# Eval("Enabled") %>' OnClick="lnkPage_Click" CssClass="gridPager" OnClientClick="return ValidateChanges();" onfocus="return ConfirmSearch();"> </asp:LinkButton>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <table width="98.14%" cellpadding="0" cellspacing="0">
                                                                <tr>
                                                                    <td width="15%" class="gridHeaderStyle_1row">Marketing Owner</td>
                                                                    <td width="15%" class="gridHeaderStyle_1row">WEA Sales Label</td>
                                                                    <td width="30%" class="gridHeaderStyle_1row">Artist</td>
                                                                    <td width="33%" class="gridHeaderStyle_1row">Project Title</td>
                                                                    <td width="7%" class="gridHeaderStyle_1row">&nbsp</td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="insertBoxStyle">
                                                                        <asp:TextBox ID="txtMarketingOwnerAddRow" runat="server" CssClass="textboxStyle" TabIndex="105" Width="95%" Style="text-transform: uppercase;"
                                                                            onfocus="return ConfirmInsert();" MaxLength="50"
                                                                            onchange="javascript: AddParticipantCheck();" onKeyPress="javascript: AddParticipantCheck();" onkeydown="javascript: AddParticipantCheck();"></asp:TextBox>

                                                                    </td>
                                                                    <td class="insertBoxStyle">
                                                                        <asp:TextBox ID="txtWEASalesLabelAddRow" runat="server" CssClass="textboxStyle" TabIndex="106" Width="95%" Style="text-transform: uppercase;"
                                                                            onfocus="return ConfirmInsert();" MaxLength="50"
                                                                            onchange="javascript: AddParticipantCheck();" onKeyPress="javascript: AddParticipantCheck();" onkeydown="javascript: AddParticipantCheck();"></asp:TextBox>
                                                                    </td>
                                                                    <td class="insertBoxStyle">
                                                                        <asp:TextBox ID="txtArtistAddRow" runat="server" Width="98%" CssClass="textboxStyle" TabIndex="107"
                                                                            onkeydown="OntxtArtistAddRowKeyDown(this);" onchange="OntxtArtistAddRowChange();"
                                                                            onfocus="return ConfirmInsert();" onKeyPress="javascript: AddParticipantCheck();"></asp:TextBox>
                                                                        <ajaxToolkit:AutoCompleteExtender ID="aceArtistAddRow" runat="server"
                                                                            ServiceMethod="FuzzySearchAllArtisList"
                                                                            ServicePath="~/Services/FuzzySearch.asmx"
                                                                            MinimumPrefixLength="1"
                                                                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                                            TargetControlID="txtArtistAddRow"
                                                                            FirstRowSelected="true"
                                                                            OnClientPopulating="ArtistAddRowListPopulating"
                                                                            OnClientPopulated="ArtistAddRowListPopulated"
                                                                            OnClientHidden="ArtistAddRowListHidden"
                                                                            OnClientItemSelected="ArtistAddRowListItemSelected"
                                                                            CompletionListElementID="pnlArtistAddRowFuzzySearch" />
                                                                        <asp:Panel ID="pnlArtistAddRowFuzzySearch" runat="server" CssClass="identifierLable" Style="bottom: 0px; top: 1000px; position: absolute; height: 200px;" />
                                                                        <asp:CustomValidator ID="cvArtistAddRow" runat="server" CssClass="requiredFieldValidator"
                                                                            ClientValidationFunction="ValArtistAddRow" ToolTip="Please select valid Artist from the search list"
                                                                            ControlToValidate="txtArtistAddRow" ValidationGroup="valSavePartGroup" ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>

                                                                    </td>
                                                                    <td class="insertBoxStyle">
                                                                        <asp:TextBox ID="txtProjectTitleAddRow" runat="server" Width="98%" CssClass="textboxStyle" TabIndex="108"
                                                                            onkeydown="OntxtProjectTitleAddRowKeyDown(this);" onchange="OntxtProjectTitleAddRowChange();"
                                                                            onfocus="return ConfirmInsert();" onKeyPress="javascript: AddParticipantCheck();"></asp:TextBox>
                                                                        <ajaxToolkit:AutoCompleteExtender ID="aceProjectTitleAddRow" runat="server"
                                                                            ServiceMethod="FuzzySearchAllProjectList"
                                                                            ServicePath="~/Services/FuzzySearch.asmx"
                                                                            MinimumPrefixLength="1"
                                                                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                                            TargetControlID="txtProjectTitleAddRow"
                                                                            FirstRowSelected="true"
                                                                            OnClientPopulating="ProjectTitleAddRowListPopulating"
                                                                            OnClientPopulated="ProjectTitleAddRowListPopulated"
                                                                            OnClientHidden="ProjectTitleAddRowListHidden"
                                                                            OnClientItemSelected="ProjectTitleAddRowListItemSelected"
                                                                            CompletionListElementID="pnlProjectTitleAddRowFuzzySearch" />
                                                                        <asp:Panel ID="pnlProjectTitleAddRowFuzzySearch" runat="server" CssClass="identifierLable" Style="bottom: 0px; top: 1000px; position: absolute; height: 200px;" />
                                                                        <asp:CustomValidator ID="cvProjectTitleAddRow" runat="server" CssClass="requiredFieldValidator"
                                                                            ClientValidationFunction="ValProjectTitleAddRow" ToolTip="Please select valid Project Title from the search list"
                                                                            ControlToValidate="txtProjectTitleAddRow" ValidationGroup="valSavePartGroup" ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>
                                                                    </td>

                                                                    <td class="insertBoxStyle_No_Padding">
                                                                        <table width="60%" align="center">
                                                                            <tr style="float: right">
                                                                                <td align="right" style="float: right" width="50%">
                                                                                    <asp:ImageButton ID="imgBtnAddParticipant" runat="server" CommandName="saverow" TabIndex="109" ImageUrl="../Images/save.png" OnClientClick="if (!ValidateSaveParticipant(this.id)) { return false;};" onfocus="return ConfirmInsert();" OnClick="imgBtnAddParticipant_Click" ValidationGroup="valSavePartGroup" />
                                                                                </td>
                                                                                <td align="right" style="float: right" width="50%">
                                                                                    <asp:ImageButton ID="imgBtnAddRowUndo" runat="server" CommandName="cancelrow" TabIndex="110" ImageUrl="../Images/cancel_row3.png"
                                                                                        ToolTip="Cancel" OnKeyDown="OnTabPress()" OnClientClick="UndoAddRowParticipant();  return false;" onfocus="return ConfirmInsert();" />
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

                                </td>
                            </tr>
                        </table>

                    </td>


                </tr>


            </table>
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
            <%--Fuzzy search pop up - Ends --%>

            <%--Save/Undo changes popup--%>
            <asp:Button ID="dummySaveUndo" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeSaveUndo" runat="server" PopupControlID="pnlSaveUndo" TargetControlID="dummySaveUndo"
                CancelControlID="btnClosePopupSaveUndo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlSaveUndo" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td align="right" style="vertical-align: top;">
                            <asp:ImageButton ID="btnClosePopupSaveUndo" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMessage" runat="server" CssClass="identifierLable"
                                Text="You have made changes which are not saved. Save or Undo changes"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnSaveChanges" runat="server" Text="Save" CssClass="ButtonStyle"
                                            OnClick="btnSaveChanges_Click" OnClientClick="if (!ValidateSaveParticipant(this.id)) { return false;};" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnUndoChanges" runat="server" Text="Undo" CssClass="ButtonStyle" OnClick="btnUndoChanges_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnPageNumber" runat="server" Value="" />
            <asp:HiddenField ID="hdnFuzzySearchText" runat="server" />
            <asp:HiddenField ID="hdnGridFuzzySearchRowId" runat="server" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" Value="" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnChangeNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnAddRowDataNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridRowSelectedPrvious" runat="server" />
            <asp:HiddenField ID="hdnFuzzyGridRowValidator" runat="server" Value="N" />
            <asp:HiddenField ID="hdnFuzzyAddRowValidator" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsValidArtist" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnIsValidProject" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnIsValidArtistGridRow" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnIsValidProjectGridRow" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnRedirectToMaintanance" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsNewRequest" runat="server" Value="Y" />

            <asp:Button ID="btnFuzzyArtistListPopup" runat="server" Style="display: none;" OnClick="btnFuzzyArtistListPopup_Click" CausesValidation="false" />
            <asp:Button ID="btnFuzzyProjectTitleListPopup" runat="server" Style="display: none;" OnClick="btnFuzzyProjectTitleListPopup_Click" CausesValidation="false" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99"></asp:Label>
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>

        </ContentTemplate>
    </asp:UpdatePanel>



</asp:Content>

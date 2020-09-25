<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CatalogueNotes.aspx.cs" Inherits="WARS.Participants.CatalogueNotes" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Catalogue Notes " MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>
<%@ Register TagPrefix="ContNav" TagName="ContractNavigation" Src="~/Contract/ContractNavigationButtons.ascx" %>
<%@ Register TagPrefix="ContHdrNav" TagName="ContractHdrNavigation" Src="~/Contract/ContractHdrLinkButtons.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">
        //to open Contract Maintenance screen in same tab
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

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
             }
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
        //======================= End

        //grid panel height adjustment functioanlity - starts
        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.55;
            document.getElementById("<%=tbNotes.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=iFrameNotes.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
        }
        //======================= End

        function SaveChanges() {
            var hdnIsNewCatNo = document.getElementById("<%=hdnIsNewCatNo.ClientID %>").value;
            if (hdnIsNewCatNo != "Y" && IsDataChanged() == false) {
                DisplayMessagePopup("No changes made to save!");
                return false;
            }
            var iframe = document.getElementById('iframe');
            var innerDoc = iframe.contentDocument || iframe.contentWindow.document;
            var txtNotes = innerDoc.getElementById('txtNotes');
            document.getElementById("<%= txtHidData.ClientID %>").value = txtNotes.value;
            return true;
        }

        function PopulateCatNo() {
            var iframe = document.getElementById('iframe');
            var innerDoc = iframe.contentDocument || iframe.contentWindow.document;
            var hdnCatNo = innerDoc.getElementById('hdnCatNo');
            var txtNotes = innerDoc.getElementById('txtNotes');
            document.getElementById("<%= txtCatNo.ClientID %>").value = hdnCatNo.value;
        }

        function SetDataChanged() {
            document.getElementById("<%=hdnIsDataChanged.ClientID %>").value = "Y";
        }

        //Validate any unsaved data on browser window close/refresh
        function RedirectToErrorPage() {
            document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
            window.location = "../Common/ExceptionPage.aspx";
        }

        function WarnOnUnSavedData() {
            var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
            var isNewCatNoSaved = document.getElementById("<%=hdnNewCatNoSaved.ClientID %>").value;
            if (isExceptionRaised != "Y" && isNewCatNoSaved != "Y") {
                if (IsDataChanged()) {
                    return warningMsgOnUnSavedData;
                }
            }
        }
        window.onbeforeunload = WarnOnUnSavedData;

        function IsDataChanged() {
            if (document.getElementById("<%=hdnIsDataChanged.ClientID %>").value == "Y") {
                return true;
            }
            else {
                return false;
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
                                    CATALOGUE - NOTES
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="4"></td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td valign="top">
                        <table width="99%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="15%" class="identifierLable_large_bold">Current CatalogueNo</td>
                                <td>
                                    <asp:TextBox ID="txtCatNo" runat="server" Width="25%" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" TabIndex="100"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td width="1%"></td>
                    <td width="15%" rowspan="4" valign="top" align="right">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td width="30%"></td>
                                            <td align="right" width="70%">
                                                <asp:Button ID="btnSave" runat="server" CssClass="ButtonStyle" OnClientClick="if (!SaveChanges()) { return false;};" OnClick="btnSave_Click"
                                                    Text="Save Changes" UseSubmitBehavior="false" Width="90%" TabIndex="101"/>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    <br />
                                    <br />
                                    <br />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td class="table_header_with_border" valign="top">Notes</td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td valign="top">
                        <table width="100%" class="table_with_border" runat="server" id="tbNotes">
                            <tr>
                                <td valign="top" width="99%">
                                    <input type="hidden" id="txtHidData" runat="server" />
                                    <iframe id="iFrameNotes" name="iframe" runat="server" src="../Participants/CatalogueNotesRichTextBox.aspx" width="100%" frameborder="0"></iframe>
                                </td>
                            </tr>
                            <tr>
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


            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnIsDataChanged" runat="server" Value="N" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnNewCatNoSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsNewCatNo" runat="server" Value="N" />
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField" onkeydown="FocusLblKeyPress();"></asp:TextBox>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WARSAffiliates.aspx.cs" Inherits="WARS.WARSAffiliates" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Menu Options" MaintainScrollPositionOnPostback="true" ClientIDMode="AutoID" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderBody">
    <script type="text/javascript">

        //debugger;
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

        function tdWMUKOn() {
            var hovermenu = $find('<%= hmeWMUK.ClientID %>');
            if (hovermenu != null) {
                hovermenu._hoverBehavior._hoverElement.style.visibility = "visible";
            }
        }

        function tdWMUKOut() {
            var hovermenu = $find('<%= hmeWMUK.ClientID %>');
            if (hovermenu != null) {
                hovermenu._hoverBehavior._hoverElement.style.visibility = "hidden";
                document.getElementById('<%= tdWMUK.ClientID %>').className = 'LandingScrn_MenuItem';
            }
        }

        function BeneluxClick() { document.getElementById('<%= btnBenelux.ClientID %>').click(); }
        function CentralEuropeClick() { document.getElementById('<%= btnCentralEurope.ClientID %>').click(); }
        function CzechRepublicClick() { document.getElementById('<%= btnCzechRepublic.ClientID %>').click(); }
        function DenmarkClick() { document.getElementById('<%= btnDenmark.ClientID %>').click(); }
        function FinlandClick() { document.getElementById('<%= btnFinland.ClientID %>').click(); }
        function ItalyClick() { document.getElementById('<%= btnItaly.ClientID %>').click(); }
        function MiddleEastClick() { document.getElementById('<%= btnMiddleEast.ClientID %>').click(); }
        function NorwayClick() { document.getElementById('<%= btnNorway.ClientID %>').click(); }
        function PolandClick() { document.getElementById('<%= btnPoland.ClientID %>').click(); }
        function PortugalClick() { document.getElementById('<%= btnPortugal.ClientID %>').click(); }
        function RussiaClick() { document.getElementById('<%= btnRussia.ClientID %>').click(); }
        function SouthAfricaClick() { document.getElementById('<%= btnSouthAfrica.ClientID %>').click(); }
        function SpainClick() { document.getElementById('<%= btnSpain.ClientID %>').click(); }
        function SwedenClick() { document.getElementById('<%= btnSweden.ClientID %>').click(); }
        function UKADAClick() { document.getElementById('<%= btnUKADA.ClientID %>').click(); }
        function UKERATOClick() { document.getElementById('<%= btnUKERATO.ClientID %>').click(); }
        function UKGBClick() { document.getElementById('<%= btnUKGB.ClientID %>').click(); }
        function UKNVCClick() { document.getElementById('<%= btnUKNVC.ClientID %>').click(); }
        function UKPLGClick() { document.getElementById('<%= btnUKPLG.ClientID %>').click(); }
        function UKTELDECClick() { document.getElementById('<%= btnUKTELDEC.ClientID %>').click(); }
        function UKWEAClick() { document.getElementById('<%= btnUKWEA.ClientID %>').click(); }
        function AustraliaNZClick() { document.getElementById('<%= btnAustraliaNZ.ClientID %>').click(); }
        function ChinaClick() { document.getElementById('<%= btnChina.ClientID %>').click(); }
        function HongKkongClick() { document.getElementById('<%= btnHongKong.ClientID %>').click(); }
        function IndonesiaClick() { document.getElementById('<%= btnIndonesia.ClientID %>').click(); }
        function JapanClick() { document.getElementById('<%= btnJapan.ClientID %>').click(); }
        function KoreaClick() { document.getElementById('<%= btnKorea.ClientID %>').click(); }
        function MalaysiaClick() { document.getElementById('<%= btnMalaysia.ClientID %>').click(); }
        function PhilippinesClick() { document.getElementById('<%= btnPhilippines.ClientID %>').click(); }
        function SingaporeClick() { document.getElementById('<%= btnSingapore.ClientID %>').click(); }
        function TaiwanClick() { document.getElementById('<%= btnTaiwan.ClientID %>').click(); }
        function ThailandClick() { document.getElementById('<%= btnThailand.ClientID %>').click(); }
        function ArgentinaClick() { document.getElementById('<%= btnArgentina.ClientID %>').click(); }
        function BrazilClick() { document.getElementById('<%= btnBrazil.ClientID %>').click(); }
        function CanadaClick() { document.getElementById('<%= btnCanada.ClientID %>').click(); }
        function LatinaClick() { document.getElementById('<%= btnLatina.ClientID %>').click(); }
        function MexicoClick() { document.getElementById('<%= btnMexico.ClientID %>').click(); }

        function DEVClick() { document.getElementById('<%= btnDEV.ClientID %>').click(); }
        function UATClick() { document.getElementById('<%= btnUAT.ClientID %>').click(); }
        function QAClick() { document.getElementById('<%= btnQA.ClientID %>').click(); }
      
        
    </script>

    <asp:UpdatePanel ID="updPnlPageLevel" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td>
                        <asp:Panel ID="pnlLaindingPageHdr" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    WARS AFFILIATES
                                </div>
                            </div>
                        </asp:Panel>
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
                                <td width="10%"></td>
                                <td width="5%"></td>
                                <td width="5%"></td>
                                <td width="10%"></td>
                                <td width="5%"></td>
                                <td width="5%"></td>
                                <td width="10%"></td>
                                <td width="5%"></td>
                                <td width="5%"></td>
                                <td width="10%"></td>
                                <td width="5%"></td>
                                <td width="5%"></td>
                                <td width="10%"></td>
                                <td width="3%"></td>
                                <td width="3%"></td>
                                <td width="3%"></td>
                            </tr>
                            <tr>
                                <td colspan="4" class="LandingScrn_Header">EMEA</td>
                                <td>&nbsp
                                     <div>
                                         <br />
                                     </div>
                                </td>
                                <td></td>
                                <td colspan="4" class="LandingScrn_Header">Asia Pacific</td>
                                <td></td>
                                <td></td>
                                <td colspan="4" class="LandingScrn_Header">North/South America</td>
                            </tr>
                            <tr>
                                <td colspan="16">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td class="LandingScrn_MenuItem" onclick="BeneluxClick();">Benelux</td>
                                <td></td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="PolandClick();">Poland</td>
                                <td>&nbsp
                                     <div>
                                         <br />
                                     </div>
                                </td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="AustraliaNZClick();">Australia/NZ</td>
                                <td></td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="SingaporeClick();">Singapore</td>
                                <td></td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="ArgentinaClick();">Argentina</td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="16">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td class="LandingScrn_MenuItem" onclick="CentralEuropeClick();">Central Europe</td>
                                <td></td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="PortugalClick();">Portugal</td>
                                <td>&nbsp
                                     <div>
                                         <br />
                                     </div>
                                </td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="ChinaClick();">China</td>
                                <td></td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="TaiwanClick();">Taiwan</td>
                                <td></td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="BrazilClick();">Brazil</td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="16">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td class="LandingScrn_MenuItem" onclick="CzechRepublicClick();">Czech Republic</td>
                                <td></td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="RussiaClick();">Russia</td>
                                <td>&nbsp
                                     <div>
                                         <br />
                                     </div>
                                </td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="HongKkongClick();">Hong Kong</td>
                                <td></td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="ThailandClick();">Thailand</td>
                                <td></td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="CanadaClick();">Canada</td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="16">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td class="LandingScrn_MenuItem" onclick="DenmarkClick();">Denmark</td>
                                <td></td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="SouthAfricaClick();">South Africa</td>
                                <td>&nbsp
                                     <div>
                                         <br />
                                     </div>
                                </td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="IndonesiaClick();">Indonesia</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="LatinaClick();">Latina</td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="16">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td class="LandingScrn_MenuItem" onclick="FinlandClick();">Finland</td>
                                <td></td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="SpainClick();">Spain</td>
                                <td>&nbsp
                                     <div>
                                         <br />
                                     </div>
                                </td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="JapanClick();">Japan</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="MexicoClick();">Mexico</td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="16">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td class="LandingScrn_MenuItem" onclick="ItalyClick();">Italy</td>
                                <td></td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="SwedenClick();">Sweden</td>
                                <td>&nbsp
                                     <div>
                                         <br />
                                     </div>
                                </td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="KoreaClick();">Korea</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="16">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td class="LandingScrn_MenuItem" onclick="MiddleEastClick();">Middle East</td>
                                <td></td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" id="tdWMUK" runat="server" onmouseout="tdWMUKOut();" onmouseover="tdWMUKOn();">UK
                                    <ajaxToolkit:HoverMenuExtender ID="hmeWMUK" runat="server" TargetControlID="tdWMUK" PopupControlID="pnlWMUK"
                                        PopupPosition="Right" HoverCssClass="LandingScrn_MenuItem_Hover">
                                    </ajaxToolkit:HoverMenuExtender>
                                    <asp:Panel ID="pnlWMUK" runat="server" CssClass="LandingScrn_SubMenu_Panel">
                                        <table class="LandingScrn_SubMenu_table" style="align-content: center">
                                            <tr>
                                                <td onclick="UKADAClick();">ADA                                                   
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="UKERATOClick();">ERATO   
                                                    
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="UKGBClick();">GB    
                                                  
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="UKNVCClick();">NVC    
                                                   
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="UKPLGClick();">PLG    
                                                   
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="UKTELDECClick();">TELDEC       
                                                         
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="UKWEAClick();">WEA     
                                                   
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                                <td>&nbsp
                                     <div>
                                         <br />
                                     </div>
                                </td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="MalaysiaClick();">Malaysia</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="16">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td class="LandingScrn_MenuItem" onclick="NorwayClick();">Norway</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td>&nbsp
                                     <div>
                                         <br />
                                     </div>
                                </td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="PhilippinesClick();">Philippines</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="16">
                                    <br />
                                </td>
                            </tr>
                            <tr id="trNonPRODAffiliates" runat="server">
                                <td class="LandingScrn_MenuItem" onclick="DEVClick();">DEV</td>
                                <td></td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="UATClick();">UAT</td>
                                <td>&nbsp
                                     <div>
                                         <br />
                                     </div>
                                </td>
                                <td></td>
                                <td class="LandingScrn_MenuItem" onclick="QAClick();">QA</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                        </table>
                    </td>
                </tr>

            </table>

            <asp:UpdateProgress ID="progressBarPageLevel" runat="server" AssociatedUpdatePanelID="updPnlPageLevel" DisplayAfter="100">
                <ProgressTemplate>
                    <div id="Search" style="font-weight: bold; color: Black">
                        <img src="../Images/InProgress2.gif" alt="" />
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <ajaxToolkit:ModalPopupExtender ID="mPopupPageLevel" runat="server" PopupControlID="progressBarPageLevel" TargetControlID="progressBarPageLevel"
                BackgroundCssClass="progressBar" RepositionMode="RepositionOnWindowResize">
            </ajaxToolkit:ModalPopupExtender>

            <%--Hidden buttons--%>
            <asp:Button ID="btnBenelux" runat="server" Style="display: none;" Text="" OnClick="btnBenelux_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnCentralEurope" runat="server" Style="display: none;" Text="" OnClick="btnCentralEurope_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnCzechRepublic" runat="server" Style="display: none;" Text="" OnClick="btnCzechRepublic_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnDenmark" runat="server" Style="display: none;" Text="" OnClick="btnDenmark_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnFinland" runat="server" Style="display: none;" Text="" OnClick="btnFinland_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnItaly" runat="server" Style="display: none;" Text="" OnClick="btnItaly_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnMiddleEast" runat="server" Style="display: none;" Text="" OnClick="btnMiddleEast_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnNorway" runat="server" Style="display: none;" Text="" OnClick="btnNorway_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnPoland" runat="server" Style="display: none;" Text="" OnClick="btnPoland_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnPortugal" runat="server" Style="display: none;" Text="" OnClick="btnPortugal_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnRussia" runat="server" Style="display: none;" Text="" OnClick="btnRussia_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnSouthAfrica" runat="server" Style="display: none;" Text="" OnClick="btnSouthAfrica_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnSpain" runat="server" Style="display: none;" Text="" OnClick="btnSpain_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnSweden" runat="server" Style="display: none;" Text="" OnClick="btnSweden_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnUKADA" runat="server" Style="display: none;" Text="" OnClick="btnUKADA_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnUKERATO" runat="server" Style="display: none;" Text="" OnClick="btnUKERATO_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnUKGB" runat="server" Style="display: none;" Text="" OnClick="btnUKGB_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnUKNVC" runat="server" Style="display: none;" Text="" OnClick="btnUKNVC_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnUKPLG" runat="server" Style="display: none;" Text="" OnClick="btnUKPLG_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnUKTELDEC" runat="server" Style="display: none;" Text="" OnClick="btnUKTELDEC_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnUKWEA" runat="server" Style="display: none;" Text="" OnClick="btnUKWEA_Click"
                UseSubmitBehavior="false" />

            <asp:Button ID="btnAustraliaNZ" runat="server" Style="display: none;" Text="" OnClick="btnAustraliaNZ_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnChina" runat="server" Style="display: none;" Text="" OnClick="btnChina_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnHongKong" runat="server" Style="display: none;" Text="" OnClick="btnHongKong_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnIndonesia" runat="server" Style="display: none;" Text="" OnClick="btnIndonesia_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnJapan" runat="server" Style="display: none;" Text="" OnClick="btnJapan_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnKorea" runat="server" Style="display: none;" Text="" OnClick="btnKorea_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnMalaysia" runat="server" Style="display: none;" Text="" OnClick="btnMalaysia_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnPhilippines" runat="server" Style="display: none;" Text="" OnClick="btnPhilippines_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnSingapore" runat="server" Style="display: none;" Text="" OnClick="btnSingapore_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnTaiwan" runat="server" Style="display: none;" Text="" OnClick="btnTaiwan_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnThailand" runat="server" Style="display: none;" Text="" OnClick="btnThailand_Click"
                UseSubmitBehavior="false" />

            <asp:Button ID="btnArgentina" runat="server" Style="display: none;" Text="" OnClick="btnArgentina_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnBrazil" runat="server" Style="display: none;" Text="" OnClick="btnBrazil_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnCanada" runat="server" Style="display: none;" Text="" OnClick="btnCanada_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnLatina" runat="server" Style="display: none;" Text="" OnClick="btnLatina_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnMexico" runat="server" Style="display: none;" Text="" OnClick="btnMexico_Click"
                UseSubmitBehavior="false" />

            <asp:Button ID="btnDEV" runat="server" Style="display: none;" Text="" OnClick="btnDEV_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnUAT" runat="server" Style="display: none;" Text="" OnClick="btnUAT_Click"
                UseSubmitBehavior="false" />
            <asp:Button ID="btnQA" runat="server" Style="display: none;" Text="" OnClick="btnQA_Click"
                UseSubmitBehavior="false" />


            <%--Hidden buttons--%>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:TextBox>
            <asp:TextBox ID="TextBox1" runat="server" Text="" TabIndex="99" CssClass="gridTextField" onkeydown="OnTabPress();"></asp:TextBox>


        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

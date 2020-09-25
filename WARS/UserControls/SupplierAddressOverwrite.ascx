<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SupplierAddressOverwrite.ascx.cs" Inherits="WARS.UserControls.SupplierAddressOverwrite" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:UpdatePanel ID="updPnlActivity" runat="server">
    <ContentTemplate>
        <table id="tblMain" width="100%">
            <tr>
                <td colspan="4"></td>
            </tr>
            <tr>
                <td width="2%"></td>
                <td colspan="3" valign="top">
                    <table width="100%">
                        <tr>
                            <td width="12%" class="identifierLable_large_bold">Payee</td>
                            <td width="20%">
                                <asp:TextBox ID="txtPayee" runat="server" Width="97%" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                            </td>
                            <td align="right">
                                <asp:Button ID="btnAddressOverwrite" runat="server" CssClass="ButtonStyle" OnClick="btnAddressOverwrite_Click"
                                    TabIndex="121" Text="Overwrite Payee Address" UseSubmitBehavior="false" Width="22%" />
                            </td>
                        </tr>
                        <tr>
                            <td class="identifierLable_large_bold">Supplier</td>
                            <td>
                                <asp:TextBox ID="txtSupplier" runat="server" Width="97%" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <td class="identifierLable_large_bold">Supplier Site Name</td>
                            <td>
                                <asp:TextBox ID="txtSupllierSiteName" runat="server" Width="97%" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <td class="identifierLable_large_bold">Mismatch Flag</td>
                            <td>
                                <asp:TextBox ID="txtMismatchFlag" runat="server" Width="15px" CssClass="textboxStyle_readonly" Font-Bold="true" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <table width="100%">
                                    <tr>
                                        <td width="48%" valign="top" class="table_with_border">
                                            <table width="100%">
                                                <tr>
                                                    <td width="5%"></td>
                                                    <td width="20%"></td>
                                                    <td width="70%"></td>
                                                    <td width="5%"></td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td class="table_header_with_border" colspan="3">Supplier Address</td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold">Supplier Name</td>
                                                    <td>
                                                        <asp:TextBox ID="txtSuppName" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold">Address 1</td>
                                                    <td>
                                                        <asp:TextBox ID="txtSuppAdd1" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold">Address 2</td>
                                                    <td>
                                                        <asp:TextBox ID="txtSuppAdd2" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold">Address 3</td>
                                                    <td>
                                                        <asp:TextBox ID="txtSuppAdd3" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold">Address 4</td>
                                                    <td>
                                                        <asp:TextBox ID="txtSuppCity" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold">Postcode</td>
                                                    <td>
                                                        <asp:TextBox ID="txtSuppPostcode" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td></td>
                                        <td width="48%" valign="top" class="table_with_border">
                                            <table width="100%">
                                                <tr>
                                                    <td width="5%"></td>
                                                    <td width="20%"></td>
                                                    <td width="70%"></td>
                                                    <td width="5%"></td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td class="table_header_with_border" colspan="3">Payee Address</td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold">Payee Name</td>
                                                    <td>
                                                        <asp:TextBox ID="txtPayeeName" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold">Address 1</td>
                                                    <td>
                                                        <asp:TextBox ID="txtPayeeAdd1" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold">Address 2</td>
                                                    <td>
                                                        <asp:TextBox ID="txtPayeeAdd2" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold">Address 3</td>
                                                    <td>
                                                        <asp:TextBox ID="txtPayeeAdd3" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold">Address 4</td>
                                                    <td>
                                                        <asp:TextBox ID="txtPayeeAdd4" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td class="identifierLable_large_bold">Postcode</td>
                                                    <td>
                                                        <asp:TextBox ID="txtPayeePostcode" runat="server" Width="99%" CssClass="textboxStyle" ReadOnly="true" TabIndex="-1"></asp:TextBox>
                                                    </td>
                                                    <td></td>
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

        <%--Save/Cancel changes popup--%>
        <asp:Button ID="dummySaveCancel" runat="server" Style="display: none" />
        <ajaxToolkit:ModalPopupExtender ID="mpeSaveCancel" runat="server" PopupControlID="pnlSaveCancel" TargetControlID="dummySaveCancel"
            CancelControlID="btnClosePopupSaveCancel" BackgroundCssClass="popupBox">
        </ajaxToolkit:ModalPopupExtender>
        <asp:Panel ID="pnlSaveCancel" runat="server" align="center" Width="25%" CssClass="popupPanel"  Style="z-index: 1; display: none;">
            <table width="100%">
                <tr class="ScreenName">
                    <td align="right" style="vertical-align: top;">
                        <asp:ImageButton ID="btnClosePopupSaveCancel" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblMessage" runat="server" CssClass="identifierLable"
                            Text="This will overwrite the Payee name and address. Select Continue to Update or Cancel to exit."></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <asp:Button ID="btnContinue" runat="server" Text="Continue" CssClass="ButtonStyle"
                                        OnClick="btnContinue_Click" />
                                </td>
                                <td></td>
                                <td>
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="ButtonStyle" OnClick="btnCancel_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <%--Save/Cancel changes popup Ends--%>

        <asp:HiddenField ID="hdnSelectedPayee" runat="server" Value="N" />
         <asp:HiddenField ID="hdnRoyaltorId" runat="server" Value="N" />
        <asp:HiddenField ID="hdnIsAddressOverwritten" runat="server" Value="N" />

    </ContentTemplate>
</asp:UpdatePanel>

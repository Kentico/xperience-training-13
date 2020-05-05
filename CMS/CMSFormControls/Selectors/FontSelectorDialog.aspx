<%@ Page Language="C#" AutoEventWireup="false" Inherits="CMSFormControls_Selectors_FontSelectorDialog"
    Theme="default" Title="Font Selector" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
     Codebehind="FontSelectorDialog.aspx.cs" %>

<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
    <div class="font-selector">
        <asp:Label runat="server" ID="lblError" CssClass="ErrorLabel" EnableViewState="false"
            Visible="false" />
        <table>
            <tr>
                <td class="FontSelectorStyleColumnHeader">
                    <%#ResHelper.GetString("fontselector.font")%>
                </td>
                <td class="FontSelectorStyleColumnHeader">
                    <%#ResHelper.GetString("fontselector.style")%>
                </td>
                <td class="FontSelectorStyleColumnHeader">
                    <%#ResHelper.GetString("fontselector.size")%>
                </td>
            </tr>
            <tr>
                <td>
                    <cms:CMSTextBox ID="txtFontType" runat="server" ReadOnly="true" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtFontStyle" runat="server" ReadOnly="true" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtFontSize" runat="server" onChange="sizeManualUpdate();" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:CMSListBox ID="lstFontType" CssClass="FontSelectorTypeListBox" runat="server" Rows="7" onChange="fontTypeChange(this.options[this.selectedIndex].value);" />
                </td>
                <td>
                    <cms:CMSListBox ID="lstFontStyle" CssClass="FontSelectorStyleListBox" runat="server" Rows="7" onChange="fontStyleChange(this.selectedIndex,this.options[this.selectedIndex].text);" />
                </td>
                <td>
                    <cms:CMSListBox ID="lstFontSize" CssClass="FontSelectorStyleListBox" runat="server" Rows="7" onChange="fontSizeChange(this.options[this.selectedIndex].value);" />
                </td>
            </tr>
        </table>
        <div class="boxes">
            <cms:CMSCheckBox ID="chkUnderline" runat="server" ResourceString="fontselector.underline" onclick="fontDecorationChange();" />
            <cms:CMSCheckBox ID="chkStrike" runat="server" ResourceString="fontselector.strikethrought" onclick="fontDecorationChange();" />
        </div>
        <asp:Panel ID="pnlSampleText" runat="server" class="FontSelectorTextSamplePanel">
            <asp:Label runat="server" ID="lblSampleText" Text="AaBbZzYy" EnableViewState="false" />
        </asp:Panel>
    </div>
</asp:Content>

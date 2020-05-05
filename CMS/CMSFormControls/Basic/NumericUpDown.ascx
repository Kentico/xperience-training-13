<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NumericUpDown.ascx.cs" Inherits="CMSFormControls_Basic_NumericUpDown" %>
<asp:Panel ID="pnlContainer" runat="server" CssClass="control-group-inline numeric-up-down">
    <cms:CMSTextBox ID="textbox" runat="server" />
    <div class="numeric-updown-buttons">
        <asp:ImageButton ID="btnImgUp" CssClass="numeric-updown-button-up" runat="server" Visible="false" OnClientClick="return false;" />
        <asp:ImageButton ID="btnImgDown" CssClass="numeric-updown-button-down" runat="server" Visible="false" OnClientClick="return false;" />
        <cms:CMSAccessibleButton ID="btnUp" IconOnly="True" IconCssClass="icon-caret-up" CssClass="numeric-updown-button-up" runat="server" Visible="True" OnClientClick="return false;" />
        <cms:CMSAccessibleButton ID="btnDown" IconCssClass="icon-caret-down" IconOnly="True" CssClass="numeric-updown-button-down" runat="server" Visible="True" OnClientClick="return false;" />
    </div>
</asp:Panel>

<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Layouts_Wizard_ConfirmationCheckbox" CodeBehind="~/CMSWebParts/Layouts/Wizard/ConfirmationCheckbox.ascx.cs" %>

<asp:Panel runat="server" ID="pnlCheckBox" CssClass="ConfirmationCheckbox" Visible="false">
    <cms:CMSCheckBox runat="server" ID="chkAccept" Visible="true" />
</asp:Panel>
<asp:Panel runat="server" ID="pnlError" CssClass="Error" Visible="false">
    <asp:Label runat="server" ID="lblError" />
</asp:Panel>

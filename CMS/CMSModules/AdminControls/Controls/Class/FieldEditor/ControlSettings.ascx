<%@ Control Language="C#" AutoEventWireup="false"  Codebehind="ControlSettings.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_Class_FieldEditor_ControlSettings" %>
<cms:LocalizedHeading runat="server" Level="4" ResourceString="templatedesigner.section.settings"></cms:LocalizedHeading>
<asp:Panel runat="server" ID="pnlSwitch" Visible="false" CssClass="form-group">
    <cms:CMSIcon runat="server" ID="icAdvanced" CssClass="icon-caret-down cms-icon-30" />
    <cms:LocalizedLinkButton runat="server" ID="lnkAdvanced" OnClick="link_Click" />
</asp:Panel>
<asp:Panel ID="pnlSettings" runat="server" CssClass="FieldPanel">
    <cms:BasicForm ID="form" runat="server" AllowMacroEditing="true" FieldGroupHeadingLevel="5" MarkRequiredFields="True" />
</asp:Panel>

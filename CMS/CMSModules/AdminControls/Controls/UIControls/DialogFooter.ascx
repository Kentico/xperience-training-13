<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/AdminControls/Controls/UIControls/DialogFooter.ascx.cs" Inherits="CMSModules_AdminControls_Controls_UIControls_DialogFooter" %>

<asp:Panel ID="pnlFooter" runat="server" CssClass="dialog-footer">
    <cms:LocalizedButton ID="btnCancel" runat="server" ButtonStyle="Default" EnableViewState="False" ResourceString="general.cancel" OnClientClick="return CloseDialog(false);" />
    <cms:CMSPlaceHolder runat="server" ID="plcContent"></cms:CMSPlaceHolder>
</asp:Panel>

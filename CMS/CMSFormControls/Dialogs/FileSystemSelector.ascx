<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSFormControls_Dialogs_FileSystemSelector"  Codebehind="FileSystemSelector.ascx.cs" %>

<asp:Panel ID="pnlForm" runat="server" CssClass="control-group-inline">
    <cms:CMSTextBox ID="txtPath" runat="server" />
    <cms:LocalizedButton ID="btnSelect" runat="server" EnableViewState="false" ButtonStyle="Default" />
    <cms:LocalizedButton ID="btnClear" runat="server" EnableViewState="false" ButtonStyle="Default" />
</asp:Panel>
<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_ImportExport_Controls_Import_cms_customtable"  Codebehind="cms_customtable.ascx.cs" %>
<asp:Panel runat="server" ID="pnlCheck" CssClass="wizard-section">
    <div class="content-block-50">
        <cms:CMSCheckBox ID="chkObject" runat="server" Visible="false" />
    </div>
    <asp:Label ID="lblInfo" runat="server" CssClass="form-group" />
</asp:Panel>

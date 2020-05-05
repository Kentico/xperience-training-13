<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_ImportExport_Controls_Export_cms_resource"  Codebehind="cms_resource.ascx.cs" %>

<asp:Panel runat="server" ID="pnlCheck" CssClass="wizard-section">
    <cms:LocalizedLabel ID="lblInfo" runat="Server" EnableViewState="false" CssClass="form-group" ResourceString="CMSExport_Resources.Info" />
    <div class="checkbox-list-vertical content-block-50">
        <cms:CMSCheckBox ID="chkSealed" runat="server" ResourceString="CMSExport_Resources.SealModules" />
    </div>
</asp:Panel>

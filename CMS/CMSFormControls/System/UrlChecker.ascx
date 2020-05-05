<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="UrlChecker.ascx.cs" Inherits="CMSFormControls_System_UrlChecker" %>

<div class="url-checker">
    <div class="control-group-inline">
        <cms:CMSTextBox ID="txtDomain" runat="server" />
        <cms:LocalizedLabel ID="lblSuffix" runat="server" CssClass="form-control-text" />
        <cms:CMSButton runat="server" ID="btnCheckServer" EnableViewState="false" ValidationGroup="UrlChecker" ButtonStyle="Default" />
        <cms:CMSPanel runat="server" ID="pnlStatus" CssClass="status form-control-text" />
    </div>
    <cms:CMSPanel runat="server" ID="pnlError" CssClass="control-group-inline">
    </cms:CMSPanel>
</div>
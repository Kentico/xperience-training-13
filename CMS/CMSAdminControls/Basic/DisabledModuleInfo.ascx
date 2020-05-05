<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="DisabledModuleInfo.ascx.cs"
    Inherits="CMSAdminControls_Basic_DisabledModuleInfo" %>
<div class="alert alert-info">
    <span class="alert-icon">
        <cms:CMSIcon ID="iconAlert" runat="server" CssClass="icon-i-circle" AlternativeText="{$general.info$}" />
    </span>
    <div class="alert-label">
        <cms:LocalizedLabel runat="server" ID="lblText" EnableViewState="false" />
        <div class="alert-buttons">
            <cms:CMSButton runat="server" ID="btnGlobal" ButtonStyle="Default" OnClick="btnGlobal_clicked" EnableViewState="false" />
            <cms:CMSButton runat="server" ID="btnSite" ButtonStyle="Default" OnClick="btnSiteOnly_clicked" EnableViewState="false" />
        </div>
    </div>
</div>
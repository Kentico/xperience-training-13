<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Dialogs_General_ViewModeMenu"
     Codebehind="ViewModeMenu.ascx.cs" %>

<div class="btn-group">
    <cms:CMSAccessibleButton runat="server" ID="btnList" OnClientClick="SetLastViewAction('list'); RaiseHiddenPostBack(); return false;" IconCssClass="icon-list" />
    <cms:CMSAccessibleButton runat="server" ID="btnThumbs" OnClientClick="SetLastViewAction('thumbnails'); RaiseHiddenPostBack(); return false;" IconCssClass="icon-pictures" />
</div>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false"></asp:Literal>
<asp:HiddenField ID="hdnLastSelectedTab" runat="server" />


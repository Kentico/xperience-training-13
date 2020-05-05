<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_UIProfiles_UIToolbar"
     Codebehind="UIToolbar.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniMenu/UniMenu.ascx" TagName="UniMenu" TagPrefix="cms" %>
<div class="UIToolbar">
    <cms:UniMenu ID="uniMenu" runat="server" ShowErrors="false" />
    <asp:Literal runat="server" ID="ltlAfter" />
</div>

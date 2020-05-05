<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="TreeLanguageMenu.ascx.cs" Inherits="CMSModules_Content_Controls_TreeLanguageMenu" %>
<%@ Register Src="~/CMSModules/Content/Controls/LanguageMenu.ascx" TagPrefix="cms" TagName="LanguageMenu" %>

<div class="tree-language-menu">
    <cms:LanguageMenu runat="server" ID="LanguageMenu" MenuDirectionUp="true" />
</div>

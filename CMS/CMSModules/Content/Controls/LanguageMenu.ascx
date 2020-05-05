<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_LanguageMenu"
     Codebehind="LanguageMenu.ascx.cs" %>

    <div class="btn-dropdown language-menu scrollable-menu">
        <div class="btn btn-default dropdown-toggle icon-only" data-toggle="dropdown" id="drpLanguages">
            <asp:Image runat="server" ID="imgLanguage" CssClass="icon-only" />
            <asp:Label runat="server" ID="lblLanguageName" CssClass="language-name-selected" EnableViewState="false"/>
            <i class="icon-caret-right-down cms-icon-30" aria-hidden="true"></i>
            <span class="sr-only"><%= GetString("languageselect.title") %></span>
        </div>
        <ul id="language-menu" class="dropdown-menu<%= MenuDirectionUp ? " pull-up" : String.Empty %>" role="menu" aria-labelledby="drpLanguages">
            <asp:Literal ID="ltlLanguages" runat="server" />
        </ul>
    </div><cms:CMSButton ID="btnCompare" runat="server" CssClass="js-split-toggle" ButtonStyle="Default" OnClientClick="ChangeSplitMode(this); return false;" />

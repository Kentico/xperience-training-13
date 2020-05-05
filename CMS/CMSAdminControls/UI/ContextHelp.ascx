<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_ContextHelp"  Codebehind="ContextHelp.ascx.cs" %>
<h2 class="sr-only"><%= GetString("contexthelp.title") %></h2>
<a class="accordion-toggle collapsed js-context-help" href="#cms-nav-help" data-toggle="collapse" title="<%= GetString("helpicon.help") %>">
    <i class="icon-question-circle cms-nav-icon-medium" aria-hidden="true"></i>
    <span class="sr-only"><%= GetString("helpicon.help") %></span>
</a>
<div class="hide" runat="server" id="pnlToolbar">
    <div id="cms-nav-help" class="navbar cms-navbar-help panel-collapse collapse no-transition">
        <ul class="nav navbar-nav navbar-left">
            <li runat="server" id="description"></li>
            <li runat="server" id="helpTopics" class="dropdown">
                <a class="dropdown-toggle" href="#" data-toggle="dropdown">
                    <cms:LocalizedLabel ID="lblHowto" runat="server" ResourceString="contexthelp.howto"></cms:LocalizedLabel>
                    <i class="caret" aria-hidden="true"></i>
                </a>
                <ul class="dropdown-menu"></ul>
            </li>
            <li>
                <a href="<%= DocumentationHelper.GetDocumentationRootUrl() %>" target="_blank"><%= GetString("contexthelp.opendocumentation") %></a>
            </li>
            <li>
                <a href="http://devnet.kentico.com/questions-answers?utm_campaign=helpbar" target="_blank"><%= GetString("contexthelp.askcommunity") %></a>
            </li>
            <li>
                <a href="http://ideas.kentico.com" target="_blank"><%= GetString("contexthelp.requestfeature") %></a>
            </li>
            <li>
                <a href="<%= ApplicationUIHelper.REPORT_BUG_URL  %>" target="_blank"><%= GetString("contexthelp.submitsupportissue") %></a>
            </li>
            <li class="navbar-text">
                <asp:Label runat="server" ID="lblVersion" />
            </li>
        </ul>
        <ul class="nav navbar-nav navbar-right navbar-inverse cms-navbar-help">
            <li class="nav-search-container" runat="server" id="search">
                <label class="sr-only" for="cms-navbar-help-search"><%= GetString("contexthelp.search") %></label>
                <input id="cms-navbar-help-search" type="search" placeholder="<%= GetString("contexthelp.search") %>">
                <i class="icon-magnifier" aria-hidden="true"></i>
            </li>
        </ul>
    </div>
</div>
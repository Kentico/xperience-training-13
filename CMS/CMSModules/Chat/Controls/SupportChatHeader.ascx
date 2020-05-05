<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Chat_Controls_SupportChatHeader"  Codebehind="SupportChatHeader.ascx.cs" %>

<li runat="server" id="headerIcon">
    <h2 class="sr-only"><%= GetString("chat.support.title") %></h2>
    <a class="chat-toggle collapsed" href="#cms-nav-chat" data-toggle="collapse" title="<%= GetString("chat.support.toggle") %>">
        <asp:Label runat="server" ID="lblNotificationNumber" CssClass="chat-req-number hide"></asp:Label>
        <i class="icon-bubble cms-nav-icon-medium" aria-hidden="true"></i>
        <span class="sr-only"><%= GetString("chat.support.toggle") %></span>
    </a>
</li>
<li class="hide">
    <div id="cms-nav-chat" class="navbar no-transition navbar-inverse cms-navbar-chat panel-collapse collapse">
        <ul class="nav navbar-nav navbar-left">
            <li>
                <a class="dropdown-toggle cms-nav-chat-menu" data-toggle="dropdown" href="#" title="<%= GetString("chat.support.menu") %>">
                    <i class="icon-menu cms-nav-icon-medium" aria-hidden="true"></i>
                    <span class="sr-only-fixed"><%= GetString("chat.support.menu") %></span>
                </a>
                <ul class="dropdown-menu" role="menu">
                    <li>
                        <cms:LocalizedHyperlink runat="server" ID="btnLogin" ResourceString="chat.support.login" href="#"></cms:LocalizedHyperlink>
                        <cms:LocalizedHyperlink runat="server" ID="btnLogout" ResourceString="chat.support.logout" href="#" CssClass="hide"></cms:LocalizedHyperlink>
                    </li>
                    <li>
                        <cms:LocalizedHyperlink runat="server" ID="btnSettings" ResourceString="chat.support.settings" href="#"></cms:LocalizedHyperlink>
                    </li>
                </ul>
            </li>
            <li runat="server" id="loginShortcutWrapper" class="navbar-text">
                <%= GetString("chat.support.offlinestatus") %><cms:LocalizedHyperlink runat="server" ID="btnLoginShortcut" ResourceString="chat.support.login" href="#" CssClass="chat-navbar-action"></cms:LocalizedHyperlink>
            </li>
            <li>
                <ul class="chat-navbar-rooms" runat="server" id="ulActiveRequests">
                </ul>
            </li>
        </ul>
        <ul class="nav navbar-nav navbar-right navbar-inverse cms-navbar-chat">
            <li class="dropdown">
                <asp:HyperLink CssClass="dropdown-toggle disabled hide" href="#" data-toggle="dropdown" runat="server" ID="lnkNewRequests">
                    <asp:Label runat="server" ID="lblNewRequests"></asp:Label><i class="caret" aria-hidden="true"></i></asp:HyperLink>
                <ul class="dropdown-menu" runat="server" id="ulNewRequests">
                </ul>
            </li>
        </ul>
    </div>
</li>



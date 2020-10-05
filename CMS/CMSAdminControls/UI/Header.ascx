<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Header.ascx.cs" Inherits="CMSAdminControls_UI_Header" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/Controls/PasswordExpiration.ascx" TagPrefix="cms" TagName="PasswordExpiration" %>
<%@ Register Src="~/CMSAdminControls/UI/UserMenu.ascx" TagPrefix="cms" TagName="UserMenu" %>
<%@ Register Src="~/CMSAdminControls/UI/ContextHelp.ascx" TagPrefix="cms" TagName="ContextHelp" %>

<div class="navbar navbar-inverse cms-navbar">
    <asp:PlaceHolder runat="server" ID="plcToggle">
        <a href="javascript:void(0)" class="applist-toggle navbar-left" id="cms-applist-toggle" title="<%= GetString("applicationlist.open") %>">
            <i aria-hidden="true" class="icon-kentico cms-nav-icon-large"></i>
            <span class="sr-only"><%= GetString("applicationlist.open") %></span>
        </a>
    </asp:PlaceHolder>
    <h2 class="sr-only"><%= GetString("breadcrumbs.title")  %></h2>
    <ul class="navbar-left breadcrumb cms-nav-breadcrumb" id="js-nav-breadcrumb">
        <li>
            <cms:localizedhyperlink id="lnkDashboard" runat="server" enableviewstate="false" tooltipresourcestring="cms.dashboard.back">
                <i aria-hidden="true" class="icon-home cms-nav-icon-medium"></i>
                <span class="sr-only"><%= GetString("cms.dashboard.back") %></span>
            </cms:localizedhyperlink>
        </li>
        <asp:PlaceHolder runat="server" ID="plcSiteSelector">
            <li class="dropdown no-ico header-site-selector">
                <cms:siteselector id="siteSelector" shortid="ss" runat="server" islivesite="false" />
            </li>
        </asp:PlaceHolder>
    </ul>
    <ul class="navbar-left breadcrumb cms-nav-breadcrumb">
        <li class="dashboard-breadcrumb-pin" id="js-single-object-dashboard-pin-list">
            <a href="javascript:void(0)">
                <i id="js-single-object-dashboard-pin-i" aria-hidden="true" class="icon-pin-o cms-icon-50 dashboard-pin icon-disabled"
                    data-action-text-pin="<%= GetString("cms.dashboard.pin") %>" data-action-text-unpin="<%= GetString("cms.dashboard.unpin") %>"></i>
                <span class="sr-only"><%= GetString("cms.dashboard.pin") %></span>
            </a>
        </li>
    </ul>
    <ul class="nav navbar-nav navbar-right navbar-inverse">
        <asp:PlaceHolder runat="server" ID="plcStagingTaskGroupContainer" Visible="false">
            <li>
                <asp:PlaceHolder runat="server" ID="plcStagingTaskGroup" Visible="false"/>
            </li>
        </asp:PlaceHolder>
        <li>
            <cms:contexthelp runat="server" id="contextHelp" />
        </li>
        <li>
            <cms:usermenu runat="server" id="userMenu" />
        </li>
    </ul>
</div>
<div id="cms-header-contexthelp"></div>
<div id="cms-header-placeholder"></div>
<div id="cms-header-messages">
    <asp:Panel runat="server" ID="pnlTechPreview" CssClass="message-panel alert-warning">
        Please note: This is a technical preview version. Changes are directly saved to
    the development database.
        <a href="#" class="alert-link" onclick="HideWarning('<%= pnlTechPreview.ClientID %>', '<% = SESSION_KEY_TECH_PREVIEW %>'); return false;"><%= GetString("general.close") %></a>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlTrial" CssClass="message-panel alert-warning">
        <asp:Literal ID="ltlText" runat="server" EnableViewState="false" />
        <a href="#" class="alert-link" onclick="HideWarning('<%= pnlTrial.ClientID %>', '<% = SESSION_KEY_TRIAL %>'); return false;"><%= GetString("general.close") %></a>
    </asp:Panel>

    <!-- Subscription Licenses -->
    <asp:Panel runat="server" ID="pnlSubscriptionLicencesWarning" CssClass="message-panel alert-warning" Visible="False">
        <asp:Literal ID="ltlSubscriptionLicenceWarning" runat="server" EnableViewState="false" />
        <a href="#" class="alert-link" onclick="HideWarning('<%= pnlSubscriptionLicencesWarning.ClientID %>', '<% = SESSION_KEY_SUBSCRIPTION_LICENCES %>'); return false;"><%= GetString("general.close") %></a>
    </asp:Panel>

    <asp:Panel runat="server" ID="pnlSubscriptionLicencesError" CssClass="message-panel alert-error" Visible="False">
        <asp:Literal ID="ltlSubscriptionLicenceError" runat="server" EnableViewState="false" />
    </asp:Panel>
    <!-- Subscription Licenses -->

    <asp:Panel runat="server" ID="pnlLicenseLimitations" CssClass="message-panel alert-error" Visible="False">
        <span class="alert-icon">
            <i aria-hidden="true" class="icon-exclamation-triangle cms-icon-150"></i>
        </span>
        <div class="alert-label">
            <asp:Literal  ID="ltlLicenseLimitations" runat="server" EnableViewState="false" />
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlPwdExp" CssClass="message-panel alert-warning">
        <cms:passwordexpiration id="pwdExpiration" runat="server" enableviewstate="false"
            islivesite="false" />
    </asp:Panel>
</div>

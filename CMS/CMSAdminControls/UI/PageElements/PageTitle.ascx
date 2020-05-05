<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="PageTitle.ascx.cs" Inherits="CMSAdminControls_UI_PageElements_PageTitle" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/Help.ascx" TagName="Help" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/BreadCrumbs.ascx" TagName="Breadcrumbs" TagPrefix="cms" %>

<asp:Panel runat="server" ID="pnlBody">
    <asp:Panel runat="server" ID="pnlTitle" CssClass="dialog-header non-selectable" Visible="false">
        <div class="dialog-header-action-buttons">
            <div class="action-button">
                <cms:Help ID="helpElem" runat="server" IconCssClass="cms-icon-80" />
            </div>
            <asp:PlaceHolder runat="server" ID="plcMisc" />
            <asp:Panel runat="server" ID="pnlMaximize" CssClass="action-button" Visible="False" EnableViewState="False">
                <a>
                    <span class="sr-only"><%= GetString("general.fullscreen") %></span>
                    <i id="btnMaximize" runat="server" class="icon-modal-maximize cms-icon-80" aria-hidden="true"></i>
                </a>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlClose" CssClass="action-button close-button" Visible="False" EnableViewState="False">
                <a>
                    <span class="sr-only"><%= GetString("general.close") %></span>
                    <i id="btnClose" runat="server" class="icon-modal-close cms-icon-150" aria-hidden="true"></i>
                </a>
            </asp:Panel>
        </div>
        <cms:LocalizedHeading runat="server" ID="headTitle" CssClass="dialog-header-title" EnableViewState="false" />
        <asp:Label ID="lblTitleInfo" runat="server" CssClass="PageTitleInfo" EnableViewState="false" Visible="false" />
    </asp:Panel>
    <cms:Breadcrumbs ID="breadcrumbs" runat="server" />
</asp:Panel>
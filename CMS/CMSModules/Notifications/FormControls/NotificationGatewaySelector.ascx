<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Notifications_FormControls_NotificationGatewaySelector"  Codebehind="NotificationGatewaySelector.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" DisplayNameFormat="{%GatewayDisplayName%}"
            ObjectType="notification.gateway" ResourcePrefix="notificationgatewayselector" />
    </ContentTemplate>
</cms:CMSUpdatePanel>

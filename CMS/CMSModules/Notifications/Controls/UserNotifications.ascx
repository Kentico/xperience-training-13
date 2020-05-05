<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Notifications_Controls_UserNotifications"
     Codebehind="UserNotifications.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<div class="UserNotifications">
    <cms:UniGrid ID="gridElem" runat="server" GridName="~/CMSModules/Notifications/Controls/UserNotifications.xml"
        ShowObjectMenu="false" OrderBy="SubscriptionTime" Columns="SubscriptionID, SubscriptionTarget, SubscriptionTime, SubscriptionEventDisplayName" />
</div>

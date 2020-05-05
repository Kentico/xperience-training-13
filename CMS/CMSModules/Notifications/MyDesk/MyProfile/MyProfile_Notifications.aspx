<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_Notifications_MyDesk_MyProfile_MyProfile_Notifications"
    Title="My profile - Notifications" Theme="Default"  Codebehind="MyProfile_Notifications.aspx.cs" %>

<%@ Register Src="~/CMSModules/Notifications/Controls/UserNotifications.ascx" TagName="UserNotifications"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="Server">
    <cms:UserNotifications ID="userNotificationsElem" runat="server" />
</asp:Content>

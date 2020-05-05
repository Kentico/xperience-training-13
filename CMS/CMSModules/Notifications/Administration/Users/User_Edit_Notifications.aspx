<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Notifications_Administration_Users_User_Edit_Notifications"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="User_Edit_Notifications.aspx.cs" %>

<%@ Register Src="~/CMSModules/Notifications/Controls/UserNotifications.ascx" TagName="UserNotifications"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UserNotifications ID="userNotificationsElem" runat="server" />
</asp:Content>

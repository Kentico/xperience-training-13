<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_Controls_Roles_RoleUsers"
     Codebehind="RoleUsers.ascx.cs" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/selectuser.ascx" TagName="SelectUser"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlBasic" runat="server" UpdateMode="Conditional">    
    <ContentTemplate>
    <cms:LocalizedHeading ID="lblAvialable" runat="server" Level="4" CssClass="listing-title" ResourceString="roleusers.available"/>    
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
<cms:SelectUser ID="usUsers" runat="server" SelectionMode="Multiple" />

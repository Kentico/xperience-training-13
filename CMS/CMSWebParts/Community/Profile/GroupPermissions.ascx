<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Community_Profile_GroupPermissions"  Codebehind="~/CMSWebParts/Community/Profile/GroupPermissions.ascx.cs" %>
<%@ Register Src="~/CMSModules/Groups/Controls/Security/GroupSecurity.ascx" TagName="GroupPermissions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/PermissionMessage.ascx" TagName="PermissionMessage"
    TagPrefix="cms" %>
<cms:PermissionMessage ID="messageElem" runat="server" Visible="false" EnableViewState="false" />
<cms:GroupPermissions ID="groupPermissions" runat="server" />

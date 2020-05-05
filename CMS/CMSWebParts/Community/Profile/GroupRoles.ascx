<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Community_Profile_GroupRoles"  Codebehind="~/CMSWebParts/Community/Profile/GroupRoles.ascx.cs" %>
<%@ Register Src="~/CMSModules/Membership/Controls/Roles/Roles.ascx" TagName="Roles"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/PermissionMessage.ascx" TagName="PermissionMessage"
    TagPrefix="cms" %>
<cms:PermissionMessage ID="messageElem" runat="server" Visible="false" EnableViewState="false" />
<cms:Roles ID="rolesElem" runat="server" />

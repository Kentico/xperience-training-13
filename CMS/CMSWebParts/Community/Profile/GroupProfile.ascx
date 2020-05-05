<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Community_Profile_GroupProfile"  Codebehind="~/CMSWebParts/Community/Profile/GroupProfile.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/System/PermissionMessage.ascx" TagName="PermissionMessage"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Groups/Controls/GroupProfile.ascx" TagName="GroupProfile"
    TagPrefix="cms" %>
<cms:PermissionMessage ID="messageElem" runat="server" EnableViewState="false" />
<cms:GroupProfile ID="groupProfileElem" runat="server" />


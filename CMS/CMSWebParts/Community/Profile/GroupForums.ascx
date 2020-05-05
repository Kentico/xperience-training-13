<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Community_Profile_GroupForums"  Codebehind="~/CMSWebParts/Community/Profile/GroupForums.ascx.cs" %>
<%@ Register Src="~/CMSModules/Forums/Controls/LiveControls/Groups.ascx" TagName="Forums"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/PermissionMessage.ascx" TagName="PermissionMessage"
    TagPrefix="cms" %>
<cms:PermissionMessage ID="messageElem" runat="server" Visible="false" EnableViewState="false" />
<cms:Forums ID="forumsElem" runat="server" />

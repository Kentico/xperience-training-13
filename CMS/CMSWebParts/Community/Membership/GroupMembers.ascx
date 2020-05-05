<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Community_Membership_GroupMembers"  Codebehind="~/CMSWebParts/Community/Membership/GroupMembers.ascx.cs" %>
<%@ Register Src="~/CMSModules/Groups/Controls/Members/Members.ascx" TagName="Members"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/PermissionMessage.ascx" TagName="PermissionMessage"
    TagPrefix="cms" %>
<cms:PermissionMessage ID="messageElem" runat="server" Visible="false" EnableViewState="false" />
<cms:Members ID="membersElem" runat="server" />

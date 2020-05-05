<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Community_Profile_GroupProperties"  Codebehind="~/CMSWebParts/Community/Profile/GroupProperties.ascx.cs" %>
<%@ Register Src="~/CMSModules/Groups/Controls/GroupEdit.ascx" TagName="GroupEdit"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/PermissionMessage.ascx" TagName="PermissionMessage"
    TagPrefix="cms" %>
<cms:PermissionMessage ID="messageElem" runat="server" Visible="false" EnableViewState="false" />
<cms:GroupEdit ID="groupEditElem" runat="server" />

<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Community_Profile_GroupMediaLibraries"
     Codebehind="~/CMSWebParts/Community/Profile/GroupMediaLibraries.ascx.cs" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/LiveControls/MediaLibraries.ascx"
    TagName="MediaLibraries" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/PermissionMessage.ascx" TagName="PermissionMessage"
    TagPrefix="cms" %>
<div class="GroupMediaLibraries">
    <cms:PermissionMessage ID="messageElem" runat="server" Visible="false" EnableViewState="false" />
    <cms:MediaLibraries ID="librariesElem" runat="server" />
</div>

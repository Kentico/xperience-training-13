<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_MediaLibrary_MediaGalleryFileList"
     Codebehind="~/CMSWebParts/MediaLibrary/MediaGalleryFileList.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/System/PermissionMessage.ascx" TagName="PermissionMessage"
    TagPrefix="cms" %>
<cms:PermissionMessage ID="messageElem" runat="server" EnableViewState="false" DisplayMessage="false" />
<cms:BasicRepeater ID="repItems" runat="server" />
<div style="clear: both">
</div>

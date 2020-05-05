<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_MediaLibrary_MediaGalleryFolderTree"  Codebehind="~/CMSWebParts/MediaLibrary/MediaGalleryFolderTree.ascx.cs" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/Filters/FolderTree.ascx" TagName="FolderTree"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/PermissionMessage.ascx" TagName="PermissionMessage"
    TagPrefix="cms" %>
<cms:PermissionMessage ID="messageElem" runat="server" EnableViewState="false" DisplayMessage="false" />
<cms:FolderTree ID="folderTree" runat="server" ShortID="t" />

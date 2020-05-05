<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_MediaLibrary_MediaGallery"  Codebehind="~/CMSWebParts/MediaLibrary/MediaGallery.ascx.cs" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/LiveControls/MediaGallery.ascx"
    TagName="MediaGallery" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/PermissionMessage.ascx" TagName="PermissionMessage"
    TagPrefix="cms" %>
<cms:PermissionMessage ID="messageElem" runat="server" EnableViewState="false" DisplayMessage="false" />
<cms:MediaGallery ID="gallery" runat="server" />

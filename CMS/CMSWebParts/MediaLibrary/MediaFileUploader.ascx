<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_MediaLibrary_MediaFileUploader"  Codebehind="~/CMSWebParts/MediaLibrary/MediaFileUploader.ascx.cs" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/LiveControls/MediaFileUploader.ascx"
    TagName="MediaFileUploader" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/PermissionMessage.ascx" TagName="PermissionMessage"
    TagPrefix="cms" %>
<cms:PermissionMessage ID="messageElem" runat="server" EnableViewState="false" DisplayMessage="false" />
<cms:MediaFileUploader ID="uploader" runat="server" />

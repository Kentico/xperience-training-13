<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MediaLibrary_Controls_LiveControls_MediaGallery"
     Codebehind="MediaGallery.ascx.cs" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/Filters/FolderTree.ascx" TagName="FolderTree"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/Filters/MediaLibrarySort.ascx"
    TagName="MediaLibrarySort" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/LiveControls/MediaFileUploader.ascx"
    TagName="FileUploader" TagPrefix="cms" %>
<%@ Register Assembly="CMS.MediaLibrary.Web.UI" Namespace="CMS.MediaLibrary.Web.UI" TagPrefix="cms" %>
<cms:MediaFileDataSource ID="fileDataSource" runat="server" FilterName="mediaLibrarySort"
    ShowGroupFiles="true" />
<div class="MediaGallery">
    <div class="MediaGalleryFolderTree" id="folderTreeContainer" runat="server">
        <cms:FolderTree ID="folderTree" runat="server" />
    </div>
    <div class="MediaGalleryContent">
        <cms:MediaLibrarySort ID="mediaLibrarySort" runat="server" />
        <cms:BasicRepeater ID="fileList" runat="server" />
        <div class="Pager">
            <cms:UniPager ID="UniPagerControl" PageControl="fileList" runat="server" />
        </div>
        <asp:Panel ID="pnlFileUploader" runat="server" CssClass="FileUploader">
            <cms:FileUploader ID="fileUploader" runat="server" />
        </asp:Panel>
    </div>
    <div style="clear: both">
    </div>
</div>

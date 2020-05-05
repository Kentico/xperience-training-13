<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ClassThumbnailSelectorFormControl.ascx.cs" Inherits="CMSModules_AdminControls_Controls_Class_ClassThumbnailSelector_ClassThumbnailSelectorFormControl" %>
<div class="class-thumbnail-selector-content" runat="server" id="divContainer">
    <a runat="server" ID="imgPreviewAnchor"><img runat="server" ID="imgPreview" /></a>
    <cms:LocalizedButton ID="btnSelectImage" runat="server" ButtonStyle="Default" ResourceString="dialogs.header.title.selectimage" />
    <asp:HiddenField runat="server" ID="hdnMetafileGuid" />
</div>
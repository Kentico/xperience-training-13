<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="DocumentTypeIconSelector.ascx.cs" Inherits="CMSModules_DocumentTypes_FormControls_DocumentTypeIconSelector" %>
<%@ Register Src="~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx"
    TagName="DirectFileUploader" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/FontIconSelector.ascx" TagPrefix="cms" TagName="FontIconSelector" %>

<cms:LocalizedLabel runat="server" ID="lblControlHidden" Visible="False" ResourceString="doctypeiconselector.hidden" EnableViewState="False" CssClass="form-control-text" />
<cms:CMSUpdatePanel runat="server" ID="pnlIcons" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="doc-type-icon-uploader">
            <div class="form-group">
                <cms:CMSDropDownList ID="lstOptions" runat="server" RepeatDirection="Horizontal" UseResourceStrings="true" AutoPostBack="True">
                    <asp:ListItem Selected="True" Text="docTypeIconUploader.images" Value="metafile" />
                    <asp:ListItem Text="docTypeIconUploader.fontIcon" Value="cssclass" />
                </cms:CMSDropDownList>
            </div>
            <asp:PlaceHolder ID="plcCssClass" runat="server" Visible="false">
                <cms:FontIconSelector runat="server" ID="fontIconSelector" />
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcUploaders" runat="server">
                <div class="form-group">
                    <div class="control-group-inline">
                        <asp:Image runat="server" ID="imgSmall" EnableViewState="false" CssClass="img-100" />
                        <cms:DirectFileUploader ID="dfuSmall" runat="server"
                            TargetFolderPath="~/App_Themes/Default/Images/DocumentTypeIcons" SourceType="PhysicalFile"
                            AllowedExtensions="png" ResizeToHeight="16" ResizeToWidth="16" AfterSaveJavascript="RefreshIcons"
                            ForceLoad="true" UploadMode="DirectSingle" />
                        <asp:Image runat="server" ID="imgLarge" EnableViewState="false" CssClass="img-300" />
                        <cms:DirectFileUploader ID="dfuLarge" runat="server"
                            TargetFolderPath="~/App_Themes/Default/Images/DocumentTypeIcons/48x48" SourceType="PhysicalFile"
                            AllowedExtensions="png" ResizeToHeight="48" ResizeToWidth="48" AfterSaveJavascript="RefreshIcons"
                            ForceLoad="true" UploadMode="DirectSingle" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>

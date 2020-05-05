<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MetafileOrFontIconSelector.ascx.cs"
    Inherits="CMSFormControls_Metafiles_MetafileOrFontIconSelector" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/File.ascx" TagName="MetafileUploader"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/FontIconSelector.ascx" TagPrefix="cms" TagName="FontIconSelector" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <div class="form-group">
            <cms:CMSDropDownList ID="drpThumbnailType" runat="server" RepeatDirection="Horizontal" UseResourceStrings="true" AutoPostBack="True">
            </cms:CMSDropDownList>
        </div>
        <asp:PlaceHolder ID="plcMetaFile" runat="server">
            <cms:MetafileUploader ID="fileUploader" runat="server" />
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcCssClass" runat="server" Visible="false">
            <cms:FontIconSelector runat="server" ID="fontIconSelector" />
        </asp:PlaceHolder>
    </ContentTemplate>
</cms:CMSUpdatePanel>

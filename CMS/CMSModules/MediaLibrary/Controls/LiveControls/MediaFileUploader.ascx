<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MediaLibrary_Controls_LiveControls_MediaFileUploader"  Codebehind="MediaFileUploader.ascx.cs" %>

<div class="MediaFileUploader">
    <cms:LocalizedLabel runat="server" ID="lblInfo" CssClass="InfoLabel" EnableViewState="false"
        Visible="false" />
    <cms:LocalizedLabel runat="server" ID="lblError" CssClass="ErrorLabel" EnableViewState="false"
        Visible="false" />
    <div class="form-horizontal" role="form">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblFile" runat="server" EnableViewState="false" ResourceString="media.library.uploadfile"
                    DisplayColon="true" CssClass="control-label" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSFileUpload ID="fileUploader" runat="server" />

            </div>
        </div>
        <asp:PlaceHolder ID="plcPreview" runat="server" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblPreview" runat="server" DisplayColon="true" EnableViewState="false"
                        ResourceString="media.library.uploadpreview" CssClass="control-label" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSFileUpload ID="previewUploader" runat="server" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-value-cell-offset editing-form-value-cell">
                <cms:LocalizedButton ID="btnUpload" runat="server" ResourceString="general.upload"
                    ButtonStyle="Default" EnableViewState="false" />
            </div>
        </div>
    </div>
</div>

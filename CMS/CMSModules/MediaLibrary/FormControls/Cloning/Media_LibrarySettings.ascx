<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Media_LibrarySettings.ascx.cs"
    Inherits="CMSModules_MediaLibrary_FormControls_Cloning_Media_LibrarySettings" %>
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblFolderName" ResourceString="clonning.settings.medialibrary.foldername"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="txtFolderName" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox runat="server" ID="txtFolderName" />
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="plcFiles">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblFiles" ResourceString="clonning.settings.medialibrary.files"
                    EnableViewState="false" DisplayColon="true" AssociatedControlID="chkFiles" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chkFiles" Checked="true" />
            </div>
        </div>
    </asp:PlaceHolder>
</div>
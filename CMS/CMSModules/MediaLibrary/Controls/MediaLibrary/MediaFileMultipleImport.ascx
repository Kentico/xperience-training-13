<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MediaLibrary_Controls_MediaLibrary_MediaFileMultipleImport"
     Codebehind="MediaFileMultipleImport.ascx.cs" %>
<%@ Register Src="~/CMSInlineControls/MediaControl.ascx" TagPrefix="cms" TagName="MediaPreview" %>
<%@ Register Src="~/CMSInlineControls/ImageControl.ascx" TagPrefix="cms" TagName="ImagePreview" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>

<asp:Literal ID="ltlScript" runat="server"></asp:Literal>
<asp:Panel ID="pnlImportFilesContent" runat="server" CssClass="PageContent">
    <cms:MessagesPlaceHolder ID="plcMessError" runat="server" ShortID="me" />
    <div>
        <cms:LocalizedHeading runat="server" ID="lblProgress" Level="3" />
    </div>
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblImportFileName" runat="server" CssClass="control-label" EnableViewState="false"
                    ResourceString="general.filename" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtImportFileName" runat="server" MaxLength="250" EnableViewState="false" CssClass="form-control" />
                <span class="CMSValidator">
                    <cms:CMSRequiredFieldValidator ID="rfvImportFileName" runat="server" ValidationGroup="MediaFileImport"
                        ControlToValidate="txtImportFileName" Display="Dynamic" EnableViewState="false" />
                </span>
            </div>
        </div>
        <div class="form-group">

            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblImportFileTitle" runat="server" CssClass="control-label" EnableViewState="false" ResourceString="media.file.filetitle" />
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizableTextBox ID="txtImportFileTitle" runat="server" MaxLength="250" EnableViewState="false" />
                <span class="CMSValidator">
                    <cms:CMSRequiredFieldValidator ID="rfvImportFileTitle" runat="server" ValidationGroup="MediaFileImport"
                        ControlToValidate="txtImportFileTitle:cntrlContainer:textbox" Display="Dynamic" EnableViewState="false" />
                </span>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblImportFileDescription" runat="server" CssClass="control-label" EnableViewState="false" DisplayColon="true" ResourceString="general.description" />
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizableTextBox ID="txtImportFileDescription" runat="server" EnableViewState="false" TextMode="MultiLine" />
                <cms:LocalizedCheckBox ID="chkImportDescriptionToAllFiles" runat="server" EnableViewState="false"
                    CssClass="CheckBoxMovedLeft" ResourceString="media.file.import.toall" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell-offset editing-form-value-cell">
                <cms:CMSButton ID="btnImportFile" runat="server" EnableViewState="false" ButtonStyle="Primary"
                    OnClick="btnImportFile_Click" ValidationGroup="MediaFileImport" DisableAfterSubmit="true" />
                <cms:CMSButton ID="btnImportCancel" runat="server" EnableViewState="false" ButtonStyle="Primary"
                    OnClick="btnImportCancel_Click" />
            </div>
        </div>
        <asp:PlaceHolder ID="plcPreview" runat="server">
            <div class="form-group">
                <div class="editing-form-value-cell-offset editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkDisplayPreview" runat="server" ResourceString="media.import.showpreview"
                        EnableViewState="false" />
                    <asp:HiddenField ID="hdnPreviewType" runat="server" />
                </div>
                <div class="editing-form-value-cell-offset editing-form-value-cell">
                    <div id="divImagePreview" style="display: none;">
                        <div class="ImportPreview">
                            <cms:ImagePreview ID="imagePreview" runat="server" />
                        </div>
                        <div class="ImportPreviewLink">
                            <asp:HyperLink ID="lnkOpenImage" runat="server" EnableViewState="false"></asp:HyperLink>
                        </div>
                    </div>
                    <div id="divMediaPreview" style="display: none;">
                        <div class="ImportPreview">
                            <cms:MediaPreview ID="mediaPreview" runat="server" AutoPlay="false" />
                        </div>
                        <div class="ImportPreviewLink">
                            <asp:HyperLink ID="lnkOpenMedia" runat="server" EnableViewState="false"></asp:HyperLink>
                        </div>
                    </div>
                    <div id="divOtherPreview" style="display: none;">
                        <div class="ImportPreviewLink">
                            <asp:HyperLink ID="lnkOpenOther" runat="server" EnableViewState="false"></asp:HyperLink>
                        </div>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
</asp:Panel>

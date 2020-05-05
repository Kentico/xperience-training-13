<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MediaLibrary_Controls_MediaLibrary_MediaFileSingleImport"
     Codebehind="MediaFileSingleImport.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>

<cms:JQueryTabContainer ID="pnlTabs" runat="server" CssClass="Dialog_Tabs">
    <cms:JQueryTab ID="tabImport" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlNewFileContent" runat="server" CssClass="PageContent single-file-import-overflow">
                <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
                    <ContentTemplate>
                        <cms:MessagesPlaceHolder ID="plcMessInfo" runat="server" ShortID="me" />
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel ID="lblNewFileName" runat="server" EnableViewState="false" ResourceString="general.filename"
                                        DisplayColon="true" CssClass="control-label" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:CMSTextBox ID="txtNewFileName" runat="server" MaxLength="250" EnableViewState="false" />
                                    <asp:Label ID="lblErrorNew" runat="server" Visible="false" CssClass="form-control-error"
                                        EnableViewState="false" />
                                    <span class="CMSValidator">
                                        <cms:CMSRequiredFieldValidator ID="rfvNewFileName" runat="server" ControlToValidate="txtNewFileName"
                                            ValidationGroup="NewFileValidation" Display="Dynamic" EnableViewState="false" />
                                    </span>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel ID="lnlNewFileTitle" runat="server" EnableViewState="false" ResourceString="media.file.filetitle"
                                        DisplayColon="false" CssClass="control-label" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:LocalizableTextBox ID="txtNewFileTitle" runat="server" MaxLength="250" EnableViewState="false" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel ID="lblNewDescription" runat="server" EnableViewState="false"
                                        ResourceString="general.description" DisplayColon="true" CssClass="control-label" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:LocalizableTextBox ID="txtNewDescripotion" runat="server" TextMode="MultiLine" EnableViewState="false" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-value-cell-offset editing-form-value-cell">
                                    <cms:LocalizedButton ID="btnNew" runat="server" ButtonStyle="Primary" OnClick="btnNew_Click"
                                        ValidationGroup="NewFileValidation" EnableViewState="false" ResourceString="general.import" DisableAfterSubmit="true" />
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </asp:Panel>
        </ContentTemplate>
    </cms:JQueryTab>
</cms:JQueryTabContainer>
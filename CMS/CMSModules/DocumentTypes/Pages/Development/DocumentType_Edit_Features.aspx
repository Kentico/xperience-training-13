<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Features"
    Theme="Default" ValidateRequest="false" EnableEventValidation="false" CodeBehind="DocumentType_Edit_Features.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UIForm ID="form" runat="server" ObjectType="cms.documenttype" RefreshHeader="true" CssClass="form-horizontal">
        <LayoutTemplate>
            <h4>
                <cms:LocalizedLabel runat="server" ID="lblHeading" ResourceString="documenttype_edit_features.heading" DisplayColon="false"></cms:LocalizedLabel></h4>
            <cms:FormField runat="server" ID="fieldClassUsesPageBuilder" Field="ClassUsesPageBuilder" Layout="Inline" UseFFI="false">
                <div data-feature="pagebuilder">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel runat="server" ID="lblPageBuilder" ResourceString="documenttype_edit_features.pagebuilder.label" DisplayColon="true" AssociatedControlID="chbPageBuilder" CssClass="control-label editing-form-label"></cms:LocalizedLabel>
                            </div>
                            <div class="editing-form-value-cell">
                                <div class="settings-group-inline keep-white-space-fixed">
                                    <cms:CMSCheckBox runat="server" ID="chbPageBuilder" />
                                    <span class="info-icon">
                                        <asp:Label runat="server" ID="spanScreenReaderPageBuilder" CssClass="sr-only"></asp:Label>
                                        <cms:CMSIcon runat="server" ID="iconHelpPageBuilder" EnableViewState="false" class="icon-question-circle" aria-hidden="true"></cms:CMSIcon>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </cms:FormField>            

            <cms:FormField runat="server" ID="fieldHasURL" Field="ClassHasURL" Layout="Inline" UseFFI="false">
                <div data-feature="url">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel runat="server" ID="lblHasURL" ResourceString="documenttype_edit_features.url.label" DisplayColon="true" AssociatedControlID="chbUrl" CssClass="control-label editing-form-label"></cms:LocalizedLabel>
                            </div>
                            <div class="editing-form-value-cell">
                                <div class="settings-group-inline keep-white-space-fixed">
                                    <cms:CMSCheckBox runat="server" ID="chbUrl" />
                                    <span class="info-icon">
                                        <asp:Label runat="server" ID="spanScreenReaderHasUrl" CssClass="sr-only"></asp:Label>
                                        <cms:CMSIcon runat="server" ID="iconHelpHasUrl" EnableViewState="false" class="icon-question-circle" aria-hidden="true"></cms:CMSIcon>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </cms:FormField>

            <cms:FormField runat="server" ID="fieldMetadata" Field="ClassHasMetadata" Layout="Inline" UseFFI="false">
                <div data-feature="metadata">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel runat="server" ID="lblHasMetadata" ResourceString="documenttype_edit_features.metadata.label" DisplayColon="true" AssociatedControlID="chbMetadata" CssClass="control-label editing-form-label"></cms:LocalizedLabel>
                            </div>
                            <div class="editing-form-value-cell">
                                <div class="settings-group-inline keep-white-space-fixed">
                                    <cms:CMSCheckBox runat="server" ID="chbMetadata" />
                                    <span class="info-icon">
                                        <asp:Label runat="server" ID="spanScreenReaderMetadata" CssClass="sr-only"></asp:Label>
                                        <cms:CMSIcon runat="server" ID="iconHelpMetadata" EnableViewState="false" class="icon-question-circle" aria-hidden="true"></cms:CMSIcon>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </cms:FormField>

            <cms:FormField runat="server" ID="fieldNavigationItem" Field="ClassIsNavigationItem" Layout="Inline" UseFFI="false">
                <div data-feature="navigationitem">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel runat="server" ID="lblIsNavigationItem" ResourceString="documenttype_edit_features.navigationitem.label" DisplayColon="true" AssociatedControlID="chbNavigationItem" CssClass="control-label editing-form-label"></cms:LocalizedLabel>
                            </div>
                            <div class="editing-form-value-cell">
                                <div class="settings-group-inline keep-white-space-fixed">
                                    <cms:CMSCheckBox runat="server" ID="chbNavigationItem" />
                                    <span class="info-icon">
                                        <asp:Label runat="server" ID="spanScreenReaderNavigationItem" CssClass="sr-only"></asp:Label>
                                        <cms:CMSIcon runat="server" ID="iconHelpNavigationItem" EnableViewState="false" class="icon-question-circle" aria-hidden="true"></cms:CMSIcon>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </cms:FormField>

            <cms:FormSubmitButton runat="server" ID="btnSubmit" />
        </LayoutTemplate>
    </cms:UIForm>


</asp:Content>


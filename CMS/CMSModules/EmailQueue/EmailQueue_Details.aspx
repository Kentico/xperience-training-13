<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="EmailQueue - Details" Inherits="CMSModules_EmailQueue_EmailQueue_Details"
    Theme="Default" CodeBehind="EmailQueue_Details.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:PlaceHolder ID="plcDetails" runat="server">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblFrom" runat="server" ResourceString="emailqueue.detail.from"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label ID="lblFromValue" CssClass="form-control-text" runat="server" EnableViewState="false" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblTo" runat="server" ResourceString="emailqueue.detail.to"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label ID="lblToValue" CssClass="form-control-text" runat="server" EnableViewState="false" />
                </div>
            </div>
            <asp:PlaceHolder ID="plcCc" runat="server">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblCc" runat="server" ResourceString="emailqueue.detail.cc"
                            DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label ID="lblCcValue" CssClass="form-control-text" runat="server" EnableViewState="false" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcBcc" runat="server">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblBcc" runat="server" ResourceString="emailqueue.detail.bcc"
                            DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label ID="lblBccValue" CssClass="form-control-text" runat="server" EnableViewState="false" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcReplyTo" runat="server">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblReplyTo" runat="server" ResourceString="emailtemplate_edit.replyto"
                            DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label ID="lblReplyToValue" CssClass="form-control-text" runat="server" EnableViewState="false" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSubject" runat="server" ResourceString="general.subject"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label ID="lblSubjectValue" CssClass="form-control-text" runat="server" EnableViewState="false" />
                </div>
            </div>
            <asp:Panel id="pnlBody" CssClass="form-group" runat="server">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblBody" runat="server" ResourceString="emailqueue.detail.body"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label ID="lblBodyValue" runat="server" EnableViewState="false" Visible="false" />
                    <cms:CMSHtmlEditor ID="htmlTemplateBody" runat="server" Width="770px" Height="300px"
                        Visible="false" Enabled="false" ToolbarSet="Disabled" />
                </div>
            </asp:Panel>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblPlainText" runat="server" ResourceString="emailqueue.detail.plaintext"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label ID="lblPlainTextValue" CssClass="form-control-text" runat="server" EnableViewState="false" />
                </div>
            </div>
            <asp:PlaceHolder ID="plcAttachments" runat="server" Visible="false">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblAttachments" runat="server" ResourceString="emailqueue.detail.attachments"
                            DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Panel ID="pnlAttachmentsList" runat="server" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcErrorMessage" runat="server">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblErorrMessage" runat="server" ResourceString="emailqueue.detail.errormessage"
                            DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label ID="lblErrorMessageValue" CssClass="form-control-text" runat="server" EnableViewState="false" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <cms:LocalizedButton runat="server" ID="btnPrevious" ButtonStyle="Primary" Enabled="false"
        EnableViewState="false" ResourceString="general.back" />
    <cms:LocalizedButton runat="server" ID="btnNext" ButtonStyle="Primary"
        Enabled="false" EnableViewState="false" ResourceString="general.next" />
</asp:Content>
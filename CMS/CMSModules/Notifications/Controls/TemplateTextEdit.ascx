<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Notifications_Controls_TemplateTextEdit"
     Codebehind="TemplateTextEdit.ascx.cs" %>

<div class="form-horizontal">
    <asp:PlaceHolder runat="server" ID="plcSubject" EnableViewState="false">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSubject" DisplayColon="true"
                    EnableViewState="false" AssociatedControlID="txtSubject" ResourceString="general.subject" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtSubject" MaxLength="250" EnableViewState="false" />
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="plcHTMLText" EnableViewState="false">
        <div class="form-group">
            <div class="editing-form-label-cell label-full-width">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblHTMLText" DisplayColon="true"
                    EnableViewState="false" AssociatedControlID="htmlText" ResourceString="notification.template.html" />
            </div>
            <div class="editing-form-value-cell textarea-full-width">
                <cms:CMSHtmlEditor ID="htmlText" runat="server" Height="315px" />
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="plcPlainText" EnableViewState="false">
        <div class="form-group">
            <div class="editing-form-label-cell label-full-width">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblPlainText" DisplayColon="true"
                    EnableViewState="false" AssociatedControlID="txtPlainText" ResourceString="notification.template.plain" />
            </div>
            <div class="editing-form-value-cell textarea-full-width">
                <cms:CMSTextArea runat="server" ID="txtPlainText" Rows="19" EnableViewState="false" />
            </div>
        </div>
    </asp:PlaceHolder>
</div>
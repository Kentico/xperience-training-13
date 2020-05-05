<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_EmailQueue_SendEmail"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="SendEmail.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/FileUploader.ascx" TagName="FileUploader" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblFrom" runat="server" CssClass="control-label" EnableViewState="false"
                    ResourceString="general.fromemail" DisplayColon="true" ShowRequiredMark="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EmailInput ID="txtFrom" runat="server" />
                <cms:CMSRequiredFieldValidator ID="rfvFrom" runat="server" Display="dynamic" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblTo" runat="server" CssClass="control-label" EnableViewState="false"
                    ResourceString="general.toemail" DisplayColon="true" ShowRequiredMark="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EmailInput ID="txtTo" runat="server" AllowMultipleAddresses="True" />
                <cms:CMSRequiredFieldValidator ID="rfvTo" runat="server" Display="dynamic" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblCc" runat="server" CssClass="control-label" EnableViewState="false"
                    ResourceString="general.cc" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EmailInput ID="txtCc" runat="server" AllowMultipleAddresses="True" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblBcc" runat="server" CssClass="control-label" EnableViewState="false"
                    ResourceString="general.bcc" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EmailInput ID="txtBcc" runat="server" AllowMultipleAddresses="True" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblSubject" runat="server" CssClass="control-label" EnableViewState="false"
                    ResourceString="general.subject" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtSubject" runat="server" MaxLength="450" />
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="plcText">
            <div class="form-group">
                <div class="editing-form-label-cell label-full-width">
                    <cms:LocalizedLabel ID="lblText" runat="server" CssClass="control-label" EnableViewState="false"
                        ResourceString="general.text" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell textarea-full-width">
                    <cms:CMSHtmlEditor ID="htmlText" runat="server" Height="400px" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcPlainText">
            <div class="form-group">
                <div class="editing-form-label-cell label-full-width">
                    <cms:LocalizedLabel ID="lblPlainText" runat="server" CssClass="control-label" EnableViewState="false"
                        ResourceString="general.plaintext" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell textarea-full-width">
                    <cms:CMSTextArea ID="txtPlainText" runat="server" Rows="19" />
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
    <div class="content-block-50">
        <cms:LocalizedHeading runat="server" Level="4" ResourceString="general.attachments" />
        <cms:FileUploader ID="uploader" runat="server" />
    </div>
</asp:Content>
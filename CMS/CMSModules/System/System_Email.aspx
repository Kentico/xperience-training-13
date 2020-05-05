<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_System_Email"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" 
    Title="Administration - System - Email"  Codebehind="System_Email.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">            
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblServer" runat="server" EnableViewState="false" 
                    ResourceString="System_Email.SMTPServer" DisplayColon="true" AssociatedControlID="txtServer" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtServer" runat="server" />
                <cms:CMSRequiredFieldValidator ID="rfvServer" runat="server" ControlToValidate="txtServer"
                    Display="dynamic" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblUserName" runat="server" EnableViewState="false" 
                    ResourceString="System_Email.SMTPUserName" DisplayColon="true" AssociatedControlID="txtUserName" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtUserName" runat="server" MaxLength="200" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblPassword" runat="server" EnableViewState="false" 
                    ResourceString="System_Email.SMTPPassword" DisplayColon="true" AssociatedControlID="txtPassword" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtPassword" runat="server" TextMode="Password" />
            </div>
        </div>

        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSSL" runat="server" EnableViewState="false" 
                    ResourceString="System_Email.SSL" DisplayColon="true" AssociatedControlID="chkSSL" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chkSSL" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblFrom" runat="server" EnableViewState="false" 
                    ResourceString="System_Email.From" DisplayColon="true" AssociatedControlID="txtFrom" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EmailInput ID="txtFrom" runat="server" />
                <cms:CMSRequiredFieldValidator ID="rfvFrom" runat="server" Display="dynamic" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblTo" runat="server" EnableViewState="false" 
                    ResourceString="System_Email.To" DisplayColon="true" AssociatedControlID="txtTo" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EmailInput ID="txtTo" runat="server" />
                <cms:CMSRequiredFieldValidator ID="rfvTo" runat="server" Display="dynamic" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSubject" runat="server" EnableViewState="false" 
                    ResourceString="general.subject" DisplayColon="true" AssociatedControlID="txtSubject" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtSubject" runat="server" MaxLength="450" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblText" runat="server" EnableViewState="false" 
                    ResourceString="System_Email.Text" DisplayColon="true" AssociatedControlID="txtText" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextArea ID="txtText" runat="server" Rows="4" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblAttachment" runat="server" EnableViewState="false" 
                    ResourceString="System_Email.Attachment" DisplayColon="true" AssociatedControlID="FileUploader" />
            </div>
            <div class="editing-form-value-cell">
                <cms:Uploader ID="FileUploader" runat="server" BorderStyle="none" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">            
                <cms:LocalizedButton ID="btnSend" runat="server" ButtonStyle="Primary" OnClick="btnSend_Click"
                    ResourceString="System_Email.Send" EnableViewState="false" />
            </div>
        </div>
    </div>
</asp:Content>
<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_BizForms_Tools_BizForm_Edit_NotificationEmail"
    Theme="Default" ValidateRequest="false"  Codebehind="BizForm_Edit_NotificationEmail.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput"
    TagPrefix="cms" %>

<asp:Content ID="aS" runat="server" ContentPlaceHolderID="plcActions">
    <div class="control-group-inline header-actions-container">
        <cms:HeaderActions ID="menu" ShortID="m" runat="server" IsLiveSite="false" />
    </div>
</asp:Content>
<asp:Content ID="plcContent" runat="server" ContentPlaceHolderID="plcContent">
    <div class="form-horizontal">
        <div class="form-group">
            <cms:CMSCheckBox ID="chkSendToEmail" runat="server" AutoPostBack="true" OnCheckedChanged="chkSendToEmail_CheckedChanged" />
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblFromEmail" runat="server" EnableViewState="False" ResourceString="bizformgeneral.notification.sender" DisplayColon="True"  ShowRequiredMark="True" ToolTipResourceString="bizformgeneral.notification.senderdesc" AssociatedControlID="txtFromEmail" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EmailInput ID="txtFromEmail" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblToEmail" runat="server" EnableViewState="False" ShowRequiredMark="True"  ResourceString="bizformgeneral.notification.recipient" DisplayColon="true" ToolTipResourceString="bizformgeneral.notification.recipientdesc" AssociatedControlID="txtToEmail" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EmailInput ID="txtToEmail" runat="server" AllowMultipleAddresses="True"/>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSubject" runat="server" EnableViewState="False"  ShowRequiredMark="True"  ResourceString="general.subject" ToolTipResourceString="bizformgeneral.notification.subjectdesc"
                    DisplayColon="true" AssociatedControlID="txtSubject" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtSubject" runat="server" MaxLength="250" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblAttachDocs" runat="server" EnableViewState="False" ResourceString="BizForm_Edit_NotificationEmail.AttachUploadedDocs" ToolTipResourceString="bizformgeneral.notification.attachdocumentdesc"
                    DisplayColon="true" AssociatedControlID="chkAttachDocs" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkAttachDocs" runat="server" AutoPostBack="false" />
            </div>
        </div>
        <div class="form-group">
            <cms:CMSCheckBox ID="chkCustomLayout" runat="server" AutoPostBack="true" OnCheckedChanged="chkCustomLayout_CheckedChanged" />
        </div>
        <div class="form-group">
            <asp:Panel ID="pnlCustomLayout" runat="server" CssClass="form-custom-layout">
                <div class="control-group-inline">
                    <cms:LocalizedButton ID="btnGenerateLayout" runat="server" OnClientClick="SetContent(GenerateTableLayout()); return false;"
                        ButtonStyle="Default" ResourceString="Bizform_Edit_Autoresponder.btnGenerateLayout" />
                </div>
                <div class="control-group-inline">
                    <div class="editor">
                        <cms:CMSHtmlEditor FullPage="True" ID="htmlEditor" runat="server" Width="650px" Height="300px" />
                    </div>
                    <div class="fields">
                        <cms:LocalizedLabel ID="lblAvailableFields" runat="server" EnableViewState="false"
                            CssClass="input-label" ResourceString="Bizform_Edit_Autoresponder.AvailableFields" />
                        <cms:CMSListBox ID="lstAvailableFields" runat="server" CssClass="fields-list" Rows="13" />
                        <div class="btns-vertical">
                            <cms:LocalizedButton ID="btnInsertLabel" runat="server" ButtonStyle="Default" ResourceString="Bizform_Edit_Autoresponder.btnInsertLabel" />
                            <cms:LocalizedButton ID="btnInsertInput" runat="server" ButtonStyle="Default" ResourceString="Bizform_Edit_Autoresponder.btnInsertInput" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
    <asp:Literal ID="ltlConfirmDelete" runat="server" />
</asp:Content>

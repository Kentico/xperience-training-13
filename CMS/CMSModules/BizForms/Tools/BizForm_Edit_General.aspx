<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_BizForms_Tools_BizForm_Edit_General"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="BizForm General"
    Theme="Default"  Codebehind="BizForm_Edit_General.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedHeading ID="headGeneral" runat="server" Level="4" ResourceString="general.general" EnableViewState="False" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ShowRequiredMark="true" CssClass="control-label" ID="lblDisplayName" runat="server" EnableViewState="False" ResourceString="BizFormGeneral.DisplayName" AssociatedControlID="txtDisplayName" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtDisplayName" runat="server"
                    MaxLength="100" />
                <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="txtDisplayName"
                    Display="dynamic" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblCodeName" runat="server" EnableViewState="False" ResourceString="BizFormGeneral.lblCodeName" AssociatedControlID="txtCodeName" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CodeName ID="txtCodeName" runat="server" MaxLength="100"
                    ReadOnly="True" Enabled="False" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblTableName" runat="server" EnableViewState="False" ResourceString="BizFormGeneral.lblTableName" AssociatedControlID="txtTableName" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtTableName" runat="server" MaxLength="100"
                    ReadOnly="True" Enabled="False" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblClassName" runat="server" EnableViewState="False" ResourceString="BizFormGeneral.lblClassName" AssociatedControlID="txtClassName" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtClassName" runat="server" MaxLength="100"
                    ReadOnly="True" Enabled="False" />
            </div>
        </div>
    </div>
    <cms:LocalizedHeading ID="headFormSettings" runat="server" Level="4" ResourceString="general.formsettings" EnableViewState="False" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblAfterSubmited" runat="server" EnableViewState="false"
                    ResourceString="BizFormGeneral.lblAfterSubmited" AssociatedControlID="radDisplay" />
            </div>
            <div class="editing-form-value-cell">
                <div class="radio-list-vertical">
                        <cms:CMSRadioButton ID="radDisplay" runat="server" GroupName="grpAfterSubmited"
                            AutoPostBack="True" OnCheckedChanged="radDisplay_CheckedChanged" ResourceString="BizFormGeneral.radDisplay" />
                    <div class="selector-subitem">
                        <cms:LocalizableTextBox ID="txtDisplay" runat="server" MaxLength="200" />
                    </div>
                        <cms:CMSRadioButton ID="radRedirect" runat="server" GroupName="grpAfterSubmited"
                            AutoPostBack="True" OnCheckedChanged="radRedirect_CheckedChanged" ResourceString="BizFormGeneral.radRedirect" />
                    <div class="selector-subitem">
                        <cms:CMSTextBox ID="txtRedirect" runat="server" MaxLength="200" />
                    </div>
                    <cms:CMSRadioButton ID="radClear" runat="server" GroupName="grpAfterSubmited"
                        AutoPostBack="True" OnCheckedChanged="radClear_CheckedChanged" ResourceString="BizFormGeneral.ReloadForm" />
                    <cms:CMSRadioButton ID="radContinue" runat="server" GroupName="grpAfterSubmited"
                        AutoPostBack="True" OnCheckedChanged="radClear_CheckedChanged" ResourceString="BizFormGeneral.radContinue" />
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblButtonText" runat="server" EnableViewState="False" ResourceString="BizFormGeneral.lblButtonText" />
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizableTextBox ID="txtButtonText" runat="server" MaxLength="200" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSubmitButtonImage" runat="server" EnableViewState="False"
                    ResourceString="BizFormGeneral.lblSubmitButtonImage" AssociatedControlID="txtSubmitButtonImage" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtSubmitButtonImage" runat="server" MaxLength="200" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="btnOk_Click"
                    ResourceString="general.ok" />
            </div>
        </div>
    </div>
</asp:Content>

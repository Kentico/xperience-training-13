<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SettingsCategoryEdit.ascx.cs"
    Inherits="CMSModules_Modules_Controls_Settings_Category_SettingsCategoryEdit" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/SelectModule.ascx" TagName="SelectModule" TagPrefix="cms" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblCategoryDisplayName" runat="server" EnableViewState="false"
                ResourceString="general.displayname" AssociatedControlID="txtCategoryDisplayName"
                DisplayColon="true" ShowRequiredMark="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtCategoryDisplayName" runat="server"
                MaxLength="200" ValidationGroup="vgCategory" EnableViewState="false" />
            <cms:CMSRequiredFieldValidator ID="rfvCategoryDisplayName" runat="server" ControlToValidate="txtCategoryDisplayName:cntrlContainer:textbox"
                ValidationGroup="vgCategory" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblCategoryName" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="general.codename" AssociatedControlID="txtCategoryName" ShowRequiredMark="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtCategoryName" runat="server" MaxLength="100"
                ValidationGroup="vgCategory" EnableViewState="false" />
            <cms:CMSRequiredFieldValidator ID="rfvCategoryName" runat="server" ControlToValidate="txtCategoryName"
                ValidationGroup="vgCategory" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group" id="trIconPath" runat="server">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblCategoryIconPath" runat="server" EnableViewState="false"
                DisplayColon="true" ResourceString="settings.iconpath" AssociatedControlID="txtIconPath" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtIconPath" runat="server" MaxLength="200"
                EnableViewState="false" />
        </div>
    </div>
    <div class="form-group" id="trParentCategory" runat="server">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblKeyCategory" runat="server" EnableViewState="false" ResourceString="settings.parentcategoryname"
                DisplayColon="true" AssociatedControlID="drpCategory" />
        </div>
        <div class="editing-form-value-cell">
            <cms:SelectSettingsCategory ID="drpCategory" runat="server" DisplayOnlyCategories="false" />
        </div>
    </div>
    <asp:Panel CssClass="form-group" ID="pnlDevelopmentMode" runat="server">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblModule" runat="server" EnableViewState="false" ResourceString="general.Module"
                DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:SelectModule runat="server" ID="ucSelectModule" DisplayAllModules="true" />
        </div>
    </asp:Panel>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:FormSubmitButton ID="btnOk" runat="server" EnableViewState="false"
                ValidationGroup="vgCategory" ResourceString="General.OK" OnClick="btnOk_Click" />
        </div>
    </div>
</div>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
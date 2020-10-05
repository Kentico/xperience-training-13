<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="LocalizableTextBox.ascx.cs"
    Inherits="CMSFormControls_System_LocalizableTextBox" %>

<cms:ActionContainer runat="server" ID="cntrlContainer">
    <InputTemplate>
        <cms:CMSTextBox ID="textbox" runat="server" />
    </InputTemplate>
    <ActionsTemplate>
        <cms:CMSAccessibleButton runat="server" ID="btnLocalize" EnableViewState="false" IconCssClass="icon-plus" CssClass="btn-first btn-last" />
        <cms:CMSAccessibleButton runat="server" ID="btnOtherLanguages" EnableViewState="false" IconCssClass="icon-plus" CssClass="btn-first" />
        <cms:CMSAccessibleButton runat="server" ID="btnRemoveLocalization" EnableViewState="false" IconCssClass="icon-bin" CssClass="btn-last" />
    </ActionsTemplate>
</cms:ActionContainer>
<asp:HiddenField ID="hdnValue" runat="server" EnableViewState="false" />
<asp:HiddenField ID="hdnIdentifier" runat="server" EnableViewState="false" />
<cms:LocalizedLabel ID="lblError" runat="server" CssClass="form-control-error" EnableViewState="false" Visible="false" />

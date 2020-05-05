<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSFormControls_Images_ImageDialogSelector" CodeBehind="ImageDialogSelector.ascx.cs" %>
<cms:CMSPanel ID="pnlRadio" runat="server" CssClass="radio-list-vertical">
    <cms:CMSRadioButton ID="radImageNo" runat="server" ResourceString="general.no"
        GroupName="EnableImage" />
    <cms:CMSRadioButton ID="radImageSimple" runat="server" ResourceString="forum.settings.simpledialog"
        GroupName="EnableImage" />
    <cms:CMSRadioButton ID="radImageAdvanced" runat="server" ResourceString="forum.settings.advanceddialog"
        GroupName="EnableImage" />
</cms:CMSPanel>

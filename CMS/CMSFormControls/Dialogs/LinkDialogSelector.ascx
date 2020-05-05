<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSFormControls_Dialogs_LinkDialogSelector" CodeBehind="LinkDialogSelector.ascx.cs" %>
<cms:CMSPanel ID="pnlRadio" runat="server" CssClass="radio-list-vertical">
    <cms:CMSRadioButton ID="radUrlNo" runat="server" ResourceString="general.no"
        GroupName="EnableURL" />
    <cms:CMSRadioButton ID="radUrlSimple" runat="server" ResourceString="forum.settings.simpledialog"
        GroupName="EnableURL" />
    <cms:CMSRadioButton ID="radUrlAdvanced" runat="server" ResourceString="forum.settings.advanceddialog"
        GroupName="EnableURL" />
</cms:CMSPanel>

<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="AllowedExtensionsSelector.ascx.cs"
    Inherits="CMSFormControls_System_AllowedExtensionsSelector" %>
<cms:CMSCheckBox ID="chkInehrit" runat="server" CssClass="CheckBoxMovedLeft"
    ResourceString="attach.inheritfromsettings" Checked="true" />
<br />
<cms:CMSTextBox ID="txtAllowedExtensions" runat="server" /><br />
<cms:LocalizedLabel ID="lblExtExample" runat="server" ResourceString="attach.extensionexample" />

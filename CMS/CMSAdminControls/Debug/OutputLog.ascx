<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_OutputLog" CodeBehind="OutputLog.ascx.cs" %>
<cms:CMSPanel runat="server" EnableViewState="false" ID="pnlHeading" Visible="False" CssClass="debug-log-info">
    <cms:LocalizedLabel runat="server" ID="lblHeading" ResourceString="OutputLog.Info" EnableViewState="False" />
</cms:CMSPanel>
<cms:CMSPanel runat="server" EnableViewState="false" ID="pnlOutputInfo">
    <cms:LocalizedLabel runat="server" ID="lblSizeCaption" AssociatedControlID="lblSize" ResourceString="OutputLog.Info" EnableViewState="false" />
    <cms:LocalizedLabel runat="server" ID="lblSize" EnableViewState="false" />
</cms:CMSPanel>
<cms:CMSTextArea runat="server" ID="txtOutput" ReadOnly="True" Rows="10" EnableViewState="false" CssClass="debug-log-output" Wrap="true" />

<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_DataProtection_FormControls_ConsentAgreement"  Codebehind="ConsentAgreement.ascx.cs" %>

<cms:CMSCheckbox runat="server" ID="chkConsent" />
<asp:Literal runat="server" ID="litConsentText" EnableViewState="False" />
<asp:HiddenField runat="server" ID="hdnValueHolder" />
<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Wizard_Header"  Codebehind="Header.ascx.cs" %>
<div class="wizard-header">
    <div class="wizard-title">
        <asp:Label ID="lblTitle" runat="server" />
    </div>
    <div id="divDescription" runat="server" class="wizard-description">
        <cms:LocalizedHeading runat="server" ID="headHeader" Level="2" EnableViewState="False" />
        <asp:Label ID="lblDescription" CssClass="description-text" runat="server" />
    </div>
</div>

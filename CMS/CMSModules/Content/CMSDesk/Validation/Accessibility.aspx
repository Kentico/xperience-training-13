<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Accessibility.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Content_CMSDesk_Validation_Accessibility" Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/Validation/AccessibilityValidator.ascx" TagName="AccessibilityValidator"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="server" ID="cnt">
    <cms:AccessibilityValidator ID="validator" runat="server" Standard="WCAG2_0AA" ShortID="v" />
</asp:Content>

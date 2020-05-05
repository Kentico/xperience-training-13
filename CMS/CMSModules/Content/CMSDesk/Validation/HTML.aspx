<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="HTML.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Content_CMSDesk_Validation_HTML" Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/Validation/HTMLValidator.ascx" TagName="HTMLValidator"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="server" ID="cnt">
    <cms:HTMLValidator ID="validator" runat="server" ShortID="v" />
</asp:Content>

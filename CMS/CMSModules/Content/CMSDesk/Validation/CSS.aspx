<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="CSS.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Content_CMSDesk_Validation_CSS" Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/Validation/CssValidator.ascx" TagName="CSSValidator"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="server" ID="cnt">
    <cms:CSSValidator ID="validator" runat="server" ShortID="v" />
</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Links.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Content_CMSDesk_Validation_Links" Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/Validation/LinkChecker.ascx" TagName="LinkChecker"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="server" ID="cnt">
    <cms:LinkChecker ID="validator" runat="server" ShortID="v" />
</asp:Content>

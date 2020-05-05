<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_MyDesk_CheckedOut_Documents"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="My desk - Checked out pages"
     Codebehind="Documents.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Documents/Documents.ascx" TagName="CheckedOut"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <cms:CheckedOut runat="server" ID="ucCheckedOut" ListingType="CheckedOut" IsLiveSite="false" />
</asp:Content>

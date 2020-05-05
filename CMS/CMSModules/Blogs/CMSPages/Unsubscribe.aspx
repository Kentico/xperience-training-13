<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Blogs_CMSPages_Unsubscribe"
    Theme="Default"  Codebehind="Unsubscribe.aspx.cs" MasterPageFile="~/CMSMasterPages/LiveSite/SimplePage.master" %>

<%@ Register Src="~/CMSModules/Blogs/Controls/Unsubscription.ascx" TagName="Unsubscription"
    TagPrefix="cms" %>
<asp:Content ID="cnt" ContentPlaceHolderID="plcContent" runat="server">
    <cms:Unsubscription ID="unsubscription" runat="server" />
</asp:Content>

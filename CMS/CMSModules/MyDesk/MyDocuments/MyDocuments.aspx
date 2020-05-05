<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_MyDesk_MyDocuments_MyDocuments"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="MyDesk - My pages"
     Codebehind="MyDocuments.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Documents/Documents.ascx" TagName="MyDocuments"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <cms:MyDocuments runat="server" ID="ucMyDocuments" ListingType="MyDocuments" IsLiveSite="false" />
</asp:Content>

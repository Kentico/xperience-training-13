<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/LiveSite/SimplePage.master"
    Inherits="CMSModules_Newsletters_CMSPages_Unsubscribe" Theme="Default" 
    Title="Unsubscribe"  Codebehind="Unsubscribe.aspx.cs"  %>

<%@ Register Src="~/CMSWebParts/Newsletters/NewsletterUnsubscriptionWebPart.ascx" TagPrefix="cms" TagName="Unsubscription" %>

<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <cms:Unsubscription runat="server" />
</asp:Content>


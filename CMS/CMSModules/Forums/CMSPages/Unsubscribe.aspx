<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_CMSPages_Unsubscribe"
    Theme="Default" MasterPageFile="~/CMSMasterPages/LiveSite/SimplePage.master"
     Codebehind="Unsubscribe.aspx.cs" %>

<%@ Register Src="~/CMSModules/Forums/Controls/Unsubscription.ascx" TagName="Unsubscription"
    TagPrefix="cms" %>
<asp:Content ID="cnt" ContentPlaceHolderID="plcContent" runat="server">
    <cms:Unsubscription ID="unsubscription" runat="server" />
</asp:Content>

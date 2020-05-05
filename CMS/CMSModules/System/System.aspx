<%@ Page Language="C#" AutoEventWireup="False" Inherits="CMSModules_System_System"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="System"
     Codebehind="System.aspx.cs" %>

<%@ Register Src="~/CMSModules/System/Controls/System.ascx" TagName="SystemInformation"
    TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent" EnableViewState="false">
    <cms:SystemInformation ID="sysInfo" runat="server" IsLiveSite="false" />
</asp:Content>

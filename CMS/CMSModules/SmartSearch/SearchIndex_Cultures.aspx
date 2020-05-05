<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_SearchIndex_Cultures"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Search Index - Cultures List"
     Codebehind="SearchIndex_Cultures.aspx.cs" %>

<%@ Register Src="~/CMSModules/SmartSearch/Controls/UI/SearchIndex_Cultures.ascx"
    TagName="CulturesList" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CulturesList ID="CulturesList" runat="server" IsLiveSite="false" />
</asp:Content>

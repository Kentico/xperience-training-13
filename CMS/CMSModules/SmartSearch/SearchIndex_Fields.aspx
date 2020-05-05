<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_SmartSearch_SearchIndex_Fields" Title="Search Index - Search fields"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" 
    Theme="Default"  Codebehind="SearchIndex_Fields.aspx.cs" %>

<%@ Register Src="~/CMSModules/SmartSearch/Controls/Edit/SearchFields.ascx"
    TagName="SearchFields" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlContent">
        <cms:SearchFields ID="searchFields" runat="server" IsLiveSite="false" />
    </asp:Panel>
</asp:Content>

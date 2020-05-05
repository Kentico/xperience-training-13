<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_SmartSearch_SearchIndex_Content_List"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Search Index - Content List" Theme="Default"  Codebehind="SearchIndex_Content_List.aspx.cs" %>

<%@ Register Src="~/CMSModules/SmartSearch/Controls/UI/SearchIndex_Content_List.ascx"
    TagName="ContentList" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/SmartSearch/Controls/UI/SearchIndex_User.ascx"
    TagName="UserList" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/SmartSearch/Controls/UI/SearchIndex_CustomTable_List.ascx"
    TagName="CustomTableList" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/SmartSearch/Controls/UI/SearchIndex_OnLineForm_List.ascx"
    TagName="OnLineFormList" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/SmartSearch/Controls/UI/General_List.ascx"
    TagName="GeneralList" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/SmartSearch/Controls/UI/SearchIndex_Custom.ascx"
    TagName="CustomList" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <cms:ContentList ID="contentList" runat="server" Visible="false" StopProcessing="true" IsLiveSite="false" />    
    <cms:UserList ID="userList" runat="server" Visible="false" StopProcessing="true" IsLiveSite="false" />
    <cms:CustomTableList ID="customTableList" runat="server" Visible="false" StopProcessing="true" IsLiveSite="false" />
    <cms:OnLineFormList ID="onLineFormList" runat="server" Visible="false" StopProcessing="true" IsLiveSite="false" />
    <cms:GeneralList ID="generalList" runat="server" Visible="false" StopProcessing="true" IsLiveSite="false" />
    <cms:CustomList ID="customList" runat="server" Visible="false" StopProcessing="true" IsLiveSite="false" />
</asp:Content>

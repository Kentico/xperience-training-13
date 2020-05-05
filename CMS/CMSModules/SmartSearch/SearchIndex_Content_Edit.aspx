<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_SmartSearch_SearchIndex_Content_Edit"
    ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" Title="Search Index - Edit"  Codebehind="SearchIndex_Content_Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/SmartSearch/Controls/UI/SearchIndex_Content_Edit.ascx"
    TagName="ContentEdit" TagPrefix="cms" %>
    <%@ Register Src="~/CMSModules/SmartSearch/Controls/UI/SearchIndex_CustomTable_Edit.ascx"
    TagName="CustomTableEdit" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/SmartSearch/Controls/UI/SearchIndex_OnLineForm_Edit.ascx"
    TagName="OnLineFormEdit" TagPrefix="cms" %>
<asp:content id="cntBody" contentplaceholderid="plcContent" runat="server">
    <cms:ContentEdit ID="ContentEdit" runat="server" Visible="false" StopProcessing="true" IsLiveSite="false" />
    <cms:CustomTableEdit ID="customTableEdit" runat="server" Visible="false" StopProcessing="true" IsLiveSite="false" />
    <cms:OnLineFormEdit ID="onLineFormEdit" runat="server" Visible="false" StopProcessing="true" IsLiveSite="false" />
</asp:content>
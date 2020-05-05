<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Settings_Pages_Categories"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Title="Settings"
     Codebehind="Categories.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/Trees/TreeBorder.ascx" TagName="TreeBorder"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Settings/Controls/SettingsTree.ascx" TagName="SettingsTree"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:TreeBorder ID="borderElem" runat="server" FramesetName="colsFrameset" />
    <cms:SettingsTree ID="TreeViewCategories" ShortID="t" runat="server" CssClass="ContentTree"
        CategoryName="CMS.Settings" MaxRelativeLevel="10" JavaScriptHandler="NodeSelected" ShowEmptyCategories="false" ShowSiteSelector="True" ShowHeaderPanel="False"/>
</asp:Content>

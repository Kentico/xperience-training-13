<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_SmartSearch_Controls_UI_SearchIndex_Content_List"  Codebehind="SearchIndex_Content_List.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" tagname="UniGrid" tagprefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<asp:Panel ID="pnlBody" runat="server">
    <cms:DisabledModule runat="server" ID="ucDisabledModule" />
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <cms:UniGrid runat="server" ID="UniGrid" GridName="~/CMSModules/SmartSearch/Controls/UI/SearchIndex_Content_List.xml"
        IsLiveSite="false" ExportFileName="cms_searchindexsettings"/>
</asp:Panel>

<%@ Page Language="C#" AutoEventWireup="false" Async="true" Inherits="CMSModules_System_Debug_System_DebugCacheItems"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Group list"
    MaintainScrollPositionOnPostback="true"  Codebehind="System_DebugCacheItems.aspx.cs" %>
<%@ Register Src="CacheItemsGrid.ascx" TagName="CacheItemsGrid" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcActions" runat="server">
    <asp:Panel runat="server" ID="pnlHeaderActions" CssClass="header-actions-container">
        <cms:CMSButton runat="server" ID="btnClear" OnClick="btnClear_Click" ButtonStyle="Default" EnableViewState="false" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" style="padding-bottom: 32px">
        <cms:LocalizedHeading runat="server" ID="headItems" Level="4" ResourceString="Debug.DataItems" EnableViewState="false" />
        <cms:CacheItemsGrid ID="gridItems" ShortID="gi" runat="server" IsLiveSite="false" />
    </asp:Panel>
    <asp:Panel runat="server">
        <cms:LocalizedHeading runat="server" ID="headDummy" Level="4" ResourceString="Debug.DummyKeys" EnableViewState="false" />
        <cms:CacheItemsGrid ID="gridDummy" ShortID="gd" runat="server" ShowDummyItems="true" IsLiveSite="false" />
    </asp:Panel>
</asp:Content>


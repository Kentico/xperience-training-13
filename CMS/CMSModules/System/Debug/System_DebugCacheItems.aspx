<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_Debug_System_DebugCacheItems"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Group list"
    MaintainScrollPositionOnPostback="true"  Codebehind="System_DebugCacheItems.aspx.cs" %>
<%@ Register Src="CacheItemsGrid.ascx" TagName="CacheItemsGrid" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcActions" runat="server">
    <div class="header-actions-container">
        <cms:CMSButton runat="server" ID="btnClear" OnClick="btnClear_Click" ButtonStyle="Default"
            EnableViewState="false" />
    </div>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedHeading runat="server" ID="headItems" Level="4" ResourceString="Debug.DataItems" EnableViewState="false" />
    <cms:CacheItemsGrid ID="gridItems" ShortID="gi" runat="server" IsLiveSite="false" />
    <cms:LocalizedHeading runat="server" ID="headDummy" Level="4" ResourceString="Debug.DummyKeys" EnableViewState="false" />
    <cms:CacheItemsGrid ID="gridDummy" ShortID="gd" runat="server" ShowDummyItems="true" IsLiveSite="false" />
</asp:Content>


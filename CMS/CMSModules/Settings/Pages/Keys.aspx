<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Settings_Pages_Keys"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Settings"
    Theme="Default"  Codebehind="Keys.aspx.cs" %>

<%@ Register TagPrefix="cms" TagName="SettingsGroupViewer" Src="~/CMSModules/Settings/Controls/SettingsGroupViewer.ascx" %>
<%@ Register TagPrefix="cms" TagName="AnchorDropup" Src="~/CMSAdminControls/UI/PageElements/AnchorDropup.ascx" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlSearch" Visible="false" DefaultButton="btnSearch" CssClass="form-horizontal">
        <div class="control-group-inline">
            <cms:LocalizedLabel ID="lblSearch" runat="server" EnableViewState="false" AssociatedControlID="txtSearch" CssClass="sr-only" ResourceString="general.search" />
            <cms:CMSTextBox ID="txtSearch" runat="server" EnableViewState="true" />
            <cms:CMSButton ID="btnSearch" runat="server" Text="Search" ButtonStyle="Primary" OnClick="btnSearch_OnClick" EnableViewState="true" />
        </div>
        <div class="control-group-inline">
            <cms:CMSCheckBox ID="chkDescription" runat="server" ResourceString="webparts.searchindescription" Checked="false" />
        </div>
    </asp:Panel>
    <cms:LocalizedLabel ID="lblNoSettings" CssClass="InfoLabel" runat="server" ResourceString="Development.Settings.NoSettingsInCat"
        DisplayColon="false" EnableViewState="false" Visible="false" />
    <cms:SettingsGroupViewer ID="SettingsGroupViewer" ShortID="s" runat="server" />
    <cms:AnchorDropup ID="anchorDropup" runat="server" MinimalAnchors="5" />
</asp:Content>

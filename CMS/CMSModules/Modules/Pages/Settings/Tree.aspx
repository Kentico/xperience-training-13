<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Tree.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Inherits="CMSModules_Modules_Pages_Settings_Tree"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/Settings/Controls/SettingsTree.ascx" TagName="SettingsTree"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Trees/TreeBorder.ascx" TagName="TreeBorder"
    TagPrefix="cms" %>
<asp:Content ID="plcContent" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:SettingsTree ID="treeSettings" runat="server" MaxRelativeLevel="10" JavaScriptHandler="NodeSelected" CategoryName="CMS.Settings" RootIsClickable="True" ShowSiteSelector="False" />
    <cms:TreeBorder ID="borderElem" runat="server" FramesetName="colsFrameset" />
</asp:Content>

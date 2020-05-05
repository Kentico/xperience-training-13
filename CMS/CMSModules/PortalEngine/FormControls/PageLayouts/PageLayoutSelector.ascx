<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_PortalEngine_FormControls_PageLayouts_PageLayoutSelector"  Codebehind="PageLayoutSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:UniSelector runat="server" ID="uniselect" SelectionMode="SingleDropDownList"
    ObjectType="cms.layout" ResourcePrefix="layoutselect" AllowEmpty="false" ReturnColumnName="LayoutID"
    OnOnSelectionChanged="uniselect_OnSelectionChanged" />

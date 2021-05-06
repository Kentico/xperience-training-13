<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_Controls_UI_SearchIndex_General"  Codebehind="SearchIndex_General.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms" TagName="DisabledModule" %>
<cms:DisabledModule runat="server" ID="ucDisabledModule" TestSettingKeys="CMSSearchIndexingEnabled" KeyScope="Global" />
<cms:UIForm ID="form" runat="server" AlternativeFormName="EditForm" ObjectType="cms.searchindex" RedirectUrlAfterCreate="SearchIndex_Frameset.aspx?indexId={%EditedObject.ID%}&saved=1" FieldGroupHeadingIsAnchor="false" RefreshHeader="True" />

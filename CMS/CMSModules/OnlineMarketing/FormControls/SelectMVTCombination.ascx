<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SelectMVTCombination.ascx.cs"
    Inherits="CMSModules_OnlineMarketing_FormControls_SelectMVTCombination" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional" Class="combination-selector">
    <ContentTemplate>
        <cms:UniSelector runat="server" ID="ucUniSelector" ShortID="s" ObjectType="OM.MVTCombination" SelectionMode="SingleDropDownList" UseTypeCondition="false"
            ResourcePrefix="selectmvtcombination" MaxDisplayedItems="1000" MaxDisplayedTotalItems="1000" UseUniSelectorAutocomplete="false"
            ReturnColumnName="MVTCombinationName" DisplayNameFormat="{%MVTCombinationCustomName%}" DefaultDisplayNameFormat="{%MVTCombinationName%}" OrderBy="MVTCombinationName" EmptyReplacement="" />
    </ContentTemplate>
</cms:CMSUpdatePanel>


<%@ Control Language="C#" AutoEventWireup="True"  Codebehind="List.ascx.cs" Inherits="CMSModules_OnlineMarketing_Controls_UI_ContentPersonalizationVariant_List" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<cms:LocalizedLabel runat="server" ID="lblError" CssClass="ErrorLabel" EnableViewState="false"
    Visible="false" />
<cms:UniGrid runat="server" ID="gridElem" Query="om.personalizationvariant.selectall" OrderBy="VariantPosition"
    Columns="VariantID,VariantEnabled,VariantDisplayName,VariantName" IsLiveSite="false" EditActionUrl="Edit.aspx?variantId={0}">
    <GridActions Parameters="VariantID">
        <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
        <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
        <ug:Action Name="up" Caption="$General.Up$" FontIconClass="icon-chevron-up" />
        <ug:Action Name="down" Caption="$General.Down$" FontIconClass="icon-chevron-down" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="VariantDisplayName" Caption="$general.displayName$" Wrap="false" />
        <ug:Column Source="VariantEnabled" Caption="$general.enabled$" Wrap="false" ExternalSourceName="#yesno" />
        <ug:Column CssClass="filling-column" />
    </GridColumns>
</cms:UniGrid>

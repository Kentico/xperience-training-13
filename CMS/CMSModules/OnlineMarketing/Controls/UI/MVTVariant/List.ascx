<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_OnlineMarketing_Controls_UI_MVTVariant_List"  Codebehind="List.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<cms:UniGrid ID="gridElem" runat="server" Query="om.mvtvariant.selectall" OrderBy="MVTVariantDisplayName"
    Columns="MVTVariantID,MVTVariantDisplayName,MVTVariantName,MVTVariantEnabled" IsLiveSite="false" EditActionUrl="Edit.aspx?mvtvariantid={0}&nodeid={?nodeid?}&varianttype={?varianttype?}">
    <GridActions Parameters="MVTVariantID">
        <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
        <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="MVTVariantDisplayName" Caption="$general.displayName$" Wrap="false" />
        <ug:Column Source="MVTVariantEnabled" Caption="$general.enabled$" Wrap="false" ExternalSourceName="#yesno" />
        <ug:Column CssClass="filling-column" />
    </GridColumns>
</cms:UniGrid>

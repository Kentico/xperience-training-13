<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_WebAnalytics_Controls_UI_Conversion_List"  Codebehind="List.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<cms:UniGrid runat="server" ID="gridElem" ObjectType="analytics.conversion" OrderBy="ConversionDisplayName"
    Columns="ConversionID,ConversionDisplayName, ISNULL(HitsCount,0) AS HitsCount, ISNULL (HitsValues,0) AS HitsValues" IsLiveSite="false" Query="Analytics.Conversion.selectwithviews">
    <GridActions Parameters="ConversionID">
        <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow"  OnClick="if (parent.updateTabHeader != null) parent.updateTabHeader();"/>
        <ug:Action Name="#delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" ModuleName="CMS.WebAnalytics" Permissions="ManageConversions" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="ConversionDisplayName" Caption="$conversion.name$" Wrap="false">
            <Filter Type="text" />
        </ug:Column>
        <ug:Column Source="HitsCount" Caption="$general.count$" Wrap="false" />
        <ug:Column Source="HitsValues" Caption="$general.value$" Wrap="false" />
        <ug:Column CssClass="filling-column" />
    </GridColumns>
    <GridOptions DisplayFilter="true" />
</cms:UniGrid>

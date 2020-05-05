<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_OnlineMarketing_Controls_UI_AbTest_List"
     Codebehind="List.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<cms:UniGrid ID="gridElem" runat="server" OnOnAction="gridElem_OnOnAction" OrderBy="ABTestStatus"
    OnOnExternalDataBound="gridElem_OnExternalDataBound">
    <GridActions Parameters="ABTestID">
        <ug:Action Name="edit" Caption="$general.edit$" FontIconClass="icon-edit" FontIconStyle="Allow" ModuleName="CMS.ABTest" Permissions="Read" />
        <ug:Action Name="delete" Caption="$general.delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$general.confirmdelete$" CommandArgument="ABTestID" ModuleName="cms.abtest" Permissions="Manage" ExternalSourceName="delete" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="ABTestDisplayName" Caption="$abtesting.testname$" Wrap="false" CssClass="main-column-100" />
        <ug:Column Source="##ALL##" Caption="$abtesting.page$" Wrap="false" ExternalSourceName="page" Name="ABTestOriginalPage" />
        <ug:Column Source="ABTestCulture" Caption="$abtesting.culture$" Wrap="false" ExternalSourceName="#culturenamewithflag|{$general.allcultures$}"/>
        <ug:Column Source="ABTestStatus" Caption="$general.status$" Wrap="false" ExternalSourceName="status" />
        <ug:Column Source="##ALL##" Caption="$abtesting.visitortotal$" Wrap="false" ExternalSourceName="visitors" AllowSorting="false"/>
        <ug:Column Source="##ALL##" Caption="$abtesting.conversiontotal$" Wrap="false" ExternalSourceName="conversions" AllowSorting="false"/>
        <ug:Column Source="ABTestOpenFrom" Caption="$general.start$" Wrap="false"/>
        <ug:Column Source="ABTestOpenTo" Caption="$general.end$" Wrap="false" />
    </GridColumns>
</cms:UniGrid>

<%@ Control Language="C#" AutoEventWireup="true"
            Inherits="CMSModules_OnlineMarketing_Controls_UI_MVTest_List"  Codebehind="List.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<cms:UniGrid ID="gridElem" runat="server" Query="om.mvtest.selectwithhits" ObjectType="om.mvtest"
             OnOnExternalDataBound="gridElem_OnExternalDataBound" OrderBy="MVTestName">
    <GridActions Parameters="MVTestID">
        <ug:Action Name="edit" Caption="$general.edit$" FontIconClass="icon-edit" FontIconStyle="Allow" ModuleName="CMS.MVTest" Permissions="Read" CommandArgument="MVTestID" />
        <ug:Action Name="#delete" Caption="$general.delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$general.confirmdelete$" Permissions="Manage" ModuleName="CMS.MVTest" CommandArgument="MVTestID" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="MVTestDisplayName" Caption="$mvtest.testname$" Wrap="false" CssClass="main-column-100">
            <Filter Type="text" />
        </ug:Column>
        <ug:Column Source="MVTestPage" Caption="$mvtest.testpage$" Wrap="false" />
        <ug:Column Source="MVTestCulture" Caption="$general.culture$" Wrap="false" ExternalSourceName="#culturenamewithflag|{$general.allcultures$}" />
        <ug:Column Source="MVTestConversions" Caption="$mvtest.testConversions$" Wrap="false" ExternalSourceName="#htmlencode|0" />
        <ug:Column Source="##ALL##" Caption="$mvtest.targetconversions$" Wrap="false" ExternalSourceName="maxconversions" />
        <ug:Column Source="HitsValue" Caption="$mvtest.totalconversionvalue$" Wrap="false" ExternalSourceName="#htmlencode|0" />
        <ug:Column Source="MVTestOpenFrom" Caption="$general.start$" Wrap="false" />
        <ug:Column Source="MVTestOpenTo" Caption="$general.end$" Wrap="false" />
        <ug:Column Source="MVTestID" ExternalSourceName="mvteststatus" Caption="$general.status$" Wrap="false" AllowSorting="false" />
    </GridColumns>
    <GridOptions DisplayFilter="true" />
</cms:UniGrid>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MediaFileUsage.ascx.cs" 
    Inherits="CMSModules_MediaLibrary_Controls_MediaLibrary_MediaFileUsage" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid"
    TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>


<cms:UniGrid ID="UsageGrid" runat="server" DelayedReload="true">
    <GridActions>
        <ug:Action Name="edit" Caption="$general.edit$" FontIconClass="icon-edit" FontIconStyle="allow" ExternalSourceName="edit" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="UsageObjectName" Caption="$medialibrary.dependencytracker.usageobjectname$" Wrap="false" runat="server" />
        <ug:Column Source="UsageObjectType" Caption="medialibrary.dependencytracker.usageobjecttype" Wrap="false" runat="server" />
        <ug:Column Source="UsageLocation" Caption="medialibrary.dependencytracker.usagelocation" Wrap="false" runat="server" Width="100%" AllowSorting="false" />
        <ug:Column Source="UsageSiteName" Caption="medialibrary.dependencytracker.usagesitename" Wrap="false" runat="server" />
    </GridColumns>
    <PagerConfig />
    <GridOptions />
</cms:UniGrid>
<asp:Literal ID="ltlUsageLoading" EnableViewState="false" runat="server" />
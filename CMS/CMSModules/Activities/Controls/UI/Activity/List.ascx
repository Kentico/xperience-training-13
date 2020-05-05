<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="List.ascx.cs" Inherits="CMSModules_Activities_Controls_UI_Activity_List" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/Activities/Controls/UI/Activity/Filter.ascx" TagPrefix="cms" TagName="ActivityFilter" %>

<asp:Panel ID="pnlUpdate" runat="server">
    <cms:UniGrid runat="server" ID="gridElem" IsLiveSite="false" HideFilterButton="true" 
        Columns="ActivityID,ActivityTitle,ActivityType,ActivityCreated,ActivitySiteID" RememberStateByParam="issitemanager">
        <GridActions Parameters="ActivityID">
            <ug:Action Name="view" ExternalSourceName="view" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="#delete" ExternalSourceName="delete" CommandArgument="ActivityID"
                Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$"
                ModuleName="CMS.Activities" Permissions="ManageActivities" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="ActivityTitle" Caption="$om.activity.gridtitle$" Wrap="false" />
            <ug:Column Source="ActivityType" ExternalSourceName="acttype" Caption="$general.type$"
                Wrap="false">
                <Tooltip Source="ActivityType" ExternalSourceName="acttypedesc" />
            </ug:Column>
            <ug:Column Source="ActivityCreated" Caption="$om.activity.activitytime$" Wrap="false" />
            <ug:Column Source="ActivitySiteID" ExternalSourceName="#sitenameorglobal" AllowSorting="false"
                Caption="$general.sitename$" Wrap="false" Name="sitename" Localize="true">
            </ug:Column>
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions DisplayFilter="true" FilterPath="~/CMSModules/Activities/Controls/UI/Activity/Filter.ascx" />
    </cms:UniGrid>
</asp:Panel>
<asp:Panel ID="pnlFooter" runat="server" Visible="false" EnableViewState="false" CssClass="form-horizontal mass-action">
    <div class="form-group">
        <div class="mass-action-value-cell">
            <cms:LocalizedLabel runat="server" AssociatedControlID="drpWhat" CssClass="sr-only" ResourceString="general.scope" />
            <cms:CMSDropDownList ID="drpWhat" runat="server" />
            <cms:LocalizedLabel runat="server" AssociatedControlID="drpAction" CssClass="sr-only" ResourceString="general.action" />
            <cms:CMSDropDownList ID="drpAction" runat="server" />
            <cms:LocalizedButton ID="btnOk" runat="server" ResourceString="general.ok" ButtonStyle="Primary"
                EnableViewState="false" />
        </div>
    </div>
    <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" EnableViewState="false" />
</asp:Panel>
<asp:HiddenField ID="hdnIdentifier" runat="server" EnableViewState="false" />
<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="List.ascx.cs" Inherits="CMSModules_Scheduler_Controls_UI_List" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<cms:UniGrid ID="gridElem" runat="server" IsLiveSite="false" OrderBy="TaskDisplayName"
    Columns="TaskID, TaskDisplayName, TaskLastRunTime, TaskNextRunTime, TaskLastResult, TaskServerName, TaskExecutions, TaskUseExternalService, TaskEnabled, TaskInterval">
    <GridActions>
        <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
        <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
        <ug:Action Name="execute" Caption="$Unigrid.Task.Actions.Execute$" ExternalSourceName="runactions"
            FontIconClass="icon-triangle-right" FontIconStyle="Allow" />
        <ug:Action Name="useexternalservice" Caption="$scheduledtask.useservicewarning$"
            ExternalSourceName="useexternalservice" FontIconClass="icon-exclamation-triangle" FontIconStyle="Warning" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="TaskDisplayName" Caption="$Unigrid.Task.Columns.TaskName$" Wrap="false"
            Localize="true">
            <Filter Type="Text" />
        </ug:Column>
        <ug:Column Source="TaskLastRunTime" Caption="$Unigrid.Task.Columns.TaskLastRunTime$"
            Wrap="false" />
        <ug:Column Source="TaskNextRunTime" Caption="$Unigrid.Task.Columns.TaskNextRunTime$"
            Wrap="false" />
        <ug:Column Source="TaskLastResult" Caption="$Unigrid.Task.Columns.TaskLastResult$"
            IsText="true" CssClass="main-column-100" />
        <ug:Column Source="TaskServerName" Caption="$Unigrid.Task.Columns.TaskServerName$"
            Wrap="false" />
        <ug:Column Source="TaskExecutions" Caption="$Unigrid.Task.Columns.TaskExecutions$" ExternalSourceName="taskexecutions"
            Wrap="false" />
        <ug:Column Source="TaskEnabled" Caption="$Unigrid.Task.Columns.TaskEnabled$" ExternalSourceName="#yesno"
            Wrap="false" />
    </GridColumns>
    <GridOptions DisplayFilter="true" />
</cms:UniGrid>

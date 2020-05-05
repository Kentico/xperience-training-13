<%@ Control Language="C#" AutoEventWireup="false"  Codebehind="List.ascx.cs" Inherits="CMSModules_Workflows_Controls_UI_WorkflowStep_SourcePoint_List" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:MessagesPlaceholder ID="plcMess" runat="server" />
<cms:UniGrid ID="gridElem" runat="server" Columns="">
    <GridActions Parameters="GUID">
        <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
        <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$"
            ModuleName="CMS.WorkflowEngine" Permissions="modify" ExternalSourceName="allowaction" />
        <ug:Action Name="moveup" ExternalSourceName="allowaction" Caption="$General.MoveUp$" FontIconClass="icon-chevron-up" />
        <ug:Action Name="movedown" ExternalSourceName="allowaction" Caption="$General.MoveDown$" FontIconClass="icon-chevron-down" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="Label" Caption="$workflowstep.sourcepoint.label$" ExternalSourceName="label" Width="50%" />
        <ug:Column Source="Condition" Caption="$workflowstep.sourcepoint.condition$" ExternalSourceName="condition" Width="50%" />
    </GridColumns>
</cms:UniGrid>
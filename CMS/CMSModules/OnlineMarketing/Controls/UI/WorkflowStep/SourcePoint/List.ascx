<%@ Control Language="C#" AutoEventWireup="false"  Codebehind="List.ascx.cs" Inherits="CMSModules_OnlineMarketing_Controls_UI_WorkflowStep_SourcePoint_List" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:MessagesPlaceholder ID="plcMess" runat="server" />
<cms:UniGrid ID="gridElem" runat="server" Columns="">
    <GridActions Parameters="GUID" Width="73px">
        <ug:Action Name="#move" Caption="$General.DragMove$" FontIconClass="icon-dots-vertical" ExternalSourceName="allowaction" />
        <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" ExternalSourceName="edit" />
        <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$"
            ModuleName="CMS.WorkflowEngine" Permissions="modify" ExternalSourceName="allowaction" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="Label" Caption="$workflowstep.sourcepoint.label$" ExternalSourceName="label" Width="120px" Style="max-width: 120px;" CssClass="text-ellipsis" Wrap="True" />
        <ug:Column Source="Condition" Caption="$workflowstep.sourcepoint.condition$" ExternalSourceName="condition" Width="247px" Style="max-width: 247px;" CssClass="text-ellipsis" />
    </GridColumns>
</cms:UniGrid>
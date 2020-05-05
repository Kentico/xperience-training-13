<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="List.ascx.cs" Inherits="CMSModules_Activities_Controls_UI_ActivityType_List" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:unigrid runat="server" id="gridElem" objecttype="om.activitytype" orderby="ActivityTypeDisplayName"
    columns="ActivityTypeID,ActivityTypeDisplayName,ActivityTypeEnabled,ActivityTypeIsCustom, ActivityTypeColor"
    islivesite="false" editactionurl="Tab_General.aspx?typeId={0}" RememberStateByParam="issitemanager">
    <GridActions Parameters="ActivityTypeID">
        <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
        <ug:Action Name="delete" ExternalSourceName="delete" Caption="$General.Delete$" CommandArgument="ActivityTypeID" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" ModuleName="CMS.Activities" Permissions="ManageActivities" />
    </GridActions>
    <GridColumns>
        <ug:Column Source="##ALL##" Caption="$general.name$" Wrap="false" Localize="true" ExternalSourceName="activityTypeName">
        <Filter Source="ActivityTypeDisplayName" Type="text" Size="250" />
        </ug:Column>
        <ug:Column Source="ActivityTypeEnabled" Caption="$general.enabled$" Wrap="false" ExternalSourceName="#yesno" />
        <ug:Column Source="ActivityTypeIsCustom" Caption="$general.iscustom$" Wrap="false" ExternalSourceName="#yesno">
        <Filter Type="bool" />
        </ug:Column>
        <ug:Column CssClass="filling-column" />
    </GridColumns>
    <GridOptions DisplayFilter="true" />
</cms:unigrid>

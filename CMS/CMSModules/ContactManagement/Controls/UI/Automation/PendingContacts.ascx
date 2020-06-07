<%@ Control Language="C#" AutoEventWireup="false"  Codebehind="PendingContacts.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_Automation_PendingContacts" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:UniGrid runat="server" ID="listElem" OrderBy="StateStatus, StateStepID" Columns="StateID, StateStepID, StateObjectID, StateWorkflowID, StateStatus, StateCreated, StateUserID"
    IsLiveSite="false" RememberStateByParam="issitemanager" OnOnAction="listElem_OnAction" OnOnExternalDataBound="listElem_OnExternalDataBound" OnOnDataReload="listElem_OnDataReload">
    <GridActions Parameters="StateID;StateObjectID">
        <ug:Action Name="edit" Caption="$ma.process.manage$" ExternalSourceName="edit" FontIconClass="icon-edit" FontIconStyle="Allow" />
        <ug:Action Name="dialogedit" Caption="$ma.process.manage$" ExternalSourceName="dialogedit" OnClick="viewPendingContactProcess({0}); return false;" FontIconClass="icon-edit" FontIconStyle="Allow" />
        <ug:Action Name="view" ExternalSourceName="view" Caption="$om.contact.viewdetail$" FontIconClass="icon-eye" FontIconStyle="Allow" CommandArgument="StateObjectID" />
        <ug:Action Name="delete" ExternalSourceName="delete" Caption="$autoMenu.RemoveState$" FontIconClass="icon-bin" FontIconStyle="Critical" />
    </GridActions>
    <GridColumns>
        <ug:Column Caption="$om.contact.firstname$" Source="StateObjectID" AllowSorting="false" Wrap="false" ExternalSourceName="#transform: om.contact : {%ContactFirstName%}" >
            <Filter Type="text" Source="ContactFirstName"/>
        </ug:Column>
        <ug:Column Caption="$om.contact.lastname$" Source="StateObjectID" AllowSorting="false" Wrap="false" ExternalSourceName="#transform: om.contact : {%ContactLastName%}">
            <Filter Type="text" Source="ContactLastName" />
        </ug:Column>
        <ug:Column Caption="$general.emailaddress$" Source="StateObjectID" AllowSorting="false" Wrap="false" ExternalSourceName="#transform: om.contact : {%ContactEmail%}">
            <Filter Type="text" Source="ContactEmail"/>
        </ug:Column>
        <ug:Column Caption="$Unigrid.Automation.Columns.ProcessName$" Source="StateWorkflowID" ExternalSourceName="#transform: cms.workflow.workflowdisplayname" Wrap="false" AllowSorting="false" />
        <ug:Column Caption="$Unigrid.Automation.Columns.StepName$" Source="StateStepID" ExternalSourceName="#transform: cms.workflowstep.stepdisplayname" AllowSorting="false" Wrap="false" />
        <ug:Column Source="StateCreated" ExternalSourceName="#userdatetimegmt" Caption="$Unigrid.Workflow.Columns.InitiatedWhen$" Wrap="false" />
        <ug:Column Source="StateUserID" Caption="$Unigrid.Workflow.Columns.InitiatedBy$" ExternalSourceName="#formattedusername|{$ma.automationstate.automatically$}" Wrap="false" />
        <ug:Column CssClass="filling-column" />
    </GridColumns>
    <GridOptions DisplayFilter="true" />
</cms:UniGrid>

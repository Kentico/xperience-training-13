<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Process_History.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Process_Process_History"
    MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" Title="Automation process – Process history" Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid ID="gridHistory" runat="server" OrderBy="HistoryID DESC" IsLiveSite="false" ObjectType="ma.automationhistory"
        Columns="HistoryStepName, HistoryStepType, HistoryStepDisplayName, HistoryTargetStepName, HistoryTargetStepType, HistoryTargetStepDisplayName, HistoryComment, HistoryWasRejected, HistoryApprovedWhen, HistoryApprovedByUserID, HistoryTransitionType"
        OnOnExternalDataBound="gridHistory_OnExternalDataBound">
        <GridColumns>
            <ug:Column Source="HistoryApprovedWhen" ExternalSourceName="#userdatetimegmt" Caption="$Unigrid.Workflow.Columns.ApprovedWhen$" Wrap="false">
                <Tooltip Source="HistoryApprovedWhen" ExternalSourceName="#usertimezonename" />
            </ug:Column>
            <ug:Column Source="##ALL##" ExternalSourceName="stepname" Caption="$Unigrid.Workflow.Columns.StepDisplayName$" Wrap="false" />
            <ug:Column Source="HistoryApprovedByUserID" ExternalSourceName="#formattedusername" Caption="$general.user$" Wrap="false" AllowSorting="false" />
            <ug:Column Source="HistoryComment" Caption="$Unigrid.Workflow.Workflow.Comment$" IsText="true" CssClass="main-column-100" AllowSorting="false" />
            <ug:Column Source="##ALL##" ExternalSourceName="Action" Caption="$general.action$" Wrap="false" />
        </GridColumns>
        <PagerConfig DefaultPageSize="10" />
    </cms:UniGrid>
</asp:Content>
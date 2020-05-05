<%@ Control Language="C#" AutoEventWireup="false"  Codebehind="Edit.ascx.cs" Inherits="CMSModules_Automation_Controls_Process_Edit" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Automation/AutomationDesigner.ascx" TagName="AutomationDesigner"
    TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Label ID="lblWorkflow" runat="server" CssClass="InfoLabel" EnableViewState="false" Visible="false" />
<asp:Panel ID="pnlWorkflow" runat="server">
    <table width="100%">
        <tr>
            <td colspan="2">
                <cms:LocalizedLabel ID="lblSteps" runat="server" CssClass="FormGroupHeader" EnableViewState="false"
                    ResourceString="ma.Steps" DisplayColon="true" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <cms:AutomationDesigner ID="ucDesigner" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <cms:LocalizedLabel ID="lblHistory" runat="server" CssClass="FormGroupHeader" EnableViewState="false" ResourceString="ma.History" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <cms:UniGrid ID="gridHistory" runat="server" OrderBy="HistoryID DESC" IsLiveSite="false"
                    DelayedReload="true" ObjectType="ma.automationhistory" Columns="HistoryStepName, HistoryStepType, HistoryStepDisplayName, HistoryTargetStepName, HistoryTargetStepType, HistoryTargetStepDisplayName, HistoryComment, HistoryWasRejected, HistoryApprovedWhen, HistoryApprovedByUserID, HistoryTransitionType">
                    <GridColumns>
                        <ug:Column Source="HistoryApprovedWhen" ExternalSourceName="#userdatetimegmt" Caption="$Unigrid.Workflow.Columns.ApprovedWhen$"
                            Wrap="false">
                            <Tooltip Source="HistoryApprovedWhen" ExternalSourceName="#usertimezonename" />
                        </ug:Column>
                        <ug:Column Source="##ALL##" ExternalSourceName="stepname" Caption="$Unigrid.Workflow.Columns.StepDisplayName$"
                            Wrap="false" />
                        <ug:Column Source="HistoryApprovedByUserID" ExternalSourceName="#formattedusername" Caption="$general.user$"
                            Wrap="false" />
                        <ug:Column Source="HistoryComment" Caption="$Unigrid.Workflow.Workflow.Comment$"
                            IsText="true" Wrap="false" CssClass="main-column-100" />
                        <ug:Column Source="##ALL##" ExternalSourceName="Action" Caption="$general.action$"
                            Wrap="false" />
                    </GridColumns>
                </cms:UniGrid>
            </td>
        </tr>
    </table>
</asp:Panel>

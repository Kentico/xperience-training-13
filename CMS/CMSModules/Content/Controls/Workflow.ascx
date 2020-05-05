<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Workflow.ascx.cs" Inherits="CMSModules_Content_Controls_Workflow" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Workflows/Controls/WorkflowDesigner.ascx" TagName="Designer"
    TagPrefix="cms" %>

<asp:Panel ID="pnlWorkflow" runat="server">
    <table width="100%">
        <tr>
            <td colspan="2">
                <cms:LocalizedHeading ID="headSteps" runat="server" Level="4" CssClass="listing-title" EnableViewState="false"
                    ResourceString="WorfklowProperties.Steps" DisplayColon="true" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:PlaceHolder ID="plcBasic" runat="server">
                    <cms:UniGrid ID="gridSteps" runat="server" OrderBy="StepOrder" IsLiveSite="false"
                        DelayedReload="true" ShowActionsMenu="false" />
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plcAdvanced" runat="server">
                    <cms:Designer ID="ucDesigner" runat="server" ReadOnly="true" Height="350px" />
                </asp:PlaceHolder>
            </td>
        </tr>
        <tr>
            <td colspan="2">&nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <cms:LocalizedHeading ID="headHistory" runat="server" Level="4" CssClass="listing-title" EnableViewState="false"
                    ResourceString="WorfklowProperties.History" DisplayColon="true" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <cms:UniGrid ID="gridHistory" runat="server" OrderBy="WorkflowHistoryID DESC" IsLiveSite="false"
                    DelayedReload="true" />
            </td>
        </tr>
    </table>
</asp:Panel>

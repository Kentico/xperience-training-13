<%@ Page Language="C#" AutoEventWireup="false" Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_List"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default"  Codebehind="List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/SmartTip.ascx" TagPrefix="cms" TagName="SmartTip" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:SmartTip runat="server" ID="tipHowMAWorks" Visible="true" />
    <cms:UniGrid ID="gridProcesses" runat="server" ObjectType="ma.automationprocess"
        Columns="WorkflowID, WorkflowDisplayName, WorkflowType, WorkflowRecurrenceType, WorkflowEnabled"
        OrderBy="WorkflowDisplayName, WorkflowID" IsLiveSite="false" OnOnAction="gridProcesses_OnAction" OnOnExternalDataBound="gridProcesses_OnExternalDataBound">
        <GridActions>
            <ug:Action Name="edit" Caption="$general.edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="delete" ExternalSourceName="delete" Caption="$general.delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                Confirmation="$ma.process.delete.confirm$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="WorkflowDisplayName" Caption="$ma.processname$" Wrap="false" Localize="true">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="WorkflowRecurrenceType" ExternalSourceName="recurrencetype" Caption="$cms.workflow.recurrency$"
                Wrap="false" Localize="true" AllowSorting="false" />
            <ug:Column Source="WorkflowEnabled" ExternalSourceName="#yesno" Caption="$general.enabled$" Localize="true" Wrap="false"/>
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>

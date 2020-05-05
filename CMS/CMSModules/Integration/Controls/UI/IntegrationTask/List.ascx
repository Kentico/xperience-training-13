<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Integration_Controls_UI_IntegrationTask_List"  Codebehind="List.ascx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog" TagPrefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <asp:Panel runat="server" ID="pnlLog" Visible="false">
            <cms:AsyncLog ID="ctlAsyncLog" runat="server" ProvideLogContext="true" LogContextNames="Integration" />
        </asp:Panel>
        <asp:Panel ID="pnlContent" runat="server">
            <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
            <cms:UniGrid runat="server" ID="gridElem" ObjectType="integration.tasklist" OrderBy="TaskID ASC" IsLiveSite="false" Columns="TaskID, TaskTitle, TaskTime, TaskType, SynchronizationErrorMessage, SynchronizationID, SynchronizationConnectorID">
                <GridActions Parameters="SynchronizationID">
                    <ug:Action Name="view" Caption="$General.View$" FontIconClass="icon-eye" FontIconStyle="Allow" ExternalSourceName="view" />
                    <ug:Action Name="run" Caption="$General.Synchronize$" FontIconClass="icon-rotate-double-right" CommandArgument="SynchronizationID" ExternalSourceName="run" />
                    <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" ModuleName="CMS.Integration" Permissions="modify" CommandArgument="SynchronizationID" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="TaskTitle" Caption="$integration.tasktitle$" Wrap="false" CssClass="main-column-100">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="TaskType" Caption="$integration.tasktype$" Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="TaskTime" Caption="$integration.tasktime$" Wrap="false" />
                    <ug:Column Source="SynchronizationConnectorID" Caption="$integration.connectorname$" Wrap="false"
                        ExternalSourceName="#transform: integration.connector.ConnectorDisplayName" Name="SynchronizationConnectorID" />
                    <ug:Column Source="##ALL##" Caption="$general.result$" ExternalSourceName="result" Wrap="false">
                        <Tooltip Source="SynchronizationErrorMessage" Encode="true" />
                    </ug:Column>
                </GridColumns>
                <GridOptions DisplayFilter="true" ShowSelection="true" SelectionColumn="SynchronizationID" />
            </cms:UniGrid>
        </asp:Panel>
        <asp:Panel ID="pnlFooter" runat="server" CssClass="form-horizontal mass-action">
            <div class="form-group">
                <div class="mass-action-value-cell">
                    <cms:CMSDropDownList ID="drpWhat" runat="server" />
                    <cms:CMSDropDownList ID="drpAction" runat="server" />
                    <cms:LocalizedButton ID="btnOk" runat="server" ResourceString="general.ok" ButtonStyle="Primary" EnableViewState="false" />
                </div>
            </div>
            <asp:Label ID="lblInfoBottom" runat="server" CssClass="InfoLabel" EnableViewState="false" />
        </asp:Panel>
    </ContentTemplate>
</cms:CMSUpdatePanel>
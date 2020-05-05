<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Polls_Controls_PollsList"
     Codebehind="PollsList.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Panel ID="pnlBody" runat="server">
    <cms:UniGrid runat="server" ID="UniGrid" ObjectType="polls.polllist" Columns="PollID, PollDisplayName, AnswerCount, PollSiteID, PollOpenFrom, PollOpenTo"
        OrderBy="PollDisplayName">
        <GridActions>
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="delete" ExternalSourceName="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="PollDisplayName" Caption="$general.displayname$" Wrap="false"
                Localize="true">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="AnswerCount" Caption="$unigrid.polls.columns.totalanswers$" Wrap="false" />
            <ug:Column Source="PollOpenFrom" Caption="$unigrid.events.columns.eventopenfrom$"
                Wrap="false">
                <Tooltip Source="PollOpenFrom" />
            </ug:Column>
            <ug:Column Source="PollOpenTo" Caption="$unigrid.events.columns.eventopento$" Wrap="false">
                <Tooltip Source="PollOpenTo" />
            </ug:Column>
            <ug:Column Source="##ALL##" ExternalSourceName="isglobal" Caption="$general.isglobal$"
                Wrap="false" />
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Panel>

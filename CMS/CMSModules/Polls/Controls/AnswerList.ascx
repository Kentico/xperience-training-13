<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Polls_Controls_AnswerList"
     Codebehind="AnswerList.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Panel ID="pnlBody" runat="server">
    <cms:UniGrid runat="server" ID="uniGrid" ObjectType="polls.pollanswer" OrderBy="AnswerOrder ASC"
        Columns="AnswerID, AnswerText, AnswerCount, AnswerEnabled, AnswerForm">
        <GridActions>
            <ug:Action Name="#move" Caption="$General.DragMove$" FontIconClass="icon-dots-vertical"/>
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" ExternalSourceName="edit"/>
            <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" ExternalSourceName="delete" />
            <ug:Action Name="viewresults" Caption="$Polls.ViewResults$" FontIconClass="icon-eye" FontIconStyle="Allow" ExternalSourceName="AnswerForm" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="AnswerText" Caption="$Unigrid.Polls_Answer.Columns.AnswerText$"
                Wrap="false" Localize="true">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="AnswerCount" Caption="$Unigrid.Polls_Answer.Columns.AnswerCount$"
                Wrap="false">
                <Filter Type="integer" />
            </ug:Column>
            <ug:Column Source="AnswerEnabled" ExternalSourceName="#yesno" Caption="$general.enabled$"
                Wrap="false" />
            <ug:Column Source="AnswerForm" ExternalSourceName="AnswerIsOpenEnded" Caption="$general.type$"
                Wrap="false" />
            <ug:Column CssClass="filling-column" />
        </GridColumns>
    </cms:UniGrid>
</asp:Panel>

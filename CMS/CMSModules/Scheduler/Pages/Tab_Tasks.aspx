<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Scheduler_Pages_Tab_Tasks"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Title="Scheduled tasks - Task List"
     Codebehind="Tab_Tasks.aspx.cs" %>

<%@ Register Src="~/CMSModules/Scheduler/Controls/UI/List.ascx" TagName="TaskList" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <cms:CMSPanel ID="pnlHeader" runat="server" FixedPosition="true">
        <div class="cms-edit-menu">
            <cms:HeaderActions ID="headerActions" runat="server" PanelCssClass="cms-edit-menu" />
            <cms:HeaderActions ID="rightHeaderActions" runat="server" PanelCssClass="cms-edit-menu" />
            <asp:Label ID="lblLastRun" runat="server" EnableViewState="false" CssClass="info-text" />
        </div>
    </cms:CMSPanel>
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="PanelUsers" runat="server" CssClass="PageContent">
                <cms:MessagesPlaceHolder ID="plcMess" runat="server" IsLiveSite="false" />
                <cms:TaskList ID="listElem" runat="server" SystemTasks="false" />
            </asp:Panel>
            <asp:Button runat="server" ID="btnRefresh" CssClass="HiddenButton" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>

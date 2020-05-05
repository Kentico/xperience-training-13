<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="EventLog.ascx.cs" Inherits="CMSModules_EventLog_Controls_EventLog" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:UniGrid runat="server" ID="gridEvents" GridName="~/CMSModules/EventLog/Controls/EventLog.xml"
            OrderBy="EventID DESC" IsLiveSite="false" ObjectType="cms.eventlog" />
        <asp:HiddenField ID="hdnIdentifier" runat="server" EnableViewState="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
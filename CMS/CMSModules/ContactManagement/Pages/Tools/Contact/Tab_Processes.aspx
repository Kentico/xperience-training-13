<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_ContactManagement_Pages_Tools_Contact_Tab_Processes"
    Theme="Default"  Codebehind="Tab_Processes.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/Automation/Controls/Process/List.ascx" TagName="ProcessList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Automation/FormControls/AutomationProcessSelector.ascx"
    TagName="ProcessSelector" TagPrefix="cms" %>
<asp:Content ID="cntActions" runat="server" ContentPlaceHolderID="plcActions">
    <div class="control-group-inline header-actions-container">
        <cms:ProcessSelector ID="ucSelector" runat="server" Enabled="false" IsLiveSite="false" />
    </div>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:ProcessList ID="listElem" runat="server" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>

<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AutomationDesigner.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_Automation_AutomationDesigner" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Automation/AutomationDesignerToolbar.ascx" TagName="AutomationDesignerToolbar" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/WebServiceChecker.ascx" TagName="WebServiceChecker" TagPrefix="cms" %>

<cms:WebServiceChecker ID="serviceChecker" runat="server" />

<asp:Panel ID="pnlHeader" runat="server" class="automation-header">
    <cms:CMSUpdatePanel ID="pnlState" runat="server">
        <ContentTemplate>
            <asp:Panel ID="icnState" runat="server" class="automation-header-icon" />
            <asp:Label ID="lblState" runat="server" EnableViewState="false" CssClass="bold-label" />
            <cms:CMSButton ID="btnToggleState" runat="server" ButtonStyle="Default" EnableViewState="false" OnClick="ToggleState" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>

<div class="automation-wrapper">
    <div class="automation-graph-wrapper">
        <cms:CMSUpdatePanel ID="pnlAutosaveWrapper" runat="server">
            <ContentTemplate>
                <cms:MessagesPlaceHolder runat="server" ID="pnlAutosave" IsLiveSite="false" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
        <cms:UniGraph runat="server" ID="uniGraph" CustomCssStylesheet="AutomationGraph.css" />
    </div>
    <cms:CMSPanel ID="toolbarContainer" ShortID="tc" runat="server">
        <cms:AutomationDesignerToolbar ID="toolbar" runat="server" />
    </cms:CMSPanel>
</div>

<div class="automation-step-properties">
    <div class="automation-step-properties-wrapper">
        <div class="automation-step-properties-slidable">
            <div class="automation-step-properties-loader">
                <%=ScriptHelper.GetLoaderHtml() %>
            </div>
            <iframe id="propertiesFrame" runat="server"></iframe>
        </div>
    </div>
</div>
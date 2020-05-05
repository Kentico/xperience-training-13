<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AutomationDesignerToolbarStep.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_Automation_AutomationDesignerToolbarStep" %>

<asp:Panel ID="pnlStep" runat="server" class="automation-step BigButton" EnableViewState="False">
    <asp:Literal ID="stepIcon" runat="server" />
    <div class='automation-step-content'>
        <asp:Label ID="stepTitle" class="automation-step-title" runat="server" />
    </div>
</asp:Panel>
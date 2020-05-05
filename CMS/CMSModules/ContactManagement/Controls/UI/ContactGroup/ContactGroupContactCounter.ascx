<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ContactGroupContactCounter.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_ContactGroup_ContactGroupContactCounter" %>
<cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional" EnableViewState="false">
    <ContentTemplate>
        <asp:Timer ID="timRefresh" runat="server" Interval="5000" EnableViewState="false" />
            <h4>
                <span runat="server" ID="lblCount" style="margin-right: 2em;"></span>
                <span runat="server" ID="lblRatio"></span>
            </h4>        
    </ContentTemplate>
</cms:CMSUpdatePanel>
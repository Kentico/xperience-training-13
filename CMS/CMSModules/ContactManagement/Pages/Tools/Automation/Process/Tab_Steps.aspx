<%@ Page Language="C#" AutoEventWireup="false"
    MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Title="Automation process – Steps"
    Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Process_Tab_Steps" Theme="Default" CodeBehind="Tab_Steps.aspx.cs" %>

<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">

    <asp:Panel ID="pnlHeader" runat="server" class="automation-header">
        <cms:CMSUpdatePanel ID="pnlState" runat="server">
            <ContentTemplate>
                <div class="btn-actions">
                    <div id="navigation" runat="server" class="btn-group">
                        <cms:CMSButton runat="server" ID="btnDesigner" ButtonStyle="Default" EnableViewState="false" />
                        <cms:CMSButton runat="server" ID="btnContacts" ButtonStyle="Default" EnableViewState="false" />
                    </div>
                </div>
                <asp:Panel ID="icnState" runat="server" class="automation-header-icon" />
                <asp:Label ID="lblState" runat="server" EnableViewState="false" CssClass="bold-label" />
                <cms:CMSButton ID="btnToggleState" runat="server" ButtonStyle="Default" EnableViewState="false" OnClick="ToggleState" />
                <cms:CMSMoreOptionsButton ID="btnMoreOptions" runat="server" DropDownItemsAlignment="Right" RenderFirstActionSeparately="false" CssClass="more-actions-button" Visible="false" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>

    <div class="main-view">
        <cms:UILayoutPane ID="mainView" runat="server" RenderAs="Iframe" Src="about:blank" />
    </div>

</asp:Content>
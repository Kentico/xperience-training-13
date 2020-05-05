<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_System_Deployment"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="System - Deployment"
     Codebehind="System_Deployment.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcBeforeBody" runat="server" ID="cntBeforeBody">
    <asp:Panel runat="server" ID="pnlLog" Visible="false">
        <cms:AsyncLog ID="ctlAsyncLog" runat="server" ProvideLogContext="true" LogContextNames="Deployment" />
    </asp:Panel>
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedHeading runat="server" ID="headDeployment" Level="4" Text="Deployment" EnableViewState="false" />
    <p>
        <asp:Label runat="server" ID="lblDeploymentInfo" EnableViewState="false" />
    </p>
    <div style="padding: 10px 0px 10px 0px;">
        <cms:LocalizedButton ID="btnSaveAll" ButtonStyle="Primary" runat="server"
            OnClick="btnSaveAll_Click" ResourceString="Deployment.SaveAll" EnableViewState="false" />
    </div>
    <cms:LocalizedHeading runat="server" ID="headSourceControl" Level="4" Text="Source control" EnableViewState="false" />
    <p>
        <asp:Label runat="server" ID="lblSourceControlInfo" EnableViewState="false" />
    </p>
    <div class="checkbox-list-vertical">
        <cms:CMSCheckBox ID="chkSaveAltFormLayouts" ResourceString="Deployment.SaveAltFormLayouts"
            runat="server" />
        <cms:CMSCheckBox ID="chkSaveFormLayouts" ResourceString="Deployment.SaveFormLayouts"
            runat="server" />
        <cms:CMSCheckBox ID="chkSaveLayouts" ResourceString="Deployment.SaveLayouts"
            runat="server" />
        <cms:CMSCheckBox ID="chkSavePageTemplate" ResourceString="Deployment.SavePageTemplates"
            runat="server" />
        <cms:CMSCheckBox ID="chkSaveTransformation" ResourceString="Deployment.SaveTansformations"
            runat="server" />
        <cms:CMSCheckBox ID="chkSaveWebpartLayout" ResourceString="Deployment.SaveWebpartLayouts"
            runat="server" />
    </div>
    <br />
    <div class="checkbox-list-vertical">
        <cms:CMSCheckBox ID="chkSaveCSS" ResourceString="Deployment.SaveCSS" runat="server"
            EnableViewState="false" />
        <cms:CMSCheckBox ID="chkSaveWebpartContainer" ResourceString="Deployment.SaveWebpartContainers"
            runat="server" />
    </div>
    <div style="padding-top: 15px">
        <p><cms:LocalizedLabel runat="server" ID="lblApplyChanges" EnableViewState="false" ResourceString="Deployment.ApplyChangesInfo" />
        &nbsp;<cms:LocalizedLabel
            runat="server" ID="lblSynchronization" EnableViewState="false" ResourceString="Deployment.SynchronizeChangesInfo"
            Visible="false" /></p>
    </div>
    <div style="padding: 10px 0px 10px 0px;">
        <cms:LocalizedButton ID="btnSourceControl" ButtonStyle="Primary" runat="server"
            OnClick="btnSourceControl_Click" ResourceString="Deployment.SourceControlStore"
            EnableViewState="false" />
        &nbsp;<cms:LocalizedButton runat="server" ID="btnSynchronize" OnClick="btnSynchronize_Click"
            Visible="false" EnableViewState="false" ButtonStyle="Primary" ResourceString="Deployment.SynchronizeChanges" />
    </div>
    <cms:LocalizedHeading runat="server" ID="headTest" Level="4" Text="Test" EnableViewState="false" />
    <p>
        <cms:LocalizedLabel runat="server" ID="lblTest" EnableViewState="false" ResourceString="Deployment.TestLabel" />
    </p>
    <cms:LocalizedButton ID="btnTest" ButtonStyle="Primary" runat="server"
        OnClick="btnTest_Click" ResourceString="Deployment.Test" EnableViewState="false" />
</asp:Content>

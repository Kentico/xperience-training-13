<%@ Page Language="C#" Inherits="CMSInstall_install" Theme="Default" EnableEventValidation="false"
    ValidateRequest="false"  Codebehind="install.aspx.cs" %>

<%@ Register Src="~/CMSInstall/Controls/WizardSteps/LicenseDialog.ascx" TagName="LicenseDialog"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSInstall/Controls/WizardSteps/CollationDialog.ascx" TagName="CollationDialog"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSInstall/Controls/WizardSteps/SiteCreationDialog.ascx" TagName="SiteCreationDialog"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/RequireScript.ascx" TagName="RequireScript"
    TagPrefix="cms" %>

<%@ Register Src="~/CMSInstall/Controls/WizardSteps/WagDialog.ascx" TagName="WagDialog"
    TagPrefix="cms" %>
<%@ Register Src="Controls/LayoutPanels/Error.ascx" TagName="ErrorPanel" TagPrefix="cms" %>
<%@ Register Src="Controls/LayoutPanels/Log.ascx" TagName="LogPanel" TagPrefix="cms" %>
<%@ Register Src="Controls/LayoutPanels/Version.ascx" TagName="VersionPanel" TagPrefix="cms" %>
<%@ Register Src="Controls/LayoutPanels/Warning.ascx" TagName="WarningPanel" TagPrefix="cms" %>
<%@ Register Src="Controls/StepNavigation.ascx" TagName="StepNavigation" TagPrefix="cms" %>
<%@ Register Src="Controls/WizardSteps/UserServer.ascx" TagName="UserServer" TagPrefix="cms" %>
<%@ Register Src="Controls/WizardSteps/DatabaseDialog.ascx" TagName="DatabaseDialog"
    TagPrefix="cms" %>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>
        <%=ResHelper.GetFileString("General.ProductName")%>
        Database Setup </title>
</head>
<body class="install-body-database <%=BodyClass%>">
    <form id="Form1" method="post" runat="server">
        <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
        <cms:RequireScript ID="rqScript" runat="server" UseFileStrings="true" />
        <asp:Panel runat="server" ID="pnlBody" CssClass="install-block">
            <asp:Panel ID="layPanel" runat="server" CssClass="install-panel">
                <cms:LocalizedHeading Level="3" ID="lblHeader" CssClass="install-header" runat="server" EnableViewState="false" />
                <asp:Panel runat="server" ID="pnlHeaderImages" CssClass="header-steps" EnableViewState="false" />
                <asp:Panel runat="server" ID="pnlWizard">
                    <asp:Button ID="btnHiddenNext" runat="server" CssClass="HiddenButton" OnClick="btnHiddenNext_onClick" />
                    <asp:Button ID="btnHiddenBack" runat="server" CssClass="HiddenButton" OnClick="btnHiddenBack_onClick" />
                    <asp:Wizard ID="wzdInstaller" runat="server" DisplaySideBar="False" OnPreviousButtonClick="wzdInstaller_PreviousButtonClick"
                        ActiveStepIndex="1" Width="100%">
                        <StepNavigationTemplate>
                            <cms:StepNavigation ID="stepNavigation" runat="server" NextPreviousVisible="True"
                                NextPreviousJS="True" />
                        </StepNavigationTemplate>
                        <StartNavigationTemplate>
                            <cms:StepNavigation ID="startStepNavigation" runat="server" NextPreviousVisible="false"
                                NextPreviousJS="True" />
                        </StartNavigationTemplate>
                        <WizardSteps>
                            <asp:WizardStep ID="stpUserServer" runat="server" StepType="Start">
                                <cms:UserServer ID="userServer" runat="server" />
                            </asp:WizardStep>
                            <asp:WizardStep ID="stpDatabase" runat="server" StepType="Step">
                                <cms:DatabaseDialog ID="databaseDialog" runat="server" />
                            </asp:WizardStep>
                            <asp:WizardStep ID="stpConnectionString" runat="server" AllowReturn="false" StepType="Start">
                                <asp:Panel ID="pnlConnectionString" runat="server">
                                    <div class="install-content">
                                        <asp:Label ID="lblConnectionString" runat="server" CssClass="install-info-title" Visible="False" />
                                        <table class="install-wizard" border="0" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td align="left">
                                                    <asp:Label ID="lblErrorConnMessage" runat="server" CssClass="connMessageError" />
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </asp:Panel>
                            </asp:WizardStep>
                            <asp:WizardStep ID="stpDBProgress" runat="server" AllowReturn="false" StepType="Step">
                                <div class="install-progress-label">
                                    <asp:Label ID="lblDBProgress" runat="server" EnableViewState="false" />
                                </div>
                                <asp:Panel ID="pnlDBProgress" runat="server">
                                    <div class="install-progress">
                                        <table class="install-wizard" border="0" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td align="left" style="vertical-align: top">
                                                    <div class="install-progress-database">
                                                        <div style="margin: 5px 0px 5px 5px;">
                                                            <cms:AsyncControl ID="ctlAsyncDB" runat="server" PostbackOnError="false" UseFileStrings="True" ProvideLogContext="True" LogContextNames="Install" ContinueOnAnyServer="false" />
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </asp:Panel>
                            </asp:WizardStep>
                            <asp:WizardStep ID="stpLicenseSetting" runat="server" AllowReturn="false" StepType="Start">
                                <div class="install-content">
                                    <table class="install-wizard" border="0" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="left">
                                                <cms:LicenseDialog ID="ucLicenseDialog" runat="server" />
                                                <cms:WagDialog ID="ucWagDialog" runat="server" Visible="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </asp:WizardStep>
                            <asp:WizardStep ID="stpSiteCreation" runat="server" AllowReturn="false" StepType="Start">
                                <asp:Panel ID="pnlSiteCreation" runat="server">
                                    <asp:Label ID="lblSiteCreation" runat="server" CssClass="install-info-title" Visible="False" />
                                    <div class="install-contentNewSite install-content">
                                        <table class="install-wizard" border="0" cellpadding="0" cellspacing="0">
                                            <tr style="vertical-align: top;">
                                                <td align="left">
                                                    <cms:SiteCreationDialog ID="ucSiteCreationDialog" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </asp:Panel>
                            </asp:WizardStep>
                            <asp:WizardStep ID="stpProgress" runat="server" AllowReturn="false" StepType="Step">
                                <asp:Panel ID="pnlProgress" runat="server">
                                    <div class="install-progress">
                                        <table class="install-wizard" border="0" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td align="left" style="vertical-align: top">
                                                    <div class="install-progress-site">
                                                        <div style="margin: 5px 0px 0px 5px;">
                                                            <cms:AsyncControl ID="ctlAsyncImport" runat="server" PostbackOnError="false" LogContextNames="Import" ProvideLogContext="True" ContinueOnAnyServer="false" />
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </asp:Panel>
                            </asp:WizardStep>
                            <asp:WizardStep ID="stpFinish" runat="server" StepType="Complete">
                                <div class="install-content">
                                    <asp:Panel ID="pnlFinished" runat="server">
                                        <table class="install-wizard" border="0" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td>&nbsp;
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="install-completed" style="text-align: center">
                                                    <asp:Label ID="lblCompleted" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="text-align: center">
                                                    <asp:Label ID="lblMediumTrustInfo" runat="server" Visible="false" /><br />
                                                    <asp:LinkButton ID="btnWebSite" runat="server" OnClick="btnWebSite_onClick" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>&nbsp;
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </div>
                                <div class="install-footer">
                                    &nbsp;
                                </div>
                            </asp:WizardStep>
                            <asp:WizardStep ID="stpCollation" runat="server" StepType="Step">
                                <asp:Panel ID="pnlCollation" runat="server">
                                    <cms:CollationDialog ID="ucCollationDialog" runat="server" />
                                </asp:Panel>
                            </asp:WizardStep>
                        </WizardSteps>
                    </asp:Wizard>
                </asp:Panel>
                <asp:Panel ID="pnlPermission" runat="server" CssClass="install-panel-info" Visible="false">
                    <div class="install-contentPermission">
                        <div style="text-align: left; padding: 0px 20px 10px 20px;">
                            <asp:Label ID="lblPermission" runat="server" />
                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel CssClass="ButtonsPanel" ID="pnlButtons" runat="server" Visible="false">
                    <div class="install-content">
                        <div style="padding-top: 10px;">
                            <cms:CMSButton ID="btnPermissionTest" runat="server" ButtonStyle="Default" />&nbsp;<cms:CMSButton
                                ID="btnPermissionSkip" runat="server" ButtonStyle="Default" />
                        </div>
                    </div>
                    <div class="install-footer">
                        &nbsp;
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlPermissionSuccess" runat="server" Visible="false">
                    <div class="install-content" style="padding: 20px 20px 10px;">
                        <asp:Label ID="lblPermissionSuccess" runat="server" /><br />
                        <br />
                        <cms:CMSButton ID="btnPermissionContinue" runat="server" ButtonStyle="Default" />
                    </div>
                    <div class="install-footer">
                        &nbsp;
                    </div>
                </asp:Panel>
            </asp:Panel>
            <cms:VersionPanel ID="versionPanel" runat="server" DisplaySupportLabel="True" />
            <cms:ErrorPanel ID="pnlError" runat="server" />
            <cms:LogPanel ID="logPanel" runat="server" />
            <cms:WarningPanel ID="pnlWarning" runat="server" />
        </asp:Panel>
        <asp:HiddenField ID="hdnState" runat="server" />
        <asp:Literal ID="ltlInstallScript" runat="server" EnableViewState="false" />
    </form>
</body>
</html>

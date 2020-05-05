<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="SeparateDB.aspx.cs" Inherits="CMSInstall_SeparateDB"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalSimplePage.master"
    Title="Contact management database separation" %>

<%@ Register Src="Controls/StepNavigation.ascx" TagName="StepNavigation" TagPrefix="cms" %>
<%@ Register Src="Controls/WizardSteps/UserServer.ascx" TagName="UserServer" TagPrefix="cms" %>
<%@ Register Src="Controls/WizardSteps/DatabaseDialog.ascx" TagName="DatabaseDialog"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSInstall/Controls/WizardSteps/AsyncProgress.ascx" TagName="AsyncProgress"
    TagPrefix="cms" %>
<%@ Register Src="Controls/WizardSteps/SeparationFinished.ascx" TagName="SeparationFinished"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSInstall/Controls/WizardSteps/CollationDialog.ascx" TagName="CollationDialog"
    TagPrefix="cms" %>

<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel runat="server" ID="pnlBody">
        <asp:Panel ID="layPanel" runat="server" CssClass="install-panel database-separation">
            <cms:LocalizedHeading Level="3" ID="lblHeader" CssClass="install-header" runat="server" />
            <asp:Panel runat="server" ID="pnlHeaderImages" CssClass="header-steps" EnableViewState="false"/>
            <asp:Panel runat="server" ID="pnlWizard" CssClass="install-content-block">
                <asp:Wizard ID="wzdInstaller" runat="server" DisplaySideBar="False" OnNextButtonClick="wzdInstaller_NextButtonClick">
                    <StepNavigationTemplate>
                        <cms:StepNavigation ID="stepNavigation" runat="server" NextPreviousVisible="True" />
                    </StepNavigationTemplate>
                    <StartNavigationTemplate>
                        <cms:StepNavigation ID="startStepNavigation" runat="server" />
                    </StartNavigationTemplate>
                    <FinishNavigationTemplate>
                        <cms:StepNavigation ID="finishNavigation" runat="server" NextButton-ResourceString="general.finish"
                            FinishStep="True" />
                    </FinishNavigationTemplate>
                    <WizardSteps>
                        <asp:WizardStep ID="stpUserServer" runat="server" StepType="Start">
                            <cms:UserServer ID="userServer" runat="server" SameServerEnabled="True" />
                        </asp:WizardStep>
                        <asp:WizardStep ID="stpDatabase" runat="server" StepType="Step">
                            <cms:DatabaseDialog ID="databaseDialog" runat="server" IsDBSeparation="True" />
                        </asp:WizardStep>
                        <asp:WizardStep ID="stpProgress" runat="server" AllowReturn="false" StepType="Step">
                            <cms:AsyncProgress ID="progress" runat="server" IsSeparation="True" />
                        </asp:WizardStep>
                        <asp:WizardStep ID="stpFinish" runat="server" StepType="Finish">
                            <cms:SeparationFinished ID="separationFinished" runat="server" IsSeparation="true" />
                        </asp:WizardStep>
                        <asp:WizardStep ID="stpCollation" runat="server" StepType="Step">
                            <cms:CollationDialog ID="ucCollationDialog" runat="server" />
                        </asp:WizardStep>
                    </WizardSteps>
                </asp:Wizard>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
    <cms:AsyncControl ID="asyncControl" runat="server" PostbackOnError="False" />
</asp:Content>

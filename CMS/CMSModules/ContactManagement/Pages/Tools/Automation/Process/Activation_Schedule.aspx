<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" AutoEventWireup="false" CodeBehind="Activation_Schedule.aspx.cs"
    Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Process_Activation_Schedule" Theme="Default" Title="Activation scheduler" %>

<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <script type="text/javascript">
        function CloseAndRefresh() {
            if (wopener) {
                wopener.location.replace(wopener.location);
            }
            CloseDialog();
        }
    </script>
    <cms:UIForm runat="server" ID="editForm" ObjectType="cms.workflow" AlternativeFormName="MarketingAutomationScheduler" IsLiveSite="false" RedirectUrlAfterCreate=""/>
    <cms:MessagesPlaceHolder ID="plcInfoMessage" runat="server" />
</asp:Content>

<asp:Content ContentPlaceHolderID="plcFooter" runat="server">
    <cms:LocalizedButton ID="btnCancel" runat="server" ButtonStyle="Default" ResourceString="general.cancel" OnClientClick="return CloseDialog();" EnableViewState="False" />
</asp:Content>
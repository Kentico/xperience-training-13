<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ProcessProperties.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Theme="Default"
    Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Process_Properties_ProcessProperties" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <div class="automation-step-properties-sidebar">
        <div class="automation-step-properties-header">
            <div class="automation-step-properties-close">
                <button class="automation-step-properties-close-btn" type="button">
                    <i class="icon-modal-close cms-icon-100"></i>
                </button>
            </div>
            <h1>
                <cms:LocalizedLabel ID="lblStepName" runat="server" />
            </h1>
        </div>
        <cms:MessagesPlaceHolder ID="plcMessages" runat="server" IsLiveSite="false"></cms:MessagesPlaceHolder>
        <iframe ID="frmStepEdit" ClientIdMode="Static" runat="server"></iframe>
    </div>
</asp:Content>
<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" AutoEventWireup="false" CodeBehind="Template_Edit.aspx.cs"
    Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Process_Template_Edit" Theme="Default" Title="Save as a template" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>


<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:MessagesPlaceHolder runat="server" ID="pnlMessagePlaceholder" />
            <asp:Panel runat="server" ID="pnlSaveAs" CssClass="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblSaveAs" runat="server" DisplayColon="true" ResourceString="ma.template.saveas"
                            EnableViewState="false" AssociatedControlID="drpSaveAs:drpSingleSelect" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:UniSelector ID="drpSaveAs" ObjectType="ma.automationtemplate" runat="server"
                            SelectionMode="SingleDropDownList" IsLiveSite="false" AllowEmpty="false" OnOnSelectionChanged="drpSaveAs_OnSelectionChanged" />
                    </div>
                </div>
            </asp:Panel>
            <cms:UIForm runat="server" ID="editForm" ObjectType="ma.automationtemplate" IsLiveSite="false" RedirectUrlAfterCreate="" OnOnAfterSave="editForm_OnAfterSave" />
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="drpSaveAs:drpSingleSelect" />
        </Triggers>
    </cms:CMSUpdatePanel>
</asp:Content>
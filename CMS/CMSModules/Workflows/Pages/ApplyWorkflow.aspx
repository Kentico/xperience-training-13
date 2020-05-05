<%@ Page Title="Comment" Language="C#" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true" EnableEventValidation="false" Inherits="CMSModules_Workflows_Pages_ApplyWorkflow"
     Codebehind="ApplyWorkflow.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>

<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel ID="pnlContent" runat="server">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblWorkflowType" ResourceString="WorfklowProperties.WorkflowType" DisplayColon="True" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:UniSelector ID="ucWorkflow" ObjectType="cms.workflow" runat="server" AllowEmpty="False" UseUniSelectorAutocomplete="false" WhereCondition="WorkflowEnabled <> 0" />
                    <div class="radio-list-vertical">
                        <cms:CMSRadioButton runat="server" ID="radDocument" Checked="true" GroupName="ScopeGroup"
                            ResourceString="WorfklowProperties.DocumentScope" />
                        <cms:CMSRadioButton runat="server" ID="radDocumentChildren" GroupName="ScopeGroup"
                            ResourceString="WorfklowProperties.DocumentChildrenScope" />
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
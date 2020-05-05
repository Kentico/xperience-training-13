<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Workflows_Controls_UI_Workflow_Edit"
     Codebehind="Edit.ascx.cs" %>

<cms:UIForm runat="server" ID="editForm" ObjectType="cms.workflow" DefaultFieldLayout="TwoColumns" RefreshHeader="True">
    <SecurityCheck Resource="CMS.Workflow" Permission="modify" />
    <LayoutTemplate>
        <cms:FormCategory runat="server" ID="pnlGeneral" CategoryTitleResourceString="general.general">
            <asp:PlaceHolder ID="plcType" runat="server">
                <cms:FormField runat="server" ID="wTypeLabel" Field="WorkflowType" Layout="Inline">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:FormLabel CssClass="control-label" ID="lblType" runat="server" EnableViewState="true" ResourceString="workflow.workflowtype"
                                DisplayColon="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:LocalizedLabel ID="lblTypeValue" runat="server" EnableViewState="true" CssClass="form-control-text" />
                        </div>
                    </div>
                </cms:FormField>
            </asp:PlaceHolder>
            <cms:FormField runat="server" ID="wDisplayName" Field="WorkflowDisplayName" FormControl="LocalizableTextBox"
                ResourceString="general.displayname" DisplayColon="true" UseFFI="True" />
            <cms:FormField runat="server" ID="wName" Field="WorkflowName" FormControl="CodeName" ResourceString="general.codename" DisplayColon="true" />
            <cms:FormField runat="server" ID="wEnabled" ResourceString="general.enabled" DisplayColon="true" Field="WorkflowEnabled" UseFFI="true" />
        </cms:FormCategory>
        <asp:PlaceHolder runat="server" ID="plcAdvanced">
            <cms:FormCategory runat="server" ID="pnlAdvanced" CategoryTitleResourceString="general.advanced">
                <asp:PlaceHolder ID="plcDocSettings" runat="server">
                    <asp:PlaceHolder ID="plcVersioning" runat="server">
                        <cms:FormField runat="server" ID="wAutoPublish" Field="WorkflowAutoPublishChanges" DisplayColon="true" Layout="Inline">
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:FormLabel CssClass="control-label" ID="lblAutoPublish" runat="server" ResourceString="development-workflow_general.autopublish" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:FormControl ID="ctrlAutoPublish" FormControlName="CheckBoxControl" runat="server" />
                                </div>
                            </div>
                        </cms:FormField>
                    </asp:PlaceHolder>
                    <cms:FormField runat="server" ID="wUseCheckInCheckOut" Field="WorkflowUseCheckinCheckout" FormControl="RadioButtonsControl" Layout="Inline">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:FormLabel CssClass="control-label" ID="lblUseCheckInCheckOut" runat="server" EnableViewState="false"
                                    ResourceString="development-workflow_general.usecheckincheckout" DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSRadioButtonList ID="rblUseCheckInCheckOut" UseResourceStrings="true" RepeatDirection="Vertical" runat="server">
                                    <asp:ListItem Text="development-workflow_general.sitesettings" Value="" />
                                    <asp:ListItem Text="general.yes" Value="True" />
                                    <asp:ListItem Text="general.no" Value="False" />
                                </cms:CMSRadioButtonList>
                            </div>
                        </div>
                    </cms:FormField>
                </asp:PlaceHolder>
            </cms:FormCategory>
        </asp:PlaceHolder>
        <cms:FormSubmit runat="server" ID="wSubmit" />
    </LayoutTemplate>
</cms:UIForm>

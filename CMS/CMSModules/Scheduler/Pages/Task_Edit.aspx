<%@ Page Language="C#" AutoEventWireup="false" Inherits="CMSModules_Scheduler_Pages_Task_Edit"
    Theme="Default" ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Scheduled tasks - Task Edit"  Codebehind="Task_Edit.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Selectors/ScheduleInterval.ascx" TagName="ScheduleInterval"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Inputs/LargeTextArea.ascx" TagName="LargeTextArea"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/SelectModule.ascx" TagPrefix="cms" TagName="SelectModule" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/SelectUser.ascx" TagPrefix="cms"
    TagName="SelectUser" %>
<%@ Register Src="~/CMSFormControls/Classes/AssemblyClassSelector.ascx" TagName="AssemblyClassSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/EnumSelector.ascx" TagName="EnumSelector"
    TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <div class="form-horizontal">
        <asp:PlaceHolder ID="plcTaskAvailability" runat="server">
            <cms:CMSUpdatePanel ID="pnlUpdateTaskAvailability" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTaskAvailability" EnableViewState="false" ResourceString="Task_Edit.TaskAvailability"
                                AssociatedControlID="rbTaskAvailability:radEnum" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:EnumSelector runat="server" ID="rbTaskAvailability" AssemblyName="CMS.Scheduler" TypeName="CMS.Scheduler.TaskAvailabilityEnum" DisplayType="RadioButtonsVertical" />
                        </div>
                    </div>
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTaskDisplayName" EnableViewState="false" ResourceString="Task_Edit.TaskDisplayNameLabel" AssociatedControlID="txtTaskDisplayName" ShowRequiredMark="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizableTextBox ID="txtTaskDisplayName" runat="server"
                    MaxLength="200" />
                <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="txtTaskDisplayName:cntrlContainer:textbox"
                    Display="Dynamic"></cms:CMSRequiredFieldValidator>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTaskName" EnableViewState="false" ResourceString="Task_Edit.TaskNameLabel" AssociatedControlID="txtTaskName" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CodeName ID="txtTaskName" runat="server" MaxLength="200" />
                <cms:CMSRequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtTaskName"
                    Display="Dynamic"></cms:CMSRequiredFieldValidator>
            </div>
        </div>
        <cms:CMSUpdatePanel ID="pnlUpdateTaskProvider" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTaskAssemblyName" EnableViewState="false" ResourceString="Task_Edit.TaskAssemblyNameLabel" AssociatedControlID="assemblyElem" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:AssemblyClassSelector ID="assemblyElem" runat="server" BaseClassNames="ITask;IWorkerTask" />
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="rbTaskAvailability:radEnum" EventName="SelectedIndexChanged" />
            </Triggers>
        </cms:CMSUpdatePanel>
        <cms:ScheduleInterval ID="schedInterval" runat="server" />
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTaskData" EnableViewState="false" ResourceString="Task_Edit.TaskDataLabel" AssociatedControlID="txtTaskData" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextArea ID="txtTaskData" runat="server" Rows="4" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblTaskCondition" runat="server" EnableViewState="false"
                    ResourceString="task.condition" DisplayColon="true" AssociatedControlID="ucMacroEditor" />
            </div>
            <div class="editing-form-value-cell">
                <cms:LargeTextArea ID="ucMacroEditor" runat="server" AllowMacros="true" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTaskEnabled" EnableViewState="false" ResourceString="Task_Edit.TaskEnabledLabel" AssociatedControlID="chkTaskEnabled" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkTaskEnabled" runat="server" Checked="true" CssClass="CheckBoxMovedLeft" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTaskDeleteAfterLastRun" EnableViewState="false" ResourceString="Task_Edit.TaskDeleteAfterLastRunLabel" AssociatedControlID="chkTaskDeleteAfterLastRun" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkTaskDeleteAfterLastRun" runat="server" Checked="false" CssClass="CheckBoxMovedLeft" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblRunInSeparateThread" ResourceString="scheduledtask.runinseparatethread"
                    DisplayColon="true" EnableViewState="false" AssociatedControlID="chkRunTaskInSeparateThread" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkRunTaskInSeparateThread" runat="server" Checked="false" CssClass="CheckBoxMovedLeft" />
            </div>
        </div>
        <cms:CMSUpdatePanel ID="pnlUpdateExternalServices" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>
                <asp:PlaceHolder ID="plcAllowExternalService" runat="server">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTaskAllowExternalService" ResourceString="ScheduledTask.TaskAllowExternalService"
                                DisplayColon="true" EnableViewState="false" AssociatedControlID="chkTaskAllowExternalService" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkTaskAllowExternalService" runat="server" Checked="true" CssClass="CheckBoxMovedLeft" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plcUseExternalService" runat="server">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTaskUseExternalService" ResourceString="ScheduledTask.TaskUseExternalService"
                                DisplayColon="true" EnableViewState="false" AssociatedControlID="chkTaskUseExternalService" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkTaskUseExternalService" runat="server" Checked="false" CssClass="CheckBoxMovedLeft" />
                        </div>
                    </div>
                </asp:PlaceHolder>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="rbTaskAvailability:radEnum" EventName="SelectedIndexChanged" />
            </Triggers>
        </cms:CMSUpdatePanel>
        <asp:PlaceHolder ID="plcRunIndividually" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblRunIndividually" ResourceString="ScheduledTask.runindividually"
                        DisplayColon="true" EnableViewState="false" AssociatedControlID="chkRunIndividually" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkRunIndividually" runat="server" Checked="false" CssClass="CheckBoxMovedLeft" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblServerName" EnableViewState="false" ResourceString="Task_Edit.TaskServerNameLabel" AssociatedControlID="txtServerName" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtServerName" runat="server" MaxLength="100" />
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="plcDevelopment">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblModule" EnableViewState="false" ResourceString="General.Module" DisplayColon="true" AssociatedControlID="drpModule" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:SelectModule ID="drpModule" runat="server" DisplayNone="true" DisplayAllModules="true"
                        IsLiveSite="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblUser" EnableViewState="false" ResourceString="ScheduledTask.UserContext" AssociatedControlID="ucUser" />
            </div>
            <div class="editing-form-value-cell">
                <cms:SelectUser ID="ucUser" runat="server" IsLiveSite="false" AllowDefault="true"
                    AllowEmpty="false" SelectionMode="SingleDropDownList" HideDisabledUsers="true"
                    HideNonApprovedUsers="true" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblExecutionsInfo" EnableViewState="false"
                    ResourceString="unigrid.task.columns.taskexecutions" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <strong>
                    <asp:Label runat="server" CssClass="form-control-text" ID="lblExecutions" EnableViewState="false" Text="0" />
                </strong>
                <asp:PlaceHolder runat="server" ID="plcResetFrom"><span class="form-control-text">(<asp:Literal runat="server"
                    ID="lblFrom" EnableViewState="false" />)</span></asp:PlaceHolder>
                <cms:LocalizedLinkButton ID="btnReset" CssClass="form-control-text" runat="server" ResourceString="general.reset" OnClick="btnReset_Click" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false"
                    ResourceString="general.ok" />
            </div>
        </div>
    </div>
</asp:Content>
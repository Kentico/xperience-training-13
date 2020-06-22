<%@ Control Language="C#" AutoEventWireup="false" Codebehind="Edit.ascx.cs" Inherits="CMSModules_Workflows_Controls_UI_WorkflowStep_Edit" %>

<%@ Register Src="~/CMSAdminControls/UI/Selectors/TimeoutSelector.ascx" TagName="TimeoutSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Workflows/FormControls/SourcePointSelector.ascx" TagName="SourcePointSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/ObjectParameters.ascx" TagName="ObjectParameters"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Workflows/Controls/UI/WorkflowStep/SourcePoint/Edit.ascx"
    TagName="SourcePointEdit" TagPrefix="cms" %>

<asp:Panel ID="pnlContainer" runat="server">
    <cms:FormSubmitButton ID="btnSubmit" OnClick="btnSubmit_Click" Visible="false" runat="server" />
    <cms:UIForm runat="server" ID="editForm" ObjectType="cms.workflowstep" RefreshHeader="True" FieldGroupHeadingIsAnchor="false"
        OnOnAfterValidate="editForm_OnAfterValidate" OnOnBeforeSave="editForm_OnBeforeSave" OnOnAfterSave="editForm_OnAfterSave" />
    <asp:PlaceHolder runat="server" ID="plcCondition" Visible="false">
        <cms:LocalizedHeading runat="server" ID="lblCondition" Level="4" />
        <cms:SourcePointEdit runat="server" ID="ucSourcePointEdit" allowconfirmation="false" />
    </asp:PlaceHolder>
    <cms:CMSUpdatePanel ID="pnlTimeout" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlTimeoutForm" runat="server">
                <cms:LocalizedHeading runat="server" ID="lblTimeout" Level="4" />
                <cms:TimeoutSelector ID="ucTimeout" runat="server" AutomaticallyDisableInactiveControl="true" IsLiveSite="false" />
                <asp:PlaceHolder ID="plcTimeoutTarget" runat="server">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel ID="lblTimeoutTarget" runat="server" EnableViewState="false" AssociatedControlID="ucTimeoutTarget"
                                ResourceString="workflowstep.timeouttarget" CssClass="control-label" DisplayColon="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:SourcePointSelector ID="ucTimeoutTarget" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <asp:PlaceHolder ID="plcParameters" runat="server">
        <cms:LocalizedHeading runat="server" ID="lblParameters" Level="4" />
        <cms:ObjectParameters runat="server" ID="ucActionParameters" Visible="false" />
    </asp:PlaceHolder>
</asp:Panel>
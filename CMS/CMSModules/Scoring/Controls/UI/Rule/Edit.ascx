<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Edit.ascx.cs" Inherits="CMSModules_Scoring_Controls_UI_Rule_Edit" %>

<%@ Register TagPrefix="cms" TagName="SelectValidity" Src="~/CMSAdminControls/UI/Selectors/SelectValidity.ascx" %>

<%-- NOTE: Following two controls *has to* be registered in order to be able to cast FormField to their type in code behind. --%>
<%@ Register Src="~/CMSFormControls/Basic/TextBoxControl.ascx" TagName="TextBoxControl" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/RadioButtonsControl.ascx" TagName="RadioButtonsControl" TagPrefix="cms" %>

<%@ Register Src="~/CMSModules/Activities/FormControls/ActivityTypeSelector.ascx" TagName="ActivityTypeSel" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Macros/ConditionBuilder.ascx" TagName="ConditionBuilder" TagPrefix="cms" %>

<cms:UIForm runat="server" ID="editForm" ObjectType="om.rule" IsLiveSite="false" FieldGroupHeadingIsAnchor="True"
    DefaultFieldLayout="TwoColumns" FieldLabelCellCssClass="RuleSettingsLabel">
    <LayoutTemplate>

        <%-- UpdatePanel for sending asynchronous requests when changing the rule type --%>
        <cms:CMSUpdatePanel ID="upnlGeneral" runat="server" Triggers="fRuleType" UpdateMode="Conditional">
            <ContentTemplate>
                <cms:FormCategory runat="server" ID="pnlGeneral" DefaultFieldLayout="TwoColumns" CategoryTitleResourceString="general.general">
                    <cms:FormField runat="server" ID="fDisplayName" Field="RuleDisplayName" />
                    <cms:FormField runat="server" ID="fValue" Field="RuleValue" />
                    <cms:FormField runat="server" ID="fRuleType" Field="RuleType" />
                </cms:FormCategory>
            </ContentTemplate>
        </cms:CMSUpdatePanel>

        <%-- UpdatePanel for handling asynchronous loading specified settings based on selected rule type --%>
        <cms:CMSUpdatePanel ID="upnlSettings" runat="server" UpdateMode="Conditional">
            <ContentTemplate>

                <%-- Wraps all three settings into HTML field set --%>
                <cms:FormCategory runat="server" ID="pnlSettings" DefaultFieldLayout="TwoColumns" CategoryTitleResourceString="om.score.rulesettings">

                    <%-- Contains Rule settings with specific fields for Attribute type --%>
                    <%-- Currently not possible to use FormField, since it has problems with displaying fields not bounded to edited object (om.rule in this case) --%>
                    <asp:PlaceHolder ID="plcAttributeSettings" runat="server">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:FormLabel CssClass="control-label" ID="lblAttribute" runat="server" EnableViewState="false" ResourceString="om.score.attribute" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSDropDownList ID="drpAttribute" runat="server" AutoPostBack="true" CssClass="DropDownField" />
                            </div>
                        </div>
                        <cms:BasicForm ID="attributeFormCondition" runat="server" DefaultFieldLayout="TwoColumns" />
                    </asp:PlaceHolder>

                    <%-- Contains Rule settings with specific fields for Activity type --%>
                    <asp:PlaceHolder ID="plcActivitySettings" runat="server">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:FormLabel CssClass="control-label" ID="lblActivity" runat="server" EnableViewState="false" ResourceString="om.score.activity" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:ActivityTypeSel ID="ucActivityType" runat="server" ShowAll="false" AutoPostBack="true" />
                            </div>
                        </div>
                        <cms:BasicForm ID="activityFormCondition" runat="server" DefaultFieldLayout="TwoColumns" />
                    </asp:PlaceHolder>

                    <%-- Contains Rule settings with specific fields for Macro type --%>
                    <asp:Panel ID="pnlMacroSettings" runat="server" CssClass="form-group">
                        <div class="editing-form-label-cell">
                            <cms:FormLabel CssClass="control-label" ID="FormLabel1" runat="server" EnableViewState="false" ResourceString="om.score.macro" DisplayColon="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:ConditionBuilder ID="macroEditor" runat="server" RuleCategoryNames="cms.onlinemarketing" DisplayRuleType="1" ResolverName="ContactResolver" MaxWidth="550" />
                        </div>
                    </asp:Panel>

                </cms:FormCategory>

                <%-- Category displayed only when Activity type is selected. Contains information about validation of activities --%>
                <asp:PlaceHolder ID="pnlActivityPlaceHolder" runat="server">
                    <cms:FormCategory runat="server" ID="pnlActivity" DefaultFieldLayout="TwoColumns"  CategoryTitleResourceString="om.score.activityvalidity">
                        <cms:FormField runat="server" ID="fRecurring" Field="RuleIsRecurring" />
                        <cms:FormField runat="server" ID="fMaxPoints" Field="RuleMaxPoints" UseFFI="true" FormControl="TextBoxControl" />
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:FormLabel runat="server" CssClass="control-label" ResourceString="om.score.validity" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:SelectValidity ID="validity" runat="server" AutoPostBack="true" AutomaticallyDisableInactiveControl="true" />
                            </div>
                        </div>
                    </cms:FormCategory>
                </asp:PlaceHolder>

            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </LayoutTemplate>
</cms:UIForm>
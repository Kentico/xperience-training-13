<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SubscriptionEdit.ascx.cs"
    Inherits="CMSModules_Reporting_Controls_SubscriptionEdit" %>

<%@ Register Src="~/CMSAdminControls/UI/Selectors/ScheduleInterval.ascx" TagName="ScheduleInterval"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Macros/ConditionBuilder.ascx" TagName="ConditionBuilder"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput"
    TagPrefix="cms" %>

<cms:LocalizedHeading runat="server" ID="headSubscription" Level="4" ResourceString="reportsubscription.settings" EnableViewState="False" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblUser" ResourceString="reportsubscription.sendto"
                DisplayColon="true" AssociatedControlID="txtEmail" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:EmailInput runat="server" ID="txtEmail" AllowMultipleAddresses="True" />
            <cms:CMSRequiredFieldValidator runat="server" ID="rfvEmail"
                Display="Dynamic" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSubject" ResourceString="general.subject"
                DisplayColon="true" AssociatedControlID="txtSubject" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox runat="server" ID="txtSubject" MaxLength="200" />
            <cms:CMSRequiredFieldValidator runat="server" ControlToValidate="txtSubject" ID="rfvSubject"
                Display="Dynamic" />
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="pnlSubscription">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblItem" ResourceString="reportsubscription.item"
                    DisplayColon="true" AssociatedControlID="drpItems" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSDropDownList runat="server" ID="drpItems" />
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="pnlFromToParams">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="reportsubscription.timerange"
                    ID="lblLast" DisplayColon="true" AssociatedControlID="rbAll" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <div class="radio-list-vertical">
                    <cms:CMSRadioButton runat="server" ID="rbAll" GroupName="Time" OnClick="disableLast(true)" />
                    <cms:CMSRadioButton runat="server" ID="rbTime" GroupName="Time" OnClick="disableLast(false)" />
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblParametersLabel" ResourceString="reportsubscription.ParametersLabel"
                    DisplayColon="true" AssociatedControlID="txtLast" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtLast" CssClass="input-width-15" MaxLength="3"  />
                <cms:CMSDropDownList runat="server" CssClass="input-width-82" ID="drpLast" />
            </div>
        </div>
    </asp:PlaceHolder>
    <cms:ScheduleInterval ID="ucInterval" runat="server" />
    <asp:PlaceHolder runat="server" ID="pnlCondition">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCondition" ResourceString="macros.macrorule.condition"
                    DisplayColon="true" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:ConditionBuilder ID="ucMacroEditor" runat="server" CssClass="ReportSubscription"
                    RuleCategoryNames="cms.reporting" MaxWidth="300" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblOnlyNonEmpty" ResourceString="reportsubscription.notempty"
                DisplayColon="true" AssociatedControlID="chkNonEmpty" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkNonEmpty" />
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="pnlEnabled">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnabled" ResourceString="general.enabled"
                    DisplayColon="true" AssociatedControlID="chkEnabled" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chkEnabled" />
            </div>
        </div>
    </asp:PlaceHolder>
</div>
<asp:Panel ID="pnlParametersEnvelope" runat="server">
    <br />
    <cms:LocalizedHeading runat="server" ID="headParameters" Level="4" ResourceString="reportsubscription.parameters.title" EnableViewState="false" />
    <asp:Panel runat="server" ID="pnlBasicForm" CssClass="SubscriptionParameters">
        <cms:BasicForm runat="server" ID="formElem" />
    </asp:Panel>
</asp:Panel>

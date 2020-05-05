<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_TimeZones_Pages_TimeZone_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="TimeZone_Edit.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/TimeZones/Controls/TimeZoneRuleEditor.ascx"
    TagName="RuleEditor" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTimeZoneDisplayName" EnableViewState="false"
                    ResourceString="TimeZ.Edit.TimeZoneDisplayName" DisplayColon="true" ShowRequiredMark="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizableTextBox ID="txtTimeZoneDisplayName" runat="server" MaxLength="200" />
                <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="txtTimeZoneDisplayName:cntrlContainer:textbox"
                    Display="Dynamic" ValidationGroup="vgTimeZone"></cms:CMSRequiredFieldValidator>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTimeZoneName" EnableViewState="false" ResourceString="General.CodeName"
                    DisplayColon="true" ShowRequiredMark="True" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CodeName ID="txtTimeZoneName" runat="server" MaxLength="200" />
                <cms:CMSRequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtTimeZoneName"
                    Display="Dynamic" ValidationGroup="vgTimeZone"></cms:CMSRequiredFieldValidator>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTimeZoneGMT" EnableViewState="false" ResourceString="TimeZ.Edit.TimeZoneGMT" AssociatedControlID="txtTimeZoneGMT" ShowRequiredMark="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtTimeZoneGMT" runat="server" />
                <cms:CMSRequiredFieldValidator ID="rfvGMT" runat="server" ControlToValidate="txtTimeZoneGMT"
                    Display="Dynamic" ValidationGroup="vgTimeZone"></cms:CMSRequiredFieldValidator>
                <cms:CMSRangeValidator ID="rvGMTDouble" runat="server" ControlToValidate="txtTimeZoneGMT"
                    Display="Dynamic" Type="Double" ValidationGroup="vgTimeZone"></cms:CMSRangeValidator>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTimeZoneDaylight" EnableViewState="false"
                    ResourceString="TimeZ.Edit.TimeZoneDaylight" AssociatedControlID="chkTimeZoneDaylight" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkTimeZoneDaylight" AutoPostBack="true" runat="server" CssClass="CheckBoxMovedLeft" />
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="plcDSTInfo" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTimeZoneRuleStartIn" EnableViewState="false"
                        ResourceString="TimeZ.Edit.TimeZoneRuleStartIn" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label CssClass="form-control-text" ID="lblTimeZoneRuleStart" runat="server" EnableViewState="false" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTimeZoneRuleEndIn" EnableViewState="false"
                        ResourceString="TimeZ.Edit.TimeZoneRuleEndIn" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label CssClass="form-control-text" ID="lblTimeZoneRuleEnd" runat="server" EnableViewState="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        
        <cms:RuleEditor ID="startRuleEditor" runat="server" />
        
        <cms:RuleEditor ID="endRuleEditor" runat="server" />

        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false"
                    ValidationGroup="vgTimeZone" ResourceString="general.ok" />
            </div>
        </div>
    </div>
</asp:Content>

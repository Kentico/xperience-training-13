<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_TimeZones_Controls_TimeZoneRuleEditor"  Codebehind="TimeZoneRuleEditor.ascx.cs" %>
<cms:CMSUpdatePanel ID="updPanel" runat="server">
    <ContentTemplate>
        <cms:LocalizedHeading ID="headText" Level="4" runat="server"></cms:LocalizedHeading>

        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblMonth" runat="server" CssClass="control-label" DisplayColon="true" EnableViewState="false" AssociatedControlID="drpMonth" ResourceString="TimeZ.RuleEditor.Month" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSDropDownList ID="drpMonth" runat="server" CssClass="DropDownFieldSmall input-width-60" />
            </div>
        </div>

        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblDay" runat="server" CssClass="control-label" DisplayColon="true" EnableViewState="false" AssociatedControlID="drpCondition" ResourceString="TimeZ.RuleEditor.Day" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSDropDownList ID="drpCondition" runat="server" AutoPostBack="true" CssClass="DropDownFieldSmall input-width-40"
                    OnSelectedIndexChanged="drpCondition_SelectedIndexChanged">
                    <asp:ListItem Value="FIRST" Text="FIRST" />
                    <asp:ListItem Value="LAST" Text="LAST" />
                    <asp:ListItem Value=">=" Text=">=" />
                    <asp:ListItem Value="<=" Text="<=" />
                    <asp:ListItem Value="=" Text="=" />
                </cms:CMSDropDownList>
                <cms:CMSDropDownList ID="drpDay" runat="server" CssClass="DropDownFieldSmall input-width-60" />
                <cms:CMSDropDownList ID="drpDayValue" CssClass="DropDownFieldShort input-width-20" runat="server" />
            </div>
        </div>

        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblTime" runat="server" CssClass="control-label" DisplayColon="true" EnableViewState="false" AssociatedControlID="txtAtHour" ResourceString="TimeZ.RuleEditor.Time" />
            </div>
            <div class="editing-form-value-cell control-group-inline">
                <cms:CMSTextBox ID="txtAtHour" runat="server" CssClass="input-width-15" /><span class="form-control-text">:</span><cms:CMSTextBox ID="txtAtMinute" runat="server" CssClass="input-width-15" />
            </div>
        </div>

        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblValue" runat="server" CssClass="control-label" DisplayColon="true" EnableViewState="false" AssociatedControlID="txtValue" ResourceString="TimeZ.RuleEditor.Value" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtValue" runat="server" CssClass="input-width-15" />
            </div>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>

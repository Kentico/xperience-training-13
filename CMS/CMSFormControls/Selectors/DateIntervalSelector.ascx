<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="DateIntervalSelector.ascx.cs"
    Inherits="CMSFormControls_Selectors_DateIntervalSelector" %>
<div class="radio-list-vertical date-interval-selector">
    <cms:CMSRadioButton runat="server" ID="radToday" GroupName="rads" ResourceString="dateintervalsel.today" Checked="true" />
    <cms:CMSRadioButton runat="server" ID="radCurrentWeek" GroupName="rads" ResourceString="dateintervalsel.week" />
    <cms:CMSRadioButton runat="server" ID="radCurrentMonth" GroupName="rads" ResourceString="dateintervalsel.month" />
    <div class="control-group-inline last-days">
        <cms:CMSRadioButton runat="server" ID="radLast" GroupName="rads" ResourceString="dateintervalsel.last" />
        <cms:CMSTextBox runat="server" ID="txtLast" CssClass="input-width-15" />
        <cms:LocalizedLabel runat="server" ID="lblDays" ResourceString="dateintervalsel.days" EnableViewState="false" CssClass="form-control-text" />
    </div>
</div>

<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSFormControls/Inputs/AgeRangeSelector.ascx.cs"
    Inherits="CMSFormControls_Inputs_AgeRangeSelector" %>
<div class="control-group-inline">
    <cms:LocalizedLabel runat="server" ID="lblBetween" ResourceString="ageselector.between" CssClass="form-control-text" />
    <cms:CMSTextBox runat="server" ID="txtBetween"  CssClass="input-width-20" />
    <cms:LocalizedLabel runat="server" ID="lbland" ResourceString="ageselector.conjunction" CssClass="form-control-text" />
    <cms:CMSTextBox runat="server" ID="txtDays" CssClass="input-width-20" />
    <cms:LocalizedLabel runat="server" ID="lblDays" ResourceString="ageselector.days" CssClass="form-control-text" />
</div>
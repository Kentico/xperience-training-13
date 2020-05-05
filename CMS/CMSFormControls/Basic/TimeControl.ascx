<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="TimeControl.ascx.cs" Inherits="CMSFormControls_Basic_TimeControl" %>
<%@ Register Src="~/CMSFormControls/Basic/TextBoxControl.ascx" TagName="TextBoxControl"
    TagPrefix="cms" %>

<div class="control-group-inline">
    <cms:TextBoxControl ID="txtHour" runat="server" MaxLength="2" FilterType="Numbers" CssClass="input-width-15" />
    <span class="form-control-text">:</span>
    <cms:TextBoxControl ID="txtMinute" runat="server" MaxLength="2" FilterType="Numbers" CssClass="input-width-15" />
    <cms:CMSDropDownList ID="drpAmPm" runat="server" CssClass="input-width-20" />
</div>

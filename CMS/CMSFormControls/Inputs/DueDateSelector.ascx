<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSFormControls/Inputs/DueDateSelector.ascx.cs"
    Inherits="CMSFormControls_Inputs_DueDateSelector" %>
<div class="control-group-inline">
    <cms:CMSTextBox ID="txtQuantity" runat="server" MaxLength="4" CssClass="input-width-20" />
    <cms:CMSDropDownList ID="drpScale" runat="server" CssClass="input-width-40" AutoPostBack="true" />
</div>

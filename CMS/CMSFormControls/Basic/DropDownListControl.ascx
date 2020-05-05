<%@ Control Language="C#" AutoEventWireup="false"  Codebehind="DropDownListControl.ascx.cs"
    Inherits="CMSFormControls_Basic_DropDownListControl" %>
    <cms:CMSDropDownList ID="dropDownList" runat="server" CssClass="DropDownField" />
<div class="autocomplete" runat="server" id="autoComplete">
    <cms:CMSTextBox ID="txtCombo" runat="server" Visible="false" CssClass="autocomplete-textbox" />
    <i runat="server" id="btnAutocomplete" class="autocomplete-icon icon-ellipsis" Visible="false" />
</div>
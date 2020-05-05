<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="BasicAutocompleteDropdown.ascx.cs"
    Inherits="CMSFormControls_Selectors_BasicAutocompleteDropdown" %>
<cms:CMSTextBox runat="server" ID="txtAutocomplete" CssClass="autocomplete-textbox" />
<i runat="server" id="btnAutocomplete" class="autocomplete-icon icon-ellipsis" />
<cms:LocalizedLabel runat="server" ID="lblIconInfo" CssClass="sr-only" ResourceString="autocompleteicon.list" />
<asp:HiddenField ID="hdnValue" runat="server" />

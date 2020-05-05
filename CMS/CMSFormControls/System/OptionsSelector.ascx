<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="OptionsSelector.ascx.cs"
    Inherits="CMSFormControls_System_OptionsSelector" %>
<cms:CMSRadioButtonList ID="lstOptions" runat="server" RepeatDirection="Horizontal" EnableViewState="false" UseResourceStrings="true" CssClass="Layout" AutoPostBack="True">
    <asp:ListItem Selected="True" Text="general.none" Value="0" />
    <asp:ListItem Text="templatedesigner.dropdownlistoptions" Value="1" />
    <asp:ListItem Text="templatedesigner.dropdownlistmacro" Value="2" />
    <asp:ListItem Text="templatedesigner.dropdownlistsql" Value="3" />
</cms:CMSRadioButtonList>
<table cellpadding="0" cellspacing="0" class="options-selector">
    <tr>
        <td>
            <div class="editor form-field-full-column-width">
                <cms:MacroEditor runat="server" ID="txtValue" UseAutoComplete="true" MixedMode="false" Height="150px" />
            </div>
        </td>
        <td valign="top" class="NoWrap">
            <span class="info-icon">
                <cms:LocalizedLabel runat="server" ID="spanScreenReader" CssClass="sr-only" ResourceString="optionselector.help" EnableViewState="False" />
                <cms:CMSIcon runat="server" ID="imgHelp" EnableViewState="false" class="icon-question-circle" aria-hidden="true" />
            </span>
        </td>
    </tr>
</table>

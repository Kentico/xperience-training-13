<%@ Control Language="C#" AutoEventWireup="false"  Codebehind="AssemblyClassSelector.ascx.cs" Inherits="CMSFormControls_Classes_AssemblyClassSelector" %>

<%@ Register Src="~/CMSFormControls/Basic/DropDownListControl.ascx" TagName="DropDownListControl"
    TagPrefix="cms" %>

<div class="content-block-50">
    <cms:DropDownListControl ID="drpAssemblyName" runat="server" EditText="true" EnableViewState="false" />
    <cms:CMSTextBox ID="txtAssemblyName" runat="server" Visible="false" MaxLength="200" />
    <cms:LocalizedLabel ID="lblAssembly" runat="server" CssClass="explanation-text"
        ResourceString="assemblyselector.assembly" />
</div>
<div>
    <cms:DropDownListControl ID="drpClassName" runat="server" EditText="true" EnableViewState="false" />
    <cms:CMSTextBox ID="txtClassName" runat="server" Visible="false" MaxLength="200" />
    <cms:LocalizedLabel ID="lblClassName" runat="server" CssClass="explanation-text"
        ResourceString="assemblyselector.class" />
</div>
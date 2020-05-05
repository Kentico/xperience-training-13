<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SharePointListSelector.ascx.cs" Inherits="CMSModules_SharePoint_FormControls_SharePointListSelector" %>
<%@ Register Src="~/CMSFormControls/Basic/DropDownListControl.ascx" TagName="DropDownListControl"
    TagPrefix="cms" %>
<cms:DropDownListControl ID="drpSharePointLists" runat="server" EditText="true" EnableViewState="true" />
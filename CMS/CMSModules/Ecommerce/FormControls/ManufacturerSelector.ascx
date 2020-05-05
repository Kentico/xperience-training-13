<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_FormControls_ManufacturerSelector"
     Codebehind="ManufacturerSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:UniSelector ID="uniSelector" runat="server" ShortID="s" DisplayNameFormat="{%ManufacturerDisplayName%}"
    EditDialogWindowHeight="400" EditDialogWindowWidth="680"
    ReturnColumnName="ManufacturerID" ObjectType="ecommerce.manufacturer" ResourcePrefix="manufacturerselector"
    SelectionMode="SingleDropDownList" AllowEmpty="false" />

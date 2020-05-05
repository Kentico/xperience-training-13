<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Ecommerce_FormControls_SupplierSelector"  Codebehind="SupplierSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>

<cms:UniSelector ID="uniSelector" runat="server" ShortID="s" DisplayNameFormat="{%SupplierDisplayName%}"
    EditDialogWindowHeight="400" EditDialogWindowWidth="680"
    ObjectType="ecommerce.supplier" ResourcePrefix="supplierselector" ReturnColumnName="SupplierID"
    SelectionMode="SingleDropDownList" AllowEmpty="false" />

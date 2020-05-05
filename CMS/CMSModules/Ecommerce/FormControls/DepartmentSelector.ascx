<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_FormControls_DepartmentSelector"
     Codebehind="DepartmentSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:UniSelector ID="uniSelector" runat="server" ShortID="s" DisplayNameFormat="{%DepartmentDisplayName%}"
    EditDialogWindowHeight="250" EditDialogWindowWidth="680"
    ObjectType="ecommerce.department" ResourcePrefix="departmentselector" SelectionMode="SingleDropDownList" />


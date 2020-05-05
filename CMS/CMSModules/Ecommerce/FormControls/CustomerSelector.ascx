<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_FormControls_CustomerSelector"
     Codebehind="CustomerSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:UniSelector ID="uniSelector" runat="server" DisplayNameFormat="{%CustomerFirstName%}, {%CustomerLastName%}, {%CustomerCompany%}, {%CustomerEmail%}"
    RemoveMultipleCommas="true" ObjectType="ecommerce.customer" ResourcePrefix="customerselector"
    ReturnColumnName="CustomerID" SelectionMode="SingleTextBox" AllowEditTextBox="false"
    IsLiveSite="false" AllowEmpty="false" />

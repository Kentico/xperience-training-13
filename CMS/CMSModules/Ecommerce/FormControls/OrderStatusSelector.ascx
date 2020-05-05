<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_FormControls_OrderStatusSelector"
     Codebehind="OrderStatusSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:UniSelector ID="uniSelector" runat="server" ShortID="s" DisplayNameFormat="{%StatusDisplayName%}"
    EditDialogWindowHeight="340" EditDialogWindowWidth="680"
    ObjectType="ecommerce.orderstatus" ResourcePrefix="orderstatusselector" SelectionMode="SingleDropDownList"
    AllowEmpty="false" OrderBy="StatusOrder" />

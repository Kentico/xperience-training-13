<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_FormControls_InternalStatusSelector"
     Codebehind="InternalStatusSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:UniSelector ID="uniSelector" runat="server" ShortID="s" DisplayNameFormat="{%InternalStatusDisplayName%}"
    EditDialogWindowHeight="250" EditDialogWindowWidth="680"
    ObjectType="ecommerce.internalstatus" ResourcePrefix="internalstatusselector" SelectionMode="SingleDropDownList"
    AllowEmpty="false" />

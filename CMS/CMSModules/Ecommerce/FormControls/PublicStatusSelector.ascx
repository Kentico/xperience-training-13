<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_FormControls_PublicStatusSelector"
     Codebehind="PublicStatusSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:UniSelector ID="uniSelector" runat="server" ShortID="s" DisplayNameFormat="{%PublicStatusDisplayName%}"
    EditDialogWindowHeight="250" EditDialogWindowWidth="680"
    ObjectType="ecommerce.publicstatus" ResourcePrefix="publicstatusselector" SelectionMode="SingleDropDownList"
    AllowEmpty="false" />

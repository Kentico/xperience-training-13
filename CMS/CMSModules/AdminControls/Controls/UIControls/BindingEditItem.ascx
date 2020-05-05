<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/AdminControls/Controls/UIControls/BindingEditItem.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_UIControls_BindingEditItem" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:UniSelector runat="server" ID="editElem" IsLiveSite="false" SelectionMode="Multiple" />

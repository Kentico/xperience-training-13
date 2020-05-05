<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_AdminControls_Controls_Class_AlternativeFormList"  Codebehind="AlternativeFormList.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" tagname="UniGrid" tagprefix="cms" %>

<cms:UniGrid ID="gridElem" runat="server" OrderBy="FormDisplayName" />
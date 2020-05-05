<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_AdminControls_Controls_Class_ClassQueries"  Codebehind="ClassQueries.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" tagname="UniGrid" tagprefix="cms" %>
<cms:UniGrid runat="server" ID="uniGrid" GridName="ClassQueries.xml" OrderBy="QueryName"
    IsLiveSite="true" />
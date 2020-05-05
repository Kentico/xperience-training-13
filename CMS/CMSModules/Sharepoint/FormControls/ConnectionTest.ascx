<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ConnectionTest.ascx.cs" Inherits="CMSModules_SharePoint_FormControls_ConnectionTest" %>

<cms:LocalizedButton ID="btnTestConnection" ResourceString="sharepoint.testconnection.testconnection" ToolTipResourceString="sharepoint.testconnection.testconnection.description" ButtonStyle="Default" 
    OnClick="btnTestConnection_OnClick" EnableViewState="False" runat="server"
    /><asp:Label ID="lblConnectionStatus" CssClass="form-control-text" EnableViewState="false" runat="server" />
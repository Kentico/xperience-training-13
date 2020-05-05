<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_System_CacheDependencies"  Codebehind="CacheDependencies.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/Inputs/LargeTextArea.ascx" TagName="LargeTextArea" TagPrefix="uc1" %>
<cms:CMSCheckBox runat="server" ID="chkDependencies" /><br />
<uc1:LargeTextArea ID="txtDependencies" runat="server" />

<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_AdminControls_Controls_Class_ClassTransformations"  Codebehind="ClassTransformations.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" tagname="UniGrid" tagprefix="cms" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<asp:PlaceHolder ID="plcTransformations" runat="server">
    <cms:UniGrid runat="server" ID="uniGrid" OrderBy="TransformationName" IsLiveSite="true" />
</asp:PlaceHolder>

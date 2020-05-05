<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_Trees_ObjectTree"  Codebehind="ObjectTree.ascx.cs" %>
<asp:Label runat="server" ID="lblError" ForeColor="Red" EnableViewState="false" />
<cms:UITreeView ID="treeElem" ShortID="t" runat="server" ShowLines="True" CssClass="ContentTree"
    EnableViewState="true" />

<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSFormControls_Classes_SelectAlternativeForm"  Codebehind="SelectAlternativeForm.ascx.cs" %>
<div class="control-group-inline">
    <cms:CMSTextBox ID="txtName" runat="server" MaxLength="200" CssClass="form-control"/>
    <cms:CMSButton ID="btnSelect" runat="server" ButtonStyle="Default" />
    <cms:CMSButton ID="btnClear" runat="server" ButtonStyle="Default" />
    <asp:Label ID="lblStatus" runat="server" CssClass="SelectorError" EnableViewState="False" />
</div>

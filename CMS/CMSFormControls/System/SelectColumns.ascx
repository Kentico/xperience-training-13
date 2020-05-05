<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSFormControls_System_SelectColumns"  Codebehind="SelectColumns.ascx.cs" %>
<cms:CMSTextBox ID="txtColumns" runat="server" ReadOnly="True" />
<cms:CMSButton ID="btnDesign" runat="server" Text="Select" ButtonStyle="Default" />
<asp:HiddenField ID="hdnSelectedColumns" runat="server" />
<asp:HiddenField ID="hdnProperties" runat="server" />

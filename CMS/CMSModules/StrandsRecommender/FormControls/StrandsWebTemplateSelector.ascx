<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="StrandsWebTemplateSelector.ascx.cs" Inherits="CMSModules_StrandsRecommender_FormControls_StrandsWebTemplateSelector" %>
<cms:CMSDropDownList ID="ddlTemplates" runat="server" CssClass="DropDownField" Enabled="false" />
<asp:HiddenField ID="hdnSelectedTemplate" runat="server" />
<asp:Label ID="lblNoToken" runat="server" Visible="false" CssClass="ErrorLabel" EnableViewState="false" ></asp:Label>

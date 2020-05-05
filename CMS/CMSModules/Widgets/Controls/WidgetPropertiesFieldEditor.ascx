<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Widgets_Controls_WidgetPropertiesFieldEditor"
     Codebehind="WidgetPropertiesFieldEditor.ascx.cs" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/FieldEditor.ascx"
    TagName="FieldEditor" TagPrefix="cms" %>
<cms:FieldEditor ID="fieldEditor" runat="server" IsLiveSite="false" AllowExtraFields="true" />

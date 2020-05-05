<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/AdminControls/Controls/UIControls/EditParameters.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_UIControls_EditParameters" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/FieldEditor.ascx"
    TagName="FieldEditor" TagPrefix="cms" %>
<cms:FieldEditor ID="editElem" runat="server" IsLiveSite="false" Mode="General" />

<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Controls_SelectWebTemplate"  Codebehind="SelectWebTemplate.ascx.cs" %>
<%@ Register Src="~/CMSModules/ImportExport/Controls/Global/ObjectAttachmentSelector.ascx" TagName="ObjectAttachmentSelector"
    TagPrefix="cms" %>
<asp:Label runat="server" ID="lblError" EnableViewState="false" Visible="false" />
<cms:ObjectAttachmentSelector id="ucSelector" runat="server" PanelHeight="368" />

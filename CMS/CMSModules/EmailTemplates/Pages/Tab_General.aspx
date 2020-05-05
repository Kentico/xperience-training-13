<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_EmailTemplates_Pages_Tab_General"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="Tab_General.aspx.cs" EnableEventValidation="false" %>

<%@ Register Src="~/CMSModules/EmailTemplates/Controls/Edit.ascx" TagName="EmailTemplateEdit"
    TagPrefix="cms" %>
<asp:Content ID="content" runat="server" ContentPlaceHolderID="plcContent">
    <cms:EmailTemplateEdit ID="editElem" runat="server" />
</asp:Content>
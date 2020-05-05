<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="New.aspx.cs" Inherits="CMSModules_EmailTemplates_Pages_New"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" %>

<%@ Register Src="~/CMSModules/EmailTemplates/Controls/Edit.ascx" TagName="EmailTemplateEdit"
    TagPrefix="cms" %>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="plcContent">
    <cms:EmailTemplateEdit ID="editElem" runat="server" />
</asp:Content>
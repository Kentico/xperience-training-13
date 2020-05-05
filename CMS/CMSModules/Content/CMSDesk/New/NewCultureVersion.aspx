<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_New_NewCultureVersion"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Content - New culture version"
     Codebehind="NewCultureVersion.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/NewCultureVersion.ascx"
    TagName="NewCultureVersion" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <cms:NewCultureVersion runat="server" ID="newCultureVersion" />
</asp:Content>

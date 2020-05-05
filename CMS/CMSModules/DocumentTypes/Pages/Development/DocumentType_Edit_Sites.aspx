<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Sites" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Page Type Edit - Sites"  Codebehind="DocumentType_Edit_Sites.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/ClassSites.ascx" TagName="ClassSites"
    TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ClassSites ID="classSites" runat="server" />
</asp:Content>


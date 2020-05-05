<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="MyProfile_Categories.aspx.cs" EnableEventValidation="false"
    Inherits="CMSModules_MyDesk_MyProfile_MyProfile_Categories" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/Categories/Controls/Categories.ascx" TagName="Categories"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:Categories ID="CategoriesElem" runat="server" DisplaySiteCategories="false" DisplaySiteSelector="false" IsLiveSite="false" />
</asp:Content>

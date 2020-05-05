<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true"
    Inherits="CMSModules_Membership_Pages_Users_User_Edit_Categories" Title="User - Categories"
    Theme="Default" EnableEventValidation="false"  Codebehind="User_Edit_Categories.aspx.cs" %>

<%@ Register Src="~/CMSModules/Categories/Controls/Categories.ascx" TagName="Categories"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:Categories ID="CategoriesElem" runat="server" DisplaySiteCategories="false" DisplaySiteSelector="false" IsLiveSite="false" />
</asp:Content>

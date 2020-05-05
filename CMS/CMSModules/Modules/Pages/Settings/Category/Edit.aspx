<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    AutoEventWireup="true"  Codebehind="Edit.aspx.cs" Inherits="CMSModules_Modules_Pages_Settings_Category_Edit"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/Modules/Controls/Settings/Category/SettingsCategoryEdit.ascx" TagName="SettingsCategoryEdit"
    TagPrefix="cms" %>
<asp:Content ID="Content5" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    <cms:SettingsCategoryEdit ID="catEdit" runat="server"></cms:SettingsCategoryEdit>
</asp:Content>

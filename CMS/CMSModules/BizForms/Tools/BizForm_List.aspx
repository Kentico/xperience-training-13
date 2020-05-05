<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="BizForm list" Inherits="CMSModules_BizForms_Tools_BizForm_List" Theme="Default"  Codebehind="BizForm_List.aspx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" tagname="UniGrid" tagprefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid ID="UniGridBizForms" runat="server" GridName="BizForm_List.xml" OrderBy="FormDisplayName"
        IsLiveSite="false" />
</asp:Content>

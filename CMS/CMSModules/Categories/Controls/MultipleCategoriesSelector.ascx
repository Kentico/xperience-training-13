<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Categories_Controls_MultipleCategoriesSelector"
     Codebehind="MultipleCategoriesSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<cms:UniSelector ID="selectCategory" runat="server" ReturnColumnName="CategoryID"
    ObjectType="cms.categorylist" ResourcePrefix="categoryselector" OrderBy="CategoryNamePath"
    AdditionalColumns="CategoryNamePath,CategoryEnabled" SelectionMode="Multiple"
    AllowEmpty="false" IsLiveSite="false" />

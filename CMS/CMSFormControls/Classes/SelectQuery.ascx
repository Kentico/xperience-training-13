<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Classes_SelectQuery" CodeBehind="SelectQuery.ascx.cs" %>

<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" FilterControl="~/CMSFormControls/Filters/DocTypeFilter.ascx"
            DisplayNameFormat="{%QueryFullName%}" AllowEditTextBox="False" ReturnColumnName="QueryFullName"
            ObjectType="cms.querylist" ResourcePrefix="queryselector" SelectionMode="SingleTextBox" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSFormControls_Classes_CustomTableSelector"  Codebehind="CustomTableSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" AllowEditTextBox="true" DisplayNameFormat="{%ClassDisplayName%} ({%ClassName%})"
            ReturnColumnName="ClassName" ObjectType="cms.customtable" ResourcePrefix="customtableselector"
            SelectionMode="SingleDropDownList" AllowEmpty="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>

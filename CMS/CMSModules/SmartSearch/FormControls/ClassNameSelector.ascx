<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_FormControls_ClassNameSelector"  Codebehind="ClassNameSelector.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:Uniselector runat="server" ID="selObjects" ReturnColumnName="ClassName" IsLiveSite="false" ObjectType="cms.class"
            AllowEmpty="false" AllowAll="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>

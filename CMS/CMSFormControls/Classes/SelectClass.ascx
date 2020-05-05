<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Classes_SelectClass"  Codebehind="SelectClass.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" DisplayNameFormat="{%ClassDisplayName%}"
            ReturnColumnName="ClassID" ObjectType="cms.class" ResourcePrefix="allowedclasscontrol"
            SelectionMode="SingleDropDownList" AllowEmpty="false" AllowAll="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>

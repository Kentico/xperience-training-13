<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_FormControls_Relationships_SelectRelationshipNames"
     Codebehind="SelectRelationshipNames.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" ObjectType="cms.relationshipname"
            AllowAll="false" SelectionMode="SingleDropDownList" DisplayNameFormat="{%RelationshipDisplayName%}" />
    </ContentTemplate>
</cms:CMSUpdatePanel>

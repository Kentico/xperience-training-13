<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_PortalEngine_FormControls_SelectContainer"  Codebehind="SelectContainer.ascx.cs" %>
    
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
    
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="selectContainer" runat="server" ObjectType="cms.WebPartContainer" ResourcePrefix="containerselect"
            AllowEmpty="false" SelectionMode="SingleDropDownList" ReturnColumnName="ContainerName" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
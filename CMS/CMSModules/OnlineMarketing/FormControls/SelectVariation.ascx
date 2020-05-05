<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SelectVariation.ascx.cs" Inherits="CMSModules_OnlineMarketing_FormControls_SelectVariation" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:UniSelector runat="server" ID="ucUniSelector" ObjectType="OM.ABVariant" SelectionMode="SingleDropDownList"
            ResourcePrefix="selectabvariant" ReturnColumnName="ABVariantName" AllowEmpty="false"/>
    </ContentTemplate>
</cms:CMSUpdatePanel>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_FormControls_SKUSelector"
     Codebehind="SKUSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" DisplayNameFormat="{%SKUName%}"
            ObjectType="ecommerce.skulist" ResourcePrefix="productselector"
            AllowEditTextBox="false" UseDefaultNameFilter="false" IsLiveSite="false"
            FilterControl="~/CMSModules/Ecommerce/Controls/Filters/SimpleProductFilter.ascx"/>
    </ContentTemplate>
</cms:CMSUpdatePanel>

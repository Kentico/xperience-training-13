<%@ Control Language="C#" AutoEventWireup="false"
    Inherits="CMSModules_BizForms_FormControls_SelectBizForm"  Codebehind="SelectBizForm.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" ResourcePrefix="bizformselect" ObjectType="cms.form"
            OrderBy="FormName" AllowEditTextBox="true" />
    </ContentTemplate>
</cms:CMSUpdatePanel>

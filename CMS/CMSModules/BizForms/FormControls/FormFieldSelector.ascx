<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="FormFieldSelector.ascx.cs" Inherits="CMSModules_BizForms_FormControls_FormFieldSelector" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <div class="content-block-25">
            <cms:UniSelector ID="selectForm" runat="server" ResourcePrefix="bizformselect"
                ReturnColumnName="FormName" ObjectType="cms.form" ObjectSiteName="#current" OrderBy="FormName"
                SelectionMode="SingleDropDownList" OnOnSelectionChanged="UniSelector_OnSelectionChanged" AllowEmpty="False" IsLiveSite="False" />
        </div>
        <div>
            <cms:CMSDropDownList ID="drpFields" runat="server" CssClass="DropDownField" />
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
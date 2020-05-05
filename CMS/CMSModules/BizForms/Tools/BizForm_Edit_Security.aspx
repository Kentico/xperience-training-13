<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Bizform - Security" Inherits="CMSModules_BizForms_Tools_BizForm_Edit_Security"
    Theme="Default"  Codebehind="BizForm_Edit_Security.aspx.cs" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Roles/securityAddRoles.ascx"
    TagName="AddRoles" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    <div class="form-horizontal">
        <cms:LocalizedHeading runat="server" Level="4" ID="headTitle" CssClass="listing-title" EnableViewState="false"
            Visible="true" ResourceString="Bizform.Security.lblTitle" />
        <div class="form-group">
            <div class="radio-list-vertical">
                <cms:CMSRadioButton ID="radAllUsers" runat="server" GroupName="form" AutoPostBack="True"
                    ResourceString="Bizform.Security.lblAllUsers" />
                <cms:CMSRadioButton ID="radOnlyRoles" runat="server" GroupName="form" AutoPostBack="True"
                    OnCheckedChanged="radOnlyRoles_CheckedChanged" ResourceString="Bizform.Security.lblOnlyRoles" />
            </div>
        </div>
        <div class="form-group">
            <div class="control-group-inline">
                <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" RenderMode="Inline" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cms:CMSListBox ID="lstRoles" runat="server" CssClass="PermissionsListBox" SelectionMode="Multiple"
                            DataTextField="RoleDisplayName" DataValueField="RoleID" Rows="12" />
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
                <div class="btns-vertical">
                    <cms:AddRoles runat="server" ID="addRoles" />
                    <cms:LocalizedButton ID="btnRemoveRole" runat="server" ButtonStyle="Default"
                        OnClick="btnRemoveRole_Click" ResourceString="general.remove" />
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="btnOk_Click"
                    ResourceString="general.ok" />
            </div>
        </div>
    </div>
</asp:Content>
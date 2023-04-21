<%@ Page Language="C#" AutoEventWireup="false" Inherits="CMSModules_EmailEngine_Pages_EmailOAuthCredentials_Edit"
    Theme="Default" ValidateRequest="false" EnableEventValidation="false" CodeBehind="EmailOAuthCredentials_Edit.aspx.cs" Title="Edit OAuth credentials"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSFormControls/Classes/AssemblyClassSelector.ascx" TagName="AssemblyClassSelector" TagPrefix="cms" %>

<asp:Content ID="cntHeader" runat="server" ContentPlaceHolderID="plcBeforeContent">
    <style>
        .SetupPanel {
            margin-bottom: 60px;
            display: block
        }

        .TokenStatus {
            margin-left: 4px;
            padding: 0 4px;
        }
    </style>
</asp:Content>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlForm" runat="server" EnableViewState="false" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:FormCategoryHeading runat="server" ID="headCredentials" Level="4" ResourceString="emailoauthcredentials_edit.credentialsheading" IsAnchor="True" />
            <asp:Panel ID="pnlSetup" runat="server" DefaultButton="btnSetupSave" CssClass="SetupPanel">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel ShowRequiredMark="true" CssClass="control-label" ID="lblDisplayName" runat="server" EnableViewState="False" ResourceString="general.displayname" AssociatedControlID="txtDisplayName" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox ID="txtDisplayName" runat="server" MaxLength="100" />
                            <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="txtDisplayName" Display="dynamic" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel ShowRequiredMark="true" CssClass="control-label" ID="lblAssemblyElem" runat="server" EnableViewState="false" ResourceString="emailoauthcredentials.oauthprovider"
                                                DisplayColon="true" AssociatedControlID="assemblyElem" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:AssemblyClassSelector  ID="assemblyElem" runat="server" AllowEmpty="false" ValidateAssembly="true" BaseClassNames="CMS.EmailEngine.IEmailOAuthProvider" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel ShowRequiredMark="true" CssClass="control-label" ID="lblClientID" runat="server" EnableViewState="false" ResourceString="emailoauthcredentials.clientid"
                                        DisplayColon="true" AssociatedControlID="txtClientID" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox runat="server" ID="txtClientID" EnableViewState="false" />
                            <cms:CMSRequiredFieldValidator ID="rfvClientID" runat="server" ControlToValidate="txtClientID" Display="dynamic" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel ShowRequiredMark="true" CssClass="control-label" ID="lblClientSecret" runat="server" EnableViewState="false" ResourceString="emailoauthcredentials.clientsecret"
                                        DisplayColon="true" AssociatedControlID="txtClientSecret" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox runat="server" ID="txtClientSecret" EnableViewState="false" />
                            <cms:CMSRequiredFieldValidator ID="rfvClientSecret" runat="server" ControlToValidate="txtClientSecret" Display="dynamic" />                           
                        </div>
                    </div>
                    <asp:PlaceHolder ID="plcTenantID" runat="server">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel ShowRequiredMark="true" CssClass="control-label" ID="lblTenantID" runat="server" EnableViewState="false" ResourceString="emailoauthcredentials.tenantid"
                                            DisplayColon="true" AssociatedControlID="txtTenantID" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox runat="server" ID="txtTenantID" EnableViewState="false" />
                                <cms:CMSRequiredFieldValidator ID="rfvTenantID" runat="server" ControlToValidate="txtTenantID" Display="dynamic" />
                            </div>
                    </div>
                    </asp:PlaceHolder>
                    <div class="form-group form-group-submit">
                            <cms:CMSButton runat="server" ButtonStyle="Primary" ID="btnSetupSave" OnClick="ButtonSetupSave_Click" EnableViewState="false" />
                    </div>
                </div>
            </asp:Panel>
            <cms:FormCategoryHeading runat="server" ID="headToken" Level="4" ResourceString="emailoauthcredentials_edit.tokenheading" IsAnchor="True" />
            <asp:Panel ID="pnlAccessToken" runat="server">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblGetToken" runat="server" EnableViewState="false" ResourceString="emailoauthcredentials.tokenstatus"
                                DisplayColon="true" />
                        </div>
                        <div class="control-group-inline">
                                <asp:Literal ID="ltlTokenStatus" runat="server" EnableViewState="false" />
                            <cms:LocalizedButton runat="server" ID="btnGetToken" OnClick="ButtonGetToken_Click" ResourceString="EmailOAuthCredentials_Edit.GetToken" CssClass="btn btn-primary" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>

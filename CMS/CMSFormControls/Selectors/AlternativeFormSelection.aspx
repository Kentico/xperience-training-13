<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSFormControls_Selectors_AlternativeFormSelection" ValidateRequest="false"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Alternative form selection" CodeBehind="AlternativeFormSelection.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagPrefix="cms" TagName="UniSelector" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblClass" runat="server" ResourceString="general.class" DisplayColon="true" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:UniSelector runat="server" DropDownSingleSelect-AutoPostBack="True" ID="drpClass" SelectionMode="SingleDropDownList" AllowEmpty="False" ObjectType="cms.class"
                            OnOnSelectionChanged="drpClass_SelectedIndexChanged" DisplayNameFormat="{%ClassDisplayName%} ({%ClassName%})" OrderBy="ClassName, ClassDisplayName"
                            ReturnColumnName="ClassID" WhereCondition="ClassID IN (SELECT DISTINCT FormClassID FROM CMS_AlternativeForm)" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="LocalizedLabel1" runat="server" ResourceString="objecttype.cms_alternativeform" DisplayColon="true" EnableViewState="false" AssociatedControlID="lstAlternativeForms" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSListBox ID="lstAlternativeForms" runat="server" CssClass="DesignerListBox" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
<asp:Content ID="plcFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedHidden ID="constNoSelection" ClientIDMode="Static" runat="server" Value="{$altforms_selectaltform.noitemselected$}" EnableViewState="false" />
    </div>
</asp:Content>

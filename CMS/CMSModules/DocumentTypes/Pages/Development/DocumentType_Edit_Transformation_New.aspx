<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Transformation_New"
    ValidateRequest="false" Theme="Default" EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="New transformation"  Codebehind="DocumentType_Edit_Transformation_New.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/Filters/DocTypeFilter.ascx" TagName="DocTypeFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Layouts/TransformationCode.ascx" TagName="TransformationCode"
    TagPrefix="cms" %>

<asp:Content ID="cntControls" runat="server" ContentPlaceHolderID="plcControls">
    <cms:DocTypeFilter runat="server" ID="filter" RenderTableTag="true" EnableViewState="true" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UIForm runat="server" ID="editElem" ObjectType="cms.transformation" DefaultFieldLayout="TwoColumns"
        CssClass="Transformation" RedirectUrlAfterCreate="">
        <LayoutTemplate>
            <cms:FormCategory runat="server" ID="pnlGeneral" CategoryTitleResourceString="general.general">
                <cms:FormField runat="server" ID="fDisplayName" Field="TransformationName" FormControl="TextBoxControl"
                    ResourceString="transformationlist.transformationname" DisplayColon="true" />
            </cms:FormCategory>
            <cms:FormCategory runat="server" ID="pnlCategory" CategoryTitleResourceString="objecttype.cms_transformation"
                CssClass="TransformationCode" DefaultFieldLayout="Inline">
                <cms:FormField runat="server" ID="fCode" Field="TransformationCode" Layout="Inline">
                    <cms:TransformationCode runat="server" ID="ucTransfCode" />
                </cms:FormField>
            </cms:FormCategory>
            <cms:FormSubmitButton runat="server" ID="btnSubmit" />
        </LayoutTemplate>
    </cms:UIForm>
</asp:Content>
<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeBehind="BizForm_Edit_FormBuilderMVC.aspx.cs" Inherits="CMSModules_BizForms_Tools_BizForm_Edit_FormBuilderMVC" Theme="Default" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MessagesPlaceHolder runat="server" ID="plcMess" OffsetX="16" OffsetY="16" UseRelativePlaceHolder="False">
        <div>
            <cms:AlertLabel runat="server" ID="alWarning" AlertType="Warning" Text="&nbsp;" CssClass="alert-dismissable alert-warning-absolute hidden" EnableViewState="False" />
        </div>
        <div>
            <cms:AlertLabel runat="server" ID="alError" AlertType="Error" Text="&nbsp;" CssClass="alert-dismissable alert-error-absolute hidden" EnableViewState="False" />
        </div>
    </cms:MessagesPlaceHolder>
    <cms:UILayout runat="server">
        <Panes>
            <cms:UILayoutPane ID="formBuilderFrame" runat="server" Direction="Center" RenderAs="Iframe" AppendSrc="true" Src="about:blank" />
        </Panes>
    </cms:UILayout>
</asp:Content>

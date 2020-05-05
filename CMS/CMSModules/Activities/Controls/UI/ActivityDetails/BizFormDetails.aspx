<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Inherits="CMSModules_Activities_Controls_UI_ActivityDetails_BizFormDetails" Theme="Default"
     Codebehind="BizFormDetails.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MessagesPlaceHolder runat="server" ID="plcMess" OffsetX="16" OffsetY="16" UseRelativePlaceHolder="False">
        <div>
            <cms:AlertLabel runat="server" ID="alWarning" AlertType="Warning" Text="&nbsp;" CssClass="alert-dismissable alert-warning-absolute hidden" EnableViewState="False" />
        </div>
    </cms:MessagesPlaceHolder>
    <iframe ID="mvcFrame" runat="server" style="width:100%;height:100%;" frameborder="0" Visible="true"></iframe>
</asp:Content>
<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Blogs_Controls_Comment_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Comment edit"  Codebehind="Comment_Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/Blogs/Controls/BlogCommentEdit.ascx" TagName="BlogCommentEdit"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlContent">
        <cms:BlogCommentEdit ID="ctrlCommentEdit" runat="server" DisplayButtons="false" AdvancedMode="true" />
    </asp:Panel>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>

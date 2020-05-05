<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Blogs_CMSPages_Comment_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/LiveSite/Dialogs/ModalDialogPage.master"
    Title="Comment edit"  Codebehind="Comment_Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/Blogs/Controls/BlogCommentEdit.ascx" TagName="BlogCommentEdit" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlBody" CssClass="PageBody">
        <asp:Panel runat="server" ID="pnlContent" >
            <cms:BlogCommentEdit ID="ctrlCommentEdit" runat="server" DisplayButtons="false" AdvancedMode="true" />
        </asp:Panel>
    </asp:Panel>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <cms:CMSButton ID="btnOk" runat="server" ButtonStyle="Primary"
        EnableViewState="false" />
</asp:Content>

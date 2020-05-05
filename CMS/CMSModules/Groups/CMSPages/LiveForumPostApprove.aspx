<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Forum post detail" Inherits="CMSModules_Groups_CMSPages_LiveForumPostApprove"
    Theme="Default"  Codebehind="LiveForumPostApprove.aspx.cs" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Posts/PostApprove.ascx" TagName="PostApprove"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Posts/PostApproveFooter.ascx" TagName="PostApproveFooter"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent" EnableViewState="false">
    <cms:PostApprove ID="PostApprove" runat="server" IsLiveSite="true" />    
</asp:Content>    

<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server" EnableViewState="false">
    <cms:PostApproveFooter ID="PostApproveFooter" runat="server" IsLiveSite="true" />
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
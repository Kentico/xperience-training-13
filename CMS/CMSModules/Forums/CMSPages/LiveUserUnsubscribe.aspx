<%@ Page Language="C#" AutoEventWireup="true" Theme="Default"  MasterPageFile="~/CMSMasterPages/LiveSite/Dialogs/ModalDialogPage.master" Title="Forum post unsubscribe"  Codebehind="LiveUserUnsubscribe.aspx.cs" Inherits="CMSModules_Forums_CMSPages_LiveUserUnsubscribe" %>

<%@ Register Src="~/CMSModules/Forums/Controls/Posts/PostApprove.ascx" TagName="PostApprove"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Posts/PostApproveFooter.ascx" TagName="PostApproveFooter"
    TagPrefix="cms" %>


<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent" EnableViewState="false">
    <cms:PostApprove ID="PostApprove" runat="server" />    
</asp:Content>    

<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server" EnableViewState="false">
    <cms:PostApproveFooter ID="PostApproveFooter" runat="server" />
</asp:Content>
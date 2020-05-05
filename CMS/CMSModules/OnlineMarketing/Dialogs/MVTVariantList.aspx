<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Mvtest properties" Inherits="CMSModules_OnlineMarketing_Dialogs_MVTVariantList"
    Theme="Default"  Codebehind="MVTVariantList.aspx.cs" %>

<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/MVTVariant/List.ascx"
    TagName="MvtVariantList" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MvtVariantList ID="listElem" runat="server" IsLiveSite="false" />
    <asp:Literal ID="ltrScript" runat="server"></asp:Literal>
</asp:Content>

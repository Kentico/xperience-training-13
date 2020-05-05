<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Mvtest properties" Inherits="CMSModules_OnlineMarketing_Dialogs_ContentPersonalizationVariantList"
    Theme="Default"  Codebehind="ContentPersonalizationVariantList.aspx.cs" %>

<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/ContentPersonalizationVariant/List.ascx"
    TagName="VariantList" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:VariantList ID="listElem" runat="server" IsLiveSite="false" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <asp:Literal ID="ltrScript" runat="server"></asp:Literal>
</asp:Content>

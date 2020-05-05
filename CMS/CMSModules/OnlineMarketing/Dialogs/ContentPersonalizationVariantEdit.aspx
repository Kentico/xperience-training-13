<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Variant properties" Inherits="CMSModules_OnlineMarketing_Dialogs_ContentPersonalizationVariantEdit"
    Theme="Default"  Codebehind="ContentPersonalizationVariantEdit.aspx.cs" %>

<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/ContentPersonalizationVariant/Edit.ascx"
    TagName="VariantEdit" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:VariantEdit ID="editElem" runat="server" IsLiveSite="false" />
    <asp:Literal ID="ltrScript" runat="server"></asp:Literal>
</asp:Content>
<asp:Content runat="server" ID="cntFooter" ContentPlaceHolderID="plcFooter">
    <cms:LocalizedButton runat="server" ID="btnSubmit" OnClick="btnSubmit_Click" ResourceString="general.saveandclose"
        ButtonStyle="Primary" />
</asp:Content>

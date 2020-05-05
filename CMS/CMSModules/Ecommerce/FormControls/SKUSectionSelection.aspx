<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="SKUSectionSelection.aspx.cs" Inherits="CMSModules_Ecommerce_FormControls_SKUSectionSelection"
    Title="Selection dialog" ValidateRequest="false" Theme="default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Trees/UniTree.ascx" TagName="UniTree" TagPrefix="cms" %>

<asp:Content ID="cntActions" ContentPlaceHolderID="plcActions" runat="server">
    <div class="header-actions-container">
        <cms:HeaderActions ID="actionsElem" ShortID="a" runat="server" IsLiveSite="false" />
    </div>
</asp:Content>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel ID="pnlBody" runat="server" CssClass="UniSelectorDialogBody Categories">
        <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel runat="server" ID="pnlContent">
                    <div class="ContentTree">
                        <asp:Panel ID="pnlTreeArea" runat="server" CssClass="TreeAreaTree">
                            <cms:UniTree runat="server" ID="treeElemG" ShortID="tg" Localize="true" IsLiveSite="false" OnOnNodeCreated="treeElem_OnNodeCreated" />
                            <div class="ClearBoth">
                            </div>
                        </asp:Panel>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
        <cms:CMSUpdatePanel runat="server" ID="pnlHidden" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:HiddenField ID="hidItem" runat="server" EnableViewState="false" />
                <asp:HiddenField ID="hidHash" runat="server" EnableViewState="false" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</asp:Content>

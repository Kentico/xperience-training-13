<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="CategorySelection.aspx.cs"
    Inherits="CMSModules_Categories_Dialogs_CategorySelection" Title="Selection dialog"
    ValidateRequest="false" Theme="default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Categories/Controls/CategorySelectionDialog.ascx"
    TagName="SelectionDialog" TagPrefix="cms" %>
<asp:Content ID="cntActions" ContentPlaceHolderID="plcActions" runat="server">
    <cms:CMSUpdatePanel runat="server" ID="pnlActions">
        <ContentTemplate>
            <div class="header-actions-container">
                <cms:HeaderActions ID="actionsElem" ShortID="a" runat="server" IsLiveSite="false" />
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:SelectionDialog ID="SelectionElem" runat="server" IsLiveSite="false" />
</asp:Content>

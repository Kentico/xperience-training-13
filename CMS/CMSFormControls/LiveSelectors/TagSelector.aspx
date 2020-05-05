<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_LiveSelectors_TagSelector"
    Theme="Default" MasterPageFile="~/CMSMasterPages/LiveSite/Dialogs/ModalDialogPage.master"
    Title="Metadata - Select tags"  Codebehind="TagSelector.aspx.cs" %>
<%@ Register Src="~/CMSModules/TagGroups/Controls/TagSelectorDialog.ascx" TagPrefix="cms" TagName="TagSelectorDialog" %>

<asp:Content ContentPlaceHolderID="plcContent" runat="Server">
    <cms:TagSelectorDialog runat="server" id="tagSelectorDialog" />
</asp:Content>
<asp:Content ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" ResourceString="general.ok" EnableViewState="false" />
        <cms:LocalizedButton ID="btnCancel" runat="server" ButtonStyle="Primary" ResourceString="general.cancel" EnableViewState="false" OnClientClick="return CloseDialog();" />
    </div>
</asp:Content>

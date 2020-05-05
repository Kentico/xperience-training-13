<%@ Page Title="Comment" Language="C#" Theme="Default" MasterPageFile="~/CMSMasterPages/LiveSite/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true" EnableEventValidation="false" Inherits="CMSModules_Workflows_CMSPages_Comment"
     Codebehind="Comment.aspx.cs" %>

<%@ Register Src="~/CMSModules/Workflows/Controls/UI/Comment.ascx" TagName="Comment"
    TagPrefix="cms" %>
<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel ID="pnlContent" CssClass="PageContent" runat="server">
        <asp:PlaceHolder ID="plcContent" runat="server">
            <cms:Comment ID="ucComment" runat="server" />
        </asp:PlaceHolder>
    </asp:Panel>
</asp:Content>
<asp:Content ID="footer" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" ResourceString="general.Save"
            EnableViewState="false" />
        <cms:LocalizedButton ID="btnCancel" runat="server" ButtonStyle="Primary" ResourceString="general.cancel"
            EnableViewState="false" OnClientClick="return CloseDialog();" />
    </div>
</asp:Content>

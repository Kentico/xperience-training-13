<%@ Page Title="Comment" Language="C#" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true" EnableEventValidation="false" Inherits="CMSModules_Workflows_Pages_Comment"
     Codebehind="Comment.aspx.cs" %>

<%@ Register Src="~/CMSModules/Workflows/Controls/UI/Comment.ascx" TagName="Comment"
    TagPrefix="cms" %>
<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel ID="pnlContent" runat="server">
        <asp:PlaceHolder ID="plcContent" runat="server">
            <cms:Comment ID="ucComment" runat="server" IsLiveSite="false" />
        </asp:PlaceHolder>
    </asp:Panel>
</asp:Content>

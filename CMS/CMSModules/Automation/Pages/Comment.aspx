<%@ Page Title="Comment" Language="C#" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true" EnableEventValidation="false" Inherits="CMSModules_Automation_Pages_Comment"
     Codebehind="Comment.aspx.cs" %>

<%@ Register Src="~/CMSModules/Automation/Controls/Comment.ascx" TagName="Comment"
    TagPrefix="cms" %>
<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel ID="pnlContent" runat="server">
        <asp:PlaceHolder ID="plcContent" runat="server">
            <cms:CMSAutomationManager ID="autoMan" runat="server" />
            <cms:Comment ID="ucComment" runat="server" IsLiveSite="false" />
        </asp:PlaceHolder>
    </asp:Panel>
</asp:Content>

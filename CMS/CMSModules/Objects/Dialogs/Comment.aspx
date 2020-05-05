<%@ Page Title="Comment" Language="C#" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true" EnableEventValidation="false" Inherits="CMSModules_Objects_Dialogs_Comment"
     Codebehind="Comment.aspx.cs" %>

<%@ Register Src="~/CMSModules/Objects/Controls/Locking/Comment.ascx" TagName="Comment"
    TagPrefix="cms" %>
<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:PlaceHolder ID="plcContent" runat="server">
        <cms:Comment ID="ucComment" runat="server" IsLiveSite="false" />
    </asp:PlaceHolder>
</asp:Content>

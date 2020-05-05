<%@ Page Language="C#" Theme="Default" AutoEventWireup="true"
    Inherits="CMSFormControls_Selectors_SelectFileOrFolder_Footer" EnableEventValidation="false"
    MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"  Codebehind="Footer.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/General/FileSystemDialogFooter.ascx" TagName="Footer"
    TagPrefix="cms" %>
<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="server">
    <div class="PageFooterLine">
        <cms:Footer ID="footerElem" runat="server" IsLiveSite="false" />
        <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    </div>  
</asp:Content>

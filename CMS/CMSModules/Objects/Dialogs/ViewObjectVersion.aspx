<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="ViewObjectVersion.aspx.cs"
    Inherits="CMSModules_Objects_Dialogs_ViewObjectVersion" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="View Object Version" %>

<%@ Register Src="~/CMSModules/Objects/Controls/Versioning/ViewObjectVersion.ascx"
    TagName="ViewObjectVersion" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:ViewObjectVersion ID="viewVersion" runat="server" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>

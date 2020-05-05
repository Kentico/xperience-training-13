<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="ObjectVersionDialog.aspx.cs"
    Inherits="CMSModules_Objects_Dialogs_ObjectVersionDialog" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="View Object Version" Theme="Default" %>

<%@ Register Src="~/CMSModules/Objects/Controls/Versioning/ObjectVersionList.ascx"
    TagPrefix="cms" TagName="VersionList" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:VersionList ID="versionList" runat="server" />
</asp:Content>

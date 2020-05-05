<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Dialogs_Properties_DocCopyMoveProperites"
    Theme="Default"  Codebehind="DocCopyMoveProperites.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" %>

<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Properties/CopyMoveLinkProperties.ascx"
    TagName="CopyMoveLinkProperties" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <cms:CopyMoveLinkProperties ID="copyMoveLinkElem" runat="server" />
</asp:Content>

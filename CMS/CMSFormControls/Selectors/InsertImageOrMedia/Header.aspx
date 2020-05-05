<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/TabsHeader.master" Theme="Default"
    AutoEventWireup="true" EnableEventValidation="false"
    Inherits="CMSFormControls_Selectors_InsertImageOrMedia_Header"  Codebehind="Header.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/General/DialogHeader.ascx" TagName="DialogHeader"
    TagPrefix="cms" %>
<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="server">
    <cms:DialogHeader ID="header" runat="server" IsLiveSite="false" />
</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="RecycleBin_Documents.aspx.cs" Inherits="CMSModules_MyDesk_RecycleBin_RecycleBin_Documents" Theme="Default" EnableEventValidation="false" MaintainScrollPositionOnPostback="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Pages recycle bin" %>

<%@ Register Src="~/CMSModules/RecycleBin/Controls/RecycleBin.ascx" TagName="RecycleBin"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:RecycleBin ID="recycleBin" runat="server" IsCMSDesk="true" IsLiveSite="false" DisplayDateTimeFilter="True" />
</asp:Content>

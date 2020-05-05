<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="RecycleBin_Objects.aspx.cs"
    Inherits="CMSModules_MyDesk_RecycleBin_RecycleBin_Objects" Theme="Default" EnableEventValidation="false"
    MaintainScrollPositionOnPostback="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="MyDesk - Objects recycle bin" %>

<%@ Register Src="~/CMSModules/Objects/Controls/ObjectsRecycleBin.ascx" TagName="RecycleBin"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:RecycleBin ID="recycleBin" runat="server" IsLiveSite="false" DisplayDateTimeFilter="True" />
</asp:Content>

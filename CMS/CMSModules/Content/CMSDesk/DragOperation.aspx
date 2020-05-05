<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_DragOperation"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Content - Drop"  Codebehind="DragOperation.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/DragOperation.ascx" TagName="DragOperation" TagPrefix="cms" %>
<asp:Content ID="cnt1" ContentPlaceHolderID="plcContent" runat="server">
    <cms:DragOperation runat="server" id="opDrag" IsLiveSite="false" />
</asp:Content>

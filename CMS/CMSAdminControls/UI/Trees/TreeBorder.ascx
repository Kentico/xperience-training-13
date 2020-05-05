<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_Trees_TreeBorder"
     Codebehind="TreeBorder.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/FrameResizer.ascx" TagName="FrameResizer"
    TagPrefix="cms" %>
<cms:FrameResizer ID="frmResizer" runat="server" MinSize="8,*" FramesetName="colsFrameset" />
<asp:Panel runat="server" ID="pnlBorder" CssClass="TreeBorder">
    &nbsp;
</asp:Panel>

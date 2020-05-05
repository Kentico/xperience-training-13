<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_ImageEditor_ImageEditor"  Codebehind="ImageEditor.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/ImageEditor/BaseImageEditor.ascx" TagName="BaseImageEditor"
    TagPrefix="cms" %>
<asp:PlaceHolder ID="plcContent" runat="server" >
    <cms:BaseImageEditor ID="baseImageEditor" runat="server" />
</asp:PlaceHolder>

<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Slider.ascx.cs" Inherits="CMSFormControls_Basic_Slider" %>
<asp:Panel ID="pnlContainer" runat="server">
    <cms:CMSTextBox ID="textbox" runat="server" />
    <asp:Label ID="lblValue" runat="server" />
    <ajaxToolkit:SliderExtender ID="exSlider" runat="server" TargetControlID="textbox"
        BoundControlID="lblValue" />
</asp:Panel>

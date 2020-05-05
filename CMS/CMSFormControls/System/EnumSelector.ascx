<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="EnumSelector.ascx.cs" Inherits="CMSFormControls_System_EnumSelector" %>
<asp:Label ID="lblError" runat="server" EnableViewState="false" CssClass="LineErrorLabel" Visible="false" />
<cms:CMSDropDownList ID="drpEnum" runat="server" CssClass="DropDownField" />
<cms:CMSRadioButtonList ID="radEnum" runat="server" Visible="false" />
<cms:CMSCheckBoxList ID="chkEnum" runat="server" Visible="false" />

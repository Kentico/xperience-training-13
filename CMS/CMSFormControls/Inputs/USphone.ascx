<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Inputs_USphone"  Codebehind="USphone.ascx.cs" %>
<span class="form-control-text">(</span>
<cms:CMSTextBox runat="server" ID="txt1st" MaxLength="3" CssClass="input-width-15" />
<span class="form-control-text">)</span>
<cms:CMSTextBox runat="server" ID="txt2nd" MaxLength="3" CssClass="input-width-15" />
<span class="form-control-text">-</span>
<cms:CMSTextBox runat="server" ID="txt3rd" MaxLength="4" CssClass="input-width-20" />
<cms:LocalizedLabel ID="lbl2nd" AssociatedControlID="txt2nd" EnableViewState="false" runat="server" ResourceString="USPhone.2nd" CssClass="sr-only"/>
<cms:LocalizedLabel ID="lbl3rd" AssociatedControlID="txt3rd" EnableViewState="false" ResourceString="USPhone.3rd" runat="server" CssClass="sr-only"/>
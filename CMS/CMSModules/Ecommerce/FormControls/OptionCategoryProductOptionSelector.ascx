<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="OptionCategoryProductOptionSelector.ascx.cs"
    Inherits="CMSModules_Ecommerce_FormControls_OptionCategoryProductOptionSelector" %>

<cms:LocalizedLabel runat="server" ID="lblInfoMessage" ResourceString="com.skuoptions.notavailablecreating" CssClass="explanation-text" Visible="false" />
<cms:CMSDropDownList ID="ddlDropDown" runat="server" OnDataBound="SelectionControl_DataBound" DataTextField="SKUName" DataValueField="SKUID" CssClass="DropDownField" />
<cms:CMSCheckBoxList ID="chbCheckBoxes" runat="server" OnDataBound="SelectionControl_DataBound" DataTextField="SKUName" DataValueField="SKUID" />
<cms:CMSRadioButtonList ID="rblRadioButtons" runat="server" OnDataBound="SelectionControl_DataBound" DataTextField="SKUName" DataValueField="SKUID" />
<cms:CMSTextBox ID="txtText" runat="server" />
<asp:Label ID="lblTextPrice" runat="server" />
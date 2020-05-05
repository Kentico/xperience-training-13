<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Basic_ThreeStateCheckBox"  Codebehind="ThreeStateCheckBox.ascx.cs" %>
<div class="radio-list-vertical">
    <cms:CMSRadioButton ID="rbPositive" runat="server" GroupName="ThreeState_<%=ClientID %>" />
    <cms:CMSRadioButton ID="rbNegative" runat="server" GroupName="ThreeState_<%=ClientID %>" />
    <cms:CMSRadioButton ID="rbNotSet" runat="server" GroupName="ThreeState_<%=ClientID %>" /> 
</div>

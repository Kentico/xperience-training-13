<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_Controls_Edit_ClassFields"  Codebehind="ClassFields.ascx.cs" %>
<asp:Panel ID="pnlBody" runat="server">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <div runat="server" id="pnlButton" class="ClassFieldsButtonPanel">
        <cms:LocalizedButton runat="server" ID="btnAutomatically" ResourceString="srch.automatically"
            Visible="false" ButtonStyle="Default" />
    </div>
    <asp:Panel ID="pnlContent" runat="server">
    </asp:Panel>
</asp:Panel>

<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Layouts_Wizard_WizardHeader"
     Codebehind="~/CMSWebParts/Layouts/Wizard/WizardHeader.ascx.cs" %>
<div class="StepHeader">
    <asp:PlaceHolder runat="server" ID="plcIcon">
        <asp:Image runat="server" ID="imgIcon" CssClass="StepIcon" />
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="plcHeader">
        <h1 class="StepName">
            <asp:Literal runat="server" ID="ltlHeader" /></h1>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="plcInfo">
        <div class="StepInfo">
            <asp:Literal runat="server" ID="ltlInfo" />
        </div>
    </asp:PlaceHolder>
</div>

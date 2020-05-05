<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_AbuseReport_Controls_AbuseReportEdit"  Codebehind="AbuseReportEdit.ascx.cs" %>

<asp:Panel ID="pnlBody" runat="server" EnableViewState="false" CssClass="abuse-report">
    <div>
        <cms:LocalizedLabel ID="lblText" EnableViewState="false" ResourceString="abuse.abusetext"
            runat="server" AssociatedControlID="txtText" />
        <cms:CMSTextArea ID="txtText" runat="server" MaxLength="1000"
            EnableViewState="false" Rows="13"/>
    </div>
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <div class="Messages">
        <cms:CMSRequiredFieldValidator ID="rfvText" runat="server" ControlToValidate="txtText"
            CssClass="ErrorLabel" Display="Static" />
    </div>
    <asp:PlaceHolder ID="plcButtons" runat="server">
        <div class="FloatRight">
            <cms:LocalizedButton ID="btnReport" runat="server" ResourceString="abuse.reportabuse"
               ButtonStyle="Primary" EnableViewState="false" OnClick="btnReport_Click" />
        </div>
    </asp:PlaceHolder>
</asp:Panel>
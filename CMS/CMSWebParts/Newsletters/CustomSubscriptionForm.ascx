<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/Newsletters/CustomSubscriptionForm.ascx.cs" Inherits="CMSWebParts_Newsletters_CustomSubscriptionForm" %>
<asp:Label ID="lblError" runat="server" EnableViewState="false" Visible="false" />
<asp:Label ID="lblInfo" runat="server" EnableViewState="false" Visible="false" />
<asp:Panel ID="pnlSubscription" runat="server" DefaultButton="btnSubmit" CssClass="Subscription">
    <cms:MessagesPlaceHolder runat="server" ID="plcMess" ShortID="p" />
    <cms:BasicForm ID="formElem" runat="server" DefaultFormLayout="SingleTable" />
    <asp:PlaceHolder runat="server" ID="plcNwsList">
        <div class="NewsletterList">
            <cms:CMSCheckBoxList runat="server" ID="chklNewsletters" CssClass="NewsletterList" />
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="pnlButtonSubmit" runat="server">
        <cms:LocalizedButton ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" ButtonStyle="Primary" CssClass="SubscriptionButton"
                             EnableViewState="false" />
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="pnlImageSubmit" runat="server">
        <asp:ImageButton ID="btnImageSubmit" runat="server" OnClick="btnSubmit_Click" EnableViewState="false" />
    </asp:PlaceHolder>
</asp:Panel>
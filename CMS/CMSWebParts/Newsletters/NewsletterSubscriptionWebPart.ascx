<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Newsletters_NewsletterSubscriptionWebPart"  Codebehind="~/CMSWebParts/Newsletters/NewsletterSubscriptionWebPart.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput" TagPrefix="cms" %>

<asp:Panel ID="pnlSubscription" runat="server" DefaultButton="btnSubmit" CssClass="Subscription">
    <asp:Label runat="server" ID="lblInfo" CssClass="InfoMessage" EnableViewState="false"
        Visible="false" />
    <asp:Label runat="server" ID="lblError" CssClass="ErrorMessage" EnableViewState="false"
        Visible="false" />
    <div class="NewsletterSubscription">
        <div class="form-horizontal">
            <asp:PlaceHolder runat="server" ID="plcFirstName">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblFirstName" runat="server" AssociatedControlID="txtFirstName"
                            EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtFirstName" runat="server" MaxLength="100" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="plcLastName">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblLastName" runat="server" AssociatedControlID="txtLastName"
                            EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtLastName" runat="server" MaxLength="100" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="plcEmail">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblEmail" runat="server" AssociatedControlID="txtEmail:txtEmailInput" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:EmailInput ID="txtEmail" runat="server" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="plcNwsList">
                <div class="form-group">
                    <div class="editing-form-value-cell editing-form-value-cell-offset">
                        <cms:CMSCheckBoxList runat="server" ID="chklNewsletters" CssClass="NewsletterList" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="pnlButtonSubmit" runat="server">
                <div class="form-group form-group-submit">
                    <cms:LocalizedButton ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" ButtonStyle="Primary" CssClass="SubscriptionButton"
                        EnableViewState="false" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="pnlImageSubmit" runat="server">
                <div class="form-group form-group-submit">
                    <asp:ImageButton ID="btnImageSubmit" runat="server" OnClick="btnSubmit_Click" EnableViewState="false" />
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Panel>

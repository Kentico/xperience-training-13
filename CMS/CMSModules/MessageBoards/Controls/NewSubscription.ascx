<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MessageBoards_Controls_NewSubscription"  Codebehind="NewSubscription.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput"
    TagPrefix="cms" %>

<asp:Panel ID="pnlContent" runat="server" CssClass="new-subscription-form">
                <cms:MessagesPlaceHolder ID="plcMessages" runat="server" />
    <asp:Panel runat="server" ID="pnlPadding" DefaultButton="btnOK">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblEmail" runat="server" AssociatedControlID="txtEmail" EnableViewState="false" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:EmailInput ID="txtEmail" runat="server" />
                                <cms:CMSRequiredFieldValidator ID="rfvEmailRequired" runat="server" Display="Dynamic" EnableViewState="false" />
                            </div>
                        </div>
                        <div class="form-group form-group-submit">
                                <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOK_Click"
                                    EnableViewState="false" />
                        </div>
                    </div>
                </asp:Panel>
</asp:Panel>
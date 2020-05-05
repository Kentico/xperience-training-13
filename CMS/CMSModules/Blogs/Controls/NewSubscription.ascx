<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Blogs_Controls_NewSubscription"  Codebehind="NewSubscription.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput" TagPrefix="cms" %>
<asp:Panel ID="pnlContent" runat="server" CssClass="BoardNewPost">
    <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" Visible="false" EnableViewState="false" />
    <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" Visible="false" EnableViewState="false" />
    <asp:Panel runat="server" ID="pnlPadding" CssClass="FormPadding" DefaultButton="btnOK">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblEmail" CssClass="control-label" runat="server" AssociatedControlID="txtEmail" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:EmailInput ID="txtEmail" runat="server" /><br />
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

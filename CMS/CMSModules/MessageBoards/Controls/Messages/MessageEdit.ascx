<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MessageBoards_Controls_Messages_MessageEdit"  Codebehind="MessageEdit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput"
    TagPrefix="cms" %>

<cms:LocalizedLabel ID="lblAlreadyrated" runat="server" EnableViewState="false" CssClass="AlreadyRatedMessage" Visible="false" />
<asp:Panel ID="pnlMessageEdit" runat="server" CssClass="message-edit">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <div class="form-horizontal">
        <asp:PlaceHolder ID="plcRating" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" ID="lblRating" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Panel ID="pnlRating" runat="server" CssClass="BoardCntRating" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcUserName" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" ID="lblUserName" AssociatedControlID="txtUserName" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtUserName" runat="server" EnableViewState="false" MaxLength="250" ProcessMacroSecurity="false" /><br />
                    <cms:CMSRequiredFieldValidator ID="rfvUserName" runat="server" ControlToValidate="txtUserName"
                        ValidationGroup="MessageEdit" Display="Dynamic" EnableViewState="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcUrl" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" ID="lblURL" AssociatedControlID="txtURL" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtURL" runat="server" EnableViewState="false" MaxLength="440" ProcessMacroSecurity="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcEmail" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" ID="lblEmail" AssociatedControlID="txtEmail" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:EmailInput ID="txtEmail" runat="server" EnableViewState="false" MaxLength="250" ProcessMacroSecurity="false" ValidateClientSide="true"/><br />
                    <cms:CMSRequiredFieldValidator ID="rfvEmail" runat="server" Display="Dynamic" EnableViewState="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" ID="lblMessage" AssociatedControlID="txtMessage" runat="server" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextArea ID="txtMessage" runat="server" Rows="4" EnableViewState="false"
                    ProcessMacroSecurity="false" /><br />
                <cms:CMSRequiredFieldValidator ID="rfvMessage" runat="server" ControlToValidate="txtMessage" ValidationGroup="MessageEdit" Display="Dynamic" EnableViewState="false" />
            </div>
        </div>
        <asp:PlaceHolder ID="plcAdvanced" runat="server" Visible="false">
            <asp:PlaceHolder ID="plcApproved" runat="server" EnableViewState="false">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblApproved" AssociatedControlID="chkApproved" runat="server" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkApproved" runat="server" CssClass="CheckBoxMovedLeft" EnableViewState="false" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" ID="lblSpam" AssociatedControlID="chkSpam" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkSpam" runat="server" CssClass="CheckBoxMovedLeft" EnableViewState="false" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" ID="lblInsertedCaption" runat="server" EnableViewState="false" Visible="false" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label ID="lblInserted" runat="server" Visible="false" CssClass="form-control-text" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcChkSubscribe" runat="server" EnableViewState="false">
            <div class="form-group">
                <div class="editing-form-value-cell editing-form-value-cell-offset">
                    <cms:CMSCheckBox ID="chkSubscribe" runat="server" EnableViewState="false" />
                    <cms:LocalizedLabel ID="lblSubscribe" runat="server" CssClass="control-label" AssociatedControlID="chkSubscribe" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="pnlOkButton" runat="server" Visible="true">
            <div class="form-group form-group-submit">
                <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOk_Click"
                    EnableViewState="false" />
            </div>
        </asp:PlaceHolder>
    </div>
    <asp:PlaceHolder ID="plcFooter" runat="server" Visible="false">
        <div class="PageFooterLine">
            <div class="FloatRight">
                <cms:LocalizedButton ID="btnOkFooter" runat="server" ButtonStyle="Primary" OnClick="btnOk_Click"
                    EnableViewState="false" ResourceString="general.add" />
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Panel>
